using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ChangeShiftManagementControl
{
    public partial class ChangeShiftApprovalControl : UserControl
    {
        private ChangeShiftManagementDAL _changeShiftManagementDAL;
        private string siteUrl;
        private const string baseViewID = "3";

        protected void Page_Load(object sender, EventArgs e)
        {
            siteUrl = SPContext.Current.Site.Url;
            _changeShiftManagementDAL = new ChangeShiftManagementDAL(siteUrl);
            int currentUserId = SPContext.Current.Web.CurrentUser.ID;
            string itemIdParam = this.Page.Request.Params.Get("itemId");

            XElement filterElement = null;
            Biz.Models.Delegation delegation = null;

            if (!string.IsNullOrEmpty(itemIdParam))
            {
                delegation = _changeShiftManagementDAL.GetDelegatedTaskInfo(itemIdParam);
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

        private void InitialViewGUID(XElement filterElement)
        {
            var guidViews = _changeShiftManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            ChangeShiftApprovalWebPart.ViewGuid = guidViews.ID.ToString();

            XElement xmlViewDef = XElement.Parse(ChangeShiftApprovalWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null && filterElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                ChangeShiftApprovalWebPart.XmlDefinition = xmlViewDef.ToString();
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
            if (delegation != null && delegation.Requester!=null && delegation.Requester.LookupId > 0)
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
