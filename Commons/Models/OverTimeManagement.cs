using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    
    public class OverTimeManagement : EntityBase
    {
        private string approvestatus = "";
        public string ApprovalStatus
        {
            get { return approvestatus; }
            set
            {
                approvestatus = value;
                switch (value)
                {
                    case "true":
                        ColForSort = 7;
                        break;
                    case "false":
                        ColForSort = 8;
                        break;
                    default:
                        ColForSort = 0;
                        break;
                }

            }
        }
        public LookupItem CommonDepartment { get; set; }
        public LookupItem CommonDepartment1066 { get; set; }
        public DateTime CommonDate { get; set; }
        public string OtherRequirements { get; set; }
        public string DHComments { get; set; }
        public string BODComments { get; set; }
        public string SecurityComments { get; set; }
        public LookupItem CommonLocation { get; set; }
        public string Place { get; set; }
        public LookupItem Requester { get; set; }
        public User ApprovedBy { get; set; }
        public User FirstApprovedBy { get; set; }
        public int SumOfEmployee { get; set; }
        public int SumOfMeal { get; set; }
        public int ColForSort { get; set; }
        public DateTime FirstApprovedDate { get; set; }
        public DateTime RequestDueDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public IList<OverTimeManagementDetail> OverTimeManagementDetailList { get; set; }
        public OverTimeManagement()
        {
            OverTimeManagementDetailList = new List<OverTimeManagementDetail>();

            CommonDepartment = new LookupItem();
            CommonLocation = new LookupItem();
            Requester = new LookupItem();
            ApprovedBy = new User();
            FirstApprovedBy = new User();
            ColForSort = 0;
        }
    }
}
