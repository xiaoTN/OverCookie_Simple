using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;

namespace TN.Common
{
    public static class DataTableUtils
    {
        public static void DebugDataTabel(this DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            string head = "";
            //拼接列头
            for (int cNum = 0; cNum < dt.Columns.Count; cNum++)
            {
                head += dt.Columns[cNum].ColumnName + "\t";
            }

            sb.AppendLine(head);
            //csv文件写入列头
            string data = "";
            //csv写入数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string data2 = string.Empty;
                //拼接行数据
                for (int cNum1 = 0; cNum1 < dt.Columns.Count; cNum1++)
                {
                    data2 = data2 + dt.Rows[i][dt.Columns[cNum1].ColumnName].ToString() + "\t";
                }

                bool flag = data != data2;
                if (flag)
                {
                    sb.AppendLine(data2);
                }

                data = data2;
            }

            // Debug.Log(sb.ToString());
        }
        
        public static IEnumerable<DataRow> ToDataRowIEnumerable(this DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                yield return row;
            }
        }
    }
}