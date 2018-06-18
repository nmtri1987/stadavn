using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Builder;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Web;
using System.Collections.Generic;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using System.Linq;
using RBVH.Stada.Intranet.Biz.Extension;
using System.Globalization;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class VehicleManagementDAL : BaseDAL<VehicleManagement>, IModuleBuilder, IDelegationManager, IFilterTaskManager
    {
        private const string TASK_NAME = "Vehicle request approval/ Duyệt yêu cầu đi xe";
        private const string LINK_MAIL = "{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/TransportationManagement/{1}.aspx";
        private readonly EmployeeInfoDAL employeeInfoDAL;
        public VehicleManagementDAL(string siteUrl) : base(siteUrl)
        {
            employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
        }

        public IList<EmployeeInfo> CreateApprovalList(int departmentId, int locationId)
        {
            return employeeInfoDAL.GetByPositionDepartment(Constants.StringConstant.EmployeePosition.DepartmentHead, departmentId, locationId);
        }

        public int SaveOrUpdate(VehicleManagement item)
        {
            int itemId = 0;
            using (SPSite spSite = new SPSite(SiteUrl))
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    itemId = SaveOrUpdate(spWeb, item);
                }
            }
            return itemId;
        }

        public int SaveOrUpdate(SPWeb spWeb, VehicleManagement item)
        {
            int itemId = 0;

            SPList splist = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            if (item.ID > 0)
            {
                SPListItem spListItem = splist.GetItemById(item.ID);
                if (!string.IsNullOrEmpty(item.ApprovalStatus))
                {
                    spListItem[StringConstant.CommonSPListField.ApprovalStatusField] = item.ApprovalStatus;
                    spListItem[StringConstant.CommonSPListField.CommonCommentField] = item.CommonComment;

                    spWeb.AllowUnsafeUpdates = true;
                    spListItem.Update();
                    itemId = spListItem.ID;
                    spWeb.AllowUnsafeUpdates = false;
                }
            }

            return itemId;
        }

        public VehicleManagement RunWorkFlow(VehicleManagement vehicleManagement, TaskManagement taskOfPrevStep)
        {
            if (vehicleManagement == null) return null;

            TaskManagement taskManagement = new TaskManagement();

            taskManagement.StartDate = DateTime.Now;
            taskManagement.DueDate = vehicleManagement.RequestDueDate;
            taskManagement.PercentComplete = 0;
            taskManagement.ItemId = vehicleManagement.ID;
            taskManagement.ItemURL = taskOfPrevStep.ItemURL;
            taskManagement.ListURL = taskOfPrevStep.ListURL;
            taskManagement.TaskName = TASK_NAME;
            taskManagement.TaskStatus = TaskStatusList.InProgress;
            taskManagement.StepModule = StepModuleList.VehicleManagement.ToString();
            taskManagement.Department = vehicleManagement.CommonDepartment.LookupId > 0 ? vehicleManagement.CommonDepartment : null;
            taskManagement.AssignedTo = taskOfPrevStep.NextAssign;
            taskManagement.NextAssign = null;

            StepManagementDAL _stepManagementDAL = new StepManagementDAL(this.SiteUrl);
            var nextStep = _stepManagementDAL.GetNextStepManagement(taskOfPrevStep.StepStatus, StepModuleList.VehicleManagement, vehicleManagement.CommonDepartment.LookupId);
            if (nextStep != null)
            {
                taskManagement.StepStatus = nextStep.StepStatus;
                ModuleBuilder moduleBuilder = new ModuleBuilder(this.SiteUrl);
                // TODO: Get location by vehicleManagement:
                var locationId = 2;
                var nextAssign = moduleBuilder.GetNextApproval(vehicleManagement.CommonDepartment.LookupId, locationId, StepModuleList.VehicleManagement, nextStep.StepNumber);
                if (nextAssign != null)
                {
                    taskManagement.NextAssign = nextAssign.ADAccount;
                }
            }

            TaskManagementDAL taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            int retId = taskManagementDAL.SaveItem(taskManagement);

            vehicleManagement.ApprovalStatus = taskManagement.StepStatus;
            this.SaveOrUpdate(vehicleManagement);

            return vehicleManagement;
        }

        public void SendEmail(VehicleManagement model, EmailTemplate EmailTempalte, EmployeeInfo approver, EmployeeInfo toUser, VehicleTypeOfEmail typeOfEmail, string webUrl)
        {
            var sendToEmail = string.Empty;
            var receiverName = string.Empty;
            var link = string.Empty;
            var approveLinkFormat = "{0}/Lists/VehicleManagement/EditForm.aspx?subSection=TransportationManagement&ID={1}&Source={2}";

            switch (toUser.EmployeePosition.LookupId)
            {
                case (int)StringConstant.EmployeePosition.Administrator:
                    link = string.Format(LINK_MAIL, webUrl, "TransportationManagementAdmin");
                    break;
                case (int)StringConstant.EmployeePosition.DepartmentHead:
                    link = string.Format(LINK_MAIL, webUrl, "TransportationManagementManager");
                    break;
                case (int)StringConstant.EmployeePosition.BOD:
                    link = string.Format(LINK_MAIL, webUrl, "TransportationManagementBOD");
                    break;
                default:
                    link = string.Format(LINK_MAIL, webUrl, "TransportationManagementMember");
                    break;
            }

            if (typeOfEmail == VehicleTypeOfEmail.Request)
            {
                link = string.Format(approveLinkFormat, webUrl, model.ID, HttpUtility.UrlEncode(link + "#tab2"));
            }
            else
            {
                link = string.Format(approveLinkFormat, webUrl, model.ID, HttpUtility.UrlEncode(link));
            }

            sendToEmail = toUser.Email;
            receiverName = toUser.FullName;

            var content = HTTPUtility.HtmlDecode(EmailTempalte.MailBody);
            var department = DepartmentListSingleton.GetDepartmentByID(model.CommonDepartment.LookupId, this.SiteUrl);
            switch (typeOfEmail)
            {
                case VehicleTypeOfEmail.Request:
                    content = content.Replace("{0}", receiverName);
                    content = content.Replace("{1}", model.Requester.LookupValue);
                    content = content.Replace("{2}", model.From.ToString("dd/MM/yyy hh:mm"));
                    content = content.Replace("{3}", model.ToDate.ToString("dd/MM/yyy hh:mm"));
                    content = content.Replace("{4}", department.Name);
                    content = content.Replace("{5}", department.VietnameseName);
                    break;
                case VehicleTypeOfEmail.Approve:
                case VehicleTypeOfEmail.Reject:
                    content = content.Replace("{0}", model.Requester.LookupValue);
                    content = content.Replace("{1}", approver.FullName);
                    content = content.Replace("{2}", model.From.ToString("dd/MM/yyy hh:mm"));
                    content = content.Replace("{3}", model.ToDate.ToString("dd/MM/yyy hh:mm"));
                    break;
                default:
                    break;
            }
            content = content.Replace("#link", link);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            sendMailActivity.SendMail(webUrl, EmailTempalte.MailSubject, sendToEmail, true, false, content);
        }

        public void SendDelegationEmail(VehicleManagement model, EmailTemplate EmailTempalte, List<EmployeeInfo> toUsers, string webUrl)
        {
            var link = string.Format(@"{0}/Lists/VehicleManagement/EditForm.aspx?subSection=TransportationManagement&ID={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", webUrl, model.ID);
            var department = DepartmentListSingleton.GetDepartmentByID(model.CommonDepartment.LookupId, this.SiteUrl);

            SendEmailActivity sendMailActivity = new SendEmailActivity();
            if (toUsers != null)
            {
                foreach (var toUser in toUsers)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(toUser.Email))
                        {
                            var content = HTTPUtility.HtmlDecode(EmailTempalte.MailBody);
                            content = content.Replace("{0}", toUser.FullName);
                            content = content.Replace("{1}", model.Requester.LookupValue);
                            content = content.Replace("{2}", model.From.ToString("dd/MM/yyy hh:mm"));
                            content = content.Replace("{3}", model.ToDate.ToString("dd/MM/yyy hh:mm"));
                            content = content.Replace("{4}", department.Name);
                            content = content.Replace("{5}", department.VietnameseName);
                            content = content.Replace("#link", link);
                            sendMailActivity.SendMail(webUrl, EmailTempalte.MailSubject, toUser.Email, true, false, content);
                        }
                    }
                    catch { }
                }
            }
        }

        public VehicleManagement SetDueDate(VehicleManagement vehicleManagement)
        {
            DateTime reqDueDate = vehicleManagement.From.Date;
            //if (reqDueDate == DateTime.Now.Date)
            //{
            //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            //else
            //{
            //    reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            vehicleManagement.RequestDueDate = reqDueDate;

            return vehicleManagement;
        }

        #region Delegation
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> listOfTasks = new List<Delegation>();

            List<string> viewFields = new List<string>() { };
            viewFields.Add(StringConstant.CommonSPListField.RequesterField);
            viewFields.Add(StringConstant.CommonSPListField.CommonDepartmentField);
            viewFields.Add(StringConstant.VehicleManagementList.VehicleType);
            viewFields.Add(StringConstant.VehicleManagementList.CommonFrom);
            viewFields.Add(StringConstant.VehicleManagementList.To);
            viewFields.Add(StringConstant.DefaultSPListField.CreatedField);
            List<VehicleManagement> itemCollection = this.GetByQuery(this.BuildQueryGetListOfTasks(fromEmployee), viewFields.ToArray());
            if (itemCollection != null)
            {
                foreach (var item in itemCollection)
                {
                    Delegation delegation = new Delegation(item);
                    listOfTasks.Add(delegation);
                }
            }

            return listOfTasks;
        }

        private string BuildQueryGetListOfTasks(EmployeeInfo employeeInfo)
        {
            string filterStr = "<Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>";

            TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
            string taskQueryStr = string.Format(@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='Status' />
                                        <Value Type='Choice'>{0}</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{1}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='AssignedTo' LookupId='TRUE' />
                                            <Value Type='User'>{2}</Value>
                                        </Eq>
                                     </And>
                                  </And>
                               </Where>", TaskStatusList.InProgress.ToString(), StepModuleList.VehicleManagement.ToString(), employeeInfo.ADAccount.ID);
            List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
            if (taskManagementCollection != null && taskManagementCollection.Count > 0)
            {
                List<int> itemIds = taskManagementCollection.Where(t => t.ItemId > 0).Select(t => t.ItemId).ToList();

                if (itemIds != null && itemIds.Count > 0)
                {
                    filterStr = "";
                    foreach (var itemId in itemIds)
                    {
                        filterStr += string.Format("<Value Type = 'Number'>{0}</Value>", itemId);
                    }
                    if (!string.IsNullOrEmpty(filterStr))
                    {
                        filterStr = string.Format("<In><FieldRef Name = 'ID'/><Values>{0}</Values></In>", filterStr);
                    }
                }
            }

            filterStr = string.Format("<Where>{0}</Where>", filterStr);

            return filterStr;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            VehicleManagement vehicleManagement = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(vehicleManagement, currentWeb);
            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem ret = null;

            VehicleManagement vehicleManagement = this.ParseToEntity(listItem);
            string approvalStatus = vehicleManagement.ApprovalStatus.ToLower();
            if (approvalStatus != ApprovalStatus.Approved.ToLower() && approvalStatus != ApprovalStatus.Rejected.ToLower() && approvalStatus != ApprovalStatus.Cancelled.ToLower())
            {
                TaskManagementDAL _taskManagementDAL = new TaskManagementDAL(this.SiteUrl);
                string taskQueryStr = string.Format(@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='CurrentStepStatus' />
                                        <Value Type='Choice'>{0}</Value>
                                     </Eq>
                                     <And>
                                        <Eq>
                                            <FieldRef Name='StepModule' />
                                            <Value Type='Choice'>{1}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='ItemId' />
                                            <Value Type='Number'>{2}</Value>
                                        </Eq>
                                     </And>
                                  </And>
                               </Where><OrderBy><FieldRef Name='ID' Ascending='False' /></OrderBy>", vehicleManagement.ApprovalStatus.ToString(), StepModuleList.VehicleManagement.ToString(), vehicleManagement.ID);
                SPQuery spQuery = new SPQuery()
                {
                    Query = taskQueryStr,
                    RowLimit = 1
                };

                List<TaskManagement> taskManagementCollection = _taskManagementDAL.GetByQuery(taskQueryStr);
                if (taskManagementCollection != null && taskManagementCollection.Count > 0)
                {
                    var currentStepApprover = employeeInfoDAL.GetByADAccount(taskManagementCollection[0].AssignedTo.ID);
                    if (currentStepApprover != null)
                        ret = new LookupItem() { LookupId = currentStepApprover.ID, LookupValue = currentStepApprover.FullName };
                }
            }

            return ret;
        }

        public bool IsValidTask(int listItemID)
        {
            return true;
        }
        #endregion

        #region "Overview"

        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion
    }
}
