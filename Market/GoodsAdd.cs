using System;
using System.Windows.Forms;

namespace Market
{
    public partial class GoodsAdd : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 标记是否预新增过商品信息
        /// </summary>
        private Boolean Added = false;
        /// <summary> 标记是否成功新增了商品
        /// </summary>
        private Boolean Added_OK = false;
        /// <summary> 新增一类商品变为添加商品个数
        /// </summary>
        private Boolean NewToAdd = false;
        /// <summary> 初始化商品新增，默认为手动输入
        /// </summary>
        /// <param name="UserMode">是否用户手动添加</param>
        /// <param name="_GoodsInfo">预添加的商品信息</param>
        /// /// <param name="New">是否是新添加的一类商品</param>
        public GoodsAdd(String[] _GoodsInfo = null,Boolean UserMode = true,Boolean New = true)
        {
            InitializeComponent();
            if (UserMode == false)
            {//扫码输入
                textBox1.Enabled = false;//禁止修改商品编号
                textBox1.Text = _GoodsInfo[0];//商品编号
                if (New == false)//不是新添加的一类商品
                {
                    textBox2.Enabled = false;//禁止修改商品名
                    textBox3.Enabled = false;//禁止修改进价
                    textBox4.Enabled = false;//禁止修改售价
                    textBox5.Enabled = false;//禁止修改品牌
                    textBox6.Enabled = false;//禁止修改单位
                    NewToAdd = true;//改为只是添加商品个数
                }
                textBox2.Text = _GoodsInfo[1];//商品名
                textBox3.Text = _GoodsInfo[2];//进价
                textBox4.Text = _GoodsInfo[3];//售价
                textBox5.Text = _GoodsInfo[6];//品牌
                textBox6.Text = _GoodsInfo[7];//单位
            }
        }
        /// <summary> 提交新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox2.Text.Equals("") ||
                textBox3.Text.Equals("") || textBox4.Text.Equals("") ||
                textBox5.Text.Equals("") || textBox6.Text.Equals("") ||
                textBox7.Text.Equals(""))
            {//有项未填
                MessageBox.Show(null, "所有信息必须完整，请重新填写！", "新增失败");
            }
            else
            {//全部填写
                int outNum;//textbox7转换为整数
                if (int.TryParse(textBox7.Text,out outNum))
                {//若数量为有效数字
                    if (DBMgr.IsGoodsExists(textBox1.Text))
                    {//商品存在
                        if (NewToAdd == true)//扫码形式的数量添加
                        {
                            if (DBMgr.UpdateGoodsNum(textBox1.Text, outNum))//数量增加
                            {
                                Added_OK = true;//标记已成功新增
                                MessageBox.Show(null, "商品添加成功！", "新增成功");
                                this.Close();//关闭新增窗体
                            }
                            else
                                MessageBox.Show(null, "商品添加失败！", "新增失败");
                        }
                        else
                        {//禁止手动添加重复商品
                            MessageBox.Show(null, "禁止手动添加已存在商品，请使用修改功能！", "新增失败");
                        }
                    }
                    else
                    {//商品不存在
                        if (DBMgr.AddGoods(new String[]{textBox1.Text,//商品编号
                                                        textBox2.Text,//商品名称
                                                        textBox3.Text,//进价
                                                        textBox4.Text,//售价
                                                        "0",//货架存量
                                                        textBox7.Text,//库存
                                                        textBox5.Text,//品牌
                                                        textBox6.Text}))//单位
                        {
                            Added_OK = true;//标记已成功新增
                            MessageBox.Show(null, "商品添加成功！", "新增成功");
                            this.Close();//关闭新增窗体
                        }
                        else
                            MessageBox.Show(null, "商品添加失败，请检查数据输入格式！", "新增失败");
                    }
                }
                else
                {//数量输入无效
                    MessageBox.Show(null, "数量为纯数字，请重新输入！", "新增失败");
                }
            }
        }
        /// <summary> 关闭新增员工窗体后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoodsAdd_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Added_OK == true)//若已成功新增
                this.DialogResult = DialogResult.OK;//向商品信息管理页面传递OK信息
            else
                this.DialogResult = DialogResult.No;//向商品信息管理页面传递No信息
        }
        /// <summary> 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals("") || !textBox2.Text.Equals("") || !textBox3.Text.Equals("") ||
                !textBox4.Text.Equals("") || !textBox5.Text.Equals("") || !textBox6.Text.Equals(""))
                Added = true;//有填写过，覆盖预新增为true
            else
                Added = false;//都没填写过
            if (Added == true)
            {
                if (DialogResult.Yes == MessageBox.Show(null, "当前已填写新增参数，是否放弃新增？", "撤销提示", MessageBoxButtons.YesNo))
                    this.Close();//关闭新增商品窗体
            }
        }
    }
}
