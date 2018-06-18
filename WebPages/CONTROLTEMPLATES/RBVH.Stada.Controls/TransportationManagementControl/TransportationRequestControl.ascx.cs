using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.TransportationManagementControl
{
    public partial class TransportationRequestControl : UserControl
    {
        private VehicleManagementDAL vehicleManagementDAL;
        private const string baseViewID = "3";
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCurrentUser();
            try
            {
                var url = SPContext.Current.Web.Url;
                vehicleManagementDAL = new VehicleManagementDAL(url);
                var guidViews = vehicleManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
                TransportationRequestControlWP.ViewGuid = guidViews.ID.ToString();
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - TransportationRequestControl - InitData", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }
        private void GetCurrentUser()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
        }
    }
}
