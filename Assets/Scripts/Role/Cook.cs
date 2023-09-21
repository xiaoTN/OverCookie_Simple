using System;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using TN.Building;
using TN.Info;
using UniRx;
using UnityEngine;

namespace TN.Role
{
    /// <summary>
    /// 厨师
    /// </summary>
    public class Cook : MonoBehaviour
    {
        /// <summary>
        /// 当前携带的物体Id
        /// </summary>
        public ObjType TakeObjId;

        /// <summary>
        /// 携带物品的数量
        /// </summary>
        public int TakeObjCount;

        /// <summary>
        /// 烹饪的食物Id
        /// </summary>
        public ObjType CookingObjId;

        /// <summary>
        /// 烹饪的食物数量
        /// </summary>
        public int CookingObjCount;

        // public int MaxTakeCount;
        public enum CookState
        {
            None,
            RunToObjContainer,
            RunToCookingBench,
            RunToFireWood,
            RunToFoodAllot,
            Cooking,
        }

        private StateMachine<CookState> _fsm;
        private MenuInfo                _curMenu;


        [Button]
        public void Init()
        {
            _fsm = StateMachine<CookState>.Initialize(this, CookState.None);
        }

        [Button("收到订单")]
        private void ReceiveOrderForm(ObjType foodId)
        {
            MenuInfo menuInfo = GameManager.Instance.MenuInfos.Find(info => info.TargetId == foodId);
            GameManager.Instance.OrderFormMenuQueue.Enqueue(menuInfo);
        }

        private void None_Update()
        {
            MenuInfo firstMenu = GameManager.Instance.CurFirstMenu;
            if (firstMenu != null)
            {
                _curMenu = GameManager.Instance.OrderFormMenuQueue.Dequeue();
                _fsm.ChangeState(CookState.RunToObjContainer);
                return;
            }

            transform.MoveToUpdate(GameManager.Instance.DiningTable.transform.position);
        }

        private void RunToObjContainer_Update()
        {
            //有订单，走到指定的位置拿东西
            // todo 这里需要考虑是否需要加工
            FoodContainer findFoodContainer = GameManager.Instance.FoodContainer;
            bool isArrive = transform.MoveToUpdate(findFoodContainer.transform.position);
            if (isArrive)
            {
                if (_curMenu.SourceMaterialInfos == null)
                {
                    _fsm.ChangeState(CookState.RunToFoodAllot);
                    return;
                }
                else
                {
                    TakeObjId = _curMenu.TargetId; // todo 暂时只携带一种

                    // TakeObjCount
                    _fsm.ChangeState(CookState.RunToCookingBench);
                    return;
                }
            }
        }

        private void RunToCookingBench_Update()
        {
            bool isArrive = transform.MoveToUpdate(GameManager.Instance.DiningTable.transform.position);
            if (isArrive)
            {
                // todo 需要判断是否加满原料了
                _fsm.ChangeState(CookState.Cooking);
                return;
            }
        }

        private void Cooking_Enter()
        {
            // GameManager.Instance.me
            // Observable.Timer(TimeSpan.FromSeconds())
        }

        private void Cooking_Exit()
        {
        }
    }
}