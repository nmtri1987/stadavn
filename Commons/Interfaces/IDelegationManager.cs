using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Interfaces
{
    /// <summary>
    /// IDelegationManager
    /// </summary>
    public interface IDelegationManager
    {
        /// <summary>
        /// GetListOfTasks
        /// </summary>
        /// <param name="fromEmployee">The EmployeeInfo object.</param>
        /// <returns>List of delegation items.</returns>
        List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee);

        Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb);

        LookupItem GetCurrentEmployeeProcessing(SPListItem listItem);

        bool IsValidTask(int listItemID);
    }
}
