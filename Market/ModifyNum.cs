using System;
using System.Windows.Forms;

namespace Market
{
    public partial class ModifyNum : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 临时保存商品信息
        /// </summary>
        private String[] tGoodsInfo;
        /// <summary> 初始化数量修改窗体
        /// </summary>
        public ModifyNum()
        {
            InitializeComponent();
            tGoodsInfo = DBMgr.GetGoodsInfo(Form1.MainFrm.listView1.FocusedItem.SubItems[0].Text);//临时获取商品信息
            textBox1.Text = Form1.MainFrm.listView1.FocusedItem.SubItems[4].Text;//textbox初始赋值
            textBox1.Focus();//定位到数量修改
            textBox1.SelectAll();//全选以方便修改
        }
        /// <summary> 取消修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary> 提交修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int TrueNum;//修改后的数值
            if (int.TryParse(textBox1.Text, out TrueNum) && TrueNum > 0 && TrueNum <= int.Parse(tGoodsInfo[4]))
            {//若输入值为纯数字，且大于0，不大于库存
                Form1.MainFrm.listView1.FocusedItem.SubItems[4].Text = textBox1.Text;//直接修改主窗体中listview的数量
                Form1.MainFrm.listView1.FocusedItem.SubItems[5].Text = (TrueNum * double.Parse(tGoodsInfo[3])).ToString();//直接修改主窗体中listview的金额
                this.Close();//关闭修改数量窗体
            }
            else
            {//输入值不是合法数字
                MessageBox.Show(null, "输入的数目不合法！\n可能因为：\n1.数值不为纯数字\n2.数值小等于0\n3.数值超出货架存量", "数值检测");
                textBox1.Focus();//输入框选中
                textBox1.SelectAll();//文本全选便于修改
            }
        }
    }
}
