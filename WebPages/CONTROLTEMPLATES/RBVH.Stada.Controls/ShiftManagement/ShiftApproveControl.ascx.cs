using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ShiftManagement
{
    public partial class ShiftApproveControl : UserControl
    {
        private const string BASE_VIEW_ID = "2";
        protected void Page_Load(object sender, EventArgs e)
        {
            var url = SPContext.Current.Web.Url;
            ShiftManagementDAL shiftManagementDal = new ShiftManagementDAL(url);
            var guidViews = shiftManagementDal.GetViewGuildID().Where(x => x.BaseViewID == BASE_VIEW_ID).FirstOrDefault();
            ShiftApproveWebPart.ViewGuid = guidViews.ID.ToString();
        }
    }
}
