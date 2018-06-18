//-----------------------------------------------------------------------
// <copyright file="AddSubsiteEventReceiver.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace RBVH.Stada.Intranet.Branding.EventReceivers.AddSubsiteEventReceiver
{
    using Core.SharePoint;
    using Microsoft.SharePoint;

    /// <summary>
    ///     Web Events
    /// </summary>
    public class AddSubsiteEventReceiver : SPWebEventReceiver
    {

        /// <summary>
        ///     A site was provisioned.
        /// </summary>
        /// <param name="properties">Event receiver properties</param>
        public override void WebProvisioned(SPWebEventProperties properties)
        {
            ////Update Master Page after new subsite was created
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                var web = properties.Web;
                if (!web.Exists)
                {
                    return;
                }
                web.AllowUnsafeUpdates = true;
                var masterPagePath = $"{web.Site.RootWeb.ServerRelativeUrl}_catalogs/masterpage/stada.master";
                web.MasterUrl = masterPagePath;
                web.CustomMasterUrl = masterPagePath;
                // web.Update();
                //SPHelper.ActiveInstalledLanguage(web);
                web.AllowUnsafeUpdates = false;

            });
        }
    }
}