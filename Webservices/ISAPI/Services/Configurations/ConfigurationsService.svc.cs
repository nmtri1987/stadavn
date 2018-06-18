using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.ServiceModel.Activation;
using RBVH.Stada.Intranet.Webservices.Model;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.Linq;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.OverviewManagement;
using System.ServiceModel;
using RBVH.Stada.Intranet.Biz.Interfaces;
using System;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Configurations
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ConfigurationsService : IConfigurationsService
    {
        private readonly string _webUrl;
        public ConfigurationsService()
        {
            _webUrl = SPContext.Current.Web.Url;
        }

        public List<Biz.Models.Configuration> GetConfigurations(List<string> keys)
        {
            return ConfigurationDAL.GetValues(_webUrl, keys);
        }
    }
}
