using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveOfAbsenceManagementControl
{
    public partial class LeaveOfAbsenceApprovalControl : UserControl
    {
        private NotOvertimeManagementDAL _notOvertimeManagementDAL;
        private string siteUrl;
        private const string baseViewID = "4";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                siteUrl = SPContext.Current.Site.Url;
                _notOvertimeManagementDAL = new NotOvertimeManagementDAL(siteUrl);

                int currentUserId = SPContext.Current.Web.CurrentUser.ID;
                string itemIdParam = this.Page.Request.Params.Get("itemId");

                XElement filterElement = null;
                Biz.Models.Delegation delegation = null;

                if (!string.IsNullOrEmpty(itemIdParam))
                {
                    delegation = _notOvertimeManagementDAL.GetDelegatedTaskInfo(itemIdParam);
                }

                if (delegation != null)
                {
                    filterElement = BuildViewFilterStringForDelegation(delegation);
                }
                else
                {
                    filterElement = BuildViewFilterString(currentUserId);
                }

                InitialViewGUID(filterElement);
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - LeaveOfAbsenceApprovalControl - InitData", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        private void InitialViewGUID(XElement filterElement)
        {
            var guidViews = _notOvertimeManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            NotOverTimeApprovalControl.ViewGuid = guidViews.ID.ToString();

            XElement xmlViewDef = XElement.Parse(NotOverTimeApprovalControl.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null && filterElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                NotOverTimeApprovalControl.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewFilterString(int currentUserId)
        {
            XElement filterElement = null;
            string filterStr = $@"<And>
                                    <Eq>
                                        <FieldRef Name='CommonApprover1' LookupId='True' />
                                        <Value Type='User' LookupId='True'>{currentUserId}</Value>
                                    </Eq>
                                    <IsNull>
                                        <FieldRef Name='ApprovalStatus' />
                                    </IsNull>
                                </And>";
            filterElement = XElement.Parse(filterStr);

            return filterElement;
        }

        private XElement BuildViewFilterStringForDelegation(Biz.Models.Delegation delegation)
        {
            XElement filterElement = null;
            string filterStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";
            if (delegation != null && delegation.Requester != null && delegation.Requester.LookupId > 0)
            {
                filterStr = $@"<And>
                                    <Eq>
                                        <FieldRef Name='ID' />
                                        <Value Type='Counter'>{delegation.ListItemID}</Value>
                                    </Eq>
                                    <IsNull>
                                        <FieldRef Name='ApprovalStatus' />
                                    </IsNull>
                                </And>";
            }
            filterElement = XElement.Parse(filterStr);

            return filterElement;
        }
    }
}
