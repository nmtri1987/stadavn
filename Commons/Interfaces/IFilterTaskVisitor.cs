
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Interfaces
{
    public interface IFilterTaskVisitor
    {
        bool CountOnly { get; set; }
        IList<FilterTask> FilterTaskList { get; set; }
        int TotalCount { get; set; }
        void Visit(ShiftManagementDAL shiftManagementDAL);
        void Visit(ChangeShiftManagementDAL changeShiftManagementDAL);
        void Visit(OverTimeManagementDAL overTimeManagementDAL);
        void Visit(NotOvertimeManagementDAL notOvertimeManagementDAL);
        void Visit(LeaveManagementDAL leaveManagementDAL);
        void Visit(VehicleManagementDAL vehicleManagementDAL);
        void Visit(FreightManagementDAL freightManagementDAL);
        void Visit(BusinessTripManagementDAL businessTripManagementDAL);
        void Visit(RequestsDAL requestDAL);
        void Visit(EmployeeRequirementSheetDAL recruitmentDAL);
        void Visit(RequestForDiplomaSupplyDAL certificateDAL);
        void Visit(RequisitionOfMeetingRoomDAL requisitionOfMeetingRoomDAL);
        void Visit(GuestReceptionManagementDAL guestReceptionManagementDAL);
    }
}
