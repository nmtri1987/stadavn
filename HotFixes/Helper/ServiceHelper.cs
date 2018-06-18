using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.HotFixes.Helper
{
    public class ServiceHelper
    {
        public static void UpdateCustomWCF()
        {
            SPWebService contentService = SPWebService.ContentService;
            SPWcfServiceSettings wcfServiceSettings = new SPWcfServiceSettings();

            wcfServiceSettings.ReaderQuotasMaxStringContentLength = Int32.MaxValue;
            wcfServiceSettings.ReaderQuotasMaxArrayLength = Int32.MaxValue;
            wcfServiceSettings.ReaderQuotasMaxBytesPerRead = Int32.MaxValue;
            wcfServiceSettings.MaxReceivedMessageSize = Int32.MaxValue;

            // Note: "runtime.svc" must be in lowercase
            contentService.WcfServiceSettings.Remove("calendarservice.svc");
            contentService.WcfServiceSettings.Remove("commonservice.svc");
            contentService.WcfServiceSettings.Remove("departmentservice.svc");
            contentService.WcfServiceSettings.Remove("employeeservice.svc");
            contentService.WcfServiceSettings.Remove("shifttimeservice.svc");
            contentService.WcfServiceSettings.Remove("shiftmanagementservice.svc");
            contentService.WcfServiceSettings.Remove("changeshiftmanagementservice.svc");
            contentService.WcfServiceSettings.Remove("overtimeservice.svc");
            contentService.WcfServiceSettings.Remove("notoverTimemanagementservice.svc");
            contentService.WcfServiceSettings.Remove("vehiclemanagementservice.svc");
            contentService.WcfServiceSettings.Remove("leavemanagementservice.svc");
            contentService.WcfServiceSettings.Remove("freightmanagementservice.svc");
            contentService.WcfServiceSettings.Remove("businesstripmanagementservice.svc");



            //contentService.WcfServiceSettings["calendarservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["commonservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["departmentservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["employeeservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["shifttimeservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["shiftmanagementservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["changeshiftmanagementservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["overtimeservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["notoverTimemanagementservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["vehiclemanagementservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["leavemanagementservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["freightmanagementservice.svc"] = wcfServiceSettings;
            //contentService.WcfServiceSettings["businesstripmanagementservice.svc"] = wcfServiceSettings;

            contentService.Update(true);
        }
    }
}
