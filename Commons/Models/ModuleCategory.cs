using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/ModuleCategory")]
    public class ModuleCategory : EntityBase
    {
        [ListColumn(StringConstant.ModuleCategoryList.NameField)]
        public string Name { get; set; }
        [ListColumn(StringConstant.ModuleCategoryList.VietNameseNameField)]
        public string VietNameseName { get; set; }
    }
}
