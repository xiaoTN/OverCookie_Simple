using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TN.Info
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObjType
    {
        FireWood,
        Beaf,
        OriginBeaf,
        Noodle,
        Scone,
    }
}