using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Extension;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{

    public class PermissionGroupDAL : BaseDAL<PermissionGroup>
    {
        public PermissionGroupDAL(string siteUrl) : base(siteUrl)
        {
        }

        public bool IsAuthorizedOnPage(SPWeb web, string namePage, IList<string> groupIds)
        {
            //Check secured page
            if (!IsSecuredPage(web, namePage))
            {
                return true;
            }

            var conditionList = new List<string>();
            if (groupIds != null)
            {
                foreach (var groupId in groupIds)
                {
                    conditionList.Add("<Eq><FieldRef Name='GroupAccess'/><Value Type='Lookup'>" + groupId + "</Value></Eq>");
                }
            }
            var groupQuery = MergeCAMLConditions(conditionList, MergeType.Or);

            SPList splist = web.GetList(web.Url + ListUrl);
            SPQuery spquery = null;
            if (!string.IsNullOrEmpty(groupQuery))
            {
                spquery = new SPQuery
                {
                    Query = $@"<Where>
                                    <And>
                                        {groupQuery}
                                        <Eq>
                                        <FieldRef Name='PageName' />
                                        <Value Type='Text'>{namePage}</Value>
                                        </Eq>
                                    </And>
                                </Where>"
                };
            }
            else
            {
                return false;
            }

            spquery.RowLimit = 1;
            spquery.ViewFields = "<FieldRef Name='ID' />";
            spquery.ViewFieldsOnly = true;
            var items = splist.GetItems(spquery);
            if (items != null && items.Count > 0)
            {
                return true;
            }

            return false;
        }

        private bool IsSecuredPage(SPWeb spweb, string namePage)
        {
            SPList splist = null;
            splist = spweb.TryGetSPList(SiteUrl + ListUrl);

            if (splist != null)
            {
                SPQuery spquery = new SPQuery
                {
                    Query =
                        $@"<Where>
                                <Eq>
                                    <FieldRef Name='PageName' />
                                    <Value Type='Text'>{namePage}</Value>
                                </Eq>
                        </Where>"
                };
                spquery.ViewFields = "<FieldRef Name='ID' />";
                spquery.ViewFieldsOnly = true;
                var items = splist.GetItems(spquery);
                if (items != null && items.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public IList<PermissionGroup> GetPagesOnLeftMenu(List<string> groupIds)
        {
            var permissionGroupList = new List<PermissionGroup>();

            var conditionList = new List<string>();
            if (groupIds != null)
            {
                foreach (var groupId in groupIds)
                {
                    conditionList.Add("<Eq><FieldRef Name='GroupAccess'/><Value Type='Lookup'>" + groupId + "</Value></Eq>");
                }
            }
            var groupQuery = MergeCAMLConditions(conditionList, MergeType.Or);

            if (!string.IsNullOrEmpty(groupQuery))
            {
                string queryStr = $@"<Where>
                                          <And>
                                             {groupQuery}
                                             <Eq>
                                                <FieldRef Name='IsOnLeftMenu' />
                                                <Value Type='Boolean'>1</Value>
                                             </Eq>
                                          </And>
                                       </Where>";

                permissionGroupList = GetByQuery(queryStr);
            }

            return permissionGroupList;
        }

        public IList<PermissionGroup> GetByModuleCategoryId(int moduleCategoryId)
        {
            var PermissionGroupList = new List<PermissionGroup>();

            var query = $@"<Where><Eq>
                              <FieldRef Name='PermissionModuleCategory' LookupId='TRUE'  />
                                      <Value Type='Lookup'>{moduleCategoryId}</Value> 
                          </Eq></Where>";

            PermissionGroupList = GetByQuery(query, "");
            return PermissionGroupList;
        }
    }
}
