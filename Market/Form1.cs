using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
            groupBox1.SetBounds(30 + 30, videoSourcePlayer1.Top, videoSourcePlayer1.Left - 60 - 60, 169);//设置员工手动输入区位置
            comboBox1.SetBounds(87,28,groupBox1.Width - 87 - 20,20);//设置手动输入条码框位置
            comboBox1.Refresh();//重绘条码输入框
            textBox2.SetBounds(87, 69, groupBox1.Width - 87 - 20, 20);//设置手动输入数量位置
            button6.SetBounds(groupBox1.Width / 2 - 55, 113, 111, 44);//设置手动输入区按钮位置
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
                {
                    if (DBMgr.IsGoodsExists(Code_str))//若已存在此类商品，才进行响应
                    {
                        String[] tGoodsInfo = DBMgr.GetGoodsInfo(Code_str);//获取商品信息存储到临时信息集中
                        if (int.Parse(tGoodsInfo[4].ToString()) > 0)//若检测到商品数量不为空
                            AddGoodsToListview(Code_str);//将商品添加到列表中
                    }
                    else//DB中不存在此类商品
                    {
                        Code_str = null;//存储的临时条码值置空
                    }
                }
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
        /// <summary> 添加商品到listview中
        /// </summary>
        /// <param name="GoodsNo">商品编号</param>
        private void AddGoodsToListview(String GoodsNo)
        {
            String[] tGoodsInfo = DBMgr.GetGoodsInfo(GoodsNo);//获取商品信息集
            Boolean HasA = false;//标记是否已有至少一个商品在列表中
            int i;//若已有至少一个商品在列表中，用于保存待更新列表项下标
            for (i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].SubItems[0].Text.Equals(GoodsNo))//若商品已存在于列表中
                {
                    HasA = true;//标记已有至少一个商品在列表中
                    break;//保存i
                }
            }
            if (!HasA)//若没有该商品在列表中
                listView1.Items.Add(new ListViewItem(new String[] { tGoodsInfo[0], tGoodsInfo[1], tGoodsInfo[2], tGoodsInfo[3], "1", tGoodsInfo[3] }));//添加商品
            else//已有商品
            {
                listView1.Items[i].SubItems[4].Text = (int.Parse(listView1.Items[i].SubItems[4].Text) + 1).ToString();//更新预购买个数
                listView1.Items[i].SubItems[5].Text = (int.Parse(listView1.Items[i].SubItems[4].Text) * double.Parse(tGoodsInfo[3])).ToString();//更新金额
            }
            UpdateTotal();//更新总数量与总金额显示
            UpdateBtnStatus();//更新结账与取消交易按钮状态
        }
        /// <summary> 用于过滤非法右键操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && //列表项右键时触发
                listView1.FocusedItem.Index >= 0 &&
                listView1.SelectedItems.Count == 1)//选定了一个列表项
            {
                contextMenuStrip1.Show(listView1, e.Location);//相对于列表弹出右键菜单
            }
        }
        /// <summary> 删除选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除本条ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.RemoveAt(listView1.FocusedItem.Index);//删除选定商品
            UpdateTotal();//更新总数量与总金额显示
            UpdateBtnStatus();//更新结账与取消交易按钮状态
        }
        /// <summary> 修改数量选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 修改数量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyNum ModifyNum_frm = new ModifyNum();//实例化修改数量窗体
            ModifyNum_frm.ShowDialog();//模态显示数量修改窗体
            UpdateTotal();//更新总数量与总金额显示
        }
        /// <summary> 取消本次交易按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;//取消交易后禁止结账
            button5.Enabled = false;//禁止多次取消交易
            listView1.Items.Clear();//清空购物车
            UpdateTotal();//更新总数量与总金额显示
        }
        /// <summary> 更新商品总数量与总金额
        /// </summary>
        private void UpdateTotal()
        {
            int TotalNum = 0;//临时保存总数量
            double TotalMoney = 0;//临时保存总金额
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                TotalNum += int.Parse(listView1.Items[i].SubItems[4].Text);//增加总数量
                TotalMoney += double.Parse(listView1.Items[i].SubItems[5].Text);//增加总金额
            }
            label2.Text = TotalNum.ToString();//更新总数量
            label4.Text = TotalMoney.ToString();//更新总金额
        }
        /// <summary> 结账按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Payment Payment_frm = new Payment(label4.Text);//实例化结算窗体
            if (Payment_frm.ShowDialog() == DialogResult.Yes)
            {//若已成功完成交易
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    DBMgr.UpdateGoodsNum_MarketLeft(listView1.Items[i].SubItems[0].Text, -int.Parse(listView1.Items[i].SubItems[4].Text));//更新商品数目
                }
                button5.PerformClick();//窗体上模拟取消交易
            }
        }
        /// <summary> 更新结账按钮与取消交易按钮状态
        /// </summary>
        private void UpdateBtnStatus()
        {
            if (label2.Text.Equals("0"))
            {
                button4.Enabled = false;//禁止结账
                button5.Enabled = false;//禁止取消交易
            }
            else
            {
                button4.Enabled = true;//运行结账
                button5.Enabled = true;//运行取消交易
            }
        }
        /// <summary> 输入商品编号检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            List<String[]> tGoodsList = DBMgr.GetGoodsInfo_Part(comboBox1.Text);//获取模糊搜索的商品列表
            comboBox1.Items.Clear();//comboBox下拉菜单清空
            if (tGoodsList != null && tGoodsList.Count != 0)
            {//模糊搜索成功
                for (int i = 0; i < tGoodsList.Count; i++)
                {//顺序加入列表内容
                    comboBox1.Items.Add(tGoodsList.ElementAt(i)[0]);//显示商品编号
                }
                comboBox1.DroppedDown = true;//显示下拉列表
            }
            comboBox1.Select(comboBox1.Text.Length, 0);//光标重新移动到尾部
            Cursor = Cursors.Default;//防止光标被遮盖
        }
        /// <summary> 手动添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            String GoodsNo = comboBox1.Text;//商品编号
            if (DBMgr.IsGoodsExists(GoodsNo))
            {//若商品存在
                String[] tGoodsInfo = DBMgr.GetGoodsInfo(GoodsNo);//临时保存商品信息
                if (textBox2.Text.Equals(""))
                {
                    MessageBox.Show(null, "添加数量为空！", "添加失败");//提示商品不存在
                }
                else
                {
                    int i;//标记商品在购物车中位置s
                    int AddNum = int.Parse(textBox2.Text);//添加量赋值
                    for (i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].SubItems[0].Text.Equals(GoodsNo))
                        {//找到已添加过该商品
                            break;
                        }
                    }
                    if ((listView1.Items.Count == 0 &&//购物车中没有商品
                         AddNum <= int.Parse(tGoodsInfo[4])) ||//添加量小等于货架存量
                        (i < listView1.Items.Count &&//购物车中没有添加过该商品
                        (AddNum + int.Parse(listView1.Items[i].SubItems[4].Text) <= int.Parse(tGoodsInfo[4]))))//商品货架存量大于等于添加量
                    {
                        for (int j = 0; j < AddNum; j++)
                            AddGoodsToListview(GoodsNo);//循环添加
                        comboBox1.Text = "";//商品编码搜索位清空
                        textBox2.Text = "";//数量填写位清空
                    }
                    else
                    {//超出货架存量
                        MessageBox.Show(null, "添加数量超过该商品货架存量！", "添加失败");//提示商品超量
                    }
                }
            }
            else
            {
                MessageBox.Show(null, "该商品未入库！", "添加失败");//提示商品不存在
            }
        }
        /// <summary> 检测各商品货架存量、库存量并预警，5秒检测一次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            List<String[]> BadMarketLeft = DBMgr.GetGoodsMarketleftNotOk();//获取货架存量不足的商品
            List<String[]> BadTrunkLeft = DBMgr.GetGoodsTrunkleftNotOk();//获取库存量不足的商品
            if (BadMarketLeft == null || BadTrunkLeft == null)//查询失败
            {
                toolStripStatusLabel1.Text = "货架存量、库存检测失败！";
                toolStripSplitButton1.Visible = false;//隐藏日志按钮
                toolStripStatusLabel1.ForeColor = Color.Red;//标红
            }
            else if (BadMarketLeft.Count != 0 || BadTrunkLeft.Count != 0)
            {
                toolStripStatusLabel1.Text = "需要补货/进货！";
                toolStripSplitButton1.Visible = true;//显示日志按钮
                toolStripStatusLabel1.ForeColor = Color.Red;//标红
            }
            else
            {
                toolStripStatusLabel1.Text = "状态正常";
                toolStripSplitButton1.Visible = false;//隐藏日志按钮
                toolStripStatusLabel1.ForeColor = Color.Black;//颜色恢复
            }
        }
        /// <summary> 告警日志查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            List<String[]> BadMarketLeft = DBMgr.GetGoodsMarketleftNotOk();//获取货架存量不足的商品
            List<String[]> BadTrunkLeft = DBMgr.GetGoodsTrunkleftNotOk();//获取库存量不足的商品
            BadLogView BadLogView_frm = new BadLogView(BadMarketLeft, BadTrunkLeft);//实例化告警日志窗体
            BadLogView_frm.ShowDialog();//模态显示
        }
    }
}
