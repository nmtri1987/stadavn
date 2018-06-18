using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// WorkflowStepDAL class.
    /// </summary>
    public class WorkflowStepDAL : BaseDAL<WorkflowStep>
    {
        public WorkflowStepDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
