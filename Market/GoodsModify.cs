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
    public partial class GoodsModify : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 商品信息
        /// </summary>
        private String[] GoodsInfo;
        /// <summary> 标记是否成功修改了商品信息
        /// </summary>
        private Boolean Modified_OK = false;
        /// <summary> 初始化商品修改
        /// </summary>
        /// <param name="_GoodsInfo">商品信息集</param>
        public GoodsModify(String[] _GoodsInfo)
        {
            InitializeComponent();
            GoodsInfo = _GoodsInfo;//获取商品原信息
            textBox1.Text = GoodsInfo[0];//商品编号
            textBox2.Text = GoodsInfo[1];//商品名
            textBox3.Text = GoodsInfo[2];//进价
            textBox4.Text = GoodsInfo[3];//售价
            textBox5.Text = GoodsInfo[4];//货架存量
            textBox6.Text = GoodsInfo[5];//库存量
            textBox7.Text = GoodsInfo[6];//品牌
            textBox8.Text = GoodsInfo[7];//单位
        }
        /// <summary> 提交修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Equals("") && textBox3.Text.Equals("") && textBox4.Text.Equals("") &&
                textBox5.Text.Equals("") && textBox6.Text.Equals("") && textBox7.Text.Equals("") &&
                textBox8.Text.Equals(""))
            {//若存在未填项
                MessageBox.Show(null, "所有信息必须完整，请重新填写！", "修改失败");
            }
            else
            {//都填写了
                if (!textBox2.Text.Equals(GoodsInfo[1]) || !textBox3.Text.Equals(GoodsInfo[2]) || !textBox4.Text.Equals(GoodsInfo[3]) ||
                    !textBox5.Text.Equals(GoodsInfo[4]) || !textBox6.Text.Equals(GoodsInfo[5]) || !textBox7.Text.Equals(GoodsInfo[6]) ||
                    !textBox8.Text.Equals(GoodsInfo[7]))//若商品信息发生变动
                {
                    if (DBMgr.UpdateGoodsInfo(new String[]{textBox1.Text,textBox2.Text,textBox3.Text,
                                                            textBox4.Text,textBox5.Text,textBox6.Text,
                                                            textBox7.Text,textBox8.Text}) == false)
                        MessageBox.Show(null, "商品信息修改失败，请检查输入是否有误！", "修改失败");
                    else
                    {
                        Modified_OK = true;//更新修改成功标记
                        MessageBox.Show(null, "商品各项信息修改成功！", "修改成功");
                        this.Close();//关闭修改窗体
                    }
                }
                else
                {//商品信息未发生变动
                    MessageBox.Show(null, "商品各项信息没有发生变动！", "修改失败");
                    this.Close();//关闭修改窗体
                }
            }
        }
        /// <summary> 取消修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();//直接关闭窗体
        }
        /// <summary> 关闭修改信息窗体后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoodsModify_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Modified_OK == true)//若已成功修改
                this.DialogResult = DialogResult.OK;//向商品信息管理页面传递OK信息
            else
                this.DialogResult = DialogResult.No;//向商品信息管理页面传递No信息
        }
    }
}
