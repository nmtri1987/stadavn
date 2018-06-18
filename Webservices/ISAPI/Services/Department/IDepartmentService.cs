using RBVH.Stada.Intranet.Webservices.Model;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RBVH.Stada.Intranet.Webservices
{
    [ServiceContract]
    interface IDepartmentService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetDepartmentForShift/{lcid}/{locationIds}",
           ResponseFormat = WebMessageFormat.Json)]
        List<DepartmentInfo> GetDepartmentForShift(string lcid, string locationIds);

        [OperationContract]
        [WebGet(UriTemplate = "GetDepartments/{languageCode}/{locationIds}",
           ResponseFormat = WebMessageFormat.Json)]
        List<DepartmentInfo> GetDepartments(string languageCode, string locationIds);

        [OperationContract]
        [WebGet(UriTemplate = "GetDepartmentsByLcid/{lcid}/{locationIds}",
           ResponseFormat = WebMessageFormat.Json)]
        List<DepartmentInfo> GetDepartmentsByLcid(string lcid, string locationIds);

        [OperationContract]
        [WebGet(UriTemplate = "GetDepartmentByIdLanguageCode/{departmentId}/{languageCode}",
           ResponseFormat = WebMessageFormat.Json)]
        DepartmentInfo GetDepartmentByIdLanguageCode(string departmentId, string languageCode);

        [OperationContract]
        [WebGet(UriTemplate = "GetByCode/{code}/{lcid}",
           ResponseFormat = WebMessageFormat.Json)]
        DepartmentInfo GetByCode(string code, string lcid);
    }
}
