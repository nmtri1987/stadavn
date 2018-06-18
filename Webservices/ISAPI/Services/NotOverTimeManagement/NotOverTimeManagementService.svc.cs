using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.ServiceModel.Activation;
using RBVH.Stada.Intranet.Webservices.Model;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.Constants;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Webservices.Helper;
using System.Net;
using System.Text;
using RBVH.Core.SharePoint;
using System.Threading;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.NotOverTimeManagement
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class NotOverTimeManagementService : INotOverTimeManagementService
    {
        public readonly NotOvertimeManagementDAL _notOvertimeManagementDAL;

        public NotOverTimeManagementService()
        {
            var webUrl = SPContext.Current.Web.Url;
            _notOvertimeManagementDAL = new NotOvertimeManagementDAL(webUrl);
        }

        public MessageResult Approve(NotOverTimeApprovalModel notOverTimeApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (notOverTimeApprovalModel.Id > 0)
                {
                    Biz.Models.NotOvertimeManagement notOvertimeManagement = _notOvertimeManagementDAL.GetByID(notOverTimeApprovalModel.Id);
                    string currentApprovalStatus = !string.IsNullOrEmpty(notOvertimeManagement.ApprovalStatus) ? notOvertimeManagement.ApprovalStatus.ToLower() : string.Empty;
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)NotOverTimeErrorCode.CannotApprove, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(notOvertimeManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)NotOverTimeErrorCode.CannotApprove, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = false;
                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    int approverId = notOverTimeApprovalModel.ApproverId;
                    approverId = approverId > 0 ? approverId : notOvertimeManagement.DH.ID;
                    EmployeeInfo currentApprover = _employeeInfoDAL.GetByADAccount(approverId);
                    if (currentApprover != null)
                    {
                        if (currentApprover.ADAccount.ID == spWeb.CurrentUser.ID)
                        {
                            hasApprovalPermission = true;
                        }
                        else
                        {
                            Delegation delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.NotOvertimeList.ListUrl, notOvertimeManagement.ID);
                            if (delegation != null)
                            {
                                hasApprovalPermission = true;
                            }
                        }
                    }

                    if (hasApprovalPermission == true)
                    {
                        notOvertimeManagement.ApprovalStatus = ApprovalStatus.Approved;
                        notOvertimeManagement.Comment = notOverTimeApprovalModel.Comment;
                        var approver = spWeb.AllUsers.GetByID(approverId);
                        notOvertimeManagement.DH = new User
                        {
                            UserName = approver.LoginName,
                            FullName = approver.Name
                        };
                        _notOvertimeManagementDAL.SaveItem(notOvertimeManagement);

                        // Send approval email to employee
                        SendEmail(spWeb.Url, notOverTimeApprovalModel.Id, "LOAbsence_Approve", notOverTimeApprovalModel.ApproverName);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)NotOverTimeErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult Reject(NotOverTimeApprovalModel notOverTimeApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (notOverTimeApprovalModel.Id > 0)
                {
                    Biz.Models.NotOvertimeManagement notOvertimeManagement = _notOvertimeManagementDAL.GetByID(notOverTimeApprovalModel.Id);
                    string currentApprovalStatus = !string.IsNullOrEmpty(notOvertimeManagement.ApprovalStatus) ? notOvertimeManagement.ApprovalStatus.ToLower() : string.Empty;
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)NotOverTimeErrorCode.CannotReject, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(notOvertimeManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)NotOverTimeErrorCode.CannotReject, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasRejectPermission = false;
                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    int approverId = notOverTimeApprovalModel.ApproverId;
                    approverId = approverId > 0 ? approverId : notOvertimeManagement.DH.ID;
                    EmployeeInfo currentApprover = _employeeInfoDAL.GetByADAccount(approverId);
                    if (currentApprover != null)
                    {
                        if (currentApprover.ADAccount.ID == spWeb.CurrentUser.ID)
                        {
                            hasRejectPermission = true;
                        }
                        else
                        {
                            Delegation delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.NotOvertimeList.ListUrl, notOvertimeManagement.ID);
                            if (delegation != null)
                            {
                                hasRejectPermission = true;
                            }
                        }
                    }

                    if (hasRejectPermission == true)
                    {
                        notOvertimeManagement.ApprovalStatus = ApprovalStatus.Rejected;
                        notOvertimeManagement.Comment = notOverTimeApprovalModel.Comment;
                        var approver = spWeb.AllUsers.GetByID(approverId);
                        notOvertimeManagement.DH = new User
                        {
                            UserName = approver.LoginName,
                            FullName = approver.Name
                        };
                        _notOvertimeManagementDAL.SaveItem(notOvertimeManagement);

                        // Send reject email to employee
                        SendEmail(spWeb.Url, notOverTimeApprovalModel.Id, "LOAbsence_Reject", notOverTimeApprovalModel.ApproverName);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)NotOverTimeErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        #region Private Methods

        public void SendEmail(string webUrl, int notOvertimeItemId, string emailKey, string approverName)
        {
            Thread thread = new Thread(delegate ()
            {
                var _emailTemplateDAL = new EmailTemplateDAL(webUrl);
                var _employeeDAL = new EmployeeInfoDAL(webUrl);
                var _shiftTimeDAL = new ShiftTimeDAL(webUrl);
                var _sendMailActivity = new SendEmailActivity();
                var notOvertimeRequestMailItem = _emailTemplateDAL.GetByKey(emailKey);
                var notOvertimeManagementItem = _notOvertimeManagementDAL.GetByID(notOvertimeItemId);

                if (notOvertimeRequestMailItem != null && notOvertimeManagementItem != null)
                {
                    var employee = _employeeDAL.GetByID(notOvertimeManagementItem.Requester.LookupId);

                    if (employee != null && !string.IsNullOrEmpty(employee.Email))
                    {
                        string emailBody = HTTPUtility.HtmlDecode(notOvertimeRequestMailItem.MailBody);

                        //lookup email
                        string link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceMember}";
                        if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                        {
                            link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceAdmin}";
                        }
                        if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                        {
                            link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveOfAbsenceManager}";
                        }
                        emailBody = string.Format(emailBody, employee.FullName, notOvertimeManagementItem.Requester.LookupValue,
                            notOvertimeManagementItem.Date.ToString(StringConstant.DateFormatddMMyyyy2),
                            notOvertimeManagementItem.Reason, notOvertimeManagementItem.Comment, approverName);
                        emailBody = emailBody.Replace("#link", link);
                        _sendMailActivity.SendMail(webUrl, notOvertimeRequestMailItem.MailSubject, employee.Email, true, false, emailBody);
                    }
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        #endregion
    }
}
