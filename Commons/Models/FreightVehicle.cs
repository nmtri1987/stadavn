using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.FreightVehicleList.ListUrl)]
    public class FreightVehicle: EntityBase
    {
        [ListColumn(StringConstant.FreightVehicleList.Fields.Vehicle)]
        public string Vehicle { get; set; }

        [ListColumn(StringConstant.FreightVehicleList.Fields.VehicleVN)]
        public string VehicleVN { get; set; }

        public FreightVehicle() {
            Vehicle = string.Empty;
            VehicleVN = string.Empty;
        }
    }
}
