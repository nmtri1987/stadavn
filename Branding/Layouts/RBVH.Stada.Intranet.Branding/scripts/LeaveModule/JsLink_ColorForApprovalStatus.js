var isRenderOnce = false;
var spContextLeave;
var leaveManagementListTitle;
var approvalStatusField;
var cancelledStatus;
var approvedStatus;

var LeaveManagement = {
    CannotCancelRequestCancelled: "Can not cancel the request that has been cancelled",
    CannotCancelRequestApproved: "Can not cancel the request that has been approved",
    CancelLink_ErrorOccurre: "An error occurred while performing this action. Please try again!",
    CancelLink_RequestCancelledSuccess: "The request has been successfully cancelled",
    CancelLink_ConfirmCancel: "Are you sure you want to cancel this request?",

    CancelLink_Cancel: "Cancel",
    ApprovalStatus_Approved: "Approved",
    ApprovalStatus_Cancelled: "Cancelled",
    ApprovalStatus_InProgress: "In-Progress",
    ApprovalStatus_Rejected: "Rejected",

    PageResourceFileName: "RBVHStadaWebpages",
    ListResourceFileName: "RBVHStadaLists"
};

function OnPageResourcesLeaveReady() {
    LeaveManagement.CannotCancelRequestCancelled = Res.cannotCancelRequestCancelled;
    LeaveManagement.CannotCancelRequestApproved = Res.cannotCancelRequestApproved;
    LeaveManagement.CancelLink_ErrorOccurre = Res.cancelLink_ErrorOccurre;
    LeaveManagement.CancelLink_RequestCancelledSuccess = Res.cancelLink_RequestCancelledSuccess;
    LeaveManagement.CancelLink_ConfirmCancel = Res.cancelLink_ConfirmCancel;
}

function OnListResourcesLeaveReady() {
    LeaveManagement.ApprovalStatus_Approved = Res.approvalStatus_Approved;
    LeaveManagement.ApprovalStatus_Cancelled = Res.approvalStatus_Cancelled;
    LeaveManagement.ApprovalStatus_InProgress = Res.approvalStatus_InProgress;
    LeaveManagement.ApprovalStatus_Rejected = Res.approvalStatus_Rejected;
    LeaveManagement.CancelLink_Cancel = Res.cancelLink_Cancel;
}

(function () {
    SP.SOD.registerSod(LeaveManagement.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + LeaveManagement.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(LeaveManagement.PageResourceFileName, "Res", OnPageResourcesLeaveReady);
    SP.SOD.registerSod(LeaveManagement.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + LeaveManagement.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(LeaveManagement.ListResourceFileName, "Res", OnListResourcesLeaveReady);

    isRenderOnce = false;
    var leaveManagementMyRequestContext = {};
    leaveManagementMyRequestContext.Templates = {};
    leaveManagementMyRequestContext.Templates.Fields = {
        "ApprovalStatus": {
            "View": leave_ApprovalStatusFieldTemplate
        },
        "CancelLink":
        {
            "View": leave_CancelRequestField
        }
    };
    leaveManagementMyRequestContext.OnPostRender = leave_MyRequest_OnPostRender;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveManagementMyRequestContext);

    ExecuteOrDelayUntilScriptLoaded(loadLeaveSpContext, "sp.js");
    leaveManagementListTitle = "Leave Management";
    approvalStatusField = "ApprovalStatus";
    cancelledStatus = "Cancelled";
    approvedStatus = "Approved";
    inprogressStatus = "In-Progress";
})();


function loadLeaveSpContext() {
    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    spContextLeave = SP.ClientContext.get_current();
}

function leave_ApprovalStatusFieldTemplate(ctx) {
    switch (ctx.CurrentItem.ApprovalStatus) {
        case "Approved":
            return "<div> <b>" + leave_getStatusWithResources(ctx.CurrentItem.ApprovalStatus) + "</b></div>";
        case "Rejected":
            return "<div style='color: red;'> " + leave_getStatusWithResources(ctx.CurrentItem.ApprovalStatus) + "</div>";
        case "Cancelled":
            return "<div style='font-style: italic;'> " + leave_getStatusWithResources(ctx.CurrentItem.ApprovalStatus) + "</div>";
        default:
            return "<div>" + leave_getStatusWithResources(ctx.CurrentItem.ApprovalStatus) + "</div>";
    }
}

function leave_getStatusWithResources(statusValue) {
    switch (statusValue) {
        case "Approved":
            return LeaveManagement.ApprovalStatus_Approved;
        case "Rejected":
            return LeaveManagement.ApprovalStatus_Rejected;
        case "In-Progress":
            return LeaveManagement.ApprovalStatus_InProgress;
        case "Cancelled":
            return LeaveManagement.ApprovalStatus_Cancelled;
        default:
            return "";
    }
}

function leave_CancelRequestField(ctx) {
    var id = ctx.CurrentItem.ID;
    var approvalStatus = ctx.CurrentItem.ApprovalStatus;
    var hiddenString = "";
    if (approvalStatus !== inprogressStatus) {
        hiddenString = " style='display:none;' ";
    }
    return '<a class="cancel-link" ' + hiddenString + ' item-id="' + id + '" name="leaveCancelLink" >' + LeaveManagement.CancelLink_Cancel + ' </a>';
}

function leave_MyRequest_OnPostRender() {
    if (isRenderOnce === false) {
        $("a[name*='leaveCancelLink']").on("click", function () {
            var cofirmText = confirm(LeaveManagement.CancelLink_ConfirmCancel);
            if (cofirmText === true) {
                var leaveManagementId = $(this).attr("item-id");
                leave_SetCancelRequest(leaveManagementId);
            }
        });
        isRenderOnce = true;
    }
}

function leave_SetCancelRequest(leaveId) {
    var oList = spContextLeave.get_web().get_lists().getByTitle(leaveManagementListTitle);
    var targetListitem = oList.getItemById(leaveId);
    spContextLeave.load(targetListitem, approvalStatusField);

    spContextLeave.executeQueryAsync(Function.createDelegate(this, function () {
        var dataApprovalStatus = targetListitem.get_item(approvalStatusField);
        if (dataApprovalStatus === approvedStatus) {
            //Can not cancel the request that has been approved
            alert(LeaveManagement.CannotCancelRequestApproved);
        }
        else if (dataApprovalStatus === cancelledStatus) {
            //Can not cancel the request that has been cancelled
            alert(LeaveManagement.CannotCancelRequestCancelled);
        }
        else {
            cancelRequestProcess(leaveId);
        }
    }),
    Function.createDelegate(this, function () {
        alert(LeaveManagement.CancelLink_ErrorOccurre);
    }));

    function cancelRequestProcess(cancelRequestId) {
        var oList = spContextLeave.get_web().get_lists().getByTitle(leaveManagementListTitle);
        this.oListItem = oList.getItemById(cancelRequestId);
        oListItem.set_item(approvalStatusField, cancelledStatus);
        oListItem.update();
        spContextLeave.executeQueryAsync(Function.createDelegate(this, function () {
            alert(LeaveManagement.CancelLink_RequestCancelledSuccess);
            location.reload();
        }), Function.createDelegate(this, function () {
            alert(LeaveManagement.CancelLink_ErrorOccurre);
        }));
    }
}
