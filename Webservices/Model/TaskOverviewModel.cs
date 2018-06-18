using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Globalization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
   public class TaskOverviewModel
    {
        public int CurrentUserADId { get; set; }
        public int CurrentUserId { get; set; }
        public int TotalWaitingApproval { get; set; }
        public int TotalWaitingApprovalToday { get; set; }
        public int TotalApprovedToday { get; set; }
        public int TotalInProcess { get; set; }
    }
}
