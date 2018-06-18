using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.ConfigurationList.Url)]
    public class Configuration : EntityBase
    {
        [ListColumn(StringConstant.ConfigurationList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.ConfigurationList.Fields.Key)]
        public string Key { get; set; }

        [ListColumn(StringConstant.ConfigurationList.Fields.Value)]
        public string Value { get; set; }
    }
}
