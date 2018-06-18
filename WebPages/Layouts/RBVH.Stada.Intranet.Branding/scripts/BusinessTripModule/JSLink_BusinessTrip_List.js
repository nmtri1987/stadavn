(function () {
    var BusinessTripRequestListConfig = {
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
        Container: "businessTrip-request-list-container",
    };

    (function () {
        var overrideBusinessTripRequestListCtx = {};
        overrideBusinessTripRequestListCtx.Templates = {};
        overrideBusinessTripRequestListCtx.Templates.Item = CustomItemBusinessTripRequest;
        overrideBusinessTripRequestListCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };

        overrideBusinessTripRequestListCtx.ListTemplateType = 10059;
        overrideBusinessTripRequestListCtx.BaseViewID = 2;
        overrideBusinessTripRequestListCtx.OnPostRender = PostRender_BusinessTripRequestList;
        overrideBusinessTripRequestListCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='businessTrip-request-detail'>" + BusinessTripRequestListConfig.BusinessTripManagement_ViewDetail + "</th>" +
            "<th id='businessTrip-request-requester'>" + BusinessTripRequestListConfig.BusinessTripManagement_Requester + "</th>" +
            "<th id='businessTrip-request-businessTripType'>" + BusinessTripRequestListConfig.BusinessTripManagement_BusinessTripTypeTitle + "</th>" +
            "<th id='businessTrip-request-purpose'>" + BusinessTripRequestListConfig.BusinessTripManagement_PurposeTitle + "</th>" +
            "<th id='businessTrip-request-created'>" + BusinessTripRequestListConfig.BusinessTripManagement_Created + "</th>" +
            "<th id='businessTrip-request-comment'>" + BusinessTripRequestListConfig.BusinessTripManagement_Comment + "</th>" +
            "<th id='businessTrip-request-status'>" + BusinessTripRequestListConfig.BusinessTripManagement_ApprovalStatus + "</th>" +
            "<th id='businessTrip-request-action'>" + '' + "</th>" +
            "<th></th>" +
            "</tr></thead><tbody>";
        overrideBusinessTripRequestListCtx.Templates.Footer = pagingControlBusinessTripRequest;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideBusinessTripRequestListCtx);
    })();

    function PostRender_BusinessTripRequestList(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            BusinessTripRequestListConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(BusinessTripRequestListConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + BusinessTripRequestListConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(BusinessTripRequestListConfig.ListResourceFileName, "Res", OnListResourcesReadyBusinessTripRequest);
        }, "strings.js");
    }

    function OnListResourcesReadyBusinessTripRequest() {
        $('.businessTrip-request-cancel').click(function () {
            $(this).attr('disabled', 'true');
            var businessTripId = $(this).attr('data-id');
            var requestLink = window.location.protocol + "//{0}/_vti_bin/services/BusinessTripManagement/BusinessTripManagementService.svc/CancelBusinessTrip/{1}",
            requestLink = RBVH.Stada.WebPages.Utilities.String.format(requestLink, location.host, businessTripId);
            if (businessTripId) {
                $.ajax({
                    type: "GET",
                    url: requestLink,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (resultData) {
                        if (resultData.Code === 5) {
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

        $('#businessTrip-request-detail').text(Res.businessTripManagement_ViewDetail);
        $('#businessTrip-request-requester').text(Res.businessTripManagement_RequesterTitle);
        $('#businessTrip-request-businessTripType').text(Res.businessTripManagement_BusinessTripTypeTitle);
        $('#businessTrip-request-purpose').text(Res.businessTripManagement_PurposeTitle);
        $('#businessTrip-request-created').text(Res.createdDate);
        $('#businessTrip-request-comment').text(Res.commonComment);
        $('#businessTrip-request-status').text(Res.businessTripManagement_ApprovalStatus);
        $('#' + BusinessTripRequestListConfig.Container + ' .viewDetail').text(Res.businessTripManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        $('.businessTrip-request-cancel').text(Res.businesstripManagement_CancelRequest);
        $('.internal-trip').html(Res.businessTripManagement_BusinessTripTypeInternalTitle);
        $('.external-trip').html(Res.businessTripManagement_BusinessTripTypeExternalTitle);
    }

    function CustomItemBusinessTripRequest(ctx) {
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

        var disabled = '';
        if ((ctx.CurrentItem.Editor[0].id !== ctx.CurrentItem.Author[0].id) || (ctx.CurrentItem.Editor[0].id === ctx.CurrentItem.Author[0].id && (status === 'approved' || status === 'cancelled' || status === 'rejected'))) {
            disabled = 'disabled';
        }
        var action = "<td><button type='button' class='btn btn-default btn-sm businessTrip-request-cancel' " + disabled + " data-id='" + ctx.CurrentItem.ID + "'>Cancel Request</button></td>";

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

        tr = "<tr>" + title + requester + businessTripType + purpose + created + comment + statusVal + action + "</tr>";
        return tr;
    }

    function pagingControlBusinessTripRequest(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();