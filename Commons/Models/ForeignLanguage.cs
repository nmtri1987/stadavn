using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.ForeignLanguagesList.Url)]
    public class ForeignLanguage : EntityBase
    {
        public ForeignLanguage()
        {
        }

        [ListColumn(StringConstant.ForeignLanguagesList.Fields.Name)]
        public string Name { get; set; }

        [ListColumn(StringConstant.ForeignLanguagesList.Fields.VietnameseName)]
        public string VietnameseName { get; set; }
    }
}
