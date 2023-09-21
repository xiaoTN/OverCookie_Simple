using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using TN.Common;

namespace TN.Common
{
    public static class CSVFileUtils
    {
        private static DataTable _heroNameTable;

        /// <summary>
        ///     将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static void SaveCSV(DataTable dt, string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    string data = "";

                    //写出列名称
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        data += dt.Columns[i].ColumnName;
                        if (i < dt.Columns.Count - 1)
                            data += ",";
                    }

                    sw.WriteLine(data);

                    //写出各行数据
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        data = "";
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            string str = dt.Rows[i][j].ToString();
                            str = str.Replace("\"", "\"\""); //替换英文冒号 英文冒号需要换成两个冒号
                            if (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n")) //含逗号 冒号 换行符的需要放到引号中
                                str = string.Format("\"{0}\"", str);

                            data += str;
                            if (j < dt.Columns.Count - 1)
                                data += ",";
                        }

                        sw.WriteLine(data);
                    }
                }
            }
        }


        public static DataTable OpenCSV(string filePath)
        {
            string content = FileUtils.ReadString(filePath);
            DataTable toDataTable = ParseToDataTable(content);
            return toDataTable;
        }

        public static DataTable OpenCSVFromStreamingAssets(string localPath)
        {
            return OpenCSV(Path.Combine(Application.streamingAssetsPath, localPath));
        }

        public static DataTable ParseToDataTable(string content)
        {
            DataTable dt = new DataTable();
            string[] lines = content.Split(new[]
            {
                "\r\n"
            }, StringSplitOptions.None);

            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;

            //标示列数
            int columnCount = 0;

            //标示是否是读取的第一行
            bool IsFirst = true;

            foreach (string line in lines)
            {
                //逐行读取CSV中的数据
                if (string.IsNullOrWhiteSpace(line) == false)
                {
                    //strLine = Common.ConvertStringUTF8(strLine, encoding);
                    //strLine = Common.ConvertStringUTF8(strLine);

                    if (IsFirst)
                    {
                        tableHead = line.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;

                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = line.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                            dr[j] = aryLine[j];

                        dt.Rows.Add(dr);
                    }
                }
            }

#if UNITY_EDITOR
            dt.DebugDataTabel();
#endif
            return dt;
        }

#if UNITY_EDITOR

        [Obsolete("Editor模式下使用")]
        public static string GetChineseName(string name)
        {
            DataTable dataTable = OpenCSVInStreamingAssets("DataTable/HeroAbility.CSV");
            foreach (DataRow row in dataTable.Rows)
            {
                if (row[CSVDefine.ABILITY_CODE].ToString().Equals(name))
                {
                    return row[CSVDefine.ABILITY_NAME].ToString();
                }
            }

            return $"找不到[{name}]中文名";
        }

        [Obsolete("Android版本出错，请使用OpenCSVAsync")]
        public static DataTable OpenCSVInStreamingAssets(string localPath)
        {
            return OpenCSVInEditor(Path.Combine(Application.streamingAssetsPath, localPath));
        }

        /// <summary>
        ///     将CSV文件的数据读取到DataTable中
        ///     只能在Editor模式下使用
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        [Obsolete("Android版本出错，请使用OpenCSVAsync")]
        public static DataTable OpenCSVInEditor(string filePath)
        {
            DataTable dt = new DataTable();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        string strLine = "";

                        //记录每行记录中的各字段内容
                        string[] aryLine = null;
                        string[] tableHead = null;

                        //标示列数
                        int columnCount = 0;

                        //标示是否是读取的第一行
                        bool IsFirst = true;

                        //逐行读取CSV中的数据
                        while ((strLine = sr.ReadLine()) != null)
                        {
                            //strLine = Common.ConvertStringUTF8(strLine, encoding);
                            //strLine = Common.ConvertStringUTF8(strLine);

                            if (IsFirst)
                            {
                                tableHead = strLine.Split(',');
                                IsFirst = false;
                                columnCount = tableHead.Length;

                                //创建列
                                for (int i = 0; i < columnCount; i++)
                                {
                                    DataColumn dc = new DataColumn(tableHead[i]);
                                    dt.Columns.Add(dc);
                                }
                            }
                            else
                            {
                                aryLine = strLine.Split(',');
                                DataRow dr = dt.NewRow();
                                for (int j = 0; j < columnCount; j++)
                                    dr[j] = aryLine[j];

                                dt.Rows.Add(dr);
                            }
                        }

                        // 取消排序，没必要
                        // if (aryLine != null && aryLine.Length > 0)
                        //     dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"读取失败：{e}");
            }

            return dt;
        }

#endif
    }
}