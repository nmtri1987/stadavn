using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Builder;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class FreightManagementDAL : BaseDAL<FreightManagement>, IIDGenerator, IDelegationManager, IFilterTaskManager
    {
        private const string TASK_NAME = "Freight Request Approval/ Duyệt yêu cầu vận chuyển hàng hóa";
        private const string LINK_MAIL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/{0}.aspx?{1}";

        public FreightManagementDAL(string siteUrl) : base(siteUrl) { }

        public CommonApproverModel CreateApprovalList(int departmentId, int locationId)
        {
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            CommonApproverModel commonApproverModel = new CommonApproverModel();

            // Department Head
            var departmentHead = _employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.DepartmentHead, departmentId, locationId);
            if (departmentHead.Count > 0)
                commonApproverModel.Approver1 = departmentHead[0];

            // BOD 
            var bodUser = DepartmentListSingleton.GetDepartmentByID(departmentId, SiteUrl).BOD;
            if (bodUser != null && bodUser.ID > 0)
            {
                var bodImployeeInfo = _employeeInfoDAL.GetByADAccount(bodUser.UserName);
                commonApproverModel.Approver2 = bodImployeeInfo;
            }

            // Department Head of HC
            int departmentIdHC = DepartmentListSingleton.GetDepartmentByCode("HR", SiteUrl).ID;
            if (departmentIdHC != departmentId)
            {
                var departmentHeadOfHC = _employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.DepartmentHead, departmentIdHC, locationId);
                if (departmentHeadOfHC.Count > 0)
                    commonApproverModel.Approver3 = departmentHeadOfHC[0];
            }

            return commonApproverModel;
        }

        public int SaveOrUpdate(FreightManagement item)
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

        public int SaveOrUpdate(SPWeb spWeb, FreightManagement item)
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

                if (item.RequestDueDate != default(DateTime))
                {
                    spListItem[StringConstant.CommonSPListField.CommonReqDueDateField] = item.RequestDueDate;
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
                spListItem[StringConstant.FreightManagementList.RequestNoField] = GetNewFreightCode(requesterInfo.Department.LookupId, DateTime.Now);
                spListItem[StringConstant.FreightManagementList.IsValidRequestField] = item.IsValidRequest;

                if (item.DH != null && !string.IsNullOrEmpty(item.DH.UserName)) // Department Head
                {
                    SPUser departmentHead = SPContext.Current.Web.EnsureUser(item.DH.UserName);
                    SPFieldUserValue departmentHeadValue = new SPFieldUserValue(SPContext.Current.Web, departmentHead.ID, departmentHead.LoginName);
                    spListItem[StringConstant.FreightManagementList.DHField] = departmentHeadValue;
                }
                if (item.BOD != null && !string.IsNullOrEmpty(item.BOD.UserName)) // BOD
                {
                    SPUser bod = SPContext.Current.Web.EnsureUser(item.BOD.UserName);
                    SPFieldUserValue bodValue = new SPFieldUserValue(SPContext.Current.Web, bod.ID, bod.LoginName);
                    spListItem[StringConstant.FreightManagementList.BODField] = bodValue;
                }
                if (item.AdminDept != null && !string.IsNullOrEmpty(item.AdminDept.UserName)) // Admin Dept
                {
                    SPUser adminDept = SPContext.Current.Web.EnsureUser(item.AdminDept.UserName);
                    SPFieldUserValue adminDeptValue = new SPFieldUserValue(SPContext.Current.Web, adminDept.ID, adminDept.LoginName);
                    spListItem[StringConstant.FreightManagementList.AdminDeptField] = adminDeptValue;
                }

            }
            spListItem[StringConstant.FreightManagementList.BringerField] = item.Bringer != null ? item.Bringer.LookupId : 0;
            spListItem[StringConstant.FreightManagementList.BringerDepartmentField] = item.BringerDepartment != null ? item.BringerDepartment.LookupId : 0;
            spListItem[StringConstant.FreightManagementList.BringerLocationField] = item.BringerLocation != null ? item.BringerLocation.LookupId : 0;
            spListItem[StringConstant.FreightManagementList.BringerNameField] = item.BringerName;
            spListItem[StringConstant.FreightManagementList.CompanyNameField] = item.CompanyName;
            spListItem[StringConstant.FreightManagementList.TransportTimeField] = item.TransportTime;
            spListItem[StringConstant.FreightManagementList.CompanyVehicleField] = item.CompanyVehicle;
            spListItem[StringConstant.FreightManagementList.ReasonField] = item.Reason;
            spListItem[StringConstant.FreightManagementList.ReceiverField] = item.Receiver;
            spListItem[StringConstant.FreightManagementList.ReceiverDepartmentLookupField] = item.ReceiverDepartmentLookup != null ? item.ReceiverDepartmentLookup.LookupId : 0;
            spListItem[StringConstant.FreightManagementList.ReceiverDepartmentTextField] = item.ReceiverDepartmentText;
            spListItem[StringConstant.FreightManagementList.ReceiverPhoneField] = item.ReceiverPhone;
            spListItem[StringConstant.FreightManagementList.FreightTypeField] = item.FreightType;
            spListItem[StringConstant.FreightManagementList.ReturnedGoodsField] = item.ReturnedGoods;
            spListItem[StringConstant.FreightManagementList.HighPriorityField] = item.HighPriority;
            spListItem[StringConstant.FreightManagementList.IsFinishedField] = item.IsFinished;
            spListItem[StringConstant.FreightManagementList.OtherReasonField] = item.OtherReason;
            spListItem[StringConstant.FreightManagementList.VehicleLookupField] = item.VehicleLookup != null ? item.VehicleLookup.LookupId : 0;
            spListItem[StringConstant.FreightManagementList.SecurityNotesField] = string.IsNullOrEmpty(item.SecurityNotes) ? "" : item.SecurityNotes;
            spListItem[StringConstant.CommonSPListField.CommonCommentField] = item.Comment;

            spWeb.AllowUnsafeUpdates = true;
            spListItem.Update();
            itemId = spListItem.ID;
            spWeb.AllowUnsafeUpdates = false;

            return itemId;
        }

        public FreightManagement StartWorkFlow(SPWeb spWeb, FreightManagement freightManagement, int freightId)
        {
            if (freightId == 0) return null;

            SPList freightList = spWeb.TryGetSPList(spWeb.Url + this.ListUrl);
            TaskManagement taskManagement = new TaskManagement();

            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = freightManagement.RequestDueDate;
            taskManagement.PercentComplete = 0;
            taskManagement.ItemId = freightManagement.ID;
            taskManagement.ItemURL = freightList.DefaultDisplayFormUrl + "?ID=" + freightId;
            taskManagement.ListURL = freightList.DefaultViewUrl;
            taskManagement.TaskName = TASK_NAME;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.StepModule = StepModuleList.FreightManagement.ToString();
            taskManagement.Department = freightManagement.Department.LookupId > 0 ? freightManagement.Department : null;

            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            EmployeeInfo departmentHeadInfo = _employeeInfoDAL.GetByADAccount(freightManagement.DH.ID);
            if (freightManagement.Requester.LookupId == departmentHeadInfo.ID)
            {
                taskManagement.StepStatus = StepStatusList.BODApproval;
                taskManagement.AssignedTo = freightManagement.BOD;
            }
            else
            {
                taskManagement.StepStatus = StepStatusList.DHApproval;
                taskManagement.AssignedTo = freightManagement.DH;
            }

            taskManagement.NextAssign = null;
            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            var nextStep = _stepManagementDAL.GetNextStepManagement(taskManagement.StepStatus, StepModuleList.FreightManagement, freightManagement.Department.LookupId);
            if (nextStep != null)
            {
                var nextAssign = GetApproverAtStep(freightManagement.Department.LookupId, freightManagement.Location.LookupId, StepModuleList.FreightManagement, nextStep.StepNumber);
                if (nextAssign != null)
                {
                    taskManagement.NextAssign = nextAssign.ADAccount;
                }
            }

            TaskManagementDAL taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            int retId = taskManagementDAL.SaveItem(taskManagement);

            freightManagement.ApprovalStatus = taskManagement.StepStatus;
            freightManagement.Comment = string.Empty;
            freightManagement.SecurityNotes = string.Empty;
            freightManagement.IsFinished = false;
            freightManagement.HighPriority = false;

            this.SaveOrUpdate(freightManagement);

            EmailTemplateDAL _emailTemplateDAL = new EmailTemplateDAL(this.SiteUrl);
            EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("FreightManagement_Request");
            EmployeeInfo toUser = _employeeInfoDAL.GetByADAccount(taskManagement.AssignedTo.ID);
            SendEmail(freightManagement, emailTemplate, null, toUser, this.SiteUrl, true);

            try
            {
                List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(toUser.ID, StringConstant.FreightManagementList.ListUrl, freightManagement.ID);
                SendDelegationEmail(freightManagement, emailTemplate, toUsers, this.SiteUrl);
            }
            catch { }

            return freightManagement;
        }

        public FreightManagement RunWorkFlow(FreightManagement freightManagement, TaskManagement taskOfPrevStep, EmployeeInfo approver, EmployeeInfo currentStepApprover)
        {
            if (freightManagement == null) return null;

            TaskManagement taskManagement = new TaskManagement();

            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = freightManagement.RequestDueDate;
            taskManagement.PercentComplete = 0;
            taskManagement.ItemId = freightManagement.ID;
            taskManagement.ItemURL = taskOfPrevStep.ItemURL;
            taskManagement.ListURL = taskOfPrevStep.ListURL;
            taskManagement.TaskName = TASK_NAME;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.StepModule = StepModuleList.FreightManagement.ToString();
            taskManagement.Department = freightManagement.Department.LookupId > 0 ? freightManagement.Department : null;

            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            User assignee = null;
            User nextAssignee = null;
            string stepStatus = string.Empty;

            StepManagement nextStep = null;
            if (currentStepApprover.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead &&
                currentStepApprover.ADAccount.ID == freightManagement.DH.ID)
            {
                nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.FreightManagement, freightManagement.Department.LookupId);
                if (freightManagement.HighPriority == true)
                {
                    if (nextStep != null)
                    {
                        stepStatus = nextStep.StepStatus;
                        var approverAtStep = GetApproverAtStep(freightManagement.Department.LookupId, freightManagement.Location.LookupId, StepModuleList.FreightManagement, nextStep.StepNumber);
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
                        nextStep = _stepManagementDAL.GetNextStepManagement(nextStep.StepStatus, StepModuleList.FreightManagement, freightManagement.Department.LookupId);
                        if (nextStep != null)
                        {
                            stepStatus = nextStep.StepStatus;
                            var approverAtStep = GetApproverAtStep(freightManagement.Department.LookupId, freightManagement.Location.LookupId, StepModuleList.FreightManagement, nextStep.StepNumber);
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
                nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.FreightManagement, freightManagement.Department.LookupId);
                if (nextStep != null)
                {
                    stepStatus = nextStep.StepStatus;
                    var approverAtStep = GetApproverAtStep(freightManagement.Department.LookupId, freightManagement.Location.LookupId, StepModuleList.FreightManagement, nextStep.StepNumber);
                    if (approverAtStep != null)
                    {
                        assignee = approverAtStep.ADAccount;
                    }
                }
            }

            // get next approver
            if (nextStep != null)
            {
                nextStep = _stepManagementDAL.GetNextStepManagement(nextStep.StepStatus, StepModuleList.FreightManagement, freightManagement.Department.LookupId);
                if (nextStep != null)
                {
                    var nextApprover = GetApproverAtStep(freightManagement.Department.LookupId, freightManagement.Location.LookupId, StepModuleList.FreightManagement, nextStep.StepNumber);
                    if (nextApprover != null)
                    {
                        nextAssignee = nextApprover.ADAccount;
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
                freightManagement.ApprovalStatus = StringConstant.ApprovalStatus.Approved.ToString();
                this.SaveOrUpdate(freightManagement);

                EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("FreightManagement_Approve");
                EmployeeInfo toUser = _employeeInfoDAL.GetByID(freightManagement.Requester.LookupId);
                SendEmail(freightManagement, emailTemplate, approver, toUser, this.SiteUrl, false);
            }
            else if (assignee != null)
            {
                TaskManagementDAL taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
                int retId = taskManagementDAL.SaveItem(taskManagement);

                freightManagement.ApprovalStatus = taskManagement.StepStatus;
                this.SaveOrUpdate(freightManagement);

                EmailTemplate emailTemplate = _emailTemplateDAL.GetByKey("FreightManagement_Request");
                EmployeeInfo toUser = _employeeInfoDAL.GetByADAccount(assignee.ID);
                SendEmail(freightManagement, emailTemplate, approver, toUser, this.SiteUrl, true);

                try
                {
                    List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(toUser.ID, StringConstant.FreightManagementList.ListUrl, freightManagement.ID);
                    SendDelegationEmail(freightManagement, emailTemplate, toUsers, this.SiteUrl);
                }
                catch { }
            }

            return freightManagement;
        }

        public List<FreightManagement> GetByDepartment(int departmentId, DateTime date)
        {
            string queryString = string.Format(@"<Where>
                                          <And>
                                             <Eq>
                                                <FieldRef Name='{0}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{1}</Value>
                                             </Eq>
                                             <Eq>
                                                <FieldRef Name='Created' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{2}</Value>
                                             </Eq>
                                          </And>
                                       </Where>
                                       <OrderBy>
                                          <FieldRef Name='ID' Ascending='False' />
                                       </OrderBy>", StringConstant.CommonSPListField.CommonDepartmentField, departmentId, date.ToString(StringConstant.DateFormatTZForCAML));
            SPQuery spQuery = new SPQuery()
            {
                Query = queryString,
                RowLimit = 1
            };

            var list = GetByQuery(spQuery, string.Empty).ToList();
            return list;
        }

        public List<FreightManagement> GetFilteredFreights(DateTime fromDate, DateTime toDate, int deptId, List<int> locationIds, int vehId)
        {
            List<FreightManagement> queriedFreights = null;
            string queryStr = string.Empty;

            try
            {
                queryStr = $@"<And>
                                <Geq>
                                    <FieldRef Name='TransportTime' />
                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                </Geq>
                                <Leq>
                                    <FieldRef Name='TransportTime' />
                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                </Leq>
                            </And>";

                if (vehId > 0)
                {
                    queryStr = $@"<And>{queryStr}
                                      <And>
                                          <Eq>
                                              <FieldRef Name='{StringConstant.FreightManagementList.VehicleLookupField}' LookupId='TRUE' />
                                              <Value Type='Lookup'>{vehId}</Value>
                                          </Eq>
                                          <Eq>
                                              <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}'/>
                                              <Value Type='Text'>{StringConstant.ApprovalStatus.Approved}</Value>
                                          </Eq>
                                      </And>
                                   </And>";
                }
                else
                {
                    queryStr = $@"<And>{queryStr}
                                      <And>
                                        <Eq>
                                            <FieldRef Name='CompanyVehicle' />
                                            <Value Type='Boolean'>1</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                            <Value Type='Text'>{ StringConstant.ApprovalStatus.Approved}</Value>
                                        </Eq>
                                    </And>
                                </And>";
                }

                if (deptId > 0)
                {
                    queryStr = $@"<And>
                                    {queryStr}
                                    <Eq>
                                        <FieldRef Name='{StringConstant.CommonSPListField.CommonDepartmentField}' LookupId='TRUE' />
                                        <Value Type='Lookup'>{deptId}</Value>
                                    </Eq>
                                </And>";
                }

                string locationFilter = CommonHelper.BuildFilterCommonLocation(locationIds);
                if (!string.IsNullOrEmpty(locationFilter))
                {
                    queryStr = $@"<And>{locationFilter}{queryStr}</And>";
                }

                queryStr = string.Format(" <Where>{0}</Where>", queryStr);
                queriedFreights = GetByQuery(queryStr);
            }
            catch { }

            return queriedFreights;
        }

        public EmployeeInfo GetApproverAtStep(int departmentID, int locationID, StepModuleList StepModule, int stepNumber)
        {
            EmployeeInfo approverInfo = null;
            CommonApproverModel approvalList = CreateApprovalList(departmentID, locationID);
            if (approvalList != null)
            {
                Type typeCommonApproverModel = typeof(CommonApproverModel);
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                PropertyInfo approverProp = typeCommonApproverModel.GetProperty(string.Format("Approver{0}", stepNumber), bindingFlags);
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

        #region Delegation
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> listOfTasks = new List<Delegation>();

            List<string> viewFields = new List<string>() { };
            viewFields.Add(StringConstant.CommonSPListField.RequesterField);
            viewFields.Add(StringConstant.CommonSPListField.CommonDepartmentField);
            viewFields.Add(StringConstant.FreightManagementList.BringerField);
            viewFields.Add(StringConstant.FreightManagementList.BringerNameField);
            viewFields.Add(StringConstant.FreightManagementList.CompanyVehicleField);
            viewFields.Add(StringConstant.FreightManagementList.ReasonField);
            viewFields.Add(StringConstant.DefaultSPListField.CreatedField);
            List<FreightManagement> itemCollection = this.GetByQuery(this.BuildQueryGetListOfTasks(fromEmployee), viewFields.ToArray());
            if (itemCollection != null)
            {
                foreach (var item in itemCollection)
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
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.FreightManagement.ToString(), employeeInfo.ADAccount.ID);
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
            FreightManagement freightManagement = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(freightManagement, currentWeb);
            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem ret = null;

            FreightManagement freightManagement = this.ParseToEntity(listItem);

            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            var currentStep = _stepManagementDAL.GetStepManagement(freightManagement.ApprovalStatus, StepModuleList.FreightManagement, freightManagement.Department.LookupId);
            if (currentStep != null)
            {
                var currentStepApprover = this.GetApproverAtStep(freightManagement.Department.LookupId, freightManagement.Location.LookupId, StepModuleList.FreightManagement, currentStep.StepNumber);
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
        public void SendEmail(FreightManagement freightManagementItem, EmailTemplate emailTemplate, EmployeeInfo approver, EmployeeInfo toUser, string webUrl, bool isApprovalLink)
        {
            if (toUser == null || string.IsNullOrEmpty(toUser.Email) || emailTemplate == null || freightManagementItem == null || string.IsNullOrEmpty(webUrl))
                return;
            var content = HTTPUtility.HtmlDecode(emailTemplate.MailBody);

            var bringerName = string.Empty;
            var bringerNameInVietnamese = string.Empty;
            if (freightManagementItem.CompanyVehicle)
            {
                bringerName = ResourceHelper.GetLocalizedString("FreightRequest_CompanyVehicle", StringConstant.ResourcesFileWebPages, 1033);
                bringerNameInVietnamese = ResourceHelper.GetLocalizedString("FreightRequest_CompanyVehicle", StringConstant.ResourcesFileWebPages, 1066);
            }
            else
            {
                bringerName = bringerNameInVietnamese = freightManagementItem.Bringer.LookupId > 0 ? freightManagementItem.Bringer.LookupValue : freightManagementItem.BringerName;
            }

            if (emailTemplate.MailKey.ToLower() == "freightmanagement_approve" || emailTemplate.MailKey.ToLower() == "freightmanagement_reject")
            {
                content = content.Replace("{0}", toUser.FullName);
                content = content.Replace("{1}", approver.FullName);
                content = content.Replace("{2}", freightManagementItem.Requester.LookupValue);
                content = content.Replace("{3}", bringerName);
                content = content.Replace("{4}", freightManagementItem.Receiver);
                content = content.Replace("{5}", bringerNameInVietnamese);
            }
            else
            {
                content = content.Replace("{0}", toUser.FullName);
                content = content.Replace("{1}", freightManagementItem.Requester.LookupValue);
                content = content.Replace("{2}", bringerName);
                content = content.Replace("{3}", freightManagementItem.Receiver);
                content = content.Replace("{4}", bringerNameInVietnamese);
            }
            var link = GetEmailLinkByUserPosition(webUrl, toUser.EmployeePosition.LookupId, freightManagementItem.ID, isApprovalLink);
            content = content.Replace("#link", link);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            sendMailActivity.SendMail(webUrl, emailTemplate.MailSubject, toUser.Email, true, false, content);
        }

        public void SendDelegationEmail(FreightManagement freightManagementItem, EmailTemplate emailTemplate, List<EmployeeInfo> toUsers, string webUrl)
        {
            if (toUsers == null || toUsers.Count == 0 || emailTemplate == null || freightManagementItem == null || string.IsNullOrEmpty(webUrl))
                return;

            SendEmailActivity sendMailActivity = new SendEmailActivity();
            var link = string.Format(@"{0}/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", webUrl, freightManagementItem.ID);
            foreach (var toUser in toUsers)
            {
                try
                {
                    if (!string.IsNullOrEmpty(toUser.Email))
                    {
                        var content = HTTPUtility.HtmlDecode(emailTemplate.MailBody);

                        var bringerName = string.Empty;
                        var bringerNameInVietnamese = string.Empty;
                        if (freightManagementItem.CompanyVehicle)
                        {
                            bringerName = ResourceHelper.GetLocalizedString("FreightRequest_CompanyVehicle", StringConstant.ResourcesFileWebPages, 1033);
                            bringerNameInVietnamese = ResourceHelper.GetLocalizedString("FreightRequest_CompanyVehicle", StringConstant.ResourcesFileWebPages, 1066);
                        }
                        else
                        {
                            bringerName = bringerNameInVietnamese = freightManagementItem.Bringer.LookupId > 0 ? freightManagementItem.Bringer.LookupValue : freightManagementItem.BringerName;
                        }

                        content = content.Replace("{0}", toUser.FullName);
                        content = content.Replace("{1}", freightManagementItem.Requester.LookupValue);
                        content = content.Replace("{2}", bringerName);
                        content = content.Replace("{3}", freightManagementItem.Receiver);
                        content = content.Replace("{4}", bringerNameInVietnamese);
                        content = content.Replace("#link", link);
                        sendMailActivity.SendMail(webUrl, emailTemplate.MailSubject, toUser.Email, true, false, content);
                    }
                }
                catch { }
            }
        }

        public FreightManagement SetDueDate(FreightManagement freightManagement)
        {
            DateTime reqDueDate = freightManagement.TransportTime.Date;
            //if (reqDueDate == DateTime.Now.Date)
            //{
            //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            //else
            //{
            //    reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            freightManagement.RequestDueDate = reqDueDate;

            return freightManagement;
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
            string approvalLinkFormat = "{0}/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId={1}&Source={2}";

            switch (position)
            {
                case (int)StringConstant.EmployeePosition.Administrator:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.FreightManagementAdmin}";
                    break;
                case (int)StringConstant.EmployeePosition.DepartmentHead:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.FreightManagementManager}";
                    break;
                case (int)StringConstant.EmployeePosition.BOD:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.FreightManagementBOD}";
                    break;
                default:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.FreightManagementMember}";
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

        public string GetNewID(int departmentId)
        {
            return GetNewFreightCode(departmentId, DateTime.Now);
        }

        public string GetNewID(int departmentId, DateTime date)
        {
            return GetNewFreightCode(departmentId, date);
        }

        private string GetCodePattern(string departmentCodeNumber, DateTime date, int requestNumber)
        {
            if (string.IsNullOrEmpty(departmentCodeNumber) || date == null || requestNumber <= 0)
            {
                return string.Empty;
            }
            else
            {
                return string.Format("{0}{1}{2}", departmentCodeNumber, date.ToString("yyMMdd"), padNumber(requestNumber));
            }
        }

        private string padNumber(int number)
        {
            return number <= 9 ? string.Format("0{0}", number) : number.ToString();
        }

        private string GetNewFreightCode(int departmentId, DateTime date)
        {
            string code = string.Empty;
            var department = DepartmentListSingleton.GetDepartmentByID(departmentId, this.SiteUrl);
            if (department != null)
            {
                var items = this.GetByDepartment(departmentId, date);

                int newRequestNumber = 0;
                if (items != null && items.Count > 0)
                {
                    string requestNo = items[0].RequestNo;
                    if (!string.IsNullOrEmpty(requestNo))
                    {
                        int.TryParse(requestNo.Substring(requestNo.Length - 2, 2), out newRequestNumber);
                    }
                }
                newRequestNumber += 1;

                code = GetCodePattern(department.DepartmentNo, date, newRequestNumber);
            }
            return code;
        }
        #endregion
    }
}
