using System;
using Sirenix.OdinInspector;
using TN.Info;
using UnityEngine;
using UnityEngine.Serialization;

namespace TN.Role
{
    /// <summary>
    /// 顾客
    /// </summary>
    public class Customer: MonoBehaviour
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
    }
}