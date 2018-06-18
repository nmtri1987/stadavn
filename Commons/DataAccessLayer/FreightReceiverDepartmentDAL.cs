using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class FreightReceiverDepartmentDAL: BaseDAL<FreightReceiverDepartment>
    {
        public FreightReceiverDepartmentDAL(string siteUrl) : base(siteUrl) { }
    }
}
