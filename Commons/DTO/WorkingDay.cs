using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class WorkingDay
    {
        public string DateStr { get; set; }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; DateStr = string.Format("{0:s}", value); }
        }

        public double LeaveHours { get; set; }
        public bool IsDefaultShift { get; set; }
        public ShiftTime Shift { get; set; }
        //public OverTimeManagementDetail OverTime { get; set; }

        public WorkingDay()
        {
            this.IsDefaultShift = true;
            this.LeaveHours = 0;
        }
    }
}
