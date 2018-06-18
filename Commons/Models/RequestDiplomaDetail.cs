using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.RequestDiplomaDetailsList.Url)]
    public class RequestDiplomaDetail : EntityBase
    {
        public RequestDiplomaDetail()
        {
        }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.Title)]
        public string Title { get; set; }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.CurrentDiploma)]
        public string CurrentDiploma { get; set; }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.GraduationYear)]
        public string GraduationYear { get; set; }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.NewDiploma)]
        public string NewDiploma { get; set; }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.Faculty)]
        public string Faculty { get; set; }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.IssuedPlace)]
        public string IssuedPlace { get; set; }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.TrainingDuration)]
        public string TrainingDuration { get; set; }

        [ListColumn(StringConstant.RequestDiplomaDetailsList.Fields.Request)]
        public LookupItem Request { get; set; }
    }
}
