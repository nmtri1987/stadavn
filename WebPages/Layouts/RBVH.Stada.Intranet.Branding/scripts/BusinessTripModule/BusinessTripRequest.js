RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");

RBVH.Stada.WebPages.pages.BusinessTripRequest = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        Id: 0,
        ServiceUrls:
        {
            GetEmployeeInDepartment: '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeListInCurrentDepartment/{1}/{2}',
            InsertBusinessTripRequest: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/InsertBusinessTripManagement',
            GetBusinessTripRequestById: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/GetBusinessTripManagementById/{1}',
            GetApprovers: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/GetApprovers/{1}/{2}',
            ApproveBusinessTripRequest: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/ApproveBusinessTrip',
            DriverCommentBusinessTrip: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/DriverUpdateBusinessTrip',
            CashierCommentBusinessTrip: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/CashierUpdateBusinessTrip',
            RejectBusinessTripRequest: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/RejectBusinessTrip',
            GetApprovalPermission: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/HasApprovalPermission/{1}',
            GetDelegatedTaskInfo: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/GetDelegatedTaskInfo/{1}',
            GetTaskHistoryInfo: '//{0}/_vti_bin/Services/BusinessTripManagement/BusinessTripManagementService.svc/GetTaskHistory/{1}/{2}',
            GetConfigurations: '//{0}/_vti_bin/Services/Configurations/ConfigurationsService.svc/GetConfigurations'
        },
        Data: {},
        Configurations: {},
        ConfigKey_ValidDepartureDate: "BusinessForm_ValidDepartureDate",
        DelegatedTaskInfo: {},
        EmployeeGrid:
        {
        },
        ScheduleGrid:
        {
            CustomFields: {
                DepartTimeField: null,
                DepartDateField: null,
            }
        },
        CurrentEmployeeLookupId: 0,

        Approvers: {
            DH: {},
            DirectBOD: {},
            BOD: {},
            AdminDept: {}
        },
        Requester: {
            LookupId: 0,
            LookupValue: ''
        },
        Driver: {
            LookupId: 0,
            LookupValue: ''
        },
        Cashier: {
            LookupId: 0,
            LookupValue: ''
        },

        IsDriver: false,
        IsCashier: false,

        CurrentEmployeeDepartmentID: 0,
        CurrentEmployeeLocationID: 0,

        EmployeeJsonArray: [], // Array using for Employee list
        ScheduleJsonArray: [], // Array using for Schedule list
        ViewMode: 'Edit' // ['Edit','View','Approve']
    },

    $.extend(true, this.Settings, settings);

    this.Initialize();
};

