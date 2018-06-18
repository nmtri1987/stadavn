using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.ForeignLanguageLevelsList.Url)]
    public class ForeignLanguageLevel : EntityBase
    {
        public ForeignLanguageLevel()
        {
        }

        [ListColumn(StringConstant.ForeignLanguageLevelsList.Fields.Title)]
        public string Level { get; set; }
    }
}
