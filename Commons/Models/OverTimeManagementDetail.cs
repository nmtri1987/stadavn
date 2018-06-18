using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.ComponentModel;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/OvertimeEmployeeDetails")]
    public class OverTimeManagementDetail : EntityBase
    {
        [ListColumn(StringConstant.OverTimeManagementDetailList.Employee)]
        public LookupItem Employee { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.EmployeeID, true)]
        public LookupItem EmployeeID { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.OvertimeFrom)]
        public DateTime OvertimeFrom { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.OvertimeTo)]
        public DateTime OvertimeTo { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.OvertimeManagementID)]
        public LookupItem OvertimeManagementID { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.KD)]
        public string KD { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.HM)]
        public string HM { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.CompanyTransport)]
        public string CompanyTransport { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.Task)]
        public string Task { get; set; }
        [ListColumn(StringConstant.OverTimeManagementDetailList.SummaryLinks)]
        public string SummaryLinks { get; set; }

        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public LookupItem ApprovalStatus { get; set; }

        [ListColumn(StringConstant.OverTimeManagementDetailList.WorkingHours)]
        public double WorkingHours { get; set; }

        public string OvertimeHours { get { return string.Format("{0}h-{1}h", OvertimeFrom.ToString("HH:mm"), OvertimeTo.ToString("HH:mm")); } }

        public OverTimeManagementDetail()
        {
            Employee = new LookupItem();
            EmployeeID = new LookupItem();
            OvertimeManagementID = new LookupItem();
            ApprovalStatus = new LookupItem();
        }
    }
}
