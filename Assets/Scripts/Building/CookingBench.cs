using Sirenix.OdinInspector;
using TN.Info;
using UnityEngine;

namespace TN.Building
{
    /// <summary>
    /// 灶台
    /// </summary>
    public class CookingBench : BaseObj
    {
        [ReadOnly]
        public ObjType CookingFoodId;

        /// <summary>
        /// 剩余柴火数
        /// </summary>
        [ReadOnly]
        public int RemainFireWood;

        [Button]
        public void Cooking()
        {
        }

        [Button]
        public void StopCooking()
        {
        }

        protected override string GizmoLabel
        {
            get
            {
                return $@"灶台";
            }
        }
    }
}