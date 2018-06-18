using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.HotFixes.Helper
{
    public class AddNewColToList
    {
        public static void AddDelegatedByColToList(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                string colName = "DelegatedBy";
                string listName = "Employees";
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        Console.Write("Processing...");
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                SPList employeePositionList = spWeb.Lists.TryGetList("Employee Position");
                                SPList spList = spWeb.Lists.TryGetList(listName);
                                if (spList != null && employeePositionList != null)
                                {
                                    Console.WriteLine();
                                    Console.Write("List Name: {0}", listName);

                                    spWeb.AllowUnsafeUpdates = true;
                                    if (spList.Fields.ContainsField(colName) == false)
                                    {
                                        spList.Fields.AddFieldAsXml(@"<Field ID='{18a99830-576c-4039-b6aa-e350d5ab8692}' Name='DelegatedBy' DisplayName='DelegatedBy' Type='LookupMulti' List='" + employeePositionList.ID.ToString() + "' ShowField='CommonName' Mult='TRUE' Required='FALSE' Group='Stada Columns'></Field>", false, SPAddFieldOptions.AddToDefaultContentType);
                                        spList.Update();

                                        SPField theField = spList.Fields[new Guid("{18a99830-576c-4039-b6aa-e350d5ab8692}")];
                                        theField.SchemaXml = @"<Field ID='{18a99830-576c-4039-b6aa-e350d5ab8692}' Name='DelegatedBy' DisplayName='$Resources:RBVHStadaLists,EmployeeInfo_DelegatedBy;' Type='LookupMulti' List='" + employeePositionList.ID.ToString() + "' ShowField='CommonName' Mult='TRUE' Required='FALSE' Group='Stada Columns'></Field>";
                                        theField.Update();

                                        Console.Write(" -> Done");
                                    }
                                    else
                                    {
                                        Console.Write(" -> Existed");
                                    }
                                    spWeb.AllowUnsafeUpdates = false;
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.Write("Cannot find list: {0}", listName);
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message));
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.Read();
            }
            else
            {
                Console.Write("Troll?");
                Console.Read();
            }
        }

        public static void AddColForSortToList(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                string colName = "ColForSort";
                List<string> listNameCollection = new List<string>() { "Change Shift Management", "Leave Of Absence For Overtime Management", "Vehicle Management", "Leave Management" }; //"Overtime Management", "Overtime Employee Details", 
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        Console.Write("Processing...");
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                #region CalculatedField
                                foreach (var listName in listNameCollection)
                                {
                                    SPList spList = spWeb.Lists.TryGetList(listName);
                                    if (spList != null)
                                    {
                                        Console.WriteLine();
                                        Console.Write("List Name: {0}", listName);

                                        spWeb.AllowUnsafeUpdates = true;
                                        if (spList.Fields.ContainsField(colName) == false)
                                        {
                                            spList.Fields.Add(colName, SPFieldType.Calculated, false);

                                            SPField spField = spList.Fields.TryGetFieldByStaticName(colName);
                                            if (spField != null)
                                            {
                                                SPFieldCalculated calculatedField = spField as SPFieldCalculated;
                                                calculatedField.OutputType = SPFieldType.Number;
                                                calculatedField.DisplayFormat = SPNumberFormatTypes.NoDecimal;
                                                calculatedField.Formula = "=IF([Approval Status]=\"Approved\",INT(\"7\"),IF([Approval Status]=\"Rejected\",INT(\"8\"),IF([Approval Status]=\"Cancelled\",INT(\"9\"),INT(\"0\"))))";
                                                calculatedField.Update();
                                                Console.Write(" -> Done");
                                            }
                                            else
                                            {
                                                Console.Write(" -> Failed");
                                            }
                                        }
                                        else
                                        {
                                            Console.Write(" -> Existed");
                                        }
                                        spWeb.AllowUnsafeUpdates = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                        Console.Write("Cannot find list: {0}", listName);
                                    }
                                }
                                #endregion

                                #region Create ColForSort for Overtime Management List
                                string overtimeManagementListName = "Overtime Management";
                                Console.WriteLine();
                                Console.Write("List Name: {0}", overtimeManagementListName);
                                SPList overtimeManagementList = spWeb.Lists.TryGetList(overtimeManagementListName);
                                if (overtimeManagementList != null)
                                {
                                    if (overtimeManagementList.Fields.ContainsField(colName) == false)
                                    {
                                        spWeb.AllowUnsafeUpdates = true;

                                        overtimeManagementList.Fields.Add(colName, SPFieldType.Number, false);
                                        SPField spField = overtimeManagementList.Fields.TryGetFieldByStaticName(colName);
                                        if (spField != null)
                                        {
                                            SPFieldNumber spFieldNumber = spField as SPFieldNumber;
                                            spFieldNumber.DisplayFormat = SPNumberFormatTypes.NoDecimal;
                                            spFieldNumber.Update();
                                            Console.Write(" -> Done");
                                        }
                                        else
                                        {
                                            Console.Write(" -> Failed");
                                        }

                                        spWeb.AllowUnsafeUpdates = false;
                                    }
                                    else
                                    {
                                        Console.Write(" -> Existed");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.Write("Cannot find list: {0}", overtimeManagementListName);
                                }
                                #endregion

                                #region Create CalculateField ColForSort for Overtime Employee Details list
                                string overtimeDetailsListName = "Overtime Employee Details";
                                Console.WriteLine();
                                Console.Write("List Name: {0}", overtimeDetailsListName);
                                SPList overtimeDetailsList = spWeb.Lists.TryGetList(overtimeDetailsListName);
                                SPList overtimeList = spWeb.Lists.TryGetList("Overtime Management");
                                if (overtimeDetailsList != null && overtimeList != null)
                                {
                                    if (overtimeDetailsList.Fields.ContainsField(colName) == false && overtimeList.Fields.ContainsField(colName) == true)
                                    {
                                        spWeb.AllowUnsafeUpdates = true;

                                        overtimeDetailsList.Fields.AddDependentLookup(colName, new Guid("{878e2996-150c-4e63-b632-ba90dec566a0}"));
                                        SPField spField = overtimeDetailsList.Fields.TryGetFieldByStaticName(colName);
                                        if (spField != null)
                                        {
                                            SPFieldLookup spFieldLookup = spField as SPFieldLookup;
                                            spFieldLookup.LookupField = colName;
                                            spFieldLookup.Update();
                                            Console.Write(" -> Done");
                                        }
                                        else
                                        {
                                            Console.Write(" -> Failed");
                                        }

                                        spWeb.AllowUnsafeUpdates = false;
                                    }
                                    else
                                    {
                                        Console.Write(" -> Existed");
                                    }
                                }
                                #endregion
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message));
                }
                Console.WriteLine();
                //Console.Write("Press any key to exit...");
                //Console.Read();
            }
            else
            {
                Console.Write("Troll?");
                Console.Read();
            }
        }

        public static void UpdateColForSortData(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                string colForSortColName = "Ordering";
                string approvalStatusColName = "Approval Status";
                string overtimeManagementListName = "Overtime Management";
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        Console.Write("Processing...");
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                #region Update data for ColForSort
                                Console.WriteLine();
                                Console.Write("List Name: {0}", overtimeManagementListName);
                                SPList overtimeManagementList = spWeb.Lists.TryGetList(overtimeManagementListName);
                                if (overtimeManagementList != null)
                                {
                                    if (overtimeManagementList.Fields.ContainsField(colForSortColName) == true && overtimeManagementList.Fields.ContainsField(approvalStatusColName) == true)
                                    {
                                        spWeb.AllowUnsafeUpdates = true;

                                        SPListItemCollection itemcollection = overtimeManagementList.Items;
                                        foreach (SPListItem item in itemcollection)
                                        {
                                            string approvalStatusVal = item[approvalStatusColName] + string.Empty;
                                            switch (approvalStatusVal)
                                            {
                                                case "true":
                                                    item[colForSortColName] = 7;
                                                    break;
                                                case "false":
                                                    item[colForSortColName] = 8;
                                                    break;
                                                default:
                                                    item[colForSortColName] = 0;
                                                    break;
                                            }
                                            item.SystemUpdate();
                                        }

                                        spWeb.AllowUnsafeUpdates = false;
                                        Console.Write(" -> Done");
                                    }
                                    else
                                    {
                                        Console.Write(" -> Existed");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.Write("Cannot find list: {0}", overtimeManagementListName);
                                }
                                #endregion
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message));
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.Read();
            }
            else
            {
                Console.Write("Troll?");
                Console.Read();
            }
        }

        public static void AddShiftRequiredColToList(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                string colName = "ShiftRequired";
                string listName = "Shift Time";
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        Console.Write("Processing...");
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                SPList shiftTimeList = spWeb.Lists.TryGetList(listName);
                                if (shiftTimeList != null)
                                {
                                    if (shiftTimeList.Fields.ContainsField(colName) == false)
                                    {
                                        spWeb.AllowUnsafeUpdates = true;
                                        SPFieldBoolean shiftRequiredField = (SPFieldBoolean)shiftTimeList.Fields.CreateNewField(SPFieldType.Boolean.ToString(), colName);
                                        shiftRequiredField.Required = false;
                                        shiftTimeList.Fields.Add(shiftRequiredField);
                                        shiftTimeList.Update();
                                        spWeb.AllowUnsafeUpdates = false;
                                    }
                                    else
                                    {
                                        Console.Write(" -> Existed");
                                    }
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message));
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.Read();
            }
            else
            {
                Console.Write("Troll?");
                Console.Read();
            }
        }

        public static void AddCommonCommentToVehicleManagement(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                string colName = "CommonComment";
                string listName = "Vehicle Management";
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
                                SPField commonComment = spList.Fields.TryGetFieldByStaticName(colName);
                                if (spList != null && commonComment == null)
                                {
                                    spWeb.AllowUnsafeUpdates = true;
                                    spList.Fields.AddFieldAsXml(@"<Field ID='{70776ca9-0489-41ac-89ed-c2433b7299bc}' Name='CommonComment' DisplayName='CommonComment' Type='Note' NumLines='6' RichText='FALSE' Required='FALSE' Group='Stada Columns'></Field>");
                                    spList.Update();

                                    SPField theField = spList.Fields[new Guid("{70776ca9-0489-41ac-89ed-c2433b7299bc}")];
                                    theField.SchemaXml = @"<Field ID='{70776ca9-0489-41ac-89ed-c2433b7299bc}' Name='CommonComment' DisplayName='$Resources:RBVHStadaLists,CommonComment;' Type='Note' NumLines='6' RichText='FALSE' Required='FALSE' Group='Stada Columns'></Field>";
                                    theField.Update();

                                    for (int i = 0; i < spList.Views.Count; i++)
                                    {
                                        SPView view = spList.Views[i];
                                        if (!view.ViewFields.Exists(theField.StaticName))
                                        {
                                            view.ViewFields.Add(theField);
                                            view.Update();
                                        }
                                    }

                                    spWeb.AllowUnsafeUpdates = false;
                                }
                                else
                                {
                                    spWeb.AllowUnsafeUpdates = true;
                                    for (int i = 0; i < spList.Views.Count; i++)
                                    {
                                        SPView view = spList.Views[i];
                                        if (!view.ViewFields.Exists(commonComment.StaticName))
                                        {
                                            view.ViewFields.Add(commonComment);
                                            view.Update();
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
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.Read();
            }
            else
            {
                Console.Write("Troll?");
                Console.Read();
            }
        }

        public static void AddCommonLocationToList(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                string colName = "CommonLocation";
                IEnumerable<string> arrList = new List<string>() { "Change Shift Management", "Leave Of Absence For Overtime Management", "Vehicle Management"};
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        Console.Write("Processing...");
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                SPList factoryLocationList = spWeb.Lists.TryGetList("Factories");
                                foreach (var listName in arrList)
                                {
                                    SPList spList = spWeb.Lists.TryGetList(listName);
                                    if (spList != null)
                                    {
                                        Console.WriteLine();
                                        Console.Write("List Name: {0}", listName);

                                        spWeb.AllowUnsafeUpdates = true;
                                        if (spList.Fields.ContainsField(colName) == false)
                                        {
                                            spList.Fields.AddFieldAsXml(@"<Field ID='{59394155-e8a3-4fde-ae54-e3bee444add4}' Name='CommonLocation' DisplayName='CommonLocation' Type='Lookup' List='" + factoryLocationList.ID.ToString() + "' ShowField='CommonName' Required='FALSE' Group='Stada Columns'></Field>", false, SPAddFieldOptions.AddToDefaultContentType);
                                            spList.Update();

                                            SPField theField = spList.Fields[new Guid("{59394155-e8a3-4fde-ae54-e3bee444add4}")];
                                            theField.SchemaXml = @"<Field ID='{59394155-e8a3-4fde-ae54-e3bee444add4}' Name='CommonLocation' DisplayName='$Resources:RBVHStadaLists,ShiftManagement_Location;' Type='Lookup' List='" + factoryLocationList.ID.ToString() + "' ShowField='CommonName' Required='FALSE' Group='Stada Columns'></Field>";
                                            theField.Update();

                                            Console.Write(" -> Done");
                                        }
                                        else
                                        {
                                            Console.Write(" -> Existed");
                                        }
                                        spWeb.AllowUnsafeUpdates = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                        Console.Write("Cannot find list: {0}", listName);
                                    }
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message));
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.Read();
            }
            else
            {
                Console.Write("Troll?");
                Console.Read();
            }
        }

        public static void AddCommonMultiLocationToList(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                string colName = "CommonMultiLocations";
                string listName = "Departments";
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        Console.Write("Processing...");
                        using (SPSite spSite = new SPSite(siteUrl))
                        {
                            using (SPWeb spWeb = spSite.RootWeb)
                            {
                                SPList factoryList = spWeb.Lists.TryGetList("Factories");
                                SPList spList = spWeb.Lists.TryGetList(listName);
                                if (spList != null && factoryList != null)
                                {
                                    Console.WriteLine();
                                    Console.Write("List Name: {0}", listName);

                                    spWeb.AllowUnsafeUpdates = true;
                                    if (spList.Fields.ContainsField(colName) == false)
                                    {
                                        spList.Fields.AddFieldAsXml(@"<Field ID='{12cb5806-d66f-42ee-a98e-f73d80763d68}' Name='CommonMultiLocations' DisplayName='CommonMultiLocations' Type='LookupMulti' List='" + factoryList.ID.ToString() + "' ShowField='CommonName' Mult='TRUE' Required='FALSE' Group='Stada Columns'></Field>", false, SPAddFieldOptions.AddToDefaultContentType);
                                        spList.Update();

                                        SPField theField = spList.Fields[new Guid("{12cb5806-d66f-42ee-a98e-f73d80763d68}")];
                                        theField.SchemaXml = @"<Field ID='{12cb5806-d66f-42ee-a98e-f73d80763d68}' Name='CommonMultiLocations' DisplayName='$Resources:RBVHStadaLists,Department_Locations;' Type='LookupMulti' List='" + factoryList.ID.ToString() + "' ShowField='CommonName' Mult='TRUE' Required='FALSE' Group='Stada Columns'></Field>";
                                        theField.Update();

                                        Console.Write(" -> Done");
                                    }
                                    else
                                    {
                                        Console.Write(" -> Existed");
                                    }
                                    spWeb.AllowUnsafeUpdates = false;
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.Write("Cannot find list: {0}", listName);
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message));
                }
                Console.WriteLine();
                Console.Write("Press any key to exit...");
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
