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
using RBVH.Stada.Intranet.Biz.Constants;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using RBVH.Stada.Intranet.WebPages.Utils;
using System.IO;
using System.ServiceModel.Web;
using Microsoft.SharePoint.Utilities;
using RBVH.Stada.Intranet.Biz.Helpers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using RBVH.Stada.Intranet.Webservices.Helper;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.DelegationManagement;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.FreightManagement
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class FreightManagementService : IFreightManagementService
    {
        FreightManagementDAL _freightManagementDAL;
        FreightDetailsDAL _freightDetailsDAL;
        FreightVehicleDAL _freightVehicleDAL;
        FreightReceiverDepartmentDAL _freightReceiverDepartmentDAL;

        public FreightManagementService()
        {
            string siteUrl = SPContext.Current.Site.Url;
            _freightManagementDAL = new FreightManagementDAL(siteUrl);
            _freightDetailsDAL = new FreightDetailsDAL(siteUrl);
            _freightVehicleDAL = new FreightVehicleDAL(siteUrl);
            _freightReceiverDepartmentDAL = new FreightReceiverDepartmentDAL(siteUrl);
        }

        public FreightManagementApproverModel GetApprovers(string departmentIdStr, string locationIdStr)
        {
            FreightManagementApproverModel approvers = new FreightManagementApproverModel();

            try
            {
                CommonApproverModel freightManagementApprover = _freightManagementDAL.CreateApprovalList(Convert.ToInt32(departmentIdStr), Convert.ToInt32(locationIdStr));
                if (freightManagementApprover != null)
                {
                    if (freightManagementApprover.Approver1 != null)
                        approvers.Approver1 = new ApproverModel() { ID = freightManagementApprover.Approver1.ID, FullLoginName = freightManagementApprover.Approver1.ADAccount.FullName, LoginName = freightManagementApprover.Approver1.ADAccount.UserName };

                    if (freightManagementApprover.Approver2 != null)
                        approvers.Approver2 = new ApproverModel() { ID = freightManagementApprover.Approver2.ID, FullLoginName = freightManagementApprover.Approver2.ADAccount.FullName, LoginName = freightManagementApprover.Approver2.ADAccount.UserName };

                    if (freightManagementApprover.Approver3 != null)
                        approvers.Approver3 = new ApproverModel() { ID = freightManagementApprover.Approver3.ID, FullLoginName = freightManagementApprover.Approver3.ADAccount.FullName, LoginName = freightManagementApprover.Approver3.ADAccount.UserName };
                }

                return approvers;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Freight Management Service - GetApprovers",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<EmployeeDepartmentModel> GetBringerList(string departmentId, string locationIds)
        {
            List<EmployeeDepartmentModel> employeeInDepartmentList = new List<EmployeeDepartmentModel>();
            List<EmployeeInfo> employeeInfoList = new List<EmployeeInfo>();
            try
            {
                int currentDepartmentId = int.Parse(departmentId);
                string siteUrl = SPContext.Current.Site.Url;
                var lcid = CultureInfo.CurrentUICulture.LCID;
                if (currentDepartmentId > 0)
                {
                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
                    string[] viewFields = new string[] { EmployeeInfoList.EmployeeIDField, EmployeeInfoList.FullNameField, EmployeeInfoList.DepartmentField, EmployeeInfoList.FactoryLocationField };
                    var employees = _employeeInfoDAL.GetByDepartment(currentDepartmentId, locationIds.SplitStringOfLocations().ConvertAll(e => Convert.ToInt32(e)), (int)StringConstant.EmployeeLevel.BOD - 1, viewFields);
                    Department department = DepartmentListSingleton.GetDepartmentByID(currentDepartmentId, siteUrl);
                    string deptName = CultureInfo.CurrentUICulture.LCID == 1033 ? department.Name : department.VietnameseName;
                    foreach (var item in employees)
                    {
                        employeeInDepartmentList.Add(new EmployeeDepartmentModel
                        {
                            ID = item.ID,
                            DepartmentId = item.Department.LookupId,
                            FullName = item.FullName,
                            LocationId = item.FactoryLocation.LookupId,
                            EmployeeId = item.EmployeeID,
                            DepartmentName = deptName
                        });
                    }
                }
                return employeeInDepartmentList;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                    new SPDiagnosticsCategory("STADA -Freight Service - GetBringerList fn",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public FreightManagementModel GetFreightManagementById(string Id, string loadDetails)
        {
            FreightManagementModel freightManagementModel = new FreightManagementModel();

            try
            {
                int itemId;
                if (!string.IsNullOrEmpty(Id) && int.TryParse(Id, out itemId))
                {
                    bool isLoadDetails = Boolean.Parse(loadDetails);
                    Biz.Models.FreightManagement freightManagement = _freightManagementDAL.GetByID(itemId);
                    freightManagementModel = ConvertToModel(freightManagement);
                    if (isLoadDetails == true)
                    {
                        freightManagementModel.FreightDetails = GetFreightDetailsByParentId(Id);
                    }
                }
            }
            catch { }

            return freightManagementModel;
        }

        public List<FreightDetailsModel> GetFreightDetailsByParentId(string parentId)
        {
            List<FreightDetailsModel> freightDetailsModelCollection = new List<FreightDetailsModel>();

            int parentIdOut;
            if (!string.IsNullOrEmpty(parentId) && int.TryParse(parentId, out parentIdOut))
            {
                List<FreightDetails> freightDetailsCollection = _freightDetailsDAL.GetItemsByParentId(parentIdOut);
                if (freightDetailsCollection != null)
                {
                    foreach (FreightDetails freightDetails in freightDetailsCollection)
                    {
                        FreightDetailsModel freightDetailsModel = ConvertToModel(freightDetails);
                        freightDetailsModelCollection.Add(freightDetailsModel);
                    }
                }
            }

            return freightDetailsModelCollection;
        }

        public MessageResult InsertFreight(FreightManagementModel freightManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (freightManagementModel.Id > 0)
                {
                    Biz.Models.FreightManagement freightObj = _freightManagementDAL.GetByID(freightManagementModel.Id);
                    if (freightObj.ModifiedBy.ID != freightObj.CreatedBy.ID && freightObj.ApprovalStatus.ToLower() != ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)FreightErrorCode.CannotUpdate, Message = MessageResultHelper.GetRequestStatusMessage(freightObj.ApprovalStatus), ObjectId = 0 };
                    }
                }

                Biz.Models.FreightManagement freightManagement = freightManagementModel.ToEntity();

                CommonApproverModel freightManagementApprover = _freightManagementDAL.CreateApprovalList(freightManagement.Department.LookupId, freightManagement.Location.LookupId);
                if (freightManagementApprover != null)
                {
                    if (freightManagementApprover.Approver1 != null)
                        freightManagement.DH = new User() { ID = freightManagementApprover.Approver1.ID, FullName = freightManagementApprover.Approver1.ADAccount.FullName, UserName = freightManagementApprover.Approver1.ADAccount.UserName };

                    if (freightManagementApprover.Approver2 != null)
                        freightManagement.BOD = new User() { ID = freightManagementApprover.Approver2.ID, FullName = freightManagementApprover.Approver2.ADAccount.FullName, UserName = freightManagementApprover.Approver2.ADAccount.UserName };

                    if (freightManagementApprover.Approver3 != null)
                        freightManagement.AdminDept = new User() { ID = freightManagementApprover.Approver3.ID, FullName = freightManagementApprover.Approver3.ADAccount.FullName, UserName = freightManagementApprover.Approver3.ADAccount.UserName };
                }

                freightManagement = _freightManagementDAL.SetDueDate(freightManagement);

                int itemId = _freightManagementDAL.SaveOrUpdate(spWeb, freightManagement);
                if (itemId > 0 && freightManagementModel.FreightDetails != null && freightManagementModel.FreightDetails.Count > 0)
                {
                    // Edit -> DELETE all details -> Insert new items
                    if (freightManagementModel.Id > 0)
                    {
                        List<FreightDetails> existingFreightDetails = _freightDetailsDAL.GetItemsByParentId(freightManagementModel.Id);
                        List<int> freightDetailIds = existingFreightDetails.Select(x => x.ID).ToList();
                        _freightDetailsDAL.DeleteItems(freightDetailIds);
                    }

                    // Insert Freight Details
                    List<FreightDetails> freightDetailsCollection = new List<FreightDetails>();
                    foreach (FreightDetailsModel freightDetailsModel in freightManagementModel.FreightDetails)
                    {
                        FreightDetails freightDetailsItem = freightDetailsModel.ToEntity();
                        freightDetailsItem.FreightManagementID = new LookupItem() { LookupId = itemId, LookupValue = itemId.ToString() };
                        freightDetailsCollection.Add(freightDetailsItem);
                    }

                    _freightDetailsDAL.SaveOrUpdate(spWeb, freightDetailsCollection);
                }

                freightManagement = _freightManagementDAL.GetByID(itemId);
                if ((freightManagementModel.Id <= 0 && itemId > 0) || (freightManagementModel.Id > 0 && freightManagement.ApprovalStatus == StringConstant.ApprovalStatus.Rejected))
                {
                    _freightManagementDAL.StartWorkFlow(spWeb, freightManagement, itemId);
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)FreightErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult ApproveFreight(FreightManagementModel freightManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (freightManagementModel.Id > 0)
                {
                    Biz.Models.FreightManagement freightManagement = _freightManagementDAL.GetByID(freightManagementModel.Id);
                    string currentApprovalStatus = freightManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)FreightErrorCode.CannotApprove, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(freightManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)FreightErrorCode.CannotApprove, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(freightManagementModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(freightManagementModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(freightManagement.ID, StepModuleList.FreightManagement.ToString());

                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Approved.ToString();
                            taskOfOriginalAssignee.Description = freightManagementModel.Comment;
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
                            clonedTask.Description = freightManagementModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }

                        if (!string.IsNullOrEmpty(freightManagementModel.Comment))
                        {
                            freightManagement.Comment = freightManagement.Comment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, freightManagementModel.Comment));
                        }
                        freightManagement.HighPriority = freightManagementModel.HighPriority;

                        EmployeeInfo currentStepApprover = _employeeInfoDAL.GetByADAccount(assigneeId);
                        _freightManagementDAL.RunWorkFlow(freightManagement, taskOfOriginalAssignee, approverInfo, currentStepApprover);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)FreightErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult RejectFreight(FreightManagementModel freightManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (freightManagementModel.Id > 0)
                {
                    Biz.Models.FreightManagement freightManagement = _freightManagementDAL.GetByID(freightManagementModel.Id);
                    string currentApprovalStatus = freightManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)FreightErrorCode.CannotReject, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(freightManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)FreightErrorCode.CannotReject, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(freightManagementModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(freightManagementModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(freightManagement.ID, StepModuleList.FreightManagement.ToString());
                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Rejected.ToString();
                            taskOfOriginalAssignee.Description = freightManagementModel.Comment;
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
                            clonedTask.Description = freightManagementModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }
                    }

                    if (!string.IsNullOrEmpty(freightManagementModel.Comment))
                    {
                        freightManagement.Comment = freightManagement.Comment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, freightManagementModel.Comment));
                    }

                    freightManagement.ApprovalStatus = StringConstant.ApprovalStatus.Rejected.ToString();
                    _freightManagementDAL.SaveOrUpdate(spWeb, freightManagement);

                    EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(spWeb.Url);
                    EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("FreightManagement_Reject");
                    EmployeeInfo toUser = _employeeInfoDAL.GetByID(freightManagement.Requester.LookupId);
                    _freightManagementDAL.SendEmail(freightManagement, emailTemplate, approverInfo, toUser, spWeb.Url, false);
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)FreightErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult CancelFreight(string freightId)
        {
            if (!string.IsNullOrEmpty(freightId))
            {
                try
                {
                    Biz.Models.FreightManagement freightManagement = _freightManagementDAL.GetByID(Convert.ToInt32(freightId));
                    if (freightManagement.ID > 0)
                    {
                        string currentApprovalStatus = freightManagement.ApprovalStatus.ToLower();
                        if (freightManagement.CreatedBy.ID == freightManagement.ModifiedBy.ID && freightManagement.CreatedBy.ID == SPContext.Current.Web.CurrentUser.ID &&
                            (currentApprovalStatus != ApprovalStatus.Approved.ToLower() && currentApprovalStatus != ApprovalStatus.Cancelled.ToLower() && currentApprovalStatus != ApprovalStatus.Rejected.ToLower()))
                        {
                            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(SPContext.Current.Site.Url);
                            IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(freightManagement.ID, StepModuleList.FreightManagement.ToString());
                            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                            {
                                _taskManagementDAL.CloseTasks(taskManagementCollection.ToList());
                            }

                            freightManagement.ApprovalStatus = StringConstant.ApprovalStatus.Cancelled;
                            _freightManagementDAL.SaveOrUpdate(SPContext.Current.Web, freightManagement);
                        }
                        else
                        {
                            return new MessageResult { Code = (int)FreightErrorCode.CannotCancel, Message = MessageResultHelper.GetRequestStatusMessage(freightManagement.ApprovalStatus.ToLower()), ObjectId = 0 };
                        }

                    }
                }
                catch (Exception ex)
                {
                    return new MessageResult { Code = (int)FreightErrorCode.Unexpected, Message = ex.Message, ObjectId = 0 };
                }
            }
            else
            {
                return new MessageResult { Code = (int)FreightErrorCode.Unexpected, Message = "Cannot find the item", ObjectId = 0 };
            }

            return new MessageResult { Code = 0, Message = "Successful", ObjectId = 0 };
        }

        public MessageResult UpdateFreightVehicle(FreightManagementModel freightManagementModel)
        {
            try
            {
                if (freightManagementModel != null && freightManagementModel.Id > 0)
                {
                    SPWeb spWeb = SPContext.Current.Web;

                    Biz.Models.FreightManagement freightManagement = _freightManagementDAL.GetByID(freightManagementModel.Id);
                    if (freightManagement != null)
                    {
                        freightManagement.VehicleLookup = freightManagementModel.VehicleLookup;
                        int itemId = _freightManagementDAL.SaveOrUpdate(spWeb, freightManagement);
                    }
                }
            }
            catch (Exception ex)
            {
                new MessageResult { Code = (int)FreightErrorCode.Unexpected, Message = ex.Message, ObjectId = 0 };
            }

            return new MessageResult { Code = 0, Message = "Successful", ObjectId = 0 };
        }

        public MessageResult SecurityUpdateFreight(FreightSecurityModel freightSecurityModel)
        {
            if (freightSecurityModel != null)
            {
                try
                {
                    Biz.Models.FreightManagement freightManagement = _freightManagementDAL.GetByID(Convert.ToInt32(freightSecurityModel.Id));
                    if (freightManagement.ID > 0)
                    {
                        int action = Convert.ToInt32(freightSecurityModel.ActionNo);
                        List<FreightDetails> freightDetails;
                        switch (action)
                        {
                            case 1: // checkin/checkout
                                var currentDateTime = DateTime.Now;
                                var checkInIds = freightSecurityModel.CheckInFreightIds != null ? freightSecurityModel.CheckInFreightIds : new List<string>();
                                var checkOutIds = freightSecurityModel.CheckOutFreightIds != null ? freightSecurityModel.CheckOutFreightIds : new List<string>();
                                var commonIds = checkInIds.Where(e => checkOutIds.Contains(e)).ToList();
                                checkInIds = checkInIds.Where(e => commonIds.Contains(e) == false).ToList();
                                checkOutIds = checkOutIds.Where(e => commonIds.Contains(e) == false).ToList();

                                freightDetails = _freightDetailsDAL.GetItemByIds(checkInIds);
                                if (freightDetails != null && freightDetails.Count > 0)
                                {
                                    foreach (var freightDetail in freightDetails)
                                    {
                                        freightDetail.ShippingIn = currentDateTime;
                                        freightDetail.CheckInBy = new LookupItem() { LookupId = Convert.ToInt32(freightSecurityModel.UpdateById), LookupValue = freightSecurityModel.UpdateByName };
                                    }
                                    _freightDetailsDAL.SaveOrUpdate(SPContext.Current.Web, freightDetails);
                                }

                                freightDetails = null;
                                freightDetails = _freightDetailsDAL.GetItemByIds(checkOutIds);
                                if (freightDetails != null && freightDetails.Count > 0)
                                {
                                    foreach (var freightDetail in freightDetails)
                                    {
                                        freightDetail.ShippingOut = currentDateTime;
                                        freightDetail.CheckOutBy = new LookupItem() { LookupId = Convert.ToInt32(freightSecurityModel.UpdateById), LookupValue = freightSecurityModel.UpdateByName };
                                    }
                                    _freightDetailsDAL.SaveOrUpdate(SPContext.Current.Web, freightDetails);
                                }

                                freightDetails = null;
                                freightDetails = _freightDetailsDAL.GetItemByIds(commonIds);
                                if (freightDetails != null && freightDetails.Count > 0)
                                {
                                    foreach (var freightDetail in freightDetails)
                                    {
                                        freightDetail.ShippingIn = currentDateTime;
                                        freightDetail.ShippingOut = currentDateTime;
                                        freightDetail.CheckInBy = new LookupItem() { LookupId = Convert.ToInt32(freightSecurityModel.UpdateById), LookupValue = freightSecurityModel.UpdateByName };
                                        freightDetail.CheckOutBy = new LookupItem() { LookupId = Convert.ToInt32(freightSecurityModel.UpdateById), LookupValue = freightSecurityModel.UpdateByName };
                                    }
                                    _freightDetailsDAL.SaveOrUpdate(SPContext.Current.Web, freightDetails);
                                }
                                break;
                            case 3: //reject
                                freightManagement.ApprovalStatus = StringConstant.ApprovalStatus.Rejected;
                                break;
                            default:
                                break;
                        }

                        List<FreightDetails> allFreightDetails = _freightDetailsDAL.GetItemsByParentId(freightManagement.ID);

                        bool isFinished = false;
                        if (freightManagement.FreightType == true)
                        {
                            if (allFreightDetails.Where(e => e.CheckOutBy == null || e.CheckOutBy != null && e.CheckOutBy.LookupId == 0).Any() == false)
                            {
                                if (freightManagement.ReturnedGoods == true)
                                {
                                    isFinished = allFreightDetails.Where(e => e.CheckInBy == null || e.CheckInBy != null && e.CheckInBy.LookupId == 0).Any() ? false : true;
                                }
                                else
                                {
                                    isFinished = true;
                                }
                            }

                        }
                        else
                        {
                            if (allFreightDetails.Where(e => e.CheckInBy == null || e.CheckInBy != null && e.CheckInBy.LookupId == 0).Any() == false)
                            {
                                if (freightManagement.ReturnedGoods == true)
                                {
                                    isFinished = allFreightDetails.Where(e => e.CheckOutBy == null || e.CheckOutBy != null && e.CheckOutBy.LookupId == 0).Any() ? false : true;
                                }
                                else
                                {
                                    isFinished = true;
                                }
                            }
                        }

                        freightManagement.IsFinished = isFinished;
                        if (!string.IsNullOrEmpty(freightSecurityModel.UpdateByName) && !string.IsNullOrEmpty(freightSecurityModel.SecurityNotes))
                        {
                            freightManagement.SecurityNotes = freightManagement.SecurityNotes.BuildComment(string.Format("{0}: {1}", freightSecurityModel.UpdateByName, freightSecurityModel.SecurityNotes));
                        }

                        _freightManagementDAL.SaveOrUpdate(SPContext.Current.Web, freightManagement);
                    }
                    else
                    {
                        return new MessageResult { Code = (int)FreightErrorCode.CannotUpdate, Message = WebPageResourceHelper.GetResourceString("CannotUpdateRequest"), ObjectId = 0 };
                    }
                }
                catch (Exception ex)
                {
                    return new MessageResult { Code = (int)FreightErrorCode.Unexpected, Message = ex.Message, ObjectId = 0 };
                }
            }

            return new MessageResult { Code = 0, Message = "Successful", ObjectId = 0 };
        }

        public bool IsUserAdminDepartment()
        {
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
            var currentEmployee = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);
            if (currentEmployee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
            {
                return true;
            }
            return false;
        }

        public bool IsSecurity()
        {
            var isSecurityGuard = false;

            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();

            AdditionalEmployeePositionDAL additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(SPContext.Current.Site.Url);
            isSecurityGuard = additionalEmployeePositionDAL.GetAdditionalPosition(currentEmployee.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);

            return isSecurityGuard;
        }

        public bool HasApprovalPermission(string freightId)
        {
            string siteUrl = SPContext.Current.Site.Url;
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(siteUrl);
            EmployeeInfo employeeInfo = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);

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
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.FreightManagement.ToString(), employeeInfo.ADAccount.ID, freightId);
            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                return true;
            }
            return false;
        }

        public List<FreightReceiverDepartmentModel> GetAllReceiverDepartment()
        {
            List<FreightReceiverDepartmentModel> freightReceiverDepartmentModelCollection = new List<FreightReceiverDepartmentModel>();

            try
            {
                List<FreightReceiverDepartment> freightReceiverDepartmentCollection = _freightReceiverDepartmentDAL.GetAll();
                if (freightReceiverDepartmentCollection != null)
                {
                    foreach (var item in freightReceiverDepartmentCollection)
                    {
                        freightReceiverDepartmentModelCollection.Add(ConvertToModel(item));
                    }
                }
            }
            catch { }

            return freightReceiverDepartmentModelCollection;
        }

        public Stream ExportFreight(string freightId)
        {
            if (string.IsNullOrEmpty(freightId))
            {
                return null;
            }

            Biz.Models.FreightManagement freightManagement = _freightManagementDAL.GetByID(Convert.ToInt32(freightId));
            if (freightManagement == null) { return null; }

            string destFilePath = "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                string templateFileName = "FreightTemplate.docx";
                string tempFolderPath = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\Freight", 15);
                Directory.CreateDirectory(tempFolderPath);
                WordHelper.RemoveOldFiles(tempFolderPath, 1);

                string newFileName = string.Format("{0}-{1}.docx", "Freight", DateTime.Now.Ticks);
                destFilePath = WordHelper.DownloadFile(SPContext.Current.Site.Url, "Shared Documents", templateFileName, tempFolderPath, newFileName);

                try
                {
                    using (WordprocessingDocument wordProcessingDoc = WordprocessingDocument.Open(destFilePath, true))
                    {
                        List<SdtContentCell> bringerObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("bringername");
                        var bringerName = string.Empty;
                        if (freightManagement.Bringer != null && freightManagement.Bringer.LookupId > 0)
                        {
                            bringerName = freightManagement.Bringer.LookupValue;
                        }
                        else if (!string.IsNullOrEmpty(freightManagement.BringerName))
                        {
                            bringerName = freightManagement.BringerName;
                        }
                        else
                        {
                            bringerName = ResourceHelper.GetLocalizedString("FreightManagement_CompanyVehicle", StringConstant.ResourcesFileLists, 1066);
                        }
                        bringerObj[0].FillTextBox(bringerName);

                        string title1 = " ";
                        string desc1 = " ";
                        string title2 = " ";
                        string desc2 = " ";
                        string title3 = " ";
                        string desc3 = " ";

                        if (!string.IsNullOrEmpty(freightManagement.Bringer.LookupValue))
                        {
                            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
                            EmployeeInfo employeeInfo = _employeeInfoDAL.GetByID(freightManagement.Bringer.LookupId);

                            title1 = "MSSV:";
                            desc1 = employeeInfo.EmployeeID;
                            title2 = "Phòng ban:";
                            desc2 = DepartmentListSingleton.GetDepartmentByID(employeeInfo.Department.LookupId, SPContext.Current.Site.Url).VietnameseName;
                            title3 = "Lý do:";
                            desc3 = !string.IsNullOrEmpty(freightManagement.Reason) ? freightManagement.Reason : " ";
                        }
                        else if (freightManagement.CompanyVehicle == true)
                        {
                            title2 = "Lý do:";
                            desc2 = !string.IsNullOrEmpty(freightManagement.Reason) ? freightManagement.Reason : " ";
                        }
                        else
                        {
                            title2 = "Tên công ty:";
                            desc2 = !string.IsNullOrEmpty(freightManagement.CompanyName) ? freightManagement.CompanyName : " ";
                            title3 = "Lý do:";
                            desc3 = !string.IsNullOrEmpty(freightManagement.Reason) ? freightManagement.Reason : " ";
                        }

                        List<SdtContentCell> title1Obj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("title1");
                        title1Obj[0].FillTextBox(title1);
                        List<SdtContentCell> desc1Obj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("desc1");
                        desc1Obj[0].FillTextBox(desc1);

                        List<SdtContentCell> title2Obj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("title2");
                        title2Obj[0].FillTextBox(title2);
                        List<SdtContentCell> desc2Obj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("desc2");
                        desc2Obj[0].FillTextBox(desc2);

                        List<SdtContentCell> title3Obj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("title3");
                        title3Obj[0].FillTextBox(title3);
                        List<SdtContentCell> desc3Obj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("desc3");
                        desc3Obj[0].FillTextBox(desc3);

                        List<SdtContentCell> receiverNameObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("receivername");
                        receiverNameObj[0].FillTextBox(!string.IsNullOrEmpty(freightManagement.Receiver) ? freightManagement.Receiver : " ");

                        List<SdtContentCell> receiverDeptNameObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("receiverdeptname");
                        receiverDeptNameObj[0].FillTextBox(freightManagement.ReceiverDepartmentLookup.LookupId > 0 ? freightManagement.ReceiverDepartmentVN.LookupValue : freightManagement.ReceiverDepartmentText);

                        List<SdtContentCell> receiverPhoneObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("receiverphone");
                        receiverPhoneObj[0].FillTextBox(!string.IsNullOrEmpty(freightManagement.ReceiverPhone) ? freightManagement.ReceiverPhone : " ");

                        List<SdtContentCell> otherReasonsObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("otherreasons");
                        otherReasonsObj[0].FillTextBox(!string.IsNullOrEmpty(freightManagement.OtherReason) ? freightManagement.OtherReason : " ");

                        DateTime? transportTime = freightManagement.TransportTime;
                        string hour = string.Empty;
                        string minute = string.Empty;
                        string second = string.Empty;
                        string day = string.Empty;
                        string month = string.Empty;
                        string year = string.Empty;

                        if (transportTime.HasValue)
                        {
                            TimeSpan timeOfDay = transportTime.Value.TimeOfDay;
                            hour = timeOfDay.Hours.ToString();
                            minute = timeOfDay.Minutes.ToString();
                            second = timeOfDay.Seconds.ToString();
                            day = transportTime.Value.Day.ToString();
                            month = transportTime.Value.Month.ToString();
                            year = transportTime.Value.Year.ToString();
                        }

                        List<SdtContentCell> exitHourObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("hr");
                        exitHourObj[0].FillTextBox(hour, JustificationValues.Right);

                        List<SdtContentCell> exitMinObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("mi");
                        exitMinObj[0].FillTextBox(minute, JustificationValues.Right);

                        List<SdtContentCell> exitDayObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("day");
                        exitDayObj[0].FillTextBox(day, JustificationValues.Left);

                        List<SdtContentCell> exitMonthObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("mo");
                        exitMonthObj[0].FillTextBox(month, JustificationValues.Left);

                        List<SdtContentCell> exitYearObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("year");
                        exitYearObj[0].FillTextBox(year, JustificationValues.Left);

                        List<SdtContentCell> requestedByObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("requestedby");
                        requestedByObj[0].FillTextBox(!string.IsNullOrEmpty(freightManagement.Requester.LookupValue) ? freightManagement.Requester.LookupValue : " ", JustificationValues.Center);

                        List<SdtContentCell> departmentHeadObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("departmenthead");
                        departmentHeadObj[0].FillTextBox(freightManagement.DH != null ? freightManagement.DH.FullName : " ", JustificationValues.Center);

                        int idxOfSendGoods = 0;
                        idxOfSendGoods = !string.IsNullOrEmpty(freightManagement.OtherReason) ? 1 : 0;

                        int idxOfReturnGoods = 0;
                        idxOfReturnGoods = freightManagement.ReturnedGoods == true ? 2 : 3;

                        List<SdtContentCell> checkBoxCollection = wordProcessingDoc.MainDocumentPart.Document.GetCheckBoxByNameFromTable("☐");
                        if (checkBoxCollection.Count >= (idxOfSendGoods + 1))
                        {
                            checkBoxCollection[idxOfSendGoods].FillTextBox("☒");
                        }

                        if (checkBoxCollection.Count >= (idxOfReturnGoods + 1))
                        {
                            checkBoxCollection[idxOfReturnGoods].FillTextBox("☒");
                        }

                        List<SdtContentCell> requestNoObj = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("reqno");
                        requestNoObj[0].FillTextBox(!string.IsNullOrEmpty(freightManagement.RequestNo) ? freightManagement.RequestNo : " ");

                        List<SdtBlock> tables = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByName("table1");
                        if (tables != null && tables.Count() > 0)
                        {
                            string templateStr = File.ReadAllText(Path.Combine(SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\XML", 15), "freighttabletemplate.xml"));
                            Table newTable = new Table(templateStr);

                            TableRow rowTemplate = newTable.Descendants<TableRow>().Last();
                            List<string> propertyList = new List<string>() { };

                            List<Biz.Models.FreightDetails> freightDetails = _freightDetailsDAL.GetItemsByParentId(freightManagement.ID);

                            for (int i = 0; i < freightDetails.Count; i++)
                            {
                                var detail = freightDetails[i];

                                if (string.IsNullOrEmpty(detail.GoodsName))
                                {
                                    continue;
                                }

                                TableRow newTableRow = new TableRow();
                                if (rowTemplate.TableRowProperties != null)
                                {
                                    newTableRow.TableRowProperties = new TableRowProperties(rowTemplate.TableRowProperties.OuterXml);
                                }

                                TableCell cellTemplate1 = rowTemplate.Descendants<TableCell>().ElementAt(0);
                                TableCell newTableCell1 = new TableCell(cellTemplate1.OuterXml);
                                newTableCell1.Descendants<Text>().First().Text = (i + 1).ToString();
                                newTableRow.Append(newTableCell1);

                                TableCell cellTemplate2 = rowTemplate.Descendants<TableCell>().ElementAt(1);
                                TableCell newTableCell2 = new TableCell(cellTemplate2.OuterXml);
                                newTableCell2.Descendants<Text>().First().Text = detail.GoodsName;
                                newTableRow.Append(newTableCell2);

                                TableCell cellTemplate3 = rowTemplate.Descendants<TableCell>().ElementAt(2);
                                TableCell newTableCell3 = new TableCell(cellTemplate3.OuterXml);
                                newTableCell3.Descendants<Text>().First().Text = detail.Unit;
                                newTableRow.Append(newTableCell3);

                                TableCell cellTemplate4 = rowTemplate.Descendants<TableCell>().ElementAt(3);
                                TableCell newTableCell4 = new TableCell(cellTemplate4.OuterXml);
                                newTableCell4.Descendants<Text>().First().Text = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0}", detail.Quantity);
                                newTableRow.Append(newTableCell4);

                                TableCell cellTemplate5 = rowTemplate.Descendants<TableCell>().ElementAt(4);
                                TableCell newTableCell5 = new TableCell(cellTemplate5.OuterXml);
                                newTableCell5.Descendants<Text>().First().Text = detail.Remarks;
                                newTableRow.Append(newTableCell5);

                                newTable.Append(newTableRow);
                            }

                            newTable.Descendants<TableRow>().ElementAt(1).Remove();
                            tables[0].Parent.InsertAfter(newTable, tables[0]);
                            tables[0].Remove();
                        }

                        wordProcessingDoc.MainDocumentPart.Document.Save();
                    }
                }
                catch { destFilePath = ""; }
            });

            if (!string.IsNullOrEmpty(destFilePath) && File.Exists(destFilePath))
            {
                String headerInfo = string.Format("attachment; filename={0}", Path.GetFileName(destFilePath));
                WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = headerInfo;
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";

                return File.OpenRead(destFilePath);
            }
            else
            {
                return null;
            }
        }

        public Stream ExportFreights(string from, string to, string departmentId, string locationIds, string vehicleId)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.IsNullOrEmpty(vehicleId))
            {
                return null;
            }

            try
            {
                DateTime fromDate;
                DateTime toDate;
                int deptId = 0;
                int vehId = 0;
                int.TryParse(departmentId, out deptId);
                if (DateTime.TryParse(from, out fromDate) && DateTime.TryParse(to, out toDate) && int.TryParse(vehicleId, out vehId))
                {
                    string templateFileName = "FreightsTemplate.xlsx";
                    string destFilePath = "";

                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        string tempFolderPath = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\Tmp", 15);
                        Directory.CreateDirectory(tempFolderPath);
                        ExcelHelper.RemoveOldFiles(tempFolderPath, 1);

                        destFilePath = ExcelHelper.DownloadFile(SPContext.Current.Site.RootWeb.Url, "Shared Documents", templateFileName, tempFolderPath, string.Empty);
                        string siteUrl = SPContext.Current.Site.Url;

                        List<Biz.Models.FreightManagement> freightManagementCollection = _freightManagementDAL.GetFilteredFreights(fromDate, toDate, deptId, locationIds.SplitStringOfLocations().ConvertAll(e => Convert.ToInt32(e)), vehId);
                        if (freightManagementCollection != null && freightManagementCollection.Count > 0)
                        {
                            using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Open(destFilePath, true))
                            {
                                string sheetName = "Sheet1";

                                string selectedVehicleName = (vehId > 0 && freightManagementCollection[0].VehicleVN != null) == true ? freightManagementCollection[0].VehicleVN.LookupValue : " ";
                                string currentDateStr = DateTime.Now.ToString(StringConstant.DateFormatddMMyyyy2);
                                ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "C", 5, selectedVehicleName);
                                ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "C", 6, currentDateStr);

                                DepartmentDAL _departmentDAL = new DepartmentDAL(siteUrl);
                                List<Department> departmentCollection = _departmentDAL.GetAll();

                                uint startRowIdx = 9;
                                for (int i = 0; i < freightManagementCollection.Count; i++)
                                {
                                    Biz.Models.FreightManagement freightManagement = freightManagementCollection[i];

                                    uint newRowIdx = startRowIdx + (uint)i + 1;
                                    ExcelHelper.DuplicateRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx, newRowIdx);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(1), newRowIdx,
                                        (i + 1).ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);

                                    string sender = freightManagement.Requester.LookupValue;
                                    sender = string.Format("{0} ({1})", sender, departmentCollection.Where(e => e.ID == freightManagement.Department.LookupId).FirstOrDefault().VietnameseName);
                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(2), newRowIdx,
                                        sender, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(3), newRowIdx,
                                        !string.IsNullOrEmpty(freightManagement.Receiver) ? freightManagement.Receiver : " ", DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(4), newRowIdx,
                                        freightManagement.ReceiverDepartmentLookup.LookupId > 0 ? freightManagement.ReceiverDepartmentVN.LookupValue : freightManagement.ReceiverDepartmentText, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    if (!string.IsNullOrEmpty(freightManagement.RequestNo))
                                    {
                                        ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(5), newRowIdx,
                                            freightManagement.RequestNo, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);
                                    }
                                }

                                ExcelHelper.RemoveRow(spreadSheetDoc.WorkbookPart, sheetName, 9);
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

        //public bool IsValidRequest()
        //{
        //    TimeSpan ts = DateTime.Now.TimeOfDay;
        //    return (ts.Hours > 14 || (ts.Hours == 14 && ts.Minutes > 30)) == true ? false : true;
        //}

        public MessageResult ValidateSumitTime()
        {
            var retMsg = string.Empty;
            TimeSpan ts = DateTime.Now.TimeOfDay;
            int hour = 14;
            int minute = 30;
            string value = ConfigurationDAL.GetValue(SPContext.Current.Web.Url, "FreightForm_ValidSumitTime");
            if (!string.IsNullOrEmpty(value))
            {
                var timeArr = value.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                hour = Convert.ToInt16(timeArr[0]);
                if (timeArr.Length > 1)
                {
                    minute = Convert.ToInt16(timeArr[1]);
                }
            }
            else
            {
                value = string.Format("{0}:{1}", hour, minute);
            }

            bool isValid = (ts.Hours > hour || (ts.Hours == hour && ts.Minutes > minute)) == true ? false : true;
            if (isValid == false)
            {
                retMsg = string.Format(System.Web.HttpUtility.UrlDecode(WebPageResourceHelper.GetResourceString("FreightRequest_WrongPolicies")), value);
                return new MessageResult() { Code = (int)FreightErrorCode.InvalidSubmitTime, Message = retMsg };
            }

            return new MessageResult() { Code = 0, Message = "" };
        }

        public FreightVehicleOperatorModel GetVehicleOperatorInfo(string employeeId)
        {
            FreightVehicleOperatorModel freightVehicleModel = new FreightVehicleOperatorModel();

            AdditionalEmployeePositionDAL _additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(SPContext.Current.Site.Url);
            freightVehicleModel.HasPermission = _additionalEmployeePositionDAL.GetAdditionalPosition(Convert.ToInt32(employeeId), AdditionalEmployeePositionModule.FreightManagement, AdditionalEmployeePositionLevelCode.VehicleOperator);
            freightVehicleModel.FreightVehicles = _freightVehicleDAL.GetAll().ToList();

            return freightVehicleModel;
        }

        public List<FreightVehicleModel> GetFreightVehicles()
        {
            List<FreightVehicleModel> freightVehicleModelCollection = new List<FreightVehicleModel>();

            List<FreightVehicle> freightVehicleCollection = _freightVehicleDAL.GetAll();
            if (freightVehicleCollection != null)
            {
                foreach (var item in freightVehicleCollection)
                {
                    freightVehicleModelCollection.Add(ConvertToModel(item));
                }
            }

            return freightVehicleModelCollection;
        }

        public DelegationModel GetDelegatedTaskInfo(string Id)
        {
            DelegationModel delegationModel = new DelegationModel();

            int listItemId = 0;
            if (int.TryParse(Id, out listItemId))
            {
                string[] viewFields = new string[] { StringConstant.FreightManagementList.DHField,StringConstant.FreightManagementList.BODField,
                    StringConstant.FreightManagementList.AdminDeptField, StringConstant.CommonSPListField.ApprovalStatusField,
                    StringConstant.CommonSPListField.CommonDepartmentField,CommonSPListField.CommonLocationField};
                string queryStr = $@"<Where>
                                      <Eq>
                                         <FieldRef Name='ID' />
                                         <Value Type='Counter'>{listItemId}</Value>
                                      </Eq>
                                   </Where>";
                string siteUrl = SPContext.Current.Site.Url;
                List<Biz.Models.FreightManagement> freightManagementCollection = _freightManagementDAL.GetByQuery(queryStr, viewFields);
                if (freightManagementCollection != null && freightManagementCollection.Count > 0)
                {
                    EmployeeInfo currentApprover = null;
                    Biz.Models.FreightManagement freightManagement = freightManagementCollection[0];
                    StepManagementDAL _stepManagementDAL = new StepManagementDAL(siteUrl);
                    var currentStep = _stepManagementDAL.GetStepManagement(freightManagement.ApprovalStatus, StepModuleList.FreightManagement, freightManagement.Department.LookupId);
                    if (currentStep != null)
                    {
                        currentApprover = _freightManagementDAL.GetApproverAtStep(freightManagement.Department.LookupId, freightManagement.Location.LookupId, StepModuleList.FreightManagement, currentStep.StepNumber);
                    }

                    if (currentApprover != null)
                    {
                        Delegation delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.FreightManagementList.ListUrl, freightManagement.ID);
                        delegationModel = new DelegationModel(delegation);
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
                TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(SPContext.Current.Site.Url);
                List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetTaskHistory(itemId, StepModuleList.FreightManagement.ToString(), allHistoricalData == 0 ? false : true);
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

        #region Private Methods

        private FreightManagementModel ConvertToModel(Biz.Models.FreightManagement freightManagement)
        {
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
            FreightManagementModel freightManagementModel = new FreightManagementModel()
            {
                Id = freightManagement.ID,
                RequestNo = freightManagement.RequestNo,
                Requester = freightManagement.Requester,
                Department = freightManagement.Department,
                Location = freightManagement.Location,
                Bringer = freightManagement.Bringer,
                BringerDepartment = freightManagement.BringerDepartment,
                BringerLocation = freightManagement.BringerLocation,
                CompanyVehicle = freightManagement.CompanyVehicle,
                BringerName = freightManagement.BringerName,
                CompanyName = freightManagement.CompanyName,
                Reason = freightManagement.Reason,
                Receiver = freightManagement.Receiver,
                ReceiverDepartmentLookup = freightManagement.ReceiverDepartmentLookup,
                ReceiverDepartmentVN = freightManagement.ReceiverDepartmentVN,
                ReceiverDepartmentText = freightManagement.ReceiverDepartmentText,
                ReceiverPhone = freightManagement.ReceiverPhone,
                FreightType = freightManagement.FreightType,
                ReturnedGoods = freightManagement.ReturnedGoods,
                HighPriority = freightManagement.HighPriority,
                OtherReason = freightManagement.OtherReason,
                VehicleLookup = freightManagement.VehicleLookup,
                VehicleVN = freightManagement.VehicleVN,
                IsValidRequest = freightManagement.IsValidRequest,
                IsFinished = freightManagement.IsFinished,
                DateString = freightManagement.TransportTime.ToString(StringConstant.DateFormatddMMyyyy2),
                Hour = freightManagement.TransportTime.Hour,
                Minute = freightManagement.TransportTime.Minute,
                SecurityNotes = freightManagement.SecurityNotes,
                Comment = freightManagement.Comment,
                ApprovalStatus = freightManagement.ApprovalStatus,
                DH = freightManagement.DH,
                BOD = freightManagement.BOD,
                AdminDept = freightManagement.AdminDept,
                CreatedBy = freightManagement.CreatedBy,
                ModifiedBy = freightManagement.ModifiedBy
            };

            if (freightManagement.RequestDueDate != null && freightManagement.RequestDueDate != default(DateTime))
            {
                freightManagementModel.RequestDueDate = freightManagement.RequestDueDate.ToString(StringConstant.DateFormatddMMyyyy2);
                if (freightManagement.RequestDueDate.Date < DateTime.Now.Date)
                {
                    //freightManagementModel.RequestExpired = true;
                    freightManagementModel.RequestExpired = false;
                }
            }

            return freightManagementModel;
        }

        private FreightDetailsModel ConvertToModel(FreightDetails freightDetails)
        {
            FreightDetailsModel freightDetailsModel = new FreightDetailsModel()
            {
                Id = freightDetails.ID,
                FreightManagementID = freightDetails.FreightManagementID,
                GoodsName = freightDetails.GoodsName,
                Unit = freightDetails.Unit,
                Quantity = freightDetails.Quantity,
                Remarks = freightDetails.Remarks,
                ShippingIn = freightDetails.ShippingIn,
                ShippingOut = freightDetails.ShippingOut,
                IsShippingIn = (freightDetails.ShippingIn != null ? true : false),
                IsShippingOut = (freightDetails.ShippingOut != null ? true : false),
                ShippingInTime = (freightDetails.ShippingIn.HasValue ? freightDetails.ShippingIn.Value.ToString(StringConstant.DateFormatddMMyyyyHHmm) : string.Empty),
                ShippingOutTime = (freightDetails.ShippingOut.HasValue ? freightDetails.ShippingOut.Value.ToString(StringConstant.DateFormatddMMyyyyHHmm) : string.Empty),
                ShippingInBy = freightDetails.CheckInBy != null ? Convert.ToString(freightDetails.CheckInBy.LookupValue) : string.Empty,
                ShippingOutBy = freightDetails.CheckOutBy != null ? Convert.ToString(freightDetails.CheckOutBy.LookupValue) : string.Empty,
            };

            return freightDetailsModel;
        }

        private FreightReceiverDepartmentModel ConvertToModel(FreightReceiverDepartment freightReceiverDepartment)
        {
            FreightReceiverDepartmentModel freightReceiverDepartmentModel = new FreightReceiverDepartmentModel()
            {
                Id = freightReceiverDepartment.ID,
                ReceiverDepartment = freightReceiverDepartment.ReceiverDepartment,
                ReceiverDepartmentVN = freightReceiverDepartment.ReceiverDepartmentVN
            };

            return freightReceiverDepartmentModel;
        }

        private FreightVehicleModel ConvertToModel(FreightVehicle freightVehicle)
        {
            FreightVehicleModel freightVehicleModel = new FreightVehicleModel()
            {
                Id = freightVehicle.ID,
                Vehicle = freightVehicle.Vehicle,
                VehicleVN = freightVehicle.VehicleVN
            };

            return freightVehicleModel;
        }

        #endregion
    }
}