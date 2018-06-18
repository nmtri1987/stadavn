using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/NotOverTimeManagement")]
    public class NotOvertimeManagement : EntityBase
    {
        [ListColumn(StringConstant.CommonSPListField.RequesterField)]
        public LookupItem Requester { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem Department { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonLocationField)]
        public LookupItem Location { get; set; }
        [ListColumn(StringConstant.NotOvertimeList.HourPerDayField)]
        public double HourPerDay { get; set; }
        [ListColumn(StringConstant.NotOvertimeList.DateField)]
        public DateTime Date { get; set; }
        [ListColumn(StringConstant.NotOvertimeList.FromDateField)]
        public DateTime FromDate { get; set; }
        [ListColumn(StringConstant.NotOvertimeList.ToDateField)]
        public DateTime ToDate { get; set; }
        [ListColumn(StringConstant.NotOvertimeList.DHField)]
        public User DH { get; set; }
        [ListColumn(StringConstant.NotOvertimeList.BODField)]
        public User BOD { get; set; }
        [ListColumn(StringConstant.NotOvertimeList.ReasonField)]
        public string Reason { get; set; }
        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public string ApprovalStatus { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonCommentField)]
        public string Comment { get; set; }
        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }
    }
}
