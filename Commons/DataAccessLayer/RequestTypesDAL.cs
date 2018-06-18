using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class RequestTypesDAL : BaseDAL<RequestType>
    {
        public RequestTypesDAL(string siteUrl) : base(siteUrl)
        {
        }

        public RequestType GetByRequestType(string requestType)
        {
            RequestType item = new RequestType();
            var results = GetByQuery($@"<Where>
                                  <Eq>
                                     <FieldRef Name='{StringConstant.RequestTypesList.RequestsTypeField}' />
                                     <Value Type='Choice'>{requestType}</Value>
                                  </Eq>
                               </Where>");
            item = results.FirstOrDefault();
            return item;
        }
    }
}
