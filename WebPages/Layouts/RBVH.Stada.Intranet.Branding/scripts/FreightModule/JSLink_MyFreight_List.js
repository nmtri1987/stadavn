(function () {
    var MyFreightListConfig = {
        FreightManagement_ViewDetail: "View Detail",
        FreightManagement_Requester: "Requester",
        FreightManagement_Bringer: "Bringer",
        FreightManagement_Receiver: "Received by",
        FreightManagement_Created: "Created",
        FreightManagement_Comment: "Comment",
        FreightManagement_ApprovalStatus: "Approval status",
        FreightManagement_RequestNumber: "Request number",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        CompanyVehicle: "Company's Vehicle",
        Locale: '',
        Container: "my-freight-list-container",
    };
    (function () {
        var overrideMyFreightCtx = {};
        overrideMyFreightCtx.Templates = {};
        overrideMyFreightCtx.Templates.Item = CustomItemMyFreight;
        overrideMyFreightCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideMyFreightCtx.ListTemplateType = 10015;
        overrideMyFreightCtx.BaseViewID = 5;
        overrideMyFreightCtx.OnPostRender = PostRender_MyFreightList;
        overrideMyFreightCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='my-freight-detail'>" + MyFreightListConfig.FreightManagement_ViewDetail + "</th>" +
            "<th id='my-freight-requesterno'>" + MyFreightListConfig.FreightManagement_RequestNumber + "</th>" +
            "<th id='my-freight-requester'>" + MyFreightListConfig.FreightManagement_Requester + "</th>" +
            "<th id='my-freight-bringer'>" + MyFreightListConfig.FreightManagement_Bringer + "</th>" +
            "<th id='my-freight-receiver'>" + MyFreightListConfig.FreightManagement_Receiver + "</th>" +
            "<th id='my-freight-created'>" + MyFreightListConfig.FreightManagement_Created + "</th>" +
            "<th id='my-freight-comment'>" + MyFreightListConfig.FreightManagement_Comment + "</th>" +
            "<th id='my-freight-status'>" + MyFreightListConfig.FreightManagement_ApprovalStatus + "</th>" +
            "<th></th>" +
            "</tr></thead><tbody>";
        overrideMyFreightCtx.Templates.Footer = pagingControlMyFreight;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideMyFreightCtx);
    })();

    function PostRender_MyFreightList(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            MyFreightListConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(MyFreightListConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + MyFreightListConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(MyFreightListConfig.ListResourceFileName, "Res", OnListResourcesReadyMyFreight);
        }, "strings.js");
    }

    function OnListResourcesReadyMyFreight() {
        $('#my-freight-detail').text(Res.freightManagement_ViewDetail);
        $('#my-freight-requester').text(Res.freightManagement_Requester);
        $('#my-freight-bringer').text(Res.freightManagement_Bringer);
        $('#my-freight-requesterno').text(Res.freightManagement_RequestNumber);
        $('#my-freight-receiver').text(Res.freightManagement_Receiver);
        $('#my-freight-created').text(Res.createdDate);
        $('#my-freight-comment').text(Res.commonComment);
        $('#my-freight-status').text(Res.freightManagement_ApprovalStatus);
        $('#' + MyFreightListConfig.Container + ' .viewDetail').text(Res.freightManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        MyFreightListConfig.CompanyVehicle = Res.freightManagement_CompanyVehicle;
        $('.companyvehicle').text(MyFreightListConfig.CompanyVehicle);
    }

    function CustomItemMyFreight(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var bringer = '<td></td>';
        if (ctx.CurrentItem.Bringer && ctx.CurrentItem.Bringer[0].lookupId > 0) {
            bringer = '<td>' + ctx.CurrentItem.Bringer[0].lookupValue + '</td>';
        }
        else if (ctx.CurrentItem.BringerName && ctx.CurrentItem.BringerName.length > 0) {
            bringer = '<td>' + ctx.CurrentItem.BringerName + '</td>';
        }
        else {
            bringer = '<td class="companyvehicle">' + MyFreightListConfig.CompanyVehicle + '</td>';
        }

        var receiver = ctx.CurrentItem.Receiver != null ? '<td>' + ctx.CurrentItem.Receiver + '</td>' : '<td></td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab0';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

        var status = ctx.CurrentItem.ApprovalStatus + "";
        status = status.toLowerCase();
        var statusVal = '';
        if (status === 'approved') {
            statusVal = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status === "cancelled") {
            statusVal = '<td><span class="label label-warning">Cancelled</span></td>';
        }
        else if (status === "rejected") {
            statusVal = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else if (status && status.length > 0) {
            statusVal = '<td><span class="label label-default">' + ctx.CurrentItem.ApprovalStatus + '</span></td>';
        }
        else {
            statusVal = '<td><span class="label label-default">In-Progress</span></td>';
        }
        var requestNo = ctx.CurrentItem.RequestNo != null ? '<td>' + ctx.CurrentItem.RequestNo + '</td>' : '<td></td>';
        tr = "<tr>" + title + requestNo + requester + bringer + receiver + created + comment + statusVal + "</tr>";
        return tr;
    }

    function pagingControlMyFreight(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();