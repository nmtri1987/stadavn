using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Text;
using System.Web.UI;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common
{
    /// <summary>
    /// SupportingDocumentControl class.
    /// </summary>
    public partial class SupportingDocumentControl : UserControl
    {
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if(!Page.IsPostBack)
                {
                    var approvalBaseUserControl = this.Parent as ApprovalBaseUserControl;
                    if (approvalBaseUserControl != null)
                    {
                        if (approvalBaseUserControl.CurrentItem != null)
                        {
                            int listItemID = approvalBaseUserControl.CurrentItem.ID;
                            if (listItemID > 0)
                            {
                                string folderName = approvalBaseUserControl.ListName;
                                //string listName = SupportingDocumentsList.ListName;
                                string folderUrl = string.Format("{0}{1}/{2}", approvalBaseUserControl.CurrentWeb.Url, SupportingDocumentsList.Url, folderName);
                                SPFolder folder = approvalBaseUserControl.CurrentWeb.GetFolder(folderUrl);
                                if (folder.ItemCount > 0)
                                {
                                    //SPList list = approvalBaseUserControl.CurrentWeb.Lists.TryGetList(SupportingDocumentsList.ListName);
                                    SPList list = approvalBaseUserControl.CurrentWeb.GetList(string.Format("{0}{1}",  approvalBaseUserControl.CurrentWeb.Url, SupportingDocumentsList.Url));
                                    SPQuery query = new SPQuery();
                                    query.Query = string.Format(@"<Where>
                                                                <Eq>
                                                                    <FieldRef Name='{0}'  />
                                                                    <Value Type='Text'>{1}</Value>
                                                                 </Eq>
                                                           </Where>", SupportingDocumentsList.Fields.ListItemID, listItemID);
                                    query.Folder = folder;
                                    SPListItemCollection items = list.GetItems(query);
                                    if (items != null && items.Count > 0)
                                    {
                                        string webUrl = approvalBaseUserControl.CurrentWeb.Url;
                                        StringBuilder gridSupportingDocumentBuilder = new StringBuilder();

                                        foreach (SPListItem item in items)
                                        {
                                            string linkToItem = BuildLinkToItem(webUrl, item);
                                            gridSupportingDocumentBuilder.Append(linkToItem);
                                        }

                                        this.GridSupportingDocument.InnerHtml = gridSupportingDocumentBuilder.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        /// <summary>
        /// OnPreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);

                var approvalBaseUserControl = this.Parent as ApprovalBaseUserControl;
                if (approvalBaseUserControl != null)
                {
                    if (approvalBaseUserControl.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.New)
                    {
                        this.AddMoreFile.Visible = true;
                    }
                    else if( approvalBaseUserControl.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit && approvalBaseUserControl.IsCreator() && approvalBaseUserControl.IsRejectedStatus())
                    {
                        this.AddMoreFile.Visible = true;
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        /// <summary>
        /// BuildLinkToItem
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        private string BuildLinkToItem(string webUrl, SPListItem listItem)
        {
            string linkToItem = string.Empty;

            try
            {
                //string urlOfFile = string.Format("{0}/{1}", webUrl, listItem.Url);
                string fileName = listItem.Name;
                string encodedFileName = Uri.EscapeDataString(fileName);
                string encodedUrlOfFile = listItem.Url.Replace(fileName, encodedFileName);
                string urlOfFile = string.Format("{0}/{1}", webUrl, encodedUrlOfFile);
                linkToItem = string.Format("<a href='{0}' target='_blank'>{1}</a>&nbsp;&nbsp;&nbsp;</br/>", urlOfFile, listItem.Name);
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
            
            return linkToItem;
        }
    }
}
