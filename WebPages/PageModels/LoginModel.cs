//-----------------------------------------------------------------------
// <copyright file="LoginModel.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RBVH.Stada.Intranet.WebPages.PageModels
{
    /// <summary>
    /// Login Model
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Gets or sets the Employee ID of the Login Model
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the Authorization Code of the Login Model
        /// </summary>
        public string AuthorizationCode { get; set; }
    }
}