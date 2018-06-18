using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Helpers.EvalExpression;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;

namespace RBVH.Stada.Intranet.Biz.ApprovalManagement
{
    /// <summary>
    /// Management approval for REQUEST module.
    /// </summary>
    public class RequestApprovalManagement : ApprovalBaseManager
    {
        #region Constants
        /// <summary>
        /// DEH
        /// </summary>
        private const string EmployeePosition_DEH_CODE = "DEH";

        /// <summary>
        /// PrintLinkEN
        /// </summary>
        public const string PrintLinkEN_Key = "PrintLinkEN";

        /// <summary>
        /// PrintLinkVN
        /// </summary>
        public const string PrintLinkVN_Key = "PrintLinkVN";

        /// <summary>
        /// ApproverEmailInfoEN
        /// </summary>
        public const string ApproverEmailInfoEN_Key = "ApproverEmailInfoEN";

        /// <summary>
        /// ApproverEmailInfoVN
        /// </summary>
        public const string ApproverEmailInfoVN_Key = "ApproverEmailInfoVN";

        #endregion

        #region Attributes
        //private SPFieldLookupValue currentStep;
        //private SPFieldLookupValue nextStep;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        /// <summary>
        /// RequestApprovalManagement
        /// </summary>
        protected RequestApprovalManagement() : base()
        {
            this.AdditionalInfoEmailObject.Add(PrintLinkEN_Key, "");
            this.AdditionalInfoEmailObject.Add(PrintLinkVN_Key, "");
            this.AdditionalInfoEmailObject.Add(ApproverEmailInfoEN_Key, "<p> You have a new <strong>The Request</strong>.");
            this.AdditionalInfoEmailObject.Add(ApproverEmailInfoVN_Key, "<p>Bạn có một <strong>Phiếu đề nghị</strong> cần duyệt.");
        }

        /// <summary>
        /// RequestApprovalManagement
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="listName"></param>
        /// <param name="currentItem"></param>
        /// <param name="currentEmployee"></param>
        public RequestApprovalManagement(string siteUrl, string listName, SPListItem currentItem, SPWeb currentWeb) : 
            base(siteUrl, listName, currentItem, currentWeb)
        {
            //this.OnBeforeBuildSubjectEmail += RequestApprovalManagement_OnBeforeBuildSubjectEmail;
            this.OnBeforeBuildBodyEmail += RequestApprovalManagement_OnBeforeBuildBodyEmail;

            this.AdditionalInfoEmailObject.Add(PrintLinkEN_Key, "");
            this.AdditionalInfoEmailObject.Add(PrintLinkVN_Key, "");

            this.AdditionalInfoEmailObject.Add(ApproverEmailInfoEN_Key, "You have a new <strong>The Request</strong>.");
            this.AdditionalInfoEmailObject.Add(ApproverEmailInfoVN_Key, "Bạn có một <strong>Phiếu đề nghị</strong> cần duyệt.");
        }
        #endregion

        #region Events
        //private void RequestApprovalManagement_OnBeforeBuildSubjectEmail(object sender, EventArgs e)
        //{
        //}

