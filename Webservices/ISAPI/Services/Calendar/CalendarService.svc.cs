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
using RBVH.Stada.Intranet.Biz.Helpers;
using RBVH.Stada.Intranet.Webservices.Helper;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Calendar
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CalendarService : ICalendarService
    {
        readonly CalendarDAL _calendarDal;
        readonly Calendar2DAL _calendar2Dal;

        public CalendarService()
        {
            _calendarDal = new CalendarDAL(SPContext.Current.Web.Url);
            _calendar2Dal = new Calendar2DAL(SPContext.Current.Web.Url);
        }
        /// <summary>
        /// Get calendar list by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        /// Call URL: _vti_bin/Services/Calendar/CalendarService.svc/GetByCategory/
        public List<CalendarModel> GetByCategory(string category)
        {
            try
            {
                List<CalendarModel> calendars = new List<CalendarModel>();
                var calendarList = _calendarDal.GetByCategory(category);
                foreach (var item in calendarList)
                {
                    calendars.Add(new CalendarModel
                    {
                        ID = item.ID,
                        Description = item.Description,
                        EndDate = item.EndDate,
                        StartDate = item.StartDate,
                        Location = item.Location,
                        Category = item.Category,
                        Title = item.Title
                    }
                    );
                }
                return calendars;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Calendar Service - GetByCategory fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }

        }

        /// <summary>
        /// Get list of calendar in passed Category
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        /// Call URL: _vti_bin/Services/Calendar/CalendarService.svc/GetByCategories/
        public List<CalendarModel> GetByCategories(string categories)
        {
            try
            {
                List<string> categoryList = new List<string>();
                categoryList = categories.Split(',').ToList();

                List<CalendarModel> calendars = new List<CalendarModel>();
                var calendarList = _calendarDal.GetByCategories(categoryList);
                foreach (var item in calendarList)
                {
                    calendars.Add(new CalendarModel
                    {
                        ID = item.ID,
                        Description = item.Description,
                        EndDate = item.EndDate,
                        StartDate = item.StartDate,
                        Location = item.Location,
                        Category = item.Category,
                        Title = item.Title
                    });
                }
                return calendars;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Calendar Service - GetByCategories fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }

        }

        /// <summary>
        /// Get holiday list in time interval 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// CALL URL: _vti_bin/Services/Calendar/CalendarService.svc/GetHolidayInRange/1-1-2016/1-1-2018/1
        public List<CalendarModel> GetHolidayInRange(string fromDate, string toDate, string location)
        {
            List<CalendarModel> calendars = new List<CalendarModel>();
            try
            {
                var fromDateValue = fromDate.ToMMDDYYYYDate(false); // mm-dd-yyyy
                var toDateValue = toDate.ToMMDDYYYYDate(true);

                if(fromDateValue != DateTime.MinValue && toDateValue != DateTime.MinValue)
                {
                    List<string> holidays = new List<string>() { StringConstant.CaledarCategory.Holiday, StringConstant.CaledarCategory.Weekend, StringConstant.CaledarCategory.CompensationDayOff };

                    List<Biz.Models.Calendar> calendarList = new List<Biz.Models.Calendar>();
                    if (location.Equals("1"))
                    {
                        calendarList = _calendarDal.GetByDateAndCategories(fromDateValue, toDateValue, holidays);
                    }
                    else if (location.Equals("2"))
                    {
                        calendarList = _calendar2Dal.GetByDateAndCategories(fromDateValue, toDateValue, holidays);
                    }
                    //get all
                    else
                    {
                        calendarList = _calendarDal.GetByDateAndCategories(fromDateValue, toDateValue, holidays);
                        var calendar2List = _calendar2Dal.GetByDateAndCategories(fromDateValue, toDateValue, holidays);
                        calendarList.AddRange(calendar2List);
                    }

                    foreach (var item in calendarList)
                    {
                        if (item.StartDate.ToString("MM/dd/yyyy") == item.EndDate.ToString("MM/dd/yyyy"))
                        {
                            calendars.Add(new CalendarModel
                            {
                                ID = item.ID,
                                Description = item.Description,
                                EndDate = item.EndDate,
                                StartDate = item.StartDate,
                                Location = item.Location,
                                Category = item.Category,
                                Title = item.Title,
                                Day = item.EndDate.Day
                            });

                        }
                        else
                        {
                            DateTime startDate = item.StartDate;
                            DateTime endDate = item.EndDate;
                            //do
                            //{
                            //    calendars.Add(new CalendarModel
                            //    {
                            //        ID = item.ID,
                            //        Description = item.Description,
                            //        EndDate = startDate,
                            //        StartDate = startDate,
                            //        Location = item.Location,
                            //        Category = item.Category,
                            //        Title = item.Title,
                            //        Day = startDate.Day
                            //    });

                            //    startDate = startDate.AddDays(1);
                            //}
                            //while (startDate. <= item.EndDate.DayOfYear);

                            for (var currentDate = startDate; currentDate.Date <= endDate.Date && currentDate.Date <= toDateValue; currentDate = currentDate.AddDays(1))
                            {
                                calendars.Add(new CalendarModel
                                {
                                    ID = item.ID,
                                    Description = item.Description,
                                    EndDate = currentDate,
                                    StartDate = currentDate,
                                    Location = item.Location,
                                    Category = item.Category,
                                    Title = item.Title,
                                    Day = currentDate.Day
                                });
                            }
                        }

                    }
                }
               
                return calendars;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Calendar Service - GetHolidayInRange fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public CalendarModel GetHolidayInfo(string dateStr, string location)
        {
            CalendarModel calendarModel = null;
            try
            {
                var fromDateValue = dateStr.ToMMDDYYYYDate(false); // mm-dd-yyyy

                if (fromDateValue != DateTime.MinValue)
                {
                    List<string> holidays = new List<string>() { StringConstant.CaledarCategory.Holiday, StringConstant.CaledarCategory.Weekend, StringConstant.CaledarCategory.CompensationDayOff };

                    Biz.Models.Calendar calendar = new Biz.Models.Calendar();
                    if (location.Equals("1"))
                    {
                        calendar = _calendarDal.GetByDateAndCategories(fromDateValue, holidays);
                    }
                    else if (location.Equals("2"))
                    {
                        calendar = _calendar2Dal.GetByDateAndCategories(fromDateValue, holidays);
                    }

                    if (calendar != null)
                    {
                        calendarModel = (new CalendarModel
                        {
                            ID = calendar.ID,
                            Description = calendar.Description,
                            EndDate = calendar.EndDate,
                            StartDate = calendar.StartDate,
                            Location = calendar.Location,
                            Category = calendar.Category,
                            Title = calendar.Title,
                            Day = calendar.EndDate.Day
                        });
                    }
                    
                }

                return calendarModel;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Calendar Service - GetHolidayInfo fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<CalendarModel> GetHolidayByLocationInYear(string year, string location)
        {
            int yearInt = Convert.ToInt16(year);
            return GetHolidayInRange(new DateTime(yearInt, 1, 1).ToShortDateString(), new DateTime(yearInt, 12, 31).ToShortDateString(), location);
        }

        public List<CalendarModel> GetHolidayInYear(string year)
        {
            int yearInt = Convert.ToInt16(year);
            return GetHolidayInRange(new DateTime(yearInt, 1, 1).ToShortDateString(), new DateTime(yearInt, 12, 31).ToShortDateString(), string.Empty);
        }

        /// <summary>
        /// Get holidays in month - year
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        /// CALL URL: CALL URL: _vti_bin/Services/Calendar/CalendarService.svc/GetHolidayInMonth/1/2017
        public List<CalendarModel> GetHolidayInMonth(string month, string year)
        {
            List<CalendarModel> calendars = new List<CalendarModel>();
            try
            {
                int monthValue, yearValue;
                if (int.TryParse(month, out monthValue) && int.TryParse(year, out yearValue))
                {
                    var fromMonth = new DateTime(yearValue, monthValue, 1, 0, 0, 0);
                    var toMonth = new DateTime(yearValue, monthValue, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    List<string> holidaysCategory = new List<string>() { StringConstant.CaledarCategory.CompensationDayOff, StringConstant.CaledarCategory.Weekend, StringConstant.CaledarCategory.Holiday };

                    var calendarList = _calendarDal.GetByDateAndCategories(fromMonth, toMonth, holidaysCategory);
                    foreach (var item in calendarList)
                    {
                        calendars.Add(new CalendarModel
                        {
                            ID = item.ID,
                            Description = item.Description,
                            EndDate = item.EndDate,
                            StartDate = item.StartDate,
                            Location = item.Location,
                            Category = item.Category,
                            Title = item.Title
                        });
                    }
                }
                return calendars;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Calendar Service - GetHolidayInMonth fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Get list holidays in month & location
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        /// CALL URL: CALL URL: _vti_bin/Services/Calendar/CalendarService.svc/GetLocationHolidayInMonth/1/2017/1
        public List<CalendarModel> GetLocationHolidayInMonth(string month, string year, string location)
        {
            List<CalendarModel> calendars = new List<CalendarModel>();
            try
            {
                int monthValue, yearValue;
                if (int.TryParse(month, out monthValue) && int.TryParse(year, out yearValue))
                {
                    var fromMonth = new DateTime(yearValue, monthValue, 1, 0, 0, 0);
                    var toMonth = new DateTime(yearValue, monthValue, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    List<string> holidaysCategory = new List<string>() { StringConstant.CaledarCategory.Holiday, StringConstant.CaledarCategory.Weekend, StringConstant.CaledarCategory.CompensationDayOff };
                    List<Biz.Models.Calendar> calendarList = new List<Biz.Models.Calendar>();
                    if (location.Equals("1"))
                    {
                        calendarList = _calendarDal.GetByDateAndCategories(fromMonth, toMonth, holidaysCategory);
                    }
                    else if (location.Equals("2"))
                    {
                        calendarList = _calendar2Dal.GetByDateAndCategories(fromMonth, toMonth, holidaysCategory);
                    }

                    foreach (var item in calendarList)
                    {
                        calendars.Add(new CalendarModel
                        {
                            ID = item.ID,
                            Description = item.Description,
                            EndDate = item.EndDate,
                            StartDate = item.StartDate,
                            Location = item.Location,
                            Category = item.Category,
                            Title = item.Title
                        });
                    }
                }
                return calendars;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Calendar Service - GetLocationHolidayInMonth fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }
    }
}