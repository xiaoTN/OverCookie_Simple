using System;

namespace TN.Role
{
    /// <summary>
    /// 服务员
    /// </summary>、
    [Serializable]
    public class Waiter
    {
        /// <summary>
        /// 携带物品的Id
        /// </summary>
        public string TakeObjId;
        /// <summary>
        /// 携带物品的数量
        /// </summary>
        public int TakeObjCount;
    }
}