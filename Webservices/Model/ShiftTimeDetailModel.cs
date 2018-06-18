using System.Runtime.Serialization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    
    public class ShiftTimeDetailModel
    {
        
        public string Value { get; set; }
        
        public string Code { get; set; }
        
        public bool Approved { get; set; }
        
        public int Day { get; set; }

        public ShiftTimeDetailModel()
        {
            Value = string.Empty;
        }
    }
}
