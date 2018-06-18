using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.RecruitmentLanguageSkillsList.Url)]
    public class RecruitmentLanguageSkill : EntityBase
    {
        public RecruitmentLanguageSkill()
        {
        }

        [ListColumn(StringConstant.RecruitmentLanguageSkillsList.Fields.ForeignLanguage)]
        public LookupItem ForeignLanguage { get; set; }

        [ListColumn(StringConstant.RecruitmentLanguageSkillsList.Fields.Level)]
        public string Level { get; set; }

        [ListColumn(StringConstant.RecruitmentLanguageSkillsList.Fields.Request)]
        public LookupItem Request { get; set; }
    }
}
