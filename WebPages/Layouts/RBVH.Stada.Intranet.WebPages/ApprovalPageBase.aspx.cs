using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.WebPages.Common;
using RBVH.Core.SharePoint;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    /// <summary>
    /// ApprovalPageBase class.
    /// </summary>
    public partial class ApprovalPageBase : PageEventHandlingBase
    {
        #region Attributes
        protected string moduleName;
        protected string formTitle;
        #endregion

        #region Properties
        public string ModuleName
        {
            get
            {
                return moduleName;
            }

            set
            {
                moduleName = value;
            }
        }

        public string FormTitle
        {
            get
            {
                return formTitle;
            }
            set
            {
                this.formTitle = value;
            }
        }
        #endregion

        #region Constructors
        public ApprovalPageBase() : base()
        {
            this.moduleName = string.Empty;
            this.formTitle = string.Empty;
        }
        #endregion

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);
                var approvalBaseUserControls = this.GetAllControlsOfType<ApprovalBaseUserControl>().ToList();
                if (approvalBaseUserControls != null && approvalBaseUserControls.Count>0)
                {
                    this.formTitle = approvalBaseUserControls[0].FormTitle;
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
    }
}
