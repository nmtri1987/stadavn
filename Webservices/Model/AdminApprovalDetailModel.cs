using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class AdminApprovalDetailModel
    {
        public List<ApprovalDayInfo> ApprovalDays { get; set; }
        public int ShiftDetailId { get; set; }
        public int ShiftManagementId { get; set; }
        public AdminApprovalDetailModel()
        {
            ApprovalDays = new List<ApprovalDayInfo>();
        }
    }
}
