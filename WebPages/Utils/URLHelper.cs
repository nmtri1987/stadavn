using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RBVH.Stada.Intranet.WebPages.Utils
{
    static class URLHelper
    {
        public static Uri AddParameter(this Uri url, Dictionary<string, string> paramCollection)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in paramCollection)
            {
                if (query.AllKeys.Contains(param.Key) == false)
                {
                    query[param.Key] = param.Value;
                }
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        public static void RedirectPage(Uri oldUri, Uri newUri)
        {
            if (HttpUtility.UrlDecode(oldUri.Query.ToLower()) != HttpUtility.UrlDecode(newUri.Query.ToLower()))
            {
                SPUtility.Redirect(newUri.ToString(), SPRedirectFlags.Static, HttpContext.Current);
            }
        }

        public static DateTime GetDefaultDateTimeOfShift()
        {
            DateTime defaultDateTimeOfShift = DateTime.Now;
            if (defaultDateTimeOfShift.Day > 20)
            {
                defaultDateTimeOfShift = defaultDateTimeOfShift.AddMonths(1);
            }
            return defaultDateTimeOfShift;
        }
    }
}
