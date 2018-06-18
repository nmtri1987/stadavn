//-----------------------------------------------------------------------
// <copyright file="SPHelper.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the SPHelper class.</summary>
//-----------------------------------------------------------------------

using System.Web.UI;

namespace RBVH.Core.SharePoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using Microsoft.Office.SecureStoreService.Server;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
    using System.Globalization;

    /// <summary>
    /// SharePoint Helper class
    /// </summary>
    public class SPHelper
    {
        #region <Register Event Receiver>

        /// <summary>
        ///     Register even receiver for content Type
        /// </summary>
        /// <param name="contentType">Content type need to register</param>
        /// <param name="eventReceiverType">Type of Content Type</param>
        /// <param name="receiverClassType">Type of class event receiver</param>
        public static void RegisterEventReceiver(SPContentType contentType, SPEventReceiverType eventReceiverType, Type receiverClassType)
        {
            if (contentType == null)
            {
                return;
            }

            try
            {
                // unRegister Event Receiver first
                UnRegisterEventReceiver(contentType, eventReceiverType);
                contentType.EventReceivers.Add(eventReceiverType, receiverClassType.Assembly.FullName, receiverClassType.FullName);
                contentType.Update(true);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        #endregion <Register Event Receiver>

        #region <UnRegister Event Receiver>

        /// <summary>
        ///     UnRegister Event Receiver
        /// </summary>
        /// <param name="contentType">Content Type need to unregister</param>
        /// <param name="eventReceiverType">Receiver Type</param>
        public static void UnRegisterEventReceiver(SPContentType contentType, SPEventReceiverType eventReceiverType)
        {
            if (contentType == null)
            {
                return;
            }

            try
            {
                for (var i = 0; i < contentType.EventReceivers.Count; i++)
                {
                    var receiver = contentType.EventReceivers[i];
                    if (receiver.Type.Equals(eventReceiverType))
                    {
                        receiver.Delete();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        #endregion <UnRegister Event Receiver>

        /// <summary>
        /// Get Credentials from SSS
        /// </summary>
        /// <param name="appId">Application Id</param>
        /// <returns>Dictionary string and string</returns>
        public static Dictionary<string, string> GetCredentialsFromSSS(string appId)
        {
            var result = new Dictionary<string, string>();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    // Get the default Secure Store Service provider.
                    var provider = SecureStoreProviderFactory.Create();
                    if (provider == null)
                    {
                        throw new InvalidOperationException("Unable to get an ISecureStoreProvider");
                    }

                    var providerContext = provider as ISecureStoreServiceContext;
                    if (providerContext == null)
                    {
                        return;
                    }

                    providerContext.Context = SPServiceContext.GetContext(GetCentralAdminSite());

                    var secureStoreProvider = new SecureStoreProvider { Context = providerContext.Context };

                    // Create the variables to hold the credentials.
                    using (var creds = provider.GetCredentials(appId))
                    {
                        if (creds == null)
                        {
                            return;
                        }

                        var fields = secureStoreProvider.GetTargetApplicationFields(appId);
                        if (fields.Count <= 0)
                        {
                            return;
                        }

                        for (var i = 0; i < fields.Count; i++)
                        {
                            var field = fields[i];
                            var credential = creds[i];
                            var decryptedCredential = GetStringFromSecureString(credential.Credential);
                            result.Add(field.Name, decryptedCredential);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Get Credential from SSS
        /// </summary>
        /// <param name="appId">Application Id</param>
        /// <param name="adminSiteUrl">Admin Site Url</param>
        /// <returns>Credential as Dictionary string and string</returns>
        public static Dictionary<string, string> GetCredentialsFromSSS(string appId, string adminSiteUrl)
        {
            var result = new Dictionary<string, string>();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    var siteAdmin = new SPSite(adminSiteUrl);

                    // Get the default Secure Store Service provider.
                    var provider = SecureStoreProviderFactory.Create();
                    if (provider == null)
                    {
                        throw new InvalidOperationException("Unable to get an ISecureStoreProvider");
                    }

                    var providerContext = provider as ISecureStoreServiceContext;
                    if (providerContext == null)
                    {
                        return;
                    }

                    providerContext.Context = SPServiceContext.GetContext(siteAdmin);

                    var secureStoreProvider = new SecureStoreProvider { Context = providerContext.Context };

                    // Create the variables to hold the credentials.
                    using (var creds = provider.GetCredentials(appId))
                    {
                        if (creds == null)
                        {
                            return;
                        }

                        var fields = secureStoreProvider.GetTargetApplicationFields(appId);
                        if (fields.Count <= 0)
                        {
                            return;
                        }

                        for (var i = 0; i < fields.Count; i++)
                        {
                            var field = fields[i];
                            var credential = creds[i];
                            var decryptedCredential = GetStringFromSecureString(credential.Credential);
                            result.Add(field.Name, decryptedCredential);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Get User field from SharePoint List item
        /// </summary>
        /// <param name="item">List Item</param>
        /// <param name="fieldName">Field Name</param>
        /// <returns>SharePoint SPFieldUserValue</returns>
        public static SPFieldUserValue GetUserField(SPListItem item, string fieldName)
        {
            try
            {
                if (item[fieldName] == null)
                {
                    return null;
                }

                var currentValue = item[fieldName].ToString();
                var spfield =
                    item.ParentList.Fields
                        .Cast<SPField>()
                        .FirstOrDefault(c => c.InternalName.Equals(fieldName, StringComparison.OrdinalIgnoreCase) ||
                                c.Title.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                var field = spfield as SPFieldUser;
                var fieldValues = (SPFieldUserValue)field?.GetFieldValue(currentValue);
                return fieldValues;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return null;
        }

        /// <summary>
        ///     Get Value of Lookup Field
        /// </summary>
        /// <param name="fldLookup">Field Lookup</param>
        /// <returns>string value</returns>
        public static string GetValueFromLookupField(object fldLookup)
        {
            if (fldLookup == null)
            {
                return string.Empty;
            }

            try
            {
                var lookupValue = new SPFieldLookupValue(fldLookup.ToString());
                return lookupValue.LookupValue ?? string.Empty;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get string from Secure string
        /// </summary>
        /// <param name="secStr">Secure string</param>
        /// <returns>String value</returns>
        private static string GetStringFromSecureString(SecureString secStr)
        {
            if (secStr == null)
            {
                return null;
            }

            var plainText = IntPtr.Zero;
            try
            {
                plainText = Marshal.SecureStringToBSTR(secStr);
                return Marshal.PtrToStringBSTR(plainText);
            }
            finally
            {
                if (plainText != IntPtr.Zero)
                {
                    Marshal.FreeBSTR(plainText);
                }
            }
        }

        /// <summary>
        /// Get central admin site
        /// </summary>
        /// <returns>SharePoint Site</returns>
        private static SPSite GetCentralAdminSite()
        {
            var adminWebApp = SPAdministrationWebApplication.Local;

            if (adminWebApp == null)
            {
                return null;
            }

            SPSite adminSite;
            var adminSiteUri = adminWebApp.GetResponseUri(SPUrlZone.Default);
            if (adminSiteUri != null)
            {
                adminSite = adminWebApp.Sites[adminSiteUri.AbsoluteUri];
            }
            else
            {
                return null;
            }

            return adminSite;
        }

        #region Group

        /// <summary>
        /// Check if group exists in site collection
        /// </summary>
        /// <param name="web"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool GroupExistsInSiteCollection(SPWeb web, string name)
        {
            return web.SiteGroups.OfType<SPGroup>().Count(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) > 0;
        }

        /// <summary>
        /// Add User to Group
        /// </summary>
        /// <param name="web"></param>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        public static void AddUserToGroup(SPWeb web, SPUser user, string groupName)
        {
            web.AllowUnsafeUpdates = true;
            SPGroup group = null;
            if (GroupExistsInSiteCollection(web, groupName))
            {
                group = web.SiteGroups[groupName];
                bool userExsists = user.Groups.Cast<SPGroup>().Any(g => g.Name.ToLower() == groupName.ToLower());
                if (group != null && !userExsists)
                {
                    SPUser spUser = web.EnsureUser(user.LoginName);
                    if (spUser != null)
                        group.AddUser(spUser);
                }
            }
            web.AllowUnsafeUpdates = false;
        }

        /// <summary>
        /// Remove User from Group
        /// </summary>
        /// <param name="web"></param>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        public static void RemoveUserFromGroup(SPWeb web, SPUser user, string groupName)
        {
            web.AllowUnsafeUpdates = true;
            SPGroup group = null;
            if (GroupExistsInSiteCollection(web, groupName))
            {
                group = web.SiteGroups[groupName];

                bool userExsists = user.Groups.Cast<SPGroup>().Any(g => g.Name.ToLower() == groupName.ToLower());

                if (group != null && userExsists)
                {
                    SPUser spUser = web.EnsureUser(user.LoginName);
                    group.RemoveUser(spUser);
                }
            }
            web.AllowUnsafeUpdates = false;
        }

        /// <summary>
        /// Delete SP Group
        /// </summary>
        /// <param name="web"></param>
        /// <param name="groupName"></param>
        public static void DeleleSPGroup(SPWeb web, string groupName)
        {
            web.AllowUnsafeUpdates = true;
            SPGroup group = null;
            if (GroupExistsInSiteCollection(web, groupName))
            {
                group = web.SiteGroups[groupName];
                if (group != null)
                {
                    web.SiteGroups.RemoveByID(group.ID);
                }
            }
            web.AllowUnsafeUpdates = false;
        }

        #endregion
        public static void ActiveInstalledLanguage(SPWeb newSite)
        {
            // Enable MUI.
            newSite.IsMultilingual = true;

            // Add support for any installed language currently not supported.
            SPLanguageCollection installed = SPLanguageSettings.GetGlobalInstalledLanguages(15);
            IEnumerable<CultureInfo> supported = newSite.SupportedUICultures;

            foreach (SPLanguage language in installed)
            {
                CultureInfo culture = new CultureInfo(language.LCID);

                if (!supported.Contains(culture))
                {
                    newSite.AddSupportedUICulture(culture);
                }
            }
            newSite.AllowUnsafeUpdates = true;
            newSite.Update();
        }

        public static SPUser EnsureUser(string userName)
        {
            try
            {
                return EnsureUser(userName, SPContext.Current.Site.Url);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
            return null;
        }

        public static SPUser EnsureUser(string userName, string siteUrl)
        {
            SPUser user = null;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var site = new SPSite(siteUrl))
                    {
                        using (SPWeb currentWeb = site.OpenWeb())
                        {
                            currentWeb.AllowUnsafeUpdates = true;
                            user = currentWeb.EnsureUser(userName);
                            currentWeb.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                return null;
            }

            return user;
        }

        public static SPUser EnsureUser(string userName, SPWeb currentWeb)
        {
            try
            {
                return EnsureUser(userName, currentWeb.Site.Url);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
            return null;
        }

        public static string GetAccountName(string accountName)
        {
            if (-1 == accountName.IndexOf('|'))
            {
                return accountName;
            }
            string[] accountPaths = accountName.Split('|');
            return accountPaths[accountPaths.Length - 1];
        }
    }
}