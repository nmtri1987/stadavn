using System;
using System.Web;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using System.Collections.Generic;
using System.Linq;

namespace RBVH.Stada.Intranet.WebPages.Utils
{
    public static class UserPermission
    {
        public static bool IsAdminDepartment
        {
            get
            {
                //EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                if (currentEmployee != null && currentEmployee.ID > 0)
                {
                    var adminDept = DepartmentListSingleton.GetDepartmentByCode("HR", SPContext.Current.Web.Url);
                    var adminDeptId = adminDept != null ? adminDept.ID : 0;
                    // 2: Administration & Human Resource
                    if (currentEmployee.Department != null && currentEmployee.Department.LookupId == adminDeptId)
                    {
                        // Admin/Manager
                        if (currentEmployee.DepartmentPermission != null && (currentEmployee.DepartmentPermission.ToLower().Equals("administrators")
                            || currentEmployee.DepartmentPermission.ToLower().Equals("contributors")))
                        {
                            return true;
                        }
                    }
                    else if (IsCurrentUserInGroup(StringConstant.BOD))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static bool IsAdminOfHRDepartment
        {
            get
            {
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                if (currentEmployee != null && currentEmployee.ID > 0)
                {
                    var adminDept = DepartmentListSingleton.GetDepartmentByCode("HR", SPContext.Current.Web.Url);
                    var adminDeptId = adminDept != null ? adminDept.ID : 0;
                    // 2: Administration & Human Resource
                    if (currentEmployee.Department != null && currentEmployee.Department.LookupId == adminDeptId)
                    {
                        // Admin/Manager
                        if (currentEmployee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public static bool IsUserAdminDepartment
        {
            get
            {
                //EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                if (currentEmployee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.Administrator)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool HasShiftCreation
        {
            get
            {
                //EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                if (currentEmployee != null && currentEmployee.Department != null && currentEmployee.Department.LookupId > 0)
                {
                    var department = DepartmentListSingleton.GetDepartmentByID(currentEmployee.Department.LookupId, SPContext.Current.Web.Url);
                    return department != null && department.IsShiftRequestRequired;
                }

                return false;
            }
        }

        public static EmployeeRole CurrentUserRole
        {
            get
            {
                //EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                if (currentEmployee != null && currentEmployee.ID > 0)
                {
                    var adminDept = DepartmentListSingleton.GetDepartmentByCode("HR", SPContext.Current.Web.Url);
                    var adminDeptId = adminDept != null ? adminDept.ID : 0;
                    // 2: Administration & Human Resource
                    if (currentEmployee.Department != null && currentEmployee.Department.LookupId == adminDeptId)
                    {
                        if (currentEmployee.DepartmentPermission != null)
                        {
                            if (currentEmployee.DepartmentPermission.ToLower().Equals("administrators"))
                                return EmployeeRole.AdminOfHR;
                            else if (currentEmployee.DepartmentPermission.ToLower().Equals("contributors"))
                                return EmployeeRole.DepartmentHeadOfHR;
                        }
                    }
                    else if (currentEmployee.EmployeePosition.LookupValue == StringConstant.EmployeePositionName.DepartmentHead)
                    {
                        return EmployeeRole.DepartmentHead;
                    }
                    else if (IsCurrentUserInGroup(StringConstant.BOD))
                    {
                        return EmployeeRole.BOD;
                    }
                }

                return EmployeeRole.Staff;
            }
        }

        public static bool IsBOD
        {
            get
            {
                return IsCurrentUserInGroup(StringConstant.BOD);
            }
        }

        public static int CurrentDepartmentId
        {
            get
            {
                //EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                if (currentEmployee != null && currentEmployee.ID > 0)
                {
                    if (currentEmployee.Department != null)
                    {
                        return currentEmployee.Department.LookupId;
                    }
                }
                return 0;
            }
        }

        public static string DepartmentsToView
        {
            get
            {
                List<int> deptList = new List<int>();
                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                UserHelper userHelper = new UserHelper();
                var currentEmployee = userHelper.GetCurrentLoginUser();
                if (currentEmployee != null && currentEmployee.ID > 0)
                {
                    if (currentEmployee.Department != null)
                    {
                        deptList.Add(currentEmployee.Department.LookupId);
                        var deptPlan = DepartmentListSingleton.GetDepartmentByCode("PLA", SPContext.Current.Web.Url);
                        if (currentEmployee.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.DepartmentHead &&
                            currentEmployee.Department.LookupId == deptPlan.ID)
                        {
                            var deptWareHouse = DepartmentListSingleton.GetDepartmentByCode("WH", SPContext.Current.Web.Url);
                            if (deptWareHouse != null)
                            {
                                deptList.Add(deptWareHouse.ID);
                            }
                        }
                    }
                }

                if (deptList.Count == 0)
                {
                    deptList.Add(0);
                }

                return string.Join("#", deptList);
            }
        }

        public static EmployeeRole GetCurrentUserRole(EmployeeInfo currentEmployee)
        {
            if (currentEmployee != null && currentEmployee.ID > 0)
            {
                var adminDept = DepartmentListSingleton.GetDepartmentByCode("HR", SPContext.Current.Web.Url);
                var adminDeptId = adminDept != null ? adminDept.ID : 0;
                // 2: Administration & Human Resource
                if (currentEmployee.Department != null && currentEmployee.Department.LookupId == adminDeptId)
                {
                    if (currentEmployee.DepartmentPermission != null)
                    {
                        if (currentEmployee.DepartmentPermission.ToLower().Equals("administrators"))
                            return EmployeeRole.AdminOfHR;
                        else if (currentEmployee.DepartmentPermission.ToLower().Equals("contributors"))
                            return EmployeeRole.DepartmentHeadOfHR;
                    }
                }
                else if (currentEmployee.EmployeePosition.LookupValue == StringConstant.EmployeePositionName.DepartmentHead)
                {
                    return EmployeeRole.DepartmentHead;
                }
                else if (IsCurrentUserInGroup(StringConstant.BOD))
                {
                    return EmployeeRole.BOD;
                }
            }

            return EmployeeRole.Staff;
        }

        public static bool IsCurrentUserInGroup(string groupName)
        {
            //Check groupName is valid
            if (string.IsNullOrEmpty(groupName) || groupName.Length > 255)
                return false;
            var isMember = false;

            var siteID = SPContext.Current.Web.Site.ID;
            var currentUser = SPContext.Current.Web.CurrentUser;

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (var site = new SPSite(siteID))
                {
                    using (var web = site.RootWeb)
                    {
                        try
                        {
                            var group = web.SiteGroups.GetByName(groupName);
                            var user = group.Users.GetByLoginNoThrow(currentUser.LoginName);
                            if (user != null)
                            {
                                isMember = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            isMember = false;
                            ULSLogging.LogError(ex);
                        }
                    }
                }
            });
            return isMember;
        }

        public static EmployeeInfo GetEmployeeInfo()
        {
            if (HttpContext.Current.Session != null)
            {
                return HttpContext.Current.Session[StringConstant.EmployeeLogedin] as EmployeeInfo;
            }
            else
            {
                return null;
            }
        }

        public static void SetEmployeeInfo(EmployeeInfo employeeInfo)
        {
            HttpContext.Current.Session[StringConstant.EmployeeLogedin] = employeeInfo;
        }
    }
}