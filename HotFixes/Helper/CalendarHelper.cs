using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.HotFixes.CalendarList
{
    public class CalendarHelper
    {
        public static void ChangeFillInChoices(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                List<string> listNames = new List<string>() { "Company Calendar - Location 1", "Company Calendar - Location 2", "Calendar" };
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            foreach (SPWeb spWeb in spSite.AllWebs)
                            {
                                Console.WriteLine("Web Url: {0}", spWeb.Url);
                                foreach (string listName in listNames)
                                {
                                    SPList spList = spWeb.Lists.TryGetList(listName);
                                    if (spList != null)
                                    {
                                        Console.Write("List Name: {0}", listName);

                                        SPField spField = spList.Fields.TryGetFieldByStaticName("Category");
                                        if (spField != null)
                                        {
                                            spWeb.AllowUnsafeUpdates = true;
                                            SPFieldChoice spFieldChoice = spField as SPFieldChoice;

                                            if (!spFieldChoice.Choices.Contains("Other Events"))
                                            {
                                                spFieldChoice.Choices.Add("Other Events");
                                            }

                                            spFieldChoice.FillInChoice = false;
                                            spFieldChoice.Update();
                                            spWeb.AllowUnsafeUpdates = false;
                                        }

                                        Console.WriteLine(" -> Done");
                                    }
                                }

                                spWeb.Close();
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
