using System;

namespace RBVH.Stada.Intranet.Biz.Models
{
    public class ShiftTime : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public DateTime WorkingHourFromHour { get; set; }
        public DateTime WorkingHourToHour { get; set; }
        public DateTime? WorkingHourMidHour { get; set; }
        public DateTime BreakHourFromHour { get; set; }
        public DateTime BreakHourToHour { get; set; }
        public double ShiftTimeWorkingHourNumber { get; set; }
        public double ShiftTimeBreakHourNumber { get; set; }
        public LookupItem UnexpectedLeaveFirstApprovalRole { get; set; }
        public bool ShiftRequired { get; set; }
        public string Description { get; set; }

        public ShiftTime()
        {
            UnexpectedLeaveFirstApprovalRole = new LookupItem();
        }
    }
}
