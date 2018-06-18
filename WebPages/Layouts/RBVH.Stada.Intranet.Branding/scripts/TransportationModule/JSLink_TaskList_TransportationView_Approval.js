var TaskListApprovalTransportationConfig = {
    VehicleManagement_Requester: "Requester",
    VehicleManagement_VehicleType: 'Vehicle Type',
    VehicleManagement_CompanyPickup: 'Company Pickup',
    VehicleManagement_CommonTo: "To",
    VehicleManagement_CommonFrom: "From",
    VehicleManagement_ApprovalStatus: "Approval Status",
    NotOverTimeManagement_Date: 'Date',
    VehicleManagement_Reason: "Reason",
    Comment: "Comment",
    IsManager: false,
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages"
};
(function () {
    var overrideCtx = {};
    overrideCtx.Templates = {};
    overrideCtx.Templates.Item = CustomItem;
    overrideCtx.OnPreRender = function (ctx) {
        $('.ms-menutoolbar').hide();
        $('.ms-csrlistview-controldiv').hide();
    };
    overrideCtx.BaseViewID = 10;
    overrideCtx.ListTemplateType = 171;
    overrideCtx.OnPostRender = PostRender;
    overrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
    "<thead><tr><th id='requesterTHApprovalTransportation'>" + TaskListApprovalTransportationConfig.VehicleManagement_Requester + "</th>" +
    "<th id='vehicleTypeApprovalTransportation'>" + TaskListApprovalTransportationConfig.VehicleManagement_VehicleType + "</th>" +
    "<th id='companyPickupApprovalTransportation'>" + TaskListApprovalTransportationConfig.VehicleManagement_CompanyPickup + "</th>" +
    "<th id='fromApprovalTransportation'>" + TaskListApprovalTransportationConfig.VehicleManagement_CommonFrom + "</th>" +
    "<th id='toApprovalTransportation'>" + TaskListApprovalTransportationConfig.VehicleManagement_CommonTo + "</th>" +
    "<th id='reasonApprovalTransportation'>" + TaskListApprovalTransportationConfig.VehicleManagement_Reason + "</th>" +
     "<th id='commentApprovalTransportation'>" + TaskListApprovalTransportationConfig.Comment + "</th>" +
    "<th></th>" +
    "</tr></thead><tbody>";
    overrideCtx.Templates.Footer = pagingControl;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
})();
function PostRender(ctx) {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(TaskListApprovalTransportationConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + TaskListApprovalTransportationConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(TaskListApprovalTransportationConfig.ListResourceFileName, "Res", OnListResourcesReady);
        SP.SOD.registerSod(TaskListApprovalTransportationConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + TaskListApprovalTransportationConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(TaskListApprovalTransportationConfig.PageResourceFileName, "Res", OnPageResourcesReady);
    }, "strings.js");
}
function OnListResourcesReady() {
    $('#requesterTHApprovalTransportation').text(Res.vehicleManagement_Requester);
    $('#approvalStatusApprovalTransportation').text(Res.vehicleManagement_ApprovalStatus);
    $('#vehicleTypeApprovalTransportation').text(Res.vehicleManagement_VehicleType);
    $("#companyPickupApprovalTransportation").text(Res.vehicleManagement_CompanyPickup);
    $('#fromApprovalTransportation').text(Res.vehicleManagement_CommonFrom);
    $('#toApprovalTransportation').text(Res.vehicleManagement_CommonTo);
    $('#reasonApprovalTransportation').text(Res.vehicleManagement_Reason);
    $('.label-success').text(Res.approvalStatus_Approved);
    $('.label-default').text(Res.approvalStatus_InProgress);
    $('.label-warning').text(Res.approvalStatus_Cancelled);
    $('.label-danger').text(Res.approvalStatus_Rejected);
    $('.cancel-request').text(Res.notOverTimeManagement_CancelRequest);
    $('#commentApprovalTransportation').text(Res.commonComment);
}
function OnPageResourcesReady() {
    $(".approve-request").text(Res.approveButton);
    $(".reject-request").text(Res.rejectButton);
}
function pad(n) {
    return (n < 10) ? ("0" + n) : n;
}
function CustomItem(ctx) {
    var tr = "";
    var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';

    var date = '<td> ' + ctx.CurrentItem.CommonDate + '</td>';
    var vehicleType = '<td> ' + ctx.CurrentItem.VehicleType + '</td>';
    var pickupTemp = ctx.CurrentItem.CompanyPickup[0] == null ? ' ' : ctx.CurrentItem.CompanyPickup[0].lookupValue;
    var companyPickup = '<td> ' + pickupTemp + '</td>';
    var hourFrom = new Date(ctx.CurrentItem.CommonFrom);
    var from = '<td> ' + hourFrom.toLocaleDateString() + ' ' + hourFrom.toLocaleTimeString() + '</td>';
    var hourTo = new Date(ctx.CurrentItem.To);
    var to = '<td> ' + hourTo.toLocaleDateString() + ' ' + hourTo.toLocaleTimeString() + '</td>';
    var Comment = '<td><input type="text" class="form-control comment' + ctx.CurrentItem.ID + '" ></input></td>';
    var reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
    var approve = "<button type='button' class='btn btn-success btn-sm approve-request' data-approvalStatus='" + ctx.CurrentItem.ApprovalStatus + "'  data-id='" + ctx.CurrentItem.ID + "' data-emp-id='" + ctx.CurrentItem.Requester[0].lookupId + "'>Approve</button>";
    var reject = "<button type='button' class='btn btn-default btn-sm reject-request'  data-id='" + ctx.CurrentItem.ID + "'>Reject</button>";
    var action = '<td>' + approve + '   ' + reject + '<td>'

    tr = "<tr>" + requester + vehicleType + companyPickup + from + to + reason + Comment + action + "</tr>";
    return tr;
}
function pagingControl(ctx) {
    return ViewUtilities.Paging.InstanceHtml(ctx);
}
