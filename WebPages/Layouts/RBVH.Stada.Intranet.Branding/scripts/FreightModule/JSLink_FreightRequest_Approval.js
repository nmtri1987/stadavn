(function () {
    var FreightRequestApprovalConfig = {
        FreightManagement_ViewDetail: "View Detail",
        FreightManagement_Requester: "Requester",
        FreightManagement_Department: "Department",
        FreightManagement_Bringer: "Bringer",
        FreightManagement_Receiver: "Received by",
        FreightManagement_Created: "Created",
        FreightManagement_ApprovalStatus: "Approval status",
        FreightManagement_RequestNumber: "Request number",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        CompanyVehicle: "Company's Vehicle",
        Locale: '',
        Container: "freight-approval-container",
    };

    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_FreightDepartment;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10015;
        overrideCSRDepartmentCtx.BaseViewID = 3;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_FreightDepartment;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='viewDetailTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_ViewDetail + "</th>" +
            "<th id='requestNoTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_RequestNumber + "</th>" +
            "<th id='requesterTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_Requester + "</th>" +
            "<th id='departmentTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_Department + "</th>" +
            "<th id='bringerTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_Bringer + "</th>" +
            "<th id='receiverTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_Receiver + "</th>" +
            "<th id='createdTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_Created + "</th>" +
            "<th id='approvalStatusTH_freightApproval'>" + FreightRequestApprovalConfig.FreightManagement_ApprovalStatus + "</th>" +
            "<th></th>" +
        "</tr></thead><tbody>";
        overrideCSRDepartmentCtx.Templates.Footer = CreateFooter;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCSRDepartmentCtx);
    })();
    function CreateFooter(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function PostRender_FreightDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            FreightRequestApprovalConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(FreightRequestApprovalConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + FreightRequestApprovalConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(FreightRequestApprovalConfig.ListResourceFileName, "Res", OnListResourcesReady_FreightApproval);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + FreightRequestApprovalConfig.Container + ' .department-locale').each(function () {
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
    function OnListResourcesReady_FreightApproval() {
        $('#viewDetailTH_freightApproval').text(Res.freightManagement_ViewDetail);
        $('#requesterTH_freightApproval').text(Res.freightManagement_Requester);
        $('#departmentTH_freightApproval').text(Res.overtime_Department);
        $('#bringerTH_freightApproval').text(Res.freightManagement_Bringer);
        $('#receiverTH_freightApproval').text(Res.freightManagement_Receiver);
        $('#requestNoTH_freightApproval').text(Res.freightManagement_RequestNumber);
        $('#createdTH_freightApproval').text(Res.createdDate);
        $('#approvalStatusTH_freightApproval').text(Res.freightManagement_ApprovalStatus);
        $('#' + FreightRequestApprovalConfig.Container + ' .viewDetail').text(Res.freightManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        FreightRequestApprovalConfig.CompanyVehicle = Res.freightManagement_CompanyVehicle;
        $('.companyvehicle').text(FreightRequestApprovalConfig.CompanyVehicle);
    }
    function CustomItem_FreightDepartment(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var bringer = '<td></td>';
        if (ctx.CurrentItem.Bringer && ctx.CurrentItem.Bringer[0].lookupId > 0) {
            bringer = '<td>' + ctx.CurrentItem.Bringer[0].lookupValue + '</td>';
        }
        else if (ctx.CurrentItem.BringerName && ctx.CurrentItem.BringerName.length > 0) {
            bringer = '<td>' + ctx.CurrentItem.BringerName + '</td>';
        }
        else {
            bringer = '<td class="companyvehicle">' + FreightRequestApprovalConfig.CompanyVehicle + '</td>';
        }

        var receiver = ctx.CurrentItem.Receiver != null ? '<td>' + ctx.CurrentItem.Receiver + '</td>' : '<td></td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab2';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

        var status = ctx.CurrentItem.ApprovalStatus + "";
        status = status.toLowerCase();
        var statusVal = '';
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
        var requestNo = ctx.CurrentItem.RequestNo != null ? '<td>' + ctx.CurrentItem.RequestNo + '</td>' : '<td></td>';
        tr = "<tr>" + title + requestNo + requester + department + bringer + receiver + created + statusVal + "</tr>";
        return tr;
    }
})();