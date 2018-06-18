using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class FreightReceiverDepartmentModel
    {
        public int Id { get; set; }
        public string ReceiverDepartment { get; set; }
        public string ReceiverDepartmentVN { get; set; }
        
        public FreightReceiverDepartmentModel() { }

        public FreightReceiverDepartment ToEntity()
        {
            var freightReceiverDepartment = new FreightReceiverDepartment();

            freightReceiverDepartment.ID = Id;
            freightReceiverDepartment.ReceiverDepartment = ReceiverDepartment;
            freightReceiverDepartment.ReceiverDepartmentVN = ReceiverDepartmentVN;

            return freightReceiverDepartment;
        }
    }
}
