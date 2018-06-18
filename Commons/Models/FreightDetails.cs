using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.FreightDetailsList.ListUrl)]
    public class FreightDetails : EntityBase
    {
        [ListColumn(StringConstant.FreightDetailsList.FreightManagementIDField)]
        public LookupItem FreightManagementID { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.GoodsNameField)]
        public string GoodsName { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.UnitField)]
        public string Unit { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.QuantityField)]
        public double Quantity { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.RemarksField)]
        public string Remarks { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.ShippingInField)]
        public DateTime? ShippingIn { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.ShippingOutField)]
        public DateTime? ShippingOut { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.CheckInByField)]
        public LookupItem CheckInBy { get; set; }

        [ListColumn(StringConstant.FreightDetailsList.CheckOutByField)]
        public LookupItem CheckOutBy { get; set; }

        public FreightDetails()
        {
            FreightManagementID = new LookupItem();
            GoodsName = string.Empty;
            Unit = string.Empty;
            Quantity = 0.0;
            Remarks = string.Empty;
            CheckInBy = new LookupItem();
            CheckOutBy = new LookupItem();
        }
    }
}
