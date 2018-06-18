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
using System.Text;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Webservices.Helper;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.VehicleManagement
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class VehicleManagementService : IVehicleManagementService
    {
        VehicleManagementDAL _vehicleManagementDAL;

        public VehicleManagementService()
        {
            _vehicleManagementDAL = new VehicleManagementDAL(SPContext.Current.Site.Url);
        }

        public MessageResult ApproveVehicle(VehicleApprovalModel vehicleApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (vehicleApprovalModel.Id > 0)
                {
                    Biz.Models.VehicleManagement vehicleManagement = _vehicleManagementDAL.GetByID(vehicleApprovalModel.Id);
                    string currentApprovalStatus = vehicleManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)VehicleErrorCode.CannotApprove, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(vehicleManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)VehicleErrorCode.CannotApprove, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(vehicleApprovalModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(vehicleApprovalModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(vehicleManagement.ID, StepModuleList.VehicleManagement.ToString());
                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();
                        User nextAssignee = taskOfOriginalAssignee.NextAssign;

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Approved.ToString();
                            taskOfOriginalAssignee.Description = vehicleApprovalModel.Comment;
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
                            clonedTask.Description = vehicleApprovalModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }

                        if (!string.IsNullOrEmpty(vehicleApprovalModel.Comment))
                        {
                            vehicleManagement.CommonComment = vehicleManagement.CommonComment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, vehicleApprovalModel.Comment));
                        }

                        if (nextAssignee == null)
                        {
                            vehicleManagement.ApprovalStatus = StringConstant.ApprovalStatus.Approved.ToString();
                            _vehicleManagementDAL.SaveOrUpdate(spWeb, vehicleManagement);

                            EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(spWeb.Url);
                            EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("VehicleManagement_Approve");
                            EmployeeInfo toUser = _employeeInfoDAL.GetByID(vehicleManagement.Requester.LookupId);
                            _vehicleManagementDAL.SendEmail(vehicleManagement, emailTemplate, approverInfo, toUser, VehicleTypeOfEmail.Approve, spWeb.Url);
                        }
                        else if (nextAssignee != null && taskManagementCollection != null && taskManagementCollection.Count > 0)
                        {
                            _vehicleManagementDAL.RunWorkFlow(vehicleManagement, taskOfOriginalAssignee);

                            EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(spWeb.Url);
                            EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("VehicleManagement_Request");
                            EmployeeInfo toUser = _employeeInfoDAL.GetByADAccount(nextAssignee.ID);
                            _vehicleManagementDAL.SendEmail(vehicleManagement, emailTemplate, approverInfo, toUser, VehicleTypeOfEmail.Request, spWeb.Url);

                            try
                            {
                                List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(toUser.ID, StringConstant.VehicleManagementList.ListUrl, vehicleManagement.ID);
                                _vehicleManagementDAL.SendDelegationEmail(vehicleManagement, emailTemplate, toUsers, spWeb.Url);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)VehicleErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public MessageResult RejectVehicle(VehicleApprovalModel vehicleApprovalModel)
        {
            MessageResult msgResult = new MessageResult();

            try
            {
                SPWeb spWeb = SPContext.Current.Web;

                if (vehicleApprovalModel.Id > 0)
                {
                    Biz.Models.VehicleManagement vehicleManagement = _vehicleManagementDAL.GetByID(vehicleApprovalModel.Id);
                    string currentApprovalStatus = vehicleManagement.ApprovalStatus.ToLower();
                    if (currentApprovalStatus == ApprovalStatus.Approved.ToLower() || currentApprovalStatus == ApprovalStatus.Cancelled.ToLower() || currentApprovalStatus == ApprovalStatus.Rejected.ToLower())
                    {
                        return new MessageResult { Code = (int)VehicleErrorCode.CannotReject, Message = MessageResultHelper.GetRequestStatusMessage(currentApprovalStatus), ObjectId = 0 };
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(vehicleManagement.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        return new MessageResult { Code = (int)VehicleErrorCode.CannotReject, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    bool hasApprovalPermission = HasApprovalPermission(vehicleApprovalModel.Id.ToString());
                    DelegationModel delegationModel = GetDelegatedTaskInfo(vehicleApprovalModel.Id.ToString());
                    bool isDelegated = (delegationModel != null && delegationModel.Requester.LookupId > 0) ? true : false;
                    if (hasApprovalPermission == false && isDelegated == false)
                    {
                        return msgResult;
                    }

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(spWeb.Url);
                    EmployeeInfo approverInfo = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID);

                    int assigneeId = hasApprovalPermission == true ? approverInfo.ADAccount.ID : (isDelegated == true ? delegationModel.FromEmployee.ID : 0);

                    TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(spWeb.Url);
                    IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(vehicleManagement.ID, StepModuleList.VehicleManagement.ToString());
                    if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                    {
                        TaskManagement taskOfOriginalAssignee = _taskManagementDAL.GetTaskByAssigneeId(taskManagementCollection, assigneeId);
                        List<TaskManagement> relatedTasks = taskManagementCollection.Where(t => t.ID != taskOfOriginalAssignee.ID).ToList();

                        if (hasApprovalPermission == true)
                        {
                            taskOfOriginalAssignee.TaskStatus = TaskStatusList.Completed.ToString();
                            taskOfOriginalAssignee.PercentComplete = 1;
                            taskOfOriginalAssignee.TaskOutcome = TaskOutcome.Rejected.ToString();
                            taskOfOriginalAssignee.Description = vehicleApprovalModel.Comment;
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
                            clonedTask.Description = vehicleApprovalModel.Comment;
                            relatedTasks.Add(taskOfOriginalAssignee);
                            _taskManagementDAL.CloseTasks(relatedTasks);
                            _taskManagementDAL.SaveItem(clonedTask);
                        }
                    }

                    if (!string.IsNullOrEmpty(vehicleApprovalModel.Comment))
                    {
                        vehicleManagement.CommonComment = vehicleManagement.CommonComment.BuildComment(string.Format("{0}: {1}", approverInfo.FullName, vehicleApprovalModel.Comment));
                    }

                    vehicleManagement.ApprovalStatus = StringConstant.ApprovalStatus.Rejected.ToString();
                    _vehicleManagementDAL.SaveOrUpdate(spWeb, vehicleManagement);

                    EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(spWeb.Url);
                    EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("VehicleManagement_Reject");
                    EmployeeInfo toUser = _employeeInfoDAL.GetByID(vehicleManagement.Requester.LookupId);
                    _vehicleManagementDAL.SendEmail(vehicleManagement, emailTemplate, approverInfo, toUser, VehicleTypeOfEmail.Reject, spWeb.Url);
                }
            }
            catch (Exception ex)
            {
                msgResult.Code = (int)VehicleErrorCode.Unexpected;
                msgResult.Message = ex.Message;
            }

            return msgResult;
        }

        public bool HasApprovalPermission(string vehicleId)
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
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.VehicleManagement.ToString(), employeeInfo.ADAccount.ID, vehicleId);
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
                string[] viewFields = new string[] { StringConstant.VehicleManagementList.DH,StringConstant.VehicleManagementList.BOD,
                    StringConstant.CommonSPListField.ApprovalStatusField,StringConstant.CommonSPListField.CommonDepartmentField};
                string queryStr = $@"<Where>
                                      <Eq>
                                         <FieldRef Name='ID' />
                                         <Value Type='Counter'>{listItemId}</Value>
                                      </Eq>
                                   </Where>";
                string siteUrl = SPContext.Current.Site.Url;
                List<Biz.Models.VehicleManagement> vehicleManagementCollection = _vehicleManagementDAL.GetByQuery(queryStr, viewFields);
                if (vehicleManagementCollection != null && vehicleManagementCollection.Count > 0)
                {
                    Biz.Models.VehicleManagement vehicleManagement = vehicleManagementCollection[0];
                    StepManagementDAL _stepManagementDAL = new StepManagementDAL(siteUrl);
                    var currentStep = _stepManagementDAL.GetStepManagement(vehicleManagement.ApprovalStatus, StepModuleList.VehicleManagement, vehicleManagement.CommonDepartment.LookupId);
                    if (currentStep != null)
                    {
                        TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(siteUrl);
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
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.VehicleManagement.ToString(), vehicleManagement.ApprovalStatus, vehicleManagement.ID);
                        List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
                        if (taskManagementCollection != null)
                        {
                            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
                            foreach (var taskManagement in taskManagementCollection)
                            {
                                EmployeeInfo assigneeInfo = _employeeInfoDAL.GetByADAccount(taskManagement.AssignedTo.ID);
                                Delegation delegation = DelegationPermissionManager.IsDelegation(assigneeInfo.ID, StringConstant.VehicleManagementList.ListUrl, vehicleManagement.ID);
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
                TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(SPContext.Current.Site.Url);
                List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetTaskHistory(itemId, StepModuleList.VehicleManagement.ToString(), allHistoricalData == 0 ? false : true);
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
    }
}