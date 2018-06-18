using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.RequestForDiplomaSuppliesList.Url)]
    public class RequestForDiplomaSupply : EntityBase
    {
        public RequestForDiplomaSupply()
        {
        }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.Employee)]
        public LookupItem Employee { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.EmployeeName)]
        public string EmployeeName { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.EmployeeCode)]
        public string EmployeeCode { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.Position)]
        public string Position { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.DateOfEmp)]
        public DateTime DateOfEmp { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.ToTheDailyWorks)]
        public string ToTheDailyWorks { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.NewSuggestions)]
        public string NewSuggestions { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.DiplomaRevision)]
        public bool DiplomaRevision { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.SalaryRevision)]
        public bool SalaryRevision { get; set; }

        [ListColumn(StringConstant.RequestForDiplomaSuppliesList.Fields.CommonFrom)]
        public DateTime CommonFrom { get; set; }

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
    }
}
