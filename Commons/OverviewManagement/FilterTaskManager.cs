using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.OverviewManagement
{
    public class FilterTaskManager
    {
        private TaskManagementDAL _taskManagementDAL;
        private WorkflowHistoryDAL _workFlowHistory;

        #region "Constructors"
        public FilterTaskManager() { }
        public FilterTaskManager(string siteUrl)
        {
            _taskManagementDAL = new TaskManagementDAL(siteUrl);
            _workFlowHistory = new WorkflowHistoryDAL(siteUrl);
        }
        #endregion
        public string BuildApprovedByDelegationQuery(int currentUserADId, string approvedByFieldName, string approvedByFieldType, IEnumerable<int> delegationItemIDs)
        {
            var delegationQuery = "";
            var approvedByQuery = "";
            if (!string.IsNullOrEmpty(approvedByFieldName))
            {
                approvedByQuery = $@"<Eq>
		                                <FieldRef Name='{approvedByFieldName}' LookupId='TRUE' />
		                                <Value Type='{approvedByFieldType}'>{currentUserADId}</Value>
	                                </Eq>";
            }

            if (delegationItemIDs.Any())
            {
                StringBuilder sbItem = new StringBuilder();
                foreach (var itemId in delegationItemIDs)
                {
                    sbItem.Append($@"<Value Type='Counter'>{itemId}</Value>");
                }
                if (!string.IsNullOrEmpty(approvedByQuery))
                {
                    delegationQuery = $@"
                                        <Or>
                                            {approvedByQuery}
                                            <In>
                                                <FieldRef Name='ID' />
                                                <Values>
                                                    {sbItem.ToString()}
                                                </Values>                        
                                            </In>
                                        </Or>";
                }
                else
                {
                    delegationQuery = $@"
                                        <In>
                                            <FieldRef Name='ID' />
                                            <Values>
                                                {sbItem.ToString()}
                                            </Values>                        
                                        </In>";
                }
            }
            else
            {
                delegationQuery = approvedByQuery;
            }

            return string.IsNullOrEmpty(delegationQuery) ? $@"<Eq>
                            <FieldRef Name='ID' />
                            <Value Type='Counter'>0</Value>
                        </Eq>" : delegationQuery;
        }

        public string BuildTaskListQuery(string listName, string status, int userAdId)
        {
            var query = $@"<Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='Status' />
                                        <Value Type='Choice'>{status}</Value>
                                    </Eq>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{listName}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                            <Value Type='User'>{userAdId}</Value>
                                        </Eq>
                                    </And>
                                </And>
                            </Where>";

            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(query);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                IEnumerable<int> itemIds = taskManagementCollection.Where(t => t.ItemId > 0).Select(t => t.ItemId).AsEnumerable();

                if (itemIds != null && itemIds.Any())
                {
                    StringBuilder sbItem = new StringBuilder();
                    foreach (var itemId in itemIds)
                    {
                        sbItem.Append($@"<Value Type='Counter'>{itemId}</Value>");
                    }

                    return $@"
                                <In>
                                    <FieldRef Name='ID' />
                                    <Values>
                                        {sbItem.ToString()}
                                    </Values>                        
                                </In>
                            ";
                }
            }
            // Default condition - EMPTY:
            return $@"<Eq>
                        <FieldRef Name='ID' />
                        <Value Type='Counter'>0</Value>
                    </Eq>";
        }

        public string BuildApprovalWorkflowHistoryQuery(string listName, string approverName, DateTime date)
        {
            var query = $@"<Where>
                                <And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{WorkflowHistoryList.Fields.CommonDate}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{WorkflowHistoryList.Fields.PostedBy}' />
                                            <Value Type='Text'>{approverName}</Value>
                                        </Eq>
                                    </And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{WorkflowHistoryList.Fields.ListName}' />
                                            <Value Type='Text'>{listName}</Value>
                                        </Eq>
                                        <Or>
                                            <Eq>
                                                <FieldRef Name='{WorkflowHistoryList.Fields.Status}' />
                                                <Value Type='Text'>{Status.Approved}</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='{WorkflowHistoryList.Fields.Status}' />
                                                <Value Type='Text'>{Status.Rejected}</Value>
                                            </Eq>
                                        </Or>
                                    </And>
                                </And>
                            </Where>";

            var approvalList = _workFlowHistory.GetByQuery(query);
            if (approvalList != null && approvalList.Any())
            {
                IEnumerable<int> requestItemIds = approvalList.Select(x => x.CommonItemID).Distinct();
                StringBuilder sbItem = new StringBuilder();
                foreach (var itemId in requestItemIds)
                {
                    sbItem.Append($@"<Value Type='Counter'>{itemId}</Value>");
                }

                return $@"<Where>
                                <In>
                                    <FieldRef Name='ID' />
                                    <Values>
                                        {sbItem.ToString()}
                                    </Values>                        
                                </In>
                            </Where>";
            }
            // Default condition - EMPTY:
            return $@"<Where>
                        <Eq>
                            <FieldRef Name='ID' />
                            <Value Type='Counter'>0</Value>
                        </Eq>
                    </Where>";

        }

        public string BuildApprovalTaskListQuery(string listName, int userAdId, DateTime date)
        {
            var query = $@"<Where>
                                <And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{DefaultSPListField.ModifiedField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                        <Or>
                                            <Eq>
                                                <FieldRef Name='{TaskManagementList.TaskOutcome}' />
                                                <Value Type='OutcomeChoice'>{TaskOutcome.Approved}</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='{TaskManagementList.TaskOutcome}' />
                                                <Value Type='OutcomeChoice'>{TaskOutcome.Rejected}</Value>
                                            </Eq>
                                        </Or>
                                    </And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{listName}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                            <Value Type='User'>{userAdId}</Value>
                                        </Eq>
                                    </And>
                                </And>
                            </Where>";

            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(query);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                IEnumerable<int> itemIds = taskManagementCollection.Where(t => t.ItemId > 0).Select(t => t.ItemId).AsEnumerable();

                if (itemIds != null && itemIds.Any())
                {
                    StringBuilder sbItem = new StringBuilder();
                    foreach (var itemId in itemIds)
                    {
                        sbItem.Append($@"<Value Type='Counter'>{itemId}</Value>");
                    }

                    return $@"
                            <Where>
                                <In>
                                    <FieldRef Name='ID' />
                                    <Values>
                                        {sbItem.ToString()}
                                    </Values>                        
                                </In>
                            </Where>";
                }
            }
            // Default condition - EMPTY:
            return $@"<Where>
                        <Eq>
                            <FieldRef Name='ID' />
                            <Value Type='Counter'>0</Value>
                        </Eq>
                    </Where>";
        }
    }
}
