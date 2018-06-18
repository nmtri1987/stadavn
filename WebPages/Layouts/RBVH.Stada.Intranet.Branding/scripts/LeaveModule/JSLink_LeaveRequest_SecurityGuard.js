var LeaveRequestSecurityConfig = {
    LaveManagement_Requester: "Requester",
    LeaveManagement_Department: "Department",
    LaveManagement_From: "From",
    LaveManagement_To: "To",
    LeaveManagement_Shift: "Shift",
    LeaveManagement_LeaveHours: "Leave Hours",
    LaveManagement_Reason: "Reason",
    LeaveManagement_LeftAt: "Left At",
    LeaveManagement_CheckOutBy: "Check Out By",
    LeaveManagement_EnterTime: "Time In",
    LeaveManagement_CheckInBy: "Check In By",
    LaveManagement_RequesterPhoto: "Photo",
    LaveManagement_ListTitle: 'Leave Management',
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages",
    DefaultAvatar: "<img src='/_layouts/15/RBVH.Stada.Intranet.Branding/images/DefaultAvatar.jpg'>",
    Container: "leave-sec-list-container"
};

(function () {
    var leaveSCR_OverrideCtx = {};
    leaveSCR_OverrideCtx.Templates = {};
    leaveSCR_OverrideCtx.Templates.Item = leaveSCR_CustomItem;
    leaveSCR_OverrideCtx.OnPreRender = function (ctx) {
        $('.ms-menutoolbar').hide();
    };
    leaveSCR_OverrideCtx.ListTemplateType = 10004;
    leaveSCR_OverrideCtx.BaseViewID = 6;
    leaveSCR_OverrideCtx.OnPostRender = leaveSCR_LeavePostRender;
    leaveSCR_OverrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr>" +
        "<th id='leaveSCR_photo'>" + LeaveRequestSecurityConfig.LaveManagement_RequesterPhoto + "</th>" +
        "<th id='leaveSCR_requester'>" + LeaveRequestSecurityConfig.LaveManagement_Requester + "</th>" +
        "<th id='leaveSCR_department'>" + LeaveRequestSecurityConfig.LeaveManagement_Department + "</th>" +
        "<th id='leaveSCR_from'>" + LeaveRequestSecurityConfig.LaveManagement_From + "</th>" +
        "<th id='leaveSCR_to'>" + LeaveRequestSecurityConfig.LaveManagement_To + "</th>" +
        "<th id='leaveSCR_shift'>" + LeaveRequestSecurityConfig.LeaveManagement_Shift + "</th>" +
        "<th id='leaveSCR_leftAt'>" + LeaveRequestSecurityConfig.LeaveManagement_LeftAt + "</th>" +
        "<th id='leaveSCR_checkoutby'>" + LeaveRequestSecurityConfig.LeaveManagement_CheckOutBy + "</th>" +
        "<th id='leaveSCR_EnterTime'>" + LeaveRequestSecurityConfig.LeaveManagement_EnterTime + "</th>" +
        "<th id='leaveSCR_checkinby'>" + LeaveRequestSecurityConfig.LeaveManagement_CheckInBy + "</th>" +
        "<th></th>" +
        "</tr></thead><tbody>";
    leaveSCR_OverrideCtx.Templates.Footer = pagingControl;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveSCR_OverrideCtx);
})();
function leaveSCR_LeavePostRender(ctx) {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(LeaveRequestSecurityConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + LeaveRequestSecurityConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(LeaveRequestSecurityConfig.ListResourceFileName, "Res", leaveSCR_OnListResourcesReady);
    }, "strings.js");

    $('.left-button').click(function () {
        $(this).attr('disabled', 'true');
        leaveSCR_UpdateLeftStatus($(this).attr('data-id'), $(this));
    });
    $('.entertime-button').click(function () {
        $(this).attr('disabled', 'true');
        leaveSCR_UpdateEnterTimeStatus($(this).attr('data-id'), $(this));
    });

    var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
    $.ajax({
        url: url,
        method: "GET",
        async: true,
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            if (data && data.length > 0) {
                $('#' + LeaveRequestSecurityConfig.Container + ' .department-locale').each(function () {
                    var id = $(this).attr('data-id');
                    var currentDepartment = $(this);
                    $(data).each(function (idx, obj) {
                        if (obj.Id.toString() === id) {
                            currentDepartment.text(obj.DepartmentName)
                        }
                    })
                });
            }
        },
        error: function (data) {
            status = 'failed';
        }
    });
}

