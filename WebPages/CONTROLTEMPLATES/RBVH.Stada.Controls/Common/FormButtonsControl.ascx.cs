using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common
{
    /// <summary>
    /// FormButtonsControl class.
    /// </summary>
    public partial class FormButtonsControl : UserControl
    {
        #region Constatns
        /// <summary>
        /// dialogResult
        /// </summary>
        public const string DialogResultParaName = "dialogResult";

        /// <summary>
        /// returnVal
        /// </summary>
        public const string ReturnValParamName = "returnVal";

        /// <summary>
        /// /_layouts/15/RBVH.Stada.Intranet.WebPages/ReAssignTask.aspx
        /// </summary>
        public const string ReAssignTaskPageUrl = "/_layouts/15/RBVH.Stada.Intranet.WebPages/ReAssignTask.aspx";
        #endregion

        #region Event Handler
        public event EventHandler OnCloseForm;
        #endregion

        #region Properties

        #region HtmlTableCells

        public HtmlTableCell TdSaveAsDraft
        {
            get
            {
                return this.tdSaveAsDraft;
            }
        }

        public HtmlTableCell TdSaveAndSubmit
        {
            get
            {
                return this.tdSaveAndSubmit;
            }
        }

        public HtmlTableCell TdPrint
        {
            get
            {
                return this.tdPrint;
            }
        }

        public HtmlTableCell TdReject
        {
            get
            {
                return this.tdReject;
            }
        }

        public HtmlTableCell TdApprove
        {
            get
            {
                return this.tdApprove;
            }
        }

        public HtmlTableCell TdReAssign
        {
            get
            {
                return this.tdReAssign;
            }
        }

        public HtmlTableCell TdCancelWorkflow
        {
            get
            {
                return this.tdCancelWorkflow;
            }
        }
        
        public HtmlTableCell TdCompleteWorkflow
        {
            get
            {
                return this.tdCompleteWorkflow;
            }
        }
        #endregion

        #region Buttons
        /// <summary>
        /// Get [Save as draft] button.
        /// </summary>
        public Button SaveDraftButton
        {
            get
            {
                return this.btnSaveDraft;
            }
        }

        /// <summary>
        /// Get [Save & Submit] button.
        /// </summary>
        public Button SaveAndSubmitButton
        {
            get
            {
                return this.btnSaveAndSubmit;
            }
        }

        public Button PrintButton
        {

            get
            {
                return this.btnPrint;
            }
        }

        /// <summary>
        /// Get [Reject] button.
        /// </summary>
        public Button RejectButton
        {
            get
            {
                return this.btnReject;
            }
        }

        /// <summary>
        /// Get [Approve] button.
        /// </summary>
        public Button ApproveButon
        {
            get
            {
                return this.btnApprove;
            }
        }

        /// <summary>
        ///  Get [ReAssign] button.
        /// </summary>
        public Button ReAssignButon
        {
            get
            {
                return this.btnReAssign;
            }
        }

        /// <summary>
        /// Get [Complete Workflow] button.
        /// </summary>
        public Button CompleteButton
        {
            get
            {
                return this.btnCompleteWorkflow;
            }
        }

        /// <summary>
        /// Get [Cancel workflow] button.
        /// </summary>
        public Button CancelWorkflowButton
        {
            get
            {
                return this.btnCancelWorkflow;
            }
        }
        
        public HiddenField AssignedToEmpoyeeIdHiddenField
        {
            get
            {
                return this.hdAssignedToEmployeeId;
            }
        }
        #endregion

        #endregion

        #region Events
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (OnCloseForm != null)
                {
                    OnCloseForm(sender, e);
                }

                CloseForm();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
        #endregion

        #region override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.Compare(Request.QueryString["IsDlg"], "1") == 0)
            {
                btnCancel.Visible = false;
                btnClose.Visible = true;
            }
            else
            {
                btnCancel.Visible = true;
                btnClose.Visible = false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Tạm thời không hiển thị button [Save As Draft] cho thống nhất với các module khác.
            TdSaveAsDraft.Visible = false;
            // Ẩn button re-assign. Tạm thời không sử dụng
            TdReAssign.Visible = false;
            // Tạm thời luôn luôn ẩn button [Cancel Workflow] vì đưa cancel nằm ngoài grid
            TdCancelWorkflow.Visible = false;
        }
        #endregion

        #region Methods
        public void CloseForm()
        {
            string returnlUrl = string.Empty;
            StringBuilder returnUrlBuilder = new StringBuilder();
            UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url);
            var sourceValues = uriBuilder.GetQueryValues(UrlParamName.ReturnUrlParamName);
            if (sourceValues != null && sourceValues.Count > 0)
            {
                var sourceValueArray = sourceValues.ElementAt(0).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (sourceValueArray != null && sourceValueArray.Length > 0)
                {
                    if (sourceValueArray.Length == 1)
                    {
                        returnUrlBuilder.Append(sourceValueArray[0]);
                    }
                    else
                    {
                        for (int i = 0; i < sourceValueArray.Length; i++)
                        {
                            if (i == 0)
                            {
                                returnUrlBuilder.AppendFormat("{0}?", (sourceValueArray[i]));
                            }
                            else if (i == sourceValueArray.Length - 1)
                            {
                                returnUrlBuilder.AppendFormat("{0}", (sourceValueArray[i]));
                            }
                            else
                            {
                                returnUrlBuilder.AppendFormat("{0}&", (sourceValueArray[i]));
                            }
                        }
                    }
                }
            }
            returnlUrl = returnUrlBuilder.ToString();
            if (!string.IsNullOrEmpty(returnlUrl))
            {
                SPUtility.Redirect(returnlUrl, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
            }
            else
            {

                SPUtility.Redirect(SPContext.Current.ListItem.ParentList.DefaultViewUrl, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
            }
        }
        #endregion
    }
}
