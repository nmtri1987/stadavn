using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.OvertimeManagementControl
{
    public partial class OvertimeRequestManagementControl : UserControl
    {
        private OverTimeManagementDAL overTimeManagementDAL;
        private const string BASE_VIEW_ID = "4";
        protected void Page_Load(object sender, EventArgs e)
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
                ParamDepartmentIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
                ParamDepartmentIDHidden.Value = Convert.ToString(currentEmployee.Department.LookupId);
            }
            var url = SPContext.Current.Web.Url;
            overTimeManagementDAL = new OverTimeManagementDAL(url);
            var guidViews = overTimeManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == BASE_VIEW_ID).FirstOrDefault();
            OvertimeRequestList.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();
        }
    }
}
