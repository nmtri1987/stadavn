//-----------------------------------------------------------------------
// <copyright file="ChangeLocaleModule.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the ChangeLocaleModule class.</summary>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Web;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;

namespace RBVH.Core.SharePoint
{
    /// <summary>
    ///     Change SharePoint Locale Module
    /// </summary>
    public class ChangeLocaleModule : IHttpModule
    {
        /// <summary>
        ///     Query string key
        /// </summary>
        private const string KeyCulture = "lang";

        /// <summary>
        ///     Init context
        /// </summary>
        /// <param name="context">Application context</param>
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += ApplicationPreRequestHandlerExecute;
        }

        /// <summary>
        ///     Dispose object
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     Application pre request handler excecute
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void ApplicationPreRequestHandlerExecute(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;
            SetCulture(context);
        }

        /// <summary>
        ///     Get requested culture
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Culture Info</returns>
        private void SetCulture(HttpContext context)
        {
            // The key of current selected language in the cookies. 
            string strKeyName = "Stada_LangSwitcher_Setting";
            // Current language. 
            string strLanguage = "vi-VN";
            string strLanguageFromQueryString = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(context.Request.QueryString[KeyCulture]))
                {
                    strLanguageFromQueryString = context.Request.QueryString[KeyCulture];
                }

                // Set the current language. 
                if (context.Session != null && context.Session[strKeyName] != null)
                {
                    strLanguage = context.Session[strKeyName].ToString();
                    if (!string.IsNullOrEmpty(strLanguageFromQueryString) && strLanguageFromQueryString != strLanguage)
                    {
                        strLanguage = strLanguageFromQueryString;
                        context.Session[strKeyName] = strLanguage;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(strLanguageFromQueryString))
                    {
                        strLanguage = strLanguageFromQueryString;
                    }

                    if (context.Session != null)
                    {
                        context.Session[strKeyName] = strLanguage;
                    }
                }

                var lang = context.Request.Headers["Accept-Language"];
                if (lang != null)
                {
                    if (!lang.Contains(strLanguage))
                        context.Request.Headers["Accept-Language"] = strLanguage + "," + context.Request.Headers["Accept-Language"];

                    var culture = new CultureInfo(strLanguage);

                    // Apply the culture. 
                    SPUtility.SetThreadCulture(culture, culture);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

    }
}