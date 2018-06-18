var NotOvertimeForm = {
    SaveButton: "Save",
    CancelButton: "Cancel",
    CloseButton: "Close",
    DontHaveAnyShiftRequestMessage: "You don't have any overtime request in selected day",
    CannotLoadCurrentUserMesage: "Can not get current user. Please try again!",
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages",
    RequiredField: "This is a required field.",
    InValidDate: "Date is not less than current date",
    OvertimeItemExist: "This date has already been requested",
    NotOvertimeDateErrorMessage: 'Date must be greater than current date {0} day(s)',
    IsManager: true,
    IsManagerServiceUrl: "/_vti_bin/Services/Employee/EmployeeService.svc/IsManager/",
    IsValidDate: false,
    DepartmentNameRequiredField: "Department Name Required Field",
    DepartmentName: "Department Name",
    CantLeaveTheBlank: "You can't leave this blank.",
    InvalidInputDate: "Invalid date",
    GetOvertimeDetailByDate: "/_vti_bin/Services/Overtime/OvertimeService.svc/GetOvertimeDetailByDate/",
    GetEmployeeApprovers: "/_vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeApprovers/",
    GetCurrentUser: "/_vti_bin/Services/Employee/EmployeeService.svc/GetCurrentUser",
    GetDepartmentByIdLanguageCode: "/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentByIdLanguageCode/",
    IsNotOvertimeExist: "/_vti_bin/Services/Overtime/OvertimeService.svc/IsNotOvertimeExist/",
    GetConfigurations: '//{0}/_vti_bin/Services/Configurations/ConfigurationsService.svc/GetConfigurations',
    IsTextOnly: false,
    ConfigKey_ValidNotOvertimeDate: 'NotOvertimeForm_ValidNotOvertimeDate',
    Configurations: {},
    DiffDays: 0,
};

(function () {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        var pm = SP.Ribbon.PageManager.get_instance();
        pm.add_ribbonInited(function () {
            hideEditRibbon();
        });
        var ribbon = null;
        try {
            ribbon = pm.get_ribbon();
        }
        catch (e) { }

        if (!ribbon) {
            if (typeof (_ribbonStartInit) == "function")
                _ribbonStartInit(_ribbon.initialTabId, false, null);
        }
        else {
            hideEditRibbon();
        }

        if (NotOvertimeForm.IsTextOnly) {
            $('#s4-ribbonrow').hide();
        }
    }, "sp.ribbon.js");

    var notOvertimeFormContext = {};
    notOvertimeFormContext.Templates = {};
    notOvertimeFormContext.OnPostRender = overtimeFormOnPostRender;
    notOvertimeFormContext.Templates.View = overtimeView;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(notOvertimeFormContext);

})();

function hideEditRibbon() {
    try {
        var ribbon = SP.Ribbon.PageManager.get_instance().get_ribbon();
        SelectRibbonTab("Ribbon.Read", true);
        try {
            ribbon.removeChild('Ribbon.ListForm.Display');
        }
        catch (ex) { }

        try {
            ribbon.removeChild('Ribbon.ListForm.Edit');
        }
        catch (ex) { }
    } catch (ex) { }
}

function hideDatePicker(ctx) {
    //common function in CommForm.js
    return Functions.hideDatePickerDisplayForm(ctx);
}

