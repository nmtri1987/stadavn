using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.Globalization;
using System.Collections.Generic;
using RBVH.Stada.Intranet.WebPages.Common;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.BusinessTripManagement
{
    public partial class BusinessTripManagementManager : PageEventHandlingBase
    {
        public bool IsAdminDepartment { get; set; }
        public int CurrentDepartmentId { get; set; }

        protected bool hasRequestPermission = false;
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

            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee.EmployeeType == StringConstant.EmployeeType.ADUser &&
                (int)Convert.ToDouble(currentEmployee.EmployeeLevel.LookupValue, CultureInfo.InvariantCulture.NumberFormat) < (int)StringConstant.EmployeeLevel.BOD)
            {
                hasRequestPermission = true;
            }
        }
    }
}
