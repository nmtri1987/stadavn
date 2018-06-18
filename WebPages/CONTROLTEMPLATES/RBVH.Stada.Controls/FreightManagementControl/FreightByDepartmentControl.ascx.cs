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
    public partial class FreightByDepartmentControl : UserControl
    {
        private FreightManagementDAL freightManagementDAL;
        protected bool isBOD = false;
        private const string baseViewID = "4";
        private const string HR_DEPARTMENT_CODE = "HR";

        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            var url = SPContext.Current.Web.Url;
            freightManagementDAL = new FreightManagementDAL(url);
            var guidViews = freightManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            FreightByDepartmentWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            UserHelper userHelper = new UserHelper();
            EmployeeInfo currentEmployeeInfo = userHelper.GetCurrentLoginUser();

            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            string selectedVehicleId = this.Page.Request.Params.Get("AdminVehicleId");
            XElement filterElement = BuildViewString(currentEmployeeInfo, selectedDepId, selectedVehicleId);

            XElement xmlViewDef = XElement.Parse(FreightByDepartmentWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                FreightByDepartmentWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(EmployeeInfo currentEmployeeInfo, string departmentId, string vehicleId)
        {
            EmployeeRole currentUserRole = UserPermission.GetCurrentUserRole(currentEmployeeInfo);
            XElement filterElement = null;

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
                                    <Geq>
                                        <FieldRef Name='TransportTime' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDateValue}</Value>
                                    </Geq>
                                    <Leq>
                                        <FieldRef Name='TransportTime' />
                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDateValue}</Value>
                                    </Leq>
                                </And>";

            string deptFilterStr = @"<Eq>
                                        <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{DepartmentId}</Value>
                                    </Eq>";

            string vehicleFilterStr = @"<Eq>
                                        <FieldRef Name='VehicleLookup' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{VehicleId}</Value>
                                    </Eq>";

            if (currentUserRole == EmployeeRole.BOD || currentUserRole == EmployeeRole.AdminOfHR || currentUserRole == EmployeeRole.DepartmentHeadOfHR)
            {
                if (!string.IsNullOrEmpty(departmentId) && !departmentId.Trim().Equals("0"))
                {
                    filterStr = string.Format("<And>{0}{1}</And>", deptFilterStr, filterStr);
                }

                if (!string.IsNullOrEmpty(vehicleId) && !vehicleId.Trim().Equals("0"))
                {
                    filterStr = string.Format("<And>{0}{1}</And>", vehicleFilterStr, filterStr);
                }
            }

            filterStr = $@"<And>{filterStr}<Eq><FieldRef Name='CommonLocation' LookupId='TRUE'/><Value Type='Lookup'>{currentEmployeeInfo.FactoryLocation.LookupId}</Value></Eq></And>";

            if (currentUserRole != EmployeeRole.BOD && currentUserRole == EmployeeRole.AdminOfHR)
            {
                filterStr = string.Format(@"<And>{0}<Eq><FieldRef Name='ApprovalStatus' /><Value Type='Text'>Approved</Value></Eq></And>", filterStr);
            }

            filterElement = XElement.Parse(filterStr);

            return filterElement;
        }
    }
}
