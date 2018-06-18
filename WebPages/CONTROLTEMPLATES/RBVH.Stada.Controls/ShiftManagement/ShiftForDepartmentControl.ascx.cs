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

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ShiftManagement
{
    public partial class ShiftForDepartmentControl : UserControl
    {
        private const string BaseViewId = "1";
        protected void Page_Load(object sender, EventArgs e)
        {
            bool isAdminDepartment = UserPermission.IsAdminDepartment;
            bool isBOD = UserPermission.IsCurrentUserInGroup(StringConstant.BOD);

            XElement xmlViewDef = XElement.Parse(ShiftRequestDepartmentWebPart.XmlDefinition);
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
                ShiftRequestDepartmentWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(bool isBOD, bool isAdminDepartment, string departmentId)
        {
            var url = SPContext.Current.Web.Url;
            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(url);
            EmployeeInfo currentEmployeeInfo = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);

            XElement filterElement = null;
            string deptFilterStr = @"<Eq>
                                        <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{ShiftDepartmentParam_DeptId}</Value>
                                    </Eq>";

            if ((!string.IsNullOrEmpty(departmentId) && departmentId.Trim().Equals("0")) && (isBOD || isAdminDepartment))
            {
                deptFilterStr = @"<IsNotNull>
                                    <FieldRef Name='CommonDepartment' />
                                </IsNotNull>";
            }

            deptFilterStr = $@"<And>
                                    {deptFilterStr}
                                    <Eq>
                                        <FieldRef Name='CommonLocation' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{currentEmployeeInfo.FactoryLocation.LookupId}</Value>
                                    </Eq>
                                </And>";

            filterElement = XElement.Parse(@"<And>" +
                                                    deptFilterStr +
                                                    @"<And>
                                                        <Eq>
                                                            <FieldRef Name='CommonMonth' />
                                                            <Value Type='Lookup'>{ShiftDepartmentParam_Month}</Value>
                                                        </Eq>
                                                        <Eq>
                                                            <FieldRef Name='CommonYear' />
                                                            <Value Type='Lookup'>{ShiftDepartmentParam_Year}</Value>
                                                        </Eq>
                                                    </And>
                                            </And>");

            return filterElement;
        }
    }
}
