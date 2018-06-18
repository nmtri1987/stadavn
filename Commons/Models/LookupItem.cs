namespace RBVH.Stada.Intranet.Biz.Models
{
    public class LookupItem
    {
        public int LookupId { get; set; }
        public string LookupValue { get; set; }

        public LookupItem()
        {
            LookupId = 0;
            LookupValue = string.Empty;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0};#{1}", LookupId, LookupValue);
        }
    }
}