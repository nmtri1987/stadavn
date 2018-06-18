using System;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    [Serializable]
    public class Employee
    {
        //public int LookupId { get; set; }

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