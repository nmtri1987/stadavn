(function () {
    var FreightRequestSecurityConfig = {
        FreightManagement_ViewDetail: "View Detail",
        FreightManagement_Requester: "Requester",
        FreightManagement_Department: "Department",
        FreightManagement_Bringer: "Bringer",
        FreightManagement_Receiver: "Received by",
        FreightManagement_Created: "Created",
        FreightManagement_RequestNumber: "Request number",
        FreightManagement_Vehicle: "Vehicle",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        CompanyVehicle: "Company's Vehicle",
        Locale: '',
        Container: "freight-security-container",
    };
    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_FreightDepartment;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10015;
        overrideCSRDepartmentCtx.BaseViewID = 6;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_FreightDepartment;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='viewDetailTH_freightSec'>" + FreightRequestSecurityConfig.FreightManagement_ViewDetail + "</th>" +
            "<th id='requestNoTH_freightSec'>" + FreightRequestSecurityConfig.FreightManagement_RequestNumber + "</th>" +
            "<th id='requesterTH_freightSec'>" + FreightRequestSecurityConfig.FreightManagement_Requester + "</th>" +
            "<th id='departmentTH_freightSec'>" + FreightRequestSecurityConfig.FreightManagement_Department + "</th>" +
            "<th id='bringerTH_freightSec'>" + FreightRequestSecurityConfig.FreightManagement_Bringer + "</th>" +
            "<th id='receiverTH_freightSec'>" + FreightRequestSecurityConfig.FreightManagement_Receiver + "</th>" +
            "<th id='createdTH_freightSec'>" + FreightRequestSecurityConfig.FreightManagement_Created + "</th>" +
            "<th id='vehicle_freightSec' style='min-width:80px;'>" + FreightRequestSecurityConfig.FreightManagement_Vehicle + "</th>" +
            //"<th></th>" +
            "</tr></thead><tbody>";
        overrideCSRDepartmentCtx.Templates.Footer = CreateFooter;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCSRDepartmentCtx);
    })();
    function CreateFooter(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function PostRender_FreightDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            FreightRequestSecurityConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(FreightRequestSecurityConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + FreightRequestSecurityConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(FreightRequestSecurityConfig.ListResourceFileName, "Res", OnListResourcesReady_FreightSecurity);
            SP.SOD.registerSod(FreightRequestSecurityConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + FreightRequestSecurityConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(FreightRequestSecurityConfig.PageResourceFileName, "Res", OnPageResourcesReady_FreightSecurity);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + FreightRequestSecurityConfig.Container + ' .department-locale').each(function () {
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
    function OnListResourcesReady_FreightSecurity() {
        $('#viewDetailTH_freightSec').text(Res.freightManagement_ViewDetail);
        $('#requestNoTH_freightSec').text(Res.freightManagement_RequestNumber);
        $('#requesterTH_freightSec').text(Res.freightManagement_Requester);
        $('#departmentTH_freightSec').text(Res.overtime_Department);
        $('#bringerTH_freightSec').text(Res.freightManagement_Bringer);
        $('#receiverTH_freightSec').text(Res.freightManagement_Receiver);
        $('#createdTH_freightSec').text(Res.createdDate);
        $('#vehicle_freightSec').text(Res.freightManagement_Vehicle);
        FreightRequestSecurityConfig.ApprovalStatus_Cancelled = Res.approvalStatus_Cancelled;
        $('#' + FreightRequestSecurityConfig.Container + ' .viewDetail').text(Res.freightManagement_ViewDetail);
        FreightRequestSecurityConfig.CompanyVehicle = Res.freightManagement_CompanyVehicle;
        $('.companyvehicle').text(FreightRequestSecurityConfig.CompanyVehicle);
    }
    function OnPageResourcesReady_FreightSecurity() {
        FreightRequestSecurityConfig.ViewDetail = Res.viewDetail;
    }
    function CustomItem_FreightDepartment(ctx) {
        var tr = "";
        var currentID = ctx.CurrentItem.ID;
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
            bringer = '<td class="companyvehicle">' + FreightRequestSecurityConfig.CompanyVehicle + '</td>';
        }

        var receiver = ctx.CurrentItem.Receiver != null ? '<td>' + ctx.CurrentItem.Receiver + '</td>' : '<td></td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';

        var vehicleName = "";
        if (ctx.CurrentItem.CompanyVehicle && ctx.CurrentItem["CompanyVehicle.value"] && ctx.CurrentItem["CompanyVehicle.value"] === "1") {
            if (_spPageContextInfo.currentLanguage === 1033) {
                if (ctx.CurrentItem.VehicleLookup && ctx.CurrentItem.VehicleLookup.length > 0) {
                    vehicleName = ctx.CurrentItem.VehicleLookup[0].lookupValue;
                }
            }
            else {
                if (ctx.CurrentItem.VehicleVN && ctx.CurrentItem.VehicleVN.length > 0) {
                    vehicleName = ctx.CurrentItem.VehicleVN;
                }
            }
        }
        vehicleName = "<td>" + vehicleName + "</td>";

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab4';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

        var requestNo = ctx.CurrentItem.RequestNo != null ? '<td>' + ctx.CurrentItem.RequestNo + '</td>' : '<td></td>';

        tr = "<tr>" + title + requestNo + requester + department + bringer + receiver + created + vehicleName + "</tr>";
        return tr;
    }
})();