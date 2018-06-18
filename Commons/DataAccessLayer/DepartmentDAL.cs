//-----------------------------------------------------------------------
// <copyright file="DepartmentDAL.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the DepartmentDAL class.</summary>
//-----------------------------------------------------------------------

using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    using Constants;
    using Core.SharePoint;
    using Helpers;
    using Microsoft.SharePoint;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Department Data Access Layer
    /// </summary>
    public class DepartmentDAL : BaseDAL<Department>
    {
        // TODO: Will move this value to list configure
        private const string DepartmentTemplateName = "Department Site";

        public DepartmentDAL(string siteUrl) : base(siteUrl)
        {
        }

        public void CreateDepartmentsSite(Guid siteId, Guid webId, SPListItemCollection departmentItems)
        {
            foreach (SPListItem departmentItem in departmentItems)
            {
                CreateDepartmentSite(siteId, webId, departmentItem);
            }
        }

        public List<Department> GetDepartmentsByLocation(List<int> locationIds)
        {
            var listResult = new List<Department>();
            try
            {
                if (locationIds == null || (locationIds != null && locationIds.Count == 0) || (locationIds != null && locationIds.Contains(0)))
                {
                    listResult = this.GetAll();
                }
                else
                {
                    var locationFilter = CommonHelper.BuildFilterCommonMultiLocations(locationIds);
                    if (!string.IsNullOrEmpty(locationFilter))
                    {
                        listResult = GetByQuery($"<Where>{locationFilter}</Where>", string.Empty);
                    }
                }
            }
            catch { }

            return listResult;
        }

        /// <summary>
        /// Create department sub site from site template
        /// </summary>
        /// <param name="currentWeb">SharePoint Current web</param>
        /// <param name="departmentItem">Department List Item</param>
        public void CreateDepartmentSite(Guid siteId, Guid webId, SPListItem departmentItem)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb currentWeb = site.OpenWeb(webId))
                    {
                        var webTemplates = currentWeb.Site.RootWeb.GetAvailableWebTemplates(1033);
                        SPWebTemplate webTemplate = null;

                        for (var i = 0; i < webTemplates.Count; i++)
                        {
                            var webTemplateSearchName = webTemplates[i].Title;

                            if (!webTemplateSearchName.Contains(DepartmentTemplateName)) continue;

                            webTemplate = webTemplates[i];
                            break;
                        }

                        if (webTemplate == null) return;
                        var department = ParseToEntity(departmentItem);
                        // check web existed or not
                        var webExisted = currentWeb.Webs.FirstOrDefault(w => w.Title == department.Name);
                        if (webExisted != null)
                        {
                            CreateUpdateDepartmentGroup(webExisted, currentWeb, department.Name);
                            return;
                        }

                        // Default language English
                        var newSite = currentWeb.Webs.Add(department.Code, department.Name, department.Name, Convert.ToUInt16(1033), webTemplate, false, false);

                        newSite.AllowUnsafeUpdates = true;
                        newSite.Navigation.UseShared = false;

                        //Set default home pages
                        var rootFolder = newSite.RootFolder;
                        rootFolder.WelcomePage = "SitePages/Home.aspx";
                        rootFolder.Update();
                        //SPHelper.ActiveInstalledLanguage(newSite);

                        // Update language resource for Site title
                        CreateUpdateDepartmentGroup(newSite, currentWeb, department.Name);
                        SPHelper.ActiveInstalledLanguage(newSite);
                        foreach (var culture in newSite.SupportedUICultures)
                        {
                            switch (culture.LCID)
                            {
                                case 1066:
                                    newSite.TitleResource.SetValueForUICulture(culture, department.VietnameseName);
                                    break;

                                default:
                                    newSite.TitleResource.SetValueForUICulture(culture, department.Name);
                                    break;
                            }
                        }
                        newSite.Update();
                    }
                }
            });
        }

        private void CreateUpdateDepartmentGroup(SPWeb departmentWeb, SPWeb parentWeb, string departmentName)
        {
            string contributorsGroupName = departmentName + " Contributors";
            string membersGroupName = departmentName + " Members";
            string administratorsGroupName = departmentName + " Administrators";

            SPRoleDefinitionCollection roleColl = departmentWeb.RoleDefinitions;

            CreateSubSiteGroup(departmentWeb, contributorsGroupName, SPRoleType.Contributor, "Description for " + departmentName + " Contributors group");
            CreateSubSiteGroup(departmentWeb, membersGroupName, SPRoleType.Reader, "Description for " + departmentName + " Members group");
            CreateSubSiteGroup(departmentWeb, administratorsGroupName, SPRoleType.Reader, "Description for " + departmentName + " Administrator group");
            AddGroupToParentWeb(parentWeb, contributorsGroupName, SPRoleType.Reader);
            AddGroupToParentWeb(parentWeb, membersGroupName, SPRoleType.Reader);
            AddGroupToParentWeb(parentWeb, administratorsGroupName, SPRoleType.Reader);

            AddGroupNameToGroupList(contributorsGroupName, parentWeb);
            AddGroupNameToGroupList(membersGroupName, parentWeb);
            AddGroupNameToGroupList(administratorsGroupName, parentWeb);
        }

        public void AddGroupNameToGroupList(string groupName, SPWeb parentWeb)
        {
            GroupDAL groupDal = new GroupDAL(parentWeb.Site.Url);
            var group = groupDal.GetByName(groupName);
            if (group == null)
            {
                //Add to list
                groupDal.Insert(new Group { Name = groupName });
            }
        }

        private void AddGroupToParentWeb(SPWeb web, string groupName, SPRoleType roleType)
        {
            SPUserCollection users = web.AllUsers;
            SPGroup group = web.SiteGroups.GetByName(groupName);
            if (group != null)
            {
                SPRoleDefinition role = web.RoleDefinitions.GetByType(roleType);
                SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
                roleAssignment.RoleDefinitionBindings.Add(role);
                web.RoleAssignments.Add(roleAssignment);
                web.Update();
            }
        }

        private void CreateSubSiteGroup(SPWeb web, string groupName, SPRoleType roleType, string groupDescription)
        {
            var groupInSite = GroupExistsInSiteCollection(web, groupName);
            if (!groupInSite)
            {
                web.AllowUnsafeUpdates = true;
                web.Update();
                web.BreakRoleInheritance(false);

                SPGroupCollection groups = web.SiteGroups;
                //for (int i = 0; i < web.SiteAdministrators.Count; i++)
                //{
                //    SPUser owner = web.SiteAdministrators[i];
                //    SPMember member = web.SiteAdministrators[i]; 
                //}
                SPUser owner = web.SiteAdministrators[0];
                SPMember member = web.SiteAdministrators[0];

                groups.Add(groupName, member, owner, groupDescription);
                SPGroup newSPGroup = groups[groupName];

                SPRoleDefinition role = web.RoleDefinitions.GetByType(roleType);

                SPRoleAssignment roleAssignment = new SPRoleAssignment(newSPGroup);
                roleAssignment.RoleDefinitionBindings.Add(role);
                web.RoleAssignments.Add(roleAssignment);
                web.Update();
            }
        }



        public void UpdateSubSiteTitle(Guid siteId, Guid webId, SPListItem departmentItem, string newEnName, string newVnName)
        {
            //find web by web url
            var department = ParseToEntity(departmentItem);

            if (department != null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb currentWeb = site.OpenWeb(webId))
                        {
                            currentWeb.AllowUnsafeUpdates = true;
                            string subSiteUrl = $"{currentWeb.Url}/{department.Code}";
                            var currentSubSite = currentWeb.Webs.Where(x => x.Url == subSiteUrl).FirstOrDefault();
                            //update url
                            if (currentSubSite != null)
                            {
                                foreach (var culture in currentSubSite.SupportedUICultures)
                                {
                                    switch (culture.LCID)
                                    {
                                        case 1066:
                                            if (!string.IsNullOrEmpty(newVnName))
                                            {
                                                currentSubSite.TitleResource.SetValueForUICulture(culture, newVnName);
                                            }
                                            break;

                                        default:
                                            if (!string.IsNullOrEmpty(newEnName))
                                            {
                                                currentSubSite.TitleResource.SetValueForUICulture(culture, newEnName);
                                            }
                                            break;
                                    }
                                }
                                currentSubSite.Update();
                            }
                            currentWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
        }

        private static bool GroupExistsInSiteCollection(SPWeb web, string name)
        {
            return web.SiteGroups.OfType<SPGroup>().Count(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) > 0;
        }

        /// <summary>
        /// Delete group 
        /// </summary>
        /// <param name="currentWeb"></param>
        /// <param name="departmentItem"></param>
        public void DeleteGroup(SPWeb currentWeb, Department department)
        {
            if (department != null)
            {
                //Delete administrators group
                SPHelper.DeleleSPGroup(currentWeb, string.Format("{0} {1}", department.Name, StringConstant.Group.Administrators));
                //Delete contributor group
                SPHelper.DeleleSPGroup(currentWeb, string.Format("{0} {1}", department.Name, StringConstant.Group.Contributors));
                //Delete member group
                SPHelper.DeleleSPGroup(currentWeb, string.Format("{0} {1}", department.Name, StringConstant.Group.Members));
            }
        }

        public void DeleteSite(Guid siteId, Guid webId, Department department)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb currentWeb = site.OpenWeb(webId))
                    {
                        string webUrl = string.Format("{0}/{1}", currentWeb.Url, department.Code);
                        SPWeb webToDelete = currentWeb.Webs.FirstOrDefault(x => x.Url == webUrl);
                        if (webToDelete != null)
                        {
                            webToDelete.Delete();
                        }

                        // Delete related group
                        DeleteGroup(currentWeb, department);
                    }
                }
            });
        }

        /// <summary>
        /// Get list of department by IsShiftRequestRequired
        /// </summary>
        /// <param name="IsShiftRequestRequired"></param>
        /// <returns></returns>
        public List<Department> GetByShiftRequestRequired(bool IsShiftRequestRequired, List<int> locationIds)
        {
            List<Department> departments = new List<Department>();
            string boolString = IsShiftRequestRequired ? "1" : "0";
            string query = $"<Eq><FieldRef Name='IsShiftRequestRequired' /><Value Type='Boolean'>{boolString}</Value></Eq>";

            if (locationIds != null && locationIds.Count > 0 && !locationIds.Contains(0))
            {
                var locationFilter = CommonHelper.BuildFilterCommonMultiLocations(locationIds);
                if (!string.IsNullOrEmpty(locationFilter))
                {
                    query = $"<And>{query}{locationFilter}</And>";
                }
            }

            departments = GetByQuery($"<Where>{query}</Where>", string.Empty);
            return departments;
        }

        /// <summary>
        /// Get list of department by locations
        /// </summary>
        /// <param name="IsShiftRequestRequired"></param>
        /// <returns></returns>
        public List<Department> GetByLocations(List<int> locationIds)
        {
            List<Department> departments = new List<Department>();
            var query = string.Empty;
            if (locationIds != null && locationIds.Count > 0 && !locationIds.Contains(0))
            {
                var locationFilter = CommonHelper.BuildFilterCommonMultiLocations(locationIds);
                if (!string.IsNullOrEmpty(locationFilter))
                {
                    query = $"{locationFilter}";
                }
            }

            departments = GetByQuery($"<Where>{query}</Where>", string.Empty);
            return departments;
        }

        /// <summary>
        /// Get department by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Department GetByCode(string code)
        {
            Department department = null;

            string queryStr = $@"<Where><Eq><FieldRef Name='Code' /><Value Type='Text'>{code}</Value></Eq></Where>";
            var items = this.GetByQuery(queryStr);
            if (items != null && items.Count > 0)
            {
                department = items[0];
            }

            return department;
        }

        public Department GetById(int Id)
        {
            Department department = null;

            string queryStr = $@"<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>{Id}</Value></Eq></Where>";
            var items = this.GetByQuery(queryStr);
            if (items != null && items.Count > 0)
            {
                department = items[0];
            }

            return department;
        }
    }
}