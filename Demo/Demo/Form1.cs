using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL;
using DBUtil;
using Models;

namespace Demo
{
    public partial class Form1 : Form
    {
        private TemplateDal m_TemplateDal = new TemplateDal();
        private TestDal m_TestDal = new TestDal();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindList();
        }

        //测试新增
        private void button1_Click(object sender, EventArgs e)
        {
            int k = 0;
            for (int i = 0; i < 100; i++)
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    try
                    {
                        DBHelper.BeginTransaction();

                        BS_Template model = new BS_Template();
                        model.id = m_TemplateDal.GetMaxId().ToString();
                        model.code = k.ToString("0000");
                        model.name = "测试" + k.ToString();
                        model.remarks = "测试" + k.ToString();
                        model.type = ((int)Enums.TemplateType.Notice).ToString();
                        m_TemplateDal.Insert(model);
                        //throw new Exception("a");

                        BS_Test test = new BS_Test();
                        test.id = m_TestDal.GetMaxId().ToString();
                        test.code = "测试" + k.ToString();
                        test.name = "测试" + k.ToString();
                        test.remarks = "测试" + k.ToString();
                        m_TestDal.Insert(test);

                        DBHelper.CommitTransaction();

                        k++;
                        if (k == 100)
                        {
                            MessageBox.Show("插入数据成功");
                            this.Invoke(new InvokeDelegate(() =>
                            {
                                BindList();
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        DBHelper.RollbackTransaction();
                        MessageBox.Show(ex.Message);
                    }
                }));
            }
        }

        #region 绑定列表
        /// <summary>
        /// 绑定列表
        /// </summary>
        private void BindList()
        {
            try
            {
                gridView.DataSource = null;
                PagerModel pager = pagerControl1.Pager;
                List<BS_Template> list = m_TemplateDal.GetList(ref pager, null, null, null, Enums.TemplateType.Notice);
                pagerControl1.Pager = pager;
                list.ForEach(a =>
                {

                });
                gridView.ClearSelection();
                gridView.Columns.Clear();
                gridView.AutoGenerateColumns = false;
                DataGridViewTextBoxColumn dc = new DataGridViewTextBoxColumn();
                dc.HeaderText = "ID";
                dc.DataPropertyName = "id";
                //dc.Visible = false;
                gridView.Columns.Add(dc);
                dc = new DataGridViewTextBoxColumn();
                dc.HeaderText = "编码";
                dc.DataPropertyName = "code";
                gridView.Columns.Add(dc);
                dc = new DataGridViewTextBoxColumn();
                dc.HeaderText = "名称";
                dc.DataPropertyName = "name";
                dc.Width = 170;
                gridView.Columns.Add(dc);
                dc = new DataGridViewTextBoxColumn();
                dc.HeaderText = "备注";
                dc.DataPropertyName = "remarks";
                gridView.Columns.Add(dc);
                gridView.ReadOnly = true;
                gridView.DataSource = list;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }
        #endregion

        //翻页事件
        private void pagerControl1_PageChanged()
        {
            BindList();
        }

        //刷新数据
        private void pagerControl1_RefreshData()
        {
            BindList();
        }

        //测试修改
        private void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();

            try
            {
                //DBHelper.BeginTransaction();
                foreach (DataGridViewRow dr in gridView.SelectedRows)
                {
                    BS_Template old = (BS_Template)dr.DataBoundItem;
                    BS_Template model = m_TemplateDal.Get2(old.id, Enums.TemplateType.Notice);
                    model.remarks = rnd.Next(1, 9999).ToString("0000");
                    m_TemplateDal.Update(model);
                }
                //DBHelper.CommitTransaction();
                MessageBox.Show("修改成功");
                BindList();
            }
            catch (Exception ex)
            {
                DBHelper.RollbackTransaction();
                MessageBox.Show("修改失败：" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DBHelper.BeginTransaction();
                foreach (DataGridViewRow dr in gridView.SelectedRows)
                {
                    BS_Template old = (BS_Template)dr.DataBoundItem;
                    m_TemplateDal.Del(Convert.ToInt32(old.id));
                }
                DBHelper.CommitTransaction();
                MessageBox.Show("删除成功");
                BindList();
            }
            catch (Exception ex)
            {
                DBHelper.RollbackTransaction();
                MessageBox.Show("修改失败：" + ex.Message);
            }
        }
    }

    /// <summary>
    /// 跨线程访问控件的委托
    /// </summary>
    public delegate void InvokeDelegate();

}
