using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Web.UI;
using System.Linq;
using RBVH.Stada.Intranet.WebPages.Utils;
using System.Xml.Linq;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using RBVH.Stada.Intranet.Biz.Models;
using System.Globalization;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.FreightManagementControl
{
    public partial class VehicleOperatorControl : UserControl
    {
        private const string baseViewID = "7";
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
            FreightRequestVehicleOperatorWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            string selectedVehicleId = this.Page.Request.Params.Get("AdminVehicleId");

            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            XElement filterElement = BuildViewString(currentEmployee, selectedDepId, selectedVehicleId);

            XElement xmlViewDef = XElement.Parse(FreightRequestVehicleOperatorWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                FreightRequestVehicleOperatorWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(EmployeeInfo currentEmployeeInfo, string departmentId, string vehicleId)
        {
            XElement filterElement = null;

            string deptFilterStr = @"<Eq>
                                        <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{DepartmentId}</Value>
                                    </Eq>";

            if (!string.IsNullOrEmpty(departmentId) && departmentId.Trim().Equals("0"))
            {
                deptFilterStr = @"<IsNotNull>
                                    <FieldRef Name='CommonDepartment' />
                                </IsNotNull>";
            }

            string fromDate = this.Page.Request.Params.Get("AdminFromDate");
            string fromDateValue = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime dtFromDate;
                bool isValidFromDate = DateTime.TryParseExact(fromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFromDate);
                if (isValidFromDate)
                    fromDateValue = $"{dtFromDate:yyyy-MM-dd}";
            }

            string toDate = this.Page.Request.Params.Get("AdminToDate");
            string toDateValue = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime dtToDate;
                bool isValidToDate = DateTime.TryParseExact(toDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtToDate);
                if (isValidToDate)
                    toDateValue = $"{dtToDate:yyyy-MM-dd};";
            }

            string filterStr = $@"<And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='CompanyVehicle' />
                                            <Value Type='Boolean'>1</Value>
                                        </Eq>
                                        {deptFilterStr}
                                    </And>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='ApprovalStatus'  />
                                            <Value Type='Text'>Approved</Value>
                                        </Eq>
                                        <And>
                                            <Geq>
                                                <FieldRef Name='TransportTime' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDateValue}</Value>
                                            </Geq>
                                            <Leq>
                                                <FieldRef Name='TransportTime' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDateValue}</Value>
                                            </Leq>
                                        </And>
                                    </And>
                                </And>";


            if (!string.IsNullOrEmpty(vehicleId) && !vehicleId.Trim().Equals("0"))
            {
                filterStr = @"<And>"
                                + filterStr +
                                @"<Eq>
                                    <FieldRef Name='VehicleLookup' LookupId='TRUE'/>
                                    <Value Type='Lookup'>{VehicleId}</Value>
                                </Eq>
                            </And>";
            }

            filterStr = $@"<And>{filterStr}<Eq><FieldRef Name='CommonLocation' LookupId='TRUE'/><Value Type='Lookup'>{currentEmployeeInfo.FactoryLocation.LookupId}</Value></Eq></And>";

            filterElement = XElement.Parse(filterStr);

            return filterElement;
        }
    }
}
