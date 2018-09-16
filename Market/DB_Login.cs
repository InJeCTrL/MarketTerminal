using System;
using System.Windows.Forms;

namespace Market
{
    public partial class DB_Login : Form
    {
        /// <summary> 初始化数据库管理器
        /// </summary>
        DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 数据库登录验证窗体
        /// </summary>
        public DB_Login()
        {
            InitializeComponent();
        }
        /// <summary> 验证账户按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DBMgr.DataBaseLogin(textBox1.Text, textBox2.Text);//验证登录是否成功
            if (DBMgr.Success == true)
            {
                DBMgr.CreateConfigFile();//创建配置文件
                this.DialogResult = DialogResult.Yes;//标记验证成功
            }
            else
            {
                label6.Text = "验证失败！";//密码错误
            }
        }
    }
}