function OnPageOvertimePageResourcesReady() {
    $("#save-button").val(Res.saveButton);
    $("#cancel-button").val(Res.cancelButton);
    NotOvertimeForm.CancelButton = Res.cancelButton;
    NotOvertimeForm.CloseButton = Res.closeButton;
    NotOvertimeForm.DontHaveAnyShiftRequestMessage = Res.notOvertime_DontHaveAnyShiftRequest;
    NotOvertimeForm.CannotLoadCurrentUserMessage = Res.notOvertime_CannotLoadCurrentUser;
    NotOvertimeForm.InValidDate = Res.notOvertime_InvalidDate;
    NotOvertimeForm.RequiredField = Res.requiredField;
    NotOvertimeForm.OvertimeItemExist = Res.notOvertime_ItemExist;
    $("span .ms-accentText").prop("title", Res.requiredField);
    $("#departmentName").prop("title", Res.departmentNameRequired);
    $("#DepartmentName-Title").text(Res.employeeDepartmentName);
    NotOvertimeForm.DepartmentNameRequiredField = Res.departmentNameRequired;
    NotOvertimeForm.CantLeaveTheBlank = Res.cantLeaveTheBlank;
    NotOvertimeForm.InvalidInputDate = Res.invalidDate;
    NotOvertimeForm.NotOvertimeDateErrorMessage = Res.notOvertimeDateErrorMessage;
}

function overtimeFormOnPostRender(ctx) {
    var paramTextOnly = Functions.getParameterByName('TextOnly');
    if (paramTextOnly) {
        NotOvertimeForm.IsTextOnly = true;
    }

    // Register resource
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(NotOvertimeForm.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + NotOvertimeForm.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(NotOvertimeForm.PageResourceFileName, "Res", OnPageOvertimePageResourcesReady);
    }, "strings.js");

    if (ctx.BaseViewID == "DisplayForm") {
        $(CustomFormControl.SaveButton).hide();
        if (NotOvertimeForm.IsTextOnly) {
            $('#close-button').hide();
        }
        else {
            $('#close-button').show();
        }
        $('#cancel-button').hide();
        var departmentLookupTypeHref = $("#DepartmentSelect a").attr('href');
        var departmentId = "";
        if (departmentLookupTypeHref) {
            departmentId = Functions.getParamByName("ID", departmentLookupTypeHref);
        }
        if (departmentId > 0) {
            var lcid = SP.Res.lcid;
            Functions.loadDepartment(departmentId, "#DepartmentSelect", "", lcid);
        }
    }
    else {
        $("#DepartmentSelect").hide();
        $("#departmentnameTitleTd").hide();
    }

    $(CustomFormControl.RequesterSelect).val(0).change();
    //Remove br
    $("#Custom_EmployeeID br").remove();
    $("#Custom_Requester br").remove();
    $("#Custom_HoursPerDay br").remove();
    $("#Custom_DepartmentName br").remove();
    loadData_EventRegister();
    loadEmployeeData();
    disableControl();
    appendEmptyDatatoDateTime();
    hideDatePicker();
    GetConfigurations();
    $("#DeltaPlaceHolderMain").addClass('border-container');
}

function loadDepartmentName(departmentId) {
    var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.DepartmentList, location.host, lcid);
    $(that.Settings.DepartmentControlSelector).attr("disabled", false);
    $(that.Settings.DepartmentControlSelector).empty();
    $.ajax({
        type: "GET",
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            $(result).each(function () {

                $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', this.Id).text(this.DepartmentName));
            });
            that.Settings.DepartmentId = $(that.Settings.DepartmentControlSelector).val();
        }
    });
}

function GetConfigurations() {
    var postData = [NotOvertimeForm.ConfigKey_ValidNotOvertimeDate];
    var url = RBVH.Stada.WebPages.Utilities.String.format(NotOvertimeForm.GetConfigurations, location.host);
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
            NotOvertimeForm.Configurations = response;
        }
    });
}

function hideDatePicker() {
    var datePickers = ["CommonFrom", "To"];
    Functions.hideDatePartInDatetimePicker(datePickers);
}

function loadData_EventRegister() {
    $(CustomFormControl.DateInput).get(0).onvaluesetfrompicker = function (resultfield) {
        NotOvertimeForm.IsValidDate = false;
        $(CustomFormControl.DateError).html('');
        if (validateNotOvertimeDate() == false) {
            $(CustomFormControl.DateError).html(RBVH.Stada.WebPages.Utilities.String.format(NotOvertimeForm.NotOvertimeDateErrorMessage, NotOvertimeForm.DiffDays));
            return false;
        }
        handleShiftTimeData();
    };
}

