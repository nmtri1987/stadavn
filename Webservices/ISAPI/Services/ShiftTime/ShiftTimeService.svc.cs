using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Activation;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.ShiftTime
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ShiftTimeService : IShiftTimeService
    {
        readonly ShiftTimeDAL _shiftTimeDAL;
        public ShiftTimeService()
        {
            _shiftTimeDAL = new ShiftTimeDAL(SPContext.Current.Web.Url);
        }
        
        public ShiftTimeModel GetShiftTimeInDate(string day, string month, string year, string departmentID, string locationID, string employeeID)
        {
            ShiftTimeModel shiftmodel = new ShiftTimeModel();
            try
            {
                var Day = int.Parse(day);
                var Month = int.Parse(month);
                var Year = int.Parse(year);
                var DepartmentId = int.Parse(departmentID);
                var LocationId = int.Parse(locationID);
                var EmployeeId = int.Parse(employeeID);
                shiftmodel = ConvertToShiftTimeModel(this._shiftTimeDAL.GetShiftTimeByDate(Day, Month, Year, DepartmentId, LocationId, EmployeeId));
                return shiftmodel;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Shift Time Service - GetShiftTimeInDate fn",
                                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }

        }

        private ShiftTimeModel ConvertToShiftTimeModel(Biz.Models.ShiftTime item)
        {
            return new ShiftTimeModel()
            {
                Id = item.ID,
                Code = item.Code,
                Name = item.Name,
                Depscription = item.Description,
                WorkingHourFromHour = item.WorkingHourFromHour.Hour,
                WorkingHourFromMinute = item.WorkingHourFromHour.Minute,
                WorkingHourToHour = item.WorkingHourToHour.Hour,
                WorkingHourToMinute = item.WorkingHourToHour.Minute,
                BreakHourFromHour = Convert.ToString(item.BreakHourFromHour.Hour),
                BreakHourFromMinute = Convert.ToString(item.BreakHourFromHour.Minute),
                BreakHourToHour = Convert.ToString(item.BreakHourToHour.Hour),
                BreakHourToMinute = Convert.ToString(item.BreakHourToHour.Minute),
                ShiftTimeWorkingHourNumber = item.ShiftTimeWorkingHourNumber,
                ShiftTimeBreakHourNumber = item.ShiftTimeBreakHourNumber,
                UnexpectedLeaveFirstApprovalRole = item.UnexpectedLeaveFirstApprovalRole,
                ShiftRequired = item.ShiftRequired
            };
        }
        /// <summary>
        /// Get all Shift Time
        /// </summary>
        /// <returns></returns>
        /// CALL URL: _vti_bin/Services/ShiftTime/ShiftTimeService.svc/GetShiftTimes
        public List<ShiftTimeModel> GetShiftTimes()
        {
            try
            {
                List<ShiftTimeModel> shiftTimes = new List<ShiftTimeModel>();
                var shiftTimeList = _shiftTimeDAL.GetShiftTimes();
                if (shiftTimeList.Any())
                {
                    foreach (var item in shiftTimeList)
                    {
                        shiftTimes.Add(
                         this.ConvertToShiftTimeModel(item)
                        );
                    }
                }
                return shiftTimes;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Shift Time Service - GetShiftTimes fn",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        ///// <summary>
        ///// Get leave hours in range
        ///// </summary>
        ///// <param name="fromDate"></param>
        ///// <param name="toDate"></param>
        ///// <returns></returns>
        ///// CALL URL: _vti_bin/Services/ShiftTime/ShiftTimeService.svc/GetLeaveHoursInRange/1-1-2016/1-1-2018/1/1
        //public LeaveTimeModel GetLeaveHoursInRange(string fromDate, string toDate, string departmentID, string locationID, string employeeID)
        //{
        //    LeaveTimeModel leaveTimeModel = new LeaveTimeModel();
        //    try
        //    {
        //        double totalHours = 0;
        //        DateTime fromDateValue = DateTime.Parse(fromDate);
        //        DateTime toDateValue = DateTime.Parse(toDate);
        //        var from = new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, 0, 0, 0);
        //        var to = new DateTime(toDateValue.Year, toDateValue.Month, toDateValue.Day, 23, 59, 59);

        //        for (var currentDate = from; currentDate.Date <= to.Date; currentDate = currentDate.AddDays(1))
        //        {
        //            Biz.Models.ShiftTime shiftTime = _shiftTimeDAL.GetShiftTimeByDate(currentDate.Day, currentDate.Month, currentDate.Year, Convert.ToInt32(departmentID), Convert.ToInt32(locationID), Convert.ToInt32(employeeID));
        //            if (shiftTime != null)
        //            {
        //                leaveTimeModel.ShiftTimeList.Add(new LeaveShiftTimeModel { ShiftTime = ConvertToShiftTimeModel(shiftTime), Date = currentDate.ToString(StringConstant.DateFormatMMddyyyy) });
        //                totalHours += shiftTime.ShiftTimeWorkingHourNumber;
        //            }
        //        }
        //        leaveTimeModel.TotalHours = totalHours;

        //        return leaveTimeModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        ULSLogging.Log(new SPDiagnosticsCategory("STADA - Shift Time Service - GetLeaveHoursInRange fn",
        //              TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
        //          string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
        //        return null;
        //    }
        //}
    }
}