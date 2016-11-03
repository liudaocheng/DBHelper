using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Objects.DataClasses;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using Models;
using MySql.Data.MySqlClient;

namespace DBUtil
{
    /// <summary>
    /// 数据库操作类
    /// 2016年09月09日
    /// </summary>
    public static class DBHelper
    {
        #region 变量
        /// <summary>
        /// 数据库类型
        /// </summary>
        private static string m_DBType = ConfigurationManager.AppSettings["DBType"];
        /// <summary>
        /// 数据库类型
        /// </summary>
        private static bool m_AutoIncrement = ConfigurationManager.AppSettings["AutoIncrement"].ToLower() == "true" ? true : false;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private static string m_ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        /// <summary>
        /// 事务
        /// </summary>
        [ThreadStatic]
        private static DbTransaction m_Tran;
        /// <summary>
        /// 带参数的SQL插入和修改语句中，参数前面的符号
        /// </summary>
        private static string m_ParameterMark = GetParameterMark();
        #endregion

        #region 生成变量
        #region 生成 IDbCommand
        /// <summary>
        /// 生成 IDbCommand
        /// </summary>
        private static DbCommand GetCommand()
        {
            DbCommand command = null;

            switch (m_DBType)
            {
                case "oracle":
                    command = new OracleCommand();
                    break;
                case "mssql":
                    command = new SqlCommand();
                    break;
                case "mysql":
                    command = new MySqlCommand();
                    break;
                case "sqlite":
                    command = new SQLiteCommand();
                    break;
            }

            return command;
        }
        /// <summary>
        /// 生成 IDbCommand
        /// </summary>
        private static DbCommand GetCommand(string sql, DbConnection conn)
        {
            DbCommand command = null;

            switch (m_DBType)
            {
                case "oracle":
                    command = new OracleCommand(sql);
                    command.Connection = conn;
                    break;
                case "mssql":
                    command = new SqlCommand(sql);
                    command.Connection = conn;
                    break;
                case "mysql":
                    command = new MySqlCommand(sql);
                    command.Connection = conn;
                    break;
                case "sqlite":
                    command = new SQLiteCommand(sql);
                    command.Connection = conn;
                    break;
            }

            return command;
        }
        #endregion

        #region 生成 IDbConnection
        /// <summary>
        /// 生成 IDbConnection
        /// </summary>
        private static DbConnection GetConnection()
        {
            DbConnection conn = null;

            switch (m_DBType)
            {
                case "oracle":
                    conn = new OracleConnection(m_ConnectionString);
                    break;
                case "mssql":
                    conn = new SqlConnection(m_ConnectionString);
                    break;
                case "mysql":
                    conn = new MySqlConnection(m_ConnectionString);
                    break;
                case "sqlite":
                    conn = new SQLiteConnection(m_ConnectionString);
                    break;
            }

            return conn;
        }
        #endregion

        #region 生成 IDbDataAdapter
        /// <summary>
        /// 生成 IDbDataAdapter
        /// </summary>
        private static DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            DbDataAdapter dataAdapter = null;

            switch (m_DBType)
            {
                case "oracle":
                    dataAdapter = new OracleDataAdapter();
                    dataAdapter.SelectCommand = cmd;
                    break;
                case "mssql":
                    dataAdapter = new SqlDataAdapter();
                    dataAdapter.SelectCommand = cmd;
                    break;
                case "mysql":
                    dataAdapter = new MySqlDataAdapter();
                    dataAdapter.SelectCommand = cmd;
                    break;
                case "sqlite":
                    dataAdapter = new SQLiteDataAdapter();
                    dataAdapter.SelectCommand = cmd;
                    break;
            }

            return dataAdapter;
        }
        #endregion

        #region 生成 m_ParameterMark
        /// <summary>
        /// 生成 m_ParameterMark
        /// </summary>
        private static string GetParameterMark()
        {
            switch (m_DBType)
            {
                case "oracle":
                    return ":";
                case "mssql":
                    return "@";
                case "mysql":
                    return "@";
                case "sqlite":
                    return ":";
            }
            return ":";
        }
        #endregion
        #endregion

