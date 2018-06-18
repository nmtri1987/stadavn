using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.FreightReceiverDepartmentList.ListUrl)]
    public class FreightReceiverDepartment : EntityBase
    {
        [ListColumn(StringConstant.FreightReceiverDepartmentList.ReceiverDepartmentField)]
        public string ReceiverDepartment { get; set; }

        [ListColumn(StringConstant.FreightReceiverDepartmentList.ReceiverDepartmentVNField)]
        public string ReceiverDepartmentVN { get; set; }

        public FreightReceiverDepartment()
        {
            ReceiverDepartment = string.Empty;
            ReceiverDepartmentVN = string.Empty;
        }
    }
}
