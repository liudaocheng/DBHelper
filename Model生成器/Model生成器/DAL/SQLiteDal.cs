using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtil;

namespace DAL
{
    /// <summary>
    /// SQLite数据库DAL
    /// </summary>
    public class SQLiteDal : IDal
    {
        #region 获取所有表名
        /// <summary>
        /// 获取数据库名
        /// </summary>
        public List<Dictionary<string, string>> GetAllTables()
        {
            SQLiteHelper sqliteHelper = new SQLiteHelper();
            DataTable dt = sqliteHelper.Query("select tbl_name from sqlite_master where type='table'");
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("table_name", dr["tbl_name"].ToString());
                dic.Add("comments", "");
                result.Add(dic);
            }
            return result;
        }
        #endregion

        #region 获取表的所有字段名及字段类型
        /// <summary>
        /// 获取表的所有字段名及字段类型
        /// </summary>
        public List<Dictionary<string, string>> GetAllColumns(string tableName)
        {
            SQLiteHelper sqliteHelper = new SQLiteHelper();
            DataTable dt = sqliteHelper.Query("PRAGMA table_info('" + tableName + "')");
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("columns_name", dr["name"].ToString());
                dic.Add("notnull", dr["notnull"].ToString() == "1" ? "1" : "0");
                dic.Add("comments", "");
                dic.Add("data_type", "string");
                dic.Add("data_scale", "");
                dic.Add("data_precision", "");

                if (dr["pk"].ToString() == "1")
                {
                    dic.Add("constraint_type", "P");
                }
                else
                {
                    dic.Add("constraint_type", "");
                }
                result.Add(dic);
            }
            return result;
        }
        #endregion

        #region 类型转换
        /// <summary>
        /// 类型转换
        /// </summary>
        public string ConvertDataType(Dictionary<string, string> column)
        {
            return "string";
        }
        #endregion

    }
}
