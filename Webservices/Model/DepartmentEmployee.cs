using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;


namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class DepartmentEmployee
    {
         public  List<EmployeeInfo> EmployeeList { get; set; }
        
        public DepartmentEmployee()
        {
            EmployeeList = new List<EmployeeInfo>();
        }
    }
}
