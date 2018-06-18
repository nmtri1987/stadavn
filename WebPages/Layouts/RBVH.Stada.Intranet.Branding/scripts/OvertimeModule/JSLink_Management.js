(function () {
    var OvertimeRequestManagementConfig = {
        ShiftManagement_Department: "Department",
        ShiftManagement_Requester: "Requester",
        ShiftManagement_Time: "Date",
        ShiftManagement_EditTitle: "Edit",
        ShiftManagement_ViewTitle: "View",
        ApprovalStatusFieldDisplayName: "Approval Status",
        CreatedDate: "Created Date",
        Overtime_Location: 'Place',
        List_Title: 'Overtime Management',
        Locale: '',
        ListResourceFileName: "RBVHStadaLists",
        Container: "overtime-request-list-container"
    };
    (function () {
        var overrideCtx = {};
        overrideCtx.Templates = {};
        overrideCtx.BaseViewID = 4;
        overrideCtx.Templates.Item = OvertimeManagementCustomItem;
        overrideCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCtx.OnPostRender = PostRender_OvertimeRequest;
        overrideCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterTH'>" + OvertimeRequestManagementConfig.ShiftManagement_Requester + "</th>" +
        "<th id='departmentTH'>" + OvertimeRequestManagementConfig.ShiftManagement_Department + "</th>" +
        "<th id='location'>" + OvertimeRequestManagementConfig.Overtime_Location + "</th>" +
        "<th id='time'>" + OvertimeRequestManagementConfig.ShiftManagement_Time + "</th>" +
        "<th id='approvalStatus'>" + OvertimeRequestManagementConfig.ApprovalStatusFieldDisplayName + "</th>" +
        "<th id='createdDate'>" + OvertimeRequestManagementConfig.CreatedDate + "</th>" +
        "<th id='action'>" + OvertimeRequestManagementConfig.ShiftManagement_EditTitle + " | " + OvertimeRequestManagementConfig.ShiftManagement_ViewTitle + "</th>" +
        "</tr></thead><tbody>";
        overrideCtx.Templates.Footer = pagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
    })();
    function PostRender_OvertimeRequest(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            OvertimeRequestManagementConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(OvertimeRequestManagementConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + OvertimeRequestManagementConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(OvertimeRequestManagementConfig.ListResourceFileName, "Res", OnListResourcesReady);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + OvertimeRequestManagementConfig.Container + ' .department-locale').each(function () {
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
        $('#' + OvertimeRequestManagementConfig.Container + ' #departmentTH').text(Res.shiftManagement_Department);
        $('#' + OvertimeRequestManagementConfig.Container + ' #requesterTH').text(Res.shiftManagement_Requester);
        $('#' + OvertimeRequestManagementConfig.Container + ' #time').text(Res.shiftManagement_Time);
        $('#' + OvertimeRequestManagementConfig.Container + ' #action').text(Res.shiftManagement_EditTitle + " | " + Res.shiftManagement_ViewTitle);
        $('#' + OvertimeRequestManagementConfig.Container + ' #approvalStatus').text(Res.approvalStatusFieldDisplayName);
        $('#' + OvertimeRequestManagementConfig.Container + ' #location').text(Res.overtime_Location);
        $('#' + OvertimeRequestManagementConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + OvertimeRequestManagementConfig.Container + ' .label-danger').text(Res.approvalStatus_Rejected);
        $('#' + OvertimeRequestManagementConfig.Container + ' .label-default').text(Res.approvalStatus_InProgress);
        $('#' + OvertimeRequestManagementConfig.Container + ' #createdDate').text(Res.createdDate);
    }
    function OvertimeManagementCustomItem(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += "#tab2";
        sourceURL = encodeURIComponent(sourceURL);

        var edit = '<span><a  href="/SitePages/OvertimeRequest.aspx?subSection=OvertimeManagement&itemid=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="table-action" style="margin-left: 10px;"><i class="fa fa-pencil-square-o" aria-hidden="true"></i></a></span>';
        var view = '<span><a  href="/SitePages/OvertimeRequest.aspx?subSection=OvertimeManagement&itemid=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '&mode=view"   class="table-action" ><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        var Date = '<td> ' + ctx.CurrentItem.CommonDate + '</td>';
        var Department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var Location = '<td>' + ctx.CurrentItem.CommonLocation[0].lookupValue + '</td>';
        var status = ctx.CurrentItem.ApprovalStatus;
        var report = ''
        if (status == 'true') {
            edit = '<span><a class="table-action" style="margin-left: 10px;"><i class="fa fa-pencil-square-o" aria-hidden="true"></i></a></span>';
            report = getAttachments(OvertimeRequestManagementConfig.List_Title, ctx.CurrentItem.ID)
            status = '<td><span class="label label-success">Approved</span></td>';

        }
        else if (status == 'false') {
            edit = '<span><a class="table-action" style="margin-left: 10px;"><i class="fa fa-pencil-square-o" aria-hidden="true"></i></a></span>';
            status = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else {
            status = '<td><span class="label label-default">In-Progress</span></td>';
        }
        var action = "<td>" + edit + view + report + "</td>";
        var createdDate = "<td>" + ctx.CurrentItem.Created + "</td>";
        tr = "<tr>" + Requester + Department + Location + Date + status + createdDate + action + "</tr>";

        return tr;
    }
    function pagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function getAttachments(listName, itemId) {
        var url = _spPageContextInfo.webAbsoluteUrl;
        var requestUri = url + "/_api/web/lists/getbytitle('" + listName + "')/items(" + itemId + ")/AttachmentFiles";
        var str = "";
        $.ajax({
            url: requestUri,
            type: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            async: false,
            success: function (data) {
                for (var i = 0; i < data.d.results.length; i++) {
                    str += "<a target='_blank' href='" + data.d.results[i].ServerRelativeUrl + "'>" +
                            "<img border='0' width='16' src='/_layouts/15/images/attach16.png?rev=23'></a>";
                    if (i != data.d.results.length - 1) {
                        str += "<br/>";
                    }
                }
            },
            error: function (err) {
            }
        });
        return str;
    }
})();