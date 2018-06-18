using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// DelegationEmployeePosition
    /// </summary>
    [ListUrl(StringConstant.DelegationEmployeePositionsList.Url)]
    public class DelegationEmployeePosition : EntityBase
    {
        public DelegationEmployeePosition()
        {
        }

        [ListColumn(StringConstant.DelegationEmployeePositionsList.Fields.EmployeePosition)]
        public LookupItem EmployeePosition { get; set; }

        [ListColumn(StringConstant.DelegationEmployeePositionsList.Fields.DelegatedEmployeePositions)]
        public List<LookupItem> DelegatedEmployeePositions { get; set; }
    }
}