RBVH.Stada.WebPages.pages.BusinessTripRequest.prototype =
{
    Initialize: function () {
        var that = this;
        ExecuteOrDelayUntilScriptLoaded(function () {
            that.Settings.CurrentLcid = SP.Res.lcid;
            that.SetCurrentEmployeeInfo();
            that.InitRequiredFields();
            that.InitControls();
        }, "sp.js", "strings.js");
    },
    SetCurrentEmployeeInfo: function () {
        var that = this;
        if (_rbvhContext && _rbvhContext.EmployeeInfo) {
            that.Settings.CurrentEmployeeID = _rbvhContext.EmployeeInfo.EmployeeID;
            that.Settings.CurrentEmployeeFullName = _rbvhContext.EmployeeInfo.FullName;
            that.Settings.CurrentEmployeeLocationID = _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
            that.Settings.CurrentEmployeeDepartmentID = (_rbvhContext && _rbvhContext.EmployeeInfo && _rbvhContext.EmployeeInfo.Department) ? _rbvhContext.EmployeeInfo.Department.LookupId : 0;
            that.Settings.CurrentEmployeeLookupId = _rbvhContext.EmployeeInfo.ID; //default is current requester
            that.Settings.Requester =
            {
                LookupId: that.Settings.CurrentEmployeeLookupId
            };
            that.Settings.CurrentEmployeeLevel = parseFloat(_rbvhContext.EmployeeInfo.EmployeeLevel.LookupValue);
        }
    },
    GetConfigurations: function () {
        var that = this;
        var postData = [that.Settings.ConfigKey_ValidDepartureDate];
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetConfigurations, location.host);
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response && response.length > 0) {
                that.Settings.Configurations = response;
            }
        });
    },
    InitControls: function () {
        var that = this;
        that.RegisterEvents();
        that.Settings.ViewMode = 'View';
        var id = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
        if (id > 0) {
            that.Settings.Id = id;
            that.BindDataToControls(id);
                that.GetApprovalPermission(id)
                    .then(function (result) {
                        if (result && result === true) {
                            if (that.Settings.Data && that.Settings.Data.RequestExpired == true) {
                                errMsg = decodeURI(that.Settings.ResourceText.RequestExpiredMsgFormat);
                                errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, that.Settings.Data.RequestDueDate);
                                RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.Controls.ErrorMsgContainerSelector, that.Settings.Controls.ErrorMsgSelector, errMsg);
                            }
                            else {
                                that.Settings.ViewMode = 'Approve';
                            }
                        }
                        else {
                            that.GetDelegatedTaskInfo(id)
                                .then(function (respData) {
                                    if (respData && respData.Requester.LookupId > 0) {
                                        if (that.Settings.Data && that.Settings.Data.RequestExpired == true) {
                                            errMsg = decodeURI(that.Settings.ResourceText.RequestExpiredMsgFormat);
                                            errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, that.Settings.Data.RequestDueDate);
                                            RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.Controls.ErrorMsgContainerSelector, that.Settings.Controls.ErrorMsgSelector, errMsg);
                                        }
                                        else {
                                            that.Settings.DelegatedTaskInfo = respData;
                                            that.Settings.ViewMode = 'Approve';
                                        }
                                    }
                                });
                        }
                    });
        }
        else {
            // Load Approvers
            that.LoadApprovers(that.Settings.CurrentEmployeeDepartmentID, that.Settings.CurrentEmployeeLocationID);

            that.Settings.ViewMode = 'Create';
            // Populate combobox
            that.BindEmployeeList(that.Settings.CurrentEmployeeDepartmentID);

            // Internal/External trip
            $(that.Settings.Controls.InternalTripTypeControlSelector).prop('checked', true);

            // Hotel Booking
            $(that.Settings.Controls.HotelBookingControlSelector).prop('checked', true);
            that.PopulateHotelBooking(true);

            // Cash
            $(that.Settings.Controls.CashOrChequeControlSelector).hide(100);

            // Other Transportation
            $(that.Settings.Controls.OtherTransportDetailControlSelector).hide(100);

            // Other request
            $(that.Settings.Controls.OtherRequestControlSelector).hide(100);
        }

        that.ToggleButtons(that.Settings.ViewMode);

        that.GetConfigurations();
        that.BindGridColumns();
        that.PopulateGrid();
    },
    RegisterEvents: function () {
        var that = this;
        // Employee LIST
        $(that.Settings.Controls.EmployeeListControlSelector).select2({ width: '150px' });
        $(that.Settings.Controls.AddEmployeeControlSelector).on('click', function () {
            var selectedEmployee = $(that.Settings.Controls.EmployeeListControlSelector + " option:selected");
            if (selectedEmployee.length > 0) {
                var employeeCode = selectedEmployee.attr("emp-code");
                var departmentName = selectedEmployee.attr("dept-name");
                $(that.Settings.Controls.SpanDepartmentSelector).text(departmentName);
                that.Settings.EmployeeJsonArray.push({
                    EmployeeId: $(that.Settings.Controls.EmployeeListControlSelector).val(),
                    FullName: selectedEmployee.text(),
                    EmployeeCode: employeeCode,
                    DepartmentName: departmentName
                });
                $(that.Settings.Controls.EmployeeListGridControlSelector).jsGrid("loadData");
                // Remove select item
                $(that.Settings.Controls.EmployeeListControlSelector).find('option:selected').remove();
            }
        });
        // Hotel Booking
        $(that.Settings.Controls.HotelBookingControlSelector).change(function () {
            that.ToggleCheckbox($(this), [$(that.Settings.Controls.PaidByContainerSelector), $(that.Settings.Controls.OtherServiceContainerSelector)]);
        });
        // Cash/Cheque
        $(that.Settings.Controls.CashChequeRequestOptionControlSelector).change(function () {
            that.ToggleCheckbox($(this), [$(that.Settings.Controls.CashOrChequeControlSelector)]);
        });
        // Other request
        $(that.Settings.Controls.OtherRequestOptionControlSelector).change(function () {
            that.ToggleCheckbox($(this), [$(that.Settings.Controls.OtherRequestControlSelector)]);
        });
        // Transportation
        $("input[name=TransportationType]:radio").change(function () {
            if ($(that.Settings.Controls.OtherTransportOptionControlSelector).prop('checked') === true)
                $(that.Settings.Controls.OtherTransportDetailControlSelector).show(100);
            else
                $(that.Settings.Controls.OtherTransportDetailControlSelector).hide(100);
        });
        // Save button
        $(that.Settings.Controls.SaveControlSelector).on('click', function () {
            var formValid = that.ValidateRequiredFields('Submit');
            if (formValid) {
                that.OnSaveData('Submit');
            }
            return false;
        });
        // Cancel button
        $(that.Settings.Controls.CancelControlSelector).on('click', function (event) {
            event.preventDefault();
            Functions.redirectToSource();
        });
        // Reject button
        $(that.Settings.Controls.RejectControlSelector).on('click', function () {
            var formValid = that.ValidateRequiredFields('Reject');
            if (formValid) {
                that.OnSaveData('Reject');
            }
            return false;
        });
        // Approve button
        $(that.Settings.Controls.ApproveControlSelector).on('click', function () {
            var formValid = that.ValidateRequiredFields('Approve');
            if (formValid) {
                that.OnSaveData('Approve');
            }
            return false;
        });
        $(that.Settings.Controls.ApprovalHistoryButtonSelector).on('click', function (event) {
            event.preventDefault();
            var itemId = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetTaskHistoryInfo, location.host, itemId, 0);
            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function (response) {
                var tableHeaders = [that.Settings.ResourceText.ApprovalStatusTitle, that.Settings.ResourceText.PostedByTitle, that.Settings.ResourceText.DateTitle, that.Settings.ResourceText.CommentTitle];
                var approvalStatus = [that.Settings.ResourceText.ApprovalStatus_Approved, that.Settings.ResourceText.ApprovalStatus_Rejected]
                var approvalHistoryTable = Functions.generateApprovalHistoryTable(response, tableHeaders, approvalStatus, that.Settings.ResourceText.NoDataAvaibleMsg);
                $(that.Settings.Controls.ApprovalHistoryContainerSelector).html(approvalHistoryTable);
            });
        });
    },
    ToggleOffEvents: function () {
        var that = this;
        $(that.Settings.Controls.BusinessTripContainerSelector + ' :input:not("' + that.Settings.Controls.CancelControlSelector + '")').prop("disabled", true);
        $(that.Settings.Controls.ApprovalHistoryButtonSelector).prop('disabled', false);
        $(that.Settings.Controls.BusinessTripContainerSelector + " span").off('click');
        $(that.Settings.Controls.AddEmployeeControlSelector).css('color', 'gray');
    },

    // -- 1.BEGIN: Bind data to controls
    ToggleCheckbox: function ($self, controls) {
        if ($self.prop('checked') === true) {
            controls.forEach(function ($element) {
                $element.show(100);
            });
        }
        else {
            controls.forEach(function ($element) {
                $element.hide(100);
            });
        }
    },
    PopulateHotelBooking: function (checked) {
        if (checked) {
            $(this.Settings.Controls.PaidByContainerSelector).show(100);
            $(this.Settings.Controls.OtherServiceContainerSelector).show(100);
        }
        else {
            $(this.Settings.Controls.PaidByContainerSelector).hide(100);
            $(this.Settings.Controls.OtherServiceContainerSelector).hide(100);
        }
    },
    BindGridColumns: function () {
        var that = this;
        // Bind custome fields
        that.BindDepartDateField();
        that.BindDepartTimeField();
        var readonlyMode = that.Settings.ViewMode === 'Approve' || that.Settings.ViewMode === 'View';
        that.Settings.EmployeeGrid.Fields = [
            { name: "EmployeeId", title: "EmployeeId", readOnly: true, headercss: "hide" },
            { name: "FullName", title: that.Settings.EmployeeGrid.ColumnTitles.GridColumn_FullName, width: 180, align: "left", type: "text" },
            { name: "EmployeeCode", title: that.Settings.EmployeeGrid.ColumnTitles.GridColumn_Code, width: 110, align: "center", type: "text" },
            { name: "DepartmentName", title: that.Settings.EmployeeGrid.ColumnTitles.GridColumn_Department, width: 200, align: "center", type: "text" },
            { type: "control", editButton: false, deleteButton: !readonlyMode, width: 40, modeSwitchButton: false }];

        jsGrid.fields.custDepartTimeField = that.Settings.ScheduleGrid.CustomFields.DepartTimeField;
        jsGrid.fields.custDepartDateField = that.Settings.ScheduleGrid.CustomFields.DepartDateField;
        that.Settings.ScheduleGrid.Fields = [
            //{ name: "ID", title: "ID", readOnly: true, headercss: "hide" },
            { name: "DepartTime", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_DepartTime, width: 130, align: "center", type: "custDepartTimeField" },
            { name: "DepartDate", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_DepartDate, width: 90, align: "center", type: "custDepartDateField" },
            { name: "FlightName", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_FlightName, width: 90, align: "left", type: "text" },
            { name: "City", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_City, width: 90, align: "left", type: "text" },
            { name: "Country", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_Country, width: 90, align: "left", type: "text" },
            { name: "ContactCompany", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_ContactCompany, width: 130, align: "left", type: "text" },
            { name: "ContactPhone", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_ContactPhone, width: 100, align: "left", type: "text" },
            { name: "OtherSchedule", title: that.Settings.ScheduleGrid.ColumnTitles.GridColumn_OtherSchedule, width: 90, align: "left", type: "text" },
            { type: "control", editButton: false, deleteButton: !readonlyMode, width: 40, modeSwitchButton: false }];
    },
    BindDepartDateField: function () {
        var that = this;
        var minDateObj = new Date();
        try {
            var configValue = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidDepartureDate);
            if (configValue) {
                var diffDays = parseInt(configValue);
                minDateObj.setDate(minDateObj.getDate() + diffDays);
            }
        }
        catch (err) { minDateObj = new Date(); }

        var defaultDateObj = minDateObj;
        that.Settings.ScheduleGrid.CustomFields.DepartDateField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.ScheduleGrid.CustomFields.DepartDateField.prototype = new jsGrid.Field({
            sorter: function (date1, date2) {
                return new Date(date1) - new Date(date2);
            },

            itemTemplate: function (value) {
                //return new Date(value).toDateString();
                return value;
            },

            insertTemplate: function (value) {
                return this._insertPicker = $("<input readonly>").datepicker({ defaultDate: defaultDateObj, minDate: minDateObj, dateFormat: "dd/mm/yy" });
            },

            editTemplate: function (value) {
                var dateObj = Functions.parseVietNameseDate(value);
                return this._editPicker = $("<input readonly>").datepicker({ dateFormat: "dd/mm/yy", minDate: minDateObj }).datepicker("setDate", dateObj);
            },

            insertValue: function () {
                var dateObj = this._insertPicker.datepicker("getDate");
                if (dateObj)
                    return Functions.parseVietnameseDateTimeToDDMMYYYY2(dateObj);
                else
                    return "";
            },

            editValue: function () {
                var dateObj = this._editPicker.datepicker("getDate");
                //return this._editPicker.datepicker("getDate").toISOString();
                if (dateObj)
                    return Functions.parseVietnameseDateTimeToDDMMYYYY2(dateObj);
                else
                    return "";
            }
        });
    },
    BindDepartTimeField: function () {
        var that = this;
        that.Settings.ScheduleGrid.CustomFields.DepartTimeField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.ScheduleGrid.CustomFields.DepartTimeField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                var $item = value;
                var times = $item.split(':');

                return ('0' + times[0]).slice(-2) + ':' + ('0' + times[1]).slice(-2);
            },
            insertTemplate: function (value) {
                return this._insertDepartTime = this._createDepartTimeBox();
            },

            editTemplate: function (value) {
                var workingTimes = value.split(':');
                $result = this._editDepartTime = this._createDepartTimeBox();
                $result[0].value = ('0' + workingTimes[0]).slice(-2);
                $result[2].value = ('0' + workingTimes[1]).slice(-2);

                return $result;
            },
            insertValue: function () {
                var fromHour = $(this._insertDepartTime[0]).val();
                var fromMinute = $(this._insertDepartTime[2]).val();
                return ('0' + fromHour).slice(-2) + ':' + ('0' + fromMinute).slice(-2);
            },

            editValue: function () {
                var fromHour = $(this._editDepartTime[0]).val();
                var fromMinute = $(this._editDepartTime[2]).val();
                return ('0' + fromHour).slice(-2) + ':' + ('0' + fromMinute).slice(-2);
            },

            _createDepartTimeBox: function () {
                var $selectDepartHour = $("<select id='cbo-depart-hour' class='form-control' style='width: 54px;' />");
                for (var i = 0; i < 24; i++) {
                    var numberValue = ('0' + i).slice(-2);
                    $selectDepartHour.append($("<option>").attr('value', numberValue).text(numberValue));
                }

                var $selectDepartMinute = $("<select id='cbo-depart-minute' class='form-control' style='width: 54px;' />");
                for (var i = 0; i < 60; i++) {
                    var numberValue = ('0' + i).slice(-2);
                    $selectDepartMinute.append($("<option>").attr('value', numberValue).text(numberValue));
                }

                return $selectDepartHour.add("<span>:<span>").add($selectDepartMinute);
            },
        });
    },
    PopulateGrid: function (control) {
        var that = this;
        $(that.Settings.Controls.EmployeeListGridControlSelector).jsGrid({
            width: "100%",
            height: "auto",
            align: "center",
            inserting: false,
            editing: false,
            sorting: false,
            autoload: true,
            noDataContent: '',
            deleteConfirm: that.Settings.ResourceText.ConfirmDeleteMessage,
            onDataLoaded: function (args) {
            },
            controller: {
                loadData: function (filter) {
                    var d = $.Deferred();

                    d.resolve(that.Settings.EmployeeJsonArray);

                    return d.promise();
                },
                insertItem: function (item) {
                },
                updateItem: function (item) {
                },
                deleteItem: function (item) {
                }
            },
            onItemDeleted: function (args) {
                $(that.Settings.Controls.EmployeeListControlSelector)
                    .prepend($("<option>")
                        .attr('value', args.item.EmployeeId)
                        .attr("emp-code", args.item.EmployeeCode)
                        .attr("dept-name", args.item.DepartmentName)
                        .text(args.item.FullName));

                $(that.Settings.Controls.EmployeeListControlSelector).prop("selectedIndex", 0);

                //$(that.Settings.Controls.EmployeeListControlSelector).select2({
                //    width: '150px',
                //    sorter: function (data) {
                //        return data.sort();
                //    }
                //})
            },

            fields: that.Settings.EmployeeGrid.Fields,
        });
        var readonlyMode = that.Settings.ViewMode === 'Approve' || that.Settings.ViewMode === 'View';
        $(that.Settings.Controls.ScheduleGridControlSelector).jsGrid({
            width: "100%",
            height: "auto",
            align: "center",
            inserting: !readonlyMode,
            editing: !readonlyMode,
            deleting: !readonlyMode,
            sorting: false,
            autoload: true,
            noDataContent: '',
            deleteConfirm: that.Settings.ResourceText.ConfirmDeleteMessage,
            onDataLoaded: function (args) {
            },
            controller: {
                loadData: function (filter) {
                    var d = $.Deferred();
                    d.resolve(that.Settings.ScheduleJsonArray);

                    return d.promise();
                },
                insertItem: function (item) {
                },
                updateItem: function (item) {
                },
                deleteItem: function (item) {
                }
            },
            onItemInserting: function (args) {
                var $selectedRow = args.grid._insertRow;
                args.cancel = !that.ValidateEmployeeItem(args, $selectedRow);
            },
            onItemInserted: function (args) {
            },
            onItemEditing: function (args) {
            },
            onItemUpdated: function (args) {
            },
            onItemUpdating: function (args) {
                var $selectedRow = args.grid._editingRow.prev();
                args.cancel = !that.ValidateEmployeeItem(args, $selectedRow);
            },
            onItemDeleted: function (args) {
            },
            onItemEditCancelling: function (args) {
            },
            fields: that.Settings.ScheduleGrid.Fields,
        });
    },
    BindEmployeeList: function (departmentId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetEmployeeInDepartment, location.host, departmentId, _rbvhContext.EmployeeInfo.FactoryLocation.LookupId);
        $.ajax({
            type: "GET",
            url: url,
            cache: false,
            //async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (data) {
            if (data) {
                $(that.Settings.Controls.EmployeeListControlSelector).empty();
                var existingEmployeeIdList = that.Settings.EmployeeJsonArray.map(function (item) { return item.EmployeeId });
                var employeeList = data.filter(function (element) {
                    return existingEmployeeIdList.indexOf(element.ID) < 0;
                });

                for (var i = 0; i < employeeList.length; i++) {
                    $(that.Settings.Controls.EmployeeListControlSelector)
                        .append($("<option>")
                            .attr('value', employeeList[i].ID)
                            .attr("emp-code", employeeList[i].EmployeeId)
                            .attr("dept-name", employeeList[i].DepartmentName)
                            .text(employeeList[i].FullName));
                }
            }
        });
    },
    BindDataToControls: function (itemId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetBusinessTripRequestById, location.host, itemId);
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            cache: false,
            async: false,
            dataType: "json",
        }).done(function (data) {
            that.Settings.Data = data;
            // Check VIEW MODE:
            if (that.Settings.CurrentEmployeeLookupId === data.Requester.LookupId) {
                if (data.ModifiedBy.ID === data.CreatedBy.ID && data.ApprovalStatus && data.ApprovalStatus.toLowerCase() !== 'cancelled') {
                    that.Settings.ViewMode = 'Edit';
                }
                else
                    that.Settings.ViewMode = 'View';
            }

            // Requester:
            that.Settings.Requester = data.Requester;

            that.Settings.Approvers.DH = data.DH;
            that.Settings.Approvers.DirectBOD = data.DirectBOD;
            that.Settings.Approvers.BOD = data.BOD;
            that.Settings.Approvers.AdminDept = data.AdminDept;

            //that.Settings.RequesterLookupId = data.Requester.LookupId;
            // Driver:
            that.Settings.Driver = data.Driver;
            // Cashier:
            that.Settings.Cashier = data.Cashier;

            // Business trip type 
            $(that.Settings.Controls.InternalTripTypeControlSelector).prop('checked', data.Domestic);
            $(that.Settings.Controls.ExternalTripTypeControlSelector).prop('checked', !data.Domestic);
            // Employee Grid
            that.Settings.EmployeeJsonArray = data.EmployeeList || [];
            // Rebind Grid
            $(that.Settings.Controls.EmployeeListGridControlSelector).jsGrid("loadData");

            if (that.Settings.ViewMode === 'View' || that.Settings.ViewMode === 'Approve') {
                $(that.Settings.Controls.EmployeeListControlSelector).parent("div").hide();
            }

            that.BindEmployeeList(that.Settings.CurrentEmployeeDepartmentID);
            // Purpose
            $(that.Settings.Controls.PurposeControlSelector).val(data.BusinessTripPurpose);
            // Schedule Grid
            that.Settings.ScheduleJsonArray = data.ScheduleList || [];
            // Rebind Grid
            $(that.Settings.Controls.ScheduleGridControlSelector).jsGrid("loadData");
            // Hotel Booking
            $(that.Settings.Controls.HotelBookingControlSelector).prop('checked', data.HotelBooking);
            if (data.HotelBooking) {
                $(that.Settings.Controls.PaidByContainerSelector).show();
                $(that.Settings.Controls.PaidByControlSelector).val(data.PaidBy);
                $(that.Settings.Controls.OtherServiceContainerSelector).show();
                $(that.Settings.Controls.OtherServiceControlSelector).val(data.OtherService);
            }
            else {
                $(that.Settings.Controls.PaidByContainerSelector).hide();
                $(that.Settings.Controls.OtherServiceContainerSelector).hide();
            }
            // Transportation Type
            $('input[name=TransportationType]:checked').prop('checked', false);
            $(that.Settings.Controls.OtherTransportDetailControlSelector).hide();
            switch (data.TransportationType) {
                case that.Settings.ResourceText.CompanyTransportationTitle:
                    $(that.Settings.Controls.CompanyTransportOptionControlSelector).prop('checked', true);
                    break;
                case that.Settings.ResourceText.PrivateTransportationTitle:
                    $(that.Settings.Controls.PrivateTransportOptionControlSelector).prop('checked', true);
                    break;
                case that.Settings.ResourceText.OtherTransportationTitle:
                    $(that.Settings.Controls.OtherTransportDetailControlSelector).show();
                    $(that.Settings.Controls.OtherTransportOptionControlSelector).prop('checked', true);
                    $(that.Settings.Controls.OtherTransportDetailControlSelector).val(data.OtherTransportationDetail);
                    break;
            }
            // Visa
            $(that.Settings.Controls.VisaRequestOptionControlSelector).prop('checked', data.HasVisa);
            // Cash or Cheque
            if (data.CashRequestDetails) {
                $(that.Settings.Controls.CashChequeRequestOptionControlSelector).prop('checked', true);
                $(that.Settings.Controls.CashOrChequeControlSelector).show();
                $(that.Settings.Controls.CashOrChequeControlSelector).val(data.CashRequestDetails);
            }
            else
                $(that.Settings.Controls.CashOrChequeControlSelector).hide();
            // Other Request
            if (data.OtherRequestDetail) {
                $(that.Settings.Controls.OtherRequestOptionControlSelector).prop('checked', true);
                $(that.Settings.Controls.OtherRequestControlSelector).show();
                $(that.Settings.Controls.OtherRequestControlSelector).val(data.OtherRequestDetail);
            }
            else
                $(that.Settings.Controls.OtherRequestControlSelector).hide();

            if (data.TripHighPriority && data.TripHighPriority === true) {
                $(that.Settings.Controls.HighPriorityControlSelector).prop("checked", true);
            }
            else {
                $(that.Settings.Controls.HighPriorityControlSelector).prop("checked", false);
            }

            // Old comments:
            if (data.Comment && data.Comment.length > 0) {
                $(that.Settings.Controls.CommentControlSelector).html(Functions.parseComment(data.Comment));
                $(that.Settings.Controls.CommentContainerSelector).show();
            }

            if (data.ApprovalStatus && data.ApprovalStatus.length > 0) {
                $(that.Settings.Controls.ApproveStatusValSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(data.ApprovalStatus));
                $(that.Settings.Controls.ApproveStatusContainerSelector).show();
            }
        });
    },
    LoadApprovers: function (departmentId, locationId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetApprovers, location.host, departmentId, locationId);
        $.ajax({
            type: "GET",
            url: url,
            cache: false,
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (data) {
            if (data) {
                that.Settings.Approvers.DH = data.Approver1;
                that.Settings.Approvers.DirectBOD = data.Approver2;
                that.Settings.Approvers.BOD = data.Approver3;
                that.Settings.Approvers.AdminDept = data.Approver4;
            }
        });
    },
    // -- 1.END: Bind data

    // -- 2.BEGIN: Required fields
    InitRequiredFields: function () {
        var that = this;
        // ES6 Arrow function syntax
        $(that.Settings.Controls.BusinessTripContainerSelector + ' ' + 'span.l-required:visible').each(function () {
            $(this).html($(this).html() + ' *');
        });
    },
    ValidateRequiredFields: function (action) {
        var that = this;
        var valid = true;
        // ES6 Arrow function syntax
        $(that.Settings.Controls.BusinessTripContainerSelector + ' ' + '.t-required:visible').each(function () {
            if ($.trim($(this).val()) === '') {
                $(this).addClass('require-error');
                valid = false;
            }
            else
                $(this).removeClass('require-error');
        });
        // Employee grid
        if (that.Settings.EmployeeJsonArray.length === 0) {
            $(that.Settings.Controls.EmployeeListGridControlSelector).addClass('require-error');
            valid = false;
        }
        else
            $(that.Settings.Controls.EmployeeListGridControlSelector).removeClass('require-error');
        // Schedule grid
        if (that.Settings.ScheduleJsonArray.length === 0) {
            $(that.Settings.Controls.ScheduleGridControlSelector).addClass('require-error');
            valid = false;
        }
        else
            $(that.Settings.Controls.ScheduleGridControlSelector).removeClass('require-error');

        if (action === 'Reject') {
            if ($(that.Settings.Controls.YourCommentControlSelector).val().length === 0) {
                $(that.Settings.Controls.YourCommentControlSelector).addClass('require-error');
                valid = false;
            }
            else {
                $(that.Settings.Controls.YourCommentControlSelector).removeClass('require-error');
            }
        }

        return valid;
    },
    ValidateEmployeeItem: function (args, $selectedRow) {
        var valid = true;
        if ($.trim(args.item.ContactCompany) === '') {
            $selectedRow.find('td:nth-child(6) input').addClass('require-error');
            valid = false;
        }
        else
            $selectedRow.find('td:nth-child(6) input').removeClass('require-error');

        if ($.trim(args.item.DepartDate) === '') {
            $selectedRow.find('td:nth-child(2) input').addClass('require-error');
            valid = false;
        }
        else
            $selectedRow.find('td:nth-child(2) input').removeClass('require-error');

        return valid;
    },
    // -- 2.END: Required fields

    // -- 3.BEGIN: Process data
    CreatePostJsonObject: function (action) {
        var that = this;
        var model = {};
        var url = '';
        var result = { Model: model, Url: url };
        if (that.Settings.IsDriver)
            action = 'Driver';
        else if (that.Settings.IsCashier)
            action = 'Cashier';
        switch (action) {
            case 'Submit':
                model.Id = that.Settings.Id;
                model.Requester = that.Settings.Requester; //{ LookupId: that.Settings.RequesterLookupId };
                model.Domestic = $(that.Settings.Controls.InternalTripTypeControlSelector).prop('checked');
                model.BusinessTripPurpose = $(that.Settings.Controls.PurposeControlSelector).val();
                model.HotelBooking = $(that.Settings.Controls.HotelBookingControlSelector).prop('checked');
                if (model.HotelBooking) {
                    model.PaidBy = $(that.Settings.Controls.PaidByControlSelector).val();
                    model.OtherService = $(that.Settings.Controls.OtherServiceControlSelector).val();
                }
                else {
                    model.PaidBy = model.OtherService = '';
                }
                //model.TransportationType = $.trim($('input[name="TransportationType"]:radio:checked').closest('label').text());
                var transportationType = $('input[name=TransportationType]:checked', that.Settings.Controls.BusinessTripContainerSelector).val();
                switch (transportationType) {
                    case 'company':
                        model.TransportationType = that.Settings.ResourceText.CompanyTransportationTitle;
                        break;
                    case 'private':
                        model.TransportationType = that.Settings.ResourceText.PrivateTransportationTitle;
                        break;
                    case 'others':
                        model.TransportationType = that.Settings.ResourceText.OtherTransportationTitle;
                        model.OtherTransportationDetail = $(that.Settings.Controls.OtherTransportDetailControlSelector).val();
                        break;
                }
                model.HasVisa = $(that.Settings.Controls.VisaRequestOptionControlSelector).prop('checked');
                model.CashRequestDetails = $(that.Settings.Controls.CashChequeRequestOptionControlSelector).prop('checked')
                                            ? $(that.Settings.Controls.CashOrChequeControlSelector).val()
                                            : '';
                model.OtherRequestDetail = $(that.Settings.Controls.OtherRequestOptionControlSelector).prop('checked')
                                            ? $(that.Settings.Controls.OtherRequestControlSelector).val()
                                            : '';

                model.Driver = that.Settings.Driver;
                model.Cashier = that.Settings.Cashier;
                model.TripHighPriority = $(that.Settings.Controls.HighPriorityControlSelector).prop('checked');
                model.Comment = $(that.Settings.Controls.YourCommentControlSelector).is(":visible")
                                            ? $(that.Settings.Controls.YourCommentControlSelector).val()
                                            : '';
                model.DH = { UserName: that.Settings.Approvers.DH !== null ? that.Settings.Approvers.DH.LoginName : '' };
                model.DirectBOD = { UserName: that.Settings.Approvers.DirectBOD !== null ? that.Settings.Approvers.DirectBOD.LoginName : '' };
                model.BOD = { UserName: that.Settings.Approvers.BOD !== null ? that.Settings.Approvers.BOD.LoginName : '' };
                model.AdminDept = { UserName: that.Settings.Approvers.AdminDept !== null ? that.Settings.Approvers.AdminDept.LoginName : '' };
                model.EmployeeList = that.Settings.EmployeeJsonArray;

                model.ScheduleList = that.Settings.ScheduleJsonArray.map(function (obj) {
                    var result = obj;
                    var departDate = moment(obj.DepartDate + ' ' + obj.DepartTime, "DD/MM/YYYY hh:mm", true);
                    result.DepartDate = departDate.toDate().toISOString();

                    return result;
                });

                result.Model = model;
                result.Url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.InsertBusinessTripRequest, location.host);
                break;
            case 'Approve':
                model = {
                    Id: this.Settings.Id,
                    TripHighPriority: $(that.Settings.Controls.HighPriorityControlSelector).prop('checked'),
                    CashRequestDetails: $(that.Settings.Controls.CashOrChequeControlSelector).val(),
                    Comment: $(this.Settings.Controls.YourCommentControlSelector).is(":visible") ? $(this.Settings.Controls.YourCommentControlSelector).val() : ''
                };
                result.Model = model;
                result.Url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ApproveBusinessTripRequest, location.host);
                break;
            case 'Driver':
                model = {
                    Id: this.Settings.Id,
                    Driver: { LookupId: that.Settings.Driver.LookupId },
                    Comment: $(this.Settings.Controls.YourCommentControlSelector).is(":visible") ? $(this.Settings.Controls.YourCommentControlSelector).val() : ''
                };
                result.Model = model;
                result.Url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.DriverCommentBusinessTrip, location.host);
                break;
            case 'Cashier':
                model = {
                    Id: this.Settings.Id,
                    Cashier: { LookupId: that.Settings.Cashier.LookupId },
                    Comment: $(this.Settings.Controls.YourCommentControlSelector).is(":visible") ? $(this.Settings.Controls.YourCommentControlSelector).val() : ''
                };
                result.Model = model;
                result.Url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.CashierCommentBusinessTrip, location.host);
                break;
            case 'Reject':
                model = {
                    Id: this.Settings.Id,
                    Comment: $(this.Settings.Controls.YourCommentControlSelector).is(":visible") ? $(this.Settings.Controls.YourCommentControlSelector).val() : ''
                };
                result.Model = model;
                result.Url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.RejectBusinessTripRequest, location.host);
                break;
        }

        return result;
    },
    ToggleButtons: function (viewMode) {
        var that = this;
        var funcButtons = $(that.Settings.Controls.BusinessTripContainerSelector + ' button:not("' + that.Settings.Controls.CancelControlSelector + '")');
        funcButtons.prop('disabled', true);
        $(that.Settings.Controls.ApprovalHistoryButtonSelector).prop('disabled', false);

        switch (viewMode) {
            case 'Approve': // Approve, Reject, Cancel
                funcButtons.hide();
                // Off events:
                that.ToggleOffEvents();
                // Show High Priority
                that.EnableApprovalElements();
                // Disable control
                $(that.Settings.Controls.SaveControlSelector).hide();
                $(that.Settings.Controls.ApproveControlSelector).show();
                $(that.Settings.Controls.RejectControlSelector).show();
                $(that.Settings.Controls.ApproveControlSelector).prop('disabled', false);
                $(that.Settings.Controls.RejectControlSelector).prop('disabled', false);
                // Show Comment:
                $(that.Settings.Controls.YourCommentContainerSelector).show();
                $(that.Settings.Controls.YourCommentControlSelector).prop('disabled', false);

                $(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").show();
                $(that.Settings.Controls.ApprovalHistoryButtonSelector).show();
                break;
            case 'Edit':
            case 'Create':
                funcButtons.hide();
                $(that.Settings.Controls.SaveControlSelector).show();
                $(that.Settings.Controls.SaveControlSelector).prop('disabled', false);
                $(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").hide();
                break;
            case 'OnSaving':
                break;
            case 'View':
                // Off events:
                that.ToggleOffEvents();
                funcButtons.hide();
                $(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").show();
                $(that.Settings.Controls.ApprovalHistoryButtonSelector).show();
                that.EnableCommentElements();
                break;
        }
    },
    OnSaveData: function (action) {
        var that = this;
        that.ToggleButtons('OnSaving');
        var data = that.CreatePostJsonObject(action);
        if (data.Model) {
            that.OnSubmitData(data.Model, data.Url)
                .then(function (response) {
                    if (response && response.Code === 0) {
                        Functions.redirectToSource();
                    }
                    else if (response && response.Code !== 0 && response.Code !== 999) {
                        alert(response.Message);
                        window.location.reload();
                    }
                    else {
                        that.ToggleButtons(that.Settings.ViewMode);
                    }
                });
        }
    },
    OnSubmitData: function (model, url) {
        var that = this;

        return $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    },
    // -- 3.END: Process data

    EnableApprovalElements: function () {
        var that = this;
        if (that.Settings.Data && that.Settings.Data.Domestic === true &&
            ((that.Settings.CurrentEmployeeLevel === 5 && _rbvhContext.EmployeeInfo.ADAccount.ID === that.Settings.Approvers.DH.ID) || // department head
            (that.Settings.DelegatedTaskInfo.FromEmployee && that.Settings.DelegatedTaskInfo.FromEmployee.ID === that.Settings.Approvers.DH.ID))) { // delegated person
            $(that.Settings.Controls.HighPriorityContainerSelector).show();
            $(that.Settings.Controls.HighPriorityControlSelector).prop("disabled", false);
        }
        else {
            $(that.Settings.Controls.HighPriorityContainerSelector).hide();
        }

        if ((_rbvhContext.EmployeeInfo.ADAccount.ID === that.Settings.Approvers.DH.ID ||
                _rbvhContext.EmployeeInfo.ADAccount.ID === that.Settings.Approvers.BOD.ID ||
                _rbvhContext.EmployeeInfo.ADAccount.ID === that.Settings.Approvers.DirectBOD.ID) ||
            (!!that.Settings.DelegatedTaskInfo.FromEmployee &&
                (that.Settings.DelegatedTaskInfo.FromEmployee.ID === that.Settings.Approvers.DH.ID ||
                that.Settings.DelegatedTaskInfo.FromEmployee.ID === that.Settings.Approvers.BOD.ID ||
                that.Settings.DelegatedTaskInfo.FromEmployee.ID === that.Settings.Approvers.DirectBOD.ID))) {
            if ($(that.Settings.Controls.CashChequeRequestOptionControlSelector).prop('checked')) {
                $(that.Settings.Controls.CashOrChequeControlSelector).prop("disabled", false);
            }
        }
    },
    EnableCommentElements: function () {
        var that = this;
        if (_rbvhContext.EmployeeInfo.ID === that.Settings.Driver.LookupId) { // Driver
            $(that.Settings.Controls.SaveControlSelector).show();
            $(that.Settings.Controls.SaveControlSelector).prop('disabled', false);
            $(that.Settings.Controls.YourCommentContainerSelector).show();
            $(that.Settings.Controls.YourCommentControlSelector).prop('disabled', false);
            that.Settings.IsDriver = true;
        }
        else if (_rbvhContext.EmployeeInfo.ID === that.Settings.Cashier.LookupId) { // Cashier
            $(that.Settings.Controls.SaveControlSelector).show();
            $(that.Settings.Controls.SaveControlSelector).prop('disabled', false);
            $(that.Settings.Controls.YourCommentContainerSelector).show();
            $(that.Settings.Controls.YourCommentControlSelector).prop('disabled', false);
            that.Settings.IsCashier = true;
        }
        else {
            $(that.Settings.Controls.YourCommentContainerSelector).hide();
            $(that.Settings.Controls.YourCommentControlSelector).prop('disabled', true);
        }
    },
    GetApprovalPermission: function (itemId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetApprovalPermission, location.host, itemId);
        return $.ajax({
            type: "GET",
            url: url,
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    },
    GetDelegatedTaskInfo: function (itemId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetDelegatedTaskInfo, location.host, itemId);
        return $.ajax({
            type: "GET",
            url: url,
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    },

    FormatNumber: function (num, size) {
        var s = "000000000" + num;
        return s.substr(s.length - size);
    },

    /* Bind property <-> HTML controls */
    ParseProperty: function (container, propertyName, controlType) {
        switch (controlType) {
            case 'radio':
                $(container).find("[name='" + propertyName + "']")
                break;

        }
    }
};
