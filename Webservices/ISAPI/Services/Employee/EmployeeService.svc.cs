using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.Employee;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Webservices.Employee
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeInfoDAL _employeeInfoDAL;
        private readonly DepartmentDAL _departmentDAL;
        private readonly OverTimeManagementDAL _overtimeManagementDAL;
        private readonly OverTimeManagementDetailDAL _overtimeManagementDetailDAL;
        private readonly EmployeePositionDAL _employeePositionDAL;

        public EmployeeService()
        {
            var url = SPContext.Current.Web.Url;

            _employeeInfoDAL = new EmployeeInfoDAL(url);
            _departmentDAL = new DepartmentDAL(url);
            _overtimeManagementDAL = new OverTimeManagementDAL(url);
            _overtimeManagementDetailDAL = new OverTimeManagementDetailDAL(url);
            _employeePositionDAL = new EmployeePositionDAL(url);
        }

        public List<EmployeeInfo> GetCommonAccountList()
        {
            //throw new NotImplementedException();
            return new List<EmployeeInfo>();
        }

        /// <summary>
        /// Get current logined user
        /// </summary>
        /// <returns></returns>
        /// CALL URL: _vti_bin/Services/Employee/EmployeeService.svc/GetCurrentUser
        public CurrentUserModel GetCurrentUser()
        {
            try
            {
                CurrentUserModel user = new CurrentUserModel();
                //Get Current Login User
                SPUser spUser = SPContext.Current.Web.CurrentUser;
                if (spUser.IsSiteAdmin)
                {
                    user.IsSystemAdmin = true;
                    //return user;
                }

                var employeeDal = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                EmployeeInfo currentEmployee = HttpContext.Current.Session[StringConstant.SessionString.EmployeeLogedin] as EmployeeInfo;

                if (currentEmployee == null)
                {
                    if (spUser != null)
                    {
                        int currentLoginName = spUser.ID;
                        currentEmployee = employeeDal.GetByADAccount(currentLoginName);
                    }
                }

                if (currentEmployee != null)
                {
                    user = new CurrentUserModel()
                    {
                        ID = currentEmployee.ID,
                        EmployeeID = currentEmployee.EmployeeID,
                        Department = currentEmployee.Department,
                        Location = currentEmployee.FactoryLocation,
                        FullName = currentEmployee.FullName,
                        EmployeePosition = (currentEmployee.EmployeePosition != null && currentEmployee.EmployeePosition.LookupId > 0) ? currentEmployee.EmployeePosition.LookupId : 0
                    };
                    if (user.Department != null && user.Department.LookupId > 0)
                    {
                        var departmentDetail = _departmentDAL.GetByID(user.Department.LookupId);
                        if (departmentDetail != null)
                        {
                            user.IsBODApprovalRequired = departmentDetail.IsBODApprovalRequired;
                        }
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                  new SPDiagnosticsCategory("STADA - Employee Service - GetCurrentUser fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Get employee list in department
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        /// CALL URL: _vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeListInCurrentDepartment/3/2
        public List<EmployeeDepartmentModel> GetEmployeeListInCurrentDepartment(string departmentId, string locationIds)
        {
            List<EmployeeDepartmentModel> employeeInDepartmentList = new List<EmployeeDepartmentModel>();
            List<EmployeeInfo> employeeInfoList = new List<EmployeeInfo>();
            try
            {
                int currentDepartmentId = int.Parse(departmentId);
                var lcid = CultureInfo.CurrentUICulture.LCID;
                //Prepare employee list
                if (currentDepartmentId > 0)
                {
                    string[] viewFields = new string[] { EmployeeInfoList.EmployeeIDField, EmployeeInfoList.FullNameField, EmployeeInfoList.DepartmentField, EmployeeInfoList.FactoryLocationField };
                    var employees = _employeeInfoDAL.GetByDepartment(currentDepartmentId, locationIds.SplitStringOfLocations().ConvertAll(e => Convert.ToInt32(e)), viewFields);
                    Department department = DepartmentListSingleton.GetDepartmentByID(currentDepartmentId, SPContext.Current.Site.Url);
                    string departmentName = lcid == 1066 ? department.VietnameseName : department.Name;
                    foreach (var item in employees)
                    {
                        employeeInDepartmentList.Add(new EmployeeDepartmentModel
                        {
                            ID = item.ID,
                            DepartmentId = item.Department.LookupId,
                            FullName = item.FullName,
                            LocationId = item.FactoryLocation.LookupId,
                            EmployeeId = item.EmployeeID,
                            DepartmentName = departmentName
                        });
                    }
                }
                return employeeInDepartmentList;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                    new SPDiagnosticsCategory("STADA -Employee Service - GetEmployeeListInCurrentDepartment fn",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Get employee list in departments
        /// </summary>
        /// <param name="departmentIds"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        /// CALL URL: _vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeListInDepartments/3;4;5/2
        public List<EmployeeDepartmentModel> GetEmployeeListInDepartments(string departmentIds, string locationId)
        {
            List<EmployeeDepartmentModel> employeeInDepartmentList = new List<EmployeeDepartmentModel>();
            List<EmployeeInfo> employeeInfoList = new List<EmployeeInfo>();
            try
            {
                int locId = int.Parse(locationId);
                var lcid = CultureInfo.CurrentUICulture.LCID;
                if (locId > 0)
                {
                    string[] viewFields = new string[] { EmployeeInfoList.EmployeeIDField, EmployeeInfoList.FullNameField, EmployeeInfoList.DepartmentField, EmployeeInfoList.FactoryLocationField };
                    var employees = _employeeInfoDAL.GetByDepartments(departmentIds.SplitStringOfLocations().ConvertAll(e => Convert.ToInt32(e)), locId, true, viewFields);
                    List<Department> departments = _departmentDAL.GetAll(SPContext.Current.Web);
                    foreach (var item in employees)
                    {
                        var departmentObj = departments.Where(e => e.ID == item.Department.LookupId).First();
                        string departmentName = lcid == 1066 ? departmentObj.VietnameseName : departmentObj.Name;

                        employeeInDepartmentList.Add(new EmployeeDepartmentModel
                        {
                            ID = item.ID,
                            DepartmentId = item.Department.LookupId,
                            FullName = item.FullName,
                            LocationId = item.FactoryLocation.LookupId,
                            EmployeeId = item.EmployeeID,
                            DepartmentName = departmentName
                        });
                    }
                }
                if (employeeInDepartmentList != null && employeeInDepartmentList.Count > 0)
                {
                    employeeInDepartmentList = employeeInDepartmentList.OrderBy(e => e.FullName).ToList();
                }
                return employeeInDepartmentList;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                    new SPDiagnosticsCategory("STADA -Employee Service - GetEmployeeListInDepartments fn",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public EmployeeModel GetEmployeeByEmployeeID(string id)
        {
            var employee = new EmployeeModel();
            try
            {
                var empItem = _employeeInfoDAL.GetByID(Convert.ToInt32(id));
                if (empItem != null)
                {
                    employee.ID = empItem.ID;
                    employee.EmployeeType = empItem.EmployeeType;
                    employee.FullName = empItem.FullName;
                    employee.EmployeeLevel = empItem.EmployeeLevel;
                    //ToDo: Add more property if need
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Employee Service - GetEmployeeByEmployeeID fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
            return employee;
        }

        /// <summary>
        /// Get active Eeployee by locationId & departmentId
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="departmentId"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        /// CALL URL: _vti_bin/Services/Employee/EmployeeService.svc/GetByDepartmentLocation/1/1
        public List<EmployeeModel> GetByDepartmentLocation(string locationId, string departmentId, string maxLevel)
        {
            try
            {
                List<EmployeeModel> employees = new List<EmployeeModel>();
                int locationIdValue, departmentIdValue;
                int level = Convert.ToInt32(maxLevel);
                if (int.TryParse(locationId, out locationIdValue) && int.TryParse(departmentId, out departmentIdValue))
                {
                    string[] viewFields = new string[] { EmployeeInfoList.EmployeeIDField, EmployeeInfoList.FullNameField, EmployeeInfoList.DepartmentField, EmployeeInfoList.FactoryLocationField, EmployeeInfoList.EmployeeTypeField, EmployeeInfoList.DepartmentPermissionField };
                    var employeeList = _employeeInfoDAL.GetByLocationAndDepartment(locationIdValue, departmentIdValue, true, level, StringConstant.EmployeeInfoList.FullNameField, viewFields);
                    if (employeeList != null)
                    {
                        foreach (var item in employeeList)
                        {
                            employees.Add(new EmployeeModel
                            {
                                EmployeeID = item.EmployeeID,
                                ID = item.ID,
                                Department = item.Department,
                                Location = item.FactoryLocation,
                                DepartmentPermission = item.DepartmentPermission,
                                FullName = item.FullName,
                                EmployeeType = item.EmployeeType
                            });
                        }
                    }
                }
                return employees;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Employee Service - GetByDepartmentLocation fn",
                     TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                 string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }

        }

        /// <summary>
        /// Get approvers of empployee by passing Id (employee list item ID)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// CALL URL: _vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeApprovers/1
        public EmployeeApproverModel GetEmployeeApprovers(string id)
        {
            try
            {
                EmployeeApproverModel employeeApprover = new EmployeeApproverModel
                {
                    EmployeeIdentity = Convert.ToInt16(id),
                };
                var currentEmployee = _employeeInfoDAL.GetByID(Convert.ToInt16(id));
                if (currentEmployee != null)
                {
                    var groupLeaders = _employeeInfoDAL.GetByPositionDepartment(StringConstant.EmployeePosition.GroupLeader, currentEmployee.Department.LookupId, currentEmployee.FactoryLocation.LookupId);
                    if (groupLeaders != null)
                    {
                        foreach (var groupLeader in groupLeaders)
                        {
                            employeeApprover.Approver1.Add(new ApproverModel
                            {
                                LoginName = groupLeader.ADAccount.UserName,
                                FullLoginName = groupLeader.FullName
                            });
                        }
                    }

                    var department = _departmentDAL.GetByID(currentEmployee.Department.LookupId);
                    var accountList = _employeeInfoDAL.GetByPositionDepartment(StringConstant.EmployeePosition.DepartmentHead, department.ID, currentEmployee.FactoryLocation.LookupId);
                    if (accountList.Count > 0)
                    {
                        var DPH = accountList.FirstOrDefault();
                        employeeApprover.Approver2 = new ApproverModel
                        {
                            LoginName = DPH.ADAccount.UserName,
                            FullLoginName = DPH.FullName
                        };
                    }

                    if (department != null && department.BOD != null)
                    {
                        employeeApprover.Approver3 = new ApproverModel
                        {
                            LoginName = department.BOD.UserName,
                            FullLoginName = department.BOD.FullName
                        };
                    }

                }
                return employeeApprover;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Employee Service - GetEmployeeApprovers fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public bool IsManager(string employeeId)
        {
            var currentEmployee = _employeeInfoDAL.GetByID(Convert.ToInt16(employeeId));
            var positionId = currentEmployee.EmployeePosition.LookupId;
            return positionId == (int)StringConstant.EmployeePosition.GroupLeader || positionId == (int)StringConstant.EmployeePosition.BOD || positionId == (int)StringConstant.EmployeePosition.DepartmentHead
                || positionId == (int)StringConstant.EmployeePosition.DirectManagement;
        }
        public bool IsUserCurrentuserInGroup(string groupName)
        {
            try
            {
                return _employeeInfoDAL.IsUserCurrentuserInGroup(groupName);
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Employee Service - IsUserCurrentuserInGroup fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }


        /// <summary>
        /// Get employee list that dont have any overtime in passing date of passing department
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="locationId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// CALL URL:  _vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeListDontHaveOvertimeInDate/1/4-21-2017
        public List<EmployeeModel> GetEmployeeListDontHaveOvertimeInDate(string departmentId, string locationId, string date)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(date);

                var fromDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                var toDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

                List<EmployeeModel> employeeDontHaveOvertimeList = new List<EmployeeModel>();
                string maxLevel = "6"; // Quản lý trực tiếp -> Exclude 'BOD'

                //List<EmployeeDepartmentModel> employeeIncurrentDepartmentList = GetEmployeeListInCurrentDepartment(departmentId);
                List<EmployeeModel> employeeIncurrentDepartmentList = GetByDepartmentLocation(locationId, departmentId, maxLevel);

                var overtimeManagements = _overtimeManagementDAL.GetByDepartmentInRange(Convert.ToInt32(departmentId), Convert.ToInt32(locationId), fromDate, toDate);

                if (overtimeManagements.Count > 0)
                {
                    // Overtime
                    List<int> employeesHaveOvertimeList = new List<int>();
                    foreach (var overtime in overtimeManagements)
                    {
                        var overtimeDetailList = _overtimeManagementDetailDAL.GetByOvertimeId(overtime.ID);
                        if (overtimeDetailList.Any())
                        {
                            foreach (var overtimeDetailItem in overtimeDetailList)
                            {
                                employeesHaveOvertimeList.Add(overtimeDetailItem.Employee.LookupId);
                            }
                        }
                    }

                    // Rejected
                    var overtimesRejected = overtimeManagements.Where(x => x.ApprovalStatus == "false");
                    List<int> employeesRejected = new List<int>();
                    foreach (var overtime in overtimesRejected)
                    {
                        var overtimeDetailList = _overtimeManagementDetailDAL.GetByOvertimeId(overtime.ID);
                        if (overtimeDetailList.Any())
                        {
                            foreach (var overtimeDetailItem in overtimeDetailList)
                            {
                                employeesRejected.Add(overtimeDetailItem.Employee.LookupId);
                            }
                        }
                    }

                    var employeesRejectedModel = employeeIncurrentDepartmentList.Where(i => employeesRejected.Contains(i.ID));

                    var employeesNotOvertimeModel = employeeIncurrentDepartmentList.Where(i => !employeesHaveOvertimeList.Contains(i.ID));
                    // Result = Not registered overtime + Rejected
                    employeeDontHaveOvertimeList = employeesNotOvertimeModel.Concat(employeesRejectedModel).ToList();
                }
                else
                {
                    employeeDontHaveOvertimeList = employeeIncurrentDepartmentList;
                }

                return employeeDontHaveOvertimeList;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Employee Service - GetEmployeeListDontHaveOvertimeInDate fn",
                   TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
               string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }
        /// <summary>
        /// Get avatar image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetAvatar(string id)
        {
            try
            {
                int itemId = Convert.ToInt32(id);
                string imageUrl = string.Empty;
                var employee = _employeeInfoDAL.GetByID(itemId);
                if (employee != null)
                {
                    imageUrl = employee.Image;
                }
                return imageUrl;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Employee Service - GetAvatar fn",
                   TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
               string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<PeoplePickerUser> GetPeoplePickerData(string SearchString)
        {
            var currentUser = GetCurrentUser();
            var userPickers = new List<PeoplePickerUser>();
            var employeeeList = _employeeInfoDAL.GetAccountByFullNameOrId(SearchString, currentUser.Department.LookupValue);
            foreach (var employeee in employeeeList)
            {
                var peoplePickerUser = new PeoplePickerUser();
                peoplePickerUser.Email = employeee.Email;
                peoplePickerUser.Login = employeee.ADAccount.UserName;
                peoplePickerUser.Name = employeee.FullName;
                peoplePickerUser.ID = employeee.ID.ToString();
                userPickers.Add(peoplePickerUser);
            }
            return userPickers;
        }

        public List<PeoplePickerUser> PostPeoplePickerData(PeoplePickerDataRequest peoplePickerDataRequest)
        {
            var currentUser = GetCurrentUser();
            var userPickers = new List<PeoplePickerUser>();
            var employeeeList = new List<EmployeeInfo>();
            if (currentUser.EmployeePosition == (int)StringConstant.EmployeePosition.DepartmentHead || currentUser.EmployeePosition == (int)StringConstant.EmployeePosition.BOD)
            {
                employeeeList = _employeeInfoDAL.GetAccountByFullNamePosition(peoplePickerDataRequest.Name, string.Empty, peoplePickerDataRequest.EmployeePositions);
            }
            else
            {
                employeeeList = _employeeInfoDAL.GetAccountByFullNamePosition(peoplePickerDataRequest.Name, currentUser.Department.LookupValue, peoplePickerDataRequest.EmployeePositions);
            }
            foreach (var employeee in employeeeList)
            {
                var peoplePickerUser = new PeoplePickerUser();
                peoplePickerUser.Email = employeee.Email;
                peoplePickerUser.Login = employeee.ADAccount.UserName;
                peoplePickerUser.Name = employeee.FullName;
                peoplePickerUser.ID = employeee.ID.ToString();
                userPickers.Add(peoplePickerUser);
            }
            return userPickers;
        }

        public List<PeoplePickerUser> GetAdditionalApprovers(PeoplePickerDataRequest peoplePickerDataRequest)
        {
            var userPickers = new List<PeoplePickerUser>();

            if (peoplePickerDataRequest.EmployeePositions != null && peoplePickerDataRequest.EmployeePositions.Count > 0)
            {
                EmployeeInfo approver = _employeeInfoDAL.GetByID(Convert.ToInt32(peoplePickerDataRequest.EmployeePositions[0].PositionName));
                if (approver != null)
                {
                    var retObj = new List<EmployeeInfo>();
                    if (approver.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.BOD)
                    {
                        retObj = _employeeInfoDAL.GetAccountByFullNamePositionDepartment(peoplePickerDataRequest.Name, new List<int>() { (int)StringConstant.EmployeePosition.BOD }, null);
                    }
                    else if (approver.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead)
                    {
                        retObj = _employeeInfoDAL.GetAccountByFullNamePositionDepartment(peoplePickerDataRequest.Name, new List<int>() { (int)StringConstant.EmployeePosition.GroupLeader }, approver.Department.LookupId);
                    }
                    else if (approver.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.TeamLeader)
                    {
                        retObj = _employeeInfoDAL.GetAccountByFullNamePositionDepartment(peoplePickerDataRequest.Name, new List<int>() { (int)StringConstant.EmployeePosition.AssociateTeamLeader }, approver.Department.LookupId);
                    }
                    else if (approver.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.ShiftLeader)
                    {
                        retObj = _employeeInfoDAL.GetAccountByFullNamePositionDepartment(peoplePickerDataRequest.Name, new List<int>() { (int)StringConstant.EmployeePosition.ShiftLeader }, approver.Department.LookupId);
                    }

                    foreach (var additionalApprover in retObj)
                    {
                        var peoplePickerUser = new PeoplePickerUser();
                        peoplePickerUser.Email = additionalApprover.Email;
                        peoplePickerUser.Login = additionalApprover.ADAccount.UserName;
                        peoplePickerUser.Name = additionalApprover.FullName;
                        peoplePickerUser.ID = additionalApprover.ID.ToString();
                        userPickers.Add(peoplePickerUser);
                    }
                }
            }

            return userPickers;
        }

        public EmployeeModel GetEmployeeInfoByADAccount(string adAccountId)
        {
            try
            {
                var user = new EmployeeModel();
                var employeeDal = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                var currentEmployee = employeeDal.GetByADAccount(Convert.ToInt32(adAccountId));
                if (currentEmployee != null)
                {
                    user = new EmployeeModel()
                    {
                        ID = currentEmployee.ID,
                        EmployeeID = currentEmployee.EmployeeID,
                        //Department = currentEmployee.Department,
                        //Location = currentEmployee.FactoryLocation,
                        FullName = currentEmployee.FullName
                    };
                }
                return user;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                  new SPDiagnosticsCategory("STADA - Employee Service - GetCurrentUser fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public bool IsMemberOfDepartment(string employeeId, string departmentId)
        {
            try
            {
                var employeeDal = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                var employee = employeeDal.GetByDepartment(Convert.ToInt32(employeeId), Convert.ToInt32(departmentId), true); //get user active
                if (employee != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                  new SPDiagnosticsCategory("STADA - Employee Service - IsMemberOfDepartment fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public bool IsMemberOfDepartmentByCode(string employeeId, string departmentCode)
        {
            try
            {
                var departmentDal = new DepartmentDAL(SPContext.Current.Web.Url);
                var employeeDal = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                var department = departmentDal.GetByCode(departmentCode);
                if (department != null)
                {
                    var employee = employeeDal.GetByDepartment(Convert.ToInt32(employeeId), Convert.ToInt32(department.ID), true); //get user active
                    if (employee != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                  new SPDiagnosticsCategory("STADA - Employee Service - IsMemberOfDepartmentByCode fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Get manager list with min level
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="minLevel"></param>
        /// <returns></returns>
        /// URL: _vti_bin/Services/Employee/EmployeeService.svc/GetManagerByDepartment/5/4
        public List<EmployeeModel> GetManagerByDepartment(string departmentId, string minLevel)
        {
            try
            {
                List<EmployeeModel> employees = new List<EmployeeModel>();
                var employeePositions = _employeePositionDAL.GetByMinLevel(double.Parse(minLevel, System.Globalization.CultureInfo.InvariantCulture));
                if (employeePositions != null && employeePositions.Count() > 0)
                {
                    var employeePositionIds = employeePositions.Select(x => x.ID).ToList();
                    var employeeList = _employeeInfoDAL.GetByUserType(true, StringConstant.EmployeeType.ADUser);

                    ////Filter by min level
                    employeeList = employeeList.Where(x => x.Department == null ||  (x.EmployeeLevel != null && employeePositionIds.Contains(x.EmployeeLevel.LookupId))).ToList();


                    if (!string.IsNullOrEmpty(departmentId) && Convert.ToInt32(departmentId) > 0)
                    {
                        employeeList = employeeList.Where(x => x.Department == null || (x.Department != null && (x.Department.LookupId == 0 || x.Department.LookupId == Convert.ToInt32(departmentId)))).ToList();
                    }

                    foreach (var item in employeeList)
                    {
                        employees.Add(new EmployeeModel
                        {
                            EmployeeID = item.EmployeeID,
                            ID = item.ID,
                            Department = item.Department,
                            Location = item.FactoryLocation,
                            DepartmentPermission = item.DepartmentPermission,
                            FullName = item.FullName,
                            EmployeeType = item.EmployeeType
                        });
                    }
                }
                return employees;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Employee Service - GetManagerByDepartment fn",
                     TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                 string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<EmployeeModel> GetAllEmployees()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            try
            {
                var employeeList = _employeeInfoDAL.GetAll().Where(x => x.IsActive).OrderBy(y => y.FullName).ToList();
                foreach (var item in employeeList)
                {
                    employees.Add(new EmployeeModel
                    {
                        EmployeeID = item.EmployeeID,
                        ID = item.ID,
                        Department = item.Department,
                        Location = item.FactoryLocation,
                        DepartmentPermission = item.DepartmentPermission,
                        FullName = item.FullName,
                        EmployeeType = item.EmployeeType
                    });
                }
                return employees;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - GetAllEmployees - GetManagerByDepartment fn",
                     TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                 string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }
    }
}