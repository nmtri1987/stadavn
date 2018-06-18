using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Client;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace RBVH.Stada.Intranet.ListEventReceiver.ShiftManagementEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ShiftManagementEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        ///  readonly 
        readonly SendEmailActivity _sendMailActivity;
        EmployeeInfoDAL _employeeInfoDAL;

        public ShiftManagementEventReceiver()
        {
            _sendMailActivity = new SendEmailActivity();
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            try
            {
                var itemId = properties.ListItem.ID;
                SendEmailToApprover(properties.Web, itemId);
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA -  Shift Management Event Receiver - ItemAdded fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        //public override void ItemUpdated(SPItemEventProperties properties)
        //{
        //    base.ItemUpdated(properties);
        //    try
        //    {
        //        //_employeeInfoDAL = new EmployeeInfoDAL(properties.WebUrl);
        //        //var itemId = properties.ListItem.ID;
        //        //string modifiedByName = string.Empty;
        //        //var modifiedByString = Convert.ToString(properties.ListItem["Modified By"]);
        //        //SPFieldUser spUserField = (SPFieldUser)properties.ListItem.Fields.GetField("Modified By");
        //        //if (spUserField != null)
        //        //{
        //        //    SPFieldUserValue spNewUserFieldValue = (SPFieldUserValue)spUserField.GetFieldValue(modifiedByString);
        //        //    SPUser spModifiedByUser = properties.Web.EnsureUser(spNewUserFieldValue.LookupValue);
        //        //    var modifiedBy = _employeeInfoDAL.GetByADAccount(spModifiedByUser.ID);
        //        //    modifiedByName = modifiedBy != null ? modifiedBy.FullName : string.Empty;
        //        //}

        //        //SendEmailToApprover(properties.Web, itemId, modifiedByName);
        //    }
        //    catch (Exception ex)
        //    {
        //        ULSLogging.Log(new SPDiagnosticsCategory("STADA -  Shift Management Event Receiver - ItemUpdated fn",
        //            TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
        //        string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
        //    }
        //}

        private void SendEmailToApprover(SPWeb web, int itemId, string modifiedBy = "")
        {
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(web.Url);
            EmailTemplateDAL emailTemplateDAL = new EmailTemplateDAL(web.Url);
            ShiftManagementDAL shiftManagementDAL = new ShiftManagementDAL(web.Url);

            var shiftManagementItem = shiftManagementDAL.GetByID(itemId);
            if (shiftManagementItem != null)
            {

                SPUser departmentHeadSPUser = web.EnsureUser(shiftManagementItem.ApprovedBy.UserName);

                var departmentHead = employeeInfoDAL.GetByADAccount(departmentHeadSPUser.ID);
                if (departmentHead != null)
                {
                    var requestEmailItem = emailTemplateDAL.GetByKey("ShiftManagement_Request");
                    if (requestEmailItem != null)
                    {
                        string emailBody = HTTPUtility.HtmlDecode(requestEmailItem.MailBody);
                        if (!string.IsNullOrEmpty(departmentHead.Email) && !string.IsNullOrEmpty(emailBody))
                        {
                            string link = string.Format("{0}/SitePages/ShiftApproval.aspx?itemId={1}&Source={0}/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementManager.aspx", web.Url, shiftManagementItem.ID);
                            var department = DepartmentListSingleton.GetDepartmentByID(shiftManagementItem.Department.LookupId, web.Url);
                            emailBody = string.Format(emailBody, departmentHead.FullName, string.IsNullOrEmpty(modifiedBy) ? shiftManagementItem.Requester.LookupValue : modifiedBy, shiftManagementItem.Month,
                                shiftManagementItem.Year, shiftManagementItem.Department.LookupValue, department.VietnameseName);

                            emailBody = emailBody.Replace("#link", link);
                            _sendMailActivity.SendMail(web.Url, requestEmailItem.MailSubject, departmentHead.Email, true, false, emailBody);

                            List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(web.Url, departmentHead.ID, StringConstant.ShiftManagementList.ListUrl, shiftManagementItem.ID);
                            link = string.Format("{0}/SitePages/ShiftApproval.aspx?subSection=ShiftManagement&itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", web.Url, shiftManagementItem.ID);
                            if (toUsers != null)
                            {
                                foreach (var toUser in toUsers)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(toUser.Email))
                                        {
                                            emailBody = HTTPUtility.HtmlDecode(requestEmailItem.MailBody);
                                            emailBody = string.Format(emailBody, toUser.FullName, string.IsNullOrEmpty(modifiedBy) ? shiftManagementItem.Requester.LookupValue : modifiedBy, shiftManagementItem.Month,
                                            shiftManagementItem.Year, shiftManagementItem.Department.LookupValue, department.VietnameseName);
                                            emailBody = emailBody.Replace("#link", link);
                                            _sendMailActivity.SendMail(web.Url, requestEmailItem.MailSubject, toUser.Email, true, false, emailBody);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}