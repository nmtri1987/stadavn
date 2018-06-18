namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class EmployeeShiftTimeResult
    {
        public EmployeeShiftTimeResult()
        {
            EmployeeShiftTime = new EmployeeShiftTime();
        }

        public EmployeeShiftTime EmployeeShiftTime { get; set; }
        public bool IsHasData { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
    }
}