//-----------------------------------------------------------------------
// <copyright file="Login.aspx.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    using System;
    using System.Web;
    using Microsoft.SharePoint;
    using PageModels;
    using Utils;
    using Biz.Constants;

    /// <summary>
    /// Login form allow user can login with common user
    /// </summary>
    public partial class Login : LayoutsPageBase
    {
        #region Constructors

        public string LogCategory { get; set; }
        /// <summary>
        /// Initializes a new instance of the Login class.
        /// </summary>
        public Login()
        {
            LogCategory = "Login";
        }

        #endregion

        #region Load Page - First load

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">A objects sender </param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Check postback
                if (!Page.IsPostBack)
                {
                    FirstLoad();
                }
            }
            catch (Exception ex)
            {
                // Write log to SharePoint
                ULSLogging.LogError(ex);

                NotificationStatusHelper.SetErrorStatus(Page.Form, WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleError), WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralMessageError));
            }
        }

        /// <summary>
        /// The first action check when form load
        /// - Validate only allow Common user
        /// </summary>
        protected void FirstLoad()
        {
            // Check and only allow Common user
            if (!UserPermission.IsCurrentUserInGroup(StringConstant.CommonAccounts) || SPContext.Current.Web.CurrentUser.IsSiteAdmin)
            {
                // Is AD or System Account
                Response.Redirect(StringConstant.PageOverviewURL);
            }
            else
            {
                // Is Common User
                // Check Logged in
                var employeeInfo = UserPermission.GetEmployeeInfo();
                if (employeeInfo != null)
                {
                    // Have not logged in -> Redirect to Login Page
                    Response.Redirect(StringConstant.PageOverviewURL);
                }
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Event delegate call On Verify
        /// </summary>
        /// <param name="sender">A objects sender </param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected void Button_Verify_Click(object sender, EventArgs e)
        {
            try
            {
                OnVerify();
            }
            catch (Exception ex)
            {
                // Write log to SharePoint
                ULSLogging.LogError(ex);

                NotificationStatusHelper.SetErrorStatus(Page.Form, WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleError), WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralMessageError));
            }
        }

        /// <summary>
        /// Process submit action
        /// 1. Get data from layout
        /// 2. Validate -> show error message?
        /// 3. Check Login
        /// </summary>
        protected void OnVerify()
        {
            // 1. Get data from layout
            // 2. Validate -> show error message?
            // 3. Check Login -> Redirect to Overview Page or show error message?
            var model = GetDataFromLayout();
            if (!Page.IsValid) return;
            bool isSuccess = CheckLogin(model);
            if (isSuccess)
            {
                // Store session IsManager
                bool isManager = !UserPermission.IsCurrentUserInGroup(StringConstant.CommonAccounts);
                HttpContext.Current.Session[StringConstant.IsAdmin] = isManager;

                // Redirect to Overview Page
                Response.Redirect(StringConstant.PageOverviewURL);
            }
            else
            {
                NotificationStatusHelper.SetErrorStatus(Page.Form, WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleError), WebPageResourceHelper.GetResourceString(StringConstant.ResourcesKeyLoginMessageFail));
            }
        }

        /// <summary>
        /// Get data from Layout Controls
        /// </summary>
        /// <returns>ChangePasswordModel: is model</returns>
        protected LoginModel GetDataFromLayout()
        {
            LoginModel changePasswordModel = new LoginModel()
            {
                EmployeeID = TextBox_EmployeeID.Text,
                AuthorizationCode = TextBox_AuthorizationCode.Text,
            };

            return changePasswordModel;
        }

        /// <summary>
        /// Check Username + Password in employee list
        /// </summary>
        /// <param name="model">Login Model store data</param>
        /// <returns>Result: true if successfully</returns>
        protected bool CheckLogin(LoginModel model)
        {
            string passwordEncrypted = StringCipher.GetMd5Hash(model.AuthorizationCode);
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
            var employeeInfo = employeeInfoDAL.GetByEmployeeID(model.EmployeeID);

            //Bug #18 - Tong_Hop_Bosch_11_01_2018 date: 12-01-2018 - Fixed by TRC81HC - Check if login user is common user
            var result = employeeInfo != null && employeeInfo.Password == passwordEncrypted && employeeInfo.EmployeeType == StringConstant.EmployeeType.CommonUser;
            if (result)
                UserPermission.SetEmployeeInfo(employeeInfo);
            else
                UserPermission.SetEmployeeInfo(null);
            return result;
        }

        #endregion
    }
}