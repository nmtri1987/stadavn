using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.HotFixes.Helper
{
    public class TaskListHelper
    {
        public static void UpdateTypeOfDateTime(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                List<string> colNames = new List<string>() { "StartDate", "DueDate" };
                string listName = "Task List";
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        Console.Write("Processing...");
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                SPList spList = spWeb.Lists.TryGetList(listName);
                                if (spList != null)
                                {
                                    Console.WriteLine();
                                    Console.Write("List Name: {0}", listName);

                                    spWeb.AllowUnsafeUpdates = true;
                                    foreach (var colName in colNames)
                                    {
                                        SPField spField = spList.Fields.TryGetFieldByStaticName(colName);
                                        if (spField != null)
                                        {
                                            Console.WriteLine();
                                            Console.Write("Column Name: {0}", colName);

                                            SPFieldDateTime spFieldDateTime = spField as SPFieldDateTime;
                                            if (!(spFieldDateTime.FriendlyDisplayFormat == SPDateTimeFieldFriendlyFormatType.Disabled))
                                            {
                                                spFieldDateTime.FriendlyDisplayFormat = SPDateTimeFieldFriendlyFormatType.Disabled;
                                                spFieldDateTime.Update();
                                            }
                                            Console.Write(" -> Done");
                                        }
                                    }
                                    spWeb.AllowUnsafeUpdates = false;
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message));
                }
                Console.Read();
            }
            else
            {
                Console.Write("Troll?");
                Console.Read();
            }
        }
    }
}
