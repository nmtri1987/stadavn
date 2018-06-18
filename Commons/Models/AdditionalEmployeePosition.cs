using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.AdditionalEmployeePositionList.Url)]
    public class AdditionalEmployeePosition : EntityBase
    {
        public AdditionalEmployeePosition()
        {
            Employee = new LookupItem();
            EmployeeID = new LookupItem();
        }

        [ListColumn(StringConstant.AdditionalEmployeePositionList.Fields.Employee)]
        public LookupItem Employee { get; set; }

        [ListColumn(StringConstant.AdditionalEmployeePositionList.Fields.EmployeeID)]
        public LookupItem EmployeeID { get; set; }

        [ListColumn(StringConstant.AdditionalEmployeePositionList.Fields.EmployeeLevel)]
        public int EmployeeLevel { get; set; }

        [ListColumn(StringConstant.AdditionalEmployeePositionList.Fields.Module)]
        public string Module { get; set; }
    }

}
