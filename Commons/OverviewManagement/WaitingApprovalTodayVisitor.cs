using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.OverviewManagement
{
    public class WaitingApprovalTodayVisitor : IFilterTaskVisitor
    {
        #region "Properties"
        private int _currentUserADId;
        private int _currentUserInfoId;
        private string _siteUrl;
        public IList<Delegation> DelegationList { get; set; }

        #region [Chart]
        public int TotalCount { get; set; }
        public bool CountOnly { get; set; }
        #endregion

        #region [Grid Details]
        public IList<FilterTask> FilterTaskList { get; set; }
        private const int ApprovalStatusId = 1; // In - Progress
        #endregion

        private FilterTaskManager _filterTaskManager;
        private ShiftManagementDetailDAL _shiftManagementDetailDAL;

        #endregion

        #region "Constructor"
        public WaitingApprovalTodayVisitor(int currentUserADId, int currentUserInfoId, string siteUrl)
        {
            FilterTaskList = new List<FilterTask>();
            _currentUserADId = currentUserADId;
            _currentUserInfoId = currentUserInfoId;
            _siteUrl = siteUrl;
            _filterTaskManager = new FilterTaskManager(siteUrl);
            _shiftManagementDetailDAL = new ShiftManagementDetailDAL(siteUrl);

        }
        #endregion

        #region "Visitor Implementation"
        public void Visit(ShiftManagementDAL shiftManagementDAL)
        {
            GetShiftTaskList(shiftManagementDAL);
        }

        public void Visit(ChangeShiftManagementDAL changeShiftManagementDAL)
        {
            GetChangeShiftTaskList(changeShiftManagementDAL);
        }

        public void Visit(OverTimeManagementDAL overTimeManagementDAL)
        {
            GetOvertimeTaskList(overTimeManagementDAL);
        }

        public void Visit(NotOvertimeManagementDAL notOvertimeManagementDAL)
        {
            GetNotOvertimeTaskList(notOvertimeManagementDAL);
        }

        public void Visit(LeaveManagementDAL leaveManagementDAL)
        {
            GetLeaveTaskList(leaveManagementDAL);
        }

        public void Visit(VehicleManagementDAL vehicleManagementDAL)
        {
            GetVehicleTaskList(vehicleManagementDAL);
        }

        public void Visit(FreightManagementDAL freightManagementDAL)
        {
            GetFreightTaskList(freightManagementDAL);
        }

        public void Visit(BusinessTripManagementDAL businessTripManagementDAL)
        {
            GetBusinessTripTaskList(businessTripManagementDAL);
        }

        public void Visit(RequestsDAL requestDAL)
        {
            GetRequestTaskList(requestDAL);
        }

        public void Visit(EmployeeRequirementSheetDAL recruitmentDAL)
        {
            GetRecruitmentTaskList(recruitmentDAL);
        }

        public void Visit(RequestForDiplomaSupplyDAL certificateDAL)
        {
            // Do nothing
        }

        public void Visit(RequisitionOfMeetingRoomDAL requisitionOfMeetingRoomDAL)
        {
            GetMeetingRoomTaskList(requisitionOfMeetingRoomDAL);
        }

        public void Visit(GuestReceptionManagementDAL guestReceptionManagementDAL)
        {
            GetGuestReceptionTaskList(guestReceptionManagementDAL);
        }
        #endregion

        #region "Private Methods"
        private void GetShiftTaskList(ShiftManagementDAL shiftManagementDAL)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == ShiftManagementList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, ShiftManagementList.ApprovedByField, "User", delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    {delegationQuery}
                                    <Or>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{ShiftManagementList.YearField}' />
                                                <Value Type='Number'>{year}</Value>
                                            </Eq>
                                            <Geq>
                                                <FieldRef Name='{ShiftManagementList.MonthField}' />
                                                <Value Type='Number'>{month}</Value>
                                            </Geq>
                                        </And>
                                        <Gt>
                                            <FieldRef Name='{ShiftManagementList.YearField}' />
                                            <Value Type='Number'>{year}</Value>
                                        </Gt>
                                    </Or>
                                </And>
                            </Where>";

            var shiftManagementList = shiftManagementDAL.GetByQuery(query);
            if (shiftManagementList != null && shiftManagementList.Count > 0)
            {
                var isValid = false;
                Type typeShiftManagementDetail = typeof(ShiftManagementDetail);
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                foreach (var shiftManagement in shiftManagementList)
                {
                    isValid = true;
                    if (shiftManagement.Year == year)
                    {
                        if (shiftManagement.Month == month) // VALID: Current Day <= 20 
                        {
                            if (day > 20)
                                isValid = false;
                        }
                        else if (shiftManagement.Month - 1 == month) // VALID: >=21. Shif Thang 12: 21/11 -> 20/12 -> Thang hien tai 11: >= 21/11
                        {
                            if (day < 21)
                                isValid = false;
                        }
                    }
                    //else if (year == shiftManagement.Year - 1)
                    //{
                    //    if (month == 12) // 21/12/2016 -> 20/1/2017: Shift Thang 1/2017 0> Thang hien tai: 12/2016
                    //        if (day < 21)
                    //            isValid = false;
                    //}

                    if (isValid)
                    {
                        var isApproved = true;
                        var shiftManagementDetails = _shiftManagementDetailDAL.GetByShiftManagementID(shiftManagement.ID);
                        if (shiftManagementDetails != null && shiftManagementDetails.Any())
                        {
                            foreach (var shiftManagementDetail in shiftManagementDetails)
                            {
                                if (!isApproved) break;

                                for (int i = 1; i <= 31; i++)
                                {
                                    PropertyInfo shiftTimeInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", i), bindingFlags);
                                    object shiftTimeValue = shiftTimeInfo.GetValue(shiftManagementDetail, null);

                                    if (shiftTimeValue != null)
                                    {
                                        LookupItem shiftTimeValueObj = shiftTimeValue as LookupItem;
                                        if (!string.IsNullOrEmpty(shiftTimeValueObj.LookupValue))
                                        {
                                            PropertyInfo shiftTimeApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", i), bindingFlags);
                                            object shiftTimeApprovalValue = shiftTimeApprovalInfo.GetValue(shiftManagementDetail, null);

                                            if (shiftTimeApprovalValue != null && Convert.ToBoolean(shiftTimeApprovalValue) == false)
                                            {
                                                isApproved = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!isApproved)
                        {
                            var filterTask = new FilterTask(shiftManagement);
                            filterTask.ApprovalStatusId = ApprovalStatusId;
                            FilterTaskList.Add(filterTask);

                            // Count tasks
                            this.TotalCount++;
                        }
                    }
                }
            }
        }
        private void GetChangeShiftTaskList(ChangeShiftManagementDAL changeShiftManagementDAL)
        {
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == ChangeShiftList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, ChangeShiftList.DHField, "User", delegatedItemIDs);
            var query = $@"<Where>
                                <And>
                                    <And>
	                                    {delegationQuery}
                                        <Or>
                                            <Eq>
                                               <FieldRef Name='{StringConstant.ChangeShiftList.FromDateField}' />
                                               <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Eq>
                                            <Eq>
                                               <FieldRef Name='{StringConstant.ChangeShiftList.FromDateField}' />
                                               <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Eq>
                                        </Or>
                                    </And>
	                                <IsNull>
		                                <FieldRef Name='{StringConstant.ChangeShiftList.AprovalStatusField}' />
	                                </IsNull>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += changeShiftManagementDAL.CountByQuery(query);
            }
            else
            {
                var changeShiftManagementList = changeShiftManagementDAL.GetByQuery(query);
                if (changeShiftManagementList != null)
                {
                    foreach (var changeShiftManagement in changeShiftManagementList)
                    {
                        var filterTask = new FilterTask(changeShiftManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetOvertimeTaskList(OverTimeManagementDAL overTimeManagementDAL)
        {
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == OverTimeManagementList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, OverTimeManagementList.ApprovedByField, "User", delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <And>
                                        {delegationQuery}
                                        <Geq>
                                           <FieldRef Name='{StringConstant.OverTimeManagementList.CommonDateField}' />
                                           <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Geq>
                                    </And>
	                                
	                                <IsNull>
		                                <FieldRef Name='{StringConstant.OverTimeManagementList.ApprovalStatusField}' />
	                                </IsNull>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += overTimeManagementDAL.CountByQuery(query);
            }
            else
            {
                var overtimeManagementList = overTimeManagementDAL.GetByQuery(query);
                if (overtimeManagementList != null && overtimeManagementList.Count > 0)
                {
                    foreach (var overtimeManagement in overtimeManagementList)
                    {
                        var filterTask = new FilterTask(overtimeManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetNotOvertimeTaskList(NotOvertimeManagementDAL notOvertimeManagementDAL)
        {
            var dueDate = DateTime.Now.AddDays(1);

            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == NotOvertimeList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, NotOvertimeList.DHField, "User", delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <And>
	                                    {delegationQuery}
                                        <Or>
                                            <Eq>
                                               <FieldRef Name='{StringConstant.NotOvertimeList.DateField}' />
                                               <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Eq>
                                            <Eq>
                                               <FieldRef Name='{StringConstant.NotOvertimeList.DateField}' />
                                               <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Eq>
                                        </Or>
                                    </And>
	                                <IsNull>
		                                <FieldRef Name='{StringConstant.NotOvertimeList.AprovalStatusField}' />
	                                </IsNull>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += notOvertimeManagementDAL.CountByQuery(query);
            }
            else
            {
                var notOvertimeManagementList = notOvertimeManagementDAL.GetByQuery(query);

                if (notOvertimeManagementList != null)
                {
                    foreach (var notOvertimeManagement in notOvertimeManagementList)
                    {
                        var filterTask = new FilterTask(notOvertimeManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetLeaveTaskList(LeaveManagementDAL leaveManagementDAL)
        {
            // DUE Date = NOW / NOW + 1;
            // NOW <= FROM AND NOW >= DUE Date
            var taskListQuery = _filterTaskManager.BuildTaskListQuery(StepModuleList.LeaveManagement.ToString(), TaskStatusList.InProgress.ToString(), _currentUserADId);
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == LeaveManagementList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, string.Empty, string.Empty, delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <Or>
                                        {taskListQuery}
                                        {delegationQuery}
                                    </Or>
                                    <Or>
                                        <Or>
                                            <And>
                                                <And>
                                                    <Gt>
                                                        <FieldRef Name='{LeaveManagementList.TotalDaysField}' />
                                                        <Value Type='Number'>0</Value>
                                                    </Gt>
                                                    <Lt>
                                                        <FieldRef Name='{LeaveManagementList.TotalDaysField}' />
                                                        <Value Type='Number'>3</Value>
                                                    </Lt>
                                                </And>
                                                <Or>
                                                    <Eq>
                                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                </Or>
                                            </And>
                                            <And>
                                                <And>
                                                    <Geq>
                                                        <FieldRef Name='{LeaveManagementList.TotalDaysField}' />
                                                        <Value Type='Number'>3</Value>
                                                    </Geq>
                                                    <Lt>
                                                        <FieldRef Name='{LeaveManagementList.TotalDaysField}' />
                                                        <Value Type='Number'>5</Value>
                                                    </Lt>
                                                </And>
                                                <Or>
                                                    <Or>
                                                        <Eq>
                                                            <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(3).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                        </Eq>
                                                        <Eq>
                                                            <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(4).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                        </Eq>
                                                    </Or>
                                                    <And>
                                                        <Geq>
                                                            <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                        </Geq>
                                                        <Lt>
                                                            <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(3).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                        </Lt>
                                                    </And>
                                                </Or>
                                            </And>
                                        </Or>
                                        <And>
                                            <Geq>
                                                <FieldRef Name='{LeaveManagementList.TotalDaysField}' />
                                                <Value Type='Number'>5</Value>
                                            </Geq>
                                            <Or>
                                                <Or>
                                                    <Eq>
                                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(15).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(16).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Eq>
                                                </Or>
                                                <And>
                                                    <Geq>
                                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Geq>
                                                    <Lt>
                                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(15).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                    </Lt>
                                                </And>
                                            </Or>
                                        </And>
                                    </Or>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += leaveManagementDAL.CountByQuery(query);
            }
            else
            {
                var leaveManagementList = leaveManagementDAL.GetByQuery(query);

                if (leaveManagementList != null)
                {
                    foreach (var leaveManagement in leaveManagementList)
                    {
                        var filterTask = new FilterTask(leaveManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetVehicleTaskList(VehicleManagementDAL vehicleManagementDAL)
        {
            var taskListQuery = _filterTaskManager.BuildTaskListQuery(StepModuleList.VehicleManagement.ToString(), TaskStatusList.InProgress.ToString(), _currentUserADId);
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == VehicleManagementList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, string.Empty, string.Empty, delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <Or>
                                        {taskListQuery}
                                        {delegationQuery}
                                    </Or>
                                    <Or>
                                        <Eq>
                                            <FieldRef Name='{VehicleManagementList.CommonFrom}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{VehicleManagementList.CommonFrom}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                    </Or>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += vehicleManagementDAL.CountByQuery(query);
            }
            else
            {
                var vehicleManagementList = vehicleManagementDAL.GetByQuery(query);

                if (vehicleManagementList != null)
                {
                    foreach (var vehicleManagement in vehicleManagementList)
                    {
                        var filterTask = new FilterTask(vehicleManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetFreightTaskList(FreightManagementDAL freightManagementDAL)
        {
            var taskListQuery = _filterTaskManager.BuildTaskListQuery(StepModuleList.FreightManagement.ToString(), TaskStatusList.InProgress.ToString(), _currentUserADId);
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == FreightManagementList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, string.Empty, string.Empty, delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <Or>
                                        {taskListQuery}
                                        {delegationQuery}
                                    </Or>
                                    <Geq>
                                        <FieldRef Name='{FreightManagementList.TransportTimeField}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                    </Geq>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += freightManagementDAL.CountByQuery(query);
            }
            else
            {
                var freightManagementList = freightManagementDAL.GetByQuery(query);

                if (freightManagementList != null)
                {
                    foreach (var freightManagement in freightManagementList)
                    {
                        var filterTask = new FilterTask(freightManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetBusinessTripTaskList(BusinessTripManagementDAL businessTripManagementDAL)
        {
            var taskListQuery = _filterTaskManager.BuildTaskListQuery(StepModuleList.BusinessTripManagement.ToString(), TaskStatusList.InProgress.ToString(), _currentUserADId);
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == BusinessTripManagementList.Url).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, string.Empty, string.Empty, delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <Or>
                                        {taskListQuery}
                                        {delegationQuery}
                                    </Or>
                                    <Or>
                                        <Eq>
                                            <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                    </Or>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += businessTripManagementDAL.CountByQuery(query);
            }
            else
            {
                var businessTripManagementList = businessTripManagementDAL.GetByQuery(query);

                if (businessTripManagementList != null)
                {
                    foreach (var businessTripManagement in businessTripManagementList)
                    {
                        var filterTask = new FilterTask(businessTripManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetRequestTaskList(RequestsDAL requestDAL)
        {
            var requestByValue = "Mua hàng/Buy new materials or equipments";
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == RequestsList.Url).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserInfoId, RequestsList.PendingAtField, "Lookup", delegatedItemIDs);

            var query = $@"<Where>
                            <And>
                                <And>
                                    {delegationQuery}
                                    <Eq>
                                        <FieldRef Name='{ApprovalFields.WFStatus}' />
                                        <Value Type='Text'>{StringConstant.ApprovalStatus.InProgress}</Value>
                                    </Eq>
                                </And>
                                <And>
                                    <Neq>
                                        <FieldRef Name='{RequestsList.RequestTypeRefField}'/>
                                        <Value Type='Lookup'>{requestByValue}</Value>
                                    </Neq>
                                    <Or>
                                        <Eq>
                                            <FieldRef Name='{RequestsList.FinishDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{RequestsList.FinishDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                    </Or>
                                </And>
                            </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += requestDAL.CountByQuery(query);
            }
            else
            {
                var requestManagementList = requestDAL.GetByQuery(query);
                if (requestManagementList != null && requestManagementList.Count > 0)
                {
                    foreach (var requestManagement in requestManagementList)
                    {
                        var filterTask = new FilterTask(requestManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetRecruitmentTaskList(EmployeeRequirementSheetDAL recruitmentDAL)
        {
            // Ngay can nhan su <= NOW + 15 + 1
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == EmployeeRequirementSheetsList.Url).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserInfoId, EmployeeRequirementSheetsList.Fields.PendingAtField, "Lookup", delegatedItemIDs);

            var query = $@"<Where>
                            <And>
                                <And>
                                    {delegationQuery}
                                    <Eq>
                                        <FieldRef Name='{ApprovalFields.WFStatus}' />
                                        <Value Type='Text'>{StringConstant.ApprovalStatus.InProgress}</Value>
                                    </Eq>
                                </And>
                                <Or>
                                    <Eq>
                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(16).ToString(StringConstant.DateFormatForCAML)}</Value>
                                    </Eq>
                                    <Or>
                                        <Eq>
                                            <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(17).ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                        <And>
                                            <Geq>
                                                <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Geq>
                                            <Lt>
                                                <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(16).ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Lt>
                                        </And>
                                    </Or>
                                </Or>
                            </And>
                        </Where>";

            if (this.CountOnly)
            {
                TotalCount += recruitmentDAL.CountByQuery(query);
            }
            else
            {
                var recruitmentManagementList = recruitmentDAL.GetByQuery(query);
                if (recruitmentManagementList != null && recruitmentManagementList.Count > 0)
                {
                    foreach (var recruitmentManagement in recruitmentManagementList)
                    {
                        var filterTask = new FilterTask(recruitmentManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }

        private void GetMeetingRoomTaskList(RequisitionOfMeetingRoomDAL requisitionOfMeetingRoomDAL)
        {
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == RequisitionOfMeetingRoomList.Url).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserInfoId, RequisitionOfMeetingRoomList.Fields.PendingAtField, "Lookup", delegatedItemIDs);

            var query = $@"<Where>
                            <And>
                                <And>
                                    {delegationQuery}
                                    <Eq>
                                        <FieldRef Name='{ApprovalFields.WFStatus}' />
                                        <Value Type='Text'>{StringConstant.ApprovalStatus.InProgress}</Value>
                                    </Eq>
                                </And>
                                <Or>
                                    <Eq>
                                        <FieldRef Name='{RequisitionOfMeetingRoomList.Fields.StartDate}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='{RequisitionOfMeetingRoomList.Fields.StartDate}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                    </Eq>
                                </Or>
                            </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += requisitionOfMeetingRoomDAL.CountByQuery(query);
            }
            else
            {
                var requisitionOfMeetingRoomList = requisitionOfMeetingRoomDAL.GetByQuery(query);
                if (requisitionOfMeetingRoomList != null && requisitionOfMeetingRoomList.Count > 0)
                {
                    foreach (var requisitionOfMeetingRoom in requisitionOfMeetingRoomList)
                    {
                        var filterTask = new FilterTask(requisitionOfMeetingRoom);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }

        private void GetGuestReceptionTaskList(GuestReceptionManagementDAL guestReceptionManagementDAL)
        {
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == GuestReceptionManagementList.Url).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserInfoId, GuestReceptionManagementList.Fields.PendingAtField, "Lookup", delegatedItemIDs);

            // Thuan todo
            var query = string.Empty;
            //var query = $@"<Where>
            //                <And>
            //                    <And>
            //                        {delegationQuery}
            //                        <Eq>
            //                            <FieldRef Name='{ApprovalFields.WFStatus}' />
            //                            <Value Type='Text'>{StringConstant.ApprovalStatus.InProgress}</Value>
            //                        </Eq>
            //                    </And>
            //                    <Or>
            //                        <Eq>
            //                            <FieldRef Name='{RequirementForGuestReceptionList.Fields.StartDate}' />
            //                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
            //                        </Eq>
            //                        <Eq>
            //                            <FieldRef Name='{RequirementForGuestReceptionList.Fields.StartDate}' />
            //                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
            //                        </Eq>
            //                    </Or>
            //                </And>
            //                </Where>";

            if (this.CountOnly)
            {
                TotalCount += guestReceptionManagementDAL.CountByQuery(query);
            }
            else
            {
                var guestReceptionManagementList = guestReceptionManagementDAL.GetByQuery(query);
                if (guestReceptionManagementList != null && guestReceptionManagementList.Count > 0)
                {
                    foreach (var guestReceptionManagement in guestReceptionManagementList)
                    {
                        var filterTask = new FilterTask(guestReceptionManagement);
                        filterTask.ApprovalStatusId = ApprovalStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        #endregion
    }
}
