using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    public partial class LeaveManagement : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = Convert.ToString(currentEmployee.ID);
            }
            ShowToDayLeaveButton();
        }

        private void ShowToDayLeaveButton()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if(currentEmployee != null )
            {
                if(userHelper.IsEmployeeHasPosition(currentEmployee.ID, Biz.Constants.StringConstant.EmployeePosition.SecurityGuard))
                {
                    btnTodayLeave.Attributes.Add("style", "display:block");
                }
                else
                {
                    //hide button today leave
                    btnTodayLeave.Attributes.Add("style", "display:none");
                }
            }
            else
            {
                btnTodayLeave.Attributes.Add("style", "display:none");
            }
        }
    }



}
