var ApprovalConfig = {
    LaveManagement_Requester: "Requester",
    LaveManagement_From: "From",
    LaveManagement_To: "To",
    LeaveManagement_LeftAt: "Left At",
    LeaveManagement_Left: "Left",
    LaveManagement_RequesterPhoto: "Photo",
    LaveManagement_ListTitle: 'Leave Management',
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages",
    DefaultAvatar: "<img src='/_layouts/15/RBVH.Stada.Intranet.Branding/images/DefaultAvatar.jpg'>"
};
(function () {
    var overrideCtx = {};
    overrideCtx.Templates = {};
    overrideCtx.Templates.Item = CustomItem;
    overrideCtx.OnPreRender = function (ctx) {
        $('.ms-menutoolbar').hide();
    };
    overrideCtx.ListTemplateType = 10004;
    overrideCtx.OnPostRender = LeavePostRender;
    overrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr>" +
        "<th id='photo'>" + ApprovalConfig.LaveManagement_RequesterPhoto + "</th>" +
        "<th id='requesterTH'>" + ApprovalConfig.LaveManagement_Requester + "</th>" +
        "<th id='from'>" + ApprovalConfig.LaveManagement_From + "</th>" +
        "<th id='to'>" + ApprovalConfig.LaveManagement_To + "</th>" +
        "<th id='left'>" + ApprovalConfig.LeaveManagement_Left + "</th>" +
        "<th id='leftAt'>" + ApprovalConfig.LeaveManagement_LeftAt + "</th>" +
        "<th></th>" +
        "</tr></thead><tbody>";
    overrideCtx.Templates.Footer = pagingControl;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
})();
function LeavePostRender(ctx) {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(ApprovalConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ApprovalConfig.ListResourceFileName, "Res", OnListResourcesReady);
    }, "strings.js");

    $('.left-button').click(function () {
        $(this).attr('disabled', 'true');
        updateLeftStatus($(this).attr('data-id'), $(this));
    });
}
function OnListResourcesReady() {
    $('#requesterTH').text(Res.leaveList_Requester);
    $('#from').text(Res.leaveList_From);
    $('#to').text(Res.leaveList_To);
    $('#left').text(Res.leaveList_Leaved);
    $('#leftAt').text(Res.leaveList_LeavedAt);
    $('#photo').text(Res.leaveList_Avatar);
    $('.left-button').text(Res.leaveList_LeftButton);
}
function CustomItem(ctx) {
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
            photo = "<td id='photo'><div class='customImageAvatar'>" + ApprovalConfig.DefaultAvatar + "</div></td>";
        }

    }, function () {
        photo = "<td  id='photo'><div class='customImageAvatar'>" + ApprovalConfig.DefaultAvatar + "</div></td>";
    }
    );

    var tr = "";
    var requesterName = '<td style="vertical-align: middle;">' + Functions.removeInvalidValue(ctx.CurrentItem.RequestFor[0].lookupValue) + '</td>';
    var fromDate = '<td style="vertical-align: middle;">' + (ctx.CurrentItem.CommonFrom) + '</td>';
    var toDate = '<td style="vertical-align: middle;">' + (ctx.CurrentItem.To) + '</td>';

    var left = '<td style="vertical-align: middle;">' + ctx.CurrentItem.Left + '</td>';
    var leaveHour = '<td style="vertical-align: middle;">' + ctx.CurrentItem.LeaveHours + '</td>';
    var transferworkTo = "";
    if (ctx.CurrentItem.TransferworkTo && ctx.CurrentItem.TransferworkTo[0] && ctx.CurrentItem.TransferworkTo[0].lookupValue) {
        transferworkTo = '<td style="vertical-align: middle;">' + Functions.removeInvalidValue(ctx.CurrentItem.TransferworkTo[0].lookupValue) + '</td>';
    }
    else {
        transferworkTo = '<td style="vertical-align: middle;">&nbsp</td>';
    }
    var reason = '<td style="vertical-align: middle;">' + ctx.CurrentItem.Reason + '</td>';
    var leftAt = '<td style="vertical-align: middle;">' + ctx.CurrentItem.LeftAt + '</td>';

    var action;
    if (ctx.CurrentItem["Left.value"] && ctx.CurrentItem["Left.value"] === "1") {
        action = "<td style='vertical-align: middle;' nowrap><button type='button'  class='btn btn-default btn-sm left-button' disabled data-id='" + currentID + "'>Left</button></td>";
    }
    else {
        action = "<td style='vertical-align: middle;' nowrap><button type='button'   class='btn btn-default btn-sm left-button'  data-id='" + currentID + "'>Left</button></td>";
    }
    tr = "<tr>" + photo + requesterName + fromDate + toDate + left + leftAt + action + "</tr>";
    return tr;
}
function pagingControl(ctx) {
    return ViewUtilities.Paging.InstanceHtml(ctx);
}
function updateLeftStatus(itemId) {
    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(ApprovalConfig.LaveManagement_ListTitle) + "')/items(" + itemId + ")";
    $.ajax({
        url: url,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            if (data) {
                updateItem(itemId);
            }
        },
        error: function (data) {
        }
    });
}
function updateItem(itemId) {
    var siteUrl = _spPageContextInfo.webServerRelativeUrl;
    var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
    var clientContext = new SP.ClientContext(fullWebUrl);
    var oList = clientContext.get_web().get_lists().getByTitle(String(ApprovalConfig.LaveManagement_ListTitle));
    clientContext.load(oList);
    this.oListItem = oList.getItemById(itemId);
    oListItem.set_item('Left', true);
    var now = new Date();
    oListItem.set_item('LeftAt', now);
    oListItem.update();
    clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
}
function onQuerySucceeded() {
    location.reload();
}
function onQueryFailed() {
    location.reload();
}
function isLeft(itemId) {
    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(ApprovalConfig.LaveManagement_ListTitle) + "')/items(" + itemId + ")";
    $.ajax({
        url: url,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            if (data) {
                return data.d.Left;
            }
            else {
                return false;
            }
        },
        error: function (data) {
            return false;
        }
    });
}
