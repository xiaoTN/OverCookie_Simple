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
        FireWood,

        [LabelText("牛排")]
        Beaf,

        [LabelText("生牛肉")]
        OriginBeaf,

        [LabelText("意大利牛排套餐")]
        Noodle,

        [LabelText("曲奇饼")]
        Scone,
    }
}