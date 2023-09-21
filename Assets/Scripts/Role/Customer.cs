using System;
using Sirenix.OdinInspector;

namespace TN.Role
{
    /// <summary>
    /// 顾客
    /// </summary>
    [Serializable]
    public class Customer
    {
        /// <summary>
        /// 正在吃的食物Id
        /// </summary>
        public string EatingFoodId;
        /// <summary>
        /// 饥饿时间
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [NonSerialized]
        public float HungerTime;
    }
}