using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TN.Info
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObjType
    {
        None,
        FireWood,
        Beaf,
        OriginBeaf,
        Noodle,
        Scone,
    }
}