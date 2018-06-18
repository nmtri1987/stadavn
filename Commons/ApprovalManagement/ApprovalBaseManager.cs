using Microsoft.SharePoint;
using Microsoft.SharePoint.Email;
using Microsoft.SharePoint.Utilities;
using RBVH.Core.SharePoint;
using RBVH.Core.SharePoint.Extension;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Helpers.EvalExpression;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Web;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.ApprovalManagement
{
    /// <summary>
    /// ApprovalBaseManager class.
    /// </summary>
    public class ApprovalBaseManager
    {
        #region Constants
        /// <summary>
        /// EvalExpressionContext
        /// </summary>
        public class EvalExpressionContext
        {
            public class Keys
            {
                /// <summary>
                /// CurrentItem
                /// </summary>
                public const string CurrentItem = "CurrentItem";

                /// <summary>
                /// EmployeeInfo
                /// </summary>
                public const string EmployeeInfo = "EmployeeInfo";
            }
        }

        /// <summary>
        /// BOD
        /// </summary>
        // public const string EmployeePosition_BOD_CODE = "BOD";
        #endregion

        #region Attributes
        private string siteUrl;
        private string listName;
        private string listUrl;
        private WorkflowStepDAL workflowStepDAL;
        private EmployeeInfoDAL employeeInfoDAL;
        private EmployeePositionDAL employeePositionDAL;
        private DepartmentDAL departmentDAL;
        private WorkflowHistoryDAL workflowHistoryDAL;
        private WorkflowEmailTemplateDAL workflowEmailTemplateDAL;
        private SPListItem currentItem;
        private SPWeb currentWeb;
        private EmployeeInfo currentEmployee;
        private SPFieldLookupValue currentLocation;
        private SPFieldLookupValue currentDepartment;
        private List<EmployeeInfo> listOfEmployeesEmailTo;
        private List<EmployeeInfo> listOfEmployeesEmailCc;
        private bool isAdditionalStep;
        private SPFieldLookupValueCollection pendingAtEmployees;
        private EmployeeInfo creator;
        private bool isWorkflowCompleted;
        /// <summary>
        /// If currentItem[ApprovalFields.AssignFrom] is not null, get this value. Otherwise get current employeee.
        /// </summary>
        private SPFieldLookupValue assignFrom;
        private SPFieldLookupValue assignTo;
        private LookupItem delegationFromEmployee;
        private Dictionary<string, object> additionalInfoEmailObject;
        #endregion

        #region Properties
        public string SiteUrl
        {
            get
            {
                return siteUrl;
            }
        }

        public string ListName
        {
            get
            {
                return listName;
            }
        }

        public string ListUrl
        {
            get
            {
                return this.listUrl;
            }
        }

        public WorkflowStepDAL WorkflowStepDAL
        {
            get
            {
                return workflowStepDAL;
            }
        }

        /// <summary>
        /// EmployeeInfoDAL
        /// </summary>
        public EmployeeInfoDAL EmployeeInfoDAL
        {
            get
            {
                return employeeInfoDAL;
            }
        }

        /// <summary>
        /// Get EmployeePositionDAL object.
        /// </summary>
        public EmployeePositionDAL EmployeePositionDAL
        {
            get
            {
                return employeePositionDAL;
            }
        }

        /// <summary>
        /// Get DepartmentDAL object.
        /// </summary>
        public DepartmentDAL DepartmentDAL
        {
            get
            {
                return this.departmentDAL;
            }
        }

        public WorkflowHistoryDAL WorkflowHistoryDAL
        {
            get
            {
                return this.workflowHistoryDAL;
            }
        }

        public WorkflowEmailTemplateDAL WorkflowEmailTemplateDAL
        {
            get
            {
                return this.workflowEmailTemplateDAL;
            }
        }

        public SPListItem CurrentItem
        {
            get
            {
                return currentItem;
            }
        }

        /// <summary>
        /// Get SPContext.Current.Web object.
        /// </summary>
        public SPWeb CurrentWeb
        {
            get
            {
                return this.currentWeb;
            }
        }

        public EmployeeInfo CurrentEmployee
        {
            get
            {
                return currentEmployee;
            }
        }

        public SPFieldLookupValue CurrentLocation
        {
            get
            {
                return currentLocation;
            }
        }

        public SPFieldLookupValue CurrentDepartment
        {
            get
            {
                return currentDepartment;
            }
        }

        /// <summary>
        /// Get or Set list of employees to send email to.
        /// </summary>
        public List<EmployeeInfo> ListOfEmployeesEmailTo
        {
            get
            {
                return listOfEmployeesEmailTo;
            }

            set
            {
                listOfEmployeesEmailTo = value;
            }
        }

        /// <summary>
        /// Get or Set list of employees to send email cc.
        /// </summary>
        public List<EmployeeInfo> ListOfEmployeesEmailCc
        {
            get
            {
                return listOfEmployeesEmailCc;
            }

            set
            {
                listOfEmployeesEmailCc = value;
            }
        }

        public bool IsAdditionalStep
        {
            get
            {
                return IsAdditionalStep;
            }
        }

        public SPFieldLookupValueCollection PendingAtEmployees
        {
            get
            {
                return pendingAtEmployees;
            }

            set
            {
                pendingAtEmployees = value;
            }
        }

        /// <summary>
        /// Get employee who create for this item.
        /// </summary>
        public EmployeeInfo Creator
        {
            get
            {
                return this.creator;
            }
        }

        public bool IsWorkflowCompleted
        {
            get
            {
                return this.isWorkflowCompleted;
            }
            set
            {
                this.isWorkflowCompleted = value;
            }
        }

        /// <summary>
        /// Get or Set assignFrom attribute.
        /// </summary>
        public SPFieldLookupValue AssignFrom
        {
            get
            {
                return this.assignFrom;
            }
            set
            {
                this.assignFrom = value;
            }
        }

        /// <summary>
        /// Get or Set assignTo attribute.
        /// </summary>
        public SPFieldLookupValue AssignTo
        {
            get
            {
                return this.assignTo;
            }
            set
            {
                this.assignTo = value;
            }
        }

        public LookupItem DelegationFromEmployee
        {
            get
            {
                return this.delegationFromEmployee;
            }
        }

        /// <summary>
        /// Get AdditionalInfoEmail Dictionary object.
        /// </summary>
        public Dictionary<string, object> AdditionalInfoEmailObject
        {
            get
            {
                return this.additionalInfoEmailObject;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// ApprovalBaseManager
        /// </summary>
        protected ApprovalBaseManager()
        {
            listOfEmployeesEmailTo = new List<EmployeeInfo>();
            listOfEmployeesEmailCc = new List<EmployeeInfo>();
            isAdditionalStep = false;
            isWorkflowCompleted = false;
            additionalInfoEmailObject = new Dictionary<string, object>();
        }

        /// <summary>
        /// ApprovalBaseManager
        /// </summary>
        /// <param name="currentEmployee"></param>
        /// <param name="currentItem"></param>
        /// <param name="currentWeb"></param>
        //public ApprovalBaseManager(/*SPFieldLookupValue currentEmployee, SPListItem currentItem, SPWeb currentWeb*/)
        //{
        //    //this.currentEmployee = currentEmployee;
        //    //this.currentItem = currentItem;
        //    //this.currentWeb = currentWeb;
        //    //this.workflowStepsDAL = new WorkflowStepsDAL(this.currentWeb.Site.Url);
        //}
        public ApprovalBaseManager(string siteUrl, string listName, SPListItem currentItem, SPWeb currentWeb)
        {
            this.siteUrl = siteUrl;
            this.listName = listName;
            this.listUrl = currentItem.ParentList.RootFolder.ServerRelativeUrl;
            this.workflowStepDAL = new WorkflowStepDAL(this.siteUrl);
            this.employeeInfoDAL = new EmployeeInfoDAL(this.siteUrl);
            this.employeePositionDAL = new EmployeePositionDAL(this.siteUrl);
            this.departmentDAL = new DepartmentDAL(this.siteUrl);
            this.workflowHistoryDAL = new WorkflowHistoryDAL(this.siteUrl);
            this.workflowEmailTemplateDAL = new WorkflowEmailTemplateDAL(this.siteUrl);
            this.currentItem = currentItem;
            this.currentWeb = currentWeb;

            this.currentEmployee = GetCurrentEmployeeInfo();
            this.listOfEmployeesEmailTo = new List<EmployeeInfo>();
            this.listOfEmployeesEmailCc = new List<EmployeeInfo>();

            if (this.currentEmployee != null)
            {
                this.currentLocation = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.CommonLocation]);
                if (this.currentLocation == null)
                {
                    this.currentLocation = new SPFieldLookupValue();
                    this.currentLocation.LookupId = this.currentEmployee.FactoryLocation.LookupId;
                }
                this.currentDepartment = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.CommonDepartment]);
                if (this.currentDepartment == null)
                {
                    this.currentDepartment = new SPFieldLookupValue();
                    this.currentDepartment.LookupId = this.currentEmployee.Department.LookupId;
                }
            }

            this.isAdditionalStep = ObjectHelper.GetBoolean(this.currentItem[ApprovalFields.IsAdditionalStep]);
            this.pendingAtEmployees = ObjectHelper.GetSPFieldLookupValueCollection(this.currentItem[ApprovalFields.PendingAt]);
            SPFieldLookupValue creatorLookupValue = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.Creator]);
            if (creatorLookupValue != null)
            {
                this.creator = employeeInfoDAL.GetByID(creatorLookupValue.LookupId);
            }
            else
            {
                this.creator = this.currentEmployee;
            }
            this.isWorkflowCompleted = false;

            #region AssignFrom
            if (this.currentItem[ApprovalFields.AssignFrom] != null)
            {
                this.assignFrom = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.AssignFrom]);
            }
            else
            {
                if (this.currentEmployee != null)
                {
                    this.assignFrom = new SPFieldLookupValue(this.currentEmployee.ID, this.currentEmployee.EmployeeID);
                }
            }
            #endregion

            // AssignTo
            this.assignTo = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.AssignTo]);

            this.additionalInfoEmailObject = new Dictionary<string, object>();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// sender : BasicEvaluationContext, e: null
        /// </summary>
        public event EventHandler OnBeforeBuildSubjectEmail;
        /// <summary>
        ///  sender : BasicEvaluationContext, e: null
        /// </summary>
        public event EventHandler OnBeforeBuildBodyEmail;
        #endregion

        #region WORKFLOW
        /// <summary>
        /// WORKFLOW - Submit
        /// </summary>
        /// <returns></returns>
        public virtual bool Submit()
        {
            #region BEGIN Submit
            //BEGIN Submit
            //-Step 1: Lấy thông tin đối tượng current employee
            //- Step 2: Lấy WorkflowStepItems < -(từ WorkflowStep list) với điều kiện: ListName = Current List Name, FirstStep = True
            //- Step 3: SatisfiedConditionWorkflowStepsItems < -Filter lại danh sách WorkflowStepsItems để lấy những item thoã điều kiện
            //- Step 4:
            // +Nếu SatisfiedConditionWorkflowStepsItems.Count = 1 thì
            //       ++ SatisfiedConditionWorkflowStepsItem = SatisfiedConditionWorkflowStepsItems[0]
            //       ++ CurrentItem["CurrentStep"] <- SatisfiedConditionWorkflowStepsItem["CurrentStep"]
            //       ++ CurrentItem["NextStep"] <- SatisfiedConditionWorkflowStepsItem["NextStep"]
            //       ++ CurrentItem["Approvers"] <- Find danh sách employees: CurrentEmployee.Location, CurrentEmployee.Department, CurrentItem["CurrentStep"].Position
            //       ++ CurrentItem["Status"] <- "Submitted"
            //   + Nếu SatisfiedConditionWorkflowStepsItems.Count = 0 thì throw exception lỗi cấu hình qui trình bị thiếu vui lòng kiểm tra lại
            //    +Nếu SatisfiedConditionWorkflowStepsItems.Count >= 2  thì throw exception lỗi cấu hình qui trình bị sai có nhiều nhánh vui lòng kiểm tra lại
            //END Submit
            #endregion

            WorkflowStep firstWorkflowStepItem = GetFirstWorkflowStep();
            if (firstWorkflowStepItem == null)
            {
                throw new Exception(StringMessages.TheFirstStepOfWorkflowIsNotExisted);
            }

            LookupItem nextStepLookupItem = firstWorkflowStepItem.NextStep;
            if (nextStepLookupItem != null)
            {
                WorkflowStep nextFirstWorkflowStepItem = FindNextWorkflowStepItemByCurrentStep(nextStepLookupItem);

                if (nextFirstWorkflowStepItem == null)
                {
                    throw new Exception(StringMessages.TheNextStepOfWorkflowIsNotExisted);
                }

                // Set Creator
                currentItem[ApprovalFields.Creator] = currentEmployee.ID;
                // Set CurrentStep
                currentItem[ApprovalFields.CurrentStep] = nextStepLookupItem.LookupId;
                #region Set NextStep
                //if (nextFirstWorkflowStepItem.NextStep != null)
                //{
                //    currentItem[ApprovalFields.NextStep] = nextFirstWorkflowStepItem.NextStep.LookupId;
                //}
                #endregion
                #region Set Approvers
                List<EmployeeInfo> employeeInfos = GetListOfEmployeesByPosition(nextStepLookupItem);
                SPFieldLookupValueCollection employees = ConvertToSPFieldLookupValueCollection(employeeInfos);
                currentItem[ApprovalFields.PendingAt] = employees;
                #endregion
                // Set Status
                currentItem[ApprovalFields.Status] = Status.Submitted;
                // Set WFStatus
                currentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.InProgress;
                // Set Location
                currentItem[ApprovalFields.CommonLocation] = this.currentLocation;
                // Set Department
                currentItem[ApprovalFields.CommonDepartment] = this.currentDepartment;
                //// Set additional step for current item
                //SetAdditionalStepForCurrentItem();
                // Save current item
                currentItem.Update();

                #region Prepareration for sending email

                // Add approvers into list of email To.
                listOfEmployeesEmailTo.AddRange(employeeInfos);

                // Get list of delegated employee
                var delegatedEmployees = GetListOfDelegatedEmployees(employeeInfos);
                if (delegatedEmployees != null && delegatedEmployees.Count > 0)
                {
                    listOfEmployeesEmailTo.AddRange(delegatedEmployees);
                }

                SetListOfEmployeesToSendEmailToAndCc(firstWorkflowStepItem);

                #endregion
            }
            else
            {
                throw new Exception(StringMessages.TheFirstStepOfWorkflowIsNotExisted);
            }

            return true;
        }

        /// <summary>
        /// WORKFLOW - Approve
        /// </summary>
        /// <returns></returns>
        public virtual bool Approve()
        {
            #region BEGIN Approve
            //BEGIN Approve
            //- Step 1: Remove Current Employee ra khỏi danh sách Approvers
            //- Step 2: 
            // +Nếu danh sách Approvers rỗng
            //        ++Move qua next Step:
            //            +++ Nếu là Addition Step
            //                ...
            //            +++ Nếu không phải là Additional Step
            //                ...
            //            +++ Set CurrentItem["IsAdditionalStep"] <- False
            //        + Nếu danh sách Approvers không rỗng
            //            ++ Do nothing
            //END Approve
            #endregion

            #region Get main approver
            int approverID = 0;

            if (IsEmployeeProcessing())
            {
                approverID = this.currentEmployee.ID;
            }
            else if (IsEmployeeReAssigned())
            {
                approverID = this.assignFrom.LookupId;
            }
            else if (IsEmployeeDelegated())
            {
                if (this.delegationFromEmployee != null)
                {
                    approverID = this.delegationFromEmployee.LookupId;
                }
            }
            #endregion

            // If removing current employee out of list of pending at employee is successful.
            #region if(this.pendingAtEmployees.Remove(approverID))
            if (this.pendingAtEmployees.Remove(approverID))
            {
                if (this.pendingAtEmployees.IsEmpty())
                {
                    #region this.pendingAtEmployees.IsEmpty()

                    #region M 2017.08.14.
                    // SPFieldLookupValue nextStep = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.NextStep]);
                    SPFieldLookupValue nextStep = null;
                    SPFieldLookupValue currentStepLookupValue = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.CurrentStep]);
                    WorkflowStep nextWorkflowStep = null;
                    if (currentStepLookupValue != null)
                    {
                        nextWorkflowStep = FindNextWorkflowStepItemByCurrentStep(currentStepLookupValue.LookupId);
                        if (nextWorkflowStep != null)
                        {
                            if (nextWorkflowStep.NextStep != null)
                            {
                                nextStep = nextWorkflowStep.NextStep.ToSPFieldLookupValue();
                            }
                        }
                    }
                    #endregion

                    // Nếu có NextStep
                    if (nextStep != null)
                    {
                        #region Move to next step

                        #region Set CurrentStep, NextStep and PendingAtEmployees.

                        this.isAdditionalStep = ObjectHelper.GetBoolean(this.currentItem[ApprovalFields.IsAdditionalStep]);

                        if (this.isAdditionalStep)
                        {
                            #region Nếu là Addition Step.

                            LookupItem nextStepLookupItem = nextStep.ToLookupItem();

                            SPFieldLookupValue additionalDepartment = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.AdditionalDepartment]);
                            // Get list of employees at current step
                            List<EmployeeInfo> listOfEmployees = GetListOfEmployees(this.currentLocation.LookupId, additionalDepartment.LookupId, nextStepLookupItem.LookupId);
                            if (listOfEmployees != null)
                            {
                                this.pendingAtEmployees = listOfEmployees.ToSPFieldLookupValueCollection();
                                this.listOfEmployeesEmailTo.AddRange(listOfEmployees);

                                // Get list of delegated employee
                                var delegatedEmployees = GetListOfDelegatedEmployees(listOfEmployees);
                                if (delegatedEmployees != null && delegatedEmployees.Count > 0)
                                {
                                    listOfEmployeesEmailTo.AddRange(delegatedEmployees);
                                }
                            }

                            // Set CurrentStep
                            //SPFieldLookupValue additionalStep = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.AdditionalStep]);
                            this.currentItem[ApprovalFields.CurrentStep] = this.currentItem[ApprovalFields.AdditionalStep];
                            // Set NextStep
                            //SPFieldLookupValue additionalNextStep = ObjectHelper.GetSPFieldLookupValue(this.currentItem[ApprovalFields.AdditionalNextStep]);
                            //this.currentItem[ApprovalFields.NextStep] = this.currentItem[ApprovalFields.AdditionalNextStep];

                            // Set CurrentItem["IsAdditionalStep"] <- False
                            this.currentItem[ApprovalFields.IsAdditionalStep] = false;

                            #endregion
                        }
                        else
                        {
                            #region Nếu không phải là Additional Step.

                            // Get list of employees at current step
                            LookupItem nextStepLookupItem = nextStep.ToLookupItem();
                            // Get list of employees at current step
                            List<EmployeeInfo> listOfEmployees = GetListOfEmployeesByPosition(nextStepLookupItem);
                            if (listOfEmployees != null && listOfEmployees.Count > 0)
                            {
                                this.pendingAtEmployees = listOfEmployees.ToSPFieldLookupValueCollection();
                                this.listOfEmployeesEmailTo.AddRange(listOfEmployees);

                                // Get list of delegated employee
                                var delegatedEmployees = GetListOfDelegatedEmployees(listOfEmployees);
                                if (delegatedEmployees != null && delegatedEmployees.Count > 0)
                                {
                                    listOfEmployeesEmailTo.AddRange(delegatedEmployees);
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(StringMessages.ApproversWereNotFoundForLocationDepartmentPosition, this.currentEmployee.FactoryLocation.LookupValue, this.currentEmployee.Department.LookupValue, nextStep.LookupValue));
                            }

                            // Set Current Step
                            //this.currentItem[ApprovalFields.CurrentStep] = this.currentItem[ApprovalFields.NextStep];
                            //this.currentItem[ApprovalFields.CurrentStep] = this.currentItem[ApprovalFields.NextStep];

                            // Find next step
                            //WorkflowStep nextWorkflowStepItem = FindNextWorkflowStepItemByCurrentStep(nextStepLookupItem);
                            //// Set Next Step
                            //if (nextWorkflowStepItem.NextStep != null)
                            //{
                            //    this.currentItem[ApprovalFields.NextStep] = nextWorkflowStepItem.NextStep.ToSPFieldLookupValue();
                            //}
                            //else
                            //{
                            //    this.currentItem[ApprovalFields.NextStep] = null;
                            //}
                            // Set Next Step
                            this.currentItem[ApprovalFields.CurrentStep] = nextStep;

                            // Set list of employees to send email To and Cc
                            //SetListOfEmployeesToSendEmailToAndCc(nextWorkflowStepItem);
                            SetListOfEmployeesToSendEmailToAndCc(nextWorkflowStep);

                            #endregion
                        }
                        #endregion

                        // Set Status
                        this.currentItem[ApprovalFields.Status] = string.Format("{0} {1}", this.currentEmployee.EmployeePosition.LookupValue, Status.Approved);

                        #endregion
                    }
                    else    // Nếu không có NextStep --> kết thúc qui trình
                    {
                        #region Nếu không có NextStep -->kết thúc qui trình
                        this.currentItem[ApprovalFields.Status] = Status.Completed;
                        // Set WFStauts
                        this.currentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.Approved;
                        this.currentItem[ApprovalFields.CurrentStep] = null;
                        this.isWorkflowCompleted = true;

                        // this.ResetAdditionalApprovalInfo();
                        #endregion
                    }

                    #region Reset Assign Info
                    if (IsEmployeeReAssigned())
                    {
                        this.currentItem[ApprovalFields.AssignFrom] = null;
                        this.currentItem[ApprovalFields.AssignTo] = null;
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    // Do nothing
                    // Another employee will be approved continuous.
                }

                // Set pending at employees
                this.currentItem[ApprovalFields.PendingAt] = this.pendingAtEmployees;

                // Update current item
                this.currentItem.Update();
            }
            #endregion

            return true;
        }

        /// <summary>
        /// WORKFLOW - Reject
        /// </summary>
        /// <returns></returns>
        public virtual bool Reject()
        {
            // Set Status
            currentItem[ApprovalFields.Status] = Status.Rejected;
            // Set WFStatus
            currentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.Rejected;
            // Set PendingAts
            currentItem[ApprovalFields.PendingAt] = currentItem[ApprovalFields.Creator];
            // Set CurrentStep
            currentItem[ApprovalFields.CurrentStep] = null;
            // Set NextStep
            currentItem[ApprovalFields.NextStep] = null;
            // Reset additional approval info
            ResetAdditionalApprovalInfo();
            // Update current item
            currentItem.Update();

            // set employee who is create request.
            this.listOfEmployeesEmailTo.Add(this.creator);

            return true;
        }

        /// <summary>
        /// WORKFLOW - ReAssign
        /// </summary>
        /// <param name="assignedToEmployeeId">The employee id.</param>
        /// <returns></returns>
        public virtual bool ReAssign(int assignedToEmployeeId)
        {
            var res = false;
            if (assignedToEmployeeId > 0)
            {
                this.currentItem[ApprovalFields.AssignFrom] = this.assignFrom;
                this.currentItem[ApprovalFields.AssignTo] = assignedToEmployeeId;
                this.currentItem.Update();

                // set employee who is assiend new task to receive email.
                this.listOfEmployeesEmailTo.Add(employeeInfoDAL.GetByID(assignedToEmployeeId));
            }
            return res;
        }

        /// <summary>
        /// WORKFLOW - Terminate
        /// </summary>
        /// <returns></returns>
        public virtual bool Terminate()
        {
            // Set Status
            currentItem[ApprovalFields.Status] = Status.CancelWorkflow;
            // Set WFStatus
            currentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.Cancelled;
            // Set PendingAts
            currentItem[ApprovalFields.PendingAt] = null;
            // Set CurrentStep
            currentItem[ApprovalFields.CurrentStep] = null;
            // Set NextStep
            currentItem[ApprovalFields.NextStep] = null;
            // Reset additional approval info
            ResetAdditionalApprovalInfo();
            // Update current item
            currentItem.Update();

            return true;
        }

        public static bool CancelWF(SPListItem listItem)
        {
            var res = false;

            // Set Status
            listItem[ApprovalFields.Status] = Status.CancelWorkflow;
            // Set WFStatus
            listItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.Cancelled;
            // Set PendingAts
            listItem[ApprovalFields.PendingAt] = null;
            // Set CurrentStep
            listItem[ApprovalFields.CurrentStep] = null;
            // Set NextStep
            listItem[ApprovalFields.NextStep] = null;
            // Reset additional approval info
            listItem[ApprovalFields.IsAdditionalStep] = false;
            listItem[ApprovalFields.AdditionalPreviousStep] = null;
            listItem[ApprovalFields.AdditionalStep] = null;
            listItem[ApprovalFields.AdditionalNextStep] = null;
            listItem[ApprovalFields.AdditionalDepartment] = null;
            // Update current item
            listItem.Update();

            res = true;

            return res;
        }
        #endregion

        #region Workflow Utils

        /// <summary>
        /// GetListOfWorkflowStepItems
        /// </summary>
        /// <param name="queryString">The CAML query string.</param>
        /// <returns></returns>
        private List<Models.WorkflowStep> GetListOfWorkflowStepItems(string queryString)
        {
            return workflowStepDAL.GetByQuery(queryString);
        }

        /// <summary>
        /// Get List of WorkflowStep items with codition that are ListName is current list name and CurrentStep is currentStep parametter.
        /// </summary>
        /// <param name="currentStep">The value of current step condition.</param>
        /// <returns></returns>
        private List<WorkflowStep> GetListOfWorkflowStepItems(LookupItem currentStep)
        {
            List<WorkflowStep> workflowStepItems = null;

            //string queryString = string.Format(@"<Where>
            //                                        <And>
            //                                            <Eq>
            //                                                <FieldRef Name='{0}' />
            //                                                <Value Type='Text'>{1}</Value>
            //                                             </Eq>
            //                                            <Eq>
            //                                                <FieldRef Name='{2}' LookupId='True' />
            //                                                <Value Type='Lookup'>{3}</Value>
            //                                             </Eq>
            //                                        </And>
            //                                   </Where>",
            //                                   StringConstant.WorkflowStepsList.Fields.ListName, listName,
            //                                   StringConstant.WorkflowStepsList.Fields.CurrentStep, currentStep.LookupId);
            //workflowStepItems = GetListOfWorkflowStepItems(queryString);
            if (currentStep != null)
            {
                workflowStepItems = GetListOfWorkflowStepItems(currentStep.LookupId);
            }

            return workflowStepItems;
        }

        /// <summary>
        /// GetListOfWorkflowStepItems
        /// </summary>
        /// <param name="currentStepId"></param>
        /// <returns></returns>
        private List<WorkflowStep> GetListOfWorkflowStepItems(int currentStepId)
        {
            List<WorkflowStep> workflowStepItems = null;

            string queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' />
                                                            <Value Type='Text'>{1}</Value>
                                                         </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{2}' LookupId='True' />
                                                            <Value Type='Lookup'>{3}</Value>
                                                         </Eq>
                                                    </And>
                                               </Where>",
                                               StringConstant.WorkflowStepsList.Fields.ListName, listName,
                                               StringConstant.WorkflowStepsList.Fields.CurrentStep, currentStepId);
            workflowStepItems = GetListOfWorkflowStepItems(queryString);

            return workflowStepItems;
        }

        /// <summary>
        /// Filter lại danh sách WorkflowStepsItems để lấy những item thoã điều kiện. Filter list item workflowStepItems with satisfy condition item.
        /// </summary>
        /// <param name="workflowStepItems"></param>
        /// <returns></returns>
        private List<WorkflowStep> GetListOfSatisfiedWorkflowStepItems(List<WorkflowStep> workflowStepItems)
        {
            List<WorkflowStep> satisfiedWorkflowStepItems = null;

            if (workflowStepItems != null && workflowStepItems.Count > 0)
            {
                satisfiedWorkflowStepItems = new List<WorkflowStep>();

                foreach (var workflowStepItem in workflowStepItems)
                {
                    string conditionalExpression = ObjectHelper.GetString(workflowStepItem.ConditionalExpression);

                    // If have branch condition
                    if (!string.IsNullOrEmpty(conditionalExpression.Trim()))
                    {
                        bool satisfiedCondition = false;
                        var context = new BasicEvaluationContext();
                        context.Objects[ApprovalManagement.ApprovalBaseManager.EvalExpressionContext.Keys.CurrentItem] = currentItem;
                        var val = context.Eval(conditionalExpression);
                        bool.TryParse(val.ToString(), out satisfiedCondition);
                        if (satisfiedCondition)
                        {
                            satisfiedWorkflowStepItems.Add(workflowStepItem);
                        }
                    }
                    else
                    {
                        satisfiedWorkflowStepItems.Add(workflowStepItem);
                    }
                }
            }

            return satisfiedWorkflowStepItems;
        }

        /// <summary>
        /// Get current employee info.
        /// </summary>
        /// <returns></returns>
        private EmployeeInfo GetCurrentEmployeeInfo()
        {
            EmployeeInfo currentStadaEmployee =
                HttpContext.Current.Session[StringConstant.EmployeeLogedin] as EmployeeInfo;

            //User is not common account, we should get from employee list
            if (currentStadaEmployee == null)
            {
                SPUser spUser = SPContext.Current.Web.CurrentUser;
                if (spUser != null)
                {
                    currentStadaEmployee = employeeInfoDAL.GetByADAccount(spUser.ID);
                }
            }
            return currentStadaEmployee;
        }

        /// <summary>
        /// Get first step of Workflow.
        /// </summary>
        /// <returns></returns>
        protected WorkflowStep GetFirstWorkflowStep()
        {
            WorkflowStep firstWorkflowStep = null;

            LookupItem currentStep = currentEmployee.EmployeePosition;
            firstWorkflowStep = FindNextWorkflowStepItemByCurrentStep(currentStep);

            return firstWorkflowStep;
        }

        /// <summary>
        /// FindWorkflowStepItemByCurrentStep
        /// </summary>
        /// <param name="currentStep"></param>
        /// <returns></returns>
        protected WorkflowStep FindNextWorkflowStepItemByCurrentStep(LookupItem currentStep)
        {
            WorkflowStep workflowStepItem = null;

            //List<WorkflowStep> listOfWorkflowStepItems = GetListOfWorkflowStepItems(currentStep);
            //if (listOfWorkflowStepItems != null && listOfWorkflowStepItems.Count > 0)
            //{
            //    List<WorkflowStep> listOfSatisfiedWorkflowStepItems = GetListOfSatisfiedWorkflowStepItems(listOfWorkflowStepItems);
            //    if (listOfSatisfiedWorkflowStepItems != null && listOfSatisfiedWorkflowStepItems.Count > 0)
            //    {
            //        if (listOfSatisfiedWorkflowStepItems.Count == 1)
            //        {
            //            workflowStepItem = listOfSatisfiedWorkflowStepItems[0];
            //        }
            //        else // listOfSatisfiedWorkflowStepItems.Count >= 2 
            //        {
            //            throw new Exception(StringMessages.TheStepOfWorkflowHasManyNextSteps);
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception(StringMessages.TheStepOfWorkflowIsNotExisted);
            //    }
            //}
            if (currentStep != null)
            {
                workflowStepItem = FindNextWorkflowStepItemByCurrentStep(currentStep.LookupId);
            }

            return workflowStepItem;
        }

        private WorkflowStep FindNextWorkflowStepItemByCurrentStep(int currentStepId)
        {
            WorkflowStep workflowStepItem = null;

            if (currentStepId > 0)
            {
                List<WorkflowStep> listOfWorkflowStepItems = GetListOfWorkflowStepItems(currentStepId);
                if (listOfWorkflowStepItems != null && listOfWorkflowStepItems.Count > 0)
                {
                    List<WorkflowStep> listOfSatisfiedWorkflowStepItems = GetListOfSatisfiedWorkflowStepItems(listOfWorkflowStepItems);
                    if (listOfSatisfiedWorkflowStepItems != null && listOfSatisfiedWorkflowStepItems.Count > 0)
                    {
                        if (listOfSatisfiedWorkflowStepItems.Count == 1)
                        {
                            workflowStepItem = listOfSatisfiedWorkflowStepItems[0];
                        }
                        else // listOfSatisfiedWorkflowStepItems.Count >= 2 
                        {
                            throw new Exception(StringMessages.TheStepOfWorkflowHasManyNextSteps);
                        }
                    }
                    else
                    {
                        throw new Exception(StringMessages.TheStepOfWorkflowIsNotExisted);
                    }
                }
            }

            return workflowStepItem;
        }

        /// <summary>
        /// Get list of employees that have location and department the same with current employee.
        /// </summary>
        /// <param name="position">The position of employee.</param>
        /// <param name="isCurrentLocationOfEmployee">If true, using current employee's location.</param>
        /// <returns>The list of EmployeeInfo.</returns>
        protected List<EmployeeInfo> GetListOfEmployeesByPosition(LookupItem position)
        {
            List<EmployeeInfo> employees = null;

            int employeeFactoryId = this.currentLocation.LookupId;
            int employeeDepartmentId = this.currentDepartment.LookupId;
            int employeePositionId = position.LookupId;

            employees = GetListOfEmployees(employeeFactoryId, employeeDepartmentId, employeePositionId);

            return employees;
        }

        /// <summary>
        /// GetListOfEmployeesByPosition
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        protected List<EmployeeInfo> GetListOfEmployeesByPosition(List<LookupItem> positions)
        {
            List<EmployeeInfo> employeeInfos = null;

            if (positions != null)
            {
                employeeInfos = new List<EmployeeInfo>();
                foreach (var position in positions)
                {
                    var employees = GetListOfEmployeesByPosition(position);
                    if (employees != null && employees.Count > 0)
                    {
                        employeeInfos.AddRange(employees);
                    }
                }
            }

            return employeeInfos;
        }

        protected virtual List<EmployeeInfo> GetListOfBODEmployees(int locationId, int departmentId, int positionId)
        {
            List<EmployeeInfo> employees = null;

            // Step 1: Get BOD info from department of current employee
            Department departmentBOD = departmentDAL.GetByID(departmentId);

            // Step 2: Get list of employees by location and position and AD Account
            string queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{2}' LookupId='True' />
                                                                <Value Type='Lookup'>{3}</Value>
                                                             </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{4}' LookupId='True' />
                                                                <Value Type='Lookup'>{5}</Value>
                                                             </Eq>
                                                        </And>
                                                    </And>
                                               </Where>", StringConstant.EmployeeInfoList.FactoryLocationField, locationId,
                                           StringConstant.EmployeeInfoList.EmployeePositionField, positionId,
                                           StringConstant.EmployeeInfoList.ADAccountField, departmentBOD.BOD.ID);
            employees = employeeInfoDAL.GetByQuery(queryString);

            return employees;
        }

        /// <summary>
        /// GetListOfEmployees
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public List<EmployeeInfo> GetListOfEmployees(int locationId, int departmentId, int positionId)
        {
            List<EmployeeInfo> employees = null;

            //Models.EmployeePosition employeePosition = employeePositionDAL.GetByID(positionId);

            // Hard code BOD position because EmployeeInfo does not storage department information for BOD position. I was embittered for this. :(
            //if (string.Compare(EmployeePosition_BOD_CODE, employeePosition.Code, true) == 0)
            if (IsEmployeeBOD(positionId))
            {
                #region BOD

                //// Step 1: Get BOD info from department of current employee
                //Department departmentBOD = departmentDAL.GetByID(departmentId);

                //// Step 2: Get list of employees by location and position and AD Account
                //string queryString = string.Format(@"<Where>
                //                                    <And>
                //                                        <Eq>
                //                                            <FieldRef Name='{0}' LookupId='True' />
                //                                            <Value Type='Lookup'>{1}</Value>
                //                                         </Eq>
                //                                        <And>
                //                                            <Eq>
                //                                                <FieldRef Name='{2}' LookupId='True' />
                //                                                <Value Type='Lookup'>{3}</Value>
                //                                             </Eq>
                //                                            <Eq>
                //                                                <FieldRef Name='{4}' LookupId='True' />
                //                                                <Value Type='Lookup'>{5}</Value>
                //                                             </Eq>
                //                                        </And>
                //                                    </And>
                //                               </Where>", StringConstant.EmployeeInfoList.FactoryLocationField, locationId,
                //                               StringConstant.EmployeeInfoList.EmployeePositionField, positionId,
                //                               StringConstant.EmployeeInfoList.ADAccountField, departmentBOD.BOD.ID);
                //employees = employeeInfoDAL.GetByQuery(queryString);

                employees = GetListOfBODEmployees(locationId, departmentId, positionId);


                #endregion
            }
            else if (IsEmployeeDRM(positionId))
            {
                #region DRM Direct Manager (Quản Lý Trực Tiếp). Tìm Người Quản Lý Trực Tiếp của người đang xử lý.

                if (this.currentEmployee.Manager != null)
                {
                    var employee = employeeInfoDAL.GetByID(this.currentEmployee.Manager.LookupId);
                    {
                        if (employee != null)
                        {
                            employees = new List<EmployeeInfo>();
                            employees.Add(employee);
                        }
                    }
                }

                #endregion
            }
            else    // Employee binh thuong: DEH (Truong Phong), GRL (Pho Phong), ...
            {
                #region Not BOD

                string queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{2}' LookupId='True' />
                                                                <Value Type='Lookup'>{3}</Value>
                                                             </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{4}' LookupId='True' />
                                                                <Value Type='Lookup'>{5}</Value>
                                                             </Eq>
                                                        </And>
                                                    </And>
                                               </Where>", StringConstant.EmployeeInfoList.FactoryLocationField, locationId,
                                               StringConstant.EmployeeInfoList.DepartmentField, departmentId,
                                               StringConstant.EmployeeInfoList.EmployeePositionField, positionId);

                employees = employeeInfoDAL.GetByQuery(queryString);

                #endregion
            }

            return employees;
        }

        /// <summary>
        /// ConvertToSPFieldLookupValueCollection
        /// </summary>
        /// <param name="employeeInfos"></param>
        /// <returns></returns>
        public SPFieldLookupValueCollection ConvertToSPFieldLookupValueCollection(List<EmployeeInfo> employeeInfos)
        {
            SPFieldLookupValueCollection employees = null;

            if (employeeInfos != null && employeeInfos.Count > 0)
            {
                employees = new SPFieldLookupValueCollection();

                foreach (var employeeInfo in employeeInfos)
                {
                    SPFieldLookupValue employee = new SPFieldLookupValue(employeeInfo.ID, employeeInfo.EmployeeID);
                    employees.Add(employee);
                }
            }

            return employees;
        }

        /// <summary>
        /// SetListOfEmployeesToSendEmailToAndCc
        /// </summary>
        /// <param name="workflowStepItem"></param>
        protected void SetListOfEmployeesToSendEmailToAndCc(WorkflowStep workflowStepItem)
        {
            #region To

            #region Roles
            if (workflowStepItem.NotificationEmailToRoles != null)
            {
                List<EmployeeInfo> employeeInfosEmailToTemp = GetListOfEmployeesByPosition(workflowStepItem.NotificationEmailToRoles);
                if (employeeInfosEmailToTemp != null && employeeInfosEmailToTemp.Count > 0)
                {
                    listOfEmployeesEmailTo.AddRange(employeeInfosEmailToTemp);
                }
            }
            #endregion

            #region Empployees
            if (workflowStepItem.NotificationEmailToEmployees != null)
            {
                foreach (var notificationEmailToEmployee in workflowStepItem.NotificationEmailToEmployees)
                {
                    EmployeeInfo employeeInfo = employeeInfoDAL.GetByID(notificationEmailToEmployee.LookupId);
                    listOfEmployeesEmailTo.Add(employeeInfo);
                }
            }
            #endregion

            #endregion

            #region Cc

            #region Roles
            if (workflowStepItem.NotificationEmailCcRoles != null)
            {
                List<EmployeeInfo> employeeInfosEmailCcTemp = GetListOfEmployeesByPosition(workflowStepItem.NotificationEmailCcRoles);
                if (employeeInfosEmailCcTemp != null && employeeInfosEmailCcTemp.Count > 0)
                {
                    listOfEmployeesEmailCc.AddRange(employeeInfosEmailCcTemp);
                }
            }
            #endregion

            #region Empployees
            if (workflowStepItem.NotificationEmailCcEmployees != null)
            {
                foreach (var notificationEmailCcEmployee in workflowStepItem.NotificationEmailCcEmployees)
                {
                    EmployeeInfo employeeInfo = employeeInfoDAL.GetByID(notificationEmailCcEmployee.LookupId);
                    listOfEmployeesEmailCc.Add(employeeInfo);
                }
            }
            #endregion

            #endregion
        }

        /// <summary>
        /// SetAdditionalStepForCurrentItem
        /// </summary>
        private void SetAdditionalStepForCurrentItem()
        {
            //-Nếu CurrentItem["NextStep"] == NULL
            //   ++ Nếu CurrentItem["CurrentStep"] == CurrentItem["AdditionalPrevisousStep"]
            //       ++ + CurrentItem["IsAdditionalStep"] <- TRUE
            //       ++ + CurrentItem["NextStep"] <- CurrentItem["AdditionalStep"]
            if (currentItem[ApprovalFields.NextStep] == null)
            {
                SPFieldLookupValue currentStep = ObjectHelper.GetSPFieldLookupValue(currentItem[ApprovalFields.CurrentStep]);
                SPFieldLookupValue additionalPrevisousStep = ObjectHelper.GetSPFieldLookupValue(currentItem[ApprovalFields.AdditionalPreviousStep]);
                if (currentStep != null && additionalPrevisousStep != null)
                {
                    if (currentStep.LookupId == additionalPrevisousStep.LookupId)
                    {
                        SPFieldLookupValue additionalStep = ObjectHelper.GetSPFieldLookupValue(currentItem[ApprovalFields.AdditionalStep]);
                        if (additionalStep != null)
                        {
                            currentItem[ApprovalFields.IsAdditionalStep] = true;
                            //currentItem[ApprovalFields.NextStep] = currentItem[ApprovalFields.AdditionalStep];
                        }
                        else
                        {
                            throw new Exception(string.Format("[{0}] {1}", "RBVH.Stada.Intranet.Biz.ApprovalManagement.ApprovalBaseManager.ApprovalBaseManager.SetAdditionalStepForCurrentItem", StringMessages.TheAdditionalStepWasNotFound));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check current employee who is processing?
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeProcessing()
        {
            bool res = false;

            // Check current user exist in list of approvers?
            if (this.pendingAtEmployees != null && this.pendingAtEmployees.Count > 0)
            {
                foreach (var emp in this.pendingAtEmployees)
                {
                    if (emp.LookupId == this.currentEmployee.ID)
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Check current employee who is assigned?
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeReAssigned()
        {
            bool res = false;

            if (this.assignTo != null)
            {
                if (this.currentEmployee.ID == assignTo.LookupId)
                {
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// Check current employee who is delegated?
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeDelegated()
        {
            bool res = false;

            int fromEmployeeId = 0;
            if (this.pendingAtEmployees != null && this.pendingAtEmployees.Count > 0)
            {
                fromEmployeeId = this.pendingAtEmployees[0].LookupId;
            }
            var delegation = DelegationPermissionManager.IsDelegation(fromEmployeeId, this.listUrl, this.currentItem.ID);
            if (delegation != null && delegation.FromEmployee != null)
            {
                this.delegationFromEmployee = delegation.FromEmployee;
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Check list item which is expired approval.
        /// </summary>
        /// <returns>If its expired approval, return true. Otherwise return false.</returns>
        public bool IsApprovalExpired(out DateTime expiredDate)
        {
            var res = false;

            expiredDate = new DateTime(9999, 12, 31);
            if (this.currentItem[CommonSPListField.CommonReqDueDateField] != null)
            {
                DateTime dueDate = (DateTime)currentItem[CommonSPListField.CommonReqDueDateField];
                if (DateTime.Today > dueDate)
                {
                    expiredDate = dueDate;
                    res = true;
                }
            }

            res = false; //turn off the feature which validates expiration of a request

            return res;
        }

        /// <summary>
        /// Check current employee who creates for this item?
        /// </summary>
        /// <returns>If return true, current employee created for this item. Otherwise return fasle.</returns>
        public bool IsEmployeeCreatorForThisItem()
        {
            return this.currentEmployee.ID == creator.ID ? true : false;
        }

        /// <summary>
        /// Check current employee who is BOD for position?
        /// </summary>
        /// <param name="positionId">The position ID.</param>
        /// <returns></returns>
        public bool IsEmployeeBOD(int positionId)
        {
            bool res = false;

            Models.EmployeePosition employeePosition = employeePositionDAL.GetByID(positionId);
            //if (string.Compare(EmployeePosition_BOD_CODE, employeePosition.Code, true) == 0)
            if (string.Compare(EmployeePositionCode.BOD, employeePosition.Code, true) == 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Check current employee who is BOD?
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeBOD()
        {
            bool res = false;

            if (this.currentEmployee.EmployeePosition != null)
            {
                int positionId = this.currentEmployee.EmployeePosition.LookupId;
                res = IsEmployeeBOD(positionId);
            }

            return res;
        }

        /// <summary>
        /// Check id postion co phai la [Direct Manager (Quản Lý Trực Tiếp)]
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public bool IsEmployeeDRM(int positionId)
        {
            bool res = false;

            Models.EmployeePosition employeePosition = employeePositionDAL.GetByID(positionId);
            if (string.Compare(EmployeePositionCode.DRM, employeePosition.Code, true) == 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// IsEmployeeDEH
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public bool IsEmployeeDEH(int positionId)
        {
            bool res = false;

            Models.EmployeePosition employeePosition = employeePositionDAL.GetByID(positionId);
            if (string.Compare(EmployeePositionCode.DEH, employeePosition.Code, true) == 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// IsEmployeeDEH
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeDEH()
        {
            bool res = false;

            if (this.currentEmployee.EmployeePosition != null)
            {
                int positionId = this.currentEmployee.EmployeePosition.LookupId;
                res = IsEmployeeDEH(positionId);
            }

            return res;
        }

        public bool IsEmployeeGRL(int positionId)
        {
            bool res = false;

            Models.EmployeePosition employeePosition = employeePositionDAL.GetByID(positionId);
            if (string.Compare(EmployeePositionCode.GRL, employeePosition.Code, true) == 0)
            {
                res = true;
            }

            return res;
        }

        public bool IsEmployeeGRL()
        {
            bool res = false;

            if (this.currentEmployee.EmployeePosition != null)
            {
                int positionId = this.currentEmployee.EmployeePosition.LookupId;
                res = IsEmployeeGRL(positionId);
            }

            return res;
        }

        /// <summary>
        /// ResetAdditionalApprovalInfo
        /// </summary>
        public void ResetAdditionalApprovalInfo()
        {
            this.currentItem[ApprovalFields.IsAdditionalStep] = false;
            this.currentItem[ApprovalFields.AdditionalPreviousStep] = null;
            this.currentItem[ApprovalFields.AdditionalStep] = null;
            this.currentItem[ApprovalFields.AdditionalNextStep] = null;
            this.currentItem[ApprovalFields.AdditionalDepartment] = null;
        }

        /// <summary>
        /// Get Factory, Department and Position of Employee.
        /// </summary>
        /// <param name="employeeItemID"></param>
        /// <param name="factoryId"></param>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        public void GetFactoryIdDepartmentIdAndPositionId(int employeeItemID, out int factoryId, out int departmentId, out int positionId)
        {
            factoryId = 0;
            departmentId = 0;
            positionId = 0;

            EmployeeInfo employeeInfo = employeeInfoDAL.GetByID(employeeItemID);
            if (employeeInfo != null)
            {
                if (employeeInfo.FactoryLocation != null)
                {
                    factoryId = employeeInfo.FactoryLocation.LookupId;
                }

                if (employeeInfo.Department != null)
                {
                    departmentId = employeeInfo.Department.LookupId;
                }

                if (employeeInfo.EmployeePosition != null)
                {
                    positionId = employeeInfo.EmployeePosition.LookupId;
                }
            }
        }

        public int GetDEHPositionId()
        {
            int DEHId = 0;

            string queryString = string.Format(@"<Where>
                                                    <Eq>
                                                        <FieldRef Name='{0}' />
                                                        <Value Type='Text'>{1}</Value>
                                                    </Eq>
                                               </Where>", "Code", EmployeePositionCode.DEH);

            List<Models.EmployeePosition> positions = this.EmployeePositionDAL.GetByQuery(queryString);
            if (positions != null && positions.Count > 0)
            {
                DEHId = positions[0].ID;
            }

            return DEHId;
        }

        /// <summary>
        /// Get list of employees who were delegated from list of employees.
        /// </summary>
        /// <param name="employeeInfos">The list of employees.</param>
        /// <returns>Th list of delegated employees.</returns>
        public List<EmployeeInfo> GetListOfDelegatedEmployees(List<EmployeeInfo> employeeInfos)
        {
            List<EmployeeInfo> res = null;

            if (employeeInfos != null && employeeInfos.Count > 0)
            {
                res = new List<EmployeeInfo>();

                foreach (var employee in employeeInfos)
                {
                    var delegatedEmployees = DelegationPermissionManager.GetListOfDelegatedEmployees(this.siteUrl, employee.ID, this.listUrl, this.currentItem.ID);
                    if (delegatedEmployees != null && delegatedEmployees.Count > 0)
                    {
                        res.AddRange(delegatedEmployees);
                    }
                }
            }

            return res;
        }

        #endregion

        #region Workflow History

        /// <summary>
        /// Post comment to workflow history.
        /// </summary>
        /// <param name="workflowAction">Approved, Rejected, Assigned, ...</param>
        /// <param name="comment">The content of comment.</param>
        /// <returns>If comment is posted, return true. Otherwise return false.</returns>
        public bool PostComment(string workflowAction, string comment)
        {
            bool res = false;

            try
            {
                WorkflowHistory workflowHistoryItem = new WorkflowHistory();
                //workflowHistoryItem.Status = string.Format(@"{0} {1}", this.currentEmployee.EmployeePosition.LookupValue, workflowAction);
                workflowHistoryItem.Status = workflowAction;
                workflowHistoryItem.CommonDate = System.DateTime.Now;
                workflowHistoryItem.PostedBy = this.currentEmployee.FullName;
                workflowHistoryItem.CommonComment = comment;
                workflowHistoryItem.ListName = this.listName;
                workflowHistoryItem.CommonItemID = this.currentItem.ID;

                List<WorkflowHistory> workflowHistories = new List<WorkflowHistory>();
                workflowHistories.Add(workflowHistoryItem);
                if (workflowHistoryDAL.SaveItems(workflowHistories))
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;
        }

        #endregion

        #region Email

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="workflowAction"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool SendEmail(EWorkflowAction workflowAction, string comment = "")
        {
            bool res = false;

            try
            {
                if (this.listOfEmployeesEmailTo.Count > 0)
                {
                    res = SendEmail(this.listOfEmployeesEmailTo, this.listOfEmployeesEmailCc, null, workflowAction, comment);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="toEmployees"></param>
        /// <param name="ccEmployees"></param>
        /// <param name="bccEmployees"></param>
        /// <param name="workflowAction"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool SendEmail(List<EmployeeInfo> toEmployees, List<EmployeeInfo> ccEmployees, List<EmployeeInfo> bccEmployees, EWorkflowAction workflowAction, string comment = "")
        {
            bool res = false;

            try
            {
                #region Build query to get email template.
                string queryString = string.Empty;

                if (workflowAction == EWorkflowAction.Submit)
                {
                    queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' />
                                                            <Value Type='Text'>{1}</Value>
                                                         </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{2}' />
                                                            <Value Type='Text'>{3}</Value>
                                                         </Eq>
                                                    </And>
                                               </Where>", WorkflowEmailTemplateList.Fields.ListName, this.listName,
                                               WorkflowEmailTemplateList.Fields.Action, Status.Submitted);
                }
                else if (workflowAction == EWorkflowAction.Approve)
                {
                    queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' />
                                                            <Value Type='Text'>{1}</Value>
                                                         </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{2}' />
                                                            <Value Type='Text'>{3}</Value>
                                                         </Eq>
                                                    </And>
                                               </Where>", WorkflowEmailTemplateList.Fields.ListName, this.listName,
                           WorkflowEmailTemplateList.Fields.Action, Status.Approved);
                }
                else if (workflowAction == EWorkflowAction.Reject)
                {
                    queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' />
                                                            <Value Type='Text'>{1}</Value>
                                                         </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{2}' />
                                                            <Value Type='Text'>{3}</Value>
                                                         </Eq>
                                                    </And>
                                               </Where>", WorkflowEmailTemplateList.Fields.ListName, this.listName,
                           WorkflowEmailTemplateList.Fields.Action, Status.Rejected);
                }
                else if (workflowAction == EWorkflowAction.ReAssign)
                {
                    queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' />
                                                            <Value Type='Text'>{1}</Value>
                                                         </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{2}' />
                                                            <Value Type='Text'>{3}</Value>
                                                         </Eq>
                                                    </And>
                                               </Where>", WorkflowEmailTemplateList.Fields.ListName, this.listName,
                           WorkflowEmailTemplateList.Fields.Action, Status.Assigned);
                }
                else if (workflowAction == EWorkflowAction.Terminate)
                {
                    queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' />
                                                            <Value Type='Text'>{1}</Value>
                                                         </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{2}' />
                                                            <Value Type='Text'>{3}</Value>
                                                         </Eq>
                                                    </And>
                                               </Where>", WorkflowEmailTemplateList.Fields.ListName, this.listName,
                           WorkflowEmailTemplateList.Fields.Action, Status.CancelWorkflow);
                }
                else if (workflowAction == EWorkflowAction.Complete)
                {
                    queryString = string.Format(@"<Where>
                                                    <And>
                                                        <Eq>
                                                            <FieldRef Name='{0}' />
                                                            <Value Type='Text'>{1}</Value>
                                                         </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{2}' />
                                                            <Value Type='Text'>{3}</Value>
                                                         </Eq>
                                                    </And>
                                               </Where>", WorkflowEmailTemplateList.Fields.ListName, this.listName,
                                                        WorkflowEmailTemplateList.Fields.Action, Status.Completed);
                }
                #endregion

                List<WorkflowEmailTemplate> workflowEmailTemplateItems = workflowEmailTemplateDAL.GetByQuery(queryString);
                if (workflowEmailTemplateItems != null && workflowEmailTemplateItems.Count > 0)
                {
                    WorkflowEmailTemplate workflowEmailTemplateItem = workflowEmailTemplateItems[0];

                    BasicEvaluationContext basicEvaluationContext = new BasicEvaluationContext();

                    #region Push data to build content email
                    // TODO (Duc.VoTan): Get string list of receivers
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.Receivers] = GetListOfReceiverNames(toEmployees);
                    //if (creator != null)
                    //{
                    //    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.Creator] = employeeInfoDAL.GetByID(this.creator.LookupId);
                    //}
                    //else
                    //{
                    //    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.Creator] = this.currentEmployee;
                    //}
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.Creator] = this.creator;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.CurrentEmployee] = this.currentEmployee;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.CurrentLocation] = this.currentLocation;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.CurrentDepartment] = this.currentDepartment;
                    if (this.currentDepartment != null)
                    {
                        var department = DepartmentListSingleton.GetDepartmentByID(this.currentDepartment.LookupId, this.siteUrl);
                        basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.RequestedDepartmentName_EN] = department.Name;
                        basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.RequestedDepartmentName_VN] = department.VietnameseName;
                    }
                    else
                    {
                        basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.RequestedDepartmentName_EN] = "";
                        basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.RequestedDepartmentName_VN] = "";
                    }
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.SiteUrl] = this.siteUrl;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.CurrentItem] = this.currentItem;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.ListName] = this.listName;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.ItemTitle] = this.currentItem.Title;
                    UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url);
                    uriBuilder.SetQuery(UrlParamName.FormModeParamName, FormMode.EditMode);
                    uriBuilder.SetQuery(UrlParamName.IDParamName, this.currentItem.ID.ToString());
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.LinkToItem] = uriBuilder.Uri.OriginalString;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.Comment] = comment;
                    basicEvaluationContext.Objects[WorkflowEmailTemplateObject.Keys.AdditionalInfoEmail] = this.additionalInfoEmailObject;
                    #endregion

                    string subject = BuildSubjectEmail(basicEvaluationContext, workflowEmailTemplateItem);
                    string body = BuildBodyEmail(basicEvaluationContext, workflowEmailTemplateItem);
                    res = SendEmail(toEmployees, ccEmployees, bccEmployees, subject, body);
                }
                else
                {
                    Exception ex = new Exception(string.Format(@"{0} - {1}", "RBVH.Stada.Intranet.Biz.ApprovalManagement.ApprovalBaseManager.SendEmail(EWorkflowAction workflowAction)", "The email template was not found."));
                    ULSLogging.LogError(ex);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="toEmployees"></param>
        /// <param name="ccEmployees"></param>
        /// <param name="bccEmployees"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendEmail(List<EmployeeInfo> toEmployees, List<EmployeeInfo> ccEmployees, List<EmployeeInfo> bccEmployees, string subject, string body)
        {
            bool res = false;

            try
            {
                if (toEmployees != null && toEmployees.Count > 0)
                {
                    string[] to = GetListOfEmail(toEmployees);
                    string[] cc = GetListOfEmail(ccEmployees);
                    string[] bcc = GetListOfEmail(bccEmployees);
                    res = SendEmail(to, cc, bcc, subject, body);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendEmail(string[] to, string[] cc, string[] bcc, string subject, string body)
        {
            bool res = false;

            try
            {
                string replyTo = "";
                StringDictionary additionalHeaders = new StringDictionary();
                IEnumerable<AttachedEmailResourceDefinition> attachments = new List<AttachedEmailResourceDefinition>(); ;
                IEnumerable<EmbeddedEmailResourceDefinition> embeddedResources = new List<EmbeddedEmailResourceDefinition>();
                if (to != null && to.Length > 0)
                {
                    Thread thread = new Thread(delegate ()
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate ()
                        {
                            using (SPSite spSite = new SPSite(this.currentWeb.Site.Url))
                            {
                                using (SPWeb spWeb = spSite.OpenWeb())
                                {
                                    SPUtility.SendEmail(spWeb, replyTo, to, cc, bcc, subject, additionalHeaders, body, attachments, embeddedResources);
                                }
                            }
                        });
                    });

                    thread.IsBackground = true;
                    thread.Start();

                    res = true;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;

        }

        /// <summary>
        /// Get list of email addresses from list of employees.
        /// </summary>
        /// <param name="employees">The list of Employees.</param>
        /// <returns></returns>
        public string[] GetListOfEmail(List<EmployeeInfo> employees)
        {
            string[] arrayAddress = null;

            if (employees != null && employees.Count > 0)
            {
                List<string> emails = new List<string>();

                foreach (var employee in employees)
                {
                    if (!string.IsNullOrEmpty(employee.Email) && !string.IsNullOrWhiteSpace(employee.Email))
                    {
                        emails.Add(employee.Email);
                    }
                }

                arrayAddress = emails.ToArray();
            }

            return arrayAddress;
        }

        /// <summary>
        /// Get list of names from list of employees.
        /// </summary>
        /// <param name="employees">The list of employees.</param>
        /// <returns>Nguyen Van A, Nguyen Van B,</returns>
        public string GetListOfReceiverNames(List<EmployeeInfo> employees)
        {
            StringBuilder receiverNamesBuilder = new StringBuilder();

            if (employees != null && employees.Count > 0)
            {
                foreach (var employee in employees)
                {
                    receiverNamesBuilder.Append(employee.FullName);
                    receiverNamesBuilder.Append(", ");
                }
            }

            return receiverNamesBuilder.ToString();
        }

        private string BuildSubjectEmail(BasicEvaluationContext basicEvaluationContext, WorkflowEmailTemplate workflowEmailTemplateItem)
        {
            var subject = this.currentItem.Title;

            try
            {
                if (OnBeforeBuildSubjectEmail != null)
                {
                    OnBeforeBuildSubjectEmail(basicEvaluationContext, null);
                }

                string subjectExpressionEvaluator = workflowEmailTemplateItem.Subject;
                subject = basicEvaluationContext.Eval<string>(subjectExpressionEvaluator);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return subject;
        }

        private string BuildBodyEmail(BasicEvaluationContext basicEvaluationContext, WorkflowEmailTemplate workflowEmailTemplateItem)
        {
            var body = string.Format(@"<a href='{0}'>{1}</a>", currentItem.GetDisplayFormUrl(), currentItem.Title);

            try
            {
                if (OnBeforeBuildBodyEmail != null)
                {
                    OnBeforeBuildBodyEmail(basicEvaluationContext, null);
                }

                string bodyExpressionEvaluator = workflowEmailTemplateItem.Body;
                body = basicEvaluationContext.Eval<string>(bodyExpressionEvaluator);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return body;
        }

        /// <summary>
        /// WorkflowEmailTemplateObject class.
        /// </summary>
        public class WorkflowEmailTemplateObject
        {
            public class Keys
            {
                /// <summary>
                /// Receivers
                /// </summary>
                public const string Receivers = "Receivers";

                /// <summary>
                /// Creator
                /// </summary>
                public const string Creator = "Creator";

                /// <summary>
                /// CurrentEmployee
                /// </summary>
                public const string CurrentEmployee = "CurrentEmployee";

                /// <summary>
                /// CurrentLocation
                /// </summary>
                public const string CurrentLocation = "CurrentLocation";

                /// <summary>
                /// CurrentDepartment
                /// </summary>
                public const string CurrentDepartment = "CurrentDepartment";

                /// <summary>
                /// RequestedDepartmentName_EN
                /// </summary>
                public const string RequestedDepartmentName_EN = "RequestedDepartmentName_EN";

                /// <summary>
                /// RequestedDepartmentName_VN
                /// </summary>
                public const string RequestedDepartmentName_VN = "RequestedDepartmentName_VN";

                /// <summary>
                /// SiteUrl
                /// </summary>
                public const string SiteUrl = "SiteUrl";

                /// <summary>
                /// CurrentItem
                /// </summary>
                public const string CurrentItem = "CurrentItem";

                /// <summary>
                /// ListName
                /// </summary>
                public const string ListName = "ListName";

                /// <summary>
                /// ItemTitle
                /// </summary>
                public const string ItemTitle = "ItemTitle";

                /// <summary>
                /// LinkToItem
                /// </summary>
                public const string LinkToItem = "LinkToItem";

                /// <summary>
                /// Comment
                /// </summary>
                public const string Comment = "Comment";

                /// <summary>
                /// AdditionalInfoEmailObject
                /// </summary>
                public const string AdditionalInfoEmail = "AdditionalInfoEmailObject";
            }
        }
        #endregion

    }
}
