using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.OverviewManagement
{
    public class InProcessVisitor : IFilterTaskVisitor
    {
        #region "Public Properties"
        private int _currentUserADId;
        private int _currentUserInfoId;
        private string _siteUrl;
        public IList<FilterTask> FilterTaskList { get; set; }
        public IList<Delegation> DelegationList { get; set; }
        public bool CountOnly { get; set; }
        public int TotalCount { get; set; }

        private FilterTaskManager _filterTaskManager;
        private const int ApprovalStatusId = 4; // In-Process

        #endregion

        #region "Constructor"
        public InProcessVisitor(int currentUserADId, int currentUserInfoId, string siteUrl)
        {
            FilterTaskList = new List<FilterTask>();
            _currentUserADId = currentUserADId;
            _currentUserInfoId = currentUserInfoId;
            _siteUrl = siteUrl;
            _filterTaskManager = new FilterTaskManager();
        }
        #endregion

        #region "Visitor Implementation"
        public void Visit(OverTimeManagementDAL overTimeManagementDAL)
        {
            // Do nothing
        }

        public void Visit(NotOvertimeManagementDAL notOvertimeManagementDAL)
        {
            // Do nothing
        }

        public void Visit(ChangeShiftManagementDAL changeShiftManagementDAL)
        {
            // Do nothing
        }

        public void Visit(ShiftManagementDAL shiftManagementDAL)
        {
            // Do nothing
        }

        public void Visit(LeaveManagementDAL leaveManagementDAL)
        {
            // Do nothing
        }

        public void Visit(VehicleManagementDAL vehicleManagementDAL)
        {
            // Do nothing
        }

        public void Visit(FreightManagementDAL freightManagementDAL)
        {
            // Do nothing
        }

        public void Visit(BusinessTripManagementDAL businessTripManagementDAL)
        {
            // Do nothing
        }

        public void Visit(RequestsDAL requestDAL)
        {
            GetRequestTaskList(requestDAL);
        }

        public void Visit(EmployeeRequirementSheetDAL recruitmentDAL)
        {
            // Do nothing
        }

        public void Visit(RequestForDiplomaSupplyDAL certificateDAL)
        {
            // Do nothing
        }

        public void Visit(RequisitionOfMeetingRoomDAL requisitionOfMeetingRoomDAL)
        {
            // Do nothing
        }

        public void Visit(GuestReceptionManagementDAL guestReceptionManagementDAL)
        {
            // Do nothing
        }

        #endregion

        #region "Private Methods"

        private void GetRequestTaskList(RequestsDAL requestDAL)
        {
            var requestByValue = "Mua hàng/Buy new materials or equipments";
            List<int> delegatedItemIDs = this.DelegationList.Where(d => d.ListUrl == RequestsList.Url).Select(d => d.ListItemID).ToList();
            var delegationQuery = _filterTaskManager.BuildApprovedByDelegationQuery(_currentUserInfoId, RequestsList.CommonCreatorField, "Lookup", delegatedItemIDs);

            var query = $@"<Where>
                            <And>
                                <And>
                                    {delegationQuery}
                                    <Eq>
                                        <FieldRef Name='{ApprovalFields.WFStatus}' />
                                        <Value Type='Text'>{StringConstant.ApprovalStatus.InProcess}</Value>
                                    </Eq>
                                </And>
                                <Or>
                                    <And>
                                        <Neq>
                                            <FieldRef Name='{RequestsList.RequestTypeRefField}'/>
                                            <Value Type='Lookup'>{requestByValue}</Value>
                                        </Neq>
                                        <Geq>
                                            <FieldRef Name='{RequestsList.FinishDateField}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Now.ToString(StringConstant.DateFormatForCAML)}</Value>
                                        </Geq>
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

        #endregion
    }
}
