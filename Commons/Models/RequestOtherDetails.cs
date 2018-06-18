using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// RequestOtherDetails class.
    /// </summary>
    [ListUrl(StringConstant.RequestOtherDetailsList.Url)]
    public class RequestOtherDetails : EntityBase
    {
        /// <summary>
        /// RequestOtherDetails
        /// </summary>
        public RequestOtherDetails() : base()
        {
            Request = new LookupItem();
        }

        [ListColumn(StringConstant.RequestOtherDetailsList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequestOtherDetailsList.Fields.Content)]
        public string Content { get; set; }

        [ListColumn(StringConstant.RequestOtherDetailsList.Fields.Unit)]
        public string Unit { get; set; }

        [ListColumn(StringConstant.RequestOtherDetailsList.Fields.Quantity)]
        public double? Quantity { get; set; }

        [ListColumn(StringConstant.RequestOtherDetailsList.Fields.Reason)]
        public string Reason { get; set; }

        [ListColumn(StringConstant.RequestOtherDetailsList.Fields.Request)]
        public LookupItem Request { get; set; }
    }
}
