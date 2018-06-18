using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveManagementControl
{
    public partial class LeaveSecurityGuardControl : UserControl
    {
        private const string baseViewID = "6";
        private LeaveManagementDAL leaveManagementDAL;
        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            var webUrl = SPContext.Current.Web.Url;
            leaveManagementDAL = new LeaveManagementDAL(webUrl);
            var guidViews = leaveManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            LeaveRequestForSecurityWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            string employeeID = this.Page.Request.Params.Get("employeeId");

            XElement filterElement = BuildViewString(webUrl, employeeID);

            XElement xmlViewDef = XElement.Parse(LeaveRequestForSecurityWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                LeaveRequestForSecurityWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(string webUrl, string employeeID)
        {
            string queryStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";
            XElement xmlQuery;

            if (!string.IsNullOrEmpty(employeeID))
            {
                employeeID = employeeID.Trim();

                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(webUrl);
                EmployeeInfo employeeInfo = _employeeInfoDAL.GetByEmployeeID(employeeID);
                if (employeeInfo != null)
                {
                    queryStr = string.Format(@"<And>
                          <Or>
                              <Eq>
                                  <FieldRef Name='CommonFrom' />
                                  <Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value>
                              </Eq>
                              <Eq>
                                  <FieldRef Name='To' />
                                  <Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value>
                              </Eq>
                          </Or>
                          <And>
                              <Eq>
                                  <FieldRef Name='RequestFor' LookupId='TRUE'/>
                                  <Value Type='Lookup'>{1}</Value>
                              </Eq>
                              <Eq>
                                  <FieldRef Name='ApprovalStatus' />
                                  <Value Type='Text'>Approved</Value>
                              </Eq>
                          </And>
                    </And>", DateTime.Now.ToString(StringConstant.DateFormatForCAML), employeeInfo.ID);
                }
            }

            xmlQuery = XElement.Parse(queryStr);

            return xmlQuery;
        }
    }
}
