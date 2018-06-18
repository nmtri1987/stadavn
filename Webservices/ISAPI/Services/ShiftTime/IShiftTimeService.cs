using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.ShiftTime
{
    [ServiceContract]
    interface IShiftTimeService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetShiftTimes",
         ResponseFormat = WebMessageFormat.Json)]
        List<ShiftTimeModel> GetShiftTimes();

        [OperationContract]
        [WebGet(UriTemplate = "GetShiftTimeInDate/{day}/{month}/{year}/{departmentID}/{locationID}/{employeeID}",
          ResponseFormat = WebMessageFormat.Json)]
        ShiftTimeModel GetShiftTimeInDate(string day, string month, string year, string departmentID, string locationID, string employeeID);

        //[OperationContract]
        //[WebGet(UriTemplate = "GetLeaveHoursInRange/{fromDate}/{toDate}/{departmentID}/{locationID}/{employeeID}", ResponseFormat = WebMessageFormat.Json)]
        //LeaveTimeModel GetLeaveHoursInRange(string fromDate, string toDate, string departmentID, string locationID, string employeeID);
    }
}
