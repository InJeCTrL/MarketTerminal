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
    public partial class Setting : Form
    {
        /// <summary> 初始化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 默认在关闭窗体时不退出程序
        /// </summary>
        private Boolean ExitOnClose = false;
        /// <summary> 设置面板初始化
        /// </summary>
        /// <param name="arg">指令参数</param>
        public Setting(String arg = "Ready")
        {
            InitializeComponent();
            if (arg.Equals("Ready_set"))
                ExitOnClose = true;//若当前正在初始化数据库，则设置关闭时结束程序
            if (DBMgr.Success == true)//若数据库已验证成功
            {
                button1.Enabled = false;//禁止重复初始化数据库
                button2.Enabled = true;//允许删除数据库与配置文件
                button3.Enabled = true;//允许管理员工
            }
            else
            {
                button1.Enabled = true;//允许初始化数据库
                button2.Enabled = false;//禁止删除数据库与配置文件
                button3.Enabled = false;//禁止管理员工
            }
        }
        /// <summary> 初始化数据库与配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DB_Login DB_Login_Frm = new DB_Login();//实例化数据库登录验证窗体
            DB_Login_Frm.ShowDialog();//模态启动验证窗体
        }
        /// <summary> 窗体关闭时根据ExitOnClose判断是否关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ExitOnClose == true)
                Application.Exit();
        }
        /// <summary> 删除数据库与配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(null, "请问是否要删除本程序创建的表与配置文件？\n删除操作无法回滚！", "删除确认", MessageBoxButtons.YesNo))
            {//仅当确认删除时执行
                if (DBMgr.DataBaseDelete() == true)//删除数据库
                {
                    DBMgr.DeleteConfigFile();//删除配置文件
                    MessageBox.Show(null, "数据库与配置文件各项删除完成，确认以重启程序！", "删除完成");//提示程序退出，即将重启程序
                    Application.Restart();
                }
                else
                    MessageBox.Show(null, "删除失败！", "删除错误");//提示删除错误
            }
        }
        /// <summary> 员工管理按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            StaffMgr StaffMgr_frm = new StaffMgr();//实例化员工管理窗体
            StaffMgr_frm.ShowDialog();//模态启动窗体
        }
    }
}
