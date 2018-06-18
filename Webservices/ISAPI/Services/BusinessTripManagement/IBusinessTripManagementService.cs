using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.BusinessTripManagement
{
    [ServiceContract]
    interface IBusinessTripManagementService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetApprovers/{departmentIdStr}/{locationIdStr}", ResponseFormat = WebMessageFormat.Json)]
        BusinessTripManagementApproverModel GetApprovers(string departmentIdStr, string locationIdStr);

        [OperationContract]
        [WebGet(UriTemplate = "GetDrivers", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeInfoModel> GetDrivers();

        [OperationContract]
        [WebGet(UriTemplate = "GetAccountants", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeInfoModel> GetAccountants();

        [OperationContract]
        [WebGet(UriTemplate = "GetBusinessTripManagementById/{Id}", ResponseFormat = WebMessageFormat.Json)]
        BusinessTripManagementModel GetBusinessTripManagementById(string Id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "InsertBusinessTripManagement", ResponseFormat = WebMessageFormat.Json)]
        MessageResult InsertBusinessTripManagement(BusinessTripManagementModel businessTripManagementModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ApproveBusinessTrip", ResponseFormat = WebMessageFormat.Json)]
        MessageResult ApproveBusinessTrip(BusinessTripManagementModel businessTripManagementModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "RejectBusinessTrip", ResponseFormat = WebMessageFormat.Json)]
        MessageResult RejectBusinessTrip(BusinessTripManagementModel businessTripManagementModel);

        [OperationContract]
        [WebGet(UriTemplate = "CancelBusinessTrip/{Id}", ResponseFormat = WebMessageFormat.Json)]
        MessageResult CancelBusinessTrip(string Id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "DriverUpdateBusinessTrip", ResponseFormat = WebMessageFormat.Json)]
        MessageResult DriverUpdateBusinessTrip(BusinessTripManagementModel businessTripManagementModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "CashierUpdateBusinessTrip", ResponseFormat = WebMessageFormat.Json)]
        MessageResult CashierUpdateBusinessTrip(BusinessTripManagementModel businessTripManagementModel);

        [OperationContract]
        [WebGet(UriTemplate = "HasApprovalPermission/{Id}", ResponseFormat = WebMessageFormat.Json)]
        bool HasApprovalPermission(string Id);

        [OperationContract]
        [WebGet(UriTemplate = "GetDelegatedTaskInfo/{Id}", ResponseFormat = WebMessageFormat.Json)]
        DelegationModel GetDelegatedTaskInfo(string Id);

        [OperationContract]
        [WebGet(UriTemplate = "GetTaskHistory/{Id}/{fulldata}", ResponseFormat = WebMessageFormat.Json)]
        List<TaskManagementModel> GetTaskHistory(string Id, string fulldata);
    }
}
