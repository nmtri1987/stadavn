using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/MailTemplate")]
    public class EmailTemplate : EntityBase
    {
        [ListColumn(StringConstant.EmailTemplateList.MailKeyField)]
        public string MailKey { get; set; }

        [ListColumn(StringConstant.EmailTemplateList.MailSubjectField)]
        public string MailSubject { get; set; }
        [ListColumn(StringConstant.EmailTemplateList.MailBodyField)]
        public string MailBody { get; set; }
    }
}
