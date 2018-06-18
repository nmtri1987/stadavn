RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.LeaveHistory = function (settings) {
    var locationPath = window.location.pathname;
    this.Settings = {
        Id: settings.Id,
        EmployeeId: 0
    };
    $.extend(true, this.Settings, settings);
    this.Initialize();
};
RBVH.Stada.WebPages.pages.LeaveHistory.prototype = {
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
            format: "dd/mm/yyyy",
            autoclose: true
        });
        
        $(that.Settings.ToDateControlSelector).datepicker({
            format: "dd/mm/yyyy",
            autoclose: true
        });

        var FromDate = RBVH.Stada.WebPages.Utilities.GetValueByParam('FromDate');
        var ToDate = RBVH.Stada.WebPages.Utilities.GetValueByParam('ToDate');
        
        if (!!FromDate === false || !!ToDate === false)
            that.RebindUrl(FromDate, ToDate, false);
        else {
            that.Settings.FromDate = FromDate;
            that.Settings.ToDate = ToDate;
            
            var fromDateTimeSpan = Date.parse(FromDate);
            if (fromDateTimeSpan !== NaN) {
                var fromDate = new Date(fromDateTimeSpan);
                var fromDay = fromDate.getDate();
                var fromMonth = fromDate.getMonth() + 1;
                var fromYear = fromDate.getFullYear();
                $(that.Settings.FromDateControlSelector).val(fromDay + '/' + fromMonth + '/' + fromYear);
            }

            var toDateTimeSpan = Date.parse(ToDate);
            if (toDateTimeSpan !== NaN) {
                var toDate = new Date(toDateTimeSpan);
                var toDay = toDate.getDate();
                var toMonth = toDate.getMonth() + 1;
                var toYear = toDate.getFullYear();
                $(that.Settings.ToDateControlSelector).val(toDay + '/' + toMonth + '/' + toYear);
            }
        }
    },
    RegisterEvents: function () {
        var that = this;
        $(that.Settings.FromDateControlSelector).on('changeDate', function (ev) {
            var dpDay = ev.date.getDate();
            var dpMonth = ev.date.getMonth(); // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            var fromDate = new Date(dpYear, dpMonth, dpDay);
            $('.datepicker').hide();
            that.RebindUrl(that.toISOStringTZ(fromDate), that.Settings.ToDate, true);
        });
        $(that.Settings.ToDateControlSelector).on('changeDate', function (ev) {
            var dpDay = ev.date.getDate();
            var dpMonth = ev.date.getMonth(); // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            var toDate = new Date(dpYear, dpMonth, dpDay);
            $('.datepicker').hide();
            that.RebindUrl(that.Settings.FromDate, that.toISOStringTZ(toDate), true);
        });
    },
    toISOStringTZ: function (dateObject) {
        var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
        var localISOTime = (new Date(dateObject.getTime() - tzoffset)).toISOString().slice(0, -1);
        return localISOTime;
    },
    RebindUrl: function (fromDate, toDate, selected) {
        var that = this;
        var hashtag = window.location.hash;
        var url = window.location.href.split('#')[0];
        var date = new Date();
        var firstDay = fromDate;
        firstDay = typeof firstDay !== 'undefined' ? firstDay : that.toISOStringTZ(new Date());
        var lastDay = toDate;
        lastDay = typeof lastDay !== 'undefined' ? lastDay : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth() + 1, 0, 23, 59, 59));

        var paddingParam = '';
        if (url.indexOf('?') >= 0) {
            paddingParam = '&';
        }
        else {
            paddingParam = '?';
        }

        if (url.indexOf('FromDate=') > 0)
            url = url.replace(/(FromDate=)[^\&]+/, '$1' + firstDay);
        else
            url = url + paddingParam + 'FromDate=' + firstDay;

        if (url.indexOf('ToDate=') > 0)
            url = url.replace(/(ToDate=)[^\&]+/, '$1' + lastDay);
        else
            url = url + '&ToDate=' + lastDay;

        if (!!selected) {
            url = ViewUtilities.Paging.RemovePagingURL(url);
        }

        if (hashtag && hashtag.length > 0) {
            url += hashtag;
        }

        window.location.href = url;
    }
};
