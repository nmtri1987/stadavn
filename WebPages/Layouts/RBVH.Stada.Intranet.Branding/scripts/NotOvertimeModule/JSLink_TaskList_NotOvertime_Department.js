(function () {
    var NotOvertimeNotOvertimeApprovalTaskConfig = {
        NotOverTimeManagement_DepartmentName: "Department Name",
        NotOverTimeManagement_Requester: "Requester",
        NotOverTimeManagement_HoursPerDay: "Hours per Day",
        NotOverTimeManagement_To: "To",
        NotOverTimeManagement_From: "From",
        Comment: "Comment",
        NotOverTimeManagement_ApprovalStatus: "Approval Status",
        NotOverTimeManagement_Date: 'Date',
        NotOverTimeManagement_Reason: "Reason",
        NotOverTimeManagement_ListTitle: 'Leave Of Absence For Overtime Management',
        NotOvertimeManagement_ViewTitle: "View",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        ItemID: '',
        ApprovalStatus: '',
        IsManager: false,
        IsManagerServiceUrl: "/_vti_bin/Services/Employee/EmployeeService.svc/IsManager/",
        Locale: '',
        ViewDetail: "View item detail",
        Container: '#leaveOfAbsenceDepartmentContainer'
    };
    (function () {
        var overrideNotOvertimeByDepartmentCtx = {};
        overrideNotOvertimeByDepartmentCtx.Templates = {};
        overrideNotOvertimeByDepartmentCtx.Templates.Item = CustomItem;
        overrideNotOvertimeByDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideNotOvertimeByDepartmentCtx.ListTemplateType = 10007;
        overrideNotOvertimeByDepartmentCtx.BaseViewID = 2;
        overrideNotOvertimeByDepartmentCtx.OnPostRender = PostRenderNotOvertimeByDepartment;
        overrideNotOvertimeByDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
        "<thead><tr><th id='requesterApprovalDepartmentTH'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_Requester + "</th>" +
        "<th id='departmentApprovalDepartmentTH'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_DepartmentName + "</th>" +
         "<th id='hoursperdayApprovalDepartment'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_HoursPerDay + "</th>" +
        "<th id='dateApprovalDepartment'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_Date + "</th>" +
        "<th id='fromApprovalDepartment'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_From + "</th>" +
        "<th id='toApprovalDepartment'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_To + "</th>" +
        "<th id='reasonApprovalDepartment'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_Reason + "</th>" +
        "<th id='approvalStatusApprovalDepartment'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_ApprovalStatus + "</th>" +
        "<th id='viewDetailNOTApprovalDepartment'>" + NotOvertimeNotOvertimeApprovalTaskConfig.NotOvertimeManagement_ViewTitle + "</th>" +
        "<th></th>" +
        "</tr></thead><tbody>";
        overrideNotOvertimeByDepartmentCtx.Templates.Footer = pagingControl;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideNotOvertimeByDepartmentCtx);
    })();
    function openDialogBox(Url) {
        var ModalDialogOptions = { url: Url, width: 800, height: 400, showClose: true, allowMaximize: false, title: NotOvertimeNotOvertimeApprovalTaskConfig.ViewDetail };
        SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
    }
    function PostRenderNotOvertimeByDepartment(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            NotOvertimeNotOvertimeApprovalTaskConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(NotOvertimeNotOvertimeApprovalTaskConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + NotOvertimeNotOvertimeApprovalTaskConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(NotOvertimeNotOvertimeApprovalTaskConfig.ListResourceFileName, "Res", OnListResourcesReady);
            SP.SOD.registerSod(NotOvertimeNotOvertimeApprovalTaskConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + NotOvertimeNotOvertimeApprovalTaskConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(NotOvertimeNotOvertimeApprovalTaskConfig.PageResourceFileName, "Res", OnPageResourcesReady);
        }, "strings.js");

        $('.approve-request').click(function () {
            $(this).prop('disabled', true);
            approval_Task($(this).attr('data-id'), $(this).attr('data-approvalstatus'), $(this).attr('data-emp-id'));
        });
        $('.reject-request').click(function () {
            $(this).prop('disabled', true);
            reject_Task($(this).attr('data-id'), $(this));
        });
        $('.viewdetaildeprequest').click(function () {
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
                    $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' .department-locale').each(function () {
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
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#departmentApprovalDepartmentTH').text(Res.notOverTimeManagement_DepartmentName);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#requesterApprovalDepartmentTH').text(Res.notOverTimeManagement_Requester);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#hoursperdayApprovalDepartment').text(Res.notOverTimeManagement_HoursPerDay);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#approvalStatusApprovalDepartment').text(Res.notOverTimeManagement_ApprovalStatus);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#dateApprovalDepartment').text(Res.notOverTimeManagement_Date);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#fromApprovalDepartment').text(Res.notOverTimeManagement_From);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#toApprovalDepartment').text(Res.notOverTimeManagement_To);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#reasonApprovalDepartment').text(Res.notOverTimeManagement_Reason);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '#viewDetailNOTApprovalDepartment').text(Res.notOvertimeManagement_ViewTitle);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '.label-success').text(Res.approvalStatus_Approved);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '.label-default').text(Res.approvalStatus_InProgress);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '.label-warning').text(Res.approvalStatus_Cancelled);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '.label-danger').text(Res.approvalStatus_Rejected);
        $(NotOvertimeNotOvertimeApprovalTaskConfig.Container + ' ' + '.cancel-request').text(Res.notOverTimeManagement_CancelRequest);
    }
    function OnPageResourcesReady() {
        NotOvertimeNotOvertimeApprovalTaskConfig.ViewDetail = Res.viewDetail;
        $(".approve-request").text(Res.approveButton);
        $(".reject-request").text(Res.rejectButton);
    }
    function pad(n) {
        return (n < 10) ? ("0" + n) : n;
    }
    function CustomItem(ctx) {
        var tr = "";
        var Requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var date = '<td> ' + ctx.CurrentItem.CommonDate + '</td>';
        var From = '<td> ' + ctx.CurrentItem.CommonFrom + '</td>';
        var To = '<td> ' + ctx.CurrentItem.To + '</td>';
        var Department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var HoursPerDay = '<td>' + ctx.CurrentItem.HoursPerDay + '</td>';
        var Reason = '<td>' + ctx.CurrentItem.Reason + '</td>';
        var Comment = '<td><input type="text" class="form-control comment' + ctx.CurrentItem.ID + '" /></td>';
        var ApprovalStatus = '';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab3';
        sourceURL = encodeURIComponent(sourceURL);
        var viewDetail = '<span><a data-url="/Lists/NotOverTimeManagement/DispForm.aspx?ID=' + ctx.CurrentItem.ID + '&TextOnly=true&Source=' + sourceURL + '"   class="table-action viewdetaildeprequest"><i class="fa fa-eye" aria-hidden="true"></i></a></span>';
        viewDetail = "<td>" + viewDetail + "</td>";

        var status = ctx.CurrentItem.ApprovalStatus;
        if (status == 'Approved') {
            ApprovalStatus = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status == "Cancelled") {
            ApprovalStatus = '<td><span class="label label-warning">Cancelled</span></td>';
        }
        else if (status == "Rejected") {
            ApprovalStatus = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else {
            ApprovalStatus = '<td><span class="label label-default">In-Progress</span></td>';
        }
        tr = "<tr>" + Requester + Department + HoursPerDay + date + From + To + Reason + ApprovalStatus + viewDetail + "</tr>";
        return tr;
    }
    function pagingControl(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function approval_Task(itemId, currentStatus, employeeId) {
        checkIsCancelled(itemId);
        if (NotOvertimeNotOvertimeApprovalTaskConfig.ApprovalStatus != 'Cancelled') {
            NotOvertimeNotOvertimeApprovalTaskConfig.ItemID = itemId;
            var siteUrl = _spPageContextInfo.webServerRelativeUrl;
            var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
            var clientContext = new SP.ClientContext(fullWebUrl);
            var oList = clientContext.get_web().get_lists().getByTitle(String(NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_ListTitle));
            clientContext.load(oList);
            this.oListItem = oList.getItemById(itemId);
            if (currentStatus == '') {
                // Check current user is MANAGER: YES -> 
                this.checkIsManager(employeeId);

                var comment = $('.comment' + itemId).val();
                oListItem.set_item('CommonComment', comment);
                oListItem.set_item('ApprovalStatus', 'Approved');
                NotOvertimeNotOvertimeApprovalTaskConfig.ApprovalStatus = 'Approved';
            }

            oListItem.update();
            clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
        }
        else {
            alert(NotOvertimeNotOvertimeApprovalTaskConfig.ApproveCancelled);
        }
    }
    function reject_Task(itemId) {
        checkIsCancelled(itemId);
        NotOvertimeNotOvertimeApprovalTaskConfig.ItemID = itemId;
        if (NotOvertimeNotOvertimeApprovalTaskConfig.ApprovalStatus != 'Cancelled') {
            var siteUrl = _spPageContextInfo.webServerRelativeUrl;
            var fullWebUrl = window.location.protocol + '//' + window.location.host + siteUrl;
            var clientContext = new SP.ClientContext(fullWebUrl);
            var oList = clientContext.get_web().get_lists().getByTitle(String(NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_ListTitle));
            clientContext.load(oList);
            this.oListItem = oList.getItemById(itemId);
            var comment = $('.comment' + itemId).val();
            oListItem.set_item('CommonComment', comment);
            oListItem.set_item('ApprovalStatus', 'Rejected');
            oListItem.update();
            clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
        }
        else {
            alert(NotOvertimeNotOvertimeApprovalTaskConfig.ApproveCancelled);
        }
    }

    function checkIsCancelled(itemId) {
        var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(NotOvertimeNotOvertimeApprovalTaskConfig.NotOverTimeManagement_ListTitle) + "')/items(" + itemId + ")";
        var d = $.Deferred();
        $.ajax({
            url: url,
            method: "GET",
            async: false,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                NotOvertimeNotOvertimeApprovalTaskConfig.ApprovalStatus = data.d.ApprovalStatus;
                d.resolve(data.d);
            },
            error: function (data) {
                status = 'failed';
            }
        });
        return d.promise();
    }

    function checkIsManager(employeeId) {
        var url = _spPageContextInfo.webAbsoluteUrl + String(NotOvertimeNotOvertimeApprovalTaskConfig.IsManagerServiceUrl) + employeeId;
        var d = $.Deferred();
        $.ajax({
            url: url,
            type: "get",
            async: false,
            success: function (data) {
                NotOvertimeNotOvertimeApprovalTaskConfig.IsManager = data;
                d.resolve(data);
            },
            error: function () {
                return false;
            }
        });
        return d.promise();
    }
})();
function onQuerySucceeded() {
    location.reload();
}
function onQueryRejectSucceeded() {
    location.reload();
}
function onQueryFailed(sender, args) {
}

