using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.RecruitmentTeamList.Url)]
    public class RecruitmentTeam : EntityBase
    {
        public RecruitmentTeam()
        {
        }

        [ListColumn(StringConstant.RecruitmentTeamList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RecruitmentTeamList.Fields.Employees)]
        public List<LookupItem> Employees { get; set; }
    }
}
