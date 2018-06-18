//-----------------------------------------------------------------------
// <copyright file="Department.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the Department class.</summary>
//-----------------------------------------------------------------------
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// Department Model
    /// </summary>
    [ListUrl("/Lists/Departments")]
    public class Department : EntityBase
    {
        [ListColumn(StringConstant.DepartmentList.CodeField)]
        /// <summary>
        /// Gets or sets Department code, use in department site url
        /// </summary>
        public string Code { get; set; }

        [ListColumn(StringConstant.DepartmentList.NameField)]
        /// <summary>
        /// Gets or sets Department name (English) or default
        /// </summary>
        public string Name { get; set; }

        [ListColumn(StringConstant.DepartmentList.VietnameseNameField)]
        /// <summary>
        /// Gets or sets Department name Vietnamese
        /// </summary>
        public string VietnameseName { get; set; }

        [ListColumn(StringConstant.DepartmentList.SortOrderField)]
        /// <summary>
        /// Gets or sets Sort Order
        /// </summary>
        public int SortOrder { get; set; }

        [ListColumn(StringConstant.DepartmentList.AdministratorField)]
        public User Administrator { get; set; }

        [ListColumn(StringConstant.DepartmentList.BODField)]
        public User BOD { get; set; }

        [ListColumn(StringConstant.DepartmentList.IsBODApprovalRequiredField)]
        public bool IsBODApprovalRequired { get; set; }

        [ListColumn(StringConstant.DepartmentList.IsShiftRequestRequiredField)]
        public bool IsShiftRequestRequired { get; set; }

        [ListColumn(StringConstant.DepartmentList.DepartmentNoField)]
        public string DepartmentNo { get; set; }

        [ListColumn(StringConstant.DepartmentList.LocationsField)]
        public List<LookupItem> Locations { get; set; }

        [ListColumn(StringConstant.DepartmentList.AutoCreateOverTime)]
        public bool AutoCreateOverTime { get; set; }
    }
}