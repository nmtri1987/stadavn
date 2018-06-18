using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// WorkflowStep
    /// </summary>
    [ListUrl("/Lists/WorkflowSteps")]
    public class WorkflowStep : EntityBase
    {
        [ListColumn(StringConstant.WorkflowStepsList.Fields.ListName)]
        public string ListName { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.CurrentStep)]
        public List<LookupItem> CurrentStep { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.NextStep)]
        public LookupItem NextStep { get; set; }

        //[ListColumn(StringConstant.WorkflowStepsList.Fields.RejectToStep)]
        //public string RejectToStep { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.NotificationEmailToRoles)]
        public List<LookupItem> NotificationEmailToRoles { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.NotificationEmailToEmployees)]
        public List<LookupItem> NotificationEmailToEmployees { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.NotificationEmailCcRoles)]
        public List<LookupItem> NotificationEmailCcRoles { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.NotificationEmailCcEmployees)]
        public List<LookupItem> NotificationEmailCcEmployees { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.AllowReject)]
        public bool AllowReject { get; set; }

        //[ListColumn(StringConstant.WorkflowStepsList.Fields.ApprovalEmailTemplate)]
        //public LookupItem ApprovalEmailTemplate { get; set; }

        //[ListColumn(StringConstant.WorkflowStepsList.Fields.RejectEmailTemplate)]
        //public LookupItem RejectEmailTemplate { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.ConditionalExpression)]
        public string ConditionalExpression { get; set; }

        [ListColumn(StringConstant.WorkflowStepsList.Fields.OrderStep)]
        public int OrderStep { get; set; }
    }
}
