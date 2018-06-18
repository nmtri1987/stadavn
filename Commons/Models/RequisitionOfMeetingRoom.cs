using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.RequisitionOfMeetingRoomList.Url)]
    public class RequisitionOfMeetingRoom : EntityBase
    {
        public RequisitionOfMeetingRoom()
        {
        }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.StartDate)]
        public DateTime StartDate { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.EndDate)]
        public DateTime EndDate { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.DiscussionMeeting)]
        public string DiscussionMeeting { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.Participation)]
        public string Participation { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.MeetingRoomLocation)]
        public LookupItem MeetingRoomLocation { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.Equipment)]
        public List<LookupItem> Equipment { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.EquipmentVN)]
        public List<LookupItem> EquipmentVN { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.Seats)]
        public string Seats { get; set; }

        [ListColumn(StringConstant.RequisitionOfMeetingRoomList.Fields.Others)]
        public string Others { get; set; }

        public DateTime CommonFrom { get; set; }

        [ListColumn(ApprovalFields.Creator)]
        public LookupItem CommonCreator { get; set; }

        [ListColumn(ApprovalFields.Status)]
        public string ApprovalStatus { get; set; }

        [ListColumn(ApprovalFields.WFStatus)]
        public string WFStatus { get; set; }

        [ListColumn(ApprovalFields.PendingAt)]
        public List<LookupItem> PendingAt { get; set; }

        [ListColumn(ApprovalFields.CurrentStep)]
        public LookupItem CurrentStep { get; set; }

        [ListColumn(ApprovalFields.NextStep)]
        public LookupItem NextStep { get; set; }

        [ListColumn(ApprovalFields.CommonLocation)]
        public LookupItem CommonLocation { get; set; }

        [ListColumn(ApprovalFields.CommonDepartment)]
        public LookupItem CommonDepartment { get; set; }

        [ListColumn(ApprovalFields.IsAdditionalStep)]
        public bool IsAdditionalStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalPreviousStep)]
        public LookupItem AdditionalPreviousStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalStep)]
        public LookupItem AdditionalStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalNextStep)]
        public LookupItem AdditionalNextStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalDepartment)]
        public LookupItem AdditionalDepartment { get; set; }

        [ListColumn(ApprovalFields.AssignFrom)]
        public LookupItem AssignFrom { get; set; }

        [ListColumn(ApprovalFields.AssignTo)]
        public LookupItem AssignTo { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedField)]
        public DateTime Modified { get; set; }
    }
}
