using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DTO
{
    public class ExcelShiftsOfDepartment
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int DepartmentId { get; set; }
        public int Location { get; set; }
        public List<ExcelEmployeeShift> EmployeeShifts { get; set; }

        public ExcelShiftsOfDepartment()
        {
            EmployeeShifts = new List<ExcelEmployeeShift>();
        }

        public ExcelShiftsOfDepartment(int month, int year) : this()
        {
            Month = month;
            Year = year;
        }

        public ExcelShiftsOfDepartment(int month, int year, int deptId) : this(month, year)
        {
            DepartmentId = deptId;
        }
    }

    public class ExcelEmployeeShift
    {
        public int EmployeeLookupID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeFullName { get; set; }
        public List<string> ShiftCodes { get; set; }

        public ExcelEmployeeShift()
        {
            this.ShiftCodes = new List<string>();
        }

        public ExcelEmployeeShift(string employeeID, string employeeFullName) : this()
        {
            EmployeeID = employeeID;
            EmployeeFullName = employeeFullName;
        }
    }
}