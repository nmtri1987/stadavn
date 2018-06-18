(function () {
    var LeaveRequestByDepartmentConfig = {
        LeaveManagement_ViewDetail: "View Detail",
        LeaveManagement_Requester: "Requester",
        LeaveManagement_RequestFor: "Request for",
        LeaveManagement_Department: "Department",
        LeaveManagement_From: "From",
        LeaveManagement_To: "To",
        Comment: "Comment",
        LeaveManagement_Reason: "Reason",
        LeaveManagement_LeaveHours: "Leave Hours",
        LeaveManagement_IsValid: "Is Valid",
        LeaveManagement_UnexpectedLeave: "Unexpected leave",
        LeaveManagement_ApprovalStatus: "Approval Status",
        ApprovalStatus_Cancelled: 'Cancelled',
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        Container: "leave-dept-list-container",
    };
    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_LeaveDepartment;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10004;
        overrideCSRDepartmentCtx.BaseViewID = 4;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_LeaveDepartment;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='viewDetail_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_ViewDetail + "</th>" +
            "<th id='requester_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_Requester + "</th>" +
            "<th id='requestFor_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_RequestFor + "</th>" +
            "<th id='department_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_Department + "</th>" +
            "<th id='from_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_From + "</th>" +
            "<th id='to_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_To + "</th>" +
            "<th id='leaveHours_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_LeaveHours + "</th>" +
            "<th id='reason_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_Reason + "</th>" +
            "<th id='comment_leaveDept'>" + LeaveRequestByDepartmentConfig.Comment + "</th>" +
            "<th id='unexpected_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_UnexpectedLeave + "</th>" +
            "<th id='isValid_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_IsValid + "</th>" +
            "<th id='approvalStatus_leaveDept'>" + LeaveRequestByDepartmentConfig.LeaveManagement_ApprovalStatus + "</th>" +
            "<th></th>" +
        "</tr></thead><tbody>";
        overrideCSRDepartmentCtx.Templates.Footer = CreateFooter;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCSRDepartmentCtx);
    })();
    function CreateFooter(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function PostRender_LeaveDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            LeaveRequestByDepartmentConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(LeaveRequestByDepartmentConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + LeaveRequestByDepartmentConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(LeaveRequestByDepartmentConfig.ListResourceFileName, "Res", OnListResourcesReady_LeaveDepartment);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + LeaveRequestByDepartmentConfig.Container + ' .department-locale').each(function () {
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
    function OnListResourcesReady_LeaveDepartment() {
        $('#viewDetail_leaveDept').text(Res.leaveList_ViewDetail);
        $('#requester_leaveDept').text(Res.leaveList_Requester);
        $('#requestFor_leaveDept').text(Res.leaveList_RequestFor);
        $('#department_leaveDept').text(Res.leaveList_Department);
        $('#from_leaveDept').text(Res.leaveList_From);
        $('#to_leaveDept').text(Res.leaveList_To);
        $('#reason_leaveDept').text(Res.leaveList_Reason);
        $('#comment_leaveDept').text(Res.commonComment);
        $('#unexpected_leaveDept').text(Res.leaveList_UnexpectedLeave);
        $('#isValid_leaveDept').text(Res.leaveList_IsValidRequest);
        $('#leaveHours_leaveDept').text(Res.leaveList_LeaveHours);
        $('#approvalStatus_leaveDept').text(Res.leaveList_ApprovalStatus);
        $('#' + LeaveRequestByDepartmentConfig.Container + ' .viewDetail').text(Res.leaveList_ViewDetail);
        $('#' + LeaveRequestByDepartmentConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + LeaveRequestByDepartmentConfig.Container + ' .label-warning').text(Res.approvalStatus_Cancelled);
        LeaveRequestByDepartmentConfig.ApprovalStatus_Cancelled = Res.approvalStatus_Cancelled;
        $('#' + LeaveRequestByDepartmentConfig.Container + ' .label-danger').text(Res.approvalStatus_Rejected);
    }
    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }
    function CustomItem_LeaveDepartment(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var requestFor = '<td>' + ctx.CurrentItem.RequestFor[0].lookupValue + '</td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var from = '<td>' + ctx.CurrentItem.CommonFrom + '</td>';
        var to = '<td>' + ctx.CurrentItem.To + '</td>';
        var leaveHours = '<td>' + ctx.CurrentItem.LeaveHours + '</td>';
        var reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';
        var status = ctx.CurrentItem.ApprovalStatus;

        //-------------
        var unexpectedLeave = "";
        if (ctx.CurrentItem["UnexpectedLeave.value"] && ctx.CurrentItem["UnexpectedLeave.value"] === "1") {
            unexpectedLeave = "<span class='glyphicon glyphicon-ok leavemanagement-item-ok'>";
        }
        var unexpectedLeave = '<td>' + unexpectedLeave + '</td>';
        //-------------
        var isRequestValidCss = "";
        var isRequestValidTh = "";
        if (ctx.CurrentItem["IsValidRequest.value"] && ctx.CurrentItem["IsValidRequest.value"] === "1") {
            isRequestValidTh = "<td><span style='margin-left:25%; ' class='glyphicon glyphicon-ok'></td>";
        }
        else {
            isRequestValidTh = "<td><span style='margin-left:25%;' class='glyphicon glyphicon-remove'></td>";
            isRequestValidCss = "style='background-color: #fff7e6;'";
        }

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab3';
        sourceURL = encodeURIComponent(sourceURL);
        var viewDetail = '<td><a href="/SitePages/LeaveRequest.aspx?subSection=LeaveManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

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
        tr = "<tr>" + viewDetail + requester + requestFor + department + from + to + leaveHours + reason + comment + unexpectedLeave + isRequestValidTh + status + "</tr>";
        return tr;
    }
})();