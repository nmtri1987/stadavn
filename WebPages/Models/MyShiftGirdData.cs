using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class MyShiftGirdData
    {
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public List<Dictionary<string, string>> MyShiftListData { get; set; }

        public MyShiftGirdData()
        {
            MyShiftListData = new List<Dictionary<string, string>>();
        }
    }
}
