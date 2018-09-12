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
    public partial class StaffModify : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 员工信息
        /// </summary>
        private String[] StaffInfo;
        /// <summary> 标记是否预修改过员工信息
        /// </summary>
        private Boolean Modified = false;
        /// <summary> 标记是否成功修改了员工信息
        /// </summary>
        private Boolean Modified_OK = false;
        /// <summary> 初始化员工信息修改窗体
        /// </summary>
        /// <param name="_StaffInfo">员工原信息</param>
        public StaffModify(String[] _StaffInfo)
        {
            InitializeComponent();
            StaffInfo = _StaffInfo;//获取员工原信息
            textBox1.Text = StaffInfo[0];//显示员工工号
            textBox2.Text = StaffInfo[1];//显示员工姓名
            textBox3.Text = StaffInfo[3];//显示员工密码
            if (StaffInfo[2].Equals("是"))
                checkBox1.Checked = true;//显示超级管理员状态
        }
        /// <summary> 提交修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(StaffInfo[1]))//若员工姓名发生变动
            {
                Modified = true;//员工信息已被预修改
                if (DBMgr.UpdateStaffName(StaffInfo[0], textBox2.Text) == false)
                    MessageBox.Show(null, "员工姓名修改失败！", "修改失败");
                else
                    Modified_OK = true;//更新修改成功标记
            }
            if (!textBox3.Text.Equals(StaffInfo[3]))//若员工密码发生变动
            {
                Modified = true;//员工信息已被预修改
                if (DBMgr.UpdateStaffPwd(StaffInfo[0], textBox3.Text) == false)
                    MessageBox.Show(null, "员工密码修改失败！", "修改失败");
                else
                    Modified_OK = true;//更新修改成功标记
            }
            if ((checkBox1.Checked == true && !StaffInfo[2].Equals("是")) ||
                (checkBox1.Checked == false && !StaffInfo[2].Equals("否")))//若超级管理员状态发生变动
            {
                Modified = true;//员工信息已被预修改
                if (DBMgr.UpdateStaffPower(StaffInfo[0],checkBox1.Checked) == false)
                    MessageBox.Show(null, "员工SU状态修改失败！", "修改失败");
                else
                    Modified_OK = true;//更新修改成功标记
            }
            if (Modified == false)
            {
                MessageBox.Show(null, "员工各项信息没有发生变动！", "修改失败");
                this.Close();//关闭修改窗体
            }
            if (Modified_OK == true)
            {
                MessageBox.Show(null, "员工各项信息修改成功！", "修改成功");
                this.Close();//关闭修改窗体
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
        private void StaffModify_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Modified_OK == true)//若已成功修改
                this.DialogResult = DialogResult.OK;//向员工信息管理页面传递OK信息
            else
                this.DialogResult = DialogResult.No;//向员工信息管理页面传递No信息
        }
    }
}
