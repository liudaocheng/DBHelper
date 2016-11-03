using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL;
using DBUtil;
using Utils;

namespace Model生成器
{
    public partial class Form1 : Form
    {
        #region Form1
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        #endregion

        //生成
        private void btnCreate_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(delegate()
            {
                IDal dal = DalFactory.CreateDal(ConfigurationManager.AppSettings["DBType"]);
                List<Dictionary<string, string>> tableList = dal.GetAllTables();
                string strNamespace = ConfigurationManager.AppSettings["Namespace"];

                #region 操作控件
                InvokeDelegate invokeDelegate = delegate()
                {
                    btnCreate.Enabled = false;
                    progressBar1.Visible = true;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = tableList.Count;
                    progressBar1.Value = 0;
                };
                InvokeUtil.Invoke(this, invokeDelegate);
                #endregion

                int i = 0;
                foreach (Dictionary<string, string> table in tableList)
                {
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sbExt = new StringBuilder();
                    List<Dictionary<string, string>> columnList = dal.GetAllColumns(table["table_name"]);

                    #region 原始Model
                    sb.Append("using System;\r\n");
                    sb.Append("using System.Collections.Generic;\r\n");
                    sb.Append("using System.Linq;\r\n");
                    sb.Append("\r\n");
                    sb.Append("namespace " + strNamespace + "\r\n");
                    sb.Append("{\r\n");
                    sb.Append("    /// <summary>\r\n");
                    sb.Append("    /// " + table["comments"] + "\r\n");
                    sb.Append("    /// </summary>\r\n");
                    sb.Append("    [Serializable]\r\n");
                    sb.Append("    public partial class " + table["table_name"] + "\r\n");
                    sb.Append("    {\r\n");
                    foreach (Dictionary<string, string> column in columnList)
                    {
                        string data_type = dal.ConvertDataType(column);

                        sb.Append("        /// <summary>\r\n");
                        sb.Append("        /// " + column["comments"] + "\r\n");
                        sb.Append("        /// </summary>\r\n");

                        if (column["constraint_type"] == "P")
                        {
                            sb.Append("        [IsId]\r\n");
                        }

                        sb.Append("        [IsDBField]\r\n");
                        sb.Append("        public " + data_type + " " + column["columns_name"] + " { get; set; }\r\n");
                    }
                    sb.Append("    }\r\n");
                    sb.Append("}\r\n");
                    FileHelper.WriteFile(Application.StartupPath + "\\models", sb.ToString(), table["table_name"]);
                    #endregion

                    #region 扩展Model
                    sbExt.Append("using System;\r\n");
                    sbExt.Append("using System.Collections.Generic;\r\n");
                    sbExt.Append("using System.Linq;\r\n");
                    sbExt.Append("\r\n");
                    sbExt.Append("namespace " + strNamespace + "\r\n");
                    sbExt.Append("{\r\n");
                    sbExt.Append("    /// <summary>\r\n");
                    sbExt.Append("    /// " + table["comments"] + "\r\n");
                    sbExt.Append("    /// </summary>\r\n");
                    sbExt.Append("    public partial class " + table["table_name"] + "\r\n");
                    sbExt.Append("    {\r\n");
                    sbExt.Append("\r\n");
                    sbExt.Append("    }\r\n");
                    sbExt.Append("}\r\n");
                    FileHelper.WriteFile(Application.StartupPath + "\\extmodels", sbExt.ToString(), table["table_name"]);
                    #endregion

                    #region 操作控件
                    invokeDelegate = delegate()
                    {
                        progressBar1.Value = ++i;
                    };
                    InvokeUtil.Invoke(this, invokeDelegate);
                    #endregion
                }

                #region 操作控件
                invokeDelegate = delegate()
                {
                    btnCreate.Enabled = true;
                    progressBar1.Visible = false;
                    progressBar1.Value = 0;
                };
                InvokeUtil.Invoke(this, invokeDelegate);
                #endregion

                MessageBox.Show("完成");
            })).Start();
        }
    }
}
