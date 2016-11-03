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
    /// MySql数据库DAL
    /// </summary>
    public class MySqlDal : IDal
    {
        #region 获取所有表信息
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<Dictionary<string, string>> GetAllTables()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ToString();
            int start = connectionString.IndexOf("database=") + 9;
            int end = connectionString.IndexOf("user id=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            MySqlHelper dbHelper = new MySqlHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                SELECT TABLE_NAME as TABLE_NAME,TABLE_COMMENT as COMMENTS 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = '{0}'", owner));
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("table_name", dr["TABLE_NAME"].ToString());
                dic.Add("comments", dr["COMMENTS"].ToString());
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
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ToString();
            int start = connectionString.IndexOf("database=") + 9;
            int end = connectionString.IndexOf("user id=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            MySqlHelper dbHelper = new MySqlHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                select * 
                from INFORMATION_SCHEMA.Columns 
                where table_name='{0}' 
                and table_schema='{1}'", tableName, owner));
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("columns_name", dr["COLUMN_NAME"].ToString());
                dic.Add("notnull", dr["IS_NULLABLE"].ToString() == "NO" ? "1" : "0");
                dic.Add("comments", dr["COLUMN_COMMENT"].ToString());
                string dataType = dr["COLUMN_TYPE"].ToString();
                int pos = dataType.IndexOf("(");
                if (pos != -1) dataType = dataType.Substring(0, pos);
                dic.Add("data_type", dataType);
                dic.Add("data_scale", dr["CHARACTER_MAXIMUM_LENGTH"].ToString());
                dic.Add("data_precision", dr["NUMERIC_SCALE"].ToString());

                if (dr["COLUMN_KEY"].ToString() == "PRI")
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
            string data_type = "string";
            switch (column["data_type"])
            {
                case "int":
                    data_type = "int";
                    break;
                case "bigint":
                    data_type = "long";
                    break;
                case "decimal":
                    data_type = "decimal";
                    break;
                case "varchar":
                    data_type = "string";
                    break;
                case "text":
                    data_type = "string";
                    break;
                case "datetime":
                    data_type = "DateTime";
                    break;
                case "longtext":
                    data_type = "string";
                    break;
            }
            return data_type;
        }
        #endregion

    }
}
