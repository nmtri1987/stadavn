using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class CommonApproverModel
    {
        public int EmployeeIdentity { get; set; }

        /// <summary>
        /// Department Head
        /// </summary>
        public EmployeeInfo Approver1 { get; set; }

        /// <summary>
        /// BOD
        /// </summary>
        public EmployeeInfo Approver2 { get; set; }
        
        /// <summary>
        /// Admin Dept
        /// </summary>
        public EmployeeInfo Approver3 { get; set; }
    }
}
