using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.Biz.Constants;
using System.Collections.Generic;
using RBVH.Stada.Intranet.WebPages.Common;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.FreightManagement
{
    public partial class FreightManagementBOD : PageEventHandlingBase
    {
        protected void Page_Load(object sender, EventArgs e)
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
    }
}
