using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common
{
    /// <summary>
    /// WorkflowHistoryControl class.
    /// </summary>
    public partial class WorkflowHistoryControl : UserControl
    {
        #region Properties
        /// <summary>
        /// Get workflow history grid view control.
        /// </summary>
        public GridView GridViewWorkflowHistory
        {
            get
            {
                return this.gridViewWorkflowHistory;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// LoadWorkflowHistory
        /// </summary>
        /// <param name="workflowHistoryDAL">The WorkflowHistoryDAL object.</param>
        /// <param name="listName">The list name.</param>
        /// <param name="itemID">The current item id.</param>
        public void LoadWorkflowHistory(WorkflowHistoryDAL workflowHistoryDAL, string listName, int itemID)
        {
            try
            {
                var approvalBaseUserControl = this.Parent as ApprovalBaseUserControl;
                if (approvalBaseUserControl != null)
                {
                    if (approvalBaseUserControl.IsVietnameseLanguage)
                    {
                        this.gridViewWorkflowHistory.Columns[1].Visible = true;
                    }
                    else
                    {
                        this.gridViewWorkflowHistory.Columns[0].Visible = true;
                    }
                }

                List<Biz.Models.WorkflowHistory> workflowHistoryItems = GetWorkflowHistory(workflowHistoryDAL, listName, itemID);
                gridViewWorkflowHistory.DataSource = workflowHistoryItems;
                gridViewWorkflowHistory.DataBind();
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        public List<Biz.Models.WorkflowHistory> GetWorkflowHistory(WorkflowHistoryDAL workflowHistoryDAL, string listName, int itemID)
        {
            List<Biz.Models.WorkflowHistory> workflowHistoryItems = null;

            try
            {
                string queryString = string.Format(@"<Where>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{0}' />
                                                                <Value Type='Text'>{1}</Value>
                                                             </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{2}' />
                                                                <Value Type='Number'>{3}</Value>
                                                             </Eq>
                                                        </And>
                                                   </Where>", WorkflowHistoryList.Fields.ListName, listName,
                                   WorkflowHistoryList.Fields.CommonItemID, itemID);
                workflowHistoryItems = workflowHistoryDAL.GetByQuery(queryString);
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return workflowHistoryItems;
        }

        #endregion
    }
}
