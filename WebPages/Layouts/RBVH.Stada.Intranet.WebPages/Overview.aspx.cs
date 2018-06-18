//-----------------------------------------------------------------------
// <copyright file="Overview.aspx.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    using System;
    using Microsoft.SharePoint;
    using Utils;
    using PageModels;
    using Biz.Constants;

    /// <summary>
    /// Overview Page show all tasks of the user
    /// </summary>
    public partial class Overview : LayoutsPageBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Overview class.
        /// </summary>
        public Overview()
        {
            LogCategory = StringConstant.EmployeeInfoLogCategory;
        }

        public string LogCategory { get; set; }

        #endregion

        #region Load Page - First load

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">A objects sender </param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #endregion

        #region Utilities

        #endregion
    }
}