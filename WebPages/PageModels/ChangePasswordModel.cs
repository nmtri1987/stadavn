//-----------------------------------------------------------------------
// <copyright file="ChangePasswordModel.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RBVH.Stada.Intranet.WebPages.PageModels
{
    /// <summary>
    /// Change Password Model
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Gets or sets the Employee ID of the Change Password Model
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the Employee of the Change Password Model
        /// </summary>
        public string Employee { get; set; }

        /// <summary>
        /// Gets or sets the Current Password of the Change Password Model
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Gets or sets the New Password of the Change Password Model
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the Confirm Password of the Change Password Model
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}