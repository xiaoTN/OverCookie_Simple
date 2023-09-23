using System;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using TN.Building;
using TN.Info;
using UnityEngine;

namespace TN.Role
{
    /// <summary>
    /// 服务员
    /// </summary>
    public class Waiter : BaseObj
    {
        public enum State
        {
            None,
            RunToDinningTable,
            RunToFoodAllot,
        }

        protected override string GizmoLabel
        {
            get
            {
                if (_curTakeOrderInfo == null)
                {
                    return String.Empty;
                }
                return $"携带：{_curTakeOrderInfo.ToString()}";
            }
        }

        private StateMachine<State> _fsm;

        [ShowInInspector]
        [ReadOnly]
        private State _curState;

        public void Init()
        {
            _fsm = StateMachine<State>.Initialize(this, State.None);
            _fsm.Changed += state =>
            {
                _curState = state;
                Log($"状态->{state.ToString()}");
            };
            _foodAllot = GameManager.Instance.FoodAllot;
        }

        public  float     MoveSpeed;
        private FoodAllot _foodAllot;

        [ShowInInspector]
        [ReadOnly]
        private OrderInfo _curTakeOrderInfo=null;

        [ShowInInspector]
        [ReadOnly]
        private DiningTable _targetDinningTable;

        private void TakeObj(OrderInfo orderInfo)
        {
            _curTakeOrderInfo = orderInfo;
        }

        private void ReleaseObj()
        {
            _curTakeOrderInfo = null;
        }

        private void None_Update()
        {
            if (transform.MoveToUpdate(_foodAllot.transform.position, MoveSpeed))
            {
                //到达餐桌旁
                if (_foodAllot.HaveFood())
                {
                    OrderInfo takeOrder = _foodAllot.RemoveFood();
                    TakeObj(takeOrder);
                    Customer findCustomer = GameManager.Instance.Customers.Find(customer => _curTakeOrderInfo.Customer == customer);
                    _targetDinningTable = findCustomer.DiningTable;
                    _fsm.ChangeState(State.RunToDinningTable);
                    return;
                }
            }
        }

        private void RunToDinningTable_Update()
        {
            if (transform.MoveToUpdate(_targetDinningTable.transform.position, MoveSpeed))
            {
                Log($"卸下身上的物品：{_curTakeOrderInfo}");
                _targetDinningTable.AddOrderInfo(_curTakeOrderInfo);
                ReleaseObj();
                _targetDinningTable = null;
                _fsm.ChangeState(State.RunToFoodAllot);
            }
        }

        private void RunToFoodAllot_Update()
        {
            if (transform.MoveToUpdate(_foodAllot.transform.position, MoveSpeed))
            {
                _fsm.ChangeState(State.None);
                return;
            }
        }
    }
}