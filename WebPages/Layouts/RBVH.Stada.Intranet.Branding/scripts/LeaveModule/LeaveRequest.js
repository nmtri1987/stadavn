RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.LeaveRequest = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        ServiceUrls:
        {
            EmployeeList: this.Protocol + '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetByDepartmentLocation/{1}/{2}/{3}',
            GetLeaveInfo: this.Protocol + "//{0}/_vti_bin/services/leavemanagement/leavemanagementservice.svc/GetAllLeaveInfo/{1}/{2}/{3}",
            GetApproversByRequester: this.Protocol + "//{0}/_vti_bin/services/leavemanagement/leavemanagementservice.svc/GetApproversByRequester/{1}/{2}/{3}/{4}",
            GetApproversByRequesterAndTime: this.Protocol + "//{0}/_vti_bin/services/leavemanagement/leavemanagementservice.svc/GetApproversByRequesterAndTime/{1}/{2}/{3}/{4}/{5}/{6}",
            GetLeaveManagementById: this.Protocol + "//{0}/_vti_bin/services/leavemanagement/leavemanagementservice.svc/GetLeaveManagementById/{1}",
            GetEmployeeById: this.Protocol + "//{0}/_vti_bin/services/Employee/EmployeeService.svc/GetEmployeeByEmployeeID/{1}",
            InsertLeaveRequest: this.Protocol + "//{0}/_vti_bin/services/leavemanagement/leavemanagementservice.svc/InsertLeaveManagement",
            ApproveLeaveRequest: '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/ApproveLeave',
            RejectLeaveRequest: '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/RejectLeave',
            GetApprovalPermission: '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/HasApprovalPermission/{1}',
            GetDelegatedTaskInfo: '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/GetDelegatedTaskInfo/{1}',
            GetTaskHistoryInfo: '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/GetTaskHistory/{1}/{2}'
        },
        Data: {},
        DelegatedTaskInfo: {},
        Id: settings.Id,
        LocationId: 0,
        FullName: '',
        EmployeeID: '0',
        EmployeeForID: '0',
        UserId: 0,
        DepartmentId: 0,
        EmployeeLevel: 0,
        EmployeeList: [],
        ID: '', //requester item ID
        ApproverID: 0,
        DateControlValue: {
            FromHour: "",
            FromMinute: "",
            ToHour: "",
            ToMinute: "",
        },
        IsFormValid: false,
        TotalHours: 0.0,
        TotalDays: 0,
        IsFormFieldsNotEmpty: false,
        IsFormLeaveRequestFollowPolicy: true,
        TLE: {},
        DH: {},
        BOD: {},
        UnexpectedLeave: false,
        ErrorConstants:
        {
            FromDateRelateToDate: 1,
            FromDateIsHoliday: 2,
            ToDateIsHoliday: 3,
            Policy1: 4,
            Policy2: 5,
            Policy3: 6,
            Overlap: 7,
            FromDateInvalid: 8, //From date less than current date
            SequenceLeave: 9,
            InvalidData: 100,
            Unexpected: 101
        },
        ADMIN_LEVEL: 3, // Van Thu
        TEAMLEAD_LEVEL: 3.2,
        CurrentApprover: {}
    }
    $.extend(true, this.Settings, settings);

    this.Initialize();
};

