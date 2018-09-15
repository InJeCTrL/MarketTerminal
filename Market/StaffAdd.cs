using System;
using System.Windows.Forms;

namespace Market
{
    public partial class StaffAdd : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 标记是否预新增过员工信息
        /// </summary>
        private Boolean Added = false;
        /// <summary> 标记是否成功新增了员工
        /// </summary>
        private Boolean Added_OK = false;
        /// <summary> 初始化新增员工窗体
        /// </summary>
        public StaffAdd()
        {
            InitializeComponent();
        }
        /// <summary> 提交新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {//员工工号为空
                MessageBox.Show(null, "员工工号必填，请重新填写！", "新增失败");
            }
            else
            {//员工工号非空
                if (textBox2.Text.Equals(""))
                {//员工姓名为空
                    MessageBox.Show(null, "员工姓名必填，请重新填写！", "新增失败");
                }
                else
                {//员工姓名非空
                    if (DBMgr.GetStaff(textBox1.Text) == null)
                    {//员工号未重复
                        if (DBMgr.AddStaff(new String[] {textBox1.Text,textBox2.Text,
                                                 checkBox1.Checked == true?"是":"否",
                                                 textBox3.Text}))//尝试新增员工
                        {//新增成功
                            Added_OK = true;//标记已成功新增
                            MessageBox.Show(null, "员工添加成功！", "新增成功");
                            this.Close();//关闭新增窗体
                        }
                        else
                            MessageBox.Show(null, "员工添加失败！", "新增失败");
                    }
                    else
                    {//员工号重复
                        MessageBox.Show(null, "员工工号重复，请重新填写！", "新增失败");
                    }
                }
            }
        }
        /// <summary> 关闭新增员工窗体后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StaffAdd_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Added_OK == true)//若已成功新增
                this.DialogResult = DialogResult.OK;//向员工信息管理页面传递OK信息
            else
                this.DialogResult = DialogResult.No;//向员工信息管理页面传递No信息
        }
        /// <summary> 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals("") || !textBox2.Text.Equals("") || !textBox3.Text.Equals(""))
                Added = true;//三项有填写过，覆盖预新增为true
            else
                Added = false;//三项都没填写过
            if (Added == true)
            {
                if (DialogResult.Yes == MessageBox.Show(null, "当前已填写新增参数，是否放弃新增？", "撤销提示", MessageBoxButtons.YesNo))
                    this.Close();//关闭新增员工窗体
            }
        }
    }
}
