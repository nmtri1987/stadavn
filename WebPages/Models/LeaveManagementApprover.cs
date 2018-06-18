using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class LeaveManagementApprover
    {
        public List<EmployeeInfo> ApproverOneList { get; set; }
        public int ApproverOneSelectedIndex { get; set; }
        public List<EmployeeInfo> ApproverTwoList { get; set; }
        public int ApproverTwoSelectedIndex { get; set; }
        public List<EmployeeInfo> ApproverThreeList { get; set; }
        public int ApproverThreeSelectedIndex { get; set; }
        public CodeMessageResult CodeMessageResult { get; set; }

        public List<EmployeeInfo> OptionalApproverOneList { get; set; }
        public List<EmployeeInfo> OptionalApproverTwoList { get; set; }

        public LeaveManagementApprover()
        {
            ApproverOneList = new List<EmployeeInfo>();
            ApproverTwoList = new List<EmployeeInfo>();
            ApproverThreeList = new List<EmployeeInfo>();
            ApproverOneSelectedIndex = 0;
            ApproverTwoSelectedIndex = 0;
            ApproverThreeSelectedIndex = 0;
            CodeMessageResult = new CodeMessageResult();
            OptionalApproverOneList = new List<EmployeeInfo>();
            OptionalApproverTwoList = new List<EmployeeInfo>();
        }
    
    
}
}
