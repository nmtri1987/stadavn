using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class BusinessTripManagementApproverModel
    {
        public int EmployeeIdentity { get; set; }

        /// <summary>
        /// Department Head
        /// </summary>
        public ApproverModel Approver1 { get; set; }

        /// <summary>
        /// BOD
        /// </summary>
        public ApproverModel Approver2 { get; set; }

        /// <summary>
        /// Direct BOD
        /// </summary>
        public ApproverModel Approver3 { get; set; }

        /// <summary>
        /// Admin Dept
        /// </summary>
        public ApproverModel Approver4 { get; set; }
    }
}
