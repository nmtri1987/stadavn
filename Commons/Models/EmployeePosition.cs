using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/EmployeePosition")]
    public class EmployeePosition : EntityBase
    {
        [ListColumn("Code")]
        public string Code { get; set; }

        public string PositionName { get; set; }

        [ListColumn("CommonName")]
        public string Name { get; set; }

        [ListColumn("CommonName1066")]
        public string VietnameseName { get; set; }
    }
}
