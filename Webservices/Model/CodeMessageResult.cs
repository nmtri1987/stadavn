using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class CodeMessageResult
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public CodeMessageResult()
        {
            Code = 0;
            Message = string.Empty;
        }
    }
}
