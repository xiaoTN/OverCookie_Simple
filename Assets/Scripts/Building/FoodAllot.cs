using System;
using UnityEngine;
using VirtualStage.Core;

namespace TN.Building
{
    /// <summary>
    /// 食物分配桌
    /// </summary>
    public class FoodAllot: BaseObj
    {
        protected override string GizmoLabel
        {
            get
            {
                return $@"食物分配桌";
            }
        }
    }
}