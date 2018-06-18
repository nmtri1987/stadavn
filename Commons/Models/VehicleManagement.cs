using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Models
{
  
    [ListUrl("/Lists/VehicleManagement")]
    public class VehicleManagement : EntityBase
    {
        [ListColumn(StringConstant.CommonSPListField.RequesterField)]
        public LookupItem Requester { get; set; }
        [ListColumn(StringConstant.VehicleManagementList.CommonFrom)]
        public DateTime From { get; set; }
        [ListColumn(StringConstant.VehicleManagementList.To)]
        public DateTime ToDate { get; set; }
        [ListColumn(StringConstant.VehicleManagementList.Reason)]
        public string Reason { get; set; }
        [ListColumn(StringConstant.VehicleManagementList.DH)]
        public User DepartmentHead { get; set; }
        [ListColumn(StringConstant.VehicleManagementList.BOD)]
        public User BOD { get; set; }
        [ListColumn(StringConstant.VehicleManagementList.VehicleType)]
        public string Type { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonDepartmentField)]
        public LookupItem CommonDepartment { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonLocationField)]
        public LookupItem Location { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonCommentField)]
        public string CommonComment { get; set; }
        [ListColumn(StringConstant.CommonSPListField.ApprovalStatusField)]
        public string ApprovalStatus { get; set; }
        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }
        [ListColumn(StringConstant.CommonSPListField.CommonReqDueDateField)]
        public DateTime RequestDueDate { get; set; }
    }
}
