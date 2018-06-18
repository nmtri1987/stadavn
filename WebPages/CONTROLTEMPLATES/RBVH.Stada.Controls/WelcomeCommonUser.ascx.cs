using System;
using System.Web.UI;
using RBVH.Stada.Intranet.WebPages.Utils;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.Web.Script.Serialization;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls
{
    public partial class WelcomeCommonUser : UserControl
    {
        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        EmployeeInfo _employeeInfo;
        public string EmployeeInfo
        {
            get
            {
                var employeeInfoStr = jsSerializer.Serialize(_employeeInfo);
                if (!string.IsNullOrEmpty(employeeInfoStr))
                {
                    employeeInfoStr = employeeInfoStr.Replace(@"\\", "");
                }
                return employeeInfoStr;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        ///     Load data
        /// </summary>
        /// <returns>model in form</returns>
        protected void LoadData()
        {
            if (!SPContext.Current.Web.CurrentUser.IsSiteAdmin && UserPermission.IsCurrentUserInGroup(StringConstant.CommonAccounts))
            {
                var employeeInfo = UserPermission.GetEmployeeInfo();

                // Common user alreay logged in
                if (employeeInfo != null)
                {
                    EmployeeNameLiteral.Text = employeeInfo.FullName;
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowWelcomeCommon", "showWelcomeCommon();", true);
                    employeeInfo.Image = string.Empty;
                    this._employeeInfo = employeeInfo;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideWelcomeCommon", "hideWelcomeCommon();", true);
                }
            }
            else
            {
                try
                {
                    EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
                    //DepartmentDAL departmentDAL = new DepartmentDAL(SPContext.Current.Site.Url);
                    var employee = employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);
                    if (employee != null)
                    {
                        employee.Image = string.Empty;
                        this._employeeInfo = employee;
                    }
                }
                catch { }

                ScriptManager.RegisterStartupScript(this, GetType(), "showWelcome", "showWelcome();", true);
            }
        }
    }
}