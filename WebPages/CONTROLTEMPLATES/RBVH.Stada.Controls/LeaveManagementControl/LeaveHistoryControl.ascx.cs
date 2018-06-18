using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveManagementControl
{
    public partial class LeaveHistoryControl : UserControl
    {
        private LeaveManagementDAL _leaveManagementDAL;
        private const string baseViewID = "5";
        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            var url = SPContext.Current.Web.Url;
            _leaveManagementDAL = new LeaveManagementDAL(url);
            var guidViews = _leaveManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            LeaveHistoryWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            XElement xmlViewDef = XElement.Parse(LeaveHistoryWebPart.XmlDefinition);
            XElement filterElement = BuildViewString();

            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                LeaveHistoryWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString()
        {
            XElement filterElement = null;

            string queryStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";

            try
            {
                string statusFilter = string.Format(@"<Eq>
                                                       <FieldRef Name='{0}' />
                                                       <Value Type='Text'>{1}</Value>
                                                    </Eq>", StringConstant.CommonSPListField.ApprovalStatusField, StringConstant.ApprovalStatus.Approved);
                queryStr = @"<And>
                                <Eq>
                                    <FieldRef Name='RequestFor' LookupId='TRUE' />
                                    <Value Type='Lookup'>{EmployeeIdParam}</Value>
                                </Eq>
                                <And>"
                                  + statusFilter +
                                @"<And>
                                    <Leq>
                                        <FieldRef Name='CommonFrom' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{ToDateParam}</Value>
                                    </Leq>
                                    <Geq>
                                        <FieldRef Name='To' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{FromDateParam}</Value>
                                    </Geq>
                                </And>
                                </And>
                            </And>";
            }
            catch { }

            filterElement = XElement.Parse(queryStr);

            return filterElement;
        }
    }
}
