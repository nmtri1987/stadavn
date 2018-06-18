using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.WebPages.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Webservices.Helper
{
    public class MessageResultHelper
    {
        public static string GetRequestStatusMessage(string currentStatus)
        {
            string msgResult = "";

            currentStatus = currentStatus.ToLower();
            if (currentStatus == ApprovalStatus.Approved.ToLower())
            {
                msgResult = WebPageResourceHelper.GetResourceString("RequestStatusApproved");
            }
            else if (currentStatus == ApprovalStatus.Rejected.ToLower())
            {
                msgResult = WebPageResourceHelper.GetResourceString("RequestStatusRejected");
            }
            else if (currentStatus == ApprovalStatus.Cancelled.ToLower())
            {
                msgResult = WebPageResourceHelper.GetResourceString("RequestStatusCancelled");
            }
            else if (!string.IsNullOrEmpty(currentStatus))
            {
                msgResult = WebPageResourceHelper.GetResourceString("RequestStatusInProgress");
            }

            return msgResult;
        }

        public static string GetRequestExpiredMessage(DateTime requestDueDate)
        {
            string msgResult = "";

            //if (requestDueDate != null && requestDueDate != default(DateTime) && requestDueDate.Date < DateTime.Now.Date)
            //{
            //    msgResult = WebPageResourceHelper.GetResourceString("RequestExpiredMsgFormat");
            //    msgResult = string.Format(HttpUtility.UrlDecode(msgResult), requestDueDate.ToString(StringConstant.DateFormatddMMyyyy2));
            //}
            
            return msgResult;
        }
    }
}
