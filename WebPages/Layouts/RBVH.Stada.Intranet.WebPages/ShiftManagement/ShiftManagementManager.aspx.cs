using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.Biz.Constants;
using System.Collections.Generic;
using RBVH.Stada.Intranet.WebPages.Common;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.ShiftManagement
{
    public partial class ShiftManagementManager : PageEventHandlingBase
    {
        protected bool IsAdminDepartment { get; set; }
        protected int CurrentDepartmentId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsAdminDepartment = UserPermission.IsAdminDepartment;
            CurrentDepartmentId = UserPermission.CurrentDepartmentId;
            DateTime defaultDateTimeOfShift = URLHelper.GetDefaultDateTimeOfShift();

            Dictionary<string, string> paramCollection = new Dictionary<string, string>();
            paramCollection.Add("AdminMonth", defaultDateTimeOfShift.Month.ToString());
            paramCollection.Add("AdminYear", defaultDateTimeOfShift.Year.ToString());

            if (IsAdminDepartment == true)
            {
                paramCollection.Add("AdminDeptId", "0");
            }
            else
            {
                paramCollection.Add("AdminDeptId", CurrentDepartmentId.ToString()); 
            }

            Uri oldUri = this.Page.Request.Url;
            Uri newUri = this.Page.Request.Url.AddParameter(paramCollection);
            URLHelper.RedirectPage(oldUri, newUri);
        }
    }
}
