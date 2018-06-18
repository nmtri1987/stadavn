using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.BusinessTripManagementList.Url)]
    public class BusinessTripManagement : EntityBase
    {
        [ListColumn(StringConstant.CommonSPListField.RequesterField)]
        public LookupItem Requester { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem CommonDepartment { get; set; }

        [ListColumn(StringConstant.CommonSPListField.DepartmentName1066Field)]
        public LookupItem DepartmentName1066 { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonLocationField)]
        public LookupItem CommonLocation { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.Domestic)]
        public bool Domestic { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.BusinessTripPurpose)]
        public string BusinessTripPurpose { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.HotelBooking)]
        public bool HotelBooking { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.TripHighPriority)]
        public bool TripHighPriority { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.PaidBy)]
        public string PaidBy { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.OtherService)]
        public string OtherService { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.TransportationType)]
        public string TransportationType { get; set; }
        [ListColumn(StringConstant.BusinessTripManagementList.Fields.OtherTransportationDetail)]
        public string OtherTransportationDetail { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.HasVisa)]
        public bool HasVisa { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.CashRequestDetail)]
        public string CashRequestDetail { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.OtherRequestDetail)]
        public string OtherRequestDetail { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.Driver)]
        public LookupItem Driver { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.Cashier)]
        public LookupItem Cashier { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonCommentField)]
        public string Comment { get; set; }

        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public string ApprovalStatus { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.DH)]
        public User DH { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.DirectBOD)]
        public User DirectBOD { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.BOD)]
        public User BOD { get; set; }

        [ListColumn(StringConstant.BusinessTripManagementList.Fields.AdminDept)]
        public User AdminDept { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedByField)]
        public User CreatedBy { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedByField)]
        public User ModifiedBy { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }

        public BusinessTripManagement()
        {
            Requester = new LookupItem();
            CommonDepartment = new LookupItem();
            DepartmentName1066 = new LookupItem();
            CommonLocation = new LookupItem();
            Driver = new LookupItem();
            Cashier = new LookupItem();
            DH = new User();
            DirectBOD = new User();
            BOD = new User();
            AdminDept = new User();
        }
    }
}
