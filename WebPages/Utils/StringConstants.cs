//-----------------------------------------------------------------------
// <copyright file="StringConstants.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RBVH.Stada.Intranet.WebPages.Utils
{
    using System;

    /// <summary>
    /// String constants share all page
    /// </summary>
    public class StringConstants
    {
        /*Lists URL*/
        //public const string LeaveManagementURL = "/Lists/LeaveManagement";
        
        //public const string ShiftTimeURL = "/Lists/ShiftTime";
        //public const string EmployeeShiftTimeURL = "/Lists/EmployeeShiftTime";
        //public const string ShiftManagementURL = "/Lists/ShiftManagement";
        //public const string OvertimeEmployeeDetailsURL = "/Lists/OvertimeEmployeeDetails";

        #region General

        /// <summary>
        /// Source Url
        /// </summary>
        public const string SourceUrl = "SourceUrl";

        /// <summary>
        /// Session IsAdmin
        /// </summary>
        public const string IsAdmin = "IsAdmin";

        /// <summary>
        /// Enable Workflow TaskList
        /// </summary>
        public const string EnableWorkflowTaskList = "EnableWorkflowTaskList";

        /// <summary>
        /// Date format
        /// </summary>
        public const string DateFormat = "MM/dd/yyyy";

        /// <summary>
        /// Date format for CAML Query
        /// </summary>
        public const string DateFormatForCAML = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Date format for Authorization code
        /// </summary>
        [Obsolete("Have not used")]
        public const string DateFormatForAuthorizationCode = "ddMMyyyy";

        #endregion

        #region Groups

        /// <summary>
        /// Group name Common Accounts
        /// </summary>
        public const string CommonAccounts = "Common Accounts";

        /// <summary>
        /// Group name Security
        /// </summary>
        public const string Security = "Security";

        /// <summary>
        /// Group name Administration Department
        /// </summary>
        public const string AdministrationDepartment = "Administration Department";

        /// <summary>
        /// Group name IT Members
        /// </summary>
        public const string ITMembers = "IT Members";

        /// <summary>
        /// Group name IT Contributors
        /// </summary>
        public const string ITContributors = "IT Contributors";

        #endregion
        
        #region Employee Info

        /// <summary>
        /// Log Category name
        /// </summary>
        public const string EmployeeInfoLogCategory = "EmployeeInfo";

        /// <summary>
        /// Employee Login page
        /// </summary>
        public const string EmployeeLogedin = "EmployeeLogin";

        /// <summary>
        /// Page Login Url
        /// </summary>
        public const string PageLoginURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/Login.aspx";

        /// <summary>
        /// Page Change Password Url
        /// </summary>
        public const string PageChangePasswordURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangePassword.aspx";

        /// <summary>
        /// Page Reset Password Url
        /// </summary>
        public const string PageResetPasswordURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/ResetPassword.aspx";

        /// <summary>
        /// Page logout Url
        /// </summary>
        public const string PageLogoutURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/Logout.aspx";

        /// <summary>
        /// List Employee url
        /// </summary>
        public const string EmployeeInfoURL = "/Lists/EmployeeInfo";    // -> List

        /// <summary>
        /// Employee info Field Employee ID
        /// </summary>
        public const string EmployeeInfoFieldEmployeeID = "EmployeeID";

        /// <summary>
        /// Employee info Field Employee Info FullName
        /// </summary>
        public const string EmployeeInfoFieldFullName = "EmployeeInfoFullName";

        /// <summary>
        /// Employee info Field Employee AD Account
        /// </summary>
        public const string EmployeeInfoFieldADAccount = "ADAccount";

        /// <summary>
        /// Employee info Field Employee Department
        /// </summary>
        public const string EmployeeInfoFieldEmployeeInfoDepartment = "EmployeeInfoDepartment";

        /// <summary>
        /// Employee info Field Employee Password
        /// </summary>
        public const string EmployeeInfoFieldPassword = "Password";

        /// <summary>
        /// Overview url
        /// </summary>
        public const string PageOverviewURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/Overview.aspx";

        #endregion

        #region Workflows

        /// <summary>
        /// Status Approved
        /// </summary>
        public const string StatusApproved = "Approved";

        /// <summary>
        /// Status Rejected
        /// </summary>
        public const string StatusRejected = "Rejected";

        /// <summary>
        /// Status In-progress
        /// </summary>
        public const string StatusInProgress = "In-Progress";

        /// <summary>
        /// Status Submitted
        /// </summary>
        public const string StatusSubmitted = "Submitted";

        #endregion

        #region Leave management
        public const string LeaveManagement_NewForm = "/Lists/LeaveManagement/NewForm.aspx";

        /// <summary>
        /// Leave management List url
        /// </summary>
        public const string LeaveManagementURL = "/Lists/LeaveManagement";

        /// <summary>
        /// Leave management New form url
        /// </summary>
        public const string LeaveManagementNewForm = "/Lists/LeaveManagement/NewForm.aspx";

        /// <summary>
        /// Leave management Site page url
        /// </summary>
        public const string SitePageLeaveManagementURL = "/SitePages/LeaveManagement.aspx";

        /// <summary>
        /// Security Leave management Site page url
        /// </summary>
        public const string SitePageSecurityLeaveManagementURL = "/SitePages/SecurityLeaveManagement.aspx";

        #endregion

        #region Shift Management

        /// <summary>
        /// Shift management workflow name
        /// </summary>
        public const string ShiftManagementWorklowName = "ShiftManagementWorkflow";

        /// <summary>
        /// My Shift paging number
        /// </summary>
        public const int MyShiftPagingNumber = 10;

        /// <summary>
        /// My shift number of Next month
        /// </summary>
        public const int MyShiftNumberOfNextMonth = 1;

        /// <summary>
        /// Start Day Number
        /// </summary>
        public const int StartDayNumber = 21;

        /// <summary>
        /// End Day Number
        /// </summary>
        public const int EndDayNumber = 20;

        /// <summary>
        /// Date format for My Shift
        /// </summary>
        public const string DateFormatForMyShift = "MM/yyyy";

        /// <summary>
        /// list url
        /// </summary>
        public const string ShiftTimeURL = "/Lists/ShiftTime";

        /// <summary>
        /// Employee list url
        /// </summary>
        public const string EmployeeShiftTimeURL = "/Lists/EmployeeShiftTime";

        /// <summary>
        /// Shift management list url
        /// </summary>
        public const string ShiftManagementURL = "/Lists/ShiftManagement";

        /// <summary>
        /// Shift request SitePage url
        /// </summary>
        public const string SitePageShiftRequestURL = "/SitePages/ShiftRequest.aspx";

        #endregion

        #region Change Shift Management

        /// <summary>
        /// Change Shift Management New form url
        /// </summary>
        public const string ChangeShiftManagementNewForm = "/Lists/ChangeShiftManagement/NewForm.aspx";

        /// <summary>
        /// Change Shift management site page url
        /// </summary>
        public const string SitePageChangeShiftManagementURL = "/SitePages/ChangeShiftManagement.aspx";
        public const string ChangeShiftManagement_NewForm = "/Lists/ChangeShiftManagement/NewForm.aspx";

        #endregion

        #region Overtime

        /// <summary>
        /// Overtime Employee Details List Url
        /// </summary>
        public const string OvertimeEmployeeDetailsURL = "/Lists/OvertimeEmployeeDetails";

        /// <summary>
        /// Overtime Management List Url
        /// </summary>
        public const string OvertimeManagementURL = "/Lists/OvertimeManagement";

        public const string SitePageOvertimeRequestURL = "/SitePages/OvertimeRequest.aspx";
        public const string SitePageOvertimeManagementURL = "/SitePages/OvertimeManagement.aspx";

        #endregion

        #region Factories
        public const string FactoriesURL = "/Lists/Factories";
        #endregion

        #region Department
        public const string DepartmentURL = "/Lists/Departments";
        #endregion Department


        #region Resources

        /// <summary>
        /// Resource Webpages filename
        /// </summary>
        public const string ResourcesFileWebPages = "RBVHStadaWebpages";

        /// <summary>
        /// Resource key General Message Error
        /// </summary>
        public const string ResourcekeyGeneralMessageError = "General_Message_Error";

        public const string ResourcekeyGeneralTitleError = "General_Prefix_Error";

        public const string ResourcekeyGeneralTitleSuccess = "General_Prefix_Success";

        /// <summary>
        /// Resource key Current Password is invalid
        /// </summary>
        public const string ResourcesKeyChangePasswordCurrentPasswordIsInValid = "ChangePassword_CurrentPassword_IsInvalid";

        /// <summary>
        /// Resource key Message success
        /// </summary>
        public const string ResourcesKeyChangePasswordMessageSuccess = "ChangePassword_Message_Success";

        /// <summary>
        /// Resource key Message fail
        /// </summary>
        public const string ResourcesKeyChangePasswordMessageUnsuccess = "ChangePassword_Message_Unsuccess";

        /// <summary>
        /// Resource key Login fail
        /// </summary>
        public const string ResourcesKeyLoginMessageFail = "Login_Message_Fail";

        /// <summary>
        /// Resource key Reset password employee is invalid
        /// </summary>
        public const string ResourcesKeyResetPasswordEmployeeIsInvalid = "ResetPassword_Employee_IsInvalid";

        /// <summary>
        /// Resource key Reset password success
        /// </summary>
        public const string ResourcesKeyResetPasswordMessageSuccess = "ResetPassword_Message_Success";

        /// <summary>
        /// Resource key Reset password fail
        /// </summary>
        public const string ResourcesKeyResetPasswordMessageUnsuccess = "ResetPassword_Message_Unsuccess";


        #endregion
    }
}