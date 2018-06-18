using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class LeaveTimeModel
    {
        public double TotalHours { get; set; }
        public List<LeaveShiftTimeModel> ShiftTimeList { get; set; }

        public LeaveTimeModel()
        {
            ShiftTimeList = new List<LeaveShiftTimeModel>();
        }
    }
}
