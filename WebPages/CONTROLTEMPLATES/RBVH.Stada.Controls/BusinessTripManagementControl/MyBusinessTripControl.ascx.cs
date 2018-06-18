using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Web.UI;
using System.Linq;
using RBVH.Stada.Intranet.WebPages.Utils;
using System.Xml.Linq;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.Constants;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.BusinessTripManagementControl
{
    public partial class MyBusinessTripControl : UserControl
    {
        private BusinessTripManagementDAL businessTripManagementDAL;
        private const string baseViewID = "7";
        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }
        private void InitialViewGUID()
        {
            var siteUrl = SPContext.Current.Web.Url;
            businessTripManagementDAL = new BusinessTripManagementDAL(siteUrl);
            var guidViews = businessTripManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            MyBusinessTripRequestWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            UserHelper userHelper = new UserHelper();
            EmployeeInfo employeeInfo = userHelper.GetCurrentLoginUser(); 

            XElement xmlViewDef = XElement.Parse(MyBusinessTripRequestWebPart.XmlDefinition);
            XElement filterElement = BuildViewString(employeeInfo, siteUrl);

            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                MyBusinessTripRequestWebPart.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private void GetCurrentUser()
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
        }

        private XElement BuildViewString(EmployeeInfo employeeInfo, string siteUrl)
        {
            XElement filterElement = null;
            string filterStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";

            if (employeeInfo == null) return XElement.Parse(filterStr);

            BusinessTripEmployeeDetailDAL _businessTripEmployeeDetailDAL = new BusinessTripEmployeeDetailDAL(siteUrl);
            string taskQueryStr = string.Format(@"<Where>
                                    <Eq>
                                        <FieldRef Name='Employee' LookupId='TRUE' />
                                        <Value Type='Lookup'>{0}</Value>
                                    </Eq>
                                </Where>", employeeInfo.ID);

            List<BusinessTripEmployeeDetail> employeeDetailCollection = _businessTripEmployeeDetailDAL.GetByQuery(taskQueryStr);
            if (employeeDetailCollection.Any())
            {
                List<int> itemIds = employeeDetailCollection.Select(t => t.BusinessTripManagementID.LookupId).Distinct().ToList();

                if (itemIds != null && itemIds.Count > 0)
                {
                    filterStr = "";
                    foreach (var itemId in itemIds)
                    {
                        filterStr += string.Format("<Value Type='Counter'>{0}</Value>", itemId);
                    }
                    if (!string.IsNullOrEmpty(filterStr))
                    {
                        filterStr = string.Format("<In><FieldRef Name='ID'/><Values>{0}</Values></In>", filterStr);
                    }
                }
            }

            filterElement = XElement.Parse(filterStr);

            return filterElement;
        }
    }
}
