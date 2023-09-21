using System;

namespace TN.Role
{
    /// <summary>
    /// 厨师
    /// </summary>
    [Serializable]
    public class Cook
    {
        /// <summary>
        /// 当前携带的物体Id
        /// </summary>
        public string TakeObjId;
        /// <summary>
        /// 携带物品的数量
        /// </summary>
        public int TakeObjCount;

        /// <summary>
        /// 烹饪的食物Id
        /// </summary>
        public string CookingObjId;
        /// <summary>
        /// 烹饪的食物数量
        /// </summary>
        public int CookingObjCount;
    }
}