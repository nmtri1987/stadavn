(function () {
    var ApprovalConfigCSRDepartment = {
        ChangeShiftManagement_Requester: "Requester",
        ChangeShiftManagement_FromShift: "From Shift",
        ChangeShiftManagement_ToShift: "To Shift",
        ChangeShiftManagement_From: "From",
        ChangeShiftManagement_To: "To",
        ChangeShiftManagement_Created: "Created",
        Comment: "Comment",
        ChangeShiftManagement_Reason: "Reason",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        ViewDetail: "View item detail"
    };

    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_CSRDepartment;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10011;
        overrideCSRDepartmentCtx.BaseViewID = 1;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_CSRDepartment;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterTH_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_Requester + "</th>" +
            "<th id='fromshift_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_FromShift + "</th>" +
            "<th id='toshift_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_ToShift + "</th>" +
            "<th id='from_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_From + "</th>" +
            "<th id='to_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_To + "</th>" +
            "<th id='created_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_Created + "</th>" +
            "<th id='reason_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_Reason + "</th>" +
            "<th id='comment_csrDept'>" + ApprovalConfigCSRDepartment.Comment + "</th>" +
            "<th id='approvalStatus_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_ApprovalStatus + "</th>" +
            "<th id='action_csrDept'>" + ApprovalConfigCSRDepartment.ChangeShiftManagement_ViewTitle + "</th>" +
            "<th></th>" +
        "</tr></thead><tbody>";
        overrideCSRDepartmentCtx.Templates.Footer = CreateFooter;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCSRDepartmentCtx);
    })();
    function CreateFooter(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function openDialogBox(Url) {
        var ModalDialogOptions = { url: Url, width: 800, height: 400, showClose: true, allowMaximize: false, title: ApprovalConfigCSRDepartment.ViewDetail };
        SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
    }
    function PostRender_CSRDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            SP.SOD.registerSod(ApprovalConfigCSRDepartment.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalConfigCSRDepartment.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ApprovalConfigCSRDepartment.ListResourceFileName, "Res", OnListResourcesReady_CSRDepartment);
            SP.SOD.registerSod(ApprovalConfigCSRDepartment.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalConfigCSRDepartment.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ApprovalConfigCSRDepartment.PageResourceFileName, "Res", OnPageResourcesReady_CSRDepartment);
        }, "strings.js");
        $('.viewdetaildepreq').click(function () {
            url = $(this).attr('data-url');
            openDialogBox(url);
        });
    }
    function OnListResourcesReady_CSRDepartment() {
        $('#requesterTH_csrDept').text(Res.changeShiftList_Requester);
        $('#fromshift_csrDept').text(Res.changeShiftList_FromShift);
        $('#toshift_csrDept').text(Res.changeShiftList_ToShift);
        $('#from_csrDept').text(Res.changeShiftList_FromDate);
        $('#to_csrDept').text(Res.changeShiftList_ToDate);
        $('#created_csrDept').text(Res.createdDate);
        $('#reason_csrDept').text(Res.changeShiftManagement_Reason);
        $('#comment_csrDept').text(Res.commonComment);
        $('#approvalStatus_csrDept').text(Res.changeShiftManagement_ApprovalStatus);
        $('#action_csrDept').text(Res.changeShiftManagement_ViewTitle);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-default').text(Res.approvalStatus_InProgress);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        $('.cancel-request').text(Res.changeShiftManagement_CancelRequest);
    }
    function OnPageResourcesReady_CSRDepartment() {
        ApprovalConfigCSRDepartment.ViewDetail = Res.viewDetail;
    }
    function CustomItem_CSRDepartment(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var FromShift = '<td>' + ctx.CurrentItem.FromShift[0].lookupValue + '</td>';
        var ToShift = '<td>' + ctx.CurrentItem.ToShift[0].lookupValue + '</td>';
        var From = '<td>' + ctx.CurrentItem.CommonFrom + '</td>';
        var To = '<td>' + ctx.CurrentItem.To + '</td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var Reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
        var Comment = '<td>' + ctx.CurrentItem.CommonComment + '</td>';
        var status = ctx.CurrentItem.ApprovalStatus;

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab3';
        sourceURL = encodeURIComponent(sourceURL);
        var view = '<span><a data-url="/Lists/ChangeShiftManagement/DispForm.aspx?ID=' + ctx.CurrentItem.ID + '&TextOnly=true&Source=' + sourceURL + '"   class="table-action viewdetaildepreq"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        var actionEditView = "<td>" + view + "</td>";

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
        }
        tr = "<tr>" + Requester + FromShift + ToShift + From + To + created  + Reason + Comment + status + actionEditView + "</tr>";
        return tr;
    }
})();