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
    [ListUrl(StringConstant.RequestsList.Url)]
    public class Request : EntityBase
    {
        public Request()
        {
            RequestTypeRef = new LookupItem();
            ReceviedBy = new LookupItem();
            ReferRequest = new LookupItem();
            CommonCreator = new LookupItem();
            PendingAt = new List<LookupItem>();
            CurrentStep = new LookupItem();
            NextStep = new LookupItem();
            Location = new LookupItem();
            Department = new LookupItem();
            AdditionalPreviousStep = new LookupItem();
            AdditionalNextStep = new LookupItem();
            AdditionalStep = new LookupItem();
            AssignTo = new LookupItem();
            AssignFrom = new LookupItem();
            AdditionalDepartment = new LookupItem();
        }

        [ListColumn(StringConstant.RequestsList.TitleField)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequestsList.RequestTypeRefField)]
        public LookupItem RequestTypeRef { get; set; }

        [ListColumn(StringConstant.RequestsList.RequestTypeRefIdField)]
        public int RequestTypeRefId { get; set; }

        [ListColumn(StringConstant.RequestsList.ReceviedByField)]
        public LookupItem ReceviedBy { get; set; }

        [ListColumn(StringConstant.RequestsList.FinishDateField)]
        public DateTime FinishDate { get; set; }

        [ListColumn(StringConstant.RequestsList.RequiredApprovalByBODField)]
        public bool RequiredApprovalByBOD { get; set; }

        [ListColumn(StringConstant.RequestsList.ReferRequestField)]
        public LookupItem ReferRequest { get; set; }

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
        public LookupItem Location { get; set; }

        [ListColumn(ApprovalFields.CommonDepartment)]
        public LookupItem Department { get; set; }

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

        [ListColumn(StringConstant.RequestsList.CommonDueDateField)]
        public DateTime DueDate { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedField)]
        public DateTime Modified { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }
    }
}
