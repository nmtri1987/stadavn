using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.BusinessTripManagementControl
{
    public partial class BusinessTripDriverControl : UserControl
    {
        private BusinessTripManagementDAL businessTripManagementDAL;
        private const string baseViewID = "5";
        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
            GetCurrentUser();
        }
        private void InitialViewGUID()
        {
            var url = SPContext.Current.Web.Url;
            businessTripManagementDAL = new BusinessTripManagementDAL(url);
            var guidViews = businessTripManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            BusinessTripDriverWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();
        }

        private void GetCurrentUser()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterDriverLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterDriverLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
        }
    }
}
