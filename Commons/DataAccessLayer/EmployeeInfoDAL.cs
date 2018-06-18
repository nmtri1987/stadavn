//-----------------------------------------------------------------------
// <copyright file="EmployeeDAL.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the EmployeeDAL class.</summary>
//-----------------------------------------------------------------------

using RBVH.Core.SharePoint;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    using System.Collections.Generic;
    using Microsoft.SharePoint;

    using System.Linq;
    using Constants;
    using Extension;
    using Models;
    using System;
    using Helpers;

    /// <summary>
    /// Access data from SharePoint for Employee Info
    /// </summary>
    public class EmployeeInfoDAL : BaseDAL<EmployeeInfo>
    {
        public EmployeeInfoDAL(string siteUrl) : base(siteUrl)
        {
            ListUrl = "/Lists/EmployeeInfo";
        }

        /// <summary>
        /// Get All Employee Info
        /// </summary>
        /// <returns>Result List Employee Info</returns>
        public List<EmployeeInfo> GetEmployeeInfos()
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        var employeeInfoItems = this.GetAllToSPListItemCollection(spWeb);

                        if (employeeInfoItems != null && employeeInfoItems.Count > 0)
                        {
                            foreach (SPListItem employeeItem in employeeInfoItems)
                            {
                                var employeeInfo = ParseToEntity(employeeItem);
                                employeeInfos.Add(employeeInfo);
                            }
                        }
                    }
                }
            }
            else
            {
                SPListItemCollection employeeInfoItems = null;
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    employeeInfoItems = this.GetAllToSPListItemCollection(currentWeb);
                }
                else
                {
                    employeeInfoItems = this.GetAllToSPListItemCollection(SPContext.Current.Site.RootWeb);
                }

                if (employeeInfoItems != null && employeeInfoItems.Count > 0)
                {
                    foreach (SPListItem employeeItem in employeeInfoItems)
                    {
                        var employeeInfo = ParseToEntity(employeeItem);
                        employeeInfos.Add(employeeInfo);
                    }
                }
            }

            return employeeInfos;
        }

        /// <summary>
        /// Get Employee Info by Employee ID
        /// </summary>
        /// <param name="employeeID">Employee ID as string</param>
        /// <returns>Employee Info Entity</returns>
        public EmployeeInfo GetByEmployeeID(string employeeID)
        {
            EmployeeInfo employeeInfo = null;

            string queryStr = $@"<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>{employeeID}</Value></Eq></Where>";
            List<EmployeeInfo> ret = ExecuteQuery(queryStr);
            if (ret != null && ret.Count > 0)
            {
                employeeInfo = ret[0];
            }

            return employeeInfo;
        }

        public List<EmployeeInfo> GetByADAccountIDs(List<int> Ids)
        {
            List<EmployeeInfo> employeeInfoCollection = new List<Models.EmployeeInfo>();

            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    string idValuesString = string.Empty;
                    foreach (int id in Ids)
                    {
                        idValuesString += string.Format("<Value Type='User'>{0}</Value>", id);
                    }

                    SPQuery spquery = new SPQuery
                    {
                        Query = $@"<Where><In><FieldRef Name='ADAccount' LookupId='TRUE'/><Values>{idValuesString}</Values></In></Where>"
                    };
                    employeeInfoCollection = this.GetByQuery(spquery);
                }
            }

            return employeeInfoCollection;
        }

        public List<EmployeeInfo> GetByIDs(List<int> Ids)
        {
            List<EmployeeInfo> employeeInfoCollection = new List<Models.EmployeeInfo>();
            employeeInfoCollection = GetByIDs(Ids, null);

            return employeeInfoCollection;
        }

        public List<EmployeeInfo> GetByIDs(List<int> Ids, string[] viewFields)
        {
            List<EmployeeInfo> employeeInfoCollection = new List<Models.EmployeeInfo>();

            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    string idValuesString = string.Empty;
                    foreach (int id in Ids)
                    {
                        idValuesString += string.Format("<Value Type='Counter'>{0}</Value>", id);
                    }

                    SPQuery spquery = new SPQuery
                    {
                        Query = $@"<Where><In><FieldRef Name='ID' /><Values>{idValuesString}</Values></In></Where>"
                    };

                    SPListItemCollection items = this.GetByQueryToSPListItemCollection(web, spquery, viewFields);
                    if (items != null && items.Count > 0)
                    {
                        foreach (SPListItem item in items)
                        {
                            employeeInfoCollection.Add(ParseToEntity(item));
                        }
                    }
                }
            }

            return employeeInfoCollection;
        }

        public List<EmployeeInfo> GetCommonAccountByFullNameOrId(string nameOrId)
        {
            List<EmployeeInfo> commonAccountList = new List<EmployeeInfo>();

            string queryStr = $"<Where><And><Eq><FieldRef Name='EmployeeType' /><Value Type='Choice'>{StringConstant.EmployeeType.CommonUser}</Value></Eq><Or><Contains><FieldRef Name='EmployeeDisplayName' /><Value Type='Text'>{nameOrId}</Value></Contains><Contains><FieldRef Name='EmployeeID' /><Value Type='Text'>{nameOrId}</Value></Contains></Or></And></Where>";
            commonAccountList = ExecuteQuery(queryStr);

            return commonAccountList;
        }
        public List<EmployeeInfo> GetAccountByFullNameOrId(string name, string departmentName)
        {
            List<EmployeeInfo> commonAccountList = new List<EmployeeInfo>();

            string queryStr = $"<Where><And><Eq><FieldRef Name='EmployeeInfoDepartment' /><Value Type='Lookup'>{departmentName}</Value></Eq><Or><Contains><FieldRef Name='EmployeeDisplayName' /><Value Type='Text'>{name}</Value></Contains><Contains><FieldRef Name='EmployeeID' /><Value Type='Text'>{name}</Value></Contains></Or></And></Where>";
            commonAccountList = ExecuteQuery(queryStr);

            return commonAccountList;
        }

        private List<EmployeeInfo> ExecuteQuery(string queryStr, string[] viewFields = null)
        {
            List<EmployeeInfo> commonAccountList = new List<EmployeeInfo>();

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        var employeeItems = this.GetByQueryToSPListItemCollection(spWeb, queryStr);
                        if (employeeItems != null && employeeItems.Count > 0)
                        {
                            foreach (SPListItem employeeItem in employeeItems)
                            {
                                var employeeInfo = ParseToEntity(employeeItem);
                                commonAccountList.Add(employeeInfo);
                            }
                        }
                    }
                }
            }
            else
            {
                SPListItemCollection employeeItems = null;
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    employeeItems = this.GetByQueryToSPListItemCollection(currentWeb, queryStr);
                }
                else
                {
                    employeeItems = this.GetByQueryToSPListItemCollection(SPContext.Current.Site.RootWeb, queryStr);
                }

                if (employeeItems != null && employeeItems.Count > 0)
                {
                    foreach (SPListItem employeeItem in employeeItems)
                    {
                        var employeeInfo = ParseToEntity(employeeItem);
                        commonAccountList.Add(employeeInfo);
                    }
                }
            }

            return commonAccountList;
        }

        public List<EmployeeInfo> GetAccountByFullNamePosition(string name, string departmentName, List<EmployeePosition> EmployeePostion)
        {
            List<EmployeeInfo> commonAccountList = new List<EmployeeInfo>();
            var conditionList = string.Empty;

            foreach (var Postion in EmployeePostion)
            {
                conditionList += string.Format("<Value Type='Text'>{0}</Value>", Postion.PositionName);
            }

            string queryString = $@"<Where>
                                        <And>
                                            <In>
                                                <FieldRef Name='Position' /><Values>{conditionList}</Values>
                                            </In>
                                            <Eq>
                                                <FieldRef Name='EmployeeInfoDepartment' />
                                                <Value Type='Lookup'>{departmentName}</Value>
                                            </Eq>
                                        </And>
                                    </Where>";

            if (string.IsNullOrEmpty(departmentName))
            {
                queryString = $@"<Where>
                                    <And>
                                        <In>
                                            <FieldRef Name='Position' /><Values>{conditionList}</Values>
                                        </In>
                                        <IsNull>
                                            <FieldRef Name='EmployeeInfoDepartment' />
                                        </IsNull>
                                    </And>
                                </Where>";
            }

            commonAccountList = ExecuteQuery(queryString);

            return commonAccountList;
        }

        public List<EmployeeInfo> GetAccountByFullNamePositionDepartment(string employeeSearchName, List<int> positionIds, int? departmentId, string[] viewFields = null)
        {
            List<EmployeeInfo> commonAccountList = new List<EmployeeInfo>();
            if (positionIds == null || positionIds.Count == 0)
            {
                return commonAccountList;
            }

            string positionValues = "";
            foreach (var positionId in positionIds)
            {
                positionValues += string.Format("<Value Type='Lookup'>{0}</Value>", positionId);
            }

            string queryString = "";
            if (departmentId != null)
            {
                queryString = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{departmentId}</Value>
                                        </Eq>
                                        <In>
                                            <FieldRef Name='Position' LookupId='TRUE'/><Values>{positionValues}</Values>
                                        </In>
                                    </And>
                                </Where>";

            }
            else
            {
                queryString = $@"<Where>
                                    <In>
                                        <FieldRef Name='Position' LookupId='TRUE'/><Values>{positionValues}</Values>
                                    </In>
                                </Where>";
            }

            commonAccountList = ExecuteQuery(queryString, viewFields);

            return commonAccountList;
        }

        /// <summary>
        /// Get Employee Info by Employee ID
        /// </summary>
        /// <param name="employeeID">Employee ID as string</param>
        /// <returns>Employee Info Entity</returns>
        public EmployeeInfo GetByADAccount(int adAccountId)
        {
            EmployeeInfo employeeInfo = null;

            string queryStr = $"<Where><Eq><FieldRef Name='ADAccount' LookupId='TRUE'/><Value Type='User'>{adAccountId}</Value></Eq></Where>";

            List<EmployeeInfo> ret = ExecuteQuery(queryStr);
            if (ret != null && ret.Count > 0)
            {
                employeeInfo = ret[0];
            }

            return employeeInfo;
        }

        public EmployeeInfo GetByADAccount(string accountUsername, string[] viewFields = null)
        {
            EmployeeInfo employeeInfo = null;
            SPUser user = null;
            if (SPContext.Current != null)
            {
                var currentWeb = SPContext.Current.Web;
                user = currentWeb.EnsureUser(accountUsername);
                if (user != null)
                {
                    string queryStr = $"<Where><Eq><FieldRef Name='ADAccount' LookupId='TRUE'/><Value Type='User'>{user.ID}</Value></Eq></Where>";
                    var employeeItems = this.GetByQueryToSPListItemCollection(currentWeb, queryStr, viewFields);
                    if (employeeItems != null && employeeItems.Count > 0)
                    {
                        // Get First and Parse to EmployeeInfo
                        var employeeItem = employeeItems[0];
                        employeeInfo = ParseToEntity(employeeItem);
                    }
                }
            }
            else
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        user = web.EnsureUser(accountUsername);
                        if (user != null)
                        {
                            string queryStr = $"<Where><Eq><FieldRef Name='ADAccount' LookupId='TRUE'/><Value Type='User'>{user.ID}</Value></Eq></Where>";
                            var employeeItems = this.GetByQueryToSPListItemCollection(web, queryStr, viewFields);
                            if (employeeItems != null && employeeItems.Count > 0)
                            {
                                // Get First and Parse to EmployeeInfo
                                var employeeItem = employeeItems[0];
                                employeeInfo = ParseToEntity(employeeItem);
                            }
                        }
                    }
                }
            }

            return employeeInfo;
        }

        /// <summary>
        /// Get Employee Info by Employee ID
        /// </summary>
        /// <param name="employeeID">Employee ID as string</param>
        /// <returns>Employee Info Entity</returns>
        public List<EmployeeInfo> GetByDepartment(int departmentId, List<int> locationIds, string[] viewFields = null)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();

            string queryStr = $"<Eq><FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE'/><Value Type='Lookup'>{departmentId}</Value></Eq>";

            if (locationIds != null && locationIds.Count > 0 && !locationIds.Contains(0))
            {
                var locationFilter = CommonHelper.BuildFilterCommonLocation(locationIds);
                if (!string.IsNullOrEmpty(locationFilter))
                {
                    queryStr = $"<And>{queryStr}{locationFilter}</And>";
                }
            }

            queryStr = $"<Where>{queryStr}</Where><OrderBy><FieldRef Name = '{StringConstant.EmployeeInfoList.FullNameField}' Ascending='True'/></OrderBy>";
            employeeInfos = ExecuteQuery(queryStr, viewFields);

            return employeeInfos;
        }

        public List<EmployeeInfo> GetByDepartment(int departmentId, List<int> locationIds, double maxLevel, string[] viewFields = null)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();

            string queryStr = $"<And><Eq><FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>1</Value></Eq><And><Eq><FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE'/><Value Type='Lookup'>{departmentId}</Value></Eq><Leq><FieldRef Name='{StringConstant.EmployeeInfoList.EmployeeLevelField}' /><Value Type='Lookup'>{maxLevel}</Value></Leq></And></And>";

            if (locationIds != null && locationIds.Count > 0 && !locationIds.Contains(0))
            {
                var locationFilter = CommonHelper.BuildFilterCommonLocation(locationIds);
                if (!string.IsNullOrEmpty(locationFilter))
                {
                    queryStr = $"<And>{queryStr}{locationFilter}</And>";
                }
            }

            queryStr = $"<Where>{queryStr}</Where><OrderBy><FieldRef Name = '{StringConstant.EmployeeInfoList.FullNameField}' Ascending='True'/></OrderBy>";
            employeeInfos = ExecuteQuery(queryStr, viewFields);

            return employeeInfos;
        }

        /// <summary>
        /// Get Employee Info by Employee ID
        /// </summary>
        /// <param name="employeeID">Employee ID as string</param>
        /// <returns>Employee Info Entity</returns>
        public List<EmployeeInfo> GetByDepartment(int departmentId, List<int> locationIds, bool active, string[] viewFields = null)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();
            int isActive = active ? 1 : 0;

            string queryStr = $"<And><Eq><FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE'/><Value Type='Lookup'>{departmentId}</Value></Eq><Eq><FieldRef Name='IsActive'/><Value Type='Boolean'>{isActive}</Value></Eq></And>";

            if (locationIds != null && locationIds.Count > 0 && !locationIds.Contains(0))
            {
                var locationFilter = CommonHelper.BuildFilterCommonLocation(locationIds);
                if (!string.IsNullOrEmpty(locationFilter))
                {
                    queryStr = $"<And>{queryStr}{locationFilter}</And>";
                }
            }

            queryStr = $"<Where>{queryStr}</Where>";
            employeeInfos = ExecuteQuery(queryStr, viewFields);

            return employeeInfos;
        }

        public List<EmployeeInfo> GetByDepartments(List<int> departmentIds, int locationId, bool active, string[] viewFields = null)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();
            int isActive = active ? 1 : 0;

            string queryStr = $"<And><Eq><FieldRef Name='CommonLocation' LookupId='TRUE'/><Value Type='Lookup'>{locationId}</Value></Eq><Eq><FieldRef Name='IsActive'/><Value Type='Boolean'>{isActive}</Value></Eq></And>";

            if (departmentIds != null && departmentIds.Count > 0 && !departmentIds.Contains(0))
            {
                var departmentFilter = CommonHelper.BuildFilterCommonDepartment(departmentIds);
                if (!string.IsNullOrEmpty(departmentFilter))
                {
                    queryStr = $"<And>{queryStr}{departmentFilter}</And>";
                }
            }

            queryStr = $"<Where>{queryStr}</Where>";
            employeeInfos = ExecuteQuery(queryStr, viewFields);

            return employeeInfos;
        }

        /// <summary>
        /// Get List of employee by location and department
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="departmentId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public List<EmployeeInfo> GetByLocationAndDepartment(int locationId, int departmentId, bool active, int maxLevel, string orderByField, string[] viewFields = null)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();
            int isActive = active ? 1 : 0;

            string queryStr = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{StringConstant.EmployeeInfoList.FactoryLocationField}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{locationId}</Value>
                                        </Eq>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}' LookupId='TRUE' />
                                                <Value Type='Lookup'>{departmentId}</Value>
                                            </Eq>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>{isActive}</Value>
                                                </Eq>
                                                <Lt>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeeLevelField}' />
                                                    <Value Type='Lookup'>{maxLevel}</Value>
                                                </Lt>
                                            </And>
                                        </And>
                                    </And>
                                </Where>
                                <OrderBy>
                                    <FieldRef Name='{orderByField}' Ascending='TRUE' />
                                </OrderBy>";

            employeeInfos = ExecuteQuery(queryStr, viewFields);
            
            return employeeInfos;
        }

        public List<EmployeeInfo> GetByMinLevel(bool active, double minLevel)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();
            int isActive = active ? 1 : 0;
            //// Query SPList
            employeeInfos =
                GetByQuery($@"<Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>{isActive}</Value>
                                    </Eq>
                                    <Geq>
                                        <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeeLevelField}' />
                                        <Value Type='Lookup'>{minLevel}</Value>
                                    </Geq>
                                </And>
                              </Where>");
            return employeeInfos;
        }

        /// <summary>
        /// get by user Type
        /// </summary>
        /// <param name="active"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        public List<EmployeeInfo> GetByUserType(bool active, string userType)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();
            int isActive = active ? 1 : 0;
            //// Query SPList
            employeeInfos =
                GetByQuery($@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeeTypeField}' />
                                        <Value Type='Choice'>{userType}</Value>
                                     </Eq>
                                     <Eq>
                                        <FieldRef Name='IsActive' />
                                        <Value Type='Boolean'>{isActive}</Value>
                                     </Eq>
                                  </And>
                               </Where>");
            return employeeInfos;
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

            //// Delegate!

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        spWeb.AllowUnsafeUpdates = true;

                        // Query SPList
                        SPList list = spWeb.GetList(spWeb.Url + ListUrl);
                        SPQuery spQuery = new SPQuery
                        {
                            Query =
                                $@"<Where><Eq><FieldRef Name='EmployeeID' /><Value Type='Text'>{employeeID}</Value></Eq></Where>"
                        };
                        var employeeInfoListItems = list.GetItems(spQuery);
                        var newPasswordEncrypt = StringCipher.GetMd5Hash(newPassword);
                        // Get First and Parse to EmployeeInfo
                        foreach (SPListItem empoListItem in employeeInfoListItems)
                        {
                            empoListItem[StringConstant.EmployeeInfoList.PasswordField] = newPasswordEncrypt;
                            empoListItem.Update();
                        }

                        result = true;

                        spWeb.AllowUnsafeUpdates = false;
                    }
                }
            });

            return result;
        }

        public List<EmployeeInfo> GetByPositionDepartment(StringConstant.EmployeePosition position, int departmentId, int locationId, string[] viewFields = null)
        {
            List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();
            int isActive = 1;

            string queryStr = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='Position' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{(int)position}</Value>
                                        </Eq>
                                        <And>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE' />
                                                    <Value Type='Lookup'>{departmentId}</Value>
                                                </Eq>
                                                <Eq>
                                                    <FieldRef Name='CommonLocation' LookupId='TRUE'/>
                                                    <Value Type='Lookup'>{locationId}</Value>
                                                 </Eq>
                                            </And>
                                            <Eq>
                                                <FieldRef Name='IsActive' />
                                                <Value Type='Boolean'>{isActive}</Value>
                                            </Eq>
                                        </And>
                                    </And>
                                </Where>";

            employeeInfos = ExecuteQuery(queryStr, viewFields);

            return employeeInfos;
        }

        /// <summary>
        /// Parse SPListItem to Employee Info entity
        /// </summary>
        /// <param name="listItem">SPListItem from Employee Info List</param>
        /// <returns>Employee Info entity</returns>
        public override EmployeeInfo ParseToEntity(SPListItem listItem)
        {
            var employeeInfo = base.ParseToEntity(listItem);

            //var employeeInfo = new EmployeeInfo
            //{
            //    ID = listItem.ID,
            //    EmployeeID = listItem.ToString(StringConstant.EmployeeInfoList.EmployeeIDField),
            //    FullName = listItem.ToString(StringConstant.EmployeeInfoList.FullNameField),
            //    DepartmentPermission = listItem.ToString(StringConstant.EmployeeInfoList.DepartmentPermissionField),
            //    ADAccount = listItem.ToUserModel(StringConstant.EmployeeInfoList.ADAccountField),
            //    Department = listItem.ToLookupItemModel(StringConstant.EmployeeInfoList.DepartmentField),
            //    Manager = listItem.ToLookupItemModel(StringConstant.EmployeeInfoList.EmployeeInfoManagerField),
            //    Password = listItem[StringConstant.EmployeeInfoList.PasswordField].ToString(),
            //    EmployeeType = listItem.ToString(StringConstant.EmployeeInfoList.EmployeeTypeField),
            //    EmployeePosition = listItem.ToLookupItemModel(StringConstant.EmployeeInfoList.EmployeePositionField),
            //    FactoryLocation = listItem.ToLookupItemModel(StringConstant.EmployeeInfoList.FactoryLocationField),
            //    IsActive = Convert.ToBoolean(listItem[StringConstant.EmployeeInfoList.IsActiveField]),
            //    Email = listItem.ToString(StringConstant.EmployeeInfoList.EmailField),
            //    Image = Convert.ToString(listItem[StringConstant.EmployeeInfoList.PublishingPageImageField]),
            //    EmployeeLevel = listItem.ToLookupItemModel(StringConstant.EmployeeInfoList.EmployeeLevelField),
            //    DelegatedBy = listItem.ToLookupItemsModel(StringConstant.EmployeeInfoList.DelegatedByField)
            //};

            if (listItem.ListItems.Fields.ContainsFieldWithStaticName(StringConstant.EmployeeInfoList.JoinedDateField))
            {
                employeeInfo.JoinedDate = Convert.ToDateTime(listItem[StringConstant.EmployeeInfoList.JoinedDateField]);
            }

            return employeeInfo;
        }

        public void AddUserToDepartmentGroupPermission(Guid siteId, Guid webId, SPListItem employeeInfoItem)
        {
            EmployeeInfo employeeInfo = ParseToEntity(employeeInfoItem);
            if (employeeInfo != null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb currentWeb = site.OpenWeb(webId))
                        {
                            SPFieldUser spuserField = (SPFieldUser)employeeInfoItem.Fields.GetField(StringConstant.EmployeeInfoList.ADAccountField);
                            SPFieldUserValue spuserFieldValue = (SPFieldUserValue)spuserField.GetFieldValue(Convert.ToString(employeeInfoItem[StringConstant.EmployeeInfoList.ADAccountField]));

                            var positionId = employeeInfo.EmployeePosition.LookupId;
                            //Add User to Group
                            if ((employeeInfo.Department != null && employeeInfo.Department.LookupId > 0) && !string.IsNullOrEmpty(employeeInfo.DepartmentPermission))
                            {
                                string groupName = string.Empty;
                                if (employeeInfo.EmployeeType.Equals(StringConstant.EmployeeType.CommonUser)) //Group name for Common Account
                                {
                                    groupName = StringConstant.Group.CommonAccountGroupName;
                                }
                                else //Group name for AD Account
                                {
                                    groupName = string.Format("{0} {1}", employeeInfo.Department.LookupValue, employeeInfo.DepartmentPermission);
                                }
                                SPHelper.AddUserToGroup(currentWeb, spuserFieldValue.User, groupName);
                            }
                            else if (employeeInfo.Department == null || employeeInfo.Department.LookupId == 0)
                            {
                                if (positionId == (int)StringConstant.EmployeePosition.BOD)
                                {
                                    SPHelper.AddUserToGroup(currentWeb, spuserFieldValue.User, "BOD");
                                }
                            }
                        }
                    }
                });

            }
        }

        public void RemoveUserFromGroupPermission(Guid siteId, Guid webId, SPListItem employeeInfoItem)
        {
            EmployeeInfo employeeInfo = ParseToEntity(employeeInfoItem);
            if (employeeInfo != null)
            {
                if (employeeInfo.Department != null && !string.IsNullOrEmpty(employeeInfo.DepartmentPermission))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb currentWeb = site.OpenWeb(webId))
                            {
                                SPFieldUser spuserField = (SPFieldUser)employeeInfoItem.Fields.GetField(StringConstant.EmployeeInfoList.ADAccountField);
                                SPFieldUserValue spuserFieldValue = (SPFieldUserValue)spuserField.GetFieldValue(Convert.ToString(employeeInfoItem[StringConstant.EmployeeInfoList.ADAccountField]));

                                if (employeeInfo.EmployeeType.Equals(StringConstant.EmployeeType.ADUser))
                                {
                                    SPHelper.RemoveUserFromGroup(currentWeb, spuserFieldValue.User, string.Format("{0} {1}", employeeInfo.Department.LookupValue, employeeInfo.DepartmentPermission));
                                }
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Update user permission
        /// </summary>
        /// <param name="currenWeb"></param>
        /// <param name="employeeInfoItem"></param>
        public void UpdateUserGroupPermission(Guid siteId, Guid webId, SPListItem employeeInfoItem)
        {
            EmployeeInfo employeeInfo = ParseToEntity(employeeInfoItem);
            if (employeeInfo != null)
            {
                if (employeeInfo.Department != null && !string.IsNullOrEmpty(employeeInfo.DepartmentPermission))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb currentWeb = site.OpenWeb(webId))
                            {
                                SPFieldUser spuserField = (SPFieldUser)employeeInfoItem.Fields.GetField(StringConstant.EmployeeInfoList.ADAccountField);
                                SPFieldUserValue spuserFieldValue = (SPFieldUserValue)spuserField.GetFieldValue(Convert.ToString(employeeInfoItem[StringConstant.EmployeeInfoList.ADAccountField]));
                                if (!employeeInfo.IsActive)
                                {
                                    SPHelper.RemoveUserFromGroup(currentWeb, spuserFieldValue.User, string.Format("{0} {1}", employeeInfo.Department.LookupValue, employeeInfo.DepartmentPermission));
                                }
                                else
                                {
                                    SPHelper.AddUserToGroup(currentWeb, spuserFieldValue.User, string.Format("{0} {1}", employeeInfo.Department.LookupValue, employeeInfo.DepartmentPermission));
                                }
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Check whether current user in group or not
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool IsUserCurrentuserInGroup(string groupName)
        {
            //Check groupName is valid
            if (string.IsNullOrEmpty(groupName) || groupName.Length > 255)
                return false;
            var isMember = false;

            var siteID = SPContext.Current.Web.Site.ID;
            var currentUser = SPContext.Current.Web.CurrentUser;

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (var site = new SPSite(siteID))
                {
                    var web = site.RootWeb;
                    try
                    {
                        var group = web.SiteGroups.GetByName(groupName);
                        var user = group.Users.GetByLoginNoThrow(currentUser.LoginName);
                        if (user != null)
                        {
                            isMember = true;
                        }
                    }
                    catch (Exception)
                    {
                        isMember = false;
                    }
                }
            });
            return isMember;
        }

        public void AddUserToGroup(Guid siteId, Guid webId, SPListItem departmentInfoItem, string groupName, string newUser)
        {
            if (departmentInfoItem != null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb currentWeb = site.OpenWeb(webId))
                        {
                            SPFieldUser spUserField = (SPFieldUser)departmentInfoItem.Fields.GetField(StringConstant.DepartmentList.BODField);

                            if (spUserField != null)
                            {
                                // Add new admin
                                SPFieldUserValue spNewUserFieldValue = (SPFieldUserValue)spUserField.GetFieldValue(newUser);
                                SPUser spNewBodUser = null;
                                if (spNewUserFieldValue == null || string.IsNullOrEmpty(spNewUserFieldValue.LookupValue))
                                {
                                    int userId;
                                    if (int.TryParse(newUser, out userId))
                                    {
                                        spNewBodUser = currentWeb.AllUsers.GetByID(userId);
                                    }
                                }
                                else
                                    spNewBodUser = currentWeb.EnsureUser(spNewUserFieldValue.LookupValue);
                                SPHelper.AddUserToGroup(currentWeb, spNewBodUser, groupName);
                            }
                        }
                    }
                });

            }
        }

        public EmployeeInfo GetByDepartment(int ID, int departmentId, bool isActive)
        {
            var employee = new EmployeeInfo();
            int isActiveNumber = isActive ? 1 : 0;
            var items = GetByQuery($@"<Where>
              <And>
                 <Eq>
                    <FieldRef Name='ID' LookupId='TRUE'/>
                    <Value Type='Counter'>{ID}</Value>
                 </Eq>
                 <And>
                    <Eq>
                       <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}' LookupId='TRUE' />
                       <Value Type='Lookup'>{departmentId}</Value>
                    </Eq>
                    <Eq>
                       <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' />
                       <Value Type='Boolean'>{isActiveNumber}</Value>
                    </Eq>
                 </And>
              </And>
           </Where>", string.Empty);
            return items.FirstOrDefault();
        }

        /// <summary>
        /// GetByPositionAndDepartment
        /// </summary>
        /// <param name="employeePositionId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public List<EmployeeInfo> GetByPositionAndDepartment(int employeePositionId, int? departmentId)
        {
            List<EmployeeInfo> employees = null;

            if (employeePositionId > 0)
            {
                string queryString = string.Empty;
                if (departmentId.HasValue)
                {
                    queryString = $@"<Where>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>1</Value>
                                                </Eq>
                                                <And> 
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeePositionField}' LookupId='TRUE'/>
                                                        <Value Type='Lookup'>{employeePositionId}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}' LookupId='TRUE'/>
                                                        <Value Type='Lookup'>{departmentId.Value}</Value>
                                                    </Eq>
                                                </And>            
                                            </And>
                                    </Where>";
                }
                else
                {
                    queryString = $@"<Where>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>1</Value>
                                                </Eq>
                                                <And> 
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeePositionField}' LookupId='TRUE'/>
                                                        <Value Type='Lookup'>{employeePositionId}</Value>
                                                    </Eq>
                                                    <IsNull>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}'></FieldRef>
                                                    </IsNull>
                                                </And>            
                                            </And>
                                    </Where>";
                }
                employees = GetByQuery(queryString, string.Empty);
            }

            return employees;
        }

        /// <summary>
        /// GetByPosition
        /// </summary>
        /// <param name="employeePositionId"></param>
        /// <returns></returns>
        public List<EmployeeInfo> GetByPosition(int employeePositionId)
        {
            List<EmployeeInfo> employees = null;

            string queryString = $@"<Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>1</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeePositionField}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{employeePositionId}</Value>
                                            </Eq>           
                                        </And>
                                    </Where>";

            employees = GetByQuery(queryString);

            return employees;
        }

        public List<EmployeeInfo> GetDelegatedEmployees(int employeePositionId, int delegatedByPositionId)
        {
            List<EmployeeInfo> employees = null;

            string queryString = $@"<Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>1</Value>
                                            </Eq>
                                            <Or>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeePositionField}' LookupId='TRUE'/>
                                                    <Value Type='Lookup'>{employeePositionId}</Value>
                                                </Eq>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.DelegatedByField}' LookupId='TRUE'/>
                                                    <Value Type='Lookup'>{delegatedByPositionId}</Value>
                                                </Eq>
                                            </Or>
                                        </And>
                                    </Where>";

            employees = GetByQuery(queryString);

            return employees;
        }

        public List<EmployeeInfo> GetDelegatedEmployeesByDepartment(int delegatedByPositionId, int? departmentId, int locationId)
        {
            List<EmployeeInfo> employees = null;

            string departmentFilterStr = "";
            if (departmentId.HasValue)
            {
                departmentFilterStr = $@"<Eq>
                                            <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}' LookupId='TRUE' /><Value Type='Lookup'>{departmentId}</Value>
                                        </Eq>";
            }
            else
            {
                departmentFilterStr = $@"<IsNull>
                                            <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}'></FieldRef>
                                        </IsNull>";
            }

            string queryString = $@" < Where>
                                        <And>
                                            <And>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.FactoryLocationField}' LookupId='TRUE' />
                                                        <Value Type='Lookup'>{locationId}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>1</Value>
                                                    </Eq>
                                                </And>
                                                {departmentFilterStr}
                                            </And>
                                            <Eq>
                                                <FieldRef Name='{StringConstant.EmployeeInfoList.DelegatedByField}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{delegatedByPositionId}</Value>
                                            </Eq>
                                        </And>
                                    </Where>";

            employees = GetByQuery(queryString);

            return employees;
        }

        public List<EmployeeInfo> GetDelegatedEmployeesByDepartment(int employeePositionId, int delegatedByPositionId, int? departmentId, int locationId)
        {
            List<EmployeeInfo> employees = null;

            string departmentFilterStr = "";
            if (departmentId.HasValue)
            {
                departmentFilterStr = $@"<Eq>
                                            <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}' LookupId='TRUE' /><Value Type='Lookup'>{departmentId}</Value>
                                        </Eq>";
            }
            else
            {
                departmentFilterStr = $@"<IsNull>
                                            <FieldRef Name='{StringConstant.EmployeeInfoList.DepartmentField}'></FieldRef>
                                        </IsNull>";
            }

            string queryString = $@"<Where>
                                        <And>
                                            <And>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.FactoryLocationField}' LookupId='TRUE' />
                                                        <Value Type='Lookup'>{locationId}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='{StringConstant.EmployeeInfoList.IsActiveField}' /><Value Type='Boolean'>1</Value>
                                                    </Eq>
                                                </And>
                                                
                                                {departmentFilterStr}
                                            </And>
                                            <Or>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.EmployeePositionField}' LookupId='TRUE'/>
                                                    <Value Type='Lookup'>{employeePositionId}</Value>
                                                </Eq>
                                                <Eq>
                                                    <FieldRef Name='{StringConstant.EmployeeInfoList.DelegatedByField}' LookupId='TRUE'/>
                                                    <Value Type='Lookup'>{delegatedByPositionId}</Value>
                                                </Eq>
                                            </Or>
                                        </And>
                                    </Where>";

            employees = GetByQuery(queryString);

            return employees;
        }
    }
}
