//-----------------------------------------------------------------------
// <copyright file="ResetPassword.aspx.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;
    using Microsoft.SharePoint;
    using PageModels;
    using Utils;
    using System.Web.Services;
    using Models;
    using Biz.Constants;

    /// <summary>
    /// Reset Password page allow reset password
    /// </summary>
    public partial class ResetPassword : LayoutsPageBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ResetPassword class.
        /// </summary>
        public ResetPassword()
        {
            LogCategory = StringConstant.EmployeeInfoLogCategory;
        }

        public string LogCategory { get; set; }

        #endregion

        #region Properties

        ///// <summary>
        ///// Gets the Message error employee is invalid
        ///// </summary>
        //protected string MessageErrorEmployeeIsInValid => $"$Resources:{StringConstant.ResourcesFileWebPages},{StringConstant.ResourcesKeyResetPasswordEmployeeIsInvalid}";

        ///// <summary>
        ///// Gets the Message error reset password success
        ///// </summary>
        //protected string MessageSuccessResetPasswordSuccess
        //{
        //    get { return string.Format("$Resources:{0},{1}", StringConstant.ResourcesFileWebPages, StringConstant.ResourcesKeyResetPasswordMessageSuccess); }
        //}

        ///// <summary>
        ///// Gets the Message error reset password fail
        ///// </summary>
        //protected string MessageErrorResetPasswordUnsuccess
        //{
        //    get { return string.Format("$Resources:{0},{1}", StringConstant.ResourcesFileWebPages, StringConstant.ResourcesKeyResetPasswordMessageUnsuccess); }
        //}

        ///// <summary>
        ///// Gets the Message error general
        ///// </summary>
        //protected string MessageErrorGeneral => $"$Resources:{StringConstant.ResourcesFileWebPages},{StringConstant.ResourcekeyGeneralMessageError}";

        ///// <summary>
        ///// Gets the div success
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override HtmlGenericControl GetDiv_Success()
        //{
        //    return Div_Success;
        //}

        ///// <summary>
        ///// Gets the label success
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override Label GetLabel_Success()
        //{
        //    return Label_Success;
        //}

        ///// <summary>
        ///// Gets the div information
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override HtmlGenericControl GetDiv_Info()
        //{
        //    return Div_Info;
        //}

        ///// <summary>
        ///// Gets the label info
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override Label GetLabel_Info()
        //{
        //    return Label_Info;
        //}

        ///// <summary>
        ///// Gets the div warning
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override HtmlGenericControl GetDiv_Warning()
        //{
        //    return Div_Warning;
        //}

        ///// <summary>
        ///// Gets the label warning
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override Label GetLabel_Warning()
        //{
        //    return Label_Warning;
        //}

        ///// <summary>
        ///// Gets the div error
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override HtmlGenericControl GetDiv_Error()
        //{
        //    return Div_Error;
        //}

        ///// <summary>
        ///// Gets the label error
        ///// </summary>
        ///// <returns>A HtmlGenericControl as div</returns>
        //protected override Label GetLabel_Error()
        //{
        //    return Label_Error;
        //}

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

                //HideAllMessage();
            }
            catch (Exception ex)
            {
                // Write log to SharePoint
                ULSLogging.LogError(ex);

                //ShowMessageError(MessageErrorGeneral);
            }
        }

        /// <summary>
        /// The first action check when form load
        /// - Validate only allow IT Members + System Account
        /// </summary>
        protected void FirstLoad()
        {
            // Check and only allow IT Members + Contributors + System Account// System admin
            if (UserPermission.IsCurrentUserInGroup(StringConstant.ITMembers) || UserPermission.IsCurrentUserInGroup(StringConstant.ITContributors) || UserPermission.IsCurrentUserInGroup(StringConstant.SystemAdmin) || (SPContext.Current.Web.CurrentUser.ID == SPContext.Current.Site.SystemAccount.ID))
            {
                // Load and update data to layout
                var model = LoadData();
                UpdateDataToLayout(model);
            }
            else
            {
                // Not allow access page => return to Overview
                Response.Redirect(SPContext.Current.Web.Url + StringConstant.PageOverviewURL);
            }
        }

        /// <summary>
        /// Load data
        /// </summary>
        /// <returns>model in form</returns>
        protected ResetPasswordModel LoadData()
        {
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);

            var employeeInfos = employeeInfoDAL.GetEmployeeInfos();
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel()
            {
                EmployeeInfos = employeeInfos,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
            };

            if (resetPasswordModel.EmployeeInfos != null)
            {
                resetPasswordModel.EmployeeInfos = resetPasswordModel.EmployeeInfos.OrderBy(e => e.FullName).ToList();
            }

            return resetPasswordModel;
        }

        /// <summary>
        /// Update Model to Layout (controls)
        /// </summary>
        /// <param name="model">model to update layout</param>
        protected void UpdateDataToLayout(ResetPasswordModel model)
        {
            List<ListItem> listItems = new List<ListItem>();
            foreach (var employeeInfo in model.EmployeeInfos)
            {
                listItems.Add(new ListItem()
                {
                    Text = string.Format("{0} ({1})", employeeInfo.FullName, employeeInfo.EmployeeID),
                    Value = employeeInfo.EmployeeID
                });
            }

            //DropDownList_Employee.DataSource = listItems;
            //DropDownList_Employee.DataBind();
            TextBox_NewPassword.Text = model.NewPassword;
            TextBox_ConfirmPassword.Text = model.ConfirmPassword;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Event delegate call On OK
        /// </summary>
        /// <param name="sender">A objects sender </param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected void Button_OK_Click(object sender, EventArgs e)
        {
            try
            {
                OnOK();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                //ShowMessageError(MessageErrorGeneral);
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
            // 3. Update Password -> show success or error message?
            var model = GetDataFromLayout();
            if (Validate(model))
            {
                bool isSuccess = UpdateNewPassword(model);
                if (isSuccess)
                {
                    NotificationStatusHelper.SetInformationStatus(Page.Form,
                    WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleSuccess),
                    WebPageResourceHelper.GetResourceString(
                        StringConstant.ResourcesKeyResetPasswordMessageSuccess));
                    //var message = WebPageResourceHelper.GetResourceString(StringConstant.ResourcesKeyResetPasswordMessageSuccess);
                    //Response.Write($"<script>alert('{message}');</script>");
                    //this.ShowMessageSuccess(MessageSuccessResetPasswordSuccess);
                }
                else
                {
                    NotificationStatusHelper.SetErrorStatus(Page.Form,
                        WebPageResourceHelper.GetResourceString(StringConstant.ResourcekeyGeneralTitleError),
                        WebPageResourceHelper.GetResourceString(
                            StringConstant.ResourcesKeyResetPasswordMessageUnsuccess));
                    // ShowMessageError(MessageErrorResetPasswordUnsuccess);
                }
            }
            else
            {
                // Validate fail!
            }
        }

        /// <summary>
        /// Get data from Layout Controls
        /// </summary>
        /// <returns>Model in form</returns>
        protected ResetPasswordModel GetDataFromLayout()
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel()
            {
                EmployeeID = SelectedEmployeeId.Value,
                NewPassword = TextBox_NewPassword.Text,
                ConfirmPassword = TextBox_ConfirmPassword.Text,
            };

            return resetPasswordModel;
        }

        /// <summary>
        /// Validate:
        /// 1. Check Employee is existed
        /// 2. New Password and Confirm Password is match and pass minimum requirement -> skip!
        /// </summary>
        /// <param name="model">Rest password model</param>
        /// <returns>Result: true is Valid all requirement</returns>
        protected bool Validate(ResetPasswordModel model)
        {
            string messageErrors = string.Empty;

            // 1. Validate Employee
            string messageErrorEmployee = Validate_Employee(model);
            //if (!string.IsNullOrEmpty(messageErrorEmployee))
            //{
            //    messageErrors = AppendMessage(messageErrors, messageErrorEmployee);
            //}

            //// 2. Validate New Password pass requirement -> skip!
            //string messageErrorMinimumRequirement = Validate_MinimumRequirementPassword(model);
            //if (!string.IsNullOrEmpty(messageErrorMinimumRequirement))
            //{
            //    messageErrors = AppendMessage(messageErrors, messageErrorMinimumRequirement);
            //}

            // Check result and show message error
            var isValid = string.IsNullOrEmpty(messageErrors);
            //if (!isValid)
            //{
            //    ShowMessageError(messageErrors);
            //}

            return isValid;
        }

        /// <summary>
        /// Validate employee
        /// </summary>
        /// <param name="model">Reset password model</param>
        /// <returns>Error message</returns>
        protected string Validate_Employee(ResetPasswordModel model)
        {
            bool isValid = !string.IsNullOrEmpty(model.EmployeeID);

            // Set Message
            //var messageError = isValid ? string.Empty : MessageErrorEmployeeIsInValid;

            //return messageError;
            return string.Empty;
        }

        /// <summary>
        /// Validate minimum requirement password
        /// </summary>
        /// <param name="model">Reset password model</param>
        /// <returns>Error message</returns>
        protected string Validate_MinimumRequirementPassword(ResetPasswordModel model)
        {
            string messageError = string.Empty;

            return messageError;
        }

        /// <summary>
        /// Update new password to Employee Info 
        /// </summary>
        /// <param name="model">Reset password model </param>
        /// <returns>Result: true if update successfully</returns>
        protected bool UpdateNewPassword(ResetPasswordModel model)
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
        /// <param name="sender">A objects sender </param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
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

                //ShowMessageError(MessageErrorGeneral);
            }
        }

        /// <summary>
        /// On Cancel return source url or default url
        /// </summary>
        protected void OnCancel()
        {
            //GoBack();
        }

        #endregion


        [WebMethod]
        public static List<EmployeeAutoCompleteResult> GetCommonAccounts(string employeeNameOrId)
        {
            EmployeeInfoDAL employeeInfoDal = new EmployeeInfoDAL(SPContext.Current.Web.Url);

            List<EmployeeAutoCompleteResult> employeeResultList = new List<EmployeeAutoCompleteResult>();
            var employees = employeeInfoDal.GetCommonAccountByFullNameOrId(employeeNameOrId);

            foreach(var employee in employees)
            {
                string displayName = string.Format("{0} ({1})", employee.FullName, employee.EmployeeID);
                employeeResultList.Add(new EmployeeAutoCompleteResult()
                {
                    FullName = employee.FullName,
                    EmployeeId = employee.EmployeeID,
                    DisplayName = displayName
                });
            }
            return employeeResultList;
        }
    }
}