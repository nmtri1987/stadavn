using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class ShiftManagementQueryResult
    {
        public DateTime Date { get; set; }
        public SPFieldLookupValueCollection EmployeesShift { get; set; }
        public string ApprovalStatus {get;set;}

        public ShiftManagementQueryResult()
        {
            EmployeesShift = new SPFieldLookupValueCollection();
        }
    }
}
