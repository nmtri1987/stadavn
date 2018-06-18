using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/PermissionGroup")]
    public class PermissionGroup : EntityBase
    {
        [ListColumn(StringConstant.PermissionGroupList.NameField)]
        public string Name { get; set; }
        [ListColumn(StringConstant.PermissionGroupList.VietNameseNameField)]
        public string VietNameseName { get; set; }
        [ListColumn(StringConstant.PermissionGroupList.IsOnLeftMenuField)]
        public bool IsOnLeftMenu { get; set; }
        [ListColumn(StringConstant.PermissionGroupList.PermissionModuleCategoryField)]
        public LookupItem PermissionModuleCategory { get; set; }

        [ListColumn(StringConstant.PermissionGroupList.PermissionModuleCategoryVNField)]
        public LookupItem PermissionModuleCategoryVN { get; set; }

        [ListColumn(StringConstant.PermissionGroupList.PageNameFiled)]
        public string PageName { get; set; }

        [ListColumn(StringConstant.PermissionGroupList.LeftMenuOrderFiled)]
        public double LeftMenuOrder { get; set; }
    }
}
