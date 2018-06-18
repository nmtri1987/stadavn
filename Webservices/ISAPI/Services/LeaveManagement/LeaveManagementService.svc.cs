using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Activation;
using System.Linq;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.Biz.Constants;
using System.Reflection;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Webservices.Helper;
using System.Text;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using System.IO;
using RBVH.Stada.Intranet.Biz.Helpers;
using Microsoft.SharePoint.Utilities;
using DocumentFormat.OpenXml.Packaging;
using System.ServiceModel.Web;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LeaveManagementService : ILeaveManagementService
    {
        private int EmployeeID { get; set; }
        private DateTime FromDate { get; set; }
        private DateTime ToDate { get; set; }
        private string SiteUrl { get; set; }

        private readonly LeaveManagementDAL _leaveManagementDAL;
        private readonly EmployeeInfoDAL _employeeInfoDAL;
        private readonly DepartmentDAL _departmentDAL;
        private readonly AdditionalEmployeePositionDAL _additionalEmployeePositionDAL;
        private readonly ShiftManagementDAL _shiftManagementDAL;
        private readonly ShiftManagementDetailDAL _shiftManagementDetailDAL;
        private readonly ShiftTimeDAL _shiftTimeDAL;
        private const int MAX_LEVEL = 4; // Văn thư(3) -> Nhân viên(2) -> Nhân Viên Cây Xanh(1) -> Bảo vệ(1) -> Tạp vụ(1)

        public LeaveManagementService()
        {
            this.SiteUrl = SPContext.Current.Site.Url;
            _leaveManagementDAL = new LeaveManagementDAL(this.SiteUrl);
            _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            _departmentDAL = new DepartmentDAL(this.SiteUrl);
            _additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(this.SiteUrl);
            _shiftManagementDAL = new ShiftManagementDAL(this.SiteUrl);
            _shiftManagementDetailDAL = new ShiftManagementDetailDAL(this.SiteUrl);
            _shiftTimeDAL = new ShiftTimeDAL(this.SiteUrl);
        }

        public MessageResult InsertLeaveManagement(LeaveManagementModel leaveManagementModel)
        {
            // Step 1: Check role of request for:   
            if (leaveManagementModel != null)
            {
                try
                {
                    SPWeb spWeb = SPContext.Current.Web;
                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    EmailTemplateDAL _emailTemplateDal = new EmailTemplateDAL(spWeb.Url);

                    var leaveManagementEntity = leaveManagementModel.ToEntity();
                    var requestForUser = _employeeInfoDAL.GetByID(Convert.ToInt16(leaveManagementModel.RequestFor.LookupId));

                    leaveManagementEntity.TLE = leaveManagementModel.TLE;
                    leaveManagementEntity.DH = leaveManagementModel.DH;
                    leaveManagementEntity.BOD = leaveManagementModel.BOD;

                    leaveManagementEntity = _leaveManagementDAL.SetDueDate(leaveManagementEntity);

                    // Step 2: Insert leave entity:
                    int itemId = _leaveManagementDAL.SaveOrUpdate(spWeb, leaveManagementEntity);
                    var dataResult = _leaveManagementDAL.CreateTaskListItem(spWeb, itemId);
                    //Step 3: send mail
                    var requestEmailItem = _emailTemplateDal.GetByKey("LeaveManagement_Request");

                    if (dataResult != null && dataResult.Item1 != null && requestEmailItem != null && dataResult.Item2 != null && dataResult.Item3 != null)
                    {
                        // Send email to TransferWorkTo
                        var transferWorkToEmailItem = _emailTemplateDal.GetByKey("LeaveManagement_TransferWorkTo");
                        var transferWorkToUser = _employeeInfoDAL.GetByID(dataResult.Item1.TransferworkTo.LookupId);
                        _leaveManagementDAL.SendTransferWorkToEmail(dataResult.Item1, transferWorkToEmailItem, transferWorkToUser, spWeb.Url);

                        // Send email to additional users
                        var additionalAprrovers = dataResult.Item3;
                        //dataResult.Item1 : leave item
                        if (additionalAprrovers.Any())
                        {
                            _leaveManagementDAL.SendRequestEmail(dataResult.Item1, requestEmailItem, additionalAprrovers, spWeb.Url);
                        }

                        var currentApprover = dataResult.Item2;
                        if (currentApprover != null)
                        {
                            try
                            {
                                List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(currentApprover.ID, StringConstant.LeaveManagementList.ListUrl, dataResult.Item1.ID);
                                _leaveManagementDAL.SendDelegationRequestEmail(dataResult.Item1, requestEmailItem, toUsers, spWeb.Url);
                            }
                            catch { }
                        }
                    }

                    return new MessageResult
                    {
                        Code = 0,
                        Message = "Success",
                        ObjectId = itemId
                    };
                }
                catch (Exception ex)
                {
                    ULSLogging.Log(new SPDiagnosticsCategory("STADA - LeaveService - InsertLeaveManagement fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));

                    return new MessageResult { Code = (int)LeaveErrorCode.Unexpected, Message = ex.Message, ObjectId = 0 };
                }
            }

            return new MessageResult { Code = (int)LeaveErrorCode.InvalidData, Message = "Invalid data", ObjectId = 0 };
        }

        public MessageResult ApproveLeave(LeaveApprovalModel leaveApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (leaveApprovalModel.Id > 0)
                {
                    Biz.Models.LeaveManagement leaveManagement = _leaveManagementDAL.GetByID(leaveApprovalModel.Id);
                    string currentApprovalStatus = leaveManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)LeaveErrorCode.CannotApprove, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(leaveManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)LeaveErrorCode.CannotApprove, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(leaveApprovalModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(leaveApprovalModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(leaveManagement.ID, StepModuleList.LeaveManagement.ToString());
                    User nextAssignee = null;
                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();
                        nextAssignee = taskOfOriginalAssignee.NextAssign;

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Approved.ToString();
                            taskOfOriginalAssignee.Description = leaveApprovalModel.Comment;
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(taskOfOriginalAssignee);
                        }
                        else if (isDelegated == true)
                        {
                            TaskManagement clonedTask = _taskManagementDAL.CloneTask(taskOfOriginalAssignee);
                            clonedTask.AssignedTo = approverInfo.ADAccount;
                            clonedTask.TaskStatus = TaskStatusList.Completed.ToString();
                            clonedTask.PercentComplete = 1;
                            clonedTask.TaskOutcome = TaskOutcome.Approved.ToString();
                            clonedTask.Description = leaveApprovalModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }
                    }

                    if (!string.IsNullOrEmpty(leaveApprovalModel.Comment))
                    {
                        leaveManagement.Comment = leaveManagement.Comment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, leaveApprovalModel.Comment));
                    }

                    if (nextAssignee == null)
                    {
                        leaveManagement.ApprovalStatus = StringConstant.ApprovalStatus.Approved.ToString();
                        _leaveManagementDAL.SaveOrUpdate(spWeb, leaveManagement);

                        EmployeeInfo requestFor = null;
                        if (leaveManagement != null)
                        {
                            EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(spWeb.Url);
                            EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("LeaveManagement_Approve");
                            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                            requestFor = _employeeInfoDAL.GetByID(leaveManagement.RequestFor.LookupId);
                            if (emailTemplate != null && requestFor != null)
                            {
                                _leaveManagementDAL.SendApproveMail(leaveManagement, emailTemplate, approverInfo, requestFor, this.SiteUrl);
                            }
                        }

                        if (requestFor != null)
                        {
                            LeaveResult leaveResult = _leaveManagementDAL.InitLeaveInfo(requestFor, leaveManagement.From.ToString(), leaveManagement.To.ToString());

                            //update shift
                            Biz.Models.ShiftTime shiftLeave = _shiftTimeDAL.GetShiftTimeByCode("P");
                            _leaveManagementDAL.UpdateShift(requestFor, leaveResult, shiftLeave.ID);
                        }
                    }
                    else if (nextAssignee != null && taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        _leaveManagementDAL.RunWorkFlow(leaveManagement, taskOfOriginalAssignee);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)LeaveErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult RejectLeave(LeaveApprovalModel leaveApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (leaveApprovalModel.Id > 0)
                {
                    Biz.Models.LeaveManagement leaveManagement = _leaveManagementDAL.GetByID(leaveApprovalModel.Id);
                    string currentApprovalStatus = leaveManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)LeaveErrorCode.CannotReject, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(leaveManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)LeaveErrorCode.CannotReject, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(leaveApprovalModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(leaveApprovalModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(leaveManagement.ID, StepModuleList.LeaveManagement.ToString());
                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Rejected.ToString();
                            taskOfOriginalAssignee.Description = leaveApprovalModel.Comment;
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(taskOfOriginalAssignee);
                        }
                        else if (isDelegated == true)
                        {
                            TaskManagement clonedTask = _taskManagementDAL.CloneTask(taskOfOriginalAssignee);
                            clonedTask.AssignedTo = approverInfo.ADAccount;
                            clonedTask.TaskStatus = TaskStatusList.Completed.ToString();
                            clonedTask.PercentComplete = 1;
                            clonedTask.TaskOutcome = TaskOutcome.Rejected.ToString();
                            clonedTask.Description = leaveApprovalModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }
                    }

                    if (!string.IsNullOrEmpty(leaveApprovalModel.Comment))
                    {
                        leaveManagement.Comment = leaveManagement.Comment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, leaveApprovalModel.Comment));
                    }

                    leaveManagement.ApprovalStatus = StringConstant.ApprovalStatus.Rejected.ToString();
                    _leaveManagementDAL.SaveOrUpdate(spWeb, leaveManagement);

                    EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(spWeb.Url);
                    EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("LeaveManagement_Reject");
                    var requestFor = _employeeInfoDAL.GetByID(leaveManagement.RequestFor.LookupId);
                    if (emailTemplate != null && requestFor != null)
                    {
                        _leaveManagementDAL.SendRejectMail(leaveManagement, emailTemplate, approverInfo, requestFor, spWeb.Url);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)LeaveErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult CancelLeaveManagement(string leaveId)
        {
            if (!string.IsNullOrEmpty(leaveId))
            {
                try
                {
                    Biz.Models.LeaveManagement leaveManagement = _leaveManagementDAL.GetByID(Convert.ToInt32(leaveId));

                    if ((leaveManagement.CreatedBy.ID != leaveManagement.ModifiedBy.ID) ||
                            (leaveManagement.CreatedBy.ID == leaveManagement.ModifiedBy.ID && leaveManagement.ApprovalStatus.ToLower() == ApprovalStatus.Cancelled.ToLower()))
                    {
                        return new MessageResult { Code = (int)LeaveErrorCode.RequestInProgress, Message = MessageResultHelper.GetRequestStatusMessage(leaveManagement.ApprovalStatus.ToLower()), ObjectId = 0 };
                    }

                    leaveManagement.ApprovalStatus = StringConstant.ApprovalStatus.Cancelled;
                    int itemId = _leaveManagementDAL.SaveOrUpdate(SPContext.Current.Web, leaveManagement);
                    if (itemId > 0)
                    {
                        TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
                        IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(itemId, StepModuleList.LeaveManagement.ToString());
                        if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                        {
                            _taskManagementDAL.CloseTasks(taskManagementCollection.ToList());
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new MessageResult { Code = (int)LeaveErrorCode.Unexpected, Message = ex.Message, ObjectId = 0 };
                }
            }
            else
            {
                return new MessageResult { Code = (int)LeaveErrorCode.Unexpected, Message = "Cannot find the item", ObjectId = 0 };
            }

            return new MessageResult { Code = 0, Message = "Successful", ObjectId = 0 };
        }

        public LeaveResult GetAllLeaveInfo(string employeeID, string fromDate, string toDate)
        {
            LeaveResult leaveResult = new LeaveResult(employeeID, fromDate, toDate);

            try
            {
                ProcessInputParams(employeeID, fromDate, toDate);

                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
                EmployeeInfo employeeInfo = _employeeInfoDAL.GetByID(this.EmployeeID);
                if (employeeInfo == null) throw new Exception(string.Format(WebPageResourceHelper.GetResourceString("LeaveManagement_ErrMsg_InvalidUserID"), employeeID));

                UserHelper userHelper = new UserHelper();
                EmployeeInfo currentUser = userHelper.GetCurrentLoginUser();

                bool isRequestFor = false;
                if (currentUser.ID != employeeInfo.ID)
                {
                    isRequestFor = true;
                }

                _leaveManagementDAL.CheckOverlap(leaveResult, employeeInfo, isRequestFor);
                bool isSecurity = _additionalEmployeePositionDAL.GetAdditionalPosition(employeeInfo.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);
                leaveResult = _leaveManagementDAL.ClassifySelectedDays(leaveResult, employeeInfo, isSecurity);
                
                List<Biz.Models.ShiftTime> shiftTimeCollection = _shiftTimeDAL.GetAll();
                leaveResult = _leaveManagementDAL.BuildDefaultShiftTime(leaveResult, _shiftTimeDAL, shiftTimeCollection, employeeInfo);

                leaveResult = _leaveManagementDAL.GetWorkingDayDetails(leaveResult, employeeInfo, isSecurity, shiftTimeCollection);
                if (isSecurity == false)
                {
                    _leaveManagementDAL.ValidateNoneWorkingDays(leaveResult);
                }

                leaveResult = _leaveManagementDAL.CalculateTotalHoursAndDays(leaveResult, shiftTimeCollection, employeeInfo);
                _leaveManagementDAL.ApplyLeavePolicy(leaveResult);

                if (_leaveManagementDAL.ExceedSequenceRequests(leaveResult, employeeInfo)) throw new LeaveException((int)LeaveErrorCode.SequenceLeave, WebPageResourceHelper.GetResourceString("LeaveManagement_ErrMsg_SequenceLeave"));
            }
            catch (LeaveException ex)
            {
                leaveResult.ErrorCode = ex.ErrorCode;
                leaveResult.ErrorMsg = ex.Message;
            }
            catch (Exception ex)
            {
                leaveResult.ErrorCode = -1;
                leaveResult.ErrorMsg = ex.Message;
            }

            return leaveResult;
        }

        public EmployeeApproverModel GetApproversByRequester(string departmentIdStr, string requesterIdStr, string requestForIdStr, string leaveHoursStr)
        {
            return this.GetApproversByRequesterAndTime(departmentIdStr, requesterIdStr, requestForIdStr, string.Empty, string.Empty, leaveHoursStr);
        }

        /// <summary>
        /// Get approvers for leave request
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        /// CALL Url: _vti_bin/services/leavemanagement/leavemanagementservice.svc/GetApprovers/departmentID
        public EmployeeApproverModel GetApproversByRequesterAndTime(string departmentIdStr, string requesterIdStr, string requestForIdStr, string fromDateStr, string toDateStr, string leaveHoursStr)
        {
            EmployeeApproverModel approvers = new EmployeeApproverModel();

            try
            {
                int requesterId = Convert.ToInt32(requesterIdStr);
                int requestForId = Convert.ToInt32(requestForIdStr);

                DateTime fromDate;
                DateTime toDate;
                DateTime.TryParse(fromDateStr, out fromDate);
                DateTime.TryParse(toDateStr, out toDate);

                double leaveHours = 0.0;
                if (!string.IsNullOrEmpty(leaveHoursStr))
                {
                    leaveHours = Convert.ToDouble(leaveHoursStr, CultureInfo.InvariantCulture);
                }
                
                var requestFor = _employeeInfoDAL.GetByID(requestForId);
                var approverLists = _leaveManagementDAL.CreateApprovalList(Convert.ToInt32(departmentIdStr), requestFor.FactoryLocation.LookupId, requestFor);

                if (approverLists.Any())
                {
                    foreach (var item in approverLists)
                    {
                        if (item != null && item.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.TeamLeader)
                        {
                            approvers.Approver1.Add(new ApproverModel { ID = item.ID, FullLoginName = item.FullName, LoginName = item.ADAccount.UserName });
                        }
                        else if (item != null && item.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                        {
                            approvers.Approver2 = new ApproverModel { ID = item.ID, FullLoginName = item.FullName, LoginName = item.ADAccount.UserName };
                        }
                        else if (item != null && item.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.BOD)
                        {
                            approvers.Approver3 = new ApproverModel { ID = item.ID, FullLoginName = item.FullName, LoginName = item.ADAccount.UserName };
                        }
                    }
                }

                if (fromDate != null && toDate != null && fromDate.Date == DateTime.Now.Date && fromDate.Date != new DateTime().Date && leaveHours < 8) //new DateTime() = 1900
                {
                    LeaveResult leaveResult = GetAllLeaveInfo(requestForIdStr, fromDateStr, toDateStr);
                    WorkingDay wkDay = null;
                    if (leaveResult != null && leaveResult.WorkingDays != null && leaveResult.WorkingDays.Count > 0)
                    {
                        wkDay = leaveResult.WorkingDays[0];
                        foreach (var wd in leaveResult.WorkingDays)
                        {
                            if (wd.Shift != null)
                            {
                                int diffDays = wd.Shift.WorkingHourToHour.Date.Subtract(wd.Shift.WorkingHourFromHour.Date).Days;
                                var fromWH = wd.Date.Date.AddTicks(wd.Shift.WorkingHourFromHour.TimeOfDay.Ticks);
                                var toWH = fromWH.AddDays(diffDays).Date.AddTicks(wd.Shift.WorkingHourToHour.TimeOfDay.Ticks);

                                if (leaveResult.From.Value < toWH && leaveResult.To > fromWH)
                                {
                                    wkDay = wd;
                                    break;
                                }
                            }
                        }
                    }

                    if (wkDay != null && wkDay.Shift != null)
                    {
                        List<EmployeeInfo> allFirstApprovers = null;

                        if ((wkDay.Shift.UnexpectedLeaveFirstApprovalRole.LookupId == (int)StringConstant.EmployeePosition.ShiftLeader && requestFor.EmployeePosition.LookupId != (int)StringConstant.EmployeePosition.ShiftLeader) ||
                            (wkDay.Shift.UnexpectedLeaveFirstApprovalRole.LookupId == (int)StringConstant.EmployeePosition.TeamLeader && requestFor.EmployeePosition.LookupId != (int)StringConstant.EmployeePosition.TeamLeader))
                        {
                            string[] viewFields = new string[] { StringConstant.EmployeeInfoList.FullNameField, StringConstant.EmployeeInfoList.ADAccountField };
                            allFirstApprovers = _employeeInfoDAL.GetAccountByFullNamePositionDepartment(string.Empty, new List<int>() { wkDay.Shift.UnexpectedLeaveFirstApprovalRole.LookupId }, requestFor.Department.LookupId, viewFields);
                        }

                        if (allFirstApprovers != null && allFirstApprovers.Count > 0)
                        {
                            ShiftManagementDAL _shiftManagementDAL = new ShiftManagementDAL(this.SiteUrl);
                            ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(this.SiteUrl);

                            DateTime dateToGetShift = wkDay.Date.Date;
                            if (fromDate.Day > 20)
                            {
                                dateToGetShift = dateToGetShift.AddMonths(1);
                            }

                            int leaderId_1 = 0;
                            int leaderId_2 = 0;

                            List<Biz.Models.ShiftManagement> shiftManagements = _shiftManagementDAL.GetByMonthYearDepartment(dateToGetShift.Month, dateToGetShift.Year, requestFor.Department.LookupId, requestFor.FactoryLocation.LookupId);
                            if (shiftManagements != null && shiftManagements.Count > 0)
                            {
                                List<Biz.Models.ShiftManagementDetail> shiftManagementDetails = _shiftManagementDetailDAL.GetByShiftManagementID(shiftManagements[0].ID);

                                List<int> firstApproverIds = allFirstApprovers.Select(e => e.ID).ToList();
                                List<Biz.Models.ShiftManagementDetail> shiftDetailsLeaders = shiftManagementDetails.Where(s => firstApproverIds.Contains(s.Employee.LookupId)).ToList();
                                Biz.Models.ShiftManagementDetail shiftDetailsRequestFor = shiftManagementDetails.Where(s => s.Employee.LookupId == requestForId).FirstOrDefault();

                                if (shiftDetailsLeaders != null && shiftDetailsLeaders.Count > 0 && shiftDetailsRequestFor != null)
                                {
                                    Type typeShiftManagementDetail = typeof(ShiftManagementDetail);
                                    BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                                    PropertyInfo shiftInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", dateToGetShift.Day), bindingFlags);
                                    PropertyInfo shiftApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", dateToGetShift.Day), bindingFlags);
                                    object shiftInfoValue = shiftInfo.GetValue(shiftDetailsRequestFor, null);
                                    object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftDetailsRequestFor, null);
                                    LookupItem shiftTimeObj = shiftInfoValue as LookupItem;
                                    if (shiftTimeObj.LookupId > 0 && shiftApprovalValue != null && shiftApprovalValue.Equals(true))
                                    {
                                        foreach (var shiftDetailsLeader in shiftDetailsLeaders)
                                        {
                                            PropertyInfo shiftInfoTmp = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", dateToGetShift.Day), bindingFlags);
                                            PropertyInfo shiftApprovalInfoTmp = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", dateToGetShift.Day), bindingFlags);
                                            object shiftInfoValueTmp = shiftInfoTmp.GetValue(shiftDetailsLeader, null);
                                            object shiftApprovalValueTmp = shiftApprovalInfoTmp.GetValue(shiftDetailsLeader, null);
                                            if (shiftApprovalValueTmp != null && shiftApprovalValueTmp.Equals(true))
                                            {
                                                LookupItem shiftTimeObjTmp = shiftInfoValueTmp as LookupItem;
                                                if (shiftTimeObj.LookupId == shiftTimeObjTmp.LookupId && requestFor.Manager.LookupId == shiftDetailsLeader.Employee.LookupId && requestFor.ID != shiftDetailsLeader.Employee.LookupId)
                                                {
                                                    leaderId_1 = shiftDetailsLeader.Employee.LookupId;
                                                    break;
                                                }
                                                else if (shiftTimeObj.LookupId == shiftTimeObjTmp.LookupId && requestFor.ID != shiftDetailsLeader.Employee.LookupId)
                                                {
                                                    leaderId_2 = shiftDetailsLeader.Employee.LookupId;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (leaderId_1 > 0)
                            {
                                var shiftLeader = allFirstApprovers.Where(e => e.ID == leaderId_1).FirstOrDefault();
                                if (shiftLeader != null)
                                {
                                    approvers.Approver1.Clear();
                                    approvers.Approver1.Add(new ApproverModel { ID = shiftLeader.ID, FullLoginName = shiftLeader.FullName, LoginName = shiftLeader.ADAccount.UserName });
                                }
                                approvers.Approver2 = null;
                                approvers.Approver3 = null;
                            }
                            else if (leaderId_2 > 0)
                            {
                                var shiftLeader = allFirstApprovers.Where(e => e.ID == leaderId_2).FirstOrDefault();
                                if (shiftLeader != null)
                                {
                                    approvers.Approver1.Clear();
                                    approvers.Approver1.Add(new ApproverModel { ID = shiftLeader.ID, FullLoginName = shiftLeader.FullName, LoginName = shiftLeader.ADAccount.UserName });
                                }
                                approvers.Approver2 = null;
                                approvers.Approver3 = null;
                            }
                            else
                            {
                                approvers.Approver1 = null;
                                approvers.Approver3 = null;
                            }
                        }
                        else
                        {
                            approvers.Approver1 = null;
                            approvers.Approver3 = null;
                        }
                    }
                    else
                    {
                        switch (requestFor.EmployeePosition.LookupId)
                        {
                            case (int)StringConstant.EmployeePosition.DepartmentHead:
                                approvers.Approver1 = null;
                                approvers.Approver2 = null;
                                break;
                            default:
                                approvers.Approver1 = null;
                                approvers.Approver3 = null;
                                break;
                        }
                    }
                }
                else
                {
                    int maxSequenceLeaveHours = 40;
                    string configKey = "LeaveForm_MaxSequenceLeaveHours";
                    string configVal = ConfigurationDAL.GetValue(this.SiteUrl, configKey);
                    if (!string.IsNullOrEmpty(configVal))
                    {
                        int.TryParse(configVal.Trim(), out maxSequenceLeaveHours);
                    }

                    if (requesterId == requestForId)
                    {
                        switch (requestFor.EmployeePosition.LookupId)
                        {
                            case (int)StringConstant.EmployeePosition.TeamLeader:
                                approvers.Approver1 = null;
                                if (leaveHours < maxSequenceLeaveHours)
                                {
                                    approvers.Approver3 = null;
                                }
                                break;
                            case (int)StringConstant.EmployeePosition.DepartmentHead:
                                approvers.Approver1 = null;
                                approvers.Approver2 = null;
                                break;
                            default:
                                if (leaveHours < maxSequenceLeaveHours)
                                {
                                    approvers.Approver3 = null;
                                }
                                break;
                        }
                    }
                    else
                    {
                        approvers.Approver1 = null;
                        switch (requestFor.EmployeePosition.LookupId)
                        {
                            case (int)StringConstant.EmployeePosition.DepartmentHead:
                                approvers.Approver1 = null;
                                approvers.Approver2 = null;
                                break;
                            default:
                                if (leaveHours < maxSequenceLeaveHours)
                                {
                                    approvers.Approver3 = null;
                                }
                                break;
                        }
                    }

                    //if (approvers.Approver1 != null && approvers.Approver1.Count > 0)
                    //{
                    //    var approver1 = approvers.Approver1.First();
                    //    var leaveList = _leaveManagementDAL.GetLeavesInRange(approver1.ID, fromDate, toDate, StringConstant.ApprovalStatus.Approved);
                    //    if (leaveList != null && leaveList.Count > 0)
                    //    {
                    //        approvers.Approver1 = null;
                    //    }
                    //}
                }

                return approvers;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Leave Management Service - GetApprovers",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        ///// <summary>
        ///// Get all leaves info of employee -> SHIFT Module
        ///// </summary>
        ///// <param name="employeeID"></param>
        ///// <param name="fromDate"></param>
        ///// <param name="toDate"></param>
        ///// <returns></returns>
        //public List<LeaveInfo> GetLeavesInRange(string employeeID, string departmentID, string locationID, string fromDate, string toDate)
        //{
        //    try
        //    {
        //        var empId = Convert.ToInt32(employeeID);
        //        var departmentId = Convert.ToInt32(departmentID);
        //        var locationId = Convert.ToInt32(locationID);
        //        var empFromDate = fromDate.ToMMDDYYYYDate(false); // mm-dd-yyyy
        //        var empToDate = toDate.ToMMDDYYYYDate(true);
        //        var leaveList = _leaveManagementDAL.GetLeavesInRange(empId, empFromDate, empToDate, StringConstant.ApprovalStatus.Approved);

        //        return CreateLeaveArray(departmentId, locationId, empId, leaveList, empFromDate, empToDate);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Get all leaves info of employee in department -> SHIFT Module
        ///// </summary>
        ///// <param name="departmentID"></param>
        ///// <param name="fromDate"></param>
        ///// <param name="toDate"></param>
        ///// <returns></returns>
        //public List<EmployeeLeaveInfo> GetLeavesInRangeByDepartment(string locationID, string departmentID, string fromDate, string toDate)
        //{
        //    var result = new List<EmployeeLeaveInfo>();
        //    try
        //    {
        //        var locationId = Convert.ToInt32(locationID);
        //        var departmentId = Convert.ToInt32(departmentID);
        //        DateTime deptFromDate;
        //        DateTime.TryParseExact(fromDate.Trim(), StringConstant.DateFormatMMddyyyy, CultureInfo.InvariantCulture, DateTimeStyles.None, out deptFromDate);
        //        DateTime deptToDate;
        //        DateTime.TryParseExact(toDate.Trim(), StringConstant.DateFormatMMddyyyy, CultureInfo.InvariantCulture, DateTimeStyles.None, out deptToDate);
        //        var leaveList = _leaveManagementDAL.GetLeavesInRangeByDepartment(departmentId, locationId, deptFromDate, deptToDate, StringConstant.ApprovalStatus.Approved);
        //        var empHasLeaveList = leaveList.Select(l => l.RequestFor.LookupId).Distinct();
        //        foreach (var empId in empHasLeaveList)
        //        {
        //            var empLeaves = leaveList.Where(l => l.RequestFor.LookupId == empId);
        //            result.Add(new EmployeeLeaveInfo
        //            {
        //                EmployeeId = empId,
        //                Leaves = CreateLeaveArray(departmentId, locationId, empId, empLeaves, deptFromDate, deptToDate)
        //            });
        //        }

        //        return result;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// Get all leaves info of employee -> SHIFT Module
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<LeaveInfo> GetLeavesInRange(string employeeID, string departmentID, string locationID, string fromDate, string toDate)
        {
            List<LeaveInfo> ret = null;
            try
            {
                var empId = Convert.ToInt32(employeeID);
                var departmentId = Convert.ToInt32(departmentID);
                var locationId = Convert.ToInt32(locationID);
                var empFromDate = fromDate.ToMMDDYYYYDate(false); // mm-dd-yyyy
                var empToDate = toDate.ToMMDDYYYYDate(true);
                var leaveList = _leaveManagementDAL.GetLeavesInRange(empId, empFromDate, empToDate, StringConstant.ApprovalStatus.Approved);

                if (leaveList != null && leaveList.Count > 0)
                {
                    List<Biz.Models.ShiftManagement> shiftCollection = _shiftManagementDAL.GetByDateRangeDepartment(empFromDate, empToDate, departmentId, locationId);
                    List<ShiftManagementDetail> shiftDetailCollection = new List<ShiftManagementDetail>();
                    if (shiftCollection != null && shiftCollection.Count > 0)
                    {
                        ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(this.SiteUrl);
                        foreach (var shift in shiftCollection)
                        {
                            List<ShiftManagementDetail> shiftDetail = _shiftManagementDetailDAL.GetByShiftManagementID(shift.ID);
                            if (shiftDetail != null && shiftDetail.Count() > 0)
                            {
                                shiftDetailCollection.AddRange(shiftDetail);
                            }
                        }
                    }

                    ShiftTimeDAL _shiftTimeDAL = new ShiftTimeDAL(this.SiteUrl);
                    List<Biz.Models.ShiftTime> shiftTimeCollection = _shiftTimeDAL.GetAll();

                    ret = CreateLeaveArray(departmentId, locationId, empId, leaveList, empFromDate, empToDate, shiftCollection, shiftDetailCollection, shiftTimeCollection);
                }
            }
            catch
            {
                return null;
            }

            return ret;
        }

        /// <summary>
        /// Get all leaves info of employee in department -> SHIFT Module
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<EmployeeLeaveInfo> GetLeavesInRangeByDepartment(string locationID, string departmentID, string fromDate, string toDate)
        {
            var result = new List<EmployeeLeaveInfo>();
            try
            {
                var locationId = Convert.ToInt32(locationID);
                var departmentId = Convert.ToInt32(departmentID);
                DateTime deptFromDate;
                DateTime.TryParseExact(fromDate.Trim(), StringConstant.DateFormatMMddyyyy, CultureInfo.InvariantCulture, DateTimeStyles.None, out deptFromDate);
                DateTime deptToDate;
                DateTime.TryParseExact(toDate.Trim(), StringConstant.DateFormatMMddyyyy, CultureInfo.InvariantCulture, DateTimeStyles.None, out deptToDate);
                var leaveList = _leaveManagementDAL.GetLeavesInRangeByDepartment(departmentId, locationId, deptFromDate, deptToDate, StringConstant.ApprovalStatus.Approved);
                var empHasLeaveList = leaveList.Select(l => l.RequestFor.LookupId).Distinct();

                if (empHasLeaveList != null && empHasLeaveList.Count() > 0)
                {
                    List<Biz.Models.ShiftManagement> shiftCollection = _shiftManagementDAL.GetByDateRangeDepartment(deptFromDate, deptToDate, departmentId, locationId);
                    List<ShiftManagementDetail> shiftDetailCollection = new List<ShiftManagementDetail>();
                    if (shiftCollection != null && shiftCollection.Count > 0)
                    {
                        ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(this.SiteUrl);
                        foreach (var shift in shiftCollection)
                        {
                            List<ShiftManagementDetail> shiftDetail = _shiftManagementDetailDAL.GetByShiftManagementID(shift.ID);
                            if (shiftDetail != null && shiftDetail.Count() > 0)
                            {
                                shiftDetailCollection.AddRange(shiftDetail);
                            }
                        }
                    }

                    ShiftTimeDAL _shiftTimeDAL = new ShiftTimeDAL(this.SiteUrl);
                    List<Biz.Models.ShiftTime> shiftTimeCollection = _shiftTimeDAL.GetAll();
                    foreach (var empId in empHasLeaveList)
                    {
                        var empLeaves = leaveList.Where(l => l.RequestFor.LookupId == empId);
                        result.Add(new EmployeeLeaveInfo
                        {
                            EmployeeId = empId,
                            Leaves = CreateLeaveArray(departmentId, locationId, empId, empLeaves, deptFromDate, deptToDate, shiftCollection, shiftDetailCollection, shiftTimeCollection)
                        });
                    }
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get leave by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>LeaveManagementModel</returns>
        public LeaveManagementModel GetLeaveManagementById(string id)
        {
            try
            {
                var leaveManagementId = int.Parse(id);
                var leaveManagementEntity = _leaveManagementDAL.GetByID(leaveManagementId);

                return ConvertToModel(leaveManagementEntity);
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - LeaveService - GetLeaveManagementById fn", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));

                return null;
            }
        }

        public bool HasApprovalPermission(string leaveId)
        {
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            UserHelper userHelper = new UserHelper();
            EmployeeInfo employeeInfo = userHelper.GetCurrentLoginUser(); //_employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);

            string taskQueryStr = string.Format(@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Status' />
                                        <Value Type='Choice'>{0}</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{1}</Value>
                                        </Eq>
                                         <And>
                                            <Eq>
                                                <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                                <Value Type='User'>{2}</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='ItemId' />
                                                <Value Type='Number'>{3}</Value>
                                            </Eq>
                                         </And>
                                     </And>
                                  </And>
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.LeaveManagement.ToString(), employeeInfo.ADAccount.ID, leaveId);
            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                return true;
            }
            return false;
        }

        public DelegationModel GetDelegatedTaskInfo(string Id)
        {
            DelegationModel delegationModel = new DelegationModel();

            int listItemId = 0;
            if (int.TryParse(Id, out listItemId))
            {
                string[] viewFields = new string[] { StringConstant.LeaveManagementList.DHField,StringConstant.LeaveManagementList.BODField,
                    StringConstant.CommonSPListField.ApprovalStatusField,StringConstant.CommonSPListField.CommonDepartmentField};
                string queryStr = $@"<Where>
                                      <Eq>
                                         <FieldRef Name='ID' />
                                         <Value Type='Counter'>{listItemId}</Value>
                                      </Eq>
                                   </Where>";
                List<Biz.Models.LeaveManagement> leaveManagementCollection = _leaveManagementDAL.GetByQuery(queryStr, viewFields);
                if (leaveManagementCollection != null && leaveManagementCollection.Count > 0)
                {
                    Biz.Models.LeaveManagement leaveManagement = leaveManagementCollection[0];
                    StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
                    var currentStep = _stepManagementDAL.GetStepManagement(leaveManagement.ApprovalStatus, StepModuleList.LeaveManagement, leaveManagement.Department.LookupId);
                    if (currentStep != null)
                    {
                        TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
                        string taskQueryStr = string.Format(@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Status' />
                                        <Value Type='Choice'>{0}</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{1}</Value>
                                        </Eq>
                                         <And>
                                            <Eq>
                                                <FieldRef Name='CurrentStepStatus' />
                                                <Value Type='Choice'>{2}</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='ItemId' />
                                                <Value Type='Number'>{3}</Value>
                                            </Eq>
                                         </And>
                                     </And>
                                  </And>
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.LeaveManagement.ToString(), leaveManagement.ApprovalStatus, leaveManagement.ID);
                        List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
                        if (taskManagementCollection != null)
                        {
                            foreach (var taskManagement in taskManagementCollection)
                            {
                                EmployeeInfo assigneeInfo = _employeeInfoDAL.GetByADAccount(taskManagement.AssignedTo.ID);
                                Delegation delegation = DelegationPermissionManager.IsDelegation(assigneeInfo.ID, StringConstant.LeaveManagementList.ListUrl, leaveManagement.ID);
                                delegationModel = new DelegationModel(delegation);
                                if (delegationModel.Requester.LookupId > 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return delegationModel;
        }

        public List<TaskManagementModel> GetTaskHistory(string Id, string fulldata)
        {
            int itemId = 0;
            int allHistoricalData = 0;
            if (int.TryParse(Id, out itemId) == true && int.TryParse(fulldata, out allHistoricalData) == true)
            {
                TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
                List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetTaskHistory(itemId, StepModuleList.LeaveManagement.ToString(), allHistoricalData == 0 ? false : true);
                List<TaskManagementModel> ret = null;
                if (taskManagementCollection != null)
                {
                    ret = new List<TaskManagementModel>();
                    foreach (var taskManagement in taskManagementCollection)
                    {
                        ret.Add(new TaskManagementModel(taskManagement));
                    }
                }
                return ret;
            }
            else
            {
                return null;
            }
        }

        public Stream ExportLeaves(string from, string to, string departmentId, string locationIds)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return null;
            }

            try
            {
                DateTime fromDate;
                DateTime toDate;
                int deptId = 0;
                int.TryParse(departmentId, out deptId);
                if (DateTime.TryParse(from, out fromDate) && DateTime.TryParse(to, out toDate) && fromDate <= toDate)
                {
                    string templateFileName = "ThongKeNghiPhep.xlsx";
                    string destFilePath = "";

                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        string tempFolderPath = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\Tmp", 15);
                        Directory.CreateDirectory(tempFolderPath);
                        ExcelHelper.RemoveOldFiles(tempFolderPath, 1);

                        destFilePath = ExcelHelper.DownloadFile(this.SiteUrl, "Shared Documents", templateFileName, tempFolderPath, string.Empty);

                        using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Open(destFilePath, true))
                        {
                            string sheetName = "Sheet1";

                            DateTime currentDateTime = DateTime.Now;
                            string filterStr = string.Format("Từ: {0} - đến: {1}", fromDate.ToString(StringConstant.DateFormatddMMyyyy2), toDate.ToString(StringConstant.DateFormatddMMyyyy2));
                            string footer = string.Format("Bình Dương, ngày {0} tháng {1} năm {2}", currentDateTime.Day, currentDateTime.Month, currentDateTime.Year);
                            ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "B", 5, filterStr);
                            ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "I", 10, footer);

                            EmployeeInfo currentUser = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);
                            if (currentUser != null)
                            {
                                ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "E", 12, currentUser.FullName);
                                EmployeeInfo departmentHead = _employeeInfoDAL.GetByPositionDepartment(StringConstant.EmployeePosition.DepartmentHead, currentUser.Department.LookupId, currentUser.FactoryLocation.LookupId).FirstOrDefault();
                                if (departmentHead != null)
                                {
                                    ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "J", 12, departmentHead.FullName);
                                }
                            }

                            string[] viewFields = new string[] { LeaveManagementList.RequestForField, CommonSPListField.CommonDepartmentField, LeaveManagementList.FromField, LeaveManagementList.ToField, LeaveManagementList.LeaveHoursField, LeaveManagementList.ReasonField, LeaveManagementList.UnexpectedLeaveField, LeaveManagementList.IsValidRequestField };
                            List<Biz.Models.LeaveManagement> leaveManagementCollection = _leaveManagementDAL.GetLeaves(fromDate, toDate, deptId, locationIds.SplitStringOfLocations().ConvertAll(e => Convert.ToInt32(e)), viewFields);

                            if (leaveManagementCollection != null && leaveManagementCollection.Count > 0)
                            {
                                List<Biz.Models.Department> departmentCollection = _departmentDAL.GetByQuery("<Where><Gt><FieldRef Name='ID'/><Value Type='Counter'>0</Value></Gt></Where>", new string[] { DepartmentList.NameField, DepartmentList.VietnameseNameField });
                                List<Biz.Models.EmployeeInfo> employeeCollection = new List<EmployeeInfo>();
                                List<int> requestForIds = leaveManagementCollection.Select(e => e.RequestFor.LookupId).Distinct().ToList();

                                int pageSize = 500;
                                if (requestForIds.Count > pageSize)
                                {
                                    int numOfPages = requestForIds.Count % pageSize > 0 ? (requestForIds.Count / pageSize) + 1 : requestForIds.Count / pageSize;
                                    for (int i = 0; i < numOfPages; i++)
                                    {
                                        List<Biz.Models.EmployeeInfo> employeeCollectionTmp = _employeeInfoDAL.GetByIDs(requestForIds.GetRange(pageSize * i, i == (numOfPages - 1) ? requestForIds.Count - pageSize * i : pageSize));
                                        if (employeeCollectionTmp != null && employeeCollectionTmp.Count > 0)
                                        {
                                            employeeCollection.AddRange(employeeCollectionTmp);
                                        }
                                    }
                                }
                                else
                                {
                                    employeeCollection = _employeeInfoDAL.GetByIDs(requestForIds);
                                }

                                uint startRowIdx = 8;
                                for (int i = 0; i < leaveManagementCollection.Count; i++)
                                {
                                    Biz.Models.LeaveManagement leaveManagement = leaveManagementCollection[i];

                                    uint newRowIdx = startRowIdx + (uint)i + 1;
                                    ExcelHelper.DuplicateRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx, newRowIdx);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(1), newRowIdx,
                                        (i + 1).ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);

                                    EmployeeInfo requestFor = employeeCollection.Where(e => e.ID == leaveManagement.RequestFor.LookupId).FirstOrDefault();
                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(2), newRowIdx,
                                        requestFor.EmployeeID, DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);
                                    ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(3), newRowIdx, requestFor.FullName);

                                    Department department = departmentCollection.Where(e => e.ID == leaveManagement.Department.LookupId).FirstOrDefault();
                                    string departmentName = department != null ? department.VietnameseName : "";
                                    ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(4), newRowIdx, departmentName);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(5), newRowIdx,
                                        leaveManagement.From.ToString(StringConstant.DateFormatddMMyyyy2), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(6), newRowIdx,
                                        leaveManagement.From.ToString("HH:mm"), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(7), newRowIdx,
                                        leaveManagement.To.ToString(StringConstant.DateFormatddMMyyyy2), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(8), newRowIdx,
                                        leaveManagement.To.ToString("HH:mm"), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(9), newRowIdx,
                                        leaveManagement.LeaveHours.ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    string unexpectedLeave = leaveManagement.UnexpectedLeave == true ? "Có" : "Không";
                                    ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(10), newRowIdx, unexpectedLeave);

                                    string isValidLeave = leaveManagement.IsValidRequest == true ? "Có" : "Không";
                                    ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(11), newRowIdx, isValidLeave);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(12), newRowIdx,
                                        leaveManagement.Reason, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);
                                }

                                ExcelHelper.RemoveRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx);
                                ExcelHelper.Save(spreadSheetDoc.WorkbookPart, sheetName);
                            }
                        }
                    });

                    if (!string.IsNullOrEmpty(destFilePath) && File.Exists(destFilePath))
                    {
                        String headerInfo = string.Format("attachment; filename={0}", Path.GetFileName(destFilePath));
                        WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = headerInfo;
                        WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";

                        return File.OpenRead(destFilePath);
                    }
                }
            }
            catch (Exception ex) { }

            return null;
        }

        /// <summary>
        /// Get shift time by date
        /// </summary>
        /// <param name="date">
        /// Date with format: yyyy-MM-dd
        /// </param>
        /// <param name="empId"></param>
        /// <param name="departmentId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public string GetShiftTimeByDate(string date, string empId, string departmentId, string locationId)
        {
            if (!string.IsNullOrEmpty(date))
            {
                DateTime dtDate;
                bool isValidDate = DateTime.TryParse(date, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtDate);
                if (!isValidDate)
                    dtDate = DateTime.Now;
                if (dtDate.Day >= 21)
                    dtDate = dtDate.AddMonths(1);
                // Get Shift Management
                var shiftManagementList = _shiftManagementDAL.GetByMonthYearDepartment(dtDate.Month, dtDate.Year, int.Parse(departmentId), int.Parse(locationId));
                var shiftManagement = shiftManagementList.FirstOrDefault();
                if (shiftManagement != null)
                {
                    var shiftManagementDetail = _shiftManagementDetailDAL.GetByShiftManagementIDEmployeeID(shiftManagement.ID, int.Parse(empId)).FirstOrDefault();
                    if (shiftManagementDetail != null)
                    {
                        Type typeShiftManagementDetail = typeof(ShiftManagementDetail);
                        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                        PropertyInfo shiftTimeInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", dtDate.Day), bindingFlags);
                        object shiftTimeValue = shiftTimeInfo.GetValue(shiftManagementDetail, null);
                        if (shiftTimeValue != null)
                        {
                            LookupItem shiftTimeValueObj = shiftTimeValue as LookupItem;
                            return shiftTimeValueObj.LookupId > 0 ? shiftTimeValueObj.LookupValue : "HC";
                        }
                    }
                }
            }

            return "HC";
        }

        #region Private Methods
        private void ProcessInputParams(string employeeID, string fromDate, string toDate)
        {
            if (!string.IsNullOrEmpty(employeeID) && !string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                this.EmployeeID = Convert.ToInt32(employeeID);
                this.FromDate = Convert.ToDateTime(fromDate.Trim());
                this.ToDate = Convert.ToDateTime(toDate.Trim());
            }

            if (this.FromDate.Date < DateTime.Now.Date)
            {
                throw new LeaveException((int)LeaveErrorCode.FromDateInvalid, WebPageResourceHelper.GetResourceString("LeaveManagement_ErrMsg_FromDateInvalid"));
            }

            TimeSpan ts = this.ToDate.Subtract(this.FromDate);
            if ((ts.Days < 0) || (ts.Days == 0 && ts.Hours <= 0))
            {
                throw new LeaveException((int)LeaveErrorCode.FromDateRelateToDate, WebPageResourceHelper.GetResourceString("LeaveManagement_ErrMsg_FromDateRelateToDate"));
            }
        }

        private LeaveManagementModel ConvertToModel(Biz.Models.LeaveManagement entity)
        {
            string dateFormat = "MM/dd/yyyy hh:mm:ss tt";
            LeaveManagementModel leaveManagementModel = new LeaveManagementModel()
            {
                Id = entity.ID,
                Requester = entity.Requester,
                RequestFor = entity.RequestFor,
                Department = entity.Department,
                Location = entity.Location,
                FromDate = entity.From.ToString(dateFormat, CultureInfo.GetCultureInfo("en-US")),
                ToDate = entity.To.ToString(dateFormat, CultureInfo.GetCultureInfo("en-US")),
                LeaveHours = entity.LeaveHours,
                Reason = entity.Reason,
                TransferworkTo = entity.TransferworkTo,
                LeftAtDate = entity.LeftAt.ToString(),
                Left = entity.Left,
                UnexpectedLeave = entity.UnexpectedLeave,
                TLE = entity.TLE,
                DH = entity.DH,
                BOD = entity.BOD,
                Approver = entity.TLE ?? entity.DH ?? entity.BOD,
                Comment = entity.Comment,
                ApprovalStatus = entity.ApprovalStatus,
                IsValidRequest = entity.IsValidRequest,
                AdditionalUser = entity.AdditionalUser,
                TotalDays = entity.TotalDays,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy
            };

            if (entity.RequestDueDate != null && entity.RequestDueDate != default(DateTime))
            {
                leaveManagementModel.RequestDueDate = entity.RequestDueDate.ToString(StringConstant.DateFormatddMMyyyy2);
                if (entity.RequestDueDate.Date < DateTime.Now.Date)
                {
                    //leaveManagementModel.RequestExpired = true;
                    leaveManagementModel.RequestExpired = false;
                }
            }

            return leaveManagementModel;
        }

        //private List<LeaveInfo> CreateLeaveArray(int departmentId, int locationId, int empId, IEnumerable<Biz.Models.LeaveManagement> empLeaves, DateTime deptFromDate, DateTime deptToDate)
        //{
        //    var leaveArray = new List<LeaveInfo>();
        //    foreach (var leaveEntity in empLeaves)
        //    {
        //        if (leaveEntity.From.Date == leaveEntity.To.Date && (leaveEntity.From.Date >= deptFromDate.Date && leaveEntity.From.Date <= deptToDate.Date)) // Same date:
        //        {
        //            var allDay = true;
        //            if (leaveEntity.LeaveHours < 8) // Not ALL DAY
        //                allDay = false;

        //            leaveArray.Add(new LeaveInfo
        //            {
        //                Day = leaveEntity.From.Day,
        //                LeaveManagementId = leaveEntity.ID,
        //                AllDay = allDay,
        //                ItemUrl = $"/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId={leaveEntity.ID}"
        //            });
        //        }
        //        else
        //        {
        //            for (var currentDate = leaveEntity.From; currentDate.Date < leaveEntity.To.Date; currentDate = currentDate.AddDays(1))
        //            {
        //                if (currentDate.Date >= deptFromDate.Date && currentDate.Date <= deptToDate.Date) // Compare DATE only
        //                {
        //                    leaveArray.Add(_leaveManagementDAL.InitLeaveInfo(currentDate, departmentId, locationId, empId, leaveEntity.ID));
        //                }
        //            }

        //            // TO date:
        //            if (leaveEntity.To.Date >= deptFromDate.Date && leaveEntity.To.Date <= deptToDate.Date)
        //            {
        //                leaveArray.Add(_leaveManagementDAL.InitLeaveInfo(leaveEntity.To, departmentId, locationId, empId, leaveEntity.ID));
        //            }
        //        }
        //    }

        //    return leaveArray;
        //}

        private List<LeaveInfo> CreateLeaveArray(int departmentId, int locationId, int empId, IEnumerable<Biz.Models.LeaveManagement> empLeaves, DateTime deptFromDate, DateTime deptToDate,
            List<Biz.Models.ShiftManagement> shiftCollection, List<ShiftManagementDetail> shiftDetailCollection, List<Biz.Models.ShiftTime> shiftTimeCollection)
        {
            var leaveArray = new List<LeaveInfo>();
            foreach (var leaveEntity in empLeaves)
            {
                if (leaveEntity.From.Date == leaveEntity.To.Date && (leaveEntity.From.Date >= deptFromDate.Date && leaveEntity.From.Date <= deptToDate.Date)) // Same date:
                {
                    var allDay = true;
                    if (leaveEntity.LeaveHours < 8) // Not ALL DAY
                        allDay = false;

                    leaveArray.Add(new LeaveInfo
                    {
                        Day = leaveEntity.From.Day,
                        LeaveManagementId = leaveEntity.ID,
                        AllDay = allDay,
                        ItemUrl = $"/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId={leaveEntity.ID}"
                    });
                }
                else
                {
                    for (var currentDate = leaveEntity.From; currentDate.Date < leaveEntity.To.Date; currentDate = currentDate.AddDays(1))
                    {
                        if (currentDate.Date >= deptFromDate.Date && currentDate.Date <= deptToDate.Date) // Compare DATE only
                        {
                            leaveArray.Add(_leaveManagementDAL.InitLeaveInfo(currentDate, departmentId, locationId, empId, leaveEntity.ID, shiftCollection, shiftDetailCollection, shiftTimeCollection));
                        }
                    }

                    // TO date:
                    if (leaveEntity.To.Date >= deptFromDate.Date && leaveEntity.To.Date <= deptToDate.Date)
                    {
                        leaveArray.Add(_leaveManagementDAL.InitLeaveInfo(leaveEntity.To, departmentId, locationId, empId, leaveEntity.ID, shiftCollection, shiftDetailCollection, shiftTimeCollection, true));
                    }
                }
            }

            return leaveArray;
        }
        #endregion
    }
}