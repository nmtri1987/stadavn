using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.Report.RecruitmentModule
{
    public class RecruitmentLanguageSkillInfo
    {
        public string ForeignLanguage { get; set; }

        public string Level { get; set; }


        public RecruitmentLanguageSkillInfo()
        {
            this.ForeignLanguage = string.Empty;
            this.Level = string.Empty;
        }

        /// <summary>
        /// RecruitmentLanguageSkillInfo
        /// </summary>
        /// <param name="recruitmentLanguageSkill"></param>
        /// <param name="foreignLanguageDAL"></param>
        public RecruitmentLanguageSkillInfo(RecruitmentLanguageSkill recruitmentLanguageSkill, ForeignLanguageDAL foreignLanguageDAL)
        {
            this.ForeignLanguage = string.Empty;

            if (recruitmentLanguageSkill.ForeignLanguage != null)
            {
                var foreignLanguage = foreignLanguageDAL.GetByID(recruitmentLanguageSkill.ForeignLanguage.LookupId);
                if (foreignLanguage != null)
                {
                    this.ForeignLanguage = string.Format("{0}/{1}", foreignLanguage.VietnameseName, foreignLanguage.Name);
                }
            }
            this.Level = recruitmentLanguageSkill.Level != null ? recruitmentLanguageSkill.Level : string.Empty;
        }
    }
}
