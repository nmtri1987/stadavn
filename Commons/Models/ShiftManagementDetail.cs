using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl("/Lists/ShiftManagementDetail")]
    public class ShiftManagementDetail : EntityBase
    {
        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftManagementIDField)]
        public LookupItem ShiftManagementID { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.EmployeeField)]
        public LookupItem Employee { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.EmployeeIDField, true)]
        public LookupItem EmployeeID { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime1Field)]
        public LookupItem ShiftTime1 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime1ApprovalField)]
        public bool ShiftTime1Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime2Field)]
        public LookupItem ShiftTime2 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime2ApprovalField)]
        public bool ShiftTime2Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime3Field)]
        public LookupItem ShiftTime3 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime3ApprovalField)]
        public bool ShiftTime3Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime4Field)]
        public LookupItem ShiftTime4 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime4ApprovalField)]
        public bool ShiftTime4Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime5Field)]
        public LookupItem ShiftTime5 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime5ApprovalField)]
        public bool ShiftTime5Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime6Field)]
        public LookupItem ShiftTime6 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime6ApprovalField)]
        public bool ShiftTime6Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime7Field)]
        public LookupItem ShiftTime7 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime7ApprovalField)]
        public bool ShiftTime7Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime8Field)]
        public LookupItem ShiftTime8 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime8ApprovalField)]
        public bool ShiftTime8Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime9Field)]
        public LookupItem ShiftTime9 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime9ApprovalField)]
        public bool ShiftTime9Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime10Field)]
        public LookupItem ShiftTime10 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime10ApprovalField)]
        public bool ShiftTime10Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime11Field)]
        public LookupItem ShiftTime11 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime11ApprovalField)]
        public bool ShiftTime11Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime12Field)]
        public LookupItem ShiftTime12 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime12ApprovalField)]
        public bool ShiftTime12Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime13Field)]
        public LookupItem ShiftTime13 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime13ApprovalField)]
        public bool ShiftTime13Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime14Field)]
        public LookupItem ShiftTime14 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime14ApprovalField)]
        public bool ShiftTime14Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime15Field)]
        public LookupItem ShiftTime15 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime15ApprovalField)]
        public bool ShiftTime15Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime16Field)]
        public LookupItem ShiftTime16 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime16ApprovalField)]
        public bool ShiftTime16Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime17Field)]
        public LookupItem ShiftTime17 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime17ApprovalField)]
        public bool ShiftTime17Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime18Field)]
        public LookupItem ShiftTime18 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime18ApprovalField)]
        public bool ShiftTime18Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime19Field)]
        public LookupItem ShiftTime19 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime19ApprovalField)]
        public bool ShiftTime19Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime20Field)]
        public LookupItem ShiftTime20 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime20ApprovalField)]
        public bool ShiftTime20Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime21Field)]
        public LookupItem ShiftTime21 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime21ApprovalField)]
        public bool ShiftTime21Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime22Field)]
        public LookupItem ShiftTime22 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime22ApprovalField)]
        public bool ShiftTime22Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime23Field)]
        public LookupItem ShiftTime23 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime23ApprovalField)]
        public bool ShiftTime23Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime24Field)]
        public LookupItem ShiftTime24 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime24ApprovalField)]
        public bool ShiftTime24Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime25Field)]
        public LookupItem ShiftTime25 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime25ApprovalField)]
        public bool ShiftTime25Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime26Field)]
        public LookupItem ShiftTime26 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime26ApprovalField)]
        public bool ShiftTime26Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime27Field)]
        public LookupItem ShiftTime27 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime27ApprovalField)]
        public bool ShiftTime27Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime28Field)]
        public LookupItem ShiftTime28 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime28ApprovalField)]
        public bool ShiftTime28Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime29Field)]
        public LookupItem ShiftTime29 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime29ApprovalField)]
        public bool ShiftTime29Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime30Field)]
        public LookupItem ShiftTime30 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime30ApprovalField)]
        public bool ShiftTime30Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime31Field)]
        public LookupItem ShiftTime31 { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.ShiftTime31ApprovalField)]
        public bool ShiftTime31Approval { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.MonthField, true)]
        public LookupItem Month { get; set; }

        [ListColumn(StringConstant.ShiftManagementDetailList.YearField, true)]
        public LookupItem Year { get; set; }

        public ShiftManagementDetail()
        {
            ShiftTime1 = new LookupItem();
            ShiftTime2 = new LookupItem();
            ShiftTime3 = new LookupItem();
            ShiftTime4 = new LookupItem();
            ShiftTime5 = new LookupItem();
            ShiftTime6 = new LookupItem();
            ShiftTime7 = new LookupItem();
            ShiftTime8 = new LookupItem();
            ShiftTime9 = new LookupItem();
            ShiftTime10 = new LookupItem();
            ShiftTime11 = new LookupItem();
            ShiftTime12 = new LookupItem();
            ShiftTime13 = new LookupItem();
            ShiftTime14 = new LookupItem();
            ShiftTime15 = new LookupItem();
            ShiftTime16 = new LookupItem();
            ShiftTime17 = new LookupItem();
            ShiftTime18 = new LookupItem();
            ShiftTime19 = new LookupItem();
            ShiftTime20 = new LookupItem();
            ShiftTime21 = new LookupItem();
            ShiftTime22 = new LookupItem();
            ShiftTime23 = new LookupItem();
            ShiftTime24 = new LookupItem();
            ShiftTime25 = new LookupItem();
            ShiftTime26 = new LookupItem();
            ShiftTime27 = new LookupItem();
            ShiftTime28 = new LookupItem();
            ShiftTime29 = new LookupItem();
            ShiftTime30 = new LookupItem();
            ShiftTime31 = new LookupItem();
        }
    }
}
