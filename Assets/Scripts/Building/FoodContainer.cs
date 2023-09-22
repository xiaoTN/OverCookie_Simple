using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using TN.Info;
using UnityEngine;

namespace TN.Building
{
    /// <summary>
    /// 食物容器
    /// </summary>
    public class FoodContainer : BaseObj
    {
        /// <summary>
        /// 容器信息
        /// </summary>
        [Serializable]
        public struct ContainerInfo
        {
            public ObjType ObjId;
            public int     Count;
        }

        [TableList]
        public List<ContainerInfo> ContainerInfos;

        protected override string GizmoLabel
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (ContainerInfo containerInfo in ContainerInfos)
                {
                    s.AppendLine($"{containerInfo.ObjId.ToString()}:{containerInfo.Count}");
                }

                return $@"食物箱:
{s.ToString()}";
            }
        }
    }
}