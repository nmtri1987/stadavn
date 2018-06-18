var ChangeShiftApprovalTaskConfig = {
    ChangeShiftManagement_Requester: "Requester",
    ChangeShiftManagement_FromShift: "FromShift",
    ChangeShiftManagement_ToShift: "ToShift",
    ChangeShiftManagement_From: "From",
    ChangeShiftManagement_To: "To",
    ChangeShiftManagement_Created: "Created",
    ChangeShiftManagement_Reason: "Reason",
    Comment: "Comment",
    ItemID: '',
    ApprovalStatus: '',
    IsManager: false,
    ChangeShiftManagement_ViewTitle: "View",
    ApproveRequest: '//{0}/_vti_bin/Services/ChangeShiftManagement/ChangeShiftManagementService.svc/Approve',
    RejectRequest: '//{0}/_vti_bin/Services/ChangeShiftManagement/ChangeShiftManagementService.svc/Reject',
    ListResourceFileName: "RBVHStadaLists",
    PageResourceFileName: "RBVHStadaWebpages",
    Locale: '',
    ViewDetail: "View item detail",
};
(function () {
    var overrideCtx = {};
    overrideCtx.Templates = {};
    overrideCtx.Templates.Item = CustomItem;
    overrideCtx.OnPreRender = function (ctx) {
        $('.ms-menutoolbar').hide();
    };
    overrideCtx.BaseViewID = 3;
    overrideCtx.ListTemplateType = 10011;
    overrideCtx.OnPostRender = PostRender;
    overrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterTH'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_Requester + "</th>" +
        "<th id='fromshift'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_FromShift + "</th>" +
        "<th id='toshift'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_ToShift + "</th>" +
        "<th id='from'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_From + "</th>" +
        "<th id='to'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_To + "</th>" +
        "<th id='created'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_Created + "</th>" +
        "<th id='reason'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_Reason + "</th>" +
        "<th id='actionTH'>" + ChangeShiftApprovalTaskConfig.ChangeShiftManagement_ViewTitle + "</th>" +
	    "<th id='comment'>" + ChangeShiftApprovalTaskConfig.Comment + "</th>" +
        "<th></th>" +
        "</tr></thead><tbody>";
    overrideCtx.Templates.Footer = pagingControl;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
})();
function openDialogBox(Url) {
    var ModalDialogOptions = { url: Url, width: 800, height: 400, showClose: true, allowMaximize: false, title: ChangeShiftApprovalTaskConfig.ViewDetail };
    SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
}
function PostRender(ctx) {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        ChangeShiftApprovalTaskConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
        SP.SOD.registerSod(ChangeShiftApprovalTaskConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ChangeShiftApprovalTaskConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ChangeShiftApprovalTaskConfig.ListResourceFileName, "Res", OnListResourcesReady);
        SP.SOD.registerSod(ChangeShiftApprovalTaskConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ChangeShiftApprovalTaskConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ChangeShiftApprovalTaskConfig.PageResourceFileName, "Res", OnPageResourcesReady);
    }, "strings.js");
    $('.approve-request').click(function () {
        $(this).prop('disabled', true);
        DoApprove($(this).attr('data-id'));
    });
    $('.reject-request').click(function () {
        $(this).prop('disabled', true);
        DoReject($(this).attr('data-id'));
    });
    $('.viewdetailapproval').click(function () {
        url = $(this).attr('data-url');
        openDialogBox(url);
    });
}
function OnListResourcesReady() {
    $('#requesterTH').text(Res.changeShiftList_Requester);
    $('#fromshift').text(Res.changeShiftList_FromShift);
    $('#toshift').text(Res.changeShiftList_ToShift);
    $('#from').text(Res.changeShiftList_FromDate);
    $('#to').text(Res.changeShiftList_ToDate);
    $('#created').text(Res.createdDate);
    $('#reason').text(Res.changeShiftManagement_Reason);
    $('#actionTH').text(Res.changeShiftManagement_ViewTitle);
    $('#comment').text(Res.commonComment);
    $('.label-success').text(Res.approvalStatus_Approved);
    $('.label-default').text(Res.approvalStatus_InProgress);
    $('.label-warning').text(Res.approvalStatus_Cancelled);
    $('.label-danger').text(Res.approvalStatus_Rejected);
    $('.cancel-request').text(Res.changeShiftManagement_CancelRequest);
}
function OnPageResourcesReady() {
    $(".approve-request").text(Res.approveButton);
    $(".reject-request").text(Res.rejectButton);
    ChangeShiftApprovalTaskConfig.ViewDetail = Res.viewDetail;
}
function pad(n) {
    return (n < 10) ? ("0" + n) : n;
}
function CustomItem(ctx) {
    var tr = "";
    var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
    var FromShift = '<td>' + ctx.CurrentItem.FromShift[0].lookupValue + '</td>';
    var ToShift = '<td>' + ctx.CurrentItem.ToShift[0].lookupValue + '</td>';
    var From = '<td>' + ctx.CurrentItem.CommonFrom + '</td>';
    var To = '<td>' + ctx.CurrentItem.To + '</td>';
    var Created = '<td>' + ctx.CurrentItem.Created + '</td>';
    var Reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
    var Comment = '<td><input type="text" class="form-control comment' + ctx.CurrentItem.ID + '" ></input></td>';
    var status = ctx.CurrentItem.ApprovalStatus;

    var sourceURL = window.location.href.split('#')[0];
    sourceURL += '#tab2';
    sourceURL = encodeURIComponent(sourceURL);
    var view = '<span><a data-url="/Lists/ChangeShiftManagement/DispForm.aspx?ID=' + ctx.CurrentItem.ID + '&TextOnly=true&Source=' + sourceURL + '"   class="table-action viewdetailapproval"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
    var actionEditView = "<td>" + view + "</td>";

    var disabled = '';
    //if (ctx.CurrentItem.CommonReqDueDate && ctx.CurrentItem.CommonReqDueDate.length > 0) {
    //    var commonReqDueDate = ctx.CurrentItem.CommonReqDueDate;
    //    if (commonReqDueDate.indexOf(' ') > 0)
    //    {
    //        commonReqDueDate = commonReqDueDate.split(' ')[0];
    //        var requestDueDateObj = Functions.parseVietNameseDate(commonReqDueDate);
    //        var nowDate = new Date();
    //        var currentDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
    //        if (requestDueDateObj.valueOf() < currentDate.valueOf()) {
    //            disabled = 'disabled';
    //        }
    //    }
    //}

    var approve = "<button type='button' class='btn btn-success btn-sm approve-request' data-approvalStatus='" + ctx.CurrentItem.ApprovalStatus + "'  data-id='" + ctx.CurrentItem.ID + "' data-emp-id='" + ctx.CurrentItem.Requester[0].lookupId + "' " + disabled + ">Approve</button>";
    var reject = "<button type='button' class='btn btn-default btn-sm reject-request'  data-id='" + ctx.CurrentItem.ID + "' " + disabled + ">Reject</button>";
    var action = '<td>' + approve + '   ' + reject + '<td>'
    tr = "<tr>" + Requester + FromShift + ToShift + From + To + Created + Reason + actionEditView + Comment + action + "</tr>";
    return tr;
}

