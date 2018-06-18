using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RBVH.Stada.Intranet.Biz.DelegationManagement
{
    /// <summary>
    /// DelegationPermissionManager
    /// </summary>
    public class DelegationPermissionManager
    {
        #region Attributes
        private static DelegationsDAL delegationsDALObject;
        private static DelegationsOfNewTaskDAL delegationsOfNewTaskDAL;
        private static DelegationEmployeePositionsDAL delegationEmployeePositionsDAL;
        private static EmployeeInfoDAL employeeInfoDAL;
        #endregion

        #region Constructors
        private DelegationPermissionManager()
        {
        }
        #endregion

        #region Methods
        public static List<Delegation> HasDelegations(int fromEmployeeId, string listUrl, int itemId)
        {
            List<Delegation> res = null;

            string webUrl = string.Empty;
            if (SPContext.Current != null)
            {
                webUrl = SPContext.Current.Site.Url;
            }
            if (!string.IsNullOrEmpty(webUrl))
            {
                res = HasDelegations(fromEmployeeId, listUrl, itemId, webUrl);
            }

            return res;
        }

        /// <summary>
        /// To check employee who has delegation.
        /// </summary>
        /// <param name="taksFromEmployeeId">The employ id who is processing for this task.</param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <param name="itemId">The id of SPListItem (task).</param>
        /// <returns>Returns list of delegations which an employee has been delegated to others.</returns>
        public static List<Delegation> HasDelegations(int fromEmployeeId, string listUrl, int itemId, string webUrl)
        {
            List<Delegation> res = null;

            if (delegationsDALObject == null)
            {
                delegationsDALObject = new DelegationsDAL(webUrl);
            }

            if (delegationsDALObject != null)
            {
                string queryString = $@"<Where>
                                        <And>
                                            <Leq>
                                                <FieldRef Name='{StringConstant.DelegationsList.Fields.FromDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Leq>
                                            <And>
                                                <And>
                                                    <And>
                                                        <Geq>
                                                            <FieldRef Name='{StringConstant.DelegationsList.Fields.ToDate}' />
                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                        </Geq>
                                                        <Eq>
                                                            <FieldRef Name='{StringConstant.DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                            <Value Type='Lookup'>{fromEmployeeId}</Value>
                                                        </Eq>
                                                    </And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsList.Fields.ListUrl}' />
                                                        <Value Type='Text'>{listUrl}</Value>
                                                    </Eq>
                                                </And>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.DelegationsList.Fields.ListItemID}' />
                                                    <Value Type='Number'>{itemId}</Value>
                                                </Eq>
                                            </And>
                                        </And>
                                    </Where>";
                SPQuery query = new SPQuery { Query = queryString };
                res = delegationsDALObject.GetByQuery(query);
            }

            return res;
        }

        /// <summary>
        /// To check employee who has delegation of new task.
        /// </summary>
        /// <param name="taksFromEmployeeId">The employ id who is processing for this task.</param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <returns>Returns list of configurations about DelegationOfNewTasks which allows to create new delegation for delegated employees.</returns>
        public static List<DelegationOfNewTask> HasDelegationOfNewTasks(int fromEmployeeId, string listUrl)
        {
            List<DelegationOfNewTask> res = null;

            string webUrl = string.Empty;
            if (SPContext.Current != null)
            {
                webUrl = SPContext.Current.Web.Url;
            }
            if (!string.IsNullOrEmpty(webUrl))
            {
                res = HasDelegationOfNewTasks(fromEmployeeId, listUrl, webUrl);
            }

            return res;
        }

        /// <summary>
        ///  To check employee who has delegation of new task.
        /// </summary>
        /// <param name="fromEmployeeId">The employ id who is processing for this task.</param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <param name="webUrl">The url of current web.</param>
        /// <returns>Returns list of configurations about DelegationOfNewTasks which allows to create new delegation for delegated employees.</returns>
        public static List<DelegationOfNewTask> HasDelegationOfNewTasks(int fromEmployeeId, string listUrl, string webUrl)
        {
            List<DelegationOfNewTask> res = null;

            if (delegationsOfNewTaskDAL == null)
            {
                delegationsOfNewTaskDAL = new DelegationsOfNewTaskDAL(webUrl);
            }

            string queryString = $@"<Where>
                                        <And>
                                            <Leq>
                                                <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.FromDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Leq>
                                            <And>
                                                <Geq>
                                                    <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.ToDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                </Geq>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                        <Value Type='Lookup'>{fromEmployeeId}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.ListUrl}' />
                                                        <Value Type='Text'>{listUrl}</Value>
                                                    </Eq>
                                                </And>
                                            </And>
                                        </And>
                                    </Where>";
            SPQuery query = new SPQuery { Query = queryString };
            res = delegationsOfNewTaskDAL.GetByQuery(query);

            return res;
        }

        /// <summary>
        /// Get list of delegated employees from employee.
        /// </summary>
        /// <param name="siteUrl">The current site url.</param>
        /// <param name="fromEmployeeId">The employ id who is processing for this task.</param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <param name="itemId">The id of SPListItem (task).</param>
        /// <returns>If return value is not null, that meaning this from employee id has delegation, return delegated employee (To Employee). Otherwise null value.</returns>
        public static List<EmployeeInfo> GetListOfDelegatedEmployees(string siteUrl, int fromEmployeeId, string listUrl, int itemId)
        {
            List<EmployeeInfo> toEmployees = null;

            var delegationOfNewTasks = HasDelegationOfNewTasks(fromEmployeeId, listUrl, siteUrl);
            if (delegationOfNewTasks != null)
            {
                List<int> employeeIds = new List<int>();
                foreach (var delegationOfNewTask in delegationOfNewTasks)
                {
                    if (delegationOfNewTask.ToEmployee != null && delegationOfNewTask.ToEmployee.Count > 0)
                    {
                        employeeIds.AddRange(delegationOfNewTask.ToEmployee.Select(e => e.LookupId).ToList());
                    }
                }

                if (employeeIds.Count > 0)
                {
                    employeeIds = employeeIds.Distinct().ToList();
                    var employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
                    toEmployees = employeeInfoDAL.GetByIDs(employeeIds);
                }
            }

            return toEmployees;
        }

        /// <summary>
        /// GetListOfDelegatedEmployees
        /// </summary>
        /// <param name="fromEmployeeId"></param>
        /// <param name="listUrl"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static List<EmployeeInfo> GetListOfDelegatedEmployees(int fromEmployeeId, string listUrl, int itemId)
        {
            List<EmployeeInfo> toEmployees = null;

            string siteUrl = string.Empty;
            if (SPContext.Current != null)
            {
                siteUrl = SPContext.Current.Web.Url;
            }
            if (!string.IsNullOrEmpty(siteUrl))
            {
                toEmployees = GetListOfDelegatedEmployees(siteUrl, fromEmployeeId, listUrl, itemId);
            }

            return toEmployees;
        }

        /// <summary>
        /// To check employee who is delegated to process for task.
        /// </summary>
        /// <param name="fromEmployeeId"></param>
        /// <param name="toEmployeeId">The employ id who is delegated for this task.</param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <param name="itemId">The id of SPListItem (task).</param>
        /// <param name="siteUrl"></param>
        /// <returns>If return not null value, this task is delegated for this employee. Otherwise return null value.</returns>
        public static Delegation IsDelegation(int fromEmployeeId, int toEmployeeId, string listUrl, int itemId, string siteUrl)
        {
            Delegation delegation = null;

            if (DelegationManager.IsValidTask(listUrl, itemId))
            {
                if (delegationsDALObject == null)
                {
                    delegationsDALObject = new DelegationsDAL(siteUrl);
                }

                string queryString = $@"<Where>
                                        <And>
                                            <Leq>
                                                <FieldRef Name='{StringConstant.DelegationsList.Fields.FromDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Leq>
                                            <And>
                                                <Geq>
                                                    <FieldRef Name='{StringConstant.DelegationsList.Fields.ToDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                </Geq>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                        <Value Type='Lookup'>{fromEmployeeId}</Value>
                                                    </Eq>
                                                    <And>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{StringConstant.DelegationsList.Fields.ToEmployee}' LookupId='TRUE'/>
                                                                <Value Type='Lookup'>{toEmployeeId}</Value>
                                                            </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{StringConstant.DelegationsList.Fields.ListUrl}' />
                                                                <Value Type='Text'>{listUrl}</Value>
                                                            </Eq>
                                                        </And>
                                                        <Eq>
                                                            <FieldRef Name='{StringConstant.DelegationsList.Fields.ListItemID}' />
                                                            <Value Type='Number'>{itemId}</Value>
                                                        </Eq>
                                                    </And>
                                                </And>
                                            </And>
                                        </And>
                                    </Where>";
                SPQuery query = new SPQuery { Query = queryString, RowLimit = 1 };
                var delegations = delegationsDALObject.GetByQuery(query);
                if (delegations != null && delegations.Count > 0)
                {
                    delegation = delegations[0];
                }
            }

            return delegation;
        }

        /// <summary>
        /// To check current employee who is delegated to process for task.
        /// </summary>
        /// <param name="fromEmployeeId"></param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <param name="itemId">The id of SPListItem (task).</param>
        /// <returns>If return not null value, this task is delegated for current employee. Otherwise return null value.</returns>
        public static Delegation IsDelegation(int fromEmployeeId, string listUrl, int itemId)
        {
            Delegation delegation = null;

            string siteUrl = string.Empty;
            if (SPContext.Current != null)
            {
                siteUrl = SPContext.Current.Web.Url;
            }
            if (!string.IsNullOrEmpty(siteUrl))
            {
                int toEmployeeId = 0;
                EmployeeInfo currentEmployeeInfo = GetCurrentEmployeeInfo(siteUrl);
                if (currentEmployeeInfo != null)
                {
                    toEmployeeId = currentEmployeeInfo.ID;
                }
                if (toEmployeeId > 0)
                {
                    delegation = IsDelegation(fromEmployeeId, toEmployeeId, listUrl, itemId, siteUrl);
                }
            }

            return delegation;
        }

        /// <summary>
        /// To check employee who is delegated to process for new task.
        /// </summary>
        /// <param name="fromEmployeeId">The employ id who is processing for this task.</param>
        /// <param name="toEmployeeId"></param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <param name="itemId">The id of SPListItem (task).</param>
        /// <param name="siteUrl">The current site url.</param>
        /// <returns></returns>
        public static DelegationOfNewTask IsDelegationOfNewTask(int fromEmployeeId, int toEmployeeId, string listUrl, string siteUrl)
        {
            DelegationOfNewTask res = null;

            if (delegationsOfNewTaskDAL == null)
            {
                delegationsOfNewTaskDAL = new DelegationsOfNewTaskDAL(siteUrl);
            }

            string queryString = $@"<Where>
                                        <And>
                                            <Leq>
                                                <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.FromDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Leq>
                                            <And>
                                                <Geq>
                                                    <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.ToDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                </Geq>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                        <Value Type='Lookup'>{fromEmployeeId}</Value>
                                                    </Eq>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.ToEmployee}' LookupId='TRUE'/>
                                                                <Value Type='Lookup'>{toEmployeeId}</Value>
                                                            </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{StringConstant.DelegationsOfNewTaskList.Fields.ListUrl}' />
                                                                <Value Type='Text'>{listUrl}</Value>
                                                            </Eq>
                                                        </And>
                                                </And>
                                            </And>
                                        </And>
                                    </Where>";
            SPQuery query = new SPQuery { Query = queryString, RowLimit = 1 };
            var delegationsOfNewTask = delegationsOfNewTaskDAL.GetByQuery(query);
            if (delegationsOfNewTask != null && delegationsOfNewTask.Count > 0)
            {
                res = delegationsOfNewTask[0];
            }

            return res;
        }

        /// <summary>
        /// To check current employee who is delegated to process for new task.
        /// </summary>
        /// <param name="fromEmployeeId"></param>
        /// <param name="listUrl">The url of list object. For example: /Lists/Requests, /Lists/Recruitments</param>
        /// <returns>If return not null value, this task is delegated for current employee. Otherwise return null value.</returns>
        public static DelegationOfNewTask IsDelegationOfNewTask(int fromEmployeeId, string listUrl)
        {
            DelegationOfNewTask delegationOfNewTask = null;

            string siteUrl = string.Empty;
            if (SPContext.Current != null)
            {
                siteUrl = SPContext.Current.Web.Url;
            }
            if (!string.IsNullOrEmpty(siteUrl))
            {
                int toEmployeeId = 0;
                EmployeeInfo currentEmployeeInfo = GetCurrentEmployeeInfo(siteUrl);
                if (currentEmployeeInfo != null)
                {
                    toEmployeeId = currentEmployeeInfo.ID;
                }

                if (toEmployeeId > 0)
                {
                    delegationOfNewTask = IsDelegationOfNewTask(fromEmployeeId, toEmployeeId, listUrl, siteUrl);
                }
            }

            return delegationOfNewTask;
        }

        /// <summary>
        /// To check current employee who has delegation permission.
        /// </summary>
        /// <returns>If return true, current employee has delegation permission. Otherwise return false.</returns>
        public static bool DoesCurrentEmployeeHasDelegationPermission()
        {
            var res = false;

            // Administrator
            if (SPContext.Current.Web.CurrentUser.IsSiteAdmin)
            {
                res = true;
            }
            else    // User
            {
                #region User

                if (delegationEmployeePositionsDAL == null)
                {
                    delegationEmployeePositionsDAL = new DelegationEmployeePositionsDAL(SPContext.Current.Web.Url);
                }

                EmployeeInfo currentEmployeeInfo = HttpContext.Current.Session[StringConstant.EmployeeLogedin] as EmployeeInfo;

                //User is not common account, we should get from employee list
                if (currentEmployeeInfo == null)
                {
                    SPUser spUser = SPContext.Current.Web.CurrentUser;
                    if (spUser != null)
                    {
                        EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                        currentEmployeeInfo = employeeInfoDAL.GetByADAccount(spUser.ID);
                        //HttpContext.Current.Session[StringConstant.EmployeeLogedin] = currentEmployeeInfo;
                    }
                }

                if (currentEmployeeInfo != null)
                {
                    if (currentEmployeeInfo.EmployeePosition != null)
                    {
                        var employeePositionId = currentEmployeeInfo.EmployeePosition.LookupId;
                        var delegationEmployeePositions = delegationEmployeePositionsDAL.GetByEmployeePosition(employeePositionId);
                        if (delegationEmployeePositions != null && delegationEmployeePositions.Count > 0)
                        {
                            res = true;
                        }
                    }
                }

                #endregion
            }

            return res;
        }

        /// <summary>
        /// To check current employee who has approval delegation permission.
        /// </summary>
        /// <returns>If return true, current employee has approval delegation permission. Otherwise return false.</returns>
        public static bool DoesCurrentEmployeeHasApprovalPermission()
        {
            var res = false;

            if (delegationEmployeePositionsDAL == null)
            {
                delegationEmployeePositionsDAL = new DelegationEmployeePositionsDAL(SPContext.Current.Web.Url);
            }

            EmployeeInfo currentEmployeeInfo = HttpContext.Current.Session[StringConstant.EmployeeLogedin] as EmployeeInfo;

            //User is not common account, we should get from employee list
            if (currentEmployeeInfo == null)
            {
                SPUser spUser = SPContext.Current.Web.CurrentUser;
                if (spUser != null)
                {
                    EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                    currentEmployeeInfo = employeeInfoDAL.GetByADAccount(spUser.ID);
                    //HttpContext.Current.Session[StringConstant.EmployeeLogedin] = currentEmployeeInfo;
                }
            }

            if (currentEmployeeInfo != null)
            {
                if (currentEmployeeInfo.EmployeePosition != null)
                {
                    var employeePositionId = currentEmployeeInfo.EmployeePosition.LookupId;
                    string queryString = $@"<Where>  
                                        <Eq>
                                            <FieldRef Name='{StringConstant.DelegationEmployeePositionsList.Fields.DelegatedEmployeePositions}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{employeePositionId}</Value>
                                        </Eq>
                                    </Where>";
                    SPQuery query = new SPQuery { Query = queryString, RowLimit = 1 };
                    var delegationEmployeePositions = delegationEmployeePositionsDAL.GetByQuery(query);
                    if (delegationEmployeePositions != null && delegationEmployeePositions.Count > 0)
                    {
                        res = true;
                    }
                    else // Look up Delegated by:
                    {
                        var delegatedByPositionIds = currentEmployeeInfo.DelegatedBy.Select(x => x.LookupId);
                        foreach (var delegatedByPositionId in delegatedByPositionIds)
                        {
                            queryString = $@"<Where>  
                                        <Eq>
                                            <FieldRef Name='{StringConstant.DelegationEmployeePositionsList.Fields.EmployeePosition}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{delegatedByPositionId}</Value>
                                        </Eq>
                                    </Where>";
                            query = new SPQuery { Query = queryString, RowLimit = 1 };
                            delegationEmployeePositions = delegationEmployeePositionsDAL.GetByQuery(query);
                            if (delegationEmployeePositions != null && delegationEmployeePositions.Count > 0)
                            {
                                return true;
                            }
                        }
                    }

                }

                if (currentEmployeeInfo.DelegatedBy != null && currentEmployeeInfo.DelegatedBy.Count > 0)
                {
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// GetCurrentEmployeeInfo
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        private static EmployeeInfo GetCurrentEmployeeInfo(string siteUrl)
        {
            EmployeeInfo currentEmployeeInfo = null;

            if (HttpContext.Current != null)
            {
                currentEmployeeInfo = HttpContext.Current.Session[StringConstant.EmployeeLogedin] as EmployeeInfo;
            }

            //User is not common account, we should get from employee list
            if (currentEmployeeInfo == null)
            {
                SPUser spUser = null;
                if (SPContext.Current != null)
                {
                    spUser = SPContext.Current.Web.CurrentUser;
                }
                if (spUser != null)
                {
                    if (employeeInfoDAL == null)
                    {
                        employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
                    }
                    currentEmployeeInfo = employeeInfoDAL.GetByADAccount(spUser.ID);
                    //if (HttpContext.Current != null)
                    //{
                    //    HttpContext.Current.Session[StringConstant.EmployeeLogedin] = currentEmployeeInfo;
                    //}
                }
            }

            return currentEmployeeInfo;
        }

        #endregion
    }
}
