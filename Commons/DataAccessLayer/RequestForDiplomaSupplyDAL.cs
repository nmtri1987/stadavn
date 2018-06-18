using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// RequestForDiplomaSupplyDAL
    /// </summary>
    public class RequestForDiplomaSupplyDAL : BaseDAL<RequestForDiplomaSupply>, IDelegationManager, IFilterTaskManager
    {
        public RequestForDiplomaSupplyDAL(string siteUrl) : base(siteUrl)
        {
        }

        /// <summary>
        /// GetCurrentEmployeeProcessing
        /// </summary>
        /// <param name="listItem"></param>
        /// <returns></returns>
        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem currentEmployeeProcessing = null;

            RequestForDiplomaSupply requestForDiplomaSupply = this.ParseToEntity(listItem);
            if (requestForDiplomaSupply.PendingAt != null && requestForDiplomaSupply.PendingAt.Count > 0)
            {
                currentEmployeeProcessing = requestForDiplomaSupply.PendingAt[0];
            }

            return currentEmployeeProcessing;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            RequestForDiplomaSupply requestForDiplomaSupply = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(requestForDiplomaSupply, currentWeb);

            return delegation;
        }

        /// <summary>
        /// Get task list of fromEmployee.
        /// </summary>
        /// <param name="fromEmployee">The employee who has processing task.</param>
        /// <returns>The list of delegation items.</returns>
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> delegations = null;

            string queryString = $@"<Where>
                                        <Eq>
                                            <FieldRef Name='{RequestForDiplomaSuppliesList.Fields.PendingAtField}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{fromEmployee.ID}</Value>
                                        </Eq>
                                    </Where>";

            var requestForDiplomaSupplies = GetByQuery(queryString);
            if (requestForDiplomaSupplies != null && requestForDiplomaSupplies.Count > 0)
            {
                delegations = new List<Delegation>();
                foreach (var requestForDiplomaSupply in requestForDiplomaSupplies)
                {
                    var delegation = new Delegation(requestForDiplomaSupply);
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
