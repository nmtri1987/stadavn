using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class FreightVehicleOperatorModel
    {
        public bool HasPermission { get; set; }
        public List<FreightVehicle> FreightVehicles { get; set; }
        
        public FreightVehicleOperatorModel() {
            HasPermission = false;
            FreightVehicles = new List<FreightVehicle>();
        }
    }
}
