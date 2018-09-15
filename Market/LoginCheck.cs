using System;
using System.IO;
using System.Windows.Forms;

namespace Market
{
    class LoginCheck
    {
        /// <summary> 检查数据库是否预备，进行相应跳转
        /// </summary>
        public LoginCheck()
        {
            if (CheckDBReady() == true)
            {
                Login Login_Frm = new Login();//数据库已预备，初始化登录窗体
                Login_Frm.Show();//显示登录窗体
            }
            else
            {
                MessageBox.Show(null,"Oracle数据库未初始化，请先进行设置！","状态错误");
                Setting Setting_Frm = new Setting("Ready_set");//数据库未就绪，初始化设置窗体
                Setting_Frm.Show();//显示设置窗体
            }
        }
        /// <summary> 检查数据库是否已经初始化
        /// </summary>
        /// <returns>返回 true：已初始化 false：未初始化</returns>
        private Boolean CheckDBReady()
        {
            if (!File.Exists(Environment.CurrentDirectory + "/Config.inf"))//根目录不存在配置文件
                return false;//未初始化
            else
            {
                String[] ConfigInfo = File.ReadAllLines(Environment.CurrentDirectory + "/Config.inf");//打开配置文件
                if (ConfigInfo[0].Equals("DBReady = True"))
                    return true;//配置文件中指示数据库已初始化
                else
                    return false;//配置文件中未指示数据库初始化
            }
        }
    }
}
