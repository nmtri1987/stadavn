using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Web;
using System.Web.UI;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls
{
    public partial class CheckPermission : UserControl
    {
        private  PermissionGroupDAL PermissionGroupDAL;
      
        protected void Page_Load(object sender, EventArgs e)
        {
            SPWeb currentWeb = SPContext.Current.Web;
            if (currentWeb.Name.ToLower().Equals("policies") || currentWeb.ServerRelativeUrl.ToLower().Equals("/policies"))
            {
                return;
            }

            Page.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            SPUser currentUser = currentWeb.CurrentUser;

            var currentPage = Request.CurrentExecutionFilePath;
            PermissionGroupDAL = new PermissionGroupDAL(SPContext.Current.Site.RootWeb.Url);
            var groups = new List<string>();

            var items = currentWeb.CurrentUser.Groups.GetEnumerator();
            while (items.MoveNext())
            {
                groups.Add(items.Current.ToString());
            }

            var hasPermission = PermissionGroupDAL.IsAuthorizedOnPage(SPContext.Current.Site.RootWeb, currentPage, groups);
            if (!hasPermission)
            {
                var ex = new SecurityException();
                SPUtility.HandleAccessDenied(ex);
            }

            if (currentUser.IsSiteAdmin == false && UserPermission.IsCurrentUserInGroup(StringConstant.Group.CommonAccountGroupName) && HttpContext.Current.Session[StringConstant.EmployeeLogedin] == null)
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                if (url.Contains(StringConstant.PageLoginURL) || url.Contains(StringConstant.PageHomeURL))
                    return;
                Response.Redirect(StringConstant.PageLoginURL);
            }
        }
    }
}
