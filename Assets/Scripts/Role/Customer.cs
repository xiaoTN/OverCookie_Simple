using System;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using TN.Building;
using TN.Info;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace TN.Role
{
    /// <summary>
    /// 顾客
    /// </summary>
    public class Customer : BaseObj
    {
        public enum State
        {
            None,
            Eating,
            Death
        }

        /// <summary>
        /// 饥饿时间
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [NonSerialized]
        public float HungerTime;

        protected override string GizmoLabel
        {
            get
            {
                return @$"饥饿时间：{HungerTime:F1}
随机下单倒计时：{_randomTimer:F1}
想吃：{WantOrderInfo?.MenuInfo.TargetId.ToString()}
正在吃：{_fsm?.State== State.Eating}
进食倒计时：{_eatTimer:F1}";
            }
        }

        [NonSerialized]
        public DiningTable DiningTable;


        private StateMachine<State> _fsm;
        private State               _curState;

        public void Init(DiningTable diningTable)
        {
            DiningTable = diningTable;
            _fsm = StateMachine<State>.Initialize(this, State.None);
            _fsm.Changed += state =>
            {
                _curState = state;
                Log($"状态->{state.ToString()}");
            };
        }

        /// <summary>
        /// 下发订单
        /// </summary>
        /// <param name="orderInfo"></param>
        [Button]
        public void SendOrderInfo(ObjType objType)
        {
            Log($"下发订单：{objType.ToString()}");
            WantOrderInfo = GameManager.Instance.AddOrder(this, objType);
        }

        [Button]
        public void SendOrderInfoRandom()
        {
            int count = GameManager.Instance.MenuInfos.Count;
            MenuInfo randomMenu = GameManager.Instance.MenuInfos[Random.Range(0, count)];
            SendOrderInfo(randomMenu.TargetId);
        }

        private void None_Enter()
        {
            UpdateRandomSendOrderTimer();
        }

        private float _randomTimer;

        public float MaxHungryTime = 30;

        [NonSerialized]
        public OrderInfo WantOrderInfo;

        private void None_Update()
        {
            HungerTime += Time.deltaTime;
            if (HungerTime > MaxHungryTime)
            {
                HungerTime = 30;
                _fsm.ChangeState(State.Death);
                return;
            }

            if(WantOrderInfo==null)//如果有订单，就不重复下单
            {
                _randomTimer -= Time.deltaTime;
                if (_randomTimer < 0)
                {
                    _randomTimer = 0;
                    SendOrderInfoRandom();
                    UpdateRandomSendOrderTimer();
                }
            }
        }

        private void None_Exit()
        {
            _randomTimer = 0;
        }

        /// <summary>
        /// 更新随机下发订单的等待时间
        /// </summary>
        private void UpdateRandomSendOrderTimer()
        {
            _randomTimer = Random.Range(5, 10f);
        }

        public void Eat()
        {
            _fsm.ChangeState(State.Eating);
        }

        private float _eatTimer;

        private void Eating_Enter()
        {
            HungerTime = 0;
            _eatTimer = Random.Range(5, 10f);
        }

        private void Eating_Update()
        {
            _eatTimer -= Time.deltaTime;
            if (_eatTimer < 0)
            {
                _eatTimer = 0;
                _fsm.ChangeState(State.None);
                return;
            }
        }

        private void Eating_Exit()
        {
            Log($"吃完了：{WantOrderInfo.ToString()}");
            WantOrderInfo = null;
        }

        private void Death_Enter()
        {
            Log("死亡");
        }
    }
}