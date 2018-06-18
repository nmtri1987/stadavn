using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
   public class CurrentUserModel
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public LookupItem Department { get; set; }
        public LookupItem Location { get; set; }
        public string FullName { get; set; }
        public string DepartmentPermission { get; set; }
        public string EmployeeType { get; set; }
        public int EmployeePosition { get; set; }
        public bool IsSystemAdmin { get; set; }
        public bool IsBODApprovalRequired { get; set; }
    }
}
