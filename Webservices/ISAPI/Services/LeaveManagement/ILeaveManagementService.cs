using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Model;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement
{
    [ServiceContract]
    interface ILeaveManagementService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetAllLeaveInfo/{employeeID}/{fromDate}/{toDate}", ResponseFormat = WebMessageFormat.Json)]
        LeaveResult GetAllLeaveInfo(string employeeID, string fromDate, string toDate);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "InsertLeaveManagement", ResponseFormat = WebMessageFormat.Json)]
        MessageResult InsertLeaveManagement(LeaveManagementModel leaveManagementModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ApproveLeave", ResponseFormat = WebMessageFormat.Json)]
        MessageResult ApproveLeave(LeaveApprovalModel leaveApprovalModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "RejectLeave", ResponseFormat = WebMessageFormat.Json)]
        MessageResult RejectLeave(LeaveApprovalModel leaveApprovalModel);

        [OperationContract]
        [WebGet(UriTemplate = "CancelLeaveManagement/{leaveId}", ResponseFormat = WebMessageFormat.Json)]
        MessageResult CancelLeaveManagement(string leaveId);

        [OperationContract]
        [WebGet(UriTemplate = "GetApproversByRequester/{departmentIdStr}/{requesterIdStr}/{requestForIdStr}/{leaveHoursStr}", ResponseFormat = WebMessageFormat.Json)]
        EmployeeApproverModel GetApproversByRequester(string departmentIdStr, string requesterIdStr, string requestForIdStr, string leaveHoursStr);

        [OperationContract]
        [WebGet(UriTemplate = "GetApproversByRequesterAndTime/{departmentIdStr}/{requesterIdStr}/{requestForIdStr}/{fromDateStr}/{toDateStr}/{leaveHoursStr}", ResponseFormat = WebMessageFormat.Json)]
        EmployeeApproverModel GetApproversByRequesterAndTime(string departmentIdStr, string requesterIdStr, string requestForIdStr, string fromDateStr, string toDateStr, string leaveHoursStr);

        [OperationContract]
        [WebGet(UriTemplate = "GetLeavesInRange/{employeeID}/{departmentID}/{locationID}/{fromDate}/{toDate}", ResponseFormat = WebMessageFormat.Json)]
        List<LeaveInfo> GetLeavesInRange(string employeeID, string departmentID, string locationID, string fromDate, string toDate);

        [OperationContract]
        [WebGet(UriTemplate = "GetLeavesInRangeByDepartment/{locationID}/{departmentID}/{fromDate}/{toDate}", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeLeaveInfo> GetLeavesInRangeByDepartment(string locationID, string departmentID, string fromDate, string toDate);

        [OperationContract]
        [WebGet(UriTemplate = "GetLeaveManagementById/{id}", ResponseFormat = WebMessageFormat.Json)]
        LeaveManagementModel GetLeaveManagementById(string id);

        [OperationContract]
        [WebGet(UriTemplate = "HasApprovalPermission/{leaveId}", ResponseFormat = WebMessageFormat.Json)]
        bool HasApprovalPermission(string leaveId);

        [OperationContract]
        [WebGet(UriTemplate = "GetDelegatedTaskInfo/{Id}", ResponseFormat = WebMessageFormat.Json)]
        DelegationModel GetDelegatedTaskInfo(string Id);

        [OperationContract]
        [WebGet(UriTemplate = "GetTaskHistory/{Id}/{fulldata}", ResponseFormat = WebMessageFormat.Json)]
        List<TaskManagementModel> GetTaskHistory(string Id, string fulldata);

        [OperationContract]
        [WebGet(UriTemplate = "ExportLeaves/{from}/{to}/{departmentId}/{locationIds}")]
        Stream ExportLeaves(string from, string to, string departmentId, string locationIds);

        [OperationContract]
        [WebGet(UriTemplate = "GetShiftTimeByDate/{date}/{empId}/{departmentId}/{locationId}", ResponseFormat = WebMessageFormat.Json)]
        string GetShiftTimeByDate(string date, string empId, string departmentId, string locationId);
    }
}
