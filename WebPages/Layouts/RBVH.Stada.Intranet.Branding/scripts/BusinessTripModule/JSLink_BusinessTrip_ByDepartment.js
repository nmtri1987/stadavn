(function () {
    var BusinessTripRequestByDepartmentConfig = {
        BusinessTripManagement_ViewDetail: "View Detail",
        BusinessTripManagement_Requester: "Requester",
        BusinessTripManagement_Department: "Department",
        BusinessTripManagement_PurposeTitle: "Purpose",
        BusinessTripManagement_BusinessTripTypeTitle: "Business trip type",
        BusinessTripManagement_Created: "Created",
        BusinessTripManagement_Comment: "Comment",
        BusinessTripManagement_ApprovalStatus: "Approval status",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        Container: "businesstrip-bydepartment-container",
    };

    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_BusinessTripDepartment;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10059;
        overrideCSRDepartmentCtx.BaseViewID = 4;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_BusinessTripByDepartment;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='viewDetailTH_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_ViewDetail + "</th>" +
            "<th id='requesterTH_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_Requester + "</th>" +
            "<th id='departmentTH_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_Department + "</th>" +
            "<th id='businessTripType_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_BusinessTripTypeTitle + "</th>" +
            "<th id='purpose_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_PurposeTitle + "</th>" +
            "<th id='createdTH_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_Created + "</th>" +
            "<th id='commentTH_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_Comment + "</th>" +
            "<th id='approvalStatusTH_businessTripByDept'>" + BusinessTripRequestByDepartmentConfig.BusinessTripManagement_ApprovalStatus + "</th>" +
            "<th></th>" +
        "</tr></thead><tbody>";
        overrideCSRDepartmentCtx.Templates.Footer = CreateFooter;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCSRDepartmentCtx);
    })();
    function CreateFooter(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function PostRender_BusinessTripByDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            BusinessTripRequestByDepartmentConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(BusinessTripRequestByDepartmentConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + BusinessTripRequestByDepartmentConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(BusinessTripRequestByDepartmentConfig.ListResourceFileName, "Res", OnListResourcesReady_BusinessTripDepartment);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + BusinessTripRequestByDepartmentConfig.Container + ' .department-locale').each(function () {
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
    function OnListResourcesReady_BusinessTripDepartment() {
        $('#viewDetailTH_businessTripByDept').text(Res.businessTripManagement_ViewDetail);
        $('#requesterTH_businessTripByDept').text(Res.businessTripManagement_RequesterTitle);
        $('#departmentTH_businessTripByDept').text(Res.overtime_Department);
        $('#businessTripType_businessTripByDept').text(Res.businessTripManagement_BusinessTripTypeTitle);
        $('#purpose_businessTripByDept').text(Res.businessTripManagement_PurposeTitle);
        $('#createdTH_businessTripByDept').text(Res.createdDate);
        $('#commentTH_businessTripByDept').text(Res.commonComment);
        $('#approvalStatusTH_businessTripByDept').text(Res.approval_Status);
        $('#' + BusinessTripRequestByDepartmentConfig.Container + ' .viewDetail').text(Res.businessTripManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        $('.internal-trip').html(Res.businessTripManagement_BusinessTripTypeInternalTitle);
        $('.external-trip').html(Res.businessTripManagement_BusinessTripTypeExternalTitle);
    }

    function CustomItem_BusinessTripDepartment(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var businessTripType = (ctx.CurrentItem["Domestic.value"] && ctx.CurrentItem["Domestic.value"] === "1") ? '<td class="internal-trip">' + '</td>' : '<td class="external-trip"></td>';
        var purpose = ctx.CurrentItem.BusinessTripPurpose != null ? '<td>' + ctx.CurrentItem.BusinessTripPurpose + '</td>' : '<td></td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';
        var status = ctx.CurrentItem.ApprovalStatus + "";
        status = status.toLowerCase();
        var statusVal = '';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab3';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/BusinessTripRequest.aspx?subSection=BusinessTripManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

        if (status == 'approved') {
            statusVal = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status == "cancelled") {
            statusVal = '<td><span class="label label-warning">Cancelled</span></td>';
        }
        else if (status == "rejected") {
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
})();