(function () {
    var LeaveHistoryConfig = {
        LeaveManagement_ViewDetail: "View Detail",
        LeaveManagement_Requester: "Requester",
        LeaveManagement_RequestFor: "Request for",
        LeaveManagement_Department: "Department",
        LeaveManagement_From: "From",
        LeaveManagement_To: "To",
        LeaveManagement_Reason: "Reason",
        LeaveManagement_LeaveHours: "Leave Hours",
        LeaveManagement_IsValid: "Is Valid",
        LeaveManagement_UnexpectedLeave: "Unexpected leave",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        Locale: '',
        Container: "leave-history-list-container",
    };
    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_LeaveHistory;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10004;
        overrideCSRDepartmentCtx.BaseViewID = 5;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_LeaveHistory;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requester_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_Requester + "</th>" +
            "<th id='requestFor_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_RequestFor + "</th>" +
            "<th id='department_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_Department + "</th>" +
            "<th id='from_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_From + "</th>" +
            "<th id='to_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_To + "</th>" +
            "<th id='leaveHours_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_LeaveHours + "</th>" +
            "<th id='reason_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_Reason + "</th>" +
            "<th id='unexpected_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_UnexpectedLeave + "</th>" +
            "<th id='isValid_leaveHistory'>" + LeaveHistoryConfig.LeaveManagement_IsValid + "</th>" +
            "<th></th>" +
        "</tr></thead><tbody>";
        overrideCSRDepartmentCtx.Templates.Footer = CreateFooter;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCSRDepartmentCtx);
    })();
    function CreateFooter(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function PostRender_LeaveHistory(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            LeaveHistoryConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(LeaveHistoryConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + LeaveHistoryConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(LeaveHistoryConfig.ListResourceFileName, "Res", OnListResourcesReady_LeaveHistory);
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + LeaveHistoryConfig.Container + ' .department-locale').each(function () {
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
    function OnListResourcesReady_LeaveHistory() {
        $('#requester_leaveHistory').text(Res.leaveList_Requester);
        $('#requestFor_leaveHistory').text(Res.leaveList_RequestFor);
        $('#department_leaveHistory').text(Res.leaveList_Department);
        $('#from_leaveHistory').text(Res.leaveList_From);
        $('#to_leaveHistory').text(Res.leaveList_To);
        $('#reason_leaveHistory').text(Res.leaveList_Reason);
        $('#unexpected_leaveHistory').text(Res.leaveList_UnexpectedLeave);
        $('#isValid_leaveHistory').text(Res.leaveList_IsValidRequest);
        $('#leaveHours_leaveHistory').text(Res.leaveList_LeaveHours);
    }
    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }
    function CustomItem_LeaveHistory(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var requestFor = '<td>' + ctx.CurrentItem.RequestFor[0].lookupValue + '</td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var from = '<td>' + ctx.CurrentItem.CommonFrom + '</td>';
        var to = '<td>' + ctx.CurrentItem.To + '</td>';
        var leaveHours = '<td>' + ctx.CurrentItem.LeaveHours + '</td>';
        var reason = '<td>' + ctx.CurrentItem.Reason + '</td>';

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

        tr = "<tr>" + requester + requestFor + department + from + to + leaveHours + reason + unexpectedLeave + isRequestValidTh + "</tr>";
        return tr;
    }
})();