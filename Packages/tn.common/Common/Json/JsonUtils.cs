using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UniRx;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

namespace TN.Common
{
    public static class JsonUtils
    {
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new List<JsonConverter>()
            {
                new VectorConverter(),
                new QuaternionConverter()
            }
        };

        public static void WriteJson<T>(T obj, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            path = Path.Combine(Application.streamingAssetsPath, path);
            if (File.Exists(path) == false)
            {
                string directoryName = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directoryName) == false)
                {
                    if (Directory.Exists(directoryName) == false)
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                }
            }

            string json = GetJson(obj, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static string GetJson<T>(T obj, Formatting formatting = Formatting.None)
        {
            string json = JsonConvert.SerializeObject(obj, formatting, _jsonSerializerSettings);
            return json;
        }

        public static T GetObj<T>(string json)
        {
            return (T)GetObj(typeof(T), json);
        }

        public static object GetObj(Type type, string json)
        {
            object t;
            try
            {
                t = JsonConvert.DeserializeObject(json,type, _jsonSerializerSettings);
            }
            catch (Exception e)
            {
                Debug.LogError($"反序列化失败，返回默认值：{e}");
                return default(Type);
            }
            return t;
        }

        public static T ReadJson<T>(string path)
        {
            string json = FileUtils.ReadString(path);
            return GetObj<T>(json);
        }

        public static object ReadJson(Type type, string path)
        {
            string json = FileUtils.ReadString(path);
            return GetObj(type, json);
        }

        public static T ReadJsonFromStreamingAssets<T>(string localPath)
        {
            return ReadJson<T>(Path.Combine(Application.streamingAssetsPath, localPath));
        }

        public static IObservable<object> ReadJsonAsync(Type type, string path)
        {
            return FileUtils.ReadFileAsObservable(path).Select(json =>
            {
                if (json.IsNullOrWhitespace())
                    return default;
                return GetObj(type,json);
            });
        }

        public static IObservable<T> ReadJsonAsync<T>(string path)
        {
            return (IObservable<T>)ReadJsonAsync(typeof(T), path);
        }
    }
}