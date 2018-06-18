using RBVH.Stada.Intranet.Biz.Models;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// DelegationsOfNewTaskDAL
    /// </summary>
    public class DelegationsOfNewTaskDAL : BaseDAL<DelegationOfNewTask>
    {
        /// <summary>
        /// DelegationsOfNewTaskDAL
        /// </summary>
        public DelegationsOfNewTaskDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
