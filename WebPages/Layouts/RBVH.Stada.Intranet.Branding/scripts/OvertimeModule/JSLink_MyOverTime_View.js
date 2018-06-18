(function () {
    var MyOvertimeConfig = {
        OvertimeDetail_OvertimeHourFrom: "Overtime Hour From",
        OvertimeDetail_OvertimeHourTo: "Overtime Hour To",
        OvertimeDetail_WorkingHour: "Working Hour(s)",
        OvertimeDetail_Task: "Work Content",
        OvertimeDetail_TransportAtHM: "Company Transport HM",
        OvertimeDetail_TransportAtKD: "Company Transport KD",
        OvertimeDetail_CompanyTransport: "Company Transport",
        ApprovalStatusFieldDisplayName: "Approval Status",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        ViewDetail: "View item detail",
        Container: "my-overtime-container"
    };
    (function () {
        var overrideCtx = {};
        overrideCtx.Templates = {};
        overrideCtx.BaseViewID = 1;
        overrideCtx.Templates.Item = MyOvertimeCustomItem;
        overrideCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCtx.OnPostRender = OnPostRender;
        overrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'><thead>" +
        "<th id='OvertimeHourFrom'>" + MyOvertimeConfig.OvertimeDetail_OvertimeHourFrom + "</th>" +
         "<th id='OvertimeHourTo'>" + MyOvertimeConfig.OvertimeDetail_OvertimeHourTo + "</th>" +
        "<th id='OvertimeDetail_WorkingHour'>" + MyOvertimeConfig.OvertimeDetail_WorkingHour + "</th>" +
        "<th id='OvertimeDetail_Task'>" + MyOvertimeConfig.OvertimeDetail_Task + "</th>" +
        "<th id='OvertimeDetail_CompanyTransport'>" + MyOvertimeConfig.OvertimeDetail_CompanyTransport + "</th>" +
        "<th id='myovertime_approvalStatus'>" + MyOvertimeConfig.ApprovalStatusFieldDisplayName + "</th>" +
        "</tr></thead><tbody>";
        overrideCtx.Templates.Footer = pagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
    })();

    function openDialogBox(Url) {
        var ModalDialogOptions = { url: Url, width: 650, height: 500, showClose: true, allowMaximize: false, title: MyOvertimeConfig.ViewDetail };
        SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
    }
    function OnPostRender(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            SP.SOD.registerSod(MyOvertimeConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + MyOvertimeConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(MyOvertimeConfig.ListResourceFileName, "Res", OnListResourcesReady);
            SP.SOD.registerSod(MyOvertimeConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + MyOvertimeConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(MyOvertimeConfig.PageResourceFileName, "Res", OnPageResourcesReady);
        }, "strings.js");

        $(document).on('click', '.viewDetail', function () {
            url = $(this).attr('data-url');
            openDialogBox(url);
        });
    }
    function OnListResourcesReady() {
        $('#' + MyOvertimeConfig.Container + ' #OvertimeHourFrom').text(Res.overtimeDetail_OvertimeHourFrom);
        $('#' + MyOvertimeConfig.Container + ' #OvertimeHourTo').text(Res.overtimeDetail_OvertimeHourTo);
        $('#' + MyOvertimeConfig.Container + ' #OvertimeDetail_Task').text(Res.overtimeDetail_Task);
        $('#' + MyOvertimeConfig.Container + ' #myovertime_approvalStatus').text(Res.approvalStatusFieldDisplayName);
        $('#' + MyOvertimeConfig.Container + ' #OvertimeDetail_WorkingHour').text(Res.overtimeDetail_WorkingHour);
        $('#' + MyOvertimeConfig.Container + ' #OvertimeDetail_CompanyTransport').text(Res.overtimeDetail_CompanyTransport);
        $('#' + MyOvertimeConfig.Container + ' .viewDetail').text(Res.overtimeDetail_ViewDetail);
        $('#' + MyOvertimeConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + MyOvertimeConfig.Container + ' .label-default').text(Res.approvalStatus_InProgress);
        $('#' + MyOvertimeConfig.Container + ' .label-danger').text(Res.approvalStatus_Rejected);
    }
    function OnPageResourcesReady()
    {
        MyOvertimeConfig.ViewDetail = Res.viewDetail;
    }
    function MyOvertimeCustomItem(ctx) {
        var tr = "";
        var OvertimeHourFrom = '<td>' + ctx.CurrentItem.OvertimeFrom + '</td>';
        var OvertimeHourTo = '<td> ' + ctx.CurrentItem.OvertimeTo + '</td>';
        var viewDetail = "";
        if (ctx.CurrentItem.SummaryLinks != null && ctx.CurrentItem.SummaryLinks != '') {
            viewDetail = "- <a class='viewDetail' href='#' data-url='" + ctx.CurrentItem.SummaryLinks + "' >View Detail<a>"
        }
        var Task = '<td>' + ctx.CurrentItem.Task + ' ' + viewDetail + '</td>';
        var CompanyTransport = '<td>' + ctx.CurrentItem.CompanyTransport + '</td>';
        var WorkingHour = '<td>' + ctx.CurrentItem.WorkingHours + '</td>';
        var status = ctx.CurrentItem.ApprovalStatus;
        if (status == 'true') {
            status = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status == 'false') {
            status = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else {
            status = '<td><span class="label label-default">In-Progress</span></td>';
        }
        tr = "<tr>" + OvertimeHourFrom + OvertimeHourTo + WorkingHour + Task + CompanyTransport + status + "</tr>";

        return tr;
    }
    function pagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();