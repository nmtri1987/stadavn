
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class TaskManagementModel
    {
        public int Id { get; set; }
        public User AssignedTo { get; set; }
        public int ItemId { get; set; }
        public string TaskOutcome { get; set; }
        public string Description { get; set; }
        public string Modified { get; set; }

        public TaskManagementModel() { }

        public TaskManagementModel(TaskManagement taskManagement)
        {
            Id = taskManagement.ID;
            AssignedTo = taskManagement.AssignedTo;
            ItemId = taskManagement.ItemId;
            TaskOutcome = taskManagement.TaskOutcome;
            Description = taskManagement.Description;
            Modified = taskManagement.Modified.ToString(StringConstant.DateFormatddMMyyyyHHmmss);
        }
    }
}
