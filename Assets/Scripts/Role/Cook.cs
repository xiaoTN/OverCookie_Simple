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
        /// todo 这种结构不支持序列化
        /// </summary>
        [ReadOnly]
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
                Debug.LogError("拿不下了");
                return false;
            }

            TakeObjIds.Add((objType, count));
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

            gameObject.UpdateAsObservable()
                      .Select(unit => GameManager.Instance.CookingBench.RemainFireWood == 0)
                      .ToReactiveProperty()
                      .Subscribe(isZero =>
                      {
                          if (isZero)
                          {
                              Debug.Log("灶台中柴火耗尽，需要添加柴火");
                              _fsm.ChangeState(CookState.RunToFireWood);
                          }
                      });
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
            if (GameManager.Instance.CookingBench.RemainFireWood == 0)
            {
                _fsm.ChangeState(CookState.RunToFireWood);
                return;
            }
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
                    if (findFoodContainer.CanPickFood(_curMenu.TargetId))
                    {
                        TakeObj(_curMenu.TargetId, 1);
                        findFoodContainer.RemoveFood(_curMenu.TargetId);
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
            bool isArrive = transform.MoveToUpdate(GameManager.Instance.CookingBench.transform.position, MoveSpeed);
            if (isArrive)
            {
                if (_prepearAddFireWood)
                {
                    _prepearAddFireWood = false;
                    Debug.Log("添加燃料");
                    int count = TakeObjIds.Sum(tuple => tuple.Item2);
                    GameManager.Instance.CookingBench.AddFireWood(count);
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

                if (_curMenu == null)
                {
                    _fsm.ChangeState(CookState.None);
                    return;
                }

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

        private CompositeDisposable _cookingDis = null;

        private void Cooking_Enter()
        {
            _cookingDis = new CompositeDisposable();
            _needCook = true;
            CookingObjId = _curMenu.TargetId;
            Observable.Timer(TimeSpan.FromSeconds(_curMenu.Duration))
                      .Subscribe(l =>
                      {
                          _needCook = false;
                          _fsm.ChangeState(CookState.RunToFoodAllot);
                      })
                      .AddTo(this)
                      .AddTo(_cookingDis);

            GameManager.Instance.CookingBench.StartCook();
        }

        private void Cooking_Update()
        {
            if (GameManager.Instance.CookingBench.RemainFireWood == 0)
            {
                _fsm.ChangeState(CookState.RunToFireWood);
            }
        }

        private void Cooking_End()
        {
            _cookingDis?.Dispose();
            GameManager.Instance.CookingBench.StopCook();
        }

        private void RunToFireWood_Enter()
        {
            //进入这个状态时，手里一定是空的
            if (TakeObjIds.Count > 0)
            {
                Debug.LogError("手里有东西，不可能进入这个状态");
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
            GameManager.Instance.CookingBench.StopCook();
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