(function () {
    var ShiftManagementApprovalConfig = {
        ShiftManagement_Department: "Department",
        ShiftManagement_Location: "Location",
        ShiftManagement_Title: "Title",
        ShiftManagement_Requester: "Requester",
        ShiftManagement_ViewDetail: "View Detail",
        ShiftManagement_Time: "Time",
        ShiftManagement_Month: "Month",
        ShiftManagement_Year: "Year",
        Locale: '',
        ListResourceFileName: "RBVHStadaLists",
        Container: "shift-approval-list-container"
    };
    (function () {
        var overrideShiftApproveCtx = {};
        overrideShiftApproveCtx.Templates = {};

        overrideShiftApproveCtx.ListTemplateType = 10010;
        overrideShiftApproveCtx.BaseViewID = 2;
        overrideShiftApproveCtx.Templates.Item = shiftApproveCustomItem;
        overrideShiftApproveCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideShiftApproveCtx.OnPostRender = PostRender_ShiftApprove;
        overrideShiftApproveCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='titleTHApproval'>Title</th>" +
        "<th id='requesterTHApproval'>" + ShiftManagementApprovalConfig.ShiftManagement_Requester + "</th>" +
        "<th id='departmentTHApproval'>" + ShiftManagementApprovalConfig.ShiftManagement_Department + "</th>" +
        "<th id='locationTHApproval'>" + ShiftManagementApprovalConfig.ShiftManagement_Location + "</th>" +
        "<th id='month'>" + ShiftManagementApprovalConfig.ShiftManagement_Month + "</th>" +
        "<th id='year'>" + ShiftManagementApprovalConfig.ShiftManagement_Year + "</th>" +
        "<tr></thead><tbody>";
        overrideShiftApproveCtx.Templates.Footer = shiftApprovePagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideShiftApproveCtx);
    })();
    function PostRender_ShiftApprove(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            ShiftManagementApprovalConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(ShiftManagementApprovalConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ShiftManagementApprovalConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(ShiftManagementApprovalConfig.ListResourceFileName, "Res", OnListShiftApproveResourcesReady);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + ShiftManagementApprovalConfig.Container + ' .department-locale').each(function () {
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
    function OnListShiftApproveResourcesReady() {
        $('#' + ShiftManagementApprovalConfig.Container + ' #titleTHApproval').text(Res.shiftManagement_Title);
        $('#' + ShiftManagementApprovalConfig.Container + ' #requesterTHApproval').text(Res.shiftManagement_Requester);
        $('#' + ShiftManagementApprovalConfig.Container + ' #departmentTHApproval').text(Res.shiftManagement_Department);
        $('#' + ShiftManagementApprovalConfig.Container + ' #locationTHApproval').text(Res.shiftManagement_Location);
        $('#' + ShiftManagementApprovalConfig.Container + ' #month').text(Res.shiftManagement_Month);
        $('#' + ShiftManagementApprovalConfig.Container + ' #year').text(Res.shiftManagement_Year);
        $('#' + ShiftManagementApprovalConfig.Container + ' .viewDetail').text(Res.shiftManagement_ViewDetail);
        $('#' + ShiftManagementApprovalConfig.Container + ' .label-success').text(Res.approvalStatus_InProgress);
        $('#' + ShiftManagementApprovalConfig.Container + ' .label-default').text(Res.approvalStatus_Approved);
    }
    function shiftApproveCustomItem(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab2';

        sourceURL = encodeURIComponent(sourceURL);

        var Title = '<td><a  href="/SitePages/ShiftApproval.aspx?subSection=ShiftManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';
        var Month = '<td> ' + ctx.CurrentItem.CommonMonth + '</td>';
        var yearValue = ctx.CurrentItem.CommonYear.replace('.', '');
        yearValue = yearValue.replace(',', '');
        var Year = '<td> ' + yearValue + '</td>';
        var Department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var Location = '<td> ' + ctx.CurrentItem.CommonLocation[0].lookupValue + '</td>';

        tr = "<tr>" + Title + Requester + Department + Location + Month + Year + "</tr>";
        return tr;
    }
    function shiftApprovePagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();