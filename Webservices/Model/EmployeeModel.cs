using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
   public class EmployeeModel
    {  
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public LookupItem Department { get; set; }
        public LookupItem Location { get; set; }
        public LookupItem EmployeeLevel { get; set; }
        public string FullName { get; set; }

        public string DepartmentPermission { get; set; }
        public string EmployeeType { get; set; }
        public EmployeeModel()
        {
            EmployeeLevel = new LookupItem();
        }
    }
}
