using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Calendar
{
    [ServiceContract]
    interface ICalendarService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetByCategory/{category}",
         ResponseFormat = WebMessageFormat.Json)]
        List<CalendarModel> GetByCategory(string category);

        [OperationContract]
        [WebGet(UriTemplate = "GetByCategories/{categories}",
        ResponseFormat = WebMessageFormat.Json)]
        List<CalendarModel> GetByCategories(string categories);

        [OperationContract]
        [WebGet(UriTemplate = "GetHolidayInRange/{fromDate}/{toDate}/{location}",
        ResponseFormat = WebMessageFormat.Json)]
        List<CalendarModel> GetHolidayInRange(string fromDate, string toDate, string location);


        [OperationContract]
        [WebGet(UriTemplate = "GetHolidayInMonth/{month}/{year}",
        ResponseFormat = WebMessageFormat.Json)]
        List<CalendarModel> GetHolidayInMonth(string month, string year);


        [OperationContract]
        [WebGet(UriTemplate = "GetLocationHolidayInMonth/{month}/{year}/{location}",
        ResponseFormat = WebMessageFormat.Json)]
        List<CalendarModel> GetLocationHolidayInMonth(string month, string year, string location);

        [OperationContract]
        [WebGet(UriTemplate = "GetHolidayByLocationInYear/{year}/{location}",
        ResponseFormat = WebMessageFormat.Json)]
        List<CalendarModel> GetHolidayByLocationInYear(string year, string location);

        [OperationContract]
        [WebGet(UriTemplate = "GetHolidayInYear/{year}",
        ResponseFormat = WebMessageFormat.Json)]
        List<CalendarModel> GetHolidayInYear(string year);

    }
}
