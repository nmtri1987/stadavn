(function () {
    var NotOvertimeViewRequestConfig = {
        NotOverTimeManagement_Requester: "Requester",
        NotOverTimeManagement_HoursPerDay: "Hours per Day",
        NotOverTimeManagement_To: "To",
        NotOverTimeManagement_From: "From",
        NotOverTimeManagement_ApprovalStatus: "Approval Status",
        NotOverTimeManagement_Date: 'Date',
        NotOverTimeManagement_Reason: "Reason",
        NotOverTimeManagement_ListTitle: 'Leave Of Absence For Overtime Management',
        NotOvertimeManagement_ViewTitle: "View",
        Comment: 'Comment',
        ApprovalStatus_Cancelled: 'Cancelled',
        RequestStatusApproved: "",
        RequestStatusCancelled: "",
        RequestStatusRejected: "",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        ViewDetail: "View item detail",
        Container: '#leaveOfAbsenceRequestContainer'
    };
    (function () {
        var overrideNotOvertimeRequestCtx = {};
        overrideNotOvertimeRequestCtx.Templates = {};
        overrideNotOvertimeRequestCtx.BaseViewID = 3;
        overrideNotOvertimeRequestCtx.ListTemplateType = 10007;
        overrideNotOvertimeRequestCtx.Templates.Item = CustomItemNotOvertimeRequest;
        overrideNotOvertimeRequestCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideNotOvertimeRequestCtx.OnPostRender = PostRenderApprovalNotOvertimeRequest;
        overrideNotOvertimeRequestCtx.Templates.Header = "<div class='col-md-12'><div hidden='true' id='cancelRequestMessage'>Can not cancel the request that was approved</div><table class='table'>" +
            "<thead><tr><th id='requesterNotOverTimeManagementTH'>" + NotOvertimeViewRequestConfig.NotOverTimeManagement_Requester + "</th>" +
            "<th id='hoursperdayNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.NotOverTimeManagement_HoursPerDay + "</th>" +
            "<th id='dateNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.NotOverTimeManagement_Date + "</th>" +
            "<th id='fromNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.NotOverTimeManagement_From + "</th>" +
            "<th id='toNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.NotOverTimeManagement_To + "</th>" +
            "<th id='reasonNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.NotOverTimeManagement_Reason + "</th>" +
            "<th id='commentNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.Comment + "</th>" +
            "<th id='approvalStatusNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.NotOverTimeManagement_ApprovalStatus + "</th>" +
            "<th id='viewDetailNotOverTimeManagement'>" + NotOvertimeViewRequestConfig.NotOvertimeManagement_ViewTitle + "</th>" +
            "<th></th>" +
            "</tr></thead><tbody>";

        overrideNotOvertimeRequestCtx.Templates.Footer = pagingControlRequest;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideNotOvertimeRequestCtx);
    })();
    function openDialogBox(Url) {
        var ModalDialogOptions = { url: Url, width: 800, height: 400, showClose: true, allowMaximize: false, title: NotOvertimeViewRequestConfig.ViewDetail };
        SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
    }
    function PostRenderApprovalNotOvertimeRequest(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            NotOvertimeViewRequestConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(NotOvertimeViewRequestConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + NotOvertimeViewRequestConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(NotOvertimeViewRequestConfig.ListResourceFileName, "Res", OnListResourcesReadyApproval);
            SP.SOD.registerSod(NotOvertimeViewRequestConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + NotOvertimeViewRequestConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(NotOvertimeViewRequestConfig.PageResourceFileName, "Res", OnPageResourcesReadyApproval);
        }, "strings.js");
        $('.cancel-request').click(function () {
            updateListItemNotOvertimeRequest($(this).attr('data-id'), $(this));
        });
        $('.viewdetailrequest').click(function () {
            url = $(this).attr('data-url');
            openDialogBox(url);
        });
    }
    function OnListResourcesReadyApproval() {
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#requesterNotOverTimeManagementTH').text(Res.notOverTimeManagement_Requester);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#hoursperdayNotOverTimeManagement').text(Res.notOverTimeManagement_HoursPerDay);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#approvalStatusNotOverTimeManagement').text(Res.notOverTimeManagement_ApprovalStatus);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#dateNotOverTimeManagement').text(Res.notOverTimeManagement_Date);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#fromNotOverTimeManagement').text(Res.notOverTimeManagement_From);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#toNotOverTimeManagement').text(Res.notOverTimeManagement_To);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#reasonNotOverTimeManagement').text(Res.notOverTimeManagement_Reason);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#commentNotOverTimeManagement').text(Res.commonComment);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '#viewDetailNotOverTimeManagement').text(Res.notOvertimeManagement_ViewTitle);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '.label-success').text(Res.approvalStatus_Approved);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '.label-default').text(Res.approvalStatus_InProgress);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '.label-warning').text(Res.approvalStatus_Cancelled);
        NotOvertimeViewRequestConfig.ApprovalStatus_Cancelled = Res.approvalStatus_Cancelled;
        $(NotOvertimeViewRequestConfig.Container + ' ' + '.label-danger').text(Res.approvalStatus_Rejected);
        $(NotOvertimeViewRequestConfig.Container + ' ' + '.cancel-request').text(Res.notOverTimeManagement_CancelRequest);
    }
    function OnPageResourcesReadyApproval() {
        NotOvertimeViewRequestConfig.RequestStatusApproved = Res.requestStatusApproved;
        NotOvertimeViewRequestConfig.RequestStatusCancelled = Res.requestStatusCancelled;
        NotOvertimeViewRequestConfig.RequestStatusRejected = Res.requestStatusRejected;
        NotOvertimeViewRequestConfig.ViewDetail = Res.viewDetail;
    }
    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }
    function CustomItemNotOvertimeRequest(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';

        var date = '<td> ' + ctx.CurrentItem.CommonDate + '</td>';
        var fromTime = ctx.CurrentItem.CommonFrom.match(/(\d{1,2}:\d{1,2})/i);
        var From = '<td> ' + fromTime[1] + '</td>';
        //var From = '<td> ' + ctx.CurrentItem.CommonFrom + '</td>';
        //var To = '<td> ' + ctx.CurrentItem.To + '</td>';
        var toTime = ctx.CurrentItem.To.match(/(\d{1,2}:\d{1,2})/i);
        var To = '<td> ' + toTime[1] + '</td>';
        var HoursPerDay = '<td>' + ctx.CurrentItem.HoursPerDay + '</td>';
        var Reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
        var Comment = '<td>' + ctx.CurrentItem.CommonComment + '</td>';
        var status = ctx.CurrentItem.ApprovalStatus;

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab1';
        sourceURL = encodeURIComponent(sourceURL);
        var viewDetail = '<span><a data-url="/Lists/NotOverTimeManagement/DispForm.aspx?ID=' + ctx.CurrentItem.ID + '&TextOnly=true&Source=' + sourceURL + '"   class="table-action viewdetailrequest"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        viewDetail = "<td>" + viewDetail + "</td>";

        var action = "<td><button type='button' class='btn btn-default btn-sm cancel-request' disabled  data-id='" + ctx.CurrentItem.ID + "'>Cancel Request</button></td>";
        if (status == 'Approved') {
            status = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status == "Cancelled") {
            status = '<td><span class="label label-warning">Cancelled</span></td>';
        }
        else if (status == "Rejected") {
            status = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else {
            status = '<td><span class="label label-default">In-Progress</span></td>';
            action = "<td><button type='button' class='btn btn-default btn-sm cancel-request'  data-id='" + ctx.CurrentItem.ID + "'>Cancel Request</button></td>";
        }
        tr = "<tr>" + Requester + HoursPerDay + date + From + To + Reason + Comment + status + viewDetail + action + "</tr>";
        return tr;
    }
    function pagingControlRequest(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function updateListItemNotOvertimeRequest(itemId, element) {
        var targetListItem;
        var siteUrl = _spPageContextInfo.webServerRelativeUrl;
        var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
        var clientContext = new SP.ClientContext(fullWebUrl);
        var targetList = clientContext.get_web().get_lists().getByTitle(String(NotOvertimeViewRequestConfig.NotOverTimeManagement_ListTitle));

        targetListItem = targetList.getItemById(itemId);
        clientContext.load(targetListItem);
        clientContext.executeQueryAsync(Function.createDelegate(this, function () {
            var currentArrovalStatus = targetListItem.get_item("ApprovalStatus");
            if (currentArrovalStatus === "Approved") {
                alert(NotOvertimeViewRequestConfig.RequestStatusApproved);
                location.reload();
            }
            else if (currentArrovalStatus === "Cancelled") {
                alert(NotOvertimeViewRequestConfig.RequestStatusCancelled);
                location.reload();
            }
            else if (currentArrovalStatus === "Rejected") {
                alert(NotOvertimeViewRequestConfig.RequestStatusRejected);
                location.reload();
            }
            else {
                updateCancelStatusNotOvertimeRequest(itemId);
                $(element).attr('disabled', 'true');
            }
        }), Function.createDelegate(this, function () {
        }));
    }
    function updateCancelStatusNotOvertimeRequest(itemId) {
        var siteUrl = _spPageContextInfo.webServerRelativeUrl;
        var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
        var clientContext = new SP.ClientContext(fullWebUrl);
        var oList = clientContext.get_web().get_lists().getByTitle(String(NotOvertimeViewRequestConfig.NotOverTimeManagement_ListTitle));
        clientContext.load(oList);
        this.oListItem = oList.getItemById(itemId);
        oListItem.set_item('ApprovalStatus', 'Cancelled');
        oListItem.update();
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
    }
})();
function onQuerySucceeded() {
    location.reload();
}
function onQueryFailed(sender, args) {
}