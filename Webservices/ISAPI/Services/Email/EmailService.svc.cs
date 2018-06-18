
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.Email;
using System;
using System.Globalization;
using System.ServiceModel.Activation;
using System.Linq;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.DelegationManagement;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.ShiftTime
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EmailService : IEmailService
    {
        readonly EmailTemplateDAL _emailTemplateDAL;
        readonly EmployeeInfoDAL _employeeDAL;
        readonly ChangeShiftManagementDAL _changeShiftManagementDAL;
        readonly NotOvertimeManagementDAL _notOvertimeMangementDAL;
        readonly SendEmailActivity _sendMailActivity;
        readonly ShiftTimeDAL _shiftTimeDAL;
        private string WebUrl;
        public EmailService()
        {
            WebUrl = SPContext.Current.Web.Url;
            _emailTemplateDAL = new EmailTemplateDAL(WebUrl);
            _employeeDAL = new EmployeeInfoDAL(WebUrl);
            _changeShiftManagementDAL = new ChangeShiftManagementDAL(WebUrl);
            _notOvertimeMangementDAL = new NotOvertimeManagementDAL(WebUrl);
            _shiftTimeDAL = new ShiftTimeDAL(WebUrl);
            _sendMailActivity = new SendEmailActivity();
        }
        #region Change_Shift_Email
        /// <summary>
        /// Send email to Requester after 
        /// </summary>
        /// <param name="changeshiftItemId"></param>
        /// <returns></returns>
        /// CALL URL: /_vti_bin/Services/Email/EmailService.svc/SendChangeShiftRequestMail/1/BOD
        public bool SendChangeShiftRequestMail(string changeshiftItemId, string toRole)
        {
            try
            {
                int idValue;
                if (int.TryParse(changeshiftItemId, out idValue))
                {
                    var changeShiftManagementItem = _changeShiftManagementDAL.GetByID(idValue);
                    var changeshiftRequestMailItem = _emailTemplateDAL.GetByKey("ChangeShift_Request");
                    if (changeshiftRequestMailItem != null && changeShiftManagementItem != null)
                    {
                        string email = string.Empty;
                        string employeeFullname = string.Empty;
                        string link = string.Empty;
                        if (toRole.Equals("DH"))
                        {
                            var accountDHItem = _employeeDAL.GetByADAccount(changeShiftManagementItem.DepartmentHead.ID);
                            email = accountDHItem.Email;
                            employeeFullname = accountDHItem.FullName;
                            link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftManager}";
                        }
                        else if (toRole.Equals("BOD"))
                        {
                            var accountBODItem = _employeeDAL.GetByADAccount(changeShiftManagementItem.BOD.ID);
                            email = accountBODItem.Email;
                            employeeFullname = accountBODItem.FullName;
                            link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftBOD}";
                        }

                        if (!string.IsNullOrEmpty(email))
                        {
                            var shiftTimeList = _shiftTimeDAL.GetShiftTimes();

                            string emailBody = HTTPUtility.HtmlDecode(changeshiftRequestMailItem.MailBody);
                            //lookup email
                            var fromShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.FromShift.LookupId).FirstOrDefault();
                            var toShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.ToShift.LookupId).FirstOrDefault();

                            emailBody = string.Format(emailBody, employeeFullname, changeShiftManagementItem.Requester.LookupValue,
                                changeShiftManagementItem.FromDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                changeShiftManagementItem.ToDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                changeShiftManagementItem.Reason, fromShiftItem.Code, toShiftItem.Code);

                            emailBody = emailBody.Replace("#link", link);
                            _sendMailActivity.SendMail(SPContext.Current.Web.Url, changeshiftRequestMailItem.MailSubject, email, true, false, emailBody);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Email Service - SendChangeShiftRequestMail fn",
                                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public bool SendDelegationChangeShiftRequestMail(string changeshiftItemId, string toRole)
        {
            try
            {
                int idValue;
                if (int.TryParse(changeshiftItemId, out idValue))
                {
                    var changeShiftManagementItem = _changeShiftManagementDAL.GetByID(idValue);
                    var changeshiftRequestMailItem = _emailTemplateDAL.GetByKey("ChangeShift_Request");
                    if (changeshiftRequestMailItem != null && changeShiftManagementItem != null)
                    {
                        var siteUrl = SPContext.Current.Site.Url;
                        var link = string.Format(@"{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftApprovalDelegation.aspx?itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", siteUrl, idValue);

                        List<EmployeeInfo> toUsers = new List<EmployeeInfo>();
                        if (toRole.Equals("DH"))
                        {
                            var dhUser = _employeeDAL.GetByADAccount(changeShiftManagementItem.DepartmentHead.ID);
                            toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(siteUrl, dhUser.ID, StringConstant.ChangeShiftList.ListUrl, idValue);
                        }
                        else if (toRole.Equals("BOD"))
                        {
                            var bodUser = _employeeDAL.GetByADAccount(changeShiftManagementItem.BOD.ID);
                            toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(siteUrl, bodUser.ID, StringConstant.ChangeShiftList.ListUrl, idValue);
                        }

                        var shiftTimeList = _shiftTimeDAL.GetShiftTimes();

                        //lookup email
                        var fromShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.FromShift.LookupId).FirstOrDefault();
                        var toShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.ToShift.LookupId).FirstOrDefault();

                        if (toUsers != null)
                        {
                            foreach (var toUser in toUsers)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(toUser.Email))
                                    {
                                        string emailBody = HTTPUtility.HtmlDecode(changeshiftRequestMailItem.MailBody);
                                        emailBody = string.Format(emailBody, toUser.FullName, changeShiftManagementItem.Requester.LookupValue,
                                            changeShiftManagementItem.FromDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                            changeShiftManagementItem.ToDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                            changeShiftManagementItem.Reason, fromShiftItem.Code, toShiftItem.Code);

                                        emailBody = emailBody.Replace("#link", link);
                                        _sendMailActivity.SendMail(siteUrl, changeshiftRequestMailItem.MailSubject, toUser.Email, true, false, emailBody);
                                    }
                                }
                                catch { }
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Email Service - SendDelegationChangeShiftRequestMail fn",
                                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="changeshiftItemId"></param>
        /// <returns></returns>
        /// CALL URL: /_vti_bin/Services/Email/EmailService.svc/SendChangeShiftApproveMail/1
        public bool SendChangeShiftApproveMail(string changeshiftItemId)
        {
            try
            {
                int idValue;
                if (int.TryParse(changeshiftItemId, out idValue))
                {
                    var changeShiftManagementItem = _changeShiftManagementDAL.GetByID(idValue);
                    var changeshiftRequestMailItem = _emailTemplateDAL.GetByKey("ChangeShift_Approve");
                    if (changeshiftRequestMailItem != null && changeShiftManagementItem != null)
                    {
                        var employee = _employeeDAL.GetByID(changeShiftManagementItem.Requester.LookupId);

                        if (employee != null && !string.IsNullOrEmpty(employee.Email))
                        {
                            var shiftTimeList = _shiftTimeDAL.GetShiftTimes();
                            string emailBody = HTTPUtility.HtmlDecode(changeshiftRequestMailItem.MailBody);

                            var fromShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.FromShift.LookupId).FirstOrDefault();
                            var toShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.ToShift.LookupId).FirstOrDefault();

                            //lookup email
                            string link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftMember}";
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                            {
                                link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftAdmin}";
                            }
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                            {
                                link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftManager}";
                            }
                            emailBody = string.Format(emailBody, employee.FullName, changeShiftManagementItem.Requester.LookupValue,
                                changeShiftManagementItem.FromDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                changeShiftManagementItem.ToDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                changeShiftManagementItem.Reason, fromShiftItem.Code, toShiftItem.Code,
                                changeShiftManagementItem.Comment);

                            emailBody = emailBody.Replace("#link", link);
                            _sendMailActivity.SendMail(SPContext.Current.Web.Url, changeshiftRequestMailItem.MailSubject, employee.Email, true, false, emailBody);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Email Service - SendChangeShiftApproveMail fn",
                                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeshiftItemId"></param>
        /// <returns></returns>
        /// CALL URL: /_vti_bin/Services/Email/EmailService.svc/SendChangeShiftRejectMail/1
        public bool SendChangeShiftRejectMail(string changeshiftItemId)
        {
            try
            {
                int idValue;
                if (int.TryParse(changeshiftItemId, out idValue))
                {
                    var changeShiftManagementItem = _changeShiftManagementDAL.GetByID(idValue);
                    var changeshiftRequestMailItem = _emailTemplateDAL.GetByKey("ChangeShift_Reject");
                    if (changeshiftRequestMailItem != null && changeShiftManagementItem != null)
                    {

                        var employee = _employeeDAL.GetByID(changeShiftManagementItem.Requester.LookupId);

                        if (employee != null && !string.IsNullOrEmpty(employee.Email))
                        {
                            var shiftTimeList = _shiftTimeDAL.GetShiftTimes();
                            string emailBody = HTTPUtility.HtmlDecode(changeshiftRequestMailItem.MailBody);

                            var fromShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.FromShift.LookupId).FirstOrDefault();
                            var toShiftItem = shiftTimeList.Where(x => x.ID == changeShiftManagementItem.ToShift.LookupId).FirstOrDefault();
                            //lookup email
                            string link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftMember}";
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                            {
                                link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftAdmin}";
                            }
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                            {
                                link = $"{WebUrl}/{StringConstant.WebPageLinks.ChangeShiftManager}";
                            }
                            emailBody = string.Format(emailBody, employee.FullName, changeShiftManagementItem.Requester.LookupValue,
                                changeShiftManagementItem.FromDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                changeShiftManagementItem.ToDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                changeShiftManagementItem.Reason, fromShiftItem.Code, toShiftItem.Code, changeShiftManagementItem.Comment);
                            emailBody = emailBody.Replace("#link", link);
                            _sendMailActivity.SendMail(SPContext.Current.Web.Url, changeshiftRequestMailItem.MailSubject, employee.Email, true, false, emailBody);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Email Service - SendChangeShiftRejectMail fn",
                                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }
        #endregion

        #region Not_Overtime_Email
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notOvertimeItemId"></param>
        /// <param name="toRole"></param>
        /// <returns></returns>
        /// CALL URL: : /_vti_bin/Services/Email/EmailService.svc/SendNotovertimeRequestRequestEmail/1/BOD
        public bool SendNotOverTimeRequestEmail(string notOvertimeItemId, string toRole)
        {
            try
            {
                int idValue;
                if (int.TryParse(notOvertimeItemId, out idValue))
                {
                    var notOvertimeManagementItem = _notOvertimeMangementDAL.GetByID(idValue);
                    var notOvertimeRequestMailItem = _emailTemplateDAL.GetByKey("LOAbsence_Request");
                    if (notOvertimeRequestMailItem != null && notOvertimeManagementItem != null)
                    {
                        string link = string.Empty;
                        string email = string.Empty;
                        string empployeeFullname = string.Empty;
                        if (toRole.Equals("DH"))
                        {
                            if (notOvertimeManagementItem.DH != null)
                            {
                                var accountDHItem = _employeeDAL.GetByADAccount(notOvertimeManagementItem.DH.ID);
                                email = accountDHItem.Email;
                                empployeeFullname = accountDHItem.FullName;
                                link = $"{WebUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceManager}#tab2";
                            }
                        }
                        else if (toRole.Equals("BOD"))
                        {
                            if (notOvertimeManagementItem.DH != null)
                            {
                                var accountBODItem = _employeeDAL.GetByADAccount(notOvertimeManagementItem.DH.ID);
                                email = accountBODItem.Email;
                                empployeeFullname = accountBODItem.FullName;
                                link = $"{WebUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceBOD}#tab2";
                            }
                        }

                        if (!string.IsNullOrEmpty(email))
                        {
                            string emailBody = HTTPUtility.HtmlDecode(notOvertimeRequestMailItem.MailBody);
                            //lookup email
                            emailBody = string.Format(emailBody, empployeeFullname, notOvertimeManagementItem.Requester.LookupValue,
                                notOvertimeManagementItem.Date.ToString(StringConstant.DateFormatddMMyyyy2),
                                notOvertimeManagementItem.Reason);
                            emailBody = emailBody.Replace("#link", link);
                            _sendMailActivity.SendMail(SPContext.Current.Web.Url, notOvertimeRequestMailItem.MailSubject, email, true, false, emailBody);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Email Service - SendNotOverTimeRequestEmail fn",
                                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public bool SendDelegationNotOverTimeRequestEmail(string notOvertimeItemId, string toRole)
        {
            try
            {
                int idValue;
                if (int.TryParse(notOvertimeItemId, out idValue))
                {
                    var notOvertimeManagementItem = _notOvertimeMangementDAL.GetByID(idValue);
                    var notOvertimeRequestMailItem = _emailTemplateDAL.GetByKey("LOAbsence_Request");
                    if (notOvertimeRequestMailItem != null && notOvertimeManagementItem != null)
                    {
                        var siteUrl = SPContext.Current.Site.Url;
                        string link = string.Format(@"{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceApprovalDelegation.aspx?itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", siteUrl, notOvertimeManagementItem.ID);

                        var dhUser = _employeeDAL.GetByADAccount(notOvertimeManagementItem.DH.ID);
                        List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(siteUrl, dhUser.ID, StringConstant.NotOvertimeList.ListUrl, notOvertimeManagementItem.ID);

                        if (toUsers != null)
                        {
                            foreach (var toUser in toUsers)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(toUser.Email))
                                    {
                                        string emailBody = HTTPUtility.HtmlDecode(notOvertimeRequestMailItem.MailBody);
                                        emailBody = string.Format(emailBody, toUser.FullName, notOvertimeManagementItem.Requester.LookupValue,
                                            notOvertimeManagementItem.Date.ToString(StringConstant.DateFormatddMMyyyy2),
                                            notOvertimeManagementItem.Reason);
                                        emailBody = emailBody.Replace("#link", link);
                                        _sendMailActivity.SendMail(SPContext.Current.Web.Url, notOvertimeRequestMailItem.MailSubject, toUser.Email, true, false, emailBody);
                                    }
                                }
                                catch { }
                            }
                        }

                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Email Service - SendDelegationNotOverTimeRequestEmail fn",
                                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notOvertimeItemId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// CALL URL: /_vti_bin/Services/Email/EmailService.svc/SendNotOvertimeRequestResultEmail/1/Approve
        public bool SendNotOvertimeRequestResultEmail(string notOvertimeItemId, string result)
        {
            try
            {
                int idValue;
                if (int.TryParse(notOvertimeItemId, out idValue))
                {
                    string key = string.Empty;
                    if (result.Equals("Approve"))
                    {
                        key = "LOAbsence_Approve";
                    }
                    else if (result.Equals("Reject"))
                    {
                        key = "LOAbsence_Reject";
                    }
                    if (!string.IsNullOrEmpty(key))
                    {
                        var notOvertimeRequestMailItem = _emailTemplateDAL.GetByKey(key);
                        var notOvertimeManagementItem = _notOvertimeMangementDAL.GetByID(idValue);

                        if (notOvertimeRequestMailItem != null && notOvertimeManagementItem != null)
                        {
                            var employee = _employeeDAL.GetByID(notOvertimeManagementItem.Requester.LookupId);

                            if (employee != null && !string.IsNullOrEmpty(employee.Email))
                            {
                                string emailBody = HTTPUtility.HtmlDecode(notOvertimeRequestMailItem.MailBody);

                                //lookup email
                                string link = $"{WebUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceMember}";
                                if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                                {
                                    link = $"{WebUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceAdmin}";
                                }
                                if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                                {
                                    link = $"{WebUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceManager}";
                                }
                                emailBody = string.Format(emailBody, employee.FullName, notOvertimeManagementItem.Requester.LookupValue,
                                    notOvertimeManagementItem.Date.ToString(StringConstant.DateFormatddMMyyyy2),
                                    notOvertimeManagementItem.Reason, notOvertimeManagementItem.Comment);
                                emailBody = emailBody.Replace("#link", link);
                                _sendMailActivity.SendMail(SPContext.Current.Web.Url, notOvertimeRequestMailItem.MailSubject, employee.Email, true, false, emailBody);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Email Service - SendNotOvertimeRequestResultEmail fn",
                                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }


        #endregion
    }
}