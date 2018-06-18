using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Model;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.FreightManagement
{
    [ServiceContract]
    interface IFreightManagementService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetBringerList/{departmentId}/{locationIds}",
          ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeDepartmentModel> GetBringerList(string departmentId, string locationIds);

        [OperationContract]
        [WebGet(UriTemplate = "GetApprovers/{departmentIdStr}/{locationIdStr}", ResponseFormat = WebMessageFormat.Json)]
        FreightManagementApproverModel GetApprovers(string departmentIdStr, string locationIdStr);

        [OperationContract]
        [WebGet(UriTemplate = "GetFreightManagementById/{Id}/{loadDetails}",
        ResponseFormat = WebMessageFormat.Json)]
        FreightManagementModel GetFreightManagementById(string Id, string loadDetails);

        [OperationContract]
        [WebGet(UriTemplate = "GetFreightDetailsByParentId/{parentId}",
        ResponseFormat = WebMessageFormat.Json)]
        List<FreightDetailsModel> GetFreightDetailsByParentId(string parentId);

        //[OperationContract]
        //[WebGet(UriTemplate = "IsValidRequest", ResponseFormat = WebMessageFormat.Json)]
        //bool IsValidRequest();

        [OperationContract]
        [WebGet(UriTemplate = "ValidateSumitTime", ResponseFormat = WebMessageFormat.Json)]
        MessageResult ValidateSumitTime();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "InsertFreight", ResponseFormat = WebMessageFormat.Json)]
        MessageResult InsertFreight(FreightManagementModel freightManagementModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ApproveFreight", ResponseFormat = WebMessageFormat.Json)]
        MessageResult ApproveFreight(FreightManagementModel freightManagementModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "RejectFreight", ResponseFormat = WebMessageFormat.Json)]
        MessageResult RejectFreight(FreightManagementModel freightManagementModel);

        [OperationContract]
        [WebGet(UriTemplate = "CancelFreight/{freightId}", ResponseFormat = WebMessageFormat.Json)]
        MessageResult CancelFreight(string freightId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateFreightVehicle", ResponseFormat = WebMessageFormat.Json)]
        MessageResult UpdateFreightVehicle(FreightManagementModel freightManagementModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SecurityUpdateFreight", ResponseFormat = WebMessageFormat.Json)]
        MessageResult SecurityUpdateFreight(FreightSecurityModel freightSecurityModel);

        [OperationContract]
        [WebGet(UriTemplate = "IsUserAdminDepartment", ResponseFormat = WebMessageFormat.Json)]
        bool IsUserAdminDepartment();

        [OperationContract]
        [WebGet(UriTemplate = "IsSecurity", ResponseFormat = WebMessageFormat.Json)]
        bool IsSecurity();

        [OperationContract]
        [WebGet(UriTemplate = "HasApprovalPermission/{freightId}", ResponseFormat = WebMessageFormat.Json)]
        bool HasApprovalPermission(string freightId);

        [OperationContract]
        [WebGet(UriTemplate = "GetDelegatedTaskInfo/{Id}", ResponseFormat = WebMessageFormat.Json)]
        DelegationModel GetDelegatedTaskInfo(string Id);

        [OperationContract]
        [WebGet(UriTemplate = "GetAllReceiverDepartment",
        ResponseFormat = WebMessageFormat.Json)]
        List<FreightReceiverDepartmentModel> GetAllReceiverDepartment();

        [OperationContract]
        [WebGet(UriTemplate = "GetVehicleOperatorInfo/{employeeId}",
        ResponseFormat = WebMessageFormat.Json)]
        FreightVehicleOperatorModel GetVehicleOperatorInfo(string employeeId);

        [OperationContract]
        [WebGet(UriTemplate = "GetFreightVehicles",
        ResponseFormat = WebMessageFormat.Json)]
        List<FreightVehicleModel> GetFreightVehicles();

        [OperationContract]
        [WebGet(UriTemplate = "GetTaskHistory/{Id}/{fulldata}", ResponseFormat = WebMessageFormat.Json)]
        List<TaskManagementModel> GetTaskHistory(string Id, string fulldata);

        [OperationContract]
        [WebGet(UriTemplate = "ExportFreights/{from}/{to}/{departmentId}/{locationIds}/{vehicleId}")]
        Stream ExportFreights(string from, string to, string departmentId, string locationIds, string vehicleId);

        [OperationContract]
        [WebGet(UriTemplate = "ExportFreight/{freightId}")]
        Stream ExportFreight(string freightId);
    }
}
