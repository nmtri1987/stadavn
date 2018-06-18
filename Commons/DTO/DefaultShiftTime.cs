using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class DefaultShiftTime
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string WorkingHourFromHour { get; set; }
        public string WorkingHourToHour { get; set; }
        public string BreakHourFromHour { get; set; }
        public string BreakHourToHour { get; set; }
        public double ShiftTimeWorkingHourNumber { get; set; }
        public double ShiftTimeBreakHourNumber { get; set; }
        public LookupItem UnexpectedLeaveFirstApprovalRole { get; set; }
    }
}
