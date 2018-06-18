RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.ListBasePageV2 = function (settings) {
    this.Settings = {
        Controls:
            {
            },
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.ListBasePageV2.prototype = {
    Initialize: function () {
        var that = this;

        $(document).ready(function () {
            that.InitControls();
            that.RegisterEvents();
        });
    },

    InitControls: function () {
        var that = this;

        var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
        var currentTab = $(hdActivedTabId).val();
        that.ActivaTab(currentTab);

        $(that.Settings.Controls.FromDateControlSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
        $(that.Settings.Controls.ToDateControlSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
        $(that.Settings.Controls.FromDateErrorMsgSelector).addClass('ms-formvalidation');
        $(that.Settings.Controls.ToDateErrorMsgSelector).addClass('ms-formvalidation');
    },

    RegisterEvents: function () {
        var that = this;

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var targetTab = $(e.target).attr("href") // activated tab

            var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
            $(hdActivedTabId).val($(this).attr('aria-controls'));
        });

        $(that.Settings.Controls.FromDateControlSelector).on('changeDate', function (ev) {
            // Refresh Page
            that.RefreshPageForDeptAndMonthChanging($(this).val(), $(that.Settings.Controls.ToDateControlSelector).val());
        });

        $(that.Settings.Controls.ToDateControlSelector).on('changeDate', function (ev) {
            // Refresh Page
            that.RefreshPageForDeptAndMonthChanging($(that.Settings.Controls.FromDateControlSelector).val(), $(this).val());
        });

        $(that.Settings.Controls.DepartmentSelector).change(function () {
            that.RefreshPageForDeptAndMonthChanging($(that.Settings.Controls.FromDateControlSelector).val(), $(that.Settings.Controls.ToDateControlSelector).val());
        });

        $('.linkAddNewItem').on('click', function () { $(".se-pre-con").fadeIn(0); });
    },
    ActivaTab: function (tab) {
        $('.nav-tabs a[href="#' + tab + '"]').tab('show');
    },
    RefreshPageForDeptAndMonthChanging: function (fromDate, toDate) {
        var that = this;

        var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
        var activedTab = $(hdActivedTabId).val();
        var tabParamName = that.Settings.Controls.TabParamName;
        var departmentParamName = that.Settings.Controls.DepartmentParamName;
        var departmentSelector = that.Settings.Controls.DepartmentSelector;
        var fromDateParamName = that.Settings.Controls.FromDateParamName;
        var toDateParamName = that.Settings.Controls.ToDateParamName;
        var uri = $URI(window.location.href);
        uri.removeQuery(tabParamName);
        uri.removeQuery(departmentParamName);
        uri.removeQuery(fromDateParamName);
        uri.removeQuery(toDateParamName);
        uri.addQuery(tabParamName, activedTab);
        uri.addQuery(departmentParamName, $(departmentSelector).val());
        uri.addQuery(fromDateParamName, fromDate);
        uri.addQuery(toDateParamName, toDate);
        window.location.href = uri.toString();
    },
};