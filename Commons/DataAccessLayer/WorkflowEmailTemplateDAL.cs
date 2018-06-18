using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// WorkflowEmailTemplateDAL data access layer.
    /// </summary>
    public class WorkflowEmailTemplateDAL : BaseDAL<WorkflowEmailTemplate>
    {
        public WorkflowEmailTemplateDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
