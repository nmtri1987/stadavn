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
using System.Linq;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.ChangeShiftManagement
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ChangeShiftManagementService : IChangeShiftManagementService
    {
        public readonly ChangeShiftManagementDAL _changeShiftManagementDAL;

        public ChangeShiftManagementService()
        {
            var webUrl = SPContext.Current.Web.Url;
            _changeShiftManagementDAL = new ChangeShiftManagementDAL(webUrl);
        }

        public MessageResult Approve(ChangeShiftApprovalModel changeShiftApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (changeShiftApprovalModel.Id > 0)
                {
                    Biz.Models.ChangeShiftManagement changeShiftManagement = _changeShiftManagementDAL.GetByID(changeShiftApprovalModel.Id);
                    string currentApprovalStatus = !string.IsNullOrEmpty(changeShiftManagement.ApprovalStatus) ? changeShiftManagement.ApprovalStatus.ToLower() : string.Empty;
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)ChangeShiftErrorCode.CannotApprove, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(changeShiftManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)BusinessTripErrorCode.CannotApprove, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = false;
                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    int approverId = changeShiftApprovalModel.ApproverId;
                    approverId = approverId > 0 ? approverId : changeShiftManagement.DepartmentHead.ID;
                    EmployeeInfo currentApprover = _employeeInfoDAL.GetByADAccount(approverId);
                    if (currentApprover != null)
                    {
                        if (currentApprover.ADAccount.ID == spWeb.CurrentUser.ID)
                        {
                            hasApprovalPermission = true;
                        }
                        else
                        {
                            Delegation delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.ChangeShiftList.ListUrl, changeShiftManagement.ID);
                            if (delegation != null)
                            {
                                hasApprovalPermission = true;
                            }
                        }
                    }

                    if (hasApprovalPermission == true)
                    {
                        changeShiftManagement.ApprovalStatus = ApprovalStatus.Approved;
                        changeShiftManagement.Comment = changeShiftApprovalModel.Comment;
                        var approver = spWeb.AllUsers.GetByID(approverId);
                        changeShiftManagement.DepartmentHead = new User
                        {
                            UserName = approver.LoginName,
                            FullName = approver.Name
                        };
                        _changeShiftManagementDAL.SaveItem(changeShiftManagement);

                        // Send approval email to employee 
                        SendEmail(spWeb.Url, changeShiftApprovalModel.Id, "ChangeShift_Approve", changeShiftApprovalModel.ApproverName);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)ChangeShiftErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult Reject(ChangeShiftApprovalModel changeShiftApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (changeShiftApprovalModel.Id > 0)
                {
                    Biz.Models.ChangeShiftManagement changeShiftManagement = _changeShiftManagementDAL.GetByID(changeShiftApprovalModel.Id);
                    string currentApprovalStatus = !string.IsNullOrEmpty(changeShiftManagement.ApprovalStatus) ? changeShiftManagement.ApprovalStatus.ToLower() : string.Empty;
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)ChangeShiftErrorCode.CannotReject, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(changeShiftManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)BusinessTripErrorCode.CannotReject, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasRejectPermission = false;
                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    int approverId = changeShiftApprovalModel.ApproverId;
                    approverId = approverId > 0 ? approverId : changeShiftManagement.DepartmentHead.ID;
                    EmployeeInfo currentApprover = _employeeInfoDAL.GetByADAccount(approverId);
                    if (currentApprover != null)
                    {
                        if (currentApprover.ADAccount.ID == spWeb.CurrentUser.ID)
                        {
                            hasRejectPermission = true;
                        }
                        else
                        {
                            Delegation delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.ChangeShiftList.ListUrl, changeShiftManagement.ID);
                            if (delegation != null)
                            {
                                hasRejectPermission = true;
                            }
                        }
                    }

                    if (hasRejectPermission == true)
                    {
                        changeShiftManagement.ApprovalStatus = ApprovalStatus.Rejected;
                        changeShiftManagement.Comment = changeShiftApprovalModel.Comment;
                        var approver = spWeb.AllUsers.GetByID(approverId);
                        changeShiftManagement.DepartmentHead = new User
                        {
                            UserName = approver.LoginName,
                            FullName = approver.Name
                        };
                        _changeShiftManagementDAL.SaveItem(changeShiftManagement);

                        // Send reject email to employee
                        SendEmail(spWeb.Url, changeShiftApprovalModel.Id, "ChangeShift_Reject", changeShiftApprovalModel.ApproverName);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)ChangeShiftErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        #region Private Methods

        private void SendEmail(string webUrl, int changeshiftItemId, string emailKey, string approverFullName)
        {
            SendEmailActivity sendMailActity = new SendEmailActivity();

            Thread thread = new Thread(delegate ()
            {
                var _emailTemplateDAL = new EmailTemplateDAL(webUrl);
                var _employeeDAL = new EmployeeInfoDAL(webUrl);
                var _shiftTimeDAL = new ShiftTimeDAL(webUrl);
                var _sendMailActivity = new SendEmailActivity();
                var changeShiftManagementItem = _changeShiftManagementDAL.GetByID(changeshiftItemId);
                var changeshiftRequestMailItem = _emailTemplateDAL.GetByKey(emailKey);
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
                        string link = $"{webUrl}/{StringConstant.WebPageLinks.ChangeShiftMember}";
                        if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                        {
                            link = $"{webUrl}/{StringConstant.WebPageLinks.ChangeShiftAdmin}";
                        }
                        if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                        {
                            link = $"{webUrl}/{StringConstant.WebPageLinks.ChangeShiftManager}";
                        }

                        emailBody = string.Format(emailBody, employee.FullName, changeShiftManagementItem.Requester.LookupValue,
                            changeShiftManagementItem.FromDate.ToString(StringConstant.DateFormatddMMyyyy2),
                            changeShiftManagementItem.ToDate.ToString(StringConstant.DateFormatddMMyyyy2),
                            changeShiftManagementItem.Reason, fromShiftItem.Code, toShiftItem.Code, changeShiftManagementItem.Comment, approverFullName);
                        emailBody = emailBody.Replace("#link", link);
                        _sendMailActivity.SendMail(webUrl, changeshiftRequestMailItem.MailSubject, employee.Email, true, false, emailBody);
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        #endregion
    }
}
