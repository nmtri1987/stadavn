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
    public class FilterTask
    {
        public int ItemId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public LookupItem Requester { get; set; }
        public string ItemApprovalUrl { get; set; }
        public LookupItem Department { get; set; }
        public string Description { get; set; }
        public string ApprovalStatus { get; set; }
        public int ApprovalStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }

        private EmployeeInfoDAL _employeeInfoDAL;

        public FilterTask(ShiftManagement shiftManagement)
        {
            this.Description = string.Format("{0}/{1} - {2}", shiftManagement.Month, shiftManagement.Year, shiftManagement.Requester.LookupValue);
            this.Requester = shiftManagement.Requester;
            this.Department = shiftManagement.Department;
            this.CreatedDate = shiftManagement.Created;
            this.DueDate = DateTime.Now;
            this.ItemId = shiftManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(ShiftManagementList.ListUrl, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(ShiftManagementList.ListUrl);
        }
        public FilterTask(ChangeShiftManagement changeShiftManagement)
        {
            InitEmployeeDAL(SPContext.Current.Web);

            this.Description = string.Format("{0}/{1} - {2}/{3}", changeShiftManagement.FromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), changeShiftManagement.FromShift.LookupValue, changeShiftManagement.ToDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), changeShiftManagement.ToShift.LookupValue);
            this.Requester = changeShiftManagement.Requester;
            var requesterInfo = _employeeInfoDAL.GetByID(changeShiftManagement.Requester.LookupId);
            this.Department = requesterInfo.Department;
            this.CreatedDate = changeShiftManagement.Created;
            this.DueDate = changeShiftManagement.RequestDueDate == DateTime.MinValue ? changeShiftManagement.FromDate : changeShiftManagement.RequestDueDate; // TODO
            this.ItemId = changeShiftManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(ChangeShiftList.ListUrl, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(ChangeShiftList.ListUrl);
        }
        public FilterTask(Models.OverTimeManagement overtimeManagement)
        {
            this.Description = string.Format("{0} - {1}", overtimeManagement.CommonDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), overtimeManagement.Requester.LookupValue);
            this.Requester = overtimeManagement.Requester;
            this.Department = overtimeManagement.CommonDepartment;
            this.CreatedDate = overtimeManagement.Created;
            this.DueDate = overtimeManagement.RequestDueDate == DateTime.MinValue ? overtimeManagement.CommonDate : overtimeManagement.RequestDueDate; // TODO
            this.ItemId = overtimeManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(OverTimeManagementList.ListUrl, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(OverTimeManagementList.ListUrl);
        }
        public FilterTask(Models.NotOvertimeManagement notOvertimeManagement)
        {
            InitEmployeeDAL(SPContext.Current.Web);

            this.Description = string.Format("{0} - {1}", notOvertimeManagement.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), notOvertimeManagement.Requester.LookupValue);
            this.Requester = notOvertimeManagement.Requester;
            var requesterInfo = _employeeInfoDAL.GetByID(notOvertimeManagement.Requester.LookupId);
            this.Department = requesterInfo.Department;
            this.CreatedDate = notOvertimeManagement.Created;
            this.DueDate = notOvertimeManagement.RequestDueDate == DateTime.MinValue ? notOvertimeManagement.FromDate : notOvertimeManagement.RequestDueDate;// TODO
            this.ItemId = notOvertimeManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(NotOvertimeList.ListUrl, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(NotOvertimeList.ListUrl);
        }
        public FilterTask(Models.LeaveManagement leaveManagement)
        {
            string requestFor = leaveManagement.RequestFor.LookupValue;
            string fromDesc = ResourceHelper.GetLocalizedString("LeaveList_From", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            string toDesc = ResourceHelper.GetLocalizedString("LeaveList_To", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            string fromVal = leaveManagement.From.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            string toVal = leaveManagement.To.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            this.Description = string.Format("{0} - ({1}: {2} - {3}: {4})", requestFor, fromDesc, fromVal, toDesc, toVal);
            this.Requester = leaveManagement.Requester;
            this.Department = leaveManagement.Department;
            this.CreatedDate = leaveManagement.Created;
            this.DueDate = leaveManagement.RequestDueDate == DateTime.MinValue ? leaveManagement.From : leaveManagement.RequestDueDate;// TODO
            this.ItemId = leaveManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(LeaveManagementList.ListUrl, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(LeaveManagementList.ListUrl);
        }
        public FilterTask(Models.VehicleManagement vehicleManagement)
        {
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
            this.Description = string.Format("{0} - ({1}: {2} - {3}: {4})", vehicleType, fromDesc, fromVal, toDesc, toVal);
            this.Requester = vehicleManagement.Requester;
            this.Department = vehicleManagement.CommonDepartment;
            this.CreatedDate = vehicleManagement.Created;
            this.DueDate = vehicleManagement.RequestDueDate == DateTime.MinValue ? vehicleManagement.From : vehicleManagement.RequestDueDate; // TODO
            this.ItemId = vehicleManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(VehicleManagementList.ListUrl, vehicleManagement.ID)}&Source=/SitePages/Overview.aspx";
            this.InitModule(VehicleManagementList.ListUrl);
        }
        public FilterTask(Models.FreightManagement freightManagement)
        {
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

            this.Description = string.Format("{0} - {1}", bringerVal, freightManagement.Reason);
            this.Requester = freightManagement.Requester;
            this.Department = freightManagement.Department;
            this.CreatedDate = freightManagement.Created;
            this.DueDate = freightManagement.RequestDueDate == DateTime.MinValue ? freightManagement.TransportTime : freightManagement.RequestDueDate; // TODO
            this.ItemId = freightManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(FreightManagementList.ListUrl, freightManagement.ID)}&Source=/SitePages/Overview.aspx";
            this.InitModule(FreightManagementList.ListUrl);
        }
        public FilterTask(Models.BusinessTripManagement businessTripManagement)
        {
            string listItemDesc = string.Empty;
            if (businessTripManagement.Domestic == true)
            {
                listItemDesc = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeInternalTitle", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            }
            else
            {
                listItemDesc = ResourceHelper.GetLocalizedString("BusinessTripManagement_BusinessTripTypeExternalTitle", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
            }

            this.Description = string.Format("{0} - {1}", listItemDesc, businessTripManagement.BusinessTripPurpose);
            this.Requester = businessTripManagement.Requester;
            this.Department = businessTripManagement.CommonDepartment;
            this.CreatedDate = businessTripManagement.Created;
            this.DueDate = businessTripManagement.RequestDueDate; // TODO
            this.ItemId = businessTripManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl2(BusinessTripManagementList.Url, businessTripManagement.ID)}&Source=/SitePages/Overview.aspx";
            this.InitModule(BusinessTripManagementList.Url);
        }
        public FilterTask(Models.Request requestManagement)
        {
            this.Description = requestManagement.Title;
            this.Requester = requestManagement.CommonCreator;
            this.Department = requestManagement.Department;
            this.CreatedDate = requestManagement.Created;
            this.DueDate = requestManagement.RequestDueDate == DateTime.MinValue ? requestManagement.FinishDate : requestManagement.RequestDueDate;// TODO
            this.ItemId = requestManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl(RequestsList.Url, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(RequestsList.Url);
        }
        public FilterTask(Models.EmployeeRequirementSheet recruitmentManagement)
        {
            this.Description = recruitmentManagement.Position;
            this.Requester = recruitmentManagement.CommonCreator;
            this.Department = recruitmentManagement.CommonDepartment;
            this.CreatedDate = recruitmentManagement.Created;
            this.DueDate = recruitmentManagement.RequestDueDate == DateTime.MinValue ? recruitmentManagement.AvailableTime : recruitmentManagement.RequestDueDate;// Policy: 15 ngay // TODO
            this.ItemId = recruitmentManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl(EmployeeRequirementSheetsList.Url, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(EmployeeRequirementSheetsList.Url);
        }
        public FilterTask(Models.RequestForDiplomaSupply requestForDiplomaSupply)
        {
            this.Description = requestForDiplomaSupply.Position;
            this.Requester = requestForDiplomaSupply.CommonCreator;
            this.Department = requestForDiplomaSupply.CommonDepartment;
            this.CreatedDate = requestForDiplomaSupply.Created;
            this.DueDate = DateTime.MinValue; // TODO
            this.ItemId = requestForDiplomaSupply.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl(RequestForDiplomaSuppliesList.Url, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(RequestForDiplomaSuppliesList.Url);
        }

        public FilterTask(Models.RequisitionOfMeetingRoom requisitionOfMeetingRoom)
        {
            this.Description = requisitionOfMeetingRoom.Title;
            this.Requester = requisitionOfMeetingRoom.CommonCreator;
            this.Department = requisitionOfMeetingRoom.CommonDepartment;
            this.CreatedDate = requisitionOfMeetingRoom.Created;
            this.DueDate = DateTime.MinValue; // TODO
            this.ItemId = requisitionOfMeetingRoom.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl(RequisitionOfMeetingRoomList.Url, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(RequisitionOfMeetingRoomList.Url);
        }

        public FilterTask(Models.GuestReceptionManagement guestReceptionManagement)
        {
            this.Description = guestReceptionManagement.Title;
            this.Requester = guestReceptionManagement.CommonCreator;
            this.Department = guestReceptionManagement.CommonDepartment;
            this.CreatedDate = guestReceptionManagement.Created;
            this.DueDate = DateTime.MinValue; // TODO
            this.ItemId = guestReceptionManagement.ID;
            this.ItemApprovalUrl = $"{DelegationManager.BuildListItemApprovalUrl(GuestReceptionManagementList.Url, this.ItemId)}&Source=/SitePages/Overview.aspx";
            this.InitModule(GuestReceptionManagementList.Url);
        }

        private void InitModule(string listUrl)
        {
            DelegationModulesDAL delegationModulesDAL = new DelegationModulesDAL(SPContext.Current.Web.Url);

            var delegationModule = delegationModulesDAL.GetByListUrl(listUrl);
            if (delegationModule != null)
            {
                this.ModuleId = delegationModule.ID;
            }
        }

        private void InitEmployeeDAL(SPWeb currentWeb)
        {
            if (_employeeInfoDAL == null)
            {
                _employeeInfoDAL = new EmployeeInfoDAL(currentWeb.Url);
            }
        }
    }
}
