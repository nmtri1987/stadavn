using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBVH.Stada.Intranet.ListEventReceiver.CustomListEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class CustomListEventReceiver : SPItemEventReceiver
    {
        #region Overrides

        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                base.ItemAdded(properties);

                // Update delegation of new task
                UpdateDelegationOfNewTask(properties);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
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

                // Update delegation of new task
                UpdateDelegationOfNewTask(properties);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When have new task, check assignee who has delegation or no. If have, insert into delegations list.
        /// </summary>
        /// <param name="properties">The SPItemEventProperties object.</param>
        private void UpdateDelegationOfNewTask(SPItemEventProperties properties)
        {
            string listUrl = properties.List.RootFolder.ServerRelativeUrl;
            int taskFromEmployeeId = 0;
            LookupItem currentEmployeeProcessing = DelegationManager.GetCurrentEmployeeProcessing(listUrl, properties.ListItem, properties.Web);
            if (currentEmployeeProcessing != null)
            {
                taskFromEmployeeId = currentEmployeeProcessing.LookupId;
            }

            if (taskFromEmployeeId > 0)
            {
                string webUrl = properties.WebUrl;
                var delegationOfNewTasks = DelegationPermissionManager.HasDelegationOfNewTasks(taskFromEmployeeId, listUrl, webUrl);
                if (delegationOfNewTasks != null)
                {
                    foreach (var delegationOfNewTask in delegationOfNewTasks)
                    {
                        try
                        {
                            var delegationListItem = DelegationManager.GetDelegationListItem(listUrl, properties.ListItem, properties.Web);
                            if (delegationListItem != null)
                            {
                                List<int> toEmployeeIds = delegationOfNewTask.ToEmployee.Select(e => e.LookupId).ToList();
                                bool isDelegationExisted = DelegationManager.IsDelegationExisted(taskFromEmployeeId, toEmployeeIds, listUrl, properties.ListItemId, delegationOfNewTask.FromDate, delegationOfNewTask.ToDate, properties.Web.Url);
                                if (isDelegationExisted == true)
                                {
                                    return;
                                }

                                delegationListItem.FromDate = delegationOfNewTask.FromDate;
                                delegationListItem.ToDate = delegationOfNewTask.ToDate;
                                delegationListItem.FromEmployee = delegationOfNewTask.FromEmployee;
                                delegationListItem.ToEmployee = delegationOfNewTask.ToEmployee;
                                StringBuilder toEmployeesBuilder = new StringBuilder();
                                foreach (var toEmployee in delegationListItem.ToEmployee)
                                {
                                    toEmployeesBuilder.AppendFormat("{0}; ", toEmployee.LookupValue);
                                }
                                delegationListItem.Title = string.Format("{0} - {1}", delegationListItem.FromEmployee.LookupValue, toEmployeesBuilder.ToString());
                                var delegationsDAL = new DelegationsDAL(webUrl);
                                int id = delegationsDAL.SaveItem(delegationListItem);
                                if (id <= 0)
                                {
                                    ULSLogging.LogError(new Exception("Updating for new delegation was unsuccessfully."));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ULSLogging.LogError(ex);
                        }
                    }
                }
            }
        }

        #endregion
    }
}