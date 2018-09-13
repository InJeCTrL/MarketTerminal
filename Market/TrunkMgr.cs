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
    public partial class TrunkMgr : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 初始化条码扫描类
        /// </summary>
        private CheckGoods ScanBarcode = new CheckGoods();
        /// <summary> 商品列表
        /// </summary>
        List<String[]> GoodsList;
        /// <summary> 初始化商品管理器，设置摄像头参数
        /// </summary>
        public TrunkMgr()
        {
            InitializeComponent();
            Flush();//填充商品列表
            if (ScanBarcode.CheckVideoDevice() == false)//检查无视频输入设备
            {
                button1.Enabled = false;//不允许使用摄像头扫码进货
                MessageBox.Show(null, "摄像头初始化失败，请检查设备！", "设备异常");//若摄像头初始化失败则报错
            }
            else
                ScanBarcode.SetVideoSource(this.videoSourcePlayer1);//视频输入设备正常，设置视频输入源参数
        }
        /// <summary> 标记是否进行过增删改
        /// </summary>
        private Boolean Modified = false;
        /// <summary> 展开右键菜单后判断是否允许修改与删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1 || listView1.FocusedItem.Index < 0 || listView1.FocusedItem.Index >= GoodsList.Count)
            {//若没有选中有效商品项
                修改ToolStripMenuItem.Enabled = false;//禁止修改
                删除ToolStripMenuItem.Enabled = false;//禁止删除
            }
            else//已选中有效商品项
            {
                修改ToolStripMenuItem.Enabled = true;//允许修改
                删除ToolStripMenuItem.Enabled = true;//允许删除
            }
        }
        /// <summary> 重新获取商品列表，并刷新listview
        /// </summary>
        private void Flush()
        {
            listView1.Items.Clear();//清空商品列表
            GoodsList = DBMgr.GetGoodsList();//重新获取商品列表
            for (int i = 0; i < GoodsList.Count; i++)//插入所有记录到listview
            {
                listView1.Items.Add(new ListViewItem(GoodsList.ElementAt(i)));//单行插入
            }
        }
        /// <summary> 修改选中项商品信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoodsModify GoodsModify_frm = new GoodsModify(new String[]{listView1.FocusedItem.SubItems[0].Text,
                                                                       listView1.FocusedItem.SubItems[1].Text,
                                                                       listView1.FocusedItem.SubItems[2].Text,
                                                                       listView1.FocusedItem.SubItems[3].Text,
                                                                       listView1.FocusedItem.SubItems[4].Text,
                                                                       listView1.FocusedItem.SubItems[5].Text,
                                                                       listView1.FocusedItem.SubItems[6].Text,
                                                                       listView1.FocusedItem.SubItems[7].Text,});//实例化信息修改窗体
            if (GoodsModify_frm.ShowDialog() == DialogResult.OK)//模态显示信息修改窗体
            {//员工信息被修改过
                Flush();//刷新商品列表
                Modified = true;//标记增删改
            }
        }
        /// <summary> 删除选中项商品信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(null, "请确认是否删除该商品，该操作无法回滚！", "删除确认", MessageBoxButtons.YesNo))
            {
                if (DBMgr.DeleteGoods(listView1.FocusedItem.SubItems[0].Text) == true)
                {
                    MessageBox.Show(null, "商品删除成功！", "删除成功");
                    Flush();//刷新商品列表
                    Modified = true;//标记增删改
                }
                else
                    MessageBox.Show(null, "商品删除失败！", "删除失败");
            }
        }
        /// <summary> 添加员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 添加ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GoodsAdd GoodsAdd_frm = new GoodsAdd();//实例化新增商品类
            if (GoodsAdd_frm.ShowDialog() == DialogResult.OK)//模态显示新增商品窗体
            {//成功新增
                Flush();//刷新列表
                Modified = true;//标记增删改
            }
        }
        /// <summary> 商品管理窗体关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrunkMgr_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Modified == true)
            {//有过增删改
                this.DialogResult = DialogResult.Yes;//传回进行过增删改
            }
            else
                this.DialogResult = DialogResult.No;//传回未进行增删改
        }
        /// <summary> 开始进货
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ScanBarcode.Start(this.videoSourcePlayer1);//启动摄像头
            button2.Enabled = true;//允许停止进货
            button1.Enabled = false;//禁止多次开始进货
            timer1.Enabled = true;//启动时钟，检测条码
        }
        /// <summary> 每个一秒检查一次摄像头条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap SnapShot = videoSourcePlayer1.GetCurrentVideoFrame();//截取当前摄像头图像
            if (SnapShot != null)//摄像头正式有效时
            {
                String Code_str = ScanBarcode.CheckBarCode(SnapShot);//存放条码值字符串
                if (Code_str != null)
                {
                    ScanBarcode.Stop(videoSourcePlayer1);//停止摄像头
                    timer1.Enabled = false;//停止计时器检查
                    Boolean New = true;//标记是否是新一类商品
                    String[] GoodsInfo;//商品信息集
                    if (DBMgr.IsGoodsExists(Code_str))//若已存在此类商品
                    {
                        GoodsInfo = DBMgr.GetGoodsInfo(Code_str);//调用数据库管理器的商品信息查询
                        New = false;//标记New为假
                    }
                    else//DB中不存在此类商品
                    {
                        GoodsInfo = ScanBarcode.GetGoodsInfo(Code_str);//调用摄像头类，网络获取商品信息
                    }
                    GoodsAdd GoodsAdd_frm = new GoodsAdd(GoodsInfo, false, New);//实例化新增商品类
                    if (GoodsAdd_frm.ShowDialog() == DialogResult.OK)//模态显示新增商品窗体
                    {//成功新增
                        Flush();//刷新列表
                        Modified = true;//标记增删改
                    }
                    ScanBarcode.Start(this.videoSourcePlayer1);//打开摄像头
                    timer1.Enabled = true;//继续检测
                }
            }
        }
        /// <summary> 停止进货
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            ScanBarcode.Stop(this.videoSourcePlayer1);//停止摄像头
            timer1.Enabled = false;//停止计时器检测
            button1.Enabled = true;//允许开始进货
            button2.Enabled = false;//禁止反复停止进货
        }
        /// <summary> 关闭商品管理窗体时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrunkMgr_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer1.Enabled == true)
            {//若计时器仍在检测，不允许关闭
                e.Cancel = true;//取消关闭
                MessageBox.Show(null,"关闭前请先停止进货！","关闭撤销");
            }
        }
    }
}
