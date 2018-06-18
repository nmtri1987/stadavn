
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class ShiftTimeModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int WorkingHourFromHour { get; set; }
        public int WorkingHourFromMinute { get; set; }
        public int WorkingHourToHour { get; set; }
        public int WorkingHourToMinute { get; set; }
        public string BreakHourFromHour { get; set; }
        public string BreakHourFromMinute { get; set; }
        public string BreakHourToHour { get; set; }
        public string BreakHourToMinute { get; set; }
        public double ShiftTimeWorkingHourNumber { get; set; }
        public double ShiftTimeBreakHourNumber { get; set; }
        public string Depscription { get; set; }
        public LookupItem UnexpectedLeaveFirstApprovalRole { get; set; }
        public bool ShiftRequired { get; set; }
    }
}
