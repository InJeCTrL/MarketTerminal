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
                if (DBMgr.DataBaseSetting() == true)//登录成功，初始化数据库
                {
                    DBMgr.CreateConfigFile();//登录成功，创建配置文件
                    MessageBox.Show(null, "数据库与配置文件各项初始化完成！", "初始化完成");//提示程序退出，即将重启程序
                    MessageBox.Show(null, "系统已自动生成默认超级员工用于登录\n员工号：admin\n密码：123\n您可在后续设置中删除此员工","初始化完成");//提示已创建初始账户
                    MessageBox.Show(null, "确认以重启程序！","初始化完成");//提示程序将重启
                    Application.Restart();//重启程序
                    Environment.Exit(0);//防止仍有线程活动，强制关闭
                }
                else
                    MessageBox.Show(null, "数据库与配置初始化错误！", "初始化失败");//提示初始化错误
            }
        }
    }
}