        #region 基础方法
        #region  执行简单SQL语句
        #region Exists
        public static bool Exists(string sqlString)
        {
            using (DbConnection conn = GetConnection())
            {
                using (DbCommand cmd = GetCommand(sqlString, conn))
                {
                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
        }
        #endregion

        #region 执行SQL语句，返回影响的记录数
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string sqlString)
        {
            DbConnection conn = m_Tran == null ? GetConnection() : m_Tran.Connection;
            using (DbCommand cmd = GetCommand(sqlString, conn))
            {
                try
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    if (m_Tran != null) cmd.Transaction = m_Tran;
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                    if (m_Tran == null) conn.Close();
                }
            }
        }
        #endregion

        #region 执行一条计算查询结果语句，返回查询结果
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）
        /// </summary>
        /// <param name="sqlString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string sqlString)
        {
            using (DbConnection conn = GetConnection())
            {
                using (DbCommand cmd = GetCommand(sqlString, conn))
                {
                    try
                    {
                        if (conn.State != ConnectionState.Open) conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        cmd.Dispose();
                    }
                }
            }
        }
        #endregion

        #region 执行查询语句，返回IDataReader
        /// <summary>
        /// 执行查询语句，返回IDataReader ( 注意：调用该方法后，一定要对IDataReader进行Close )
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>IDataReader</returns>
        public static DbDataReader ExecuteReader(string sqlString)
        {
            DbConnection conn = GetConnection();
            DbCommand cmd = GetCommand(sqlString, conn);
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                DbDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 执行查询语句，返回DataSet
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string sqlString)
        {
            using (DbConnection conn = GetConnection())
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    using (DbCommand cmd = GetCommand(sqlString, conn))
                    {
                        DbDataAdapter adapter = GetDataAdapter(cmd);
                        adapter.Fill(ds, "ds");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
                return ds;
            }
        }
        #endregion
        #endregion

