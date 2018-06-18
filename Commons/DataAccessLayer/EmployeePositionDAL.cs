using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeePositionDAL : BaseDAL<EmployeePosition>
    {
        public EmployeePositionDAL(string siteUrl) : base(siteUrl)
        {
        }
        
        public List<EmployeePosition> GetByMinLevel(double minLevel)
        {
            List<EmployeePosition> employeePositions = new List<EmployeePosition>();
            var levelString = minLevel.ToString("#,##0.0000");
            //// Query SPList
            employeePositions =
                GetByQuery($@"<Where>
                              <Geq>
                                 <FieldRef Name='EmployeeLevel' />
                                 <Value Type='Number'>{levelString}</Value>
                              </Geq>
                           </Where>");
            return employeePositions;
        }
    }
}
