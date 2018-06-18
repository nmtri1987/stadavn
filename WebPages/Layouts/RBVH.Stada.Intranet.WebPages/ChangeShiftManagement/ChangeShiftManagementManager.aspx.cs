using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.Linq;
using RBVH.Core.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using RBVH.Stada.Intranet.WebPages.Utils;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.WebPages.Common;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.ChangeShiftManagement
{
    public partial class ChangeShiftManagementManager : PageEventHandlingBase
    {
        protected bool IsAdminDepartment { get; set; }
        protected int CurrentDepartmentId { get; set; }      

        protected void Page_Load(object sender, EventArgs e)
        {
            IsAdminDepartment = UserPermission.IsAdminDepartment;
            CurrentDepartmentId = UserPermission.CurrentDepartmentId;

            DateTime currentDateTime = DateTime.Now;
            string fromDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1).ToString(StringConstant.DateFormatyyyyMMddHHmmssfff);
            string toDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1).ToString(StringConstant.DateFormatyyyyMMddHHmmssfff);
            Dictionary<string, string> paramCollection = new Dictionary<string, string>();
            paramCollection.Add("AdminStartMonth", fromDate);
            paramCollection.Add("AdminEndMonth", toDate);

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
