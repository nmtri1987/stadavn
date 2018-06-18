RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.LeaveByDepartment = function (settings) {
    var locationPath = window.location.pathname;
    this.Settings = {
        Id: settings.Id,
        BeginDate: 21,
        DepartmentList: window.location.protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentsByLcid/{1}/{2}',
        ExportLeaves: window.location.protocol + '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/ExportLeaves/{1}/{2}/{3}/{4}',
        CurrentDepartmentId: 0,
        AllDepartments: "All/Tất cả",
        DeptIds:[]
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.LeaveByDepartment.prototype = {
    Initialize: function () {
        var that = this;
        $(document).ready(function () {
            ExecuteOrDelayUntilScriptLoaded(function () {
                that.InitControls();
                that.RegisterEvents();
            }, "sp.js");
        });
    },
    InitControls: function () {
        var that = this;
        $(that.Settings.FromDateControlSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
        $(that.Settings.ToDateControlSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
        
        if (that.Settings.DepartmentsToView && that.Settings.DepartmentsToView.length > 0) {
            that.Settings.DeptIds = that.Settings.DepartmentsToView.split('#');
        }

        that.PopulateDepartment()

        var FromDate = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminFromDate');
        var ToDate = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminToDate');
        var DeptId = that.Settings.CurrentDepartmentId > 0 ? that.Settings.CurrentDepartmentId : RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminDeptId');

        if (that.Settings.DeptIds && that.Settings.DeptIds.length > 1) {
            var DeptId = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminDeptId');
        }

        if (!!FromDate == false || !!ToDate == false || !!DeptId == false)
            that.RebindUrl(FromDate, ToDate, DeptId);
        else {
            that.Settings.DepartmentId = DeptId;
            that.Settings.FromDate = FromDate;
            that.Settings.ToDate = ToDate;
            $(that.Settings.DepartmentControlSelector).val(DeptId);

            $(that.Settings.FromDateControlSelector).val(FromDate);
            $(that.Settings.ToDateControlSelector).val(ToDate);

            if (that.Settings.CurrentDepartmentId > 0)
                $(that.Settings.DepartmentControlSelector).prop('disabled', true);

            if (that.Settings.CurrentDepartmentId > 0 && that.Settings.DeptIds && that.Settings.DeptIds.length > 1) {
                $(that.Settings.DepartmentControlSelector).prop('disabled', false);
            }
        }
    },

    RegisterEvents: function () {
        var that = this;
        $(that.Settings.FromDateControlSelector).on('changeDate', function (ev) {
            that.RebindUrl($(this).val(), that.Settings.ToDate, that.Settings.DepartmentId, true);
        });
        $(that.Settings.ToDateControlSelector).on('changeDate', function (ev) {
            that.RebindUrl(that.Settings.FromDate, $(this).val(), that.Settings.DepartmentId, true);
        });

        $(that.Settings.DepartmentControlSelector).on('change', function () {
            that.Settings.DepartmentId = $(this).val();
            that.RebindUrl(that.Settings.FromDate, that.Settings.ToDate, that.Settings.DepartmentId, true);
        });

        if ($(that.Settings.ExportControlSelector).length > 0) {
            $(that.Settings.ExportControlSelector).on('click', function () {
                var fromDateArray = $(that.Settings.FromDateControlSelector).val().split('/');
                var toDateArray = $(that.Settings.ToDateControlSelector).val().split('/');
                var from = fromDateArray[2] + '-' + fromDateArray[1] + '-' + fromDateArray[0];
                var to = toDateArray[2] + '-' + toDateArray[1] + '-' + toDateArray[0];
                var departmentId = that.Settings.DepartmentId;
                var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ExportLeaves, location.host, from, to, departmentId, _rbvhContext.EmployeeInfo.FactoryLocation.LookupId);
                window.location = url;
            });
        }
    },

    PopulateDepartment: function () {
        var that = this;
        var lcid = SP.Res.lcid;
        var locationId = _rbvhContext.EmployeeInfo != null ? _rbvhContext.EmployeeInfo.FactoryLocation.LookupId : 2;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.DepartmentList, location.host, lcid, locationId);
        $(that.Settings.DepartmentControlSelector).attr("disabled", false);
        $(that.Settings.DepartmentControlSelector).empty();
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                if (that.Settings.DeptIds && that.Settings.DeptIds.length > 1) {
                    $(result).each(function () {
                        if (that.Settings.DeptIds.indexOf(this.Id.toString()) > -1) {
                            $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', this.Id).text(this.DepartmentName));
                        }
                    });
                }
                else {
                    $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', "0").text(that.Settings.AllDepartments));
                    $(result).each(function () {
                        $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', this.Id).text(this.DepartmentName));
                    });
                    that.Settings.DepartmentId = $(that.Settings.DepartmentControlSelector).val();
                }
            }
        });
    },
    toISOStringTZ: function (dateObject) {
        var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
        var localISOTime = (new Date(dateObject.getTime() - tzoffset)).toISOString().slice(0, -1);
        return localISOTime;
    },
    RebindUrl: function (FromDate, ToDate, AdminDeptId, selected) {
        var that = this;
        var hashtag = window.location.hash;
        var url = window.location.href.split('#')[0];
        var date = new Date();
        var firstDay = FromDate;
        firstDay = typeof firstDay != 'undefined' ? firstDay : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth(), date.getDay()));
        var lastDay = ToDate;
        lastDay = typeof lastDay != 'undefined' ? lastDay : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth(), date.getDay()));

        var deptId = AdminDeptId;
        deptId = typeof deptId != 'undefined' ? deptId : '0';
        var paddingParam = '';
        if (url.indexOf('?') >= 0) {
            paddingParam = '&';
        }
        else {
            paddingParam = '?';
        }

        if (url.indexOf('AdminFromDate=') > 0)
            url = url.replace(/(AdminFromDate=)[^\&]+/, '$1' + firstDay);
        else
            url = url + paddingParam + 'AdminFromDate=' + firstDay;
        if (url.indexOf('AdminToDate=') > 0)
            url = url.replace(/(AdminToDate=)[^\&]+/, '$1' + lastDay);
        else
            url = url + '&AdminToDate=' + lastDay;
        if (url.indexOf('AdminDeptId=') > 0)
            url = url.replace(/(AdminDeptId=)[^\&]+/, '$1' + deptId);
        else
            url = url + '&AdminDeptId=' + deptId;

        if (url.indexOf('AdminDeptId=') > 0)
            url = url.replace(/(AdminDeptId=)[^\&]+/, '$1' + deptId);
        else
            url = url + paddingParam + 'AdminDeptId=' + deptId;

        if (!!selected) {
            url = ViewUtilities.Paging.RemovePagingURL(url);
        }

        if (hashtag && hashtag.length > 0) {
            url += hashtag;
        }
        window.location.href = url;
    }
};
