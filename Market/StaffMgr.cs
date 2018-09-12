using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OracleClient;

namespace Market
{
    public partial class StaffMgr : Form
    {
        /// <summary> 实例化数据库管理器
        /// </summary>
        DataBaseManager DBMgr = new DataBaseManager();
        /// <summary> 初始化员工管理窗体
        /// </summary>
        public StaffMgr()
        {
            InitializeComponent();
            List<String[]> StaffList = DBMgr.GetStaffList();//生成员工列表
            for (int i=0;i<StaffList.Count;i++)//插入所有记录到listview
                listView1.Items.Add(new ListViewItem(StaffList.ElementAt(i)));//单行插入
        }
    }
}
