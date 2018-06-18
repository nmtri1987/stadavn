using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.StepManagementList.ListUrl)]
    public class StepManagement : EntityBase
    {
        [ListColumn(StringConstant.StepManagementList.StepModule)]
        public string StepModule { get; set; }
        [ListColumn(StringConstant.StepManagementList.StepNumber)]
        public int StepNumber { get; set; }
        [ListColumn(StringConstant.StepManagementList.StepStatus)]
        public string StepStatus { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem CommonDepartment { get; set; }
    }
}
