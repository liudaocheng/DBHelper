using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Models;

namespace XXPLClient.UI
{
    public partial class PagerControl : UserControl
    {
        #region PageSize
        private int _pageSize = 20;
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value == 0)
                {
                    _pageSize = 1;
                    Pager.rows = 1;
                }
                else
                {
                    _pageSize = value;
                    Pager.rows = value;
                }
            }
        }
        #endregion

        #region 事件
        public event PageChangedHandler PageChanged = null;
        public event RefreshDataHandler RefreshData = null;
        #endregion

        #region Pager
        private PagerModel _pager = new PagerModel(1, 20);
        public PagerModel Pager
        {
            get
            {
                return _pager;
            }
            set
            {
                _pager = value;
                txtCurrentPage.Text = _pager.page.ToString();
                lblTotalPage.Text = " / " + _pager.pageCount.ToString();
            }
        }
        #endregion

        #region PagerControl 构造函数
        public PagerControl()
        {
            InitializeComponent();
        }
        #endregion

        #region PagerControl_Load
        private void PagerControl_Load(object sender, EventArgs e)
        {
            tools.BackColor = SystemColors.Control;
        }
        #endregion

        private void btnFirst_Click(object sender, EventArgs e)
        {
            Pager.page = 1;
            if (PageChanged != null)
            {
                PageChanged();
            }
        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            Pager.page = Pager.prePage;
            if (PageChanged != null)
            {
                PageChanged();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Pager.page = Pager.nextPage;
            if (PageChanged != null)
            {
                PageChanged();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            Pager.page = Pager.pageCount;
            if (PageChanged != null)
            {
                PageChanged();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (RefreshData != null)
            {
                RefreshData();
            }
        }

        private void btnCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCurrentPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (Convert.ToInt32(txtCurrentPage.Text) < 1)
            {
                txtCurrentPage.Text = "1";
            }
            if (Convert.ToInt32(txtCurrentPage.Text) > Pager.pageCount)
            {
                txtCurrentPage.Text = Pager.pageCount.ToString();
            }
            Pager.page = Convert.ToInt32(txtCurrentPage.Text);
            if (PageChanged != null)
            {
                PageChanged();
            }
        }
    }
    /// <summary>
    /// 翻页
    /// </summary>
    public delegate void PageChangedHandler();
    /// <summary>
    /// 刷新数据
    /// </summary>
    public delegate void RefreshDataHandler();
}
