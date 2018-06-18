using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class BusinessTripManagementModel
    {
        public int Id { get; set; }
        public LookupItem Requester { get; set; }
        public LookupItem Department { get; set; }
        public LookupItem Location { get; set; }
        public bool Domestic { get; set; }
        public string BusinessTripPurpose { get; set; }
        public bool HotelBooking { get; set; }
        public bool TripHighPriority { get; set; }
        public string PaidBy { get; set; }
        public string OtherService { get; set; }
        public string TransportationType { get; set; }
        public string OtherTransportationDetail { get; set; }
        public bool HasVisa { get; set; }
        public string CashRequestDetails { get; set; }
        public string OtherRequestDetail { get; set; }
        public LookupItem Driver { get; set; }
        public LookupItem Cashier { get; set; }
        public string Comment { get; set; }
        public string ApprovalStatus { get; set; }
        public User DH { get; set; }
        public User DirectBOD { get; set; }
        public User BOD { get; set; }
        public User AdminDept { get; set; }
        public List<BusinessTripEmployeeModel> EmployeeList { get; set; }
        public List<BusinessTripScheduleModel> ScheduleList { get; set; }
        public bool RequestExpired { get; set; }
        public string RequestDueDate { get; set; }
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set; }

        public BusinessTripManagementModel() {
            Requester = new LookupItem();
            Department = new LookupItem();
            Location = new LookupItem();
            Driver = new LookupItem();
            Cashier = new LookupItem();
            EmployeeList = new List<BusinessTripEmployeeModel>();
            ScheduleList = new List<BusinessTripScheduleModel>();
        }

        public BusinessTripManagement ToEntity()
        {
            BusinessTripManagement businessTripManagement = new BusinessTripManagement();

            businessTripManagement.ID = Id;
            businessTripManagement.Requester = Requester;
            businessTripManagement.CommonDepartment = Department;
            businessTripManagement.CommonLocation = Location;
            businessTripManagement.Domestic = Domestic;
            businessTripManagement.BusinessTripPurpose = BusinessTripPurpose;
            businessTripManagement.HotelBooking = HotelBooking;
            businessTripManagement.TripHighPriority = TripHighPriority;
            businessTripManagement.PaidBy = PaidBy;
            businessTripManagement.OtherService = OtherService;
            businessTripManagement.TransportationType = TransportationType;
            businessTripManagement.OtherTransportationDetail = OtherTransportationDetail;
            businessTripManagement.HasVisa = HasVisa;
            businessTripManagement.CashRequestDetail = CashRequestDetails;
            businessTripManagement.OtherRequestDetail = OtherRequestDetail;
            businessTripManagement.Driver = Driver;
            businessTripManagement.Cashier = Cashier;
            businessTripManagement.Comment = Comment;
            businessTripManagement.ApprovalStatus = ApprovalStatus;
            businessTripManagement.DH = DH;
            businessTripManagement.DirectBOD = DirectBOD;
            businessTripManagement.BOD = BOD;
            businessTripManagement.AdminDept = AdminDept;

            return businessTripManagement;
        }
    }
}
