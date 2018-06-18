using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class EmailTemplateDAL : BaseDAL<EmailTemplate>
    {
        public EmailTemplateDAL(string siteUrl) : base(siteUrl)
        {
        }

        public EmailTemplate GetByKey(string key)
        {
            EmailTemplate emailTemplate = null;
            
            string queryStr = $@"<Where>
                                    <Eq>
                                        <FieldRef Name='MailKey' />
                                        <Value Type='Text'>{key}</Value>
                                    </Eq>
                                </Where>";

            var emailTemplateItems = this.GetByQuery(queryStr);
            if (emailTemplateItems != null && emailTemplateItems.Count > 0)
            {
                emailTemplate = emailTemplateItems[0];
            }

            return emailTemplate;
        }
    }
}
