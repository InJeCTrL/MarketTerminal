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
    public partial class AccountModify : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 账目信息
        /// </summary>
        private String[] AccountInfo;
        /// <summary> 标记是否成功修改了账目信息
        /// </summary>
        private Boolean Modified_OK = false;
        /// <summary> 初始化账目修改
        /// </summary>
        /// <param name="_GoodsInfo">账目信息集</param>
        public AccountModify(String[] _AccountInfo)
        {
            InitializeComponent();
            AccountInfo = _AccountInfo;//获取账目原信息
            textBox1.Text = AccountInfo[0];//商品流水
            textBox2.Text = AccountInfo[1];//商品编号
            textBox3.Text = AccountInfo[2];//商品名称
            textBox4.Text = AccountInfo[3];//进货价
            textBox5.Text = AccountInfo[4];//销售单价
            textBox6.Text = AccountInfo[5];//数量
            textBox7.Text = AccountInfo[6];//单位
            textBox8.Text = AccountInfo[7];//利润
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
                if (!textBox2.Text.Equals(AccountInfo[1]) || !textBox3.Text.Equals(AccountInfo[2]) || !textBox4.Text.Equals(AccountInfo[3]) ||
                    !textBox5.Text.Equals(AccountInfo[4]) || !textBox6.Text.Equals(AccountInfo[5]) || !textBox7.Text.Equals(AccountInfo[6]) ||
                    !textBox8.Text.Equals(AccountInfo[7]))//若账目信息发生变动
                {
                    if (DBMgr.UpdateAccountInfo(new String[]{textBox1.Text,textBox2.Text,textBox3.Text,
                                                            textBox4.Text,textBox5.Text,textBox6.Text,
                                                            textBox7.Text,textBox8.Text}) == false)
                        MessageBox.Show(null, "账目信息修改失败，请检查输入是否有误！", "修改失败");
                    else
                    {
                        Modified_OK = true;//更新修改成功标记
                        MessageBox.Show(null, "账目各项信息修改成功！", "修改成功");
                        this.Close();//关闭修改窗体
                    }
                }
                else
                {//账目信息未发生变动
                    MessageBox.Show(null, "账目各项信息没有发生变动！", "修改失败");
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
        private void AccountModify_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Modified_OK == true)//若已成功修改
                this.DialogResult = DialogResult.OK;//向商品信息管理页面传递OK信息
            else
                this.DialogResult = DialogResult.No;//向商品信息管理页面传递No信息
        }
    }
}
