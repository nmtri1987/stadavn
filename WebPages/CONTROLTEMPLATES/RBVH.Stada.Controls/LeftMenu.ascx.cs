using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.WebPages.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls
{
    public partial class LeftMenu : UserControl
    {
        private PermissionGroupDAL permissionGroupDAL;
        private const string STR_KEY_NAME = "Stada_LangSwitcher_Setting";
        private const string DEFAULT_LANG = "en-US";
        private const string SUB_SECTION_PARAM = "subSection";

        protected void Page_Load(object sender, EventArgs e)
        {
            SPWeb currentWeb = SPContext.Current.Web;
            if (currentWeb.Name.ToLower().Equals("policies") || currentWeb.ServerRelativeUrl.ToLower().Equals("/policies"))
            {
                return;
            }

            InitialData(SPContext.Current.Site.RootWeb);
        }

        private void InitialData(SPWeb spWeb)
        {
            var url = spWeb.Url;
            permissionGroupDAL = new PermissionGroupDAL(url);

            // Get groups of current user
            var groups = new List<string>();
            var items = spWeb.CurrentUser.Groups.GetEnumerator();
            while (items.MoveNext())
            {
                groups.Add(items.Current.ToString());
            }

            var permissionGroupList = permissionGroupDAL.GetPagesOnLeftMenu(groups).Where(x => x.PermissionModuleCategory != null); 
            if (permissionGroupList != null && permissionGroupList.Count() > 0)
            {
                var groupPermissionGroupList = permissionGroupList.OrderBy(x => x.LeftMenuOrder).GroupBy(x => x.PermissionModuleCategory.LookupValue);
                BindingMenuView(spWeb, groupPermissionGroupList);
            }
        }

        private void BindingMenuView(SPWeb spWeb, IEnumerable<IGrouping<string, PermissionGroup>> groupPermissionGroupList)
        {
            IList<RootMenu> RootMenuList = new List<RootMenu>();
            var currentLang = Context.Session != null ? Context.Session[STR_KEY_NAME].ToString() : "";
            var subSection = Request.Params[SUB_SECTION_PARAM];
            subSection = subSection != null ? string.Format("/{0}/", subSection) : null;
            var currentURL = Request.CurrentExecutionFilePath;
            
            string webUrl = spWeb.Url;
            foreach (var permissionGroup in groupPermissionGroupList)
            {
                var RootMenu = new RootMenu();
                RootMenu.Name = permissionGroup.Key;
                if (RootMenu.Name.ToLower() == "timesheet")
                {
                    RootMenu.Name = DEFAULT_LANG == currentLang ? RootMenu.Name : "Chấm công";
                }
                else if (RootMenu.Name.ToLower() == "management")
                {
                    RootMenu.Name = DEFAULT_LANG == currentLang ? RootMenu.Name : "Quản lý";
                }

                foreach (var page in permissionGroup.ToList<PermissionGroup>())
                {
                    var fulUrl = webUrl + page.PageName;
                    page.Name = DEFAULT_LANG == currentLang ? page.Name : page.VietNameseName;
                    page.VietNameseName = "";
                    RootMenu.PermissionGroups.Add(page);
                    if (fulUrl.Contains(currentURL) || (subSection != null && fulUrl.Contains(subSection)))
                    {
                        //set temp value of actived item to  VietNameseName
                        page.VietNameseName = "selected  ms-core-listMenu-selected";
                    }
                }
                RootMenuList.Add(RootMenu);

            }
            RootItem.DataSource = RootMenuList;
            RootItem.DataBind();
        }
    }
}
