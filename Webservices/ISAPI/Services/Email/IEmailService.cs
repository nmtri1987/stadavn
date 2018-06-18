using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Email
{
    [ServiceContract]
    interface IEmailService
    {
        [OperationContract]
        [WebGet(UriTemplate = "SendChangeShiftRequestMail/{changeshiftItemId}/{toRole}",
        ResponseFormat = WebMessageFormat.Json)]
        bool SendChangeShiftRequestMail(string changeshiftItemId, string toRole);

        [OperationContract]
        [WebGet(UriTemplate = "SendDelegationChangeShiftRequestMail/{changeshiftItemId}/{toRole}",
        ResponseFormat = WebMessageFormat.Json)]
        bool SendDelegationChangeShiftRequestMail(string changeshiftItemId, string toRole);

        [OperationContract]
        [WebGet(UriTemplate = "SendChangeShiftRejectMail/{changeshiftItemId}",
        ResponseFormat = WebMessageFormat.Json)]
        bool SendChangeShiftRejectMail(string changeshiftItemId);

        [OperationContract]
        [WebGet(UriTemplate = "SendChangeShiftApproveMail/{changeshiftItemId}",
        ResponseFormat = WebMessageFormat.Json)]
        bool SendChangeShiftApproveMail(string changeshiftItemId);

        [OperationContract]
        [WebGet(UriTemplate = "SendNotOverTimeRequestEmail/{notOvertimeItemId}/{toRole}",
            ResponseFormat = WebMessageFormat.Json)]
        bool SendNotOverTimeRequestEmail(string notOvertimeItemId, string toRole);

        [OperationContract]
        [WebGet(UriTemplate = "SendDelegationNotOverTimeRequestEmail/{notOvertimeItemId}/{toRole}",
            ResponseFormat = WebMessageFormat.Json)]
        bool SendDelegationNotOverTimeRequestEmail(string notOvertimeItemId, string toRole);

        [OperationContract]
        [WebGet(UriTemplate = "SendNotOvertimeRequestResultEmail/{notOvertimeItemId}/{result}",
        ResponseFormat = WebMessageFormat.Json)]
        bool SendNotOvertimeRequestResultEmail(string notOvertimeItemId, string result);

    }
}
