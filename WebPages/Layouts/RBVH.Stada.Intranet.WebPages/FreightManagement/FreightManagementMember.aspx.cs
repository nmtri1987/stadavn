using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.Globalization;
using System.Collections.Generic;
using RBVH.Stada.Intranet.WebPages.Common;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.FreightManagement
{
    public partial class FreightManagementMember : PageEventHandlingBase
    {
        protected bool isSecurityGuard = false;
        protected bool isVehicleOperator = false;
        protected bool hasRequestPermission = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckCurrentUser();
        }

        private void CheckCurrentUser()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();

            var webUrl = SPContext.Current.Web.Url;
            AdditionalEmployeePositionDAL additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(webUrl);
            isSecurityGuard = additionalEmployeePositionDAL.GetAdditionalPosition(currentEmployee.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);
            
            if (isSecurityGuard == true)
            {
                DateTime currentDateTime = DateTime.Now;
                string fromDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1).ToString(StringConstant.DateFormatyyyyMMddHHmmssfff);
                string toDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1).ToString(StringConstant.DateFormatyyyyMMddHHmmssfff);
                Dictionary<string, string> paramCollection = new Dictionary<string, string>();
                paramCollection.Add("AdminSelectedDate", fromDate);
                paramCollection.Add("AdminSelectedToDate", toDate);
                paramCollection.Add("AdminDeptId", "0");
                paramCollection.Add("AdminVehicleId", "0");
                paramCollection.Add("reqnum", "0");
                paramCollection.Add("searchtype", "0");

                Uri oldUri = this.Page.Request.Url;
                Uri newUri = this.Page.Request.Url.AddParameter(paramCollection);
                URLHelper.RedirectPage(oldUri, newUri);
            }
            else
            {
                DateTime currentDateTime = DateTime.Now;
                string fromDate = $"{DateTime.Now:dd/MM/yyyy}";
                //string toDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1).ToString(StringConstant.DateFormatyyyyMMddHHmmssfff);
                string toDate = fromDate;
                Dictionary<string, string> paramCollection = new Dictionary<string, string>();
                paramCollection.Add("AdminFromDate", fromDate);
                paramCollection.Add("AdminToDate", toDate);
                paramCollection.Add("AdminDeptId", "0");
                paramCollection.Add("AdminVehicleId", "0");

                Uri oldUri = this.Page.Request.Url;
                Uri newUri = this.Page.Request.Url.AddParameter(paramCollection);
                URLHelper.RedirectPage(oldUri, newUri);
            }

            isVehicleOperator = additionalEmployeePositionDAL.GetAdditionalPosition(currentEmployee.ID, StringConstant.AdditionalEmployeePositionModule.FreightManagement, StringConstant.AdditionalEmployeePositionLevelCode.VehicleOperator);
            if (currentEmployee.EmployeeType == StringConstant.EmployeeType.ADUser &&
                (int)Convert.ToDouble(currentEmployee.EmployeeLevel.LookupValue, CultureInfo.InvariantCulture.NumberFormat) < (int)StringConstant.EmployeeLevel.BOD)
            {
                hasRequestPermission = true;
            }
        }
    }
}
