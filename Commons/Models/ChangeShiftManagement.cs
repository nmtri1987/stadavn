using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/ChangeShiftManagement")]
    public class ChangeShiftManagement : EntityBase
    {
        [ListColumn(StringConstant.CommonSPListField.RequesterField)]
        public LookupItem Requester { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem Department { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonLocationField)]
        public LookupItem Location { get; set; }
        [ListColumn(StringConstant.ChangeShiftList.FromDateField)]
        public DateTime FromDate { get; set; }
        [ListColumn(StringConstant.ChangeShiftList.FromShiftField)]
        public LookupItem FromShift { get; set; }
        [ListColumn(StringConstant.ChangeShiftList.ToDateField)]
        public DateTime ToDate { get; set; }
        [ListColumn(StringConstant.ChangeShiftList.ToShiftField)]
        public LookupItem ToShift { get; set; }
        [ListColumn(StringConstant.ChangeShiftList.ReasonField)]
        public string Reason { get; set; }
        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public string ApprovalStatus { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonCommentField)]
        public string Comment { get; set; }
        [ListColumn(StringConstant.ChangeShiftList.DHField)]
        public User DepartmentHead { get; set; }
        [ListColumn(StringConstant.ChangeShiftList.BODField)]
        public User BOD { get; set; }
        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }
    }
}
