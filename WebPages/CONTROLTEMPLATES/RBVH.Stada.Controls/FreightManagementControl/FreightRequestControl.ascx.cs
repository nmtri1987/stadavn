using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Web.UI;
using System.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.FreightManagementControl
{
    public partial class FreightRequestControl : UserControl
    {
        private FreightManagementDAL freightManagementDAL;
        private const string baseViewID = "2";
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCurrentUser();
            var url = SPContext.Current.Web.Url;
            freightManagementDAL = new FreightManagementDAL(url);
            var guidViews = freightManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            FreightRequestWebPart.ViewGuid = guidViews.ID.ToString();
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
