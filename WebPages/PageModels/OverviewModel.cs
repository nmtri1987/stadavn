//-----------------------------------------------------------------------
// <copyright file="OverviewModel.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.PageModels
{
    using Models;

    /// <summary>
    /// Overview Model store data
    /// </summary>
    public class OverviewModel
    {
        /// <summary>
        /// Gets or sets the Employee Info Entity of the Overview Model
        /// </summary>
        public EmployeeInfo Employee { get; set; }
    }
}