namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestBuyDetailInfo
    /// </summary>
    public class RequestBuyDetailInfo
    {
        #region Properties
        public string No { get; set; }

        public string Content { get; set; }

        public string Form { get; set; }

        public string Unit { get; set; }

        public string Quantity { get; set; }

        public string Reason { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// RequestBuyDetailInfo
        /// </summary>
        public RequestBuyDetailInfo()
        {
            this.No = string.Empty;
            this.Content = string.Empty;
            this.Form = string.Empty;
            this.Unit = string.Empty;
            this.Quantity = string.Empty;
            this.Reason = string.Empty;
        }

        /// <summary>
        /// RequestBuyDetailInfo
        /// </summary>
        /// <param name="requestBuyDetails"></param>
        public RequestBuyDetailInfo(Models.RequestBuyDetails requestBuyDetails)
        {
            this.No = string.Empty;
            this.Content = requestBuyDetails.Content;
            this.Form = requestBuyDetails.Form;
            this.Unit = requestBuyDetails.Unit;
            //this.Quantity = requestBuyDetails.Quantity.HasValue ? requestBuyDetails.Quantity.Value.ToString("N0") : string.Empty;
            this.Quantity = requestBuyDetails.Quantity.HasValue ? requestBuyDetails.Quantity.Value.ToString() : string.Empty;
            this.Reason = requestBuyDetails.Reason;
        }
        #endregion
    }
}
