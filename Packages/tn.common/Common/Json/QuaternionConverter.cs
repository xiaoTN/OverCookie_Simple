using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TN.Common
{
    public class QuaternionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Quaternion quaternion = (Quaternion)value;
            string context = $"{quaternion.x:f3},{quaternion.y:f3},{quaternion.z:f3},{quaternion.w:f3}";
            serializer.Serialize(writer, context);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string context = reader.Value?.ToString();
            if (context != null)
            {
                string[] strs = context.Split(',');
                return new Quaternion(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]), float.Parse(strs[3]));
            }

            return Quaternion.identity;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Quaternion);
        }
    }
}