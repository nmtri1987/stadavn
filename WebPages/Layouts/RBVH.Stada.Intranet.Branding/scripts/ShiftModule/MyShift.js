RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.MyShift = function (settings) {
    var locationPath = window.location.pathname;
    this.Settings = {
        Id: settings.Id,
        BeginDate: 21,
        PageUrl: '//{0}' + locationPath + '?Month={1}&Year={2}',
    };
    $.extend(true, this.Settings, settings);
    this.Initialize();
};
RBVH.Stada.WebPages.pages.MyShift.prototype = {
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

        var Month = RBVH.Stada.WebPages.Utilities.GetValueByParam('MyMonth');
        var Year = RBVH.Stada.WebPages.Utilities.GetValueByParam('MyYear');
        if (!!Month == false || !!Year == false)
            that.RebindUrl(Month, Year);
        else
        {
            $(that.Settings.MonthControlSelector).val(Month + '/' + Year);
        }
    },

    RegisterEvents: function () {
        var that = this;

        $(that.Settings.MonthControlSelector).on('changeDate', function (ev) {
            var dpMonth = ev.date.getMonth() + 1; // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            $('.datepicker').hide();
            that.RebindUrl(dpMonth, dpYear, true);
            //__doPostBack('', (dpMonth + 1) + '/' + dpYear);
        });
    },

    RebindUrl: function (Month, Year, selected) {
        var month = Month;
        var year = Year;

        var hashtag = window.location.hash;
        var url = window.location.href.split('#')[0];

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

        var paddingParam = '';
        if (url.indexOf('?') >= 0) {
            paddingParam = '&';
        }
        else {
            paddingParam = '?';
        }
        if (url.indexOf('MyMonth=') > 0)
            url = url.replace(/(MyMonth=)[^\&]+/, '$1' + month);
        else
            url = url + paddingParam + 'MyMonth=' + month;
        if (url.indexOf('MyYear=') > 0)
            url = url.replace(/(MyYear=)[^\&]+/, '$1' + year);
        else
            url = url + '&MyYear=' + year;

        if (hashtag && hashtag.length > 0) {
            url += hashtag;
        }
        window.location.href = url;
    }
};