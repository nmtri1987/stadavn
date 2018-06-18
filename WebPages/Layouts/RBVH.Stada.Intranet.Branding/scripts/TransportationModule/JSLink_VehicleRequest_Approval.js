(function () {
    var ApprovalTransportationConfig = {
        VehicleManagement_ViewDetail: "View Detail",
        VehicleManagement_Requester: "Requester",
        VehicleManagement_Department: "Department",
        VehicleManagement_VehicleType: 'Vehicle Type',
        VehicleManagement_CommonFrom: "From",
        VehicleManagement_CommonTo: "To",
        VehicleManagement_Reason: "Reason",
        VehicleManagement_ApprovalStatus: "Approval Status",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        Container: "vehicle-approval-container"
    };
    (function () {
        var overrideCtx = {};
        overrideCtx.Templates = {};
        overrideCtx.Templates.Item = CustomItem;
        overrideCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCtx.BaseViewID = 1;
        overrideCtx.ListTemplateType = 10013;
        overrideCtx.OnPostRender = PostRender;
        overrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='viewDetailTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_ViewDetail + "</th>" +
        "<th id='requesterTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_Requester + "</th>" +
        "<th id='departmentTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_Department + "</th>" +
        "<th id='vehicleTypeTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_VehicleType + "</th>" +
        "<th id='fromTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_CommonFrom + "</th>" +
        "<th id='toTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_CommonTo + "</th>" +
        "<th id='reasonTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_Reason + "</th>" +
        "<th id='approvalStatusTHVehicleApproval'>" + ApprovalTransportationConfig.VehicleManagement_ApprovalStatus + "</th>" +
        "</tr></thead><tbody>";
        overrideCtx.Templates.Footer = pagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
    })();
    function PostRender(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            ApprovalTransportationConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(ApprovalTransportationConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalTransportationConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ApprovalTransportationConfig.ListResourceFileName, "Res", OnListResourcesReady);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + ApprovalTransportationConfig.Container + ' .department-locale').each(function () {
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
    function OnListResourcesReady() {
        $('#viewDetailTHVehicleApproval').text(Res.vehicleManagement_ViewDetail);
        $('#requesterTHVehicleApproval').text(Res.vehicleManagement_Requester);
        $('#departmentTHVehicleApproval').text(Res.vehicleManagement_Department);
        $('#vehicleTypeTHVehicleApproval').text(Res.vehicleManagement_VehicleType);
        $('#fromTHVehicleApproval').text(Res.vehicleManagement_CommonFrom);
        $('#toTHVehicleApproval').text(Res.vehicleManagement_CommonTo);
        $('#reasonTHVehicleApproval').text(Res.vehicleManagement_Reason);
        $('#approvalStatusTHVehicleApproval').text(Res.vehicleManagement_ApprovalStatus);
        $('#' + ApprovalTransportationConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + ApprovalTransportationConfig.Container + ' .label-warning').text(Res.approvalStatus_Cancelled);
        $('#' + ApprovalTransportationConfig.Container + ' .label-danger').text(Res.approvalStatus_Rejected);
        $('#' + ApprovalTransportationConfig.Container + ' .viewDetail').text(Res.vehicleManagement_ViewDetail);
        $('#vehicle-approval-container tr').find('td:nth(3):contains("Company")').text(Res.vehicleManagement_VehicleType_Choice_Company);
        $('#vehicle-approval-container tr').find('td:nth(3):contains("Private")').text(Res.vehicleManagement_VehicleType_Choice_Private);
    }
    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }
    function CustomItem(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var vehicleType = '<td> ' + ctx.CurrentItem.VehicleType + '</td>';
        var from = '<td> ' + ctx.CurrentItem.CommonFrom + '</td>';
        var to = '<td> ' + ctx.CurrentItem.To + '</td>';
        var reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab2';
        sourceURL = encodeURIComponent(sourceURL);
        var viewDetail = '<td><a href="/Lists/VehicleManagement/EditForm.aspx?subSection=TransportationManagement&ID=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '" class="viewDetail" \>View Detail</a></td>';

        var status = ctx.CurrentItem.ApprovalStatus;
        if (status == 'Approved') {
            status = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status == "Cancelled") {
            status = '<td><span class="label label-warning">Cancelled</span></td>';
        }
        else if (status == "Rejected") {
            status = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else if (status && status.length > 0) {
            status = '<td><span class="label label-default">' + status + '</span></td>';
        }
        else {
            status = '<td><span class="label label-default">In-Progress</span></td>';
        }
        tr = "<tr>" + viewDetail + requester + department + vehicleType + from + to + reason + status + "</tr>";
        return tr;
    }
    function pagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();
