using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class LeaveManagementModel
    {
        public int Id { get; set; }
        public LookupItem Requester { get; set; }
        public LookupItem RequestFor { get; set; }
        public LookupItem Department { get; set; }
        public LookupItem Location { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        private double totalHourVal = 0;
        public double LeaveHours
        {
            get { return totalHourVal; }
            set
            {
                totalHourVal = value;
                TimeSpan interval = TimeSpan.FromHours(value);
                //TotalHoursDisp = string.Format("{0:00}:{1:00}", (int)interval.TotalHours, interval.Minutes);
                TotalHoursDisp = string.Format("{0:00}:{1:00}", (interval.Days * 24 + interval.Hours), interval.ToString("mm"));
            }
        }
        public string TotalHoursDisp
        {
            get; set;
        }
        public double TotalDays { get; set; }
        public string Reason { get; set; }
        public LookupItem TransferworkTo { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool Left { get; set; }
        public bool UnexpectedLeave { get; set; }
        public User TLE { get; set; }
        public User DH { get; set; }
        public User BOD { get; set; }
        public User Approver { get; set; }
        public string Comment { get; set; }
        public string ApprovalStatus { get; set; }
        public bool IsValidRequest { get; set; }
        public LeaveManagementModel() { }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string LeftAtDate { get; set; }
        public bool RequestExpired { get; set; }
        public string RequestDueDate { get; set; }
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set; }
        public List<User> AdditionalUser { get; set; }

        public LeaveManagement ToEntity()
        {
            var leaveManagement = new LeaveManagement();
            leaveManagement.ID = Id;
            leaveManagement.Requester.LookupId = Requester.LookupId;
            leaveManagement.RequestFor.LookupId = RequestFor.LookupId;
            leaveManagement.Department.LookupId = Department.LookupId;
            leaveManagement.Location.LookupId = Location.LookupId;
            leaveManagement.From = DateTime.Parse(FromDate);
            leaveManagement.To = DateTime.Parse(ToDate);
            leaveManagement.LeaveHours = LeaveHours;
            leaveManagement.Reason = Reason;
            leaveManagement.TransferworkTo.LookupId = TransferworkTo.LookupId;
            leaveManagement.LeftAt = LeftAtDate != null ? DateTime.Parse(LeftAtDate) : leaveManagement.From;
            leaveManagement.Left = Left;
            leaveManagement.UnexpectedLeave = UnexpectedLeave;
            leaveManagement.TLE = TLE;
            leaveManagement.DH = DH;
            leaveManagement.BOD = BOD;
            leaveManagement.Approver = Approver;
            leaveManagement.Comment = Comment;
            leaveManagement.ApprovalStatus = ApprovalStatus;
            leaveManagement.IsValidRequest = IsValidRequest;
            leaveManagement.AdditionalUser = AdditionalUser;
            leaveManagement.TotalDays = TotalDays;
            return leaveManagement;
        }
    }
}
