namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestOtherDetailInfo
    /// </summary>
    public class RequestOtherDetailInfo
    {
        #region Properties
        public string No { get; set; }

        public string Content { get; set; }

        public string Unit { get; set; }

        public string Quantity { get; set; }

        public string Reason { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// RequestOtherDetailInfo
        /// </summary>
        public RequestOtherDetailInfo()
        {
            this.No = string.Empty;
            this.Content = string.Empty;
            this.Unit = string.Empty;
            this.Quantity = string.Empty;
            this.Reason = string.Empty;
        }

        /// <summary>
        /// RequestOtherDetailInfo
        /// </summary>
        /// <param name="requestOtherDetails"></param>
        public RequestOtherDetailInfo(Models.RequestOtherDetails requestOtherDetails)
        {
            this.No = string.Empty;
            this.Content = requestOtherDetails.Content;
            this.Unit = requestOtherDetails.Unit;
            //this.Quantity = requestOtherDetails.Quantity.HasValue ? requestOtherDetails.Quantity.Value.ToString("N0") : string.Empty;
            this.Quantity = requestOtherDetails.Quantity.HasValue ? requestOtherDetails.Quantity.Value.ToString() : string.Empty;
            this.Reason = requestOtherDetails.Reason;
        }
        #endregion
    }
}
