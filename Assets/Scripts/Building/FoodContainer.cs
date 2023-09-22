using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TN.Info;
using UnityEngine;

namespace TN.Building
{
    /// <summary>
    /// 食物容器
    /// </summary>
    public class FoodContainer: MonoBehaviour
    {
        /// <summary>
        /// 容器信息
        /// </summary>
        [Serializable]
        public struct ContainerInfo
        {
            public ObjType ObjId;
            public int    Count;
        }
[TableList]
        public List<ContainerInfo> ContainerInfos;
    }
}