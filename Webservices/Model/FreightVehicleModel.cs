using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class FreightVehicleModel
    {
        public int Id { get; set; }
        public string Vehicle { get; set; }
        public string VehicleVN { get; set; }
        
        public FreightVehicleModel() {
            Vehicle = string.Empty;
            VehicleVN = string.Empty;
        }

        public FreightVehicle ToEntity()
        {
            var freightVehicle = new FreightVehicle();

            freightVehicle.ID = Id;
            freightVehicle.Vehicle = Vehicle;
            freightVehicle.VehicleVN = VehicleVN;

            return freightVehicle;
        }
    }
}
