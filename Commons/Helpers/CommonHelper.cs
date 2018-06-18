using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Helpers
{
    public class CommonHelper
    {
        public static string BuildFilterCommonMultiLocations(List<int> locationIds)
        {
            var locationFilter = "";
            for (int i = 0; i < locationIds.Count; i++)
            {
                if (i == 0)
                {
                    locationFilter = string.Format("<Eq><FieldRef Name='CommonMultiLocations' LookupId='TRUE'/><Value Type='LookupMulti'>{0}</Value></Eq>", locationIds[i]);
                }
                else
                {
                    locationFilter = string.Format("<Or><Eq><FieldRef Name='CommonMultiLocations' LookupId='TRUE'/><Value Type='LookupMulti'>{0}</Value></Eq>{1}</Or>", locationIds[i], locationFilter);
                }
            }

            return locationFilter;
        }

        public static string BuildFilterCommonLocation(List<int> locationIds)
        {
            var locationFilter = "";
            for (int i = 0; i < locationIds.Count; i++)
            {
                if (i == 0)
                {
                    locationFilter = string.Format("<Eq><FieldRef Name='CommonLocation' LookupId='TRUE'/><Value Type='Lookup'>{0}</Value></Eq>", locationIds[i]);
                }
                else
                {
                    locationFilter = string.Format("<Or><Eq><FieldRef Name='CommonLocation' LookupId='TRUE'/><Value Type='Lookup'>{0}</Value></Eq>{1}</Or>", locationIds[i], locationFilter);
                }
            }

            return locationFilter;
        }

        public static string BuildFilterCommonDepartment(List<int> departmentIds)
        {
            var departmentFilter = "";
            for (int i = 0; i < departmentIds.Count; i++)
            {
                if (i == 0)
                {
                    departmentFilter = string.Format("<Eq><FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE'/><Value Type='Lookup'>{0}</Value></Eq>", departmentIds[i]);
                }
                else
                {
                    departmentFilter = string.Format("<Or><Eq><FieldRef Name='EmployeeInfoDepartment' LookupId='TRUE'/><Value Type='Lookup'>{0}</Value></Eq>{1}</Or>", departmentIds[i], departmentFilter);
                }
            }

            return departmentFilter;
        }
    }
}
