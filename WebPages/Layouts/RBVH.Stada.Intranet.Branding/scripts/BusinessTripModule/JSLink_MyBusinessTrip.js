(function () {
    var MyBusinessTripListConfig = {
        BusinessTripManagement_ViewDetail: "View Detail",
        BusinessTripManagement_Requester: "Requester",
        BusinessTripManagement_PurposeTitle: "Purpose",
        BusinessTripManagement_BusinessTripTypeTitle: "Business trip type",
        BusinessTripManagement_Created: "Created",
        BusinessTripManagement_Comment: "Comment",
        BusinessTripManagement_ApprovalStatus: "Approval status",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        Container: "my-business-trip-container",
    };

    (function () {
        var overrideMyBusinessTripCtx = {};
        overrideMyBusinessTripCtx.Templates = {};
        overrideMyBusinessTripCtx.Templates.Item = CustomItemMyBusinessTrip;
        overrideMyBusinessTripCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };

        overrideMyBusinessTripCtx.ListTemplateType = 10059;
        overrideMyBusinessTripCtx.BaseViewID = 7;
        overrideMyBusinessTripCtx.OnPostRender = PostRender_MyBusinessTrip;
        overrideMyBusinessTripCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='myBusinessTrip-detail'>" + MyBusinessTripListConfig.BusinessTripManagement_ViewDetail + "</th>" +
            "<th id='myBusinessTrip-requester'>" + MyBusinessTripListConfig.BusinessTripManagement_Requester + "</th>" +
            "<th id='myBusinessTrip-businessTripType'>" + MyBusinessTripListConfig.BusinessTripManagement_BusinessTripTypeTitle + "</th>" +
            "<th id='myBusinessTrip-purpose'>" + MyBusinessTripListConfig.BusinessTripManagement_PurposeTitle + "</th>" +
            "<th id='myBusinessTrip-created'>" + MyBusinessTripListConfig.BusinessTripManagement_Created + "</th>" +
            "<th id='myBusinessTrip-comment'>" + MyBusinessTripListConfig.BusinessTripManagement_Comment + "</th>" +
            "<th id='myBusinessTrip-status'>" + MyBusinessTripListConfig.BusinessTripManagement_ApprovalStatus + "</th>" +
            "<th id='myBusinessTrip-action'>" + '' + "</th>" +
            "<th></th>" +
            "</tr></thead><tbody>";
        overrideMyBusinessTripCtx.Templates.Footer = pagingControlMyBusinessTrip;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideMyBusinessTripCtx);
    })();

    function PostRender_MyBusinessTrip(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            MyBusinessTripListConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(MyBusinessTripListConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + MyBusinessTripListConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(MyBusinessTripListConfig.ListResourceFileName, "Res", OnListResourcesReadyMyBusinessTrip);
        }, "strings.js");
    }

    function OnListResourcesReadyMyBusinessTrip() {
        $('#myBusinessTrip-detail').text(Res.businessTripManagement_ViewDetail);
        $('#myBusinessTrip-requester').text(Res.businessTripManagement_RequesterTitle);
        $('#myBusinessTrip-businessTripType').text(Res.businessTripManagement_BusinessTripTypeInternalTitle);
        $('#myBusinessTrip-purpose').text(Res.businessTripManagement_PurposeTitle);
        $('#myBusinessTrip-created').text(Res.createdDate);
        $('#myBusinessTrip-comment').text(Res.commonComment);
        $('#myBusinessTrip-status').text(Res.businessTripManagement_ApprovalStatus);
        $('#' + MyBusinessTripListConfig.Container + ' .viewDetail').text(Res.businessTripManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        $('.internal-trip').html(Res.businessTripManagement_BusinessTripTypeInternalTitle);
        $('.external-trip').html(Res.businessTripManagement_BusinessTripTypeExternalTitle);
    }
    
    function CustomItemMyBusinessTrip(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';
        var businessTripType = (ctx.CurrentItem["Domestic.value"] && ctx.CurrentItem["Domestic.value"] === "1") ? '<td class="internal-trip">' + '</td>' : '<td class="external-trip"></td>';
        var purpose = ctx.CurrentItem.BusinessTripPurpose != null ? '<td>' + ctx.CurrentItem.BusinessTripPurpose + '</td>' : '<td></td>';
        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab1';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/BusinessTripRequest.aspx?subSection=BusinessTripManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

        var status = ctx.CurrentItem.ApprovalStatus + "";
        status = status.toLowerCase();
        var statusVal = '';
        var disabled = '';
        if ((ctx.CurrentItem.Editor[0].id !== ctx.CurrentItem.Author[0].id) || (ctx.CurrentItem.Editor[0].id === ctx.CurrentItem.Author[0].id && status === 'cancelled')) {
            disabled = 'disabled';
        }

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

        tr = "<tr>" + title + requester + businessTripType + purpose + created + comment + statusVal + "</tr>";
        return tr;
    }

    function pagingControlMyBusinessTrip(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();