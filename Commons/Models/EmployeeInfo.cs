//-----------------------------------------------------------------------
// <copyright file="EmployeeInfo.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the EmployeeInfo class.</summary>
//-----------------------------------------------------------------------
namespace RBVH.Stada.Intranet.Biz.Models
{
    using Constants;
    using Helpers;
    using System;
    using System.Collections.Generic;
    using static Constants.StringConstant;

    /// <summary>
    /// Employee Info Entity
    /// </summary>
    [Serializable]
    public class EmployeeInfo : EntityBase
    {
        /// <summary>
        /// Gets or sets the Employee ID of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.EmployeeIDField)]
        public string EmployeeID { get; set; }

        [ListColumn(EmployeeInfoList.ADAccountField)]
        public User ADAccount { get; set; }

        /// <summary>
        /// Gets or sets the FullName of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.FullNameField)]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentPermission of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.DepartmentPermissionField)]
        public string DepartmentPermission { get; set; }
        
        /// <summary>
        /// Gets or sets the Department of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.DepartmentField)]
        public LookupItem Department { get; set; }

        /// <summary>
        /// Gets or sets the Factory of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.FactoryLocationField)]
        public LookupItem FactoryLocation { get; set; }

        /// <summary>
        /// Gets or sets the EmployeePosition of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.EmployeePositionField)]
        public LookupItem EmployeePosition { get; set; }

        [ListColumn(EmployeeInfoList.EmployeeInfoManagerField)]
        public LookupItem Manager { get; set; }
        
        /// <summary>
        /// Gets or sets the Password of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.PasswordField)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the EmployeeType of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.EmployeeTypeField)]
        public string EmployeeType { get; set; }

        /// <summary>
        /// Gets or sets the IsActive of the Employee Info.
        /// </summary>
        [ListColumn(EmployeeInfoList.IsActiveField)]
        public bool IsActive { get; set; }

        [ListColumn(EmployeeInfoList.EmailField)]
        public string Email { get; set; }

        [ListColumn(EmployeeInfoList.PublishingPageImageField)]
        public string Image { get; set; }

        [ListColumn(EmployeeInfoList.EmployeeLevelField)]
        public LookupItem EmployeeLevel { get; set; }

        [ListColumn(EmployeeInfoList.JoinedDateField)]
        public DateTime JoinedDate { get; set; }

        [ListColumn(EmployeeInfoList.DelegatedByField)]
        public List<LookupItem> DelegatedBy { get; set; }
    }
}