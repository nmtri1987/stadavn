(function () {
    var ApprovalConfigChangeShiftRequest = {
        ChangeShiftManagement_Requester: "Requester",
        ChangeShiftManagement_FromShift: "From Shift",
        ChangeShiftManagement_ToShift: "To Shift",
        ChangeShiftManagement_From: "From",
        ChangeShiftManagement_To: "To",
        ChangeShiftManagement_Created: "Created",
        Comment: "Comment",
        ChangeShiftManagement_Reason: "Reason",
        ApprovalStatus: '',
        RequestStatusApproved: '',
        RequestStatusCancelled: '',
        RequestStatusRejected: '',
        ChangeShiftManagement_ApprovalStatus: "Approval Status",
        ChangeShiftManagement_ListTitle: 'Change Shift Management',
        ChangeShiftManagement_ViewTitle: "View",
        ApprovalStatus_Cancelled: 'Cancelled',
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        ViewDetail: 'View item detail',
    };

    (function () {
        var overrideChangeShiftRequestCtx = {};
        overrideChangeShiftRequestCtx.Templates = {};
        overrideChangeShiftRequestCtx.Templates.Item = CustomItemChangeShiftRequest;
        overrideChangeShiftRequestCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideChangeShiftRequestCtx.ListTemplateType = 10011;
        overrideChangeShiftRequestCtx.BaseViewID = 2;
        overrideChangeShiftRequestCtx.OnPostRender = ChangeShiftRequestPostRender;
        overrideChangeShiftRequestCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterTH_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_Requester + "</th>" +
            "<th id='fromshift_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_FromShift + "</th>" +
            "<th id='toshift_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_ToShift + "</th>" +
            "<th id='from_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_From + "</th>" +
            "<th id='to_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_To + "</th>" +
            "<th id='created_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_Created + "</th>" +
            "<th id='reason_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_Reason + "</th>" +
            "<th id='comment_csRequest'>" + ApprovalConfigChangeShiftRequest.Comment + "</th>" +
            "<th id='approvalStatus_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_ApprovalStatus + "</th>" +
            "<th id='action_csRequest'>" + ApprovalConfigChangeShiftRequest.ChangeShiftManagement_ViewTitle + "</th>" +
            "<th></th>" +
        "</tr></thead><tbody>";
        overrideChangeShiftRequestCtx.Templates.Footer = pagingControlChangeShiftRequest;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideChangeShiftRequestCtx);
    })();
    function openDialogBox(Url) {
        var ModalDialogOptions = { url: Url, width: 800, height: 400, showClose: true, allowMaximize: false, title: ApprovalConfigChangeShiftRequest.ViewDetail };
        SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
    }
    function ChangeShiftRequestPostRender(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            ApprovalConfigChangeShiftRequest.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(ApprovalConfigChangeShiftRequest.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalConfigChangeShiftRequest.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ApprovalConfigChangeShiftRequest.ListResourceFileName, "Res", OnListResourcesReadyChangeShiftRequest);
            SP.SOD.registerSod(ApprovalConfigChangeShiftRequest.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalConfigChangeShiftRequest.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ApprovalConfigChangeShiftRequest.PageResourceFileName, "Res", OnPageResourcesReadyChangeShiftRequest);
        }, "strings.js");
        $('.cancel-request').click(function () {
            updateListItem_ChangeShiftRequest($(this).attr('data-id'), $(this));
            $(this).attr('disabled', 'true');
        });
        $('.viewdetailmyrequest').click(function () {
            url = $(this).attr('data-url');
            openDialogBox(url);
        });
    }
    function OnListResourcesReadyChangeShiftRequest() {
        $('#requesterTH_csRequest').text(Res.changeShiftList_Requester);
        $('#fromshift_csRequest').text(Res.changeShiftList_FromShift);
        $('#toshift_csRequest').text(Res.changeShiftList_ToShift);
        $('#from_csRequest').text(Res.changeShiftList_FromDate);
        $('#to_csRequest').text(Res.changeShiftList_ToDate);
        $('#created_csRequest').text(Res.createdDate);
        $('#reason_csRequest').text(Res.changeShiftManagement_Reason);
        $('#comment_csRequest').text(Res.commonComment);
        $('#approvalStatus_csRequest').text(Res.changeShiftManagement_ApprovalStatus);
        $('#action_csRequest').text(Res.changeShiftManagement_ViewTitle);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-default').text(Res.approvalStatus_InProgress);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        ApprovalConfigChangeShiftRequest.ApprovalStatus_Cancelled = Res.approvalStatus_Cancelled;
        $('.label-danger').text(Res.approvalStatus_Rejected);
        $('.cancel-request').text(Res.changeShiftManagement_CancelRequest);
    }
    function OnPageResourcesReadyChangeShiftRequest() {
        ApprovalConfigChangeShiftRequest.RequestStatusApproved = Res.requestStatusApproved;
        ApprovalConfigChangeShiftRequest.RequestStatusCancelled = Res.requestStatusCancelled;
        ApprovalConfigChangeShiftRequest.RequestStatusRejected = Res.requestStatusRejected;
        ApprovalConfigChangeShiftRequest.ViewDetail = Res.viewDetail;
    }
    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }
    function CustomItemChangeShiftRequest(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var FromShift = '<td>' + ctx.CurrentItem.FromShift[0].lookupValue + '</td>';
        var ToShift = '<td>' + ctx.CurrentItem.ToShift[0].lookupValue + '</td>';
        var From = '<td>' + ctx.CurrentItem.CommonFrom + '</td>';
        var To = '<td>' + ctx.CurrentItem.To + '</td>';
        var CreatedDate = '<td>' + ctx.CurrentItem.Created + '</td>';
        var Reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
        var Comment = '<td>' + ctx.CurrentItem.CommonComment + '</td>';
        var status = ctx.CurrentItem.ApprovalStatus;

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab1';
        sourceURL = encodeURIComponent(sourceURL);
        var view = '<span><a data-url="/Lists/ChangeShiftManagement/DispForm.aspx?ID=' + ctx.CurrentItem.ID + '&TextOnly=true&Source=' + sourceURL + '"   class="table-action viewdetailmyrequest"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        var actionEditView = "<td>" + view + "</td>";

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
        tr = "<tr>" + Requester + FromShift + ToShift + From + To + CreatedDate + Reason + Comment + status + actionEditView + action + "</tr>";
        return tr;
    }
    function pagingControlChangeShiftRequest(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function updateListItem_ChangeShiftRequest(itemId) {
        checkIsCancelled_ChangeShiftRequest(itemId);
        if (ApprovalConfigChangeShiftRequest.ApprovalStatus === "Approved") {
            alert(ApprovalConfigChangeShiftRequest.RequestStatusApproved);
            location.reload();
        }
        else if (ApprovalConfigChangeShiftRequest.ApprovalStatus === "Cancelled") {
            alert(ApprovalConfigChangeShiftRequest.RequestStatusCancelled);
            location.reload();
        }
        else if (ApprovalConfigChangeShiftRequest.ApprovalStatus === "Rejected") {
            alert(ApprovalConfigChangeShiftRequest.RequestStatusRejected);
            location.reload();
        }
        else {
            var siteUrl = _spPageContextInfo.webServerRelativeUrl;
            var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
            var clientContext = new SP.ClientContext(fullWebUrl);
            var oList = clientContext.get_web().get_lists().getByTitle(String(ApprovalConfigChangeShiftRequest.ChangeShiftManagement_ListTitle));
            clientContext.load(oList);
            this.oListItem = oList.getItemById(itemId);
            oListItem.set_item('ApprovalStatus', 'Cancelled');
            oListItem.update();
            clientContext.executeQueryAsync(Function.createDelegate(this, function () { location.reload(); }), Function.createDelegate(this, function () { }));
        }
    }
    function checkIsCancelled_ChangeShiftRequest(itemId) {
        var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(ApprovalConfigChangeShiftRequest.ChangeShiftManagement_ListTitle) + "')/items(" + itemId + ")";
        var d = $.Deferred();
        $.ajax({
            url: url,
            method: "GET",
            async: false,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                ApprovalConfigChangeShiftRequest.ApprovalStatus = data.d.ApprovalStatus;
                d.resolve(data.d);
            },
            error: function (data) {
                status = 'failed';
            }
        });
        return d.promise();
    }
})();