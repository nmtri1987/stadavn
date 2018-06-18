namespace RBVH.Stada.Intranet.WebPages.Models
{
    public  class OvertimeDataResult
    {
        public CodeMessageResult CodeMessageResult { get; set; }
        public int OvertimeId { get; set; }

        public OvertimeDataResult()
        {
            CodeMessageResult = new CodeMessageResult();
        }
    }
}
