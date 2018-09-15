using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Market
{
    public partial class StaffMgr : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 员工列表
        /// </summary>
        List<String[]> StaffList;
        /// <summary> 标记员工信息被增删改过
        /// </summary>
        private Boolean Modified = false;
        /// <summary> 初始化员工管理窗体
        /// </summary>
        public StaffMgr()
        {
            InitializeComponent();
            Flush();//填充员工列表
        }
        /// <summary> 展开右键菜单后判断是否允许修改与删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1 || listView1.FocusedItem.Index < 0 || listView1.FocusedItem.Index >= StaffList.Count)
            {//若没有选中有效员工项
                修改ToolStripMenuItem.Enabled = false;//禁止修改
                删除ToolStripMenuItem.Enabled = false;//禁止删除
            }
            else//已选中有效员工项
            {
                修改ToolStripMenuItem.Enabled = true;//允许修改
                删除ToolStripMenuItem.Enabled = true;//允许删除
            }
        }
        /// <summary> 重新获取员工列表，并刷新listview
        /// </summary>
        private void Flush()
        {
            int SU_Num = 0;//统计超级管理员数量
            listView1.Items.Clear();//清空员工列表
            StaffList = DBMgr.GetStaffList();//重新获取员工列表
            for (int i = 0; i < StaffList.Count; i++)//插入所有记录到listview
            {
                if (StaffList.ElementAt(i)[2].Equals("是"))
                    SU_Num++;//识别为超级管理员，统计+1
                listView1.Items.Add(new ListViewItem(StaffList.ElementAt(i)));//单行插入
            }
            if (StaffList.Count == 0)
            {//若当前系统中没有员工
                DBMgr.AddStaff(new String[]{"admin","DefaultSuperUser","是","123"});//新增默认SU
                MessageBox.Show(null, "系统必须至少有一个超级管理员以启动并管理！\n已自动添加默认超级管理员账户", "员工检测");
                Flush();//重新刷新
            }
            else if (SU_Num == 0)
            {//若当前系统中没有超级管理员
                DBMgr.UpdateStaffPower(StaffList.ElementAt(0)[0], true);//第一个员工设为SU
                MessageBox.Show(null, "系统必须至少有一个超级管理员以启动并管理！\n已自动将列表中第一位员工设置为超级管理员", "员工检测");
                Flush();//重新刷新
            }
        }
        /// <summary> 修改选中项员工信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StaffModify StaffModify_frm = new StaffModify(new String[]{listView1.FocusedItem.SubItems[0].Text,
                                                                       listView1.FocusedItem.SubItems[1].Text,
                                                                       listView1.FocusedItem.SubItems[2].Text,
                                                                       listView1.FocusedItem.SubItems[3].Text,});//实例化信息修改窗体
            if (StaffModify_frm.ShowDialog() == DialogResult.OK)//模态显示信息修改窗体
            {//员工信息被修改过
                Flush();//刷新员工列表
                Modified = true;//标记增删改
            }
        }
        /// <summary> 删除选中项员工信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(null, "请确认是否删除该员工，该操作无法回滚！", "删除确认", MessageBoxButtons.YesNo))
            {
                if (DBMgr.DeleteStaff(listView1.FocusedItem.SubItems[0].Text) == true)
                {
                    MessageBox.Show(null, "员工删除成功！", "删除成功");
                    Flush();//刷新员工列表
                    Modified = true;//标记增删改
                }
                else
                    MessageBox.Show(null, "员工删除失败！", "删除失败");
            }
        }
        /// <summary> 增加员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 增加ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StaffAdd StaffAdd_frm = new StaffAdd();//实例化新增员工类
            if (StaffAdd_frm.ShowDialog() == DialogResult.OK)//模态显示新增员工窗体
            {//成功新增
                Flush();//刷新员工列表
                Modified = true;//标记增删改
            }
        }
        /// <summary> 员工管理窗体关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StaffMgr_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Modified == true)
            {//有过增删改
                this.DialogResult = DialogResult.Yes;//传回进行过增删改
            }
            else
                this.DialogResult = DialogResult.No;//传回未进行增删改
        }
    }
}
