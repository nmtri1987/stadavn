//-----------------------------------------------------------------------
// <copyright file="EmployeeLoginInfo.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the EmployeeLoginInfo class.</summary>
//-----------------------------------------------------------------------
namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// Employee login information
    /// </summary>
    public class EmployeeLoginInfo
    {
        /// <summary>
        /// Gets or sets Employee Id
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAdmin
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}