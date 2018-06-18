using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    [Serializable]
    public class ShiftRequestGridData
    {
        
        public string EmployeeIdOption { get; set; }
        public string ShiftTimeOption { get; set; }
        
        public List<Dictionary<string,string>> ShiftRequestDataList { get; set; }

        
        public ShiftRequestGridData()
        {
            ShiftRequestDataList = new List<Dictionary<string, string>>();
        }
    }
}
