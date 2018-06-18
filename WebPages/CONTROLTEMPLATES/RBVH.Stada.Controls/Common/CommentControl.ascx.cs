using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common
{
    /// <summary>
    /// CommentControl class.
    /// </summary>
    public partial class CommentControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Get content comment.
        /// </summary>
        public string ContentComment
        {
            get
            {
                return this.txtComment.Text;
            }
        }

        public TextBox TextboxControl
        {
            get
            {
                return this.txtComment;
            }
        }

        public HtmlGenericControl WorkflowHistory
        {
            get
            {
                return this.workflowHistory;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// If content comment is empty, return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public bool IsContentEmpty()
        {
            string comment = this.txtComment.Text.Trim();
            return string.IsNullOrEmpty(comment) || string.IsNullOrWhiteSpace(comment);
        }

        /// <summary>
        /// Show error message.
        /// </summary>
        public void ShowErrorMessage()
        {
            this.lblErrorMessage.Visible = true;
        }

        /// <summary>
        /// Hide error message.
        /// </summary>
        public void HideErrorMessage()
        {
            this.lblErrorMessage.Visible = false;
        }
        #endregion

        /// <summary>
        /// OnPreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);

                var approvalBaseUserControl = this.Parent as ApprovalBaseUserControl;
                if (approvalBaseUserControl != null)
                {
                    if (approvalBaseUserControl.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
                    {
                        this.txtComment.Enabled = false;
                        if (approvalBaseUserControl.WorkflowHistoryStyle == EWorkflowHistoryStyle.Simple)
                        {
                            this.txtComment.Visible = false;
                        }
                    }

                    if (approvalBaseUserControl.AllowPostComment)
                    {
                        this.txtComment.Enabled = true;
                        this.btnPostComment.Visible = true;
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        /// <summary>
        /// BtnPostComment_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnPostComment_Click(object sender, EventArgs e)
        {
            try
            {
                var approvalBaseUserControl = this.Parent as ApprovalBaseUserControl;
                if (approvalBaseUserControl != null)
                {
                    if (approvalBaseUserControl.ApprovalBaseManagerObject != null)
                    {
                        if (approvalBaseUserControl.ApprovalBaseManagerObject.PostComment(Status.Comment, txtComment.Text))
                        {
                            approvalBaseUserControl.FormButtonsControlObject.CloseForm();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
    }
}
