var TransportationForm = {
    SaveButton: "Save",
    CancelButton: "Cancel",
    CloseButton: "Close",
    Comments: 'Comments',
    CommonCommentTitle: 'Your Comment',
    PrivateVehicle: "Private Vehicle",
    CompanyVehicle: "Company's Vehicle",
    EmployeeID: 'Employee ID',
    Department: 'Department',
    DepartmentId: "",
    ViewOnly: false,
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages",
    RequiredField: "This is a required field.",
    FromDateErrorMessage_1: 'From Date must be greater than current date {0} day(s)',
    IsManager: false,
    DelegatedTaskInfo: {},
    Locale: '',
    IsManagerServiceUrl: "/_vti_bin/Services/Employee/EmployeeService.svc/IsManager/",
    GetApprovalPermission: '//{0}/_vti_bin/Services/VehicleManagement/VehicleManagementService.svc/HasApprovalPermission/{1}',
    GetDelegatedTaskInfo: '//{0}/_vti_bin/Services/VehicleManagement/VehicleManagementService.svc/GetDelegatedTaskInfo/{1}',
    ApproveVehicleRequest: '//{0}/_vti_bin/Services/VehicleManagement/VehicleManagementService.svc/ApproveVehicle',
    RejectVehicleRequest: '//{0}/_vti_bin/Services/VehicleManagement/VehicleManagementService.svc/RejectVehicle',
    GetTaskHistoryInfo: '//{0}/_vti_bin/Services/VehicleManagement/VehicleManagementService.svc/GetTaskHistory/{1}/{2}',
    GetConfigurations: '//{0}/_vti_bin/Services/Configurations/ConfigurationsService.svc/GetConfigurations',
    Configurations: {},
    ConfigKey_ValidFromDate: "VehicleForm_ValidFromDate",
    DiffDays: 1,
    CantLeaveTheBlank: '',
    ApprovalStatusTitle: "",
    PostedByTitle: "",
    DateTitle: "",
    CommentTitle: "",
    NoDataAvaibleMsg: "",
    ApprovalStatus_Approved: "",
    ApprovalStatus_Rejected: "",
    ApprovalHistoryButton: "",
    RequestExpiredMsgFormat: "",
    IsRequestExpired: false,
    RequestDueDateStr: "",
};

