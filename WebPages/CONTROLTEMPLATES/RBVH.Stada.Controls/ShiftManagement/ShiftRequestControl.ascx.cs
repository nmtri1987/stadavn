using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ShiftManagement
{
    public partial class ShiftRequestControl : UserControl
    {
        private const string BASE_VIEW_ID = "4";
        protected void Page_Load(object sender, EventArgs e)
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamShiftRequestIDHidden.Value = "";
                ParamDepartmentIDHidden.Value = "";
            }
            else
            {
                ParamShiftRequestIDHidden.Value = Convert.ToString(currentEmployee.ID);
                ParamDepartmentIDHidden.Value = Convert.ToString(currentEmployee.Department.LookupId);
            }
            // Set GUID for Web part
            var url = SPContext.Current.Web.Url;
            ShiftManagementDAL shiftManagementDal = new ShiftManagementDAL(url);
            var guidViews = shiftManagementDal.GetViewGuildID().Where(x => x.BaseViewID == BASE_VIEW_ID).FirstOrDefault();
            ShiftRequestsWebPart.ViewGuid = guidViews.ID.ToString();

            // Check shift create permission:
            btnAddNewOvertime.Visible = UserPermission.HasShiftCreation;
        }
    }
}
