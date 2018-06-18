using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.BusinessTripManagementControl
{
    public partial class BusinessTripCashierControl : UserControl
    {
        private BusinessTripManagementDAL businessTripManagementDAL;
        
        private const string baseViewID = "6";
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
            BusinessTripCashierWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();
        }
        private void GetCurrentUser()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterCashierLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterCashierLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
        }
    }
}
