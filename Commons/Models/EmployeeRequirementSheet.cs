using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.EmployeeRequirementSheetsList.Url)]
    public class EmployeeRequirementSheet : EntityBase
    {
        public EmployeeRequirementSheet()
        {
        }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.RecruitmentDepartment)]
        public LookupItem RecruitmentDepartment { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.DepartmentCode)]
        public string DepartmentCode { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.Position)]
        public string Position { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.Quantity)]
        public int Quantity { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.ReasonsForRecruitment)]
        public string ReasonsForRecruitment { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.Sex)]
        public string Sex { get; set; }

        //[ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.Age)]
        //public int Age { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.FromAge)]
        public int FromAge { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.ToAge)]
        public int ToAge { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.AvailableTime)]
        public DateTime AvailableTime { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.MaritalStatus)]
        public string MaritalStatus { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.WorkingTime)]
        public string WorkingTime { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.EducationLevel)]
        public string EducationLevel { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.Appearance)]
        public string Appearance { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.WorkingExperience)]
        public string WorkingExperience { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.Specialities)]
        public string Specialities { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.DescriptionOfBasicWork)]
        public string DescriptionOfBasicWork { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.MoralVocations)]
        public string MoralVocations { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.WorkingAbilities)]
        public string WorkingAbilities { get; set; }

        //[ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.EnglishSkills)]
        //public string EnglishSkills { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.ComputerSkills)]
        public string ComputerSkills { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.OtherSkills)]
        public string OtherSkills { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.OtherRequirement)]
        public string OtherRequirement { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.IsValidRequest)]
        public bool IsValidRequest { get; set; }

        [ListColumn(StringConstant.EmployeeRequirementSheetsList.Fields.IsTemplate)]
        public bool IsTemplate { get; set; }

        [ListColumn(ApprovalFields.Creator)]
        public LookupItem CommonCreator { get; set; }

        [ListColumn(ApprovalFields.Status)]
        public string ApprovalStatus { get; set; }

        [ListColumn(ApprovalFields.WFStatus)]
        public string WFStatus { get; set; }

        [ListColumn(ApprovalFields.PendingAt)]
        public List<LookupItem> PendingAt { get; set; }

        [ListColumn(ApprovalFields.CurrentStep)]
        public LookupItem CurrentStep { get; set; }

        [ListColumn(ApprovalFields.NextStep)]
        public LookupItem NextStep { get; set; }

        [ListColumn(ApprovalFields.CommonLocation)]
        public LookupItem CommonLocation { get; set; }

        [ListColumn(ApprovalFields.CommonDepartment)]
        public LookupItem CommonDepartment { get; set; }

        [ListColumn(ApprovalFields.IsAdditionalStep)]
        public bool IsAdditionalStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalPreviousStep)]
        public LookupItem AdditionalPreviousStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalStep)]
        public LookupItem AdditionalStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalNextStep)]
        public LookupItem AdditionalNextStep { get; set; }

        [ListColumn(ApprovalFields.AdditionalDepartment)]
        public LookupItem AdditionalDepartment { get; set; }

        [ListColumn(ApprovalFields.AssignFrom)]
        public LookupItem AssignFrom { get; set; }

        [ListColumn(ApprovalFields.AssignTo)]
        public LookupItem AssignTo { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedField)]
        public DateTime Modified { get; set; }

        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }
    }
}
