using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Market
{
    public partial class Login : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 员工登录页面
        /// </summary>
        public Login()
        {
            InitializeComponent();
        }
        /// <summary> 验证员工口令是否正确
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (DBMgr.CheckStaff(textBox1.Text, textBox2.Text) == true)//检查口令
            {
                Form1 Form1_frm = new Form1(textBox1.Text);//实例化主窗体
                Form1_frm.Show();//显示主窗体
                this.Close();//关闭登录窗体
            }
            else
                label6.Text = "验证失败！";
        }
        /// <summary> 取消登录则关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
