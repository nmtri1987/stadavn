using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Overtime
{
    [ServiceContract]
    interface IOvertimeService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetById/{Id}",
         ResponseFormat = WebMessageFormat.Json)]
        OverTimeModel GetById(string Id);
        [OperationContract]
        [WebInvoke(UriTemplate = "InsertOvertime", Method = "POST",
        ResponseFormat = WebMessageFormat.Json)]
        MessageResult InsertOvertime(OverTimeModel overTimeModel);
        [OperationContract]
        [WebInvoke(UriTemplate = "UpdateApprove", Method = "POST",
        ResponseFormat = WebMessageFormat.Json)]
        MessageResult UpdateApprove(OverTimeModel overTimeModel);

        [OperationContract]
        [WebGet(UriTemplate = "GetOvertimeDetailByDate/{employeeLookupId}/{date}",
        ResponseFormat = WebMessageFormat.Json)]
        OvertimeEmployeeInDateModel GetOvertimeDetailByDate(string employeeLookupId, string date);

        [OperationContract]
        [WebGet(UriTemplate = "IsNotOvertimeExist/{employeeId}/{date}", ResponseFormat = WebMessageFormat.Json)]
        bool IsNotOvertimeExist(string employeeId, string date);

        [OperationContract]
        [WebGet(UriTemplate = "IsDelegated/{fromApproverId}/{itemId}", ResponseFormat = WebMessageFormat.Json)]
        bool IsDelegated(string fromApproverId, string itemId);
    }
}
