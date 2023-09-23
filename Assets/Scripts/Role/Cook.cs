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
using Random = UnityEngine.Random;

namespace TN.Role
{
    /// <summary>
    /// 厨师
    /// </summary>
    public class Cook : BaseObj
    {
        /// <summary>
        /// 当前携带的物体Id
        /// todo 这种结构不支持序列化
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        public List<(ObjType, int)> TakeObjIds = new List<(ObjType, int)>();

        /// <summary>
        /// 最大携带单位
        /// </summary>
        public int MaxTakeCount;

        public float MoveSpeed;

        /// <summary>
        /// 携带物品
        /// </summary>
        /// <param name="objType"></param>
        private bool TakeObj(ObjType objType, int count)
        {
            if (TakeObjIds.Count > MaxTakeCount)
            {
                LogError("拿不下了");
                return false;
            }

            TakeObjIds.Add((objType, count));
            Log($"携带 {objType} [{count}个]");
            return true;
        }

        /// <summary>
        /// 从身上卸下物品
        /// </summary>
        private void ReleaseObj()
        {
            Log($"从身上卸下物品");
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

        /// <summary>
        /// 当前的订单信息
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        private OrderInfo _curOrder;

        private Queue<ObjType> _needTakeObjIds = new Queue<ObjType>();


        [Button]
        public void Init()
        {
            _fsm = StateMachine<CookState>.Initialize(this, CookState.None);
            _fsm.Changed += state =>
            {
                _curState = state;
                Log($"进入状态：{state}");
            };
            _foodAllot = GameManager.Instance.FoodAllot;
            _cookingBench = GameManager.Instance.CookingBench;
            gameObject.UpdateAsObservable()
                      .Select(unit => _cookingBench.RemainFireWood == 0)
                      .ToReactiveProperty()
                      .Subscribe(isZero =>
                      {
                          if (isZero)
                          {
                              Log("灶台中柴火耗尽，需要添加柴火");
                              _fsm.ChangeState(CookState.RunToFireWood);
                          }
                      });
        }
        

        private void None_Update()
        {
            if (_cookingBench.RemainFireWood == 0)
            {
                _fsm.ChangeState(CookState.RunToFireWood);
                return;
            }

            if (GameManager.Instance.HaveOrder)
            {
                _curOrder = GameManager.Instance.RemoveOrder();
                Log($"接收订单：{_curOrder}");
                _needTakeObjIds.Clear();
                if (_curOrder.MenuInfo.SourceMaterialInfos != null)
                {
                    //需要加工
                    foreach (SourceMaterialInfo curMenuSourceMaterialInfo in _curOrder.MenuInfo.SourceMaterialInfos)
                    {
                        for (int i = 0; i < curMenuSourceMaterialInfo.Count; i++)
                        {
                            _needTakeObjIds.Enqueue(curMenuSourceMaterialInfo.Id);
                        }
                    }
                }
                else
                {
                    _needTakeObjIds.Enqueue(_curOrder.MenuInfo.TargetId);
                }

                _fsm.ChangeState(CookState.RunToObjContainer);
                return;
            }

            transform.MoveToUpdate(_cookingBench.transform.position, MoveSpeed);
        }

        private void RunToObjContainer_Update()
        {
            //有订单，走到指定的位置拿东西
            FoodContainer findFoodContainer = GameManager.Instance.FoodContainer;
            bool isArrive = transform.MoveToUpdate(findFoodContainer.transform.position, MoveSpeed);
            if (isArrive)
            {
                if (_curOrder.MenuInfo.SourceMaterialInfos == null)
                {
                    if (findFoodContainer.CanPickFood(_curOrder.MenuInfo.TargetId))
                    {
                        TakeObj(_curOrder.MenuInfo.TargetId, 1);
                        findFoodContainer.RemoveFood(_curOrder.MenuInfo.TargetId);
                        _fsm.ChangeState(CookState.RunToFoodAllot);
                        return;
                    }
                }
                else
                {
                    bool isTakeComplete = true;
                    if (_needTakeObjIds.Count > MaxTakeCount)
                    {
                        // 一次带不满
                        for (int i = 0; i < MaxTakeCount; i++)
                        {
                            ObjType objType = _needTakeObjIds.Dequeue();
                            if (findFoodContainer.CanPickFood(objType))
                            {
                                TakeObj(objType, 1);
                                findFoodContainer.RemoveFood(objType);
                            }
                            else
                            {
                                isTakeComplete = false;
                            }
                        }
                    }
                    else
                    {
                        foreach (ObjType needTakeObjId in _needTakeObjIds)
                        {
                            if (findFoodContainer.CanPickFood(needTakeObjId))
                            {
                                findFoodContainer.RemoveFood(needTakeObjId);
                                TakeObj(needTakeObjId, 1);
                            }
                            else
                            {
                                isTakeComplete = false;
                            }
                        }

                        if (isTakeComplete)
                        {
                            _needTakeObjIds.Clear();
                        }
                    }

                    if (isTakeComplete)
                    {
                        _fsm.ChangeState(CookState.RunToCookingBench);
                        return;
                    }
                }
            }
        }

        private bool _prepearAddFireWood = false;
        private bool _needCook           = false;

        private void RunToCookingBench_Update()
        {
            bool isArrive = transform.MoveToUpdate(_cookingBench.transform.position, MoveSpeed);
            if (isArrive)
            {
                if (_prepearAddFireWood)
                {
                    _prepearAddFireWood = false;
                    Log("添加燃料");
                    int count = TakeObjIds.Sum(tuple => tuple.Item2);
                    _cookingBench.AddFireWood(count);
                    ReleaseObj();
                    if (_needCook)
                    {
                        _fsm.ChangeState(CookState.Cooking);
                    }
                    else
                    {
                        _fsm.ChangeState(CookState.None);
                    }

                    return;
                }

                if (_curOrder == null)
                {
                    _fsm.ChangeState(CookState.None);
                    return;
                }

               

                // 开始做饭
                if (_cookingBench.CanUse())
                {
                    ReleaseObj(); //卸下身上的原料
                    if (_needTakeObjIds.Count > 0)
                    {
                        //继续拿原料
                        _fsm.ChangeState(CookState.RunToObjContainer);
                        return;
                    }
                    _fsm.ChangeState(CookState.Cooking);
                    return;
                }
            }
        }

        private CompositeDisposable _cookingDis = null;
        private CookingBench        _cookingBench;
        private FoodAllot           _foodAllot;

        private void Cooking_Enter()
        {
            _cookingDis = new CompositeDisposable();
            _needCook = true;
            CookingObjId = _curOrder.MenuInfo.TargetId;
            Observable.Timer(TimeSpan.FromSeconds(_curOrder.MenuInfo.Duration))
                      .Subscribe(l =>
                      {
                          _needCook = false;
                          TakeObj(CookingObjId, 1);
                          _fsm.ChangeState(CookState.RunToFoodAllot);
                      })
                      .AddTo(this)
                      .AddTo(_cookingDis);

            _cookingBench.StartCook();
        }

        private void Cooking_Update()
        {
            if (_cookingBench.RemainFireWood == 0)
            {
                _fsm.ChangeState(CookState.RunToFireWood);
            }
        }

        private void Cooking_End()
        {
            _cookingDis?.Dispose();
            _cookingBench.StopCook();
        }

        private void RunToFireWood_Enter()
        {
            //进入这个状态时，手里一定是空的
            if (TakeObjIds.Count > 0)
            {
                LogError("手里有东西，不可能进入这个状态");
            }
        }

        private void RunToFireWood_Update()
        {
            if (transform.MoveToUpdate(GameManager.Instance.FireWoodContainer.transform.position, MoveSpeed))
            {
                for (int i = 0; i < MaxTakeCount; i++)
                {
                    int count = GameManager.Instance.FireWoodContainer.Pick();
                    if (count == 0)
                    {
                        break;
                    }

                    TakeObj(ObjType.FireWood, count);
                }

                _prepearAddFireWood = true;
                _fsm.ChangeState(CookState.RunToCookingBench);
            }
        }

        private void Cooking_Exit()
        {
            CookingObjId = ObjType.None;
            _cookingBench.StopCook();
        }


        private void RunToFoodAllot_Enter()
        {
        }

        private void RunToFoodAllot_Update()
        {
            bool isArrive = transform.MoveToUpdate(_foodAllot.transform.position, MoveSpeed);
            if (isArrive)
            {
                //到达分配桌附近
                if(_foodAllot.CanEnqueueOrder())
                {
                    _foodAllot.EnqueueOrder(_curOrder);
                    _curOrder = null;

                    ReleaseObj();
                    _fsm.ChangeState(CookState.RunToCookingBench);
                }
            }
        }


        protected override string GizmoLabel
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (TakeObjIds != null)
                {
                    foreach ((ObjType, int) item in TakeObjIds)
                    {
                        s.AppendLine($"{item.Item1}:{item.Item2}");
                    }
                }

                return $@"携带的物品：{s.ToString()}
正在烹饪的物品：{CookingObjId.ToString()}";
            }
        }
    }
}