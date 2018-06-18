using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class ShiftManagementModel
    {
        public int Id { get; set; }
        public DepartmentInfo Department { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string ApprovalStatus { get; set; }
        public LookupItem Requester { get; set; }
        public LookupItem Location { get; set; }
        public User ApprovedBy { get; set; }
        public User ModifiedBy { get; set; }
        public string ModifiedByString { get; set; }
        public List<User> AdditionalUser { get; set; }
        public string ApproverFullName { get; set; }
        public List<ShiftManagementDetailModel> ShiftManagementDetailModelList { get; set; }
        public bool RequestExpired { get; set; }
        public string RequestDueDate { get; set; }

        public ShiftManagementModel()
        {
            Department = new DepartmentInfo();
            ShiftManagementDetailModelList = new List<ShiftManagementDetailModel>();
        }

        public ShiftManagement ToEntity()
        {
            var shiftManagement = new ShiftManagement();
            shiftManagement.ID = Id;
            shiftManagement.Department.LookupId = Department.Id;
            shiftManagement.Requester.LookupId = Requester.LookupId;
            shiftManagement.Month = Month;
            shiftManagement.Year = Year;
            shiftManagement.Location.LookupId = Location.LookupId;
            shiftManagement.ApprovedBy = ApprovedBy;
            shiftManagement.CommonAddApprover1 = AdditionalUser;
            shiftManagement.ModifiedBy = ModifiedBy;
            return shiftManagement;
        }
    }
}
