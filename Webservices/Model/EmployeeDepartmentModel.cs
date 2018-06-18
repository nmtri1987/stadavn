using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
   public  class EmployeeDepartmentModel
    {
        public int ID { get; set; }
        public string EmployeeId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
    }

}
