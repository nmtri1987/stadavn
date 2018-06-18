using RBVH.Stada.Intranet.Biz.ApprovalManagement;
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
    public class WaitingApprovalVisitor : IFilterTaskVisitor
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
        #endregion

        #region "Constructor"
        public WaitingApprovalVisitor(int currentUserADId, int currentUserInfoAd, string siteUrl)
        {
            FilterTaskList = new List<FilterTask>();
            _currentUserADId = currentUserADId;
            _currentUserInfoId = currentUserInfoAd;
            _siteUrl = siteUrl;
            _filterTaskManager = new FilterTaskManager(siteUrl);
        }
        #endregion

        #region "Visitor Implementation"
        public void Visit(ShiftManagementDAL shiftManagementDAL)
        {
            // DO NOTHING
        }

        public void Visit(ChangeShiftManagementDAL changeShiftManagementDAL)
        {
            GetChangeShiftTaskList(changeShiftManagementDAL);
        }

        public void Visit(OverTimeManagementDAL overTimeManagementDAL)
        {
            // DO NOTHING
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
            // Do nothing
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
            GetCertificateTaskList(certificateDAL);
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
        private void GetChangeShiftTaskList(ChangeShiftManagementDAL changeShiftManagementDAL)
        {
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == ChangeShiftList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, ChangeShiftList.DHField, "User", delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <And>
	                                    {delegationQuery}
                                        <Gt>
                                           <FieldRef Name='{StringConstant.ChangeShiftList.FromDateField}' />
                                           <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Gt>
                                    </And>
	                                <IsNull>
		                                <FieldRef Name='ApprovalStatus' />
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
        private void GetNotOvertimeTaskList(NotOvertimeManagementDAL notOvertimeManagementDAL)
        {
            var dueDate = DateTime.Now.AddDays(1);
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == NotOvertimeList.ListUrl).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserADId, NotOvertimeList.DHField, "User", delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    <And>
	                                    {delegationQuery}
                                        <Gt>
                                           <FieldRef Name='{StringConstant.NotOvertimeList.DateField}' />
                                           <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Gt>
                                    </And>
	                                <IsNull>
		                                <FieldRef Name='ApprovalStatus' />
	                                </IsNull>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += notOvertimeManagementDAL.CountByQuery(query);
            }
            else
            {
                List<NotOvertimeManagement> notOvertimeManagementList = notOvertimeManagementDAL.GetByQuery(query);

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
                                                <Gt>
                                                    <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                </Gt>
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
                                                <Geq>
                                                    <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(3).ToString(StringConstant.DateFormatForCAML)}</Value>
                                                </Geq>
                                            </And>
                                        </Or>
                                        <And>
                                            <Geq>
                                                <FieldRef Name='{LeaveManagementList.TotalDaysField}' />
                                                <Value Type='Number'>5</Value>
                                            </Geq>
                                            <Gt>
                                                <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(15).ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Gt>
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
                                    <Gt>
                                        <FieldRef Name='{VehicleManagementList.CommonFrom}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                    </Gt>
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
                                    <Gt>
                                        <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                    </Gt>
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
                                <Or>
                                    <And>
                                        <Neq>
                                            <FieldRef Name='{RequestsList.RequestTypeRefField}'/>
                                            <Value Type='Lookup'>{requestByValue}</Value>
                                        </Neq>
                                        <Gt>
                                            <FieldRef Name='{RequestsList.FinishDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Gt>
                                    </And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{RequestsList.RequestTypeRefField}'/>
                                            <Value Type='Lookup'>{requestByValue}</Value>
                                        </Eq>
                                        <Leq>
                                            <FieldRef Name='{StringConstant.DefaultSPListField.CreatedField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Leq>
                                    </And>
                                </Or>
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
            // Ngay can nhan su > NOW + 15 + 1
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
                                <Gt>
                                    <FieldRef Name='{CommonSPListField.CommonReqDueDateField}' />
                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(16).ToString(StringConstant.DateFormatForCAML)}</Value>
                                </Gt>
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
        private void GetCertificateTaskList(RequestForDiplomaSupplyDAL certificateDAL)
        {
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == RequestForDiplomaSuppliesList.Url).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserInfoId, RequestForDiplomaSuppliesList.Fields.PendingAtField, "Lookup", delegatedItemIDs);

            var query = $@"<Where>
                                <And>
                                    {delegationQuery}
                                    <Eq>
                                        <FieldRef Name='{ApprovalFields.WFStatus}' />
                                        <Value Type='Text'>{StringConstant.ApprovalStatus.InProgress}</Value>
                                    </Eq>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += certificateDAL.CountByQuery(query);
            }
            else
            {
                var certificateManagementList = certificateDAL.GetByQuery(query);
                if (certificateManagementList != null && certificateManagementList.Count > 0)
                {
                    foreach (var certificateManagement in certificateManagementList)
                    {
                        var filterTask = new FilterTask(certificateManagement);
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
                                <Gt>
                                    <FieldRef Name='{RequisitionOfMeetingRoomList.Fields.StartDate}' />
                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
                                </Gt>
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
            //                    <Gt>
            //                        <FieldRef Name='{RequisitionOfMeetingRoomList.Fields.StartDate}' />
            //                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.AddDays(1).ToString(StringConstant.DateFormatForCAML)}</Value>
            //                    </Gt>
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