function leaveSCR_OnListResourcesReady() {
    $('#leaveSCR_requester').text(Res.leaveList_Requester);
    $('#leaveSCR_department').text(Res.leaveList_Department);
    $('#leaveSCR_from').text(Res.leaveList_From);
    $('#leaveSCR_to').text(Res.leaveList_To);
    $('#leaveSCR_shift').text(Res.leaveList_Shift);
    $('#leaveSCR_leftAt').text(Res.leaveList_LeavedAt);
    $('#leaveSCR_checkoutby').text(Res.leaveList_CheckOutBy);
    $('#leaveSCR_photo').text(Res.leaveList_Avatar);
    $('#leaveSCR_EnterTime').text(Res.leaveList_EnterTime);
    $('#leaveSCR_checkinby').text(Res.leaveList_CheckInBy);
    $('#' + LeaveRequestSecurityConfig.Container + ' .left-button').text(Res.leaveList_LeftButton);
    $('#' + LeaveRequestSecurityConfig.Container + ' .entertime-button').text(Res.leaveList_Entered);
}

function leaveSCR_CustomItem(ctx) {
    var currentID = ctx.CurrentItem.ID;
    var getAvatarUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Employee/EmployeeService.svc/GetAvatar/" + ctx.CurrentItem.RequestFor[0].lookupId;
    var getAvatarPromise = $.ajax({
        url: getAvatarUrl,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
    });
    var photo = "";
    getAvatarPromise.then(function (image) {
        if (image != "") {
            photo = "<td id='photo'><div class='customImageAvatar'>" + image + "</div></td>";
        }
        else {
            photo = "<td id='photo'><div class='customImageAvatar'>" + LeaveRequestSecurityConfig.DefaultAvatar + "</div></td>";
        }

    }, function () {
        photo = "<td  id='photo'><div class='customImageAvatar'>" + LeaveRequestSecurityConfig.DefaultAvatar + "</div></td>";
    });

    var tr = "";
    var requesterName = '<td style="vertical-align: middle;">' + Functions.removeInvalidValue(ctx.CurrentItem.RequestFor[0].lookupValue) + '</td>';
    var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '" style="vertical-align: middle;">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
    var fromDate = '<td style="vertical-align: middle;">' + (ctx.CurrentItem.CommonFrom) + '</td>';
    var toDate = '<td style="vertical-align: middle;">' + (ctx.CurrentItem.To) + '</td>';
    var leftAt = '<td style="vertical-align: middle;">' + ctx.CurrentItem.LeftAt + '</td>';
    var checkOutByVal = "";
    if (ctx.CurrentItem.CheckOutBy) {
        checkOutByVal = ctx.CurrentItem.CheckOutBy[0].lookupValue;
    }
    var checkOutBy = '<td style="vertical-align: middle;">' + checkOutByVal + '</td>';

    var timeIn = '<td style="vertical-align: middle;">' + ctx.CurrentItem.EnterTime + '</td>';
    var checkInByVal = "";
    if (ctx.CurrentItem.CheckInBy) {
        checkInByVal = ctx.CurrentItem.CheckInBy[0].lookupValue;
    }
    var checkInBy = '<td style="vertical-align: middle;">' + checkInByVal + '</td>';

    var action = "";
    var disabledLeftAtButton = "";
    var disabledTimeInButton = "";
    if (ctx.CurrentItem.LeftAt || ctx.CurrentItem.LeftAt != "") {
        disabledLeftAtButton = "disabled";
    }

    if (ctx.CurrentItem.EnterTime && ctx.CurrentItem.EnterTime != "") {
        disabledTimeInButton = "disabled";
    }
    
    // Get Shift Time:
    var departmentId = ctx.CurrentItem.CommonDepartment[0].lookupId;
    var empId = ctx.CurrentItem.RequestFor[0].lookupId;
    var locationId = _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
    var date = new Date();
    var dateString = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
    var shiftValue = '<td style="vertical-align: middle;">' + 'HC' + '</td>';
    var getShiftTimeUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/LeaveManagement/LeaveManagementService.svc/GetShiftTimeByDate/" + dateString + "/" + empId + "/" + departmentId + "/" + locationId;
    $.ajax({
        url: getShiftTimeUrl,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
    }).done(function (response) {
        shiftValue = '<td style="vertical-align: middle;">' + response + '</td>';
    });

    action = "<td style='vertical-align: middle;' nowrap><button type='button'  class='btn btn-default btn-sm left-button' " + disabledLeftAtButton + " data-id='" + currentID + "'>Left</button><button type='button' style='margin-left: 10px;'  class='btn btn-default btn-sm entertime-button' " + disabledTimeInButton + " data-id='" + currentID + "'>Entered</button></td>";
    tr = "<tr>" + photo + requesterName + department + fromDate + toDate + shiftValue + leftAt + checkOutBy + timeIn + checkInBy + action + "</tr>";
    return tr;
}

