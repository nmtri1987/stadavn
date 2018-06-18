using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class FreightManagementModel
    {
        public int Id { get; set; }
        public string RequestNo { get; set; }
        public LookupItem Requester { get; set; }
        public LookupItem Department { get; set; }
        public LookupItem Location { get; set; }
        public LookupItem Bringer { get; set; }
        public LookupItem BringerDepartment { get; set; }
        public LookupItem BringerLocation { get; set; }
        public bool CompanyVehicle { get; set; }
        public string BringerName { get; set; }
        public string CompanyName { get; set; }
        public string Reason { get; set; }
        public string Receiver { get; set; }
        public LookupItem ReceiverDepartmentLookup { get; set; }
        public LookupItem ReceiverDepartmentVN { get; set; }
        public string ReceiverDepartmentText { get; set; }
        public string ReceiverPhone { get; set; }
        public bool FreightType { get; set; }
        public bool ReturnedGoods { get; set; }
        public bool HighPriority { get; set; }
        public string OtherReason { get; set; }
        public LookupItem VehicleLookup { get; set; }
        public LookupItem VehicleVN { get; set; }
        public bool IsValidRequest { get; set; }
        public bool IsFinished { get; set; }
        public List<FreightDetailsModel> FreightDetails { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string SecurityNotes { get; set; }
        public string Comment { get; set; }
        public string ApprovalStatus { get; set; }
        public bool RequestExpired { get; set; }
        public string RequestDueDate { get; set; }
        public User DH { get; set; }
        public User BOD { get; set; }
        public User AdminDept { get; set; }
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set; }

        public FreightManagementModel()
        {
            FreightDetails = new List<FreightDetailsModel>();
            Date = DateTime.Now;
        }

        public FreightManagement ToEntity()
        {
            var freightManagement = new FreightManagement();

            freightManagement.ID = Id;
            freightManagement.RequestNo = RequestNo;
            freightManagement.Requester = Requester;
            freightManagement.Department = Department;
            freightManagement.Location = Location;
            freightManagement.Bringer = Bringer;
            freightManagement.BringerDepartment = BringerDepartment;
            freightManagement.BringerLocation = BringerLocation;
            freightManagement.CompanyVehicle = CompanyVehicle;
            freightManagement.BringerName = BringerName;
            freightManagement.CompanyName = CompanyName;
            freightManagement.Reason = Reason;
            freightManagement.Receiver = Receiver;
            freightManagement.ReceiverDepartmentLookup = ReceiverDepartmentLookup;
            freightManagement.ReceiverDepartmentVN = ReceiverDepartmentVN;
            freightManagement.ReceiverDepartmentText = ReceiverDepartmentText;
            freightManagement.ReceiverPhone = ReceiverPhone;
            freightManagement.FreightType = FreightType;
            freightManagement.ReturnedGoods = ReturnedGoods;
            freightManagement.HighPriority = HighPriority;
            freightManagement.OtherReason = OtherReason;
            freightManagement.VehicleLookup = VehicleLookup;
            freightManagement.VehicleVN = VehicleVN;
            freightManagement.IsValidRequest = IsValidRequest;
            DateTime dateTime;
            DateTime.TryParseExact(DateString, StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, Hour, Minute, 0);
            freightManagement.TransportTime = date;
            freightManagement.SecurityNotes = SecurityNotes;
            freightManagement.Comment = Comment;
            freightManagement.ApprovalStatus = ApprovalStatus;
            freightManagement.DH = DH;
            freightManagement.BOD = BOD;
            freightManagement.AdminDept = AdminDept;

            return freightManagement;
        }
    }
}
