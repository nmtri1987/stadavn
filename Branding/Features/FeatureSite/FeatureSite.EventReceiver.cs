//-----------------------------------------------------------------------
// <copyright file="FeatureWebEventReceiver.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the FeatureWebEventReceiver class.</summary>
//-----------------------------------------------------------------------
// ReSharper disable once CheckNamespace

namespace RBVH.Stada.Intranet.Branding.Features.FeatureWeb
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;

    /// <summary>
    ///     This class handles events raised during feature activation, deactivation, installation, uninstallation, and
    ///     upgrade.
    /// </summary>
    /// <remarks>
    ///     The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    [Guid("65cfcfbd-de32-4afd-bc58-e12d3189022f")]
    public class RBVHStadaIntranetEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    var site = (SPSite)properties.Feature.Parent;
                    foreach (SPWeb spWeb in site.AllWebs)
                    {
                        spWeb.AllowUnsafeUpdates = true;
                        spWeb.MasterUrl = spWeb.Site.RootWeb.ServerRelativeUrl + "_catalogs/masterpage/stada.master";
                        spWeb.CustomMasterUrl = spWeb.Site.RootWeb.ServerRelativeUrl + "_catalogs/masterpage/stada.master";
                        spWeb.Update();
                        spWeb.AllowUnsafeUpdates = false;
                    }
                });
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("STADA", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                //throw ex;
                //   ULSLogging.LogError(ex);
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}