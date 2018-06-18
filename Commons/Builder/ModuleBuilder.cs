using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Builder
{
    public class ModuleBuilder
    {
        public string SiteUrl { get; set; }
        public ModuleBuilder(string siteURL)
        {
            SiteUrl = siteURL;
        }

        public EmployeeInfo GetNextApproval(int departmentID, int locationID, StepModuleList StepModule, int stepNumber)
        {
            IModuleBuilder moduleBuilder = null;
            EmployeeInfo nextAssignee = null;

            switch (StepModule)
            {
                case StepModuleList.VehicleManagement:
                    moduleBuilder = new VehicleManagementDAL(SiteUrl);
                    break;
                case StepModuleList.LeaveManagement:
                    moduleBuilder = new LeaveManagementDAL(SiteUrl);
                    break;
                default:
                    return null;
            }

            var approvalList = moduleBuilder.CreateApprovalList(departmentID, locationID);
            if (approvalList.Count > 0 && approvalList.Count() >= stepNumber)
            {
                nextAssignee = approvalList.ElementAt(stepNumber - 1);
            }

            return nextAssignee;
        }
    }
}