        #region 执行带参数的SQL语句
        #region 执行SQL语句，返回影响的记录数
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params DbParameter[] cmdParms)
        {
            DbConnection conn = m_Tran == null ? GetConnection() : m_Tran.Connection;
            using (DbCommand cmd = GetCommand())
            {
                try
                {
                    PrepareCommand(cmd, conn, m_Tran, SQLString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    if (m_Tran == null) conn.Close();
                }
            }
        }
        #endregion

        #region 执行查询语句，返回IDataReader
        /// <summary>
        /// 执行查询语句，返回IDataReader ( 注意：调用该方法后，一定要对IDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>IDataReader</returns>
        public static DbDataReader ExecuteReader(string sqlString, params DbParameter[] cmdParms)
        {
            DbConnection conn = GetConnection();
            DbCommand cmd = GetCommand();
            try
            {
                PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                DbDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 执行查询语句，返回DataSet
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string sqlString, params DbParameter[] cmdParms)
        {
            DbConnection conn = GetConnection();
            DbCommand cmd = GetCommand();
            PrepareCommand(cmd, conn, null, sqlString, cmdParms);
            using (DbDataAdapter da = GetDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
                return ds;
            }
        }
        #endregion

        #region PrepareCommand
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null) cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (DbParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
        #endregion
        #endregion
        #endregion

        #region 增删改查
        #region 获取最大编号
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <typeparam name="T">实体Model</typeparam>
        /// <param name="key">主键</param>
        public static int GetMaxID<T>(string key)
        {
            Type type = typeof(T);

            string sql = null;
            switch (m_DBType)
            {
                case "oracle":
                    sql = string.Format("SELECT Max({0}) FROM {1}", key, type.Name);
                    break;
                case "mssql":
                    sql = string.Format("SELECT Max({0}) FROM {1}", key, type.Name);
                    break;
                case "mysql":
                    sql = string.Format("SELECT Max({0}) FROM {1}", key, type.Name);
                    break;
                case "sqlite":
                    sql = string.Format("SELECT Max(cast({0} as int)) FROM {1}", key, type.Name);
                    break;
            }

            using (DbConnection conn = GetConnection())
            {
                using (IDbCommand cmd = GetCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return 1;
                        }
                        else
                        {
                            return int.Parse(obj.ToString()) + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        public static void Insert(object obj)
        {
            Insert(obj, m_AutoIncrement);
        }
        /// <summary>
        /// 添加
        /// </summary>
        public static void Insert(object obj, bool autoIncrement)
        {
            StringBuilder strSql = new StringBuilder();
            Type type = obj.GetType();
            strSql.Append(string.Format("insert into {0}(", type.Name));

            PropertyInfo[] propertyInfoList = GetEntityProperties(type);
            List<string> propertyNameList = new List<string>();
            int savedCount = 0;
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                if (propertyInfo.GetCustomAttributes(typeof(IsIdAttribute), false).Length > 0 && autoIncrement) return;
                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    propertyNameList.Add(propertyInfo.Name);
                    savedCount++;
                }
            }

            strSql.Append(string.Format("{0})", string.Join(",", propertyNameList.ToArray())));
            strSql.Append(string.Format(" values ({0})", string.Join(",", propertyNameList.ConvertAll<string>(a => m_ParameterMark + a).ToArray())));
            SQLiteParameter[] parameters = new SQLiteParameter[savedCount];
            int k = 0;
            for (int i = 0; i < propertyInfoList.Length && savedCount > 0; i++)
            {
                PropertyInfo propertyInfo = propertyInfoList[i];
                if (propertyInfo.GetCustomAttributes(typeof(IsIdAttribute), false).Length > 0 && autoIncrement) return;
                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    object val = propertyInfo.GetValue(obj, null);
                    SQLiteParameter param = new SQLiteParameter(m_ParameterMark + propertyInfo.Name, val == null ? DBNull.Value : val);
                    parameters[k++] = param;
                }
            }

            ExecuteSql(strSql.ToString(), parameters);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        public static void Update(object obj)
        {
            object oldObj = Find(obj, false);
            if (oldObj == null) throw new Exception("无法获取到旧数据");

            StringBuilder strSql = new StringBuilder();
            Type type = obj.GetType();
            strSql.Append(string.Format("update {0} ", type.Name));

            PropertyInfo[] propertyInfoList = GetEntityProperties(type);
            List<string> propertyNameList = new List<string>();
            int savedCount = 0;
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    object oldVal = propertyInfo.GetValue(oldObj, null);
                    object val = propertyInfo.GetValue(obj, null);
                    if (!object.Equals(oldVal, val))
                    {
                        propertyNameList.Add(propertyInfo.Name);
                        savedCount++;
                    }
                }
            }

            strSql.Append(string.Format(" set "));
            SQLiteParameter[] parameters = new SQLiteParameter[savedCount];
            StringBuilder sbPros = new StringBuilder();
            int k = 0;
            for (int i = 0; i < propertyInfoList.Length && savedCount > 0; i++)
            {
                PropertyInfo propertyInfo = propertyInfoList[i];
                if (propertyInfo.GetCustomAttributes(typeof(IsDBFieldAttribute), false).Length > 0)
                {
                    object oldVal = propertyInfo.GetValue(oldObj, null);
                    object val = propertyInfo.GetValue(obj, null);
                    if (!object.Equals(oldVal, val))
                    {
                        sbPros.Append(string.Format(" {0}=:{0},", propertyInfo.Name));
                        SQLiteParameter param = new SQLiteParameter(m_ParameterMark + propertyInfo.Name, val == null ? DBNull.Value : val);
                        parameters[k++] = param;
                    }
                }
            }
            if (sbPros.Length > 0)
            {
                strSql.Append(sbPros.ToString(0, sbPros.Length - 1));
            }
            strSql.Append(string.Format(" where {0}='{1}'", GetIdName(obj.GetType()), GetIdVal(obj).ToString()));

            if (savedCount > 0)
            {
                ExecuteSql(strSql.ToString(), parameters);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据Id删除
        /// </summary>
        public static void Delete<T>(int id)
        {
            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(string.Format("delete from {0} where {2}='{1}'", type.Name, id, GetIdName(type)));

            ExecuteSql(sbSql.ToString());
        }
        /// <summary>
        /// 根据Id集合删除
        /// </summary>
        public static void BatchDelete<T>(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return;

            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(string.Format("delete from {0} where {2} in ({1})", type.Name, ids, GetIdName(type)));

            ExecuteSql(sbSql.ToString());
        }
        /// <summary>
        /// 根据条件删除
        /// </summary>
        public static void Delete<T>(string conditions)
        {
            if (string.IsNullOrWhiteSpace(conditions)) return;

            Type type = typeof(T);
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(string.Format("delete from {0} where {1}", type.Name, conditions));

            ExecuteSql(sbSql.ToString());
        }
        #endregion

        #region 获取实体
        #region 根据实体获取实体
        /// <summary>
        /// 根据实体获取实体
        /// </summary>
        private static object Find(object obj, bool readCache = true)
        {
            Type type = obj.GetType();

            object result = Activator.CreateInstance(type);
            bool hasValue = false;
            IDataReader rd = null;

            string sql = string.Format("select * from {0} where {2}='{1}'", type.Name, GetIdVal(obj), GetIdName(obj.GetType()));

            try
            {
                rd = ExecuteReader(sql);

                PropertyInfo[] propertyInfoList = GetEntityProperties(type);

                int fcnt = rd.FieldCount;
                List<string> fileds = new List<string>();
                for (int i = 0; i < fcnt; i++)
                {
                    fileds.Add(rd.GetName(i).ToUpper());
                }

                while (rd.Read())
                {
                    hasValue = true;
                    IDataRecord record = rd;

                    foreach (PropertyInfo pro in propertyInfoList)
                    {
                        if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                        {
                            continue;
                        }

                        pro.SetValue(result, record[pro.Name] == DBNull.Value ? null : getReaderValue(record[pro.Name], pro.PropertyType), null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            if (hasValue)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 根据Id获取实体
        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        private static object FindById(Type type, int id)
        {
            object result = Activator.CreateInstance(type);
            IDataReader rd = null;
            bool hasValue = false;

            string sql = string.Format("select * from {0} where {2}='{1}'", type.Name, id, GetIdName(type));

            try
            {
                rd = ExecuteReader(sql);

                PropertyInfo[] propertyInfoList = GetEntityProperties(type);

                int fcnt = rd.FieldCount;
                List<string> fileds = new List<string>();
                for (int i = 0; i < fcnt; i++)
                {
                    fileds.Add(rd.GetName(i).ToUpper());
                }

                while (rd.Read())
                {
                    hasValue = true;
                    IDataRecord record = rd;

                    foreach (PropertyInfo pro in propertyInfoList)
                    {
                        if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                        {
                            continue;
                        }

                        pro.SetValue(result, record[pro.Name] == DBNull.Value ? null : getReaderValue(record[pro.Name], pro.PropertyType), null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            if (hasValue)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 根据Id获取实体
        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        public static T FindById<T>(string id) where T : new()
        {
            Type type = typeof(T);
            T result = (T)Activator.CreateInstance(type);
            IDataReader rd = null;
            bool hasValue = false;

            string sql = string.Format("select * from {0} where {2}='{1}'", type.Name, id, GetIdName(type));

            try
            {
                rd = ExecuteReader(sql);

                PropertyInfo[] propertyInfoList = GetEntityProperties(type);

                int fcnt = rd.FieldCount;
                List<string> fileds = new List<string>();
                for (int i = 0; i < fcnt; i++)
                {
                    fileds.Add(rd.GetName(i).ToUpper());
                }

                while (rd.Read())
                {
                    hasValue = true;
                    IDataRecord record = rd;

                    foreach (PropertyInfo pro in propertyInfoList)
                    {
                        if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                        {
                            continue;
                        }

                        pro.SetValue(result, record[pro.Name] == DBNull.Value ? null : getReaderValue(record[pro.Name], pro.PropertyType), null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            if (hasValue)
            {
                return result;
            }
            else
            {
                return default(T);
            }
        }
        #endregion

        #region 根据sql获取实体
        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        public static T FindBySql<T>(string sql) where T : new()
        {
            Type type = typeof(T);
            T result = (T)Activator.CreateInstance(type);
            IDataReader rd = null;
            bool hasValue = false;

            try
            {
                rd = ExecuteReader(sql);

                PropertyInfo[] propertyInfoList = GetEntityProperties(type);

                int fcnt = rd.FieldCount;
                List<string> fileds = new List<string>();
                for (int i = 0; i < fcnt; i++)
                {
                    fileds.Add(rd.GetName(i).ToUpper());
                }

                while (rd.Read())
                {
                    hasValue = true;
                    IDataRecord record = rd;

                    foreach (PropertyInfo pro in propertyInfoList)
                    {
                        if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                        {
                            continue;
                        }

                        pro.SetValue(result, record[pro.Name] == DBNull.Value ? null : getReaderValue(record[pro.Name], pro.PropertyType), null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            if (hasValue)
            {
                return result;
            }
            else
            {
                return default(T);
            }
        }
        #endregion
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        public static List<T> FindListBySql<T>(string sql) where T : new()
        {
            List<T> list = new List<T>();
            object obj;
            IDataReader rd = null;

            try
            {
                rd = ExecuteReader(sql);

                if (typeof(T) == typeof(int))
                {
                    while (rd.Read())
                    {
                        list.Add((T)rd[0]);
                    }
                }
                else if (typeof(T) == typeof(string))
                {
                    while (rd.Read())
                    {
                        list.Add((T)rd[0]);
                    }
                }
                else
                {
                    PropertyInfo[] propertyInfoList = (typeof(T)).GetProperties();

                    int fcnt = rd.FieldCount;
                    List<string> fileds = new List<string>();
                    for (int i = 0; i < fcnt; i++)
                    {
                        fileds.Add(rd.GetName(i).ToUpper());
                    }

                    while (rd.Read())
                    {
                        IDataRecord record = rd;
                        obj = new T();


                        foreach (PropertyInfo pro in propertyInfoList)
                        {
                            if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                            {
                                continue;
                            }

                            pro.SetValue(obj, record[pro.Name] == DBNull.Value ? null : getReaderValue(record[pro.Name], pro.PropertyType), null);
                        }
                        list.Add((T)obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return list;
        }
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        public static List<T> FindListBySql<T>(string sql, params SQLiteParameter[] cmdParms) where T : new()
        {
            List<T> list = new List<T>();
            object obj;
            IDataReader rd = null;

            try
            {
                rd = ExecuteReader(sql, cmdParms);

                if (typeof(T) == typeof(int))
                {
                    while (rd.Read())
                    {
                        list.Add((T)rd[0]);
                    }
                }
                else if (typeof(T) == typeof(string))
                {
                    while (rd.Read())
                    {
                        list.Add((T)rd[0]);
                    }
                }
                else
                {
                    PropertyInfo[] propertyInfoList = (typeof(T)).GetProperties();

                    int fcnt = rd.FieldCount;
                    List<string> fileds = new List<string>();
                    for (int i = 0; i < fcnt; i++)
                    {
                        fileds.Add(rd.GetName(i).ToUpper());
                    }

                    while (rd.Read())
                    {
                        IDataRecord record = rd;
                        obj = new T();


                        foreach (PropertyInfo pro in propertyInfoList)
                        {
                            if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                            {
                                continue;
                            }

                            pro.SetValue(obj, record[pro.Name] == DBNull.Value ? null : getReaderValue(record[pro.Name], pro.PropertyType), null);
                        }
                        list.Add((T)obj);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return list;
        }
        #endregion

        #region 分页获取列表
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        public static PagerModel FindPageBySql<T>(string sql, string orderby, int pageSize, int currentPage) where T : new()
        {
            PagerModel pagerModel = new PagerModel(currentPage, pageSize);

            using (DbConnection connection = GetConnection())
            {
                connection.Open();
                IDbCommand cmd = null;
                StringBuilder sb = new StringBuilder();
                string commandText = null;
                int startRow = 0;
                int endRow = 0;
                switch (m_DBType)
                {
                    case "oracle":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);
                        endRow = startRow + pageSize;

                        sb.Append("select * from ( select row_limit.*, rownum rownum_ from (");
                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.Append(" ) row_limit where rownum <= ");
                        sb.Append(endRow);
                        sb.Append(" ) where rownum_ >");
                        sb.Append(startRow);
                        #endregion
                        break;
                    case "mssql":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1) + 1;
                        endRow = startRow + pageSize - 1;

                        sb.Append(string.Format(@"
                            select * from 
                            (select ROW_NUMBER() over({1}) as rowNumber, t.* from ({0}) t) tempTable
                            where rowNumber between {2} and {3} ", sql, orderby, startRow, endRow));
                        #endregion
                        break;
                    case "mysql":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);

                        sb.Append("select * from (");
                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.AppendFormat(" ) row_limit limit {0},{1}", startRow, pageSize);
                        #endregion
                        break;
                    case "sqlite":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);

                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.AppendFormat(" limit {0} offset {1}", pageSize, startRow);
                        #endregion
                        break;
                }

                List<T> list = FindListBySql<T>(sb.ToString());
                pagerModel.result = list;
            }

            return pagerModel;
        }
        #endregion

        #region 分页获取列表
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static PagerModel FindPageBySql<T>(string sql, string orderby, int pageSize, int currentPage, params SQLiteParameter[] cmdParms) where T : new()
        {
            PagerModel pagerModel = new PagerModel(currentPage, pageSize);

            using (DbConnection connection = GetConnection())
            {
                connection.Open();
                IDbCommand cmd = null;
                StringBuilder sb = new StringBuilder();
                string commandText = null;
                int startRow = 0;
                int endRow = 0;
                switch (m_DBType)
                {
                    case "oracle":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);
                        endRow = startRow + pageSize;

                        sb.Append("select * from ( select row_limit.*, rownum rownum_ from (");
                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.Append(" ) row_limit where rownum <= ");
                        sb.Append(endRow);
                        sb.Append(" ) where rownum_ >");
                        sb.Append(startRow);
                        #endregion
                        break;
                    case "mssql":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1) + 1;
                        endRow = startRow + pageSize - 1;

                        sb.Append(string.Format(@"
                            select * from 
                            (select ROW_NUMBER() over({1}) as rowNumber, t.* from ({0}) t) tempTable
                            where rowNumber between {2} and {3} ", sql, orderby, startRow, endRow));
                        #endregion
                        break;
                    case "mysql":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);

                        sb.Append("select * from (");
                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.AppendFormat(" ) row_limit limit {0},{1}", startRow, pageSize);
                        #endregion
                        break;
                    case "sqlite":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        pagerModel.totalRows = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);

                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.AppendFormat(" limit {0} offset {1}", pageSize, startRow);
                        #endregion
                        break;
                }

                List<T> list = FindListBySql<T>(sb.ToString(), cmdParms);
                pagerModel.result = list;
            }

            return pagerModel;
        }


        #endregion

        #region 分页获取列表
        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        public static DataSet FindPageBySql(string sql, string orderby, int pageSize, int currentPage, out int totalCount, params SQLiteParameter[] cmdParms)
        {
            DataSet ds = null;

            using (DbConnection connection = GetConnection())
            {
                connection.Open();
                IDbCommand cmd = null;
                StringBuilder sb = new StringBuilder();
                string commandText = null;
                int startRow = 0;
                int endRow = 0;
                totalCount = 0;
                switch (m_DBType)
                {
                    case "oracle":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        totalCount = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);
                        endRow = startRow + pageSize;

                        sb.Append("select * from ( select row_limit.*, rownum rownum_ from (");
                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.Append(" ) row_limit where rownum <= ");
                        sb.Append(endRow);
                        sb.Append(" ) where rownum_ >");
                        sb.Append(startRow);
                        #endregion
                        break;
                    case "mssql":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        totalCount = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1) + 1;
                        endRow = startRow + pageSize - 1;

                        sb.Append(string.Format(@"
                            select * from 
                            (select ROW_NUMBER() over({1}) as rowNumber, t.* from ({0}) t) tempTable
                            where rowNumber between {2} and {3} ", sql, orderby, startRow, endRow));
                        #endregion
                        break;
                    case "mysql":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        totalCount = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);

                        sb.Append("select * from (");
                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.AppendFormat(" ) row_limit limit {0},{1}", startRow, pageSize);
                        #endregion
                        break;
                    case "sqlite":
                        #region 分页查询语句
                        commandText = string.Format("select count(*) from ({0}) T", sql);
                        cmd = GetCommand(commandText, connection);
                        totalCount = int.Parse(cmd.ExecuteScalar().ToString());

                        startRow = pageSize * (currentPage - 1);

                        sb.Append(sql);
                        if (!string.IsNullOrWhiteSpace(orderby))
                        {
                            sb.Append(" ");
                            sb.Append(orderby);
                        }
                        sb.AppendFormat(" limit {0} offset {1}", pageSize, startRow);
                        #endregion
                        break;
                }

                ds = Query(sql, cmdParms);
            }
            return ds;
        }
        #endregion

        #region getReaderValue 转换数据
        /// <summary>
        /// 转换数据
        /// </summary>
        private static Object getReaderValue(Object rdValue, Type ptype)
        {
            if (ptype == typeof(double))
                return Convert.ToDouble(rdValue);

            if (ptype == typeof(decimal))
                return Convert.ToDecimal(rdValue);

            if (ptype == typeof(int))
                return Convert.ToInt32(rdValue);

            if (ptype == typeof(long))
                return Convert.ToInt64(rdValue);

            if (ptype == typeof(DateTime))
                return Convert.ToDateTime(rdValue);

            if (ptype == typeof(Nullable<double>))
                return Convert.ToDouble(rdValue);

            if (ptype == typeof(Nullable<decimal>))
                return Convert.ToDecimal(rdValue);

            if (ptype == typeof(Nullable<int>))
                return Convert.ToInt32(rdValue);

            if (ptype == typeof(Nullable<long>))
                return Convert.ToInt64(rdValue);

            if (ptype == typeof(Nullable<DateTime>))
                return Convert.ToDateTime(rdValue);

            return rdValue;
        }
        #endregion

        #region 获取主键名称
        /// <summary>
        /// 获取主键名称
        /// </summary>
        public static string GetIdName(Type type)
        {
            PropertyInfo[] propertyInfoList = GetEntityProperties(type);
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                if (propertyInfo.GetCustomAttributes(typeof(IsIdAttribute), false).Length > 0)
                {
                    return propertyInfo.Name;
                }
            }
            return "Id";
        }
        #endregion

        #region 获取主键值
        /// <summary>
        /// 获取主键名称
        /// </summary>
        public static object GetIdVal(object val)
        {
            string idName = GetIdName(val.GetType());
            if (!string.IsNullOrWhiteSpace(idName))
            {
                return val.GetType().GetProperty(idName).GetValue(val, null);
            }
            return 0;
        }
        #endregion

        #region 获取实体类属性
        /// <summary>
        /// 获取实体类属性
        /// </summary>
        private static PropertyInfo[] GetEntityProperties(Type type)
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            PropertyInfo[] propertyInfoList = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                if (propertyInfo.GetCustomAttributes(typeof(EdmRelationshipNavigationPropertyAttribute), false).Length == 0
                    && propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), false).Length == 0)
                {
                    result.Add(propertyInfo);
                }
            }
            return result.ToArray();
        }
        #endregion

        #region 获取基类
        /// <summary>
        /// 获取基类
        /// </summary>
        public static Type GetBaseType(Type type)
        {
            while (type.BaseType != null && type.BaseType.Name != typeof(Object).Name)
            {
                type = type.BaseType;
            }
            return type;
        }
        #endregion
        #endregion

        #region 事务
        #region 开始事务
        /// <summary>
        /// 开始事务
        /// </summary>
        public static void BeginTransaction()
        {
            DbConnection conn = GetConnection();
            if (conn.State != ConnectionState.Open) conn.Open();
            m_Tran = conn.BeginTransaction();
        }
        #endregion

        #region 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public static void CommitTransaction()
        {
            DbConnection conn = m_Tran.Connection;
            try
            {
                m_Tran.Commit();
            }
            catch (Exception ex)
            {
                m_Tran.Rollback();
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
                m_Tran.Dispose();
                m_Tran = null;
            }
        }
        #endregion

        #region 回滚事务(出错时调用该方法回滚)
        /// <summary>
        /// 回滚事务(出错时调用该方法回滚)
        /// </summary>
        public static void RollbackTransaction()
        {
            DbConnection conn = m_Tran.Connection;
            m_Tran.Rollback();
            if (conn.State == ConnectionState.Open) conn.Close();
        }
        #endregion
        #endregion
    }
}
