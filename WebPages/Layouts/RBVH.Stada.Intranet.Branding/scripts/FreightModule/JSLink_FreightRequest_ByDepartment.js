(function () {
    var FreightRequestByDepartmentConfig = {
        FreightManagement_ViewDetail: "View Detail",
        FreightManagement_Requester: "Requester",
        FreightManagement_Department: "Department",
        FreightManagement_Bringer: "Bringer",
        FreightManagement_Receiver: "Received by",
        FreightManagement_Created: "Created",
        FreightManagement_Comment: "Comment",
        FreightManagement_ApprovalStatus: "Approval status",
        FreightManagement_RequestNumber: "Request number",
        FreightManagement_Vehicle: "Vehicle",
        ApprovalStatus: '',
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        CompanyVehicle: "Company's Vehicle",
        Locale: '',
        Container: "freight-bydepartment-container",
    };

    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_FreightDepartment;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10015;
        overrideCSRDepartmentCtx.BaseViewID = 4;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_FreightDepartment;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='viewDetailTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_ViewDetail + "</th>" +
            "<th id='requestNoTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_RequestNumber + "</th>" +
            "<th id='requesterTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_Requester + "</th>" +
            "<th id='departmentTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_Department + "</th>" +
            "<th id='bringerTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_Bringer + "</th>" +
            "<th id='receiverTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_Receiver + "</th>" +
            "<th id='createdTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_Created + "</th>" +
            "<th id='commentTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_Comment + "</th>" +
            "<th id='isValid_freightDept'>" + FreightRequestByDepartmentConfig.LeaveManagement_IsValid + "</th>" +
            "<th id='approvalStatusTH_freightDept'>" + FreightRequestByDepartmentConfig.FreightManagement_ApprovalStatus + "</th>" +
            "<th id='vehicle_freightDept' style='min-width:80px;'>" + FreightRequestByDepartmentConfig.FreightManagement_Vehicle + "</th>" +
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
            FreightRequestByDepartmentConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(FreightRequestByDepartmentConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + FreightRequestByDepartmentConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(FreightRequestByDepartmentConfig.ListResourceFileName, "Res", OnListResourcesReady_FreightDepartment);
        }, "strings.js", "sp.js", "string.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + FreightRequestByDepartmentConfig.Container + ' .department-locale').each(function () {
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
    function OnListResourcesReady_FreightDepartment() {
        $('#viewDetailTH_freightDept').text(Res.freightManagement_ViewDetail);
        $('#requesterTH_freightDept').text(Res.freightManagement_Requester);
        $("#requestNoTH_freightDept").text(Res.freightManagement_RequestNumber);
        $('#departmentTH_freightDept').text(Res.overtime_Department);
        $('#bringerTH_freightDept').text(Res.freightManagement_Bringer);
        $('#receiverTH_freightDept').text(Res.freightManagement_Receiver);
        $('#createdTH_freightDept').text(Res.createdDate);
        $('#commentTH_freightDept').text(Res.commonComment);
        $('#isValid_freightDept').text(Res.freightList_IsValidRequest);
        $('#approvalStatusTH_freightDept').text(Res.freightManagement_ApprovalStatus);
        $('#vehicle_freightDept').text(Res.freightManagement_Vehicle);
        $('#' + FreightRequestByDepartmentConfig.Container + ' .viewDetail').text(Res.freightManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        FreightRequestByDepartmentConfig.CompanyVehicle = Res.freightManagement_CompanyVehicle;
        $('.companyvehicle').text(FreightRequestByDepartmentConfig.CompanyVehicle);
    }

    function CustomItem_FreightDepartment(ctx) {
        var lcid = _spPageContextInfo.currentLanguage;
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var requestNo = ctx.CurrentItem.RequestNo != null ? '<td>' + ctx.CurrentItem.RequestNo + '</td>' : '<td></td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var bringer = '<td></td>';
        if (ctx.CurrentItem.Bringer && ctx.CurrentItem.Bringer[0].lookupId > 0) {
            bringer = '<td>' + ctx.CurrentItem.Bringer[0].lookupValue + '</td>';
        }
        else if (ctx.CurrentItem.BringerName && ctx.CurrentItem.BringerName.length > 0) {
            bringer = '<td>' + ctx.CurrentItem.BringerName + '</td>';
        }
        else {
            bringer = '<td class="companyvehicle">' + FreightRequestByDepartmentConfig.CompanyVehicle + '</td>';
        }
        var receiver = ctx.CurrentItem.Receiver != null ? '<td>' + ctx.CurrentItem.Receiver + '</td>' : '<td></td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab3';
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
        var vehicleValue = "";
        if (ctx.CurrentItem.VehicleLookup[0]) {
            vehicleValue = ctx.CurrentItem.VehicleLookup[0].lookupValue;
        }
        if (lcid == 1066) {
            vehicleValue = ctx.CurrentItem.VehicleVN != null ? ctx.CurrentItem.VehicleVN : "";
        }
        var vehicle = '<td>' + vehicleValue + '</td>';

        var isRequestValidCss = "";
        var isRequestValidTh = "";
        if (ctx.CurrentItem["IsValidRequest.value"] && ctx.CurrentItem["IsValidRequest.value"] === "1") {
            isRequestValidTh = "<td><span style='margin-left:25%; ' class='glyphicon glyphicon-ok'></td>";
        }
        else {
            isRequestValidTh = "<td><span style='margin-left:25%;' class='glyphicon glyphicon-remove'></td>";
            isRequestValidCss = "style='background-color: #fff7e6;'";
        }

        tr = "<tr " + isRequestValidCss + " >" + title + requestNo + requester + department + bringer + receiver + created + comment + isRequestValidTh + statusVal + vehicle + "</tr>";
        return tr;
    }
})();