using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.HotFixes.Helper
{
    static class MoveData
    {
        public static void MoveDateUserToMultiUser(string siteUrl, string listName, string sourceColumn, string destinationColumn)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    if (!string.IsNullOrEmpty(siteUrl))
                    {
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                SPList spList = spWeb.Lists.TryGetList(listName);
                                if (spList != null)
                                {
                                    SPQuery spQuery = new SPQuery()
                                    {
                                        RowLimit = 100,
                                        Query = "<Where><Gt><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Gt></Where>",
                                    };

                                    do
                                    {
                                        SPListItemCollection items = spList.GetItems(spQuery);
                                        foreach (SPListItem item in items)
                                        {
                                            var userFieldValue = item[item.Fields.GetFieldByInternalName(sourceColumn).Id] + string.Empty;
                                            if (!string.IsNullOrEmpty(userFieldValue))
                                            {
                                                SPFieldUserValueCollection multiUserFieldValue = new SPFieldUserValueCollection(spWeb, userFieldValue);
                                                item[item.Fields.GetFieldByInternalName(destinationColumn).Id] = multiUserFieldValue;
                                                item.SystemUpdate(true);
                                            }
                                        }

                                        spQuery.ListItemCollectionPosition = items.ListItemCollectionPosition;
                                    }
                                    while (spQuery.ListItemCollectionPosition != null);
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("Cannot find the list '{0}'", listName));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}
