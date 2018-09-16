using System;
using System.Windows.Forms;

namespace Market
{
    public partial class Setting : Form
    {
        /// <summary> 声明数据库管理器
        /// </summary>
        private DataBaseManager DBMgr;
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
            {
                ExitOnClose = true;//若当前正在进行初始化设置，则设置关闭时结束程序
                DataBaseManager TryLink = new DataBaseManager();//尝试连接
                if (TryLink.Success == false)
                {//尝试连接失败
                    MessageBox.Show(null, "尝试连接至Oracle数据库失败，可能因为：\n1.配置文件缺失或信息错误\n2.配置文件中记录的用户名与密码错误\n3.Oracle XE(X86)未正常安装\n\n请尝试重新登录数据库！", "状态错误");
                    DB_Login DB_Login_Frm = new DB_Login();//实例化数据库登录验证窗体
                    if (DB_Login_Frm.ShowDialog() != DialogResult.Yes)//模态启动验证窗体
                    {//验证失败
                        Application.Exit();//关闭程序
                        Environment.Exit(0);//防止仍有线程活动，强制关闭
                    }
                }
            }
            DBMgr = new DataBaseManager();//初始化数据库管理器
            if (DBMgr.CheckDBReady())
            {//检测到数据库中已有相应表和触发器
                button1.Enabled = false;//禁止重复初始化数据库
                button2.Enabled = true;//允许删除数据库与配置文件
                button3.Enabled = true;//允许管理员工
            }
            else
            {//检测到数据库中没有相应表和触发器
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
            if (DBMgr.DataBaseSetting() == true)//登录成功，初始化数据库
            {
                DBMgr.CreateConfigFile();//登录成功，创建配置文件
                MessageBox.Show(null, "数据库与配置文件各项初始化完成！", "初始化完成");//提示程序退出，即将重启程序
                MessageBox.Show(null, "系统已自动生成默认超级员工用于登录\n员工号：admin\n密码：123\n您可在后续设置中删除此员工", "初始化完成");//提示已创建初始账户
                MessageBox.Show(null, "确认以重启程序！", "初始化完成");//提示程序将重启
                Application.Restart();//重启程序
                Environment.Exit(0);//防止仍有线程活动，强制关闭
            }
            else
                MessageBox.Show(null, "数据库与配置初始化错误！", "初始化失败");//提示初始化错误
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
                    Application.Restart();//重启程序
                    Environment.Exit(0);//防止仍有线程活动，强制关闭
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
            if (DialogResult.Yes == StaffMgr_frm.ShowDialog())//模态启动窗体
            {
                MessageBox.Show(null, "员工信息变更后必须重启程序以检测生效，确认以重启程序！", "员工检测");//提示程序退出，即将重启程序
                Application.Restart();//重启程序
                Environment.Exit(0);//防止仍有线程活动，强制关闭
            }
        }
    }
}
