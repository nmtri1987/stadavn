using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// DelegationOfNewTask
    /// </summary>
    [ListUrl(StringConstant.DelegationsOfNewTaskList.Url)]
    public class DelegationOfNewTask : EntityBase
    {
        public DelegationOfNewTask()
        {
        }
        [ListColumn(StringConstant.DelegationsList.Fields.Title)]
        public string Title { get; set; }
        [ListColumn(StringConstant.DelegationsList.Fields.ModuleName)]
        public string ModuleName { get; set; }
        [ListColumn(StringConstant.DelegationsList.Fields.VietnameseModuleName)]
        public string VietnameseModuleName { get; set; }
        [ListColumn(StringConstant.DelegationsOfNewTaskList.Fields.FromDate)]
        public DateTime FromDate { get; set; }

        [ListColumn(StringConstant.DelegationsOfNewTaskList.Fields.ToDate)]
        public DateTime ToDate { get; set; }

        [ListColumn(StringConstant.DelegationsOfNewTaskList.Fields.FromEmployee)]
        public LookupItem FromEmployee { get; set; }

        [ListColumn(StringConstant.DelegationsOfNewTaskList.Fields.ToEmployee)]
        public List<LookupItem> ToEmployee { get; set; }

        [ListColumn(StringConstant.DelegationsOfNewTaskList.Fields.ListUrl)]
        public string ListUrl { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedField)]
        public DateTime Modified { get; set; }
    }
}
