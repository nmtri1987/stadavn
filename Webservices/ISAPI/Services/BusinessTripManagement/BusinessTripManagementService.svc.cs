using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.ServiceModel.Activation;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using Microsoft.SharePoint;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.Linq;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Core.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.WebPages.Utils;
using System.Text;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Webservices.Helper;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.BusinessTripManagement
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BusinessTripManagementService : IBusinessTripManagementService
    {
        public readonly BusinessTripEmployeeDetailDAL _businessTripEmployeeDetailDAL;
        public readonly BusinessTripManagementDAL _businessTripManagementDAL;
        public readonly BusinessTripScheduleDAL _businessTripScheduleDAL;

        public BusinessTripManagementService()
        {
            var webUrl = SPContext.Current.Web.Url;
            _businessTripEmployeeDetailDAL = new BusinessTripEmployeeDetailDAL(webUrl);
            _businessTripManagementDAL = new BusinessTripManagementDAL(webUrl);
            _businessTripScheduleDAL = new BusinessTripScheduleDAL(webUrl);
        }

        public BusinessTripManagementApproverModel GetApprovers(string departmentIdStr, string locationIdStr)
        {
            if (string.IsNullOrEmpty(departmentIdStr)) return null;

            BusinessTripManagementApproverModel approvers = new BusinessTripManagementApproverModel();
            try
            {
                BusinessTripManagementApprover businessTripManagementApprover = _businessTripManagementDAL.CreateApprovalList(Convert.ToInt32(departmentIdStr), Convert.ToInt32(locationIdStr));
                if (businessTripManagementApprover != null)
                {
                    if (businessTripManagementApprover.Approver1 != null)
                        approvers.Approver1 = new ApproverModel() { ID = businessTripManagementApprover.Approver1.ID, FullLoginName = businessTripManagementApprover.Approver1.ADAccount.FullName, LoginName = businessTripManagementApprover.Approver1.ADAccount.UserName };

                    if (businessTripManagementApprover.Approver2 != null)
                        approvers.Approver2 = new ApproverModel() { ID = businessTripManagementApprover.Approver2.ID, FullLoginName = businessTripManagementApprover.Approver2.ADAccount.FullName, LoginName = businessTripManagementApprover.Approver2.ADAccount.UserName };

                    if (businessTripManagementApprover.Approver3 != null)
                        approvers.Approver3 = new ApproverModel() { ID = businessTripManagementApprover.Approver3.ID, FullLoginName = businessTripManagementApprover.Approver3.ADAccount.FullName, LoginName = businessTripManagementApprover.Approver3.ADAccount.UserName };

                    if (businessTripManagementApprover.Approver4 != null)
                        approvers.Approver4 = new ApproverModel() { ID = businessTripManagementApprover.Approver4.ID, FullLoginName = businessTripManagementApprover.Approver4.ADAccount.FullName, LoginName = businessTripManagementApprover.Approver4.ADAccount.UserName };
                }

                return approvers;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Business Trip Management Service - GetApprovers",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<EmployeeInfoModel> GetDrivers()
        {
            List<EmployeeInfoModel> employeeInfoModelCollection = new List<EmployeeInfoModel>();
            AdditionalEmployeePositionDAL _additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(SPContext.Current.Site.Url);
            List<Biz.Models.AdditionalEmployeePosition> additionalEmployeePositionCollection = _additionalEmployeePositionDAL.GetEmployeesByLevel((int)StringConstant.AdditionalEmployeePosition.Driver, StepModuleList.BusinessTripManagement.ToString());
            if (additionalEmployeePositionCollection != null)
            {
                foreach (var additionalEmployeePosition in additionalEmployeePositionCollection)
                {
                    employeeInfoModelCollection.Add(new EmployeeInfoModel()
                    {
                        Id = additionalEmployeePosition.Employee.LookupId,
                        EmployeeId = additionalEmployeePosition.EmployeeID.LookupValue,
                        FullName = additionalEmployeePosition.Employee.LookupValue
                    });
                }
            }

            return employeeInfoModelCollection;
        }

        public List<EmployeeInfoModel> GetAccountants()
        {
            List<EmployeeInfoModel> employeeInfoModelCollection = new List<EmployeeInfoModel>();
            AdditionalEmployeePositionDAL _additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(SPContext.Current.Site.Url);
            List<Biz.Models.AdditionalEmployeePosition> additionalEmployeePositionCollection = _additionalEmployeePositionDAL.GetEmployeesByLevel((int)StringConstant.AdditionalEmployeePosition.Accountant, StepModuleList.BusinessTripManagement.ToString());
            if (additionalEmployeePositionCollection != null)
            {
                foreach (var additionalEmployeePosition in additionalEmployeePositionCollection)
                {
                    employeeInfoModelCollection.Add(new EmployeeInfoModel()
                    {
                        Id = additionalEmployeePosition.Employee.LookupId,
                        EmployeeId = additionalEmployeePosition.EmployeeID.LookupValue,
                        FullName = additionalEmployeePosition.Employee.LookupValue
                    });
                }
            }

            return employeeInfoModelCollection;
        }

        public BusinessTripManagementModel GetBusinessTripManagementById(string Id)
        {
            BusinessTripManagementModel businessTripManagementModel = new BusinessTripManagementModel();

            try
            {
                int itemId;
                if (!string.IsNullOrEmpty(Id) && int.TryParse(Id, out itemId))
                {
                    Biz.Models.BusinessTripManagement businessTripManagement = _businessTripManagementDAL.GetByID(itemId);
                    businessTripManagementModel = ConvertToModel(businessTripManagement);
                    businessTripManagementModel.EmployeeList = GetBusinessTripEmployeeByParentId(Id, businessTripManagement.CommonDepartment.LookupId);
                    businessTripManagementModel.ScheduleList = GetBusinessTripScheduleByParentId(Id);
                }
            }
            catch { }

            return businessTripManagementModel;
        }

        public MessageResult InsertBusinessTripManagement(BusinessTripManagementModel businessTripManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (businessTripManagementModel.Id > 0)
                {
                    Biz.Models.BusinessTripManagement businessTripObj = _businessTripManagementDAL.GetByID(businessTripManagementModel.Id);
                    if (businessTripObj.ModifiedBy.ID != businessTripObj.CreatedBy.ID && businessTripObj.ApprovalStatus.ToLower() != ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)BusinessTripErrorCode.CannotUpdate, Message = MessageResultHelper.GetRequestStatusMessage(businessTripObj.ApprovalStatus), ObjectId = 0 };
                    }
                }

                Biz.Models.BusinessTripManagement businessTripManagement = businessTripManagementModel.ToEntity();
                if (businessTripManagementModel.Id <= 0)
                {
                    AdditionalEmployeePositionDAL _additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(SPContext.Current.Site.Url);
                    if (businessTripManagementModel.Driver == null || (businessTripManagementModel.Driver != null && businessTripManagementModel.Driver.LookupId == 0))
                    {
                        List<Biz.Models.AdditionalEmployeePosition> drivers = _additionalEmployeePositionDAL.GetEmployeesByLevel((int)StringConstant.AdditionalEmployeePosition.Driver, StepModuleList.BusinessTripManagement.ToString());
                        if (drivers != null && drivers.Count > 0)
                        {
                            businessTripManagement.Driver = new LookupItem() { LookupId = drivers[0].Employee.LookupId, LookupValue = drivers[0].Employee.LookupValue };
                        }
                    }

                    if (businessTripManagement.Cashier == null || (businessTripManagement.Cashier != null && businessTripManagement.Cashier.LookupId == 0))
                    {
                        List<Biz.Models.AdditionalEmployeePosition> accountants = _additionalEmployeePositionDAL.GetEmployeesByLevel((int)StringConstant.AdditionalEmployeePosition.Accountant, StepModuleList.BusinessTripManagement.ToString());
                        if (accountants != null && accountants.Count > 0)
                        {
                            businessTripManagement.Cashier = new LookupItem() { LookupId = accountants[0].Employee.LookupId, LookupValue = accountants[0].Employee.LookupValue };
                        }
                    }
                }

                DateTime startTripDate = DateTime.Now;
                List<BusinessTripSchedule> businessTripScheduleCollection = new List<BusinessTripSchedule>();
                if (businessTripManagementModel.ScheduleList != null && businessTripManagementModel.ScheduleList.Count > 0)
                {
                    foreach (BusinessTripScheduleModel businessTripScheduleModel in businessTripManagementModel.ScheduleList)
                    {
                        BusinessTripSchedule businessTripSchedule = businessTripScheduleModel.ToEntity();
                        businessTripScheduleCollection.Add(businessTripSchedule);
                    }
                    startTripDate = businessTripScheduleCollection.Select(e => e.DepartDate).Min().Value;
                }
                businessTripManagement = _businessTripManagementDAL.SetDueDate(businessTripManagement, startTripDate);

                int itemId = _businessTripManagementDAL.SaveOrUpdate(spWeb, businessTripManagement);
                if (itemId > 0)
                {
                    if (businessTripManagementModel.EmployeeList != null && businessTripManagementModel.EmployeeList.Count > 0)
                    {
                        // Edit -> DELETE all details -> Insert new items
                        if (businessTripManagementModel.Id > 0)
                        {
                            List<BusinessTripEmployeeDetail> businessTripEmployeeDetails = _businessTripEmployeeDetailDAL.GetItemsByParentId(businessTripManagementModel.Id);
                            _businessTripEmployeeDetailDAL.DeleteItems(businessTripEmployeeDetails.Select(x => x.ID).ToList());
                        }

                        List<BusinessTripEmployeeDetail> businessTripEmployeeDetailCollection = new List<BusinessTripEmployeeDetail>();
                        foreach (BusinessTripEmployeeModel employee in businessTripManagementModel.EmployeeList)
                        {
                            BusinessTripEmployeeDetail businessTripEmployeeDetail = new BusinessTripEmployeeDetail();
                            businessTripEmployeeDetail.BusinessTripManagementID = new LookupItem() { LookupId = itemId };
                            businessTripEmployeeDetail.Employee = new LookupItem { LookupId = employee.EmployeeId, LookupValue = employee.EmployeeCode };
                            businessTripEmployeeDetailCollection.Add(businessTripEmployeeDetail);
                        }
                        _businessTripEmployeeDetailDAL.SaveOrUpdate(spWeb, businessTripEmployeeDetailCollection);
                    }

                    if (businessTripManagementModel.ScheduleList != null && businessTripManagementModel.ScheduleList.Count > 0)
                    {
                        // Edit -> DELETE all details -> Insert new items
                        if (businessTripManagementModel.Id > 0)
                        {
                            List<BusinessTripSchedule> businessTripSchedules = _businessTripScheduleDAL.GetItemsByParentId(businessTripManagementModel.Id);
                            _businessTripScheduleDAL.DeleteItems(businessTripSchedules.Select(x => x.ID).ToList());
                        }

                        foreach (BusinessTripSchedule businessTripSchedule in businessTripScheduleCollection)
                        {
                            businessTripSchedule.ID = 0;
                            businessTripSchedule.BusinessTripManagementID = new LookupItem() { LookupId = itemId };
                        }
                        _businessTripScheduleDAL.SaveOrUpdate(spWeb, businessTripScheduleCollection);
                    }
                }

                businessTripManagement = _businessTripManagementDAL.GetByID(itemId);
                if ((businessTripManagementModel.Id <= 0 && itemId > 0))
                {
                    _businessTripManagementDAL.StartWorkFlow(businessTripManagement);
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)BusinessTripErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult ApproveBusinessTrip(BusinessTripManagementModel businessTripManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (businessTripManagementModel.Id > 0)
                {
                    Biz.Models.BusinessTripManagement businessTripManagement = _businessTripManagementDAL.GetByID(businessTripManagementModel.Id);
                    string currentApprovalStatus = businessTripManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)BusinessTripErrorCode.CannotApprove, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(businessTripManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)BusinessTripErrorCode.CannotApprove, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(businessTripManagementModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(businessTripManagementModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(businessTripManagement.ID, StepModuleList.BusinessTripManagement.ToString());
                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Approved.ToString();
                            taskOfOriginalAssignee.Description = businessTripManagementModel.Comment;
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
                            clonedTask.Description = businessTripManagementModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }

                        if (!string.IsNullOrEmpty(businessTripManagementModel.Comment))
                        {
                            businessTripManagement.Comment = businessTripManagement.Comment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, businessTripManagementModel.Comment));
                        }
                        businessTripManagement.TripHighPriority = businessTripManagementModel.TripHighPriority;
                        businessTripManagement.CashRequestDetail = businessTripManagementModel.CashRequestDetails;

                        EmployeeInfo currentStepApprover = _employeeInfoDAL.GetByADAccount(assigneeId);
                        _businessTripManagementDAL.RunWorkFlow(businessTripManagement, taskOfOriginalAssignee, approverInfo, currentStepApprover);
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)BusinessTripErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult DriverUpdateBusinessTrip(BusinessTripManagementModel businessTripManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (businessTripManagementModel.Id > 0)
                {
                    Biz.Models.BusinessTripManagement businessTripManagement = _businessTripManagementDAL.GetByID(businessTripManagementModel.Id);
                    if (!string.IsNullOrEmpty(businessTripManagementModel.Comment) && businessTripManagementModel.Driver != null)
                    {
                        EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
                        EmployeeInfo currentUserInfo = _employeeInfoDAL.GetByID(businessTripManagementModel.Driver.LookupId);

                        if (currentUserInfo != null)
                        {
                            businessTripManagement.Comment = businessTripManagement.Comment.BuildComment(string.Format("{0}: {1}", currentUserInfo.FullName, businessTripManagementModel.Comment));
                            _businessTripManagementDAL.SaveOrUpdate(spWeb, businessTripManagement);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)BusinessTripErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult CashierUpdateBusinessTrip(BusinessTripManagementModel businessTripManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (businessTripManagementModel.Id > 0)
                {
                    Biz.Models.BusinessTripManagement businessTripManagement = _businessTripManagementDAL.GetByID(businessTripManagementModel.Id);
                    if (!string.IsNullOrEmpty(businessTripManagementModel.Comment) && businessTripManagementModel.Cashier != null)
                    {
                        EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
                        EmployeeInfo currentUserInfo = _employeeInfoDAL.GetByID(businessTripManagementModel.Cashier.LookupId);

                        if (currentUserInfo != null)
                        {
                            businessTripManagement.Comment = businessTripManagement.Comment.BuildComment(string.Format("{0}: {1}", currentUserInfo.FullName, businessTripManagementModel.Comment));
                            _businessTripManagementDAL.SaveOrUpdate(spWeb, businessTripManagement);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)BusinessTripErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult RejectBusinessTrip(BusinessTripManagementModel businessTripManagementModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (businessTripManagementModel.Id > 0)
                {
                    Biz.Models.BusinessTripManagement businessTripManagement = _businessTripManagementDAL.GetByID(businessTripManagementModel.Id);
                    string currentApprovalStatus = businessTripManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)BusinessTripErrorCode.CannotReject, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(businessTripManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)BusinessTripErrorCode.CannotReject, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(businessTripManagementModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(businessTripManagementModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(businessTripManagement.ID, StepModuleList.BusinessTripManagement.ToString());
                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Rejected.ToString();
                            taskOfOriginalAssignee.Description = businessTripManagementModel.Comment;
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
                            clonedTask.Description = businessTripManagementModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }
                    }

                    if (!string.IsNullOrEmpty(businessTripManagementModel.Comment))
                    {
                        businessTripManagement.Comment = businessTripManagement.Comment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, businessTripManagementModel.Comment));
                    }

                    businessTripManagement.ApprovalStatus = ApprovalStatus.Rejected.ToString();
                    _businessTripManagementDAL.SaveOrUpdate(spWeb, businessTripManagement);

                    EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(spWeb.Url);
                    EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("BusinessTripManagement_Reject");
                    EmployeeInfo toUser = _employeeInfoDAL.GetByID(businessTripManagement.Requester.LookupId);
                    _businessTripManagementDAL.SendEmail(businessTripManagement, emailTemplate, approverInfo, toUser, spWeb.Url, false);
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)BusinessTripErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult CancelBusinessTrip(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                try
                {
                    Biz.Models.BusinessTripManagement businessTripManagement = _businessTripManagementDAL.GetByID(Convert.ToInt32(Id));
                    if (businessTripManagement.ID > 0)
                    {
                        string currentApprovalStatus = businessTripManagement.ApprovalStatus.ToLower();
                        if (businessTripManagement.CreatedBy.ID == businessTripManagement.ModifiedBy.ID && businessTripManagement.CreatedBy.ID == SPContext.Current.Web.CurrentUser.ID &&
                            (currentApprovalStatus != ApprovalStatus.Approved.ToLower() && currentApprovalStatus != ApprovalStatus.Cancelled.ToLower() && currentApprovalStatus != ApprovalStatus.Rejected.ToLower()))
                        {
                            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(SPContext.Current.Site.Url);
                            IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(businessTripManagement.ID, StepModuleList.BusinessTripManagement.ToString());
                            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                            {
                                _taskManagementDAL.CloseTasks(taskManagementCollection.ToList());
                            }

                            businessTripManagement.ApprovalStatus = StringConstant.ApprovalStatus.Cancelled;
                            _businessTripManagementDAL.SaveOrUpdate(SPContext.Current.Web, businessTripManagement);
                        }
                        else
                        {
                            return new MessageResult { Code = (int)BusinessTripErrorCode.CannotCancel, Message = MessageResultHelper.GetRequestStatusMessage(businessTripManagement.ApprovalStatus.ToLower()), ObjectId = 0 };
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new MessageResult { Code = (int)BusinessTripErrorCode.Unexpected, Message = ex.Message, ObjectId = 0 };
                }
            }
            else
            {
                return new MessageResult { Code = (int)BusinessTripErrorCode.Unexpected, Message = "Cannot find the item", ObjectId = 0 };
            }

            return new MessageResult { Code = 0, Message = "Successful", ObjectId = 0 };
        }

        public bool HasApprovalPermission(string Id)
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
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.BusinessTripManagement.ToString(), employeeInfo.ADAccount.ID, Id);
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
                string[] viewFields = new string[] { StringConstant.BusinessTripManagementList.Fields.DH,StringConstant.BusinessTripManagementList.Fields.DirectBOD,
                    StringConstant.BusinessTripManagementList.Fields.BOD,StringConstant.BusinessTripManagementList.Fields.AdminDept,
                    StringConstant.CommonSPListField.ApprovalStatusField,StringConstant.CommonSPListField.CommonDepartmentField, CommonSPListField.CommonLocationField};
                string queryStr = $@"<Where>
                                      <Eq>
                                         <FieldRef Name='ID' />
                                         <Value Type='Counter'>{listItemId}</Value>
                                      </Eq>
                                   </Where>";
                string siteUrl = SPContext.Current.Site.Url;
                List<Biz.Models.BusinessTripManagement> businessTripManagementCollection = _businessTripManagementDAL.GetByQuery(queryStr, viewFields);
                if (businessTripManagementCollection != null && businessTripManagementCollection.Count > 0)
                {
                    EmployeeInfo currentApprover = null;
                    Biz.Models.BusinessTripManagement businessTripManagement = businessTripManagementCollection[0];
                    StepManagementDAL _stepManagementDAL = new StepManagementDAL(siteUrl);
                    var currentStep = _stepManagementDAL.GetStepManagement(businessTripManagement.ApprovalStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                    if (currentStep != null)
                    {
                        currentApprover = _businessTripManagementDAL.GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, currentStep.StepNumber);
                    }

                    if (currentApprover != null)
                    {
                        Delegation delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.BusinessTripManagementList.Url, businessTripManagement.ID);
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
                List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetTaskHistory(itemId, StepModuleList.BusinessTripManagement.ToString(), allHistoricalData == 0 ? false : true);
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
        private List<BusinessTripEmployeeModel> GetBusinessTripEmployeeByParentId(string parentId, int departmentId)
        {
            List<BusinessTripEmployeeModel> businessTripEmployeeModelCollection = new List<BusinessTripEmployeeModel>();

            int parentIdOut;
            if (!string.IsNullOrEmpty(parentId) && int.TryParse(parentId, out parentIdOut))
            {
                Department currentDept = DepartmentListSingleton.GetDepartmentByID(departmentId, SPContext.Current.Site.Url);
                string deptName = CultureInfo.CurrentUICulture.LCID == 1033 ? currentDept.Name : currentDept.VietnameseName;
                List<BusinessTripEmployeeDetail> businessTripEmployeeDetailCollection = _businessTripEmployeeDetailDAL.GetItemsByParentId(parentIdOut);

                if (businessTripEmployeeDetailCollection != null)
                {
                    foreach (BusinessTripEmployeeDetail businessTripEmployeeDetail in businessTripEmployeeDetailCollection)
                    {
                        BusinessTripEmployeeModel businessTripEmployeeModel = ConvertToModel(businessTripEmployeeDetail);
                        businessTripEmployeeModel.DepartmentName = deptName;
                        businessTripEmployeeModelCollection.Add(businessTripEmployeeModel);
                    }
                }
            }

            return businessTripEmployeeModelCollection;
        }

        private List<BusinessTripScheduleModel> GetBusinessTripScheduleByParentId(string parentId)
        {
            List<BusinessTripScheduleModel> businessTripScheduleModelCollection = new List<BusinessTripScheduleModel>();

            int parentIdOut;
            if (!string.IsNullOrEmpty(parentId) && int.TryParse(parentId, out parentIdOut))
            {
                List<BusinessTripSchedule> businessTripScheduleCollection = _businessTripScheduleDAL.GetItemsByParentId(parentIdOut);
                if (businessTripScheduleCollection != null)
                {
                    foreach (BusinessTripSchedule businessTripSchedule in businessTripScheduleCollection)
                    {
                        BusinessTripScheduleModel businessTripScheduleModel = ConvertToModel(businessTripSchedule);
                        businessTripScheduleModelCollection.Add(businessTripScheduleModel);
                    }
                }
            }

            return businessTripScheduleModelCollection;
        }

        private BusinessTripManagementModel ConvertToModel(Biz.Models.BusinessTripManagement businessTripManagement)
        {
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
            BusinessTripManagementModel businessTripManagementModel = new BusinessTripManagementModel()
            {
                Id = businessTripManagement.ID,
                Requester = businessTripManagement.Requester,
                Department = businessTripManagement.CommonDepartment,
                Location = businessTripManagement.CommonLocation,
                BusinessTripPurpose = businessTripManagement.BusinessTripPurpose,
                Domestic = businessTripManagement.Domestic,
                TransportationType = businessTripManagement.TransportationType,
                OtherTransportationDetail = businessTripManagement.OtherTransportationDetail,
                TripHighPriority = businessTripManagement.TripHighPriority,
                HasVisa = businessTripManagement.HasVisa,
                HotelBooking = businessTripManagement.HotelBooking,
                OtherService = businessTripManagement.OtherService,
                PaidBy = businessTripManagement.PaidBy,
                Driver = businessTripManagement.Driver,
                Cashier = businessTripManagement.Cashier,
                CashRequestDetails = businessTripManagement.CashRequestDetail,
                OtherRequestDetail = businessTripManagement.OtherRequestDetail,
                Comment = businessTripManagement.Comment,
                ApprovalStatus = businessTripManagement.ApprovalStatus,
                DH = businessTripManagement.DH,
                DirectBOD = businessTripManagement.DirectBOD,
                BOD = businessTripManagement.BOD,
                AdminDept = businessTripManagement.AdminDept,
                CreatedBy = businessTripManagement.CreatedBy,
                ModifiedBy = businessTripManagement.ModifiedBy
            };

            if (businessTripManagement.RequestDueDate != null && businessTripManagement.RequestDueDate != default(DateTime))
            {
                businessTripManagementModel.RequestDueDate = businessTripManagement.RequestDueDate.ToString(StringConstant.DateFormatddMMyyyy2);
                if (businessTripManagement.RequestDueDate.Date < DateTime.Now.Date)
                {
                    //businessTripManagementModel.RequestExpired = true;
                    businessTripManagementModel.RequestExpired = false;
                }
            }

            return businessTripManagementModel;
        }

        private BusinessTripEmployeeModel ConvertToModel(BusinessTripEmployeeDetail businessTripEmployeeDetail, string departmentName = "")
        {
            BusinessTripEmployeeModel businessTripEmployeeModel = new BusinessTripEmployeeModel()
            {
                Id = businessTripEmployeeDetail.ID,
                BusinessTripManagementID = businessTripEmployeeDetail.BusinessTripManagementID,
                EmployeeId = businessTripEmployeeDetail.Employee.LookupId,
                EmployeeCode = businessTripEmployeeDetail.EmployeeID.LookupValue,
                DepartmentName = departmentName,
                FullName = businessTripEmployeeDetail.Employee.LookupValue
            };

            return businessTripEmployeeModel;
        }

        private BusinessTripScheduleModel ConvertToModel(BusinessTripSchedule businessTripSchedule)
        {
            BusinessTripScheduleModel businessTripScheduleModel = new BusinessTripScheduleModel()
            {
                Id = businessTripSchedule.ID,
                BusinessTripManagementID = businessTripSchedule.BusinessTripManagementID,
                City = businessTripSchedule.City,
                ContactCompany = businessTripSchedule.ContactCompany,
                ContactPhone = businessTripSchedule.ContactPhone,
                Country = businessTripSchedule.Country,
                DepartDate = businessTripSchedule.DepartDate.Value.ToString(StringConstant.DateFormatddMMyyyy2),
                DepartTime = $"{businessTripSchedule.DepartDate.Value.Hour}:{businessTripSchedule.DepartDate.Value.Minute}",
                DepartHour = businessTripSchedule.DepartDate.Value.Hour,
                DepartMinute = businessTripSchedule.DepartDate.Value.Minute,
                FlightName = businessTripSchedule.FlightName,
                OtherSchedule = businessTripSchedule.OtherSchedule
            };

            return businessTripScheduleModel;
        }
        #endregion
    }
}
