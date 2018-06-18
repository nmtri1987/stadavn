using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    public partial class MyOverTime : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
        }
        private void InitialData()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
            DateTime date = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1,0,0,0);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            this.HiddenStartMonth.Value = firstDayOfMonth.ToString("yyyy-MM-ddTHH:mm:ssZ");
            this.HiddenEndMonth.Value = lastDayOfMonth.ToString("yyyy-MM-ddTHH:mm:ssZ");
            DateTextbox.Text = date.Month + "/" + date.Year;
        }
    }
}
