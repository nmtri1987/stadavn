//-----------------------------------------------------------------------
// <copyright file="EmployeeInfoDAL.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RBVH.Stada.Intranet.WebPages.DALs
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SharePoint;
    using RBVH.Stada.Intranet.WebPages.Models;
    using RBVH.Stada.Intranet.WebPages.Utils;

    /// <summary>
    /// Access data from SharePoint for Employee Info
    /// </summary>
    public class EmployeeInfoDAL
    {
        /// <summary>
        /// Get All Employee Info
        /// </summary>
        /// <returns>Result List Employee Info</returns>
        public List<EmployeeInfo> GetEmployeeInfos()
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();

            // Query SPList
            SPWeb spweb = SPContext.Current.Web;
            SPList splist = spweb.GetList(spweb.Url + StringConstants.EmployeeInfoURL);
            SPQuery spquery = new SPQuery()
            {
                RowLimit = 5000, // Get All Items
            };
            var splistItemCollection_EmployeeInfo = splist.GetItems(spquery);

            // Parse to employeeInfos
            foreach (SPListItem splistItem_EmployeeInfo in splistItemCollection_EmployeeInfo)
            {
                var employeeInfo = this.ParseToEntity(splistItem_EmployeeInfo);
                employeeInfos.Add(employeeInfo);
            }

            return employeeInfos;
        }

        /// <summary>
        /// Get Employee Info by Employee ID
        /// </summary>
        /// <param name="employeeID">Employee ID as string</param>
        /// <returns>Employee Info Entity</returns>
        public EmployeeInfo GetEmployeeInfoByEmployeeID(string employeeID)
        {
            EmployeeInfo employeeInfo = null;

            // Query SPList
            SPWeb spweb = SPContext.Current.Web;
            SPList splist = spweb.GetList(spweb.Url + StringConstants.EmployeeInfoURL);
            SPQuery spquery = new SPQuery()
            {
                Query = string.Format(@"<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>{0}</Value></Eq></Where>", employeeID)
            };
            var splistItemCollection_EmployeeInfo = splist.GetItems(spquery);

            // Get First and Parse to EmployeeInfo
            var spistItem_EmployeeInfo = splistItemCollection_EmployeeInfo[0];
            employeeInfo = this.ParseToEntity(spistItem_EmployeeInfo);

            return employeeInfo;
        }

        /// <summary>
        /// Update Password for Employee get by Employee ID
        /// </summary>
        /// <param name="employeeID">Employee ID as string</param>
        /// <param name="newPassword">New Password  as string</param>
        /// <returns>result update success or fail</returns>
        public bool UpdatePassword(string employeeID, string newPassword)
        {
            bool result = false;

            // Delegate!
            Guid spsiteId = SPContext.Current.Site.ID;
            Guid spwebId = SPContext.Current.Web.ID;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite spSite = new SPSite(spsiteId))
                {
                    using (SPWeb spWeb = spSite.OpenWeb(spwebId))
                    {
                        spWeb.AllowUnsafeUpdates = true;

                        // Query SPList
                        SPList m_SPSList = spWeb.GetList(spWeb.Url + StringConstants.EmployeeInfoURL);
                        SPQuery m_SPQuery = new SPQuery()
                        {
                            Query = string.Format(@"<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>{0}</Value></Eq></Where>", employeeID)
                        };
                        var m_SPListItemCollection_EmployeeInfo = m_SPSList.GetItems(m_SPQuery);

                        // Get First and Parse to EmployeeInfo
                        foreach (SPListItem m_SPListItem_EmployeeInfo in m_SPListItemCollection_EmployeeInfo)
                        {
                            m_SPListItem_EmployeeInfo[StringConstants.EmployeeInfoFieldPassword] = newPassword;
                            m_SPListItem_EmployeeInfo.Update();
                        }

                        result = true;

                        spWeb.AllowUnsafeUpdates = false;
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Parse SPListItem to Employee Info entity
        /// </summary>
        /// <param name="splistItem">SPListItem from Employee Info List</param>
        /// <returns>Employee Info entity</returns>
        protected EmployeeInfo ParseToEntity(SPListItem splistItem)
        {
            SPFieldLookupValue spfieldLookupValue_ADAccount = new SPFieldLookupValue((string)splistItem[StringConstants.EmployeeInfoFieldADAccount]);
            SPFieldLookupValue spfieldLookupValue_Department = new SPFieldLookupValue((string)splistItem[StringConstants.EmployeeInfoFieldEmployeeInfoDepartment]);
            string fullName = SplitStrings.GetSecondString(splistItem[StringConstants.EmployeeInfoFieldFullName] + string.Empty);

            var employeeInfo = new EmployeeInfo()
            {
                EmployeeID = (string)splistItem[StringConstants.EmployeeInfoFieldEmployeeID],
                FullName = fullName,
                LookupId = splistItem.ID,
                AdAccountLookupValue = spfieldLookupValue_ADAccount != null ? spfieldLookupValue_ADAccount.LookupId : 0,
                AdAccountLookupValueString = spfieldLookupValue_ADAccount != null ? spfieldLookupValue_ADAccount.LookupValue : string.Empty,
                Department = spfieldLookupValue_Department != null ? spfieldLookupValue_Department.LookupValue : string.Empty,
                Password = (string)splistItem[StringConstants.EmployeeInfoFieldPassword]
            };

            return employeeInfo;
        }
    }
}
