using TN.Info;
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
                return $@"数量：{MaxCount}";
            }
        }

        public int Pick()
        {
            ObjInfo fireWood = GameManager.Instance.ObjInfos.Find(info => info.Id== ObjType.柴火);
            if (MaxCount > fireWood.MaxCountPerGrid)
            {
                MaxCount -= fireWood.MaxCountPerGrid;
                return fireWood.MaxCountPerGrid;
            }

            int copyCount = MaxCount;
            MaxCount = 0;
            return copyCount;
        }
    }
}