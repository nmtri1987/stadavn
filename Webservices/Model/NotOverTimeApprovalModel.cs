using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class NotOverTimeApprovalModel
    {
        public int Id { get; set; }
        public string Comment { get; set; }

        public string ApproverName { get; set; }

        public int ApproverId { get; set; }
    }
}
