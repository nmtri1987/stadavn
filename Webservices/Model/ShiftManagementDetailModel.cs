using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.ISAPI.Services.LeaveManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class ShiftManagementDetailModel
    {
        protected LeaveManagementService _leaveManagementService = new LeaveManagementService();
        public int Id { get; set; }
        private bool _isExisted;
        public bool IsExisted
        {
            get
            {
                if (Id > 0)
                {
                    _isExisted = true;
                }
                else
                {
                    _isExisted = false;
                }
                return _isExisted;
            }
            set
            {
                _isExisted = value;
            }
        }

        public EmployeeInfoModel Employee { get; set; }
        public ShiftTimeDetailModel ShiftTime1 { get; set; }
        public ShiftTimeDetailModel ShiftTime2 { get; set; }
        public ShiftTimeDetailModel ShiftTime3 { get; set; }
        public ShiftTimeDetailModel ShiftTime4 { get; set; }
        public ShiftTimeDetailModel ShiftTime5 { get; set; }
        public ShiftTimeDetailModel ShiftTime6 { get; set; }
        public ShiftTimeDetailModel ShiftTime7 { get; set; }
        public ShiftTimeDetailModel ShiftTime8 { get; set; }
        public ShiftTimeDetailModel ShiftTime9 { get; set; }
        public ShiftTimeDetailModel ShiftTime10 { get; set; }
        public ShiftTimeDetailModel ShiftTime11 { get; set; }
        public ShiftTimeDetailModel ShiftTime12 { get; set; }
        public ShiftTimeDetailModel ShiftTime13 { get; set; }
        public ShiftTimeDetailModel ShiftTime14 { get; set; }
        public ShiftTimeDetailModel ShiftTime15 { get; set; }
        public ShiftTimeDetailModel ShiftTime16 { get; set; }
        public ShiftTimeDetailModel ShiftTime17 { get; set; }
        public ShiftTimeDetailModel ShiftTime18 { get; set; }
        public ShiftTimeDetailModel ShiftTime19 { get; set; }
        public ShiftTimeDetailModel ShiftTime20 { get; set; }
        public ShiftTimeDetailModel ShiftTime21 { get; set; }
        public ShiftTimeDetailModel ShiftTime22 { get; set; }
        public ShiftTimeDetailModel ShiftTime23 { get; set; }
        public ShiftTimeDetailModel ShiftTime24 { get; set; }
        public ShiftTimeDetailModel ShiftTime25 { get; set; }
        public ShiftTimeDetailModel ShiftTime26 { get; set; }
        public ShiftTimeDetailModel ShiftTime27 { get; set; }
        public ShiftTimeDetailModel ShiftTime28 { get; set; }
        public ShiftTimeDetailModel ShiftTime29 { get; set; }
        public ShiftTimeDetailModel ShiftTime30 { get; set; }
        public ShiftTimeDetailModel ShiftTime31 { get; set; }
        public int ShiftManagementID { get; set; }
        public bool NewApproval { get; set; }
        public List<ApprovalDayInfo> ApprovalDays { get; set; }

        #region Constructors and Methods

        public ShiftManagementDetailModel()
        {
            Employee = new EmployeeInfoModel();
            ShiftTime1 = new ShiftTimeDetailModel();
            ShiftTime2 = new ShiftTimeDetailModel();
            ShiftTime3 = new ShiftTimeDetailModel();
            ShiftTime4 = new ShiftTimeDetailModel();
            ShiftTime5 = new ShiftTimeDetailModel();
            ShiftTime6 = new ShiftTimeDetailModel();
            ShiftTime7 = new ShiftTimeDetailModel();
            ShiftTime8 = new ShiftTimeDetailModel();
            ShiftTime9 = new ShiftTimeDetailModel();
            ShiftTime10 = new ShiftTimeDetailModel();
            ShiftTime11 = new ShiftTimeDetailModel();
            ShiftTime12 = new ShiftTimeDetailModel();
            ShiftTime13 = new ShiftTimeDetailModel();
            ShiftTime14 = new ShiftTimeDetailModel();
            ShiftTime15 = new ShiftTimeDetailModel();
            ShiftTime16 = new ShiftTimeDetailModel();
            ShiftTime17 = new ShiftTimeDetailModel();
            ShiftTime18 = new ShiftTimeDetailModel();
            ShiftTime19 = new ShiftTimeDetailModel();
            ShiftTime20 = new ShiftTimeDetailModel();
            ShiftTime21 = new ShiftTimeDetailModel();
            ShiftTime22 = new ShiftTimeDetailModel();
            ShiftTime23 = new ShiftTimeDetailModel();
            ShiftTime24 = new ShiftTimeDetailModel();
            ShiftTime25 = new ShiftTimeDetailModel();
            ShiftTime26 = new ShiftTimeDetailModel();
            ShiftTime27 = new ShiftTimeDetailModel();
            ShiftTime28 = new ShiftTimeDetailModel();
            ShiftTime29 = new ShiftTimeDetailModel();
            ShiftTime30 = new ShiftTimeDetailModel();
            ShiftTime31 = new ShiftTimeDetailModel();
            ApprovalDays = new List<ApprovalDayInfo>();
        }
        public ShiftManagementDetail ToEntity()
        {
            var result = new ShiftManagementDetail
            {
                ID = Id,
                Employee = new LookupItem
                {
                    LookupId = Employee.Id,
                    LookupValue = Employee.FullName
                },
                ShiftManagementID = new LookupItem
                {
                    LookupId = ShiftManagementID,
                    LookupValue = ShiftManagementID.ToString()
                }
            };
            for (int i = 1; i <= 31; i++)
            {
                var shiftTimePropertyName = string.Format("ShiftTime{0}", i);
                var shiftTime = GetType().GetProperty(shiftTimePropertyName);
                var shiftTimeEntity = result.GetType().GetProperty(shiftTimePropertyName);
                var shiftTimeApprovalEntity = result.GetType().GetProperty(shiftTimePropertyName + "Approval");
                var value = shiftTime.GetValue(this) as ShiftTimeDetailModel;
                if (value != null)
                {
                    shiftTimeEntity.SetValue(result, new LookupItem
                    {
                        LookupId = string.IsNullOrEmpty(value.Value) ? 0 : int.Parse(value.Value),
                        LookupValue = value.Code
                    });
                    shiftTimeApprovalEntity.SetValue(result, value.Approved);
                }
                else
                {
                    shiftTimeEntity.SetValue(result, new LookupItem
                    {
                        LookupId = 0,
                        LookupValue = string.Empty
                    });
                    shiftTimeApprovalEntity.SetValue(result, false);
                }
                
            }
            return result;
        }

        public ShiftManagementDetailModel(ShiftManagementDetail fromEntity, Dictionary<int,string> managerList)
        {
            Employee = new EmployeeInfoModel
            {
                FullName = fromEntity.Employee.LookupValue,
                Id = fromEntity.Employee.LookupId,
                EmployeeId = fromEntity.EmployeeID != null ? fromEntity.EmployeeID.LookupValue : string.Empty
            };

            Employee.ManagerName = managerList[Convert.ToInt32(Employee.Id)];

            Id = fromEntity.ID;
            ShiftManagementID = fromEntity.ShiftManagementID.LookupId;

            for (int i = 1; i <= 31; i++)
            {
                var shiftTimePropertyName = string.Format("ShiftTime{0}", i);
                var shiftTime = GetType().GetProperty(shiftTimePropertyName);
                var shiftTimeFromEntity = fromEntity.GetType().GetProperty(string.Format("ShiftTime{0}", i.ToString())).GetValue(fromEntity) as LookupItem;
                var shiftTimeApprovedFromEntity = fromEntity.GetType().GetProperty(string.Format("ShiftTime{0}Approval", i.ToString())).GetValue(fromEntity);
                var shiftValue = new ShiftTimeDetailModel();
                if (shiftTimeFromEntity != null)
                {
                    shiftValue.Value = shiftTimeFromEntity.LookupId.ToString();
                    shiftValue.Code = shiftTimeFromEntity.LookupValue;
                    shiftValue.Approved = shiftTimeApprovedFromEntity != null ? (bool)shiftTimeApprovedFromEntity : false;
                }
                shiftTime.SetValue(this, shiftValue);
            }
        }

        public ShiftManagementDetailModel(EmployeeInfo employee)
        {
            Employee = new EmployeeInfoModel
            {
                FullName = employee.FullName,
                Id = employee.ID,
                EmployeeId = employee.EmployeeID,
                ManagerName = employee.Manager != null ? employee.Manager.LookupValue : string.Empty
            };

            Id = 0;
        }

        public List<ShiftManagementDetailModel> FromEntitieAndEmployees(List<ShiftManagementDetail> details, List<EmployeeInfo> employees, int month, int year, Dictionary<int,string> managerList)
        {
            
            var result = new List<ShiftManagementDetailModel>();
            if (details != null && details.Count > 0)
            {
                foreach (var detail in details)
                {
                    result.Add(new ShiftManagementDetailModel(detail, managerList));
                }
            }
            if (employees != null && employees.Count > 0)
            {
                foreach (var employee in employees)
                {
                    //result.Add(new ShiftManagementDetailModel(employee, month, year));
                    result.Add(new ShiftManagementDetailModel(employee));
                }
            }
            return result;
        }
        #endregion
    }
}
