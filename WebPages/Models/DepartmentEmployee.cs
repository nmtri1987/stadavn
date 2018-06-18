using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public   class DepartmentEmployee
    {
        public  List<EmployeeInfo> EmployeeList { get; set; }
        public CodeMessageResult CodeMessageResult { get; set; }

        public DepartmentEmployee()
        {
            CodeMessageResult = new CodeMessageResult();
            EmployeeList = new List<EmployeeInfo>();
        }
    }
}
