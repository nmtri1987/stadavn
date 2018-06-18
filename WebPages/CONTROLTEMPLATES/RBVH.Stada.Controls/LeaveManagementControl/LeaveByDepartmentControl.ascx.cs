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
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveManagementControl
{
    public partial class LeaveByDepartmentControl : UserControl
    {
        private LeaveManagementDAL leaveManagementDAL;
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
            leaveManagementDAL = new LeaveManagementDAL(url);
            var guidViews = leaveManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            LeaveByDepartmentWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            UserHelper userHelper = new UserHelper();
            EmployeeInfo currentEmployeeInfo = userHelper.GetCurrentLoginUser();

            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            XElement filterElement = BuildViewString(currentEmployeeInfo, selectedDepId);

            XElement xmlViewDef = XElement.Parse(LeaveByDepartmentWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                LeaveByDepartmentWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(EmployeeInfo currentEmployeeInfo, string departmentId)
        {
            EmployeeRole currentUserRole = UserPermission.CurrentUserRole;
            XElement filterElement = null;
            string deptFilterStr = @"<Eq>
                                        <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{DepartmentId}</Value>
                                    </Eq>";

            if (currentUserRole == EmployeeRole.BOD || currentUserRole == EmployeeRole.AdminOfHR || currentUserRole == EmployeeRole.DepartmentHeadOfHR)
            {
                if ((!string.IsNullOrEmpty(departmentId) && departmentId.Trim().Equals("0")))
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

            if (currentUserRole != EmployeeRole.BOD && currentUserRole == EmployeeRole.AdminOfHR)
            {
                statusQuery = @"<Eq>
                                    <FieldRef Name='ApprovalStatus'  />
                                    <Value Type='Text'>Approved</Value>
                                </Eq>";
            }

            string fromDate = this.Page.Request.Params.Get("AdminFromDate");
            //string fromDateValue = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
            DateTime fromDateValue = DateTime.Now;
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime dtFromDate;
                bool isValidFromDate = DateTime.TryParseExact(fromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFromDate);
                if (isValidFromDate)
                    fromDateValue = dtFromDate; // $"{dtFromDate:yyyy-MM-dd}";
            }

            string toDate = this.Page.Request.Params.Get("AdminToDate");
            //string toDateValue = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
            DateTime toDateValue = DateTime.Now;
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime dtToDate;
                bool isValidToDate = DateTime.TryParseExact(toDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtToDate);
                if (isValidToDate)
                    toDateValue = dtToDate; // $"{dtToDate:yyyy-MM-dd};";
            }

            if (fromDateValue.Date <= toDateValue.Date)
            {
                filterElement = XElement.Parse($@"<And>{statusQuery}
                                                    <And>
                                                        {deptFilterStr}
                                                        <And>
                                                            <Leq>
                                                                <FieldRef Name='CommonFrom' />
                                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDateValue.ToString("yyyy-MM-dd")}</Value>
                                                            </Leq>
                                                            <Geq>
                                                                <FieldRef Name='To' />
                                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDateValue.ToString("yyyy-MM-dd")}</Value>
                                                            </Geq>
                                                        </And>
                                                  </And>
                                            </And>");
            }
            else
            {
                filterElement = XElement.Parse(@"<Eq><FieldRef Name='ID'/><Value Type='Counter'>0</Value></Eq>");
            }

            return filterElement;
        }
    }
}
