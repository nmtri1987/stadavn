using System.Runtime.Serialization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class EmployeeInfoModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        
        public string ManagerName { get; set; }
    }
}
