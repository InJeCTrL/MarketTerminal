using System;
using System.IO;
using System.Windows.Forms;

namespace Market
{
    class LoginCheck
    {
        /// <summary> 初始化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 检查数据库初始化是否正常，进行相应跳转
        /// </summary>
        public LoginCheck()
        {
            if (DBMgr.CheckDBReady())
            {
                Login Login_Frm = new Login();//数据库初始化已预备，初始化登录窗体
                Login_Frm.Show();//显示登录窗体
            }
            else
            {
                MessageBox.Show(null,"Oracle数据库尚未初始化或配置文件异常","状态错误");
                Setting Setting_Frm = new Setting("Ready_set");//未初始化数据库，初始化设置窗体
                Setting_Frm.Show();//显示设置窗体
            }
        }
    }
}
