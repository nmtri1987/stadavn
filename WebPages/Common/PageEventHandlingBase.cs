using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.WebPages.Common
{
    public class PageEventHandlingBase : LayoutsPageBase
    {
        public string PageName { get; private set; }
        private Stopwatch stopWatch = new Stopwatch();
        private bool EnableServerLogging
        {
            get
            {
                var value = Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["EnableServerLogging"]);

                return value == true;
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (!EnableServerLogging)
                    return;

                stopWatch.Start();
                PageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                // Begin log:
                ULSLogging.LogMessageToFile($"-- BEGIN {PageName} at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (!EnableServerLogging)
                    return;

                TimeSpan ts = stopWatch.Elapsed;
                // End log:
                ULSLogging.LogMessageToFile($"-- END {PageName} at: {DateTime.Now}, DURATION: {ts.Milliseconds / 10} (Milliseconds)");
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
    }
}
