using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Model;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.VehicleManagement
{
    [ServiceContract]
    interface IVehicleManagementService
    {
        [OperationContract]
        [WebGet(UriTemplate = "HasApprovalPermission/{vehicleId}", ResponseFormat = WebMessageFormat.Json)]
        bool HasApprovalPermission(string vehicleId);

        [OperationContract]
        [WebGet(UriTemplate = "GetDelegatedTaskInfo/{Id}", ResponseFormat = WebMessageFormat.Json)]
        DelegationModel GetDelegatedTaskInfo(string Id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ApproveVehicle", ResponseFormat = WebMessageFormat.Json)]
        MessageResult ApproveVehicle(VehicleApprovalModel vehicleApprovalModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "RejectVehicle", ResponseFormat = WebMessageFormat.Json)]
        MessageResult RejectVehicle(VehicleApprovalModel vehicleApprovalModel);

        [OperationContract]
        [WebGet(UriTemplate = "GetTaskHistory/{Id}/{fulldata}", ResponseFormat = WebMessageFormat.Json)]
        List<TaskManagementModel> GetTaskHistory(string Id, string fulldata);
    }
}
