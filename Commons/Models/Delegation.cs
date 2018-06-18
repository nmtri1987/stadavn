using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.Models
{
    /// <summary>
    /// Delegation
    /// </summary>
    [ListUrl(StringConstant.DelegationsList.Url)]
    public class Delegation : EntityBase
    {
        #region Properties
        [ListColumn(StringConstant.DelegationsList.Fields.Title)]
        public string Title { get; set; }
        [ListColumn(StringConstant.DelegationsList.Fields.ModuleName)]
        public string ModuleName { get; set; }
        [ListColumn(StringConstant.DelegationsList.Fields.VietnameseModuleName)]
        public string VietnameseModuleName { get; set; }
        [ListColumn(StringConstant.DelegationsList.Fields.FromDate)]
        public DateTime FromDate { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.ToDate)]
        public DateTime ToDate { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.FromEmployee)]
        public LookupItem FromEmployee { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.ToEmployee)]
        public List<LookupItem> ToEmployee { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.Requester)]
        public LookupItem Requester { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.Department)]
        public LookupItem Department { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.ListUrl)]
        public string ListUrl { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.ListItemID)]
        public int ListItemID { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.ListItemDescription)]
        public string ListItemDescription { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.ListItemCreatedDate)]
        public DateTime ListItemCreatedDate { get; set; }

        [ListColumn(StringConstant.DelegationsList.Fields.ListItemApprovalUrl)]
        public string ListItemApprovalUrl { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.CreatedField)]
        public DateTime Created { get; set; }

        [ListColumn(StringConstant.DefaultSPListField.ModifiedField)]
        public DateTime Modified { get; set; }

        private SPWeb CurrentWeb { get; set; }
        #endregion

        #region Private attributes

        private EmployeeInfoDAL _employeeInfoDAL; // = new EmployeeInfoDAL(SPContext.Current.Web.Url);

        #endregion

        #region Constructors
        public Delegation()
        {
        }

        public Delegation(SPWeb currentWeb)
        {
            if (currentWeb == null)
            {
                this.CurrentWeb = SPContext.Current.Web;
            }
            else
            {
                this.CurrentWeb = currentWeb;
            }
        }

        /// <summary>
        /// Initialize from Request object.
        /// </summary>
        /// <param name="request">The Request object.</param>
        public Delegation(Models.Request request, SPWeb currentWeb = null) : this(currentWeb)
        {
            //this.ModuleName = "Requests Management";
            //this.VietnameseModuleName = "Quản Lý Phiếu Đề Nghị";
            this.ListItemDescription = request.Title;
            this.Requester = request.CommonCreator;
            this.Department = request.Department;
            this.ListItemCreatedDate = request.Created;
            this.ListUrl = RequestsList.Url;
            this.ListItemID = request.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl(this.CurrentWeb, this.ListUrl, this.ListItemID);
            this.InitModuleName(RequestsList.Url);
        }

        /// <summary>
        /// Initialize from EmployeeRequirementSheet object.
        /// </summary>
        /// <param name="employeeRequirementSheet">The EmployeeRequirementSheet object.</param>
        public Delegation(Models.EmployeeRequirementSheet employeeRequirementSheet, SPWeb currentWeb = null) : this(currentWeb)
        {
            //this.ModuleName = "Recruitment Management";
            //this.VietnameseModuleName = "Quản Lý Yêu Cầu Tuyển Dụng";
            this.ListItemDescription = employeeRequirementSheet.Position;
            this.Requester = employeeRequirementSheet.CommonCreator;
            this.Department = employeeRequirementSheet.CommonDepartment;
            this.ListItemCreatedDate = employeeRequirementSheet.Created;
            this.ListUrl = EmployeeRequirementSheetsList.Url;
            this.ListItemID = employeeRequirementSheet.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl(this.CurrentWeb, this.ListUrl, this.ListItemID);
            this.InitModuleName(EmployeeRequirementSheetsList.Url);
        }

        /// <summary>
        /// Initialize from RequestForDiplomaSupply object.
        /// </summary>
        /// <param name="requestForDiplomaSupply">The RequestForDiplomaSupply object.</param>
        public Delegation(Models.RequestForDiplomaSupply requestForDiplomaSupply, SPWeb currentWeb = null) : this(currentWeb)
        {
            //this.ModuleName = "Request For Diploma Supply Management";
            //this.VietnameseModuleName = "Quản Lý Phiếu Đề Nghị Bổ Sung Bằng Cấp";
            this.ListItemDescription = string.Format("{0} - {1}", requestForDiplomaSupply.EmployeeName, requestForDiplomaSupply.Position);
            this.Requester = requestForDiplomaSupply.CommonCreator;
            this.Department = requestForDiplomaSupply.CommonDepartment;
            this.ListItemCreatedDate = requestForDiplomaSupply.Created;
            this.ListUrl = RequestForDiplomaSuppliesList.Url;
            this.ListItemID = requestForDiplomaSupply.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl(this.CurrentWeb, this.ListUrl, this.ListItemID);
            this.InitModuleName(RequestForDiplomaSuppliesList.Url);
        }

        /// <summary>
        /// Initialize from Shift Management object.
        /// </summary>
        /// <param name="shiftManagement">The Shift Management object.</param>
        public Delegation(Models.ShiftManagement shiftManagement, SPWeb currentWeb = null) : this(currentWeb)
        {
            //this.ModuleName = "Shift Management";
            //this.VietnameseModuleName = "Quản Lý Đi Ca";
            if (currentWeb == null)
            {
                currentWeb = SPContext.Current.Web;
            }
            this.ListItemDescription = string.Format("{0}/{1} - {2}", shiftManagement.Month, shiftManagement.Year, shiftManagement.Requester.LookupValue);
            this.Requester = shiftManagement.Requester;
            this.Department = shiftManagement.Department;
            this.ListItemCreatedDate = shiftManagement.Created;
            this.ListUrl = ShiftManagementList.ListUrl;
            this.ListItemID = shiftManagement.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, this.ListItemID);
            this.InitModuleName(ShiftManagementList.ListUrl);
        }

        /// <summary>
        /// Initialize from Overtime Management object.
        /// </summary>
        /// <param name="shiftManagement">The Overtime Management object.</param>
        public Delegation(Models.OverTimeManagement overtimeManagement, SPWeb currentWeb = null) : this(currentWeb)
        {
            if (currentWeb == null)
            {
                currentWeb = SPContext.Current.Web;
            }
            this.ListItemDescription = string.Format("{0} - {1}", overtimeManagement.CommonDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), overtimeManagement.Requester.LookupValue);
            this.Requester = overtimeManagement.Requester;
            this.Department = overtimeManagement.CommonDepartment;
            this.ListItemCreatedDate = overtimeManagement.Created;
            this.ListUrl = OverTimeManagementList.ListUrl;
            this.ListItemID = overtimeManagement.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, this.ListItemID);
            this.InitModuleName(OverTimeManagementList.ListUrl);
        }

        /// <summary>
        /// Initialize from Change Shift Management object.
        /// </summary>
        /// <param name="shiftManagement">The Change Shift Management object.</param>
        public Delegation(Models.ChangeShiftManagement changeShiftManagement, EmployeeInfo fromEmployee, SPWeb currentWeb = null) : this(currentWeb)
        {
            if (currentWeb == null)
            {
                currentWeb = SPContext.Current.Web;
            }
            InitEmployeeDAL(currentWeb); 

            this.ListItemDescription = string.Format("{0}/{1} - {2}/{3}", changeShiftManagement.FromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), changeShiftManagement.FromShift.LookupValue, changeShiftManagement.ToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), changeShiftManagement.ToShift.LookupValue);
            this.Requester = changeShiftManagement.Requester;
            var requesterInfo = _employeeInfoDAL.GetByID(changeShiftManagement.Requester.LookupId);
            this.Department = requesterInfo.Department;
            this.ListItemCreatedDate = changeShiftManagement.Created;
            this.ListUrl = ChangeShiftList.ListUrl;
            this.ListItemID = changeShiftManagement.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, this.ListItemID);
            this.InitModuleName(ChangeShiftList.ListUrl);
        }

        /// <summary>
        /// Initialize from Not Overtime Management object.
        /// </summary>
        /// <param name="shiftManagement">The Not Overtime Management object.</param>
        public Delegation(Models.NotOvertimeManagement notOvertimeManagement, EmployeeInfo fromEmployee, SPWeb currentWeb = null) : this(currentWeb)
        {
            if (currentWeb == null)
            {
                currentWeb = SPContext.Current.Web;
            }
            InitEmployeeDAL(currentWeb);

            this.ListItemDescription = string.Format("{0} - {1}", notOvertimeManagement.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), notOvertimeManagement.Requester.LookupValue);
            this.Requester = notOvertimeManagement.Requester;
            var requesterInfo = _employeeInfoDAL.GetByID(notOvertimeManagement.Requester.LookupId);
            this.Department = requesterInfo.Department;
            this.ListItemCreatedDate = notOvertimeManagement.Created;
            this.ListUrl = NotOvertimeList.ListUrl;
            this.ListItemID = notOvertimeManagement.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, this.ListItemID);
            this.InitModuleName(NotOvertimeList.ListUrl);
        }

        public Delegation(Models.LeaveManagement leaveManagement, SPWeb currentWeb = null) : this(currentWeb)
        {
            //ModuleName = "Leave Management";
            //VietnameseModuleName = "Quản Lý Nghỉ Phép";
            ListItemID = leaveManagement.ID;
            ListUrl = StringConstant.LeaveManagementList.ListUrl;
            ListItemCreatedDate = leaveManagement.Created;
            Requester = leaveManagement.Requester;
            Department = leaveManagement.Department;

            string requestFor = leaveManagement.RequestFor.LookupValue;
            string fromDesc = ResourceHelper.GetLocalizedString("LeaveList_From", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            string toDesc = ResourceHelper.GetLocalizedString("LeaveList_To", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            string fromVal = leaveManagement.From.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            string toVal = leaveManagement.To.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            ListItemDescription = string.Format("{0} - ({1}: {2} - {3}: {4})", requestFor, fromDesc, fromVal, toDesc, toVal);
            ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, leaveManagement.ID);
            this.InitModuleName(LeaveManagementList.ListUrl);
        }

        public Delegation(Models.VehicleManagement vehicleManagement, SPWeb currentWeb = null) : this(currentWeb)
        {
            //ModuleName = "Vehicle Registration Management";
            //VietnameseModuleName = "Quản Lý Đăng Ký Đi Xe";
            ListItemID = vehicleManagement.ID;
            ListUrl = StringConstant.VehicleManagementList.ListUrl;
            ListItemCreatedDate = vehicleManagement.Created;
            Requester = vehicleManagement.Requester;
            Department = vehicleManagement.CommonDepartment;

            string vehicleType = vehicleManagement.Type;
            if (CultureInfo.CurrentUICulture.LCID == 1066)
            {
                if (vehicleType.ToLower().IndexOf("company") >= 0)
                {
                    vehicleType = ResourceHelper.GetLocalizedString("VehicleManagement_VehicleType_Choice_Company", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                }
                else
                {
                    vehicleType = ResourceHelper.GetLocalizedString("VehicleManagement_VehicleType_Choice_Private", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                }
            }

            string fromDesc = ResourceHelper.GetLocalizedString("VehicleManagement_CommonFrom", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            string toDesc = ResourceHelper.GetLocalizedString("VehicleManagement_CommonTo", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            string fromVal = vehicleManagement.From.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            string toVal = vehicleManagement.ToDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            ListItemDescription = string.Format("{0} - ({1}: {2} - {3}: {4})", vehicleType, fromDesc, fromVal, toDesc, toVal);
            ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, vehicleManagement.ID);
            this.InitModuleName(VehicleManagementList.ListUrl);
        }

        public Delegation(Models.FreightManagement freightManagement, SPWeb currentWeb = null) : this(currentWeb)
        {
            //ModuleName = "Freight Management";
            //VietnameseModuleName = "Quản Lý Vận Chuyển Hàng Hóa";
            ListItemID = freightManagement.ID;
            ListUrl = StringConstant.FreightManagementList.ListUrl;
            ListItemCreatedDate = freightManagement.Created;
            Requester = freightManagement.Requester;
            Department = freightManagement.Department;

            string bringerVal = string.Empty;
            if (freightManagement.CompanyVehicle == true)
            {
                bringerVal = ResourceHelper.GetLocalizedString("FreightManagement_CompanyVehicle", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            }
            else
            {
                LookupItem bringerLookupItem = freightManagement.Bringer;
                if (bringerLookupItem != null && bringerLookupItem.LookupId > 0)
                    bringerVal = bringerLookupItem.LookupValue;
                else
                    bringerVal = freightManagement.BringerName;
            }
            ListItemDescription = string.Format("{0} - {1}", bringerVal, freightManagement.Reason);
            ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, freightManagement.ID);
            this.InitModuleName(FreightManagementList.ListUrl);
        }

        public Delegation(Models.BusinessTripManagement businessTripManagement, SPWeb currentWeb = null) : this(currentWeb)
        {
            //ModuleName = "Business Trip Management";
            //VietnameseModuleName = "Quản Lý Đi Công Tác";
            ListItemID = businessTripManagement.ID;
            ListUrl = StringConstant.BusinessTripManagementList.Url;
            ListItemCreatedDate = businessTripManagement.Created;
            Requester = businessTripManagement.Requester;
            Department = businessTripManagement.CommonDepartment;

            string listItemDesc = string.Empty;
            if (businessTripManagement.Domestic == true)
            {
                listItemDesc = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            }
            else
            {
                listItemDesc = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            }
            ListItemDescription = string.Format("{0} - {1}", listItemDesc, businessTripManagement.BusinessTripPurpose);
            ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl2(this.ListUrl, businessTripManagement.ID);
            this.InitModuleName(BusinessTripManagementList.Url);
        }

        /// <summary>
        /// Initialize from RequisitionOfMeetingRoom object.
        /// </summary>
        /// <param name="request">The RequisitionOfMeetingRoom object.</param>
        public Delegation(Models.RequisitionOfMeetingRoom requisitionOfMeetingRoom, SPWeb currentWeb = null) : this(currentWeb)
        {
            //this.ModuleName = "Requisition Of Meeting Room Management";
            //this.VietnameseModuleName = "Quản Lý Yêu Cầu Phòng Họp";
            this.ListItemDescription = requisitionOfMeetingRoom.Title;
            this.Requester = requisitionOfMeetingRoom.CommonCreator;
            this.Department = requisitionOfMeetingRoom.CommonDepartment;
            this.ListItemCreatedDate = requisitionOfMeetingRoom.Created;
            this.ListUrl = RequisitionOfMeetingRoomList.Url;
            this.ListItemID = requisitionOfMeetingRoom.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl(this.CurrentWeb, this.ListUrl, this.ListItemID);
            this.InitModuleName(RequisitionOfMeetingRoomList.Url);
        }

        /// <summary>
        /// Initialize from GuestReceptionManagement object.
        /// </summary>
        /// <param name="request">The GuestReceptionManagement object.</param>
        public Delegation(Models.GuestReceptionManagement guestReceptionManagement, SPWeb currentWeb = null) : this(currentWeb)
        {
            //this.ModuleName = "Requirement For Guest Reception Management";
            //this.VietnameseModuleName = "Quản Lý Yêu Đón Tiếp Khách";
            this.ListItemDescription = guestReceptionManagement.Title;
            this.Requester = guestReceptionManagement.CommonCreator;
            this.Department = guestReceptionManagement.CommonDepartment;
            this.ListItemCreatedDate = guestReceptionManagement.Created;
            this.ListUrl = GuestReceptionManagementList.Url;
            this.ListItemID = guestReceptionManagement.ID;
            this.ListItemApprovalUrl = DelegationManager.BuildListItemApprovalUrl(this.CurrentWeb, this.ListUrl, this.ListItemID);
            this.InitModuleName(GuestReceptionManagementList.Url);
        }

        #endregion

        #region Methods

        private void InitModuleName(string listUrl)
        {
            DelegationModulesDAL delegationModulesDAL = new DelegationModulesDAL(this.CurrentWeb.Url);

            var delegationModule = delegationModulesDAL.GetByListUrl(listUrl);
            if (delegationModule != null)
            {
                this.ModuleName = delegationModule.ModuleName;
                this.VietnameseModuleName = delegationModule.VietnameseModuleName;
            }
        }

        private void InitEmployeeDAL(SPWeb currentWeb)
        {
            if (currentWeb == null)
            {
                currentWeb = SPContext.Current.Web;
            }
            if (_employeeInfoDAL == null)
            {
                _employeeInfoDAL = new EmployeeInfoDAL(currentWeb.Url);
            }
        }
        #endregion
        
    }
}
