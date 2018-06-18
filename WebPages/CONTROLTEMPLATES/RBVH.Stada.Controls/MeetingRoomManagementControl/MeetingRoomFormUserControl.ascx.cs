using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.MeetingRoomManagementControl
{
    public partial class MeetingRoomFormUserControl : ApprovalBaseUserControl
    {
        private const string NoneSelectedValue = "0";

        #region Attirbutes
        private bool isEditable;
        private MeetingRoomDAL meetingRoomDAL;
        private EquipmentDAL equipmentDAL;
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    // Load data
                    this.LoadData();
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        #endregion

        #region Override

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                this.isEditable = this.IsEditable();
                this.meetingRoomDAL = new MeetingRoomDAL(this.SiteUrl);
                this.equipmentDAL = new EquipmentDAL(this.SiteUrl);
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        public override void Validate()
        {
            try
            {
                base.Validate();

                string requiredFieldErrorMessage = ResourceHelper.GetLocalizedString("RequiredField_WithParam", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                string invalidErrorMessage = ResourceHelper.GetLocalizedString("Invalid_WithParam", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                if (string.IsNullOrEmpty(txtDiscussionMeeting.Text))
                {
                    IsValid = false;
                    string fieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_DiscussionMeeting", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                    this.ErrorMessage = string.Format(requiredFieldErrorMessage, fieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }
                else if (string.IsNullOrEmpty(txtParticipation.Text))
                {
                    IsValid = false;
                    string fieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_Participation", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                    this.ErrorMessage = string.Format(requiredFieldErrorMessage, fieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }
                else if (string.Compare(ddlLocation.SelectedValue, NoneSelectedValue, true) == 0)
                {
                    IsValid = false;
                    string fieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_MeetingRoomLocation", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                    this.ErrorMessage = string.Format(requiredFieldErrorMessage, fieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }

                if (dtStartTime.IsDateEmpty)
                {
                    IsValid = false;
                    string fieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_Start_Time", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                    this.ErrorMessage = string.Format(requiredFieldErrorMessage, fieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }
                else if (!dtStartTime.IsValid)
                {
                    IsValid = false;
                    string fieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_Start_Time", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                    this.ErrorMessage = string.Format(invalidErrorMessage, fieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }

                if (dtEndTime.IsDateEmpty)
                {
                    IsValid = false;
                    string fieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_EndTime", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                    this.ErrorMessage = string.Format(requiredFieldErrorMessage, fieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }
                else if (!dtEndTime.IsValid)
                {
                    IsValid = false;
                    string fieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_EndTime", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                    this.ErrorMessage = string.Format(invalidErrorMessage, fieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }

                string lessThanMessage = ResourceHelper.GetLocalizedString("LessThan_WithParam", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                string startTimeFieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_Start_Time", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);
                string endTimeFieldName = ResourceHelper.GetLocalizedString("RequisitionOfMeetingRoom_EndTime", StringConstant.ResourcesFileLists, CultureInfo.CurrentUICulture.LCID);

                if (dtStartTime.SelectedDate > dtEndTime.SelectedDate)
                {
                    IsValid = false;
                    this.ErrorMessage = string.Format(lessThanMessage, startTimeFieldName, endTimeFieldName);
                    this.ShowClientMessage(this.ErrorMessage);
                    return;
                }
                else if (dtStartTime.SelectedDate == dtEndTime.SelectedDate)
                {
                    if (dtStartTime.SelectedDate.TimeOfDay.Hours > dtEndTime.SelectedDate.TimeOfDay.Hours)
                    {
                        IsValid = false;
                        this.ErrorMessage = string.Format(lessThanMessage, startTimeFieldName, endTimeFieldName);
                        this.ShowClientMessage(this.ErrorMessage);
                        return;
                    }
                    else if (dtStartTime.SelectedDate.TimeOfDay.Hours == dtEndTime.SelectedDate.TimeOfDay.Hours)
                    {
                        if (dtStartTime.SelectedDate.TimeOfDay.Minutes >= dtEndTime.SelectedDate.TimeOfDay.Minutes)
                        {
                            IsValid = false;
                            this.ErrorMessage = string.Format(lessThanMessage, startTimeFieldName, endTimeFieldName);
                            this.ShowClientMessage(this.ErrorMessage);
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                IsValid = false;
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        protected override bool SaveForm()
        {
            bool res = false;

            try
            {
                if (this.isEditable)
                {
                    // Set data to list item from form controls
                    this.SaveData();

                    res = base.SaveForm();
                }

                res = true;
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
                return false;
            }

            return res;
        }

        #endregion

        #region Methods

        private void SaveData()
        {
            string fullName = "";
            if (this.ApprovalBaseManagerObject.Creator != null)
            {
                fullName = this.ApprovalBaseManagerObject.Creator.FullName;
            }
            string startTime = "";
            if (!this.dtStartTime.IsDateEmpty && this.dtStartTime.IsValid)
            {
                startTime = this.dtStartTime.SelectedDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            }
            string endTime = "";
            if (!this.dtEndTime.IsDateEmpty && this.dtEndTime.IsValid)
            {
                endTime = this.dtEndTime.SelectedDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
            }
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Title] = $"{fullName}: {startTime} -> {endTime}";
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.DiscussionMeeting] = this.txtDiscussionMeeting.Text;
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Participation] = this.txtParticipation.Text;
            if (string.Compare(this.ddlLocation.SelectedValue, NoneSelectedValue, true) != 0)
            {
                this.CurrentItem[RequisitionOfMeetingRoomList.Fields.MeetingRoomLocation] = this.ddlLocation.SelectedValue;
            }
            SPFieldLookupValueCollection equipments = new SPFieldLookupValueCollection();
            foreach(ListItem item in this.cblEquipment.Items)
            {
                if (item.Selected)
                {
                    var equipment = new SPFieldLookupValue(int.Parse(item.Value), item.Text);
                    equipments.Add(equipment);
                }
            }
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Equipment] = equipments;
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Seats] = this.txtSeats.Text;
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Others] = this.txtOthers.Text;
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.StartDate] = this.dtStartTime.SelectedDate;
            this.CurrentItem[RequisitionOfMeetingRoomList.Fields.EndDate] = this.dtEndTime.SelectedDate;
        }

        private void LoadData()
        {
            // Load requester info
            LoadRqueterInfo();

            LoadDiscussionMeeting();

            LoadParticipation();

            #region Seats
            this.txtSeats.Text = ObjectHelper.GetString(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Seats]);
            this.txtSeats.Enabled = this.isEditable;
            #endregion

            #region Others
            this.txtOthers.Text = ObjectHelper.GetString(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Others]);
            this.txtOthers.Enabled = this.isEditable;
            #endregion

            #region Start time
            var startTime = ObjectHelper.GetDateTime(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.StartDate]);
            this.dtStartTime.SelectedDate = startTime;
            this.dtStartTime.Enabled = this.isEditable;
            #endregion

            #region End time
            var endTime = ObjectHelper.GetDateTime(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.EndDate]);
            this.dtEndTime.SelectedDate = endTime;
            this.dtEndTime.Enabled = this.isEditable;
            #endregion

            // Load list of rooms
            LoadListOfRooms();

            LoadListOfEquipments();

            // Load Approval Status
            hdApprovalStatus.Value = GetApprovalStatus();
        }

        /// <summary>
        /// Loading requester info
        /// </summary>
        private void LoadRqueterInfo()
        {
            if (ApprovalBaseManagerObject.Creator != null)
            {
                this.lblRequester.Text = this.ApprovalBaseManagerObject.Creator.FullName;
                var department = DepartmentListSingleton.GetDepartmentByID(ApprovalBaseManagerObject.Creator.Department.LookupId, this.SiteUrl);
                if (department != null)
                {
                    this.lblDepartment.Text = this.IsVietnameseLanguage ? department.VietnameseName : department.Name;
                }
            }
        }

        private void LoadDiscussionMeeting()
        {
            this.txtDiscussionMeeting.Text = ObjectHelper.GetString(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.DiscussionMeeting]);
            this.txtDiscussionMeeting.Enabled = this.isEditable;
        }

        private void LoadParticipation()
        {
            this.txtParticipation.Text = ObjectHelper.GetString(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Participation]);
            this.txtParticipation.Enabled = this.isEditable;
        }

        private void LoadListOfRooms()
        {
            var meetingRoms = this.meetingRoomDAL.GetAll();
            var selectedItemText = ResourceHelper.GetLocalizedString("Dropdown_Select", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
            meetingRoms.Insert(0, new Biz.Models.MeetingRoom { ID = int.Parse(NoneSelectedValue), Title = selectedItemText });
            this.ddlLocation.DataSource = meetingRoms;
            this.ddlLocation.DataBind();
            var locationLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.MeetingRoomLocation]);
            if (locationLookupValue != null)
            {
                var selectedRoom = this.ddlLocation.Items.Cast<ListItem>().Where(x => x.Value.Equals(locationLookupValue.LookupId.ToString())).FirstOrDefault();
                if (selectedRoom != null)
                {
                    selectedRoom.Selected = true;
                }
            }
            this.ddlLocation.Enabled = this.isEditable;
        }

        private void LoadListOfEquipments()
        {
            if (this.IsVietnameseLanguage)
            {
                this.cblEquipment.DataTextField = EquipmentList.Fields.CommonName1066;
            }
            else
            {
                this.cblEquipment.DataTextField = EquipmentList.Fields.CommonName;
            }
            var equipments = this.equipmentDAL.GetAll();
            this.cblEquipment.DataSource = equipments;
            this.cblEquipment.DataBind();

            var selectedItems = ObjectHelper.GetSPFieldLookupValueCollection(this.CurrentItem[RequisitionOfMeetingRoomList.Fields.Equipment]);
            if (selectedItems != null)
            {
                var listItems = this.cblEquipment.Items;
                foreach(var item in selectedItems)
                {
                    var selectedItem = listItems.Cast<ListItem>().Where(x => x.Value.Equals(item.LookupId.ToString())).FirstOrDefault();
                    if (selectedItem != null)
                    {
                        selectedItem.Selected = true;
                    }
                }
            }

            this.cblEquipment.Enabled = this.isEditable;
        }

        #endregion
    }
}
