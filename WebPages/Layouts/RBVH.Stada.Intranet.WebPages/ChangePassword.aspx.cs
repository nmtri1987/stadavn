//-----------------------------------------------------------------------
// <copyright file="ChangePassword.aspx.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;
using RBVH.Core.SharePoint.Extension;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    using System;
    using Microsoft.SharePoint;
    using PageModels;
    using Utils;
    using Biz.Constants;

    /// <summary>
    /// Change Password allow user can change password
    /// </summary>
    public partial class ChangePassword : LayoutsPageBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ChangePassword class.
        /// </summary>
        public ChangePassword()
        {
            LogCategory = "ChangePassword";
        }

        public string LogCategory { get; set; }

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
                NotificationStatusHelper.SetErrorStatus(Page.Form, WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleError), WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralMessageError));

                // Write log to SharePoint
                ULSLogging.LogError(ex);
            }
        }

        /// <summary>
        /// The first action check when form load
        /// - Validate only allow Common user
        /// </summary>
        protected void FirstLoad()
        {
            // Check and only allow Common user
            if (!UserPermission.IsCurrentUserInGroup(StringConstant.CommonAccounts) || (SPContext.Current.Web.CurrentUser.ID == SPContext.Current.Site.SystemAccount.ID))
            {
                // Is AD or System Account
                Response.Redirect(StringConstant.PageOverviewURL);
            }
            else
            {
                // Is Common User
                // Check Logged in
                var employeeInfo = UserPermission.GetEmployeeInfo();
                if (employeeInfo == null)
                {
                    // Have not logged in -> Redirect to Login Page
                    Response.Redirect(StringConstant.PageLoginURL);
                }
                else
                {
                    // Logged in -> Allow access page
                    // Load and update data to layout
                    var model = LoadData();
                    UpdateDataToLayout(model);
                }
            }
        }

        /// <summary>
        /// Load data
        /// </summary>
        /// <returns>ChangePasswordModel: model in form</returns>
        protected ChangePasswordModel LoadData()
        {
            var employeeInfo = UserPermission.GetEmployeeInfo();
            ChangePasswordModel changePasswordModel = new ChangePasswordModel
            {
                EmployeeID = employeeInfo.EmployeeID,
                Employee = $"{employeeInfo.FullName} ({employeeInfo.EmployeeID})",
                CurrentPassword = string.Empty,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
            };

            return changePasswordModel;
        }

        /// <summary>
        /// Update Model to Layout (controls)
        /// </summary>
        /// <param name="model">A Change Password Model store data</param>
        protected void UpdateDataToLayout(ChangePasswordModel model)
        {
            HiddenField_EmployeeID.Value = model.EmployeeID;
            TextBox_Employee.Text = model.Employee;
            TextBox_CurrentPassword.Text = model.CurrentPassword;
            TextBox_NewPassword.Text = model.NewPassword;
            TextBox_ConfirmPassword.Text = model.ConfirmPassword;
        }

        #endregion

        #region Actions 

        /// <summary>
        /// Event delegate call On OK
        /// </summary>
        /// <param name="sender">A object sender</param>
        /// <param name="e">A System.EventArgs store data</param>
        protected void Button_OK_Click(object sender, EventArgs e)
        {
            try
            {
                OnOK();
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
        /// 3. Update Password -> show success / error message?
        /// </summary>
        protected void OnOK()
        {
            // 1. Get data from layout
            // 2. Validate -> show error message?
            // 3. Update Password -> show success / error message?
            var model = GetDataFromLayout();
            if (!Page.IsValid) return;
            bool currentPasswordValid = ValidateCurrentPassword(model);
            if (!currentPasswordValid)
            {
                NotificationStatusHelper.SetErrorStatus(Page.Form,
                    WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleError),
                    WebPageResourceHelper.GetResourceString(
                        StringConstant.ResourcesKeyChangePasswordCurrentPasswordIsInValid));
            }
            else
            {
                bool isSuccess = UpdateNewPassword(model);
                if (!isSuccess)
                    NotificationStatusHelper.SetErrorStatus(Page.Form,
                        WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleSuccess),
                        WebPageResourceHelper.GetResourceString(
                            StringConstant.ResourcesKeyChangePasswordMessageUnsuccess));
                else
                {
                    NotificationStatusHelper.SetInformationStatus(Page.Form,
                        WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleSuccess),
                        WebPageResourceHelper.GetResourceString(
                            StringConstant.ResourcesKeyChangePasswordMessageSuccess));

                    //Log out
                    Response.Redirect(StringConstant.PageLogoutURL);
                }
            }
        }

        /// <summary>
        /// Get data from Layout Controls
        /// </summary>
        /// <returns>Model in form</returns>
        protected ChangePasswordModel GetDataFromLayout()
        {
            ChangePasswordModel changePasswordModel = new ChangePasswordModel
            {
                EmployeeID = HiddenField_EmployeeID.Value,
                Employee = TextBox_Employee.Text,
                CurrentPassword = TextBox_CurrentPassword.Text,
                NewPassword = TextBox_NewPassword.Text,
                ConfirmPassword = TextBox_ConfirmPassword.Text,
            };

            return changePasswordModel;
        }

        /// <summary>
        /// Validate current password and Username is existed in Employee info list
        /// </summary>
        /// <param name="model">A ChangePasswordModel store data</param>
        /// <returns>A string as message error</returns>
        protected bool ValidateCurrentPassword(ChangePasswordModel model)
        {
            // Check Current Password
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
            EmployeeInfo employeeInfo = employeeInfoDAL.GetByEmployeeID(model.EmployeeID);
            var isValid = employeeInfo != null && StringCipher.VerifyMd5Hash(model.CurrentPassword, employeeInfo.Password);

            return isValid;
        }


        /// <summary>
        /// Update new password to Employee Info 
        /// </summary>
        /// <param name="model">A ChangePasswordModel store data</param>
        /// <returns>Result: true if successfully</returns>
        protected bool UpdateNewPassword(ChangePasswordModel model)
        {
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
            var result = employeeInfoDAL.UpdatePassword(model.EmployeeID, model.NewPassword);

            return result;
        }

        /// <summary>
        /// Process Cancel action
        /// 1. Return to Source Url
        /// 2. Return to Overview if source url is empty
        /// </summary>
        /// <param name="sender">A object sender</param>
        /// <param name="e">A EventArgs store data</param>
        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                OnCancel();
            }
            catch (Exception ex)
            {
                // Write log
                ULSLogging.LogError(ex);

                NotificationStatusHelper.SetErrorStatus(Page.Form, WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleError), WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralMessageError));
            }
        }

        /// <summary>
        /// On Cancel action
        /// </summary>
        protected void OnCancel()
        {
            Page.GoBack(StringConstant.PageOverviewURL);
        }

        #endregion
    }
}