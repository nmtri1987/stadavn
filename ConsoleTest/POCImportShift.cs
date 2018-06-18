using DocumentFormat.OpenXml.Packaging;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RBVH.Stada.Intranet.ConsoleTest
{
    public class POCImportShift
    {
        public void ImportExcel()
        {
            ExcelShiftsOfDepartment excelShiftsOfDepartment = ReadExcelShiftData();
            excelShiftsOfDepartment.DepartmentId = 2; //HR
            excelShiftsOfDepartment.Location = 2;
            excelShiftsOfDepartment.Month = 7;
            excelShiftsOfDepartment.Year = 2017;

            Dictionary<string, int> duplicatedEmployees = ValidateDuplicatedData(excelShiftsOfDepartment);

            string siteUrl = "http://rbvhspdev01:81/";

            ShiftManagementDAL _shiftManagementDAL = new ShiftManagementDAL(siteUrl);
            ShiftManagementDetailDAL _shiftManagementDetailDAL = new ShiftManagementDetailDAL(siteUrl);

            List<ShiftManagement> shiftManagements = _shiftManagementDAL.GetByMonthYearDepartment(excelShiftsOfDepartment.Month, excelShiftsOfDepartment.Year, excelShiftsOfDepartment.DepartmentId, excelShiftsOfDepartment.Location);
            List<ShiftManagementDetail> shiftManagementDetails = new List<ShiftManagementDetail>();

            if (shiftManagements != null && shiftManagements.Count > 0)
            {
                shiftManagementDetails = _shiftManagementDetailDAL.GetByShiftManagementID(shiftManagements[0].ID);
            }

            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
            List<EmployeeInfo> employeesOfDepartment = _employeeInfoDAL.GetByLocationAndDepartment(2, excelShiftsOfDepartment.DepartmentId, true, 4, StringConstant.EmployeeInfoList.EmployeeIDField);

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

                List<int> employeeLookupIds = excelShiftsOfDepartment.EmployeeShifts.Select(e => e.EmployeeLookupID).ToList();
                shiftManagementDetails = shiftManagementDetails.Where(e => employeeLookupIds.Contains(e.Employee.LookupId)).ToList();
            }

            foreach (ExcelEmployeeShift excelEmployeeShift in excelShiftsOfDepartment.EmployeeShifts)
            {
                ShiftManagementDetail shiftManagementDetail = shiftManagementDetails.Where(e => e.Employee.LookupId == excelEmployeeShift.EmployeeLookupID).FirstOrDefault();
                if (shiftManagementDetail == null)
                {
                    shiftManagementDetail = new ShiftManagementDetail();
                    EmployeeInfo employeeInfo = employeesOfDepartment.Where(e => e.EmployeeID == excelEmployeeShift.EmployeeID).FirstOrDefault();
                    shiftManagementDetail.Employee = new LookupItem() { LookupId = employeeInfo.ID, LookupValue = employeeInfo.FullName };
                    shiftManagementDetails.Add(shiftManagementDetail);
                }

                ShiftTimeDAL _shiftTimeDAL = new ShiftTimeDAL(siteUrl);
                List<ShiftTime> shiftTimes = _shiftTimeDAL.GetAll();

                for (int i = 0; i < 11; i++)
                {
                    ShiftTime shiftTime = shiftTimes.Where(e => e.Code.ToUpper() == excelEmployeeShift.ShiftCodes[i].ToUpper()).FirstOrDefault();
                    if (shiftTime != null)
                    {
                        Type type = typeof(ShiftManagementDetail);
                        PropertyInfo shiftApprovalInfo = type.GetProperty(string.Format("ShiftTime{0}Approval", i + 21));
                        object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftManagementDetail, null);
                        if (shiftApprovalValue != null && Convert.ToBoolean(shiftApprovalValue) == false)
                        {
                            PropertyInfo shiftInfo = type.GetProperty(string.Format("ShiftTime{0}", i + 21));
                            LookupItem shiftValue = shiftInfo.GetValue(shiftManagementDetail, null) as LookupItem;
                            if (shiftValue == null)
                            {
                                shiftValue = new LookupItem();
                            }
                            shiftValue.LookupId = shiftTime.ID;
                            shiftValue.LookupValue = shiftTime.Code;
                        }
                    }
                }

                for (int i = 11; i < 31; i++)
                {
                    ShiftTime shiftTime = shiftTimes.Where(e => e.Code.ToUpper() == excelEmployeeShift.ShiftCodes[i].ToUpper()).FirstOrDefault();
                    if (shiftTime != null)
                    {
                        Type type = typeof(ShiftManagementDetail);
                        PropertyInfo shiftApprovalInfo = type.GetProperty(string.Format("ShiftTime{0}Approval", i - 10));
                        object shiftApprovalValue = shiftApprovalInfo.GetValue(shiftManagementDetail, null);
                        if (shiftApprovalValue != null && Convert.ToBoolean(shiftApprovalValue) == false)
                        {
                            PropertyInfo shiftInfo = type.GetProperty(string.Format("ShiftTime{0}", i - 10));
                            LookupItem shiftValue = shiftInfo.GetValue(shiftManagementDetail, null) as LookupItem;
                            if (shiftValue == null)
                            {
                                shiftValue = new LookupItem();
                            }
                            shiftValue.LookupId = shiftTime.ID;
                            shiftValue.LookupValue = shiftTime.Code;
                        }
                    }
                }
            }

            int shiftId = 0;
            if (shiftManagements == null || shiftManagements.Count == 0)
            {
                Biz.Models.ShiftManagement shiftManagement = new Biz.Models.ShiftManagement();
                shiftManagement.Department = new LookupItem() { LookupId = excelShiftsOfDepartment.DepartmentId, LookupValue = excelShiftsOfDepartment.DepartmentId.ToString() };
                shiftManagement.Location = new LookupItem() { LookupId = excelShiftsOfDepartment.Location, LookupValue = excelShiftsOfDepartment.Location.ToString() };
                shiftManagement.Month = excelShiftsOfDepartment.Month;
                shiftManagement.Year = excelShiftsOfDepartment.Year;

                EmployeeInfo requester = _employeeInfoDAL.GetByADAccount(122);
                shiftManagement.Requester = new LookupItem() { LookupId = requester.ID, LookupValue = requester.FullName };

                EmployeeInfo approvedBy = _employeeInfoDAL.GetByPositionDepartment(StringConstant.EmployeePosition.DepartmentHead, excelShiftsOfDepartment.DepartmentId, excelShiftsOfDepartment.Location).FirstOrDefault();
                shiftManagement.ApprovedBy = new User() { ID = approvedBy.ADAccount.ID, UserName = approvedBy.ADAccount.UserName, FullName = approvedBy.FullName, IsGroup = false };

                shiftId = _shiftManagementDAL.SaveOrUpdate(shiftManagement);
            }
            else
            {
                shiftId = shiftManagements[0].ID;
            }

            if (shiftId > 0)
            {
                foreach (ShiftManagementDetail shiftManagementDetail in shiftManagementDetails)
                {
                    shiftManagementDetail.ShiftManagementID = new LookupItem() { LookupId = shiftId, LookupValue = shiftId.ToString() };
                    int shiftDetailId = _shiftManagementDetailDAL.SaveOrUpdate(shiftManagementDetail);
                }
            }
        }

        public Dictionary<string, int> ValidateDuplicatedData(ExcelShiftsOfDepartment excelShiftsOfDepartment)
        {
            Dictionary<string, int> duplicatedEmployees = excelShiftsOfDepartment.EmployeeShifts.GroupBy(e => e.EmployeeID).Where(x => x.Count() > 1).ToDictionary(x => x.Key, y => y.Count());

            return duplicatedEmployees;
        }

        public ExcelShiftsOfDepartment ReadExcelShiftData()
        {
            ExcelShiftsOfDepartment excelShiftsOfDepartment = new ExcelShiftsOfDepartment();

            string excelTemplateFileName = "HC_2017_07_2.xlsx";
            string excelTemplateFilePath = Path.Combine(Application.StartupPath, excelTemplateFileName);

            using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Open(excelTemplateFileName, false))
            {
                var sheetName = "Sheet1";
                var workbookPart = spreadSheetDoc.WorkbookPart;

                //excelShiftsOfDepartment.Month = int.Parse(ExcelHelper.GetCellValue(workbookPart, sheetName, "B8").Trim());
                //excelShiftsOfDepartment.Year = int.Parse(ExcelHelper.GetCellValue(workbookPart, sheetName, "B9").Trim());

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

                        for (var colIdx = 3; colIdx <= 33; colIdx++)
                        {
                            string cellAddress = string.Format("{0}{1}", ExcelHelper.ConvertColumnIndexToLetter(colIdx), rowIdx);
                            string cellValue = ExcelHelper.GetCellValue(workbookPart, sheetName, cellAddress);

                            excelEmployeeShift.ShiftCodes.Add(cellValue.ToUpper().Trim());
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
    }
}
