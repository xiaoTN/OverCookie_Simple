using System;
using Sirenix.OdinInspector;
using TN.Building;
using TN.Info;
using UnityEngine;
using UnityEngine.Serialization;

namespace TN.Role
{
    /// <summary>
    /// 顾客
    /// </summary>
    public class Customer: BaseObj
    {
        /// <summary>
        /// 正在吃的食物Id
        /// </summary>
        public ObjType EatingFoodId;
        /// <summary>
        /// 饥饿时间
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [NonSerialized]
        public float HungerTime;

        protected override string GizmoLabel { get; }
        [NonSerialized]
        public DiningTable DiningTable;

        public ObjType  WantEatFoodType;
        public void Init(DiningTable diningTable)
        {
            DiningTable = diningTable;
        }

        /// <summary>
        /// 下发订单
        /// </summary>
        /// <param name="orderInfo"></param>
        [Button]
        public void SendOrderInfo(ObjType objType)
        {
            Log($"下发订单：{objType.ToString()}");
            WantEatFoodType = objType;
            GameManager.Instance.AddOrder(this,objType);
        }
    }
}