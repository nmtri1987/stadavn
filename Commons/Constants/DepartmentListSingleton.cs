using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Constants
{
    public sealed class DepartmentListSingleton
    {
        private static IList<Department> departments = null;
        private static DepartmentDAL departmentDAL;
        private DepartmentListSingleton()
        {

        }
        public static IList<Department> DepartmentListInstance(string siteURL)
        {
            // TODO: not thread-safe, should lock object to ensure thread-safe
            if (departments == null)
            {
                departmentDAL = new DepartmentDAL(siteURL);
                departments = departmentDAL.GetAll();
            }
            return departments;
        }
        public static Department GetDepartmentByName(string name, string siteURL)
        {
            var department = DepartmentListInstance(siteURL).Where(x => x.Name == name).SingleOrDefault();
            
            return department;
        }
        public static Department GetDepartmentByID(int Id, string siteURL)
        {
            var department = DepartmentListInstance(siteURL).Where(x => x.ID == Id).SingleOrDefault();
            return department;
        }
        public static Department GetDepartmentByCode(string code, string siteURL)
        {
            var department = DepartmentListInstance(siteURL).Where(x => x.Code == code).SingleOrDefault();
            return department;
        }
        public static void ResetDepartmentListInstance()
        {
            departments = null;
        }
    }
}
