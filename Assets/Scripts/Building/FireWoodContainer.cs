using UnityEngine;

namespace TN.Building
{
    /// <summary>
    /// 柴火容器
    /// </summary>
    public class FireWoodContainer : BaseObj
    {
        public int MaxCount;

        protected override string GizmoLabel
        {
            get
            {
                return "柴火箱";
            }
        }
    }
}