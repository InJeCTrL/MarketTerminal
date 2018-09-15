using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Market
{
    public partial class BadLogView : Form
    {
        /// <summary> 初始化告警日志，直接填充列表
        /// </summary>
        public BadLogView(List<String[]> BadMarketLeft,List<String[]> BadTrunkLeft)
        {
            InitializeComponent();
            if (BadMarketLeft != null)//若列表正常传入
            {
                for (int i = 0; i < BadMarketLeft.Count; i++)//全部导入
                    listView1.Items.Add(new ListViewItem(new String[] { BadMarketLeft.ElementAt(i)[0], BadMarketLeft.ElementAt(i)[1], "超市货架存量不足" }));
            }
            if (BadTrunkLeft != null)//若列表正常传入
            {
                for (int i = 0; i < BadTrunkLeft.Count; i++)//全部导入
                    listView1.Items.Add(new ListViewItem(new String[] { BadTrunkLeft.ElementAt(i)[0], BadTrunkLeft.ElementAt(i)[1], "仓库库存存量不足" }));
            }
        }
    }
}
