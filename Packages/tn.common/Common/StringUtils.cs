using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.Utilities;
using UnityEngine;

namespace TN.Common
{
    public static class StringUtils
    {
        public static string LocalPath
        {
            get
            {
                string dataPath = Application.dataPath;
                string projectPath = dataPath.Substring(0, dataPath.Length - "/Assets".Length);
                return projectPath;
            }
        }

        /// <summary>
        /// 将秒转化为分秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetTimeMS(float seconds)
        {
            return GetTimeMS((int)seconds);
        }

        /// <summary>
        /// 将秒转化为分秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetTimeMS(int seconds)
        {
            seconds = Mathf.Abs(seconds);
            int m = seconds / 60;
            int s = seconds % 60;
            string time = $"{m:00}:{s:00}";
            return time;
        }

        public static string GetTimeHMS(int seconds)
        {
            seconds = Mathf.Abs(seconds);
            int m = seconds / 60;
            int h = m / 60;
            m %= 60;
            int s = seconds % 60;
            string time = $"{h:00}:{m:00}:{s:00}";
            return time;
        }
        
        /// <summary>  
        /// （秒级） 时间戳Timestamp转换成日期 
        /// </summary>  
        /// <param name="timeStamp"></param>  
        /// <returns></returns>  
        public static DateTime GetDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp) * 10000000;
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }
        
        # region 为字符串添加颜色
        public static string ToRed(this string text) => $"<color=red>{text}</color>";
        public static string ToYellow(this string text) => $"<color=yellow>{text}</color>";
        public static string ToGreen(this string text) => $"<color=green>{text}</color>";
        public static string ToGray(this string text) => $"<color=gray>{text}</color>";
        public static string ToOrange(this string text) => $"<color=orange>{text}</color>";
        public static string ToColor(this string text, Color color) => $"<color={ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        public static string ToRed(this int text) => $"<color=red>{text}</color>";
        public static string ToGreen(this int text) => $"<color=green>{text}</color>";
        public static string ToGray(this int text) => $"<color=gray>{text}</color>";
        public static string ToOrange(this int text) => $"<color=orange>{text}</color>";
        public static string ToColor(this int text, Color color) => $"<color={ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        # endregion


        public static string GetHeroItemName(string itemSpriteName)
        {
            Match match = Regex.Match(itemSpriteName, @"(.*?)_png");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return String.Empty;
        }
        
        public static string GetLocalIp()
        {
            //获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        public static Vector3 GetVector3FromString(string str)
        {
            Match match = Regex.Match(str, @"(^.+)\|(.+)\|(.+$)");
            string x = match.Groups[1].Value;
            string y = match.Groups[2].Value;
            string z = match.Groups[3].Value;
            return new Vector3(float.Parse(x),float.Parse(y),float.Parse(z));
        }

        public static string GetString(this Vector3 v3)
        {
            return $"{v3.x:f3}|{v3.y:f3}|{v3.z:f3}";
        }

        public static string GetString(this Hashtable hashtable)
        {
            StringBuilder sb=new StringBuilder();
            foreach (DictionaryEntry dic in hashtable)
            {
                sb.AppendLine($"{dic.Key}:{dic.Value}");
            }

            return sb.ToString();
        }

        public static string GetString<T1, T2>(this Dictionary<T1, T2> dict)
        {
            StringBuilder sb=new StringBuilder();
            foreach (KeyValuePair<T1,T2> item in dict)
            {
                sb.AppendLine($"{item.Key}:{item.Value}");
            }

            return sb.ToString();
        }
        
        public static void ParsePath(ref string path)
        {
            if (path.Contains("\\"))
            {
                Debug.LogWarning($"资源路径只能存在\"/\"这种斜杠:{path}");
                path = path.Replace("\\", "/");
            }
        }
        
        public static int GetByteLength(string str, Encoding encoding){
            int length = encoding.GetBytes(str).Length;
            // Debug.Log($"字节长度为：{length}B = {length/1024f:f1}KB");
            return length;
        }

        public static string GetEnglishNameByGsi(string gsiName)
        {
            if (gsiName.IsNullOrWhitespace())
            {
                Debug.LogError("无法获取英雄名，英雄名为null");
                return String.Empty;
            }
            return gsiName.Substring(14);
        }
        
        public static bool IsNullOrWhitespace(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int index = 0; index < str.Length; ++index)
                {
                    if (!char.IsWhiteSpace(str[index]))
                        return false;
                }
            }
            return true;
        }
        
        public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;
    }
}