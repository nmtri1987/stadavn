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
    public class WriteLogModule : IHttpModule
    {
        EventHandler beginRequestEventHandler;
        EventHandler endRequestEventHandler;
        HttpApplication applicationContext;
        protected void OnBeginRequest(object sender, EventArgs e)
        {
            // Begin log:
            ULSLogging.LogMessageToFile($"-- BEGIN ListBaseUserUserControl at: {DateTime.Now}");
        }

        protected void OnEndRequest(object sender, EventArgs e)
        {
            // End log:
            ULSLogging.LogMessageToFile($"-- END RequestList at: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.ff")}");
        }

        public void Init(HttpApplication context)
        {
            applicationContext = context;

            beginRequestEventHandler = new System.EventHandler(this.OnBeginRequest);
            applicationContext.BeginRequest += beginRequestEventHandler;
            endRequestEventHandler = new System.EventHandler(this.OnEndRequest);
            applicationContext.BeginRequest += endRequestEventHandler;
        }

        public void Dispose()
        {
            applicationContext.BeginRequest -= beginRequestEventHandler;
            beginRequestEventHandler = null;
            applicationContext.EndRequest -= beginRequestEventHandler;
            beginRequestEventHandler = null;
        }
    }
}