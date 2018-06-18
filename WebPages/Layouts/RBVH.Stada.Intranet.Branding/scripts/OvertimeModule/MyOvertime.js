(function () {
    RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
    RBVH.Stada.WebPages.pages.MyOvertime = function (settings) {
        var locationPath = window.location.pathname;
        this.Settings = {
            Id: settings.Id,
            BeginDate: 21,
            PageUrl: '//{0}' + locationPath + '?StartMonth={1}&EndMonth={2}',
        };
        $.extend(true, this.Settings, settings);
        this.Initialize();
    };
    RBVH.Stada.WebPages.pages.MyOvertime.prototype = {
        Initialize: function () {
            var that = this;
            $(document).ready(function () {
                that.InitControls();
                that.RegisterEvents();
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

            var StartMonth = RBVH.Stada.WebPages.Utilities.GetValueByParam('MyStartMonth');
            var EndMonth = RBVH.Stada.WebPages.Utilities.GetValueByParam('MyEndMonth');
            if (!!StartMonth == false || !!EndMonth == false)
                that.RebindUrl(StartMonth, EndMonth);
            else {
                var startMonthTimeSpan = Date.parse(StartMonth);
                if (startMonthTimeSpan != NaN) {
                    var startMonth = new Date(startMonthTimeSpan);
                    var currentMonth = startMonth.getMonth() + 1;
                    var currentYear = startMonth.getFullYear();
                    $(that.Settings.MonthControlSelector).val(currentMonth + '/' + currentYear);
                }
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
                that.RebindUrl(that.toISOStringTZ(firstDayDp), that.toISOStringTZ(lastDayDp), true);
                //__doPostBack('', (dpMonth + 1) + '/' + dpYear);
            });
        },
        toISOStringTZ: function (dateObject) {
            var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
            var localISOTime = (new Date(dateObject.getTime() - tzoffset)).toISOString().slice(0, -1);
            return localISOTime;
        },
        RebindUrl: function (StartMonth, EndMonth, selected) {
            var that = this;
            var hashtag = window.location.hash;
            var url = window.location.href.split('#')[0];
            var date = new Date();
            var firstDay = StartMonth;
            firstDay = typeof firstDay != 'undefined' ? firstDay : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth(), 1));
            var lastDay = EndMonth;
            lastDay = typeof lastDay != 'undefined' ? lastDay : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth() + 1, 0, 23, 59, 59));
            
            var paddingParam = '';
            var paddingParam = '';
            if (url.indexOf('?') >= 0) {
                paddingParam = '&';
            }
            else {
                paddingParam = '?';
            }
            if (url.indexOf('MyStartMonth=') > 0)
                url = url.replace(/(MyStartMonth=)[^\&]+/, '$1' + firstDay);
            else
                url = url + paddingParam + 'MyStartMonth=' + firstDay;
            if (url.indexOf('MyEndMonth=') > 0)
                url = url.replace(/(MyEndMonth=)[^\&]+/, '$1' + lastDay);
            else
                url = url + '&MyEndMonth=' + lastDay;

            if (hashtag && hashtag.length > 0) {
                url += hashtag;
            }
            window.location.href = url;
        }
    };
})();