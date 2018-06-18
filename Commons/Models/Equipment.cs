using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.EquipmentList.Url)]
    public class Equipment : EntityBase
    {
        public Equipment()
        {
        }

        [ListColumn(StringConstant.EquipmentList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.EquipmentList.Fields.CommonName)]
        public string CommonName { get; set; }

        [ListColumn(StringConstant.EquipmentList.Fields.CommonName1066)]
        public string CommonName1066 { get; set; }
    }
}
