using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class OverTimeManagementDetailDAL : BaseDAL<OverTimeManagementDetail>
    {
        public OverTimeManagementDetailDAL(string siteUrl) : base(siteUrl)
        {
            ListUrl = "/Lists/OvertimeEmployeeDetails";
        }

        public List<OverTimeManagementDetail> GetByOvertimeId(int Id)
        {
            List<OverTimeManagementDetail> overTimeManagementDetail = new List<OverTimeManagementDetail>();

            overTimeManagementDetail = GetByQuery(
                             $@"<Where>
                                  <Eq>
                                     <FieldRef Name='OvertimeManagementID' />
                                     <Value Type='Lookup' LookupId='TRUE'>{Id}</Value>
                                  </Eq>
                               </Where>", string.Empty);

            return overTimeManagementDetail;
        }

        public List<OverTimeManagementDetail> GetEmployeeOvertimeByManagementId(int overtimeManagementId, int EmployeeID)
        {
            List<OverTimeManagementDetail> overTimeManagementDetail = new List<OverTimeManagementDetail>();

            // Query SPList
            overTimeManagementDetail = GetByQuery(
                            $@"<Where>
                                    <And>
                                             <Eq>
                                                 <FieldRef Name='OvertimeManagementID' LookupId='TRUE' />
                                                 <Value Type='Lookup' LookupId='TRUE'>{overtimeManagementId}</Value>
                                              </Eq>
                                             <Eq>
                                                   <FieldRef Name='Employee' LookupId='TRUE'/>
                                                   <Value Type='Lookup' LookupId='TRUE' >{EmployeeID}</Value>
                                             </Eq>
                                     </And>
                          </Where>", string.Empty);

            return overTimeManagementDetail;
        }

        public IList<OverTimeManagementDetail> GetEmployeeByDate(int employeeLookupId, DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            var startDateString = startDate.ToString(StringConstant.DateFormatTZForCAML);
            var endDateString = endDate.ToString(StringConstant.DateFormatTZForCAML);
            // Query SPList
            var overTimeManagementDetailItems = GetByQuery($@"
            <Where>
              <And>
                  <Or>
                      <IsNull>
                            <FieldRef Name='ApprovalStatus' />
                      </IsNull>
                     <Eq>
                            <FieldRef Name='ApprovalStatus' />
                            <Value Type='Text'>true</Value>
                     </Eq>
                   </Or>
                   <And>
                         <Eq>
                                 <FieldRef Name='Employee' LookupId='TRUE' />
                                 <Value Type='Lookup'>{employeeLookupId}</Value>
                         </Eq>
                         <And>
                                   <Geq>
                                            <FieldRef Name='CommonDate' />
                                                   <Value IncludeTimeValue='TRUE' Type='DateTime'>{startDateString}</Value>
                                   </Geq>
                                   <Leq>
                                             <FieldRef Name='CommonDate' />
                                             <Value IncludeTimeValue='TRUE' Type='DateTime'>{endDateString}</Value>
                                   </Leq>
                        </And>
                  </And>
            </And>       
          </Where>", string.Empty);

            return overTimeManagementDetailItems;

        }
        /// <summary>
        /// Get by date & employee lookup value
        /// </summary>
        /// <returns></returns>
        ///CALL URL:  _vti_bin/Services/Overtime/OvertimeService.svc/GetOvertimeDetailByDate/4/3-24-2017
        public IList<OverTimeManagementDetail> GetForEmployeeByDateRange(int employeeLookupId, DateTime fromDate, DateTime toDate)
        {
            // Query SPList
            var overTimeManagementDetailItems = GetByQuery($"<Where><And><Eq><FieldRef Name='Employee' LookupId='TRUE' /><Value Type='Lookup'>{employeeLookupId}</Value></Eq><And><Geq><FieldRef Name='OvertimeFrom' /><Value IncludeTimeValue='TRUE' Type='DateTime'>{fromDate.ToString(StringConstant.DateFormatTZForCAML)}</Value></Geq><Leq><FieldRef Name='OvertimeFrom' /><Value IncludeTimeValue='TRUE' Type='DateTime'>{toDate.ToString(StringConstant.DateFormatTZForCAML)}</Value></Leq></And></And></Where>"
                           , string.Empty);

            return overTimeManagementDetailItems;

        }

        public IList<OverTimeManagementDetail> GetOvertimeEmployeeByDate(int employeeLookupId, DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            var startDateString = startDate.ToString(StringConstant.DateFormatTZForCAML);
            var endDateString = endDate.ToString(StringConstant.DateFormatTZForCAML);
            // Query SPList
            var overTimeManagementDetailItems = GetByQuery($@"
                <Where>
                    <And>
                        <Or>
                            <IsNull>
                                <FieldRef Name='ApprovalStatus' />
                            </IsNull>
                            <Eq>
                                <FieldRef Name='ApprovalStatus' />
                                <Value Type='Text'>true</Value>
                            </Eq>
                        </Or>
                        <And>
                            <Eq>
                                <FieldRef Name='Employee' LookupId='TRUE' />
                                <Value Type='Lookup'>{employeeLookupId}</Value>
                            </Eq>
                            <And>
                                <Geq>
                                    <FieldRef Name='CommonDate' />
                                    <Value IncludeTimeValue='TRUE' Type='DateTime'>{startDateString}</Value>
                                </Geq>
                                <Leq>
                                    <FieldRef Name='CommonDate' />
                                    <Value IncludeTimeValue='TRUE' Type='DateTime'>{endDateString}</Value>
                                </Leq>
                            </And>
                        </And>
                    </And>       
                </Where>", string.Empty);

            return overTimeManagementDetailItems;

        }

        public IList<OverTimeManagementDetail> GetByMasterIds(IList<int> ids)
        {
            string query = QueryBuilder.In(StringConstant.OverTimeManagementDetailList.OvertimeManagementID, "Lookup", ids);
            var overTimeManagementDetailItems = GetByQuery(query);
            return overTimeManagementDetailItems;
        }

        public void BulkInsert(List<OverTimeManagementDetail> newItems)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList overtimeManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        if (overtimeManagementDetail != null)
                        {
                            web.AllowUnsafeUpdates = true;
                            Guid listGuid = overtimeManagementDetail.ID;
                            StringBuilder query = new StringBuilder();
                            query.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                            foreach (var item in newItems)
                            {
                                query.AppendFormat("<Method ID=\"{0}\">" +
                                "<SetList>{1}</SetList>" +
                                "<SetVar Name=\"ID\">New</SetVar>" +
                                "<SetVar Name=\"Cmd\">Save</SetVar>" +
                                "<SetVar Name=\"{2}Employee\">" + item.Employee.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}OvertimeTo\">" + item.OvertimeTo.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</SetVar>" +
                                "<SetVar Name=\"{2}OvertimeFrom\">" + item.OvertimeFrom.ToString("yyyy-MM-ddTHH:mm:ssZ") + "</SetVar>" +
                                "<SetVar Name=\"{2}OvertimeManagementID\">" + item.OvertimeManagementID.LookupId + "</SetVar>" +
                                //"<SetVar Name=\"{2}HM\">" + item.HM + "</SetVar>" +
                                //"<SetVar Name=\"{2}KD\">" + item.KD + "</SetVar>" +
                                "<SetVar Name=\"{2}CompanyTransport\">" + item.CompanyTransport + "</SetVar>" +
                                "<SetVar Name=\"{2}Task\">" + item.Task + "</SetVar>" +
                                "<SetVar Name=\"{2}WorkingHours\">" + item.WorkingHours + "</SetVar>" +
                                "</Method>", "OvertimeEmployeeDetails", listGuid,
                                "urn:schemas-microsoft-com:office:office#");
                            }
                            query.Append("</Batch>");
                            web.ProcessBatchData(query.ToString());
                            web.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
        }

        public void BulkUpdate(List<OverTimeManagementDetail> newItems)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList overtimeManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        if (overtimeManagementDetail != null)
                        {
                            web.AllowUnsafeUpdates = true;
                            Guid listGuid = overtimeManagementDetail.ID;
                            StringBuilder query = new StringBuilder();
                            query.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                            foreach (var item in newItems)
                            {
                                query.AppendFormat("<Method>" +
                                     "<SetVar Name=\"Cmd\">Save</SetVar>" +
                                    "<SetList Scope=\"Request\" >{1}</SetList>" +
                                     "<SetVar Name=\"ID\">" + item.ID + "</SetVar>" +
                                    "<SetVar Name=\"{2}Employee\">" + item.Employee.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}OvertimeTo\">" + item.OvertimeTo.ToString("yyyy-MM-ddTHH:mm:ss") + "</SetVar>" +
                                    "<SetVar Name=\"{2}OvertimeFrom\">" + item.OvertimeFrom.ToString("yyyy-MM-ddTHH:mm:ss") + "</SetVar>" +
                                    "<SetVar Name=\"{2}OvertimeManagementID\">" + item.OvertimeManagementID.LookupId + "</SetVar>" +
                                    //"<SetVar Name=\"{2}HM\">" + item.HM + "</SetVar>" +
                                    //"<SetVar Name=\"{2}KD\">" + item.KD + "</SetVar>" +
                                    "<SetVar Name=\"{2}CompanyTransport\">" + item.CompanyTransport + "</SetVar>" +
                                    "<SetVar Name=\"{2}Task\">" + item.Task + "</SetVar>" +
                                    "<SetVar Name=\"{2}WorkingHours\">" + item.WorkingHours + "</SetVar>" +
                                     "<SetVar Name=\"{2}SummaryLinks\">" + item.SummaryLinks + "</SetVar>" +
                                    "</Method>", "OvertimeEmployeeDetails", listGuid,
                                    "urn:schemas-microsoft-com:office:office#");
                            }
                            query.Append("</Batch>");
                            web.ProcessBatchData(query.ToString());
                            web.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
        }

        public void DeleteBatch(List<OverTimeManagementDetail> deleteItems)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;

                        SPList overtimeManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        if (deleteItems.Count > 0)
                        {
                            StringBuilder sbDelete = new StringBuilder();
                            sbDelete.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                            string stringcommand = "<Method><SetList Scope=\"Request\">" + overtimeManagementDetail.ID + "</SetList><SetVar Name=\"ID\">{0}</SetVar><SetVar Name=\"Cmd\">Delete</SetVar></Method>";
                            foreach (var item in deleteItems)
                            {
                                sbDelete.Append(string.Format(stringcommand, item.ID.ToString()));
                            }

                            sbDelete.Append("</Batch>");
                            web.ProcessBatchData(sbDelete.ToString());
                            web.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
        }
    }
}
