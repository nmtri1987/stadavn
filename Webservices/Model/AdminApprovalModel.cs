using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class AdminApprovalModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public List<string> EmployeeNameList { get; set; }
        public int ShiftManagementId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public string ModifiedBy {get;set;}
        public string ApproverFullName { get; set; }
        public AdminApprovalModel()
        {
            EmployeeNameList = new List<string>();
        }
    }
}
