using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TN.Common
{
    public class VectorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector3 vector3 = (Vector3)value;
            string context = $"{vector3.x:f3},{vector3.y:f3},{vector3.z:f3}";
            serializer.Serialize(writer, context);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string context = reader.Value?.ToString();
            if (context != null)
            {
                string[] strs = context.Split(',');
                return new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]));
            }

            return Vector3.zero;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }
    }
}