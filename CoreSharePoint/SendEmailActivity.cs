using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;
using System.Globalization;
using System.Threading;

namespace RBVH.Core.SharePoint
{
    public class SendEmailActivity
    {
        public void SendMail(string siteURL, string subject, string toAddress, bool appendHtmlTag, bool htmlEncode, string message, bool newThread = true)
        {
            if (newThread)
            {
                Thread thread = new Thread(delegate ()
                {
                    SendMail(siteURL, subject, toAddress, appendHtmlTag, htmlEncode, message);
                });

                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                SendMail(siteURL, subject, toAddress, appendHtmlTag, htmlEncode, message);
            }
        }

        private static void SendMail(string siteURL, string subject, string toAddress, bool appendHtmlTag, bool htmlEncode, string message)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteURL))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        try
                        {
                            SPUtility.SendEmail(web, appendHtmlTag, htmlEncode, toAddress, subject, message);
                        }
                        catch (Exception ex)
                        {
                            ULSLogging.Log(new SPDiagnosticsCategory("STADA - SendEmailActivity - SendMail",
                                         TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                                     string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                        }
                    }
                }
            });
        }

        public string OvertimeRequestEmailHtmlTemplate(string proposer, string department, string from, string to, string place, string fullname, string employID, string workingHour, string workContent, string HM, string KD, string date)
        {
            return string.Concat("<h4><span style='color: #000000;'><strong><em><span style='font-size: 12pt; font-family: 'Arial',sans-serif;'>Hi/Chào ", fullname, "</span></em></strong></span></h4>",
                 "<p><strong><em> Người đề nghị / Proposer </em></strong>: &hellip;&hellip;&hellip;&hellip;&hellip;&hellip;",
                  proposer,
                 "&hellip;&hellip;&hellip;&hellip;&hellip;&hellip;&hellip;&hellip;<strong><em> Bộ phận / Dept.</em></strong>:&hellip;&hellip;&hellip;&hellip;",
                  department,
                 " &hellip;&hellip;&hellip;&hellip;&hellip;&hellip;&hellip;&hellip;.</p>",
                 "<p><strong><em> Thời gian / Time </em></strong>: Từ Ngày /<em> from </em> &hellip;&hellip;&hellip;",
                  date,
                 "&hellip;&hellip;đến ngày/<em> to </em> &hellip;&hellip;&hellip;",
                  date,
                 "&hellip;&hellip;&hellip;&hellip;<strong><em> Địa điểm / Place </em></strong>:&hellip;&hellip;&hellip;",
                 place,
                 "&hellip;&hellip;&hellip;&hellip;...</p>",
                 "<p> &nbsp;</p>",
                 "<table style = 'height: 206px;' border = '1' width = '100%'><tbody><tr><td style = 'background-color: #d6e3bc;' rowspan = '2' width = '191'><p><strong> &nbsp;</strong></p><p><strong> &nbsp; &nbsp; &nbsp; &nbsp; Họ & Tên / <em> Full name </em></strong></p></td><td style = 'background-color: #d6e3bc;' rowspan = '2' width = '48'>",
                "<p><strong> &nbsp;</strong></p><p><strong> Mã số NV /<em>Employee ID </em></strong></p></td>",
                "<td style = 'background-color: #d6e3bc;' rowspan = '2' width = '63'><p><strong> Số giờ làm việc trong ngày (giờ) / <em> Working hour(s) </em></strong></p>",
                "</td><td style = 'background-color: #d6e3bc;' rowspan = '2' width = '66'><p><strong> Số giờ làm thêm /<em> Overtime hour(s </em>) </strong></p>",
                "<p><strong> Từ /<em> from:</em> &hellip;h Đến/<em> to </em>:&hellip;h </strong></p></td><td style = 'background-color: #d6e3bc;' rowspan = '2' width = '93'><p><strong> &nbsp;</strong></p>",
                "<p><strong> Nội dung công việc / <em> Work content </em></strong></p></td>",
                "<td style = 'background-color: #d6e3bc;' colspan = '2' width = '96'><p><strong> Xe đưa đón / <em> Company bus </em></strong></p></td>",
                "</tr><tr><td style = 'background-color: #d6e3bc;' width = '48'><p> HM </p></td><td style = 'background-color: #d6e3bc;' width = '48'><p> KD </p>",
                "</td></tr><tr><td width = '191'><p> ", fullname, "</p></td><td width = '48'> <p>", employID, "</p></td> <td width = '63'> <p> &nbsp;",
                "", workingHour, "</p></td> <td width = '66'><p>", from, " - ", to, "</p> </td> <td width = '93'>  <p>", workContent, "</p>",
                 "</td> <td width = '48'><p>", HM, "</p></td> <td width = '48'>  <p>", KD, "</p> </td> </tr>   </tbody> </table>");

        }
    }
}
