using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class ForeignLanguageDAL : BaseDAL<ForeignLanguage>
    {
        public ForeignLanguageDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
