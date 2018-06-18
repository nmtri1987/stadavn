using System;
using System.Web;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.WebPages.Utils
{
    public class UserHelper
    {
        public EmployeeInfo GetCurrentLoginUser()
        {
            try
            {
                SPUser spUser = SPContext.Current.Web.CurrentUser;
                var employeeDal = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                EmployeeInfo currentStadaEmployee =
                    HttpContext.Current.Session[StringConstant.EmployeeLogedin] as EmployeeInfo;

                //User is not common account, we should get from employee list
                if (currentStadaEmployee == null)
                {
                    if (spUser != null)
                    {
                        currentStadaEmployee = employeeDal.GetByADAccount(spUser.ID);
                    }
                }
                return currentStadaEmployee;
            }
            catch (Exception)
            { 
                return null;
            }
        }

        public bool IsEmployeeHasPosition(int employeeItemId, StringConstant.EmployeePosition posision)
        {
            try
            {
                var employeeDal = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                var employee = employeeDal.GetByID(employeeItemId);
                if(employee != null)
                {
                    return employee.EmployeePosition.LookupId == (int)posision;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
