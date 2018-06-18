using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using RBVH.Core.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System.Linq;
using RBVH.Stada.Intranet.Biz.Constants;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.DelegationManagement;

namespace RBVH.Stada.Intranet.ListEventReceiver.TransportationEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class TransportationEventReceiver : SPItemEventReceiver
    {
        private EmployeeInfoDAL employeeInfoDAL;
        private TaskManagementDAL taskManagementDAL;
        private const string TASK_NAME = "Vehicle request approval/ Duyệt yêu cầu đi xe";

        public TransportationEventReceiver() { }

        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);

            try
            {
                object fromDateObj = properties.AfterProperties[StringConstant.VehicleManagementList.CommonFrom];
                if (fromDateObj != null)
                {
                    DateTime fromDate = Convert.ToDateTime(fromDateObj);
                    DateTime reqDueDate = fromDate.Date;
                    //if (reqDueDate == DateTime.Now.Date)
                    //{
                    //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    //}
                    //else
                    //{
                    //    reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                    //}
                    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    properties.AfterProperties[StringConstant.CommonSPListField.CommonReqDueDateField] = reqDueDate.ToString(StringConstant.DateFormatTZForCAML);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Transportation Event Receiver - ItemAdding fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                base.ItemAdded(properties);

                var siteURL = properties.WebUrl;
                var vehicleManagementDAL = new VehicleManagementDAL(siteURL);
                var ItemID = properties.ListItemId;
                var currentItem = vehicleManagementDAL.GetByID(ItemID);
                
                taskManagementDAL = new TaskManagementDAL(siteURL);
                TaskManagement taskManagement = new TaskManagement();
                taskManagement.StartDate = DateTime.Now;
                taskManagement.DueDate = currentItem.RequestDueDate;
                taskManagement.PercentComplete = 0;
                taskManagement.ItemId = currentItem.ID;
                taskManagement.ItemURL = properties.List.DefaultDisplayFormUrl + "?ID=" + properties.ListItemId;
                taskManagement.ListURL = properties.List.DefaultViewUrl;
                taskManagement.TaskName = TASK_NAME;
                taskManagement.TaskStatus = TaskStatusList.InProgress;
                taskManagement.StepModule = StepModuleList.VehicleManagement.ToString();
                taskManagement.Department = currentItem.CommonDepartment;
                taskManagement.AssignedTo = currentItem.DepartmentHead;

                employeeInfoDAL = new EmployeeInfoDAL(siteURL);
                EmployeeInfo requesterInfo = employeeInfoDAL.GetByID(currentItem.Requester.LookupId);

                if ((int)Convert.ToDouble(requesterInfo.EmployeeLevel.LookupValue, CultureInfo.InvariantCulture.NumberFormat) == (int)StringConstant.EmployeeLevel.DepartmentHead)
                {
                    taskManagement.StepStatus = StepStatusList.BODApproval;
                }
                else
                {
                    taskManagement.StepStatus = StepStatusList.DHApproval;
                }

                DepartmentDAL _departmentDAL = new DepartmentDAL(siteURL);
                Department departmentHR = _departmentDAL.GetByCode("HR");
                if (departmentHR.ID == currentItem.CommonDepartment.LookupId)
                {
                    taskManagement.NextAssign = null;
                }
                else
                {
                    EmployeeInfo deptHeadOfHR = employeeInfoDAL.GetByPositionDepartment(StringConstant.EmployeePosition.DepartmentHead, requesterInfo.FactoryLocation.LookupId, departmentHR.ID).FirstOrDefault();
                    if (deptHeadOfHR != null)
                    {
                        taskManagement.NextAssign = deptHeadOfHR.ADAccount;
                    }
                }

                taskManagementDAL.SaveItem(taskManagement);

                var mailDAL = new EmailTemplateDAL(siteURL);
                var emailTemplate = mailDAL.GetByKey("VehicleManagement_Request");
                EmployeeInfo assigneeInfo = employeeInfoDAL.GetByADAccount(taskManagement.AssignedTo.ID);
                currentItem.ApprovalStatus = taskManagement.StepStatus;
                
                vehicleManagementDAL.SaveOrUpdate(currentItem);
                
                vehicleManagementDAL.SendEmail(currentItem, emailTemplate, null, assigneeInfo, VehicleTypeOfEmail.Request, siteURL);

                try
                {
                    List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(siteURL, assigneeInfo.ID, StringConstant.VehicleManagementList.ListUrl, currentItem.ID);
                    vehicleManagementDAL.SendDelegationEmail(currentItem, emailTemplate, toUsers, siteURL);
                }
                catch { }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA -  Transportation Event Receiver - ItemAdded fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            try
            {
                base.ItemUpdated(properties);

                var siteURL = properties.WebUrl;
                var vehicleManagementDAL = new VehicleManagementDAL(siteURL);
                var ItemID = properties.ListItemId;
                var currentItem = vehicleManagementDAL.GetByID(ItemID);
                if (currentItem.ApprovalStatus == "Cancelled")
                {
                    taskManagementDAL = new TaskManagementDAL(siteURL);
                    var tasks = taskManagementDAL.GetByItemID(currentItem.ID, StepModuleList.VehicleManagement.ToString()).ToList();
                    foreach (var task in tasks)
                    {
                        task.TaskStatus = TaskStatusList.Deferred;
                    }
                    taskManagementDAL.SaveItems(tasks);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA -  Transportation Event Receiver - ItemAdded fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }
    }
}