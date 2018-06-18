using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// WorkflowHistory DAL.
    /// </summary>
    public class WorkflowHistoryDAL : BaseDAL<WorkflowHistory>
    {
        public WorkflowHistoryDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
