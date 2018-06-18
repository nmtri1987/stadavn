using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class TaskManagementDAL : BaseDAL<TaskManagement>
    {
        public TaskManagementDAL(string siteUrl) : base(siteUrl) { }

        public void CloseTasks(List<string> taskIds)
        {
            if (taskIds != null && taskIds.Count() > 0)
            {
                List<TaskManagement> taskManagements = new List<TaskManagement>();
                foreach (var item in taskIds)
                {
                    int taskId;
                    if (int.TryParse(item, out taskId))
                    {
                        var task = GetByID(taskId);
                        task.TaskStatus = TaskStatusList.Deferred;
                        task.PercentComplete = 0;
                        taskManagements.Add(task);
                    }
                }
                SaveItems(taskManagements);
            }
        }

        public void CloseTasks(List<TaskManagement> taskManagementCollection)
        {
            if (taskManagementCollection != null && taskManagementCollection.Count() > 0)
            {
                foreach (var taskManagement in taskManagementCollection)
                {
                    taskManagement.TaskStatus = TaskStatusList.Deferred;
                    taskManagement.PercentComplete = 0;
                }
                SaveItems(taskManagementCollection);
            }
        }

        public void RejectTasks(List<string> taskIds)
        {
            if (taskIds != null && taskIds.Count() > 0)
            {
                List<TaskManagement> taskManagements = new List<TaskManagement>();
                foreach (var item in taskIds)
                {
                    int taskId;
                    if (int.TryParse(item, out taskId))
                    {
                        var task = GetByID(taskId);
                        task.TaskStatus = TaskStatusList.Completed;
                        task.PercentComplete = 1;
                        task.TaskOutcome = TaskOutcome.Rejected.ToString();
                        taskManagements.Add(task);
                    }
                }
                SaveItems(taskManagements);
            }
        }

        public void RejectTasks(List<TaskManagement> taskManagementCollection)
        {
            if (taskManagementCollection != null && taskManagementCollection.Count() > 0)
            {
                foreach (var taskManagement in taskManagementCollection)
                {
                    taskManagement.TaskStatus = TaskStatusList.Completed;
                    taskManagement.PercentComplete = 1;
                    taskManagement.TaskOutcome = TaskOutcome.Rejected.ToString();
                }
                SaveItems(taskManagementCollection);
            }
        }

        public void ApproveTasks(List<string> taskIds)
        {
            if (taskIds != null && taskIds.Count() > 0)
            {
                List<TaskManagement> taskManagements = new List<TaskManagement>();
                foreach (var item in taskIds)
                {
                    int taskId;
                    if (int.TryParse(item, out taskId))
                    {
                        var task = GetByID(taskId);
                        task.TaskStatus = TaskStatusList.Completed;
                        task.PercentComplete = 1;
                        task.TaskOutcome = TaskOutcome.Approved.ToString();
                        taskManagements.Add(task);
                    }
                }
                SaveItems(taskManagements);
            }
        }

        public void ApproveTasks(List<TaskManagement> taskManagementCollection)
        {
            if (taskManagementCollection != null && taskManagementCollection.Count() > 0)
            {
                foreach (var taskManagement in taskManagementCollection)
                {
                    taskManagement.TaskStatus = TaskStatusList.Completed;
                    taskManagement.PercentComplete = 1;
                    taskManagement.TaskOutcome = TaskOutcome.Approved.ToString();
                }
                SaveItems(taskManagementCollection);
            }
        }

        public TaskManagement GetTaskByAssigneeId(IList<TaskManagement> taskCollection, int userId)
        {
            TaskManagement ret = new TaskManagement();

            if (taskCollection != null && taskCollection.Count > 0)
            {
                ret = taskCollection.Where(e => e.AssignedTo.ID == userId).FirstOrDefault();
            }

            return ret;
        }

        public IList<TaskManagement> GetByItemID(int itemId, string stepModule)
        {
            var queryStr = $@"<Where>
                            <And>
                                <Eq>
                                   <FieldRef Name='ItemId' />
                                   <Value Type='Number'>{itemId}</Value>
                                </Eq>
                                <Eq>
                                    <FieldRef Name='StepModule' />
                                    <Value Type='Choice'>{stepModule}</Value>
                                </Eq>
                            </And>
                         </Where>";
            return GetByQuery(queryStr);
        }

        public IList<TaskManagement> GetRelatedTasks(int itemId, string stepModule)
        {
            string queryStr = string.Format(@"<Where>
                                                  <And>
                                                     <Eq>
                                                        <FieldRef Name='ItemId' />
                                                        <Value Type='Number'>{0}</Value>
                                                     </Eq>
                                                     <And>
                                                        <Eq>
                                                           <FieldRef Name='StepModule' />
                                                           <Value Type='Choice'>{1}</Value>
                                                        </Eq>
                                                        <And>
                                                           <Neq>
                                                              <FieldRef Name='Status' />
                                                              <Value Type='Choice'>Completed</Value>
                                                           </Neq>
                                                           <Neq>
                                                              <FieldRef Name='Status' />
                                                              <Value Type='Choice'>Deferred</Value>
                                                           </Neq>
                                                        </And>
                                                     </And>
                                                  </And>
                                               </Where>", itemId, stepModule);

            return GetByQuery(queryStr);
        }

        public List<TaskManagement> GetTaskHistory(int itemId, string stepModule, bool fullData = true)
        {
            string queryStr = string.Format(@"<Where>
                                                  <And>
                                                     <Eq>
                                                        <FieldRef Name='ItemId' />
                                                        <Value Type='Number'>{0}</Value>
                                                     </Eq>
                                                     <And>
                                                        <Eq>
                                                           <FieldRef Name='StepModule' />
                                                           <Value Type='Choice'>{1}</Value>
                                                        </Eq>
                                                        <Or>
                                                           <Eq>
                                                              <FieldRef Name='TaskOutcome' />
                                                              <Value Type='Choice'>Approved</Value>
                                                           </Eq>
                                                           <Eq>
                                                              <FieldRef Name='TaskOutcome' />
                                                              <Value Type='Choice'>Rejected</Value>
                                                           </Eq>
                                                        </Or>
                                                     </And>
                                                  </And>
                                               </Where>
                                               <OrderBy>
                                                    <FieldRef Name='ID' Ascending='{2}' />
                                               </OrderBy>", itemId, stepModule, fullData.ToString());
            SPQuery spQuery = new SPQuery();
            spQuery.Query = queryStr;
            string[] viewFields = new string[] { StringConstant.TaskManagementList.AssignedTo, StringConstant.TaskManagementList.Description,
                    StringConstant.TaskManagementList.ItemId, StringConstant.TaskManagementList.TaskOutcome, StringConstant.DefaultSPListField.ModifiedField};
            if (fullData == false)
            {
                spQuery.RowLimit = 1;
            }

            List<TaskManagement> ret = GetByQuery(spQuery, viewFields);
            if (ret != null && ret.Count > 0)
            {
                List<int> assigneeIds = ret.Select(e => e.AssignedTo.ID).ToList();
                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SiteUrl);
                List<EmployeeInfo> employeeInfoCollection = _employeeInfoDAL.GetByADAccountIDs(assigneeIds);
                if (employeeInfoCollection != null && employeeInfoCollection.Count > 0)
                {
                    foreach (var item in ret)
                    {
                        EmployeeInfo employeeInfo = employeeInfoCollection.Where(e => e.ADAccount.ID == item.AssignedTo.ID).FirstOrDefault();
                        if (employeeInfo != null)
                        {
                            item.AssignedTo.FullName = employeeInfo.FullName;
                        }
                    }
                }
            }

            return ret;
        }

        public TaskManagement CloneTask(TaskManagement originalTask)
        {
            TaskManagement newTask = new TaskManagement();

            newTask.AssignedTo = originalTask.AssignedTo;
            newTask.Department = originalTask.Department;
            newTask.Description = originalTask.Description;
            newTask.DueDate = originalTask.DueDate;
            newTask.ItemId = originalTask.ItemId;
            newTask.ItemURL = originalTask.ItemURL;
            newTask.ListURL = originalTask.ListURL;
            newTask.NextAssign = originalTask.NextAssign;
            newTask.PercentComplete = originalTask.PercentComplete;
            newTask.RelatedTasks = originalTask.RelatedTasks;
            newTask.StartDate = originalTask.StartDate;
            newTask.StepModule = originalTask.StepModule;
            newTask.StepStatus = originalTask.StepStatus;
            newTask.TaskName = originalTask.TaskName;
            newTask.TaskOutcome = originalTask.TaskOutcome;
            newTask.TaskStatus = originalTask.TaskStatus;

            return newTask;
        }

        public override int SaveItem(TaskManagement entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;

                    SPList list = web.GetList($"{web.Url}{ListUrl}");

                    if (entity.UniqueId == null)
                    {
                        SPListItem newElem = list.AddItem();
                        MapToListItem(entity, newElem);

                        newElem.Update();
                        //UpdateCalculatedFields(list);
                        entity.ID = newElem.ID;
                        entity.UniqueId = newElem.UniqueId;
                    }
                    else
                    {
                        SPListItem existingItem = list.GetItemById(entity.ID);
                        MapToListItem(entity, existingItem);

                        existingItem.Update();
                        //UpdateCalculatedFields(list);
                    }
                }
            }
            return entity.ID;
        }
    }
}