var CustomTransportationFormControl = {
    DepartmentNameInput: "#Custom_Department input",
    EmployeIDSelect: "#Custom_EmployeeID select",
    RequesterError: "#CustomError_Requester",
    VehicleTypeSelect: '#vehicleType_select input[type="radio"]',
    CompanyPickUpSelect: "#CompanyPickUp select",
    EmployeeID: '#employeeID',
    DepartmentName: '#departmentName',
    DepartmentCustom: "#departmentCustom",
    DepartmentSelectHidden: "#Custom_DepartmentSelect select",
    LocationSelectHidden: "#Custom_LocationSelect select",
    OldCommentTextArea: "#oldcommentcontainer div",
    ApprovalStatusContainer: "#approvalstatuscontainer",
    CommentTextArea: "#commentcontainer textarea",
    ApprovalHistoryButtonSelector: "#loadApprovalHistory",
    ApprovalHistoryContainerSelector: "#approvalHistoryContainer",
    ErrorMsgContainerSelector: "#error-msg-container",
    ErrorMsgSelector: "#error-msg",
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
            if (typeof (_ribbonStartInit) === "function")
                _ribbonStartInit(_ribbon.initialTabId, false, null);
        }
        else {
            hideEditRibbon();
        }
    }, "sp.ribbon.js");

    var TransportationFormContext = {};
    TransportationFormContext.Templates = {};
    TransportationFormContext.OnPostRender = TransportationFormOnPostRender;
    TransportationFormContext.Templates.View = CustomNewTransportationForm;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(TransportationFormContext);
})();
function OnListResourcesReady() {
    TransportationForm.EmployeeID = Res.employeeInfo_EmployeeID
    TransportationForm.Department = Res.employeeInfo_Department;
    $('#oldcommentfieldname').text(Res.commonComment);
    $('#commentfieldname').text(Res.leaveList_YourComment);
    $('#vehicleType_select label:nth(0)').text(Res.vehicleManagement_VehicleType_Choice_Private);
    $('#vehicleType_select label:nth(1)').text(Res.vehicleManagement_VehicleType_Choice_Company);
    TransportationForm.FromDateErrorMessage_1 = Res.fromDateGeqTodateErrorMessage_1;
    TransportationForm.ApprovalStatus_Approved = Res.approvalStatus_Approved;
    TransportationForm.ApprovalStatus_Rejected = Res.approvalStatus_Rejected;
}
function OnPageResourcesReady() {
    $("#save-button").val(Res.saveButton);
    $("#cancel-button").val(Res.cancelButton);
    $("#approve-button").val(Res.approveButton);
    $("#reject-button").val(Res.rejectButton);
    TransportationForm.CantLeaveTheBlank = Res.cantLeaveTheBlank;
    TransportationForm.CannotLoadCurrentUserMessage = Res.notOvertime_CannotLoadCurrentUser;
    TransportationForm.ApprovalStatusTitle = Res.approvalHistory_ApprovalStatusTitle;
    TransportationForm.PostedByTitle = Res.approvalHistory_PostedByTitle;
    TransportationForm.DateTitle = Res.approvalHistory_DateTitle;
    TransportationForm.CommentTitle = Res.approvalHistory_CommentTitle;
    TransportationForm.NoDataAvaibleMsg = Res.approvalHistory_NoDataAvaibleMsg;
    TransportationForm.ApprovalHistoryButton = Res.approvalHistoryButton;
    TransportationForm.RequestExpiredMsgFormat = Res.requestExpiredMsgFormat;
    $(CustomTransportationFormControl.ApprovalHistoryButtonSelector).html('<span>' + TransportationForm.ApprovalHistoryButton + '</span>');

    //if (TransportationForm.IsRequestExpired === true) {
    //    errMsg = decodeURI(TransportationForm.RequestExpiredMsgFormat);
    //    errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, TransportationForm.RequestDueDateStr);
    //    RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(CustomTransportationFormControl.ErrorMsgContainerSelector, CustomTransportationFormControl.ErrorMsgSelector, errMsg);
    //}
}
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
function TransportationFormOnPostRender(ctx) {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        TransportationForm.Locale = Strings.STS.L_CurrentUICulture_Name;
        SP.SOD.registerSod(TransportationForm.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + TransportationForm.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(TransportationForm.ListResourceFileName, "Res", OnListResourcesReady);
        SP.SOD.registerSod(TransportationForm.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + TransportationForm.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(TransportationForm.PageResourceFileName, "Res", OnPageResourcesReady);
    }, "strings.js");

    $("#DeltaPlaceHolderMain").addClass('border-container');
    DisableControl(ctx);
    CheckApprovalPermission(ctx);
    LoadEmployeeData(ctx);
    GetConfigurations();
    InitTransportationForm();
}
function GetConfigurations() {
    var postData = [TransportationForm.ConfigKey_ValidFromDate];
    var url = RBVH.Stada.WebPages.Utilities.String.format(TransportationForm.GetConfigurations, location.host);
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
            TransportationForm.Configurations = response;
        }
    });
}
function DisableControl(ctx) {
    if (ctx.BaseViewID === "DisplayForm" || ctx.BaseViewID === "EditForm") {
        $('#save-button').hide();
        $(CustomTransportationFormControl.ApprovalStatusContainer).show();
        if (ctx.BaseViewID === "EditForm") {
            $("td.ms-dtinput > input[id$='DateTimeFieldDate']").attr('readonly', 'readonly');
            $("td.ms-dtinput > a").attr('onclick', '').unbind('click');
            $("td.ms-dttimeinput > select[id$='DateTimeFieldDateHours']").attr('disabled', 'disabled');
            $("td.ms-dttimeinput > select[id$='DateTimeFieldDateMinutes']").attr('disabled', 'disabled');

            $("textarea[id^='Reason']").prop('disabled', true);
            $("input[name^='VehicleType']").prop('disabled', true);
        }
    }

    $(CustomTransportationFormControl.EmployeIDSelect).prop('disabled', true);
    $(CustomTransportationFormControl.DepartmentSelectHidden).prop('disabled', true);
    $(CustomTransportationFormControl.CommentTextArea).prop('disabled', true);
}
function CheckApprovalPermission(ctx) {
    var vehicleId = RBVH.Stada.WebPages.Utilities.GetValueByParam('ID');
    if (ctx.ListData && ctx.ListData.Items && ctx.ListData.Items.length > 0) {
        var commonReqDueDate = ctx.ListData.Items[0]["CommonReqDueDate"];
        if (commonReqDueDate && commonReqDueDate.length > 0 && commonReqDueDate.indexOf(' ') > 0) {
            TransportationForm.RequestDueDateStr = commonReqDueDate.split(' ')[0];
            var requestDueDateObj = Functions.parseVietNameseDate(TransportationForm.RequestDueDateStr);
            var nowDate = new Date();
            var currentDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
            if (requestDueDateObj.valueOf() > currentDate.valueOf()) {
                //TransportationForm.IsRequestExpired = true;
                TransportationForm.IsRequestExpired = false;
            }
        }
    }

    if (vehicleId && vehicleId > 0) {
        GetApprovalPermission(vehicleId)
            .then(function (result) {
                if (result && result === true) {
                    if (TransportationForm.IsRequestExpired === false) {
                        ShowControlForApprover(true);
                    }
                }
                else {
                    GetDelegatedTaskInfo(vehicleId)
                        .then(function (respData) {
                            if (respData && respData.Requester.LookupId > 0) {
                                if (TransportationForm.IsRequestExpired === false) {
                                    TransportationForm.DelegatedTaskInfo = respData;
                                    ShowControlForApprover(true);
                                }
                            }
                            else {
                                ShowControlForApprover(false);
                            }
                        });
                }
            });
    }
}
function GetApprovalPermission(itemId) {
    var url = RBVH.Stada.WebPages.Utilities.String.format(TransportationForm.GetApprovalPermission, location.host, itemId);
    return $.ajax({
        type: "GET",
        url: url,
        cache: false,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    });
}
function GetDelegatedTaskInfo(itemId) {
    var url = RBVH.Stada.WebPages.Utilities.String.format(TransportationForm.GetDelegatedTaskInfo, location.host, itemId);
    return $.ajax({
        type: "GET",
        url: url,
        cache: false,
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    });
}
function ShowControlForApprover(hasApprovalPermission) {
    if (hasApprovalPermission === true) {
        $('#approve-button').show();
        $('#reject-button').show();
        $('#commentcontainer').show();
        $(CustomTransportationFormControl.CommentTextArea).prop('disabled', false);
    }
}
function LoadEmployeeData(ctx) {
    var vehicleId = RBVH.Stada.WebPages.Utilities.GetValueByParam('ID');
    if (vehicleId && vehicleId > 0) {
        var deptValue = ctx.ListData.Items[0]["CommonDepartment"];
        GetDepartmentName(deptValue.split(";#")[0], deptValue.split("#")[1])
    }
    else {
        $(CustomTransportationFormControl.ApprovalHistoryButtonSelector).hide();
        var getCurrentUserServiceURL = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Employee/EmployeeService.svc/GetCurrentUser";
        var loadDataPromise = $.ajax({
            type: "GET",
            url: getCurrentUserServiceURL,
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });

        loadDataPromise.then(function (data) {
            if (data !== null) {
                AppendDataToControl(data)
            }
            else {
                $(CustomTransportationFormControl.RequesterError).html(TransportationForm.CannotLoadCurrentUserMessage);
            }
        },
        function () {
            //console.log("Can not load current employee");
        });
    }
}
function AppendDataToControl(data) {
    if (data.Department !== null) {
        GetDepartmentName(data.Department.LookupId, data.Department.LookupValue);
        TransportationForm.DepartmentId = data.Department.LookupId;
        $(CustomTransportationFormControl.DepartmentSelectHidden).val(data.Department.LookupId).change();
        $(CustomTransportationFormControl.LocationSelectHidden).val(data.Location.LookupId).change();
    }

    if (data.ID !== undefined && data.ID > 0) {
        $(CustomTransportationFormControl.EmployeIDSelect).val(data.ID).change();
        LoadApprovers(data.ID);
    }
}
function GetDepartmentName(departmentId, departmentNameDefault) {
    var getDepartmentURL = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentByIdLanguageCode/" + departmentId + "/" + TransportationForm.Locale;
    var getDepartmentPromise = $.ajax({
        type: "GET",
        url: getDepartmentURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
    getDepartmentPromise.then(function (data) {
        if (data !== null) {
            $('select[id^="CommonDepartment"] option:selected').text(data.DepartmentName);
        }
    }, function () {
    });
}
function InitTransportationForm() {
    var vehicleId = RBVH.Stada.WebPages.Utilities.GetValueByParam('ID');
    if (vehicleId ===null || vehicleId === undefined) {
        var fromDateObj = Functions.parseVietNameseDate($("#TransportationForm_FromDate input").val());
        try {
            var configVal = Functions.getConfigValue(TransportationForm.Configurations, TransportationForm.ConfigKey_ValidFromDate);
            if (configVal)
            {
                TransportationForm.DiffDays = parseInt(configVal);
            }
        }
        catch (err) { TransportationForm.DiffDays = 1; }
        fromDateObj.setDate(fromDateObj.getDate() + TransportationForm.DiffDays);

        $("#TransportationForm_FromDate input").val(Functions.parseVietnameseDateTimeToDDMMYYYY2(fromDateObj));
        $("#TransportationForm_ToDate input").val(Functions.parseVietnameseDateTimeToDDMMYYYY2(fromDateObj));
    }

    if (TransportationForm.PrivateVehicle === $(CustomTransportationFormControl.VehicleTypeSelect).val()) {
        $(CustomTransportationFormControl.CompanyPickUpSelect).prop('disabled', true);
    }
    else {
        $(CustomTransportationFormControl.CompanyPickUpSelect).prop('disabled', false);
    }
    $(CustomTransportationFormControl.VehicleTypeSelect).change(function () {
        if (TransportationForm.PrivateVehicle === $(this).val()) {
            $(CustomTransportationFormControl.CompanyPickUpSelect).prop('disabled', true).val(0).change();
        }
        else {
            $(CustomTransportationFormControl.CompanyPickUpSelect).prop('disabled', false);
            RemoveFirstElement();
        }
    });
    $('#approve-button').on('click', function (event) {
        event.preventDefault();
        Approve();
    });
    $('#reject-button').on('click', function (event) {
        event.preventDefault();
        Reject();
    });

    if ($(CustomTransportationFormControl.OldCommentTextArea).html().length > 0) {
        $('#oldcommentcontainer').show();
    }
    else {
        $('#oldcommentcontainer').hide();
    }

    $(CustomTransportationFormControl.ApprovalHistoryButtonSelector).on('click', function (event) {
        event.preventDefault();
        var itemId = RBVH.Stada.WebPages.Utilities.GetValueByParam('ID');
        var url = RBVH.Stada.WebPages.Utilities.String.format(TransportationForm.GetTaskHistoryInfo, location.host, itemId, 0);
        $.ajax({
            type: "GET",
            url: url,
            cache: false,
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (response) {
            var tableHeaders = [TransportationForm.ApprovalStatusTitle, TransportationForm.PostedByTitle, TransportationForm.DateTitle, TransportationForm.CommentTitle];
            var approvalStatus = [TransportationForm.ApprovalStatus_Approved, TransportationForm.ApprovalStatus_Rejected]
            var approvalHistoryTable = Functions.generateApprovalHistoryTable(response, tableHeaders, approvalStatus, TransportationForm.NoDataAvaibleMsg);
            $(CustomTransportationFormControl.ApprovalHistoryContainerSelector).html(approvalHistoryTable);
        });
    });
}
function RemoveFirstElement() {
    var companyPickupList = $(CustomTransportationFormControl.CompanyPickUpSelect).children();
    if (companyPickupList.length > 0) {
        var firstElement = companyPickupList.first();
        if (firstElement.length && firstElement.val() === '0') {
            firstElement.remove();

        }
        $(CustomTransportationFormControl.CompanyPickUpSelect).val($(CustomTransportationFormControl.CompanyPickUpSelect).children().first().val());
    }
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
            if (data !== null) {
                PopulateApprovers(data, currentEmployeeId)
            }
        },
        function () {
            //console.log("Can not load Approvers");
        });
}
function PopulateApprovers(data, currentEmployeeId) {
    var approverData = [];
    approverData.push(null);
    CheckIsManager(currentEmployeeId);
    var approver2LoginName = '';
    if (TransportationForm.IsManager) {
        approver2LoginName = data.Approver3 ? data.Approver3.LoginName : '';
    }
    else {
        approver2LoginName = data.Approver2 ? data.Approver2.LoginName : '';
    }
    approverData.push({ InternalFieldName: "CommonApprover1", FullLoginUserName: approver2LoginName });

    ExecuteOrDelayUntilScriptLoaded(function () {
        var control = $(CustomTransportationFormControl.CustomApprover);
        Functions.populateApprovertoPeoplePicker(approverData, currentEmployeeId, control);
    }, 'clientpeoplepicker.js');
}
function CheckIsManager(employeeId) {
    var url = _spPageContextInfo.webAbsoluteUrl + String(TransportationForm.IsManagerServiceUrl) + employeeId;
    var d = $.Deferred();
    $.ajax({
        url: url,
        type: "get",
        async: false,
        success: function (data) {
            TransportationForm.IsManager = data;
            d.resolve(data);
        },
        error: function () {
            return false;
        }
    });
    return d.promise();
}
function Submit(TransportationFormId) {
    $("#TransportationForm_FromDate_Msg").html("");

    var vehicleId = RBVH.Stada.WebPages.Utilities.GetValueByParam('ID');
    if (vehicleId && vehicleId > 0) {
        SPClientForms.ClientFormManager.SubmitClientForm(TransportationFormId);
    }
    else {
        if (ValidateDates() === true) {
            SPClientForms.ClientFormManager.SubmitClientForm(TransportationFormId);
        }
        else {
            var errMsg = decodeURI(TransportationForm.FromDateErrorMessage_1);
            errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, TransportationForm.DiffDays);
            $("#TransportationForm_FromDate_Msg").html(errMsg);
        }
    }
}
function Cancel() {
    Functions.redirectToSource();
}
function ValidateDates() {
    if ($("#TransportationForm_FromDate input").val() !== '') {
        GetConfigurations();

        var fromDate = Functions.parseVietNameseDate($("#TransportationForm_FromDate input").val());
        try {
            var configVal = Functions.getConfigValue(TransportationForm.Configurations, TransportationForm.ConfigKey_ValidFromDate);
            if (configVal) {
                TransportationForm.DiffDays = parseInt(configVal);
            }
        }
        catch (err) { TransportationForm.DiffDays = 1; }

        var nowDate = new Date();
        var minDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
        minDate.setDate(minDate.getDate() + TransportationForm.DiffDays);

        return (fromDate.valueOf() >= minDate.valueOf());
    }
    return false;
}
function Approve() {
    $('#approve-button').prop('disabled', true);
    ShowOrHideErrorMessage($('#commonComment_Error'), "");
    var postData = GetPostData();
    if (postData) {
        var url = RBVH.Stada.WebPages.Utilities.String.format(TransportationForm.ApproveVehicleRequest, location.host);
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response.Code === 0) {
                Functions.redirectToSource();
            }
            else if (response.Code === 2) {
                alert(response.Message);
                window.location.reload();
            }
        });
    }
    else {
        $('#approve-button').prop('disabled', false);
    }
}
function Reject() {
    $('#reject-button').prop('disabled', true);
    ShowOrHideErrorMessage($('#commonComment_Error'), "");
    var postData = GetPostData();
    if (postData) {
        if (postData && postData.Comment.length > 0) {
            var url = RBVH.Stada.WebPages.Utilities.String.format(TransportationForm.RejectVehicleRequest, location.host);
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response.Code === 0) {
                    Functions.redirectToSource();
                }
                else if (response.Code === 3) {
                    alert(response.Message);
                    window.location.reload();
                }
            });
        }
        else {
            ShowOrHideErrorMessage($('#commonComment_Error'), TransportationForm.CantLeaveTheBlank);
            $('#reject-button').prop('disabled', false);
        }
    }
    else {
        $('#reject-button').prop('disabled', false);
    }
}
function GetPostData() {
    var postData = {};

    var vehicleId = RBVH.Stada.WebPages.Utilities.GetValueByParam('ID');
    postData.Id = vehicleId || 0;
    postData.Comment = $(CustomTransportationFormControl.CommentTextArea).val();
    return postData;
}
function ShowOrHideErrorMessage(ctrl, msg) {
    if (msg && msg.length > 0) {
        $(ctrl).html(msg);
        $(ctrl).show();
    }
    else {
        $(ctrl).hide();
        $(ctrl).html("");
    }
}
function CustomNewTransportationForm(ctx) {
    var TransportationFormTable = TransportationForms.getNewFomHtml();
    if (ctx.BaseViewID === "DisplayForm") {
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Requester}}", Functions.getSPFieldLookupValue(ctx, "Requester"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Department}}", Functions.getSPFieldLookupValue(ctx, "CommonDepartment"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Location}}", Functions.getSPFieldLookupValue(ctx, "CommonLocation"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_HOD}}", Functions.getSPFieldLookupValue(ctx, "CommonApprover1"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_CompanyPickUp}}", Functions.getSPFieldLookupValue(ctx, "CompanyPickup"));
    }
    else {
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Requester}}", Functions.getSPFieldRender(ctx, "Requester"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Department}}", Functions.getSPFieldRender(ctx, "CommonDepartment"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Location}}", Functions.getSPFieldRender(ctx, "CommonLocation"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_HOD}}", Functions.getSPFieldRender(ctx, "CommonApprover1"));
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_CompanyPickUp}}", Functions.getSPFieldRender(ctx, "CompanyPickup"));
    }
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_FromDate}}", Functions.getSPFieldRender(ctx, "CommonFrom"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_ToDate}}", Functions.getSPFieldRender(ctx, "To"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Reason}}", Functions.getSPFieldRender(ctx, "Reason"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_VehicleType}}", Functions.getSPFieldRender(ctx, "VehicleType"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_CommonOldComment}}", Functions.parseComment(Functions.getSPFieldValue(ctx, "CommonComment")));

    var approvalStatusObj = RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(Functions.getSPFieldValue(ctx, "ApprovalStatus"));
    if (approvalStatusObj && approvalStatusObj.length > 0) {
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_ApprovalStatus}}", approvalStatusObj[0].outerHTML);
    }
    else {
        TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_ApprovalStatus}}", "");
    }

    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Requester_FieldName}}", Functions.getSPFieldTitle(ctx, "Requester"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_FromDate_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonFrom"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_ToDate_FieldName}}", Functions.getSPFieldTitle(ctx, "To"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Reason_FieldName}}", Functions.getSPFieldTitle(ctx, "Reason"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_CompanyPickUp_FieldName}}", Functions.getSPFieldTitle(ctx, "CompanyPickup"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_HOD_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonApprover1"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Department_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonDepartment"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_Location_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonLocation"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_VehicleType_FieldName}}", Functions.getSPFieldTitle(ctx, "VehicleType"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_CommonOldComment_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonComment"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_ApprovalStatus_FieldName}}", Functions.getSPFieldTitle(ctx, "ApprovalStatus"));
    TransportationFormTable = TransportationFormTable.replace("{{TransportationForm_CommonComment_FieldName}}", TransportationForm.CommonCommentTitle);

    TransportationFormTable = TransportationFormTable.replace("{{SaveButton}}", TransportationForm.SaveButton);
    TransportationFormTable = TransportationFormTable.replace("{{CancelButton}}", TransportationForm.CancelButton);

    TransportationFormTable = TransportationFormTable.replace("{{TransportationFormId}}", ctx.FormUniqueId);

    return TransportationFormTable;
}
var TransportationForms = {
    getNewFomHtml: function () {
        var html =
            "<table class='ms-TransportationFormtable' style='margin-top: 8px;' border='0' cellpadding='0' cellspacing='0' width='100%'>" +
            "<tbody>" +
            "<tr style='height: 50px;'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr>{{TransportationForm_Requester_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
            "</h3>" +
            " </td>" +
            "<td valign='top' width='350px' class='ms-TransportationFormbody'>" +
            "<div title='Requester' id='Custom_EmployeeID'>{{TransportationForm_Requester}}<span id='CustomError_Requester' class='ms-TransportationFormvalidation ms-csrTransportationFormvalidation'><span role='alert'></span></div>" +
            "</td>" +
            "</tr>" +
            "<tr style='height: 50px;'>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr>{{TransportationForm_Department_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
            "</h3>" +
            "</td>" +
            "<td valign='top' width='350px' class='ms-TransportationFormbody'>" +
            "<div title='Department' id='Custom_DepartmentSelect'>{{TransportationForm_Department}}<span id='CustomError_Department' class='ms-TransportationFormvalidation ms-csrTransportationFormvalidation'><span role='alert'></span></div>" +
            "</td>" +
            "</tr>" +
            // Location
            "<tr style='display: none;'>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr>{{TransportationForm_Location_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
            "</h3>" +
            "</td>" +
            "<td valign='top' width='350px' class='ms-TransportationFormbody'>" +
            "<div title='Location' id='Custom_LocationSelect'>{{TransportationForm_Location}}<span id='CustomError_Location' class='ms-TransportationFormvalidation ms-csrTransportationFormvalidation'><span role='alert'></span></div>" +
            "</td>" +
            "</tr>" +
            //---

            "<tr style='height: 50px;'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            " <h3 class='ms-standardheader'>" +
            " <nobr>{{TransportationForm_FromDate_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
            " </h3>" +
            "  </td>" +
            "<td valign='top' width='350px' class='ms-TransportationFormbody'>" +
            " <div id='TransportationForm_FromDate'>{{TransportationForm_FromDate}}</div>" +
            " <div id='TransportationForm_FromDate_Msg' style='color:#bf0000;'></div>" +
            " </td>" +
            "</tr>" +
            "<tr style='height: 50px;'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            " <nobr>{{TransportationForm_ToDate_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
            " </h3>" +
            "</td>" +
            " <td valign='top' width='350px' class='ms-TransportationFormbody'>" +
            "  <div id='TransportationForm_ToDate'>{{TransportationForm_ToDate}}</div>" +
            "</td>" +
            "</tr>" +
            "<tr style='height: 50px;'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr>{{TransportationForm_Reason_FieldName}}</nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' class='ms-formlabel'>" +
            " <nobr>{{TransportationForm_Reason}}</nobr>" +
            " </td>" +
            " </tr>" +
            "<tr style='height: 50px;'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            " <nobr>{{TransportationForm_VehicleType_FieldName}}</nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' id='vehicleType_select' class='ms-formlabel '>" +
            "<h3 class='ms-standardheader'>" +
            " <nobr>{{TransportationForm_VehicleType}}</nobr>" +
            " </h3>" +
            " </td>" +
            " </tr>" +
            "<tr style='height: 50px;'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            " <h3 class='ms-standardheader'>" +
            "   <nobr>{{TransportationForm_CompanyPickUp_FieldName}}</nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' id='CompanyPickUp' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            " <nobr>{{TransportationForm_CompanyPickUp}}</nobr>" +
            " </h3>" +
            " </td>" +
            " </tr>" +
            "<tr style='height: 50px; display:none;'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            " <h3 class='ms-standardheader'>" +
            "   <nobr>{{TransportationForm_HOD_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader' style='pointer-events: none;'>" +
            " <nobr>{{TransportationForm_HOD}}</nobr>" +
            " </h3>" +
            " </td>" +
            " </tr>" +
            "<tr style='height: 50px; display:none;' id='oldcommentcontainer'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr id='oldcommentfieldname'>{{TransportationForm_CommonOldComment_FieldName}}</nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' class='ms-formlabel'>" +
            " <div id='lblOldComment'>{{TransportationForm_CommonOldComment}}</div>" +
            " </td>" +
            " </tr>" +
            "<tr style='display:none;' id='approvalstatuscontainer'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr>{{TransportationForm_ApprovalStatus_FieldName}}</nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' class='ms-formlabel'>" +
            " <div>{{TransportationForm_ApprovalStatus}}</div>" +
            " </td>" +
            " </tr>" +
            "<tr style='display:none;' id='error-msg-container'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr></nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' class='ms-formlabel'>" +
            " <div id='error-msg'></div>" +
            " </td>" +
            " </tr>" +
            "<tr style='height: 50px; display:none;' id='commentcontainer'>" +
            "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
            "<h3 class='ms-standardheader'>" +
            "<nobr id='commentfieldname'>{{TransportationForm_CommonComment_FieldName}}</nobr>" +
            " </h3>" +
            "</td>" +
            "<td nowrap='true' valign='top' colspan='3' class='ms-formlabel'>" +
            " <nobr><span dir='none'><textarea disabled='' class='ms-long' id='txtCommonComment' rows='6' cols='20'></textarea><br></span><span id='commonComment_Error' class='ms-formvalidation ms-csrformvalidation' style='display:none;'></span></nobr>" +
            " </td>" +
            " </tr>" +
            "<tr>" +
                "<td colspan='4'><div style='float: right;margin: 5px;'>" +
                "<input  type='button' value='{{ApproveButton}}' id='approve-button' style='display:none;' class='ms-ButtonHeightWidth btn btn-success' ><input value='{{RejectButton}}' id='reject-button' style='display:none;' class='ms-ButtonHeightWidth btn btn-primary' type='button'>" +
                "<input  type='button' value='{{SaveButton}}' onclick=\"Submit('{{TransportationFormId}}')\" id='save-button' style='' class='ms-ButtonHeightWidth' > <input id='cancel-button' class='ms-ButtonHeightWidth' type='button' value='{{CancelButton}}' onclick=\"Cancel()\">   <input class='ms-ButtonHeightWidth' type='button' value='{{CloseButton}}' onclick=\"Close()\" style='display:none' id='close-button'></div></td>" +
            "</tr>" +
            "</tbody>" +
            "</table>" +
            '<div><button id="loadApprovalHistory" class="btn btn-default">' +
            '<span></span></button><br /><div id="approvalHistoryContainer"></div></div>';
        return html;
    }
}