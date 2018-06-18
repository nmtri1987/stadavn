using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Net;
using Microsoft.SharePoint.Client;
using System.Text;
using RBVH.Core.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.ListEventReceiver.ChangeShiftEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ChangeShiftEventReceiver : SPItemEventReceiver
    {
        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);

            try
            {
                object fromDateObj = properties.AfterProperties[StringConstant.ChangeShiftList.FromDateField];
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
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ChangeShiftEventReceiver - ItemAdding fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// An item is  added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                base.ItemAdded(properties);
                string changeShiftItemID = Convert.ToString(properties.ListItemId);

                var changeShiftRequestMailService = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendChangeShiftRequestMail/{1}/{2}",
                    properties.WebUrl, changeShiftItemID, "DH");

                var delegationChangeShiftRequestMailService = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendDelegationChangeShiftRequestMail/{1}/{2}",
                    properties.WebUrl, changeShiftItemID, "DH");

                CallService(changeShiftRequestMailService);
                CallService(delegationChangeShiftRequestMailService);
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ChangeShiftEventReceiver - ItemAdded fn",
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
                ChangeShiftManagementDAL notovertimeDAL = new ChangeShiftManagementDAL(properties.WebUrl);
                var item = notovertimeDAL.GetByID(itemID);
                string url = string.Empty;
                if (item != null)
                {
                    SPUser currentUser = properties.Web.CurrentUser;
                    if (item.ApprovalStatus.Equals("1"))
                    {
                        var changeShiftRequestMailService = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendChangeShiftRequestMail/{1}/{2}", properties.WebUrl, itemID, "BOD");
                        var delegationChangeShiftRequestMailService = string.Format("{0}/_vti_bin/Services/Email/EmailService.svc/SendDelegationChangeShiftRequestMail/{1}/{2}", properties.WebUrl, itemID, "BOD");
                        CallService(changeShiftRequestMailService);
                        CallService(delegationChangeShiftRequestMailService);
                    }
                    else if (item.ApprovalStatus.Equals("Approved"))
                    {
                        //update new shift
                        string updateShiftUrl = string.Format("{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/UpdateShiftDetailForWorkflow/{1}/{2}/{3}/{4}",
                                properties.WebUrl, item.Requester.LookupId, item.FromDate.ToString(StringConstant.DateFormatMMddyyyy), item.ToDate.ToString(StringConstant.DateFormatMMddyyyy), item.ToShift.LookupId);
                        CallService(updateShiftUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - ChangeShiftEventReceiver - ItemUpdated fn",
                    TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        private void CallService(string url)
        {
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            client.DownloadString(url);
        }
    }
}