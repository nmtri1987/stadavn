(function () {
    var ShiftManagementDepartmentConfig = {
        ShiftManagement_Department: "Department",
        ShiftManagement_Location: "Location",
        ShiftManagement_Requester: "Requester",
        ShiftManagement_Month: "Month",
        ShiftManagement_Year: "Year",
        ShiftManagement_Approver: "Approver",
        Locale: '',
        ShiftManagement_ViewTitle: "View",
        ListResourceFileName: "RBVHStadaLists",
        Container: "shift-by-department-list-container"
    };
    (function () {
        var overrideShiftRequestByDepartmentCtx = {};
        overrideShiftRequestByDepartmentCtx.Templates = {};
        overrideShiftRequestByDepartmentCtx.ListTemplateType = 10010;
        overrideShiftRequestByDepartmentCtx.BaseViewID = 3;
        overrideShiftRequestByDepartmentCtx.Templates.Item = shiftManagementDepartmentCustomItem;
        overrideShiftRequestByDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideShiftRequestByDepartmentCtx.OnPostRender = PostRender_ShiftRequestByDepartment;
        overrideShiftRequestByDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterTHDept'>" + ShiftManagementDepartmentConfig.ShiftManagement_Requester + "</th>" +
        "<th id='departmentTHDept'>" + ShiftManagementDepartmentConfig.ShiftManagement_Department + "</th>" +
        "<th id='locationTHDept'>" + ShiftManagementDepartmentConfig.ShiftManagement_Location + "</th>" +
        "<th id='monthDept'>" + ShiftManagementDepartmentConfig.ShiftManagement_Month + "</th>" +
        "<th id='yearDept'>" + ShiftManagementDepartmentConfig.ShiftManagement_Year + "</th>" +
        "<th id='approverDept'>" + ShiftManagementDepartmentConfig.ShiftManagement_Approver + "</th>" +
        "<th id='actionDept'>" + ShiftManagementDepartmentConfig.ShiftManagement_ViewTitle + "</th>" +
        "</tr></thead><tbody>";
        overrideShiftRequestByDepartmentCtx.Templates.Footer = shiftManagementDepartmentPagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideShiftRequestByDepartmentCtx);
    })();
    function PostRender_ShiftRequestByDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            ShiftManagementDepartmentConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(ShiftManagementDepartmentConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ShiftManagementDepartmentConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ShiftManagementDepartmentConfig.ListResourceFileName, "Res", OnListShiftManagementDepartmentResourcesReady);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + ShiftManagementDepartmentConfig.Container + ' .department-locale').each(function () {
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
    function OnListShiftManagementDepartmentResourcesReady() {
        $('#' + ShiftManagementDepartmentConfig.Container + ' #requesterTHDept').text(Res.shiftManagement_Requester);
        $('#' + ShiftManagementDepartmentConfig.Container + ' #departmentTHDept').text(Res.shiftManagement_Department);
        $('#' + ShiftManagementDepartmentConfig.Container + ' #locationTHDept').text(Res.shiftManagement_Location);
        $('#' + ShiftManagementDepartmentConfig.Container + ' #monthDept').text(Res.shiftManagement_Month);
        $('#' + ShiftManagementDepartmentConfig.Container + ' #yearDept').text(Res.shiftManagement_Year);
        $('#' + ShiftManagementDepartmentConfig.Container + ' #approverDept').text(Res.shiftManagement_Approver);
        $('#' + ShiftManagementDepartmentConfig.Container + ' #actionDept').text(Res.shiftManagement_ViewTitle);
        $('#' + ShiftManagementDepartmentConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + ShiftManagementDepartmentConfig.Container + ' .label-default').text(Res.approvalStatus_InProgress);
    }
    function shiftManagementDepartmentCustomItem(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab3';
        sourceURL = encodeURIComponent(sourceURL);

        var edit = '<span><a  href="/SitePages/ShiftRequest.aspx?subSection=ShiftManagement&itemid=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="table-action" style="margin-left: 10px;"><i class="fa fa-pencil-square-o" aria-hidden="true"></i></a></span>';
        var view = '<span><a  href="/SitePages/ShiftRequest.aspx?subSection=ShiftManagement&itemid=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '&mode=view"   class="table-action"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        var action = "<td>" + view + "</td>";
        var Month = '<td> ' + ctx.CurrentItem.CommonMonth + '</td>';
        var yearValue = ctx.CurrentItem.CommonYear.replace('.', '');
        yearValue = yearValue.replace(',', '');
        var Year = '<td> ' + yearValue + '</td>';
        var Department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var location = '<td> ' + ctx.CurrentItem.CommonLocation[0].lookupValue + '</td>';
        var approverValue = !!ctx.CurrentItem.CommonApprover1 ? ctx.CurrentItem.CommonApprover1[0].title : '';
        var Approver = '<td> ' + approverValue + '</td>';
        
        tr = "<tr>" + Requester + Department + location + Month + Year + Approver + action + "</tr>";

        return tr;
    }
    function shiftManagementDepartmentPagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();