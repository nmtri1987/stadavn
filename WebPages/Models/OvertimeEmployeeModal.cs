using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class OvertimeEmployeeModal
    {
        public List<EmployeeInfo> EmployeeInfoList { get; set; }
        public int WorkingHour { get; set; }
        public int OvertimeFrom { get; set; }
        public int OvertimeTo { get; set; }
        public string Task { get; set; }
        public List<DropdownItem> TransportList { get; set; }

        public OvertimeEmployeeModal()
        {
            EmployeeInfoList = new List<EmployeeInfo>();
            TransportList = new List<DropdownItem>();
            WorkingHour = 8;
            CodeMessageResult = new CodeMessageResult();
        }

        //Just Result information status
        public CodeMessageResult CodeMessageResult { get; set; }
    }
   
}
