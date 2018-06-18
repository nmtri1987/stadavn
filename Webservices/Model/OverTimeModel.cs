using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class OverTimeModel
    {
        public int ID { get; set; }
        public string ApprovalStatus { get; set; }
        public LookupItem CommonDepartment { get; set; }
        public string Date
        {
            get; set;
        }
        public bool RequiredBODApprove { get; set; }
        public string OtherRequirements { get; set; }
        public LookupItem CommonLocation { get; set; }
        public string Place { get; set; }
        public LookupItem Requester { get; set; }
        public User ApprovedBy { get; set; }
        public User FirstApprovedBy { get; set; }
        public string SumOfEmployee { get; set; }
        public string SumOfMeal { get; set; }
        public List<OvertimeDetailModel> OvertimeDetailModelList { get; set; }
        public string ApproverFullName { get; set; }
        public string FirstApprovedDate { get; set; }
        public string Modified { get; set; }
        public int ApprovedLevel { get; set; }
        public string DHComments { get; set; }
        public string BODComments { get; set; }
        public string SecurityComments { get; set; }
        public bool RequestExpired { get; set; }
        public string RequestDueDate { get; set; }

        public OverTimeModel()
        {
            this.CommonDepartment = new LookupItem();
            this.CommonLocation = new LookupItem();
            this.Requester = new LookupItem();
            this.ApprovedBy = new User();
            this.FirstApprovedBy = new User();
            OvertimeDetailModelList = new List<OvertimeDetailModel>();
        }
        public OverTimeManagement ToEntity()
        {
            OverTimeManagement overtimeManagement = new OverTimeManagement();
            overtimeManagement.ID = this.ID;
            overtimeManagement.ApprovalStatus = this.ApprovalStatus;
            overtimeManagement.ApprovedBy = this.ApprovedBy;
            overtimeManagement.FirstApprovedBy = this.FirstApprovedBy;
            overtimeManagement.CommonDepartment = this.CommonDepartment;
            overtimeManagement.CommonDate = Convert.ToDateTime(this.Date);
            overtimeManagement.CommonLocation.LookupId = this.CommonLocation.LookupId;
            overtimeManagement.Place = this.Place;
            overtimeManagement.OtherRequirements = this.OtherRequirements;
            overtimeManagement.DHComments = this.DHComments;
            overtimeManagement.BODComments = this.BODComments;
            overtimeManagement.SecurityComments = this.SecurityComments;
            overtimeManagement.Requester.LookupId = this.Requester.LookupId;
            overtimeManagement.Requester.LookupValue = this.Requester.LookupValue;
            overtimeManagement.SumOfEmployee = string.IsNullOrEmpty(this.SumOfEmployee) ? 0 : int.Parse(this.SumOfEmployee);
            overtimeManagement.SumOfMeal = string.IsNullOrEmpty(this.SumOfMeal) ? 0 : int.Parse(this.SumOfMeal);
            return overtimeManagement;
        }
    }
}
