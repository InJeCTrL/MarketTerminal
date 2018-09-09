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
        /// <summary> 商品扫描并检查
        /// </summary>
        private CheckGoods ChkGoods = new CheckGoods();
        /// <summary> 收银主窗体初始化，初始化摄像头
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            if (InitCamera() == false)
            {
                button2.Enabled = false;//不允许开始营业
                MessageBox.Show(null, "摄像头初始化失败，请检查设备！", "设备异常");//若摄像头初始化失败则报错
            }
        }
        /// <summary> 询问是否退出超市供销系统，并执行相应操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(null, "是否要退出系统？", "退出确认", MessageBoxButtons.YesNo))
            {
                StopCamera();//关闭摄像头
                Wnd_Stop();//设置各控件到暂停营业状态
                Application.Exit();//退出程序
            }
        }
        /// <summary> 窗体显示时设置各控件位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            button1.SetBounds(this.Size.Width - 110 - 30, 30, 111, 44);//设置关闭系统按钮位置
            videoSourcePlayer1.SetBounds(this.Size.Width - 30 - 301, 87, 301, 219);//设置视频输出区位置
        }
        /// <summary> 初始化摄像头
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean InitCamera()
        {
            if (ChkGoods.CheckVideoDevice() == false)//检查视频输入设备
                return false;//不存在视频输入设备返回false
            else
            {
                ChkGoods.SetVideoSource();//设置视频输入源
                videoSourcePlayer1.VideoSource = ChkGoods.GetVideoSource();//将收银窗体中视频控件与视频输入设备关联
                return true;
            }
        }
        /// <summary> 开启摄像头，开始捕获
        /// </summary>
        private void StartCamera()
        {
            videoSourcePlayer1.Start();//开启摄像头
        }
        /// <summary> 关闭摄像头，停止捕获
        /// </summary>
        private void StopCamera()
        {
            videoSourcePlayer1.Stop();
        }
        /// <summary> 开始营业时设置各控件状态
        /// </summary>
        private void Wnd_Start()
        {
            StartCamera();//开启摄像头
            button3.Enabled = true;//允许暂停营业
            button2.Enabled = false;//不允许多次开始营业
            timer1.Enabled = true;//启动定时器，定时监测条码
        }
        /// <summary> 暂停营业时设置各控件状态
        /// </summary>
        private void Wnd_Stop()
        {
            StopCamera();//停止摄像头
            button3.Enabled = false;//允许开始营业
            button2.Enabled = true;//不允许多次暂停营业
            timer1.Enabled = false;//关闭定时器，不再监测条码
        }
        /// <summary> 开始营业按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Wnd_Start();//设置各控件到营业状态
        }
        /// <summary> 暂停营业按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            Wnd_Stop();//设置各控件到暂停营业状态
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
    }
}
