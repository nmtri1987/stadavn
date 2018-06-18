//-----------------------------------------------------------------------
// <copyright file="LogOut.aspx.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using RBVH.Core.SharePoint;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    using System;
    using Microsoft.SharePoint.WebControls;
    using Utils;
    using Biz.Constants;

    /// <summary>
    /// Log out form action
    /// </summary>
    public partial class LogOut : LayoutsPageBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the LogOut class.
        /// </summary>
        public LogOut()
        {
            LogCategory = StringConstant.EmployeeInfoLogCategory;
        }

        public string LogCategory { get; set; }

        #endregion

        #region Page load - First load

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">A objects sender </param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.OnLogout();
            }
            catch (Exception ex)
            {
                // Write log
                ULSLogging.LogError(ex);
            }
        }

        /// <summary>
        /// Clear all session and redirect home page
        /// </summary>
        protected void OnLogout()
        {
            UserPermission.SetEmployeeInfo(null);
            Response.Redirect(StringConstant.PageLoginURL);
        }

        #endregion

        #region Actions

        #endregion
    }
}