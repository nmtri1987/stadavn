RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.ShiftByDepartment = function (settings) {
    var locationPath = window.location.pathname;
    this.Settings = {
        Id: settings.Id,
        BeginDate: 21,
        PageUrl: window.location.protocol + '//{0}' + locationPath + '?Month={1}&Year={2}',
        DepartmentList: window.location.protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentForShift/{1}/{2}',
        CurrentDepartmentId: 0
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.ShiftByDepartment.prototype = {
    Initialize: function () {
        var that = this;
        ExecuteOrDelayUntilScriptLoaded(function () {
            that.InitControls();
            that.RegisterEvents();
        }, "sp.js");
    },

    InitControls: function () {
        var that = this;
        $(that.Settings.MonthControlSelector).datepicker({
            viewMode: "months",
            minViewMode: "months",
            format: "mm/yyyy",
            autoclose: true
        });

        var Month = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminMonth');
        var Year = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminYear');
        var DeptId = that.Settings.CurrentDepartmentId > 0 ? that.Settings.CurrentDepartmentId : RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminDeptId');
        if (!!Month == false || !!Year == false || !!DeptId == false)
            that.RebindUrl(Month, Year, DeptId);
        else {
            that.PopulateDepartment();
            that.Settings.DepartmentId = DeptId;
            that.Settings.Month = Month;
            that.Settings.Year = Year;
            $(that.Settings.MonthControlSelector).val(Month + '/' + Year);
            $(that.Settings.DepartmentControlSelector).val(DeptId);
        }

        if (that.Settings.CurrentDepartmentId > 0)
            $(that.Settings.DepartmentControlSelector).prop('disabled', true);
    },

    RegisterEvents: function () {
        var that = this;

        $(that.Settings.MonthControlSelector).on('changeDate', function (ev) {
            var dpMonth = ev.date.getMonth() + 1; // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            $('.datepicker').hide();
            that.RebindUrl(dpMonth, dpYear, that.Settings.DepartmentId, true);
            //__doPostBack('', (dpMonth + 1) + '/' + dpYear);
        });

        $(that.Settings.DepartmentControlSelector).on('change', function () {
            that.Settings.DepartmentId = $(this).val();
            that.RebindUrl(that.Settings.Month, that.Settings.Year, that.Settings.DepartmentId, true);
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
                if (that.Settings.CurrentDepartmentId == 0) {
                    $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', that.Settings.CurrentDepartmentId).text("All/Tất cả"));
                }

                $(result).each(function () {
                    $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', this.Id).text(this.DepartmentName));
                });
                that.Settings.DepartmentId = $(that.Settings.DepartmentControlSelector).val();
            }
        });
    },

    RebindUrl: function (Month, Year, DeptId, selected) {
        var month = Month;
        var year = Year;

        var hashtag = window.location.hash;
        //window.location.hash = '';
        var url = window.location.href.split('#')[0];

        var deptId = DeptId;
        if (!!Month == false || !!Year == false) {
            var today = new Date();
            var day = today.getDate();
            month = today.getMonth() + 1; // 0 -> 11
            year = today.getYear() + 1900;
            if (day > 20)
                month++;
            if (month == 13) {
                year++;
                month = 1;
            }
        }
        deptId = typeof deptId != 'undefined' ? DeptId : '0';
        var paddingParam = '';
        if (url.indexOf('?') >= 0) {
            paddingParam = '&';
        }
        else {
            paddingParam = '?';
        }
        if (url.indexOf('AdminMonth=') > 0)
            url = url.replace(/(AdminMonth=)[^\&]+/, '$1' + month);
        else
            url = url + paddingParam + 'AdminMonth=' + month;
        if (url.indexOf('AdminYear=') > 0)
            url = url.replace(/(AdminYear=)[^\&]+/, '$1' + year);
        else
            url = url + '&AdminYear=' + year;
        if (url.indexOf('AdminDeptId=') > 0)
            url = url.replace(/(AdminDeptId=)[^\&]+/, '$1' + deptId);
        else
            url = url + '&AdminDeptId=' + deptId;

        if (!!selected) {
            url = ViewUtilities.Paging.RemovePagingURL(url);
        }

        if (hashtag && hashtag.length > 0) {
            url += hashtag;
        }
        window.location.href = url;
    }
};







