using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestRepairDetailInfo
    /// </summary>
    public class RequestRepairDetailInfo
    {
        #region Properties
        public string No { get; set; }

        public string Content { get; set; }

        public string Reason { get; set; }

        public string Place { get; set; }

        public string From { get; set; }

        public string To { get; set; }
        #endregion

        #region Constructors

        public RequestRepairDetailInfo()
        {
            this.No = string.Empty;
            this.Content = string.Empty;
            this.Reason = string.Empty;
            this.Place = string.Empty;
            this.From = string.Empty;
            this.To = string.Empty;
        }

        /// <summary>
        /// RequestRepairDetailInfo
        /// </summary>
        /// <param name="requestRepairDetails">The RequestRepairDetails object.</param>
        public RequestRepairDetailInfo(Models.RequestRepairDetails requestRepairDetails)
        {
            this.No = string.Empty;
            this.Content = requestRepairDetails.Content;
            this.Reason = requestRepairDetails.Reason;
            this.Place = requestRepairDetails.Place;
            if (requestRepairDetails.From != null && requestRepairDetails.From.HasValue)
            {
                this.From = requestRepairDetails.From.Value.ToString(StringConstant.DateFormatMMddyyyySlash);
            }
            else
            {
                this.From = string.Empty;
            }
            if (requestRepairDetails.To != null && requestRepairDetails.To.HasValue)
            {
                this.To = requestRepairDetails.To.Value.ToString(StringConstant.DateFormatMMddyyyySlash);
            }
            else
            {
                this.To = string.Empty;
            }
        }

        #endregion
    }
}
