using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.Common
{
    /// <summary>
    /// ApprovalBaseUserControl class.
    /// </summary>
    public class ApprovalBaseUserControl : FormBaseUserControl
    {
        #region Attributes
        /// <summary>
        /// Form button user control.
        /// </summary>
        private FormButtonsControl formButtonsControlObject;

        /// <summary>
        /// commentBox
        /// </summary>
        private CommentControl commentBoxControlObject;

        /// <summary>
        /// workflowHistoryControlObject
        /// </summary>
        private WorkflowHistoryControl workflowHistoryControlObject;

        /// <summary>
        /// Status of current item.
        /// </summary>
        private string currentStatus;

        /// <summary>
        /// The ApprovalBaseManager object.
        /// </summary>
        private ApprovalBaseManager approvalBaseManagerObject;

        /// <summary>
        /// The form title.
        /// </summary>
        protected string formTitle;

        /// <summary>
        /// allowPostComment
        /// </summary>
        protected bool allowPostComment;

        protected EWorkflowHistoryStyle workflowHistoryStyle;

        #endregion

        #region Properties
        /// <summary>
        /// Form button user control.
        /// </summary>
        public FormButtonsControl FormButtonsControlObject
        {
            get
            {
                return formButtonsControlObject;
            }
        }

        /// <summary>
        /// Get comment control.
        /// </summary>
        public CommentControl CommentBoxControlObject
        {
            get
            {
                return this.commentBoxControlObject;
            }
        }

        /// <summary>
        /// Get workflow history control.
        /// </summary>
        public WorkflowHistoryControl WorkflowHistoryControlObject
        {
            get
            {
                return this.workflowHistoryControlObject;
            }
        }

        /// <summary>
        /// /Status of current item.
        /// </summary>
        public string CurrentStatus
        {
            get
            {
                return currentStatus;
            }
        }

        public ApprovalBaseManager ApprovalBaseManagerObject
        {
            get
            {
                return approvalBaseManagerObject;
            }
        }

        /// <summary>
        /// It's title of current item.
        /// </summary>
        public string FormTitle
        {
            get
            {
                return this.formTitle;
            }
        }

        /// <summary>
        /// If current language of page is Vietnamese, return true. Otherwise return false.
        /// </summary>
        public bool IsVietnameseLanguage
        {
            get
            {
                return this.Page.LCID == PageLanguages.Vietnamese ? true : false;
            }
        }

        /// <summary>
        /// If current language of page is English, return true. Otherwise return false.
        /// </summary>
        public bool IsEnglishLanguage
        {
            get
            {
                return this.Page.LCID == PageLanguages.English ? true : false;
            }
        }

        /// <summary>
        /// AllowPostComment
        /// </summary>
        public bool AllowPostComment
        {
            get
            {
                return this.allowPostComment;
            }
        }
        
        /// <summary>
        /// Get workflow history style.
        /// </summary>
        public EWorkflowHistoryStyle WorkflowHistoryStyle
        {
            get
            {
                return this.workflowHistoryStyle;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// Init status of current item is Draft.
        /// </summary>
        public ApprovalBaseUserControl() : base()
        {
            this.currentStatus = Status.Draft;
            this.allowPostComment = false;
            this.workflowHistoryStyle = EWorkflowHistoryStyle.Full;
        }
        #endregion

        #region EventHandler
        /// <summary>
        /// This event will be raised after SaveForm method.
        /// </summary>
        public event EventHandler OnAfterSaveAsDraft;

        /// <summary>
        /// This event will raised before approvalManager.Submit() is called.
        /// </summary>
        public event EventHandler OnBeforeSubmit;

        /// <summary>
        /// This event will raised after approvalManager.Submit() is called and isSubmitted is true.
        /// </summary>
        public event EventHandler OnAfterSubmitted;

        /// <summary>
        /// This event will raised before approvalManager.Reject() is called.
        /// </summary>
        public event EventHandler OnBeforeReject;

        /// <summary>
        /// This event will raised after approvalManager.Reject() is called and isRejected is true.
        /// </summary>
        public event EventHandler OnAfterRejected;

        /// <summary>
        /// This event will raised before approvalManager.Approve() is called.
        /// </summary>
        public event EventHandler OnBeforeApprove;

        /// <summary>
        /// This event will raised after approvalManager.Approve() is called and isApproved is true.
        /// </summary>
        public event EventHandler OnAfterApproved;

        /// <summary>
        /// This event will raised before approvalManager.ReAssign() is called.
        /// </summary>
        public event EventHandler OnBeforeReAssign;

        /// <summary>
        /// This event will raised after approvalManager.ReAssign() is called and isApproved is true.
        /// </summary>
        public event EventHandler OnAfterReAssign;

        /// <summary>
        /// This event will raised after approvalManager.Cancel() is called.
        /// </summary>
        public event EventHandler OnBeforeCancelWorkflow;

        /// <summary>
        /// This event will raised before approvalManager.Cancel() is called and isCanceled is true.
        /// </summary>
        public event EventHandler OnAfterCanceledWorkflow;

        /// <summary>
        /// This event will raised when user click on Close button.
        /// </summary>
        public event EventHandler OnCloseForm;
        #endregion

        #region Events
        /// <summary>
        /// btnSaveDraft_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                // Set status
                CurrentItem[ApprovalFields.Status] = Status.Draft;
                CurrentItem[ApprovalFields.Creator] = approvalBaseManagerObject.Creator.ID;

                if (SaveForm())
                {
                    if (OnAfterSaveAsDraft != null)
                    {
                        OnAfterSaveAsDraft(sender, e);
                    }

                    // Close Form.
                    CloseForm(sender);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        /// <summary>
        /// btnSaveAndSubmit_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// DucVT M 2015.08.17. Khi request completed, hệ thống gửi mail cho sales admin, cc cho những người tham gia trong quy trình (UNIT-VBL_Meeting Minute on 2015.08.14.doc)
        /// DucVT M 2015.07.13. Re-submit is the same submit.
        /// DucVT M 2015.06.16. Change from Page.IsValid to IsValid.
        /// </history>
        void btnSaveAndSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                #region IsValid
                if (IsValid)
                {
                    // Save form
                    if (SaveForm())
                    {
                        if (OnBeforeSubmit != null)
                        {
                            OnBeforeSubmit(sender, e);
                        }

                        // Do submission
                        approvalBaseManagerObject.Submit();

                        if (OnAfterSubmitted != null)
                        {
                            OnAfterSubmitted(sender, e);
                        }

                        // Send Email
                        approvalBaseManagerObject.SendEmail(EWorkflowAction.Submit, this.commentBoxControlObject.ContentComment);

                        // Post Comment
                        approvalBaseManagerObject.PostComment(Status.Submitted, this.commentBoxControlObject.ContentComment);

                        // Close Form.
                        CloseForm(sender);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        /// <summary>
        /// btnApprove_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                // Duc.VoTan modified on 21-March-2018. TFS #1466. Expired request need locking.
                DateTime expiredDate;
                if (!approvalBaseManagerObject.IsApprovalExpired(out expiredDate))
                {
                    #region IsValid
                    if (IsValid)
                    {
                        // Save form
                        if (SaveForm())
                        {
                            // 2011 https://rbvhsharepointprojects.visualstudio.com/STADA/_workitems/edit/2011
                            ValidateItemApproval();
                            if (!IsValid)
                                return;

                            if (OnBeforeApprove != null)
                            {
                                OnBeforeApprove(sender, e);
                            }

                            // Do approval
                            approvalBaseManagerObject.Approve();

                            if (OnAfterApproved != null)
                            {
                                OnAfterApproved(sender, e);
                            }

                            string status = ObjectHelper.GetString(this.CurrentItem[ApprovalFields.Status]);
                            // Neu kết thúc qui trình thì gửi email cho người đề nghị.
                            if (string.Compare(status, Status.Completed, true) == 0)
                            {
                                // Add To Email Requester
                                approvalBaseManagerObject.ListOfEmployeesEmailTo.Add(approvalBaseManagerObject.Creator);

                                // Send Email
                                approvalBaseManagerObject.SendEmail(EWorkflowAction.Complete, this.commentBoxControlObject.ContentComment);

                                // Post Comment
                                approvalBaseManagerObject.PostComment(Status.Completed, this.commentBoxControlObject.ContentComment);
                            }
                            else    // Nếu chưa kết thúc qui trình thì gửi email cho người phê duyệt kế tiếp
                            {
                                // Send Email
                                approvalBaseManagerObject.SendEmail(EWorkflowAction.Approve, this.commentBoxControlObject.ContentComment);

                                // Post Comment
                                approvalBaseManagerObject.PostComment(Status.Approved, this.commentBoxControlObject.ContentComment);
                            }

                            // Close Form.
                            CloseForm(sender);
                        }
                    }
                    #endregion
                }
                else
                {
                    ShowExpiredMessage(expiredDate);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        /// <summary>
        /// btnReAssign_Click
        /// </summary>
        void btnReAssign_Click(object sender, EventArgs e)
        {
            try
            {
                if (OnBeforeReAssign != null)
                {
                    OnBeforeReAssign(sender, e);
                }

                int assignedToEmployeeId = 0;
                int.TryParse(this.formButtonsControlObject.AssignedToEmpoyeeIdHiddenField.Value, out assignedToEmployeeId);
                if (assignedToEmployeeId > 0)
                {
                    // Do Re-Assign
                    approvalBaseManagerObject.ReAssign(assignedToEmployeeId);

                    if (OnAfterReAssign != null)
                    {
                        OnAfterReAssign(sender, e);
                    }

                    // TODO (Duc.VoTan): Send Email
                    approvalBaseManagerObject.SendEmail(EWorkflowAction.ReAssign, this.commentBoxControlObject.ContentComment);

                    // Post Comment
                    approvalBaseManagerObject.PostComment(Status.Assigned, this.commentBoxControlObject.ContentComment);

                    // Close Form.
                    CloseForm(sender);
                }
                else
                {

                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        /// <summary>
        /// btnReject_Click
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The EventArgs object.</param>
        /// <history>
        /// DucVT M 2015.07.15. Add allowRejection field to check validation that must comment.
        /// </history>
        void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                // Duc.VoTan modified on 21-March-2018. TFS #1466. Expired request need locking.
                DateTime expiredDate;
                if (!approvalBaseManagerObject.IsApprovalExpired(out expiredDate))
                {
                    if (!this.commentBoxControlObject.IsContentEmpty())
                    {
                        // Hide error message of comment.
                        this.commentBoxControlObject.HideErrorMessage();

                        // 2011 https://rbvhsharepointprojects.visualstudio.com/STADA/_workitems/edit/2011
                        ValidateItemApproval();
                        if (!IsValid)
                            return;

                        if (OnBeforeReject != null)
                        {
                            OnBeforeReject(sender, e);
                        }

                        approvalBaseManagerObject.Reject();

                        // Do Reject
                        if (OnAfterRejected != null)
                        {
                            OnAfterRejected(sender, e);
                        }

                        // Send Email
                        approvalBaseManagerObject.SendEmail(EWorkflowAction.Reject, this.commentBoxControlObject.ContentComment);

                        // Post Comment
                        approvalBaseManagerObject.PostComment(Status.Rejected, this.commentBoxControlObject.ContentComment);

                        // Close Form.
                        CloseForm(sender);
                    }
                    else
                    {
                        this.commentBoxControlObject.ShowErrorMessage();
                    }
                }
                else
                {
                    ShowExpiredMessage(expiredDate);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        /// <summary>
        /// btnCancelWorkflow_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCancelWorkflow_Click(object sender, EventArgs e)
        {
            try
            {
                if (OnBeforeCancelWorkflow != null)
                {
                    OnBeforeCancelWorkflow(sender, e);
                }

                // Do Terminate
                approvalBaseManagerObject.Terminate();

                if (OnAfterCanceledWorkflow != null)
                {
                    OnAfterCanceledWorkflow(sender, e);
                }

                // Send Email
                approvalBaseManagerObject.SendEmail(EWorkflowAction.Terminate, this.commentBoxControlObject.ContentComment);

                // Post Comment
                approvalBaseManagerObject.PostComment(Status.CancelWorkflow, this.commentBoxControlObject.ContentComment);

                // Close Form.
                CloseForm(sender);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Visible and Invisible controls on the form such as button (Save As Draft, Submit, ...)
        /// </summary>
        protected override void DisplayControls()
        {
            try
            {
                base.DisplayControls();

                DisplayButtonsControl();
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Updating list item.
        /// </summary>
        /// <returns></returns>
        protected override bool SaveForm()
        {
            var res = false;

            res = base.SaveForm();
            if (res)
            {
                string errorMessage = UploadSupportingDocuments();

                if(!string.IsNullOrEmpty(errorMessage))
                {
                    res = false;
                }
            }

            return res;
        }

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                Inits();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);

                // Load Workflow History
                LoadWorkflowHistory();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                throw ex;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// InitApprovalManagementObject
        /// </summary>
        private void InitApprovalManagementObject()
        {
            if (string.Compare(StringConstant.RequestsList.ListName, ListName, true) == 0)
            {
                approvalBaseManagerObject = new RequestApprovalManagement(SiteUrl, ListName, CurrentItem, CurrentWeb);
            }
            else if (string.Compare(StringConstant.EmployeeRequirementSheetsList.ListName, ListName, true) == 0)
            {
                approvalBaseManagerObject = new RecruitmentApprovalManagement(SiteUrl, ListName, CurrentItem, CurrentWeb);
            }
            else if(string.Compare(StringConstant.RequestForDiplomaSuppliesList.ListName, ListName, true) == 0)
            {
                approvalBaseManagerObject = new RequestForDiplomaSupplyApprovalManager(SiteUrl, ListName, CurrentItem, CurrentWeb);
            }
            else if (string.Compare(StringConstant.RequisitionOfMeetingRoomList.ListName, ListName, true) == 0)
            {
                approvalBaseManagerObject = new MeetingRoomApprovalManagement(SiteUrl, ListName, CurrentItem, CurrentWeb);
            }
            else
            {
                approvalBaseManagerObject = new ApprovalBaseManager(SiteUrl, ListName, CurrentItem, CurrentWeb);
            }
        }

        /// <summary>
        /// InitClickEventForFormButtons
        /// </summary>
        private void InitClickEventForFormButtons()
        {
            #region Form Button User Control
            var listOfFormButtonUserControls = this.GetAllControlsOfType<FormButtonsControl>().ToList();
            if (listOfFormButtonUserControls != null && listOfFormButtonUserControls.Count > 0)
            {
                formButtonsControlObject = listOfFormButtonUserControls[0];
            }

            if (formButtonsControlObject != null)
            {
                #region Save as draft Button
                if (formButtonsControlObject.SaveDraftButton != null)
                {
                    formButtonsControlObject.SaveDraftButton.Click += new EventHandler(btnSaveDraft_Click);
                }
                #endregion

                #region Save & Submit Button
                if (formButtonsControlObject.SaveAndSubmitButton != null)
                {
                    formButtonsControlObject.SaveAndSubmitButton.Click += new EventHandler(btnSaveAndSubmit_Click);
                }
                #endregion

                #region Reject Button
                if (formButtonsControlObject.RejectButton != null)
                {
                    formButtonsControlObject.RejectButton.Click += new EventHandler(btnReject_Click);
                }
                #endregion

                #region Approve Button
                if (formButtonsControlObject.ApproveButon != null)
                {
                    formButtonsControlObject.ApproveButon.Click += new EventHandler(btnApprove_Click);
                }
                #endregion

                #region Re-Assign Button
                if (formButtonsControlObject.ReAssignButon != null)
                {
                    formButtonsControlObject.ReAssignButon.Click += btnReAssign_Click;
                }
                #endregion

                #region Cancel Workflow Button
                if (formButtonsControlObject.CancelWorkflowButton != null)
                {
                    formButtonsControlObject.CancelWorkflowButton.Click += new EventHandler(btnCancelWorkflow_Click);
                }
                #endregion

                #region Cancel Button
                if (OnCloseForm != null)
                {
                    formButtonsControlObject.OnCloseForm += OnCloseForm;
                }
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// Init comment and workflow history controls.
        /// </summary>
        private void InitCommentAndWorkflowHistoryControls()
        {
            var listOfCommentControls = this.GetAllControlsOfType<CommentControl>().ToList();
            if (listOfCommentControls != null && listOfCommentControls.Count > 0)
            {
                this.commentBoxControlObject = listOfCommentControls[0];
            }

            var listOfWorkflowHistoryControls = this.GetAllControlsOfType<WorkflowHistoryControl>().ToList();
            if (listOfWorkflowHistoryControls != null && listOfWorkflowHistoryControls.Count > 0)
            {
                this.workflowHistoryControlObject = listOfWorkflowHistoryControls[0];
            }
        }

        /// <summary>
        /// InitStatus
        /// </summary>
        private void InitStatus()
        {
            #region Init Status
            if (CurrentItem != null)
            {
                if (CurrentItem[ApprovalFields.Status] != null)
                {
                    currentStatus = CurrentItem[ApprovalFields.Status].ToString();
                }
            }
            #endregion
        }

        /// <summary>
        /// InitFormTitle
        /// </summary>
        private void InitFormTitle()
        {
            this.formTitle = string.Empty;

            if (CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.New)
            {
                string modeResource =  ResourceHelper.GetLocalizedString("NewItem", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                this.formTitle = string.Format(" - {0}", modeResource);
            }
            else if (CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit || CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
            {
                if (!string.IsNullOrEmpty(this.CurrentItem.Title))
                {
                    this.formTitle = string.Format(" - {0}", CurrentItem.Title);
                }
            }
        }

        /// <summary>
        /// // Validate Approved/Rejected Or not
        /// </summary>
        private void ValidateItemApproval()
        {
            if (this.FormButtonsControlObject.TdApprove.Visible == true)
            {
                bool isEmployeeProcessing = this.ApprovalBaseManagerObject.IsEmployeeProcessing();
                bool isEmployeeReAssigned = this.ApprovalBaseManagerObject.IsEmployeeReAssigned();
                bool isEmployeeDelegated = this.ApprovalBaseManagerObject.IsEmployeeDelegated();
                if (this.FormButtonsControlObject.TdApprove.Visible && (!isEmployeeProcessing && !isEmployeeReAssigned && !isEmployeeDelegated))
                {
                    IsValid = false;
                    this.FormButtonsControlObject.TdApprove.Visible = this.FormButtonsControlObject.TdReject.Visible = false;
                    string approvedMessage = ResourceHelper.GetLocalizedString("ApprovedMessage", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    this.ShowClientMessage(approvedMessage);
                }
            }
        }

        /// <summary>
        /// Initialize some attribute of object.
        /// </summary>
        protected virtual void Inits()
        {
            // Init Status
            InitStatus();

            // Init Approval Management Object
            InitApprovalManagementObject();

            // Init form buttons
            InitClickEventForFormButtons();

            // Init comment and workflow history control
            InitCommentAndWorkflowHistoryControls();

            // Init form title
            InitFormTitle();
        }

        /// <summary>
        /// Check current status of item. If Draft, return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public bool IsDraftStatus()
        {
            return (string.Compare(currentStatus, Status.Draft, true) == 0);
        }

        /// <summary>
        /// Check current status of item. If Submitted, return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public bool IsSubmittedStatus()
        {
            return (string.Compare(currentStatus, Status.Submitted, true) == 0);
        }

        /// <summary>
        /// Check current status of item. If Rejected, return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public bool IsRejectedStatus()
        {
            return (string.Compare(currentStatus, Status.Rejected, true) == 0);
        }

        /// <summary>
        /// Check current status of item. If Approved, return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public bool IsApprovedStatus()
        {
            return (string.Compare(currentStatus, Status.Approved, true) == 0);
        }

        /// <summary>
        /// Check current status of item. If Completed, return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public bool IsCompletedStatus()
        {
            return (string.Compare(currentStatus, Status.Completed, true) == 0);
        }

        /// <summary>
        /// If current employ is creator for this item, return true. Otherwise return false.
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsCreator()
        {
            return this.approvalBaseManagerObject.IsEmployeeCreatorForThisItem();
        }

        /// <summary>
        /// IsEmployeeBOD
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeBOD()
        {
            return this.approvalBaseManagerObject.IsEmployeeBOD();
        }

        /// <summary>
        /// IsEmployeeDEH
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeDEH()
        {
            return this.approvalBaseManagerObject.IsEmployeeDEH();
        }

        public bool IsEmployeeGRL()
        {
            return this.approvalBaseManagerObject.IsEmployeeGRL();
        }

        /// <summary>
        /// Check status who can edi for this item.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEditable()
        {
            bool res = false;

            if (CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.New)
            {
                res = true;
            }
            else if (CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
            {
                if (this.IsCreator())
                {
                    if (this.IsDraftStatus() || this.IsRejectedStatus())
                    {
                        res = true;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// IsRejectionAllowableAtFinalStep
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRejectionAllowableAtAdditionalStep()
        {
            return true;
        }

        /// <summary>
        /// Check current status of item. If Cancel Workflow, return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public bool IsCancelWorkflowStatus()
        {
            return (string.Compare(currentStatus, Status.CancelWorkflow, true) == 0);
        }

        /// <summary>
        /// Cancelled status
        /// </summary>
        /// <returns></returns>
        public bool IsCancelledStatus()
        {
            return (string.Compare(currentStatus, Status.Cancelled, true) == 0);
        }

        /// <summary>
        /// Show cancelled status only
        /// </summary>
        /// <returns></returns>
        public string GetApprovalStatus()
        {
            if (!string.IsNullOrEmpty(CurrentStatus) && this.ApprovalBaseManagerObject.CurrentItem[ApprovalFields.WFStatus] != null && !IsDraftStatus())
            {
                var wfStatus = this.ApprovalBaseManagerObject.CurrentItem[ApprovalFields.WFStatus].ToString();
                var currentStatusKeyValuePair = ApprovalStatusMapping.FirstOrDefault(x => String.Equals(x.Key, wfStatus, StringComparison.OrdinalIgnoreCase));
                if (currentStatusKeyValuePair.Key != null)
                {
                    return Page.LCID == PageLanguages.Vietnamese ? currentStatusKeyValuePair.Value : currentStatusKeyValuePair.Key;
                }
                return Page.LCID == PageLanguages.Vietnamese ? ApprovalStatusMapping[ApprovalStatus.InProgress] : ApprovalStatus.InProgress;
            }

            return string.Empty;
        }

        /// <summary>
        /// Check to show and hide buttons to precess on the form.
        /// </summary>
        private void DisplayButtonsControl()
        {
            if (formButtonsControlObject != null)
            {
                if (CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.New)
                {
                    #region New Mode
                    formButtonsControlObject.TdSaveAsDraft.Visible = true;
                    formButtonsControlObject.TdSaveAndSubmit.Visible = true;
                    #endregion
                }
                else if (CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                {
                    #region Edit Mode
                    if (IsDraftStatus())
                    {
                        if (approvalBaseManagerObject.IsEmployeeCreatorForThisItem())
                        {
                            formButtonsControlObject.TdSaveAsDraft.Visible = true;
                            formButtonsControlObject.TdSaveAndSubmit.Visible = true;
                        }
                    }
                    else if (IsSubmittedStatus())
                    {
                        var hasDelegationApprovalPermission = DelegationPermissionManager.DoesCurrentEmployeeHasApprovalPermission();
                        if (hasDelegationApprovalPermission || this.approvalBaseManagerObject.Creator.ID != this.approvalBaseManagerObject.CurrentEmployee.ID)
                        {
                            bool isEmployeeProcessing = approvalBaseManagerObject.IsEmployeeProcessing();
                            bool isEmployeeReAssigned = approvalBaseManagerObject.IsEmployeeReAssigned();
                            bool isEmployeeDelegated = approvalBaseManagerObject.IsEmployeeDelegated();
                            if (isEmployeeProcessing || isEmployeeReAssigned || isEmployeeDelegated)
                            {
                                formButtonsControlObject.TdApprove.Visible = true;
                                // Duc.VoTan added on 21-March-2018. TFS #1466. Expired request need locking.
                                DateTime expiredDate;
                                var isApprovalExpired = approvalBaseManagerObject.IsApprovalExpired(out expiredDate);
                                if (isApprovalExpired)
                                {
                                    formButtonsControlObject.ApproveButon.Enabled = false;
                                }
                                formButtonsControlObject.TdReject.Visible = true;
                                // Duc.VoTan added on 21-March-2018. TFS #1466. Expired request need locking.
                                if (isApprovalExpired)
                                {
                                    formButtonsControlObject.RejectButton.Enabled = false;
                                }
                                if (isEmployeeProcessing)
                                {
                                    formButtonsControlObject.TdReAssign.Visible = true;
                                }
                            }
                        }
                        // Trạng thái: Submitted (WFStatus: In-Progress mà chưa có người approve) --> cho phép Cancel Workflow.
                        else
                        {
                            formButtonsControlObject.TdCancelWorkflow.Visible = true;
                        }
                    }
                    else if (IsRejectedStatus())
                    {
                        // Nếu là người tạo thì mới cho phép re-submit và Terminate
                        if (this.approvalBaseManagerObject.Creator.ID == this.approvalBaseManagerObject.CurrentEmployee.ID)
                        {
                            formButtonsControlObject.TdSaveAndSubmit.Visible = true;
                            formButtonsControlObject.TdCancelWorkflow.Visible = true;
                        }
                    }
                    else if (IsCompletedStatus())
                    {
                        // Do nothing
                    }
                    else if (IsCancelWorkflowStatus())
                    {
                        // Do nothing
                    }
                    else    // In-Progress status
                    {
                        bool isEmployeeProcessing = approvalBaseManagerObject.IsEmployeeProcessing();
                        bool isEmployeeReAssigned = approvalBaseManagerObject.IsEmployeeReAssigned();
                        bool isEmployeeDelegated = approvalBaseManagerObject.IsEmployeeDelegated();
                        if (isEmployeeProcessing || isEmployeeReAssigned || isEmployeeDelegated)
                        {
                            formButtonsControlObject.TdApprove.Visible = true;
                            // Duc.VoTan added on 21-March-2018. TFS #1466. Expired request need locking.
                            DateTime expiredDate;
                            var isApprovalExpired = approvalBaseManagerObject.IsApprovalExpired(out expiredDate);
                            if (isApprovalExpired)
                            {
                                formButtonsControlObject.ApproveButon.Enabled = false;
                            }
                            if (this.CurrentItem[ApprovalFields.CurrentStep] != null)
                            {
                                formButtonsControlObject.TdReject.Visible = true;
                                // Duc.VoTan added on 21-March-2018. TFS #1466. Expired request need locking.
                                if (isApprovalExpired)
                                {
                                    formButtonsControlObject.RejectButton.Enabled = false;
                                }
                            }
                            else
                            {
                                if (this.IsRejectionAllowableAtAdditionalStep())
                                {
                                    formButtonsControlObject.TdReject.Visible = true;
                                    // Duc.VoTan added on 21-March-2018. TFS #1466. Expired request need locking.
                                    if (isApprovalExpired)
                                    {
                                        formButtonsControlObject.RejectButton.Enabled = false;
                                    }
                                }
                            }
                            if (isEmployeeProcessing)
                            {
                                formButtonsControlObject.TdReAssign.Visible = true;
                            }
                        }
                    }
                    #endregion
                }
                else if (CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
                {
                    #region Display Mode
                    // Do nothing
                    #endregion
                }
            }
        }

        /// <summary>
        /// Load workflow history grid.
        /// </summary>
        private void LoadWorkflowHistory()
        {
            // Full Style
            if (this.workflowHistoryStyle == EWorkflowHistoryStyle.Full)
            {
                if (this.workflowHistoryControlObject != null)
                {
                    this.workflowHistoryControlObject.GridViewWorkflowHistory.Visible = true;
                    this.workflowHistoryControlObject.LoadWorkflowHistory(approvalBaseManagerObject.WorkflowHistoryDAL, this.ListName, this.ItemID);
                }
            }
            else    // Simple Style
            {
                if (this.commentBoxControlObject != null)
                {
                    this.commentBoxControlObject.WorkflowHistory.Visible = true;

                    List<Biz.Models.WorkflowHistory> workflowHistoryItems = this.workflowHistoryControlObject.GetWorkflowHistory(approvalBaseManagerObject.WorkflowHistoryDAL, this.ListName, this.ItemID);
                    if (workflowHistoryItems != null && workflowHistoryItems.Count > 0)
                    {
                        StringBuilder workflowHistoryItemsBuilder = new StringBuilder();

                        foreach (var workflowHistoryItem in workflowHistoryItems)
                        {
                            workflowHistoryItemsBuilder.AppendFormat("<b>{0}</b>: {1}<br/>", workflowHistoryItem.PostedBy, workflowHistoryItem.CommonComment);
                        }

                        this.commentBoxControlObject.WorkflowHistory.InnerHtml = workflowHistoryItemsBuilder.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// UploadSupportingDocuments
        /// </summary>
        /// <returns></returns>
        private string UploadSupportingDocuments()
        {
            StringBuilder errorBuilder = new StringBuilder();

            if (Page.Request.Files.Count > 0)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (var site = new SPSite(this.CurrentWeb.Url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;

                            for (int i =0; i< Page.Request.Files.Count; i++)
                            {
                                try
                                {
                                    HttpPostedFile filePosted = Page.Request.Files[i];
                                    if (filePosted.ContentLength > 0)
                                    {
                                        //string listName = SupportingDocumentsList.ListName;
                                        string folder = this.ListName;
                                        string fileName = string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(filePosted.FileName), System.DateTime.Now.ToString("yyyyMMddHHmmss"), System.IO.Path.GetExtension(filePosted.FileName));
                                        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePosted.FileName);
                                        string urlOfFile = string.Format("{0}{1}/{2}/{3}", web.Url, SupportingDocumentsList.Url, folder, fileName);
                                        Hashtable properties = new Hashtable();
                                        properties.Add(SupportingDocumentsList.Fields.ListItemID, this.CurrentItem.ID);
                                        properties.Add("Title", fileNameWithoutExtension);
                                        web.Files.Add(urlOfFile, filePosted.InputStream, properties, false);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorBuilder.Append(ex.Message);
                                    ULSLogging.LogError(ex);
                                }
                            }

                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }

            return errorBuilder.ToString();
        }

        /// <summary>
        /// ShowExpiredMessage
        /// </summary>
        /// <param name="expiredDate"></param>
        private void ShowExpiredMessage(DateTime expiredDate)
        {
            var requestExpiredMsgFormat = WebPageResourceHelper.GetResourceString("RequestExpiredMsgFormat");
            requestExpiredMsgFormat = HttpUtility.UrlDecode(requestExpiredMsgFormat);
            var expiredDateString = expiredDate.ToString(StringConstant.DateFormatddMMyyyy2);
            var message = string.Format(requestExpiredMsgFormat, expiredDateString);
            this.ShowClientMessage(message);
        }
        #endregion
    }

    /// <summary>
    /// EWorkflowHistoryStyle
    /// </summary>
    public enum EWorkflowHistoryStyle
    {
        Simple,
        Full
    }
}
