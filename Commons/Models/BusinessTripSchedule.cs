using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.BusinessTripScheduleList.Url)]
    public class BusinessTripSchedule : EntityBase
    {
        public BusinessTripSchedule()
        {
            BusinessTripManagementID = new LookupItem();
            ApprovalStatus = new LookupItem();
        }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.BusinessTripManagementID)]
        public LookupItem BusinessTripManagementID { get; set; }

        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public LookupItem ApprovalStatus { get; set; }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.DepartDate)]
        public System.DateTime? DepartDate { get; set; }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.FlightName)]
        public string FlightName { get; set; }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.City)]
        public string City { get; set; }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.Country)]
        public string Country { get; set; }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.ContactCompany)]
        public string ContactCompany { get; set; }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.ContactPhone)]
        public string ContactPhone { get; set; }

        [ListColumn(StringConstant.BusinessTripScheduleList.Fields.OtherSchedule)]
        public string OtherSchedule { get; set; }
    }
}
