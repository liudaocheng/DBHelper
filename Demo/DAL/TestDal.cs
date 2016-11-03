using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtil;
using Models;

namespace DAL
{
    public class TestDal
    {
        #region 分页获取
        /// <summary>
        /// 分页获取
        /// </summary>
        public List<BS_Test> GetList(ref PagerModel pager, string name)
        {
            StringBuilder sql = new StringBuilder(string.Format(@"
                select *
                from BS_Test t
                where 1=1 "));

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql.AppendFormat(" and t.name like '%{0}%'", name);
            }

            string orderby = string.Empty;
            pager = DBHelper.FindPageBySql<BS_Test>(sql.ToString(), orderby, pager.rows, pager.page);
            return pager.result as List<BS_Test>;
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        public List<BS_Test> GetList(string name)
        {
            StringBuilder sql = new StringBuilder(string.Format(@"
                select *
                from BS_Test t
                where 1=1 "));

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql.AppendFormat(" and t.name like '%{0}%'", name);
            }

            return DBHelper.FindListBySql<BS_Test>(sql.ToString());
        }
        #endregion

        #region 获取
        public BS_Test Get(string id)
        {
            return DBHelper.FindById<BS_Test>(id);
        }
        #endregion

        #region GetMaxId
        /// <summary>
        /// GetMaxId
        /// </summary>
        public int GetMaxId()
        {
            return DBHelper.GetMaxID<BS_Test>("id");
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        public void Insert(object obj)
        {
            DBHelper.Insert(obj);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        public void Update(object obj)
        {
            DBHelper.Update(obj);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        public void Del(string id)
        {
            DBHelper.Delete<BS_Test>(Convert.ToInt32(id));
        }
        #endregion
    }
}
