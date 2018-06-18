using System;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.Linq;

namespace RBVH.Stada.Intranet.Biz.Extension
{
    public static class SPFieldExtension
    {
        public static LookupItem ToLookupItemModel(this SPListItem item, string fieldName)
        {
            var result = new LookupItem();
            if (!string.IsNullOrEmpty(Convert.ToString(item[fieldName])))
            {
                SPFieldLookupValue fieldLookupValue = new SPFieldLookupValue(item[fieldName].ToString());
                if (fieldLookupValue != null)
                {
                    result.LookupId = fieldLookupValue.LookupId;
                    result.LookupValue = fieldLookupValue.LookupValue;
                }
            }
            return result;
        }

        public static List<LookupItem> ToLookupItemsModel(this SPListItem item, string fieldName)
        {
            var objLookupFieldValueCol = new SPFieldLookupValueCollection(item[fieldName].ToString());
            return
                objLookupFieldValueCol.Select(
                    singleValue => new LookupItem { LookupId = singleValue.LookupId, LookupValue = singleValue.LookupValue }).ToList();
        }

        public static string ToString(this SPListItem item, string fieldName)
        {
            var returnValue = Convert.ToString(item[fieldName]);
            return returnValue;
        }

        public static User ToUserModel(this SPListItem item, string fieldName)
        {
            SPFieldUser spuserField = (SPFieldUser)item.Fields.GetField(fieldName);
            SPFieldUserValue spuserFieldValue = (SPFieldUserValue)spuserField.GetFieldValue(Convert.ToString(item[fieldName]));

            User user = new User();
            if (spuserFieldValue != null)
            {
                user.UserName = spuserFieldValue.User.LoginName;
                user.FullName = spuserFieldValue.User.Name;
                // Duc.VoTan Add
                user.ID = spuserFieldValue.LookupId;
            }
            return user;
        }
        public static IList<User> ToUserModelList(this SPListItem item, string fieldName)
        {
            SPFieldUser spuserField = (SPFieldUser)item.Fields.GetField(fieldName);
            IList<User> UserList = new List<User>();
            SPFieldUserValueCollection spuserFieldValueList = (SPFieldUserValueCollection)spuserField.GetFieldValue(Convert.ToString(item[fieldName]));
            if (spuserFieldValueList != null)
            {
      
                foreach (var userFieldValue in spuserFieldValueList)
                {
                    User user = new User();
                    user.UserName = userFieldValue.User.LoginName;
                    user.FullName = userFieldValue.User.Name;
                    UserList.Add(user);
                }
            }
    
            return UserList;


        }

        public static string CalculatedFieldToString(this SPListItem item, string fieldName)
        {
            SPFieldCalculated cf = (SPFieldCalculated)item.Fields.GetField(fieldName); ;
            return cf.GetFieldValueForEdit(item[fieldName]);
        }

        public static List<string> ToValuesFromMultiChoice(this SPListItem item, string fieldName)
        {
            var output = new List<string>();
            var multiChoiceValue = new SPFieldMultiChoiceValue(item[fieldName].ToString());
            for (int i = 0; i < multiChoiceValue.Count; i++)
            {
                output.Add(multiChoiceValue[i]);
            }
            return output;
        }

        public static List<User> ToUsersModel(this SPListItem item, string fieldName)
        {
            var users = new List<User>();

            using (var site = new SPSite(item.Web.Site.Url))
            {
                var fieldValues = item[fieldName] as SPFieldUserValueCollection;
                if (fieldValues != null)
                {
                    for (int i = 0; i < fieldValues.Count; i++)
                    {
                        SPFieldUserValue singlevalue = fieldValues[i];
                        if (singlevalue.User != null)
                        {
                            //do stuff for the singlevalue.User
                            users.Add(new User
                            {
                                UserName = singlevalue.User.LoginName,
                                FullName = singlevalue.User.Name
                            });
                        }
                    }
                }
            }

            return users;
        }

        public static SPGroup ToGroup(this SPListItem item, string columnName)
        {
            var group = (SPFieldUser)item.Fields[columnName];

            var user = (SPFieldUserValue)group.GetFieldValue(item[columnName].ToString());

            SPGroupCollection allGroups;

            using (SPWeb web = SPContext.Current.Site.OpenWeb())
            {
                allGroups = web.SiteGroups;
            }

            return allGroups[user.LookupValue];
        }
    }
}