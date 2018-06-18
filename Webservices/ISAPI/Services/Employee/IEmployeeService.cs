using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Employee
{
    [ServiceContract]
    interface IEmployeeService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetEmployeeListInCurrentDepartment/{departmentId}/{locationIds}", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeDepartmentModel> GetEmployeeListInCurrentDepartment(string departmentId, string locationIds);

        [OperationContract]
        [WebGet(UriTemplate = "GetEmployeeListInDepartments/{departmentIds}/{locationId}", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeDepartmentModel> GetEmployeeListInDepartments(string departmentIds, string locationId);

        [OperationContract]
        [WebGet(UriTemplate = "GetCommonAccountList", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeInfo> GetCommonAccountList();

        [OperationContract]
        [WebGet(UriTemplate = "GetCurrentUser", ResponseFormat = WebMessageFormat.Json)]
        CurrentUserModel GetCurrentUser();

        [WebGet(UriTemplate = "GetEmployeeByEmployeeID/{id}",ResponseFormat = WebMessageFormat.Json)]
        EmployeeModel GetEmployeeByEmployeeID(string id);

        [WebGet(UriTemplate = "GetByDepartmentLocation/{locationId}/{departmentId}/{maxLevel}", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeModel> GetByDepartmentLocation(string locationId, string departmentId, string maxLevel);

        [WebGet(UriTemplate = "GetEmployeeApprovers/{id}", ResponseFormat = WebMessageFormat.Json)]
        EmployeeApproverModel GetEmployeeApprovers(string id);

        [WebGet(UriTemplate = "IsUserCurrentuserInGroup/{groupName}", ResponseFormat = WebMessageFormat.Json)]
        bool IsUserCurrentuserInGroup(string groupName);

        [WebGet(UriTemplate = "GetEmployeeListDontHaveOvertimeInDate/{departmentId}/{locationId}/{date}", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeModel> GetEmployeeListDontHaveOvertimeInDate(string departmentId, string locationId, string date);

        [WebGet(UriTemplate = "IsManager/{employeeId}", ResponseFormat = WebMessageFormat.Json)]
        bool IsManager(string employeeId);

        [WebGet(UriTemplate = "GetAvatar/{id}", ResponseFormat = WebMessageFormat.Json)]
        string GetAvatar(string id);

        [WebGet(UriTemplate = "GetPeoplePickerData/{SearchString}", ResponseFormat = WebMessageFormat.Json)]
        List<PeoplePickerUser> GetPeoplePickerData(string SearchString);

        [WebInvoke(UriTemplate = "PostPeoplePickerData", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        List<PeoplePickerUser> PostPeoplePickerData(PeoplePickerDataRequest PeoplePickerDataRequest);

        [WebInvoke(UriTemplate = "GetAdditionalApprovers", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        List<PeoplePickerUser> GetAdditionalApprovers(PeoplePickerDataRequest peoplePickerDataRequest);

        [WebGet(UriTemplate = "GetEmployeeInfoByADAccount/{adAccountId}", ResponseFormat = WebMessageFormat.Json)]
        EmployeeModel GetEmployeeInfoByADAccount(string adAccountId);

        [WebGet(UriTemplate = "IsMemberOfDepartment/{employeeId}/{departmentId}", ResponseFormat = WebMessageFormat.Json)]
        bool IsMemberOfDepartment(string employeeId, string departmentId);

        [WebGet(UriTemplate = "IsMemberOfDepartmentByCode/{employeeId}/{departmentCode}", ResponseFormat = WebMessageFormat.Json)]
        bool IsMemberOfDepartmentByCode(string employeeId, string departmentCode);

        [WebGet(UriTemplate = "GetManagerByDepartment/{departmentId}/{minLevel}", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeModel> GetManagerByDepartment(string departmentId, string minLevel);

        [WebGet(UriTemplate = "GetAllEmployees", ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeModel> GetAllEmployees();
    }
}
