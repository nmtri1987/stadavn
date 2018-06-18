using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveOfAbsenceManagementControl
{
    public partial class LeaveOfAbsenceRequestControl : UserControl
    {
        private NotOvertimeManagementDAL overTimeManagementDAL;
        private const string baseViewID = "3";
        protected void Page_Load(object sender, EventArgs e)
        {
            initialViewGUID();
            GetCurrentUser();
        }
        private void initialViewGUID()
        {
            var url = SPContext.Current.Web.Url;
            overTimeManagementDAL = new NotOvertimeManagementDAL(url);
            var guidViews = overTimeManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            NotOverTimeRequestControl.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

        }
        private void GetCurrentUser()
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
    }
}
