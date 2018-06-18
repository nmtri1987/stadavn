using System;
using System.Linq;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Net;
using System.Text;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using System.Globalization;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Constants;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.ListEventReceiver.NotOvertimeEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class NotOvertimeEventReceiver : SPItemEventReceiver
    {
        private OverTimeManagementDetailDAL _overTimeManagementDetailDAL { get; set; }
        private OverTimeManagementDAL _overTimeManagementlDAL { get; set; }

        private const string WORKCONTENT = " - Đã được duyệt không đi tăng ca";

        private const string URL = "/Lists/NotOverTimeManagement/DispForm.aspx?ID={0}&TextOnly=true";

        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);

            try
            {
                object fromDateObj = properties.AfterProperties[StringConstant.NotOvertimeList.DateField];
                if (fromDateObj != null)
                {
                    DateTime fromDate = Convert.ToDateTime(fromDateObj);
                    DateTime reqDueDate = fromDate.Date;
                    //if (reqDueDate == DateTime.Now.Date)
                    //{
                    //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    //}
                    //else
                    //{
                    //    reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                    //}
                    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    properties.AfterProperties[StringConstant.CommonSPListField.CommonReqDueDateField] = reqDueDate.ToString(StringConstant.DateFormatTZForCAML);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - NotOvertime Event Receiver - ItemAdding fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            try
            {
                base.ItemAdded(properties);

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(properties.SiteId))
                    {
                        using (SPWeb web = site.OpenWeb(properties.Web.ID))
                        {
                            string itemID = Convert.ToString(properties.ListItemId);
                            bool isBOD = false;
                            var approverString = Convert.ToString(properties.ListItem[StringConstant.NotOvertimeList.DHField]);
                            if (!string.IsNullOrEmpty(approverString))
                            {
                                SPListItem curItem = properties.ListItem;

                                var bodGroup = web.SiteGroups.GetByName("BOD");

                                var spUserField = properties.ListItem.Fields.GetField(StringConstant.NotOvertimeList.DHField);
                                SPFieldUserValue spNewUserFieldValue = (SPFieldUserValue)spUserField.GetFieldValue(approverString);
                                SPUser spNewBodUser = web.EnsureUser(spNewUserFieldValue.LookupValue);
                                var bodUser = bodGroup.Users.GetByLoginNoThrow(spNewBodUser.LoginName);
                                if (bodUser != null)
                                {
                                    isBOD = true;
                                }
                            }

                            var notOverTimeRequestEmail = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendNotOverTimeRequestEmail/{1}/{2}", properties.WebUrl, itemID, isBOD ? "BOD" : "DH");
                            var delegationNotOverTimeRequestEmail = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendDelegationNotOverTimeRequestEmail/{1}/{2}", properties.WebUrl, itemID, isBOD ? "BOD" : "DH");
                            SendEmail(notOverTimeRequestEmail);
                            SendEmail(delegationNotOverTimeRequestEmail);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - NotOvertime Event Receiver - ItemAdded fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            try
            {
                int itemID = Convert.ToInt32(properties.ListItemId);
                NotOvertimeManagementDAL notovertimeDAL = new NotOvertimeManagementDAL(properties.WebUrl);
                OverTimeManagementDetailDAL overTimeManagementDetailDAL = new OverTimeManagementDetailDAL(properties.WebUrl);
                var item = notovertimeDAL.GetByID(itemID);

                string url = string.Empty;
                if (item != null)
                {
                    if (item.ApprovalStatus.Equals("1"))
                    {
                        var notOverTimeRequestEmail = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendNotOverTimeRequestEmail/{1}/{2}", properties.WebUrl, itemID, "BOD");
                        var delegationNotOverTimeRequestEmail = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendDelegationNotOverTimeRequestEmail/{1}/{2}", properties.WebUrl, itemID, "BOD");
                        SendEmail(notOverTimeRequestEmail);
                        SendEmail(delegationNotOverTimeRequestEmail);
                    }
                    else if (item.ApprovalStatus.Equals("Approved"))
                    {
                        // udpate overtimeDetail
                        var overtimeDetail = overTimeManagementDetailDAL.GetOvertimeEmployeeByDate(item.Requester.LookupId, item.Date).Where(x => x.ApprovalStatus != null && x.ApprovalStatus.LookupValue == "true").SingleOrDefault();
                        if (overtimeDetail != null)
                        {
                            overtimeDetail.Task = WORKCONTENT;
                            overtimeDetail.SummaryLinks = string.Format(URL, itemID);
                            overTimeManagementDetailDAL.BulkUpdate(new List<OverTimeManagementDetail>() { overtimeDetail });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - NotOvertime Event Receiver - ItemUpdated fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        private void SendEmail(string url)
        {
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            client.DownloadString(url);
        }
    }
}