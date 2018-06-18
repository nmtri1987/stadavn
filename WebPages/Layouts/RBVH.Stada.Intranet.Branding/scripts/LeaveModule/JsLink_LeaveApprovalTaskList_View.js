var ApprovalTaskListConfig = {
    Requester: "Requester",
    From: "From",
    To: "To",
    LeaveHours: "Leave Hours",
    TransferWorkTo: "Transfer work To",
    Reason: "Reason",
    Comment: 'Comment',
    ApprovalStatus: "Approval Status",
    LeftAt: "Left At",
    Left: "Left",
    ApprovalStatus_Approved: "Approved",
    ApprovalStatus_InProgress: "In-Progress",
    ApprovalStatus_Cancelled: 'Cancelled',
    ApprovalStatus_Rejected: 'Rejected',
    CancelRequest: "Cancel Request",
    ListTitle: 'Leave Management',
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages",
    ItemID: ""
};
(function () {
    var overrideCtx = {};
    overrideCtx.Templates = {};
    overrideCtx.Templates.Item = CustomItem;
    overrideCtx.OnPreRender = function (ctx) {
        $('.ms-menutoolbar').hide();
    };
    overrideCtx.ListTemplateType = 10004;
    overrideCtx.OnPostRender = PostRender;
    overrideCtx.Templates.Header = "<div class='col-md-12'><div hidden='true' id='cancelRequestMessage'>Can not cancel the request that was approved</div><table class='table'>" +
        "<thead><tr><th id='requesterTH'>" + ApprovalTaskListConfig.Requester + "</th>" +
        "<th id='from'>" + ApprovalTaskListConfig.From + "</th>" +
        "<th id='to'>" + ApprovalTaskListConfig.To + "</th>" +
        "<th id='leaveHours'>" + ApprovalTaskListConfig.LeaveHours + "</th>" +
        "<th id='transferWorkTo'>" + ApprovalTaskListConfig.TransferWorkTo + "</th>" +
        "<th id='reason'>" + ApprovalTaskListConfig.Reason + "</th>" +
        "<th id='comment'>" + ApprovalTaskListConfig.Comment + "</th>" +
        "<th></th>" +
        "</tr></thead><tbody>";
    overrideCtx.Templates.Footer = pagingControl;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
})();
function PostRender(ctx) {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(ApprovalTaskListConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalTaskListConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ApprovalTaskListConfig.ListResourceFileName, "Res", OnListResourcesReady);
        SP.SOD.registerSod(ApprovalTaskListConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalTaskListConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ApprovalTaskListConfig.PageResourceFileName, "Res", OnPageResourcesReady);
    }, "strings.js");

    $('.approve-request').click(function () {
        $(this).prop('disabled', true);
        approval_Task($(this).attr('data-id'), $(this).attr('data-approvalstatus'), $(this).attr('data-emp-id'));
    });
    $('.reject-request').click(function () {
        $(this).prop('disabled', true);
        reject_Task($(this).attr('data-id'), $(this));
    });
}
function OnListResourcesReady() {
    $('#requesterTH').text(Res.leaveList_Requester);
    $('#approvalStatus').text(Res.ApprovalStatus);
    $('#from').text(Res.leaveList_From);
    $('#to').text(Res.leaveList_To);
    $('#reason').text(Res.leaveList_Reason);
    $('#transferWorkTo').text(Res.leaveList_TransferWorkTo);
    $('#leaveHours').text(Res.leaveList_LeaveHours);
    $('#comment').text(Res.leaveList_Comment);
}
function OnPageResourcesReady() {
    $('#cancelRequestMessage').html(Res.cannotCancelApprovedRequest);
    $(".approve-request").text(Res.approveButton);
    $(".reject-request").text(Res.rejectButton);
}
function pad(n) {
    return (n < 10) ? ("0" + n) : n;
}
function CustomItem(ctx) {
    var tr = "";
    var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
    var from = '<td >' + ctx.CurrentItem.CommonFrom + '</td>';
    var to = '<td >' + ctx.CurrentItem.To + '</td>';
    var transferWorkTo = '<td>' + ctx.CurrentItem.TransferworkTo[0].lookupValue + '</td>';
    var leaveHours = '<td >' + ctx.CurrentItem.LeaveHours + '</td>';
    var reason = '<td >' + ctx.CurrentItem.Reason + '</td>';
    var comment = '<td ><input type="text" class="form-control comment' + ctx.CurrentItem.ID + '" /></td>';

    var approve = "<button type='button' class='btn btn-success btn-sm approve-request' data-approvalStatus='" + ctx.CurrentItem.ApprovalStatus + "'  data-id='" + ctx.CurrentItem.ID + "' data-emp-id='" + ctx.CurrentItem.Requester[0].lookupId + "'>Approve</button>";
    var reject = "<button type='button' class='btn btn-default btn-sm reject-request'  data-id='" + ctx.CurrentItem.ID + "'>Reject</button>";
    var action = '<td>' + approve + '   ' + reject + '<td>';
    tr = "<tr>" + requester + from + to + leaveHours + transferWorkTo + reason + comment + action + "</tr>";
    return tr;
}

