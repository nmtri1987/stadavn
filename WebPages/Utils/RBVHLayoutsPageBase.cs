using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RBVH.Stada.Intranet.WebPages.Utils
{
    public abstract class RBVHLayoutsPageBase : LayoutsPageBase
    {
        #region Properties

        protected abstract HtmlGenericControl GetDiv_Success();
        protected abstract Label GetLabel_Success();
        protected abstract HtmlGenericControl GetDiv_Info();
        protected abstract Label GetLabel_Info();
        protected abstract HtmlGenericControl GetDiv_Warning();
        protected abstract Label GetLabel_Warning();
        protected abstract HtmlGenericControl GetDiv_Error();
        protected abstract Label GetLabel_Error();

        public virtual string LogCategory { get; set; }

        #endregion

        #region Utilities

        protected virtual void WriteLog(Exception ex)
        {
            SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(LogCategory, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
        }
        protected virtual string AppendMessage(string messageOrginal, string message)
        {
            return messageOrginal + (string.IsNullOrEmpty(messageOrginal) ? message : "<br/>" + message);
        }

        protected virtual void ShowMessageSuccess(string message)
        {
            Label m_Lable_Success = GetLabel_Success();
            HtmlGenericControl m_Div_Success = GetDiv_Success();
            m_Lable_Success.Text = message;
            m_Div_Success.Visible = true;
        }
        protected virtual void ShowMessageError(string message)
        {
            Label m_Label_Error = GetLabel_Error();
            HtmlGenericControl m_Div_Error = GetDiv_Error();
            m_Label_Error.Text = message;
            m_Div_Error.Visible = true;
        }
        protected virtual void ShowMessageInfo(string message)
        {
            Label m_Label_Info = GetLabel_Info();
            HtmlGenericControl m_Div_Info = GetDiv_Info();
            m_Label_Info.Text = message;
            m_Div_Info.Visible = true;
        }
        protected virtual void ShowMessageWarning(string message)
        {
            Label m_Label_Warning = GetLabel_Warning();
            HtmlGenericControl m_Div_Warning = GetDiv_Warning();
            m_Label_Warning.Text = message;
            m_Div_Warning.Visible = true;
        }
        protected virtual void HideAllMessage()
        {
            HtmlGenericControl m_Div_Success = GetDiv_Success();
            HtmlGenericControl m_Div_Info = GetDiv_Info();
            HtmlGenericControl m_Div_Warning = GetDiv_Warning();
            HtmlGenericControl m_Div_Error = GetDiv_Error();
            m_Div_Success.Visible = false;
            m_Div_Info.Visible = false;
            m_Div_Warning.Visible = false;
            m_Div_Error.Visible = false;
        }

        protected virtual void GoBack()
        {
            string m_SourceUrl = Request[StringConstants.SourceUrl];
            if (string.IsNullOrEmpty(m_SourceUrl))
                m_SourceUrl = StringConstants.PageOverviewURL;

            Response.Redirect(m_SourceUrl);
        }

        #endregion
    }
}
