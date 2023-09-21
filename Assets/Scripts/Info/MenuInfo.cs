using System;
using System.Collections.Generic;

namespace TN.Info
{
    /// <summary>
    /// 食物制作表
    /// eg: 用SourceMaterialInfos经过Duration秒，制作出1个TargetId
    /// </summary>
    [Serializable]
    public class MenuInfo
    {
        /// <summary>
        /// 成品食物Id
        /// </summary>
        public string TargetId;
        /// <summary>
        /// 所需的原材料信息
        /// </summary>
        public List<SourceMaterialInfo> SourceMaterialInfos;

        /// <summary>
        /// 加工时间 s
        /// </summary>
        public float Duration;
    }

    /// <summary>
    /// 原材料
    /// </summary>
    [Serializable]
    public class SourceMaterialInfo
    {
        public string Id;
        public int Count;
    }
}