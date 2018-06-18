using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// DelegationModulesDAL
    /// </summary>
    public class DelegationModulesDAL : BaseDAL<DelegationModule>
    {
        /// <summary>
        /// DelegationModulesDAL
        /// </summary>
        public DelegationModulesDAL(string siteUrl) : base(siteUrl)
        {
        }

        /// <summary>
        /// GetByListUrl
        /// </summary>
        /// <param name="listUrl"></param>
        /// <returns></returns>
        public DelegationModule GetByListUrl(string listUrl)
        {
            DelegationModule delegationModule = null;

            string queryString = $@"<Where>
                                        <Eq>
                                            <FieldRef Name='{DelegationModulesList.Fields.ListUrl}' />
                                            <Value Type='Text'>{listUrl}</Value>
                                        </Eq>
                                    </Where>";
            SPQuery query = new SPQuery { Query = queryString, RowLimit = 1 };
            var delegationModules = GetByQuery(query);
            if (delegationModules != null && delegationModules.Count > 0)
            {
                delegationModule = delegationModules[0];
            }

            return delegationModule;
        }
    }
}
