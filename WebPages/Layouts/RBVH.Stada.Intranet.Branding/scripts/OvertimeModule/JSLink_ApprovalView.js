(function () {
    var OvertimeApprovalConfig = {
        ShiftManagement_Department: "Department",
        ShiftManagement_Requester: "Requester",
        ShiftManagement_Time: "Date",
        ApprovalStatusFieldDisplayName: "Approval Status",
        CreatedDate: "Created Date",
        Overtime_Location: 'Place',
        Locale: '',
        ListResourceFileName: "RBVHStadaLists",
        Container: "overtime-approval-list-container"
    };
    (function () {
        var overrideOvertimeApprovalCtx = {};
        overrideOvertimeApprovalCtx.Templates = {};
        overrideOvertimeApprovalCtx.Templates.Item = OvertimeApproveCustomItem;
        overrideOvertimeApprovalCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideOvertimeApprovalCtx.BaseViewID = 2;
        overrideOvertimeApprovalCtx.OnPostRender = PostRender_OvertimeApproval;
        overrideOvertimeApprovalCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='title-approval-header'>Title</th>" +
        "<th id='requester-approval-header'>" + OvertimeApprovalConfig.ShiftManagement_Requester + "</th>" +
        "<th id='department-approval-header'>" + OvertimeApprovalConfig.ShiftManagement_Department + "</th>" +
         "<th id='location-approval-header'>" + OvertimeApprovalConfig.Overtime_Location + "</th>" +
        "<th id='time-approval-header'>" + OvertimeApprovalConfig.ShiftManagement_Time + "</th>" +
        "<th id='approvalStatus-approval-header'>" + OvertimeApprovalConfig.ApprovalStatusFieldDisplayName + "</th>" +
        "<th id='createdDate-approval-header'>" + OvertimeApprovalConfig.CreatedDate + "</th>" +
        "<tr></thead><tbody>";
        overrideOvertimeApprovalCtx.Templates.Footer = pagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideOvertimeApprovalCtx);
    })();
    function PostRender_OvertimeApproval(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            OvertimeApprovalConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(OvertimeApprovalConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + OvertimeApprovalConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(OvertimeApprovalConfig.ListResourceFileName, "Res", OnListResourcesReady);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + OvertimeApprovalConfig.Container + ' .department-locale').each(function () {
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
        $('#' + OvertimeApprovalConfig.Container + ' #title-approval-header').text(Res.shiftManagement_Title);
        $('#' + OvertimeApprovalConfig.Container + ' #requester-approval-header').text(Res.shiftManagement_Requester);
        $('#' + OvertimeApprovalConfig.Container + ' #department-approval-header').text(Res.shiftManagement_Department);
        $('#' + OvertimeApprovalConfig.Container + ' #location-approval-header').text(Res.overtime_Location);
        $('#' + OvertimeApprovalConfig.Container + ' #time-approval-header').text(Res.shiftManagement_Time);
        $('#' + OvertimeApprovalConfig.Container + ' #approvalStatus-approval-header').text(Res.approvalStatusFieldDisplayName);
        $('#' + OvertimeApprovalConfig.Container + ' #createdDate-approval-header').text(Res.createdDate);
        $('#' + OvertimeApprovalConfig.Container + ' .viewDetail').text(Res.shiftManagement_ViewDetail);
        $('#' + OvertimeApprovalConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + OvertimeApprovalConfig.Container + ' .label-danger').text(Res.approvalStatus_Rejected);
        $('#' + OvertimeApprovalConfig.Container + ' .label-default').text(Res.approvalStatus_InProgress);
    }
    function OvertimeApproveCustomItem(ctx) {
        var tr = "";
        var status = ctx.CurrentItem.ApprovalStatus;
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab2';
        sourceURL = encodeURIComponent(sourceURL);

        var Title = '<td><a  href="/SitePages/OverTimeApproval.aspx?subSection=OvertimeManagement&itemid=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + (status == 'true' ? '&mode=view' : '') + '"   class="viewDetail" \>View Detail</a></td>';
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