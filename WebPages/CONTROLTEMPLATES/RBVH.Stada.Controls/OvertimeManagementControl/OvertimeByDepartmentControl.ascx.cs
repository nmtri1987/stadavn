using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.OvertimeManagementControl
{
    public partial class OvertimeByDepartmentControl : UserControl
    {
        private OverTimeManagementDAL overTimeManagementDAL;
        private const string BASE_VIEW_ID = "3";
        private const string HR_DEPARTMENT_CODE = "HR";
        protected bool isBOD = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            initialViewGUID();
        }
        private void initialViewGUID()
        {
            var url = SPContext.Current.Web.Url;
            overTimeManagementDAL = new OverTimeManagementDAL(url);
            var guidViews = overTimeManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == BASE_VIEW_ID).FirstOrDefault();
            OvertimeDepartment.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();
            CheckCurrentUser(url);

            bool isAdminDepartment = UserPermission.IsAdminDepartment;

            XElement xmlViewDef = XElement.Parse(OvertimeDepartment.XmlDefinition);
            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            XElement filterElement = BuildViewString(isBOD, isAdminDepartment, selectedDepId);

            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                OvertimeDepartment.XmlDefinition = xmlViewDef.ToString();
            }
        }
        private void CheckCurrentUser(string currentWebURL)
        {
            if (UserPermission.IsCurrentUserInGroup(StringConstant.BOD))
            {
                isBOD = true;
            }
            else
            {
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                var currentDepartment = DepartmentListSingleton.GetDepartmentByID(currentEmployee.Department.LookupId, currentWebURL);
                if (currentDepartment != null)
                {
                    if (currentDepartment.Code == HR_DEPARTMENT_CODE)
                    {
                        isBOD = false;
                    }
                }
            }
        }

        private XElement BuildViewString(bool isBOD, bool isAdminDepartment, string departmentId)
        {
            var url = SPContext.Current.Web.Url;

            UserHelper userHelper = new UserHelper();
            EmployeeInfo currentEmployeeInfo = userHelper.GetCurrentLoginUser();
            
            XElement filterElement = null;
            string deptFilterStr = @"<Eq>
                                        <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{DepartmentId}</Value>
                                    </Eq>";

            if (isBOD || isAdminDepartment)
            {
                if ((!string.IsNullOrEmpty(departmentId) && departmentId.Trim().Equals("0")) && (isBOD || isAdminDepartment))
                {
                    deptFilterStr = @"<IsNotNull>
                                        <FieldRef Name='CommonDepartment' />
                                    </IsNotNull>";
                }
            }

            deptFilterStr = $@"<And>
                                    {deptFilterStr}
                                    <Eq>
                                        <FieldRef Name='CommonLocation' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{currentEmployeeInfo.FactoryLocation.LookupId}</Value>
                                    </Eq>
                                </And>";

            string statusQuery = @"<Gt>
                                    <FieldRef Name='ID' />
                                    <Value Type='Counter'>0</Value>
                                </Gt>";

            if (!isBOD && isAdminDepartment)
            {
                statusQuery = @"<Eq>
                                    <FieldRef Name='ApprovalStatus' />
                                    <Value Type='Text'>true</Value>
                                </Eq>";
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
                    toDateValue = $"{dtToDate:yyyy-MM-dd}";
            }

            filterElement = XElement.Parse($@"<And>{statusQuery}
                                                    <And>
                                                        {deptFilterStr}
                                                        <And>
                                                            <Leq>
                                                                <FieldRef Name='CommonDate' />
                                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDateValue}</Value>
                                                            </Leq>
                                                            <Geq>
                                                                <FieldRef Name='CommonDate' />
                                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDateValue}</Value>
                                                            </Geq>
                                                        </And>
                                                  </And>
                                            </And>");

            return filterElement;
        }
    }
}
