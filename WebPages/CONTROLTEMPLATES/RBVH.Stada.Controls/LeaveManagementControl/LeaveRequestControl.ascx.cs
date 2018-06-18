using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveManagementControl
{
    public partial class LeaveRequestControl : UserControl
    {
        private LeaveManagementDAL LeaveManagementDal;
        private const string baseViewID = "2";
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCurrentUser();
            var url = SPContext.Current.Web.Url;
            LeaveManagementDal = new LeaveManagementDAL(url);
            var guidViews = LeaveManagementDal.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            LeaveRequestWebPart.ViewGuid = guidViews.ID.ToString();
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
