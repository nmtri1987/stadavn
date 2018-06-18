using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.Report;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class OverTimeManagementDAL : BaseDAL<OverTimeManagement>, IDelegationManager, IFilterTaskManager
    {
        private readonly OverTimeManagementDetailDAL _overTimeManagementDetailDAL;
        private readonly EmployeeInfoDAL _employeeInfoDAL;
        private readonly SendEmailActivity _sendEmailActivity;
        private readonly EmailTemplateDAL _emailTemplateDAL;

        public OverTimeManagementDAL(string siteUrl) : base(siteUrl)
        {
            ListUrl = "/Lists/OvertimeManagement";
            _overTimeManagementDetailDAL = new OverTimeManagementDetailDAL(SiteUrl);
            _employeeInfoDAL = new EmployeeInfoDAL(SiteUrl);
            _sendEmailActivity = new SendEmailActivity();
            _emailTemplateDAL = new EmailTemplateDAL(SiteUrl);
        }

        //public void AddAttachment(int id, string attachmentName)
        //{
        //    ULSLogging.Log(new SPDiagnosticsCategory("STADA - OverTimeManagementDAL - AddAttachment fn",
        //                    TraceSeverity.None, EventSeverity.Information), TraceSeverity.Unexpected, string.Format(CultureInfo.InvariantCulture, "Start Add Attachment"));

        //    OverTimeManagement overTimeManagement = new OverTimeManagement();

        //    //// Query SPList
        //    if (id > 0)
        //    {
        //        using (SPSite site = new SPSite(SiteUrl))
        //        {
        //            using (SPWeb web = site.OpenWeb())
        //            {
        //                web.AllowUnsafeUpdates = true;
        //                SPList splist = web.GetList($"{web.Url}{ListUrl}");
        //                SPListItem overTimeManagementItem = splist.GetItemById(id);

        //                if (overTimeManagementItem != null)
        //                {
        //                    // TODO: Huong will update add attachment in Base
        //                    var overtimeManagmentDetail = _overTimeManagementDetailDAL.GetByOvertimeId(id);
        //                    var overtimeManagementModel = ParseToEntity(overTimeManagementItem);
        //                    overtimeManagementModel.OverTimeManagementDetailList = overtimeManagmentDetail;
        //                    MemoryStream fileStream = WordHelper.CreateOverTimeWorkApplication(overtimeManagementModel);
        //                    if (fileStream != null && fileStream.Length > 0)
        //                    {
        //                        overTimeManagementItem.Attachments.Add("Giay de nghi lam ngoai gio.docx", fileStream.ToArray());
        //                        overTimeManagementItem.Update();
        //                    }
        //                }
        //                web.AllowUnsafeUpdates = false;
        //            }
        //        }
        //    }
        //}

        //public List<OverTimeManagement> GetByDateDepartment(string datestring, int departmentId)
        //{
        //    var overTimeManagementList = new List<OverTimeManagement>();
        //    //// Query SPList
        //    DateTime date;

        //    if (DateTime.TryParse(datestring, out date))
        //    {
        //        var startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        //        var endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);
        //        var startDateString = startDate.ToString(StringConstant.DateFormatTZForCAML);
        //        var endDateString = endDate.ToString(StringConstant.DateFormatTZForCAML);

        //        overTimeManagementList = GetByQuery($@"<Where>
        //                              <And>
        //                                 <Eq>
        //                                    <FieldRef Name='CommonDepartment' LookupId='TRUE' />
        //                                    <Value Type='Lookup' LookupId='TRUE'>{departmentId}</Value>
        //                                 </Eq>
        //                                 <And>

        //                                     <IsNull>
        //                                                <FieldRef Name='ApprovalStatus' />
        //                                     </IsNull>

        //                                     <And>
        //                                        <Geq>
        //                                           <FieldRef Name='CommonDate' />
        //                                           <Value IncludeTimeValue='TRUE' Type='DateTime'>{startDateString}</Value>
        //                                        </Geq>
        //                                        <Leq>
        //                                           <FieldRef Name='CommonDate' />
        //                                           <Value IncludeTimeValue='TRUE' Type='DateTime'>{endDateString}</Value>
        //                                        </Leq>
        //                                     </And>
        //                                 </And>
        //                              </And>
        //                           </Where>", string.Empty);
        //    }
        //    return overTimeManagementList;
        //}

        public List<OverTimeManagement> GetByApprovalStatus(DateTime date, int departmentId, int locationId, EnumApprovalStatus status)
        {
            var overTimeManagementList = new List<OverTimeManagement>();
            //// Query SPList
            string camlStatus = string.Empty;
            switch (status)
            {
                case EnumApprovalStatus.InProgress:
                    camlStatus = @"<IsNull>
                                    <FieldRef Name = 'ApprovalStatus' />
                                </IsNull>";
                    break;
                default:
                    camlStatus = @"<IsNull>
                                    <FieldRef Name = 'ApprovalStatus' />
                                </IsNull>";
                    break;
            }

            var startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            var startDateString = startDate.ToString(StringConstant.DateFormatTZForCAML);
            var endDateString = endDate.ToString(StringConstant.DateFormatTZForCAML);

            overTimeManagementList = GetByQuery($@"
                                    <Where>
                                        <And>
                                            <And>
                                                <Eq>
                                                    <FieldRef Name='CommonDepartment' LookupId='TRUE' />
                                                    <Value Type='Lookup' LookupId='TRUE'>{departmentId}</Value>
                                                </Eq>
                                                <Eq>
                                                    <FieldRef Name='CommonLocation' LookupId='TRUE' />
                                                    <Value Type='Lookup' LookupId='TRUE'>{locationId}</Value>
                                                </Eq>
                                            </And>
                                            
                                            <And>
                                                {camlStatus}
                                                <And>
                                                    <Geq>
                                                        <FieldRef Name='CommonDate' />
                                                        <Value IncludeTimeValue='TRUE' Type='DateTime'>{startDateString}</Value>
                                                    </Geq>
                                                    <Leq>
                                                        <FieldRef Name='CommonDate' />
                                                        <Value IncludeTimeValue='TRUE' Type='DateTime'>{endDateString}</Value>
                                                    </Leq>
                                                </And>
                                            </And>
                                        </And>
                                    </Where>", string.Empty);
            return overTimeManagementList;
        }

        //public List<OverTimeManagement> GetByShiftMonth(int month, int year, int departmentId)
        //{
        //    var overTimeManagementList = new List<OverTimeManagement>();
        //    //// Query SPList

        //    //var startDate = new DateTime(year, month - 1, 21, 0, 0, 0);
        //    var endDate = new DateTime(year, month, 21, 0, 0, 0);
        //    var startDate = endDate.AddMonths(-1);
        //    var startDateString = startDate.ToString(StringConstant.DateFormatTZForCAML);
        //    var endDateString = endDate.ToString(StringConstant.DateFormatTZForCAML);

        //    overTimeManagementList = GetByQuery($@"
        //                            <Where>
        //                                <And>
        //                                    <Eq>
        //                                        <FieldRef Name='CommonDepartment' LookupId='TRUE' />
        //                                        <Value Type='Lookup' LookupId='TRUE'>{departmentId}</Value>
        //                                    </Eq>
        //                                    <And>
        //                                        <And>
        //                                            <Geq>
        //                                                <FieldRef Name='CommonDate' />
        //                                                <Value IncludeTimeValue='TRUE' Type='DateTime'>{startDateString}</Value>
        //                                            </Geq>
        //                                            <Leq>
        //                                                <FieldRef Name='CommonDate' />
        //                                                <Value IncludeTimeValue='TRUE' Type='DateTime'>{endDateString}</Value>
        //                                            </Leq>
        //                                        </And>
        //                                    </And>
        //                                </And>
        //                            </Where>", string.Empty);
        //    return overTimeManagementList;
        //}

        public bool UpdateApprovalStatus(int id, string status)
        {
            //SPSecurity.RunWithElevatedPrivileges(delegate ()
            //{
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList splist = web.GetList(string.Format("{0}{1}", web.Url, ListUrl));
                    SPListItem item = splist.GetItemById(id);
                    if (item != null)
                    {
                        item[StringConstant.CommonSPListField.ApprovalStatusField] = status;
                        var overtimeManagement = ParseToEntity(item);
                        item.Update();

                        if (status == "true")
                        {
                            var overtimedetailLlist = _overTimeManagementDetailDAL.GetByOvertimeId(id);
                            foreach (var overTimeItem in overtimedetailLlist)
                            {
                                var employee = _employeeInfoDAL.GetByID(overTimeItem.Employee.LookupId);
                                var emailTempate = _emailTemplateDAL.GetByKey("Overtime_work_application");
                                var bodyHTML = emailTempate.MailBody;

                                bodyHTML = bodyHTML.Replace("{{EmployeeFullName}}", employee.FullName);
                                bodyHTML = bodyHTML.Replace("{{Proposer}}", overtimeManagement.Requester.LookupValue);
                                bodyHTML = bodyHTML.Replace("{{Department}}", overtimeManagement.CommonDepartment.LookupValue);
                                bodyHTML = bodyHTML.Replace("{{place}}", overtimeManagement.CommonLocation.LookupValue);
                                bodyHTML = bodyHTML.Replace("{{from}}", overTimeItem.OvertimeFrom.ToString("HH:mm"));
                                bodyHTML = bodyHTML.Replace("{{to}}", overTimeItem.OvertimeTo.ToString("HH:mm"));
                                bodyHTML = bodyHTML.Replace("{{EmployeeId}}", employee.EmployeeID);
                                bodyHTML = bodyHTML.Replace("{{totalHour}}", overTimeItem.WorkingHours + "");
                                bodyHTML = bodyHTML.Replace("{{date}}", overtimeManagement.CommonDate.ToString("dd/MM/yyyy"));
                                bodyHTML = bodyHTML.Replace("{{hm}}", overTimeItem.HM);
                                bodyHTML = bodyHTML.Replace("{{kd}}", overTimeItem.KD);
                                bodyHTML = bodyHTML.Replace("{{workcontent}}", overTimeItem.Task);

                                _sendEmailActivity.SendMail(web.Url, "Overtime Work Application/Giấy đề nghị làm thêm giờ", employee.Email, true, false, bodyHTML);
                            }
                        }
                    }
                    web.AllowUnsafeUpdates = false;
                }
            }

            return true;
        }

        //public bool Update(OverTimeManagement newItem)
        //{
        //    SPSecurity.RunWithElevatedPrivileges(delegate
        //    {
        //        using (SPSite site = new SPSite(SiteUrl))
        //        {
        //            using (SPWeb web = site.OpenWeb())
        //            {
        //                web.AllowUnsafeUpdates = true;
        //                SPList splist = web.GetList(string.Format("{0}{1}", web.Url, ListUrl));
        //                SPListItem overTimeManagementListItem = splist.GetItemById(newItem.ID);
        //                if (overTimeManagementListItem != null)
        //                {
        //                    overTimeManagementListItem[StringConstant.CommonSPListField.ApprovalStatusField] = newItem.ApprovalStatus;
        //                    overTimeManagementListItem[StringConstant.CommonSPListField.CommonDepartmentField] = newItem.CommonDepartment.LookupId;
        //                    overTimeManagementListItem[StringConstant.CommonSPListField.CommonLocationField] = newItem.CommonLocation.LookupId;
        //                    overTimeManagementListItem[StringConstant.CommonSPListField.RequesterField] = newItem.Requester.LookupId;
        //                    overTimeManagementListItem[StringConstant.OverTimeManagementList.CommonDateField] = newItem.CommonDate;
        //                    overTimeManagementListItem[StringConstant.OverTimeManagementList.OtherRequirementsField] = newItem.OtherRequirements;
        //                    overTimeManagementListItem[StringConstant.OverTimeManagementList.SumOfEmployeeField] = newItem.SumOfEmployee;
        //                    overTimeManagementListItem[StringConstant.OverTimeManagementList.SumOfMealField] = newItem.SumOfMeal;
        //                    if (SPContext.Current != null)
        //                    {
        //                        SPUser approver = SPContext.Current.Web.EnsureUser(newItem.ApprovedBy.UserName);
        //                        SPFieldUserValue approverValue = new SPFieldUserValue(SPContext.Current.Web, approver.ID, approver.LoginName);
        //                        overTimeManagementListItem[StringConstant.OverTimeManagementList.ApprovedByField] = approverValue;
        //                    }

        //                    overTimeManagementListItem.Update();

        //                }
        //                web.AllowUnsafeUpdates = false;
        //            }
        //        }
        //    });
        //    return true;
        //}

        //public int Insert(OverTimeManagement newItem)
        //{
        //    int returnId = 0;
        //    SPSecurity.RunWithElevatedPrivileges(delegate
        //    {
        //        using (SPSite site = new SPSite(SiteUrl))
        //        {
        //            using (SPWeb web = site.OpenWeb())
        //            {
        //                web.AllowUnsafeUpdates = true;
        //                SPList overTimeManagement = web.GetList($"{web.Url}{ListUrl}");

        //                SPListItem overTimeManagementListItem = overTimeManagement.AddItem();
        //                overTimeManagementListItem[StringConstant.CommonSPListField.ApprovalStatusField] = newItem.ApprovalStatus;
        //                overTimeManagementListItem[StringConstant.CommonSPListField.CommonDepartmentField] = newItem.CommonDepartment.LookupId;
        //                overTimeManagementListItem[StringConstant.CommonSPListField.CommonLocationField] = newItem.CommonLocation.LookupId;
        //                overTimeManagementListItem[StringConstant.CommonSPListField.RequesterField] = newItem.Requester.LookupId;
        //                overTimeManagementListItem[StringConstant.OverTimeManagementList.CommonDateField] = newItem.CommonDate;
        //                overTimeManagementListItem[StringConstant.OverTimeManagementList.OtherRequirementsField] = newItem.OtherRequirements;
        //                overTimeManagementListItem[StringConstant.OverTimeManagementList.SumOfEmployeeField] = newItem.SumOfEmployee;
        //                overTimeManagementListItem[StringConstant.OverTimeManagementList.SumOfMealField] = newItem.SumOfMeal;
        //                SPUser approver = SPContext.Current.Web.EnsureUser(newItem.ApprovedBy.UserName);
        //                SPFieldUserValue approverValue = new SPFieldUserValue(SPContext.Current.Web, approver.ID, approver.LoginName);
        //                overTimeManagementListItem[StringConstant.OverTimeManagementList.ApprovedByField] = approverValue;

        //                overTimeManagementListItem.Update();
        //                returnId = overTimeManagementListItem.ID;
        //                web.AllowUnsafeUpdates = false;
        //            }
        //        }
        //    });
        //    return returnId;
        //}

        //public void InsertOrUpdate(OverTimeManagement overTimeManagement)
        //{
        //    var overtimeMangementId = 0;
        //    if (overTimeManagement.ID > 0)
        //    {
        //        overtimeMangementId = overTimeManagement.ID;
        //        this.Update(overTimeManagement);
        //    }
        //    else
        //    {
        //        overtimeMangementId = this.Insert(overTimeManagement);

        //    }
        //    if (overtimeMangementId > 0)
        //    {
        //        foreach (var detail in overTimeManagement.OverTimeManagementDetailList)
        //        {
        //            _overTimeManagementDetailDAL.SaveItem(detail);
        //        }
        //    }
        //}

        //public int SaveOrUpdate(OverTimeManagement overTimeManagementEntity)
        //{
        //    int returnId = 0;
        //    SPSecurity.RunWithElevatedPrivileges(delegate
        //    {
        //        using (SPSite site = new SPSite(SiteUrl))
        //        {
        //            using (SPWeb web = site.OpenWeb())
        //            {
        //                web.AllowUnsafeUpdates = true;
        //                SPListItem spitemOvertimeManagement = null;
        //                SPList splistOvertimeManagement = web.GetList($"{web.Url}{ListUrl}");
        //                if (overTimeManagementEntity.ID > 0)
        //                {
        //                    spitemOvertimeManagement = splistOvertimeManagement.GetItemById(overTimeManagementEntity.ID);
        //                }
        //                else
        //                {
        //                    spitemOvertimeManagement = splistOvertimeManagement.AddItem();
        //                    spitemOvertimeManagement[StringConstant.CommonSPListField.CommonReqDueDateField] = overTimeManagementEntity.RequestDueDate;
        //                }

        //                spitemOvertimeManagement[StringConstant.CommonSPListField.ApprovalStatusField] = overTimeManagementEntity.ApprovalStatus;
        //                spitemOvertimeManagement[StringConstant.CommonSPListField.CommonDepartmentField] = overTimeManagementEntity.CommonDepartment.LookupId;
        //                spitemOvertimeManagement[StringConstant.CommonSPListField.CommonLocationField] = overTimeManagementEntity.CommonLocation.LookupId;
        //                spitemOvertimeManagement[StringConstant.CommonSPListField.RequesterField] = overTimeManagementEntity.Requester.LookupId;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.CommonDateField] = overTimeManagementEntity.CommonDate;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.PlaceField] = overTimeManagementEntity.Place;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.OtherRequirementsField] = overTimeManagementEntity.OtherRequirements;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.DHCommentsField] = overTimeManagementEntity.DHComments;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.BODCommentsField] = overTimeManagementEntity.BODComments;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.SecurityCommentsField] = overTimeManagementEntity.SecurityComments;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.SumOfEmployeeField] = overTimeManagementEntity.SumOfEmployee;
        //                spitemOvertimeManagement[StringConstant.OverTimeManagementList.SumOfMealField] = overTimeManagementEntity.SumOfMeal;
        //                spitemOvertimeManagement[StringConstant.CommonSPListField.ColForSortField] = overTimeManagementEntity.ColForSort;
        //                if (overTimeManagementEntity.ApprovedBy != null)
        //                {
        //                    SPUser approver = null;
        //                    if (overTimeManagementEntity.ApprovedBy.ID > 0)
        //                        approver = web.AllUsers.GetByID(overTimeManagementEntity.ApprovedBy.ID);
        //                    else if (!string.IsNullOrEmpty(overTimeManagementEntity.ApprovedBy.UserName))
        //                        approver = web.EnsureUser(overTimeManagementEntity.ApprovedBy.UserName);

        //                    SPFieldUserValue approverValue = new SPFieldUserValue(web, approver.ID, approver.LoginName);
        //                    spitemOvertimeManagement[StringConstant.OverTimeManagementList.ApprovedByField] = approverValue;
        //                }

        //                if (overTimeManagementEntity.FirstApprovedBy != null)
        //                {
        //                    SPUser firstApprover = null;
        //                    if (overTimeManagementEntity.FirstApprovedBy.ID > 0)
        //                        firstApprover = web.AllUsers.GetByID(overTimeManagementEntity.FirstApprovedBy.ID);
        //                    else if (!string.IsNullOrEmpty(overTimeManagementEntity.FirstApprovedBy.UserName))
        //                        firstApprover = web.EnsureUser(overTimeManagementEntity.FirstApprovedBy.UserName);

        //                    if (firstApprover != null)
        //                    {
        //                        SPFieldUserValue firstApproverValue = new SPFieldUserValue(web, firstApprover.ID, firstApprover.LoginName);
        //                        spitemOvertimeManagement[StringConstant.OverTimeManagementList.FirstApprovedByField] = firstApproverValue;
        //                        spitemOvertimeManagement[StringConstant.OverTimeManagementList.FirstApprovedDateField] = overTimeManagementEntity.FirstApprovedDate; //.ToString(StringConstant.DateFormatddMMyyyyHHmmss);
        //                    }
        //                }

        //                spitemOvertimeManagement.Update();
        //                web.AllowUnsafeUpdates = false;

        //                returnId = spitemOvertimeManagement.ID;

        //            }
        //        }
        //    });
        //    return returnId;
        //}

        public int SaveOrUpdate(SPWeb web, OverTimeManagement overTimeManagementEntity)
        {
            int returnId = 0;
            web.AllowUnsafeUpdates = true;
            SPListItem spitemOvertimeManagement = null;
            SPList splistOvertimeManagement = web.GetList($"{web.Url}{ListUrl}");
            if (overTimeManagementEntity.ID > 0)
            {
                spitemOvertimeManagement = splistOvertimeManagement.GetItemById(overTimeManagementEntity.ID);
            }
            else
            {
                spitemOvertimeManagement = splistOvertimeManagement.AddItem();
                spitemOvertimeManagement[StringConstant.CommonSPListField.CommonReqDueDateField] = overTimeManagementEntity.RequestDueDate;
            }

            spitemOvertimeManagement[StringConstant.CommonSPListField.ApprovalStatusField] = overTimeManagementEntity.ApprovalStatus;
            spitemOvertimeManagement[StringConstant.CommonSPListField.CommonDepartmentField] = overTimeManagementEntity.CommonDepartment.LookupId;
            spitemOvertimeManagement[StringConstant.CommonSPListField.CommonLocationField] = overTimeManagementEntity.CommonLocation.LookupId;
            spitemOvertimeManagement[StringConstant.CommonSPListField.RequesterField] = overTimeManagementEntity.Requester.LookupId;
            spitemOvertimeManagement[StringConstant.OverTimeManagementList.CommonDateField] = overTimeManagementEntity.CommonDate;
            spitemOvertimeManagement[StringConstant.OverTimeManagementList.PlaceField] = overTimeManagementEntity.Place;
            spitemOvertimeManagement[StringConstant.OverTimeManagementList.OtherRequirementsField] = overTimeManagementEntity.OtherRequirements;
            spitemOvertimeManagement[StringConstant.OverTimeManagementList.SumOfEmployeeField] = overTimeManagementEntity.SumOfEmployee;
            spitemOvertimeManagement[StringConstant.OverTimeManagementList.SumOfMealField] = overTimeManagementEntity.SumOfMeal;
            spitemOvertimeManagement[StringConstant.CommonSPListField.ColForSortField] = overTimeManagementEntity.ColForSort;
            if (overTimeManagementEntity.ApprovedBy != null)
            {
                SPUser approver = null;
                if (overTimeManagementEntity.ApprovedBy.ID > 0)
                    approver = web.AllUsers.GetByID(overTimeManagementEntity.ApprovedBy.ID);
                else if (!string.IsNullOrEmpty(overTimeManagementEntity.ApprovedBy.UserName))
                    approver = web.EnsureUser(overTimeManagementEntity.ApprovedBy.UserName);

                SPFieldUserValue approverValue = new SPFieldUserValue(web, approver.ID, approver.LoginName);
                spitemOvertimeManagement[StringConstant.OverTimeManagementList.ApprovedByField] = approverValue;
            }

            if (overTimeManagementEntity.FirstApprovedBy != null)
            {
                SPUser firstApprover = null;
                if (overTimeManagementEntity.FirstApprovedBy.ID > 0)
                    firstApprover = web.AllUsers.GetByID(overTimeManagementEntity.FirstApprovedBy.ID);
                else if (!string.IsNullOrEmpty(overTimeManagementEntity.FirstApprovedBy.UserName))
                    firstApprover = web.EnsureUser(overTimeManagementEntity.FirstApprovedBy.UserName);

                if (firstApprover != null)
                {
                    SPFieldUserValue firstApproverValue = new SPFieldUserValue(web, firstApprover.ID, firstApprover.LoginName);
                    spitemOvertimeManagement[StringConstant.OverTimeManagementList.FirstApprovedByField] = firstApproverValue;
                    spitemOvertimeManagement[StringConstant.OverTimeManagementList.FirstApprovedDateField] = System.DateTime.Now.ToString(StringConstant.DateFormatMMddyyyy);
                }
            }

            spitemOvertimeManagement.Update();
            web.AllowUnsafeUpdates = false;

            returnId = spitemOvertimeManagement.ID;

            return returnId;
        }

        public override OverTimeManagement ParseToEntity(SPListItem listItem)
        {
            var OverTimeManagement = new OverTimeManagement
            {
                ID = listItem.ID,
                ApprovalStatus = listItem.ToString(StringConstant.CommonSPListField.ApprovalStatusField),
                CommonDepartment = listItem.ToLookupItemModel(StringConstant.CommonSPListField.CommonDepartmentField),
                CommonDepartment1066 = listItem.ToLookupItemModel(StringConstant.CommonSPListField.DepartmentName1066Field),
                CommonLocation = listItem.ToLookupItemModel(StringConstant.CommonSPListField.CommonLocationField),
                Requester = listItem.ToLookupItemModel(StringConstant.CommonSPListField.RequesterField),
                CommonDate = Convert.ToDateTime(listItem[StringConstant.OverTimeManagementList.CommonDateField]),
                Place = listItem.ToString(StringConstant.OverTimeManagementList.PlaceField),
                OtherRequirements = listItem.ToString(StringConstant.OverTimeManagementList.OtherRequirementsField),
                DHComments = listItem.ToString(StringConstant.OverTimeManagementList.DHCommentsField),
                BODComments = listItem.ToString(StringConstant.OverTimeManagementList.BODCommentsField),
                SecurityComments = listItem.ToString(StringConstant.OverTimeManagementList.SecurityCommentsField),
                SumOfEmployee = Convert.ToInt32(listItem.ToString(StringConstant.OverTimeManagementList.SumOfEmployeeField)),
                SumOfMeal = Convert.ToInt32(listItem.ToString(StringConstant.OverTimeManagementList.SumOfMealField)),
                ApprovedBy = listItem.ToUserModel(StringConstant.OverTimeManagementList.ApprovedByField),
                FirstApprovedDate = Convert.ToDateTime(listItem[StringConstant.OverTimeManagementList.FirstApprovedDateField]),
                FirstApprovedBy = listItem.ToUserModel(StringConstant.OverTimeManagementList.FirstApprovedByField),
                Created = Convert.ToDateTime(listItem[StringConstant.DefaultSPListField.CreatedField]),
                Modified = Convert.ToDateTime(listItem[StringConstant.DefaultSPListField.ModifiedField]),
                RequestDueDate = Convert.ToDateTime(listItem[StringConstant.CommonSPListField.CommonReqDueDateField])
            };

            return OverTimeManagement;
        }

        public List<OverTimeManagement> GetByDepartmentInRange(int departmentId, int locationId, DateTime fromDate, DateTime toDate)
        {
            List<OverTimeManagement> overtimeManagementList = new List<OverTimeManagement>();

            string queryStr = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='CommonDepartment' LookupId='TRUE' />
                                            <Value Type='Lookup'>{departmentId}</Value>
                                        </Eq>
                                        <And>
                                            <Geq>
                                                <FieldRef Name='CommonDate' />
                                                <Value IncludeTimeValue='TRUE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                            </Geq>
                                                <And>
                                                <Leq>
                                                    <FieldRef Name='CommonDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                                </Leq>
                                                <Eq>
                                                    <FieldRef Name='CommonLocation' LookupId='TRUE'/>
                                                    <Value Type='Lookup'>{locationId}</Value>
                                                </Eq>
                                            </And>
                                        </And>
                                    </And>
                                </Where>";

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        var overtimeManagements = this.GetByQueryToSPListItemCollection(spWeb, queryStr);
                        if (overtimeManagements.Count > 0)
                        {
                            foreach (SPListItem overtimeItem in overtimeManagements)
                            {
                                var overtime = ParseToEntity(overtimeItem);
                                overtimeManagementList.Add(overtime);
                            }
                        }
                    }
                }
            }
            else
            {
                SPListItemCollection overtimeManagements = null;
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    overtimeManagements = this.GetByQueryToSPListItemCollection(currentWeb, queryStr);
                }
                else
                {
                    overtimeManagements = this.GetByQueryToSPListItemCollection(SPContext.Current.Site.RootWeb, queryStr);
                }

                if (overtimeManagements.Count > 0)
                {
                    foreach (SPListItem overtimeItem in overtimeManagements)
                    {
                        var overtime = ParseToEntity(overtimeItem);
                        overtimeManagementList.Add(overtime);
                    }
                }
            }

            return overtimeManagementList;
        }

        public void Approve(int overtimeManagementId, string approverFullName)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        // Attachment
                        web.AllowUnsafeUpdates = true;
                        SPList splist = web.GetList($"{web.Url}{ListUrl}");
                        SPListItem overTimeManagementItem = splist.GetItemById(overtimeManagementId);

                        if (overTimeManagementItem != null)
                        {
                            var overtimeManagementModel = ParseToEntity(overTimeManagementItem);
                            var overtimeManagmentDetail = _overTimeManagementDetailDAL.GetByOvertimeId(overtimeManagementId);
                            overtimeManagementModel.OverTimeManagementDetailList = overtimeManagmentDetail;
                            MemoryStream fileStream = WordHelper.CreateOverTimeWorkApplication(overtimeManagementModel);
                            if (fileStream != null && fileStream.Length > 0)
                            {
                                overTimeManagementItem.Attachments.Add("Giay de nghi lam ngoai gio.docx", fileStream.ToArray());
                                overTimeManagementItem.Update();
                            }

                            // Send email to employees
                            SendApproveEmailToEmployees(web.Url, overtimeManagementModel, overtimeManagmentDetail, approverFullName);
                            // Send email to Admin
                            SendApproveEmailToAdmin(web.Url, overtimeManagementModel, approverFullName);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        public void Reject(int overtimeId, string approverFullName)
        {
            string webUrl = SiteUrl;
            SendRejectEmailToEmployees(webUrl, overtimeId, approverFullName);
            SendRejectEmailToAdmin(webUrl, overtimeId, approverFullName);
        }

        //public void DepartmentHeadApprove(int overtimeId)
        //{
        //    string webUrl = SiteUrl;
        //    //SendApproveEmailToBOD
        //}

        #region Send_Mail
        private void SendApproveEmailToEmployees(string webUrl, OverTimeManagement overtimeManagementItem, List<OverTimeManagementDetail> overtimeManagementDetail, string approverFullName)
        {
            EmailTemplateDAL emailTemplateDAL = new EmailTemplateDAL(webUrl);
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(webUrl);
            SendEmailActivity sendMailActivity = new SendEmailActivity();

            var approveEmailItem = emailTemplateDAL.GetByKey("OvertimeManagement_Approve");

            if (overtimeManagementItem != null && approveEmailItem != null)
            {
                if (overtimeManagementDetail != null && overtimeManagementDetail.Count > 0)
                {
                    foreach (var item in overtimeManagementDetail)
                    {
                        string emailBody = HTTPUtility.HtmlDecode(approveEmailItem.MailBody);
                        var employee = employeeInfoDAL.GetByID(item.Employee.LookupId);
                        if (employee != null && !string.IsNullOrEmpty(employee.Email) && !string.IsNullOrEmpty(emailBody)) //only send mail if user has email
                        {
                            string link = string.Format("{0}/{1}", webUrl, StringConstant.WebPageLinks.OvertimeMember);
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                            {
                                link = string.Format("{0}/{1}", webUrl, StringConstant.WebPageLinks.OvertimeAdmin);
                            }
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                            {
                                link = string.Format("{0}/{1}", webUrl, StringConstant.WebPageLinks.OvertimeManager);
                            }
                            string timeString = string.Format("{0} {1}-{2}", overtimeManagementItem.CommonDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                                                              item.OvertimeFrom.ToString("HH:mm"),
                                                                              item.OvertimeTo.ToString("HH:mm"));
                            emailBody = string.Format(emailBody, employee.FullName, approverFullName, overtimeManagementItem.Requester.LookupValue,
                                                           timeString, item.Task);

                            emailBody = emailBody.Replace("#link", link);
                            sendMailActivity.SendMail(webUrl, approveEmailItem.MailSubject, employee.Email, true, false, emailBody);
                        }
                    }
                }
            }
        }

        private void SendApproveEmailToAdmin(string webUrl, OverTimeManagement item, string approverFullName)
        {
            EmailTemplateDAL emailTemplateDAL = new EmailTemplateDAL(webUrl);

            var approveEmailItem = emailTemplateDAL.GetByKey("OvertimeManagement_Approve_Requester");
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(webUrl);
            if (item != null && approveEmailItem != null)
            {
                var employee = employeeInfoDAL.GetByID(item.Requester.LookupId);
                if (employee != null && !string.IsNullOrEmpty(employee.Email) && !string.IsNullOrEmpty(approveEmailItem.MailBody)) //only send mail if user has email
                {
                    var department = DepartmentListSingleton.GetDepartmentByID(employee.Department.LookupId, webUrl);
                    string link = string.Format("{0}/SitePages/OvertimeRequest.aspx?subSection=OvertimeManagement&itemid={1}&mode=view&Source={0}/{2}", webUrl, item.ID, StringConstant.WebPageLinks.OvertimeAdmin);
                    string emailBody = HTTPUtility.HtmlDecode(approveEmailItem.MailBody);
                    emailBody = string.Format(emailBody, employee.FullName, approverFullName, item.CommonDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                                    item.CommonDepartment.LookupValue, department.VietnameseName);
                    emailBody = emailBody.Replace("#link", link);
                    sendMailActivity.SendMail(webUrl, approveEmailItem.MailSubject, employee.Email, true, false, emailBody);
                }
            }
        }

        private void SendRejectEmailToEmployees(string webUrl, int overtimeManagementId, string approverFullName)
        {
            EmailTemplateDAL emailTemplateDAL = new EmailTemplateDAL(webUrl);
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(webUrl);
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            OverTimeManagementDAL overtimeManagementDAL = new OverTimeManagementDAL(webUrl);
            OverTimeManagementDetailDAL overtimeDetailDAL = new OverTimeManagementDetailDAL(webUrl);

            var overtimeManagementItem = overtimeManagementDAL.GetByID(overtimeManagementId);
            var approveEmailItem = emailTemplateDAL.GetByKey("OvertimeManagement_Reject");

            if (overtimeManagementItem != null && approveEmailItem != null)
            {
                var overtimeManagementDetails = overtimeDetailDAL.GetByOvertimeId(overtimeManagementItem.ID);
                if (overtimeManagementDetails != null && overtimeManagementDetails.Count > 0)
                {

                    foreach (var item in overtimeManagementDetails)
                    {
                        string emailBody = HTTPUtility.HtmlDecode(approveEmailItem.MailBody);
                        var employee = employeeInfoDAL.GetByID(item.Employee.LookupId);
                        if (employee != null && !string.IsNullOrEmpty(employee.Email) && !string.IsNullOrEmpty(emailBody)) //only send mail if user has email
                        {
                            string link = string.Format("{0}/{1}", webUrl, StringConstant.WebPageLinks.OvertimeMember);
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                            {
                                link = string.Format("{0}/{1}", webUrl, StringConstant.WebPageLinks.OvertimeAdmin);
                            }
                            if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                            {
                                link = string.Format("{0}/{1}", webUrl, StringConstant.WebPageLinks.OvertimeManager);
                            }
                            string timeString = string.Format("{0} {1}-{2}", overtimeManagementItem.CommonDate.ToString(StringConstant.DateFormatddMMyyyy2),
                                                                              item.OvertimeFrom.ToString("HH:mm"),
                                                                              item.OvertimeTo.ToString("HH:mm"));
                            emailBody = string.Format(emailBody, employee.FullName, overtimeManagementItem.Requester.LookupValue,
                                                            item.Employee.LookupValue, approverFullName, timeString, item.Task);
                            emailBody = emailBody.Replace("#link", link);
                            sendMailActivity.SendMail(webUrl, approveEmailItem.MailSubject, employee.Email, true, false, emailBody);
                        }
                    }
                }
            }
        }

        private void SendRejectEmailToAdmin(string webUrl, int overtimeManagementId, string approverFullName)
        {
            EmailTemplateDAL emailTemplateDAL = new EmailTemplateDAL(webUrl);
            var approveEmailItem = emailTemplateDAL.GetByKey("OvertimeManagement_Reject_Requester");
            SendEmailActivity sendMailActivity = new SendEmailActivity();
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(webUrl);
            OverTimeManagementDAL overtimeManagementDAL = new OverTimeManagementDAL(webUrl);
            var item = overtimeManagementDAL.GetByID(overtimeManagementId);
            if (item != null && approveEmailItem != null)
            {
                var employee = employeeInfoDAL.GetByID(item.Requester.LookupId);
                string emailBody = HTTPUtility.HtmlDecode(approveEmailItem.MailBody);
                if (employee != null && !string.IsNullOrEmpty(employee.Email) && !string.IsNullOrEmpty(emailBody)) //only send mail if user has email
                {
                    var department = DepartmentListSingleton.GetDepartmentByID(employee.Department.LookupId, webUrl);
                    string link = string.Format("{0}/SitePages/OvertimeRequest.aspx?subSection=OvertimeManagement&itemid={1}&mode=view&Source={0}/{2}", webUrl, item.ID, StringConstant.WebPageLinks.OvertimeAdmin);
                    emailBody = string.Format(emailBody, employee.FullName, item.CommonDate.ToString(StringConstant.DateFormatddMMyyyy2), approverFullName,
                                                    item.CommonDepartment.LookupValue, department.VietnameseName);
                    emailBody = emailBody.Replace("#link", link);
                    sendMailActivity.SendMail(webUrl, approveEmailItem.MailSubject, employee.Email, true, false, emailBody);
                }
            }
        }

        public void SendEmailToApprover(SPWeb web, OverTimeManagement overtimeManagementItem, int approverType)
        {
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(web.Url);
            EmailTemplateDAL emailTemplateDAL = new EmailTemplateDAL(web.Url);
            SendEmailActivity sendMailActivity = new SendEmailActivity();

            if (overtimeManagementItem != null)
            {
                var departmentHead = employeeInfoDAL.GetByADAccount(overtimeManagementItem.ApprovedBy.ID);
                if (departmentHead != null)
                {
                    var requestEmailItem = emailTemplateDAL.GetByKey("OvertimeManagement_Request");
                    if (requestEmailItem != null && !string.IsNullOrEmpty(departmentHead.Email))
                    {
                        string link = string.Empty;
                        if (approverType == (int)StringConstant.EmployeePosition.DepartmentHead)
                        {
                            link = string.Format("{0}/SitePages/OverTimeApproval.aspx?itemid={1}&Source={0}/{2}", web.Url, overtimeManagementItem.ID, StringConstant.WebPageLinks.OvertimeManager);
                        }
                        else if (approverType == (int)StringConstant.EmployeePosition.BOD)
                        {
                            link = string.Format("{0}/SitePages/OverTimeApproval.aspx?itemid={1}&Source={0}/{2}", web.Url, overtimeManagementItem.ID, StringConstant.WebPageLinks.OvertimeBOD);
                        }

                        string emailBody = HTTPUtility.HtmlDecode(requestEmailItem.MailBody);
                        var department = DepartmentListSingleton.GetDepartmentByID(overtimeManagementItem.CommonDepartment.LookupId, web.Url);
                        emailBody = string.Format(emailBody, departmentHead.FullName, overtimeManagementItem.Requester.LookupValue, overtimeManagementItem.CommonDate.ToString(StringConstant.DateFormatddMMyyyy2), department.Name, department.VietnameseName);
                        emailBody = emailBody.Replace("#link", link);
                        sendMailActivity.SendMail(web.Url, requestEmailItem.MailSubject, departmentHead.Email, true, false, emailBody);
                    }
                }
            }
        }

        public void SendEmailToDelegatedApprover(SPWeb web, OverTimeManagement overtimeManagementItem)
        {
            EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(web.Url);
            EmailTemplateDAL emailTemplateDAL = new EmailTemplateDAL(web.Url);
            SendEmailActivity sendMailActivity = new SendEmailActivity();

            if (overtimeManagementItem != null)
            {
                var approver = employeeInfoDAL.GetByADAccount(overtimeManagementItem.ApprovedBy.ID);
                if (approver != null)
                {
                    var requestEmailItem = emailTemplateDAL.GetByKey("OvertimeManagement_Request");
                    if (requestEmailItem != null)
                    {
                        string link = string.Format(@"{0}/SitePages/OverTimeApproval.aspx?itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", web.Url, overtimeManagementItem.ID);
                        List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(web.Url, approver.ID, StringConstant.OverTimeManagementList.ListUrl, overtimeManagementItem.ID);
                        var department = DepartmentListSingleton.GetDepartmentByID(overtimeManagementItem.CommonDepartment.LookupId, web.Url);
                        foreach (var toUser in toUsers ?? Enumerable.Empty<EmployeeInfo>())
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(toUser.Email))
                                {
                                    string emailBody = HTTPUtility.HtmlDecode(requestEmailItem.MailBody);
                                    emailBody = string.Format(emailBody, toUser.FullName, overtimeManagementItem.Requester.LookupValue, overtimeManagementItem.CommonDate.ToString(StringConstant.DateFormatddMMyyyy2), department.Name, department.VietnameseName);
                                    emailBody = emailBody.Replace("#link", link);
                                    sendMailActivity.SendMail(web.Url, requestEmailItem.MailSubject, toUser.Email, true, false, emailBody);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        #endregion Send_Mail

        /// <summary>
        /// Get Overtime Approval List
        /// </summary>
        /// <param name="fromEmployee">Approver</param>
        /// <returns>List of Overtime Delegation</returns>
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> delegations = new List<Delegation>();

            var query = $@"<Where>
                                <And>
	                                <Eq>
		                                <FieldRef Name='CommonApprover1' LookupId='TRUE' />
		                                <Value Type='User'>{fromEmployee.ADAccount.ID}</Value>
	                                </Eq>
	                                <IsNull>
		                                <FieldRef Name='ApprovalStatus' />
	                                </IsNull>
                                </And>
                            </Where>";

            var overtimeManagementList = GetByQuery(query);
            if (overtimeManagementList != null && overtimeManagementList.Count > 0)
            {
                delegations = new List<Delegation>();
                foreach (var overtimeManagement in overtimeManagementList)
                {
                    var delegation = new Delegation(overtimeManagement);
                    delegations.Add(delegation);
                }
            }

            return delegations;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            OverTimeManagement overTimeManagement = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(overTimeManagement, currentWeb);
            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            SPFieldUser spuserField = (SPFieldUser)listItem.Fields.GetField(OverTimeManagementList.ApprovedByField);
            SPFieldUserValue spuserFieldValue = (SPFieldUserValue)spuserField.GetFieldValue(Convert.ToString(listItem[OverTimeManagementList.ApprovedByField]));
            if (spuserFieldValue != null)
            {
                EmployeeInfo approver = _employeeInfoDAL.GetByADAccount(spuserFieldValue.LookupId);
                string approvalStatus = listItem[listItem.Fields.GetField(CommonSPListField.ApprovalStatusField).Id] + string.Empty;
                approvalStatus = approvalStatus.ToLower();
                if (approver != null && (approvalStatus != "true" && approvalStatus != "false"))
                {
                    return new LookupItem { LookupId = approver.ID, LookupValue = approver.FullName };
                }
            }

            return null;
        }

        public bool IsValidTask(int listItemID)
        {
            return true;
        }

        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        public OverTimeManagement SetDueDate(OverTimeManagement overTimeManagement)
        {
            DateTime reqDueDate = overTimeManagement.CommonDate.Date;
            //if (reqDueDate == DateTime.Now.Date)
            //{
            //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            //else
            //{
            //    reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            //}
            reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            overTimeManagement.RequestDueDate = reqDueDate;

            return overTimeManagement;
        }
    }
}
