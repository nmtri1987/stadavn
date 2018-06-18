RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.ListBasePage = function (settings) {
    this.Settings = {
        Controls:
            {
            },
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.ListBasePage.prototype = {
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

        var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
        var currentTab = $(hdActivedTabId).val();
        that.ActivaTab(currentTab);

        $(that.Settings.Controls.MonthControlSelector).datepicker({
            viewMode: "months",
            minViewMode: "months",
            format: "mm/yyyy",
            autoclose: true
        });
    },

    RegisterEvents: function () {
        var that = this;

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var targetTab = $(e.target).attr("href") // activated tab

            var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
            $(hdActivedTabId).val($(this).attr('aria-controls'));
        });

        $(that.Settings.Controls.MonthControlSelector).on('changeDate', function (ev) {
            var month = ev.date.getMonth() + 1; // 0 -> 11
            var year = ev.date.getYear() + 1900;
            $('.datepicker').hide();

            // Refresh Page
            that.RefreshPageForDeptAndMonthChanging(month, year);
        });

        $(that.Settings.Controls.DepartmentSelector).change(function () {
            var value = $(that.Settings.Controls.MonthControlSelector).val();
            var selectedDate = moment(value, 'MM/YYYY').toDate();
            var month = selectedDate.getMonth() + 1; // 0 -> 11;
            var year = selectedDate.getYear() + 1900;
            that.RefreshPageForDeptAndMonthChanging(month, year);
        });

        $('.linkAddNewItem').on('click', function () { $(".se-pre-con").fadeIn(0); });
    },
    ActivaTab: function (tab) {
        $('.nav-tabs a[href="#' + tab + '"]').tab('show');
    },
    RefreshPageForDeptAndMonthChanging: function (month, year) {
        var that = this;

        var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
        var activedTab = $(hdActivedTabId).val();
        var tabParamName = that.Settings.Controls.TabParamName;
        var departmentParamName = that.Settings.Controls.DepartmentParamName;
        var departmentSelector = that.Settings.Controls.DepartmentSelector;
        var monthParamname = that.Settings.Controls.MonthParamname;
        var yearParamName = that.Settings.Controls.YearParamName;
        var uri = URI(window.location.href);
        uri.removeQuery(tabParamName);
        uri.removeQuery(departmentParamName);
        uri.removeQuery(monthParamname);
        uri.removeQuery(yearParamName);
        uri.addQuery(tabParamName, activedTab);
        uri.addQuery(departmentParamName, $(departmentSelector).val());
        uri.addQuery(monthParamname, month);
        uri.addQuery(yearParamName, year);
        window.location.href = uri.toString();
    },
};