RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.LeaveSecurityGuard = function (settings) {
    var locationPath = window.location.pathname;
    this.Settings = {
        EmployeeIDForSearch: ''
    };

    $.extend(true, this.Settings, settings);
    this.Initialize();
};
RBVH.Stada.WebPages.pages.LeaveSecurityGuard.prototype = {
    Initialize: function () {
        var that = this;
        ExecuteOrDelayUntilScriptLoaded(function () {
            that.InitControls();
            that.RegisterEvents();
        }, "sp.js");
    },
    InitControls: function () {
        var that = this;

        var employeeId = RBVH.Stada.WebPages.Utilities.GetValueByParam('employeeId');
        if (!!employeeId == false)
            that.RebindUrl(employeeId);
        else {
            that.Settings.EmployeeIDForSearch = employeeId;
            if (that.Settings.EmployeeIDForSearch && that.Settings.EmployeeIDForSearch.length > 0 && that.Settings.EmployeeIDForSearch !== "0") {
                $(that.Settings.Controls.InputEmployeeID).val(that.Settings.EmployeeIDForSearch);
            }
        }
    },
    RegisterEvents: function () {
        var that = this;
        if ($(that.Settings.Controls.SearchButtonID).length > 0) {
            $(that.Settings.Controls.SearchButtonID).on('click', function () {
                if ($(that.Settings.Controls.InputEmployeeID).val() == '')
                    return;
                var employeeId = $(that.Settings.Controls.InputEmployeeID).val();
                that.RebindUrl(employeeId);
            });
        }
    },
    RebindUrl: function (employeeId) {
        var that = this;
        var hashtag = window.location.hash;
        var url = window.location.href.split('#')[0];
        
        var employeeIdVal = employeeId;
        employeeIdVal = typeof employeeIdVal != 'undefined' ? employeeIdVal : '0';
        var paddingParam = '';
        if (url.indexOf('?') >= 0) {
            paddingParam = '&';
        }
        else {
            paddingParam = '?';
        }

        if (url.indexOf('employeeId=') > 0)
            url = url.replace(/(employeeId=)[^\&]+/, '$1' + employeeIdVal);
        else
            url = url + paddingParam + 'employeeId=' + employeeIdVal;

        if (hashtag && hashtag.length > 0) {
            url += hashtag;
        }

        if (window.location.href === url) {
            window.location.reload();
        }
        else {
            window.location.href = url;
        }
    }
};
