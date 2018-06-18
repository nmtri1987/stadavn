using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class FreightSecurityModel
    {
        public string Id { get; set; }
        public string ActionNo { get; set; }
        public string UpdateById { get; set; }
        public string UpdateByName { get; set; }
        public List<string> CheckInFreightIds { get; set; }
        public List<string> CheckOutFreightIds { get; set; }
        public string SecurityNotes { get; set; }
    }
}
