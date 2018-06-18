using System;
using System.Web;

namespace RBVH.Core.SharePoint
{
    public static class HTTPUtility
    {
        public static string HtmlDecode(string html)
        {
            return Convert.ToString(HttpUtility.HtmlDecode(html));
        }
    }
}
