using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using Microsoft.SharePoint;
using System;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// EmployeeRequirementSheetDAL class.
    /// </summary>
    public class EmployeeRequirementSheetDAL : BaseDAL<EmployeeRequirementSheet>, IDelegationManager, IFilterTaskManager
    {
        public EmployeeRequirementSheetDAL(string siteUrl) : base(siteUrl)
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

            EmployeeRequirementSheet employeeRequirementSheet = this.ParseToEntity(listItem);
            if (employeeRequirementSheet.PendingAt != null && employeeRequirementSheet.PendingAt.Count > 0)
            {
                currentEmployeeProcessing = employeeRequirementSheet.PendingAt[0];
            }

            return currentEmployeeProcessing;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            EmployeeRequirementSheet employeeRequirementSheet = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(employeeRequirementSheet, currentWeb);

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
                                            <FieldRef Name='{EmployeeRequirementSheetsList.Fields.PendingAtField}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{fromEmployee.ID}</Value>
                                        </Eq>
                                    </Where>";

            var employeeRequirementSheets = GetByQuery(queryString);
            if (employeeRequirementSheets != null && employeeRequirementSheets.Count > 0)
            {
                delegations = new List<Delegation>();
                foreach (var employeeRequirementSheet in employeeRequirementSheets)
                {
                    var delegation = new Delegation(employeeRequirementSheet);
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
