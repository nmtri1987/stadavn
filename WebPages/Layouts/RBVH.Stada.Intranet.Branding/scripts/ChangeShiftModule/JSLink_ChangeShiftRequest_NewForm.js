var ChangeShiftRequestForm = {
    SaveButton: "Save",
    CancelButton: "Cancel",
    CloseButton: "Close",
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages",
    RequiredField: "This is a required field.",
    DepartmentName: "Department Name",
    DepartmentNameRequiredField: "Department Name Required Field",
    DontHaveShift: "You don't have any shift in selected date",
    DepartmentId: "",
    EmployeeID: "",
    ViewOnly: false,
    FromDateErrorMessage_1: 'From Date must be greater than current date {0} day(s)',
    ShiftsEqualErrorMessage: 'To shift must be different with from shift',
    IsManagerServiceUrl: "/_vti_bin/Services/Employee/EmployeeService.svc/IsManager/",
    IsChangeShifExistServiceUrl: "/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/IsChangeShiftExist/",
    GetConfigurations: '//{0}/_vti_bin/Services/Configurations/ConfigurationsService.svc/GetConfigurations',
    ChangeShiftItemExist: "This date has already been requested",
    IsManager: true,
    IsHasShift: false,
    IsTextOnly: false,
    ConfigKey_ValidFromDate: 'ChangeShiftForm_ValidFromDate',
    Configurations: {},
    DiffDays: 0,
};
(function () {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        var pm = SP.Ribbon.PageManager.get_instance();
        pm.add_ribbonInited(function () {
            HideEditRibbon();
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
            HideEditRibbon();
        }

        if (ChangeShiftRequestForm.IsTextOnly) {
            $('#s4-ribbonrow').hide();
        }
    }, "sp.ribbon.js");

    var changeShiftRequestFormContext = {};
    changeShiftRequestFormContext.Templates = {};
    changeShiftRequestFormContext.OnPostRender = ChangeShiftRequestFormOnPostRender;
    changeShiftRequestFormContext.Templates.View = ChangeShiftRequestView;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(changeShiftRequestFormContext);
})();
function HideEditRibbon() {
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
function OnPageResourcesReady() {
    $("#save-button").val(Res.saveButton);
    $("#cancel-button").val(Res.cancelButton);
    $("#DepartmentName-Title").text(Res.employeeDepartmentName);
    ChangeShiftRequestForm.RequiredField = Res.requiredField;
    ChangeShiftRequestForm.DepartmentNameRequiredField = Res.departmentNameRequired;
    ChangeShiftRequestForm.ChangeShiftItemExist = Res.changeShift_ItemExist;
    $("span .ms-accentText").prop("title", Res.requiredField);
    $("#departmentName").prop("title", Res.departmentNameRequired);
    ChangeShiftRequestForm.DontHaveShift = Res.changeShift_DontHaveShift;
}
function OnListResourcesReady() {
    ChangeShiftRequestForm.FromDateErrorMessage_1 = decodeURI(Res.fromDateGeqTodateErrorMessage_1);
    ChangeShiftRequestForm.ShiftsEqualErrorMessage = Res.changeShiftList_ShiftsEqualErrorMessage;
}
function GetParameterByName(name) {
    url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
function ChangeShiftRequestFormOnPostRender(ctx) {
    var paramTextOnly = GetParameterByName('TextOnly');
    if (paramTextOnly) {
        ChangeShiftRequestForm.IsTextOnly = true;
    }
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(ChangeShiftRequestForm.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ChangeShiftRequestForm.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ChangeShiftRequestForm.PageResourceFileName, "Res", OnPageResourcesReady);
        SP.SOD.registerSod(ChangeShiftRequestForm.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ChangeShiftRequestForm.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ChangeShiftRequestForm.ListResourceFileName, "Res", OnListResourcesReady);

        if (ctx.BaseViewID == "DisplayForm") {
            $('#save-button').hide();
            if (ChangeShiftRequestForm.IsTextOnly) {
                $('#close-button').hide();
            }
            else {
                $('#close-button').show();
            }
            $('#cancel-button').hide();
            ChangeShiftRequestForm.ViewOnly = true;
            //remove from shift link
            var fromShiftText = $("#Custom_FromShift a").text();
            $("#Custom_FromShift a").remove();
            $("#Custom_FromShift").html(fromShiftText);
            //remove to shift link
            var toShiftText = $("#Custom_ToShift a").text();
            $("#Custom_ToShift a").remove();
            $("#Custom_ToShift").html(toShiftText);
        }
        else {
            $(".commomcomment").hide();
            $("#Custom_ToShift select option").each(function () {
                if ($(this).val() == 7 || $(this).val() == 8 || $(this).val() == 9 || $(this).val() == 10) {
                    $(this).remove();
                }
            });
        }
        //Remove br
        $("#Custom_FromShift select").html("<option selected></option>");
        $("#Custom_Requester br").remove();
        $("#Custom_FromShift select").attr("disabled", true);
        LoadEmployeeData();
        DisableControl();
        GetConfigurations();

        var fromDateObj = Functions.parseVietNameseDate($(CustomFormControl.ChangeShiftFromSelect).val());
        try {
            var configVal = Functions.getConfigValue(ChangeShiftRequestForm.Configurations, ChangeShiftRequestForm.ConfigKey_ValidFromDate);
            if (configVal) {
                ChangeShiftRequestForm.DiffDays = parseInt(configVal);
            }
            else {
                ChangeShiftRequestForm.DiffDays = 0;
            }
        }
        catch (err) { ChangeShiftRequestForm.DiffDays = 0; }
        fromDateObj.setDate(fromDateObj.getDate() + ChangeShiftRequestForm.DiffDays);
        $(CustomFormControl.ChangeShiftFromSelect).val(Functions.parseVietnameseDateTimeToDDMMYYYY2(fromDateObj));
        $(CustomFormControl.ChangeShiftToSelect).val(Functions.parseVietnameseDateTimeToDDMMYYYY2(fromDateObj));

        // TFS #2017: [23.02.2018][Đổi ca] "Hành chánh QLK" bị dư -> cần ẩn đi
        $("#Custom_ToShift select").find("option:contains('QL Kho')").remove();

        $(CustomFormControl.ChangeShiftFromSelect).get(0).onvaluesetfrompicker = function (resultfield) {
            var date = $(CustomFormControl.ChangeShiftFromSelect).val();
            $("#Custom_To input").val(date);
            GetShiftTimeFrom(Functions.parseVietNameseDate($(CustomFormControl.ChangeShiftFromSelect).val()), ChangeShiftRequestForm.DepartmentId, ChangeShiftRequestForm.LocationId, ChangeShiftRequestForm.employeeId);
            $("#Custom_From_Error").html("");
            ChangeShiftRequestForm.IsHasShift = false;
        };
    }, "strings.js", "sp.js");
    $("#DeltaPlaceHolderMain").addClass('border-container');
}
function DisableControl() {
    $(CustomFormControl.DepartmentNameInput).prop('disabled', true);
    $(CustomFormControl.RequesterSelect).prop('disabled', true);
    $(CustomFormControl.ChangeShiftToSelect).prop('disabled', true);
    $(CustomFormControl.ChangeShiftToIcon).hide();
}
function GetConfigurations() {
    var postData = [ChangeShiftRequestForm.ConfigKey_ValidFromDate];
    var url = RBVH.Stada.WebPages.Utilities.String.format(ChangeShiftRequestForm.GetConfigurations, location.host);
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
            ChangeShiftRequestForm.Configurations = response;
        }
    });
}
function LoadEmployeeData() {
    var getCurrentUserServiceURL = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Employee/EmployeeService.svc/GetCurrentUser";
    var loadDataPromise = $.ajax({
        type: "GET",
        url: getCurrentUserServiceURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    });

    loadDataPromise.then(function (data) {
        if (data != null) {
            LoadApprovers(data.ID);
            AppendDataToControl(data)
        }
        else {
            $(CustomFormControl.RequesterError).html(NotOvertimeForm.CannotLoadCurrentUserMessage);
        }
    },
        function () {
        });
}
function AppendDataToControl(data) {
    if (data.Department != null) {
        GetDepartmentName(data.Department.LookupId, data.Department.LookupValue);
        ChangeShiftRequestForm.DepartmentId = data.Department.LookupId;
        $(CustomFormControl.DepartmentSelectHidden).val(data.Department.LookupId).change();
        ChangeShiftRequestForm.LocationId = data.Location.LookupId;
        $(CustomFormControl.LocationSelectHidden).val(data.Location.LookupId).change();
    }

    if (data.ID != undefined && data.ID > 0) {
        $(CustomFormControl.RequesterSelect).val(data.ID).change();
        ChangeShiftRequestForm.employeeId = data.ID;
    }

    var date = Functions.parseVietNameseDate($(CustomFormControl.ChangeShiftFromSelect).val());
    GetShiftTimeFrom(date, data.Department.LookupId, data.Location.LookupId, data.ID);
}
function GetShiftTimeFrom(date, departmentId, locationId, employeeId) {
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    var getShiftTimeUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/ShiftTime/ShiftTimeService.svc/GetShiftTimeInDate/" + day + '/' + month + '/' + year + '/' + departmentId + '/' + locationId + '/' + employeeId;
    var loadDataPromise = $.ajax({
        type: "GET",
        url: getShiftTimeUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    });
    loadDataPromise.then(function (data) {
        $('#save-button').show();
        if (data.Id == 0) {
            $("#Custom_FromShift select").html("<option selected></option>");
            ChangeShiftRequestForm.IsHasShift = false;
        }
        else {
            $("#Custom_FromShift select").html("<option value='" + data.Id + "'>" + data.Name + "</option>");
            $("span[role='alert']").text("");
            ChangeShiftRequestForm.IsHasShift = true;
            if (data.Code == "P") {
                $('#save-button').hide();
            }
        }
    },
        function () {
        });
}
function GetDepartmentName(departmentId, departmentNameDefault) {
    var lcid = SP.Res.lcid;
    var getDepartmentURL = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentByIdLanguageCode/" + departmentId + "/" + lcid;

    var getDepartmentPromise = $.ajax({
        type: "GET",
        url: getDepartmentURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
    getDepartmentPromise.then(function (data) {
        if (data != null) {
            if (ChangeShiftRequestForm.ViewOnly)
                $(CustomFormControl.DepartmentNameLabel).html(data.DepartmentName);
            $(CustomFormControl.DepartmentNameInput).val(data.DepartmentName);
        }
        else {
            if (ChangeShiftRequestForm.ViewOnly)
                $(CustomFormControl.DepartmentNameLabel).html(departmentNameDefault);
            $(CustomFormControl.DepartmentNameInput).val(departmentNameDefault);
        }

    }, function () {
        if (ChangeShiftRequestForm.ViewOnly)
            $(CustomFormControl.DepartmentNameLabel).html(departmentNameDefault);
        $(CustomFormControl.DepartmentNameInput).val(departmentNameDefault);
    });
}
function LoadApprovers(currentEmployeeId) {
    var loadApproverURL = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeApprovers/" + currentEmployeeId;
    var loadapproversPromise = $.ajax({
        type: "GET",
        url: loadApproverURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
    loadapproversPromise.then(
        function (data) {
            if (data != null) {
                PopulateApprovers(data, currentEmployeeId);
            }
        },
        function () {
        });
}
function PopulateApprovers(data, currentEmployeeId) {
    var approverData = [];
    approverData.push(null);
    CheckIsManager(currentEmployeeId);
    var approver2LoginName = '';
    if (ChangeShiftRequestForm.IsManager) {
        approver2LoginName = data.Approver3 ? data.Approver3.LoginName : '';
    }
    else {
        approver2LoginName = data.Approver2 ? data.Approver2.LoginName : '';
    }
    approverData.push({ InternalFieldName: "CommonApprover1", FullLoginUserName: approver2LoginName });
    ExecuteOrDelayUntilScriptLoaded(function () {
        var control = $(CustomFormControl.ApproverCustom);
        Functions.populateApprovertoPeoplePicker(approverData, currentEmployeeId, control);
    }, 'clientpeoplepicker.js');
}

var CustomFormControl =
    {
        DepartmentNameInput: "#Custom_DepartmentName input",
        DepartmentNameLabel: "#Custom_DepartmentName #lblDepartmentName",
        RequesterSelect: "#Custom_Requester select",
        RequesterError: "#CustomError_Requester",
        ChangeShiftFromSelect: "#Custom_From input",
        ChangeShiftToSelect: "#Custom_To input",
        ChangeShiftToIcon: "#Custom_To a",
        ApproverCustom: "#DPH",
        DepartmentSelectHidden: "#Custom_DepartmentSelect select",
        LocationSelectHidden: "#Custom_LocationSelect select",
    }

function Submit(formId) {
    if (ValidateForm()) {
        $("#Custom_From_Error").html("");
        $("#Custom_ToShift_Error").html("");

        IsChangeShiftExist().then(function (result) {
            if (result) {
                $("#Custom_From_Error").html(ChangeShiftRequestForm.ChangeShiftItemExist);
            } else {

                var isSuccess = SPClientForms.ClientFormManager.SubmitClientForm(formId);
                $("#save-button").prop("disabled", isSuccess);
            }
        })
    }
}

function ValidateForm() {
    if (!ValidateDates()) {
        $("#Custom_From_Error").html(RBVH.Stada.WebPages.Utilities.String.format(ChangeShiftRequestForm.FromDateErrorMessage_1, ChangeShiftRequestForm.DiffDays));
        return false;
    }
    if (!ValidateShifts()) {
        $("#Custom_ToShift_Error").html(ChangeShiftRequestForm.ShiftsEqualErrorMessage);
        return false;
    }
    if (!ChangeShiftRequestForm.IsHasShift) {
        $("span[role='alert']").text("");
        $("#Custom_From_Error").html(ChangeShiftRequestForm.DontHaveShift);
        return false;
    }
    $("#Custom_From_Error").html();
    $("#Custom_ToShift_Error").html();
    return true;
}

function ValidateShifts() {
    return $("#Custom_FromShift select").val() != $("#Custom_ToShift select").val();
}

function ValidateDates() {
    if ($("#Custom_To input").val() == '') {
        return false;
    }

    var fromDate = Functions.parseVietNameseDate($(CustomFormControl.ChangeShiftFromSelect).val());
    var toDate = Functions.parseVietNameseDate($("#Custom_To input").val());

    GetConfigurations();
    try {
        var configVal = Functions.getConfigValue(ChangeShiftRequestForm.Configurations, ChangeShiftRequestForm.ConfigKey_ValidFromDate);
        if (configVal) {
            ChangeShiftRequestForm.DiffDays = parseInt(configVal);
        }
        else {
            ChangeShiftRequestForm.DiffDays = 0;
        }
    }
    catch (err) { ChangeShiftRequestForm.DiffDays = 0; }

    var nowDate = new Date();
    nowDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
    var minDateObj = nowDate.setDate(nowDate.getDate() + ChangeShiftRequestForm.DiffDays);

    return (fromDate.valueOf() == toDate.valueOf() && fromDate.valueOf() >= minDateObj.valueOf());
}

function Cancel() {
    var sourcePath = Functions.getParameterByName("Source");
    if (typeof sourcePath == 'undefined' || sourcePath.length == 0) {
        sourcePath = _spPageContextInfo.listUrl;
    }
    window.location.href = sourcePath;
}

function ChangeShiftRequestView(ctx) {
    var formTable = ChangeShiftRequestForms.getHtml();
    formTable = formTable.replace("{{Requester_Title}}", Functions.getSPFieldTitle(ctx, "Requester"));
    formTable = formTable.replace("{{DepartmentName_Title}}", ChangeShiftRequestForm.DepartmentName);
    formTable = formTable.replace("{{FromShift_Title}}", Functions.getSPFieldTitle(ctx, "FromShift"));
    formTable = formTable.replace("{{From_Title}}", Functions.getSPFieldTitle(ctx, "CommonFrom"));
    formTable = formTable.replace("{{ToShift_Title}}", Functions.getSPFieldTitle(ctx, "ToShift"));
    formTable = formTable.replace("{{To_Title}}", Functions.getSPFieldTitle(ctx, "To"));
    formTable = formTable.replace("{{Reason_Title}}", Functions.getSPFieldTitle(ctx, "Reason"));
    formTable = formTable.replace("{{BOD_Title}}", Functions.getSPFieldTitle(ctx, "CommonApprover2"));
    formTable = formTable.replace("{{DPH_Title}}", Functions.getSPFieldTitle(ctx, "CommonApprover1"));
    formTable = formTable.replace("{{DepartmentID_Title}}", Functions.getSPFieldTitle(ctx, "CommonDepartment"));
    formTable = formTable.replace("{{LocationID_Title}}", Functions.getSPFieldTitle(ctx, "CommonLocation"));
    formTable = formTable.replace("{{CommonComment_Title}}", Functions.getSPFieldTitle(ctx, "CommonComment"));
    formTable = formTable.replace("{{Requester}}", Functions.getSPFieldRender(ctx, "Requester"));

    var departmentHTML = '<span dir="none">'
        + '  <input type="text" value="" maxlength="255" id="departmentName" title="{{RequiredFieldDepartment}}" style="ime-mode : " class="ms-long ms-spellcheck-true" />'
        + '</span>';

    if (ctx.BaseViewID == "DisplayForm") {
        departmentHTML = '<span id="lblDepartmentName"></span>';
    }

    formTable = formTable.replace("{{DepartmentName}}", departmentHTML); // TODO
    formTable = formTable.replace("{{FromShift}}", Functions.getSPFieldRender(ctx, "FromShift"));
    formTable = formTable.replace("{{From}}", Functions.getSPFieldRender(ctx, "CommonFrom"));
    formTable = formTable.replace("{{ToShift}}", Functions.getSPFieldRender(ctx, "ToShift"));
    formTable = formTable.replace("{{To}}", Functions.getSPFieldRender(ctx, "To"));
    formTable = formTable.replace("{{Reason}}", Functions.getSPFieldRender(ctx, "Reason"));
    formTable = formTable.replace("{{DPH}}", Functions.getSPFieldRender(ctx, "CommonApprover1"));
    formTable = formTable.replace("{{DepartmentID}}", Functions.getSPFieldRender(ctx, "CommonDepartment"));
    formTable = formTable.replace("{{LocationID}}", Functions.getSPFieldRender(ctx, "CommonLocation"));
    formTable = formTable.replace("{{CommonComment}}", Functions.getSPFieldRender(ctx, "CommonComment"));

    formTable = formTable.replace("{{SaveButton}}", ChangeShiftRequestForm.SaveButton);
    formTable = formTable.replace("{{CancelButton}}", ChangeShiftRequestForm.CancelButton);
    formTable = formTable.replace("{{CloseButton}}", ChangeShiftRequestForm.CloseButton);

    formTable = formTable.replace(/{{RequiredFieldText}}/gi, ChangeShiftRequestForm.RequiredField);
    formTable = formTable.replace("{{RequiredFieldDepartment}}", ChangeShiftRequestForm.DepartmentNameRequiredField);

    formTable = formTable.replace("{{FormId}}", ctx.FormUniqueId);
    return formTable;
}

var ChangeShiftRequestForms =
    {
        getHtml: function () {
            var html = '<table class="ms-formtable" style="margin-top: 8px; min-width:720px;" border="0" cellpadding="0" cellspacing="0" width="100%">'
                + '<tbody>'
                + ' <tr>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel" width="15%">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{Requester_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '         </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody"  width="33%">'
                + '         <div id="Custom_Requester" >{{Requester}}<span id="CustomError_Requester" class="ms-formvalidation ms-csrformvalidation"><span role="alert"></span></div>'
                + '     </td>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel custom-table-cell"  width="15%">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr><span id="DepartmentName-Title">{{DepartmentName_Title}}</span><span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '         </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody"  width="33%">'
                + '         <div id="Custom_DepartmentName" >{{DepartmentName}}<div>'
                + '     </td>'
                + ' </tr>'
                + ' <tr>'

                + '     <td nowrap="true" valign="top" class="ms-formlabel">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '             <nobr> {{From_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '         </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="Custom_From">{{From}}</div>'
                + '         <div id="Custom_From_Error" style="color:#bf0000;"></div>'
                + '     </td>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel custom-table-cell">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '             <nobr>{{FromShift_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '         </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody ">'
                + '         <div id="Custom_FromShift">{{FromShift}}</div>'
                + '     </td>'
                + ' </tr>'
                + ' <tr>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '             <nobr> {{To_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '         </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="Custom_To">{{To}}</div>'
                + '         <div id="Custom_To_Error" style="color:#bf0000;"></div>'
                + '     </td>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel custom-table-cell">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '             <nobr>{{ToShift_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"> *</span></nobr>'
                + '         </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="Custom_ToShift">{{ToShift}}</div>'
                + '         <div id="Custom_ToShift_Error" style="color:#bf0000;"></div>'
                + '     </td>'
                + ' </tr>'
                + ' <tr>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '             <nobr> {{DPH_Title}}<span class="ms-accentText" title="{{RequiredFieldText}}"></span></nobr>'
                + '         </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="DPH">{{DPH}}</div>'
                + '     </td>'
                + ' </tr>'

                + ' <tr>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{Reason_Title}}</nobr>'
                + '	        </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody" colspan="3">'
                + '         {{Reason}}'
                + '     </td>'
                + ' </tr>'
                + ' <tr style="display: none;">'
                + '     <td nowrap="true" valign="top" class="ms-formlabel">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{DepartmentID_Title}}</nobr>'
                + '	        </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody">'
                + '         <div id="Custom_DepartmentSelect">{{DepartmentID}}</div>'
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
                + ' <tr>'
                + '     <td nowrap="true" valign="top" class="ms-formlabel commomcomment">'
                + '         <span class="ms-h3 ms-standardheader">'
                + '		        <nobr>{{CommonComment_Title}}</nobr>'
                + '	        </span>'
                + '     </td>'
                + '     <td valign="top" class="ms-formbody commomcomment">'
                + '         <div>{{CommonComment}}</div>'
                + '     </td>'
                + ' </tr>'
                + " <tr>"
                + "     <td colspan='4'><div style='float: right;'><input type='button' value='{{SaveButton}}' onclick=\"Submit('{{FormId}}')\" id='save-button' style='margin-left:0' class='ms-ButtonHeightWidth' > <input class='ms-ButtonHeightWidth' type='button' value='{{CancelButton}}' id='cancel-button' onclick=\"Cancel()\"> <input class='ms-ButtonHeightWidth' type='button' value='{{CloseButton}}' onclick=\"Cancel()\" style='display:none' id='close-button'> </div></td>"
                + " </tr>"
                + '</tbody>'
                + '</table>';
            return html;
        }
    }

function CheckIsManager(employeeId) {
    var url = _spPageContextInfo.webAbsoluteUrl + String(ChangeShiftRequestForm.IsManagerServiceUrl) + employeeId;
    var d = $.Deferred();
    $.ajax({
        url: url,
        type: "get",
        async: false,
        success: function (data) {
            ChangeShiftRequestForm.IsManager = data;
            d.resolve(data);
        },
        error: function () {
            return false;
        }
    });
    return d.promise();
}

function IsChangeShiftExist() {
    var fromDateString = Functions.parseVietNameseDate($(CustomFormControl.ChangeShiftFromSelect).val());
    var currentEmployeeLookupId = $(CustomFormControl.RequesterSelect + " option:selected").val();
    //parse date to MM-dd-yyyy
    var dateString = Functions.parseDateTimeToMMDDYYYY(fromDateString);
    var d = $.Deferred();
    var checkOvertimePromiseURL = _spPageContextInfo.webAbsoluteUrl + String(ChangeShiftRequestForm.IsChangeShifExistServiceUrl) + currentEmployeeLookupId + "/" + dateString;
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
            return true;
        }
    });
    return d.promise();
}

