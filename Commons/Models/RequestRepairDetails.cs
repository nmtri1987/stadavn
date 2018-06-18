using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// RequestRepairDetails class.
    /// </summary>
    [ListUrl(StringConstant.RequestRepairDetailsList.Url)]
    public class RequestRepairDetails : EntityBase
    {
        /// <summary>
        /// RequestRepairDetails
        /// </summary>
        public RequestRepairDetails() : base()
        {
            Request = new LookupItem();
        }

        [ListColumn(StringConstant.RequestRepairDetailsList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequestRepairDetailsList.Fields.Content)]
        public string Content { get; set; }

        [ListColumn(StringConstant.RequestRepairDetailsList.Fields.Reason)]
        public string Reason { get; set; }

        [ListColumn(StringConstant.RequestRepairDetailsList.Fields.Place)]
        public string Place { get; set; }

        [ListColumn(StringConstant.RequestRepairDetailsList.Fields.From)]
        public DateTime? From { get; set; }

        [ListColumn(StringConstant.RequestRepairDetailsList.Fields.To)]
        public DateTime? To { get; set; }

        [ListColumn(StringConstant.RequestRepairDetailsList.Fields.Request)]
        public LookupItem Request { get; set; }

        public string FromDateString { get; set; }

        public string ToDateString { get; set; }
    }
}
