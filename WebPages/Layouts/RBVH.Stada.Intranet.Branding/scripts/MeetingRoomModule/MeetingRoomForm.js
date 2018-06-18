function ValidateBeforeSaveAndSubmit() {
    var isValidForm = (MeetingRoomModule.ValidateForm());
    return isValidForm;
}

var MeetingRoomModule = (function () {
    'use strict';
    var that = {};

    that.Settings = {
        Id: 0,
        ServiceUrls:
        {
            //GetApprovalPermission: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/HasApprovalPermission/{1}',
            //GetDelegatedTaskInfo: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/GetDelegatedTaskInfo/{1}',
            //GetTaskHistoryInfo: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/GetTaskHistory/{1}/{2}',
        },
    };

    // Check dirty or not
    that.FormDataObject = {};
    that.OriginalFormDataObjectString = '';
    that.CurrentFormDataObjectString = '';
    that.Id = 0;

    function initialize(settings) {
        console.log('Initializing...');

        initData();
        registerEvents();
        initControls();
    }

    function initData() {
        var now = new Date();
        var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        that.Settings.Today = currentDate;

        // Show/hide Approval Status
        var approvalStatusValue = $(that.Settings.Controls.ApprovalStatusValueSelector).val();
        if (!!approvalStatusValue) {
            $(that.Settings.Controls.ApprovalStatusTdSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(approvalStatusValue));
            $(that.Settings.Controls.ApprovalStatusTrSelector).show();
        }

        var requestId = Functions.getParameterByName("ID");
        if (!!requestId) {
            that.Id = requestId;
            // Create form data
            createFormDataObject();
            that.OriginalFormDataObjectString = JSON.stringify(that.FormDataObject);
        }
    }

    function initControls() {
        // Hide date selector in case of editing
        if ($(that.Settings.Controls.StartTimeSelector).prop('disabled'))
            $(that.Settings.Controls.StartTimeImageSelector).hide();
        else
            $(that.Settings.Controls.StartTimeImageSelector).show();

        if ($(that.Settings.Controls.EndTimeSelector).prop('disabled'))
            $(that.Settings.Controls.EndTimeImageSelector).hide();
        else
            $(that.Settings.Controls.EndTimeImageSelector).show();
    }

    // Begin Validation
    function validate() {
        var isValid = validateRequiredFields() && validateDateFields();
        if (isValid) {
            if (that.OriginalFormDataObjectString != '') {
                createFormDataObject(); // Rebind current data
                that.CurrentFormDataObjectString = JSON.stringify(that.FormDataObject);

                if (that.CurrentFormDataObjectString == that.OriginalFormDataObjectString) // Dirty data
                {
                    alert(that.Settings.ResourceText.DataMustBeDifferentAfterEdit);

                    return false;
                }
            }
        }

        return isValid;
    }

    function validateRequiredFields() {
        var isValid = true;
        $(that.Settings.Controls.ContainerSelector + ' ' + that.Settings.Controls.RequiredFieldClassSelector).each(function () {
            $(this).html('');
            var targetId = $(this).attr('target-id');
            if (typeof targetId != 'undefined' && $("#" + targetId).is(':visible')) {
                var $target = $("#" + targetId);
                var validValue = true;
                if ($target.is('textarea')) // textarea
                {
                    if ($target.val() == '') {
                        validValue = false;
                    }
                }
                else if ($target.is('select')) // select
                {
                    if ($target.val() == '0') {
                        validValue = false;
                    }
                }
                else if ($target.is('input')) {
                    var type = $target.attr('type');

                    if (type == 'text') // textbox
                    {
                        if ($target.val() == '') {
                            validValue = false;
                        }
                    }
                }

                if (!validValue) {
                    $(this).html(that.Settings.ResourceText.CantLeaveTheBlank);
                    $(this).show();

                    isValid = false;
                }
            }
        });

        return isValid;
    }

    function validateDateFields() {
        var isValid = true;

        $(that.Settings.Controls.StartTimeErrorSelector).html('');
        $(that.Settings.Controls.EndTimeErrorSelector).html('');

        var startTime = $(that.Settings.Controls.StartTimeSelector).val();
        var startTimeObj = Functions.parseVietNameseDate(startTime);
        if (isNaN(startTimeObj.getTime())) {
            $(that.Settings.Controls.StartTimeErrorSelector).html(that.Settings.ResourceText.InvalidDate);
            $(that.Settings.Controls.StartTimeErrorSelector).show();
            isValid = false;
        }
        else if (startTimeObj < that.Settings.Today) {
            $(that.Settings.Controls.StartTimeErrorSelector).html(that.Settings.ResourceText.CantLessThanToday);
            $(that.Settings.Controls.StartTimeErrorSelector).show();
            isValid = false;
        }

        var endTime = $(that.Settings.Controls.EndTimeSelector).val();
        var endTimeObj = Functions.parseVietNameseDate(endTime);
        if (isNaN(endTimeObj.getTime())) {
            $(that.Settings.Controls.EndTimeErrorSelector).html(that.Settings.ResourceText.InvalidDate);
            $(that.Settings.Controls.EndTimeErrorSelector).show();
            isValid = false;
        }
        else if (endTimeObj < that.Settings.Today) {
            $(that.Settings.Controls.EndTimeErrorSelector).html(that.Settings.ResourceText.CantLessThanToday);
            $(that.Settings.Controls.EndTimeErrorSelector).show();
            isValid = false;
        }

        // From <= To
        if (isValid) {
            var startTimeValues = startTime.split('/');
            var endTimeValues = endTime.split('/');

            var startDate = new Date(startTimeValues[2], startTimeValues[1], startTimeValues[0], $(that.Settings.Controls.StartTimeHourSelector).val().replace(':', ''), $(that.Settings.Controls.StartTimeMinuteSelector).val());
            var endDate = new Date(endTimeValues[2], endTimeValues[1], endTimeValues[0], $(that.Settings.Controls.EndTimeHourSelector).val().replace(':', ''), $(that.Settings.Controls.EndTimeMinuteSelector).val());
            if (startDate >= endDate) {
                $(that.Settings.Controls.EndTimeErrorSelector).html(that.Settings.ResourceText.StartDateLessThanEndDate);
                $(that.Settings.Controls.EndTimeErrorSelector).show();
                isValid = false;
            }
        }

        return isValid;
    }

    // End Validation

    function registerEvents() {
        $(document).on("input", that.Settings.Controls.SeatsSelector, function () {
            this.value = this.value.replace(/\D/g, '');

            if (this.value.length == 1 && this.value == 0)
                this.value = '';
        });
    }

    // Begin Check dirty

    function createFormDataObject() {
        that.FormDataObject = that.FormDataObject || {};
        that.FormDataObject.DiscussionMeeting = $(that.Settings.Controls.DiscussionMeetingSelector).val();
        that.FormDataObject.Participation = $(that.Settings.Controls.ParticipationSelector).val();
        that.FormDataObject.Location = $(that.Settings.Controls.LocationSelector).val();
        var equipments = '';
        $(that.Settings.Controls.EquipmentSelector + ' input[type=checkbox]:checked').each(function () {
            equipments += this.value;
        });
        that.FormDataObject.Equipment = equipments;
        that.FormDataObject.Seats = $(that.Settings.Controls.SeatsSelector).val();
        that.FormDataObject.Others = $(that.Settings.Controls.OthersSelector).val();
        var startTime = $(that.Settings.Controls.StartTimeSelector).val();
        var startTimeValues = startTime.split('/');
        var startDate = new Date(startTimeValues[2], startTimeValues[1], startTimeValues[0], $(that.Settings.Controls.StartTimeHourSelector).val().replace(':', ''), $(that.Settings.Controls.StartTimeMinuteSelector).val());
        var endTime = $(that.Settings.Controls.EndTimeSelector).val();
        var endTimeValues = endTime.split('/');
        var endDate = new Date(endTimeValues[2], endTimeValues[1], endTimeValues[0], $(that.Settings.Controls.EndTimeHourSelector).val().replace(':', ''), $(that.Settings.Controls.EndTimeMinuteSelector).val());

        that.FormDataObject.StartDate = startDate;
        that.FormDataObject.EndDate = endDate;
    }

    // End Check dirty

    return {
        Initialize: function (settings) {
            $.extend(true, that.Settings, settings);
            initialize(settings);
        },
        ValidateForm: validate
    };
})();











