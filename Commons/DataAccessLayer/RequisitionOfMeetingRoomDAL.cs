using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class RequisitionOfMeetingRoomDAL : BaseDAL<RequisitionOfMeetingRoom>, IDelegationManager, IFilterTaskManager
    {
        public RequisitionOfMeetingRoomDAL(string siteUrl) : base(siteUrl)
        {
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem currentEmployeeProcessing = null;

            RequisitionOfMeetingRoom requisitionOfMeetingRoom = this.ParseToEntity(listItem);
            if (requisitionOfMeetingRoom.PendingAt != null && requisitionOfMeetingRoom.PendingAt.Count > 0)
            {
                currentEmployeeProcessing = requisitionOfMeetingRoom.PendingAt[0];
            }

            return currentEmployeeProcessing;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            RequisitionOfMeetingRoom requisitionOfMeetingRoom = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(requisitionOfMeetingRoom, currentWeb);

            return delegation;
        }

        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> delegations = null;

            string queryString = $@"<Where>
                                        <Eq>
                                            <FieldRef Name='{RequisitionOfMeetingRoomList.Fields.PendingAtField}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{fromEmployee.ID}</Value>
                                        </Eq>
                                    </Where>";

            var requisitionOfMeetingRoomCollection = GetByQuery(queryString);
            if (requisitionOfMeetingRoomCollection != null && requisitionOfMeetingRoomCollection.Count > 0)
            {
                delegations = new List<Delegation>();
                foreach (var requisitionOfMeetingRoom in requisitionOfMeetingRoomCollection)
                {
                    var delegation = new Delegation(requisitionOfMeetingRoom);
                    delegations.Add(delegation);
                }
            }

            return delegations;
        }

        public bool IsValidTask(int listItemID)
        {
            return true;
        }

        #region "Overview"
        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }
        #endregion
    }
}
