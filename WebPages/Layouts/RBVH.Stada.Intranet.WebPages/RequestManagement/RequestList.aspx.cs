using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.WebPages.Common;
using System;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.RequestManagement
{
    /// <summary>
    /// RequestList class.
    /// </summary>
    public partial class RequestList : PageEventHandlingBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
    }
}
