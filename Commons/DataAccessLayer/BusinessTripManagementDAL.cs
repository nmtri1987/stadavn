using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Linq;
using System.Reflection;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.DelegationManagement;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class BusinessTripManagementDAL : BaseDAL<BusinessTripManagement>, IDelegationManager, IFilterTaskManager
    {
        private const string TASK_NAME = "Business Trip Request Approval/ Duyệt yêu cầu đi công tác";
        private const string LINK_MAIL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/{0}.aspx?{1}";

        public BusinessTripManagementDAL(string siteUrl) : base(siteUrl) { }

        public BusinessTripManagementApprover CreateApprovalList(int departmentId, int locationId)
        {
            BusinessTripManagementApprover businessTripManagementApprover = new BusinessTripManagementApprover();

            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            // Department Head
            var departmentHead = _employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.DepartmentHead, departmentId, locationId);
            if (departmentHead.Count > 0)
                businessTripManagementApprover.Approver1 = departmentHead[0];

            // Direct BOD 
            if (businessTripManagementApprover.Approver1 != null && businessTripManagementApprover.Approver1.Manager != null && businessTripManagementApprover.Approver1.Manager.LookupId > 0)
            {
                var directBODInfo = _employeeInfoDAL.GetByID(businessTripManagementApprover.Approver1.Manager.LookupId);
                if (directBODInfo != null)
                    businessTripManagementApprover.Approver2 = directBODInfo;
            }

            // BOD 
            var bodImployeeInfo = _employeeInfoDAL.GetByEmployeeID("11009"); // Ms. Su
            if (bodImployeeInfo != null)
                businessTripManagementApprover.Approver3 = bodImployeeInfo;

            // Department Head of HC
            int departmentIdHC = DepartmentListSingleton.GetDepartmentByCode("HR", SiteUrl).ID;
            if (departmentIdHC != departmentId)
            {
                var departmentHeadOfHC = _employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.DepartmentHead, departmentIdHC, locationId);
                if (departmentHeadOfHC.Count > 0)
                    businessTripManagementApprover.Approver4 = departmentHeadOfHC[0];
            }

            if (businessTripManagementApprover.Approver2 == null)
            {
                businessTripManagementApprover.Approver2 = businessTripManagementApprover.Approver3;
            }

            return businessTripManagementApprover;
        }

        public int SaveOrUpdate(BusinessTripManagement item)
        {
            int itemId = 0;
            using (SPSite spSite = new SPSite(SiteUrl))
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    itemId = SaveOrUpdate(spWeb, item);
                }
            }
            return itemId;
        }

        public int SaveOrUpdate(SPWeb spWeb, BusinessTripManagement item)
        {
            int itemId = 0;

            SPList splist = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            SPListItem spListItem;
            if (item.ID > 0)
            {
                spListItem = splist.GetItemById(item.ID);
                if (!string.IsNullOrEmpty(item.ApprovalStatus))
                {
                    spListItem[StringConstant.CommonSPListField.ApprovalStatusField] = item.ApprovalStatus;
                }
            }
            else
            {
                spListItem = splist.AddItem();

                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
                EmployeeInfo requesterInfo = null;
                int requesterLookupId = item.Requester.LookupId;
                if (requesterLookupId == 0)
                {
                    requesterInfo = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID);
                }
                else
                {
                    requesterInfo = _employeeInfoDAL.GetByID(requesterLookupId);
                }
                requesterLookupId = requesterInfo.ID;
                spListItem[StringConstant.CommonSPListField.RequesterField] = requesterLookupId;
                spListItem[StringConstant.CommonSPListField.CommonDepartmentField] = requesterInfo.Department.LookupId;
                spListItem[StringConstant.CommonSPListField.CommonLocationField] = requesterInfo.FactoryLocation.LookupId;

                if (item.DH != null && !string.IsNullOrEmpty(item.DH.UserName)) // Department Head
                {
                    SPUser departmentHead = SPContext.Current.Web.EnsureUser(item.DH.UserName);
                    SPFieldUserValue departmentHeadValue = new SPFieldUserValue(SPContext.Current.Web, departmentHead.ID, departmentHead.LoginName);
                    spListItem[StringConstant.BusinessTripManagementList.Fields.DH] = departmentHeadValue;
                }
                if (item.DirectBOD != null && !string.IsNullOrEmpty(item.DirectBOD.UserName)) // Direct BOD
                {
                    SPUser directBOD = SPContext.Current.Web.EnsureUser(item.DirectBOD.UserName);
                    SPFieldUserValue directBODValue = new SPFieldUserValue(SPContext.Current.Web, directBOD.ID, directBOD.LoginName);
                    spListItem[StringConstant.BusinessTripManagementList.Fields.DirectBOD] = directBODValue;
                }
                if (item.BOD != null && !string.IsNullOrEmpty(item.BOD.UserName)) // BOD
                {
                    SPUser bod = SPContext.Current.Web.EnsureUser(item.BOD.UserName);
                    SPFieldUserValue bodValue = new SPFieldUserValue(SPContext.Current.Web, bod.ID, bod.LoginName);
                    spListItem[StringConstant.BusinessTripManagementList.Fields.BOD] = bodValue;
                }
                if (item.AdminDept != null && !string.IsNullOrEmpty(item.AdminDept.UserName)) // Admin Dept
                {
                    SPUser adminDept = SPContext.Current.Web.EnsureUser(item.AdminDept.UserName);
                    SPFieldUserValue adminDeptValue = new SPFieldUserValue(SPContext.Current.Web, adminDept.ID, adminDept.LoginName);
                    spListItem[StringConstant.BusinessTripManagementList.Fields.AdminDept] = adminDeptValue;
                }

                spListItem[StringConstant.CommonSPListField.CommonReqDueDateField] = item.RequestDueDate;
            }

            spListItem[StringConstant.BusinessTripManagementList.Fields.Domestic] = item.Domestic;
            spListItem[StringConstant.BusinessTripManagementList.Fields.BusinessTripPurpose] = item.BusinessTripPurpose;
            spListItem[StringConstant.BusinessTripManagementList.Fields.HotelBooking] = item.HotelBooking;
            spListItem[StringConstant.BusinessTripManagementList.Fields.TripHighPriority] = item.TripHighPriority;
            spListItem[StringConstant.BusinessTripManagementList.Fields.PaidBy] = item.PaidBy;
            spListItem[StringConstant.BusinessTripManagementList.Fields.OtherService] = item.OtherService;
            spListItem[StringConstant.BusinessTripManagementList.Fields.TransportationType] = item.TransportationType;
            spListItem[StringConstant.BusinessTripManagementList.Fields.HasVisa] = item.HasVisa;
            spListItem[StringConstant.BusinessTripManagementList.Fields.CashRequestDetail] = item.CashRequestDetail;
            spListItem[StringConstant.BusinessTripManagementList.Fields.OtherTransportationDetail] = item.OtherTransportationDetail;
            spListItem[StringConstant.BusinessTripManagementList.Fields.OtherRequestDetail] = item.OtherRequestDetail;
            spListItem[StringConstant.BusinessTripManagementList.Fields.Driver] = item.Driver.LookupId;
            spListItem[StringConstant.BusinessTripManagementList.Fields.Cashier] = item.Cashier.LookupId;
            spListItem[StringConstant.CommonSPListField.CommonCommentField] = item.Comment;

            spWeb.AllowUnsafeUpdates = true;
            spListItem.Update();
            itemId = spListItem.ID;
            spWeb.AllowUnsafeUpdates = false;

            return itemId;
        }

        public BusinessTripManagement StartWorkFlow(BusinessTripManagement businessTripManagement)
        {
            SPListItem spListItem = this.GetByIDToListItem(businessTripManagement.ID);
            TaskManagement taskManagement = new TaskManagement();

            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = businessTripManagement.RequestDueDate;
            taskManagement.PercentComplete = 0;
            taskManagement.ItemId = businessTripManagement.ID;
            taskManagement.ItemURL = spListItem.ParentList.DefaultDisplayFormUrl + "?ID=" + businessTripManagement.ID;
            taskManagement.ListURL = spListItem.ParentList.DefaultViewUrl;
            taskManagement.TaskName = TASK_NAME;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.StepModule = StepModuleList.BusinessTripManagement.ToString();
            taskManagement.Department = businessTripManagement.CommonDepartment.LookupId > 0 ? businessTripManagement.CommonDepartment : null;

            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            EmployeeInfo departmentHeadInfo = _employeeInfoDAL.GetByADAccount(businessTripManagement.DH.ID);
            if (businessTripManagement.Requester.LookupId == departmentHeadInfo.ID)
            {
                taskManagement.StepStatus = StepStatusList.BODApproval;
                taskManagement.AssignedTo = businessTripManagement.DirectBOD.ID > 0 ? businessTripManagement.DirectBOD : businessTripManagement.BOD;
            }
            else
            {
                taskManagement.StepStatus = StepStatusList.DHApproval;
                taskManagement.AssignedTo = businessTripManagement.DH;
            }

            taskManagement.NextAssign = null;

            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            var nextStep = _stepManagementDAL.GetNextStepManagement(taskManagement.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
            if (nextStep != null)
            {
                var nextAssign = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                if (nextAssign != null)
                {
                    taskManagement.NextAssign = nextAssign.ADAccount;
                }
            }

            TaskManagementDAL taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            int retId = taskManagementDAL.SaveItem(taskManagement);

            businessTripManagement.ApprovalStatus = taskManagement.StepStatus;
            this.SaveOrUpdate(businessTripManagement);

            EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(this.SiteUrl);
            EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("BusinessTripManagement_Request");
            EmployeeInfo toUser = _employeeInfoDAL.GetByADAccount(taskManagement.AssignedTo.ID);
            SendEmail(businessTripManagement, emailTemplate, null, toUser, this.SiteUrl, true);

            try
            {
                List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(toUser.ID, StringConstant.BusinessTripManagementList.Url, businessTripManagement.ID);
                SendDelegationEmail(businessTripManagement, emailTemplate, toUsers, this.SiteUrl);
            }
            catch { }

            return businessTripManagement;
        }

        public BusinessTripManagement RunWorkFlow(BusinessTripManagement businessTripManagement, TaskManagement taskOfPrevStep, EmployeeInfo approver, EmployeeInfo currentStepApprover)
        {
            if (businessTripManagement == null) return null;

            TaskManagement taskManagement = new TaskManagement();

            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = businessTripManagement.RequestDueDate;
            taskManagement.PercentComplete = 0;
            taskManagement.ItemId = businessTripManagement.ID;
            taskManagement.ItemURL = taskOfPrevStep.ItemURL;
            taskManagement.ListURL = taskOfPrevStep.ListURL;
            taskManagement.TaskName = TASK_NAME;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.StepModule = StepModuleList.BusinessTripManagement.ToString();
            taskManagement.Department = businessTripManagement.CommonDepartment.LookupId > 0 ? businessTripManagement.CommonDepartment : null;

            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            User assignee = null;
            User nextAssignee = null;
            string stepStatus = string.Empty;

            if (businessTripManagement.Domestic == true) //Domestic Business Trip
            {
                StepManagement nextStep = null;

                if (currentStepApprover.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead &&
                    currentStepApprover.ADAccount.ID == businessTripManagement.DH.ID)
                {
                    nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                    if (businessTripManagement.TripHighPriority == true)
                    {
                        if (nextStep != null)
                        {
                            stepStatus = nextStep.StepStatus;
                            var approverAtStep = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                            if (approverAtStep != null)
                            {
                                assignee = approverAtStep.ADAccount;
                            }
                        }
                    }
                    else
                    {
                        if (nextStep != null)
                        {
                            nextStep = _stepManagementDAL.GetNextStepManagement(nextStep.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                            if (nextStep != null)
                            {
                                stepStatus = nextStep.StepStatus;
                                var approverAtStep = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                                if (approverAtStep != null)
                                {
                                    assignee = approverAtStep.ADAccount;
                                }
                            }
                        }
                    }
                }
                else
                {
                    nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                    if (nextStep != null)
                    {
                        stepStatus = nextStep.StepStatus;
                        var approverAtStep = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                        if (approverAtStep != null)
                        {
                            assignee = approverAtStep.ADAccount;
                        }
                    }
                }

                // get next approver
                if (nextStep != null)
                {
                    nextStep = _stepManagementDAL.GetNextStepManagement(nextStep.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                    if (nextStep != null)
                    {
                        var nextApprover = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                        if (nextApprover != null)
                        {
                            nextAssignee = nextApprover.ADAccount;
                        }
                    }
                }
            }
            else // Overseas Business Trip
            {
                StepManagement nextStep = null;
                if (currentStepApprover.ADAccount.ID == businessTripManagement.DirectBOD.ID)
                {
                    if (businessTripManagement.DirectBOD.ID != businessTripManagement.BOD.ID)
                    {
                        assignee = businessTripManagement.BOD;
                        stepStatus = StepStatusList.BODApproval;
                        nextStep = new StepManagement() { StepStatus = stepStatus, StepModule = StepModuleList.BusinessTripManagement.ToString() };
                    }
                    else
                    {
                        nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                        if (nextStep != null)
                        {
                            stepStatus = nextStep.StepStatus;
                            var approverAtStep = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                            if (approverAtStep != null)
                            {
                                assignee = approverAtStep.ADAccount;
                            }
                        }
                    }
                }
                else
                {
                    nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                    if (nextStep != null)
                    {
                        stepStatus = nextStep.StepStatus;
                        var approverAtStep = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                        if (approverAtStep != null)
                        {
                            assignee = approverAtStep.ADAccount;
                        }
                    }
                }

                // get next approver
                if (nextStep != null)
                {
                    nextStep = _stepManagementDAL.GetNextStepManagement(nextStep.StepStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
                    if (nextStep != null)
                    {
                        var nextApprover = GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, nextStep.StepNumber);
                        if (nextApprover != null)
                        {
                            nextAssignee = nextApprover.ADAccount;
                        }
                    }
                }
            }

            taskManagement.AssignedTo = assignee;
            taskManagement.NextAssign = nextAssignee;
            taskManagement.StepStatus = stepStatus;

            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(this.SiteUrl);
            if (assignee == null)
            {
                businessTripManagement.ApprovalStatus = StringConstant.ApprovalStatus.Approved.ToString();
                this.SaveOrUpdate(businessTripManagement);

                EmailTemplate emailTemplateRequester = _emailTemplateDAL.GetByKey("BusinessTripManagement_Approve");
                EmployeeInfo toRequester = _employeeInfoDAL.GetByID(businessTripManagement.Requester.LookupId);
                SendEmail(businessTripManagement, emailTemplateRequester, approver, toRequester, this.SiteUrl, false);

                if (businessTripManagement.TransportationType == ResourceHelper.GetLocalizedString("BusinessTripManagement_TransportationTypeCompanyTitle", StringConstant.ResourcesFileLists, 1033))
                {
                    EmailTemplate emailTemplateDriver = _emailTemplateDAL.GetByKey("BusinessTripManagement_Driver");
                    EmployeeInfo toDriver = _employeeInfoDAL.GetByID(businessTripManagement.Driver.LookupId);
                    SendEmail(businessTripManagement, emailTemplateDriver, approver, toDriver, this.SiteUrl, false);
                }

                if (!string.IsNullOrEmpty(businessTripManagement.CashRequestDetail))
                {
                    EmailTemplate emailTemplateAccountant = _emailTemplateDAL.GetByKey("BusinessTripManagement_Accountant");
                    EmployeeInfo toAccountant = _employeeInfoDAL.GetByID(businessTripManagement.Cashier.LookupId);
                    SendEmail(businessTripManagement, emailTemplateAccountant, approver, toAccountant, this.SiteUrl, false);
                }
            }
            else if (assignee != null)
            {
                TaskManagementDAL taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
                int retId = taskManagementDAL.SaveItem(taskManagement);

                businessTripManagement.ApprovalStatus = taskManagement.StepStatus;
                this.SaveOrUpdate(businessTripManagement);

                EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("BusinessTripManagement_Request");
                EmployeeInfo toUser = _employeeInfoDAL.GetByADAccount(assignee.ID);
                SendEmail(businessTripManagement, emailTemplate, approver, toUser, this.SiteUrl, true);

                try
                {
                    List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(toUser.ID, StringConstant.BusinessTripManagementList.Url, businessTripManagement.ID);
                    SendDelegationEmail(businessTripManagement, emailTemplate, toUsers, this.SiteUrl);
                }
                catch { }
            }

            return businessTripManagement;
        }

        public EmployeeInfo GetApproverAtStep(int departmentID, int locationID, StepModuleList StepModule, int stepNumber)
        {
            EmployeeInfo approverInfo = null;
            BusinessTripManagementApprover approvalList = CreateApprovalList(departmentID, locationID);
            if (approvalList != null)
            {
                Type typeBusinessTripManagementApprover = typeof(BusinessTripManagementApprover);
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                if (stepNumber == 3)
                {
                    stepNumber += 1;
                }
                PropertyInfo approverProp = typeBusinessTripManagementApprover.GetProperty(string.Format("Approver{0}", stepNumber), bindingFlags);
                if (approverProp != null)
                {
                    object approverValue = approverProp.GetValue(approvalList, null);
                    if (approverValue != null)
                    {
                        approverInfo = approverValue as EmployeeInfo;
                    }
                }
            }

            return approverInfo;
        }

        public BusinessTripManagement SetDueDate(BusinessTripManagement businessTripManagement, DateTime startTripDate)
        {
            DateTime reqDueDate = startTripDate.Date;

            //if (reqDueDate == DateTime.Now.Date)
            //{
            //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            //else
            //{
            //    reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            businessTripManagement.RequestDueDate = reqDueDate;

            return businessTripManagement;
        }

        #region Delegation
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> listOfTasks = new List<Delegation>();

            List<string> viewFields = new List<string>() { };
            viewFields.Add(StringConstant.CommonSPListField.RequesterField);
            viewFields.Add(StringConstant.CommonSPListField.CommonDepartmentField);
            viewFields.Add(StringConstant.BusinessTripManagementList.Fields.Domestic);
            viewFields.Add(StringConstant.BusinessTripManagementList.Fields.BusinessTripPurpose);
            viewFields.Add(StringConstant.DefaultSPListField.CreatedField);
            List<BusinessTripManagement> itemCollection = this.GetByQuery(this.BuildQueryGetListOfTasks(fromEmployee), viewFields.ToArray());
            if (itemCollection != null)
            {
                foreach (BusinessTripManagement item in itemCollection)
                {
                    Delegation delegation = new Delegation(item);
                    listOfTasks.Add(delegation);
                }
            }
            return listOfTasks;
        }

        private string BuildQueryGetListOfTasks(EmployeeInfo employeeInfo)
        {
            string filterStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";

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
                                        <Eq>
                                            <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                            <Value Type='User'>{2}</Value>
                                        </Eq>
                                     </And>
                                  </And>
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.BusinessTripManagement.ToString(), employeeInfo.ADAccount.ID);
            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                List<int> itemIds = taskManagementCollection.Where(t => t.ItemId > 0).Select(t => t.ItemId).ToList();

                if (itemIds != null && itemIds.Count > 0)
                {
                    filterStr = "";
                    foreach (var itemId in itemIds)
                    {
                        filterStr += string.Format("<Value Type = 'Number'>{0}</Value>", itemId);
                    }
                    if (!string.IsNullOrEmpty(filterStr))
                    {
                        filterStr = string.Format("<In><FieldRef Name = 'ID'/><Values>{0}</Values></In>", filterStr);
                    }
                }
            }

            filterStr = string.Format("<Where>{0}</Where>", filterStr);

            return filterStr;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            BusinessTripManagement businessTripManagement = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(businessTripManagement, currentWeb);
            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem ret = null;

            BusinessTripManagement businessTripManagement = this.ParseToEntity(listItem);

            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            var currentStep = _stepManagementDAL.GetStepManagement(businessTripManagement.ApprovalStatus, StepModuleList.BusinessTripManagement, businessTripManagement.CommonDepartment.LookupId);
            if (currentStep != null)
            {
                var currentStepApprover = this.GetApproverAtStep(businessTripManagement.CommonDepartment.LookupId, businessTripManagement.CommonLocation.LookupId, StepModuleList.BusinessTripManagement, currentStep.StepNumber);
                if (currentStepApprover != null)
                {
                    ret = new LookupItem() { LookupId = currentStepApprover.ID, LookupValue = currentStepApprover.FullName };
                }
            }

            return ret;
        }

        public bool IsValidTask(int listItemID)
        {
            return true;
        }
        #endregion

        #region Email
        public void SendEmail(BusinessTripManagement businessTripManagement, EmailTemplate emailTemplate, EmployeeInfo approver, EmployeeInfo toUser, string webUrl, bool isApprovalLink)
        {
            if (toUser == null || string.IsNullOrEmpty(toUser.Email) || emailTemplate == null || businessTripManagement == null || string.IsNullOrEmpty(webUrl))
                return;
            var content = HTTPUtility.HtmlDecode(emailTemplate.MailBody);

            content = content.Replace("{0}", toUser.FullName);
            if (emailTemplate.MailKey.ToLower() == "businesstripmanagement_approve" || emailTemplate.MailKey.ToLower() == "businesstripmanagement_reject")
            {
                content = content.Replace("{1}", approver.FullName);
                content = content.Replace("{2}", businessTripManagement.Requester.LookupValue);
                content = content.Replace("{3}", businessTripManagement.BusinessTripPurpose);

                string typeOfBusinessTrip = "";
                if (businessTripManagement.Domestic)
                {
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, 1033);
                    content = content.Replace("{4}", typeOfBusinessTrip);
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, 1066);
                    content = content.Replace("{5}", typeOfBusinessTrip);
                }
                else
                {
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, 1033);
                    content = content.Replace("{4}", typeOfBusinessTrip);
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, 1066);
                    content = content.Replace("{5}", typeOfBusinessTrip);
                }
            }
            else
            {
                content = content.Replace("{1}", businessTripManagement.Requester.LookupValue);
                content = content.Replace("{2}", businessTripManagement.BusinessTripPurpose);

                string typeOfBusinessTrip = "";
                if (businessTripManagement.Domestic)
                {
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, 1033);
                    content = content.Replace("{3}", typeOfBusinessTrip);
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, 1066);
                    content = content.Replace("{4}", typeOfBusinessTrip);
                }
                else
                {
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, 1033);
                    content = content.Replace("{3}", typeOfBusinessTrip);
                    typeOfBusinessTrip = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, 1066);
                    content = content.Replace("{4}", typeOfBusinessTrip);
                }
            }

            var link = GetEmailLinkByUserPosition(webUrl, toUser.EmployeePosition.LookupId, businessTripManagement.ID, isApprovalLink);
            content = content.Replace("#link", link);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            sendMailActivity.SendMail(webUrl, emailTemplate.MailSubject, toUser.Email, true, false, content);
        }

        public void SendDelegationEmail(BusinessTripManagement businessTripManagement, EmailTemplate emailTemplate, List<EmployeeInfo> toUsers, string webUrl)
        {
            if (toUsers == null || toUsers.Count == 0 || emailTemplate == null || businessTripManagement == null || string.IsNullOrEmpty(webUrl))
                return;

            string typeOfBusinessTripEN = "";
            string typeOfBusinessTripVN = "";
            if (businessTripManagement.Domestic)
            {
                typeOfBusinessTripEN = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, 1033);
                typeOfBusinessTripVN = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, 1066);
            }
            else
            {
                typeOfBusinessTripEN = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, 1033);
                typeOfBusinessTripVN = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, 1066);
            }
            var link = string.Format(@"{0}/SitePages/BusinessTripRequest.aspx?subSection=BusinessTripManagement&itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", webUrl, businessTripManagement.ID);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            foreach (var toUser in toUsers)
            {
                try
                {
                    if (!string.IsNullOrEmpty(toUser.Email))
                    {
                        var content = HTTPUtility.HtmlDecode(emailTemplate.MailBody);

                        content = content.Replace("{0}", toUser.FullName);
                        content = content.Replace("{1}", businessTripManagement.Requester.LookupValue);
                        content = content.Replace("{2}", businessTripManagement.BusinessTripPurpose);

                        content = content.Replace("{3}", typeOfBusinessTripEN);
                        content = content.Replace("{4}", typeOfBusinessTripVN);

                        content = content.Replace("#link", link);
                        sendMailActivity.SendMail(webUrl, emailTemplate.MailSubject, toUser.Email, true, false, content);
                    }
                }
                catch { }
            }
        }
        #endregion

        #region "Overview"

        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion

        #region Private Methods
        private string GetEmailLinkByUserPosition(string webUrl, int position, int itemId, bool isApprovalLink)
        {
            var link = string.Empty;
            string approvalLinkFormat = "{0}/SitePages/BusinessTripRequest.aspx?subSection=BusinessTripManagement&itemId={1}&Source={2}";

            switch (position)
            {
                case (int)StringConstant.EmployeePosition.Administrator:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.BusinessTripManagementAdmin}";
                    break;
                case (int)StringConstant.EmployeePosition.DepartmentHead:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.BusinessTripManagementManager}";
                    break;
                case (int)StringConstant.EmployeePosition.BOD:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.BusinessTripManagementBOD}";
                    break;
                default:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.BusinessTripManagementMember}";
                    break;
            }

            if (isApprovalLink)
            {
                link = string.Format(approvalLinkFormat, webUrl, itemId, HttpUtility.UrlEncode(link + "#tab2"));
            }
            else
            {
                link = string.Format(approvalLinkFormat, webUrl, itemId, HttpUtility.UrlEncode(link));
            }

            return link;
        }
        #endregion
    }
}