function pagingControl(ctx) {
    return ViewUtilities.Paging.InstanceHtml(ctx);
}

function leaveSCR_UpdateLeftStatus(itemId) {
    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(LeaveRequestSecurityConfig.LaveManagement_ListTitle) + "')/items(" + itemId + ")";
    $.ajax({
        url: url,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            if (data) {
                leaveSCR_UpdateItem(itemId);
            }
        },
        error: function (data) {
        }
    });
}

function leaveSCR_UpdateItem(itemId) {
    var siteUrl = _spPageContextInfo.webServerRelativeUrl;
    var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
    var clientContext = new SP.ClientContext(fullWebUrl);
    var oList = clientContext.get_web().get_lists().getByTitle(String(LeaveRequestSecurityConfig.LaveManagement_ListTitle));
    var currentUser = clientContext.get_web().get_currentUser();
    clientContext.load(currentUser);
    clientContext.load(oList);
    this.oListItem = oList.getItemById(itemId);
    var now = new Date();
    oListItem.set_item('LeftAt', now);
    oListItem.set_item('CheckOutBy', _rbvhContext.EmployeeInfo.ID);
    oListItem.update();
    clientContext.executeQueryAsync(Function.createDelegate(this, this.leaveSCR_onQuerySucceeded), Function.createDelegate(this, this.leaveSCR_onQueryFailed));
}

function leaveSCR_UpdateEnterTimeStatus(itemId) {
    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(LeaveRequestSecurityConfig.LaveManagement_ListTitle) + "')/items(" + itemId + ")";
    $.ajax({
        url: url,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            if (data) {
                leaveSCR_UpdateEnterTimeItem(itemId);
            }
        },
        error: function (data) {
        }
    });
}

function leaveSCR_UpdateEnterTimeItem(itemId) {
    var siteUrl = _spPageContextInfo.webServerRelativeUrl;
    var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
    var clientContext = new SP.ClientContext(fullWebUrl);
    var oList = clientContext.get_web().get_lists().getByTitle(String(LeaveRequestSecurityConfig.LaveManagement_ListTitle));
    var currentUser = clientContext.get_web().get_currentUser();
    clientContext.load(currentUser);
    clientContext.load(oList);
    this.oListItem = oList.getItemById(itemId);
    var now = new Date();
    oListItem.set_item('EnterTime', now);
    oListItem.set_item('CheckInBy', _rbvhContext.EmployeeInfo.ID);
    oListItem.update();
    clientContext.executeQueryAsync(Function.createDelegate(this, this.leaveSCR_onQuerySucceeded), Function.createDelegate(this, this.leaveSCR_onQueryFailed));
}

function leaveSCR_onQuerySucceeded() {
    location.reload();
}
function leaveSCR_onQueryFailed() {
    location.reload();
}
