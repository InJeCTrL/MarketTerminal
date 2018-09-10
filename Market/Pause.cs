using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Market
{
    public partial class Pause : Form
    {
        /// <summary> 本机员工工号
        /// </summary>
        private String StaffCode;
        /// <summary> 标记密码输入是否正确
        /// </summary>
        private Boolean RightPwd = false;
        /// <summary> 初始化锁定窗体
        /// </summary>
        /// <param name="_StaffCode">本机员工工号</param>
        public Pause(String _StaffCode)
        {
            InitializeComponent();
            StaffCode = _StaffCode;//Pause类中的工号赋值
        }
        /// <summary> 连接数据库验证员工工号与密码是否正确
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("InJ"))//密码正确
            {
                RightPwd = true;//设置密码正确标记为真
                this.Close();//若密码验证正确则关闭锁定窗体
                Form1.MainFrm.button2.PerformClick();//模拟单击收银主窗体中的开始营业按钮
            }
            else
                label6.Text = "验证失败！";//密码错误
        }
        /// <summary> 锁定窗体关闭时检查密码是否正确，错误则不允许关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pause_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RightPwd == false)
                e.Cancel = true;//密码错误，不允许关闭
        }
    }
}
