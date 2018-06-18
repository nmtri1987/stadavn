using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.ChangeShiftManagement
{
    [ServiceContract]
    interface IChangeShiftManagementService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Approve", ResponseFormat = WebMessageFormat.Json)]
        MessageResult Approve(ChangeShiftApprovalModel changeShiftApprovalModel);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Reject", ResponseFormat = WebMessageFormat.Json)]
        MessageResult Reject(ChangeShiftApprovalModel changeShiftApprovalModel);
    }
}