        private void RequestApprovalManagement_OnBeforeBuildBodyEmail(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            BasicEvaluationContext basicEvaluationContext = sender as BasicEvaluationContext;
            if (basicEvaluationContext != null)
            {
                SPFieldLookupValue requestTypeRefLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[StringConstant.RequestsList.RequestTypeRefField]);
                if (requestTypeRefLookupValue != null)
                {
                    basicEvaluationContext.Objects[StringConstant.RequestsList.RequestTypeRefField] = requestTypeRefLookupValue.LookupValue;
                }

                string status = ObjectHelper.GetString(this.CurrentItem[ApprovalFields.Status]);
                if (string.Compare(status, Status.Completed, true) == 0)
                {
                    string approverEmailEN = string.Format("Your <strong>Request</strong> has been <strong>approved</strong> by: {0}.</p>", this.CurrentEmployee.FullName);
                    string approverEmailVN = string.Format("<strong>Phiếu đề nghị</strong>của bạn được <strong>duyệt</strong> bởi: {0}.</p>", this.CurrentEmployee.FullName);
                    basicEvaluationContext.Objects[ApproverEmailInfoEN_Key] = approverEmailEN;
                    basicEvaluationContext.Objects[ApproverEmailInfoVN_Key] = approverEmailVN;
                }
            }
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Approve
        /// </summary>
        /// <returns></returns>
        public override bool Approve()
        {
            bool res = false;

            try
            {
                #region DEL 2017.07.25.14.25
                //SPFieldLookupValue currentStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CurrentStep]);
                //SPFieldLookupValue additionalPreviousStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalPrevisousStep]);

                //if (currentStep != null && additionalPreviousStep != null)
                //{
                //    // CurrentStep == AdditionalPrevisousStep
                //    if (currentStep.LookupId == additionalPreviousStep.LookupId)
                //    {
                //        SPFieldLookupValue additionalStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalStep]);
                //        if (additionalStep != null)
                //        {
                //            this.CurrentItem[ApprovalFields.IsAdditionalStep] = true;
                //            this.CurrentItem[ApprovalFields.NextStep] = additionalStep;
                //        }
                //        else
                //        {
                //            throw new Exception(StringMessages.TheAdditionalStepWasNotFound);
                //        }
                //    }
                //}
                #endregion

                #region M 2017.08.14. Fix bug workflow.

                #region DEL 2017.08.14
                //// Trưởng phòng thực hiện duyệt sau bước BOD
                //// SPFieldLookupValue currentStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CurrentStep]);
                //SPFieldLookupValue nextStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.NextStep]);
                //SPFieldLookupValue additionalPreviousStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalPreviousStep]);
                //SPFieldLookupValue additionalStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalStep]);
                //SPFieldLookupValue additionalDepartment = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalDepartment]);
                ////if ((string.Compare(currentStep.LookupValue, EmployeePosition_BOD_CODE, true) == 0) && (nextStep == null))
                //// Neu ket thuc qui trinh thi chuyen cho Truong Bo Phan xu ly buoc ke tiep. (Neu next step bang NULL va Khong co
                //if (nextStep == null && additionalPreviousStep == null && additionalStep == null && additionalDepartment == null)
                //{
                //    this.CurrentItem[ApprovalFields.IsAdditionalStep] = true;
                //    this.CurrentItem[ApprovalFields.AdditionalPreviousStep] = this.CurrentItem[ApprovalFields.CurrentStep];
                //    this.CurrentItem[ApprovalFields.AdditionalStep] = GetDEHPositionId(); // DEH
                //    this.CurrentItem[ApprovalFields.NextStep] = this.CurrentItem[ApprovalFields.AdditionalStep];
                //    this.CurrentItem[ApprovalFields.AdditionalDepartment] = this.CurrentItem[StringConstant.RequestsList.ReceviedByField];
                //}

                //res = base.Approve();
                #endregion

                #region A 2017.08.14

                res = base.Approve();

                // Trưởng phòng thực hiện duyệt sau bước BOD
                // SPFieldLookupValue currentStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CurrentStep]);
                // SPFieldLookupValue nextStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.NextStep]);
                SPFieldLookupValue additionalPreviousStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalPreviousStep]);
                SPFieldLookupValue additionalStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalStep]);
                SPFieldLookupValue additionalDepartment = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalDepartment]);
                //if ((string.Compare(currentStep.LookupValue, EmployeePosition_BOD_CODE, true) == 0) && (nextStep == null))
                // Neu ket thuc qui trinh thi chuyen cho Truong Bo Phan xu ly buoc ke tiep. (Neu next step bang NULL va Khong co
                //if (nextStep == null && additionalPreviousStep == null && additionalStep == null && additionalDepartment == null)
                if (((this.PendingAtEmployees == null) || (this.PendingAtEmployees != null && this.PendingAtEmployees.Count == 0)) &&
                    (additionalPreviousStep == null) && (additionalStep == null) && (additionalDepartment == null))
                {
                    int currentLocationId = 0;
                    int additionalApprovalDepartmentId = 0;
                    int additionalApprovalPositionId = GetDEHPositionId(); // DEH;
                    SPFieldLookupValue receivedByLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[StringConstant.RequestsList.ReceviedByField]);
                    SPFieldLookupValue locationLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CommonLocation]);
                    if (locationLookupValue != null)
                    {
                        currentLocationId = locationLookupValue.LookupId;
                    }
                    this.CurrentItem[ApprovalFields.IsAdditionalStep] = true;
                    this.CurrentItem[ApprovalFields.AdditionalPreviousStep] = this.CurrentItem[ApprovalFields.CurrentStep];
                    this.CurrentItem[ApprovalFields.AdditionalStep] = additionalApprovalPositionId; // DEH
                    //this.CurrentItem[ApprovalFields.NextStep] = this.CurrentItem[ApprovalFields.AdditionalStep];
                    if (receivedByLookupValue != null)
                    {
                        additionalApprovalDepartmentId = receivedByLookupValue.LookupId;
                        this.CurrentItem[ApprovalFields.AdditionalDepartment] = additionalApprovalDepartmentId;
                    }

                    List<EmployeeInfo> listOfEmployees = GetListOfEmployees(currentLocationId, additionalApprovalDepartmentId, additionalApprovalPositionId);
                    if (listOfEmployees != null && listOfEmployees.Count > 0)
                    {
                        this.PendingAtEmployees = listOfEmployees.ToSPFieldLookupValueCollection();
                        this.ListOfEmployeesEmailTo.AddRange(listOfEmployees);
                    }
                    // Set PendingAt
                    this.CurrentItem[ApprovalFields.PendingAt] = this.PendingAtEmployees;

                    // Set Status
                    this.CurrentItem[ApprovalFields.Status] = string.Format("{0} {1}", this.CurrentEmployee.EmployeePosition.LookupValue, Status.Approved);

                    // Set WFStauts
                    this.CurrentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.InProgress;
                    this.IsWorkflowCompleted = false;

                    this.CurrentItem.Update();
                }
                
                #endregion

                #endregion

                #region Duc.VoTan.ADD.2017.10.12. TFS#1597. After department head of received by approve, update status of request to requester continue final step.
                string status = ObjectHelper.GetString(this.CurrentItem[ApprovalFields.Status]);
                if (string.Compare(status, Status.Completed, true) == 0)
                {
                    // Add To Email Requester
                    this.ListOfEmployeesEmailTo.Add(this.Creator);
                    // Change status: Completed -> Approved
                    this.CurrentItem[ApprovalFields.Status] = Status.Approved;
                    // khi “Trưởng phòng thực hiện 
                    // (bên tiếp nhận)” duyệt đơn thì phiếu đề nghị đó sẽ có trạng thái là “Đang thực hiện” 
                    // để bên đề nghị hoặc BOD nhìn vào sẽ biết được là đơn này đã được tiếp nhận và
                    // đang trong quá trình thực hiện yêu cầu
                    this.CurrentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.InProcess;
                    // Update Request item
                    this.CurrentItem.Update();
                }
                #endregion
            }
            catch (Exception ex)
            {
                res = false;
                ULSLogging.LogError(ex);
            }

            return res;
        }

        public override bool Submit()
        {
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
                this.CurrentItem[ApprovalFields.Creator] = this.CurrentEmployee.ID;
                // Set CurrentStep
                CurrentItem[ApprovalFields.CurrentStep] = nextStepLookupItem.LookupId;
                #region Set NextStep
                //if (nextFirstWorkflowStepItem.NextStep != null)
                //{
                //    currentItem[ApprovalFields.NextStep] = nextFirstWorkflowStepItem.NextStep.LookupId;
                //}
                #endregion
                #region Set Approvers
                List<EmployeeInfo> employeeInfos = GetListOfEmployeesByPosition(nextStepLookupItem);
                SPFieldLookupValueCollection employees = ConvertToSPFieldLookupValueCollection(employeeInfos);
                CurrentItem[ApprovalFields.PendingAt] = employees;
                #endregion
                // Set Status
                CurrentItem[ApprovalFields.Status] = Status.Submitted;
                // Set WFStatus
                CurrentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.InProgress;
                // Set Location
                CurrentItem[ApprovalFields.CommonLocation] = this.CurrentLocation;
                // Set Department
                CurrentItem[ApprovalFields.CommonDepartment] = this.CurrentDepartment;
                //// Set additional step for current item
                //SetAdditionalStepForCurrentItem();
                // Save current item
                CurrentItem.Update();

                #region Prepareration for sending email

                // Add approvers into list of email To.
                ListOfEmployeesEmailTo.AddRange(employeeInfos);

                // Get list of delegated employee
                var delegatedEmployees = GetListOfDelegatedEmployees(employeeInfos);
                if (delegatedEmployees != null && delegatedEmployees.Count > 0)
                {
                    ListOfEmployeesEmailTo.AddRange(delegatedEmployees);
                }

                SetListOfEmployeesToSendEmailToAndCc(firstWorkflowStepItem);

                #endregion
            }
            else // Truong hop Truong Phong submit neu khong len BOD thi chuyen cho Truong Phong Thuc Hien
            {
                // Set Creator
                this.CurrentItem[ApprovalFields.Creator] = this.CurrentEmployee.ID;
                // Set Location
                CurrentItem[ApprovalFields.CommonLocation] = this.CurrentLocation;
                // Set Department
                CurrentItem[ApprovalFields.CommonDepartment] = this.CurrentDepartment;
                int currentLocationId = 0;
                int additionalApprovalDepartmentId = 0;
                int additionalApprovalPositionId = GetDEHPositionId(); // DEH;
                SPFieldLookupValue receivedByLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[StringConstant.RequestsList.ReceviedByField]);
                SPFieldLookupValue locationLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CommonLocation]);
                if (locationLookupValue != null)
                {
                    currentLocationId = locationLookupValue.LookupId;
                }
                this.CurrentItem[ApprovalFields.IsAdditionalStep] = true;
                this.CurrentItem[ApprovalFields.AdditionalPreviousStep] = this.CurrentItem[ApprovalFields.CurrentStep];
                this.CurrentItem[ApprovalFields.AdditionalStep] = additionalApprovalPositionId; // DEH
                if (receivedByLookupValue != null)
                {
                    additionalApprovalDepartmentId = receivedByLookupValue.LookupId;
                    this.CurrentItem[ApprovalFields.AdditionalDepartment] = additionalApprovalDepartmentId;
                }

                List<EmployeeInfo> listOfEmployees = GetListOfEmployees(currentLocationId, additionalApprovalDepartmentId, additionalApprovalPositionId);
                if (listOfEmployees != null && listOfEmployees.Count > 0)
                {
                    this.PendingAtEmployees = listOfEmployees.ToSPFieldLookupValueCollection();
                    this.ListOfEmployeesEmailTo.AddRange(listOfEmployees);
                }
                // Set PendingAt
                this.CurrentItem[ApprovalFields.PendingAt] = this.PendingAtEmployees;

                // Set Status
                this.CurrentItem[ApprovalFields.Status] = string.Format("{0} {1}", this.CurrentEmployee.EmployeePosition.LookupValue, Status.Approved);

                // Set WFStauts
                this.CurrentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.InProgress;
                this.IsWorkflowCompleted = false;

                this.CurrentItem.Update();
            }

            return true;
        }

        #endregion

    }
}
