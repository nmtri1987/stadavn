using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// DelegationModule
    /// </summary>
    [ListUrl(StringConstant.DelegationModulesList.Url)]
    public class DelegationModule : EntityBase
    {
        public DelegationModule()
        {
        }

        [ListColumn(StringConstant.DelegationModulesList.Fields.ModuleName)]
        public string ModuleName { get; set; }
        [ListColumn(StringConstant.DelegationModulesList.Fields.VietnameseModuleName)]
        public string VietnameseModuleName { get; set; }
        [ListColumn(StringConstant.DelegationModulesList.Fields.ListUrl)]
        public string ListUrl { get; set; }
    }
}
