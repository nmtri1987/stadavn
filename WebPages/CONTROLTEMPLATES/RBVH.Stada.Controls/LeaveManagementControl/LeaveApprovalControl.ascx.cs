using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using System.Xml.Linq;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.Constants;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveManagementControl
{
    public partial class LeaveApprovalControl : UserControl
    {
        private LeaveManagementDAL leaveManagementDAL;
        private const string baseViewID = "3";

        protected void Page_Load(object sender, EventArgs e)
        {
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            var siteUrl = SPContext.Current.Web.Url;
            leaveManagementDAL = new LeaveManagementDAL(siteUrl);
            var guidViews = leaveManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
            LeaveApprovalWebPart.ViewGuid = guidViews == null ? "" : guidViews.ID.ToString();

            XElement xmlViewDef = XElement.Parse(LeaveApprovalWebPart.XmlDefinition);
            XElement filterElement = BuildViewString(SPContext.Current.Web.CurrentUser.ID, siteUrl);

            XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
            if (whereElement != null)
            {
                if (whereElement.HasElements)
                {
                    whereElement.RemoveNodes();
                }
                whereElement.Add(filterElement);
                LeaveApprovalWebPart.XmlDefinition = xmlViewDef.ToString();
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
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.LeaveManagement.ToString(), assigneeId);
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
