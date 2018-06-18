using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class LeaveResult
    {
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        public string EmployeeID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DefaultShiftTime DefaultShiftTime { get; set; }
        public List<NoneWorkingDay> NoneWorkingDays { get; set; }
        public List<WorkingDay> WorkingDays { get; set; }
        private double totalHourVal = 0;
        public double TotalHours
        {
            get { return totalHourVal; }
            set
            {
                totalHourVal = value;
                TimeSpan interval = TimeSpan.FromHours(value);
                TotalHoursDisp = string.Format("{0:00}:{1:00}", (interval.Days * 24 + interval.Hours), interval.ToString("mm"));
                //TotalHoursDisp = string.Format("{0:00}:{1:00}", (int)interval.TotalHours, interval.Minutes);
            }
        }
        public string TotalHoursDisp { get; set; }
        public double TotalDays { get; set; }
        public bool UnexpectedLeave { get; set; }

        public LeaveResult() {
            NoneWorkingDays = new List<NoneWorkingDay>();
            WorkingDays = new List<WorkingDay>();
            UnexpectedLeave = false;
        }

        public LeaveResult(string employeeID, string fromDate, string toDate) : this()
        {
            this.EmployeeID = employeeID;
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.From = Convert.ToDateTime(fromDate);
            this.To = Convert.ToDateTime(toDate);
        }
    }
}
