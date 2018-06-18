using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.BusinessTripManagementControl
{
    public partial class BusinessTripByDepartmentControl : UserControl
    {
        private BusinessTripManagementDAL businessTripManagementDAL;
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
            businessTripManagementDAL = new BusinessTripManagementDAL(url);
            var guidViews = businessTripManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            BusinessTripByDepartmentWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            UserHelper userHelper = new UserHelper();
            EmployeeInfo currentEmployeeInfo = userHelper.GetCurrentLoginUser();

            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            XElement filterElement = BuildViewString(currentEmployeeInfo, selectedDepId);

            XElement xmlViewDef = XElement.Parse(BusinessTripByDepartmentWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                BusinessTripByDepartmentWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(EmployeeInfo currentEmployeeInfo, string departmentId)
        {
            EmployeeRole currentUserRole = UserPermission.GetCurrentUserRole(currentEmployeeInfo);
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

            filterElement = XElement.Parse(@"<And>" + statusQuery +
                                                    @"<And>" +
                                                        deptFilterStr +
                                                        @"<And>
                                                            <Geq>
                                                                <FieldRef Name='Created' />
                                                                <Value IncludeTimeValue='TRUE' Type='DateTime'>{StartMonth}</Value>
                                                            </Geq>
                                                            <Leq>
                                                                <FieldRef Name='Created' />
                                                                <Value IncludeTimeValue='TRUE' Type='DateTime'>{EndMonth}</Value>
                                                            </Leq>
                                                        </And>
                                                  </And>
                                            </And>");

            return filterElement;
        }
    }
}
