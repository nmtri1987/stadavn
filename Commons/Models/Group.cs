using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/Groups")]
    public class Group : EntityBase
    {
        [ListColumn(StringConstant.GroupList.NameField)]
        public string Name { get; set; }
    }
}
