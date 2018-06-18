namespace RBVH.Stada.Intranet.Biz.Report.RecruitmentModule.Constants
{
    public class ComputerSkillValues
    {
        /// <summary>
        /// Word
        /// </summary>
        public const string Word = "Word";

        /// <summary>
        /// Power point
        /// </summary>
        public const string PowerPoint = "Power point";

        /// <summary>
        /// Excel
        /// </summary>
        public const string Excel = "Excel";

        /// <summary>
        /// Internet
        /// </summary>
        public const string Internet = "Internet";

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
                return new string[] { Word, PowerPoint, Excel, Internet, Others };
            }
        }
    }
}
