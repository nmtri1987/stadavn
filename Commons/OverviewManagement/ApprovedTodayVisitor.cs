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
    public class ApprovedTodayVisitor : IFilterTaskVisitor
    {
        #region "Properties"
        private int _currentUserADId;
        private int _currentUserInfoId;
        private string _siteUrl;
        public IList<FilterTask> FilterTaskList { get; set; }

        public bool CountOnly { get; set; }
        public int TotalCount { get; set; }
        public string ApproverFullName { get; set; }

        private const int RejectedStatusId = 2; // Rejected
        private const int ApprovedStatusId = 3; // Approved
        private const int InProgressStatusId = 1; // In-Progress
        private const int InProcessStatusId = 4; // In-Process
        private const int CompletedId = 5; // Completed
        private FilterTaskManager _filterTaskManager;
        #endregion

        #region "Constructor"
        public ApprovedTodayVisitor(int currentUserADId, int currentUserInfoId, string siteUrl)
        {
            FilterTaskList = new List<FilterTask>();
            _currentUserADId = currentUserADId;
            _currentUserInfoId = currentUserInfoId;
            _siteUrl = siteUrl;
            _filterTaskManager = new FilterTaskManager(siteUrl);
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

        private void GetShiftTaskList(ShiftManagementDAL shiftManagementDAL)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            var query = $@"<Where>
                                <And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.ShiftManagementList.CommonAddApprover1Field}' LookupId='TRUE' />
                                            <Value Type='User'>{_currentUserADId}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.DefaultSPListField.ModifiedByField}' LookupId='TRUE' />
                                            <Value Type='User'>{_currentUserADId}</Value>
                                        </Eq>
                                    </And>
                                    <Eq>
                                        <FieldRef Name='{StringConstant.DefaultSPListField.ModifiedField}' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                    </Eq>
                                </And>
                            </Where>";

            if (this.CountOnly)
            {
                TotalCount += shiftManagementDAL.CountByQuery(query);
            }
            else
            {
                var shiftManagementList = shiftManagementDAL.GetByQuery(query);
                if (shiftManagementList != null && shiftManagementList.Count > 0)
                {
                    foreach (var shiftManagement in shiftManagementList)
                    {
                        var filterTask = new FilterTask(shiftManagement);
                        filterTask.ApprovalStatusId = ApprovedStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetChangeShiftTaskList(ChangeShiftManagementDAL changeShiftManagementDAL)
        {
            var query = $@"<Where>
                                <And>
                                    <Or>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                            <Value Type='Text'>Approved</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                            <Value Type='Text'>Rejected</Value>
                                        </Eq>
                                    </Or>
                                    <And>
	                                    <Eq>
                                            <FieldRef Name='{StringConstant.ChangeShiftList.DHField}' LookupId='TRUE' />
                                            <Value Type='User'>{_currentUserADId}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.DefaultSPListField.ModifiedField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                    </And>
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
                        filterTask.ApprovalStatusId = changeShiftManagement.ApprovalStatus == "Approved" ? ApprovedStatusId : RejectedStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetOvertimeTaskList(OverTimeManagementDAL overTimeManagementDAL)
        {
            var query = $@"<Where>
                                <Or>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.OverTimeManagementList.FirstApprovedByField}' LookupId='TRUE' />
                                            <Value Type='User'>{_currentUserADId}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.OverTimeManagementList.FirstApprovedDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                    </And>
                                    <And>
                                        <Or>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                                <Value Type='Text'>true</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                                <Value Type='Text'>false</Value>
                                            </Eq>
                                        </Or>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.OverTimeManagementList.ApprovedByField}' LookupId='TRUE' />
                                                <Value Type='User'>{_currentUserADId}</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.DefaultSPListField.ModifiedField}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                            </Eq>
                                        </And>
                                    </And>
                                </Or>
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
                        filterTask.ApprovalStatusId = overtimeManagement.ApprovalStatus == "true" ? ApprovedStatusId
                            : overtimeManagement.ApprovalStatus == "false" ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetNotOvertimeTaskList(NotOvertimeManagementDAL notOvertimeManagementDAL)
        {
            var dueDate = DateTime.Now.AddDays(1);

            var query = $@"<Where>
                                <And>
                                    <Or>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                            <Value Type='Text'>Approved</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                            <Value Type='Text'>Rejected</Value>
                                        </Eq>
                                    </Or>
                                    <And>
	                                    <Eq>
                                            <FieldRef Name='{StringConstant.NotOvertimeList.DHField}' LookupId='TRUE' />
                                            <Value Type='User'>{_currentUserADId}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.DefaultSPListField.ModifiedField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Eq>
                                    </And>
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
                        filterTask.ApprovalStatusId = notOvertimeManagement.ApprovalStatus == "Approved" ? ApprovedStatusId : RejectedStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetLeaveTaskList(LeaveManagementDAL leaveManagementDAL)
        {
            var query = _filterTaskManager.BuildApprovalTaskListQuery(StepModuleList.LeaveManagement.ToString(), _currentUserADId, DateTime.Now);

            if (this.CountOnly)
            {
                TotalCount += leaveManagementDAL.CountByQuery(query);
            }
            else
            {
                var leaveManagementList = leaveManagementDAL.GetByQuery(query);
                if (leaveManagementList != null && leaveManagementList.Count > 0)
                {
                    foreach (var leaveManagement in leaveManagementList)
                    {
                        var filterTask = new FilterTask(leaveManagement);
                        filterTask.ApprovalStatusId = leaveManagement.ApprovalStatus == Status.Approved ? ApprovedStatusId
                            : leaveManagement.ApprovalStatus == Status.Rejected ? RejectedStatusId 
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetVehicleTaskList(VehicleManagementDAL vehicleManagementDAL)
        {
            var query = _filterTaskManager.BuildApprovalTaskListQuery(StepModuleList.VehicleManagement.ToString(), _currentUserADId, DateTime.Now);

            if (this.CountOnly)
            {
                TotalCount += vehicleManagementDAL.CountByQuery(query);
            }
            else
            {
                var vehicleManagementList = vehicleManagementDAL.GetByQuery(query);
                if (vehicleManagementList != null && vehicleManagementList.Count > 0)
                {
                    foreach (var vehicleManagement in vehicleManagementList)
                    {
                        var filterTask = new FilterTask(vehicleManagement);
                        filterTask.ApprovalStatusId = vehicleManagement.ApprovalStatus == Status.Approved ? ApprovedStatusId
                            : vehicleManagement.ApprovalStatus == Status.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetFreightTaskList(FreightManagementDAL freightManagementDAL)
        {
            var query = _filterTaskManager.BuildApprovalTaskListQuery(StepModuleList.FreightManagement.ToString(), _currentUserADId, DateTime.Now);

            if (this.CountOnly)
            {
                TotalCount += freightManagementDAL.CountByQuery(query);
            }
            else
            {
                var freightManagementList = freightManagementDAL.GetByQuery(query);
                if (freightManagementList != null && freightManagementList.Count > 0)
                {
                    foreach (var freightManagement in freightManagementList)
                    {
                        var filterTask = new FilterTask(freightManagement);
                        filterTask.ApprovalStatusId = freightManagement.ApprovalStatus == Status.Approved ? ApprovedStatusId
                            : freightManagement.ApprovalStatus == Status.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetBusinessTripTaskList(BusinessTripManagementDAL businessTripManagementDAL)
        {
            var query = _filterTaskManager.BuildApprovalTaskListQuery(StepModuleList.BusinessTripManagement.ToString(), _currentUserADId, DateTime.Now);

            if (this.CountOnly)
            {
                TotalCount += businessTripManagementDAL.CountByQuery(query);
            }
            else
            {
                var businessTripManagementList = businessTripManagementDAL.GetByQuery(query);
                if (businessTripManagementList != null && businessTripManagementList.Count > 0)
                {
                    foreach (var businessTripManagement in businessTripManagementList)
                    {
                        var filterTask = new FilterTask(businessTripManagement);
                        filterTask.ApprovalStatusId = businessTripManagement.ApprovalStatus == Status.Approved ? ApprovedStatusId
                            : businessTripManagement.ApprovalStatus == Status.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetRequestTaskList(RequestsDAL requestDAL)
        {
            var query = _filterTaskManager.BuildApprovalWorkflowHistoryQuery(RequestsList.ListName, this.ApproverFullName, DateTime.Now);

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
                        filterTask.ApprovalStatusId = requestManagement.WFStatus == ApprovalStatus.Approved ? ApprovedStatusId 
                            : requestManagement.WFStatus == ApprovalStatus.InProcess ? InProcessStatusId
                            : requestManagement.WFStatus == ApprovalStatus.Completed ? CompletedId
                            : requestManagement.WFStatus == ApprovalStatus.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetRecruitmentTaskList(EmployeeRequirementSheetDAL recruitmentDAL)
        {
            var query = _filterTaskManager.BuildApprovalWorkflowHistoryQuery(EmployeeRequirementSheetsList.ListName, this.ApproverFullName, DateTime.Now);

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
                        filterTask.ApprovalStatusId = recruitmentManagement.WFStatus == Status.Approved ? ApprovedStatusId
                            : recruitmentManagement.WFStatus == ApprovalStatus.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }
        private void GetCertificateTaskList(RequestForDiplomaSupplyDAL certificateDAL)
        {
            var query = _filterTaskManager.BuildApprovalWorkflowHistoryQuery(RequestForDiplomaSuppliesList.ListName, this.ApproverFullName, DateTime.Now);

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
                        filterTask.ApprovalStatusId = certificateManagement.WFStatus == Status.Approved ? ApprovedStatusId
                            : certificateManagement.WFStatus == ApprovalStatus.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }

        private void GetMeetingRoomTaskList(RequisitionOfMeetingRoomDAL requisitionOfMeetingRoomDAL)
        {
            var query = _filterTaskManager.BuildApprovalWorkflowHistoryQuery(RequisitionOfMeetingRoomList.ListName, this.ApproverFullName, DateTime.Now);

            if (this.CountOnly)
            {
                TotalCount += requisitionOfMeetingRoomDAL.CountByQuery(query);
            }
            else
            {
                var requisitionOfMeetingList = requisitionOfMeetingRoomDAL.GetByQuery(query);
                if (requisitionOfMeetingList != null && requisitionOfMeetingList.Count > 0)
                {
                    foreach (var requisitionOfMeeting in requisitionOfMeetingList)
                    {
                        var filterTask = new FilterTask(requisitionOfMeeting);
                        filterTask.ApprovalStatusId = requisitionOfMeeting.WFStatus == Status.Approved ? ApprovedStatusId
                            : requisitionOfMeeting.WFStatus == ApprovalStatus.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }

        private void GetGuestReceptionTaskList(GuestReceptionManagementDAL guestReceptionManagementDAL)
        {
            var query = _filterTaskManager.BuildApprovalWorkflowHistoryQuery(GuestReceptionManagementList.ListName, this.ApproverFullName, DateTime.Now);

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
                        filterTask.ApprovalStatusId = guestReceptionManagement.WFStatus == Status.Approved ? ApprovedStatusId
                            : guestReceptionManagement.WFStatus == ApprovalStatus.Rejected ? RejectedStatusId
                            : InProgressStatusId;
                        FilterTaskList.Add(filterTask);
                    }
                }
            }
        }

        #endregion
    }
}
