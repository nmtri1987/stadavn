using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.RequestManagement
{
    /// <summary>
    /// RequestForm class.
    /// </summary>
    public partial class RequestForm : ApprovalPageBase
    {
        #region Constructors
        public RequestForm() : base()
        {
            //this.moduleName = ConstantStrings.ModuleName;
            this.moduleName = string.Empty;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