function pagingControl(ctx) {
    return ViewUtilities.Paging.InstanceHtml(ctx);
}

function updateListItem(itemId, element) {
    //get current status 
    var targetListItem;
    var siteUrl = _spPageContextInfo.webServerRelativeUrl;
    var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
    var clientContext = new SP.ClientContext(fullWebUrl);
    var targetList = clientContext.get_web().get_lists().getByTitle(String(ApprovalTaskListConfig.ListTitle));

    targetListItem = targetList.getItemById(itemId);
    clientContext.load(targetListItem);
    clientContext.executeQueryAsync(Function.createDelegate(this, function () {

        var currentArrovalStatus = targetListItem.get_item("ApprovalStatus");
        if (currentArrovalStatus == "Approved" || currentArrovalStatus == "Rejected") {
            alert($("#cancelRequestMessage").text());
            location.reload();
        }
        else {
            updateCancelStatus(itemId);
            $(element).attr('disabled', 'true');
            $(element).closest('tr').find('.label').removeClass('label-default').addClass('label-warning');
            $(element).closest('tr').find('.label').removeClass('label-default').text(ApprovalTaskListConfig.ApprovalStatus_Cancelled);
        }
    }), Function.createDelegate(this, function () {

    }));
}

function updateCancelStatus(itemId) {
    var siteUrl = _spPageContextInfo.webServerRelativeUrl;
    var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
    var clientContext = new SP.ClientContext(fullWebUrl);
    var oList = clientContext.get_web().get_lists().getByTitle(String(ApprovalTaskListConfig.ListTitle));
    clientContext.load(oList);
    this.oListItem = oList.getItemById(itemId);
    oListItem.set_item('ApprovalStatus', 'Cancelled');
    oListItem.update();
    clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
}

function onQuerySucceeded() {

}

function onQueryFailed(sender, args) {

}

function approval_Task(itemId, currentStatus, employeeId) {
    checkIsCancelled(itemId);
    if (ApprovalTaskListConfig.ApprovalStatus != 'Cancelled') {
        ApprovalTaskListConfig.ItemID = itemId;
        var siteUrl = _spPageContextInfo.webServerRelativeUrl;
        var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
        var clientContext = new SP.ClientContext(fullWebUrl);
        var oList = clientContext.get_web().get_lists().getByTitle(String(ApprovalTaskListConfig.ListTitle));
        clientContext.load(oList);
        this.oListItem = oList.getItemById(itemId);
        if (currentStatus == '') {
            this.checkIsManager(employeeId);
            var comment = $('.comment' + itemId).val();
            oListItem.set_item('CommonComment', comment);
            oListItem.set_item('ApprovalStatus', 'Approved');
            ApprovalTaskListConfig.ApprovalStatus = 'Approved';
        }
        else if (currentStatus == '1') {
            var comment = $('.comment' + itemId).val();
            oListItem.set_item('CommonComment', comment);
            oListItem.set_item('ApprovalStatus', 'Approved');
            ApprovalTaskListConfig.ApprovalStatus = 'Approved';
        }

        oListItem.update();
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
    }
    else {
        // alert(ApprovalTaskListConfig.ApproveCancelled);
    }
}
function reject_Task(itemId) {
    checkIsCancelled(itemId);
    ApprovalTaskListConfig.ItemID = itemId;
    if (ApprovalTaskListConfig.ApprovalStatus != 'Cancelled') {
        var siteUrl = _spPageContextInfo.webServerRelativeUrl;
        var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
        var clientContext = new SP.ClientContext(fullWebUrl);
        var oList = clientContext.get_web().get_lists().getByTitle(String(ApprovalTaskListConfig.ListTitle));
        clientContext.load(oList);
        this.oListItem = oList.getItemById(itemId);
        var comment = $('.comment' + itemId).val();
        oListItem.set_item('CommonComment', comment);
        oListItem.set_item('ApprovalStatus', 'Rejected');
        oListItem.update();
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
    }
    else {
        //  alert(ApprovalTaskListConfig.ApproveCancelled);
    }
}

function onQuerySucceeded() {
    location.reload();
}

function onQueryRejectSucceeded() {
    location.reload();
}

function onQueryFailed(sender, args) {
}

function checkIsCancelled(itemId) {
    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(ApprovalTaskListConfig.ListTitle) + "')/items(" + itemId + ")";
    var d = $.Deferred();
    $.ajax({
        url: url,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            ApprovalTaskListConfig.ApprovalStatus = data.d.ApprovalStatus;
            d.resolve(data.d);
        },
        error: function (data) {
            status = 'failed';
        }
    });
    return d.promise();
}

function checkIsManager(employeeId) {
    var url = _spPageContextInfo.webAbsoluteUrl + String(ApprovalTaskListConfig.IsManagerServiceUrl) + employeeId;
    var d = $.Deferred();
    $.ajax({
        url: url,
        type: "get",
        async: false,
        success: function (data) {
            ApprovalTaskListConfig.IsManager = data;
            d.resolve(data);
        },
        error: function () {
            return false;
        }
    });
    return d.promise();
}
