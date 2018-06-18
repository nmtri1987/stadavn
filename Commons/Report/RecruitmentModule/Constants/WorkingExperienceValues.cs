namespace RBVH.Stada.Intranet.Biz.Report.RecruitmentModule.Constants
{
    public class WorkingExperienceValues
    {
        /// <summary>
        /// Dưới 1 năm/Under 1 year
        /// </summary>
        public const string Under1Year = "Dưới 1 năm/Under 1 year";

        /// <summary>
        /// Từ 1 đến dưới 2 năm/From 1 to 2 years
        /// </summary>
        public const string From1To2Years = "Từ 1 đến dưới 2 năm/From 1 to 2 years";

        /// <summary>
        /// Không cần/Unnecessary
        /// </summary>
        public const string Unnecessary = "Không cần/Unnecessary";

        /// <summary>
        /// Khác/Others
        /// </summary>
        public const string Others = "Khác/Others";


        /// <summary>
        /// Get all of values.
        /// </summary>
        /// <returns></returns>
        public static string[] AllValues
        {
            get
            {
                return new string[] { Under1Year, From1To2Years, Unnecessary, Others };
            }
        }
    }
}
