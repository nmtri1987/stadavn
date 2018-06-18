using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.WebControls;

namespace RBVH.Core.SharePoint
{
    public class NotificationStatusHelper
    {
        public static void SetStatus(HtmlForm form, string title, string message, SPPageStatusColor color)
        {
            var statusSetter = new SPPageStatusSetter { Visible = true };
            statusSetter.AddStatus(title + ":", message, color);
            form.Controls.Add(statusSetter);
        }

        public static void SetErrorStatus(HtmlForm form, string title, string message)
        {
            SetStatus(form, title, message, SPPageStatusColor.Red);
        }

        public static void SetWarningStatus(HtmlForm form, string title, string message)
        {
            SetStatus(form, title, message, SPPageStatusColor.Yellow);
        }

        public static void SetInformationStatus(HtmlForm form, string title, string message)
        {
            SetStatus(form, title, message, SPPageStatusColor.Green);
        }
    }
}
