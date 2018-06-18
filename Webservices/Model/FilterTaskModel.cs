using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Globalization;

namespace RBVH.Stada.Intranet.Webservices.Model
{
   public class FilterTaskModel
    {
        public int ItemId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int RequesterId { get; set; }
        public string RequesterName { get; set; }
        public string ItemApprovalUrl { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public string ApprovalStatus { get; set; }
        public int ApprovalStatusId { get; set; }
        public string CreatedDate { get; set; }
        public string DueDate { get; set; }

        public static FilterTaskModel FromDTO(FilterTask item)
        {
            return new FilterTaskModel
            {
                ItemId = item.ItemId,
                ModuleId = item.ModuleId,
                ModuleName = item.ModuleName,
                RequesterId = item.Requester.LookupId,
                RequesterName = item.Requester.LookupValue,
                ItemApprovalUrl = item.ItemApprovalUrl,
                DepartmentId = item.Department.LookupId,
                DepartmentName = item.Department.LookupValue,
                Description = item.Description,
                ApprovalStatus = item.ApprovalStatus,
                ApprovalStatusId = item.ApprovalStatusId,
                CreatedDate = item.CreatedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                DueDate = item.DueDate == DateTime.MinValue ? string.Empty : item.DueDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
            };
        }
    }
}
