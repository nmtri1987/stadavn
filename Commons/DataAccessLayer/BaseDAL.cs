using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using static RBVH.Stada.Intranet.Biz.Models.EntityBase;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public enum MergeType { Or, And };
    public class BaseDAL<T> : IDAL<T> where T : EntityBase, new()
    {
        private string listUrl;
        /// <summary>
        /// Gets or sets ListUrl
        /// </summary>
        public string ListUrl
        {
            get
            {
                if (string.IsNullOrEmpty(listUrl))
                {
                    var listAttribute = typeof(T).GetAttributeValue((ListUrl list) => list.Url);
                    return listAttribute;
                }
                return listUrl;
            }
            set
            {
                listUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets ListUrl
        /// </summary>
        public string SiteUrl { get; set; }

        public BaseDAL(string siteUrl)
        {
            SiteUrl = siteUrl;
        }

        #region Public Methods
        public IList<SPView> GetViewGuildID()
        {
            IList<SPView> viewList = new List<SPView>();
            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        viewList = GetViewGuildID(spWeb);
                    }
                }
            }
            else
            {
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    viewList = GetViewGuildID(currentWeb);
                }
                else
                {
                    viewList = GetViewGuildID(SPContext.Current.Site.RootWeb);
                }
            }

            return viewList;
        }

        public T GetByID(int id)
        {
            T entityModel = default(T);

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        entityModel = GetByID(spWeb, id);
                    }
                }
            }
            else
            {
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    entityModel = GetByID(currentWeb, id);
                }
                else
                {
                    entityModel = GetByID(SPContext.Current.Site.RootWeb, id);
                }
            }

            return entityModel;
        }

        public T GetByID(SPWeb spWeb, int id)
        {
            T entityModel = default(T);

            SPList splist = spWeb.GetList(spWeb.Url + ListUrl);
            var listItem = splist.GetItemById(id);
            if (listItem != null)
            {
                entityModel = ParseToEntity(listItem);
            }

            return entityModel;
        }

        public SPListItem GetByIDToListItem(int id)
        {
            SPListItem employeeItem;
            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        employeeItem = GetByIDToListItem(spWeb, id);
                    }
                }
            }
            else
            {
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    employeeItem = GetByIDToListItem(currentWeb, id);
                }
                else
                {
                    employeeItem = GetByIDToListItem(SPContext.Current.Site.RootWeb, id);
                }
            }

            return employeeItem;
        }

        public SPListItem GetByIDToListItem(SPWeb spWeb, int id)
        {
            SPList splist = spWeb.GetList(spWeb.Url + ListUrl);
            var employeeItem = splist.GetItemById(id);
            return employeeItem;
        }

        public virtual T ParseToEntity(SPListItem listItem)
        {
            var newItem = new T();
            newItem.ID = listItem.ID;
            newItem.UniqueId = listItem.UniqueId;

            var propertiesDic = new Dictionary<string, string>();
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                ListColumn columnAttribute = (ListColumn)prop.GetCustomAttribute(typeof(ListColumn));
                if (columnAttribute != null)
                {
                    var columnName = columnAttribute.ColumnName;
                    SetValueToEntity(listItem, newItem, columnName, prop.Name);
                }
            }

            return newItem;
        }

        public List<T> GetAll()
        {
            List<T> entityList = new List<T>();

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        entityList = GetAll(spWeb);
                    }
                }
            }
            else
            {
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    entityList = GetAll(currentWeb);
                }
                else
                {
                    entityList = GetAll(SPContext.Current.Site.RootWeb);
                }
            }

            return entityList;
        }

        public List<T> GetAll(SPWeb spWeb)
        {
            List<T> entityList = new List<T>();

            SPList list = spWeb.TryGetSPList(spWeb.Url + ListUrl);
            if (list == null)
            {
                return entityList;
            }

            SPQuery spquery = new SPQuery
            {
                RowLimit = 0,
            };

            var listItems = list.GetItems(spquery);

            if (listItems != null && listItems.Count > 0)
            {
                foreach (SPListItem listItem in listItems)
                {
                    var entityObj = ParseToEntity(listItem);
                    entityList.Add(entityObj);
                }
            }
            return entityList;
        }

        //public SPListItemCollection GetAllToSPListItemCollection()
        //{
        //    SPListItemCollection itemCollection = null;
        //    if (SPContext.Current == null)
        //    {
        //        using (SPSite spSite = new SPSite(SiteUrl))
        //        {
        //            using (SPWeb spWeb = spSite.OpenWeb())
        //            {
        //                itemCollection = GetAllToSPListItemCollection(spWeb);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var currentWeb = SPContext.Current.Web;
        //        if (currentWeb.IsRootWeb == true)
        //        {
        //            itemCollection = GetAllToSPListItemCollection(currentWeb);
        //        }
        //        else
        //        {
        //            itemCollection = GetAllToSPListItemCollection(SPContext.Current.Site.RootWeb);
        //        }
        //    }

        //    return itemCollection;
        //}

        public SPListItemCollection GetAllToSPListItemCollection(SPWeb spWeb)
        {
            SPListItemCollection itemCollection = null;

            SPList list = spWeb.TryGetSPList(spWeb.Url + ListUrl);
            if (list == null)
            {
                return itemCollection;
            }

            SPQuery spquery = new SPQuery
            {
                RowLimit = 0,
            };
            itemCollection = list.GetItems(spquery);

            return itemCollection;
        }

        public bool Delete(int id)
        {
            if (id > 0)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            SPList list = web.GetList(string.Format("{0}{1}", web.Url, ListUrl));

                            SPListItem item = list.GetItemById(id);
                            if (item != null)
                            {
                                item.Delete();
                                list.Update();
                            }

                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
                return true;
            }
            return false;
        }

        public string MergeCAMLConditions(List<string> conditions, MergeType type)
        {
            if (conditions.Count == 0) return "";

            string typeStart = (type == MergeType.And ? "<And>" : "<Or>");
            string typeEnd = (type == MergeType.And ? "</And>" : "</Or>");

            // Build hierarchical structure
            while (conditions.Count >= 2)
            {
                List<string> complexConditions = new List<string>();

                for (int i = 0; i < conditions.Count; i += 2)
                {
                    if (conditions.Count == i + 1) // Only one condition left
                        complexConditions.Add(conditions[i]);
                    else // Two conditions - merge
                        complexConditions.Add(typeStart + conditions[i] + conditions[i + 1] + typeEnd);
                }

                conditions = complexConditions;
            }

            return conditions[0];
        }

        /// <summary>
        ///     Formats the value for SpFieldChoice
        /// </summary>
        /// <param name="input">List of values to insert</param>
        /// <returns></returns>
        public SPFieldMultiChoiceValue FormatValuesForMultiChoice(List<string> input)
        {
            var multiChoiceValue = new SPFieldMultiChoiceValue();
            foreach (string s in input)
            {
                multiChoiceValue.Add(s);
            }
            return multiChoiceValue;
        }

        public virtual int SaveItem(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;

                        SPList list = web.GetList($"{web.Url}{ListUrl}");

                        if (entity.UniqueId == null)
                        {
                            SPListItem newElem = list.AddItem();
                            MapToListItem(entity, newElem);

                            newElem.Update();
                            //UpdateCalculatedFields(list);
                            entity.ID = newElem.ID;
                            entity.UniqueId = newElem.UniqueId;
                        }
                        else
                        {
                            SPListItem existingItem = list.GetItemById(entity.ID);
                            MapToListItem(entity, existingItem);

                            existingItem.Update();
                            //UpdateCalculatedFields(list);
                        }
                    }
                }
            });
            return entity.ID;
        }

        public bool SaveItems(List<T> entities)
        {
            return SaveItems(entities, false);
        }

        public bool SaveItems(List<T> entities, bool deleteDataFirst)
        {
            bool result = false;
            if (deleteDataFirst)
                DeleteItems(entities.Select(x => x.ID).ToList());

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.GetList($"{web.Url}{ListUrl}");
                        SPListItem newElem = list.AddItem();

                        string buildQuery = string.Empty;
                        for (int itemCount = 0; itemCount < entities.Count; itemCount++)
                        {
                            T entity = entities[itemCount];
                            var batchContent = string.Empty;
                            string id = "New";
                            // Add new all items
                            if (deleteDataFirst)
                            {
                                batchContent = MapToBatchItem(list.ID, entity, itemCount, newElem);
                            }
                            // Update or delete the items
                            else
                            {
                                if (entity.ID > 0)
                                {
                                    id = entity.ID.ToString();
                                }
                                // Set value for the other fields if new or update
                                if (entity.BatchCommand != BatchCmd.Delete)
                                {
                                    batchContent = MapToBatchItem(list.ID, entity, itemCount, newElem);
                                }
                            }
                            buildQuery += string.Format("<Method ID=\"{0}\">" +
                                       "<SetList Scope=\"Request\">{1}</SetList>" +
                                       "<SetVar Name=\"ID\">{2}</SetVar>" +
                                       "<SetVar Name=\"Cmd\">{3}</SetVar>{4}" +
                                       "</Method>", string.Format("ID{0}", itemCount), list.ID, id, entity.BatchCommand, batchContent);
                        }
                        var query = string.Format(StringConstant.BatchFormat, buildQuery);
                        web.AllowUnsafeUpdates = true;
                        var stringResult = web.ProcessBatchData(query);
                        web.AllowUnsafeUpdates = false;

                    };
                }
            });

            result = true;

            return result;
        }

        public virtual void DeleteItems(IList<int> ids)
        {
            if (ids != null && ids.Any())
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.GetList($"{web.Url}{ListUrl}");
                            var deletebuilder = new StringBuilder();
                            deletebuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                            string command = "<Method><SetList Scope=\"Request\">" + list.ID +
                                             "</SetList><SetVar Name=\"ID\">{0}</SetVar><SetVar Name=\"Cmd\">Delete</SetVar></Method>";
                            foreach (int id in ids)
                            {
                                deletebuilder.Append(string.Format(command, id));
                            }
                            deletebuilder.Append("</Batch>");
                            web.AllowUnsafeUpdates = true;
                            var stringResult = web.ProcessBatchData(deletebuilder.ToString());
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
        }

        public List<T> GetByQuery(string queryString)
        {
            return GetByQuery(queryString, string.Empty);
        }

        public List<T> GetByQuery(string queryString, params string[] viewFieldsQuery)
        {
            if (string.IsNullOrEmpty(queryString) && (viewFieldsQuery == null || viewFieldsQuery.Count() == 0))
            {
                return GetAll();
            }

            var returnValue = new List<T>();

            var spQuery = new SPQuery { Query = queryString };
            if (viewFieldsQuery != null && viewFieldsQuery.Count() > 0 && !string.IsNullOrEmpty(viewFieldsQuery[0]))
            {
                string viewFields = "<FieldRef Name='UniqueId' /><FieldRef Name='ID' />";
                foreach (var viewField in viewFieldsQuery)
                {
                    viewFields += string.Format("<FieldRef Name='{0}' />", viewField);
                }

                spQuery.ViewFields = viewFields;
                spQuery.ViewFieldsOnly = true;
            }

            if (SPContext.Current == null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite spSite = new SPSite(SiteUrl))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            returnValue = GetByQuery(spWeb, spQuery);
                        }
                    }
                });
            }
            else
            {
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    returnValue = GetByQuery(currentWeb, spQuery);
                }
                else
                {
                    returnValue = GetByQuery(SPContext.Current.Site.RootWeb, spQuery);
                }
            }

            return returnValue;
        }

        public List<T> GetByQuery(SPQuery spQuery, params string[] viewFieldsQuery)
        {
            if (spQuery == null && (viewFieldsQuery == null || viewFieldsQuery.Count() == 0))
            {
                return GetAll();
            }

            var returnValue = new List<T>();

            if (viewFieldsQuery != null && viewFieldsQuery.Count() > 0 && !string.IsNullOrEmpty(viewFieldsQuery[0]))
            {
                string viewFields = "<FieldRef Name='UniqueId' /><FieldRef Name='ID' />";
                foreach (var viewField in viewFieldsQuery)
                {
                    viewFields += string.Format("<FieldRef Name='{0}' />", viewField);
                }

                spQuery.ViewFields = viewFields;
                spQuery.ViewFieldsOnly = true;
            }

            if (SPContext.Current == null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite spSite = new SPSite(SiteUrl))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            returnValue = GetByQuery(spWeb, spQuery);
                        }
                    }
                });
            }
            else
            {
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    returnValue = GetByQuery(currentWeb, spQuery);
                }
                else
                {
                    returnValue = GetByQuery(SPContext.Current.Site.RootWeb, spQuery);
                }
            }

            return returnValue;
        }

        //public SPListItemCollection GetByQueryToSPListItemCollection(string queryString)
        //{
        //    return GetByQueryToSPListItemCollection(queryString, string.Empty);
        //}

        //public SPListItemCollection GetByQueryToSPListItemCollection(string queryString, params string[] viewFieldsQuery)
        //{
        //    if (string.IsNullOrEmpty(queryString) && (viewFieldsQuery == null || viewFieldsQuery.Count() == 0))
        //    {
        //        return GetAllToSPListItemCollection();
        //    }

        //    SPListItemCollection returnValue = null;

        //    var spQuery = new SPQuery { Query = queryString };
        //    if (viewFieldsQuery != null && viewFieldsQuery.Count() > 0 && !string.IsNullOrEmpty(viewFieldsQuery[0]))
        //    {
        //        string viewFields = "<FieldRef Name='UniqueId' /><FieldRef Name='ID' />";
        //        foreach (var viewField in viewFieldsQuery)
        //        {
        //            viewFields += string.Format("<FieldRef Name='{0}' />", viewField);
        //        }

        //        spQuery.ViewFields = viewFields;
        //        spQuery.ViewFieldsOnly = true;
        //    }

        //    if (SPContext.Current == null)
        //    {
        //        SPSecurity.RunWithElevatedPrivileges(delegate
        //        {
        //            using (SPSite spSite = new SPSite(SiteUrl))
        //            {
        //                using (SPWeb spWeb = spSite.OpenWeb())
        //                {
        //                    returnValue = GetByQueryToSPListItemCollection(spWeb, spQuery);
        //                }
        //            }
        //        });
        //    }
        //    else
        //    {
        //        var currentWeb = SPContext.Current.Web;
        //        if (currentWeb.IsRootWeb == true)
        //        {
        //            returnValue = GetByQueryToSPListItemCollection(currentWeb, spQuery);
        //        }
        //        else
        //        {
        //            returnValue = GetByQueryToSPListItemCollection(SPContext.Current.Site.RootWeb, spQuery);
        //        }
        //    }

        //    return returnValue;
        //}

        public SPListItemCollection GetByQueryToSPListItemCollection(SPWeb spWeb, string queryString, params string[] viewFieldsQuery)
        {
            if (string.IsNullOrEmpty(queryString) && (viewFieldsQuery == null || viewFieldsQuery.Count() == 0))
            {
                return GetAllToSPListItemCollection(spWeb);
            }

            SPListItemCollection returnValue = null;

            var spQuery = new SPQuery { Query = queryString };
            if (viewFieldsQuery != null && viewFieldsQuery.Count() > 0 && !string.IsNullOrEmpty(viewFieldsQuery[0]))
            {
                string viewFields = "<FieldRef Name='UniqueId' /><FieldRef Name='ID' />";
                foreach (var viewField in viewFieldsQuery)
                {
                    viewFields += string.Format("<FieldRef Name='{0}' />", viewField);
                }

                spQuery.ViewFields = viewFields;
                spQuery.ViewFieldsOnly = true;
            }

            returnValue = GetByQueryToSPListItemCollection(spWeb, spQuery);

            return returnValue;
        }

        //public SPListItemCollection GetByQueryToSPListItemCollection(SPQuery spQuery, params string[] viewFieldsQuery)
        //{
        //    if (spQuery == null && (viewFieldsQuery == null || viewFieldsQuery.Count() == 0))
        //    {
        //        return GetAllToSPListItemCollection();
        //    }

        //    SPListItemCollection returnValue = null;

        //    if (viewFieldsQuery != null && viewFieldsQuery.Count() > 0 && !string.IsNullOrEmpty(viewFieldsQuery[0]))
        //    {
        //        string viewFields = "<FieldRef Name='UniqueId' /><FieldRef Name='ID' />";
        //        foreach (var viewField in viewFieldsQuery)
        //        {
        //            viewFields += string.Format("<FieldRef Name='{0}' />", viewField);
        //        }

        //        spQuery.ViewFields = viewFields;
        //        spQuery.ViewFieldsOnly = true;
        //    }

        //    if (SPContext.Current == null)
        //    {
        //        SPSecurity.RunWithElevatedPrivileges(delegate
        //        {
        //            using (SPSite spSite = new SPSite(SiteUrl))
        //            {
        //                using (SPWeb spWeb = spSite.OpenWeb())
        //                {
        //                    returnValue = GetByQueryToSPListItemCollection(spWeb, spQuery);
        //                }
        //            }
        //        });
        //    }
        //    else
        //    {
        //        var currentWeb = SPContext.Current.Web;
        //        if (currentWeb.IsRootWeb == true)
        //        {
        //            returnValue = GetByQueryToSPListItemCollection(currentWeb, spQuery);
        //        }
        //        else
        //        {
        //            returnValue = GetByQueryToSPListItemCollection(SPContext.Current.Site.RootWeb, spQuery);
        //        }
        //    }

        //    return returnValue;
        //}

        public SPListItemCollection GetByQueryToSPListItemCollection(SPWeb spWeb, SPQuery spQuery, params string[] viewFieldsQuery)
        {
            if (spQuery == null && (viewFieldsQuery == null || viewFieldsQuery.Count() == 0))
            {
                return GetAllToSPListItemCollection(spWeb);
            }

            SPListItemCollection returnValue = null;

            if (viewFieldsQuery != null && viewFieldsQuery.Count() > 0 && !string.IsNullOrEmpty(viewFieldsQuery[0]))
            {
                string viewFields = "<FieldRef Name='UniqueId' /><FieldRef Name='ID' />";
                foreach (var viewField in viewFieldsQuery)
                {
                    viewFields += string.Format("<FieldRef Name='{0}' />", viewField);
                }

                spQuery.ViewFields = viewFields;
                spQuery.ViewFieldsOnly = true;
            }

            returnValue = GetByQueryToSPListItemCollection(spWeb, spQuery);

            return returnValue;
        }

        public SPFieldUserValueCollection ConvertMultUser(IList<User> Users, SPWeb web)
        {
            SPFieldUserValueCollection userCollection = null;
            if (Users != null && Users.Count > 0)
            {
                userCollection = new SPFieldUserValueCollection();
                foreach (var user in Users)
                {
                    SPUser userToAdd;
                    if (user.ID > 0)
                        userToAdd = web.AllUsers.GetByID(user.ID);
                    else
                        userToAdd = web.EnsureUser(user.UserName);
                    if (userToAdd != null)
                    {
                        SPFieldUserValue SPFieldUserValue = new SPFieldUserValue(web, userToAdd.ID, userToAdd.LoginName);
                        userCollection.Add(SPFieldUserValue);
                    }
                }
            }
            return userCollection;
        }

        /// <summary>
        /// Count items by query
        /// </summary>
        /// <returns>Total items</returns>
        public int CountByQuery(string queryString)
        {
            var result = 0;

            if (SPContext.Current == null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite spSite = new SPSite(SiteUrl))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            result = CountByQuery(spWeb, queryString);
                        }
                    }
                });
            }
            else
            {
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    result = CountByQuery(currentWeb, queryString);
                }
                else
                {
                    result = CountByQuery(SPContext.Current.Site.RootWeb, queryString);
                }
            }

            return result;
        }

        public int CountByQuery(SPWeb spWeb, string queryString)
        {
            var result = 0;
            var query = new SPQuery
            {
                ViewFields = "<FieldRef Name='ID'/>",
                Query = queryString,
                IncludePermissions = false,
                RowLimit = 20000,
                ViewFieldsOnly = true
            };

            SPList list = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            var itemCollection = list.GetItems(query);
            if (itemCollection != null)
            {
                result = itemCollection.Count;
            }

            return result;
        }
        #endregion

        #region Private methods

        private void SetValueToEntity(SPListItem item, T entity, string columnName, string propertyName)
        {
            object value;
            try
            {
                SPField field = item.Fields.GetField(columnName);
                value = item[field.Id];
            }
            catch (Exception)
            {
                value = null;
            }

            if (!item.Fields.ContainsField(columnName)) return;
            SPField currentField = item.Fields.GetField(columnName);

            if (currentField is SPFieldCalculated)
            {
                try
                {
                    value = currentField.GetFieldValueAsText(value);
                    Type propertyType = typeof(T).GetProperty(propertyName).PropertyType;
                    value = TryConvertSimpleValueType(value, propertyType);
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException("Error on getting calculated value", ex);
                }
            }
            else if (value != null)
            {
                Type propertyType = typeof(T).GetProperty(propertyName).PropertyType;
                value = TryConvertSimpleValueType(value, propertyType);
                if (propertyType == typeof(LookupItem))
                {
                    value = item.ToLookupItemModel(columnName);
                }
                else if (propertyType == typeof(List<LookupItem>))
                    value = item.ToLookupItemsModel(columnName);
                else if (propertyType == typeof(List<string>))
                    value = item.ToValuesFromMultiChoice(columnName);
                else if (propertyType == typeof(List<User>))
                    value = item.ToUsersModel(columnName);
                else if (propertyType == typeof(User))
                    value = item.ToUserModel(columnName);
                else if (propertyType == typeof(SPGroup))
                    value = item.ToGroup(columnName);
            }

            try
            {
                typeof(T).InvokeMember(
                    propertyName,
                    BindingFlags.SetProperty,
                    null,
                    entity,
                    new[] { value });
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Error when setting property '{0}' value '{1}'",
                    propertyName, value);
                throw new ArgumentNullException(errorMessage, ex);
            }
        }

        private static object TryConvertSimpleValueType(object value, Type propertyType)
        {
            if (value == null)
                return null;
            if (value.ToString().Contains(";#"))
            {
                string[] newvalue = value.ToString().Split(';');
                value = newvalue[0];
            }

            if (propertyType == typeof(double) || propertyType == typeof(double?))
                value = Convert.ToDouble(value);
            else if (propertyType == typeof(int) || propertyType == typeof(int?))
                value = Convert.ToInt32(value);
            else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                value = Convert.ToDecimal(value);
            else if (propertyType == typeof(float) || propertyType == typeof(float?))
                value = float.Parse(value.ToString());
            else if (propertyType == typeof(string))
                value = value.ToString();
            else if (propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?))
                value = TimeSpan.Parse(value.ToString());
            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                DateTime dateValue;
                value = DateTime.TryParse(value.ToString(), out dateValue) ? (object)dateValue : null;
            }
            return value;
        }

        private static SPFieldUserValueCollection FormatUserCollection(IEnumerable<User> usersList,
         SPListItem item,
         string siteUrl = null)
        {
            var userValueCollection = new SPFieldUserValueCollection();
            foreach (User user in usersList)
            {
                if (user.IsGroup)
                {
                    SPGroup group = item.Web.Groups.GetByID(user.ID);
                    if (group != null)
                    {
                        userValueCollection.Add(new SPFieldUserValue(item.Web, group.ID, group.LoginName));
                    }
                }
                else
                {
                    SPUser spuser = string.IsNullOrEmpty(siteUrl)
                        ? SPHelper.EnsureUser(SPHelper.GetAccountName(user.UserName))
                        : SPHelper.EnsureUser(SPHelper.GetAccountName(user.UserName), siteUrl);
                    if (spuser != null)
                    {
                        userValueCollection.Add(new SPFieldUserValue(item.Web, spuser.ID, user.UserName));
                    }
                    else
                    {
                        throw new ArgumentException(
                            string.Format("User {0} not found in the {1} webapplication", user.UserName, item.Web.Title),
                            "usersList");
                    }
                }
            }

            return userValueCollection;
        }

        private object GetEntityValue(SPListItem item, string propertyName, object value, string columnName, bool useForBatch = false)
        {
            if (value == null) return null;
            object valueToSet = value;
            Type propertyType = typeof(T).GetProperty(propertyName).PropertyType;
            if (propertyType == typeof(double) || propertyType == typeof(double?) ||
                propertyType == typeof(decimal) || propertyType == typeof(decimal?) ||
                propertyType == typeof(float) || propertyType == typeof(float?))
                valueToSet = Convert.ToDecimal(value);
            else if (propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?))
            {
                valueToSet = value.ToString();
            }
            else if (propertyType == typeof(List<LookupItem>))
            {
                valueToSet = FormatValuesForMultipleLookup((List<LookupItem>)value, item, columnName);
            }
            else if (propertyType == typeof(List<string>))
            {
                valueToSet = FormatValuesForMultiChoice((List<string>)value);
            }
            else if (propertyType == typeof(List<User>))
            {
                SPFieldUserValueCollection users = FormatUserCollection((List<User>)value, item, SiteUrl);
                if (users != null && users.Count > 0)
                {
                    valueToSet = users;
                }
            }
            else if (propertyType == typeof(User))
            {
                valueToSet = GetUserValue(item, value);
            }
            else if (propertyType == typeof(LookupItem))
            {
                var lookUpItem = (LookupItem)value;
                valueToSet = FormatValuesForLookup(lookUpItem, item, columnName);
            }
            else if (propertyType == typeof(DateTime))
            {
                var datetimeValue = (DateTime)value;
                if (datetimeValue != DateTime.MinValue)
                {
                    valueToSet = useForBatch ? SPUtility.CreateISO8601DateTimeFromSystemDateTime(datetimeValue) : value;
                }
                else
                {
                    valueToSet = null;
                }
            }
            else if (propertyType == typeof(DateTime?))
            {
                var datetimeValue = value as DateTime?;

                if (datetimeValue.HasValue && datetimeValue.Value != DateTime.MinValue)
                {
                    valueToSet = useForBatch ? SPUtility.CreateISO8601DateTimeFromSystemDateTime(datetimeValue.Value) : value;
                }
                else
                {
                    valueToSet = null;
                }
            }
            return valueToSet;
        }

        private object GetUserValue(SPListItem item, object value)
        {
            object valueResult = null;
            var personInfo = (User)value;

            if (personInfo.ID != 0)
            {
                valueResult = new SPFieldUserValue(item.Web, personInfo.ID, personInfo.UserName);
            }
            else if (personInfo.IsGroup)
            {
                SPGroup group = item.Web.Groups.GetByID(personInfo.ID);
                if (@group != null)
                {
                    valueResult = new SPFieldUserValue(item.Web, @group.ID, @group.LoginName);
                }
            }
            else
            {
                SPUser userOrGroupToAdd = string.IsNullOrEmpty(SiteUrl)
                    ? SPHelper.EnsureUser(SPHelper.GetAccountName(personInfo.UserName))
                    : SPHelper.EnsureUser(
                        SPHelper.GetAccountName(personInfo.UserName), SiteUrl);
                if (userOrGroupToAdd != null)
                {
                    valueResult = new SPFieldUserValue(item.Web, userOrGroupToAdd.ID,
                        userOrGroupToAdd.LoginName);
                }
            }
            return valueResult;
        }

        private static SPFieldLookupValueCollection FormatValuesForMultipleLookup(IEnumerable<LookupItem> input, SPListItem item, string key)
        {
            var lookupFld = (SPFieldLookup)item.Fields.GetFieldByInternalName(key);
            SPList sourceList = item.Web.Lists[new Guid(lookupFld.LookupList)];
            var multipleLookUpValues = new SPFieldLookupValueCollection();
            foreach (LookupItem i in input)
            {
                try
                {
                    SPListItem splookupItem = sourceList.GetItemById(i.LookupId);
                    multipleLookUpValues.Add(new SPFieldLookupValue(splookupItem.ID,
                        splookupItem[lookupFld.LookupField].ToString()));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        string.Format("Element with id  {0} not found in the {1} list", i.LookupId,
                            item.ParentList.Title), sourceList.Title, ex);
                }
            }
            return multipleLookUpValues;
        }

        private SPFieldLookupValue FormatValuesForLookup(LookupItem input, SPListItem item, string key)
        {
            try
            {
                var spv = new SPFieldLookupValue(input.LookupId, input.LookupValue);
                return spv;
            }
            catch (Exception ex)
            {
                string errorMessage =
                    string.Format("FormatValuesForLookup - Element with id  {0} not found in the {1} list",
                        input.LookupId,
                        item.ParentList.Title);
                //_logger.Error("SharepointRepositoryBase", errorMessage, ex.Message, ex);
                throw new ArgumentException(errorMessage, key, ex);
            }
        }

        private IList<SPView> GetViewGuildID(SPWeb spWeb)
        {
            IList<SPView> viewList = new List<SPView>();
            SPList splist = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            foreach (SPView view in splist.Views)
            {
                viewList.Add(view);
            }
            return viewList;
        }

        private List<T> GetByQuery(SPWeb spWeb, SPQuery spQuery)
        {
            var returnValue = new List<T>();

            SPList list = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            SPListItemCollection itemCollection = list.GetItems(spQuery);
            if (itemCollection != null)
            {
                foreach (SPListItem item in itemCollection)
                {
                    var newItem = new T();
                    newItem = ParseToEntity(item);
                    returnValue.Add(newItem);
                }
            }

            return returnValue;
        }

        private SPListItemCollection GetByQueryToSPListItemCollection(SPWeb spWeb, SPQuery spQuery)
        {
            SPListItemCollection itemCollection = null;

            SPList list = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            itemCollection = list.GetItems(spQuery);

            return itemCollection;
        }

        #endregion

        #region Protected methods

        protected static void UpdateCalculatedFields(SPList list)
        {
            for (int i = 0; i < list.Fields.Count; i++)
            {
                SPField field = list.Fields[i];
                if (field is SPFieldCalculated && !field.ReadOnlyField)
                {
                    field.Update();
                }
            }
        }

        protected virtual void MapToListItem(T entity, SPListItem item)
        {
            // base fields
            SPFieldCollection allFields = item.ParentList.Fields;
            SPField editorField = allFields.GetFieldByInternalName("Editor");
            SPField authorField = allFields.GetFieldByInternalName("Author");
            if (SPContext.Current != null)
            {
                //if (entity.UniqueId.HasValue)
                //{
                //    item[editorField.Title] = SPContext.Current.Web.CurrentUser;
                //}
                //else
                //{
                //    item[authorField.Title] = SPContext.Current.Web.CurrentUser;
                //    item[editorField.Title] = SPContext.Current.Web.CurrentUser;
                //}
            }

            // map properties from configuration
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                ListColumn columnAttribute = (ListColumn)prop.GetCustomAttribute(typeof(ListColumn));
                if (columnAttribute == null)
                    continue;
                string columnName = columnAttribute.ColumnName;
                if (string.IsNullOrEmpty(columnName) || columnAttribute.ReadOnly)
                    continue;
                if (columnAttribute.ColumnName.ToLower() == "id") continue;

                object value = typeof(T).InvokeMember(
                    prop.Name,
                    BindingFlags.GetProperty,
                    null,
                    entity,
                    new object[] { });

                if (value == null)
                {
                    item[columnName] = null;
                    continue;
                }

                var valueToSet = GetEntityValue(item, prop.Name, value, columnName);
                item[columnName] = valueToSet;

                if (entity.Attachments != null && entity.Attachments.Count > 0)
                {
                    foreach (var attachement in entity.Attachments)
                    {
                        item.Attachments.Add(attachement.FileName, attachement.FileContent);
                    }
                }
            }
        }

        protected virtual string MapToBatchItem(Guid listGuidID, T entity, int index, SPListItem item)
        {
            var buildQuery = new StringBuilder();
            // map properties from configuration
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.Name.ToLower().Equals("id") || prop.Name.ToLower().Equals("UniqueId")) continue;
                ListColumn columnAttribute = (ListColumn)prop.GetCustomAttribute(typeof(ListColumn));
                if (columnAttribute == null) { continue; }
                string columnName = columnAttribute.ColumnName;
                if (string.IsNullOrEmpty(columnName) || columnAttribute.ReadOnly)
                    continue;

                object value = typeof(T).InvokeMember(
                    prop.Name,
                    BindingFlags.GetProperty,
                    null,
                    entity,
                    new object[] { });
                var valueToSet = GetEntityValue(item, prop.Name, value, columnName, true);

                buildQuery.AppendLine(string.Format(CultureInfo.GetCultureInfo("en-US"), "<SetVar Name=\"urn:schemas-microsoft-com:office:office#{0}\">{1}</SetVar>", columnName, valueToSet));
            }

            return buildQuery.ToString();
        }

        #endregion
    }
}