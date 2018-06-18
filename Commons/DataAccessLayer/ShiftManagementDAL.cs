using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Constants;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using System;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Core.SharePoint;
using System.Reflection;
using System.Linq;
using RBVH.Stada.Intranet.Biz.DTO;
using System.Text;
using System.Collections.ObjectModel;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class ShiftManagementDAL : BaseDAL<ShiftManagement>, IDelegationManager, IFilterTaskManager
    {
        private EmployeeInfoDAL _employeeInfoDAL;
        public ShiftManagementDAL(string siteUrl) : base(siteUrl)
        {
            ListUrl = "/Lists/ShiftManagement";
            _employeeInfoDAL = new EmployeeInfoDAL(SiteUrl);
        }

        public List<ShiftManagement> GetByMonthYear(int requesterId, int month, int year)
        {
            List<ShiftManagement> shiftManagementList = new List<ShiftManagement>();

            var query = $@"<Where><And><Eq><FieldRef Name='Requester' LookupId='TRUE' /><Value Type='Lookup'>{requesterId}</Value></Eq><And><Eq><FieldRef Name='CommonMonth' /><Value Type='Number'>{month}</Value></Eq><And><Eq><FieldRef Name='CommonYear' /><Value Type='Number'>{year}</Value></Eq></And></And></And></Where>";
            shiftManagementList = GetByQuery(query);

            return shiftManagementList;
        }

        public List<ShiftManagement> GetByMonthYear(int month, int year)
        {
            List<ShiftManagement> shiftManagementList = new List<ShiftManagement>();

            var query = $@"<Where><And><Eq><FieldRef Name='CommonMonth' /><Value Type='Number'>{month}</Value></Eq><Eq><FieldRef Name='CommonYear' /><Value Type='Number'>{year}</Value></Eq></And></Where>";
            shiftManagementList = GetByQuery(query);

            return shiftManagementList;
        }

        public List<ShiftManagement> GetByMonthYearDepartment(int month, int year, int departmentId, int locationId)
        {
            List<ShiftManagement> shiftManagementList = new List<ShiftManagement>();

            string query =
                  $@"<Where>
                        <And>
                            <Eq>
                            <FieldRef Name='{ShiftManagementList.MonthField}' />
                            <Value Type='Number'>{month}</Value>
                            </Eq>
                            <And>
                            <Eq>
                                <FieldRef Name='{ShiftManagementList.YearField}' />
                                <Value Type='Number'>{year}</Value>
                            </Eq>
                            <And>
                                <Eq>
                                    <FieldRef LookupId='TRUE' Name='{ShiftManagementList.LocationField}' />
                                    <Value Type='Lookup'>{locationId}</Value>
                                </Eq>
                                <Eq>
                                    <FieldRef LookupId='TRUE' Name='{ShiftManagementList.DepartmentField}' />
                                    <Value Type='Lookup'>{departmentId}</Value>
                                </Eq>
                            </And>
                            </And>
                        </And>
                    </Where>";

            shiftManagementList = GetByQuery(query);

            return shiftManagementList;
        }

        public List<ShiftManagement> GetByDateRangeDepartment(DateTime fromDate, DateTime toDate, int departmentId, int locationId)
        {
            List<ShiftManagement> shiftCollection = new List<ShiftManagement>();

            Dictionary<string, Collection<DateTime>> monthYearDict = BuildMonthYearForShift(fromDate, toDate);
            if (monthYearDict != null)
            {
                foreach (var monthYear in monthYearDict)
                {
                    string[] monthYearArr = monthYear.Key.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    var shifts = GetByMonthYearDepartment(Convert.ToInt32(monthYearArr[0]), Convert.ToInt32(monthYearArr[1]), departmentId, locationId);
                    if (shifts != null && shifts.Count() > 0)
                    {
                        foreach (var shift in shifts)
                        {
                            if (!shiftCollection.Where(e => e.ID == shift.ID).Any())
                            {
                                shiftCollection.Add(shift);
                            }
                        }
                    }
                }
            }

            return shiftCollection;
        }

        private Dictionary<string, Collection<DateTime>> BuildMonthYearForShift(DateTime fromDate, DateTime toDate)
        {
            Dictionary<string, Collection<DateTime>> monthYearDict = new Dictionary<string, Collection<DateTime>>();

            for (DateTime dateObj = fromDate.Date; dateObj.Date <= toDate.Date; dateObj = dateObj.AddDays(1))
            {
                string monthYearVal = "";
                if (dateObj.Day >= 21)
                {
                    DateTime tmp = dateObj.AddMonths(1);
                    monthYearVal = string.Format("{0}#{1}", tmp.Month, tmp.Year);
                }
                else
                {
                    monthYearVal = string.Format("{0}#{1}", dateObj.Month, dateObj.Year);
                }

                if (!monthYearDict.ContainsKey(monthYearVal))
                {
                    monthYearDict.Add(monthYearVal, new Collection<DateTime>() { dateObj });
                }
                else
                {
                    monthYearDict[monthYearVal].Add(dateObj);
                }
            }

            return monthYearDict;
        }

        public ShiftManagement SetDueDate(ShiftManagement shiftManagement)
        {
            shiftManagement.RequestDueDate = new DateTime(shiftManagement.Year, shiftManagement.Month, 19);
            return shiftManagement;
        }

        public int SaveOrUpdate(ShiftManagement newItem)
        {
            int returnId = 0;
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList splist = web.GetList($"{web.Url}{ListUrl}");
                    SPListItem spListItem;
                    if (newItem.ID > 0)
                    {
                        spListItem = splist.GetItemById(newItem.ID);
                        if (spListItem != null)
                        {
                            spListItem["Modified"] = System.DateTime.Now.ToString(StringConstant.DateFormatTZForCAML);

                            //spListItem[StringConstant.ShiftManagementList.CommonAddApprover1Field] = ConvertMultUser(newItem.CommonAddApprover1, web);
                        }
                    }
                    else
                    {
                        spListItem = splist.AddItem();
                        spListItem[StringConstant.ShiftManagementList.MonthField] = newItem.Month;
                        spListItem[StringConstant.ShiftManagementList.YearField] = newItem.Year;
                        spListItem[StringConstant.CommonSPListField.ApprovalStatusField] = newItem.ApprovalStatus;
                        var requester = _employeeInfoDAL.GetByADAccount(web.CurrentUser.ID);
                        var requesterLookupId = newItem.Requester.LookupId == 0 ? requester.ID : newItem.Requester.LookupId;
                        spListItem[StringConstant.CommonSPListField.RequesterField] = requesterLookupId;
                        spListItem[StringConstant.CommonSPListField.CommonDepartmentField] = newItem.Department.LookupId;
                        spListItem[StringConstant.CommonSPListField.CommonLocationField] = newItem.Location.LookupId;

                        SPUser approver = SPContext.Current.Web.EnsureUser(newItem.ApprovedBy.UserName);
                        SPFieldUserValue approverValue = new SPFieldUserValue(SPContext.Current.Web, approver.ID, approver.LoginName);
                        spListItem[StringConstant.ShiftManagementList.ApprovedByField] = approverValue;

                        if (newItem.RequestDueDate != null && newItem.RequestDueDate != default(DateTime))
                        {
                            spListItem[StringConstant.CommonSPListField.CommonReqDueDateField] = newItem.RequestDueDate;
                        }

                        //spListItem[StringConstant.ShiftManagementList.CommonAddApprover1Field] = ConvertMultUser(newItem.CommonAddApprover1, web);
                    }

                    spListItem.Update();
                    returnId = spListItem.ID;
                    web.AllowUnsafeUpdates = false;
                }
            }

            return returnId;
        }

        public void UpdateApprover(int itemId, List<User> commonAddApprover1)
        {
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList splist = web.GetList($"{web.Url}{ListUrl}");
                    SPListItem spListItem;
                    if (itemId > 0)
                    {
                        spListItem = splist.GetItemById(itemId);
                        if (spListItem != null)
                        {
                            spListItem["Modified"] = System.DateTime.Now.ToString(StringConstant.DateFormatTZForCAML);
                            spListItem[StringConstant.ShiftManagementList.CommonAddApprover1Field] = ConvertMultUser(commonAddApprover1, web);
                            spListItem.Update();
                        }
                    }

                    web.AllowUnsafeUpdates = false;
                }
            }
        }

        /// <summary>
        /// Get Shift Approval List
        /// </summary>
        /// <param name="fromEmployee">Approver</param>
        /// <param name="year">Current year</param>
        /// <param name="month">Current month</param>
        /// <returns>List of Shift Delegation</returns>
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> delegations = new List<Delegation>();

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            var query = $@"<Where>
                                <And>
                                    <Or>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='CommonYear' />
                                                <Value Type='Number'>{year}</Value>
                                            </Eq>
                                            <Geq>
                                                <FieldRef Name='CommonMonth' />
                                                <Value Type='Number'>{month}</Value>
                                            </Geq>
                                        </And>
                                        <Gt>
                                            <FieldRef Name='CommonYear' />
                                            <Value Type='Number'>{year}</Value>
                                        </Gt>
                                    </Or>
                                    <Eq>
                                        <FieldRef Name='CommonApprover1' LookupId='TRUE' />
                                        <Value Type='User'>{fromEmployee.ADAccount.ID}</Value>
                                    </Eq>
                                </And>
                            </Where>";

            var shiftManagementList = GetByQuery(query);
            if (shiftManagementList != null && shiftManagementList.Count > 0)
            {
                delegations = new List<Delegation>();
                var isValid = false;
                foreach (var shiftManagement in shiftManagementList)
                {
                    isValid = true;
                    if (shiftManagement.Year == year)
                    {
                        if (shiftManagement.Month == month) // Current Day <= 20
                        {
                            if (day > 20)
                                isValid = false;
                            else if (shiftManagement.Month - 1 == month) // Current Day >= 21
                            {
                                if (day < 21)
                                    isValid = false;
                            }
                        }
                    }
                    else if (year == shiftManagement.Year - 1)
                    {
                        if (month == 12) // 21/12/2016 -> 20/1/2017
                        {
                            if (day < 21)
                                isValid = false;
                        }
                    }

                    if (isValid)
                    {
                        if (!IsShiftApproved(shiftManagement.ID))
                        {
                            var delegation = new Delegation(shiftManagement);
                            delegations.Add(delegation);
                        }
                    }
                }
            }

            return delegations;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            ShiftManagement shiftManagement = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(shiftManagement, currentWeb);
            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            SPFieldUser spuserField = (SPFieldUser)listItem.Fields.GetField(ShiftManagementList.ApprovedByField);
            SPFieldUserValue spuserFieldValue = (SPFieldUserValue)spuserField.GetFieldValue(Convert.ToString(listItem[ShiftManagementList.ApprovedByField]));
            if (spuserFieldValue != null)
            {
                EmployeeInfo approver = _employeeInfoDAL.GetByADAccount(spuserFieldValue.LookupId);

                string approvalStatus = listItem[listItem.Fields.GetField(CommonSPListField.ApprovalStatusField).Id] + string.Empty;
                if (approver != null && IsShiftApproved(listItem.ID) == false)
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

        private bool IsShiftApproved(int shiftId)
        {
            bool ret = true;

            ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(SiteUrl);
            List<ShiftManagementDetail> shiftManagementDetails = _shiftManagementDetailDAL.GetByShiftManagementID(shiftId);
            if (shiftManagementDetails != null && shiftManagementDetails.Count > 0)
            {
                ShiftTimeDAL _shiftTimeDAL = new ShiftTimeDAL(SiteUrl);
                List<Biz.Models.ShiftTime> shiftTimes = _shiftTimeDAL.GetShiftTimes();
                List<string> shiftCodeList = shiftTimes.Where(e => e.ShiftRequired == true).Select(e => e.Code.ToUpper()).ToList();

                Type typeShiftManagementDetail = typeof(ShiftManagementDetail);
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                foreach (var shiftManagementDetail in shiftManagementDetails)
                {
                    if (ret == false)
                    {
                        break;
                    }

                    for (int idx = 1; idx <= 31; idx++)
                    {
                        PropertyInfo shiftTimeInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", idx), bindingFlags);
                        object shiftTimeValue = shiftTimeInfo.GetValue(shiftManagementDetail, null);

                        if (shiftTimeValue != null)
                        {
                            LookupItem shiftTimeValueObj = shiftTimeValue as LookupItem;
                            if (!string.IsNullOrEmpty(shiftTimeValueObj.LookupValue) && shiftCodeList.Contains(shiftTimeValueObj.LookupValue))
                            {
                                PropertyInfo shiftTimeApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", idx), bindingFlags);
                                object shiftTimeApprovalValue = shiftTimeApprovalInfo.GetValue(shiftManagementDetail, null);

                                if (shiftTimeApprovalValue != null && Convert.ToBoolean(shiftTimeApprovalValue) == true)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// VISITOR
        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
