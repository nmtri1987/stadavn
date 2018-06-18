(function () {
    var BusinessTripDriverListConfig = {
        BusinessTripManagement_ViewDetail: "View Detail",
        BusinessTripManagement_Requester: "Requester",
        BusinessTripManagement_Department: "Department",
        BusinessTripManagement_BusinessTripTypeTitle: "Business trip type",
        BusinessTripManagement_PurposeTitle: "Purpose",
        BusinessTripManagement_Created: "Created",
        BusinessTripManagement_Comment: "Comment",
        BusinessTripManagement_ApprovalStatus: "Approval status",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        Container: "businessTrip-driver-container",
    };

    (function () {
        var overrideBusinessTripDriverCtx = {};
        overrideBusinessTripDriverCtx.Templates = {};
        overrideBusinessTripDriverCtx.Templates.Item = CustomItemBusinessTripDriver;
        overrideBusinessTripDriverCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };

        overrideBusinessTripDriverCtx.ListTemplateType = 10059;
        overrideBusinessTripDriverCtx.BaseViewID = 5;
        overrideBusinessTripDriverCtx.OnPostRender = PostRender_BusinessTripDriver;
        overrideBusinessTripDriverCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='businessTrip-driver-detail'>" + BusinessTripDriverListConfig.BusinessTripManagement_ViewDetail + "</th>" +
            "<th id='businessTrip-driver-requester'>" + BusinessTripDriverListConfig.BusinessTripManagement_Requester + "</th>" +
            "<th id='businessTrip-driver-department'>" + BusinessTripDriverListConfig.BusinessTripManagement_Department + "</th>" +
            "<th id='businessTrip-driver-businessTripType'>" + BusinessTripDriverListConfig.BusinessTripManagement_BusinessTripTypeTitle + "</th>" +
            "<th id='businessTrip-driver-purpose'>" + BusinessTripDriverListConfig.BusinessTripManagement_PurposeTitle + "</th>" +
            "<th id='businessTrip-driver-created'>" + BusinessTripDriverListConfig.BusinessTripManagement_Created + "</th>" +
            "<th id='businessTrip-driver-comment'>" + BusinessTripDriverListConfig.BusinessTripManagement_Comment + "</th>" +
            "<th id='businessTrip-driver-status'>" + BusinessTripDriverListConfig.BusinessTripManagement_ApprovalStatus + "</th>" +
            "<th></th>" +
            "</tr></thead><tbody>";
        overrideBusinessTripDriverCtx.Templates.Footer = pagingControlBusinessTripDriver;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideBusinessTripDriverCtx);
    })();

    function PostRender_BusinessTripDriver(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            BusinessTripDriverListConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(BusinessTripDriverListConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + BusinessTripDriverListConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(BusinessTripDriverListConfig.ListResourceFileName, "Res", OnListResourcesReadyBusinessTripDriver);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + BusinessTripDriverListConfig.Container + ' .department-locale').each(function () {
                        var id = $(this).attr('data-id');
                        var currentDepartment = $(this);
                        $(data).each(function (idx, obj) {
                            if (obj.Id.toString() === id) {
                                currentDepartment.text(obj.DepartmentName)
                            }
                        })
                    });
                }
            },
            error: function (data) {
                status = 'failed';
            }
        });
    }

    function OnListResourcesReadyBusinessTripDriver() {
        $('#businessTrip-driver-detail').text(Res.businessTripManagement_ViewDetail);
        $('#businessTrip-driver-requester').text(Res.businessTripManagement_RequesterTitle);
        $('#businessTrip-driver-department').text(Res.overtime_Department);
        $('#businessTrip-driver-businessTripType').text(Res.businessTripManagement_BusinessTripTypeTitle);
        $('#businessTrip-driver-purpose').text(Res.businessTripManagement_PurposeTitle);
        $('#businessTrip-driver-created').text(Res.createdDate);
        $('#businessTrip-driver-comment').text(Res.commonComment);
        $('#businessTrip-driver-status').text(Res.businessTripManagement_ApprovalStatus);
        $('#' + BusinessTripDriverListConfig.Container + ' .viewDetail').text(Res.businessTripManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        $('.internal-trip').html(Res.businessTripManagement_BusinessTripTypeInternalTitle);
        $('.external-trip').html(Res.businessTripManagement_BusinessTripTypeExternalTitle);
    }

    function CustomItemBusinessTripDriver(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';
        var businessTripType = (ctx.CurrentItem["Domestic.value"] && ctx.CurrentItem["Domestic.value"] === "1") ? '<td class="internal-trip">' + '</td>' : '<td class="external-trip"></td>';
        var purpose = ctx.CurrentItem.BusinessTripPurpose != null ? '<td>' + ctx.CurrentItem.BusinessTripPurpose + '</td>' : '<td></td>';
        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab5';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/BusinessTripRequest.aspx?subSection=BusinessTripManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

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

        tr = "<tr>" + title + requester + department + businessTripType + purpose + created + comment + statusVal + "</tr>";
        return tr;
    }

    function pagingControlBusinessTripDriver(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();