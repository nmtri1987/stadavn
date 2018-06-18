using RBVH.Stada.Intranet.Webservices.Model;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.ShiftManagement
{
    [ServiceContract]
    interface IShiftManagementService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetShiftManagementById/{Id}",
        ResponseFormat = WebMessageFormat.Json)]
        ShiftManagementModel GetShiftManagementById(string Id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetByPreviousShift",
        ResponseFormat = WebMessageFormat.Json)]
        ShiftManagementModel GetByPreviousShift(GetShiftManagementRequest getShiftManagementRequest);

        [OperationContract]
        [WebGet(UriTemplate = "UpdateShiftDetailForWorkflow/{requesterId}/{fromDate}/{toDate}/{toShiftId}",
        ResponseFormat = WebMessageFormat.Json)]
        bool UpdateShiftDetailForWorkflow(string requesterId, string fromDate, string toDate, string toShiftId);

        // Load test
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "InsertShiftManagementMasterDetail", ResponseFormat = WebMessageFormat.Json)]
        MessageResult InsertShiftManagementMasterDetail(ShiftManagementModel shiftManagementModel);

        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "InsertShiftManagementMaster", ResponseFormat = WebMessageFormat.Json)]
        //MessageResult InsertShiftManagementMaster(ShiftManagementModel shiftManagementModel);

        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "InsertShiftManagementDetail", ResponseFormat = WebMessageFormat.Json)]
        //MessageResult InsertShiftManagementDetail(ShiftManagementDetailModel shiftManagementDetailModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ApproveShiftManagementDetail", ResponseFormat = WebMessageFormat.Json)]
        MessageResult ApproveShiftManagementDetail(ShiftManagementModel shiftManagementDetailModel);

        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "ApproveShiftManagementDetails", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //MessageResult ApproveShiftManagementDetails(List<ShiftManagementDetailModel> shiftManagementDetailModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "RefreshShiftManagementDetail", ResponseFormat = WebMessageFormat.Json)]
        MessageResult RefreshShiftManagementDetail(ShiftManagementModel shiftManagementDetailModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SendAdminApprovalEmail", ResponseFormat = WebMessageFormat.Json)]
        MessageResult SendAdminApprovalEmail(AdminApprovalModel adminApprovalModel);

        [OperationContract]
        [WebGet(UriTemplate = "IsChangeShiftExist/{employeeId}/{date}", ResponseFormat = WebMessageFormat.Json)]
        bool IsChangeShiftExist(string employeeId, string date);

        [OperationContract]
        [WebGet(UriTemplate = "ExportShifts/{Ids}")]
        Stream ExportShifts(string Ids);

        [OperationContract]
        [WebGet(UriTemplate = "ImportShifts/{month}/{year}/{departmentId}/{locationId}/{fileName}", ResponseFormat = WebMessageFormat.Json)]
        MessageResult ImportShifts(string month, string year, string departmentId, string locationId, string fileName);

        [OperationContract]
        [WebGet(UriTemplate = "IsDelegated/{fromApproverId}/{itemId}", ResponseFormat = WebMessageFormat.Json)]
        bool IsDelegated(string fromApproverId, string itemId);
    }
}
