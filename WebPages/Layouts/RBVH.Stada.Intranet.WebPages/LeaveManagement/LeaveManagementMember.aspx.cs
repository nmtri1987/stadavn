using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.Collections.Generic;
using RBVH.Stada.Intranet.WebPages.Common;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.LeaveManagement
{
    public partial class LeaveManagementMember : PageEventHandlingBase
    {
        protected bool isSecurityGuard = false;
        protected bool isTeamLeader = false;
        protected bool isShiftLeader = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckCurrentUser();
        }

        private void CheckCurrentUser()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();

            var url = SPContext.Current.Web.Url;
            AdditionalEmployeePositionDAL additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(url);
            isSecurityGuard = additionalEmployeePositionDAL.GetAdditionalPosition(currentEmployee.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);
            if (isSecurityGuard == true)
            {
                Dictionary<string, string> paramCollection = new Dictionary<string, string>();
                paramCollection.Add("employeeId", "0");
                
                Uri oldUri = this.Page.Request.Url;
                Uri newUri = this.Page.Request.Url.AddParameter(paramCollection);
                URLHelper.RedirectPage(oldUri, newUri);
            }

            isTeamLeader = (currentEmployee.EmployeePosition.LookupValue == StringConstant.EmployeePositionName.TeamLeader);
            isShiftLeader = (currentEmployee.EmployeePosition.LookupValue == StringConstant.EmployeePositionName.ShiftLeader);
        }
    }
}
