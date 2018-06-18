using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// RequestRepairDetailsDAL
    /// </summary>
    public class RequestRepairDetailsDAL : BaseDAL<RequestRepairDetails>
    {
        /// <summary>
        /// RequestRepairDetailsDAL
        /// </summary>
        /// <param name="siteUrl"></param>
        public RequestRepairDetailsDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
