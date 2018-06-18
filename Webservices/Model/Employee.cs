using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    [Serializable]
    public class Employee
    {

        public Employee()
        {
            ListEmployees = new List<EmployeeInfo>();
        }

        // public SPUser SpCurrentUser { get; set; } 
        public EmployeeInfo CurrentEmployee { get; set; } //Store Session
        public bool IsAdmin { get; set; }
        public List<EmployeeInfo> ListEmployees { get; set; }
    }
}
