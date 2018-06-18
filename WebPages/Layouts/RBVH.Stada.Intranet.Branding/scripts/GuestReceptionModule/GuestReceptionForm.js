function ValidateBeforeSaveAndSubmit() {
    var isValidForm = (GuestReceptionModule.ValidateForm());
    return isValidForm;
}

var GuestReceptionModule = (function () {
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
        Controls: {
            Grids: {
                "GuestInfo": {
                    GenderArray: [{ "Id": 1, Name: "Male" }, { "Id": 2, Name: "Female" }],
                    NationalityArray: [{ "Id": 1, Name: "Chinese" }, { "Id": 2, Name: "American" }, { "Id": 3, Name: "Vietnamese" }],
                    IsEditing: false,
                    DumpData: [
                        {
                            "GuestName": "Lê Quốc Tiến",
                            "GuestGender": 1,
                            "GuestNationality": 3,
                            "GuestJobTitle": "IT Staff",
                            "GuestPassportNo": "P 123456",
                            "GuestDateOfIssue": "01/06/2018",
                            "GuestDateOfArrival": "01/07/2018",
                            "GuestVisaValidity": "01/06/2028",
                        },
                        {
                            "GuestName": "Võ Thị Hồng Nhớ",
                            "GuestGender": 2,
                            "GuestNationality": 1,
                            "GuestJobTitle": "QC Staff",
                            "GuestPassportNo": "P 654321",
                            "GuestDateOfIssue": "01/05/2017",
                            "GuestDateOfArrival": "01/10/2018",
                            "GuestVisaValidity": "01/06/2028",
                        }
                    ],
                    Data: []
                },
                "GuestPickup": {
                    IsEditing: false,
                    DumpData: [
                        {
                            "PickupDatetime": "01/01/2018 07:30",
                            "PickupWorkingPlace": "Hoc Mon",
                            "PickupAttendant": "BOSCH staffs"
                        }
                    ],
                    Data: []
                }
            }
        },
    };

    // Check dirty or not
    that.FormDataObject = {};
    that.OriginalFormDataObjectString = '';
    that.CurrentFormDataObjectString = '';

    function initialize(settings) {
        console.log('Initializing...');

        initData();
        registerEvents();
        initControls();
    }

    function initDumpData() {
        $(that.Settings.Controls.GuestInfoDataSelector).val(JSON.stringify(that.Settings.Controls.Grids["GuestInfo"].DumpData));
        $(that.Settings.Controls.GuestPickupDataSelector).val(JSON.stringify(that.Settings.Controls.Grids["GuestPickup"].DumpData));

        var guestInfoJson = $(that.Settings.Controls.GuestInfoDataSelector).val();
        if (guestInfoJson != '') {
            that.Settings.Controls.Grids["GuestInfo"].Data = JSON.parse(guestInfoJson);
        }

        var guestPickupJson = $(that.Settings.Controls.GuestPickupDataSelector).val();
        if (guestPickupJson != '') {
            that.Settings.Controls.Grids["GuestPickup"].Data = JSON.parse(guestPickupJson);
        }
    }

    // 1. Initialize
    function initData() {
        var now = new Date();
        var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        that.Settings.Today = currentDate;

        initDumpData();

        // Show/hide Approval Status
        //var approvalStatusValue = $(that.Settings.Controls.ApprovalStatusValueSelector).val();
        //if (!!approvalStatusValue) {
        //    $(that.Settings.Controls.ApprovalStatusTdSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(approvalStatusValue));
        //    $(that.Settings.Controls.ApprovalStatusTrSelector).show();
        //}

        //var requestId = Functions.getParameterByName("ID");
        //if (!!requestId) {
        //    that.Id = requestId;
        //    // Create form data
        //    createFormDataObject();
        //    that.OriginalFormDataObjectString = JSON.stringify(that.FormDataObject);
        //}
    }

    function initControls() {
        // Date PICKER
        $(that.Settings.Controls.ContainerSelector + ' .date-picker').each(function (i) {
            if ($(this).prop('disabled'))
                $(this).closest('tr').find('img[id*=DatePickerImage]').hide();
            else
                $(this).closest('tr').find('img[id*=DatePickerImage]').show();
        });

        // Hotel Booking CONTAINER
        $(that.Settings.Controls.HotelBookingContainerSelector).toggle($(that.Settings.Controls.HotelBookingSelector).is(':checked'));

        // Pickup Car At Airport CONTAINER
        $(that.Settings.Controls.PickupCarAtAirportContainerSelector).toggle($(that.Settings.Controls.PickupCarAtAirportSelector).is(':checked'));

        // Souvenir CONTAINER
        $(that.Settings.Controls.SouvenirContainerSelector).toggle($(that.Settings.Controls.SouvenirSelector).is(':checked'));

        // Car To Airport CONTAINER
        $(that.Settings.Controls.CarToAirportContainerSelector).toggle($(that.Settings.Controls.CarToAirportSelector).is(':checked'));

        // Guest Working Schedule CONTAINER
        $(that.Settings.Controls.GuestWorkingScheduleContainerSelector).toggle($(that.Settings.Controls.GuestWorkingScheduleSelector).is(':checked'));

        // Other Lunch SERVICE
        $(that.Settings.Controls.OtherLunchServiceTextSelector).toggle($(that.Settings.Controls.OtherLunchServiceSelector).is(':checked'));

        // Other Equipment
        $(that.Settings.Controls.OtherEquipmentTextSelector).toggle($(that.Settings.Controls.OtherEquipmentSelector).is(':checked'));

        // Grid
        initGrid();
    }

    function initGrid() {
        initGridColumns();

        var processGuestInfoItem = function (args) {
            var hasVisaApplication = $(that.Settings.Controls.VisaApplicationSelector).is(':checked');

            var currentRow = that.Settings.Controls.Grids["GuestInfo"].IsEditing ? args.grid._editingRow.prev() : args.grid._insertRow;
            currentRow.find('td.jsgrid-cell input, td.jsgrid-cell select').removeClass('require-error');
            args.grid._insertRow.find('td.jsgrid-cell input, td.jsgrid-cell select').removeClass('require-error');

            if (!args.item.GuestName) {
                currentRow.find('td.jsgrid-cell:eq(0) input[type="text"]').addClass('require-error');
                args.cancel = true;
            }
            if (hasVisaApplication && !args.item.GuestGender) {
                currentRow.find('td.jsgrid-cell:eq(1) select').addClass('require-error');
                args.cancel = true;
            }
            if (!args.item.GuestNationality) {
                currentRow.find('td.jsgrid-cell:eq(2) select').addClass('require-error');
                args.cancel = true;
            }
            if (hasVisaApplication && !args.item.GuestJobTitle) {
                currentRow.find('td.jsgrid-cell:eq(3) input[type="text"]').addClass('require-error');
                args.cancel = true;
            }
            if (hasVisaApplication && !args.item.GuestPassportNo) {
                currentRow.find('td.jsgrid-cell:eq(4) input[type="text"]').addClass('require-error');
                args.cancel = true;
            }
            if (hasVisaApplication && !args.item.GuestDateOfIssue) {
                currentRow.find('td.jsgrid-cell:eq(5) input').addClass('require-error');
                args.cancel = true;
            }
            if (hasVisaApplication && !args.item.GuestDateOfArrival) {
                currentRow.find('td.jsgrid-cell:eq(6) input').addClass('require-error');
                args.cancel = true;
            }
            if (hasVisaApplication && !args.item.GuestVisaValidity) {
                currentRow.find('td.jsgrid-cell:eq(7) input').addClass('require-error');
                args.cancel = true;
            }
        }

        var processCompanyPickupItem = function (args) {
            var currentRow = that.Settings.Controls.Grids["GuestPickup"].IsEditing ? args.grid._editingRow.prev() : args.grid._insertRow;
            currentRow.find('td.jsgrid-cell input, td.jsgrid-cell select').removeClass('require-error');
            args.grid._insertRow.find('td.jsgrid-cell input, td.jsgrid-cell select').removeClass('require-error');

            if (!args.item.PickupDatetime) {
                currentRow.find('td.jsgrid-cell:eq(0) input').addClass('require-error');
                args.cancel = true;
            }
            if (!args.item.PickupWorkingPlace) {
                currentRow.find('td.jsgrid-cell:eq(1) input').addClass('require-error');
                args.cancel = true;
            }
        }

        var toggleGuestInfoError = function () {
            $(that.Settings.Controls.GuestInfoDataSelector).val(JSON.stringify(that.Settings.Controls.Grids["GuestInfo"].Data));
            showErrorMessage($(that.Settings.Controls.GuestInfoGridSelector), that.Settings.Controls.Grids["GuestInfo"].Data.length > 0 ? '' : that.Settings.ResourceText.CantLeaveTheBlank);
        }

        // init guest grid
        $(that.Settings.Controls.GuestInfoGridSelector).jsGrid({
            height: "auto",
            width: "100%",
            filtering: false, // TODO
            sorting: true, // TODO
            paging: true,
            autoload: true,
            inserting: true,
            editing: true,
            pageSize: 20,
            deleteConfirm: that.Settings.ResourceText.ConfirmDeleteMessage,
            noDataContent: '',

            controller: {
                loadData: function (filter) {
                    return that.Settings.Controls.Grids["GuestInfo"].Data;
                },
            },
            onItemInserting: function (args) {
                processGuestInfoItem(args);
            },
            onItemUpdating: function (args) { // Validate required fields
                processGuestInfoItem(args);
            },
            onItemEditing: function (args) { // Get current editing row
                that.Settings.Controls.Grids["GuestInfo"].IsEditing = true;
            },
            onItemUpdated: function (args) {
                that.Settings.Controls.Grids["GuestInfo"].IsEditing = false;

                toggleGuestInfoError();
            },
            onItemEditCancelling: function (args) {
                that.Settings.Controls.Grids["GuestInfo"].IsEditing = false;
            },

            onItemInserted: function (args) {
                toggleGuestInfoError();
            },
            onItemDeleted: function (args) {
                toggleGuestInfoError();
            },

            fields: [
                { name: "GuestName", type: "text", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestName, width: "200px", align: "left" }, // Text
                { name: "GuestGender", type: "select", items: that.Settings.Controls.Grids["GuestInfo"].GenderArray, valueField: "Id", textField: "Name", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestGender + '(*)', width: "75px", align: "left" }, // Text
                { name: "GuestNationality", type: "select", items: that.Settings.Controls.Grids["GuestInfo"].NationalityArray, valueField: "Id", textField: "Name", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestNationality, width: "100px", align: "left" }, // Text
                { name: "GuestJobTitle", type: "text", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestJobTitle + '(*)', width: "100px", align: "left" }, // Text
                { name: "GuestPassportNo", type: "text", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestPassportNo + '(*)', width: "100px", align: "left" }, // Text
                { name: "GuestDateOfIssue", type: "MyDateField", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestDateOfIssue + '(*)', width: "100px", align: "center" }, // Text
                { name: "GuestDateOfArrival", type: "MyDateField", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestDateOfArrival + '(*)', width: "100px", align: "center" }, // Text
                { name: "GuestVisaValidity", type: "MyDateField", title: that.Settings.Controls.Grids["GuestInfo"].Columns.GuestVisaValidity + '(*)', width: "100px", align: "center" }, // Text
                { type: "control", editButton: false, deleteButton: true, width: 60, modeSwitchButton: false }
            ],
        });

        // init guest pickup
        $(that.Settings.Controls.GuestPickupGridSelector).jsGrid({
            height: "auto",
            width: "100%",
            filtering: false, // TODO
            sorting: true, // TODO
            paging: true,
            autoload: true,
            inserting: true,
            editing: true,
            pageSize: 20,
            deleteConfirm: that.Settings.ResourceText.ConfirmDeleteMessage,
            noDataContent: '',

            controller: {
                loadData: function (filter) {
                    return that.Settings.Controls.Grids["GuestPickup"].Data;
                },
            },
            onItemInserting: function (args) {
                processCompanyPickupItem(args);
            },
            onItemUpdating: function (args) { // Validate required fields
                processCompanyPickupItem(args);
            },
            onItemEditing: function (args) { // Get current editing row
                that.Settings.Controls.Grids["GuestPickup"].IsEditing = true;
            },
            onItemEditCancelling: function (args) {
                processCompanyPickupItem(args);
                that.Settings.Controls.Grids["GuestPickup"].IsEditing = false;
            },

            onItemInserted: function (args) {
                $(that.Settings.Controls.GuestPickupDataSelector).val(JSON.stringify(that.Settings.Controls.Grids["GuestPickup"].Data));
            },
            onItemUpdated: function (args) {
                that.Settings.Controls.Grids["GuestPickup"].IsEditing = false;
                $(that.Settings.Controls.GuestPickupDataSelector).val(JSON.stringify(that.Settings.Controls.Grids["GuestPickup"].Data));
            },
            onItemDeleted: function (args) {
                $(that.Settings.Controls.GuestPickupDataSelector).val(JSON.stringify(that.Settings.Controls.Grids["GuestPickup"].Data));
            },

            fields: [
                { name: "PickupDatetime", type: "MyDatetimeField", title: that.Settings.Controls.Grids["GuestPickup"].Columns.PickupDatetime, width: "20%", align: "center" }, // Text
                { name: "PickupWorkingPlace", type: "text", title: that.Settings.Controls.Grids["GuestPickup"].Columns.PickupWorkingPlace, width: "40%", align: "left" }, // Text
                { name: "PickupAttendant", type: "text", title: that.Settings.Controls.Grids["GuestPickup"].Columns.PickupAttendant, width: "40%", align: "left" }, // Text
                { type: "control", editButton: false, deleteButton: true, width: 60, modeSwitchButton: false }
            ],
        });
    }

    function initGridColumns() {
        that.Settings.Controls.Grids.MyDatetimeField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.Controls.Grids.MyDatetimeField.prototype = new jsGrid.Field({
            sorter: function (date1, date2) {
                return new Date(date1) - new Date(date2);
            },

            itemTemplate: function (value) {
                return value;
            },

            insertTemplate: function (value) {
                return this._insertPicker = $("<input>").datetimepicker({
                    format: 'DD/MM/YYYY HH:mm', showClear: true
                });
            },

            editTemplate: function (value) {
                this._editPicker = $("<input>").datetimepicker({
                    format: 'DD/MM/YYYY HH:mm', showClear: true
                });

                if (typeof value != 'undefined')
                    this._editPicker.data("DateTimePicker").date(value);

                return this._editPicker;
            },

            insertValue: function () {
                return this._insertPicker.data('date');
            },

            editValue: function () {
                return this._editPicker.data('date');
            }
        });

        jsGrid.fields.MyDatetimeField = that.Settings.Controls.Grids.MyDatetimeField;

        that.Settings.Controls.Grids.MyDateField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.Controls.Grids.MyDateField.prototype = new jsGrid.Field({
            sorter: function (date1, date2) {
                return new Date(date1) - new Date(date2);
            },

            itemTemplate: function (value) {
                return value;
            },

            insertTemplate: function (value) {
                return this._insertPicker = $("<input>").datetimepicker({
                    format: 'DD/MM/YYYY', showClear: true
                });
            },

            editTemplate: function (value) {
                this._editPicker = $("<input>").datetimepicker({
                    format: 'DD/MM/YYYY', showClear: true
                });

                if (typeof value != 'undefined')
                    this._editPicker.data("DateTimePicker").date(value);

                return this._editPicker;
            },

            insertValue: function () {
                return this._insertPicker.data('date');
            },

            editValue: function () {
                return this._editPicker.data('date');
            }
        });

        jsGrid.fields.MyDateField = that.Settings.Controls.Grids.MyDateField;
    }
    // End Initialize

    // 2. Begin Validation
    function validate() {
        var isValid = validateRequiredFields() && validateDateFields();


        return true;
    }

    function validateRequiredFields() {
        var isValid = true;
        // $(#<<selector>>:enabled)
        $(that.Settings.Controls.ContainerSelector + ' ' + that.Settings.Controls.RequiredFieldClass + ':enabled:visible' + ', ' + that.Settings.Controls.ContainerSelector + ' div' + that.Settings.Controls.RequiredFieldClass + ':visible').each(function () {
            var hasValue = true;
            if ($(this).is('input')) {
                var type = $(this).attr('type');

                if (type == 'text') // textbox
                {
                    hasValue = $(this).val() == '' ? false : true;
                }
            }
            else if ($(this).is('div')) {
                if ($(this).hasClass('jsgrid')) {
                    hasValue = $('#' + $(this).attr('id') + ' .jsgrid-grid-body .jsgrid-table tr:not(.jsgrid-nodata-row)').length == 0 ? false : true;
                }
            }

            isValid = hasValue;

            showErrorMessage($(this), isValid ? '' : that.Settings.ResourceText.CantLeaveTheBlank);

        });

        return isValid;
    }

    function validateDate($date) {
        var value = $date.val();
        if (value == '') return true;

        var dateObj = Functions.parseVietNameseDate(value);
        var errorMessage = '';
        if (isNaN(dateObj.getTime())) {
            errorMessage = that.Settings.ResourceText.InvalidDate;
        }
        else if (dateObj < that.Settings.Today) {
            errorMessage = that.Settings.ResourceText.CantLessThanToday;
        }

        // Show error:
        showErrorMessage($date, errorMessage);

        return errorMessage == '';
    }

    function compareDates($startDate, $endDate, errorMessage) {
        var startDateObj = Functions.parseVietNameseDate($startDate.val());
        var endDateObj = Functions.parseVietNameseDate($endDate.val());

        if (startDateObj > endDateObj) {
            showErrorMessage($startDate, errorMessage);

            return false;
        }

        return true;
    }

    function compareDatetimes($startDate, $endDate, startHour, endHour, startMinute, endMinute, errorMessage) {
        var startDateValues = $startDate.val().split('/');
        var endDateValues = $endDate.val().split('/');

        var startDateTime = new Date(startDateValues[2], startDateValues[1], startDateValues[0], startHour, startMinute);
        var endDateTime = new Date(endDateValues[2], endDateValues[1], endDateValues[0], endHour, endMinute);
        if (startDateTime > endDateTime) {
            showErrorMessage($startDate, errorMessage);

            return false;
        }

        return true;
    }

    function validateDateFields() {
        var isValid = true;

        isValid = validateDate($(that.Settings.Controls.CheckInSelector)) && isValid;
        isValid = validateDate($(that.Settings.Controls.CheckOutSelector)) && isValid;
        isValid = validateDate($(that.Settings.Controls.PickupSelector)) && isValid;
        isValid = validateDate($(that.Settings.Controls.CarToTheAirportSelector)) && isValid;
        isValid = validateDate($(that.Settings.Controls.WorkingFromSelector)) && isValid;
        isValid = validateDate($(that.Settings.Controls.WorkingToSelector)) && isValid;

        isValid = compareDatetimes($(that.Settings.Controls.CheckInSelector),
                                   $(that.Settings.Controls.CheckOutSelector),
                                   $(that.Settings.Controls.CheckInHourSelector).val().replace(':', ''),
                                   $(that.Settings.Controls.CheckInMinuteSelector).val(),
                                   $(that.Settings.Controls.CheckOutHourSelector).val().replace(':', ''),
                                   $(that.Settings.Controls.CheckOutMinuteSelector).val(),
                                   String.format(that.Settings.ResourceText.NotGreaterThan_WithParam, that.Settings.Controls.CheckInTitle, that.Settings.Controls.CheckOutTitle)) && isValid;
        isValid = compareDates($(that.Settings.Controls.WorkingFromSelector),
                               $(that.Settings.Controls.WorkingToSelector),
                               String.format(that.Settings.ResourceText.NotGreaterThan_WithParam, that.Settings.Controls.WorkingFromTitle, that.Settings.Controls.WorkingToTitle)) && isValid;

        return isValid;
    }
    // End Validation

    // 3. Events
    function registerEvents() {
        // Number validation
        $(document).on("input", that.Settings.Controls.SeatSelector + ", " + that.Settings.Controls.WorkingClothSelector, function () {
            this.value = this.value.replace(/\D/g, '');

            if (this.value.length == 1 && this.value == 0)
                this.value = '';
        });

        // Show/hide Hotel Booking CONTAINER
        $(that.Settings.Controls.HotelBookingSelector).change(function (e) {
            $(that.Settings.Controls.HotelBookingContainerSelector).toggle($(this).is(':checked'));
        });

        // Show/hide Pick-up car at airport CONTAINER
        $(that.Settings.Controls.PickupCarAtAirportSelector).change(function (e) {
            $(that.Settings.Controls.PickupCarAtAirportContainerSelector).toggle($(this).is(':checked'));
        });

        // Show/hide Souvenir CONTAINER
        $(that.Settings.Controls.SouvenirSelector).change(function (e) {
            $(that.Settings.Controls.SouvenirContainerSelector).toggle($(this).is(':checked'));
        });

        // Show/hide Car To Airport CONTAINER
        $(that.Settings.Controls.CarToAirportSelector).change(function (e) {
            $(that.Settings.Controls.CarToAirportContainerSelector).toggle($(this).is(':checked'));
        });

        // Show/hide Guest Working Schedule CONTAINER
        $(that.Settings.Controls.GuestWorkingScheduleSelector).change(function (e) {
            $(that.Settings.Controls.GuestWorkingScheduleContainerSelector).toggle($(this).is(':checked'));
        });

        // Show/hide Other Lunch SERVICE
        $(that.Settings.Controls.OtherLunchServiceSelector).change(function (e) {
            $(that.Settings.Controls.OtherLunchServiceTextSelector).toggle($(this).is(':checked'));
        });

        // Show/hide Other EQUIPMENT
        $(that.Settings.Controls.OtherEquipmentSelector).change(function (e) {
            $(that.Settings.Controls.OtherEquipmentTextSelector).toggle($(this).is(':checked'));
        });

        // Checkbox list select ONLY ONE
        $('input:checkbox[id*=' + that.Settings.Controls.ReceptionId + ']' + ', ' +
            'input:checkbox[id*=' + that.Settings.Controls.SouvenirId + ']').on('click', function () {
                $(this).closest('tbody').find('input:checkbox[id!=' + $(this).attr('id') + ']').prop('checked', false);
            });

        // Lunch SERVICE
        $('input:checkbox[id*=' + that.Settings.Controls.LunchServiceId + ']').on('click', function () {
            $(this).closest('tbody').find('input:checkbox[id!=' + $(this).attr('id') + ']').prop('checked', false);

            $(that.Settings.Controls.OtherLunchServiceSelector).trigger('change'); // Trigger 'change' event to show/hide textbox
        });

        // Disable enter to submit form
        $(window).keydown(function (event) {
            if ((event.keyCode == 13) && ($(event.target)[0] != $("textarea")[0])) {
                event.preventDefault();
                return false;
            }
        });

        // GRID
        // Required fields
        $(document).on("blur", 'tr.jsgrid-insert-row', function () {
            $(this).find('td.jsgrid-cell input, td.jsgrid-cell select').removeClass('require-error');
        });
        // Cancel when losing focus
        $(document).on("blur", 'tr.jsgrid-edit-row', function () {
            $(this).find('.jsgrid-cancel-edit-button').click();
        });
        // Enter/ESC
        $(that.Settings.Controls.GuestInfoGridSelector + ', ' + that.Settings.Controls.GuestPickupGridSelector).on("keyup", ".jsgrid-edit-row", function (e) {
            if (e.which == 13) // Enter
                $(this).find('.jsgrid-update-button').click();
            else if (e.which == 27)
                $(this).find('.jsgrid-cancel-edit-button').click();
        });
        // First focus when editing row
        $(document).on('click', '.jsgrid-row , .jsgrid-alt-row', function () {
            var editingRow = $(this).prev();
            editingRow.find("td input:enabled:not([readonly]):first").focus();
        });
    }
    // End Events

    /* Check dirty

    function createFormDataObject() {

    }

    */

    // Utilities
    function showErrorMessage($element, errorMessage) {
        var $spanError = $element.closest('div').find('span.ms-formvalidation');

        if (errorMessage != '') {
            if ($spanError.length == 0) {
                var $spanError = $(that.Settings.Controls.RequiredErrorElement);
                $spanError.html(errorMessage);
                $element.closest('div').append($spanError);
            }
            else {
                $spanError.html(errorMessage);
            }
        }
        else {
            $spanError.remove();
        }
    }

    return {
        Initialize: function (settings) {
            $.extend(true, that.Settings, settings);
            initialize(settings);
        },
        ValidateForm: validate
    };
})();











