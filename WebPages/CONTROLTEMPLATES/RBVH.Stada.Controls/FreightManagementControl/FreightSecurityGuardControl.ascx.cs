using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Web.UI;
using System.Linq;
using RBVH.Stada.Intranet.WebPages.Utils;
using System.Xml.Linq;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.FreightManagementControl
{
    public partial class FreightSecurityGuardControl : UserControl
    {
        private const string baseViewID = "6";
        private FreightManagementDAL freightManagementDAL;
        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            var url = SPContext.Current.Web.Url;
            freightManagementDAL = new FreightManagementDAL(url);
            var guidViews = freightManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            FreightRequestForSecurityWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            string selectedVehicleId = this.Page.Request.Params.Get("AdminVehicleId");
            string requestNumber = this.Page.Request.Params.Get("reqnum");
            string searchType = this.Page.Request.Params.Get("searchtype");

            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();

            XElement filterElement = BuildViewString(currentEmployee, selectedDepId, selectedVehicleId, requestNumber, searchType);

            XElement xmlViewDef = XElement.Parse(FreightRequestForSecurityWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                FreightRequestForSecurityWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(EmployeeInfo currentEmployee, string departmentId, string vehicleId, string requestNumber, string searchType)
        {
            string filterStr = @"<Eq>
                                    <FieldRef Name='ID' />
                                    <Value Type='Counter'>0</Value>
                                </Eq>";

            if (!string.IsNullOrEmpty(searchType) && searchType.Trim().Equals("1") && !string.IsNullOrEmpty(requestNumber) && !requestNumber.Trim().Equals("0"))
            {
                filterStr = @"<And>
                                <And>
                                    <Eq>
                                        <FieldRef Name='IsFinished' />
                                        <Value Type='Boolean'>0</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='ApprovalStatus' />
                                        <Value Type='Text'>Approved</Value>
                                    </Eq>
                                </And>
                                <Contains>
                                    <FieldRef Name='RequestNo'/>
                                    <Value Type='Text'>{reqnum}</Value>
                                </Contains>
                            </And>";
            }
            else
            {
                filterStr = @"<And>
                                <And>
                                    <Eq>
                                        <FieldRef Name='IsFinished' />
                                        <Value Type='Boolean'>0</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='ApprovalStatus' />
                                        <Value Type='Text'>Approved</Value>
                                    </Eq>
                                </And>
                                <And>
                                    <Geq>
                                        <FieldRef Name='TransportTime' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{SelectedDate}</Value>
                                    </Geq>
                                    <Leq>
                                        <FieldRef Name='TransportTime' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{SelectedToDate}</Value>
                                    </Leq>
                                </And>
                            </And>";

                if (!string.IsNullOrEmpty(departmentId) && !departmentId.Trim().Equals("0"))
                {
                    filterStr = @"<And>"
                                    + filterStr +
                                    @"<Eq>
                                        <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{DepartmentId}</Value>
                                    </Eq>
                                </And>";
                }
            }

            filterStr = $@"<And>{filterStr}<Eq><FieldRef Name='CommonLocation' LookupId='TRUE'/><Value Type='Lookup'>{currentEmployee.FactoryLocation.LookupId}</Value></Eq></And>";

            XElement xmlQuery = XElement.Parse(filterStr);

            return xmlQuery;
        }
    }
}
