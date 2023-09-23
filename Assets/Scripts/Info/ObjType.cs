using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;

namespace TN.Info
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObjType
    {
        None,

        柴火,

        牛排,

        生牛排,

        意大利牛排套餐,
        曲奇,
        意面,
    }
}