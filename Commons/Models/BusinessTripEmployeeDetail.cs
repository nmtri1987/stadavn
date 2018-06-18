using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.BusinessTripEmployeeDetailsList.Url)]
    public class BusinessTripEmployeeDetail : EntityBase
    {
        public BusinessTripEmployeeDetail()
        {
            BusinessTripManagementID = new LookupItem();
            ApprovalStatus = new LookupItem();
            Employee = new LookupItem();
            EmployeeID = new LookupItem();
        }

        [ListColumn(StringConstant.BusinessTripEmployeeDetailsList.Fields.BusinessTripManagementID)]
        public LookupItem BusinessTripManagementID { get; set; }

        [ListColumn(StringConstant.BusinessTripEmployeeDetailsList.Fields.ApprovalStatus)]
        public LookupItem ApprovalStatus { get; set; }

        [ListColumn(StringConstant.BusinessTripEmployeeDetailsList.Fields.Employee)]
        public LookupItem Employee { get; set; }

        [ListColumn(StringConstant.BusinessTripEmployeeDetailsList.Fields.EmployeeID)]
        public LookupItem EmployeeID { get; set; }
    }
}
