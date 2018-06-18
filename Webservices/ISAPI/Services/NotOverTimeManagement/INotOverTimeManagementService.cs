using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.NotOverTimeManagement
{
    [ServiceContract]
    interface INotOverTimeManagementService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Approve", ResponseFormat = WebMessageFormat.Json)]
        MessageResult Approve(NotOverTimeApprovalModel notOverTimeApprovalModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Reject", ResponseFormat = WebMessageFormat.Json)]
        MessageResult Reject(NotOverTimeApprovalModel notOverTimeApprovalModel);
    }
}
