
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Interfaces
{
    public interface IModuleBuilder
    {
        IList<EmployeeInfo> CreateApprovalList(int departmentId, int locationId);
    }
}