function pagingControl(ctx) {
    return ViewUtilities.Paging.InstanceHtml(ctx)
}

function DoApprove(itemId) {
    var postData = {};
    postData.Id = itemId;
    postData.Comment = $('.comment' + itemId).val();
    postData.ApproverName = _rbvhContext.EmployeeInfo.FullName;
    postData.ApproverId = _rbvhContext.EmployeeInfo.ADAccount.ID;
    var url = RBVH.Stada.WebPages.Utilities.String.format(ChangeShiftApprovalTaskConfig.ApproveRequest, location.host);
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    }).done(function (response) {
        if (response.Code === 2) {
            alert(response.Message);
        }
        Redirect();
    });
}
function DoReject(itemId) {
    var postData = {};
    postData.Id = itemId;
    postData.Comment = $('.comment' + itemId).val();
    postData.ApproverName = _rbvhContext.EmployeeInfo.FullName;
    postData.ApproverId = _rbvhContext.EmployeeInfo.ADAccount.ID;
    var url = RBVH.Stada.WebPages.Utilities.String.format(ChangeShiftApprovalTaskConfig.RejectRequest, location.host);
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    }).done(function (response) {
        if (response.Code === 3) {
            alert(response.Message);
        }
        Redirect();
    });
}
function Redirect() {
    var sourceParam = Functions.getParameterByName("Source");
    if (sourceParam) {
        Functions.redirectToSource();
    }
    else {
        window.location.reload();
    }
}
