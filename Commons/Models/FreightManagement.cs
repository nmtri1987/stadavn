using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.FreightManagementList.ListUrl)]
    public class FreightManagement : EntityBase
    {
        [ListColumn(StringConstant.FreightManagementList.RequestNoField)]
        public string RequestNo { get; set; }

        [ListColumn(StringConstant.CommonSPListField.RequesterField)]
        public LookupItem Requester { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem Department { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonLocationField)]
        public LookupItem Location { get; set; }

        [ListColumn(StringConstant.FreightManagementList.BringerField)]
        public LookupItem Bringer { get; set; }

        [ListColumn(StringConstant.FreightManagementList.BringerDepartmentField)]
        public LookupItem BringerDepartment { get; set; }

        [ListColumn(StringConstant.FreightManagementList.BringerLocationField)]
        public LookupItem BringerLocation { get; set; }

        [ListColumn(StringConstant.FreightManagementList.BringerNameField)]
        public string BringerName { get; set; }

        [ListColumn(StringConstant.FreightManagementList.CompanyNameField)]
        public string CompanyName { get; set; }

        [ListColumn(StringConstant.FreightManagementList.CompanyVehicleField)]
        public bool CompanyVehicle { get; set; }

        [ListColumn(StringConstant.FreightManagementList.ReasonField)]
        public string Reason { get; set; }

        [ListColumn(StringConstant.FreightManagementList.ReceiverField)]
        public string Receiver { get; set; }

        [ListColumn(StringConstant.FreightManagementList.ReceiverDepartmentLookupField)]
        public LookupItem ReceiverDepartmentLookup { get; set; }

        [ListColumn(StringConstant.FreightManagementList.ReceiverDepartmentVNField)]
        public LookupItem ReceiverDepartmentVN { get; set; }

        [ListColumn(StringConstant.FreightManagementList.ReceiverDepartmentTextField)]
        public string ReceiverDepartmentText { get; set; }

        [ListColumn(StringConstant.FreightManagementList.ReceiverPhoneField)]
        public string ReceiverPhone { get; set; }

        [ListColumn(StringConstant.FreightManagementList.FreightTypeField)]
        public bool FreightType { get; set; }

        [ListColumn(StringConstant.FreightManagementList.ReturnedGoodsField)]
        public bool ReturnedGoods { get; set; }

        [ListColumn(StringConstant.FreightManagementList.HighPriorityField)]
        public bool HighPriority { get; set; }

        [ListColumn(StringConstant.FreightManagementList.OtherReasonField)]
        public string OtherReason { get; set; }

        [ListColumn(StringConstant.FreightManagementList.TransportTimeField)]
        public DateTime TransportTime { get; set; }

        [ListColumn(StringConstant.FreightManagementList.VehicleLookupField)]
        public LookupItem VehicleLookup { get; set; }

        [ListColumn(StringConstant.FreightManagementList.VehicleVNField)]
        public LookupItem VehicleVN { get; set; }

        [ListColumn(StringConstant.FreightManagementList.IsValidRequestField)]
        public bool IsValidRequest { get; set; }

        [ListColumn(StringConstant.FreightManagementList.IsFinishedField)]
        public bool IsFinished { get; set; }

        [ListColumn(StringConstant.FreightManagementList.SecurityNotesField)]
        public string SecurityNotes { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonCommentField)]
        public string Comment { get; set; }

        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public string ApprovalStatus { get; set; }

        [ListColumn(StringConstant.FreightManagementList.DHField)]
        public User DH { get; set; }

        [ListColumn(StringConstant.FreightManagementList.BODField)]
        public User BOD { get; set; }

        [ListColumn(StringConstant.FreightManagementList.AdminDeptField)]
        public User AdminDept { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedByField)]
        public User CreatedBy { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedByField)]
        public User ModifiedBy { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }

        public FreightManagement()
        {
            RequestNo = string.Empty;
            Requester = new LookupItem();
            Department = new LookupItem();
            Location = new LookupItem();
            Bringer = new LookupItem();
            BringerDepartment = new LookupItem();
            BringerLocation = new LookupItem();
            BringerName = string.Empty;
            CompanyName = string.Empty;
            CompanyVehicle = false;
            Reason = string.Empty;
            Receiver = string.Empty;
            ReceiverDepartmentLookup = new LookupItem();
            ReceiverDepartmentVN = new LookupItem();
            ReceiverDepartmentText = string.Empty;
            ReceiverPhone = string.Empty;
            FreightType = true; //true == out, false == in
            ReturnedGoods = false;
            HighPriority = false;
            OtherReason = string.Empty;
            TransportTime = new DateTime();
            VehicleLookup = new LookupItem();
            VehicleVN = new LookupItem();
            IsValidRequest = true;
            IsFinished = false;
            SecurityNotes = string.Empty;
            Comment = string.Empty;
            ApprovalStatus = string.Empty;
            DH = new User();
            BOD = new User();
            AdminDept = new User();
            CreatedBy = new User();
            ModifiedBy = new User();
        }
    }
}
