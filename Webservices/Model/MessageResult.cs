using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class MessageResult
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public int ObjectId { get; set; }

        public MessageResult()
        {
            Code = ObjectId = 0;
            Message = string.Empty;
        }
    }
}
