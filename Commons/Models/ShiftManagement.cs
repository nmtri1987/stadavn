
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/ShiftManagement")]
    public class ShiftManagement : EntityBase
    {
        [ListColumn(StringConstant.CommonSPListField.RequesterField)]
        public LookupItem Requester { get; set; }

        [ListColumn(StringConstant.ShiftManagementList.MonthField)]
        public int Month { get; set; }

        [ListColumn(StringConstant.ShiftManagementList.YearField)]
        public int Year { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem Department { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonLocationField)]
        public LookupItem Location { get; set; }

        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public string ApprovalStatus { get; set; }

        [ListColumn(StringConstant.ShiftManagementList.ApprovedByField)]
        public User ApprovedBy { get; set; }

        [ListColumn(StringConstant.ShiftManagementList.CommonAddApprover1Field)]
        public List<User> CommonAddApprover1 { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedByField)]
        public User ModifiedBy { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }

        public ShiftManagement()
        {
            Requester = new LookupItem();
            CommonAddApprover1 = new List<User>();
            ApprovalStatus = string.Empty;
            Department = new LookupItem();
            Location = new LookupItem();
            ApprovedBy = new User();
            ModifiedBy = new User();
        }
    }
}
