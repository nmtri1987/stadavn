using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.OvertimeManagementControl
{
    public partial class OvertimeApprovalListControl : UserControl
    {
        private OverTimeManagementDAL overTimeManagementDAL;
        private const string BASE_VIEW_ID = "2";
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Guid
            var url = SPContext.Current.Web.Url;
            overTimeManagementDAL = new OverTimeManagementDAL(url);
            var guidViews = overTimeManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == BASE_VIEW_ID).FirstOrDefault();
            OvertimeApprovalControl.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();
        }
    }
}
