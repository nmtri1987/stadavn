using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.TaskManagementList.ListUrl)]
    public class TaskManagement :EntityBase
    {
        [ListColumn(StringConstant.TaskManagementList.PercentComplete)]
        public decimal PercentComplete { get; set; }
        [ListColumn(StringConstant.TaskManagementList.AssignedTo)]
        public User AssignedTo { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem Department { get; set; }
        [ListColumn(StringConstant.TaskManagementList.Description)]
        public string Description { get; set; }
        [ListColumn(StringConstant.TaskManagementList.DueDate)]
        public DateTime DueDate { get; set; }
        [ListColumn(StringConstant.TaskManagementList.ItemId)]
        public int ItemId { get; set; }
        [ListColumn(StringConstant.TaskManagementList.ItemURL)]
        public string ItemURL { get; set; }
        [ListColumn(StringConstant.TaskManagementList.ListURL)]
        public string ListURL { get; set; }
        [ListColumn(StringConstant.TaskManagementList.StartDate)]
        public DateTime StartDate { get; set; }
        [ListColumn(StringConstant.TaskManagementList.StepStatus)]
        public string StepStatus { get; set; }
        [ListColumn(StringConstant.TaskManagementList.TaskName)]
        public string TaskName { get; set; }
        [ListColumn(StringConstant.TaskManagementList.TaskOutcome)]
        public string TaskOutcome { get; set; }
        [ListColumn(StringConstant.TaskManagementList.TaskStatus)]
        public string TaskStatus { get; set; }
        [ListColumn(StringConstant.TaskManagementList.NextAssign)]
        public User NextAssign { get; set; }
        [ListColumn(StringConstant.TaskManagementList.StepModule)]
        public string StepModule { get; set; }
        [ListColumn(StringConstant.TaskManagementList.RelatedTasks)]
        public List<string> RelatedTasks { get; set; }
        [ListColumn(StringConstant.DefaultSPListField.ModifiedField)]
        public DateTime Modified { get; set; }
    }
}
