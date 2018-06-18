using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// RequestsDAL
    /// </summary>
    public class RequestsDAL : BaseDAL<Request>, IDelegationManager, IFilterTaskManager
    {
        public RequestsDAL(string siteUrl) : base(siteUrl)
        {
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem currentEmployeeProcessing = null;

            Request request = this.ParseToEntity(listItem);
            if (request.PendingAt != null && request.PendingAt.Count > 0)
            {
                currentEmployeeProcessing = request.PendingAt[0];
            }

            return currentEmployeeProcessing;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            Request request = this.ParseToEntity(listItem);
            Delegation delegation = new Delegation(request, currentWeb);

            return delegation;
        }

        /// <summary>
        /// Get task list of fromEmployee.
        /// </summary>
        /// <param name="fromEmployee">The employee who has processing task.</param>
        /// <returns>The list of delegation items.</returns>
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> delegations = null;

            string queryString = $@"<Where>
                                        <Eq>
                                            <FieldRef Name='{RequestsList.PendingAtField}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{fromEmployee.ID}</Value>
                                        </Eq>
                                    </Where>";

            var requests = GetByQuery(queryString);
            if (requests != null && requests.Count > 0)
            {
                delegations = new List<Delegation>();
                foreach (var request in requests)
                {
                    var delegation = new Delegation(request);
                    delegations.Add(delegation);
                }
            }

            return delegations;
        }

        public bool IsValidTask(int listItemID)
        {
            return true;
        }

        #region "Overview"

        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion

        /// <summary>
        /// Get request list by passing conditions
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public List<Request> GetRequests(DateTime fromDate, DateTime toDate, int departmentId, string[] viewFields)
        {
            var resultList = new List<Request>();

            string queryString = $@"<Where>
                              <And>
                                 <Geq>
                                    <FieldRef Name='Created' />
                                    <Value IncludeTimeValue='TRUE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                 </Geq>
                                 <And>
                                    <Leq>
                                       <FieldRef Name='Created' />
                                       <Value IncludeTimeValue='TRUE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                                    </Leq>
                                    <Eq>
                                       <FieldRef Name='CommonDepartment' LookupId='TRUE' />
                                       <Value Type='Lookup'>{departmentId}</Value>
                                    </Eq>
                                 </And>
                              </And>
                           </Where>";
            resultList = GetByQuery(queryString, viewFields).ToList();
            return resultList;
        }

        /// <summary>
        /// Get list of request by "Received Department".
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="departmentid"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<Request> GetReceivedDepartmentRequests(int locationId, int departmentid, DateTime fromDate, DateTime toDate, string status = "")
        {
            #region Build Query

            SPQuery query = new SPQuery();
            if (string.IsNullOrEmpty(status))
            {
                query.Query = string.Format(@"<Where>
                                                  <And>
                                                     <Geq>
                                                        <FieldRef Name='Created' />
                                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value>
                                                     </Geq>
                                                     <And>
                                                        <Leq>
                                                           <FieldRef Name='Created' />
                                                           <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                                        </Leq>
                                                        <And>
                                                           <Eq>
                                                              <FieldRef Name='{2}' LookupId='True' />
                                                              <Value Type='Lookup'>{3}</Value>
                                                           </Eq>
                                                           <Eq>
                                                              <FieldRef Name='{4}' LookupId='True' />
                                                              <Value Type='Lookup'>{5}</Value>
                                                           </Eq>
                                                        </And>
                                                     </And>
                                                  </And>
                                               </Where>
                                                    <OrderBy>
	                                                    <FieldRef Name='{6}' Ascending='TRUE'/>
                                                        <FieldRef Name='{7}' Ascending='FALSE'/>
                                                    </OrderBy>", fromDate.ToString(StringConstant.DateFormatTZForCAML),
                                                        toDate.ToString(StringConstant.DateFormatTZForCAML),
                                                        StringConstant.RequestsList.CommonLocationField, locationId,
                                                        StringConstant.RequestsList.ReceviedByField, departmentid,
                                                        ApprovalManagement.ApprovalFields.StatusOrder, "Created");
            }
            else
            {
                query.Query = string.Format(@"<Where>
                                              <And>
                                                 <Geq>
                                                    <FieldRef Name='Created' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value>
                                                 </Geq>
                                                 <And>
                                                    <Leq>
                                                       <FieldRef Name='Created' />
                                                       <Value IncludeTimeValue='FALSE' Type='DateTime'>{1}</Value>
                                                    </Leq>
                                                    <And>
                                                       <Eq>
                                                          <FieldRef Name='{2}' LookupId='True' />
                                                          <Value Type='Lookup'>{3}</Value>
                                                       </Eq>
                                                       <And>
                                                          <Eq>
                                                             <FieldRef Name='{4}' LookupId='True' />
                                                             <Value Type='Lookup'>{5}</Value>
                                                          </Eq>
                                                          <Eq>
                                                             <FieldRef Name='{6}' />
                                                             <Value Type='Text'>{7}</Value>
                                                          </Eq>
                                                       </And>
                                                    </And>
                                                 </And>
                                              </And>
                                           </Where>
                                            <OrderBy>
	                                            <FieldRef Name='{8}' Ascending='TRUE'/>
                                                <FieldRef Name='{9}' Ascending='FALSE'/>
                                            </OrderBy>", fromDate.ToString(StringConstant.DateFormatTZForCAML),
                                                    toDate.ToString(StringConstant.DateFormatTZForCAML),
                                                    StringConstant.RequestsList.CommonLocationField, locationId,
                                                     StringConstant.RequestsList.ReceviedByField, departmentid,
                                                     ApprovalManagement.ApprovalFields.WFStatus, status,
                                                     ApprovalManagement.ApprovalFields.StatusOrder, "Created");
            }
            
            #endregion

            var items = this.GetByQuery(query);
            return items;
        }
    }
}
