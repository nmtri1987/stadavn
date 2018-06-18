using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class PeoplePickerDataRequest
    {
        public string Name { get; set; }
        public List<EmployeePosition> EmployeePositions { get; set; }
    }
}
