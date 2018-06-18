using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.WebPages.Models
{
   public class EmployeeShiftTime
    {
        public int EmployeeLookupId { get; set; }
        public DateTime Date { get; set; }
        public int ShiftLookupId { get; set; }
        public int EmployeeShiftTimeID { get; set; }
        public string MonthYear { get; set; }
        public bool IsValid { get; set; }
    }
}
