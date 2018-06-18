using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class PeoplePickerUser
    {
        //[DataMember]
        //internal int LookupId;

        public string Login { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public string ID { get; set; }
    }
}
