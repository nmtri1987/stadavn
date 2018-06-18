using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// WorkflowHistory Model.
    /// </summary>
    [ListUrl(WorkflowHistoryList.Url)]
    public class WorkflowHistory : EntityBase
    {
        [ListColumn(WorkflowHistoryList.Fields.Status)]
        public string Status { get; set; }

        [ListColumn(WorkflowHistoryList.Fields.VietnameseStatus)]
        public string VietnameseStatus { get; set; }

        [ListColumn(WorkflowHistoryList.Fields.PostedBy)]
        public string PostedBy { get; set; }

        [ListColumn(WorkflowHistoryList.Fields.CommonDate)]
        public DateTime CommonDate { get; set; }

        [ListColumn(WorkflowHistoryList.Fields.CommonComment)]
        public string CommonComment { get; set; }

        [ListColumn(WorkflowHistoryList.Fields.ListName)]
        public string ListName { get; set; }

        [ListColumn(WorkflowHistoryList.Fields.CommonItemID)]
        public int CommonItemID { get; set; }
    }
}
