using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class OvertimeManagement
    {
        public  int RequesterId { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int SumEmployee { get; set; }
        public int SumMeal { get; set; }
        public string OtherRequirement { get; set; }
    }
}
