using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class LeaveException : Exception
    {
        public int ErrorCode { get; protected set; }

        public LeaveException(string message) : base(message) { }

        public LeaveException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
