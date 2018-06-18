using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class GuestReceptionManagementDAL : BaseDAL<GuestReceptionManagement>, IDelegationManager, IFilterTaskManager
    {
        public GuestReceptionManagementDAL(string siteUrl) : base(siteUrl)
        {
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem currentEmployeeProcessing = null;

            GuestReceptionManagement guestReceptionManagement = this.ParseToEntity(listItem);
            if (guestReceptionManagement.PendingAt != null && guestReceptionManagement.PendingAt.Count > 0)
            {
                currentEmployeeProcessing = guestReceptionManagement.PendingAt[0];
            }

            return currentEmployeeProcessing;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            GuestReceptionManagement guestReceptionManagement = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(guestReceptionManagement, currentWeb);

            return delegation;
        }

        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> delegations = null;

            string queryString = $@"<Where>
                                        <Eq>
                                            <FieldRef Name='{GuestReceptionManagementList.Fields.PendingAtField}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{fromEmployee.ID}</Value>
                                        </Eq>
                                    </Where>";

            var guestReceptionManagementCollection = GetByQuery(queryString);
            if (guestReceptionManagementCollection != null && guestReceptionManagementCollection.Count > 0)
            {
                delegations = new List<Delegation>();
                foreach (var guestReceptionManagement in guestReceptionManagementCollection)
                {
                    var delegation = new Delegation(guestReceptionManagement);
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
