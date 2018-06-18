using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Interfaces
{
    public interface IIDGenerator
    {
        string GetNewID(int departmentId);
        string GetNewID(int departmentId, DateTime date);
    }
}
