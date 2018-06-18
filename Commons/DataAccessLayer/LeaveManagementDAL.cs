using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Builder;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class LeaveManagementDAL : BaseDAL<LeaveManagement>, IModuleBuilder, IDelegationManager, IFilterTaskManager
    {
        private const string TASK_NAME = "Leave request approval/ Duyệt yêu cầu nghỉ phép";
        private const string LINK_MAIL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/{0}.aspx?{1}";

        private readonly ShiftTimeDAL _shiftTimeDAL;
        private readonly EmployeeInfoDAL _employeeInfoDAL;
        private readonly CalendarDAL _calendarDAL;
        private readonly Calendar2DAL _calendar2DAL;
        private readonly EmailTemplateDAL _emailTemplateDAL;
        private readonly AdditionalEmployeePositionDAL _additionalEmployeePositionDAL;

        private readonly string dateFormat = "MM-dd-yyyy";
        private readonly string retDateFormat = "{0:s}";

        public object WebPageResourceHelper { get; private set; }

        public LeaveManagementDAL(string siteUrl) : base(siteUrl)
        {
            _shiftTimeDAL = new ShiftTimeDAL(siteUrl);
            _employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
            _emailTemplateDAL = new EmailTemplateDAL(siteUrl);
            _calendarDAL = new CalendarDAL(siteUrl);
            _calendar2DAL = new Calendar2DAL(siteUrl);
            _additionalEmployeePositionDAL = new AdditionalEmployeePositionDAL(siteUrl);
        }

        public IList<EmployeeInfo> CreateApprovalList(int departmentId, int locationId)
        {
            List<EmployeeInfo> approvalList = new List<EmployeeInfo>();
            //team leader
            var teamLeader = _employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.TeamLeader, departmentId, locationId);
            if (teamLeader.Count > 0)
                approvalList.Add(teamLeader[0]);
            // department head
            var departmentHead = _employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.DepartmentHead, departmentId, locationId);
            if (departmentHead.Count > 0)
                approvalList.Add(departmentHead[0]);
            // BOD 
            var bodUser = DepartmentListSingleton.GetDepartmentByID(departmentId, SiteUrl).BOD;

            var bodImployeeInfo = _employeeInfoDAL.GetByADAccount(bodUser.UserName);
            if (bodImployeeInfo != null)
                approvalList.Add(bodImployeeInfo);

            return approvalList;
        }

        public IList<EmployeeInfo> CreateApprovalList(int departmentId, int locationId, EmployeeInfo requestFor)
        {
            List<EmployeeInfo> approvalList = new List<EmployeeInfo>();

            //manager
            if (requestFor.Manager != null && requestFor.Manager.LookupId > 0)
            {
                var manager = _employeeInfoDAL.GetByID(requestFor.Manager.LookupId);
                if (manager != null)
                    approvalList.Add(manager);
            }

            // department head
            var departmentHead = _employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.DepartmentHead, departmentId, locationId);
            if (departmentHead.Count > 0)
                approvalList.Add(departmentHead[0]);

            // BOD 
            var bodUser = DepartmentListSingleton.GetDepartmentByID(departmentId, SiteUrl).BOD;
            var bodImployeeInfo = _employeeInfoDAL.GetByADAccount(bodUser.UserName);
            if (bodImployeeInfo != null)
                approvalList.Add(bodImployeeInfo);

            return approvalList;
        }

        public IList<LeaveManagement> GetLeavesInRange(int employeeLookupId, DateTime fromDate, DateTime toDate, string status)
        {
            string query =
                    $@"<Where>
                            <And>
                                <Eq>
                                    <FieldRef Name='RequestFor' LookupId='TRUE'/>
                                    <Value Type='Lookup'>{employeeLookupId}</Value>
                                </Eq>
                                <And>
                                    <Leq>
                                        <FieldRef Name='CommonFrom' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                    </Leq>
                                    <And>
                                        <Geq>
                                            <FieldRef Name='To' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                        </Geq>
                                        <Eq>
                                            <FieldRef Name='ApprovalStatus' />
                                            <Value Type='Text'>{status}</Value>
                                        </Eq>
                                    </And>
                                </And>
                            </And>
                        </Where>";

            return GetByQuery(query);
        }

        public IList<LeaveManagement> GetLeavesInRangeByDepartment(int departmentId, int locationId, DateTime fromDate, DateTime toDate, string status)
        {
            string query =
                    $@"<Where>
                            <And>
                                <Eq>
                                    <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                    <Value Type='Lookup'>{departmentId}</Value>
                                </Eq>
                                <And>
                                    <Eq>
                                        <FieldRef Name='CommonLocation' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{locationId}</Value>
                                    </Eq>
                                    <And>
                                        <Leq>
                                            <FieldRef Name='CommonFrom' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                        </Leq>
                                        <And>
                                            <Geq>
                                                <FieldRef Name='To' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                            </Geq>
                                            <Eq>
                                                <FieldRef Name='ApprovalStatus' />
                                                <Value Type='Text'>{status}</Value>
                                            </Eq>
                                        </And>
                                    </And>
                                </And>
                            </And>
                        </Where>";

            return GetByQuery(query);
        }

        public Tuple<LeaveManagement, EmployeeInfo, List<EmployeeInfo>> CreateTaskListItem(SPWeb spWeb, int sourceItemId)
        {
            if (sourceItemId == 0) return null;

            LeaveManagement leaveManagement = this.GetByID(sourceItemId);

            SPList leaveList = spWeb.TryGetSPList(spWeb.Url + this.ListUrl);

            List<EmployeeInfo> toUsers = new List<EmployeeInfo>();
            EmployeeInfo currentApprover = null;
            List<int> processedTaskCollection = new List<int>();

            TaskManagement taskManagement = new TaskManagement();

            taskManagement.Department = leaveManagement.Department;
            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = leaveManagement.RequestDueDate;
            taskManagement.ItemId = leaveManagement.ID;
            taskManagement.ItemURL = leaveList.DefaultDisplayFormUrl + "?ID=" + sourceItemId;
            taskManagement.ListURL = leaveList.DefaultViewUrl;
            taskManagement.PercentComplete = 0;
            taskManagement.TaskName = TASK_NAME;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.StepModule = StepModuleList.LeaveManagement.ToString();

            string stepStatus = "";
            User nextAssign = null;
            var requestForId = leaveManagement.RequestFor.LookupId;

            if (leaveManagement.TLE != null)
            {
                EmployeeInfo approver = _employeeInfoDAL.GetByADAccount(leaveManagement.TLE.UserName);
                if (approver.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.TeamLeader)
                {
                    stepStatus = StepStatusList.TLEApproval;
                }
                else if (approver.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.ShiftLeader)
                {
                    stepStatus = StepStatusList.SLDApproval;
                }

                taskManagement.StepStatus = stepStatus;
                taskManagement.AssignedTo = leaveManagement.TLE;
                taskManagement.NextAssign = leaveManagement.DH;
                nextAssign = leaveManagement.DH;
            }
            else if (leaveManagement.DH != null)
            {
                stepStatus = StepStatusList.DHApproval;
                taskManagement.StepStatus = stepStatus;
                taskManagement.AssignedTo = leaveManagement.DH;
                taskManagement.NextAssign = leaveManagement.BOD;
                nextAssign = leaveManagement.BOD;
            }
            else if (leaveManagement.BOD != null)
            {
                stepStatus = StepStatusList.BODApproval;
                taskManagement.StepStatus = stepStatus;
                taskManagement.AssignedTo = leaveManagement.BOD;
                taskManagement.NextAssign = null;
                nextAssign = null;
            }

            TaskManagementDAL taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            int retId = taskManagementDAL.SaveItem(taskManagement);
            currentApprover = _employeeInfoDAL.GetByADAccount(taskManagement.AssignedTo.ID);
            toUsers.Add(currentApprover);
            if (retId > 0 && !processedTaskCollection.Contains(retId)) processedTaskCollection.Add(retId);

            if (leaveManagement.AdditionalUser != null)
            {
                foreach (User additionalUser in leaveManagement.AdditionalUser)
                {
                    TaskManagement taskManagementTmp = BuildTaskManagement(leaveManagement, leaveList, sourceItemId, additionalUser, nextAssign, stepStatus);
                    retId = taskManagementDAL.SaveItem(taskManagementTmp);
                    if (retId > 0 && !processedTaskCollection.Contains(retId)) processedTaskCollection.Add(retId);
                }
            }
            else
            {
                if (leaveManagement.TLE != null)
                {
                    var assignee = _employeeInfoDAL.GetByADAccount(leaveManagement.TLE.UserName);
                    if (assignee != null && assignee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.ShiftLeader)
                    {
                        List<EmployeeInfo> managers = _employeeInfoDAL.GetAccountByFullNamePositionDepartment(string.Empty, new List<int>() { assignee.EmployeePosition.LookupId, (int)StringConstant.EmployeePosition.DepartmentHead }, assignee.Department.LookupId);
                        List<EmployeeInfo> leaders = managers.Where(m => m.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.ShiftLeader).ToList();

                        if (leaders.Any())
                        {
                            toUsers.AddRange(leaders);
                            foreach (var leader in leaders)
                            {
                                if (leader.ID != assignee.ID)
                                {
                                    TaskManagement taskManagementTmp = BuildTaskManagement(leaveManagement, leaveList, sourceItemId, leader.ADAccount, nextAssign, stepStatus);
                                    retId = taskManagementDAL.SaveItem(taskManagementTmp);
                                    if (retId > 0 && !processedTaskCollection.Contains(retId)) processedTaskCollection.Add(retId);
                                }
                            }

                            EmployeeInfo departmentHead = managers.Where(m => m.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead).FirstOrDefault();
                            if (departmentHead != null)
                            {
                                TaskManagement taskManagementDH = BuildTaskManagement(leaveManagement, leaveList, sourceItemId, departmentHead.ADAccount, null, StepStatusList.DHApproval);
                                retId = taskManagementDAL.SaveItem(taskManagementDH);
                                toUsers.Add(departmentHead);
                                if (retId > 0 && !processedTaskCollection.Contains(retId)) processedTaskCollection.Add(retId);
                            }
                        }
                    }
                }
            }

            if (processedTaskCollection != null && processedTaskCollection.Count > 1)
            {
                foreach (int processedItemId in processedTaskCollection)
                {
                    TaskManagement task = taskManagementDAL.GetByID(processedItemId);
                    if (task != null)
                    {
                        IEnumerable<int> relatedItemIds = processedTaskCollection.Where(e => e != processedItemId);
                        if (relatedItemIds != null && relatedItemIds.Count() > 0)
                        {
                            task.RelatedTasks = relatedItemIds.ToList<int>().ConvertAll<string>(e => e.ToString());
                            taskManagementDAL.SaveItem(task);
                        }
                    }
                }
            }

            leaveManagement.ApprovalStatus = taskManagement.StepStatus;
            SaveItem(leaveManagement);

            toUsers = toUsers.GroupBy(e => e.ID).Select(g => g.First()).ToList();
            return new Tuple<LeaveManagement, EmployeeInfo, List<EmployeeInfo>>(leaveManagement, currentApprover, toUsers);
        }

        public int SaveOrUpdate(LeaveManagement item)
        {
            int itemId = 0;
            using (SPSite spSite = new SPSite(SiteUrl))
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    itemId = SaveOrUpdate(spWeb, item);
                }
            }
            return itemId;
        }

        public int SaveOrUpdate(SPWeb spWeb, LeaveManagement item)
        {
            int returnId = 0;
            spWeb.AllowUnsafeUpdates = true;
            SPList splist = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            SPListItem spListItem;
            if (item.ID > 0)
            {
                spListItem = splist.GetItemById(item.ID);
                if (spListItem != null)
                {
                    spListItem["Modified"] = System.DateTime.Now.ToString(StringConstant.DateFormatTZForCAML);
                    spListItem[StringConstant.CommonSPListField.CommonCommentField] = item.Comment;
                    spListItem[StringConstant.CommonSPListField.ApprovalStatusField] = item.ApprovalStatus;
                    spListItem[StringConstant.LeaveManagementList.AdditionalApproverField] = ConvertMultUser(item.AdditionalUser, spWeb);
                }
            }
            else
            {
                spListItem = splist.AddItem();
                int requesterLookupId = item.Requester.LookupId;
                if (requesterLookupId == 0)
                {
                    requesterLookupId = _employeeInfoDAL.GetByADAccount(spWeb.CurrentUser.ID).ID;
                }

                spListItem[StringConstant.CommonSPListField.RequesterField] = requesterLookupId;
                spListItem[StringConstant.LeaveManagementList.RequestForField] = item.RequestFor.LookupId == 0 ? spWeb.CurrentUser.ID : item.RequestFor.LookupId;
                spListItem[StringConstant.CommonSPListField.CommonDepartmentField] = item.Department.LookupId;
                spListItem[StringConstant.CommonSPListField.CommonLocationField] = item.Location.LookupId;
                spListItem[StringConstant.LeaveManagementList.FromField] = item.From.ToString(StringConstant.DateFormatForCAML);
                spListItem[StringConstant.LeaveManagementList.ToField] = item.To.ToString(StringConstant.DateFormatForCAML);
                spListItem[StringConstant.LeaveManagementList.LeaveHoursField] = item.LeaveHours;
                spListItem[StringConstant.LeaveManagementList.ReasonField] = item.Reason;
                spListItem[StringConstant.LeaveManagementList.TotalDaysField] = item.TotalDays;
                spListItem[StringConstant.LeaveManagementList.TransferworkToField] = item.TransferworkTo.LookupId == 0 ? spWeb.CurrentUser.ID : item.TransferworkTo.LookupId;
                spListItem[StringConstant.LeaveManagementList.UnexpectedLeaveField] = item.UnexpectedLeave;
                spListItem[StringConstant.LeaveManagementList.IsValidRequestField] = item.IsValidRequest;

                if (item.TLE != null && !string.IsNullOrEmpty(item.TLE.UserName)) // Team Leader/Shift Leader
                {
                    SPUser teamLeader = SPContext.Current.Web.EnsureUser(item.TLE.UserName);
                    SPFieldUserValue teamLeaderValue = new SPFieldUserValue(SPContext.Current.Web, teamLeader.ID, teamLeader.LoginName);
                    spListItem[StringConstant.LeaveManagementList.TLEField] = teamLeaderValue;
                }
                if (item.DH != null && !string.IsNullOrEmpty(item.DH.UserName)) // Department Head
                {
                    SPUser departmentHead = SPContext.Current.Web.EnsureUser(item.DH.UserName);
                    SPFieldUserValue departmentHeadValue = new SPFieldUserValue(SPContext.Current.Web, departmentHead.ID, departmentHead.LoginName);
                    spListItem[StringConstant.LeaveManagementList.DHField] = departmentHeadValue;
                }
                if (item.BOD != null && !string.IsNullOrEmpty(item.BOD.UserName)) // BOD
                {
                    SPUser bod = SPContext.Current.Web.EnsureUser(item.BOD.UserName);
                    SPFieldUserValue bodValue = new SPFieldUserValue(SPContext.Current.Web, bod.ID, bod.LoginName);
                    spListItem[StringConstant.LeaveManagementList.BODField] = bodValue;
                }

                spListItem[StringConstant.CommonSPListField.ApprovalStatusField] = item.ApprovalStatus;
                spListItem[StringConstant.LeaveManagementList.AdditionalApproverField] = ConvertMultUser(item.AdditionalUser, spWeb);
                spListItem[StringConstant.CommonSPListField.CommonReqDueDateField] = item.RequestDueDate;
            }

            spListItem.Update();
            returnId = spListItem.ID;
            spWeb.AllowUnsafeUpdates = false;

            return returnId;
        }

        //public LeaveInfo InitLeaveInfo(DateTime currentDate, int departmentId, int locationId, int employeeId, int leaveId)
        //{
        //    var allDay = false;
        //    // Get Shift Time by Date
        //    var shiftTimeInfo = _shiftTimeDAL.GetShiftTimeByDate(currentDate.Day, currentDate.Month, currentDate.Year, departmentId, locationId, employeeId);
        //    if (shiftTimeInfo == null || shiftTimeInfo.ID == 0) // Get Default Shift Time: Ca 'HC'
        //        shiftTimeInfo = _shiftTimeDAL.GetShiftTimeByCode("HC");

        //    var shiftTimeFromTimeOnly = shiftTimeInfo.WorkingHourFromHour.TimeOfDay;
        //    var shiftTimeToTimeOnly = shiftTimeInfo.WorkingHourToHour.TimeOfDay;
        //    var currentTimeOnly = currentDate.TimeOfDay;
        //    if (shiftTimeFromTimeOnly >= currentTimeOnly || shiftTimeToTimeOnly <= currentTimeOnly)  // <= 7h || >= 16h: ALL DAY
        //        allDay = true;

        //    return new LeaveInfo
        //    {
        //        Day = currentDate.Day,
        //        LeaveManagementId = leaveId,
        //        AllDay = allDay,
        //        ItemUrl = $"/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId={leaveId}"
        //    };
        //}

        public LeaveInfo InitLeaveInfo(DateTime currentDate, int departmentId, int locationId, int employeeId, int leaveId,
            List<Biz.Models.ShiftManagement> shiftCollection, List<ShiftManagementDetail> shiftDetailCollection, List<Biz.Models.ShiftTime> shiftTimeCollection, bool isToDate = false)
        {
            var allDay = false;
            // Get Shift Time by Date
            var shiftTimeInfo = GetApprovedShiftTime(currentDate, employeeId, departmentId, locationId, shiftCollection, shiftDetailCollection, shiftTimeCollection);
            if (shiftTimeInfo == null || shiftTimeInfo.ID == 0) // Get Default Shift Time: Ca 'HC'
                shiftTimeInfo = shiftTimeCollection.Where(e => e.Code.ToUpper() == "HC").FirstOrDefault();

            if (shiftTimeInfo != null)
            {
                var shiftTimeFromTimeOnly = shiftTimeInfo.WorkingHourFromHour.TimeOfDay;
                var shiftTimeToTimeOnly = shiftTimeInfo.WorkingHourToHour.TimeOfDay;
                var currentTimeOnly = currentDate.TimeOfDay;
                if (isToDate == false)
                {
                    if (shiftTimeFromTimeOnly >= currentTimeOnly || shiftTimeToTimeOnly <= currentTimeOnly)  // <= 7:15h || >= 16h: ALL DAY
                        allDay = true;
                }
                else
                {
                    if (currentTimeOnly <= shiftTimeFromTimeOnly)  // <= 7:15h
                    {
                        return new LeaveInfo() { Day = 0, LeaveManagementId = 0, AllDay = false, ItemUrl = "" };
                    }
                    else if (currentTimeOnly >= shiftTimeToTimeOnly)
                    {
                        allDay = true;
                    }
                    else
                    {
                        allDay = false;
                    }
                }
            }

            return new LeaveInfo
            {
                Day = currentDate.Day,
                LeaveManagementId = leaveId,
                AllDay = allDay,
                ItemUrl = $"/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId={leaveId}"
            };
        }

        private Biz.Models.ShiftTime GetApprovedShiftTime(DateTime currentDate, int employeeId, int departmentId, int locationId, List<ShiftManagement> shiftCollection, List<ShiftManagementDetail> shiftDetailCollection, List<Biz.Models.ShiftTime> shiftTimeCollection)
        {
            if (currentDate.Day >= 21)
            {
                currentDate = currentDate.Date.AddMonths(1);
            }

            Biz.Models.ShiftTime shiftTime = new Biz.Models.ShiftTime();

            var shiftManagementList = shiftCollection.Where(e => e.Month == currentDate.Month && e.Year == currentDate.Year && e.Department.LookupId == departmentId && e.Location.LookupId == locationId).SingleOrDefault();
            if (shiftManagementList != null)
            {
                var shiftmanagementDetail = shiftDetailCollection.Where(e => e.ShiftManagementID.LookupId == shiftManagementList.ID && e.Employee.LookupId == employeeId).SingleOrDefault();
                if (shiftmanagementDetail != null)
                {
                    var lookupItem = (LookupItem)shiftmanagementDetail.GetType().GetProperty("ShiftTime" + currentDate.Day).GetValue(shiftmanagementDetail);
                    var isApproved = (Boolean)shiftmanagementDetail.GetType().GetProperty("ShiftTime" + currentDate.Day + "Approval").GetValue(shiftmanagementDetail);
                    if (lookupItem != null && lookupItem.LookupId > 0 && isApproved)
                    {
                        shiftTime = shiftTimeCollection.Where(e => e.ID == lookupItem.LookupId).FirstOrDefault();
                    }

                }
            }

            return shiftTime;
        }

        public bool ExceedSequenceRequests(LeaveResult leaveResult, EmployeeInfo employeeInfo)
        {
            bool ret = false;

            double leaveHours = leaveResult.TotalHours;
            var currentWeb = SPContext.Current.Web;
            // Check backward
            DateTime bottomOfRange = leaveResult.From.Value.AddDays(-21);
            DateTime topOfRange = leaveResult.From.Value;

            SPQuery spQuery = new SPQuery();
            spQuery.Query = string.Format(@"<Where>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='RequestFor' LookupId='TRUE' />
                                                        <Value Type='Lookup'>{0}</Value>
                                                    </Eq>
                                                    <And>
                                                        <Neq>
                                                            <FieldRef Name='ApprovalStatus' />
                                                            <Value Type='Text'>Cancelled</Value>
                                                        </Neq>
                                                        <And>
                                                            <Neq>
                                                                <FieldRef Name='ApprovalStatus' />
                                                                <Value Type='Text'>Rejected</Value>
                                                            </Neq>
                                                            <And>
                                                                <Geq>
                                                                    <FieldRef Name='CommonFrom' />
                                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                                                </Geq>
                                                                <Leq>
                                                                    <FieldRef Name='To' />
                                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{2}</Value>
                                                                </Leq>
                                                            </And>
                                                        </And>
                                                    </And>
                                                </And>
                                            </Where>
                                            <OrderBy>
                                                <FieldRef Name='CommonFrom' Ascending='False' />
                                            </OrderBy>", employeeInfo.ID, bottomOfRange.ToString(DateFormatTZForCAML), topOfRange.ToString(DateFormatTZForCAML));

            spQuery.ViewFields = @"<FieldRef Name='ID' />
                                          <FieldRef Name='Requester' />
                                          <FieldRef Name='RequestFor' />
                                          <FieldRef Name='CommonFrom' />
                                          <FieldRef Name='To' />
                                          <FieldRef Name='LeaveHours' />";

            SPListItemCollection items = this.GetByQueryToSPListItemCollection(currentWeb, spQuery);
            if (items != null && items.Count > 0)
            {
                DateTime topBound = leaveResult.From.Value;

                foreach (SPListItem item in items)
                {
                    DateTime commonFromVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.FromField).Id];
                    DateTime toVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.ToField).Id];

                    List<DateTime> datesToCheck = new List<DateTime>();
                    int idx = -1;
                    while (topBound.Date.AddDays(idx) > toVal.Date)
                    {
                        datesToCheck.Add(topBound.Date.AddDays(idx));
                        idx -= 1;
                    }

                    bool hasWorkingDay = false;
                    if (datesToCheck.Count > 0)
                    {
                        foreach (var date in datesToCheck)
                        {
                            Biz.Models.Calendar calendar = GetHolidayInfo(date.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
                            if (calendar != null)
                            {
                                hasWorkingDay = true;
                            }
                        }
                    }

                    if (hasWorkingDay == true)
                    {
                        break;
                    }
                    else if (hasWorkingDay == false)
                    {
                        string fieldValue = item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.LeaveHoursField).Id] + string.Empty;
                        if (!string.IsNullOrEmpty(fieldValue))
                        {
                            double numVal = 0;
                            double.TryParse(fieldValue, out numVal);
                            leaveHours += numVal;
                        }

                        if (leaveHours >= 40)
                        {
                            return true;
                        }
                    }

                    topBound = commonFromVal;
                }
            }

            //Check forward
            bottomOfRange = leaveResult.To.Value;
            topOfRange = leaveResult.To.Value.AddDays(21);

            spQuery = new SPQuery();
            spQuery.Query = string.Format(@"<Where>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='RequestFor' LookupId='TRUE' />
                                                        <Value Type='Lookup'>{0}</Value>
                                                    </Eq>
                                                    <And>
                                                        <Neq>
                                                            <FieldRef Name='ApprovalStatus' />
                                                            <Value Type='Text'>Cancelled</Value>
                                                        </Neq>
                                                        <And>
                                                            <Neq>
                                                                <FieldRef Name='ApprovalStatus' />
                                                                <Value Type='Text'>Rejected</Value>
                                                            </Neq>
                                                            <And>
                                                                <Geq>
                                                                    <FieldRef Name='CommonFrom' />
                                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                                                </Geq>
                                                                <Leq>
                                                                    <FieldRef Name='To' />
                                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{2}</Value>
                                                                </Leq>
                                                            </And>
                                                        </And>
                                                    </And>
                                                </And>
                                            </Where>
                                            <OrderBy>
                                                <FieldRef Name='CommonFrom' Ascending='False' />
                                            </OrderBy>", employeeInfo.ID, bottomOfRange.ToString(DateFormatTZForCAML), topOfRange.ToString(DateFormatTZForCAML));

            spQuery.ViewFields = @"<FieldRef Name='ID' />
                                          <FieldRef Name='Requester' />
                                          <FieldRef Name='RequestFor' />
                                          <FieldRef Name='CommonFrom' />
                                          <FieldRef Name='To' />
                                          <FieldRef Name='LeaveHours' />";

            items = this.GetByQueryToSPListItemCollection(currentWeb, spQuery);
            if (items != null && items.Count > 0)
            {
                DateTime bottomBound = leaveResult.To.Value;
                foreach (SPListItem item in items)
                {
                    DateTime commonFromVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.FromField).Id];
                    DateTime toVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.ToField).Id];

                    List<DateTime> datesToCheck = new List<DateTime>();
                    int idx = 1;
                    while (bottomBound.Date.AddDays(idx) < commonFromVal.Date)
                    {
                        datesToCheck.Add(bottomBound.Date.AddDays(idx));
                        idx += 1;
                    }

                    bool hasWorkingDay = false;
                    if (datesToCheck.Count > 0)
                    {
                        foreach (var date in datesToCheck)
                        {
                            Biz.Models.Calendar calendar = GetHolidayInfo(date.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
                            if (calendar != null)
                            {
                                hasWorkingDay = true;
                            }
                        }
                    }

                    if (hasWorkingDay == true)
                    {
                        break;
                    }
                    else if (hasWorkingDay == false)
                    {
                        string fieldValue = item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.LeaveHoursField).Id] + string.Empty;
                        if (!string.IsNullOrEmpty(fieldValue))
                        {
                            double numVal = 0;
                            double.TryParse(fieldValue, out numVal);
                            leaveHours += numVal;
                        }

                        if (leaveHours >= 40)
                        {
                            return true;
                        }
                    }

                    bottomBound = toVal;
                }
            }

            return ret;
        }

        //public bool ExceedSequenceRequestsBack(LeaveResult leaveResult, EmployeeInfo employeeInfo)
        //{
        //    bool ret = false;

        //    DateTime topOfRange = leaveResult.From.Value;
        //    DateTime bottomOfRange = leaveResult.From.Value.AddDays(-21);

        //    SPQuery spQuery = new SPQuery();
        //    spQuery.Query = string.Format(@"<Where>
        //                                        <And>
        //                                            <Eq>
        //                                                <FieldRef Name='RequestFor' LookupId='TRUE' />
        //                                                <Value Type='Lookup'>{0}</Value>
        //                                            </Eq>
        //                                            <And>
        //                                                <Eq>
        //                                                    <FieldRef Name='IsValidRequest' />
        //                                                    <Value Type='Boolean'>1</Value>
        //                                                </Eq>
        //                                                <And>
        //                                                    <Eq>
        //                                                        <FieldRef Name='ApprovalStatus' />
        //                                                        <Value Type='Text'>Approved</Value>
        //                                                    </Eq>
        //                                                    <And>
        //                                                        <Geq>
        //                                                            <FieldRef Name='CommonFrom' />
        //                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
        //                                                        </Geq>
        //                                                        <Lt>
        //                                                            <FieldRef Name='CommonFrom' />
        //                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{2}</Value>
        //                                                        </Lt>
        //                                                    </And>
        //                                                </And>
        //                                            </And>
        //                                        </And>
        //                                    </Where>
        //                                    <OrderBy>
        //                                        <FieldRef Name='CommonFrom' Ascending='False' />
        //                                    </OrderBy>", employeeInfo.ID, string.Format(retDateFormat, bottomOfRange), string.Format(retDateFormat, topOfRange));

        //    spQuery.ViewFields = @"<FieldRef Name='ID' />
        //                                  <FieldRef Name='Requester' />
        //                                  <FieldRef Name='RequestFor' />
        //                                  <FieldRef Name='CommonFrom' />
        //                                  <FieldRef Name='To' />
        //                                  <FieldRef Name='LeaveHours' />";

        //    SPListItemCollection items = this.GetByQueryToSPListItemCollection(spQuery);
        //    if (items != null && items.Count > 0)
        //    {
        //        double leaveHours = leaveResult.TotalHours;
        //        DateTime topBound = leaveResult.From.Value;

        //        foreach (SPListItem item in items)
        //        {
        //            string fieldValue = item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.LeaveHoursField).Id] + string.Empty;
        //            if (!string.IsNullOrEmpty(fieldValue))
        //            {
        //                double numVal = 0;
        //                double.TryParse(fieldValue, out numVal);
        //                leaveHours += numVal;
        //            }

        //            DateTime commonFromVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.FromField).Id];
        //            DateTime toVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.ToField).Id];

        //            List<DateTime> datesToCheck = new List<DateTime>();
        //            int idx = -1;
        //            while (topBound.Date.AddDays(idx) > toVal.Date)
        //            {
        //                datesToCheck.Add(topBound.Date.AddDays(idx));
        //                idx -= 1;
        //            }

        //            bool hasWorkingDay = false;
        //            if (datesToCheck.Count > 0)
        //            {
        //                foreach (var date in datesToCheck)
        //                {
        //                    Biz.Models.Calendar calendar = GetHolidayInfo(date.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
        //                    if (calendar == null)
        //                    {
        //                        hasWorkingDay = true;
        //                    }
        //                }
        //            }

        //            if (hasWorkingDay == true)
        //            {
        //                return false;
        //            }
        //            else if (hasWorkingDay == false && leaveHours >= 40)
        //            {
        //                return true;
        //            }

        //            topBound = commonFromVal;
        //        }
        //    }

        //    return ret;
        //}

        //public bool ExceedSequenceRequestsForward(LeaveResult leaveResult, EmployeeInfo employeeInfo)
        //{
        //    bool ret = false;

        //    DateTime topOfRange = leaveResult.To.Value.AddDays(21);
        //    DateTime bottomOfRange = leaveResult.To.Value;

        //    SPQuery spQuery = new SPQuery();
        //    spQuery.Query = string.Format(@"<Where>
        //                                        <And>
        //                                            <Eq>
        //                                                <FieldRef Name='RequestFor' LookupId='TRUE' />
        //                                                <Value Type='Lookup'>{0}</Value>
        //                                            </Eq>
        //                                            <And>
        //                                                <Eq>
        //                                                    <FieldRef Name='IsValidRequest' />
        //                                                    <Value Type='Boolean'>1</Value>
        //                                                </Eq>
        //                                                <And>
        //                                                    <Eq>
        //                                                        <FieldRef Name='ApprovalStatus' />
        //                                                        <Value Type='Text'>Approved</Value>
        //                                                    </Eq>
        //                                                    <And>
        //                                                        <Gt>
        //                                                            <FieldRef Name='CommonFrom' />
        //                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
        //                                                        </Gt>
        //                                                        <Leq>
        //                                                            <FieldRef Name='CommonFrom' />
        //                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{2}</Value>
        //                                                        </Leq>
        //                                                    </And>
        //                                                </And>
        //                                            </And>
        //                                        </And>
        //                                    </Where>
        //                                    <OrderBy>
        //                                        <FieldRef Name='CommonFrom' Ascending='True' />
        //                                    </OrderBy>", employeeInfo.ID, string.Format(retDateFormat, bottomOfRange), string.Format(retDateFormat, topOfRange));

        //    spQuery.ViewFields = @"<FieldRef Name='ID' />
        //                                  <FieldRef Name='Requester' />
        //                                  <FieldRef Name='RequestFor' />
        //                                  <FieldRef Name='CommonFrom' />
        //                                  <FieldRef Name='To' />
        //                                  <FieldRef Name='LeaveHours' />";

        //    SPListItemCollection items = this.GetByQueryToSPListItemCollection(spQuery);
        //    if (items != null && items.Count > 0)
        //    {
        //        double leaveHours = leaveResult.TotalHours;

        //        DateTime bottomBound = leaveResult.To.Value;
        //        foreach (SPListItem item in items)
        //        {
        //            string fieldValue = item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.LeaveHoursField).Id] + string.Empty;
        //            if (!string.IsNullOrEmpty(fieldValue))
        //            {
        //                double numVal = 0;
        //                double.TryParse(fieldValue, out numVal);
        //                leaveHours += numVal;
        //            }

        //            DateTime commonFromVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.FromField).Id];
        //            DateTime toVal = (DateTime)item[item.Fields.GetFieldByInternalName(StringConstant.LeaveManagementList.ToField).Id];

        //            List<DateTime> datesToCheck = new List<DateTime>();
        //            int idx = 1;
        //            while (bottomBound.Date.AddDays(idx) < commonFromVal.Date)
        //            {
        //                datesToCheck.Add(bottomBound.Date.AddDays(idx));
        //                idx += 1;
        //            }

        //            bool hasWorkingDay = false;
        //            if (datesToCheck.Count > 0)
        //            {
        //                foreach (var date in datesToCheck)
        //                {
        //                    Biz.Models.Calendar calendar = GetHolidayInfo(date.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
        //                    if (calendar == null)
        //                    {
        //                        hasWorkingDay = true;
        //                    }
        //                }
        //            }

        //            if (hasWorkingDay == true)
        //            {
        //                return false;
        //            }
        //            else if (hasWorkingDay == false && leaveHours >= 40)
        //            {
        //                return true;
        //            }

        //            bottomBound = toVal;
        //        }
        //    }

        //    return ret;
        //}

        public LeaveResult CalculateTotalHoursAndDays(LeaveResult leaveResult, List<Biz.Models.ShiftTime> shiftTimeCollection, EmployeeInfo employeeInfo)
        {
            bool isSecurity = _additionalEmployeePositionDAL.GetAdditionalPosition(employeeInfo.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);

            if (isSecurity == true)
            {
                return CalculateTotalHoursAndDaysSecurityGuard(leaveResult, shiftTimeCollection, employeeInfo);
            }
            else
            {
                return CalculateTotalHoursAndDaysGeneral(leaveResult, shiftTimeCollection, employeeInfo);
            }
        }

        public LeaveResult CalculateTotalHoursAndDaysGeneral(LeaveResult leaveResult, List<Biz.Models.ShiftTime> shiftTimeCollection, EmployeeInfo employeeInfo)
        {
            if (leaveResult.WorkingDays == null || leaveResult.WorkingDays.Count == 0) return leaveResult;

            if (shiftTimeCollection == null) shiftTimeCollection = _shiftTimeDAL.GetAll();
            Biz.Models.ShiftTime defaultShiftTime = GetDefaultShiftTime(_shiftTimeDAL, shiftTimeCollection, employeeInfo);

            if (leaveResult.From.Value.Date == DateTime.Now.Date)
            {
                leaveResult.UnexpectedLeave = true;
            }

            for (int i = 0; i < leaveResult.WorkingDays.Count; i++)
            {
                WorkingDay workingDay = leaveResult.WorkingDays[i];
                var shiftOfWorkingDay = workingDay.IsDefaultShift == true ? defaultShiftTime : workingDay.Shift;
                int diffDays = shiftOfWorkingDay.WorkingHourToHour.Date.Subtract(shiftOfWorkingDay.WorkingHourFromHour.Date).Days;
                var fromWH = workingDay.Date.Date.AddTicks(shiftOfWorkingDay.WorkingHourFromHour.TimeOfDay.Ticks);
                var toWH = fromWH.AddDays(diffDays).Date.AddTicks(shiftOfWorkingDay.WorkingHourToHour.TimeOfDay.Ticks);

                double timeToMinus = 0;
                if (fromWH < leaveResult.From.Value && leaveResult.From.Value < toWH)
                {
                    if (i == 0 && diffDays > 0 && leaveResult.From.Value.Date.AddDays(-1) == DateTime.Now.Date)
                    {
                        leaveResult.UnexpectedLeave = true;
                    }

                    if (shiftOfWorkingDay.ShiftTimeBreakHourNumber > 0)
                    {
                        var breakingHourFrom = workingDay.Date.Date.AddTicks(shiftOfWorkingDay.BreakHourFromHour.TimeOfDay.Ticks);
                        var diffBreakingDays = shiftOfWorkingDay.BreakHourToHour.Date.Subtract(shiftOfWorkingDay.BreakHourFromHour.Date).Days;
                        var breakingHourTo = breakingHourFrom.AddDays(diffBreakingDays).Date.AddTicks(shiftOfWorkingDay.BreakHourToHour.TimeOfDay.Ticks);

                        if (leaveResult.From.Value <= breakingHourFrom)
                        {
                            timeToMinus += (leaveResult.From.Value - fromWH).TotalHours;
                        }
                        else if (leaveResult.From.Value >= breakingHourTo)
                        {
                            timeToMinus += (leaveResult.From.Value - fromWH).TotalHours - shiftOfWorkingDay.ShiftTimeBreakHourNumber;
                        }
                        else if (breakingHourFrom < leaveResult.From.Value && leaveResult.From.Value < breakingHourTo)
                        {
                            timeToMinus += (breakingHourFrom - fromWH).TotalHours;
                        }
                    }
                    else
                    {
                        timeToMinus += (leaveResult.From.Value - fromWH).TotalHours;
                    }
                }

                if (fromWH < leaveResult.To.Value && leaveResult.To.Value < toWH)
                {
                    if (shiftOfWorkingDay.ShiftTimeBreakHourNumber > 0)
                    {
                        var breakingHourFrom = workingDay.Date.Date.AddTicks(shiftOfWorkingDay.BreakHourFromHour.TimeOfDay.Ticks);
                        var diffBreakingDays = shiftOfWorkingDay.BreakHourToHour.Date.Subtract(shiftOfWorkingDay.BreakHourFromHour.Date).Days;
                        var breakingHourTo = breakingHourFrom.AddDays(diffBreakingDays).Date.AddTicks(shiftOfWorkingDay.BreakHourToHour.TimeOfDay.Ticks);

                        if (leaveResult.To.Value <= breakingHourFrom)
                        {
                            timeToMinus += (toWH - leaveResult.To.Value).TotalHours - shiftOfWorkingDay.ShiftTimeBreakHourNumber;
                        }
                        else if (leaveResult.To.Value >= breakingHourTo)
                        {
                            timeToMinus += (toWH - leaveResult.To.Value).TotalHours;
                        }
                        else if (breakingHourFrom < leaveResult.To.Value && leaveResult.To.Value < breakingHourTo)
                        {
                            timeToMinus += (toWH - breakingHourTo).TotalHours;
                        }
                    }
                    else
                    {
                        timeToMinus += (toWH - leaveResult.To.Value).TotalHours;
                    }
                }

                if (leaveResult.From.Value >= toWH || leaveResult.To.Value <= fromWH)
                {
                    workingDay.LeaveHours = 0;
                }
                else
                {
                    workingDay.LeaveHours = shiftOfWorkingDay.ShiftTimeWorkingHourNumber;
                }

                workingDay.LeaveHours -= timeToMinus;
            }

            for (int i = 0; i < leaveResult.WorkingDays.Count; i++)
            {
                WorkingDay workingDay = leaveResult.WorkingDays[i];
                leaveResult.TotalHours += workingDay.LeaveHours;

                double workingHourNumber = (workingDay.IsDefaultShift == true || workingDay.Shift == null) ? leaveResult.DefaultShiftTime.ShiftTimeWorkingHourNumber : workingDay.Shift.ShiftTimeWorkingHourNumber;
                if (workingHourNumber > 0)
                {
                    leaveResult.TotalDays += (workingDay.LeaveHours / workingHourNumber);
                }
            }

            double tempNum = leaveResult.TotalHours - (int)leaveResult.TotalHours;
            if (tempNum <= 0.25)
            {
                leaveResult.TotalHours = leaveResult.TotalHours - tempNum;
            }
            leaveResult.TotalHours = Math.Round(leaveResult.TotalHours * 2, MidpointRounding.AwayFromZero) / 2;
            leaveResult.TotalDays = Math.Round(leaveResult.TotalDays, 1);

            return leaveResult;
        }

        public LeaveResult CalculateTotalHoursAndDaysSecurityGuard(LeaveResult leaveResult, List<Biz.Models.ShiftTime> shiftTimeCollection, EmployeeInfo employeeInfo)
        {
            if (leaveResult.WorkingDays == null || leaveResult.WorkingDays.Count == 0) return leaveResult;

            if (shiftTimeCollection == null) shiftTimeCollection = _shiftTimeDAL.GetAll();
            Biz.Models.ShiftTime defaultShiftTime = GetDefaultShiftTime(_shiftTimeDAL, shiftTimeCollection, employeeInfo);

            if (leaveResult.From.Value.Date == DateTime.Now.Date)
            {
                leaveResult.UnexpectedLeave = true;
            }

            for (int i = 0; i < leaveResult.WorkingDays.Count; i++)
            {
                WorkingDay workingDay = leaveResult.WorkingDays[i];
                var shiftOfWorkingDay = workingDay.IsDefaultShift == true ? defaultShiftTime : workingDay.Shift;
                int diffDays = shiftOfWorkingDay.WorkingHourToHour.Date.Subtract(shiftOfWorkingDay.WorkingHourFromHour.Date).Days;

                DateTime fromWHForChecking = DateTime.MinValue;
                DateTime toWHForChecking = DateTime.MinValue;
                switch (i)
                {
                    case 0: //6:00AM 01/03/2018-> 6:00AM 02/03/2018
                        fromWHForChecking = workingDay.Date.Date.AddTicks(shiftOfWorkingDay.WorkingHourFromHour.TimeOfDay.Ticks);
                        toWHForChecking = fromWHForChecking.AddDays(diffDays).Date.AddTicks(shiftOfWorkingDay.WorkingHourFromHour.TimeOfDay.Ticks);
                        break;
                    case 1: //6:00AM 01/03/2018-> 6:30AM 02/03/2018
                        fromWHForChecking = workingDay.Date.Date.AddTicks(shiftOfWorkingDay.WorkingHourFromHour.TimeOfDay.Ticks);
                        toWHForChecking = fromWHForChecking.AddDays(diffDays).Date.AddTicks(shiftOfWorkingDay.WorkingHourToHour.TimeOfDay.Ticks);
                        break;
                    default: //6:30AM 01/03/2018-> 6:30AM 02/03/2018
                        fromWHForChecking = workingDay.Date.Date.AddTicks(shiftOfWorkingDay.WorkingHourToHour.TimeOfDay.Ticks);
                        toWHForChecking = fromWHForChecking.AddDays(diffDays).Date.AddTicks(shiftOfWorkingDay.WorkingHourToHour.TimeOfDay.Ticks);
                        break;
                }

                DateTime fromWH = (fromWHForChecking <= leaveResult.From && leaveResult.From <= toWHForChecking) ? leaveResult.From.Value : fromWHForChecking;
                DateTime toWH = (fromWHForChecking <= leaveResult.To && leaveResult.To <= toWHForChecking) ? leaveResult.To.Value : toWHForChecking;
                DateTime midWH = fromWH.Date.Date.AddTicks(shiftOfWorkingDay.WorkingHourMidHour.Value.TimeOfDay.Ticks);

                if (!(toWH <= leaveResult.From || fromWH >= leaveResult.To))
                {
                    if (fromWH < midWH && midWH < toWH)
                    {
                        leaveResult.WorkingDays[i].LeaveHours += CalculateActualWorkingHourSecurity(fromWH, midWH, workingDay, shiftOfWorkingDay);
                        leaveResult.WorkingDays[i].LeaveHours += CalculateActualWorkingHourSecurity(midWH, toWH, workingDay, shiftOfWorkingDay);
                    }
                    else
                    {
                        leaveResult.WorkingDays[i].LeaveHours = CalculateActualWorkingHourSecurity(fromWH, toWH, workingDay, shiftOfWorkingDay);
                    }
                }
            }

            for (int i = 0; i < leaveResult.WorkingDays.Count; i++)
            {
                leaveResult.TotalHours += leaveResult.WorkingDays[i].LeaveHours;
            }
            leaveResult.TotalDays = (leaveResult.TotalHours / leaveResult.DefaultShiftTime.ShiftTimeWorkingHourNumber);

            double tempNum = leaveResult.TotalHours - (int)leaveResult.TotalHours;
            if (tempNum <= 0.25)
            {
                leaveResult.TotalHours = leaveResult.TotalHours - tempNum;
            }
            leaveResult.TotalHours = Math.Round(leaveResult.TotalHours * 2, MidpointRounding.AwayFromZero) / 2;
            leaveResult.TotalDays = Math.Round(leaveResult.TotalDays, 1);

            return leaveResult;
        }

        public LeaveResult GetWorkingDayDetails(LeaveResult leaveResult, EmployeeInfo employeeInfo, bool isSecurity, List<Biz.Models.ShiftTime> shiftTimeCollection)
        {
            if (leaveResult.NoneWorkingDays.Count() > 0)
            {
                leaveResult.WorkingDays = leaveResult.WorkingDays == null ? leaveResult.WorkingDays = new List<WorkingDay>() : leaveResult.WorkingDays;

                foreach (NoneWorkingDay noneWorkingDay in leaveResult.NoneWorkingDays)
                {
                    DateTime noneWorkingDate = Convert.ToDateTime(noneWorkingDay.DateStr);

                    if (leaveResult.From.Value.Date == noneWorkingDate.Date)
                    {
                        var prevDate = leaveResult.From.Value.AddDays(-1).Date;
                        Biz.Models.Calendar holidayInfo = GetHolidayInfo(prevDate.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
                        if (holidayInfo == null)
                        {
                            var fromDate = leaveResult.WorkingDays.Where(wd => wd.Date.Date == prevDate).FirstOrDefault();
                            if (fromDate == null)
                            {
                                leaveResult.WorkingDays.Insert(0, new WorkingDay() { Date = prevDate });
                            }
                        }
                    }
                    else if (leaveResult.To.Value.Date == noneWorkingDate.Date)
                    {
                        var prevDate = leaveResult.To.Value.AddDays(-1).Date;
                        Biz.Models.Calendar holidayInfo = GetHolidayInfo(prevDate.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
                        if (holidayInfo == null)
                        {
                            var toDate = leaveResult.WorkingDays.Where(wd => wd.Date.Date == prevDate).FirstOrDefault();
                            if (toDate == null)
                            {
                                leaveResult.WorkingDays.Add(new WorkingDay() { Date = prevDate });
                            }
                        }
                    }
                }
            }

            if (leaveResult.WorkingDays != null && leaveResult.WorkingDays.Count == 1)
            {
                var prevDate = leaveResult.WorkingDays[0].Date.AddDays(-1).Date;

                if (isSecurity == false)
                {
                    Biz.Models.Calendar holidayInfo = GetHolidayInfo(prevDate.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
                    if (holidayInfo == null)
                    {
                        var prevOne = leaveResult.WorkingDays.Where(wd => wd.Date.Date == prevDate).FirstOrDefault();
                        if (prevOne == null)
                        {
                            leaveResult.WorkingDays.Insert(0, new WorkingDay() { Date = prevDate });
                        }
                    }
                }
                else
                {
                    //var prevOne = leaveResult.WorkingDays.Where(wd => wd.Date.Date == prevDate).FirstOrDefault();
                    //if (prevOne == null)
                    //{
                    //    leaveResult.WorkingDays.Insert(0, new WorkingDay() { Date = prevDate });
                    //}
                }
            }

            if (leaveResult.WorkingDays != null && leaveResult.WorkingDays.Count > 0 && isSecurity == true)
            {
                var prevDate = leaveResult.WorkingDays[0].Date.AddDays(-1).Date;
                var prevOne = leaveResult.WorkingDays.Where(wd => wd.Date.Date == prevDate).FirstOrDefault();
                if (prevOne == null)
                {
                    leaveResult.WorkingDays.Insert(0, new WorkingDay() { Date = prevDate });
                }
            }

            if (leaveResult.WorkingDays == null || leaveResult.WorkingDays.Count == 0) return leaveResult;

            #region Shift
            ShiftManagementDAL _shiftManagementDAL = new ShiftManagementDAL(this.SiteUrl);
            List<Biz.Models.ShiftManagement> shiftCollection = new List<Biz.Models.ShiftManagement>();

            Dictionary<string, Collection<DateTime>> monthYearDict = new Dictionary<string, Collection<DateTime>>();
            foreach (var workingDay in leaveResult.WorkingDays)
            {
                string monthYearVal = "";
                if (workingDay.Date.Day >= 21)
                {
                    DateTime tmp = workingDay.Date.AddMonths(1);
                    monthYearVal = string.Format("{0}#{1}", tmp.Month, tmp.Year);
                }
                else
                {
                    monthYearVal = string.Format("{0}#{1}", workingDay.Date.Month, workingDay.Date.Year);
                }

                if (!monthYearDict.ContainsKey(monthYearVal))
                {
                    monthYearDict.Add(monthYearVal, new Collection<DateTime>() { workingDay.Date });
                }
                else
                {
                    monthYearDict[monthYearVal].Add(workingDay.Date);
                }
            }

            foreach (var monthYear in monthYearDict)
            {
                string[] monthYearArr = monthYear.Key.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                var shifts = _shiftManagementDAL.GetByMonthYearDepartment(Convert.ToInt32(monthYearArr[0]), Convert.ToInt32(monthYearArr[1]), employeeInfo.Department.LookupId, employeeInfo.FactoryLocation.LookupId);
                if (shifts != null && shifts.Count() > 0)
                {
                    foreach (var shift in shifts)
                    {
                        if (shiftCollection.Where(e => e.ID == shift.ID).FirstOrDefault() == null)
                            shiftCollection.Add(shift);
                        else continue;
                    }
                }
            }

            List<ShiftManagementDetail> shiftDetailCollection = null;
            if (shiftCollection != null && shiftCollection.Count > 0)
            {
                ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(this.SiteUrl);
                shiftDetailCollection = new List<ShiftManagementDetail>();
                foreach (var shift in shiftCollection)
                {
                    List<ShiftManagementDetail> shiftDetail = _shiftManagementDetailDAL.GetByShiftManagementIDEmployeeID(shift.ID, employeeInfo.ID);
                    if (shiftDetail != null && shiftDetail.Count() > 0)
                    {
                        shiftDetailCollection.AddRange(shiftDetail);
                    }
                }
            }

            if (shiftDetailCollection != null && shiftDetailCollection.Count() > 0)
            {
                shiftTimeCollection = _shiftTimeDAL.GetAll();

                Type typeShiftManagementDetail = typeof(ShiftManagementDetail);
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                foreach (var monthYear in monthYearDict)
                {
                    string[] monthYearArr = monthYear.Key.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    Biz.Models.ShiftManagement shiftManagement = shiftCollection.Where(e => e.Month == Convert.ToInt32(monthYearArr[0]) && e.Year == Convert.ToInt32(monthYearArr[1])).FirstOrDefault();
                    if (shiftManagement == null) continue;

                    int shiftId = shiftManagement.ID;
                    ShiftManagementDetail shiftDetails = shiftDetailCollection.Where(e => e.ShiftManagementID.LookupId == shiftId).FirstOrDefault();
                    if (shiftDetails == null) continue;

                    foreach (var date in monthYear.Value)
                    {
                        WorkingDay workingDay = leaveResult.WorkingDays.Where(e => e.Date.Equals(date)).First();

                        if (workingDay != null & workingDay.IsDefaultShift == true)
                        {
                            PropertyInfo shiftInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", date.Day), bindingFlags);
                            PropertyInfo shiftApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", date.Day), bindingFlags);
                            object shiftInfoValue = shiftInfo.GetValue(shiftDetails, null);
                            object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftDetails, null);
                            LookupItem shiftTimeObj = shiftInfoValue as LookupItem;
                            if (shiftTimeObj.LookupId > 0 && shiftApprovalValue != null && shiftApprovalValue.Equals(true))
                            {
                                var shift = shiftTimeCollection.Where(e => e.ID.Equals(shiftTimeObj.LookupId)).FirstOrDefault();
                                int diffDays = shift.WorkingHourToHour.Date.Subtract(shift.WorkingHourFromHour.Date).Days;
                                if (workingDay.Date.AddDays(1).Date == leaveResult.From.Value.Date && diffDays > 0 && shift.Code != leaveResult.DefaultShiftTime.Code)
                                {
                                    var fromDate = leaveResult.NoneWorkingDays.Where(nw => Convert.ToDateTime(nw.DateStr).Date == leaveResult.From.Value.Date).FirstOrDefault();
                                    if (fromDate != null)
                                    {
                                        leaveResult.NoneWorkingDays.Remove(fromDate);
                                    }
                                }
                                else if (workingDay.Date.AddDays(1).Date == leaveResult.To.Value.Date && diffDays > 0 && shift.Code != leaveResult.DefaultShiftTime.Code)
                                {
                                    var toDate = leaveResult.NoneWorkingDays.Where(nw => Convert.ToDateTime(nw.DateStr).Date == leaveResult.To.Value.Date).FirstOrDefault();
                                    if (toDate != null)
                                    {
                                        leaveResult.NoneWorkingDays.Remove(toDate);
                                    }
                                }

                                workingDay.IsDefaultShift = false;
                                workingDay.Shift = shift;
                            }
                        }
                    }
                }
            }

            //bool isSecurity = _additionalEmployeePositionDAL.GetAdditionalPosition(employeeInfo.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);
            if (isSecurity == true ||
                employeeInfo.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Gardener ||
                employeeInfo.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Helper)
            {
                foreach (var monthYear in monthYearDict)
                {
                    foreach (var date in monthYear.Value)
                    {
                        WorkingDay workingDay = leaveResult.WorkingDays.Where(e => e.Date.Equals(date)).First();

                        if (workingDay != null & workingDay.IsDefaultShift == true)
                        {
                            var shift = shiftTimeCollection.Where(e => e.Code.Equals(leaveResult.DefaultShiftTime.Code)).FirstOrDefault();
                            int diffDays = shift.WorkingHourToHour.Date.Subtract(shift.WorkingHourFromHour.Date).Days;
                            if (workingDay.Date.AddDays(1).Date == leaveResult.From.Value.Date && diffDays > 0)
                            {
                                var fromDate = leaveResult.NoneWorkingDays.Where(nw => Convert.ToDateTime(nw.DateStr).Date == leaveResult.From.Value.Date).FirstOrDefault();
                                if (fromDate != null)
                                {
                                    leaveResult.NoneWorkingDays.Remove(fromDate);
                                }
                            }
                            else if (workingDay.Date.AddDays(1).Date == leaveResult.To.Value.Date && diffDays > 0)
                            {
                                var toDate = leaveResult.NoneWorkingDays.Where(nw => Convert.ToDateTime(nw.DateStr).Date == leaveResult.To.Value.Date).FirstOrDefault();
                                if (toDate != null)
                                {
                                    leaveResult.NoneWorkingDays.Remove(toDate);
                                }
                            }
                        }
                    }
                }
            }

            return leaveResult;
            #endregion
        }

        public void CheckOverlap(LeaveResult leaveResult, EmployeeInfo employeeInfo, bool isRequestFor = false)
        {
            List<Biz.Models.LeaveManagement> leaveOverlapCollection = GetByQuery(BuildOverlapQuery(employeeInfo, leaveResult.From.Value, leaveResult.To.Value, isRequestFor));
            if (leaveOverlapCollection.Any())
            {
                throw new LeaveException((int)LeaveErrorCode.Overlap,
                    ResourceHelper.GetLocalizedString("LeaveManagement_ErrMsg_OverlapDates", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID));
            }
        }

        public LeaveResult BuildDefaultShiftTime(LeaveResult leaveResult, ShiftTimeDAL _shiftTimeDAL, List<Biz.Models.ShiftTime> shiftTimeCollection, EmployeeInfo employeeInfo)
        {
            Biz.Models.ShiftTime defaultShift = GetDefaultShiftTime(_shiftTimeDAL, shiftTimeCollection, employeeInfo);
            if (defaultShift != null)
            {
                leaveResult.DefaultShiftTime = new DefaultShiftTime()
                {
                    Name = defaultShift.Name,
                    Code = defaultShift.Code,
                    WorkingHourFromHour = string.Format(retDateFormat, defaultShift.WorkingHourFromHour),
                    WorkingHourToHour = string.Format(retDateFormat, defaultShift.WorkingHourToHour),
                    ShiftTimeWorkingHourNumber = defaultShift.ShiftTimeWorkingHourNumber,
                    ShiftTimeBreakHourNumber = defaultShift.ShiftTimeBreakHourNumber,
                    BreakHourFromHour = string.Format(retDateFormat, defaultShift.BreakHourFromHour),
                    BreakHourToHour = string.Format(retDateFormat, defaultShift.BreakHourToHour),
                    UnexpectedLeaveFirstApprovalRole = defaultShift.UnexpectedLeaveFirstApprovalRole
                };
            }

            return leaveResult;
        }

        public void ApplyLeavePolicy(LeaveResult leaveResult)
        {
            int configPolicy_1_Value = 1;
            int configPolicy_2_Value = 3;
            int configPolicy_3_Value = 15;

            int[] configPolicy_1_Range_Value = new int[] { 1, 2 };
            int[] configPolicy_2_Range_Value = new int[] { 3, 4 };
            int[] configPolicy_3_Range_Value = new int[] { 5 };

            string configPolicy_1_Id = "LeaveForm_Policy_1";
            string configPolicy_2_Id = "LeaveForm_Policy_2";
            string configPolicy_3_Id = "LeaveForm_Policy_3";

            string configPolicy_1_Range_Id = "LeaveForm_Policy_1_Range";
            string configPolicy_2_Range_Id = "LeaveForm_Policy_2_Range";
            string configPolicy_3_Range_Id = "LeaveForm_Policy_3_Range";

            List<Configuration> configs = ConfigurationDAL.GetValues(this.SiteUrl, new List<string>() { configPolicy_1_Id, configPolicy_2_Id, configPolicy_3_Id, configPolicy_1_Range_Id, configPolicy_2_Range_Id, configPolicy_3_Range_Id });

            //Policy value 1
            var valObj = configs.Where(e => e.Key.ToLower() == configPolicy_1_Id.ToLower()).FirstOrDefault();
            if (valObj != null && !string.IsNullOrEmpty(valObj.Value))
            {
                int.TryParse(valObj.Value.Trim(), out configPolicy_1_Value);
            }

            //Policy value 2
            valObj = configs.Where(e => e.Key.ToLower() == configPolicy_2_Id.ToLower()).FirstOrDefault();
            if (valObj != null && !string.IsNullOrEmpty(valObj.Value))
            {
                int.TryParse(valObj.Value.Trim(), out configPolicy_2_Value);
            }

            //Policy value 3
            valObj = configs.Where(e => e.Key.ToLower() == configPolicy_3_Id.ToLower()).FirstOrDefault();
            if (valObj != null && !string.IsNullOrEmpty(valObj.Value))
            {
                int.TryParse(valObj.Value.Trim(), out configPolicy_3_Value);
            }

            //Policy range 1
            valObj = configs.Where(e => e.Key.ToLower() == configPolicy_1_Range_Id.ToLower()).FirstOrDefault();
            if (valObj != null && !string.IsNullOrEmpty(valObj.Value))
            {
                string[] arr = valObj.Value.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 1)
                {
                    int.TryParse(arr[0].Trim(), out configPolicy_1_Range_Value[0]);
                    int.TryParse(arr[1].Trim(), out configPolicy_1_Range_Value[1]);
                }
            }

            //Policy range 2
            valObj = configs.Where(e => e.Key.ToLower() == configPolicy_2_Range_Id.ToLower()).FirstOrDefault();
            if (valObj != null && !string.IsNullOrEmpty(valObj.Value))
            {
                string[] arr = valObj.Value.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 1)
                {
                    int.TryParse(arr[0].Trim(), out configPolicy_2_Range_Value[0]);
                    int.TryParse(arr[1].Trim(), out configPolicy_2_Range_Value[1]);
                }
            }

            //Policy range 1
            valObj = configs.Where(e => e.Key.ToLower() == configPolicy_3_Range_Id.ToLower()).FirstOrDefault();
            if (valObj != null && !string.IsNullOrEmpty(valObj.Value))
            {
                string[] arr = valObj.Value.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 1)
                {
                    int.TryParse(arr[0].Trim(), out configPolicy_3_Range_Value[0]);
                }
            }

            if (leaveResult.WorkingDays != null && leaveResult.WorkingDays.Count() > 0)
            {
                double totalDays = leaveResult.TotalDays;
                TimeSpan ts = leaveResult.From.Value.Date.Subtract(DateTime.Now.Date);

                if (leaveResult.UnexpectedLeave == true && totalDays > 0 && totalDays < 1) //unexpected leave in current date
                {
                    if (ts.Days < configPolicy_1_Value) throw new LeaveException((int)LeaveErrorCode.Policy1,
                        string.Format(ResourceHelper.GetLocalizedString("LeaveManagement_ErrMsg_Policy_1", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID), totalDays, configPolicy_1_Value));
                }
                else if (totalDays >= (configPolicy_1_Range_Value[0]) && totalDays < configPolicy_1_Range_Value[1] + 1) // 1 <= workingDays <= 2
                {
                    if (ts.Days < configPolicy_1_Value) throw new LeaveException((int)LeaveErrorCode.Policy1,
                        string.Format(ResourceHelper.GetLocalizedString("LeaveManagement_ErrMsg_Policy_1", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID), totalDays, configPolicy_1_Value));
                }
                else if (totalDays >= configPolicy_2_Range_Value[0] && totalDays < configPolicy_2_Range_Value[1] + 1) // 3 <= workingDays <= 4
                {
                    if (ts.Days < configPolicy_2_Value) throw new LeaveException((int)LeaveErrorCode.Policy2,
                        string.Format(ResourceHelper.GetLocalizedString("LeaveManagement_ErrMsg_Policy_2", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID), totalDays, configPolicy_2_Value));
                }
                else if (totalDays >= configPolicy_3_Range_Value[0]) // workingDays >= 5
                {
                    if (ts.Days < configPolicy_3_Value) throw new LeaveException((int)LeaveErrorCode.Policy3,
                        string.Format(ResourceHelper.GetLocalizedString("LeaveManagement_ErrMsg_Policy_3", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID), totalDays, configPolicy_3_Value));
                }
            }
        }

        public void ValidateNoneWorkingDays(LeaveResult leaveResult)
        {
            if (leaveResult.NoneWorkingDays.Count() > 0)
            {
                foreach (NoneWorkingDay noneWorkingDay in leaveResult.NoneWorkingDays)
                {
                    DateTime noneWorkingDate = Convert.ToDateTime(noneWorkingDay.DateStr);

                    if (leaveResult.From.Value.Date == noneWorkingDate.Date)
                    {
                        throw new LeaveException((int)LeaveErrorCode.FromDateIsNoneWorkingDay,
                            string.Format(ResourceHelper.GetLocalizedString("LeaveManagement_ErrMsg_FromDateIsNoneWorkingDay", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID),
                                CultureInfo.CurrentUICulture.LCID == 1033 ? noneWorkingDay.Category.ToLower() : noneWorkingDay.Title.ToLower()));
                    }
                    else if (leaveResult.To.Value.Date == noneWorkingDate.Date)
                    {
                        throw new LeaveException((int)LeaveErrorCode.ToDateIsNoneWorkingDay,
                            string.Format(ResourceHelper.GetLocalizedString("LeaveManagement_ErrMsg_ToDateIsNoneWorkingDay", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID),
                                CultureInfo.CurrentUICulture.LCID == 1033 ? noneWorkingDay.Category.ToLower() : noneWorkingDay.Title.ToLower()));
                    }
                }
            }
        }

        public LeaveResult ClassifySelectedDays(LeaveResult leaveResult, EmployeeInfo employeeInfo, bool isSecurity)
        {
            Collection<DateTime> rangeOfDate = TransformRangeOfDate(leaveResult.From.Value, leaveResult.To.Value);
            //bool isSecurity = _additionalEmployeePositionDAL.GetAdditionalPosition(employeeInfo.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);

            foreach (DateTime date in rangeOfDate)
            {
                if (isSecurity == true)
                {
                    leaveResult.WorkingDays.Add(new WorkingDay() { Date = date });
                }
                else
                {
                    Biz.Models.Calendar holidayInfo = GetHolidayInfo(date.ToString(dateFormat), employeeInfo.FactoryLocation.LookupId.ToString());
                    if (holidayInfo != null)
                    {
                        leaveResult.NoneWorkingDays.Add(new NoneWorkingDay()
                        {
                            DateStr = string.Format(retDateFormat, date),
                            Title = holidayInfo.Title,
                            Location = holidayInfo.Location,
                            Category = holidayInfo.Category
                        });
                    }
                    else
                    {
                        leaveResult.WorkingDays.Add(new WorkingDay() { Date = date });
                    }
                }
            }

            return leaveResult;
        }

        public bool UpdateShift(EmployeeInfo requesterInfo, LeaveResult leaveResult, int toShiftId)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    if (requesterInfo != null)
                    {
                        ShiftManagementDAL _shiftManagementDAL = new ShiftManagementDAL(this.SiteUrl);
                        ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(this.SiteUrl);

                        if (leaveResult.WorkingDays != null && leaveResult.WorkingDays.Count > 0)
                        {
                            foreach (var workingDay in leaveResult.WorkingDays)
                            {
                                if ((workingDay.IsDefaultShift && workingDay.LeaveHours < leaveResult.DefaultShiftTime.ShiftTimeWorkingHourNumber)
                                || workingDay.IsDefaultShift == false && workingDay.LeaveHours < workingDay.Shift.ShiftTimeWorkingHourNumber)
                                {
                                    continue;
                                }

                                DateTime dateToGetShift = workingDay.Date;
                                dateToGetShift = (1 <= dateToGetShift.Day && dateToGetShift.Day <= StringConstant.EndDayNumber) ? workingDay.Date : (workingDay.Date.AddMonths(1));
                                int month = dateToGetShift.Month;
                                int year = dateToGetShift.Year;

                                var shiftList = _shiftManagementDAL.GetByMonthYearDepartment(month, year, requesterInfo.Department.LookupId, requesterInfo.FactoryLocation.LookupId);
                                if (shiftList.Any())
                                {
                                    int shiftDetailIdToUpdate = 0;
                                    var shiftIdList = shiftList.Select(x => x.ID).Distinct().ToList();
                                    foreach (var itemId in shiftIdList)
                                    {
                                        var shiftDetailItem = _shiftManagementDetailDAL.GetByShiftManagementIDEmployeeID(itemId, requesterInfo.ID).FirstOrDefault();

                                        if (shiftDetailItem != null)
                                        {
                                            shiftDetailIdToUpdate = shiftDetailItem.ID;
                                            break;
                                        }
                                    }

                                    if (shiftDetailIdToUpdate > 0)
                                    {
                                        var columName = string.Format("ShiftTime{0}", workingDay.Date.Day);
                                        _shiftManagementDetailDAL.UpdateLeaveValue(shiftDetailIdToUpdate, columName, toShiftId);
                                    }
                                }
                            }
                        }
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Leave Management DAL - UpdateShift fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public LeaveResult InitLeaveInfo(EmployeeInfo requestForInfo, string fromDate, string toDate)
        {
            LeaveResult leaveResult = new LeaveResult(requestForInfo.ID.ToString(), fromDate, toDate);
            bool isSecurity = _additionalEmployeePositionDAL.GetAdditionalPosition(requestForInfo.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);
            leaveResult = ClassifySelectedDays(leaveResult, requestForInfo, isSecurity);

            List<Biz.Models.ShiftTime> shiftTimeCollection = _shiftTimeDAL.GetAll();
            leaveResult = BuildDefaultShiftTime(leaveResult, _shiftTimeDAL, shiftTimeCollection, requestForInfo);

            leaveResult = GetWorkingDayDetails(leaveResult, requestForInfo, isSecurity, shiftTimeCollection);
            leaveResult = CalculateTotalHoursAndDays(leaveResult, shiftTimeCollection, requestForInfo);

            return leaveResult;
        }

        public LeaveManagement RunWorkFlow(LeaveManagement leaveManagement, TaskManagement taskOfPrevStep)
        {
            if (leaveManagement == null) return null;

            var taskManagement = new TaskManagement();
            taskManagement.ItemId = leaveManagement.ID;
            taskManagement.ItemURL = taskOfPrevStep.ItemURL;
            taskManagement.ListURL = taskOfPrevStep.ListURL;
            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = leaveManagement.RequestDueDate;
            taskManagement.PercentComplete = 0;
            taskManagement.TaskName = taskOfPrevStep.TaskName;
            taskManagement.StepModule = taskOfPrevStep.StepModule;
            taskManagement.Department = taskOfPrevStep.Department;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.AssignedTo = taskOfPrevStep.NextAssign;
            taskManagement.NextAssign = null;

            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            var nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.LeaveManagement, taskOfPrevStep.Department.LookupId);
            if (nextStep != null)
            {
                taskManagement.StepStatus = nextStep.StepStatus;

                if (nextStep != null)
                {
                    ModuleBuilder moduleBuilder = new ModuleBuilder(this.SiteUrl);
                    var nextAssign = moduleBuilder.GetNextApproval(taskOfPrevStep.Department.LookupId, leaveManagement.Location.LookupId, StepModuleList.LeaveManagement, nextStep.StepNumber + 1);
                    if (nextAssign != null)
                    {
                        switch (nextStep.StepNumber + 1)
                        {
                            case 2:
                                if (leaveManagement.DH != null)
                                {
                                    taskManagement.NextAssign = leaveManagement.DH;
                                }
                                break;
                            case 3:
                                if (leaveManagement.BOD != null)
                                {
                                    taskManagement.NextAssign = leaveManagement.BOD;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            TaskManagementDAL taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            var taskId = taskManagementDAL.SaveItem(taskManagement);

            leaveManagement.ApprovalStatus = taskManagement.StepStatus;
            this.SaveOrUpdate(leaveManagement);

            if (leaveManagement != null)
            {
                var emailTemplate = _emailTemplateDAL.GetByKey("LeaveManagement_Request");
                var assignee = _employeeInfoDAL.GetByADAccount(taskManagement.AssignedTo.UserName);
                if (emailTemplate != null && assignee != null)
                {
                    this.SendRequestEmail(leaveManagement, emailTemplate, new List<EmployeeInfo>() { assignee }, this.SiteUrl);

                    try
                    {
                        List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(assignee.ID, StringConstant.LeaveManagementList.ListUrl, leaveManagement.ID);
                        this.SendDelegationRequestEmail(leaveManagement, emailTemplate, toUsers, this.SiteUrl);
                    }
                    catch { }
                }
            }

            return leaveManagement;
        }

        public List<LeaveManagement> GetLeaves(DateTime fromDate, DateTime toDate, int deptId, List<int> locationIds, string[] viewFields)
        {
            List<LeaveManagement> queriedLeaves = null;

            try
            {
                string queryStr = $@"<And>
                                        <Geq>
                                            <FieldRef Name='{StringConstant.LeaveManagementList.ToField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Geq>
                                        <Leq>
                                            <FieldRef Name='{StringConstant.LeaveManagementList.FromField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Leq>
                                    </And>";

                var approvalFilter = $@"<Eq>
                                            <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                            <Value Type='Text'>{StringConstant.ApprovalStatus.Approved}</Value>
                                        </Eq>";

                if (deptId > 0)
                {
                    queryStr = $@"<And>
                                    {queryStr}
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.CommonSPListField.CommonDepartmentField}' LookupId='TRUE' />
                                            <Value Type='Lookup'>{deptId}</Value>
                                        </Eq>
                                        {approvalFilter}
                                    </And>
                                </And>";
                }
                else
                {
                    queryStr = string.Format(@"<And>{0}{1}</And>", queryStr, approvalFilter);
                }

                var locationFilter = CommonHelper.BuildFilterCommonLocation(locationIds);
                if (!string.IsNullOrEmpty(locationFilter))
                {
                    queryStr = $@"<And>{queryStr}{locationFilter}</And>";
                }

                queryStr = string.Format("<Where>{0}</Where><OrderBy><FieldRef Name='CommonDepartment' Ascending='True' /><FieldRef Name='RequestFor' Ascending='True' /></OrderBy>", queryStr);

                SPQuery spQuery = new SPQuery();
                spQuery.Query = queryStr;

                queriedLeaves = GetByQuery(spQuery, viewFields);
            }
            catch { }

            return queriedLeaves;
        }

        public string BuildLeaveHistoryQuery(string employeeLookupId, DateTime fromDate, DateTime toDate, bool includeWhereClause)
        {
            string ret = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";

            if (fromDate <= toDate)
            {
                ret = string.Format($@"<And>
                                <And>
                                    <Eq>
                                        <FieldRef Name='{StringConstant.LeaveManagementList.RequestForField}' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{0}</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                        <Value Type='Text'>{StringConstant.ApprovalStatus.Approved}</Value>
                                    </Eq>
                                </And>
                                <And>
                                    <Leq>
                                        <FieldRef Name='{StringConstant.LeaveManagementList.FromField}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                    </Leq>
                                    <Geq>
                                        <FieldRef Name='{StringConstant.LeaveManagementList.ToField}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{2}</Value>
                                    </Geq>
                                </And>
                            </And>", employeeLookupId, toDate.ToString(StringConstant.DateFormatTZForCAML), fromDate.ToString(StringConstant.DateFormatTZForCAML));
            }

            if (includeWhereClause) ret = string.Format("<Where>{0}</Where>", ret);

            return ret;
        }

        #region Delegation
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> listOfTasks = new List<Delegation>();

            List<string> viewFields = new List<string>() { };
            viewFields.Add(StringConstant.CommonSPListField.RequesterField);
            viewFields.Add(StringConstant.CommonSPListField.CommonDepartmentField);
            viewFields.Add(StringConstant.LeaveManagementList.RequestForField);
            viewFields.Add(StringConstant.LeaveManagementList.FromField);
            viewFields.Add(StringConstant.LeaveManagementList.ToField);
            viewFields.Add(StringConstant.DefaultSPListField.CreatedField);
            List<LeaveManagement> itemCollection = this.GetByQuery(this.BuildQueryGetListOfTasks(fromEmployee), viewFields.ToArray());
            if (itemCollection != null)
            {
                foreach (var item in itemCollection)
                {
                    Delegation delegation = new Delegation(item);
                    listOfTasks.Add(delegation);
                }
            }

            return listOfTasks;
        }

        private string BuildQueryGetListOfTasks(EmployeeInfo employeeInfo)
        {
            string filterStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";

            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            string taskQueryStr = string.Format(@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Status' />
                                        <Value Type='Choice'>{0}</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{1}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                            <Value Type='User'>{2}</Value>
                                        </Eq>
                                     </And>
                                  </And>
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.LeaveManagement.ToString(), employeeInfo.ADAccount.ID);
            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                List<int> itemIds = taskManagementCollection.Where(t => t.ItemId > 0).Select(t => t.ItemId).ToList();

                if (itemIds != null && itemIds.Count > 0)
                {
                    filterStr = "";
                    foreach (var itemId in itemIds)
                    {
                        filterStr += string.Format("<Value Type = 'Number'>{0}</Value>", itemId);
                    }
                    if (!string.IsNullOrEmpty(filterStr))
                    {
                        filterStr = string.Format("<In><FieldRef Name = 'ID'/><Values>{0}</Values></In>", filterStr);
                    }
                }
            }

            filterStr = string.Format("<Where>{0}</Where>", filterStr);

            return filterStr;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            LeaveManagement leaveManagement = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(leaveManagement, currentWeb);
            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem ret = null;

            LeaveManagement leaveManagement = this.ParseToEntity(listItem);

            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            IList<TaskManagement> taskManagementCollection = _taskManagementDAL.GetRelatedTasks(leaveManagement.ID, StepModuleList.LeaveManagement.ToString());
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                var assignee = taskManagementCollection[0].AssignedTo;
                if (assignee != null)
                {
                    var currentStepApprover = _employeeInfoDAL.GetByADAccount(assignee.ID);
                    if (currentStepApprover != null)
                    {
                        ret = new LookupItem() { LookupId = currentStepApprover.ID, LookupValue = currentStepApprover.FullName };
                    }
                }
            }

            return ret;
        }

        public bool IsValidTask(int listItemID)
        {
            return true;
        }

        public LeaveManagement SetDueDate(LeaveManagement leaveManagement)
        {
            DateTime reqDueDate = leaveManagement.From.Date;
            //if (reqDueDate == DateTime.Now.Date)
            //{
            //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            //else
            //{
            //    double totalDays = leaveManagement.TotalDays;

            //    if (totalDays > 0 && totalDays < 3) // 1 <= workingDays <= 2
            //    {
            //        if (reqDueDate.AddDays(-1) < DateTime.Now.Date)
            //        {
            //            reqDueDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            //        }
            //        else
            //        {
            //            reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            //        }

            //    }
            //    else if (totalDays >= 3 && totalDays < 5) // 3 <= workingDays <= 4
            //    {
            //        if (reqDueDate.AddDays(-3) < DateTime.Now.Date)
            //        {
            //            reqDueDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            //        }
            //        else
            //        {
            //            reqDueDate = reqDueDate.AddDays(-3).AddHours(23).AddMinutes(59).AddSeconds(59);
            //        }
            //    }
            //    else if (totalDays >= 5) // workingDays >= 5
            //    {
            //        if (reqDueDate.AddDays(-15) < DateTime.Now.Date)
            //        {
            //            reqDueDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            //        }
            //        else
            //        {
            //            reqDueDate = reqDueDate.AddDays(-15).AddHours(23).AddMinutes(59).AddSeconds(59);
            //        }
            //    }
            //}
            reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            leaveManagement.RequestDueDate = reqDueDate;

            return leaveManagement;
        }
        #endregion

        #region Email

        private string GetEmailLinkByUserPosition(string webUrl, int position, int itemId, bool isApprovalLink)
        {
            string link = string.Empty;
            string approvalLinkFormat = "{0}/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId={1}&Source={2}";

            switch (position)
            {
                case (int)StringConstant.EmployeePosition.TeamLeader:
                case (int)StringConstant.EmployeePosition.ShiftLeader:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveManagementMember}";
                    break;
                case (int)StringConstant.EmployeePosition.Administrator:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveManagementAdmin}";
                    break;
                case (int)StringConstant.EmployeePosition.DepartmentHead:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveManagementManager}";
                    break;
                case (int)StringConstant.EmployeePosition.BOD:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveManagementBOD}";
                    break;
                default:
                    link = $"{webUrl}/{StringConstant.WebPageLinks.LeaveManagementMember}";
                    break;
            }

            if (isApprovalLink)
            {
                link = string.Format(approvalLinkFormat, webUrl, itemId, HttpUtility.UrlEncode(link + "#tab2"));
            }
            else
            {
                link = string.Format(approvalLinkFormat, webUrl, itemId, HttpUtility.UrlEncode(link));
            }

            return link;
        }

        public void SendTransferWorkToEmail(LeaveManagement leaveItem, EmailTemplate emailTemplate, EmployeeInfo toUser, string webUrl)
        {
            if (toUser == null || string.IsNullOrEmpty(toUser.Email) || emailTemplate == null || leaveItem == null || string.IsNullOrEmpty(webUrl))
                return;
            var content = HTTPUtility.HtmlDecode(emailTemplate.MailBody);

            content = content.Replace("{0}", toUser.FullName);
            content = content.Replace("{1}", leaveItem.RequestFor.LookupValue);
            content = content.Replace("{2}", leaveItem.From.ToString(StringConstant.DateFormatddMMyyyyHHmm));
            content = content.Replace("{3}", leaveItem.To.ToString(StringConstant.DateFormatddMMyyyyHHmm));
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            sendMailActivity.SendMail(webUrl, emailTemplate.MailSubject, toUser.Email, true, false, content);
        }

        public void SendRequestEmail(LeaveManagement leaveItem, EmailTemplate emailTemplate, List<EmployeeInfo> toUsers, string webUrl)
        {
            if (toUsers == null || toUsers.Count == 0 || emailTemplate == null || leaveItem == null || string.IsNullOrEmpty(webUrl))
                return;

            var department = DepartmentListSingleton.GetDepartmentByID(leaveItem.Department.LookupId, webUrl);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            foreach (var toUser in toUsers)
            {
                try
                {
                    if (!string.IsNullOrEmpty(toUser.Email))
                    {
                        var content = HTTPUtility.HtmlDecode(emailTemplate.MailBody);
                        content = content.Replace("{0}", toUser.FullName);
                        content = content.Replace("{1}", leaveItem.Requester.LookupValue);
                        content = content.Replace("{2}", leaveItem.RequestFor.LookupValue);
                        content = content.Replace("{3}", leaveItem.From.ToString(StringConstant.DateFormatddMMyyyyHHmm));
                        content = content.Replace("{4}", leaveItem.To.ToString(StringConstant.DateFormatddMMyyyyHHmm));
                        content = content.Replace("{5}", Convert.ToString(leaveItem.LeaveHours));
                        content = content.Replace("{6}", department.Name);
                        content = content.Replace("{7}", department.VietnameseName);
                        var link = GetEmailLinkByUserPosition(webUrl, toUser.EmployeePosition.LookupId, leaveItem.ID, true);
                        content = content.Replace("#link", link);
                        sendMailActivity.SendMail(webUrl, emailTemplate.MailSubject, toUser.Email, true, false, content);
                    }
                }
                catch { }
            }
        }

        public void SendDelegationRequestEmail(LeaveManagement leaveItem, EmailTemplate emailTemplate, List<EmployeeInfo> toUsers, string webUrl)
        {
            if (toUsers == null || toUsers.Count == 0 || emailTemplate == null || leaveItem == null || string.IsNullOrEmpty(webUrl))
                return;

            SendEmailActivity sendMailActivity = new SendEmailActivity();
            var link = string.Format(@"{0}/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", webUrl, leaveItem.ID);
            var department = DepartmentListSingleton.GetDepartmentByID(leaveItem.Department.LookupId, webUrl);
            foreach (var toUser in toUsers)
            {
                try
                {
                    if (!string.IsNullOrEmpty(toUser.Email))
                    {
                        var content = HTTPUtility.HtmlDecode(emailTemplate.MailBody);

                        content = content.Replace("{0}", toUser.FullName);
                        content = content.Replace("{1}", leaveItem.Requester.LookupValue);
                        content = content.Replace("{2}", leaveItem.RequestFor.LookupValue);
                        content = content.Replace("{3}", leaveItem.From.ToString(StringConstant.DateFormatddMMyyyyHHmm));
                        content = content.Replace("{4}", leaveItem.To.ToString(StringConstant.DateFormatddMMyyyyHHmm));
                        content = content.Replace("{5}", Convert.ToString(leaveItem.LeaveHours));
                        content = content.Replace("{6}", department.Name);
                        content = content.Replace("{7}", department.VietnameseName);
                        content = content.Replace("#link", link);
                        sendMailActivity.SendMail(webUrl, emailTemplate.MailSubject, toUser.Email, true, false, content);
                    }
                }
                catch { }
            }
        }

        public void SendApproveMail(LeaveManagement leaveItem, EmailTemplate approveEmailTemplate, EmployeeInfo approver, EmployeeInfo toUser, string webUrl)
        {
            if (toUser == null || string.IsNullOrEmpty(toUser.Email) || approveEmailTemplate == null || leaveItem == null || string.IsNullOrEmpty(webUrl))
                return;
            var content = HTTPUtility.HtmlDecode(approveEmailTemplate.MailBody);
            var department = DepartmentListSingleton.GetDepartmentByID(leaveItem.Department.LookupId, webUrl);
            content = content.Replace("{0}", toUser.FullName);
            content = content.Replace("{1}", approver.FullName);
            content = content.Replace("{2}", leaveItem.Requester.LookupValue);
            content = content.Replace("{3}", leaveItem.RequestFor.LookupValue);
            content = content.Replace("{4}", leaveItem.From.ToString(StringConstant.DateFormatddMMyyyyHHmm));
            content = content.Replace("{5}", leaveItem.To.ToString(StringConstant.DateFormatddMMyyyyHHmm));
            content = content.Replace("{6}", Convert.ToString(leaveItem.LeaveHours));
            content = content.Replace("{7}", department.Name);
            content = content.Replace("{8}", department.VietnameseName);
            var link = GetEmailLinkByUserPosition(webUrl, toUser.EmployeePosition.LookupId, leaveItem.ID, false);
            content = content.Replace("#link", link);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            sendMailActivity.SendMail(webUrl, approveEmailTemplate.MailSubject, toUser.Email, true, false, content);
        }

        public void SendRejectMail(LeaveManagement leaveItem, EmailTemplate rejectMailTemplate, EmployeeInfo approver, EmployeeInfo toUser, string webUrl)
        {
            if (toUser == null || string.IsNullOrEmpty(toUser.Email) || rejectMailTemplate == null || leaveItem == null || string.IsNullOrEmpty(webUrl))
                return;
            var content = HTTPUtility.HtmlDecode(rejectMailTemplate.MailBody);
            var department = DepartmentListSingleton.GetDepartmentByID(leaveItem.Department.LookupId, webUrl);
            content = content.Replace("{0}", toUser.FullName);
            content = content.Replace("{1}", approver.FullName);
            content = content.Replace("{2}", leaveItem.Requester.LookupValue);
            content = content.Replace("{3}", leaveItem.RequestFor.LookupValue);
            content = content.Replace("{4}", leaveItem.From.ToString(StringConstant.DateFormatddMMyyyyHHmm));
            content = content.Replace("{5}", leaveItem.To.ToString(StringConstant.DateFormatddMMyyyyHHmm));
            content = content.Replace("{6}", Convert.ToString(leaveItem.LeaveHours));
            content = content.Replace("{7}", department.Name);
            content = content.Replace("{8}", department.VietnameseName);
            var link = GetEmailLinkByUserPosition(webUrl, toUser.EmployeePosition.LookupId, leaveItem.ID, false);
            content = content.Replace("#link", link);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            sendMailActivity.SendMail(webUrl, rejectMailTemplate.MailSubject, toUser.Email, true, false, content);
        }
        #endregion

        #region "Overview"

        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion

        #region Private Methods
        private double CalculateActualWorkingHourSecurity(DateTime fromWH, DateTime toWH, WorkingDay workingDay, Models.ShiftTime shiftOfWorkingDay)
        {
            double leaveHours24h = 0;

            int diffDays = shiftOfWorkingDay.WorkingHourToHour.Date.Subtract(shiftOfWorkingDay.WorkingHourFromHour.Date).Days;
            if (shiftOfWorkingDay.ShiftTimeBreakHourNumber > 0)
            {
                var breakingHourFrom = workingDay.Date.Date.AddTicks(shiftOfWorkingDay.BreakHourFromHour.TimeOfDay.Ticks);
                var diffBreakingDays = shiftOfWorkingDay.BreakHourToHour.Date.Subtract(shiftOfWorkingDay.BreakHourFromHour.Date).Days;
                var breakingHourTo = breakingHourFrom.AddDays(diffBreakingDays).Date.AddTicks(shiftOfWorkingDay.BreakHourToHour.TimeOfDay.Ticks);

                if ((fromWH >= breakingHourTo) || (toWH <= breakingHourFrom))
                {
                    leaveHours24h = (toWH - fromWH).TotalHours;
                }
                else if ((fromWH < breakingHourFrom) && (breakingHourFrom <= toWH && toWH <= breakingHourTo))
                {
                    leaveHours24h = (breakingHourFrom - fromWH).TotalHours;
                }
                else if ((breakingHourFrom <= fromWH && fromWH <= breakingHourTo) && (breakingHourTo < toWH))
                {
                    leaveHours24h = (toWH - breakingHourTo).TotalHours;
                }
                else if ((fromWH < breakingHourFrom && breakingHourTo < toWH))
                {
                    leaveHours24h = (toWH - fromWH).TotalHours - shiftOfWorkingDay.ShiftTimeBreakHourNumber;
                }
                else if ((breakingHourFrom <= fromWH && toWH <= breakingHourTo))
                {
                    leaveHours24h = 0;
                }
            }
            else
            {
                leaveHours24h = (toWH - fromWH).TotalHours;
            }

            if (leaveHours24h >= 8)
            {
                return 8;
            }
            else
            {
                return leaveHours24h;
            }
        }

        private Biz.Models.ShiftTime GetDefaultShiftTime(ShiftTimeDAL _shiftTimeDAL, List<Biz.Models.ShiftTime> shiftTimeCollection, EmployeeInfo employeeInfo)
        {
            string shiftCode = "";
            switch (employeeInfo.EmployeePosition.LookupId)
            {
                case (int)StringConstant.EmployeePosition.Gardener:
                    shiftCode = "CX";
                    break;
                case (int)StringConstant.EmployeePosition.Helper:
                    shiftCode = "TV";
                    break;
                default:
                    shiftCode = "HC";
                    break;
            }

            bool isSecurity = _additionalEmployeePositionDAL.GetAdditionalPosition(employeeInfo.ID, null, StringConstant.AdditionalEmployeePositionLevelCode.SecurityGuard);
            if (isSecurity == true)
            {
                shiftCode = "BV";
            }

            Biz.Models.ShiftTime defaultShiftTime = null;
            if (shiftTimeCollection != null && shiftTimeCollection.Count > 0)
            {
                defaultShiftTime = shiftTimeCollection.Where(e => e.Code.ToUpper() == shiftCode).FirstOrDefault();
            }

            if (defaultShiftTime == null)
            {
                defaultShiftTime = _shiftTimeDAL.GetShiftTimeByCode(shiftCode);
            }

            if (shiftCode == "HC")
            {
                DepartmentDAL _departmentDAL = new DepartmentDAL(this.SiteUrl);
                Department emp_dept = _departmentDAL.GetByID(employeeInfo.Department.LookupId);
                if (emp_dept != null)
                {
                    string specDeptCode = emp_dept.Code.ToUpper();
                    if (specDeptCode == "QA" || specDeptCode == "WH")
                    {
                        defaultShiftTime.BreakHourFromHour = defaultShiftTime.BreakHourFromHour.AddMinutes(-30);
                        defaultShiftTime.BreakHourToHour = defaultShiftTime.BreakHourToHour.AddMinutes(-30);
                    }
                }
            }

            return defaultShiftTime;
        }

        private string BuildChangeShiftManagementQuery(EmployeeInfo employeeInfo, DateTime fromDate, DateTime toDate)
        {
            string queryStr = string.Format(@"<Where>
                              <And>
                                 <Geq>
                                    <FieldRef Name='To' />
                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value>
                                 </Geq>
                                 <And>
                                    <Leq>
                                       <FieldRef Name='To' />
                                       <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                    </Leq>
                                    <And>
                                       <Eq>
                                          <FieldRef Name='ApprovalStatus' />
                                          <Value Type='Text'>Approved</Value>
                                       </Eq>
                                       <And>
                                          <Eq>
                                             <FieldRef Name='Requester' LookupId='TRUE'/>
                                             <Value Type='Lookup'>{2}</Value>
                                          </Eq>
                                          <Eq>
                                             <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                             <Value Type='Lookup'>{3}</Value>
                                          </Eq>
                                       </And>
                                    </And>
                                 </And>
                              </And>
                           </Where>", string.Format(retDateFormat, fromDate.Date), string.Format(retDateFormat, toDate.Date), employeeInfo.ID, employeeInfo.Department.LookupId);

            return queryStr;
        }

        private string BuildNotOverTimeManagementQuery(EmployeeInfo employeeInfo, DateTime fromDate, DateTime toDate)
        {
            string queryStr = string.Format(@" < Where>
                              <And>
                                 <Geq>
                                    <FieldRef Name='CommonDate' />
                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value>
                                 </Geq>
                                 <And>
                                    <Leq>
                                       <FieldRef Name='CommonDate' />
                                       <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                    </Leq>
                                    <And>
                                       <Eq>
                                          <FieldRef Name='ApprovalStatus' />
                                          <Value Type='Text'>Approved</Value>
                                       </Eq>
                                       <And>
                                          <Eq>
                                             <FieldRef Name='Requester' LookupId='TRUE'/>
                                             <Value Type='Lookup'>{2}</Value>
                                          </Eq>
                                          <Eq>
                                             <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                             <Value Type='Lookup'>{3}</Value>
                                          </Eq>
                                       </And>
                                    </And>
                                 </And>
                              </And>
                           </Where>", string.Format(retDateFormat, fromDate.Date), string.Format(retDateFormat, toDate.Date), employeeInfo.ID, employeeInfo.Department.LookupId);

            return queryStr;
        }

        private string BuildOverTimeManagementQuery(EmployeeInfo employeeInfo, DateTime fromDate, DateTime toDate)
        {
            string queryStr = string.Format(@"<Where>
                              <And>
                                 <Geq>
                                    <FieldRef Name='CommonDate' />
                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value>
                                 </Geq>
                                 <And>
                                    <Leq>
                                       <FieldRef Name='CommonDate' />
                                       <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                    </Leq>
                                    <And>
                                       <Eq>
                                          <FieldRef Name='ApprovalStatus' />
                                          <Value Type='Text'>true</Value>
                                       </Eq>
                                       <And>
                                          <Eq>
                                             <FieldRef Name='Requester' LookupId='TRUE' />
                                             <Value Type='Lookup'>{2}</Value>
                                          </Eq>
                                          <Eq>
                                             <FieldRef Name='CommonDepartment' LookupId='TRUE' />
                                             <Value Type='Lookup'>{3}</Value>
                                          </Eq>
                                       </And>
                                    </And>
                                 </And>
                              </And>
                           </Where>", string.Format(retDateFormat, fromDate.Date), string.Format(retDateFormat, toDate.Date), employeeInfo.ID, employeeInfo.Department.LookupId);

            return queryStr;
        }

        private string BuildOverlapQuery(EmployeeInfo employeeInfo, DateTime fromDate, DateTime toDate, bool isRequestFor = false)
        {
            string queryStr = string.Format(@"<Where>
                <And>
                    <Eq>
                        <FieldRef Name='RequestFor' LookupId='TRUE'/>
                        <Value Type='Lookup'>{0}</Value>
                    </Eq>
                    <And>
                        <Leq>
                            <FieldRef Name='CommonFrom' />
                            <Value IncludeTimeValue='TRUE' Type='DateTime'>{1}</Value>
                        </Leq>
                        <And>
                            <Geq>
                                <FieldRef Name='To' />
                                <Value IncludeTimeValue='TRUE' Type='DateTime'>{2}</Value>
                            </Geq>
                            <And>
                                <Neq>
                                    <FieldRef Name='ApprovalStatus' />
                                    <Value Type='Text'>{3}</Value>
                                </Neq>
                                <Neq>
                                    <FieldRef Name='ApprovalStatus' />
                                    <Value Type='Text'>{4}</Value>
                                </Neq>
                            </And>
                        </And>
                    </And>
                </And>
            </Where>", employeeInfo.ID, toDate.ToString(StringConstant.DateFormatTZForCAML), fromDate.ToString(StringConstant.DateFormatTZForCAML),
                    StringConstant.ApprovalStatus.Cancelled.ToString(), StringConstant.ApprovalStatus.Rejected.ToString());

            if (isRequestFor == true)
            {
                queryStr = string.Format(@"<Where>
                <And>
                    <Eq>
                        <FieldRef Name='RequestFor' LookupId='TRUE'/>
                        <Value Type='Lookup'>{0}</Value>
                    </Eq>
                    <And>
                        <Leq>
                            <FieldRef Name='CommonFrom' />
                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                        </Leq>
                        <And>
                            <Geq>
                                <FieldRef Name='To' />
                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{2}</Value>
                            </Geq>
                            <And>
                                <Neq>
                                    <FieldRef Name='ApprovalStatus' />
                                    <Value Type='Text'>{3}</Value>
                                </Neq>
                                <Neq>
                                    <FieldRef Name='ApprovalStatus' />
                                    <Value Type='Text'>{4}</Value>
                                </Neq>
                            </And>
                        </And>
                    </And>
                </And>
            </Where>", employeeInfo.ID, toDate.ToString(StringConstant.DateFormatTZForCAML), fromDate.ToString(StringConstant.DateFormatTZForCAML),
                    StringConstant.ApprovalStatus.Cancelled.ToString(), StringConstant.ApprovalStatus.Rejected.ToString());
            }

            return queryStr;
        }

        private Collection<DateTime> TransformRangeOfDate(DateTime fromDate, DateTime toDate)
        {
            Collection<DateTime> ret = new Collection<DateTime>();

            int idx = 0;

            while (fromDate.Date.AddDays(idx) <= toDate.Date)
            {
                ret.Add(fromDate.AddDays(idx));
                idx++;
            }

            if (ret.Count > 1)
            {
                ret[0] = fromDate;
                ret[ret.Count - 1] = toDate;
            }

            return ret;
        }

        private bool IsManager(int employeeId)
        {
            var currentEmployee = _employeeInfoDAL.GetByID(Convert.ToInt16(employeeId));
            var positionId = currentEmployee.EmployeePosition.LookupId;
            return positionId == (int)StringConstant.EmployeePosition.BOD || positionId == (int)StringConstant.EmployeePosition.DepartmentHead
                || positionId == (int)StringConstant.EmployeePosition.DirectManagement; //positionId == (int)StringConstant.EmployeePosition.GroupLeader || 
        }

        private TaskManagement BuildTaskManagement(LeaveManagement leaveManagement, SPList leaveList, int sourceItemId, User additionalUser, User nextAssign, string stepStatus)
        {
            TaskManagement taskManagement = new TaskManagement();

            taskManagement.Department = leaveManagement.Department;
            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = leaveManagement.RequestDueDate;
            taskManagement.ItemId = leaveManagement.ID;
            taskManagement.ItemURL = leaveList.DefaultDisplayFormUrl + "?ID=" + sourceItemId;
            taskManagement.ListURL = leaveList.DefaultViewUrl;
            taskManagement.PercentComplete = 0;
            taskManagement.TaskName = TASK_NAME;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.StepModule = StepModuleList.LeaveManagement.ToString();

            taskManagement.StepStatus = stepStatus;
            taskManagement.AssignedTo = additionalUser;
            if (nextAssign != null)
            {
                taskManagement.NextAssign = nextAssign;
            }

            return taskManagement;
        }

        private Biz.Models.Calendar GetHolidayInfo(string dateStr, string location)
        {
            Biz.Models.Calendar calendar = null;
            try
            {
                var fromDateValues = dateStr.Split('-'); // mm-dd-yyyy
                var fromDateValue = new DateTime(Convert.ToInt32(fromDateValues[2]), Convert.ToInt32(fromDateValues[0]), Convert.ToInt32(fromDateValues[1]), 0, 0, 0);

                if (fromDateValue != DateTime.MinValue)
                {
                    List<string> holidays = new List<string>() { StringConstant.CaledarCategory.Holiday, StringConstant.CaledarCategory.Weekend, StringConstant.CaledarCategory.CompensationDayOff };

                    if (location.Equals("1"))
                    {
                        calendar = _calendarDAL.GetByDateAndCategories(fromDateValue, holidays);
                    }
                    else if (location.Equals("2"))
                    {
                        calendar = _calendar2DAL.GetByDateAndCategories(fromDateValue, holidays);
                    }
                }
            }
            catch { }

            return calendar;
        }
        #endregion

    }
}
