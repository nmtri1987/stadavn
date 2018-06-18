using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class OvertimeDetailModel
    {
        public int ID { get; set; }
        public LookupItem OvertimeMgmtApprovalStatus { get; set; }
        public LookupItem Employee { get; set; }

        public string FullName { get; set; }

        public string OvertimeHourFrom { get; set; }

        public string OvertimeHourTo { get; set; }

        //public LookupItem OvertimeMgmtFromDate { get; set; }
        public string OvertimeFrom { get; set; }
 
        public string OvertimeTo { get; set; }
        public LookupItem OvertimeManagementID { get; set; }
        //public LookupItem OvertimeMgmtToDate { get; set; }

        public string HM { get; set; }
        public string KD { get; set; }
        public string CompanyTransport { get; set; }
        public string Task { get; set; }
        public int Day { get; set; }

        public string SummaryLinks { get; set; }

        public string WorkingHours { get; set; }
        public OvertimeDetailModel()
        {
            this.OvertimeMgmtApprovalStatus = new LookupItem();
            this.Employee = new LookupItem();
            //this.OvertimeMgmtFromDate = new LookupItem();
            this.OvertimeManagementID = new LookupItem();
            //this.OvertimeMgmtToDate = new LookupItem();
        }
    }
}
