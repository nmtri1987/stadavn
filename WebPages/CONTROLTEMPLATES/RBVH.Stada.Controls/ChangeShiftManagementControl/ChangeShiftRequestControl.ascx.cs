using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ChangeShiftManagementControl
{
    public partial class ChangeShiftRequestControl : UserControl
    {
        private ChangeShiftManagementDAL changeShiftManagementDal;
        private const string baseViewID = "2";
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCurrentUser();
            var url = SPContext.Current.Web.Url;
            changeShiftManagementDal = new ChangeShiftManagementDAL(url);
            var guidViews = changeShiftManagementDal.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            ChangeShiftRequestWebPart.ViewGuid = guidViews.ID.ToString();
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
