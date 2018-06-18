using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class EmployeeApproverModel
    {
        public int EmployeeIdentity { get; set; }

        /// <summary>
        /// Team leader / Shift Leader
        /// </summary>
        public List<ApproverModel> Approver1 { get; set; }

        /// <summary>
        /// Department head
        /// </summary>
        public ApproverModel Approver2 { get; set; }

        /// <summary>
        /// BOD
        /// </summary>
        public ApproverModel Approver3 { get; set; }
        
        public EmployeeApproverModel()
        {
            Approver1 = new List<ApproverModel>();
        }
    }
}