function validateNotOvertimeDate() {
    try {
        var configVal = Functions.getConfigValue(NotOvertimeForm.Configurations, NotOvertimeForm.ConfigKey_ValidNotOvertimeDate);
        if (configVal) {
            NotOvertimeForm.DiffDays = parseInt(configVal);
        }
    }
    catch (err) { NotOvertimeForm.DiffDays = 0; }

    var nowDate = new Date();
    var minDateObj = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
    minDateObj = minDateObj.setDate(minDateObj.getDate() + NotOvertimeForm.DiffDays);

    var selectedDate = $(CustomFormControl.DateInput).val();
    var notOvertimeDate = Functions.parseVietNameseDate(selectedDate);

    return (notOvertimeDate.valueOf() >= minDateObj.valueOf());
}

function handleShiftTimeData() {
    var currentEmployeeLookupId = $(CustomFormControl.RequesterSelect + " option:selected").val();
    var selectedDate = $(CustomFormControl.DateInput).val();

    if (!$.isNumeric(currentEmployeeLookupId)) {
        return;
    }
    //var date = new Date(selectedDate);
    var date = Functions.parseVietNameseDate(selectedDate);
    var dateString = (date.getMonth() * 1 + 1) + "-" + date.getDate() + "-" + date.getFullYear();

    var loadOvertimeDetailURL = _spPageContextInfo.webAbsoluteUrl + NotOvertimeForm.GetOvertimeDetailByDate + currentEmployeeLookupId + "/" + dateString;
    var loadOvertimeDetailDataPromise = $.ajax({
        type: "GET",
        cache: false,
        async: false,
        url: loadOvertimeDetailURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });

    loadOvertimeDetailDataPromise.then(
        function (data) {
            if (data != null) {
                populateOvertimeData(data);
            }
        },
        function (error) {
            //console.log(error);
        });
}

