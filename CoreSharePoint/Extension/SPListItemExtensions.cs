using Microsoft.SharePoint;

namespace RBVH.Core.SharePoint.Extension
{
    /// <summary>
    /// SPListItem extensions.
    /// </summary>
    public static class SPListItemExtensions
    {
 
        public static string GetDisplayFormUrl(this SPListItem item)
        {
            return string.Format("{0}/{1}?ID={2}", item.ParentList.ParentWeb.Url, item.ParentList.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url, item.ID);
        }

        public static string GetEditFormUrl(this SPListItem item)
        {
            return string.Format("{0}/{1}?ID={2}", item.ParentList.ParentWeb.Url, item.ParentList.Forms[PAGETYPE.PAGE_EDITFORM].Url, item.ID);
        }
    }
}
