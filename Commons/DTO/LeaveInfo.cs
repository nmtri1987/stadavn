using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class LeaveInfo
    {
        public int LeaveManagementId { get; set; }
        public int Day { get; set; }
        public bool AllDay { get; set; }
        public string ItemUrl { get; set; }
    }
}
