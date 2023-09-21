using System;

namespace TN.Info
{
    [Serializable]
    public class ObjInfo
    {
        public string Id;
        public string Name;

        /// <summary>
        /// 每单位的最大数量
        /// </summary>
        public int MaxCountPerGrid;
    }
}