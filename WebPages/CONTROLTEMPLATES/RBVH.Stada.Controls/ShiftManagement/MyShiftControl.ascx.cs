using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ShiftManagement
{
    public partial class MyShiftControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamMyShiftRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamMyShiftRequesterLookupIDHidden.Value = Convert.ToString(currentEmployee.ID);
            }
        }
    }
}
