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

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveOfAbsenceManagementControl
{
    public partial class LeaveOfAbsenceDepartmentControl : UserControl
    {
        private NotOvertimeManagementDAL overTimeManagementDAL;
        private const string baseViewID = "2";
        private const string HR_DEPARTMENT_CODE = "HR";
        protected bool isBOD = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            var url = SPContext.Current.Web.Url;
            overTimeManagementDAL = new NotOvertimeManagementDAL(url);
            var guidViews = overTimeManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            NotOverTimeDepartmentControl.ViewGuid = NotOverTimeDepartmentControl.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            UserHelper userHelper = new UserHelper();
            EmployeeInfo currentEmployeeInfo = userHelper.GetCurrentLoginUser();

            CheckCurrentUser(url, currentEmployeeInfo.Department != null ? currentEmployeeInfo.Department.LookupId : 0);

            bool isAdminDepartment = UserPermission.IsAdminDepartment;
            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            XElement filterElement = BuildViewString(isAdminDepartment, selectedDepId, currentEmployeeInfo.FactoryLocation.LookupId);

            XElement xmlViewDef = XElement.Parse(NotOverTimeDepartmentControl.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                NotOverTimeDepartmentControl.XmlDefinition = xmlViewDef.ToString();
            }
        }
        private void CheckCurrentUser(string currentWebURL, int departmentId)
        {

            if (UserPermission.IsCurrentUserInGroup(StringConstant.BOD))
            {
                isBOD = true;
            }
            else
            {
                var currentDepartment = DepartmentListSingleton.GetDepartmentByID(departmentId, currentWebURL);
                if (currentDepartment != null)
                {
                    if (currentDepartment.Code == HR_DEPARTMENT_CODE)
                    {
                        isBOD = false;
                    }
                }
            }
        }
        private XElement BuildViewString(bool isAdminDepartment, string departmentId, int locationId)
        {
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
                                        <Value Type='Lookup'>{locationId}</Value>
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
