(function () {
    var NotOvertimeApprovalTaskConfig = {
        NotOverTimeManagement_DepartmentName: "Department Name",
        NotOverTimeManagement_Requester: "Requester",
        NotOverTimeManagement_HoursPerDay: "Hours per Day",
        NotOverTimeManagement_To: "To",
        NotOverTimeManagement_From: "From",
        Comment: "Comment",
        NotOverTimeManagement_Date: 'Date',
        NotOverTimeManagement_Reason: "Reason",
        NotOverTimeManagement_ListTitle: 'Leave Of Absence For Overtime Management',
        NotOvertimeManagement_ViewTitle: "View",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        ApproveRequest: '//{0}/_vti_bin/Services/NotOverTimeManagement/NotOverTimeManagementService.svc/Approve',
        RejectRequest: '//{0}/_vti_bin/Services/NotOverTimeManagement/NotOverTimeManagementService.svc/Reject',
        Locale: '',
        ViewDetail: "View item detail",
        Container: '#leaveOfAbsenceApprovalContainer'
    };
    (function () {
        var overrideNotOvertimeApprovalTaskCtx = {};
        overrideNotOvertimeApprovalTaskCtx.Templates = {};
        overrideNotOvertimeApprovalTaskCtx.Templates.Item = NotOvertimeApprovalTaskCustomItem;
        overrideNotOvertimeApprovalTaskCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideNotOvertimeApprovalTaskCtx.ListTemplateType = 10007;
        overrideNotOvertimeApprovalTaskCtx.BaseViewID = 4;
        overrideNotOvertimeApprovalTaskCtx.OnPostRender = PostRenderNotOvertimeApprovalTask;
        overrideNotOvertimeApprovalTaskCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOverTimeManagement_Requester + "</th>" +
        "<th id='departmentApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOverTimeManagement_DepartmentName + "</th>" +
         "<th id='hoursperdayApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOverTimeManagement_HoursPerDay + "</th>" +
        "<th id='dateApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOverTimeManagement_Date + "</th>" +
        "<th id='fromApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOverTimeManagement_From + "</th>" +
        "<th id='toApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOverTimeManagement_To + "</th>" +
        "<th id='reasonApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOverTimeManagement_Reason + "</th>" +
        "<th id='viewDetailNOTApprovalTH'>" + NotOvertimeApprovalTaskConfig.NotOvertimeManagement_ViewTitle + "</th>" +
        "<th id='commentApprovalTH'>" + NotOvertimeApprovalTaskConfig.Comment + "</th>" +
        "<th></th>" +
        "</tr></thead><tbody>";
        overrideNotOvertimeApprovalTaskCtx.Templates.Footer = pagingControlNotOvertimeApprovalTask;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideNotOvertimeApprovalTaskCtx);
    })();
    function openDialogBox(Url) {
        var ModalDialogOptions = { url: Url, width: 800, height: 400, showClose: true, allowMaximize: false, title: NotOvertimeApprovalTaskConfig.ViewDetail };
        SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
    }
    function PostRenderNotOvertimeApprovalTask(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            NotOvertimeApprovalTaskConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(NotOvertimeApprovalTaskConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + NotOvertimeApprovalTaskConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(NotOvertimeApprovalTaskConfig.ListResourceFileName, "Res", OnListResourcesReadyNotOvertimeApprovalTask);
            SP.SOD.registerSod(NotOvertimeApprovalTaskConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + NotOvertimeApprovalTaskConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(NotOvertimeApprovalTaskConfig.PageResourceFileName, "Res", OnPageResourcesReadyNotOvertimeApprovalTask);
        }, "strings.js");

        $('.approve-requestTask').click(function () {
            $(this).prop('disabled', true);
            DoApprove($(this).attr('data-id'));
        });
        $('.reject-requestTask').click(function () {
            $(this).prop('disabled', true);
            DoReject($(this).attr('data-id'));
        });
        $('.viewdetailapproval').click(function () {
            url = $(this).attr('data-url');
            openDialogBox(url);
        });

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $(NotOvertimeApprovalTaskConfig.Container + ' .department-localeTask').each(function () {
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
    function OnListResourcesReadyNotOvertimeApprovalTask() {
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#departmentApprovalTH').text(Res.notOverTimeManagement_DepartmentName);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#requesterApprovalTH').text(Res.notOverTimeManagement_Requester);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#hoursperdayApprovalTH').text(Res.notOverTimeManagement_HoursPerDay);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#approvalStatusApprovalTH').text(Res.notOverTimeManagement_ApprovalStatus);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#dateApprovalTH').text(Res.notOverTimeManagement_Date);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#fromApprovalTH').text(Res.notOverTimeManagement_From);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#toApprovalTH').text(Res.notOverTimeManagement_To);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#reasonApprovalTH').text(Res.notOverTimeManagement_Reason);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#viewDetailNOTApprovalTH').text(Res.notOvertimeManagement_ViewTitle);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '#commentApprovalTH').text(Res.commonComment);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '.label-success').text(Res.approvalStatus_Approved);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '.label-default').text(Res.approvalStatus_InProgress);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '.label-warning').text(Res.approvalStatus_Cancelled);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '.label-danger').text(Res.approvalStatus_Rejected);
        $(NotOvertimeApprovalTaskConfig.Container + ' ' + '.cancel-request').text(Res.notOverTimeManagement_CancelRequest);
    }

    function OnPageResourcesReadyNotOvertimeApprovalTask() {
        NotOvertimeApprovalTaskConfig.ViewDetail = Res.viewDetail;
        $(".approve-requestTask").text(Res.approveButton);
        $(".reject-requestTask").text(Res.rejectButton);
    }
    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }
    function NotOvertimeApprovalTaskCustomItem(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var date = '<td> ' + ctx.CurrentItem.CommonDate + '</td>';
        var From = '<td> ' + ctx.CurrentItem.CommonFrom + '</td>';
        var To = '<td> ' + ctx.CurrentItem.To + '</td>';
        var Department = '<td class="department-localeTask" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var HoursPerDay = '<td>' + ctx.CurrentItem.HoursPerDay + '</td>';
        var Reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
        var Comment = '<td><input type="text" class="form-control comment' + ctx.CurrentItem.ID + '" /></td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab2';
        sourceURL = encodeURIComponent(sourceURL);
        var viewDetail = '<span><a data-url="/Lists/NotOverTimeManagement/DispForm.aspx?ID=' + ctx.CurrentItem.ID + '&TextOnly=true&Source=' + sourceURL + '"   class="table-action viewdetailapproval"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        viewDetail = "<td>" + viewDetail + "</td>";

        var disabled = '';
        //if (ctx.CurrentItem.CommonReqDueDate && ctx.CurrentItem.CommonReqDueDate.length > 0) {
        //    var commonReqDueDate = ctx.CurrentItem.CommonReqDueDate;
        //    if (commonReqDueDate.indexOf(' ') > 0) {
        //        commonReqDueDate = commonReqDueDate.split(' ')[0];
        //        var requestDueDateObj = Functions.parseVietNameseDate(commonReqDueDate);
        //        var nowDate = new Date();
        //        var currentDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
        //        if (requestDueDateObj.valueOf() < currentDate.valueOf()) {
        //            disabled = 'disabled';
        //        }
        //    }
        //}

        var approve = "<button type='button' class='btn btn-success btn-sm approve-requestTask' data-approvalStatus='" + ctx.CurrentItem.ApprovalStatus + "'  data-id='" + ctx.CurrentItem.ID + "' data-emp-id='" + ctx.CurrentItem.Requester[0].lookupId + "' " + disabled + ">Approve</button>";
        var reject = "<button type='button' class='btn btn-default btn-sm reject-requestTask'  data-id='" + ctx.CurrentItem.ID + "' " + disabled + ">Reject</button>";
        var action = '<td>' + approve + '   ' + reject + '<td>'

        tr = "<tr>" + Requester + Department + HoursPerDay + date + From + To + Reason + viewDetail + Comment + action + "</tr>";
        return tr;
    }
    function pagingControlNotOvertimeApprovalTask(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }

    function DoApprove(itemId) {
        var postData = {};
        postData.Id = itemId;
        postData.Comment = $('.comment' + itemId).val();
        postData.ApproverName = _rbvhContext.EmployeeInfo.FullName;
        postData.ApproverId = _rbvhContext.EmployeeInfo.ADAccount.ID;
        var url = RBVH.Stada.WebPages.Utilities.String.format(NotOvertimeApprovalTaskConfig.ApproveRequest, location.host);
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response.Code === 2) {
                alert(response.Message);
            }
            Redirect();
        });
    }
    function DoReject(itemId) {
        var postData = {};
        postData.Id = itemId;
        postData.Comment = $('.comment' + itemId).val();
        postData.ApproverName = _rbvhContext.EmployeeInfo.FullName;
        postData.ApproverId = _rbvhContext.EmployeeInfo.ADAccount.ID;
        var url = RBVH.Stada.WebPages.Utilities.String.format(NotOvertimeApprovalTaskConfig.RejectRequest, location.host);
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response.Code === 3) {
                alert(response.Message);
            }
            Redirect();
        });
    }
    function Redirect() {
        var sourceParam = Functions.getParameterByName("Source");
        if (sourceParam) {
            Functions.redirectToSource();
        }
        else {
            window.location.reload();
        }
    }
})()
