using System;
using TN.Info;
using UnityEngine;

namespace TN.Role
{
    /// <summary>
    /// 服务员
    /// </summary>
    public class Waiter: BaseObj
    {
        /// <summary>
        /// 携带物品的Id
        /// </summary>
        public ObjType TakeObjId;
        /// <summary>
        /// 携带物品的数量
        /// </summary>
        public int TakeObjCount;

        protected override string GizmoLabel { get; }
    }
}