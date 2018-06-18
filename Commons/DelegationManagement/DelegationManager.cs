using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DelegationManagement
{
    /// <summary>
    /// DelegationManager
    /// </summary>
    public class DelegationManager
    {
        /// <summary>
        /// DelegationManager
        /// </summary>
        private DelegationManager()
        {
        }

        /// <summary>
        /// Get list of tasks from Employee and Module.
        /// </summary>
        /// <param name="fromEmployee">The employee who need to get tasks of them.</param>
        /// <param name="listUrl">The list of URL corresponding to module. If listUrl is empty, get all modules. Otherwise get for specific module.</param>
        /// <returns></returns>
        public static List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee, string listUrl)
        {
            List<Delegation> listOfTasks = new List<Delegation>();

            if (fromEmployee == null)
            {
                throw new System.ArgumentNullException("fromEmployee");
            }
            if (string.IsNullOrEmpty(listUrl))
            {
                throw new System.ArgumentNullException("listUrl");
            }

            IDelegationManager moduleDAL = InitDALObject(listUrl);
            if (moduleDAL != null)
            {
                listOfTasks = moduleDAL.GetListOfTasks(fromEmployee);
            }

            return listOfTasks;
        }

        /// <summary>
        /// GetDelegationListItem
        /// </summary>
        /// <param name="listUrl"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        public static Delegation GetDelegationListItem(string listUrl, SPListItem listItem, SPWeb currentWeb)
        {
            Delegation delegationListItem = null;

            if (string.IsNullOrEmpty(listUrl))
            {
                throw new System.ArgumentNullException("listUrl");
            }
            if (listItem == null)
            {
                throw new ArgumentNullException("listItem");
            }

            IDelegationManager moduleDAL = InitDALObject(listUrl, currentWeb);
            if (moduleDAL != null)
            {
                delegationListItem = moduleDAL.GetDelegationListItem(listItem, currentWeb);
            }

            return delegationListItem;
        }

        public static LookupItem GetCurrentEmployeeProcessing(string listUrl, SPListItem listItem, SPWeb currentWeb)
        {
            LookupItem currentEmployeeProcessing = null;

            if (listItem == null)
            {
                throw new ArgumentNullException("listItem");
            }

            IDelegationManager moduleDAL = InitDALObject(listUrl, currentWeb);
            if (moduleDAL != null)
            {
                currentEmployeeProcessing = moduleDAL.GetCurrentEmployeeProcessing(listItem);
            }

            return currentEmployeeProcessing;
        }

        /// <summary>
        /// Initialize object for each module.
        /// </summary>
        /// <param name="listUrl">The list url of module.</param>
        /// <returns></returns>
        private static IDelegationManager InitDALObject(string listUrl)
        {
            IDelegationManager moduleDAL = null;

            moduleDAL = InitDALObject(listUrl, SPContext.Current.Web);

            return moduleDAL;
        }

        /// <summary>
        /// InitDALObject
        /// </summary>
        /// <param name="listUrl"></param>
        /// <param name="currentWeb"></param>
        /// <returns></returns>
        private static IDelegationManager InitDALObject(string listUrl, SPWeb currentWeb)
        {
            IDelegationManager moduleDAL = null;

            if (string.Compare(listUrl, ShiftManagementList.ListUrl, true) == 0)
            {
                moduleDAL = new ShiftManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, ChangeShiftList.ListUrl, true) == 0)
            {
                moduleDAL = new ChangeShiftManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, OverTimeManagementList.ListUrl, true) == 0)
            {
                moduleDAL = new OverTimeManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, NotOvertimeList.ListUrl, true) == 0)
            {
                moduleDAL = new NotOvertimeManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, VehicleManagementList.ListUrl, true) == 0)
            {
                moduleDAL = new VehicleManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, LeaveManagementList.ListUrl, true) == 0)
            {
                moduleDAL = new LeaveManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, FreightManagementList.ListUrl, true) == 0)
            {
                moduleDAL = new FreightManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, BusinessTripManagementList.Url, true) == 0)
            {
                moduleDAL = new BusinessTripManagementDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, RequestsList.Url, true) == 0)
            {
                moduleDAL = new RequestsDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, EmployeeRequirementSheetsList.Url, true) == 0)
            {
                moduleDAL = new EmployeeRequirementSheetDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, RequestForDiplomaSuppliesList.Url, true) == 0)
            {
                moduleDAL = new RequestForDiplomaSupplyDAL(currentWeb.Url);
            }
            else if (string.Compare(listUrl, RequisitionOfMeetingRoomList.Url, true) == 0)
            {
                moduleDAL = new RequisitionOfMeetingRoomDAL(currentWeb.Url);
            }

            return moduleDAL;
        }

        /// <summary>
        /// IsValidTask
        /// </summary>
        /// <param name="listUrl"></param>
        /// <param name="listItemID"></param>
        /// <param name="currentWeb"></param>
        /// <returns></returns>
        public static bool IsValidTask(string listUrl, int listItemID, SPWeb currentWeb)
        {
            var res = true;

            IDelegationManager moduleDAL = null;

            moduleDAL = InitDALObject(listUrl, currentWeb);

            if (moduleDAL != null)
            {
                res = moduleDAL.IsValidTask(listItemID);
            }

            return res;
        }

        /// <summary>
        /// IsValidTask
        /// </summary>
        /// <param name="listUrl"></param>
        /// <param name="listItemID"></param>
        /// <returns></returns>
        public static bool IsValidTask(string listUrl, int listItemID)
        {
            var res = true;

            SPWeb currentWeb = null;
            if (SPContext.Current != null)
            {
                currentWeb = SPContext.Current.Web;
            }
            if (currentWeb != null)
            {
                res = IsValidTask(listUrl, listItemID, currentWeb);
            }

            return res;
        }

        /// <summary>
        /// Build url for list item approval.
        /// </summary>
        /// <param name="listUrl">The list url.</param>
        /// <param name="listItemID">The list item ID.</param>
        /// <returns>The string list item approval url.</returns>
        public static string BuildListItemApprovalUrl(string listUrl, int listItemID)
        {
            SPWeb currentWeb = SPContext.Current.Web;
            return BuildListItemApprovalUrl(currentWeb, listUrl, listItemID);
        }

        /// <summary>
        ///  Build url for list item approval.
        /// </summary>
        /// <param name="currentWeb">The current SPWeb object.</param>
        /// <param name="listUrl">The list url.</param>
        /// <param name="listItemID">The list item ID.</param>
        /// <returns>The string list item approval url.</returns>
        public static string BuildListItemApprovalUrl(SPWeb currentWeb, string listUrl, int listItemID)
        {
            var res = "javascript:void(0);";

            try
            {
                SPList spList = currentWeb.GetList(string.Format("{0}{1}", currentWeb.Url, listUrl));
                string listId = spList.ID.ToString();
                res = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}",
                           currentWeb.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(), listId, listItemID);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;
        }

        /// <summary>
        /// Build url for list item approval (Old Modules).
        /// </summary>
        /// <param name="listUrl">The list url.</param>
        /// <param name="listItemID">The list item ID.</param>
        /// <returns>The string list item approval url.</returns>
        public static string BuildListItemApprovalUrl2(string listUrl, int listItemID)
        {
            var res = "javascript:void(0);";

            try
            {
                if (string.Compare(listUrl, ShiftManagementList.ListUrl, true) == 0)
                {
                    res = string.Format("{0}{1}", "/SitePages/ShiftApproval.aspx?subSection=ShiftManagement&itemId=", listItemID);
                }
                else if (string.Compare(listUrl, ChangeShiftList.ListUrl, true) == 0)
                {
                    res = string.Format("{0}{1}", "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftApprovalDelegation.aspx?itemId=", listItemID);
                }
                else if (string.Compare(listUrl, OverTimeManagementList.ListUrl, true) == 0)
                {
                    res = string.Format("{0}{1}", "/SitePages/OverTimeApproval.aspx?itemId=", listItemID);
                }
                else if (string.Compare(listUrl, NotOvertimeList.ListUrl, true) == 0)
                {
                    res = string.Format("{0}{1}", "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceApprovalDelegation.aspx?itemId=", listItemID);
                }
                else if (string.Compare(listUrl, VehicleManagementList.ListUrl, true) == 0)
                {
                    res = string.Format("{0}{1}", "/Lists/VehicleManagement/EditForm.aspx?subSection=TransportationManagement&ID=", listItemID);
                }
                else if (string.Compare(listUrl, LeaveManagementList.ListUrl, true) == 0)
                {
                    res = string.Format("{0}{1}", "/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId=", listItemID);
                }
                else if (string.Compare(listUrl, FreightManagementList.ListUrl, true) == 0)
                {
                    res = string.Format("{0}{1}", "/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId=", listItemID);
                }
                else if (string.Compare(listUrl, BusinessTripManagementList.Url, true) == 0)
                {
                    res = string.Format("{0}{1}", "/SitePages/BusinessTripRequest.aspx?subSection=BusinessTripManagement&itemId=", listItemID);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;
        }

        public static bool IsDelegationExisted(int fromEmployeeId, List<int> toEmployeeIds, string listUrl, int itemId, DateTime fromDate, DateTime toDate, string siteUrl)
        {
            bool ret = false;

            DelegationsDAL delegationsDALObject = new DelegationsDAL(siteUrl);
            if (delegationsDALObject != null)
            {
                string queryString = $@"<Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{fromEmployeeId}</Value>
                                            </Eq>
                                            <And>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsList.Fields.ListUrl}' />
                                                        <Value Type='Text'>{listUrl}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsList.Fields.ListItemID}' />
                                                        <Value Type='Number'>{itemId}</Value>
                                                    </Eq>
                                                </And>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsList.Fields.FromDate}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsList.Fields.ToDate}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                </And>
                                            </And>
                                        </And>
                                    </Where>";

                toEmployeeIds.Sort();
                string toEmployeeIdsStr = string.Join("#", toEmployeeIds);
                toEmployeeIdsStr = !string.IsNullOrEmpty(toEmployeeIdsStr) ? string.Format("#{0}#", toEmployeeIdsStr) : toEmployeeIdsStr;

                SPQuery query = new SPQuery { Query = queryString };
                var delegations = delegationsDALObject.GetByQuery(query);
                if (delegations != null && delegations.Count > 0)
                {
                    foreach (var delegation in delegations)
                    {
                        List<int> delegatedEmployeeIds = delegation.ToEmployee.Select(e => e.LookupId).ToList();
                        delegatedEmployeeIds.Sort();
                        string delegatedEmployeeIdsStr = string.Join("#", delegatedEmployeeIds);
                        delegatedEmployeeIdsStr = !string.IsNullOrEmpty(delegatedEmployeeIdsStr) ? string.Format("#{0}#", delegatedEmployeeIdsStr) : delegatedEmployeeIdsStr;
                        if (delegatedEmployeeIdsStr.IndexOf(toEmployeeIdsStr) >= 0)
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        public static bool IsDelegationExisted(Delegation delegation, string siteUrl)
        {
            var result = false;

            List<int> toEmployeeIds = new List<int>();
            foreach (var toEmployee in delegation.ToEmployee)
            {
                toEmployeeIds.Add(toEmployee.LookupId);
            }
            result = IsDelegationExisted(delegation.FromEmployee.LookupId, toEmployeeIds, delegation.ListUrl, delegation.ListItemID, delegation.FromDate, delegation.ToDate, siteUrl);

            return result;
        }

        public static bool IsDelegationOfNewTaskExisted(int fromEmployeeId, List<int> toEmployeeIds, string listUrl, DateTime fromDate, DateTime toDate, string siteUrl)
        {
            bool ret = false;

            DelegationsOfNewTaskDAL delegationsOfNewTaskDALObject = new DelegationsOfNewTaskDAL(siteUrl);
            if (delegationsOfNewTaskDALObject != null)
            {
                string queryString = $@"<Where>
                                            <And>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.ListUrl}' />
                                                        <Value Type='Text'>{listUrl}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                        <Value Type='Lookup'>{fromEmployeeId}</Value>
                                                    </Eq>
                                                </And>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.FromDate}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.ToDate}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                </And>
                                            </And>
                                    </Where>";

                toEmployeeIds.Sort();
                string toEmployeeIdsStr = string.Join("#", toEmployeeIds);
                toEmployeeIdsStr = !string.IsNullOrEmpty(toEmployeeIdsStr) ? string.Format("#{0}#", toEmployeeIdsStr) : toEmployeeIdsStr;

                SPQuery query = new SPQuery { Query = queryString };
                var delegations = delegationsOfNewTaskDALObject.GetByQuery(query);
                if (delegations != null && delegations.Count > 0)
                {
                    foreach (var delegation in delegations)
                    {
                        List<int> delegatedEmployeeIds = delegation.ToEmployee.Select(e => e.LookupId).ToList();
                        delegatedEmployeeIds.Sort();
                        string delegatedEmployeeIdsStr = string.Join("#", delegatedEmployeeIds);
                        delegatedEmployeeIdsStr = !string.IsNullOrEmpty(delegatedEmployeeIdsStr) ? string.Format("#{0}#", delegatedEmployeeIdsStr) : delegatedEmployeeIdsStr;
                        if (delegatedEmployeeIdsStr.IndexOf(toEmployeeIdsStr) >= 0)
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        public static bool IsDelegationOfNewTaskExisted(DelegationOfNewTask delegationOfNewTask, string siteUrl)
        {
            var result = false;

            List<int> toEmployeeIds = new List<int>();
            foreach (var toEmployee in delegationOfNewTask.ToEmployee)
            {
                toEmployeeIds.Add(toEmployee.LookupId);
            }
            result = IsDelegationOfNewTaskExisted(delegationOfNewTask.FromEmployee.LookupId, toEmployeeIds, delegationOfNewTask.ListUrl, delegationOfNewTask.FromDate, delegationOfNewTask.ToDate, siteUrl);

            return result;
        }
    }
}
