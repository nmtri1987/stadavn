using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class RootMenu
    {
        public RootMenu()
        {
            PermissionGroups = new List<PermissionGroup>();
        }
        public string Name { get; set; }
        public IList<PermissionGroup> PermissionGroups { get; set; }
    }
}
