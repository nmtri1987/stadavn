using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// RequestBuyDetails
    /// </summary>
    [ListUrl(StringConstant.RequestBuyDetailsList.Url)]
    public class RequestBuyDetails : EntityBase
    {
        /// <summary>
        /// RequestBuyDetails
        /// </summary>
        public RequestBuyDetails() : base()
        {
            Request = new LookupItem();
        }

        [ListColumn(StringConstant.RequestBuyDetailsList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequestBuyDetailsList.Fields.Content)]
        public string Content { get; set; }

        [ListColumn(StringConstant.RequestBuyDetailsList.Fields.Form)]
        public string Form { get; set; }

        [ListColumn(StringConstant.RequestBuyDetailsList.Fields.Unit)]
        public string Unit { get; set; }

        [ListColumn(StringConstant.RequestBuyDetailsList.Fields.Quantity)]
        public double? Quantity { get; set; }

        [ListColumn(StringConstant.RequestBuyDetailsList.Fields.Reason)]
        public string Reason { get; set; }

        [ListColumn(StringConstant.RequestBuyDetailsList.Fields.Request)]
        public LookupItem Request { get; set; }
    }
}
