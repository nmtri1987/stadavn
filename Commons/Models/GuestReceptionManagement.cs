using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.GuestReceptionManagementList.Url)]
    public class GuestReceptionManagement : EntityBase
    {
        public GuestReceptionManagement() { }

        [ListColumn(StringConstant.GuestReceptionManagementList.Fields.Title)]
        public string Title { get; set; }

        public DateTime CommonFrom { get; set; }

        [ListColumn(ApprovalFields.Creator)]
        public LookupItem CommonCreator { get; set; }

        [ListColumn(ApprovalFields.Status)]
        public string ApprovalStatus { get; set; }

        [ListColumn(ApprovalFields.WFStatus)]
        public string WFStatus { get; set; }

        [ListColumn(ApprovalFields.PendingAt)]
        public List<LookupItem> PendingAt { get; set; }

        [ListColumn(ApprovalFields.CurrentStep)]
        public LookupItem CurrentStep { get; set; }

        [ListColumn(ApprovalFields.NextStep)]
        public LookupItem NextStep { get; set; }

        [ListColumn(ApprovalFields.CommonLocation)]
        public LookupItem CommonLocation { get; set; }

        [ListColumn(ApprovalFields.CommonDepartment)]
        public LookupItem CommonDepartment { get; set; }

        [ListColumn(ApprovalFields.IsAdditionalStep)]
        public bool IsAdditionalStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalPreviousStep)]
        public LookupItem AdditionalPreviousStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalStep)]
        public LookupItem AdditionalStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalNextStep)]
        public LookupItem AdditionalNextStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalDepartment)]
        public LookupItem AdditionalDepartment { get; set; }

        [ListColumn(ApprovalFields.AssignFrom)]
        public LookupItem AssignFrom { get; set; }

        [ListColumn(ApprovalFields.AssignTo)]
        public LookupItem AssignTo { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedField)]
        public DateTime Modified { get; set; }
    }
}
