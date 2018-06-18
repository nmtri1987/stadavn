using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.TransportationManagementControl
{
    public partial class TransportationApprovalControl : UserControl
    {
        private VehicleManagementDAL vehicleManagementDAL;
        private const string baseViewID = "1";

        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            var siteUrl = SPContext.Current.Web.Url;
            vehicleManagementDAL = new VehicleManagementDAL(siteUrl);
            var guidViews = vehicleManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            TransportationApprovalControlWP.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            XElement filterElement = BuildViewString(SPContext.Current.Web.CurrentUser.ID, siteUrl);

            XElement xmlViewDef = XElement.Parse(TransportationApprovalControlWP.XmlDefinition);
            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                TransportationApprovalControlWP.XmlDefinition = xmlViewDef.ToString();
            }
        }

        private XElement BuildViewString(int assigneeId, string siteUrl)
        {
            XElement filterElement = null;
            string filterStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";
            
            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(siteUrl);
            string taskQueryStr = string.Format(@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Status' />
                                        <Value Type='Choice'>{0}</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{1}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                            <Value Type='User'>{2}</Value>
                                        </Eq>
                                     </And>
                                  </And>
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.VehicleManagement.ToString(), assigneeId);
            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                List<int> itemIds = taskManagementCollection.Where(t => t.ItemId > 0).Select(t => t.ItemId).ToList();

                if (itemIds != null && itemIds.Count > 0)
                {
                    filterStr = "";
                    foreach (var itemId in itemIds)
                    {
                        filterStr += string.Format("<Value Type = 'Counter'>{0}</Value>", itemId);
                    }
                    if (!string.IsNullOrEmpty(filterStr))
                    {
                        filterStr = string.Format("<In><FieldRef Name = 'ID'/><Values>{0}</Values></In>", filterStr);
                    }
                }
            }

            filterElement = XElement.Parse(filterStr);

            return filterElement;
        }
    }
}
