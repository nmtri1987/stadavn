RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.NotOvertimeByDepartment = function (settings) {
    var locationPath = window.location.pathname;
    this.Settings = {
        Id: settings.Id,
        BeginDate: 21,
        DepartmentList: window.location.protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentsByLcid/{1}/{2}',
        CurrentDepartmentId: 0,
        AllDepartments: "All/Tất cả"
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.NotOvertimeByDepartment.prototype = {
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
        $(that.Settings.MonthControlSelector).datepicker({
            viewMode: "months",
            minViewMode: "months",
            format: "mm/yyyy",
            autoclose: true
        });
        that.PopulateDepartment()
        var DeptId = that.Settings.CurrentDepartmentId > 0 ? that.Settings.CurrentDepartmentId : RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminDeptId');

        var StartMonth = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminStartMonth');
        var EndMonth = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminEndMonth');

        if (!!StartMonth == false || !!EndMonth == false || !!DeptId == false)
            that.RebindUrl(StartMonth, EndMonth, DeptId);
        else {
            that.Settings.DepartmentId = DeptId;
            that.Settings.StartMonth = StartMonth;
            that.Settings.EndMonth = EndMonth;
            $(that.Settings.DepartmentControlSelector).val(DeptId);

            var startMonthTimeSpan = Date.parse(StartMonth);
            if (startMonthTimeSpan != NaN) {
                var startMonth = new Date(startMonthTimeSpan);
                var currentMonth = startMonth.getMonth() + 1;
                var currentYear = startMonth.getFullYear();
                $(that.Settings.MonthControlSelector).val(currentMonth + '/' + currentYear);
            }

            if (that.Settings.CurrentDepartmentId > 0)
                $(that.Settings.DepartmentControlSelector).prop('disabled', true);
        }
    },

    RegisterEvents: function () {
        var that = this;
        $(that.Settings.MonthControlSelector).on('changeDate', function (ev) {
            var dpMonth = ev.date.getMonth(); // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            $('.datepicker').hide();
            var firstDayDp = new Date(dpYear, dpMonth, 1);
            var lastDayDp = new Date(dpYear, dpMonth + 1, 0, 23, 59, 59);
            that.RebindUrl(that.toISOStringTZ(firstDayDp), that.toISOStringTZ(lastDayDp), that.Settings.DepartmentId, true);
            //__doPostBack('', (dpMonth + 1) + '/' + dpYear);
        });

        $(that.Settings.DepartmentControlSelector).on('change', function () {
            that.Settings.DepartmentId = $(this).val();
            that.RebindUrl(that.Settings.StartMonth, that.Settings.EndMonth, that.Settings.DepartmentId, true);
        });
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
                $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', "0").text(that.Settings.AllDepartments));
                $(result).each(function () {
                    $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', this.Id).text(this.DepartmentName));
                });
                that.Settings.DepartmentId = $(that.Settings.DepartmentControlSelector).val();
            }
        });
    },
    toISOStringTZ: function (dateObject) {
        var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
        var localISOTime = (new Date(dateObject.getTime() - tzoffset)).toISOString().slice(0, -1);
        return localISOTime;
    },
    RebindUrl: function (AdminStartMonth, AdminEndMonth, AdminDeptId, selected) {
        var that = this;

        var hashtag = window.location.hash;
        //window.location.hash = '';
        var url = window.location.href.split('#')[0];

        var date = new Date();
        var firstDay = AdminStartMonth;
        firstDay = typeof firstDay != 'undefined' ? firstDay : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth(), 1));
        var lastDay = AdminEndMonth;
        lastDay = typeof lastDay != 'undefined' ? lastDay : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth() + 1, 0, 23, 59, 59));

        var deptId = AdminDeptId;
        deptId = typeof deptId != 'undefined' ? deptId : '0';
        var paddingParam = '';
        if (url.indexOf('?') >= 0) {
            paddingParam = '&';
        }
        else {
            paddingParam = '?';
        }

        if (url.indexOf('AdminStartMonth=') > 0)
            url = url.replace(/(AdminStartMonth=)[^\&]+/, '$1' + firstDay);
        else
            url = url + paddingParam + 'AdminStartMonth=' + firstDay;
        if (url.indexOf('AdminEndMonth=') > 0)
            url = url.replace(/(AdminEndMonth=)[^\&]+/, '$1' + lastDay);
        else
            url = url + '&AdminEndMonth=' + lastDay;
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
