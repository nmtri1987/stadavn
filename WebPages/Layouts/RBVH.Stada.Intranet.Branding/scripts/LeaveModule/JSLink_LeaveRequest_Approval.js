(function () {
    var LeaveRequestApprovalConfig = {
        LeaveManagement_ViewDetail: "View Detail",
        LeaveManagement_Requester: "Requester",
        LeaveManagement_RequestFor: "Request for",
        LeaveManagement_Department: "Department",
        LeaveManagement_From: "From",
        LeaveManagement_To: "To",
        LeaveManagement_LeaveHours: "Leave Hours",
        LeaveManagement_ApprovalStatus: "Approval Status",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        Container: "leave-approval-list-container",
    };

    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_LeaveDepartment;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10004;
        overrideCSRDepartmentCtx.BaseViewID = 3;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_LeaveDepartment;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='leave_approval_viewDetail'>" + LeaveRequestApprovalConfig.LeaveManagement_ViewDetail + "</th>" +
            "<th id='leaveapproval_requester'>" + LeaveRequestApprovalConfig.LeaveManagement_Requester + "</th>" +
            "<th id='leaveapproval_requestFor'>" + LeaveRequestApprovalConfig.LeaveManagement_RequestFor + "</th>" +
            "<th id='leaveapproval_department'>" + LeaveRequestApprovalConfig.LeaveManagement_Department + "</th>" +
            "<th id='leaveapproval_from'>" + LeaveRequestApprovalConfig.LeaveManagement_From + "</th>" +
            "<th id='leaveapproval_to'>" + LeaveRequestApprovalConfig.LeaveManagement_To + "</th>" +
            "<th id='leaveapproval_leaveHours'>" + LeaveRequestApprovalConfig.LeaveManagement_LeaveHours + "</th>" +
            "<th id='leaveapproval_approvalStatus'>" + LeaveRequestApprovalConfig.LeaveManagement_ApprovalStatus + "</th>" +
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
            LeaveRequestApprovalConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(LeaveRequestApprovalConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + LeaveRequestApprovalConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(LeaveRequestApprovalConfig.ListResourceFileName, "Res", OnListResourcesReady_LeaveDepartment);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + LeaveRequestApprovalConfig.Container + ' .department-locale').each(function () {
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
        $('#leave_approval_viewDetail').text(Res.leaveList_ViewDetail);
        $('#leaveapproval_requester').text(Res.leaveList_Requester);
        $('#leaveapproval_requestFor').text(Res.leaveList_RequestFor);
        $('#leaveapproval_department').text(Res.leaveList_Department);
        $('#leaveapproval_from').text(Res.leaveList_From);
        $('#leaveapproval_to').text(Res.leaveList_To);
        $('#leaveapproval_leaveHours').text(Res.leaveList_LeaveHours);
        $('#leaveapproval_approvalStatus').text(Res.leaveList_ApprovalStatus);
        $('#' + LeaveRequestApprovalConfig.Container + ' .viewDetail').text(Res.leaveList_ViewDetail);
        $('#' + LeaveRequestApprovalConfig.Container + ' .label-success').text(Res.approvalStatus_Approved);
        $('#' + LeaveRequestApprovalConfig.Container + ' .label-warning').text(Res.approvalStatus_Cancelled);
        $('#' + LeaveRequestApprovalConfig.Container + ' .label-danger').text(Res.approvalStatus_Rejected);
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
        var status = ctx.CurrentItem.ApprovalStatus;

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab2';
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
        tr = "<tr>" + viewDetail + requester + requestFor + department + from + to + leaveHours + status + "</tr>";
        return tr;
    }
})();