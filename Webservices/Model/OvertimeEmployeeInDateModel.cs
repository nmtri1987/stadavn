using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
   public class OvertimeEmployeeInDateModel
    {
        public bool IsHasValue { get; set; }
        public int FromHour { get; set; }
        public int FromMinute { get; set; }
        public int ToHour { get; set; }
        public int ToMinute { get; set; }
        public double HourPerDay { get; set; }
        public string ApprovalStatus { get; set; }


    }
}
