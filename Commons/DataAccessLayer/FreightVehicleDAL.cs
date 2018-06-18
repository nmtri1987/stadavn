using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class FreightVehicleDAL : BaseDAL<FreightVehicle>
    {
        public FreightVehicleDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
