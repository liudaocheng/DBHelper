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
    /// Oracle数据库DAL
    /// </summary>
    public class OracleDal : IDal
    {
        #region 获取所有表信息
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<Dictionary<string, string>> GetAllTables()
        {
            OracleHelper dbHelper = new OracleHelper();
            DataTable dt = dbHelper.Query(@"
                select a.TABLE_NAME,b.COMMENTS 
                from user_tables a,user_tab_comments b 
                WHERE a.TABLE_NAME=b.TABLE_NAME");
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
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
            int start = connectionString.IndexOf("User Id=") + 8;
            int end = connectionString.IndexOf("Password=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            OracleHelper dbHelper = new OracleHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                select a.*,b.COMMENTS
                from user_tab_columns a, user_col_comments b
                where a.TABLE_NAME=b.TABLE_NAME and a.COLUMN_NAME=b.COLUMN_NAME 
                and a.TABLE_NAME='{0}'
                order by column_id", tableName));
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("columns_name", dr["COLUMN_NAME"].ToString());
                dic.Add("notnull", dr["NULLABLE"].ToString() == "N" ? "1" : "0");
                dic.Add("comments", dr["COMMENTS"].ToString());
                dic.Add("data_type", dr["DATA_TYPE"].ToString());
                dic.Add("data_scale", dr["DATA_SCALE"].ToString());
                dic.Add("data_precision", dr["DATA_PRECISION"].ToString());

                DataTable dt2 = dbHelper.Query(string.Format(@"
                    select *
                    from user_cons_columns c,user_constraints d
                    where c.owner='{2}' and c.constraint_name=d.constraint_name
                    and c.TABLE_NAME='{0}' and c.COLUMN_NAME='{1}'", tableName, dr["COLUMN_NAME"].ToString(), owner));
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        if (dr2["CONSTRAINT_TYPE"].ToString() == "P")
                        {
                            dic.Add("constraint_type", dr2["CONSTRAINT_TYPE"].ToString());
                        }
                    }
                }
                if (!dic.ContainsKey("constraint_type"))
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
                case "NUMBER":
                    if (column["data_scale"].Trim() == "0")
                    {
                        if (column["data_precision"].Trim() != "" && int.Parse(column["data_precision"].Trim()) > 9)
                        {
                            if (column["notnull"] == "1")
                            {
                                data_type = "long";
                            }
                            else
                            {
                                data_type = "long?";
                            }
                        }
                        else
                        {
                            if (column["notnull"] == "1")
                            {
                                data_type = "int";
                            }
                            else
                            {
                                data_type = "int?";
                            }
                        }
                    }
                    else
                    {
                        if (column["notnull"] == "1")
                        {
                            data_type = "decimal";
                        }
                        else
                        {
                            data_type = "decimal?";
                        }
                    }
                    break;
                case "VARCHAR2":
                    data_type = "string";
                    break;
                case "CHAR":
                    data_type = "string";
                    break;
                case "DATE":
                    if (column["notnull"] == "1")
                    {
                        data_type = "DateTime";
                    }
                    else
                    {
                        data_type = "DateTime?";
                    }
                    break;
                case "CLOB":
                    data_type = "string";
                    break;
            }
            return data_type;
        }
        #endregion

    }
}