function populateOvertimeData(data) {
    if (!data.IsHasValue) {
        //Dont have any overtime request in selected date
        $(CustomFormControl.DateError).html(NotOvertimeForm.DontHaveAnyShiftRequestMessage);

        appendEmptyDatatoDateTime();
        NotOvertimeForm.IsValidDate = false;
    }
    else {
        $(CustomFormControl.DateError).html("");
        var fromMinute = data.FromMinute < 9 ? ("0" + (data.FromMinute * 1)) : data.FromMinute * 1;
        var toMinute = data.ToMinute < 9 ? ("0" + (data.ToMinute * 1)) : data.ToMinute * 1;

        $(CustomFormControl.FromHourSelect).val(data.FromHour).change();
        $(CustomFormControl.FromMinuteSelect).val(fromMinute).change();
        $(CustomFormControl.ToHourSelect).val(data.ToHour).change();
        $(CustomFormControl.ToMinuteSelect).val(toMinute).change();
        $(CustomFormControl.HoursPerDayInput).val(data.HourPerDay);

        $("span[role='alert']").text("");
        NotOvertimeForm.IsValidDate = true;
    }
}
function loadApprovers(currentEmployeeId) {

    var loadApproverURL = _spPageContextInfo.webAbsoluteUrl + NotOvertimeForm.GetEmployeeApprovers + currentEmployeeId;
    var loadapproversPromise = $.ajax({
        type: "GET",
        url: loadApproverURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
    loadapproversPromise.then(
        function (data) {
            if (data != null) {
                populateApprovers(data, currentEmployeeId)
            }
        },
        function () {
            //console.log("Can not load Approvers");
        });
}

function populateApprovers(data, currentEmployeeId) {
    var approverData = [];
    approverData.push(null);
    checkIsManager(currentEmployeeId);

    if (NotOvertimeForm.IsManager) {
        var approver2LoginName = data.Approver3 ? data.Approver3.LoginName : '';
    }
    else {
        var approver2LoginName = data.Approver2 ? data.Approver2.LoginName : '';
    }
    approverData.push({ InternalFieldName: "CommonApprover1", FullLoginUserName: approver2LoginName });

    ExecuteOrDelayUntilScriptLoaded(function () {
        var control = $(CustomFormControl.CustomApprover);
        Functions.populateApprovertoPeoplePicker(approverData, currentEmployeeId, control);
    }, 'clientpeoplepicker.js');
}

function loadEmployeeData() {
    var getCurrentUserServiceURL = _spPageContextInfo.webAbsoluteUrl + NotOvertimeForm.GetCurrentUser;
    var loadDataPromise = $.ajax({
        type: "GET",
        url: getCurrentUserServiceURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });

    loadDataPromise.then(function (data) {
        if (data != null) {
            appendDataToControl(data);
        }
        else {
            $(CustomFormControl.RequesterError).html(NotOvertimeForm.CannotLoadCurrentUserMessage);
        }
    },
        function () {
            //console.log("Can not load current employee");
        }
    );
}

function appendDataToControl(data) {
    if (data.Department != null) {
        getDepartmentName(data.Department.LookupId, data.Department.LookupValue);
        $(CustomFormControl.DepartmentSelect).val(data.Department.LookupId).change();
        $(CustomFormControl.LocationSelectHidden).val(data.Location.LookupId).change();
    }
    if (data.ID != undefined && data.ID > 0) {

        $(CustomFormControl.RequesterSelect).val(data.ID).change();
        loadApprovers(data.ID);
    }
}

function getDepartmentName(departmentId, departmentNameDefault) {
    var lcid = SP.Res != null ? SP.Res.lcid : _spPageContextInfo.currentLanguage;

    var getDepartmentURL = _spPageContextInfo.webAbsoluteUrl + NotOvertimeForm.GetDepartmentByIdLanguageCode + departmentId + "/" + lcid;

    var getDepartmentPromise = $.ajax({
        type: "GET",
        async: false,
        url: getDepartmentURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
    getDepartmentPromise.then(function (data) {
        if (data != null) {
            $(CustomFormControl.DepartmentNameInput).val(data.DepartmentName);
        }
        else {
            $(CustomFormControl.DepartmentNameInput).val(departmentNameDefault);
        }
    }, function () {
        $(CustomFormControl.DepartmentNameInput).val(departmentNameDefault);
    });
}

function disableControl() {
    $(CustomFormControl.DepartmentNameInput).prop('disabled', true);
    $(CustomFormControl.RequesterSelect).prop('disabled', true);
    $(CustomFormControl.HoursPerDayInput).prop('disabled', true);
    $('#Custom_From select').css('width', 'auto');
    $('#Custom_To select').css('width', 'auto');
}

function appendEmptyDatatoDateTime() {
    $(CustomFormControl.FromHourSelect).prop('disabled', true).val('');
    $(CustomFormControl.FromMinuteSelect).prop('disabled', true).val('');
    $(CustomFormControl.ToHourSelect).prop('disabled', true).val('');
    $(CustomFormControl.ToMinuteSelect).prop('disabled', true).val('');
    $(CustomFormControl.HoursPerDayInput).prop('disabled', true).val('');
}

var CustomFormControl =
    {
        DepartmentSelect: "#DepartmentSelect select",
        DepartmentNameInput: "#Custom_DepartmentName input",
        LocationSelectHidden: "#Custom_LocationSelect select",
        EmployeIDSelect: "#Custom_EmployeeID select",
        RequesterSelect: "#Custom_Requester select",
        FromTimeInput: "#Custom_From input",
        FromHourSelect: "#Custom_From select:first",
        FromMinuteSelect: "#Custom_From select:last",
        ToTimeInput: "#Custom_To input",
        ToHourSelect: "#Custom_To select:first",
        ToMinuteSelect: "#Custom_To select:last",
        DateInput: "#Custom_Date input",
        HoursPerDayInput: "#Custom_HoursPerDay input",
        BOBInput: "#Custom_BOD",
        DateError: "#CustomError_CommonDate",
        RequesterError: "#CustomError_Requester",
        CustomApprover: "#Custom_Apporver1",
        SaveButton: "#save-button"
    }

function calculate_HourPerDay_EventRegister() {
    $(CustomFormControl.FromHourSelect).change(function () {
        calculate_HourPerDay();
    });
    $(CustomFormControl.FromMinuteSelect).change(function () {
        calculate_HourPerDay();
    });
    $(CustomFormControl.ToHourSelect).change(function () {
        calculate_HourPerDay();
    });
    $(CustomFormControl.ToMinuteSelect).change(function () {
        calculate_HourPerDay();
    });
}

function isValidDate(stringDate) {
    var date = Functions.parseVietNameseDate(stringDate);
    var timestamp = Date.parse(date)

    if (isNaN(timestamp) == false) {
        return true;
    }
    return false;
}

function Submit(formId) {
    if ($(CustomFormControl.DateInput).val() == "") {
        $(CustomFormControl.DateError).html(NotOvertimeForm.CantLeaveTheBlank);
        return;
    }
    else if (!isValidDate($(CustomFormControl.DateInput).val())) {
        $(CustomFormControl.DateError).html(NotOvertimeForm.InvalidInputDate);
        return;
    }
    else {
        $(CustomFormControl.DateError).html("");
    }

    if (!NotOvertimeForm.IsValidDate) {
        $(CustomFormControl.DateError).html(NotOvertimeForm.DontHaveAnyShiftRequestMessage);
        return;
    }
    else {
        checkNotOvertimeExist().then(function (result) {
            $(CustomFormControl.DateError).html("")
            // true: Existed
            if (result) {
                $(CustomFormControl.DateError).html(NotOvertimeForm.OvertimeItemExist)
            }
            else {
                if (ValidateForm()) {
                    $(CustomFormControl.FromTimeInput).val($(CustomFormControl.DateInput).val());
                    $(CustomFormControl.ToTimeInput).val($(CustomFormControl.DateInput).val());
                    var isSuccess = SPClientForms.ClientFormManager.SubmitClientForm(formId);
                    $("#save-button").prop("disabled", isSuccess);
                }
            }
        });
    }
}

function checkNotOvertimeExist() {
    var currentEmployeeLookupId = $(CustomFormControl.RequesterSelect + " option:selected").val();
    var selectedDate = $(CustomFormControl.DateInput).val();
    var dateString = Functions.parseVietnameseDateTimeToMMDDYYYY(selectedDate);

    var d = $.Deferred();
    var checkOvertimePromiseURL = _spPageContextInfo.webAbsoluteUrl + NotOvertimeForm.IsNotOvertimeExist + currentEmployeeLookupId + "/" + dateString;
    $.ajax({
        type: "GET",
        cache: false,
        url: checkOvertimePromiseURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            d.resolve(data);
        },
        error: function () {
            return false;
        }
    });
    return d.promise();
}

function ValidateForm() {
    GetConfigurations();
    if (validateNotOvertimeDate() == false) {
        $(CustomFormControl.DateError).html(RBVH.Stada.WebPages.Utilities.String.format(NotOvertimeForm.NotOvertimeDateErrorMessage, NotOvertimeForm.DiffDays));
        return false;
    }
    $(CustomFormControl.DateError).html("");
    return true;
}

function Cancel() {
    var sourcePath = Functions.getParameterByName("Source");
    if (sourcePath == undefined || sourcePath.length == 0) {
        sourcePath = _spPageContextInfo.webAbsoluteUrl;
    }
    window.location.href = sourcePath;
}

function overtimeView(ctx) {
    var formTable = "";
    if (ctx.BaseViewID == "DisplayForm") {
        formTable = OvertimeForms.getHtmlDisplayForm();
    }
    else {
        formTable = OvertimeForms.getHtml();
    }
    var departmentHTML = "";
    if (ctx.BaseViewID == "NewForm") {
        departmentHTML = '<span dir="none"><input type="text" value="" maxlength="255" id="departmentName" title="{{RequiredFieldDepartment}}" style="ime-mode : " class="ms-long ms-spellcheck-true" /></span>';
        formTable = formTable.replace("{{hiddenDepartmentName}}", "");
    }
    else {
        formTable = formTable.replace("{{hiddenDepartmentName}}", "style='display:none'");
    }
    formTable = formTable.replace("{{DepartmentName}}", departmentHTML);
    formTable = formTable.replace("{{RequiredFieldDepartment}}", NotOvertimeForm.DepartmentNameRequiredField);
    formTable = formTable.replace("{{DepartmentName_Title}}", NotOvertimeForm.DepartmentName);
    formTable = formTable.replace("{{Requester_Title}}", Functions.getSPFieldTitle(ctx, "Requester"));
    formTable = formTable.replace("{{DepartmentID_Title}}", Functions.getSPFieldTitle(ctx, "CommonDepartment"));
    formTable = formTable.replace("{{LocationID_Title}}", Functions.getSPFieldTitle(ctx, "CommonLocation"));
    formTable = formTable.replace("{{HoursPerDay_Title}}", Functions.getSPFieldTitle(ctx, "HoursPerDay"));
    formTable = formTable.replace("{{Date_Title}}", Functions.getSPFieldTitle(ctx, "CommonDate"));
    formTable = formTable.replace("{{From_Title}}", Functions.getSPFieldTitle(ctx, "CommonFrom"));
    formTable = formTable.replace("{{To_Title}}", Functions.getSPFieldTitle(ctx, "To"));
    formTable = formTable.replace("{{HeadOfDepartment_Title}}", Functions.getSPFieldTitle(ctx, "CommonApprover1"));
    formTable = formTable.replace("{{BOD_Title}}", Functions.getSPFieldTitle(ctx, "CommonApprover2"));
    formTable = formTable.replace("{{Reason_Title}}", Functions.getSPFieldTitle(ctx, "Reason"));


    formTable = formTable.replace("{{Requester}}", Functions.getSPFieldRender(ctx, "Requester"));

    formTable = formTable.replace("{{DepartmentID}}", Functions.getSPFieldRender(ctx, "CommonDepartment"));
    formTable = formTable.replace("{{LocationID}}", Functions.getSPFieldRender(ctx, "CommonLocation"));
    formTable = formTable.replace("{{HoursPerDay}}", Functions.getSPFieldRender(ctx, "HoursPerDay"));
    formTable = formTable.replace("{{Date}}", Functions.getSPFieldRender(ctx, "CommonDate"));
    formTable = formTable.replace("{{From}}", Functions.getSPFieldRender(ctx, "CommonFrom"));
    formTable = formTable.replace("{{To}}", Functions.getSPFieldRender(ctx, "To"));
    formTable = formTable.replace("{{HeadOfDepartment}}", Functions.getSPFieldRender(ctx, "CommonApprover1"));
    formTable = formTable.replace("{{Reason}}", Functions.getSPFieldRender(ctx, "Reason"));

    formTable = formTable.replace("{{SaveButton}}", NotOvertimeForm.SaveButton);
    formTable = formTable.replace("{{CancelButton}}", NotOvertimeForm.CancelButton);
    formTable = formTable.replace("{{CloseButton}}", NotOvertimeForm.CloseButton);

    formTable = formTable.replace(/{{RequiredFieldText}}/gi, NotOvertimeForm.RequiredField);

    formTable = formTable.replace("{{FormId}}", ctx.FormUniqueId);
    return formTable;
}

var OvertimeForms =
    {
        getHtml: function () {
            var html = '<table class="ms-formtable" style="margin-top: 8px; min-width:720px;" border="0" cellpadding="0" cellspacing="0" width="100%">'
                + ' <tbody>'
                + '   <tr>'
                + '       <td nowrap="true" valign="top" class="ms-formlabel"  width="15%">'
                + '           <span class="ms-h3 ms-standardheader">'
                + '		            <nobr>{{Requester_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '           </span>'
                + '       </td>'
                + '        <td valign="top" class="ms-formbody"  width="33%">'
                + '           <div id="Custom_Requester" >{{Requester}}<span id="CustomError_Requester" class="ms-formvalidation ms-csrformvalidation"><span role="alert"></span></div>'
                + '       </td>'
                + '       <td nowrap="true" valign="top" class="ms-formlabel custom-table-cell" width="15%" {{hiddenDepartmentName}}>'
                + '           <span class="ms-h3 ms-standardheader">'
                + '            <nobr><span id="DepartmentName-Title">{{DepartmentName_Title}}</span><span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody"  width="33%">'
                + '            <div id="Custom_DepartmentName" >{{DepartmentName}}<div>'
                + '       </td>'
                + '   </tr>'
                + '  <tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr>{{Date_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '       <td valign="top" class="ms-formbody">'
                + '           <div id="Custom_Date"> {{Date}}<span id="CustomError_CommonDate" class="ms-formvalidation ms-csrformvalidation"><span role="alert"></span></span></div>'
                + '        </td>'
                + '    </tr>'
                + '    <tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr>{{From_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody ">'
                + '            <div id="Custom_From">{{From}}</div>'
                + '        </td>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel custom-table-cell">'
                + '           <span class="ms-h3 ms-standardheader">'
                + '            <nobr> {{To_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody">'
                + '            <div id="Custom_To">{{To}}</div>'
                + '        </td>'
                + '    </tr>'
                + '    <tr>'
                + '       <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr>{{HoursPerDay_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody">'
                + '            <div id="Custom_HoursPerDay">{{HoursPerDay}}<div>'
                + '        </td>'
                + '    </tr>'
                + '    <tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr> {{HeadOfDepartment_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody custom-input">'
                + '            <div id="Custom_Apporver1">{{HeadOfDepartment}}</div>'
                + '        </td>'
                + '    </tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '		<nobr>{{Reason_Title}}</nobr>'
                + '	    </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody" colspan="3">'
                + '            {{Reason}}'
                + '        </td>'
                + '    </tr>'
                + ' <tr>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel" id="departmentnameTitleTd">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{DepartmentID_Title}}</nobr>'
                + '	        </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="DepartmentSelect">{{DepartmentID}}</div>'
                + '     </td>'
                + ' </tr>'
                + ' <tr style="display: none;">'
                + '     <td nowrap="true" valign="top" class="ms-formlabel">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{LocationID_Title}}</nobr>'
                + '	        </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="Custom_LocationSelect">{{LocationID}}</div>'
                + '     </td>'
                + ' </tr>'
                + "<tr>"
                + "<td colspan='4'><div style='float: right;'><input  type='button' value='{{SaveButton}}' onclick=\"Submit('{{FormId}}')\" id='save-button' style='margin-left:0' class='ms-ButtonHeightWidth' > <input id='cancel-button' class='ms-ButtonHeightWidth' type='button' value='{{CancelButton}}' onclick=\"Cancel()\">   <input class='ms-ButtonHeightWidth' type='button' value='{{CloseButton}}' onclick=\"Cancel()\" style='display:none' id='close-button'></div></td>"
                + "</tr>"
                + '</tbody>'
                + '</table>';
            return html;
        },
        getHtmlDisplayForm: function () {
            var html = '<table class="ms-formtable" style="margin-top: 8px; min-width:720px;" border="0" cellpadding="0" cellspacing="0" width="100%">'
                + ' <tbody>'
                + '   <tr>'
                + '       <td nowrap="true" valign="top" class="ms-formlabel"  width="15%">'
                + '           <span class="ms-h3 ms-standardheader">'
                + '		            <nobr>{{Requester_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '           </span>'
                + '       </td>'
                + '        <td valign="top" class="ms-formbody"  width="33%">'
                + '           <div id="Custom_Requester" >{{Requester}}<span id="CustomError_Requester" class="ms-formvalidation ms-csrformvalidation"><span role="alert"></span></div>'
                + '       </td>'
                + '       <td nowrap="true" valign="top" class="ms-formlabel custom-table-cell" width="15%" {{hiddenDepartmentName}}>'
                + '           <span class="ms-h3 ms-standardheader">'
                + '            <nobr><span id="DepartmentName-Title">{{DepartmentName_Title}}</span><span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody"  width="33%">'
                + '            <div id="Custom_DepartmentName" >{{DepartmentName}}<div>'
                + '       </td>'
                + '   </tr>'
                + '  <tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr>{{Date_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '       <td valign="top" class="ms-formbody">'
                + '           <div id="Custom_Date"> {{Date}}<span id="CustomError_CommonDate" class="ms-formvalidation ms-csrformvalidation"><span role="alert"></span></span></div>'
                + '        </td>'
                + '    </tr>'
                + '    <tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr>{{From_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody ">'
                + '            <div id="Custom_From">{{From}}</div>'
                + '        </td>'
                + '    </tr>'
                + '    <tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '           <span class="ms-h3 ms-standardheader">'
                + '            <nobr> {{To_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody">'
                + '            <div id="Custom_To">{{To}}</div>'
                + '        </td>'
                + '    </tr>'
                + '    <tr>'
                + '       <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr>{{HoursPerDay_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody">'
                + '            <div id="Custom_HoursPerDay">{{HoursPerDay}}<div>'
                + '        </td>'
                + '    </tr>'
                + '    <tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '            <nobr> {{HeadOfDepartment_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '            </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody custom-input">'
                + '            <div id="Custom_Apporver1">{{HeadOfDepartment}}</div>'
                + '        </td>'
                + '    </tr>'
                + '        <td nowrap="true" valign="top" class="ms-formlabel">'
                + '            <span class="ms-h3 ms-standardheader">'
                + '		<nobr>{{Reason_Title}}</nobr>'
                + '	    </span>'
                + '        </td>'
                + '        <td valign="top" class="ms-formbody" colspan="3">'
                + '            {{Reason}}'
                + '        </td>'
                + '    </tr>'
                + ' <tr>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel" id="departmentnameTitleTd">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{DepartmentID_Title}}</nobr>'
                + '	        </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="DepartmentSelect">{{DepartmentID}}</div>'
                + '     </td>'
                + ' </tr>'
                + ' <tr style="display: none;">'
                + '     <td nowrap="true" valign="top" class="ms-formlabel">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{LocationID_Title}}</nobr>'
                + '	        </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="Custom_LocationSelect">{{LocationID}}</div>'
                + '     </td>'
                + ' </tr>'
                + "<tr>"
                + "<td colspan='4'><div style='float: right;'><input  type='button' value='{{SaveButton}}' onclick=\"Submit('{{FormId}}')\" id='save-button' style='margin-left:0' class='ms-ButtonHeightWidth' > <input id='cancel-button' class='ms-ButtonHeightWidth' type='button' value='{{CancelButton}}' onclick=\"Cancel()\">   <input class='ms-ButtonHeightWidth' type='button' value='{{CloseButton}}' onclick=\"Cancel()\" style='display:none' id='close-button'></div></td>"
                + "</tr>"
                + '</tbody>'
                + '</table>';
            return html;
        }
    }
function checkIsManager(employeeId) {

    var url = _spPageContextInfo.webAbsoluteUrl + String(NotOvertimeForm.IsManagerServiceUrl) + employeeId;
    var d = $.Deferred();
    $.ajax({
        url: url,
        type: "get",
        async: false,
        success: function (data) {
            NotOvertimeForm.IsManager = data;
            d.resolve(data);
        },
        error: function () {
            return false;
        }
    });
    return d.promise();
}
