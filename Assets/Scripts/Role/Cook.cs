using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using TN.Building;
using TN.Info;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TN.Role
{
    /// <summary>
    /// 厨师
    /// </summary>
    public class Cook : BaseObj
    {
        /// <summary>
        /// 当前携带的物体Id
        /// </summary>
        [ReadOnly]
        public List<ObjType> TakeObjIds;

        /// <summary>
        /// 最大携带单位
        /// </summary>
        public int MaxTakeCount;

        public float MoveSpeed;

        /// <summary>
        /// 携带物品
        /// </summary>
        /// <param name="objType"></param>
        private bool TakeObj(ObjType objType)
        {
            if (TakeObjIds.Count > MaxTakeCount)
            {
                Debug.LogError("拿不下了");
                return false;
            }

            TakeObjIds.Add(objType);
            return true;
        }

        /// <summary>
        /// 从身上卸下物品
        /// </summary>
        private void ReleaseObj()
        {
            TakeObjIds.Clear();
        }

        /// <summary>
        /// 烹饪的食物Id
        /// </summary>
        [ReadOnly]
        public ObjType CookingObjId;

        /// <summary>
        /// 烹饪的食物数量
        /// </summary>
        [ReadOnly]
        public int CookingObjCount;

        // public int MaxTakeCount;
        public enum CookState
        {
            None,

            /// <summary>
            /// 走向物品箱
            /// </summary>
            RunToObjContainer,

            /// <summary>
            /// 走向灶台
            /// </summary>
            RunToCookingBench,

            /// <summary>
            /// 走向柴火
            /// </summary>
            RunToFireWood,

            /// <summary>
            /// 走向食物分配桌
            /// </summary>
            RunToFoodAllot,
            Cooking,
        }

        private StateMachine<CookState> _fsm;

        [ReadOnly]
        [ShowInInspector]
        private CookState _curState;

        private MenuInfo       _curMenu;
        private Queue<ObjType> _needTakeObjIds = new Queue<ObjType>();


        [Button]
        public void Init()
        {
            _fsm = StateMachine<CookState>.Initialize(this, CookState.None);
            _fsm.Changed += state =>
            {
                _curState = state;
                Debug.Log($"进入状态：{state}");
            };
        }

        [Button("收到订单")]
        private void ReceiveOrderForm(ObjType foodId)
        {
            Debug.Log($"收到订单：{foodId}");
            MenuInfo menuInfo = GameManager.Instance.MenuInfos.Find(info => info.TargetId == foodId);
            GameManager.Instance.OrderFormMenuQueue.Enqueue(menuInfo);
        }

        private void None_Update()
        {
            MenuInfo firstMenu = GameManager.Instance.CurFirstMenu;
            if (firstMenu != null)
            {
                _curMenu = GameManager.Instance.OrderFormMenuQueue.Dequeue();
                _needTakeObjIds.Clear();
                if (_curMenu.SourceMaterialInfos != null)
                {
                    //需要加工
                    foreach (SourceMaterialInfo curMenuSourceMaterialInfo in _curMenu.SourceMaterialInfos)
                    {
                        for (int i = 0; i < curMenuSourceMaterialInfo.Count; i++)
                        {
                            _needTakeObjIds.Enqueue(curMenuSourceMaterialInfo.Id);
                        }
                    }
                }
                else
                {
                    _needTakeObjIds.Enqueue(_curMenu.TargetId);
                }

                _fsm.ChangeState(CookState.RunToObjContainer);
                return;
            }

            transform.MoveToUpdate(GameManager.Instance.CookingBench.transform.position, MoveSpeed);
        }

        private void RunToObjContainer_Update()
        {
            //有订单，走到指定的位置拿东西
            FoodContainer findFoodContainer = GameManager.Instance.FoodContainer;
            bool isArrive = transform.MoveToUpdate(findFoodContainer.transform.position, MoveSpeed);
            if (isArrive)
            {
                if (_curMenu.SourceMaterialInfos == null)
                {
                    _fsm.ChangeState(CookState.RunToFoodAllot);
                    return;
                }
                else
                {
                    // todo 这里没有判断一次没有带满的情况
                    if (_needTakeObjIds.Count > MaxTakeCount)
                    {
                        // 一次带不满
                        for (int i = 0; i < MaxTakeCount; i++)
                        {
                            TakeObj(_needTakeObjIds.Dequeue());
                        }
                    }
                    else
                    {
                        foreach (ObjType needTakeObjId in _needTakeObjIds)
                        {
                            TakeObj(needTakeObjId);
                        }

                        _needTakeObjIds.Clear();
                    }

                    _fsm.ChangeState(CookState.RunToCookingBench);
                    return;
                }
            }
        }

        private void RunToCookingBench_Update()
        {
            bool isArrive = transform.MoveToUpdate(GameManager.Instance.CookingBench.transform.position, MoveSpeed);
            if (isArrive)
            {
                if (_curMenu == null)
                {
                    _fsm.ChangeState(CookState.None);
                    return;
                }

                // todo 需要判断是否加满原料了
                ReleaseObj(); //卸下身上的原料
                if (_needTakeObjIds.Count > 0)
                {
                    //继续拿原料
                    _fsm.ChangeState(CookState.RunToObjContainer);
                    return;
                }

                // 开始做饭
                _fsm.ChangeState(CookState.Cooking);
                return;
            }
        }

        private void Cooking_Enter()
        {
            CookingObjId = _curMenu.TargetId;
            Observable.Timer(TimeSpan.FromSeconds(_curMenu.Duration))
                      .Subscribe(l =>
                      {
                          _fsm.ChangeState(CookState.RunToFoodAllot);
                      })
                      .AddTo(this);
        }

        private void Cooking_Exit()
        {
            CookingObjId = ObjType.None;
        }


        private void RunToFoodAllot_Enter()
        {
            _curMenu = null;
        }

        private void RunToFoodAllot_Update()
        {
            bool isArrive = transform.MoveToUpdate(GameManager.Instance.FoodAllot.transform.position, MoveSpeed);
            if (isArrive)
            {
                //到达分配桌附近
                ReleaseObj();
                _fsm.ChangeState(CookState.RunToCookingBench);
            }
        }

        protected override string GizmoLabel
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (ObjType takeObjId in TakeObjIds)
                {
                    s.AppendLine(takeObjId.ToString());
                }

                return $@"厨师：
携带的物品：{s.ToString()}
正在烹饪的物品：{CookingObjId.ToString()}";
            }
        }
    }
}