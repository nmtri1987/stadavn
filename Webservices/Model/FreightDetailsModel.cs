using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class FreightDetailsModel
    {
        public int Id { get; set; }
        public LookupItem FreightManagementID { get; set; }
        public string GoodsName { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public string Remarks { get; set; }
        public DateTime? ShippingIn { get; set; }
        public DateTime? ShippingOut { get; set; }

        public bool IsShippingIn { get; set; }
        public bool IsShippingOut { get; set; }
        public string ShippingInBy { get; set; }
        public string ShippingOutBy { get; set; }
        public string ShippingInTime { get; set; }
        public string ShippingOutTime { get; set; }
        public FreightDetailsModel() { }

        public FreightDetails ToEntity()
        {
            var freightDetails = new FreightDetails();

            freightDetails.ID = Id;
            freightDetails.FreightManagementID = FreightManagementID;
            freightDetails.GoodsName = GoodsName;
            freightDetails.Unit = Unit;
            freightDetails.Quantity = Quantity;
            freightDetails.Remarks = Remarks;
            freightDetails.ShippingIn = ShippingIn;
            freightDetails.ShippingOut = ShippingOut;
            
            return freightDetails;
        }
    }
}
