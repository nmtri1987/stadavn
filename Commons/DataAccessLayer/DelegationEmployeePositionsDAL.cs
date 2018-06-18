using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// DelegationEmployeePositionsDAL
    /// </summary>
    public class DelegationEmployeePositionsDAL : BaseDAL<DelegationEmployeePosition>
    {
        /// <summary>
        /// DelegationEmployeePositionsDAL
        /// </summary>
        public DelegationEmployeePositionsDAL(string siteUrl) : base(siteUrl)
        {
        }

        /// <summary>
        /// GetByEmployeePosition
        /// </summary>
        /// <param name="employeePositionId"></param>
        /// <returns></returns>
        public List<DelegationEmployeePosition> GetByEmployeePosition(int employeePositionId)
        {
            List<DelegationEmployeePosition> delegationEmployeePositions = null;

            if (employeePositionId > 0)
            {
                string queryString = $@"<Where>  
                                        <Eq>
                                            <FieldRef Name='{StringConstant.DelegationEmployeePositionsList.Fields.EmployeePosition}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{employeePositionId}</Value>
                                        </Eq>
                                    </Where>";
                delegationEmployeePositions = GetByQuery(queryString);
            }

            return delegationEmployeePositions;
        }
    }
}
