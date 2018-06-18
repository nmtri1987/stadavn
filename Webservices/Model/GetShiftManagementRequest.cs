using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class GetShiftManagementRequest
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public string DepartmentId { get; set; }
        public string LocationId { get; set; }
    }
}
