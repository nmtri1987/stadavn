(function () {
    var FreightRequestListConfig = {
        FreightManagement_ViewDetail: "View Detail",
        FreightManagement_Requester: "Requester",
        FreightManagement_Bringer: "Bringer",
        FreightManagement_Receiver: "Received by",
        FreightManagement_Created: "Created",
        FreightManagement_Comment: "Comment",
        FreightManagement_IsValid: "Is valid",
        FreightManagement_ApprovalStatus: "Approval status",
        FreightManagement_RequestNumber: "Request number",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        CompanyVehicle: "Company's Vehicle",
        Locale: '',
        Container: "freight-request-list-container",
    };

    (function () {
        var overrideFreightRequestCtx = {};
        overrideFreightRequestCtx.Templates = {};
        overrideFreightRequestCtx.Templates.Item = CustomItemFreightRequest;
        overrideFreightRequestCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };

        overrideFreightRequestCtx.ListTemplateType = 10015;
        overrideFreightRequestCtx.BaseViewID = 2;
        overrideFreightRequestCtx.OnPostRender = PostRender_FreightRequestList;
        overrideFreightRequestCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='freight-request-detail'>" + FreightRequestListConfig.FreightManagement_ViewDetail + "</th>" +
            "<th id='freight-request-requestNo'>" + FreightRequestListConfig.FreightManagement_RequestNumber + "</th>" +
            "<th id='freight-request-requester'>" + FreightRequestListConfig.FreightManagement_Requester + "</th>" +
            "<th id='freight-request-bringer'>" + FreightRequestListConfig.FreightManagement_Bringer + "</th>" +
            "<th id='freight-request-receiver'>" + FreightRequestListConfig.FreightManagement_Receiver + "</th>" +
            "<th id='freight-request-created'>" + FreightRequestListConfig.FreightManagement_Created + "</th>" +
            "<th id='freight-request-comment'>" + FreightRequestListConfig.FreightManagement_Comment + "</th>" +
            "<th id='freight-request-status'>" + FreightRequestListConfig.FreightManagement_ApprovalStatus + "</th>" +
            "<th id='freight-request-isValid'>" + FreightRequestListConfig.FreightManagement_IsValid + "</th>" +
            "<th id='freight-request-action'>" + '' + "</th>" +
            "<th></th>" +
            "</tr></thead><tbody>";
        overrideFreightRequestCtx.Templates.Footer = pagingControlFreightRequest;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideFreightRequestCtx);
    })();

    function PostRender_FreightRequestList(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            FreightRequestListConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(FreightRequestListConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + FreightRequestListConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(FreightRequestListConfig.ListResourceFileName, "Res", OnListResourcesReadyFreightRequest);
        }, "strings.js");
    }

    function OnListResourcesReadyFreightRequest() {
        $('.freight-request-cancel').click(function () {
            $(this).attr('disabled', 'true');
            var freightId = $(this).attr('data-id');
            var requestLink = window.location.protocol + "//{0}/_vti_bin/services/Freightmanagement/Freightmanagementservice.svc/CancelFreight/{1}",
            requestLink = RBVH.Stada.WebPages.Utilities.String.format(requestLink, location.host, freightId);
            if (freightId) {
                $.ajax({
                    type: "GET",
                    url: requestLink,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (resultData) {
                        if (resultData.Code === 6) {
                            alert(resultData.Message);
                        }
                        window.location.reload();
                    },
                    error: function (error) {
                        console.log("Error while cancelling this request");
                    }
                });
            }
        });

        $('#freight-request-detail').text(Res.freightManagement_ViewDetail);
        $('#freight-request-requestNo').text(Res.freightManagement_RequestNumber);
        $('#freight-request-requester').text(Res.freightManagement_Requester);
        $('#freight-request-bringer').text(Res.freightManagement_Bringer);
        $('#freight-request-receiver').text(Res.freightManagement_Receiver);
        $('#freight-request-created').text(Res.createdDate);
        $('#freight-request-comment').text(Res.commonComment);
        $('#freight-request-status').text(Res.freightManagement_ApprovalStatus);
        $('#freight-request-isValid').text(Res.freightList_IsValidRequest);
        $('#' + FreightRequestListConfig.Container + ' .viewDetail').text(Res.freightManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        $('.freight-request-cancel').text(Res.freightManagement_CancelRequest);
        FreightRequestListConfig.CompanyVehicle = Res.freightManagement_CompanyVehicle;
        $('.companyvehicle').text(FreightRequestListConfig.CompanyVehicle);
    }

    function CustomItemFreightRequest(ctx) {
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
            bringer = '<td class="companyvehicle">' + FreightRequestListConfig.CompanyVehicle + '</td>';
        }

        var receiver = ctx.CurrentItem.Receiver != null ? '<td>' + ctx.CurrentItem.Receiver + '</td>' : '<td></td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab1';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

        var status = ctx.CurrentItem.ApprovalStatus + "";
        status = status.toLowerCase();
        var statusVal = '';

        var disabled = '';
        if ((ctx.CurrentItem.Editor[0].id !== ctx.CurrentItem.Author[0].id) || (ctx.CurrentItem.Editor[0].id === ctx.CurrentItem.Author[0].id && status === 'cancelled')) {
            disabled = 'disabled';
        }
        var action = "<td><button type='button' class='btn btn-default btn-sm freight-request-cancel' " + disabled + " data-id='" + ctx.CurrentItem.ID + "'>Cancel Request</button></td>";

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

        var isRequestValidCss = "";
        var isRequestValidTh = "";
        if (ctx.CurrentItem["IsValidRequest.value"] && ctx.CurrentItem["IsValidRequest.value"] === "1") {
            isRequestValidTh = "<td><span style='margin-left:25%; ' class='glyphicon glyphicon-ok'></td>";
        }
        else {
            isRequestValidTh = "<td><span style='margin-left:25%;' class='glyphicon glyphicon-remove'></td>";
            isRequestValidCss = "style='background-color: #fff7e6;'";
        }
        var requestNo = ctx.CurrentItem.RequestNo != null ? '<td>' + ctx.CurrentItem.RequestNo + '</td>' : '<td></td>';
        tr = "<tr " + isRequestValidCss + " >" + title + requestNo + requester + bringer + receiver + created + comment + statusVal + isRequestValidTh + action + "</tr>";
        return tr;
    }

    function pagingControlFreightRequest(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();