using System;

namespace RBVH.Stada.Intranet.Biz.Models
{
    public class Calendar : EntityBase
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
