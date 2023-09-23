using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;

namespace TN.Info
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObjType
    {
        None,

        [LabelText("柴火")]
        柴火,

        [LabelText("牛排")]
        牛排,

        [LabelText("生牛肉")]
        生牛排,

        [LabelText("意大利牛排套餐")]
        意大利牛排套餐,

        [LabelText("曲奇饼")]
        曲奇,
    }
}