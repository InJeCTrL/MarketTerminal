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
    public partial class Payment : Form
    {
        /// <summary> 应付金额
        /// </summary>
        private String Money;
        /// <summary> 标记是否成功交易
        /// </summary>
        private Boolean Success = false;
        /// <summary> 结算窗体初始化
        /// </summary>
        /// <param name="_Money">应付金额</param>
        public Payment(String _Money)
        {
            InitializeComponent();
            Money = _Money;//应付金额赋值
            textBox1.Text = Money;//应付金额赋值
            textBox2.Text = Money;//实付金额初始
            textBox3.Text = "0";//找零金额初始
            SetInputMode();//设置为输入实收金额模式
        }
        /// <summary> 设置输入实收金额模式，方便输入
        /// </summary>
        private void SetInputMode()
        {
            textBox2.Focus();//实收金额获取焦点
            textBox2.SelectAll();//选取全部
        }
        /// <summary> 完成结算按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Equals("实收金额有误"))
            {
                MessageBox.Show(null, "实收金额有误，请重新输入！", "数值检测");//提示重新输入
                SetInputMode();//设置为输入实收金额模式
            }
            else
            {
                Success = true;//标记成功交易
                this.Close();//交易完成，关闭窗体
            }
        }
        /// <summary> 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();//关闭结算窗体
        }
        /// <summary> 关闭后判断是否成功交易
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Payment_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Success)
            {//成功交易
                this.DialogResult = DialogResult.Yes;
            }
            else
                this.DialogResult = DialogResult.No;
        }
        /// <summary> 实收金额文本框动态检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            double Receive;//实收金额
            double Must = double.Parse(textBox1.Text);//初始化应付金额
            if (double.TryParse(textBox2.Text, out Receive) && Receive >= Must)
            {//若输入合法
                textBox3.Text = (Receive - Must).ToString();//计算找零并显示
            }
            else
            {
                textBox3.Text = "实收金额有误";//找零文本框显示错误
            }
        }
    }
}
