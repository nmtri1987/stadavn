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

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ChangeShiftManagementControl
{
    public partial class ChangeShiftByDepartmentControl : UserControl
    {
        private ChangeShiftManagementDAL changeShiftManagementDal;
        private const string BASE_VIEW_ID = "1";
        private const string HR_DEPARTMENT_CODE = "HR";
        protected bool isBOD = false;
        protected bool IsShowAllItems = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            var url = SPContext.Current.Web.Url;
            changeShiftManagementDal = new ChangeShiftManagementDAL(url);
            var guidViews = changeShiftManagementDal.GetViewGuildID().Where(x => x.BaseViewID == BASE_VIEW_ID).FirstOrDefault();
            ChangeShiftByDepartmentWebPart.ViewGuid = guidViews.ID.ToString();
            CheckCurrentUser(url);

            bool isAdminDepartment = UserPermission.IsAdminDepartment;
            XElement xmlViewDef = XElement.Parse(ChangeShiftByDepartmentWebPart.XmlDefinition);
            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            XElement filterElement = BuildViewString(isAdminDepartment, selectedDepId);

            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                ChangeShiftByDepartmentWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }
        private void CheckCurrentUser(string currentWebURL)
        {
            isBOD = false;
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

        private XElement BuildViewString(bool isAdminDepartment, string departmentId)
        {
            var url = SPContext.Current.Web.Url;
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(url);
            EmployeeInfo currentEmployeeInfo = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);

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
                                                                <Value IncludeTimeValue='TRUE' Type='CommonFrom'>{FromDate}</Value>
                                                            </Geq>
                                                            <Leq>
                                                                <FieldRef Name='Created' />
                                                                <Value IncludeTimeValue='TRUE' Type='CommonFrom'>{ToDate}</Value>
                                                            </Leq>
                                                        </And>
                                                  </And>
                                            </And>");

            return filterElement;
        }
    }
}

