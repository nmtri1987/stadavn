using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class OvertimePage
    {
        public Employee RequesterInfo { get; set; }
        public int SelectedRequesterId { get; set; }
        public List<Factory> FactoryList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public int SelectedFactoryId { get; set; }
        public int SelectedDepartmentId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string OtherRequirement { get; set; }
        public int SumOfMeal { get; set; }
        public int SumOfEmployee { get; set; }
        public CodeMessageResult CodeMessageResult { get; set; }
        public OvertimePage()
        {
            RequesterInfo = new Employee();
            FactoryList = new List<Factory>();
            SumOfEmployee = 0;
            SumOfMeal = 0;
            CodeMessageResult  = new CodeMessageResult();
            DepartmentList = new List<Department>();
        }
        
    }
}
