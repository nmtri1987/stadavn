using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// DelegationsDAL
    /// </summary>
    public class DelegationsDAL : BaseDAL<Delegation>
    {
        /// <summary>
        /// DelegationsDAL
        /// </summary>
        public DelegationsDAL(string siteUrl) : base(siteUrl)
        {
        }

        public List<Delegation> GetDelegationApprovalList(SPWeb web, int currentUserInfoId)
        {
            string queryString = $@"<Where>
                                        <And>    
                                            <Eq>
                                                <FieldRef Name='{DelegationsList.Fields.ToEmployee}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{currentUserInfoId}</Value>
                                            </Eq>
                                            <And>
                                                <Leq>
                                                    <FieldRef Name='{DelegationsList.Fields.FromDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatForCAML)}</Value>
                                                </Leq>
                                                <Geq>
                                                    <FieldRef Name='{DelegationsList.Fields.ToDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatForCAML)}</Value>
                                                </Geq>
                                            </And>
                                        </And>
                                    </Where>
                                    <OrderBy>
                                        <FieldRef Name='Created' Ascending='False' />
                                    </OrderBy>";
            var delegations = this.GetByQuery(queryString);
            // Filter delegations which is not approved yet.
            var listOfDelegationsAreNotApprovedYet = new List<Delegation>();
            if (delegations != null && delegations.Count > 0)
            {
                foreach (var delegation in delegations)
                {
                    if (this.IsDelegationNotApprovedYet(delegation, currentUserInfoId, web))
                    {
                        listOfDelegationsAreNotApprovedYet.Add(delegation);
                    }
                }
            }

            return listOfDelegationsAreNotApprovedYet;
        }

        private bool IsDelegationNotApprovedYet(Delegation delegation, int currentUserInfoId, SPWeb web)
        {
            var isDelegationNotApprovedYet = false;

            string listUrl = delegation.ListUrl;
            int itemId = delegation.ListItemID;
            var queryString = $"<Where><Eq><FieldRef Name='ID'></FieldRef><Value Type='Integer'>{itemId.ToString(CultureInfo.InvariantCulture)}</Value></Eq></Where>";
            SPList list = web.GetList(string.Format("{0}{1}", this.SiteUrl, listUrl));
            var query = new SPQuery { Query = queryString };
            var results = list.GetItems(query);
            if (results != null && results.Count > 0)
            {
                int fromEmployeeId = 0;
                var delegatedEmployee = DelegationManager.GetCurrentEmployeeProcessing(listUrl, results[0], web);
                if (delegatedEmployee != null)
                {
                    fromEmployeeId = delegatedEmployee.LookupId;
                }
                if (fromEmployeeId > 0)
                {
                    var isDelegation = DelegationPermissionManager.IsDelegation(fromEmployeeId, currentUserInfoId, listUrl, itemId, this.SiteUrl);
                    if (isDelegation != null)
                    {
                        isDelegationNotApprovedYet = true;
                    }
                }
                return isDelegationNotApprovedYet;
            }

            return false;
        }

    }
}
