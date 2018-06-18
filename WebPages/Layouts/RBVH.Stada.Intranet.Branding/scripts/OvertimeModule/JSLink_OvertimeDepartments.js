(function () {
    var OvertimeDepartmentConfig = {
        ShiftManagement_Department: "Department",
        ShiftManagement_Requester: "Requester",
        ShiftManagement_Time: "Date",
        ApprovalStatusFieldDisplayName: "Approval Status",
        CreatedDate: "Created Date",
        Overtime_Location: 'Place',
        Locale: '',
        ListResourceFileName: "RBVHStadaLists",
        Container: "overtime-by-department-list-container"
    };
    (function () {
        var overrideCtx = {};
        overrideCtx.Templates = {};
        overrideCtx.Templates.Item = CustomItem;
        overrideCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCtx.BaseViewID = 3;
        overrideCtx.OnPostRender = PostRender_OvertimeByDepartment;
        overrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='overtime-bydep-title-header'>Title</th>" +
        "<th id='overtime-bydep-requester-header'>" + OvertimeDepartmentConfig.ShiftManagement_Requester + "</th>" +
        "<th id='overtime-bydep-department-header'>" + OvertimeDepartmentConfig.ShiftManagement_Department + "</th>" +
         "<th id='overtime-bydep-location-header'>" + OvertimeDepartmentConfig.Overtime_Location + "</th>" +
        "<th id='overtime-bydep-time-header'>" + OvertimeDepartmentConfig.ShiftManagement_Time + "</th>" +
        "<th id='overtime-bydep-approvalStatus-header'>" + OvertimeDepartmentConfig.ApprovalStatusFieldDisplayName + "</th>" +
        "<th id='overtime-bydep-createdDate-header'>" + OvertimeDepartmentConfig.CreatedDate + "</th>" +
        "<tr></thead><tbody>";
        overrideCtx.Templates.Footer = pagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
    })();
    function PostRender_OvertimeByDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            OvertimeDepartmentConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(OvertimeDepartmentConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + OvertimeDepartmentConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(OvertimeDepartmentConfig.ListResourceFileName, "Res", OnListResourcesReady);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + OvertimeDepartmentConfig.Container + ' .department-locale').each(function () {
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
        $('#' + OvertimeDepartmentConfig.Container + ' #overtime-bydep-department-header').text(Res.shiftManagement_Department);
        $('#' + OvertimeDepartmentConfig.Container + ' #overtime-bydep-requester-header').text(Res.shiftManagement_Requester);
        $('#' + OvertimeDepartmentConfig.Container + ' #overtime-bydep-title-header').text(Res.shiftManagement_Title);
        $('#' + OvertimeDepartmentConfig.Container + ' #overtime-bydep-time-header').text(Res.shiftManagement_Time);
        $('#' + OvertimeDepartmentConfig.Container + ' #overtime-bydep-approvalStatus-header').text(Res.approvalStatusFieldDisplayName);
        $('#' + OvertimeDepartmentConfig.Container + ' #overtime-bydep-location-header').text(Res.overtime_Location);
        $('#' + OvertimeDepartmentConfig.Container + ' .viewDetail').text(Res.shiftManagement_ViewDetail);
        $('#' + OvertimeDepartmentConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + OvertimeDepartmentConfig.Container + ' .label-danger').text(Res.approvalStatus_Rejected);
        $('#' + OvertimeDepartmentConfig.Container + ' .label-default').text(Res.approvalStatus_InProgress);
        $('#' + OvertimeDepartmentConfig.Container + ' #overtime-bydep-createdDate-header').text(Res.createdDate);
    }
    function CustomItem(ctx) {
        var currentUrl = window.location.href.split('#')[0];
        currentUrl += '#tab3';
        currentUrl = encodeURIComponent(currentUrl);

        var tr = "";
        var status = ctx.CurrentItem.ApprovalStatus;
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var Title = '<td><a  href="/SitePages/OverTimeRequest.aspx?subSection=OvertimeManagement&itemid=' + ctx.CurrentItem.ID + '&mode=view' + '&Source=' + currentUrl + '"   class="viewDetail" \>View Detail</a></td>';
        var Date = '<td> ' + ctx.CurrentItem.CommonDate + '</td>';
        var Department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var Location = '<td>' + ctx.CurrentItem.CommonLocation[0].lookupValue + '</td>';
        var createdDate = '<td>' + ctx.CurrentItem.Created + '</td>';

        if (status == 'true') {
            status = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status == 'false') {
            status = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else {
            status = '<td><span class="label label-default">In-Progress</span></td>';
        }
        tr = "<tr>" + Title + Requester + Department + Location + Date + status + createdDate + "</tr>";
        return tr;
    }
    function pagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
})();