RBVH.Stada.WebPages.pages.LeaveRequest.prototype = {
    Initialize: function () {
        var that = this;
        that.Settings.Id = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemId');
        ExecuteOrDelayUntilScriptLoaded(function () {
            that.InitControls();
            that.DisableControls();
            that.PopulateData();
            that.RegisterEvents();
        }, "sp.js");
    },

    InitControls: function () {
        var that = this;
        $(that.Settings.Controls.ReasonControlSelector).val('');
        $("#tbLeaveRequestContainer .ms-formvalidation.ms-csrformvalidation").hide();
        $(that.Settings.Controls.TotalHoursControlSelector_Error).hide();
        $(that.Settings.Controls.RequestForControlSelector).select2();
        $(that.Settings.Controls.TransferWorkToControlSelector).select2();
    },

    PopulateCurrentUserInfo: function () {
        if (_rbvhContext && _rbvhContext.EmployeeInfo) {
            this.Settings.EmployeeID = _rbvhContext.EmployeeInfo.EmployeeID;
            this.Settings.FullName = _rbvhContext.EmployeeInfo.FullName;
            this.Settings.LocationId = _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
            this.Settings.UserId = _rbvhContext.EmployeeInfo.ID;
            this.Settings.DepartmentId = _rbvhContext.EmployeeInfo.Department.LookupId;
            this.Settings.EmployeeForID = _rbvhContext.EmployeeInfo.ID; //default is current requester
            this.Settings.ID = _rbvhContext.EmployeeInfo.ID;
            this.Settings.EmployeeLevel = parseFloat(_rbvhContext.EmployeeInfo.EmployeeLevel.LookupValue);
        }
    },

    PopulateData: function () {
        var that = this;
        if (!!that.Settings.Id) // Edit -> Set READ-ONLY
        {
            $(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").show();
            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetLeaveManagementById, location.host, that.Settings.Id);
            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response) {
                    that.Settings.Data = response;
                    if (response && response.Id > 0) {
                        that.GetApprovalPermission(response.Id)
                            .then(function (result) {
                                if (result && result === true) {
                                    if (that.Settings.Data && that.Settings.Data.RequestExpired == true) {
                                        errMsg = decodeURI(that.Settings.ResourceText.RequestExpiredMsgFormat);
                                        errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, that.Settings.Data.RequestDueDate);
                                        RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.Controls.ErrorMsgContainerSelector, that.Settings.Controls.ErrorMsgSelector, errMsg);
                                    }
                                    else {
                                        that.ShowControlForApprover(true);
                                    }
                                }
                                else {
                                    that.GetDelegatedTaskInfo(response.Id)
                                        .then(function (respData) {
                                            if (respData && respData.Requester.LookupId > 0) {
                                                if (that.Settings.Data && that.Settings.Data.RequestExpired == true) {
                                                    errMsg = decodeURI(that.Settings.ResourceText.RequestExpiredMsgFormat);
                                                    errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, that.Settings.Data.RequestDueDate);
                                                    RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.Controls.ErrorMsgContainerSelector, that.Settings.Controls.ErrorMsgSelector, errMsg);
                                                }
                                                else {
                                                    that.Settings.DelegatedTaskInfo = respData;
                                                    that.ShowControlForApprover(true);
                                                }
                                            }
                                            else {
                                                that.ShowControlForApprover(false);
                                            }
                                        });
                                }
                            });
                    }
                    // Requester
                    $(that.Settings.Controls.RequesterControlSelector).html(response.Requester.LookupValue);
                    // Request For
                    $(that.Settings.Controls.RequestForControlSelector).empty();
                    $(that.Settings.Controls.RequestForControlSelector).append($("<option>").attr('value', response.RequestFor.LookupId).text(response.RequestFor.LookupValue));
                    $(that.Settings.Controls.RequestForControlSelector).prop('disabled', true);
                    // From
                    var fromDate = RBVH.Stada.WebPages.Utilities.String.toMomentDateTime(response.FromDate, _spPageContextInfo.currentLanguage); //moment(response.FromDate).toDate();
                    var fromDay = fromDate.getDate(); // 0 -> 11
                    var fromMonth = fromDate.getMonth() + 1; // 0 -> 11
                    var fromYear = fromDate.getYear() + 1900;
                    var fromHour = fromDate.getHours();
                    fromHour = fromHour < 10 ? ('0' + fromHour) : fromHour;
                    fromHour = fromHour + ':';
                    var fromMinute = fromDate.getMinutes();
                    fromMinute = fromMinute < 10 ? ('0' + fromMinute) : fromMinute;
                    $(that.Settings.Controls.FromDateControlSelector).val(fromDay + '/' + fromMonth + '/' + fromYear);
                    $(that.Settings.Controls.FromHourControlSelector).val(fromHour);
                    $(that.Settings.Controls.FromMinuteControlSelector).val(fromMinute);

                    if (response.ApprovalStatus && response.ApprovalStatus.length > 0) {
                        $(that.Settings.Controls.ApprovalStatusValSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(response.ApprovalStatus));
                        $(that.Settings.Controls.ApprovalStatusHeaderSelector).show();
                        $(that.Settings.Controls.ApprovalStatusValSelector).show();
                    }
                    // To
                    var toDate = RBVH.Stada.WebPages.Utilities.String.toMomentDateTime(response.ToDate, _spPageContextInfo.currentLanguage);//moment(response.ToDate).toDate();
                    var toDay = toDate.getDate(); // 0 -> 11
                    var toMonth = toDate.getMonth() + 1; // 0 -> 11
                    var toYear = toDate.getYear() + 1900;
                    var toHour = toDate.getHours();
                    toHour = toHour < 10 ? ('0' + toHour) : toHour;
                    toHour = toHour + ':';
                    var toMinute = toDate.getMinutes();
                    toMinute = toMinute < 10 ? ('0' + toMinute) : toMinute;
                    $(that.Settings.Controls.ToDateControlSelector).val(toDay + '/' + toMonth + '/' + toYear);
                    $(that.Settings.Controls.ToHourControlSelector).val(toHour);
                    $(that.Settings.Controls.ToMinuteControlSelector).val(toMinute);
                    // Total hours
                    $(that.Settings.Controls.TotalHoursControlSelector).val(response.TotalHoursDisp);
                    // Transfer work to
                    $(that.Settings.Controls.TransferWorkToControlSelector).empty();
                    $(that.Settings.Controls.TransferWorkToControlSelector).append($("<option>").attr('value', response.TransferworkTo.LookupId).text(response.TransferworkTo.LookupValue));
                    $(that.Settings.Controls.TransferWorkToControlSelector).prop('disabled', true);
                    // Approver
                    $(that.Settings.Controls.ApproverToControlSelector).html(response.Approver.FullName);
                    // Reason
                    $(that.Settings.Controls.ReasonControlSelector).val(response.Reason);
                    // Commment
                    if (response.Comment && response.Comment.length > 0) {
                        $(that.Settings.Controls.OldCommentControlSelector).html(Functions.parseComment(response.Comment));
                        $(that.Settings.Controls.OldCommentControlSelector).closest('tr').show();
                    }
                    //Additional Approvers
                    if (response.AdditionalUser && response.AdditionalUser.length > 0) {
                        var fullNameArray = [];
                        for (var i = 0; i < response.AdditionalUser.length; i++) {
                            fullNameArray.push(response.AdditionalUser[i].FullName);
                        }
                        $("#ctl00_PlaceHolderMain_UploadLeavePplPic").val(fullNameArray.join(";"));
                    }
                }
            });
        }
        else {
            $(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").hide();
            this.PopulateCurrentUserInfo();
            this.PopulateDefaultValue();
            this.PopulateComboboxes();
            this.LoadApprovers();
            $(this.Settings.Controls.RequesterControlSelector).html(this.Settings.FullName);
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
    ShowControlForApprover: function (hasApprovalPermission) {
        var that = this;
        if (hasApprovalPermission === true) {
            $(that.Settings.Controls.ApproveControlSelector).show();
            $(that.Settings.Controls.RejectControlSelector).show();
            if (that.Settings.Data && that.Settings.Data.Comment && that.Settings.Data.Comment.length > 0) {
                $(that.Settings.Controls.OldCommentControlSelector).closest("tr").show();
            }
            else {
                $(that.Settings.Controls.OldCommentControlSelector).closest("tr").hide();
            }
            $(that.Settings.Controls.CommentControlSelector).closest("tr").show();
            $(that.Settings.Controls.CommentControlSelector).prop('disabled', false);
            $(that.Settings.Controls.LeaveHistorySelector).closest("tr").show();
        }
        else {
            $(that.Settings.Controls.ApproveControlSelector).hide();
            $(that.Settings.Controls.RejectControlSelector).hide();
            $(that.Settings.Controls.CommentControlSelector).closest("tr").hide()
            $(that.Settings.Controls.CommentControlSelector).prop('disabled', true);
            $(that.Settings.Controls.LeaveHistorySelector).closest("tr").hide();
        }
    },

    /* populate default value when page load*/
    PopulateDefaultValue: function () {
        var that = this;
        $(that.Settings.Controls.OldCommentControlSelector).closest("tr").css("display", "none");
        $(that.Settings.Controls.CommentControlSelector).closest("tr").css("display", "none");
        //Set default date for from date textbox
        var currentDate = new Date();
        currentDate.setDate(currentDate.getDate() + 1);
        var defaultFromDateObject = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate(), 7, 15, 0);
        $(that.Settings.Controls.FromDateControlSelector).val(Functions.parseVietnameseDateTimeToDDMMYYYY2(defaultFromDateObject));
        $(that.Settings.Controls.ToDateControlSelector).val("");
        $(that.Settings.Controls.TotalHoursControlSelector).val("");
        //if (!$(that.Settings.Controls.FromDateControlSelector).val()) {
        //    $(that.Settings.Controls.FromDateControlSelector).val(Functions.parseVietnameseDateTimeToDDMMYYYY2(defaultFromDateObject));
        //}
    },

    PopulateComboboxes: function () {
        if (this.Settings.EmployeeLevel == this.Settings.ADMIN_LEVEL || this.Settings.EmployeeLevel == this.Settings.TEAMLEAD_LEVEL) // Van thu -> Bind data to combobox
        {
            this.PopulateEmployeeList([this.Settings.Controls.RequestForControlSelector, this.Settings.Controls.TransferWorkToControlSelector]);
        }
        else {
            $(this.Settings.Controls.RequestForControlSelector).empty();
            $(this.Settings.Controls.RequestForControlSelector).append($("<option>").attr('value', this.Settings.UserId).text(this.Settings.FullName));
            $(this.Settings.Controls.RequestForControlSelector).prop('disabled', true);

            this.PopulateEmployeeList([this.Settings.Controls.TransferWorkToControlSelector]);
        }
    },

    ParseDateFromControl: function (controlDateString, hourString, minuteString, ss) {
        if (!ss) ss = 0;
        var controlDateObj = Functions.parseVietNameseDate(controlDateString);
        var hourNumber = parseInt(hourString.substring(":", hourString.length - 1));
        var minuteNumber = parseInt(minuteString);

        if (controlDateObj) {
            return new Date(controlDateObj.getFullYear(), controlDateObj.getMonth(), controlDateObj.getDate(), hourNumber, minuteNumber, ss);
        }
        else {
            return false;
        }
    },

    ShowInvalidRuleMessage: function () {
        var that = this;
        $.confirm({
            title: that.Settings.Modal.Title,
            content: that.Settings.Modal.WrongPolicies,
            buttons: {
                somethingElse: {
                    text: that.Settings.Modal.SeePolicesButton,
                    btnClass: 'btn-blue',
                    action: function () {
                        window.open("/Policies", "_blank")
                    }
                },
                close: {
                    text: that.Settings.Modal.CloseButton,
                    action: function () {
                    }
                },
            }
        });
    },

    OnCancelClick: function () {
        Functions.redirectToSource();
    },

    RegisterEvents: function () {
        var that = this;
        $(that.Settings.Controls.FromDateControlSelector).get(0).onvaluesetfrompicker = function (resultfield) {
            that.ProcessLeaveRequestInfo(true, false);
            that.LoadApprovers();
        };

        $(that.Settings.Controls.FromHourControlSelector).change(function () {
            that.ProcessLeaveRequestInfo(true, false);
            that.LoadApprovers();
        });

        $(that.Settings.Controls.FromMinuteControlSelector).change(function () {
            that.ProcessLeaveRequestInfo(true, false);
            that.LoadApprovers();
        });

        $(that.Settings.Controls.ToDateControlSelector).get(0).onvaluesetfrompicker = function (resultfield) {
            that.ProcessLeaveRequestInfo(true, false);
            that.LoadApprovers();
        };
        $(that.Settings.Controls.ToHourControlSelector).change(function () {
            that.ProcessLeaveRequestInfo(true, false);
            that.LoadApprovers();
        });
        $(that.Settings.Controls.ToMinuteControlSelector).change(function () {
            that.ProcessLeaveRequestInfo(true, false);
            that.LoadApprovers();
        });
        $(that.Settings.Controls.SaveControlSelector).on("click", function (event) {
            that.OnSaveData();
            event.preventDefault();
        });
        $(that.Settings.Controls.ApproveControlSelector).on("click", function (event) {
            that.OnApproveData();
            event.preventDefault();
        });
        $(that.Settings.Controls.RejectControlSelector).on("click", function (event) {
            that.OnRejectData();
            event.preventDefault();
        });
        $(that.Settings.Controls.CancelControlSelector).on("click", function (event) {
            event.preventDefault();
            that.OnCancelClick();
        });
        $(that.Settings.Controls.RequestForControlSelector).change(function () {
            that.Settings.EmployeeForID = $(that.Settings.Controls.RequestForControlSelector + " option:selected").val();
            if (that.Settings.UserId !== parseInt(that.Settings.EmployeeForID)) {
                var currentDate = new Date();
                var defaultFromDateObject = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate(), 0, 0, 0);
                $(that.Settings.Controls.FromDateControlSelector).val(Functions.parseVietnameseDateTimeToDDMMYYYY2(defaultFromDateObject));

                $("td.ms-dtinput > input[id$='dtFromDate']").prop('disabled', true);
                var parentObj = $("td.ms-dtinput > input[id$='dtFromDate']").parent().parent();
                $(parentObj).find("td.ms-dtinput > a").hide();
            }
            else {
                var currentDate = new Date();
                currentDate.setDate(currentDate.getDate() + 1);
                var defaultFromDateObject = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate(), 0, 0, 0);
                $(that.Settings.Controls.FromDateControlSelector).val(Functions.parseVietnameseDateTimeToDDMMYYYY2(defaultFromDateObject));
                $("td.ms-dtinput > input[id$='dtFromDate']").removeAttr('disabled');
                var parentObj = $("td.ms-dtinput > input[id$='dtFromDate']").parent().parent();
                $(parentObj).find("td.ms-dtinput > a").show();
            }

            that.ProcessLeaveRequestInfo(true, false);
            that.LoadApprovers();
            //that.ResetAdditionalApprovers();
        });
        if ($(that.Settings.Controls.LeaveHistorySelector).length > 0) {
            $(that.Settings.Controls.LeaveHistorySelector).on("click", function (event) {
                event.preventDefault();
                var url = "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveHistory.aspx?EmployeeId=" + $(that.Settings.Controls.RequestForControlSelector + " option:selected").val();;
                that.OpenDialogBox(url);
            });
        }
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

    //ResetAdditionalApprovers: function () {
    //    var that = this;
    //    $(that.Settings.Controls.AddtionalControlSelector).val("[]");
    //},

    OnPreGetLeaveRequest: function () {
        var that = this;
        var fromDateValue = $(that.Settings.Controls.FromDateControlSelector).val();
        var fromDateValueObject = Functions.parseVietNameseDate(fromDateValue);
        var fromHourValue = $(that.Settings.Controls.FromHourControlSelector + " option:selected").val();
        var fromMinuteValue = $(that.Settings.Controls.FromMinuteControlSelector + " option:selected").val();

        var toDateValue = $(that.Settings.Controls.ToDateControlSelector).val();
        var toDateValueObject = Functions.parseVietNameseDate(toDateValue);
        var toHourValue = $(that.Settings.Controls.ToHourControlSelector + " option:selected").val();
        var toMinuteValue = $(that.Settings.Controls.ToMinuteControlSelector + " option:selected").val();

        if (!fromDateValue || fromDateValue == "" || fromDateValueObject == false) {
            return false;
        }

        if (!toDateValue || toDateValue == "" || toDateValueObject == false) {
            return false;
        }
        var fromHourNumber = parseInt(fromHourValue.substring(":", fromHourValue.length - 1));
        var fromMinuteNumber = parseInt(fromMinuteValue);

        var toHourNumber = parseInt(toHourValue.substring(":", toHourValue.length - 1));
        var toMinuteNumber = parseInt(toMinuteValue);

        var dateTimeObject = {};
        dateTimeObject.FromDate = new Date(fromDateValueObject.getFullYear(), fromDateValueObject.getMonth(), fromDateValueObject.getDate(), fromHourNumber, fromMinuteNumber, 0);
        dateTimeObject.ToDate = new Date(toDateValueObject.getFullYear(), toDateValueObject.getMonth(), toDateValueObject.getDate(), toHourNumber, toMinuteNumber, 0);
        dateTimeObject.RequestForId = parseInt($(that.Settings.Controls.RequestForControlSelector + " option:selected").val());
        return dateTimeObject;
    },

    RebindHourMinuteControl: function () {
        var that = this;
        var fromHour = that.Settings.DateControlValue.FromHour;
        var fromMinute = that.Settings.DateControlValue.FromMinute;

        var toHour = that.Settings.DateControlValue.ToHour;
        var toMinute = that.Settings.DateControlValue.ToMinute;

        if (fromHour && fromHour.length == 2 && fromMinute && fromMinute.length == 2) {
            $(that.Settings.Controls.FromHourControlSelector).val(fromHour + ":").change();
            $(that.Settings.Controls.FromMinuteControlSelector).val(fromMinute).change();
        }
        if (toHour && toHour.length == 2 && toMinute && toMinute.length == 2) {
            $(that.Settings.Controls.ToHourControlSelector).val(toHour + ":").change();
            $(that.Settings.Controls.ToMinuteControlSelector).val(toMinute).change();
        }
    },

    SetHourMinuteValue: function (fromDateString, toDateString) {
        var that = this;
        if (fromDateString) {
            var fromDateObject = RBVH.Stada.WebPages.Utilities.String.parseISOLocal(fromDateString);
            if (fromDateObject) {
                var fromHour = RBVH.Stada.WebPages.Utilities.String.padDate(fromDateObject.getHours());
                var fromMinute = RBVH.Stada.WebPages.Utilities.String.padDate(fromDateObject.getMinutes());
                that.Settings.DateControlValue.FromHour = fromHour;
                that.Settings.DateControlValue.FromMinute = fromMinute;
            }
        }
        if (toDateString) {
            var toDateObject = RBVH.Stada.WebPages.Utilities.String.parseISOLocal(toDateString);
            if (toDateObject) {
                var toHour = RBVH.Stada.WebPages.Utilities.String.padDate(toDateObject.getHours());
                var toMinute = RBVH.Stada.WebPages.Utilities.String.padDate(toDateObject.getMinutes());
                that.Settings.DateControlValue.ToHour = toHour;
                that.Settings.DateControlValue.ToMinute = toMinute;
            }
        }
    },

    ProcessLeaveRequestInfo: function (isShowPopup, async) {
        var that = this;
        var requestData = that.OnPreGetLeaveRequest();
        if (!requestData || requestData == false || requestData.RequestForId == 0 || !requestData.RequestForId) {
            return;
        }
        else {
            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetLeaveInfo, location.host, requestData.RequestForId, RBVH.Stada.WebPages.Utilities.String.toISOStringTZ(requestData.FromDate), RBVH.Stada.WebPages.Utilities.String.toISOStringTZ(requestData.ToDate));
            $.ajax({
                type: "GET",
                url: url,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: async,
                cache: false,
                success: function (resultData) {
                    if (resultData) {
                        that.ProcessData(resultData, isShowPopup);
                    }
                },
                error: function (error) {
                }
            });
        }
    },

    LoadApprovers: function () {
        var that = this;

        that.Settings.CurrentApprover = {};
        that.Settings.TLE = {};
        that.Settings.DH = {};
        that.Settings.BOD = {};
        $(that.Settings.Controls.ApproverToControlSelector).html("");

        var currentDepartmentId = that.Settings.DepartmentId;
        var fromStr = "";
        if ($(that.Settings.Controls.FromDateControlSelector).val()) {
            var fromVal = that.ParseDateFromControl($(that.Settings.Controls.FromDateControlSelector).val(), $(that.Settings.Controls.FromHourControlSelector + " option:selected").val(), $(that.Settings.Controls.FromMinuteControlSelector + " option:selected").val(), 0);
            fromStr = RBVH.Stada.WebPages.Utilities.String.toISOStringTZ(fromVal);
        }
        var toStr = "";
        if ($(that.Settings.Controls.ToDateControlSelector).val()) {
            var toVal = that.ParseDateFromControl($(that.Settings.Controls.ToDateControlSelector).val(), $(that.Settings.Controls.ToHourControlSelector + " option:selected").val(), $(that.Settings.Controls.ToMinuteControlSelector + " option:selected").val(), 0);
            toStr = RBVH.Stada.WebPages.Utilities.String.toISOStringTZ(toVal)
        }

        var getApproverUrl = "";
        if (toStr.length > 0) {
            getApproverUrl = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetApproversByRequesterAndTime, location.host, currentDepartmentId, that.Settings.ID, that.Settings.EmployeeForID, fromStr, toStr, that.Settings.TotalHours);
        }
        else {
            getApproverUrl = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetApproversByRequester, location.host, currentDepartmentId, that.Settings.ID, that.Settings.EmployeeForID, that.Settings.TotalHours);
        }

        if (currentDepartmentId && currentDepartmentId > 0) {
            $.ajax({
                type: "GET",
                url: getApproverUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (resultData) {
                    if (resultData) {
                        that.PopulateApprovers(resultData);
                    }
                },
                error: function (error) {
                }
            });
        }
    },

    PopulateApprovers: function (data) {
        var that = this;
        //var getEmployeeInfoUrl = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetEmployeeById, location.host, that.Settings.EmployeeForID);
        //$.ajax({
        //    type: "GET",
        //    url: getEmployeeInfoUrl,
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    cache: false,
        //    async: false,
        //    success: function (resultData) {
        //        var employLevel = RBVH.Stada.WebPages.Constants.EmployeeLevel;
        //        if (resultData && that.Settings.EmployeeLevel) {
        //            if (data.Approver1 && data.Approver1[0] != null) {
        //                that.Settings.TLE = { UserName: data.Approver1[0].LoginName, ID: data.Approver1[0].ID }
        //            }
        //            else {
        //                that.Settings.TLE = null;
        //            }

        //            if (data.Approver2 && data.Approver2 != null) {
        //                that.Settings.DH = { UserName: data.Approver2.LoginName, ID: data.Approver2.ID }
        //            }
        //            else {
        //                that.Settings.DH = null;
        //            }

        //            if (data.Approver3 && data.Approver3 != null) {
        //                that.Settings.BOD = { UserName: data.Approver3.LoginName, ID: data.Approver3.ID }
        //            }
        //            else {
        //                that.Settings.BOD = null;
        //            }

        //            if (that.Settings.TLE != null) {
        //                that.Settings.CurrentApprover = data.Approver1[0];
        //                $(that.Settings.Controls.ApproverToControlSelector).html(data.Approver1[0].FullLoginName);
        //            }
        //            else if (that.Settings.DH != null) {
        //                that.Settings.CurrentApprover = data.Approver2;
        //                $(that.Settings.Controls.ApproverToControlSelector).html(data.Approver2.FullLoginName);
        //            }
        //            else if (that.Settings.BOD != null) {
        //                that.Settings.CurrentApprover = data.Approver3;
        //                $(that.Settings.Controls.ApproverToControlSelector).html(data.Approver3.FullLoginName);
        //            }

        //            //if (that.Settings.CurrentApprover) {
        //            //    InitPeoplePicker(that.Settings.CurrentApprover.ID);
        //            //}
        //        }
        //    },
        //    error: function (error) {
        //    }
        //});

        if (data.Approver1 && data.Approver1[0] != null) {
            that.Settings.TLE = { UserName: data.Approver1[0].LoginName, ID: data.Approver1[0].ID }
        }
        else {
            that.Settings.TLE = null;
        }

        if (data.Approver2 && data.Approver2 != null) {
            that.Settings.DH = { UserName: data.Approver2.LoginName, ID: data.Approver2.ID }
        }
        else {
            that.Settings.DH = null;
        }

        if (data.Approver3 && data.Approver3 != null) {
            that.Settings.BOD = { UserName: data.Approver3.LoginName, ID: data.Approver3.ID }
        }
        else {
            that.Settings.BOD = null;
        }

        if (that.Settings.TLE != null) {
            that.Settings.CurrentApprover = data.Approver1[0];
            $(that.Settings.Controls.ApproverToControlSelector).html(data.Approver1[0].FullLoginName);
        }
        else if (that.Settings.DH != null) {
            that.Settings.CurrentApprover = data.Approver2;
            $(that.Settings.Controls.ApproverToControlSelector).html(data.Approver2.FullLoginName);
        }
        else if (that.Settings.BOD != null) {
            that.Settings.CurrentApprover = data.Approver3;
            $(that.Settings.Controls.ApproverToControlSelector).html(data.Approver3.FullLoginName);
        }
    },

    ProcessData: function (data, isShowPopup) {
        var that = this;
        $(that.Settings.Controls.ToDateControlSelector_Error).hide();
        $(that.Settings.Controls.FromDateControlSelector_Error).hide();
        $(that.Settings.Controls.TotalHoursControlSelector).val("");
        $(that.Settings.Controls.FromDateControlSelector_Error).css("color", "");
        $(that.Settings.Controls.ToDateControlSelector_Error).css("color", "");

        that.SetHourMinuteValue(data.FromDate, data.ToDate);
        that.Settings.IsFormLeaveRequestFollowPolicy = true;
        that.Settings.UnexpectedLeave = data.UnexpectedLeave;
        that.Settings.TotalDays = data.TotalDays;
        if (data.ErrorCode !== 0 || data.ErrorMsg != null) {
            that.Settings.IsFormValid = false;

            var errCode = that.Settings.ErrorConstants;
            switch (data.ErrorCode) {
                case -1:
                    location.reload();
                    break;
                case errCode.FromDateRelateToDate:
                    $(that.Settings.Controls.ToDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.ToDateControlSelector_Error).show();
                    break;
                case errCode.FromDateIsHoliday:
                    $(that.Settings.Controls.FromDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.FromDateControlSelector_Error).show();
                    break;
                case errCode.ToDateIsHoliday:
                    $(that.Settings.Controls.ToDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.ToDateControlSelector_Error).show();
                    break;
                case errCode.Policy1: //This case isn't an error, can sunmit datato server
                    $(that.Settings.Controls.FromDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.FromDateControlSelector_Error).css("color", "orange");
                    $(that.Settings.Controls.FromDateControlSelector_Error).show();
                    that.Settings.IsFormLeaveRequestFollowPolicy = false;
                    that.Settings.IsFormValid = true;
                    that.Settings.TotalDays = data.TotalDays;
                    that.Settings.TotalHours = data.TotalHours;
                    that.PopulateLeaveInfo(data.TotalHoursDisp);
                    if (isShowPopup == true)
                        that.ShowInvalidRuleMessage();
                    break;
                case errCode.Policy2: //This case isn't an error, can sunmit datato server
                    $(that.Settings.Controls.FromDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.FromDateControlSelector_Error).css("color", "orange");
                    $(that.Settings.Controls.FromDateControlSelector_Error).show();
                    that.Settings.IsFormValid = true;
                    that.Settings.IsFormLeaveRequestFollowPolicy = false;
                    that.Settings.TotalDays = data.TotalDays;
                    that.Settings.TotalHours = data.TotalHours;
                    that.PopulateLeaveInfo(data.TotalHoursDisp);
                    if (isShowPopup == true)
                        that.ShowInvalidRuleMessage();
                    break;
                case errCode.Policy3: //This case isn't an error, can sunmit datato server
                    $(that.Settings.Controls.FromDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.FromDateControlSelector_Error).css("color", "orange");
                    $(that.Settings.Controls.FromDateControlSelector_Error).show();
                    that.Settings.IsFormLeaveRequestFollowPolicy = false;
                    that.Settings.IsFormValid = true;
                    that.Settings.TotalDays = data.TotalDays;
                    that.Settings.TotalHours = data.TotalHours;
                    that.PopulateLeaveInfo(data.TotalHoursDisp);
                    if (isShowPopup == true)
                        that.ShowInvalidRuleMessage();
                    break;
                case errCode.Overlap:
                    $(that.Settings.Controls.FromDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.FromDateControlSelector_Error).show();
                    break;
                case errCode.FromDateInvalid:
                    $(that.Settings.Controls.FromDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.FromDateControlSelector_Error).show();
                    break;
                case errCode.SequenceLeave:
                    $(that.Settings.Controls.FromDateControlSelector_Error).html(data.ErrorMsg);
                    $(that.Settings.Controls.FromDateControlSelector_Error).show();
                    break;
            }
        }
        else {
            that.Settings.IsFormValid = true;
            that.Settings.TotalHours = data.TotalHours;
            that.Settings.TotalDays = data.TotalDays;
            that.PopulateLeaveInfo(data.TotalHoursDisp);
        }
    },

    PopulateLeaveInfo: function (totalLeaveHour) {
        var that = this;
        $(that.Settings.Controls.TotalHoursControlSelector_Error).hide();
        $(that.Settings.Controls.TotalHoursControlSelector).val(totalLeaveHour);
    },

    OnPreSaveData: function () {
        var that = this;
        that.Settings.IsFormValid = false;
        var isFiledNotEmpty = that.ValidateRequiredField();
        var isTransferWorkToValid = that.ValidateTransferworkTo();
        var isRequestForDateValid = that.ValidateRequestForDate();

        if (!isFiledNotEmpty || !isTransferWorkToValid || !isRequestForDateValid) {
            return false;
        }

        that.ProcessLeaveRequestInfo(false, false);
        if (that.Settings.IsFormValid == false) {
            return false;
        }

        var isLeaveHourValid = that.ValidateLeaveHours();
        if (!isLeaveHourValid) {
            return false;
        }

        return true;
    },

    /*Validate required fileds. If user leave blank, error message will be displayed */
    ValidateRequiredField: function () {
        $("span[class='ms-formvalidation ms-csrformvalidation']").html("");
        var that = this;
        //requester control
        var emptyFieldCount = 0;
        //requester
        emptyFieldCount += that.ValidateEmptyField(that.Settings.ID, that.Settings.Controls.RequesterControlSelector_Error, that.Settings.ValidationMessage.CantLeaveTheBlank) == true ? 0 : 1;
        //request for
        emptyFieldCount += that.ValidateEmptyField(that.Settings.EmployeeForID, that.Settings.Controls.RequestForControlSelector_Error, that.Settings.ValidationMessage.CantLeaveTheBlank) == true ? 0 : 1;
        //from date control
        emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.FromDateControlSelector).val(), that.Settings.Controls.FromDateControlSelector_Error, that.Settings.ValidationMessage.CantLeaveTheBlank) == true ? 0 : 1;
        //to date control
        emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.ToDateControlSelector).val(), that.Settings.Controls.ToDateControlSelector_Error, that.Settings.ValidationMessage.CantLeaveTheBlank) == true ? 0 : 1;

        emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.TransferWorkToControlSelector + " option:selected").val(), that.Settings.Controls.TransferWorkToControlSelector_Error, that.Settings.ValidationMessage.CantLeaveTheBlank) == true ? 0 : 1;

        return emptyFieldCount == 0;
    },

    ValidateLeaveHours: function () {
        var that = this;
        $(that.Settings.Controls.TotalHoursControlSelector_Error).html("");
        if (that.Settings.TotalHours <= 0) {
            $(that.Settings.Controls.TotalHoursControlSelector_Error).html(that.Settings.ValidationMessage.LeaveHoursInvalid);
            $(that.Settings.Controls.TotalHoursControlSelector_Error).show();
            return false;
        }
        $(that.Settings.Controls.TotalHoursControlSelector_Error).hide();
        return true;
    },

    /*Validate request for and transfer work to must be different*/
    /*Return:: true: different; false: same */
    ValidateTransferworkTo: function () {
        var that = this;
        $(that.Settings.Controls.TransferWorkToControlSelector_Error).html("");
        var requestForID = that.Settings.EmployeeForID;
        var transferWorkToID = $(that.Settings.Controls.TransferWorkToControlSelector + " option:selected").val();

        if (requestForID && transferWorkToID && parseInt(requestForID) == parseInt(transferWorkToID)) {
            $(that.Settings.Controls.TransferWorkToControlSelector_Error).html(that.Settings.ValidationMessage.TransferWorkToInvalid);
            $(that.Settings.Controls.TransferWorkToControlSelector_Error).show();
            return false;
        }
        $(that.Settings.Controls.TransferWorkToControlSelector_Error).hide();
        return true;
    },

    ValidateRequestForDate: function () {
        if (this.Settings.EmployeeLevel == this.Settings.ADMIN_LEVEL || this.Settings.EmployeeLevel == this.Settings.TEAMLEAD_LEVEL) {
            var requesterId = this.Settings.ID;
            var requestForId = this.Settings.EmployeeForID;
            if (requesterId > 0 && requestForId > 0 && requesterId != requestForId) {
                var currentDate = new Date();
                var currentDateString = Functions.parseVietnameseDateTimeToDDMMYYYY2(currentDate);
                if ($(this.Settings.Controls.FromDateControlSelector).val() != currentDateString) // Today != From Date: InValid
                {
                    $(this.Settings.Controls.FromDateControlSelector_Error).html(this.Settings.ValidationMessage.RequestForDateInvalid);
                    $(this.Settings.Controls.FromDateControlSelector_Error).show();
                    return false;
                }
            }
        }
        $(this.Settings.Controls.FromDateControlSelector_Error).hide();
        return true;
    },

    ValidateEmptyField: function (controlValue, errorControl, message) {
        if (!controlValue || controlValue == "") {
            $(errorControl).html(message);
            $(errorControl).show();
        }
        else {
            $(errorControl).hide();
            return true;
        }
    },

    OnSaveData: function () {
        var that = this;
        var isValid = that.OnPreSaveData();
        if (isValid == true) {
            var postData = that.GetPostData();
            $(that.Settings.Controls.SaveControlSelector).prop("disabled", "disabled");
            that.SaveLeaveData(postData);
        }
    },

    SaveLeaveData: function (postData) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.InsertLeaveRequest, location.host);
        if (postData) {
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                that.OnEndSaveLeaveData();
            });
        }
    },

    OnApproveData: function () {
        var that = this;
        that.ShowOrHideErrorMessage($('.comment_Error'), "");
        var postData = {};
        postData.Id = that.Settings.Id;
        postData.Comment = $(that.Settings.Controls.CommentControlSelector).val();
        $(that.Settings.Controls.ApproveControlSelector).prop("disabled", "disabled");
        that.DoApproveLeave(postData);
    },

    DoApproveLeave: function (postData) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ApproveLeaveRequest, location.host);
        if (postData) {
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response && response.Code === 0) {
                    Functions.redirectToSource();
                }
                else if (response && response.Code === 11) {
                    alert(response.Message);
                    window.location.reload();
                }
            });
        }
    },

    OnRejectData: function () {
        var that = this;
        that.ShowOrHideErrorMessage($('.comment_Error'), "");
        var postData = {};
        postData.Id = that.Settings.Id;
        postData.Comment = $(that.Settings.Controls.CommentControlSelector).val();
        if (postData.Comment && postData.Comment.length > 0) {
            $(that.Settings.Controls.SaveControlSelector).prop("disabled", "disabled");
            that.DoRejectLeave(postData);
        }
        else {
            that.ShowOrHideErrorMessage($('.comment_Error'), that.Settings.ResourceText.CantLeaveTheBlank);
        }
    },

    DoRejectLeave: function (postData) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.RejectLeaveRequest, location.host);
        if (postData) {
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response && response.Code === 0) {
                    Functions.redirectToSource();
                }
                else if (response && response.Code === 12) {
                    alert(response.Message);
                    window.location.reload();
                }
            });
        }
    },

    OnEndSaveLeaveData: function () {
        Functions.redirectToSource();
    },
    ShowOrHideErrorMessage: function (ctrl, msg) {
        if (msg && msg.length > 0) {
            $(ctrl).html(msg);
            $(ctrl).show();
        }
        else {
            $(ctrl).hide();
            $(ctrl).html("");
        }
    },
    GetPostData: function () {
        var that = this;
        var postData = {};
        postData.Requester = { LookupId: that.Settings.ID };
        postData.RequestFor = { LookupId: $(that.Settings.Controls.RequestForControlSelector + " option:selected").val() };
        postData.Department = { LookupId: that.Settings.DepartmentId };
        postData.Location = { LookupId: that.Settings.LocationId };
        var from = that.ParseDateFromControl($(that.Settings.Controls.FromDateControlSelector).val(), $(that.Settings.Controls.FromHourControlSelector + " option:selected").val(), $(that.Settings.Controls.FromMinuteControlSelector + " option:selected").val(), 0);
        postData.FromDate = RBVH.Stada.WebPages.Utilities.String.toISOStringTZ(from);
        var to = that.ParseDateFromControl($(that.Settings.Controls.ToDateControlSelector).val(), $(that.Settings.Controls.ToHourControlSelector + " option:selected").val(), $(that.Settings.Controls.ToMinuteControlSelector + " option:selected").val(), 0);
        postData.ToDate = RBVH.Stada.WebPages.Utilities.String.toISOStringTZ(to);
        postData.LeaveHours = parseFloat(that.Settings.TotalHours);
        postData.Reason = $(that.Settings.Controls.ReasonControlSelector).val();
        postData.Comment = $(that.Settings.Controls.CommentControlSelector).val();
        postData.TransferworkTo = { LookupId: $(that.Settings.Controls.TransferWorkToControlSelector + " option:selected").val() };
        postData.TLE = that.Settings.TLE;
        postData.DH = that.Settings.DH;
        postData.BOD = that.Settings.BOD;
        postData.Left = false;
        postData.UnexpectedLeave = that.Settings.UnexpectedLeave;
        if (that.Settings.UnexpectedLeave == true) {
            that.Settings.IsFormLeaveRequestFollowPolicy = false;
        }
        postData.IsValidRequest = that.Settings.IsFormLeaveRequestFollowPolicy;
        postData.Approver = { ID: that.Settings.ApproverID };
        //postData.AdditionalUser = that.GetAdditionalApprover();
        postData.TotalDays = that.Settings.TotalDays;
        return postData;
    },

    DisableControls: function () {
        var that = this;
        $(that.Settings.Controls.FromDateControlSelector).keydown(function (e) { e.preventDefault(); return false; });
        $(that.Settings.Controls.ToDateControlSelector).keydown(function (e) { e.preventDefault(); return false; });

        if (!!that.Settings.Id) // Edit -> Set READ-ONLY
        {
            $(that.Settings.Controls.SaveControlSelector).prop("disabled", "disabled");
            $(that.Settings.Controls.FromDateControlSelector).prop("disabled", "disabled");
            $("#ctl00_PlaceHolderMain_dtFrom_dtFromDateDatePickerImage").hide();
            $(that.Settings.Controls.FromHourControlSelector).prop("disabled", "disabled");
            $(that.Settings.Controls.FromMinuteControlSelector).prop("disabled", "disabled");
            $("#ctl00_PlaceHolderMain_dtTo_dtToDateDatePickerImage").hide();
            $(that.Settings.Controls.ToDateControlSelector).prop("disabled", "disabled");
            $(that.Settings.Controls.ToHourControlSelector).prop("disabled", "disabled");
            $(that.Settings.Controls.ToMinuteControlSelector).prop("disabled", "disabled");
            $(that.Settings.Controls.AddtionalDisplayControlSelector).prop("disabled", "disabled")
            $("input[name='ctl00$PlaceHolderMain$UploadLeavePplPic']").prop("disabled", "disabled");
            $(that.Settings.Controls.ReasonControlSelector).prop("disabled", "disabled");
            $('td[name="tdApprovalStatus"]').show();
        } else {
            $('td[name="tdApprovalStatus"]').hide();
        }
    },

    GetAdditionalApprover: function () {
        var that = this;
        var approvers = [];
        var additionalApproverValue = $(that.Settings.Controls.AddtionalControlSelector).attr("value");
        if (additionalApproverValue) {
            var additionalApproverArray = JSON.parse(additionalApproverValue);
            if (additionalApproverArray) {
                for (var i = 0; i < additionalApproverArray.length; i++) {
                    approvers.push({ UserName: additionalApproverArray[i].Login })
                }
            }
        }
        return approvers;
    },

    ValidateAdditionalApprover: function () {
        var that = this;
        var count = 0;
        var approvers = that.GetAdditionalApprover();
        if (approvers.length > 0) {
            var currentApprover = that.Settings.CurrentApprover;
            if (currentApprover) {
                for (var i = 0; i < approvers.length; i++) {
                    if (approvers[i].UserName != "" && approvers[i].UserName == currentApprover.LoginName) {
                        count++;
                    }
                }
            }
        }
        if (count > 0) {
            $(that.Settings.Controls.AddtionalControlSelector_Error).html(that.Settings.ValidationMessage.AdditionApproversInValid);
            $(that.Settings.Controls.AddtionalControlSelector_Error).show();
            return false;
        }
        else {
            $(that.Settings.Controls.AddtionalControlSelector_Error).html("");
            $(that.Settings.Controls.AddtionalControlSelector_Error).hide();
            return true;
        }
    },

    PopulateEmployeeList: function (comboboxes) {
        var that = this;
        var maxLevel = 6; // Quản lý trực tiếp
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.EmployeeList, location.host, that.Settings.LocationId, that.Settings.DepartmentId, maxLevel);

        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function (result) {
                that.Settings.EmployeeList = result;
                if (result && result.length > 0) {
                    $(that.Settings.RequestForControlSelector).empty();
                    $(that.Settings.TransferWorkToControlSelector).empty();
                    $.each(result, function (i, item) {
                        $.each(comboboxes, function (i, combobox) {
                            $(combobox).append($("<option>").attr('value', item.ID).text(item.FullName));
                        });
                    });
                    $(that.Settings.Controls.RequestForControlSelector).val(that.Settings.EmployeeForID).change();
                }
            }
        });
    },
    OpenDialogBox: function (Url) {
        var that = this;
        var modelDialogOptions = { url: Url, width: 1000, height: 550, showClose: true, allowMaximize: false, title: $('#leavehistorytitle').html() };
        SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', modelDialogOptions);
    }
};