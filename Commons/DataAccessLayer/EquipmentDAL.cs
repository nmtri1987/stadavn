using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class EquipmentDAL : BaseDAL<Equipment>
    {
        public EquipmentDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
