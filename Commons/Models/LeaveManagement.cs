using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/LeaveManagement")]
    public class LeaveManagement : EntityBase
    {
        [ListColumn(StringConstant.CommonSPListField.RequesterField)]
        public LookupItem Requester { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.RequestForField)]
        public LookupItem RequestFor { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem Department { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonLocationField)]
        public LookupItem Location { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.FromField)]
        public DateTime From { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.ToField)]
        public DateTime To { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.LeaveHoursField)]
        public double LeaveHours { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.ReasonField)]
        public string Reason { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.TransferworkToField)]
        public LookupItem TransferworkTo { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.LeftAtField)]
        public DateTime LeftAt { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.LeftField)]
        public bool Left { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.UnexpectedLeaveField)]
        public bool UnexpectedLeave { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.TLEField)]
        public User TLE { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.DHField)]
        public User DH { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.BODField)]
        public User BOD { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonCommentField)]
        public string Comment { get; set; }
        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public string ApprovalStatus { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.IsValidRequestField)]
        public bool IsValidRequest { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.TotalDaysField)]
        public double TotalDays { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public User Approver { get; set; }
        [ListColumn(StringConstant.LeaveManagementList.AdditionalApproverField)]
        public List<User> AdditionalUser { get; set; }
        [ListColumn(StringConstant.DefaultSPListField.CreatedByField)]
        public User CreatedBy { get; set; }
        [ListColumn(StringConstant.DefaultSPListField.ModifiedByField)]
        public User ModifiedBy { get; set; }
        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }

        public LeaveManagement()
        {
            Requester = new LookupItem();
            RequestFor = new LookupItem();
            TransferworkTo = new LookupItem();
            ApprovalStatus = string.Empty;
            Department = new LookupItem();
            Location = new LookupItem();
            TLE = new User();
            DH = new User();
            BOD = new User();
            Approver = new User();
            AdditionalUser = new List<User>();
            CreatedBy = new User();
            ModifiedBy = new User();
        }
    }
}
