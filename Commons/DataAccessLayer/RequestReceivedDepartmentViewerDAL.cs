using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// RequestReceivedDepartmentViewerDAL
    /// </summary>
    public class RequestReceivedDepartmentViewerDAL : BaseDAL<RequestReceivedDepartmentViewer>
    {
        public RequestReceivedDepartmentViewerDAL(string siteUrl) : base(siteUrl)
        {
        }

        public RequestReceivedDepartmentViewer GetReceivedDepartmentRequestViewerByLocaltionAndDepartmentAndEmployee(int locationId, int departmentId, int employeeId)
        {
            RequestReceivedDepartmentViewer item = null;

            try
            {
                string[] viewFieldsQuery = { StringConstant.RequestReceivedDepartmentViewersList.Fields.Title,
                                                StringConstant.RequestReceivedDepartmentViewersList.Fields.Location,
                                                StringConstant.RequestReceivedDepartmentViewersList.Fields.Department,
                                                StringConstant.RequestReceivedDepartmentViewersList.Fields.Employees };
                SPQuery query = new SPQuery();
                query.Query = string.Format(@"<Where>
                                                  <And>
                                                     <Eq>
                                                        <FieldRef Name='{0}' LookupId='True' />
                                                        <Value Type='Lookup'>{1}</Value>
                                                     </Eq>
                                                     <And>
                                                        <Eq>
                                                           <FieldRef Name='{2}' LookupId='True' />
                                                           <Value Type='Lookup'>{3}</Value>
                                                        </Eq>
                                                        <Eq>
                                                           <FieldRef Name='{4}' LookupId='True' />
                                                           <Value Type='LookupMulti'>{5}</Value>
                                                        </Eq>
                                                     </And>
                                                  </And>
                                               </Where>", StringConstant.RequestReceivedDepartmentViewersList.Fields.Location, locationId,
                                               StringConstant.RequestReceivedDepartmentViewersList.Fields.Department, departmentId,
                                               StringConstant.RequestReceivedDepartmentViewersList.Fields.Employees, employeeId);
                query.RowLimit = 1;
                var items = this.GetByQuery(query, viewFieldsQuery);
                if (items != null && items.Count > 0)
                {
                    item = items[0];
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return item;
        }
    }
}
