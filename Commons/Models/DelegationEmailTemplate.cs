namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// Delegation Email Template
    /// </summary>
    public class DelegationEmailTemplate
    {
        #region Properties
        public string Receivers { get; set; }

        public string FromEmployee { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string AccessLink { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DelegationEmailTemplate()
        {
            this.Receivers = string.Empty;
            this.FromEmployee = string.Empty;
            this.FromDate = string.Empty;
            this.ToDate = string.Empty;
            this.AccessLink = string.Empty;
        }
        #endregion
    }
}
