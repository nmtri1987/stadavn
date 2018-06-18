//-----------------------------------------------------------------------
// <copyright file="ResetPasswordModel.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.PageModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Reset Password Model
    /// </summary>
    public class ResetPasswordModel
    {
        /// <summary>
        /// Gets or sets the Employees Information of the Reset Password Model
        /// </summary>
        public List<EmployeeInfo> EmployeeInfos { get; set; }

        /// <summary>
        /// Gets or sets the Employee ID of the Reset Password Model
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the New Password of the Reset Password Model
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the Confirm Password of the Reset Password Model
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}