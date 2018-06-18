using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.RequestReceivedDepartmentViewersList.Url)]
    public class RequestReceivedDepartmentViewer : EntityBase
    {
        [ListColumn(StringConstant.RequestReceivedDepartmentViewersList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequestReceivedDepartmentViewersList.Fields.Location)]
        public LookupItem Location { get; set; }

        [ListColumn(StringConstant.RequestReceivedDepartmentViewersList.Fields.Department)]
        public LookupItem Department { get; set; }

        [ListColumn(StringConstant.RequestReceivedDepartmentViewersList.Fields.Employees)]
        public List<LookupItem> Employees { get; set; }
    }
}
