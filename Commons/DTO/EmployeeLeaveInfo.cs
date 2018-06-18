using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class EmployeeLeaveInfo
    {
        public int EmployeeId { get; set; }
        public List<LeaveInfo> Leaves { get; set; }
    }
}
