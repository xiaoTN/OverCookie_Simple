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
        public class ContainerInfo
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

                return $@"{s.ToString()}";
            }
        }

        public bool CanPickFood(ObjType objType, int count = 1)
        {
            ContainerInfo containerInfo = ContainerInfos.Find(info => info.ObjId == objType);
            if (containerInfo == null)
                return false;
            return true;
        }
        public int RemoveFood(ObjType objType, int count=1)
        {
            ContainerInfo containerInfo = ContainerInfos.Find(info => info.ObjId == objType);
            if (containerInfo.Count < count)
            {
                Debug.Log($"箱子里食物【{objType}】只有{containerInfo.Count}，但需要拿出{count}");
                ContainerInfos.Remove(containerInfo);
                return containerInfo.Count;
            }

            containerInfo.Count -= count;
            return count;
        }
    }
}