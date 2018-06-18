using System;

namespace RBVH.Stada.Intranet.Biz.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ListUrl : Attribute
    {
        public string Url { get; set; }

        public ListUrl(string listUrl)
        {
            Url = listUrl;
        }
    }
}
