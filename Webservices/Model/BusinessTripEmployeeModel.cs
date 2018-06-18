using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class BusinessTripEmployeeModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public string DepartmentName { get; set; }
        public LookupItem BusinessTripManagementID { get; set; }

        public BusinessTripEmployeeModel() {
            BusinessTripManagementID = new LookupItem();
        }

        public BusinessTripEmployeeDetail ToEntity()
        {
            BusinessTripEmployeeDetail businessTripEmployeeDetail = new BusinessTripEmployeeDetail();

            businessTripEmployeeDetail.ID = Id;
            businessTripEmployeeDetail.Employee = new LookupItem() { LookupId = EmployeeId, LookupValue = "" };
            businessTripEmployeeDetail.BusinessTripManagementID = BusinessTripManagementID;

            return businessTripEmployeeDetail;
        }
    }
}
