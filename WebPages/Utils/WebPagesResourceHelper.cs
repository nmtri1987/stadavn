using System.Globalization;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.WebPages.Utils
{
    public static class WebPageResourceHelper
    {
        public static string GetResourceString(string resourceKey)
        {
            return ResourceHelper.GetLocalizedString(resourceKey, StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
        }
    }
}