using RBVH.Stada.Intranet.Biz.Helpers;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// WorkflowEmailTemplate Model.
    /// </summary>
    [ListUrl(WorkflowEmailTemplateList.Url)]
    public class WorkflowEmailTemplate : EntityBase
    {
        [ListColumn(WorkflowEmailTemplateList.Fields.Key)]
        public string Key { get; set; }

        [ListColumn(WorkflowEmailTemplateList.Fields.Subject)]
        public string Subject { get; set; }

        [ListColumn(WorkflowEmailTemplateList.Fields.Body)]
        public string Body { get; set; }

        [ListColumn(WorkflowEmailTemplateList.Fields.Description)]
        public string Description { get; set; }

        [ListColumn(WorkflowEmailTemplateList.Fields.ListName)]
        public string ListName { get; set; }

        [ListColumn(WorkflowEmailTemplateList.Fields.Action)]
        public string Action { get; set; }
    }
}
