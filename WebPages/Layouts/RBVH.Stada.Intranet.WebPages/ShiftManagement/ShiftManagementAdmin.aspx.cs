using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System.Collections.Generic;
using Microsoft.SharePoint.Utilities;
using System.Web;
using RBVH.Stada.Intranet.WebPages.Common;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.ShiftManagement
{
    public partial class ShiftManagementAdmin : PageEventHandlingBase
    {
        protected bool IsAdminDepartment { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsAdminDepartment = UserPermission.IsAdminDepartment;
            DateTime defaultDateTimeOfShift = URLHelper.GetDefaultDateTimeOfShift();

            Dictionary<string, string> paramCollection = new Dictionary<string, string>();
            paramCollection.Add("MyMonth", defaultDateTimeOfShift.Month.ToString());
            paramCollection.Add("MyYear", defaultDateTimeOfShift.Year.ToString());

            if (IsAdminDepartment == true)
            {
                paramCollection.Add("AdminMonth", defaultDateTimeOfShift.Month.ToString());
                paramCollection.Add("AdminYear", defaultDateTimeOfShift.Year.ToString());
                paramCollection.Add("AdminDeptId", "0");
            }

            Uri oldUri = this.Page.Request.Url;
            Uri newUri = this.Page.Request.Url.AddParameter(paramCollection);
            URLHelper.RedirectPage(oldUri, newUri);
        }
    }
}
