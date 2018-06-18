using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.BusinessLayer;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Helper;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.Calendar;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.Overtime;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.ShiftManagement
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ShiftManagementService : IShiftManagementService
    {
        private readonly ShiftManagementDAL _shiftManagementDAL;
        private readonly ShiftManagementDetailDAL _shiftManagementDetailDAL;
        private readonly DepartmentDAL _departmentDAL;
        private readonly EmployeeInfoDAL _employeeInfoDAL;
        private readonly CalendarService CalendarService;
        private readonly LeaveManagementService _leaveManagementService;
        private readonly ShiftTimeDAL _shiftTimeDAL;
        private readonly OverTimeManagementDAL _overTimeManagementDAL;
        private readonly OverTimeManagementDetailDAL _overTimeManagementDetailDAL;
        private readonly OvertimeService OvertimeService;
        private readonly EmailTemplateDAL emailTemplateDAL;
        private readonly ChangeShiftManagementDAL changeShiftManagementDAL;
        private const int MAX_LEVEL = 4; // Văn thư(3) -> Nhân viên(2) -> Nhân Viên Cây Xanh(1) -> Bảo vệ(1) -> Tạp vụ(1)
                                         //private readonly OvertimeRepository _overtimeRepository;

        public ShiftManagementService()
        {
            var webUrl = SPContext.Current.Web.Url;
            _shiftManagementDAL = new ShiftManagementDAL(webUrl);
            _shiftManagementDetailDAL = new ShiftManagementDetailDAL(webUrl);
            _departmentDAL = new DepartmentDAL(webUrl);
            _employeeInfoDAL = new EmployeeInfoDAL(webUrl);
            CalendarService = new CalendarService();
            _shiftTimeDAL = new ShiftTimeDAL(webUrl);
            OvertimeService = new OvertimeService();
            _overTimeManagementDAL = new OverTimeManagementDAL(webUrl);
            _overTimeManagementDetailDAL = new OverTimeManagementDetailDAL(webUrl);
            changeShiftManagementDAL = new ChangeShiftManagementDAL(webUrl);
            emailTemplateDAL = new EmailTemplateDAL(webUrl);
            //_overtimeRepository = new OvertimeRepository(webUrl);
            _leaveManagementService = new LeaveManagementService();
        }

        public ShiftManagementModel GetByPreviousShift(GetShiftManagementRequest getShiftManagementRequest)
        {
            try
            {
                int month = int.Parse(getShiftManagementRequest.Month);
                int year = int.Parse(getShiftManagementRequest.Year);
                int departmentId = int.Parse(getShiftManagementRequest.DepartmentId);
                int locationId = int.Parse(getShiftManagementRequest.LocationId);
                var shiftManagementList = _shiftManagementDAL.GetByMonthYearDepartment(month, year, departmentId, locationId);
                var shiftManagement = shiftManagementList.FirstOrDefault();

                if (shiftManagement != null)
                {
                    return GetShiftManagementById(shiftManagement.ID.ToString());
                }
                else
                {
                    var department = _departmentDAL.GetByID(departmentId);
                    var employees = _employeeInfoDAL.GetByLocationAndDepartment(locationId, departmentId, true, MAX_LEVEL, StringConstant.EmployeeInfoList.EmployeeIDField);
                    var ShiftManagementModel = new ShiftManagementModel()
                    {
                        Department = new DepartmentInfo
                        {
                            DepartmentName = department.Name,
                            Id = department.ID
                        },
                        Month = month,
                        Year = year,
                    };
                    foreach (var employee in employees)
                    {
                        //ShiftManagementDetailModel detail = new ShiftManagementDetailModel(employee, month, year);
                        ShiftManagementDetailModel detail = new ShiftManagementDetailModel();
                        detail.Employee.FullName = employee.FullName;
                        detail.Employee.Id = employee.ID;
                        detail.Employee.EmployeeId = employee.EmployeeID;
                        ShiftManagementModel.ShiftManagementDetailModelList.Add(detail);
                    }
                    return ShiftManagementModel;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - GetByPreviousShift fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public ShiftManagementModel GetShiftManagementById(string Id)
        {
            try
            {
                var shiftManagementModel = new ShiftManagementModel();
                var id = int.Parse(Id);
                var shiftManagement = _shiftManagementDAL.GetByID(id);
                // Validate input:
                if (shiftManagement.Department == null || shiftManagement == null)
                    return shiftManagementModel;

                var departmentId = shiftManagement.Department.LookupId;
                var locationId = shiftManagement.Location.LookupId;
                var employees = _employeeInfoDAL.GetByLocationAndDepartment(locationId, departmentId, true, MAX_LEVEL, StringConstant.EmployeeInfoList.EmployeeIDField);

                // Get manager list
                Dictionary<int, string> managerList = employees.Select(e => new { Id = e.ID, ManagerName = e.Manager != null ? e.Manager.LookupValue : string.Empty }).ToDictionary(e => e.Id, e => e.ManagerName);

                shiftManagementModel = new ShiftManagementModel()
                {
                    ApprovalStatus = shiftManagement.ApprovalStatus,
                    Department = new DepartmentInfo
                    {
                        DepartmentName = shiftManagement.Department.LookupValue,
                        Id = shiftManagement.Department.LookupId
                    },
                    Id = shiftManagement.ID,
                    Month = shiftManagement.Month,
                    Year = shiftManagement.Year,
                    Location = shiftManagement.Location,
                    Requester = shiftManagement.Requester,
                    ApprovedBy = shiftManagement.ApprovedBy,
                    AdditionalUser = shiftManagement.CommonAddApprover1,
                    ModifiedBy = shiftManagement.ModifiedBy
                };

                if (shiftManagement.RequestDueDate != null && shiftManagement.RequestDueDate != default(DateTime))
                {
                    shiftManagementModel.RequestDueDate = shiftManagement.RequestDueDate.ToString(StringConstant.DateFormatddMMyyyy2);
                    if (shiftManagement.RequestDueDate.Date < DateTime.Now.Date)
                    {
                        //shiftManagementModel.RequestExpired = true;
                        shiftManagementModel.RequestExpired = false;
                    }
                }

                var shiftManagementDetails = _shiftManagementDetailDAL.GetByShiftManagementID(shiftManagement.ID);
                var employeesInShift = shiftManagementDetails.Select(x => x.Employee.LookupId).ToList();

                for (int i = shiftManagementDetails.Count - 1; i >= 0; i--)
                {
                    var empInShiftId = shiftManagementDetails[i].Employee.LookupId;
                    var employeeInShift = employees.FirstOrDefault(x => x.ID == empInShiftId);
                    if (employeeInShift != null)
                    {
                        shiftManagementDetails[i].EmployeeID = new LookupItem
                        {
                            LookupId = employeeInShift.ID,
                            LookupValue = employeeInShift.EmployeeID
                        };
                    }
                    else
                        shiftManagementDetails.RemoveAt(i);
                }

                var employeesCanBeInShift = employees.Select(x => x.ID).ToList();
                var employeeIDsNotInShift = employeesCanBeInShift.Except(employeesInShift).ToList();
                var employeeListTobeInShift = new List<EmployeeInfo>();

                if (employeeIDsNotInShift != null && employeeIDsNotInShift.Count() > 0)
                {
                    var employeesNotInShift = employeesCanBeInShift.Except(employeesInShift);
                    employeeListTobeInShift = employees.Where(x => employeeIDsNotInShift.Contains(x.ID)).ToList();
                }

                var shiftDetailModel = new ShiftManagementDetailModel();
                shiftManagementModel.ShiftManagementDetailModelList = shiftDetailModel.FromEntitieAndEmployees(shiftManagementDetails, employeeListTobeInShift, shiftManagement.Month, shiftManagement.Year, managerList);

                shiftManagementModel.ShiftManagementDetailModelList = shiftManagementModel.ShiftManagementDetailModelList.OrderBy(e => e.Employee.EmployeeId).ToList();
                //shiftManagementModel.ShiftManagementDetailModelList = shiftManagementModel.ShiftManagementDetailModelList.OrderBy(e => e.Employee.ManagerName).ThenBy(e => e.Employee.EmployeeId).ToList();
                return shiftManagementModel;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - GetShiftManagementById fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public MessageResult UpdateShiftManagement(ShiftManagementModel getShiftManagementRequest)
        {
            try
            {
                var shiftManagementID = getShiftManagementRequest.Id;
                if (shiftManagementID > 0)
                {
                    var ShiftManagementDetail = new List<ShiftManagementDetail>();
                    List<ShiftManagementDetail> entityDetailList = new List<ShiftManagementDetail>();
                    foreach (var shiftManagementDetailModel in getShiftManagementRequest.ShiftManagementDetailModelList)
                    {
                        shiftManagementDetailModel.ShiftManagementID = getShiftManagementRequest.Id;
                        var shiftEntity = shiftManagementDetailModel.ToEntity();
                        entityDetailList.Add(shiftEntity);
                    }
                    if (entityDetailList != null && entityDetailList.Count > 0)
                    {
                        _shiftManagementDetailDAL.SaveItems(entityDetailList);
                    }
                }
                return new MessageResult
                {
                    Code = 0,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - UpdateShiftManagement fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return new MessageResult
                {
                    Code = 1,
                    Message = ex.Message
                };
            }
        }

        private void SendMailToEmployee(string webUrl, int shiftManagementDetailId, int employeeId, int month, int year, string approverFullName)
        {
            SendEmailActivity sendMailActity = new SendEmailActivity();
            var approveEmailItem = emailTemplateDAL.GetByKey("ShiftManagement_Approve");
            var employeeDal = new EmployeeInfoDAL(webUrl);

            Thread thread = new Thread(delegate ()
            {
                var shiftDetailItem = _shiftManagementDetailDAL.GetById(shiftManagementDetailId);
                if (approveEmailItem != null)
                {
                    string emailBody = HTTPUtility.HtmlDecode(approveEmailItem.MailBody);
                    var employee = employeeDal.GetByID(employeeId);
                    if (employee != null && !string.IsNullOrEmpty(employee.Email))
                    {
                        string link = string.Format("{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementMember.aspx", webUrl);
                        if (employee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                        {
                            link = string.Format("{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementAdmin.aspx", webUrl);
                        }
                        string shiftTableHtml = GetShiftTable(webUrl, shiftDetailItem);
                        emailBody = string.Format(emailBody, employee.FullName, approverFullName, employee.FullName, month, year, shiftTableHtml);

                        //emailBody = string.Format(emailBody, employee.FullName, approverFullName, employee.FullName, month, year, shiftTableHtml);
                        emailBody = emailBody.Replace("#link", link);
                        sendMailActity.SendMail(webUrl, approveEmailItem.MailSubject, employee.Email, true, false, emailBody, false);
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public MessageResult ApproveShiftManagementDetail(ShiftManagementModel getShiftManagementRequest)
        {
            string webUrl = SPContext.Current.Web.Url;
            try
            {
                Biz.Models.ShiftManagement shiftManagementObj = _shiftManagementDAL.GetByID(getShiftManagementRequest.Id);
                string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(shiftManagementObj.RequestDueDate);
                if (!string.IsNullOrEmpty(requestExpiredMsg))
                {
                    return new MessageResult { Code = 2, Message = requestExpiredMsg, ObjectId = 0 };
                }

                List<Biz.Models.ShiftTime> allShiftTimes = this._shiftTimeDAL.GetAll();

                List<ShiftManagementDetail> shiftEntities = new List<ShiftManagementDetail>();
                List<OverTimeModel> overTimeModels = new List<OverTimeModel>();

                var departmentId = getShiftManagementRequest.Department.Id;
                var locationId = getShiftManagementRequest.Location.LookupId;

                var empMonth = getShiftManagementRequest.Month;
                var empYear = getShiftManagementRequest.Year;

                var shiftModifiedBy = _employeeInfoDAL.GetByADAccount(getShiftManagementRequest.ModifiedByString);
                var requester = getShiftManagementRequest.Requester;
                if (!string.IsNullOrEmpty(getShiftManagementRequest.ModifiedByString))
                {
                    requester = new LookupItem
                    {
                        LookupId = shiftModifiedBy.ID,
                        LookupValue = shiftModifiedBy.FullName
                    };
                }

                var departmentObj = _departmentDAL.GetById(departmentId);
                foreach (var shiftManagementDetailModel in getShiftManagementRequest.ShiftManagementDetailModelList)
                {
                    shiftManagementDetailModel.ShiftManagementID = getShiftManagementRequest.Id;
                    shiftEntities.Add(shiftManagementDetailModel.ToEntity());

                    if (departmentObj.AutoCreateOverTime == false)
                    {
                        continue;
                    }

                    foreach (var dayInfo in shiftManagementDetailModel.ApprovalDays)
                    {
                        var empDay = dayInfo.Day;

                        var empDate = DateTime.Now;
                        if (empDay >= 21 && empDay <= 31)
                        {
                            empDate = new DateTime(empYear, empMonth, 1).AddMonths(-1).AddDays(empDay - 1);
                        }
                        else
                        {
                            empDate = new DateTime(empYear, empMonth, empDay);
                        }

                        // Step 1: Check employee has overtime or not (Approve + In-progress)
                        var overtimeEmployees = this._overTimeManagementDetailDAL.GetOvertimeEmployeeByDate(shiftManagementDetailModel.Employee.Id, empDate);
                        if (!overtimeEmployees.Any())
                        {
                            // Step 2: Check has Overtime MASTER on date or not:
                            OverTimeModel overTimeModel = new OverTimeModel();
                            List<OverTimeManagement> overtimeInprogress = new List<OverTimeManagement>();

                            var existedOverTimeModel = overTimeModels.Where(e => e.Requester.LookupId == requester.LookupId && e.CommonDepartment.LookupId == departmentId
                                && e.CommonLocation.LookupId == locationId && e.Date.ToLower() == empDate.Date.ToString().ToLower()).FirstOrDefault();

                            if (existedOverTimeModel == null)
                            {
                                overtimeInprogress = this._overTimeManagementDAL.GetByApprovalStatus(empDate, departmentId, locationId, StringConstant.EnumApprovalStatus.InProgress);
                            }

                            if (existedOverTimeModel != null)
                            {
                                overTimeModel = existedOverTimeModel;

                                overTimeModel.SumOfEmployee = Convert.ToInt32(overTimeModel.SumOfEmployee) + 1 + "";
                                overTimeModel.SumOfMeal = Convert.ToInt32(overTimeModel.SumOfMeal) + 1 + "";
                            }
                            else if (overtimeInprogress.Any())
                            {
                                // Append emp to Overtime MASTER:
                                var overtimeMaster = overtimeInprogress.First();

                                overTimeModel.ID = overtimeMaster.ID;
                                overTimeModel.CommonDepartment = overtimeMaster.CommonDepartment;
                                overTimeModel.CommonLocation.LookupId = overtimeMaster.CommonLocation.LookupId;
                                overTimeModel.Requester = overtimeMaster.Requester;
                                overTimeModel.ApprovedBy = overtimeMaster.ApprovedBy;
                                overTimeModel.ApprovalStatus = overtimeMaster.ApprovalStatus;
                                overTimeModel.Date = overtimeMaster.CommonDate.ToString();
                                overTimeModel.SumOfEmployee = overtimeMaster.SumOfEmployee + 1 + "";
                                overTimeModel.SumOfMeal = overtimeMaster.SumOfMeal + 1 + "";
                                overTimeModel.OtherRequirements = overtimeMaster.OtherRequirements;
                            }
                            else
                            {
                                // Create new Overtime
                                overTimeModel.CommonDepartment.LookupId = getShiftManagementRequest.Department.Id;
                                overTimeModel.CommonLocation = getShiftManagementRequest.Location;
                                overTimeModel.Requester = requester;
                                overTimeModel.ApprovedBy = getShiftManagementRequest.ApprovedBy;
                                overTimeModel.Date = empDate.ToString();
                                overTimeModel.SumOfEmployee = "1";
                                overTimeModel.SumOfMeal = "1";
                            }

                            // CREATE overtime detail
                            var overTimeDetail = new OvertimeDetailModel();
                            overTimeDetail.Employee.LookupId = shiftManagementDetailModel.Employee.Id;

                            var shift = allShiftTimes.Where(e => e.ID == dayInfo.ShiftTimeId).FirstOrDefault();
                            overTimeDetail.OvertimeFrom = empDate.AddHours(shift.WorkingHourFromHour.Hour).AddMinutes(shift.WorkingHourFromHour.Minute).AddSeconds(shift.WorkingHourFromHour.Second).ToString();

                            var overtimeToDay = empDate.AddHours(shift.WorkingHourToHour.Hour).AddMinutes(shift.WorkingHourToHour.Minute).AddSeconds(shift.WorkingHourToHour.Second);
                            if ((shift.WorkingHourFromHour.Hour > 12 && shift.WorkingHourToHour.Hour < 12) || shift.ShiftTimeWorkingHourNumber > 24) // Next day
                            {
                                overtimeToDay = overtimeToDay.AddDays(1);
                            }
                            overTimeDetail.OvertimeTo = overtimeToDay.ToString();
                            overTimeDetail.WorkingHours = Convert.ToString(shift.ShiftTimeWorkingHourNumber);
                            overTimeDetail.Day = empDay;
                            overTimeDetail.CompanyTransport = "Tự túc";

                            if (overTimeModel.OvertimeDetailModelList == null)
                            {
                                overTimeModel.OvertimeDetailModelList = new List<OvertimeDetailModel>();
                            }

                            overTimeModel.OvertimeDetailModelList.Add(overTimeDetail);

                            if (!overTimeModels.Where(e => e.Requester.LookupId == overTimeModel.Requester.LookupId && e.CommonDepartment.LookupId == overTimeModel.CommonDepartment.LookupId
                                && e.CommonLocation.LookupId == overTimeModel.CommonLocation.LookupId && e.Date.ToLower() == overTimeModel.Date.ToLower()).Any())
                            {
                                overTimeModels.Add(overTimeModel);
                            }
                        }
                    }
                }

                // Step 1: Approve shift
                this._shiftManagementDetailDAL.Approve(shiftEntities);

                // 28.11.2017: Update latest approver:
                this._shiftManagementDAL.UpdateApprover(getShiftManagementRequest.Id, getShiftManagementRequest.AdditionalUser);

                foreach (var shiftManagementDetailModel in getShiftManagementRequest.ShiftManagementDetailModelList)
                {
                    if (shiftManagementDetailModel.NewApproval)
                        SendMailToEmployee(webUrl, shiftManagementDetailModel.Id, shiftManagementDetailModel.Employee.Id, getShiftManagementRequest.Month, getShiftManagementRequest.Year, getShiftManagementRequest.ApproverFullName);
                }

                if (overTimeModels != null && overTimeModels.Count > 0)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate
                    {
                        using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                foreach (var overTimeModel in overTimeModels)
                                {
                                    this.OvertimeService.InsertOvertime(web, overTimeModel, true);
                                }
                            }
                        }
                    });
                }

                return new MessageResult
                {
                    Code = 0,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - ApproveShiftManagementDetail fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return new MessageResult
                {
                    Code = 1,
                    Message = ex.Message
                };
            }
        }

        //public MessageResult ApproveShiftManagementDetail(ShiftManagementModel getShiftManagementRequest)
        //{
        //    string webUrl = SPContext.Current.Web.Url;
        //    try
        //    {
        //        // Step 1: Approve shift
        //        foreach (var shiftManagementDetailModel in getShiftManagementRequest.ShiftManagementDetailModelList)
        //        {
        //            shiftManagementDetailModel.ShiftManagementID = getShiftManagementRequest.Id;
        //            var shiftEntity = shiftManagementDetailModel.ToEntity();
        //            this._shiftManagementDetailDAL.Approve(shiftEntity);

        //            // 28.11.2017: Update latest approver:
        //            this._shiftManagementDAL.UpdateApprover(getShiftManagementRequest.Id, getShiftManagementRequest.AdditionalUser);

        //            if (shiftManagementDetailModel.NewApproval)
        //                SendMailToEmployee(webUrl, shiftManagementDetailModel.Id, shiftManagementDetailModel.Employee.Id, getShiftManagementRequest.Month, getShiftManagementRequest.Year, getShiftManagementRequest.ApproverFullName);

        //            var departmentId = getShiftManagementRequest.Department.Id;
        //            var locationId = getShiftManagementRequest.Location.LookupId;
        //            foreach (var dayInfo in shiftManagementDetailModel.ApprovalDays)
        //            {
        //                var empMonth = getShiftManagementRequest.Month;
        //                var empYear = getShiftManagementRequest.Year;
        //                var empDay = dayInfo.Day;
        //                var empDate = new DateTime(empYear, empMonth, empDay);
        //                if (empDay >= 21 && empDay <= 31) // Month --
        //                {
        //                    empDate = empDate.AddMonths(-1);
        //                }
        //                // Step 1: Check employee has overtime or not (Approve + In-progress)
        //                var overtimeEmployees = this._overTimeManagementDetailDAL.GetOvertimeEmployeeByDate(shiftManagementDetailModel.Employee.Id, empDate);
        //                if (!overtimeEmployees.Any())
        //                {
        //                    // Step 2: Check has Overtime MASTER on date or not:
        //                    var overtimeInprogress = this._overTimeManagementDAL.GetByApprovalStatus(empDate, departmentId, locationId, StringConstant.EnumApprovalStatus.InProgress);
        //                    var overTimeModel = new OverTimeModel();

        //                    // CREATE overtime detail
        //                    var overTimeDetail = new OvertimeDetailModel();
        //                    overTimeDetail.Employee.LookupId = shiftManagementDetailModel.Employee.Id;
        //                    // Get Shift time info
        //                    var shift = this._shiftTimeDAL.GetByID(dayInfo.ShiftTimeId);
        //                    overTimeDetail.OvertimeFrom = new DateTime(empYear, empMonth, empDay, shift.WorkingHourFromHour.Hour, shift.WorkingHourFromHour.Minute, shift.WorkingHourFromHour.Second).ToString();
        //                    var overtimeToDay = new DateTime(empYear, empMonth, empDay, shift.WorkingHourToHour.Hour, shift.WorkingHourToHour.Minute, shift.WorkingHourToHour.Second);
        //                    if ((shift.WorkingHourFromHour.Hour > 12 && shift.WorkingHourToHour.Hour < 12) || shift.ShiftTimeWorkingHourNumber > 24) // Next day
        //                    {
        //                        overtimeToDay = overtimeToDay.AddDays(1);
        //                    }
        //                    overTimeDetail.OvertimeTo = overtimeToDay.ToString();
        //                    overTimeDetail.WorkingHours = Convert.ToString(shift.ShiftTimeWorkingHourNumber);
        //                    overTimeDetail.Day = empDay;
        //                    overTimeDetail.CompanyTransport = "Tự túc";

        //                    if (overtimeInprogress.Any())
        //                    {
        //                        // Append emp to Overtime MASTER:
        //                        var overtimeMaster = overtimeInprogress.First();

        //                        overTimeModel.ID = overtimeMaster.ID;
        //                        overTimeModel.ApprovalStatus = overtimeMaster.ApprovalStatus;
        //                        overTimeModel.ApprovedBy = overtimeMaster.ApprovedBy;
        //                        overTimeModel.CommonDepartment = overtimeMaster.CommonDepartment;
        //                        overTimeModel.Date = overtimeMaster.CommonDate.ToString();
        //                        overTimeModel.CommonLocation.LookupId = overtimeMaster.CommonLocation.LookupId;
        //                        overTimeModel.OtherRequirements = overtimeMaster.OtherRequirements;
        //                        overTimeModel.Requester.LookupValue = overtimeMaster.Requester.LookupValue;
        //                        overTimeModel.Requester.LookupId = overtimeMaster.Requester.LookupId;
        //                        overTimeModel.SumOfEmployee = overtimeMaster.SumOfEmployee + 1 + "";
        //                        overTimeModel.SumOfMeal = overtimeMaster.SumOfMeal + 1 + "";
        //                        overTimeModel.OvertimeDetailModelList = new List<OvertimeDetailModel> { overTimeDetail };
        //                        OvertimeService.InsertOvertime(overTimeModel);
        //                    }
        //                    else
        //                    {
        //                        // Create new Overtime
        //                        overTimeModel.CommonDepartment.LookupId = getShiftManagementRequest.Department.Id;
        //                        overTimeModel.Date = empDate.ToString();

        //                        // Create Overtime with Shift ModifiedBy
        //                        if (!string.IsNullOrEmpty(getShiftManagementRequest.ModifiedByString))
        //                        {
        //                            var shiftModifiedBy = _employeeInfoDAL.GetByADAccount(getShiftManagementRequest.ModifiedByString);
        //                            overTimeModel.Requester = new LookupItem
        //                            {
        //                                LookupId = shiftModifiedBy.ID,
        //                                LookupValue = shiftModifiedBy.FullName
        //                            };
        //                        }
        //                        else
        //                            overTimeModel.Requester = getShiftManagementRequest.Requester;

        //                        overTimeModel.ApprovedBy = getShiftManagementRequest.ApprovedBy;
        //                        overTimeModel.SumOfEmployee = "1";
        //                        overTimeModel.SumOfMeal = "1";
        //                        overTimeModel.CommonLocation = getShiftManagementRequest.Location;

        //                        // Create Overtime DETAIL:

        //                        overTimeModel.OvertimeDetailModelList = new List<OvertimeDetailModel> { overTimeDetail };

        //                        this.OvertimeService.InsertOvertime(overTimeModel);
        //                    }
        //                }
        //            }
        //        }

        //        return new MessageResult
        //        {
        //            Code = 0,
        //            Message = "Success"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - ApproveShiftManagementDetail fn",
        //               TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
        //           string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
        //        return new MessageResult
        //        {
        //            Code = 1,
        //            Message = ex.Message
        //        };
        //    }
        //}

        private List<OverTimeManagementDetail> CreateOvertimeDetail(int month, int year, ShiftManagementDetailModel shiftManagementDetailModel, List<Biz.Models.ShiftTime> shiftTimeList)
        {
            var result = new List<OverTimeManagementDetail>();
            try
            {
                foreach (var dayInfo in shiftManagementDetailModel.ApprovalDays)
                {
                    var empMonth = month;
                    var empYear = year;
                    var empDay = dayInfo.Day;
                    var empDate = new DateTime(empYear, empMonth, empDay);
                    if (empDay >= 21 && empDay <= 31) // Month --
                    {
                        empDate = empDate.AddMonths(-1);
                    }
                    var overTimeDetail = new OverTimeManagementDetail
                    {
                        Employee = new LookupItem { LookupId = shiftManagementDetailModel.Employee.Id, LookupValue = shiftManagementDetailModel.Employee.FullName }
                    };
                    var shift = shiftTimeList.FirstOrDefault(x => x.ID == dayInfo.ShiftTimeId);
                    overTimeDetail.OvertimeFrom = new DateTime(empYear, empMonth, empDay, shift.WorkingHourFromHour.Hour, shift.WorkingHourFromHour.Minute, shift.WorkingHourFromHour.Second);
                    var overtimeToDay = new DateTime(empYear, empMonth, empDay, shift.WorkingHourToHour.Hour, shift.WorkingHourToHour.Minute, shift.WorkingHourToHour.Second);
                    if ((shift.WorkingHourFromHour.Hour > 12 && shift.WorkingHourToHour.Hour < 12) || shift.ShiftTimeWorkingHourNumber > 24) // Next day
                    {
                        overtimeToDay = overtimeToDay.AddDays(1);
                    }
                    overTimeDetail.OvertimeTo = overtimeToDay;
                    overTimeDetail.WorkingHours = shift.ShiftTimeWorkingHourNumber;
                    result.Add(overTimeDetail);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - ApproveShiftManagementDetail fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
            return result;
        }

        public MessageResult RefreshShiftManagementDetail(ShiftManagementModel getShiftManagementRequest)
        {
            try
            {
                var shiftManagementID = getShiftManagementRequest.Id;
                var ShifdetailSingle = getShiftManagementRequest.ShiftManagementDetailModelList[0];
                IList<OvertimeDetailModel> overtimeDetails = new List<OvertimeDetailModel>();
                if (shiftManagementID > 0)
                {
                    var shiftdetail = new ShiftManagementDetail();
                    //if (ShifdetailSingle != null)
                    //{
                    //    var currentShiftmanagementDetail = ConvertToShiftManagementDetailModel(_shiftManagementDetailDAL.GetByID(ShifdetailSingle.Id));
                    //    ShifdetailSingle.ShiftManagementID = shiftManagementID;
                    //    shiftdetail = ShifdetailSingle.ToEntity();//this.ConvertToShiftManagementDetail(ShifdetailSingle, shiftManagementID);
                    //    overtimeDetails = CheckOvertime(ShifdetailSingle, getShiftManagementRequest.Month, getShiftManagementRequest.Year, currentShiftmanagementDetail);
                    //    //ShiftManagementDetail.Add(shiftdetail);

                    //}

                    // set value for all ShiftTime[1->31]Approval field to false
                    for (int i = 1; i < 31; i++)
                    {
                        var value = shiftdetail.GetType().GetProperty(string.Format("ShiftTime{0}Approval", i.ToString()));
                        value.SetValue(shiftdetail, false);
                    }

                    _shiftManagementDetailDAL.SaveItem(shiftdetail);

                }
                return new MessageResult
                {
                    Code = 0,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - UpdateShiftManagement fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return new MessageResult
                {
                    Code = 1,
                    Message = ex.Message
                };
            }
        }

        private void SendMailToEmployees(string webUrl, List<ShiftManagementDetailModel> detailModel, List<EmployeeInfo> employeeList, int month, int year)
        {
            SendEmailActivity sendMailActity = new SendEmailActivity();
            var approveEmailItem = emailTemplateDAL.GetByKey("ShiftManagement_Approve");

            Thread thread = new Thread(delegate ()
            {
                if (approveEmailItem != null)
                {
                    foreach (var shiftDetailItem in detailModel)
                    {
                        string emailBody = HTTPUtility.HtmlDecode(approveEmailItem.MailBody);
                        var employee = employeeList.FirstOrDefault(x => x.ID == shiftDetailItem.Employee.Id);
                        if (employee != null && !string.IsNullOrEmpty(employee.Email))
                        {
                            string shiftTableHtml = GetShiftTable(webUrl, shiftDetailItem);
                            emailBody = string.Format(emailBody, employee.FullName, employee.FullName, month, year, shiftTableHtml);
                            string link = string.Format("{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagementMember.aspx", webUrl);
                            emailBody = string.Format(emailBody, employee.FullName, employee.FullName, month, year, shiftTableHtml);
                            emailBody = emailBody.Replace("#link", link);
                            sendMailActity.SendMail(webUrl, approveEmailItem.MailSubject, employee.Email, true, false, emailBody, false);
                        }
                    }

                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void SendEmailToApprover(int itemId, RBVH.Stada.Intranet.Biz.Models.ShiftManagement shiftManagementItem)
        {
            SendEmailActivity sendMailActity = new SendEmailActivity();
            var webUrl = SPContext.Current.Web.Url;
            var modifiedBy = shiftManagementItem.ModifiedBy.FullName;
            if (shiftManagementItem != null)
            {
                var departmentHead = _employeeInfoDAL.GetByADAccount(shiftManagementItem.ApprovedBy.ID);
                if (departmentHead != null)
                {
                    var requestEmailItem = emailTemplateDAL.GetByKey("ShiftManagement_Request");
                    if (requestEmailItem != null)
                    {
                        string emailBody = HTTPUtility.HtmlDecode(requestEmailItem.MailBody);
                        if (!string.IsNullOrEmpty(departmentHead.Email) && !string.IsNullOrEmpty(emailBody))
                        {
                            string link = string.Format("{0}/SitePages/ShiftApproval.aspx?itemId={1}&Source={0}/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementManager.aspx", webUrl, shiftManagementItem.ID);
                            var department = DepartmentListSingleton.GetDepartmentByID(shiftManagementItem.Department.LookupId, webUrl);
                            emailBody = string.Format(emailBody, departmentHead.FullName, string.IsNullOrEmpty(modifiedBy) ? shiftManagementItem.Requester.LookupValue : modifiedBy, shiftManagementItem.Month,
                                shiftManagementItem.Year, shiftManagementItem.Department.LookupValue, department.VietnameseName);

                            emailBody = emailBody.Replace("#link", link);
                            sendMailActity.SendMail(webUrl, requestEmailItem.MailSubject, departmentHead.Email, true, false, emailBody);

                            List<EmployeeInfo> toUsers = DelegationPermissionManager.GetListOfDelegatedEmployees(webUrl, departmentHead.ID, StringConstant.ShiftManagementList.ListUrl, shiftManagementItem.ID);
                            link = string.Format("{0}/SitePages/ShiftApproval.aspx?subSection=ShiftManagement&itemId={1}&Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx&Source=Tab=DelegationsApprovalTab", webUrl, shiftManagementItem.ID);
                            if (toUsers != null)
                            {
                                foreach (var toUser in toUsers)
                                {
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(toUser.Email))
                                        {
                                            emailBody = HTTPUtility.HtmlDecode(requestEmailItem.MailBody);
                                            emailBody = string.Format(emailBody, toUser.FullName, string.IsNullOrEmpty(modifiedBy) ? shiftManagementItem.Requester.LookupValue : modifiedBy, shiftManagementItem.Month,
                                            shiftManagementItem.Year, shiftManagementItem.Department.LookupValue, department.VietnameseName);
                                            emailBody = emailBody.Replace("#link", link);
                                            sendMailActity.SendMail(webUrl, requestEmailItem.MailSubject, toUser.Email, true, false, emailBody);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IList<OvertimeDetailModel> CheckOvertime(ShiftManagementDetailModel model, int month, int year, ShiftManagementDetailModel ShiftManagementDetailModel)
        {
            IList<OvertimeDetailModel> OvertimeDetailModelList = new List<OvertimeDetailModel>();
            try
            {
                foreach (var pro in model.GetType().GetProperties())
                {
                    if (pro.Name.Contains("ShiftTime"))
                    {
                        var shifttime = (ShiftTimeDetailModel)pro.GetValue(model, null);
                        var currentShifttime = (ShiftTimeDetailModel)ShiftManagementDetailModel.GetType().GetProperty(pro.Name).GetValue(ShiftManagementDetailModel, null);
                        if (shifttime.Approved && !currentShifttime.Approved)
                        {

                            var day = int.Parse(pro.Name.Replace("ShiftTime", ""));
                            var monthVal = month;
                            var yearVal = year;
                            if (day > 20)
                            {

                                if (month == 1)
                                {
                                    monthVal = 12;
                                    yearVal = year - 1;

                                }
                                else
                                {
                                    monthVal = month - 1;
                                }

                            }
                            if (shifttime.Day > 0)
                            {
                                var shifttimeId = int.Parse(shifttime.Value);

                                var shift = _shiftTimeDAL.GetByID(shifttimeId);

                                var overTimeManagementDetail = new OvertimeDetailModel();
                                overTimeManagementDetail.Employee.LookupId = model.Employee.Id;
                                overTimeManagementDetail.OvertimeFrom = new DateTime(yearVal, monthVal, day, shift.WorkingHourFromHour.Hour, shift.WorkingHourFromHour.Minute, shift.WorkingHourFromHour.Second).ToString();
                                var overtimeToDay = new DateTime(yearVal, monthVal, day, shift.WorkingHourToHour.Hour, shift.WorkingHourToHour.Minute, shift.WorkingHourToHour.Second);
                                if ((shift.WorkingHourFromHour.Hour > 12 && shift.WorkingHourToHour.Hour < 12) || shift.ShiftTimeWorkingHourNumber > 24) // Next day
                                {
                                    overtimeToDay = overtimeToDay.AddDays(1);
                                }
                                overTimeManagementDetail.OvertimeTo = overtimeToDay.ToString();
                                //overTimeManagementDetail.OvertimeTo = shift.WorkingHourToHour.ToString();
                                overTimeManagementDetail.WorkingHours = Convert.ToString(shift.ShiftTimeWorkingHourNumber);
                                overTimeManagementDetail.Day = day;
                                OvertimeDetailModelList.Add(overTimeManagementDetail);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return OvertimeDetailModelList;
        }

        /// <summary>
        /// Udpate shift Detail for change shift Workflow
        /// </summary>
        /// <param name="requesterId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="toShiftId"></param>
        /// <returns></returns>
        public bool UpdateShiftDetailForWorkflow(string requesterId, string fromDate, string toDate, string toShiftId)
        {
            try
            {
                DateTime fromDateValue, toDateValue;
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    var requesterInfo = _employeeInfoDAL.GetByID(Convert.ToInt32(requesterId));
                    if (DateTime.TryParseExact(fromDate, StringConstant.DateFormatMMddyyyy, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue)
                        && DateTime.TryParseExact(toDate, StringConstant.DateFormatMMddyyyy, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateValue) && requesterInfo != null)
                    {
                        int month = (1 <= fromDateValue.Day && fromDateValue.Day <= StringConstant.EndDayNumber) ? fromDateValue.Month : (fromDateValue.AddMonths(1)).Month;
                        int year;
                        if (fromDateValue.Month == 12 && fromDateValue.Day > StringConstant.EndDayNumber)
                        {
                            year = (fromDateValue.AddMonths(1)).Year;
                        }
                        else
                        {
                            year = fromDateValue.Year;
                        }

                        var shiftList = _shiftManagementDAL.GetByMonthYearDepartment(month, year, requesterInfo.Department.LookupId, requesterInfo.FactoryLocation.LookupId);

                        if (shiftList.Any())
                        {
                            int shiftDetailIdToUpdate = 0;
                            var shiftIdList = shiftList.Select(x => x.ID).Distinct().ToList();
                            foreach (var itemId in shiftIdList)
                            {
                                var shiftDetailItem = _shiftManagementDetailDAL.GetByShiftManagementIDEmployeeID(itemId, requesterInfo.ID).FirstOrDefault();

                                if (shiftDetailItem != null)
                                {
                                    shiftDetailIdToUpdate = shiftDetailItem.ID;
                                    break;
                                }
                            }

                            if (shiftDetailIdToUpdate > 0)
                            {
                                //Update shift detail == > change shift
                                if (fromDateValue.Day == toDateValue.Day)
                                {
                                    var columName = string.Format("ShiftTime{0}", toDateValue.Day);
                                    _shiftManagementDetailDAL.UpdateNewShiftValue(shiftDetailIdToUpdate, columName, Convert.ToInt32(toShiftId));
                                }
                            }
                        }
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Shift Management Service - UpdateShiftDetailForWorkflow fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public MessageResult InsertShiftManagementMasterDetail(ShiftManagementModel shiftManagementModel)
        {
            try
            {
                int shiftManagementId = 0;
                List<Biz.Models.ShiftManagement> shiftManagements = _shiftManagementDAL.GetByMonthYearDepartment(shiftManagementModel.Month, shiftManagementModel.Year, shiftManagementModel.Department.Id, shiftManagementModel.Location.LookupId);
                List<ShiftManagementDetail> shiftManagementDetails = new List<ShiftManagementDetail>();

                if (shiftManagements != null && shiftManagements.Count > 0)
                {
                    shiftManagementDetails = _shiftManagementDetailDAL.GetByShiftManagementID(shiftManagements[0].ID);
                }

                Type typeShiftManagementDetail = typeof(ShiftManagementDetail);
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

                DateTime dateOfShift = new DateTime(shiftManagementModel.Year, shiftManagementModel.Month, 1);
                dateOfShift = dateOfShift.AddMonths(-1);
                int daysInMonth = DateTime.DaysInMonth(dateOfShift.Year, dateOfShift.Month);

                if (shiftManagementModel != null && shiftManagementModel.ShiftManagementDetailModelList != null)
                {
                    List<Biz.Models.ShiftTime> shiftTimes = _shiftTimeDAL.GetAll();

                    foreach (ShiftManagementDetailModel shiftManagementDetailModel in shiftManagementModel.ShiftManagementDetailModelList)
                    {
                        ShiftManagementDetail shiftManagementDetail = shiftManagementDetails.Where(e => e.Employee.LookupId == shiftManagementDetailModel.Employee.Id).FirstOrDefault();

                        if (shiftManagementDetail == null)
                        {
                            shiftManagementDetail = shiftManagementDetailModel.ToEntity();
                            shiftManagementDetails.Add(shiftManagementDetail);
                        }
                        else
                        {
                            var shiftManagementDetailNew = shiftManagementDetailModel.ToEntity();
                            for (int i = 0; i < 11; i++)
                            {
                                if ((i + 21) <= daysInMonth)
                                {
                                    PropertyInfo shiftInfoNew = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", i + 21), bindingFlags);
                                    LookupItem shiftValueNew = shiftInfoNew.GetValue(shiftManagementDetailNew, null) as LookupItem;

                                    PropertyInfo shiftApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", i + 21), bindingFlags);
                                    object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftManagementDetail, null);
                                    if (shiftApprovalValue != null && Convert.ToBoolean(shiftApprovalValue) == false)
                                    {
                                        PropertyInfo shiftInfoOld = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", i + 21), bindingFlags);
                                        LookupItem shiftValueOld = shiftInfoOld.GetValue(shiftManagementDetail, null) as LookupItem;
                                        Biz.Models.ShiftTime shiftTime = shiftTimes.Where(e => e.ID == shiftValueNew.LookupId).FirstOrDefault();
                                        if (shiftTime != null)
                                        {
                                            shiftValueOld = new LookupItem() { LookupId = shiftTime.ID, LookupValue = shiftTime.Code };
                                            shiftInfoOld.SetValue(shiftManagementDetail, shiftValueOld);
                                        }
                                        else
                                        {
                                            shiftInfoOld.SetValue(shiftManagementDetail, new LookupItem());
                                        }
                                    }
                                }
                            }

                            for (int i = 11; i < 31; i++)
                            {
                                PropertyInfo shiftInfoNew = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", i - 10), bindingFlags);
                                LookupItem shiftValueNew = shiftInfoNew.GetValue(shiftManagementDetailNew, null) as LookupItem;

                                PropertyInfo shiftApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", i - 10), bindingFlags);
                                object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftManagementDetail, null);
                                if (shiftApprovalValue != null && Convert.ToBoolean(shiftApprovalValue) == false)
                                {
                                    PropertyInfo shiftInfoOld = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", i - 10), bindingFlags);
                                    LookupItem shiftValueOld = shiftInfoOld.GetValue(shiftManagementDetail, null) as LookupItem;
                                    Biz.Models.ShiftTime shiftTime = shiftTimes.Where(e => e.ID == shiftValueNew.LookupId).FirstOrDefault();
                                    if (shiftTime != null)
                                    {
                                        shiftValueOld = new LookupItem() { LookupId = shiftTime.ID, LookupValue = shiftTime.Code };
                                        shiftInfoOld.SetValue(shiftManagementDetail, shiftValueOld);
                                    }
                                    else
                                    {
                                        shiftInfoOld.SetValue(shiftManagementDetail, new LookupItem());
                                    }
                                }
                            }
                        }
                    }
                }

                // insert master
                if (shiftManagements == null || shiftManagements.Count == 0)
                {
                    var shiftManagement = shiftManagementModel.ToEntity();
                    shiftManagement = _shiftManagementDAL.SetDueDate(shiftManagement);
                    shiftManagementId = _shiftManagementDAL.SaveOrUpdate(shiftManagement);
                }
                else if (shiftManagementDetails != null && shiftManagementDetails.Count > 0)
                {
                    var shiftManagement = shiftManagements[0];
                    shiftManagementId = shiftManagement.ID;
                    shiftManagement = _shiftManagementDAL.SetDueDate(shiftManagement);
                    _shiftManagementDAL.SaveOrUpdate(shiftManagement);
                }

                // insert details
                if (shiftManagementId > 0)
                {
                    if (shiftManagementDetails != null && shiftManagementDetails.Count > 0)
                    {
                        foreach (ShiftManagementDetail shiftManagementDetail in shiftManagementDetails)
                        {
                            shiftManagementDetail.ShiftManagementID = new LookupItem() { LookupId = shiftManagementId, LookupValue = shiftManagementId.ToString() };
                        }
                        _shiftManagementDetailDAL.SaveOrUpdate(shiftManagementDetails);
                    }

                    // send email to approver
                    var shiftItem = _shiftManagementDAL.GetByID(shiftManagementId);
                    SendEmailToApprover(shiftManagementId, shiftItem);
                }

                return new MessageResult
                {
                    ObjectId = shiftManagementId,
                    Code = 0,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new MessageResult
                {
                    ObjectId = 0,
                    Code = 1,
                    Message = ex.Message
                };
            }
        }

        //// Service Improvement
        //public MessageResult InsertShiftManagementMaster(ShiftManagementModel shiftManagementModel)
        //{
        //    try
        //    {
        //        var shiftManagement = shiftManagementModel.ToEntity();
        //        var shiftManagementId = _shiftManagementDAL.SaveOrUpdate(shiftManagement);
        //        //var shiftManagementId = _shiftManagementDAL.SaveItem(shiftManagement);

        //        // Send email to Approver:
        //        var shiftItem = _shiftManagementDAL.GetByID(shiftManagementId);
        //        SendEmailToApprover(shiftManagementId, shiftItem);
        //        return new MessageResult
        //        {
        //            ObjectId = shiftManagementId,
        //            Code = 0,
        //            Message = "Success"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new MessageResult
        //        {
        //            Code = 1,
        //            Message = ex.Message,
        //            ObjectId = 0
        //        };
        //    }
        //}

        //public MessageResult InsertShiftManagementDetail(ShiftManagementDetailModel shiftManagementDetailModel)
        //{
        //    try
        //    {
        //        //var shiftManagementDetail = shiftManagementDetailModel.ToEntity(); //this.ConvertToShiftManagementDetail(shiftManagementDetailModel, shiftManagementDetailModel.ShiftManagementID);
        //        //var shiftManagementID = _shiftManagementDetailDAL.SaveItem(shiftManagementDetail);
        //        var shiftManagementDetail = shiftManagementDetailModel.ToEntity(); //this.ConvertToShiftManagementDetail(shiftManagementDetailModel, shiftManagementDetailModel.ShiftManagementID);
        //        var shiftManagementID = this._shiftManagementDetailDAL.SaveOrUpdate(shiftManagementDetail);

        //        return new MessageResult
        //        {
        //            ObjectId = shiftManagementID,
        //            Code = 0,
        //            Message = "Success"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new MessageResult
        //        {
        //            Code = 1,
        //            Message = ex.Message,
        //            ObjectId = 0
        //        };
        //    }
        //}

        private string GetShiftTable(string webUrl, ShiftManagementDetail shiftDetail)
        {
            var shiftTimeDAL = new ShiftTimeDAL(webUrl);
            var shiftTimeList = shiftTimeDAL.GetShiftTimes();

            StringBuilder sbHeader = new StringBuilder("<tr>");
            StringBuilder sbBody = new StringBuilder("<tr>");

            string[] headerDates = new[] { "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };
            foreach (var headerDate in headerDates)
            {
                sbHeader.AppendFormat("<td style='border: 1px solid black; width: 20px; text-align: center;'>{0}</td>", headerDate);

                LookupItem lookupItem = (LookupItem)shiftDetail.GetType().GetProperty($"ShiftTime{headerDate}").GetValue(shiftDetail);
                bool approvedValue = (bool)shiftDetail.GetType().GetProperty($"ShiftTime{headerDate}Approval").GetValue(shiftDetail);

                sbBody.Append(GetRowCell(shiftTimeList, lookupItem.LookupId, approvedValue));
            }
            sbHeader.Append("</tr>");
            sbBody.Append("</tr>");

            return $"<table cellspacing='5' cellpadding='5' style='border-collapse: collapse; border: 1px solid black;'><tbody>{sbHeader.ToString()}{sbBody.ToString()}</tbody></table>";
        }

        private string GetShiftTable(string webUrl, ShiftManagementDetailModel shiftDetailModel)
        {
            var shiftTimeDAL = new ShiftTimeDAL(webUrl);
            var shiftTimeList = shiftTimeDAL.GetShiftTimes();
            string tableHtml = "<table cellspacing='5' cellpadding='5' style='border-collapse: collapse; border: 1px solid black;'><tbody>{0}{1}</tbody></table>";

            string headerString = "<tr>";
            string rowString = "<tr>";

            string[] headerDates = new[] { "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };
            foreach (var headerDate in headerDates)
            {
                headerString += string.Format("<td style='border: 1px solid black;'>{0}</td>", headerDate);

                string columnName = string.Format("ShiftTime{0}", headerDate);
                string approvedColumnName = columnName + "Approval";
                ShiftTimeDetailModel shiftTimeItem = (ShiftTimeDetailModel)shiftDetailModel.GetType().GetProperty(columnName).GetValue(shiftDetailModel);
                bool approvedValue = (bool)shiftDetailModel.GetType().GetProperty(approvedColumnName).GetValue(shiftDetailModel);

                rowString += GetRowCell(shiftTimeItem);
            }
            headerString += "</tr>";
            rowString += "</tr>";

            tableHtml = string.Format(tableHtml, headerString, rowString);

            return tableHtml;
        }

        private string GetRowCell(ShiftTimeDetailModel detailModel)
        {
            if (detailModel != null && !string.IsNullOrWhiteSpace(detailModel.Code))
            {
                if (detailModel.Approved)
                {
                    return string.Format("<td style='background-color: #c6ffcc;border: 1px solid black;'>{0}</td>", detailModel.Code);
                }
                else
                {
                    return string.Format("<td style='border: 1px solid black;'>{0}</td>", detailModel.Code);
                }
            }
            else
                return "<td style='border: 1px solid black;'></td>";
        }

        private string GetRowCell(List<Biz.Models.ShiftTime> shiftTimeList, int shiftTimeId, bool isApproved)
        {
            var shiftTime = shiftTimeList.Where(x => x.ID == shiftTimeId).FirstOrDefault();
            if (shiftTime != null)
            {
                if (isApproved)
                {
                    return string.Format("<td style='background-color: #c6ffcc;border: 1px solid black; width: 20px; text-align: center;'>{0}</td>", shiftTime.Code);
                }
                else
                {
                    return string.Format("<td style='border: 1px solid black; width: 20px; text-align: center;'>{0}</td>", shiftTime.Code);
                }
            }
            else
                return "<td style='border: 1px solid black; width: 20px; text-align: center;'></td>";
        }

        public MessageResult SendAdminApprovalEmail(AdminApprovalModel adminApprovalModel)
        {
            try
            {
                if (adminApprovalModel.EmployeeNameList.Any())
                {
                    //Send email to requester to inform
                    SendMailInformShiftApprove(adminApprovalModel.EmployeeNameList, adminApprovalModel.ShiftManagementId, adminApprovalModel.DepartmentId, adminApprovalModel.LocationId, adminApprovalModel.ApproverFullName);
                }
                return new MessageResult
                {
                    ObjectId = 111,
                    Code = 0,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ShiftManagementService - SendAdminApprovalEmail",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return new MessageResult
                {
                    Code = 1,
                    Message = ex.Message
                };
            }
        }

        private void SendMailInformShiftApprove(List<string> employeeNameList, int shiftManagementId, int departmentId, int locationId, string approverFullName)
        {
            SendEmailActivity sendMailActity = new SendEmailActivity();
            var mailItem = emailTemplateDAL.GetByKey("ShiftManagement_Approve_Requester");
            var shiftManagement = _shiftManagementDAL.GetByID(shiftManagementId);

            string webUrl = SPContext.Current.Web.Url;
            if (shiftManagement != null && mailItem != null)
            {
                string employeeNameString = string.Join(", ", employeeNameList);
                string mailBody = string.Empty;

                // Get all Admin of Department
                var adminList = _employeeInfoDAL.GetByPositionDepartment(Biz.Constants.StringConstant.EmployeePosition.Administrator, departmentId, locationId);
                string shiftRequestLink = string.Format("{0}/SitePages/ShiftRequest.aspx?itemid={1}&mode=view&Source={0}/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementAdmin.aspx", webUrl, shiftManagement.ID);
                foreach (var employeeRequester in adminList)
                {
                    if (!string.IsNullOrEmpty(employeeRequester.Email))
                    {
                        mailBody = HTTPUtility.HtmlDecode(mailItem.MailBody);
                        mailBody = string.Format(mailBody, employeeRequester.FullName, approverFullName, shiftManagement.Month, shiftManagement.Year, employeeNameString);
                        mailBody = mailBody.Replace("#link", shiftRequestLink);
                        sendMailActity.SendMail(webUrl, mailItem.MailSubject, employeeRequester.Email, true, false, mailBody);
                    }
                }
            }
        }

        public bool IsChangeShiftExist(string employeeId, string date)
        {
            try
            {
                var fromDateValue = date.ToMMDDYYYYDate(false); // mm-dd-yyyy
                                                                //get change change item with status null or approved
                var overtimeItem = changeShiftManagementDAL.GetByDate(Convert.ToInt32(employeeId), fromDateValue);
                //if change shift item exist
                if (overtimeItem != null)
                {
                    //return true: user cannot register one more item in the same date
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA -ShiftManagementService - IsChangeShiftExist fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return true;
            }
        }

        public Stream ExportShifts(string Ids)
        {
            if (string.IsNullOrEmpty(Ids))
            {
                return null;
            }

            Collection<ShiftManagementModel> shiftManagementCollection = new Collection<ShiftManagementModel>();
            string[] shiftIds = Ids.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string shiftId in shiftIds)
            {
                try
                {
                    ShiftManagementModel shiftManagement = GetShiftManagementById(shiftId);
                    if (shiftManagement != null)
                    {
                        shiftManagementCollection.Add(shiftManagement);
                    }
                }
                catch { }
            }

            string templateFileName = "GiayDangKyCa.xlsx";

            string destFilePath = "";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                string tempFolderPath = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\Tmp", 15);
                Directory.CreateDirectory(tempFolderPath);
                ExcelHelper.RemoveOldFiles(tempFolderPath, 1);

                destFilePath = ExcelHelper.DownloadFile(SPContext.Current.Site.RootWeb.Url, "Shared Documents", templateFileName, tempFolderPath, string.Empty);

                if (shiftManagementCollection.Count > 0)
                {
                    using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Open(destFilePath, true))
                    {
                        string sheetName = "Sheet1";
                        uint startRowIdx = 7;
                        uint startColIdx = 1;
                        string startColName = ExcelHelper.ConvertColumnIndexToLetter((int)startColIdx);

                        List<Department> departmentCollection = _departmentDAL.GetAll();
                        string departmentName = ExcelHelper.GetCellValue(spreadSheetDoc.WorkbookPart, sheetName, "C4");
                        foreach (ShiftManagementModel shiftManagement in shiftManagementCollection)
                        {
                            ShiftManagementModel processedShiftManagement = RemoveEmployeeHasNoShift(shiftManagement);

                            Department departmentObj = departmentCollection.Where(e => e.ID == processedShiftManagement.Department.Id).FirstOrDefault();
                            departmentName = string.Format("{0} {1}", departmentName, departmentObj.VietnameseName);
                            ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "C", 4, departmentName);

                            for (int i = 0; i < processedShiftManagement.ShiftManagementDetailModelList.Count; i++)
                            {
                                ShiftManagementDetailModel shiftManagementDetailModel = processedShiftManagement.ShiftManagementDetailModelList[i];
                                if (!EmployeeHasShifts(shiftManagementDetailModel))
                                {
                                    continue;
                                }

                                uint newRowIdx = startRowIdx + (uint)i + 1;
                                ExcelHelper.DuplicateRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx, newRowIdx);

                                ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(1), newRowIdx,
                                    shiftManagementDetailModel.Employee.EmployeeId, CellValues.String);
                                ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(2), newRowIdx,
                                    shiftManagementDetailModel.Employee.FullName, CellValues.String);
                                ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(3), newRowIdx,
                                    processedShiftManagement.Month.ToString(), CellValues.Number);
                                ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(4), newRowIdx,
                                    processedShiftManagement.Year.ToString(), CellValues.Number);

                                Type typeShiftManagementDetailModel = typeof(ShiftManagementDetailModel);
                                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                                for (int idx = 21; idx <= 31; idx++)
                                {
                                    PropertyInfo propertyInfo = typeShiftManagementDetailModel.GetProperty(string.Format("ShiftTime{0}", idx), bindingFlags);
                                    object value = propertyInfo.GetValue(shiftManagementDetailModel, null);
                                    ShiftTimeDetailModel shiftTimeObj = value as ShiftTimeDetailModel;

                                    ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(idx - 16), newRowIdx,
                                        (shiftTimeObj.Approved == true ? shiftTimeObj.Code.ToString() : ""));
                                }

                                for (int idx = 1; idx <= 20; idx++)
                                {
                                    PropertyInfo propertyInfo = typeShiftManagementDetailModel.GetProperty(string.Format("ShiftTime{0}", idx), bindingFlags);
                                    object value = propertyInfo.GetValue(shiftManagementDetailModel, null);
                                    ShiftTimeDetailModel shiftTimeObj = value as ShiftTimeDetailModel;

                                    ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(idx + 15), newRowIdx,
                                        (shiftTimeObj.Approved == true ? shiftTimeObj.Code : ""));
                                }
                            }
                        }

                        ExcelHelper.RemoveRow(spreadSheetDoc.WorkbookPart, sheetName, 7);
                        ExcelHelper.Save(spreadSheetDoc.WorkbookPart, sheetName);
                    }
                }
            });

            if (!string.IsNullOrEmpty(destFilePath) && File.Exists(destFilePath))
            {
                String headerInfo = string.Format("attachment; filename={0}", Path.GetFileName(destFilePath));
                WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = headerInfo;
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";

                return File.OpenRead(destFilePath);
            }
            else
            {
                return null;
            }
        }

        private ShiftManagementModel RemoveEmployeeHasNoShift(ShiftManagementModel shiftManagement)
        {
            for (int i = 0; i < shiftManagement.ShiftManagementDetailModelList.Count(); i++)
            {
                ShiftManagementDetailModel shiftManagementDetailModel = shiftManagement.ShiftManagementDetailModelList[i];
                if (!EmployeeHasShifts(shiftManagementDetailModel))
                {
                    shiftManagement.ShiftManagementDetailModelList.RemoveAt(i);
                    i--;
                }
            }
            return shiftManagement;
        }

        private bool EmployeeHasShifts(ShiftManagementDetailModel shiftManagementDetail)
        {
            bool ret = false;

            Type typeShiftManagementDetailModel = typeof(ShiftManagementDetailModel);
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            for (int idx = 1; idx <= 31; idx++)
            {
                PropertyInfo propertyInfo = typeShiftManagementDetailModel.GetProperty(string.Format("ShiftTime{0}", idx), bindingFlags);
                object value = propertyInfo.GetValue(shiftManagementDetail, null);
                ShiftTimeDetailModel shiftTimeObj = value as ShiftTimeDetailModel;
                if (shiftTimeObj != null && shiftTimeObj.Approved)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        //public MessageResult ApproveShiftManagementDetails(List<ShiftManagementDetailModel> shiftManagementDetailModel)
        //{
        //    return new MessageResult
        //    {
        //        ObjectId = 111,
        //        Code = 0,
        //        Message = "Success"
        //    };
        //}

        private string SaveFile(Stream shiftData, string directory, string fileName)
        {
            string filePath = "";

            try
            {
                if (shiftData != null)
                {
                    filePath = Path.Combine(directory, fileName);
                    int length = 0;
                    using (Stream stream = new FileStream(filePath, FileMode.Create))
                    {
                        int readCount;
                        var buffer = new byte[8192];
                        while ((readCount = shiftData.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            stream.Write(buffer, 0, readCount);
                            length += readCount;
                        }
                    }
                }
            }
            catch
            {
                filePath = "";
            }

            return filePath;
        }

        private ExcelShiftsOfDepartment ReadExcelShiftData(string filePath)
        {
            ExcelShiftsOfDepartment excelShiftsOfDepartment = new ExcelShiftsOfDepartment();

            using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Open(filePath, false))
            {
                var sheetName = "Sheet1";
                var workbookPart = spreadSheetDoc.WorkbookPart;

                int rowIdx = 9;
                bool flag = true;
                while (flag)
                {
                    ExcelEmployeeShift excelEmployeeShift = new ExcelEmployeeShift();

                    string employeeIdCellAddress = string.Format("{0}{1}", ExcelHelper.ConvertColumnIndexToLetter(1), rowIdx);
                    string employeeIdCellValue = ExcelHelper.GetCellValue(workbookPart, sheetName, employeeIdCellAddress);
                    if (rowIdx >= 1000 || string.IsNullOrEmpty(employeeIdCellValue))
                    {
                        break;
                    }
                    else
                    {
                        string employeeFullNameCellAddress = string.Format("{0}{1}", ExcelHelper.ConvertColumnIndexToLetter(2), rowIdx);
                        string employeeFullNameCellValue = ExcelHelper.GetCellValue(workbookPart, sheetName, employeeFullNameCellAddress);
                        excelEmployeeShift.EmployeeID = employeeIdCellValue.Trim();
                        excelEmployeeShift.EmployeeFullName = employeeFullNameCellValue;

                        List<Biz.Models.ShiftTime> shiftTimes = _shiftTimeDAL.GetShiftTimes();
                        List<string> shiftCodeList = shiftTimes.Where(e => e.ShiftRequired == true).Select(e => e.Code.ToUpper()).ToList();

                        for (var colIdx = 3; colIdx <= 33; colIdx++)
                        {
                            string cellAddress = string.Format("{0}{1}", ExcelHelper.ConvertColumnIndexToLetter(colIdx), rowIdx);
                            string cellValue = ExcelHelper.GetCellValue(workbookPart, sheetName, cellAddress) + string.Empty;
                            cellValue = cellValue.Trim().ToUpper();
                            cellValue = shiftCodeList.Contains(cellValue) == true ? cellValue : string.Empty;

                            excelEmployeeShift.ShiftCodes.Add(cellValue);
                        }

                        if (excelEmployeeShift.ShiftCodes.Any(e => !string.IsNullOrEmpty(e)))
                        {
                            excelShiftsOfDepartment.EmployeeShifts.Add(excelEmployeeShift);
                        }
                    }

                    rowIdx++;
                }
            }

            return excelShiftsOfDepartment;
        }

        private Dictionary<string, int> ValidateDuplicatedData(ExcelShiftsOfDepartment excelShiftsOfDepartment)
        {
            Dictionary<string, int> duplicatedEmployees = excelShiftsOfDepartment.EmployeeShifts.GroupBy(e => e.EmployeeID).Where(x => x.Count() > 1).ToDictionary(x => x.Key, y => y.Count());

            return duplicatedEmployees;
        }

        public MessageResult ImportShifts(string month, string year, string departmentId, string locationId, string fileName)
        {
            bool hasError = false;
            string newFileName = string.Format("{0}-{1}.xlsx", "STADA_ShiftData", DateTime.Now.Ticks);
            string directory = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\ShiftData", 15);
            string filePath = "";

            MessageResult retMsg = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    Directory.CreateDirectory(directory);
                    ExcelHelper.RemoveOldFiles(directory, 1);

                    filePath = ExcelHelper.DownloadFile(SPContext.Current.Site.RootWeb.Url, "Shared Documents", fileName, directory, newFileName);

                    if (string.IsNullOrEmpty(filePath))
                    {
                        retMsg = new MessageResult() { Code = 999, Message = "Cannot save the file" };
                        hasError = true;
                    }
                }
                catch { }
            });

            int shiftId = 0;
            if (hasError == false)
            {
                try
                {
                    ExcelShiftsOfDepartment excelShiftsOfDepartment = ReadExcelShiftData(filePath);
                    excelShiftsOfDepartment.DepartmentId = Convert.ToInt32(departmentId);
                    excelShiftsOfDepartment.Location = Convert.ToInt32(locationId);
                    excelShiftsOfDepartment.Month = Convert.ToInt32(month);
                    excelShiftsOfDepartment.Year = Convert.ToInt32(year);

                    Dictionary<string, int> duplicatedEmployees = ValidateDuplicatedData(excelShiftsOfDepartment);

                    if (duplicatedEmployees != null && duplicatedEmployees.Count > 0)
                    {
                        retMsg = new MessageResult() { Code = 998, Message = "Duplicate data" };
                        hasError = true;
                    }

                    if (hasError == false)
                    {
                        DateTime topBound = new DateTime(excelShiftsOfDepartment.Year, excelShiftsOfDepartment.Month, 20);
                        DateTime temp = topBound.AddMonths(-1);
                        DateTime bottomBound = new DateTime(temp.Year, temp.Month, 21);

                        DateTime currentDate = DateTime.Now;
                        if (currentDate.Date >= bottomBound && currentDate.Date <= topBound)
                        {
                            if (currentDate.Day >= 21 && currentDate.Day <= 31)
                            {
                                foreach (var employeeShift in excelShiftsOfDepartment.EmployeeShifts)
                                {
                                    for (int i = 21; i <= currentDate.Day; i++) // include current date "i <= currentDate.Day" (not include current date "i < currentDate.Day")
                                    {
                                        employeeShift.ShiftCodes[i - 21] = "";
                                    }
                                }
                            }
                            else if (currentDate.Day >= 1 && currentDate.Day <= 20)
                            {
                                int count = 11 + currentDate.Day;
                                foreach (var employeeShift in excelShiftsOfDepartment.EmployeeShifts)
                                {
                                    for (int i = 0; i < count; i++)  // include current date "i < count" (not include current date "i < count - 1")
                                    {
                                        employeeShift.ShiftCodes[i] = "";
                                    }
                                }
                            }
                        }
                        else if (currentDate.Date > topBound)
                        {
                            retMsg = new MessageResult() { Code = 0, Message = "Successful", ObjectId = shiftId };
                            hasError = true;
                        }

                        if (hasError == false)
                        {
                            for (int i = 0; i < excelShiftsOfDepartment.EmployeeShifts.Count; i++)
                            {
                                ExcelEmployeeShift excelEmployeeShift = excelShiftsOfDepartment.EmployeeShifts[i];
                                if (excelEmployeeShift.ShiftCodes.Any(e => !string.IsNullOrEmpty(e)))
                                {
                                    continue;
                                }
                                else
                                {
                                    excelShiftsOfDepartment.EmployeeShifts.RemoveAt(i);
                                    i -= 1;
                                }
                            }

                            string siteUrl = SPContext.Current.Site.Url;
                            ShiftManagementDAL _shiftManagementDAL = new ShiftManagementDAL(siteUrl);
                            ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(siteUrl);

                            List<Biz.Models.ShiftManagement> shiftManagements = _shiftManagementDAL.GetByMonthYearDepartment(excelShiftsOfDepartment.Month, excelShiftsOfDepartment.Year, excelShiftsOfDepartment.DepartmentId, excelShiftsOfDepartment.Location);
                            List<ShiftManagementDetail> shiftManagementDetails = new List<ShiftManagementDetail>();

                            if (shiftManagements != null && shiftManagements.Count > 0)
                            {
                                shiftManagementDetails = _shiftManagementDetailDAL.GetByShiftManagementID(shiftManagements[0].ID);
                            }

                            List<EmployeeInfo> employeesOfDepartment = _employeeInfoDAL.GetByLocationAndDepartment(excelShiftsOfDepartment.Location, excelShiftsOfDepartment.DepartmentId, true, 4, StringConstant.EmployeeInfoList.EmployeeIDField);

                            if (shiftManagementDetails != null && shiftManagementDetails.Count > 0)
                            {
                                foreach (var employeeShift in excelShiftsOfDepartment.EmployeeShifts)
                                {
                                    EmployeeInfo employeeInfo = employeesOfDepartment.Where(e => e.EmployeeID == employeeShift.EmployeeID).FirstOrDefault();
                                    if (employeeInfo != null)
                                    {
                                        employeeShift.EmployeeLookupID = employeeInfo.ID;
                                    }
                                }

                                excelShiftsOfDepartment.EmployeeShifts = excelShiftsOfDepartment.EmployeeShifts.Where(e => e.EmployeeLookupID > 0).ToList();
                                List<int> employeeLookupIds = excelShiftsOfDepartment.EmployeeShifts.Select(e => e.EmployeeLookupID).ToList();
                                shiftManagementDetails = shiftManagementDetails.Where(e => employeeLookupIds.Contains(e.Employee.LookupId)).ToList();
                            }

                            ShiftTimeDAL _shiftTimeDAL = new ShiftTimeDAL(siteUrl);
                            List<Biz.Models.ShiftTime> shiftTimes = _shiftTimeDAL.GetAll();

                            DateTime dateOfShift = new DateTime(excelShiftsOfDepartment.Year, excelShiftsOfDepartment.Month, 1);
                            dateOfShift = dateOfShift.AddMonths(-1);
                            int daysInMonth = DateTime.DaysInMonth(dateOfShift.Year, dateOfShift.Month);

                            Type typeShiftManagementDetail = typeof(ShiftManagementDetail);
                            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                            foreach (ExcelEmployeeShift excelEmployeeShift in excelShiftsOfDepartment.EmployeeShifts)
                            {
                                ShiftManagementDetail shiftManagementDetail = shiftManagementDetails.Where(e => e.Employee.LookupId == excelEmployeeShift.EmployeeLookupID).FirstOrDefault();
                                if (shiftManagementDetail == null)
                                {
                                    shiftManagementDetail = new ShiftManagementDetail();
                                    EmployeeInfo employeeInfo = employeesOfDepartment.Where(e => e.EmployeeID == excelEmployeeShift.EmployeeID).FirstOrDefault();
                                    if (employeeInfo != null)
                                    {
                                        shiftManagementDetail.Employee = new LookupItem() { LookupId = employeeInfo.ID, LookupValue = employeeInfo.FullName };
                                        shiftManagementDetails.Add(shiftManagementDetail);
                                    }
                                }

                                for (int i = 0; i < 11; i++)
                                {
                                    if ((i + 21) <= daysInMonth)
                                    {
                                        Biz.Models.ShiftTime shiftTime = shiftTimes.Where(e => e.Code.ToUpper() == excelEmployeeShift.ShiftCodes[i].ToUpper()).FirstOrDefault();
                                        if (shiftTime != null)
                                        {
                                            PropertyInfo shiftApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", i + 21), bindingFlags);
                                            object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftManagementDetail, null);
                                            if (shiftApprovalValue != null && Convert.ToBoolean(shiftApprovalValue) == false)
                                            {
                                                PropertyInfo shiftInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", i + 21), bindingFlags);
                                                LookupItem shiftValue = shiftInfo.GetValue(shiftManagementDetail, null) as LookupItem;
                                                if (shiftValue == null)
                                                {
                                                    shiftValue = new LookupItem();
                                                }
                                                shiftValue.LookupId = shiftTime.ID;
                                                shiftValue.LookupValue = shiftTime.Code;
                                                shiftInfo.SetValue(shiftManagementDetail, shiftValue);
                                            }
                                        }
                                    }
                                }

                                for (int i = 11; i < 31; i++)
                                {
                                    Biz.Models.ShiftTime shiftTime = shiftTimes.Where(e => e.Code.ToUpper() == excelEmployeeShift.ShiftCodes[i].ToUpper()).FirstOrDefault();
                                    if (shiftTime != null)
                                    {
                                        PropertyInfo shiftApprovalInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}Approval", i - 10), bindingFlags);
                                        object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftManagementDetail, null);
                                        if (shiftApprovalValue != null && Convert.ToBoolean(shiftApprovalValue) == false)
                                        {
                                            PropertyInfo shiftInfo = typeShiftManagementDetail.GetProperty(string.Format("ShiftTime{0}", i - 10), bindingFlags);
                                            LookupItem shiftValue = shiftInfo.GetValue(shiftManagementDetail, null) as LookupItem;
                                            if (shiftValue == null)
                                            {
                                                shiftValue = new LookupItem();
                                            }
                                            shiftValue.LookupId = shiftTime.ID;
                                            shiftValue.LookupValue = shiftTime.Code;
                                            shiftInfo.SetValue(shiftManagementDetail, shiftValue);
                                        }
                                    }
                                }
                            }

                            if (shiftManagements == null || shiftManagements.Count == 0)
                            {
                                Biz.Models.ShiftManagement shiftManagement = new Biz.Models.ShiftManagement();
                                shiftManagement.Department = new LookupItem() { LookupId = excelShiftsOfDepartment.DepartmentId, LookupValue = excelShiftsOfDepartment.DepartmentId.ToString() };
                                shiftManagement.Location = new LookupItem() { LookupId = excelShiftsOfDepartment.Location, LookupValue = excelShiftsOfDepartment.Location.ToString() };
                                shiftManagement.Month = excelShiftsOfDepartment.Month;
                                shiftManagement.Year = excelShiftsOfDepartment.Year;

                                EmployeeInfo requester = _employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);
                                shiftManagement.Requester = new LookupItem() { LookupId = requester.ID, LookupValue = requester.FullName };

                                EmployeeInfo approvedBy = _employeeInfoDAL.GetByPositionDepartment(StringConstant.EmployeePosition.DepartmentHead, excelShiftsOfDepartment.DepartmentId, excelShiftsOfDepartment.Location).FirstOrDefault();
                                shiftManagement.ApprovedBy = new User() { ID = approvedBy.ADAccount.ID, UserName = approvedBy.ADAccount.UserName, FullName = approvedBy.FullName, IsGroup = false };

                                shiftId = _shiftManagementDAL.SaveOrUpdate(shiftManagement);
                            }
                            else if (shiftManagementDetails != null && shiftManagementDetails.Count > 0)
                            {
                                shiftId = shiftManagements[0].ID;
                                _shiftManagementDAL.SaveOrUpdate(shiftManagements[0]);
                            }

                            if (shiftId > 0)
                            {
                                if (shiftManagementDetails != null && shiftManagementDetails.Count > 0)
                                {
                                    foreach (ShiftManagementDetail shiftManagementDetail in shiftManagementDetails)
                                    {
                                        shiftManagementDetail.ShiftManagementID = new LookupItem() { LookupId = shiftId, LookupValue = shiftId.ToString() };
                                    }
                                    _shiftManagementDetailDAL.SaveOrUpdate(shiftManagementDetails);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    retMsg = new MessageResult() { Code = 997, Message = "Error" };
                    hasError = true;
                }
            }

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch { }
                }
            });

            try
            {
                ExcelHelper.DeleteLibraryFile(SPContext.Current.Site.RootWeb.Url, "Shared Documents", fileName);
            }
            catch { }

            if (hasError == false)
            {
                retMsg = new MessageResult() { Code = 0, Message = "Successful", ObjectId = shiftId };
            }

            return retMsg;
        }

        public bool IsDelegated(string fromApproverId, string itemId)
        {
            var approverInfo = _employeeInfoDAL.GetByADAccount(Convert.ToInt32(fromApproverId));
            return DelegationPermissionManager.IsDelegation(approverInfo.ID, ShiftManagementList.ListUrl, Convert.ToInt32(itemId)) != null;
        }
    }
}