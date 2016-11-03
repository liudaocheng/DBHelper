using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    /// <summary>
    /// 分页
    /// </summary>
    [Serializable]
    public class PagerModel
    {
        #region 字段
        /// <summary>
        /// 当前页数
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string sort { get; set; }
        /// <summary>
        /// 排序的方式asc，desc
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 记录
        /// </summary>
        public object result { get; set; }
        /// <summary>
        /// 记录数
        /// </summary>
        public int totalRows { get; set; }
        #endregion

        #region 构造函数
        public PagerModel()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">当前页数</param>
        /// <param name="rows">每页记录数</param>
        public PagerModel(int page, int rows)
        {
            this.page = page;
            this.rows = rows;
        }
        #endregion

        #region 扩展字段
        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount
        {
            get
            {
                if (rows != 0)
                {
                    return (totalRows - 1) / rows + 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 上一页
        /// </summary>
        public int prePage
        {
            get
            {
                if (page - 1 > 0)
                {
                    return page - 1;
                }
                return 1;
            }
        }
        /// <summary>
        /// 下一页
        /// </summary>
        public int nextPage
        {
            get
            {
                if (page + 1 < pageCount)
                {
                    return page + 1;
                }
                return pageCount;
            }
        }
        #endregion

    }
}