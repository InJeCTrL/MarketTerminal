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
    public partial class AccountMgr : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        private DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 账目列表
        /// </summary>
        List<String[]> AccountList;
        /// <summary> 标记是否进行过删改
        /// </summary>
        private Boolean Modified = false;
        /// <summary> 初始化账目管理器
        /// </summary>
        public AccountMgr()
        {
            InitializeComponent();
            Flush();//填充账目列表
        }
        /// <summary> 展开右键菜单后判断是否允许修改与删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1 || listView1.FocusedItem.Index < 0 || listView1.FocusedItem.Index >= AccountList.Count)
            {//若没有选中有效账目
                修改ToolStripMenuItem.Enabled = false;//禁止修改
                删除ToolStripMenuItem.Enabled = false;//禁止删除
            }
            else//已选中有效账目
            {
                修改ToolStripMenuItem.Enabled = true;//允许修改
                删除ToolStripMenuItem.Enabled = true;//允许删除
            }
        }
        /// <summary> 重新获取账目列表，并刷新listview
        /// </summary>
        /// <param name="QueryMode">查询模式</param>
        /// /// <param name="Query">查询串</param>
        private void Flush(int QueryMode = -1,String Query = "")
        {
            listView1.Items.Clear();//清空账目列表
            AccountList = DBMgr.GetAccountList();//重新获取账目列表
            for (int i = 0; i < AccountList.Count; i++)//插入所有记录到listview
            {
                switch (QueryMode)
                {
                    case 0://按流水编号搜索
                        if (AccountList.ElementAt(i)[0].Contains(Query))//模糊查找
                            listView1.Items.Add(new ListViewItem(AccountList.ElementAt(i)));//单行插入
                        break;
                    case 1://按商品编号搜索
                        if (AccountList.ElementAt(i)[1].Contains(Query))//模糊查找
                            listView1.Items.Add(new ListViewItem(AccountList.ElementAt(i)));//单行插入
                        break;
                    case 2://按商品名称搜索
                        if (AccountList.ElementAt(i)[2].Contains(Query))//模糊查找
                            listView1.Items.Add(new ListViewItem(AccountList.ElementAt(i)));//单行插入
                        break;
                    default://未指定
                        listView1.Items.Add(new ListViewItem(AccountList.ElementAt(i)));//单行插入
                        break;
                }
            }
        }
        /// <summary> 修改选中项账目信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountModify AccountModify_frm = new AccountModify(new String[]{listView1.FocusedItem.SubItems[0].Text,
                                                                       listView1.FocusedItem.SubItems[1].Text,
                                                                       listView1.FocusedItem.SubItems[2].Text,
                                                                       listView1.FocusedItem.SubItems[3].Text,
                                                                       listView1.FocusedItem.SubItems[4].Text,
                                                                       listView1.FocusedItem.SubItems[5].Text,
                                                                       listView1.FocusedItem.SubItems[6].Text,
                                                                       listView1.FocusedItem.SubItems[7].Text,});//实例化信息修改窗体
            if (AccountModify_frm.ShowDialog() == DialogResult.OK)//模态显示信息修改窗体
            {//账目信息被修改过
                Flush();//刷新账目列表
                Modified = true;//标记增删改
            }
        }
        /// <summary> 删除选中项账目信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(null, "请确认是否删除该账目，该操作无法回滚！", "删除确认", MessageBoxButtons.YesNo))
            {
                if (DBMgr.DeleteAccount(listView1.FocusedItem.SubItems[0].Text) == true)
                {
                    MessageBox.Show(null, "账目删除成功！", "删除成功");
                    Flush();//刷新账目列表
                    Modified = true;//标记增删改
                }
                else
                    MessageBox.Show(null, "账目删除失败！", "删除失败");
            }
        }
        /// <summary> 商品管理窗体关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountMgr_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Modified == true)
            {//有过增删改
                this.DialogResult = DialogResult.Yes;//传回进行过增删改
            }
            else
                this.DialogResult = DialogResult.No;//传回未进行增删改
        }
        /// <summary> 模糊查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;//选定项
            if (index < 0)//未指定查询方式
                MessageBox.Show(null, "请选择查询方式！", "查询失败");//提示查询失败
            else
                Flush(index, textBox1.Text);//按模式查询
        }
    }
}
