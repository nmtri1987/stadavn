using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.RequestTypesList.ListUrl)]
    public class RequestType : EntityBase
    {
        [ListColumn(StringConstant.RequestTypesList.RequestTypeNameField)]
        public string RequestTypeName { get; set; }

        [ListColumn(StringConstant.RequestTypesList.RequestsTypeField)]
        public string RequestsType { get; set; }

        [ListColumn(StringConstant.RequestTypesList.DepartmentsField)]
        public List<LookupItem> Departments { get; set; }
    }
}
