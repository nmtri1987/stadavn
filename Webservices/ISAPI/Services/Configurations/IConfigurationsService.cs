using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Configurations
{
    [ServiceContract]
    interface IConfigurationsService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetConfigurations", ResponseFormat = WebMessageFormat.Json)]
        List<Biz.Models.Configuration> GetConfigurations(List<string> keys);
    }
}
