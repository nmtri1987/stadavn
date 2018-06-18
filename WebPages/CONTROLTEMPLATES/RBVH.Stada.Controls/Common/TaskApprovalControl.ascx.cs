using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common
{
    public partial class TaskApprovalControl : UserControl
    {
        private TaskManagementDAL taskManagementDAL;
        private const string baseViewID = "10";
        public string BaseViewID
        {
            get; set;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //ListViewByQuery lv = new ListViewByQuery();
            //SPList list = SPContext.Current.Web.Lists["Task List"];
            //lv.List = list;
            //SPQuery query = new SPQuery(lv.List.DefaultView);
            //query.Query = string.Format(@"<where>{0}</where>", BuildQueryString());
            //lv.Query = query;
            
            ////this.Controls.Add(lv);
            //this.newTasks.Controls.Add(lv);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitialViewGUID();
        }

        private void InitialViewGUID()
        {
            try
            {
                var url = SPContext.Current.Web.Url;
                taskManagementDAL = new TaskManagementDAL(url);

                var guidViews = taskManagementDAL.GetViewGuildID().Where(x => x.BaseViewID == baseViewID).FirstOrDefault();
                TaskApprovalControlWP.ViewGuid = guidViews.ID.ToString();
                XElement xmlViewDef = XElement.Parse(TaskApprovalControlWP.XmlDefinition);
                XElement whereElement = xmlViewDef.Descendants("Where").FirstOrDefault();
                if (whereElement != null)
                {
                    if (whereElement.HasElements)
                    {
                        whereElement.RemoveNodes();
                    }
                   // whereElement.Add(BuildQueryString());
                    //TaskApprovalControlWP.XmlDefinition = xmlViewDef.ToString();
                    //SPContext.Current.Web.AllowUnsafeUpdates = true;
                    var module = Page.Request["AdminModule"];
                    if (module != null)
                    {
                        TaskApprovalControlWP.View.Query = string.Format(@"<Where>
                                            <And>   
                                                <In>
                                                    <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                                    <Values>
                                                        <Value Type='Integer'>
                                                            <UserID />
                                                        </Value>
                                                    </Values>
                                                </In> 
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='StepModule' />
                                                        <Value Type='Choice'>{0}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='Status' />
                                                        <Value Type='Choice'>In Progress</Value>
                                                    </Eq>
                                                </And>
                                            </And>
                                        </Where>", module);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Task Approval Control - InitData", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        private XElement BuildQueryString()
        {
            XElement filterElement = null;
            filterElement = XElement.Parse(@"<And>   
                                                <Eq>
                                                    <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                                    <Value Type='Integer'><UserID /></Value>
                                                 </Eq>
                                                <And>
                                                    <Eq>
                                                        <FieldRef Name='StepModule' />
                                                        <Value Type='Choice'>{Module}</Value>
                                                    </Eq>
                                                    <Eq>
                                                        <FieldRef Name='Status' />
                                                        <Value Type='Choice'>In Progress</Value>
                                                    </Eq>
                                                </And>
                                            </And>");
            return filterElement;
        }
    }
}
