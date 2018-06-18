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
    public partial class BusinessTripByExtAdminControl : UserControl
    {
        private BusinessTripManagementDAL businessTripManagementDAL;
        private const string baseViewID = "4";
        protected void Page_Load(object sender, EventArgs e)
        {
            var webUrl = SPContext.Current.Web.Url;
            UserHelper userHelper = new UserHelper();
            EmployeeInfo currentEmployeeInfo = userHelper.GetCurrentLoginUser();

            InitialViewGUID(webUrl);
            BuildViewFilter(currentEmployeeInfo);
        }

        private void InitialViewGUID(string webUrl)
        {
            businessTripManagementDAL = new BusinessTripManagementDAL(webUrl);
            var guidViews = businessTripManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            BusinessTripByExtAdminWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();
        }

        private void BuildViewFilter(EmployeeInfo currentEmployeeInfo)
        {
            string selectedDepId = this.Page.Request.Params.Get("AdminDeptId");
            XElement filterElement = BuildViewString(currentEmployeeInfo, selectedDepId);

            XElement xmlViewDef = XElement.Parse(BusinessTripByExtAdminWebPart.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                BusinessTripByExtAdminWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(EmployeeInfo currentEmployeeInfo, string departmentId)
        {
            XElement filterElement = null;
            string deptFilterStr = @"<Eq>
                                        <FieldRef Name='CommonDepartment' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{DepartmentId}</Value>
                                    </Eq>";

            if ((!string.IsNullOrEmpty(departmentId) && departmentId.Trim().Equals("0")))
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

            string filterStr = @"<And>"
                                + deptFilterStr +
                                    @"<And>
                                        <Eq>
                                            <FieldRef Name='ApprovalStatus' />
                                            <Value Type='Text'>Approved</Value>
                                        </Eq>
                                        <And>
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
                            </And>";

            filterElement = XElement.Parse(filterStr);

            return filterElement;
        }
    }
}
