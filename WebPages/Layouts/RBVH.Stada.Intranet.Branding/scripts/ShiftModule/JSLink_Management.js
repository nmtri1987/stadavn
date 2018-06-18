(function () {
    var ShiftManagementConfig = {
        ShiftManagement_Department: "Department",
        ShiftManagement_Location: "Location",
        ShiftManagement_Requester: "Requester",
        ShiftManagement_Month: "Month",
        ShiftManagement_Year: "Year",
        ShiftManagement_Approver: "Approver",
        ShiftManagement_ModifiedBy: "Modified By",
        Locale: '',
        ShiftManagement_EditTitle: "Edit",
        ShiftManagement_ViewTitle: "View",
        ListResourceFileName: "RBVHStadaLists",
        Container: "shift-request-list-container"
    };
    (function () {
        var overrideShiftRequestCtx = {};
        overrideShiftRequestCtx.Templates = {};
        overrideShiftRequestCtx.ListTemplateType = 10010;
        overrideShiftRequestCtx.BaseViewID = 4;
        overrideShiftRequestCtx.Templates.Item = shiftRequestCustomItem;
        overrideShiftRequestCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideShiftRequestCtx.OnPostRender = PostRender_ShiftRequest;
        overrideShiftRequestCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterTH'>" + ShiftManagementConfig.ShiftManagement_Requester + "</th>" +
        "<th id='departmentTH'>" + ShiftManagementConfig.ShiftManagement_Department + "</th>" +
        "<th id='locationTH'>" + ShiftManagementConfig.ShiftManagement_Location + "</th>" +
        "<th id='month'>" + ShiftManagementConfig.ShiftManagement_Month + "</th>" +
        "<th id='year'>" + ShiftManagementConfig.ShiftManagement_Year + "</th>" +
        "<th id='approver'>" + ShiftManagementConfig.ShiftManagement_Approver + "</th>" +
        "<th id='modifiedBy'>" + ShiftManagementConfig.ShiftManagement_ModifiedBy + "</th>" +
        "<th id='action'>" + ShiftManagementConfig.ShiftManagement_EditTitle + " | " + ShiftManagementConfig.ShiftManagement_ViewTitle + "</th>" +
        "</tr></thead><tbody>";
        overrideShiftRequestCtx.Templates.Footer = shiftRequestPagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideShiftRequestCtx);
    })();
    function PostRender_ShiftRequest(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            ShiftManagementConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(ShiftManagementConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ShiftManagementConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ShiftManagementConfig.ListResourceFileName, "Res", OnListShiftRequestResourcesReady);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + ShiftManagementConfig.Container + ' .department-locale').each(function () {
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
    function OnListShiftRequestResourcesReady() {
        $('#' + ShiftManagementConfig.Container + ' #departmentTH').text(Res.shiftManagement_Department);
        $('#' + ShiftManagementConfig.Container + ' #locationTH').text(Res.shiftManagement_Location);
        $('#' + ShiftManagementConfig.Container + ' #requesterTH').text(Res.shiftManagement_Requester);
        $('#' + ShiftManagementConfig.Container + ' #month').text(Res.shiftManagement_Month);
        $('#' + ShiftManagementConfig.Container + ' #year').text(Res.shiftManagement_Year);
        $('#' + ShiftManagementConfig.Container + ' #approver').text(Res.shiftManagement_Approver);
        $('#' + ShiftManagementConfig.Container + ' #modifiedBy').text(Res.shiftManagement_ModifiedBy);
        $('#' + ShiftManagementConfig.Container + ' #action').text(Res.shiftManagement_EditTitle + " | " + Res.shiftManagement_ViewTitle);
        $('#' + ShiftManagementConfig.Container + ' #approvalStatus').text(Res.approvalStatusFieldDisplayName);
        $('#' + ShiftManagementConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + ShiftManagementConfig.Container + ' .label-default').text(Res.approvalStatus_InProgress);
    }
    function shiftRequestCustomItem(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var ModifiedBy = '<td>' + (ctx.CurrentItem.Editor.length > 0 ? ctx.CurrentItem.Editor[0].title : '') + '</td>';
        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab2';
        sourceURL = encodeURIComponent(sourceURL);

        var edit = '<span><a  href="/SitePages/ShiftRequest.aspx?itemid=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="table-action" style="margin-left: 10px;"><i class="fa fa-pencil-square-o" aria-hidden="true"></i></a></span>';
        var view = '<span><a  href="/SitePages/ShiftRequest.aspx?itemid=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '&mode=view"   class="table-action"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        var action = "<td>" + edit + view + "</td>";
        var Month = '<td> ' + ctx.CurrentItem.CommonMonth + '</td>';
        var yearValue = ctx.CurrentItem.CommonYear.replace('.', '');
        yearValue = yearValue.replace(',', '');
        var Year = '<td> ' + yearValue + '</td>';
        var Department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var location = '<td> ' + ctx.CurrentItem.CommonLocation[0].lookupValue + '</td>';
        var approverValue = !!ctx.CurrentItem.CommonApprover1 ? ctx.CurrentItem.CommonApprover1[0].title : '';
        var Approver = '<td> ' + approverValue + '</td>';
        tr = "<tr>" + Requester + Department + location + Month + Year + Approver + ModifiedBy + action + "</tr>";

        return tr;
    }
    function shiftRequestPagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();