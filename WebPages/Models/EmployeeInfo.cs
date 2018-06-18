//-----------------------------------------------------------------------
// <copyright file="EmployeeInfo.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RBVH.Stada.Intranet.WebPages.Models
{
    using System;

    /// <summary>
    /// Employee Info Entity
    /// </summary>
    [Serializable]
    public class EmployeeInfo
    {
        /// <summary>
        /// Gets or sets the Employee ID of the Employee Info.
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the Lookup Id (ID) of the Employee Info.
        /// </summary>
        public int LookupId { get; set; }

        /// <summary>
        /// Gets or sets the Ad Account Lookup Value integer of the Employee Info.
        /// </summary>
        public int AdAccountLookupValue { get; set; }

        /// <summary>
        /// Gets or sets the Ad Account Lookup Value string of the Employee Info.
        /// </summary>
        public string AdAccountLookupValueString { get; set; }

        /// <summary>
        /// Gets or sets the FullName of the Employee Info.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Department Name of the Employee Info.
        /// </summary>
        public string Department { get; set; } 


        /// <summary>
        /// Gets or sets the Department Id of the Employee Info.
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the Location Name of the Employee Info.
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Gets or sets the Location Id of the Employee Info.
        /// </summary>
        public int FactoryId { get; set; }

        /// <summary>
        /// Gets or sets the Ad Account ID of the Employee Info.
        /// </summary>
        public int AdAccountID { get; set; }

        /// <summary>
        /// Gets or sets the Password of the Employee Info.
        /// </summary>
        public string Password { get; set; }
    }
}