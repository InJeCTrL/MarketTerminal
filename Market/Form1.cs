using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Controls;

namespace Market
{
    /// <summary> 收银主窗体
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary> 使收银主窗体全局可访问
        /// </summary>
        public static Form1 MainFrm;
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 商品扫描并检查
        /// </summary>
        private CheckGoods ChkGoods = new CheckGoods();
        /// <summary> 保存工号
        /// </summary>
        private String StaffCode;
        /// <summary> 标记用户是否选择退出
        /// </summary>
        private Boolean UserExit = false;
        /// <summary> 收银主窗体初始化，初始化摄像头
        /// </summary>
        /// <param name="StaffCode">员工工号</param>
        public Form1(String _StaffCode)
        {
            InitializeComponent();
            MainFrm = this;//设置MainFrm为本主窗体，使其他类可访问到
            if (ChkGoods.CheckVideoDevice() == false)//检查无视频输入设备
            {
                button2.Enabled = false;//不允许开始营业
                MessageBox.Show(null, "摄像头初始化失败，请检查设备！", "设备异常");//若摄像头初始化失败则报错
                Application.Exit();//程序退出
            }
            else
                ChkGoods.SetVideoSource(this.videoSourcePlayer1);//视频输入设备正常，设置视频输入源参数
            StaffCode = _StaffCode;//接收工号
        }
        /// <summary> 询问是否退出超市供销系统，并执行相应操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(null, "是否要退出系统？", "退出确认", MessageBoxButtons.YesNo))
            {
                ChkGoods.Stop(this.videoSourcePlayer1);//关闭摄像头
                Wnd_Stop();//设置各控件到暂停营业状态
                UserExit = true;//设置用户退出标记为真
                Application.Exit();//退出程序
            }
        }
        /// <summary> 窗体显示时设置各控件位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            label5.Text = StaffCode;//设置工号显示
            button1.SetBounds(this.Size.Width - 110 - 30, 30, 111, 44);//设置关闭系统按钮位置
            button7.SetBounds(button1.Left - 20 - 111, button1.Top, 111, 44);//设置系统设置按钮位置
            button9.SetBounds(button7.Left - 20 - 111, button7.Top, 111, 44);//设置账目管理按钮位置
            button8.SetBounds(button9.Left - 20 - 111, button9.Top, 111, 44);//设置商品管理按钮位置
            button3.SetBounds(button8.Left - 20 - 111, button8.Top, 111,44);//设置暂停营业按钮位置
            button2.SetBounds(button3.Left - 20 - 111, button3.Top, 111, 44);//设置开始营业按钮位置
            videoSourcePlayer1.SetBounds(this.Size.Width / 2 - 150, this.Height - 219 - 30, 301, 219);//设置视频输出区位置
            listView1.SetBounds(30,button1.Top + 44 + 50,this.Width - 60,videoSourcePlayer1.Top - button1.Top - 44 - 80);
            listView1.Columns[0].Width = listView1.Width / 5;//设置商品编号标签宽度
            listView1.Columns[1].Width = listView1.Width / 5;//设置商品名称标签宽度
            listView1.Columns[2].Width = listView1.Width / 5;//设置商品进货单价标签宽度
            listView1.Columns[3].Width = listView1.Width / 5;//设置商品销售单价标签宽度
            listView1.Columns[4].Width = listView1.Width / 10;//设置商品数量标签宽度
            listView1.Columns[5].Width = listView1.Width / 10 - 2;//设置商品总价标签宽度
            label1.SetBounds(this.Width - 109 - 30 - 150, listView1.Top + listView1.Height + 60, 109, 20);//设置商品总数(str)Label位置
            label2.SetBounds(label1.Left + label1.Width + 10, label1.Top, 109, 20);//设置商品总数(Num)Label位置
            label3.SetBounds(label1.Left, label1.Top + 20 + 20, 109, 20);//设置应付总金额(str)Label位置
            label4.SetBounds(label3.Left + label3.Width + 10, label3.Top, 109, 20);//设置应付总金额(Num)Label位置
            label6.SetBounds(30, 30, 147, 27);//设置本机工号(str)Label位置
            label5.SetBounds(label6.Left + label6.Width + 30, 30, 147, 27);//设置本机工号(Num)Label位置
            button4.SetBounds(label3.Left - 60, label3.Top + 20 + 40, 111, 44);//设置结账按钮位置
            button5.SetBounds(button4.Left + 111 + 30, button4.Top, 111, 44);//设置取消交易按钮位置
            groupBox1.SetBounds(30 + 30, videoSourcePlayer1.Top, 301, 169);//设置员工手动输入区位置
        }
        /// <summary> 开始营业时设置各控件状态
        /// </summary>
        private void Wnd_Start()
        {
            button2.Enabled = false;//不允许多次开始营业
            button7.Enabled = false;//不允许系统设置
            button8.Enabled = false;//不允许商品管理
            button9.Enabled = false;//不允许账目管理
            timer1.Enabled = true;//启动定时器，定时监测条码
        }
        /// <summary> 暂停营业时设置各控件状态
        /// </summary>
        private void Wnd_Stop()
        {
            button2.Enabled = true;//允许开始营业
            button7.Enabled = true;//允许系统设置
            button8.Enabled = true;//允许商品管理
            button9.Enabled = true;//允许账目管理
            timer1.Enabled = false;//关闭定时器，不再监测条码
        }
        /// <summary> 开始营业按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            ChkGoods.Start(this.videoSourcePlayer1);//启动摄像头并开始检查条码
            Wnd_Start();//设置各控件到营业状态
        }
        /// <summary> 暂停营业按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ChkGoods.Stop(this.videoSourcePlayer1);//关闭摄像头并暂停检查条码
            Wnd_Stop();//设置各控件到暂停营业状态
            Pause PauseFrm = new Pause(label5.Text);//实例化暂停营业锁定窗体
            PauseFrm.ShowDialog();//模态启动锁定窗体
        }
        /// <summary> 一秒检查一次摄像头截屏，进行条码识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap SnapShot = videoSourcePlayer1.GetCurrentVideoFrame();//截取当前摄像头图像
            if (SnapShot != null)//摄像头正式有效时
            {
                String Code_str = ChkGoods.CheckBarCode(SnapShot);//存放条码值字符串
                if (Code_str != null)
                    MessageBox.Show(Code_str);//若条码非空则显示条码
            }
        }
        /// <summary> 屏蔽非常规退出(Alt+F4)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UserExit == false)
                e.Cancel = true;//取消关闭
        }
        /// <summary> 判断输入的字符是否纯数字
        /// </summary>
        /// <param name="Input"></param>
        /// <returns>返回 true：纯数字 false：非数字</returns>
        private Boolean IsNumInput(char Input)
        {
            if (!Char.IsNumber(Input) && Input != (char)8)
                return false;//若输入为非数字并且不是backspace则返回false
            return true;
        }
        /// <summary> 判断输入的编号是否只为数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsNumInput(e.KeyChar) == false)
                e.Handled = true;//过滤非数字
        }
        /// <summary> 判断输入的数量是否只为数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsNumInput(e.KeyChar) == false)
                e.Handled = true;//过滤非数字
        }
        /// <summary> 打开系统设置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (DBMgr.IsSuperUser(StaffCode) == true)//验证本机登录为超级管理员
            {
                Setting SettingPage = new Setting();//实例化系统设置页面类
                SettingPage.ShowDialog();//模态显示系统设置页面
            }
            else
                MessageBox.Show(null,"权限不足，请与管理员联系！","拒绝访问");
        }
        /// <summary> 账目管理按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (DBMgr.IsSuperUser(StaffCode) == true)//验证本机登录为超级管理员
            {
                AccountMgr AccountMgr_frm = new AccountMgr();//实例化账目管理窗体
                AccountMgr_frm.ShowDialog();//模态显示
            }
            else
                MessageBox.Show(null, "权限不足，请与管理员联系！", "拒绝访问");
        }
        /// <summary> 商品管理按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (DBMgr.IsSuperUser(StaffCode) == true)//验证本机登录为超级管理员
            {
                TrunkMgr TrunkMgr_frm = new TrunkMgr();//实例化商品管理窗体
                TrunkMgr_frm.ShowDialog();//模态显示
                if (TrunkMgr_frm.DialogResult == DialogResult.Yes)
                {//若商品信息有实质上的修改
                    MessageBox.Show(null, "商品信息变更后必须重启程序以检测生效，确认以重启程序！", "商品检测");//提示程序退出，即将重启程序
                    Application.Restart();//重启程序
                    Environment.Exit(0);//防止仍有线程活动，强制关闭
                }
            }
            else
                MessageBox.Show(null, "权限不足，请与管理员联系！", "拒绝访问");
        }
    }
}
