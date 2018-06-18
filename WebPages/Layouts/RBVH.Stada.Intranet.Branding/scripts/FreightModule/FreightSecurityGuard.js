RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.FreightSecurityGuard = function (settings) {
    this.Settings = {
        ServiceUrls: {
            DepartmentList: window.location.protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentsByLcid/{1}/{2}'
        },
        AllDepartments: "All/Tất cả",
        CurrentDepartmentId: 0,
        SelectedFromDate: '',
        SelectedToDate: '',
        DepartmentId: '0',
        SearchType: '0',
        RequestNumber: '0'
    };
    $.extend(true, this.Settings, settings);
    this.Initialize();
};
RBVH.Stada.WebPages.pages.FreightSecurityGuard.prototype = {
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

        var selectedFromDate = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminSelectedDate');
        var selectedToDate = RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminSelectedToDate');
        var deptId = that.Settings.CurrentDepartmentId > 0 ? that.Settings.CurrentDepartmentId : RBVH.Stada.WebPages.Utilities.GetValueByParam('AdminDeptId');
        var searchType = RBVH.Stada.WebPages.Utilities.GetValueByParam('searchtype');
        var reqNum = RBVH.Stada.WebPages.Utilities.GetValueByParam('reqnum');
        if (!!selectedFromDate === false || !!selectedToDate === false || !!deptId === false || !!searchType === false || !!reqNum === false) {
            var date = new Date();
            selectedFromDate = typeof selectedFromDate !== 'undefined' ? selectedFromDate : that.toISOStringTZ(date);
            selectedToDate = typeof selectedToDate !== 'undefined' ? selectedToDate : that.toISOStringTZ(new Date(date.getFullYear(), date.getMonth() + 1, 0, 23, 59, 59));

            deptId = typeof deptId !== 'undefined' ? deptId : '0';
            searchType = typeof searchType !== 'undefined' ? searchType : '0';
            reqNum = typeof reqNum !== 'undefined' ? reqNum : '0';

            that.RebindUrl(selectedFromDate, selectedToDate, deptId, searchType, reqNum, false);
        }
        else {
            that.Settings.DepartmentId = deptId;
            that.Settings.SelectedFromDate = selectedFromDate;
            that.Settings.SelectedToDate = selectedToDate;
            that.Settings.DepartmentId = deptId;
            that.Settings.SearchType = searchType;
            that.Settings.RequestNumber = reqNum;

            var that = this;
            var calendarOptions = {
                format: "dd/mm/yyyy",
                autoclose: true
            };
            $(that.Settings.Controls.FromDateControlSelector).datepicker(calendarOptions);
            $(that.Settings.Controls.ToDateControlSelector).datepicker(calendarOptions);
            that.PopulateComboboxes();

            $(that.Settings.Controls.DepartmentControlSelector).val(that.Settings.DepartmentId);

            var selectedFromDateTimeSpan = Date.parse(that.Settings.SelectedFromDate);
            if (selectedFromDateTimeSpan !== NaN) {
                var selectedDate = new Date(selectedFromDateTimeSpan);
                var currentDay = selectedDate.getDate();
                var currentMonth = selectedDate.getMonth() + 1;
                var currentYear = selectedDate.getFullYear();
                $(that.Settings.Controls.FromDateControlSelector).val(currentDay + '/' + currentMonth + '/' + currentYear);
            }
            var selectedToDateTimeSpan = Date.parse(that.Settings.SelectedToDate);
            if (selectedToDateTimeSpan !== NaN) {
                var selectedDate = new Date(selectedToDateTimeSpan);
                var currentDay = selectedDate.getDate();
                var currentMonth = selectedDate.getMonth() + 1;
                var currentYear = selectedDate.getFullYear();
                $(that.Settings.Controls.ToDateControlSelector).val(currentDay + '/' + currentMonth + '/' + currentYear);
            }

            that.ShowHideSearchControl(that.Settings.SearchType);

            if (that.Settings.RequestNumber !== '0') {
                $(that.Settings.Controls.TxtRequestNumberID).val(that.Settings.RequestNumber);
            }
            else {
                $(that.Settings.Controls.TxtRequestNumberID).val("");
            }
        }
    },
    RegisterEvents: function () {
        var that = this;
        $("input[type='radio'][name='searchType']").click(function () {
            $(that.Settings.Controls.TxtRequestNumberID).val("");
            var searchType = $(this).val();
            that.ShowHideSearchControl(searchType);
        });

        $(that.Settings.Controls.SearchButtonID).on('click', function (event) {
            event.preventDefault();
            $(that.Settings.Controls.TxtRequestNumberID).removeClass('require-error');
            that.Settings.RequestNumber = $(that.Settings.Controls.TxtRequestNumberID).val();
            if (that.Settings.RequestNumber === undefined || that.Settings.RequestNumber === null || that.Settings.RequestNumber.length === 0) {
                that.Settings.RequestNumber = '0';
                $(that.Settings.Controls.TxtRequestNumberID).addClass('require-error');
            }
            else {
                that.Settings.SearchType = '1';
                that.RebindUrl(that.Settings.SelectedFromDate, that.Settings.SelectedToDate, that.Settings.DepartmentId, that.Settings.SearchType, that.Settings.RequestNumber, true);
            }
        });

        $(that.Settings.Controls.FromDateControlSelector).on('changeDate', function (ev) {
            var dpDay = ev.date.getDate();
            var dpMonth = ev.date.getMonth(); // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            var selectedDate = new Date(dpYear, dpMonth, dpDay);
            $('.datepicker').hide();
            that.Settings.SearchType = '0';
            that.Settings.RequestNumber = '0';
            that.RebindUrl(that.toISOStringTZ(selectedDate), that.Settings.SelectedToDate, that.Settings.DepartmentId, that.Settings.SearchType, that.Settings.RequestNumber, true);
        });
        $(that.Settings.Controls.ToDateControlSelector).on('changeDate', function (ev) {
            var dpDay = ev.date.getDate();
            var dpMonth = ev.date.getMonth(); // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            var selectedDate = new Date(dpYear, dpMonth, dpDay);
            $('.datepicker').hide();
            that.Settings.SearchType = '0';
            that.Settings.RequestNumber = '0';
            that.RebindUrl(that.Settings.SelectedFromDate, that.toISOStringTZ(selectedDate), that.Settings.DepartmentId, that.Settings.SearchType, that.Settings.RequestNumber, true);
        });

        $(that.Settings.Controls.DepartmentControlSelector).on('change', function () {
            that.Settings.DepartmentId = $(this).val();
            that.Settings.SearchType = '0';
            that.Settings.RequestNumber = '0';
            that.RebindUrl(that.Settings.SelectedFromDate, that.Settings.SelectedToDate, that.Settings.DepartmentId, that.Settings.SearchType, that.Settings.RequestNumber, true);
        });
    },
    ShowHideSearchControl: function (searchType) {
        var that = this;
        if (searchType === '0') {
            $("input[type='radio'][name='searchType']:first").prop("checked", "checked");
            $(that.Settings.Controls.BasicSearchClass).show();
            $(that.Settings.Controls.SearchByReqNumClass).hide();
        }
        else {
            $("input[type='radio'][name='searchType']:last").prop("checked", "checked");
            $(that.Settings.Controls.BasicSearchClass).hide();
            $(that.Settings.Controls.SearchByReqNumClass).show();
        }
    },
    PopulateComboboxes: function () {
        var that = this;
        var lcid = SP.Res.lcid;
        var locationId = _rbvhContext.EmployeeInfo != null ? _rbvhContext.EmployeeInfo.FactoryLocation.LookupId : 2;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.DepartmentList, location.host, lcid, locationId);
        $(that.Settings.Controls.DepartmentControlSelector).attr("disabled", false);
        $(that.Settings.Controls.DepartmentControlSelector).empty();
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                $(that.Settings.Controls.DepartmentControlSelector).append($("<option>").attr('value', "0").text(that.Settings.AllDepartments));
                $(result).each(function () {
                    $(that.Settings.Controls.DepartmentControlSelector).append($("<option>").attr('value', this.Id).text(this.DepartmentName));
                });
                that.Settings.DepartmentId = $(that.Settings.Controls.DepartmentControlSelector).val();
            }
        });
    },
    RebindUrl: function (adminSelectedDate, adminSelectedToDate, adminDeptId, searchType, reqNum, selected) {
        var that = this;
        var hashtag = window.location.hash;
        var url = window.location.href.split('#')[0];

        var paddingParam = '';
        if (url.indexOf('?') >= 0) {
            paddingParam = '&';
        }
        else {
            paddingParam = '?';
        }

        if (url.indexOf('AdminSelectedDate=') > 0)
            url = url.replace(/(AdminSelectedDate=)[^\&]+/, '$1' + adminSelectedDate);
        else
            url = url + paddingParam + 'AdminSelectedDate=' + adminSelectedDate;

        if (url.indexOf('AdminSelectedToDate=') > 0)
            url = url.replace(/(AdminSelectedToDate=)[^\&]+/, '$1' + adminSelectedToDate);
        else
            url = url + '&AdminSelectedToDate=' + adminSelectedToDate;

        if (url.indexOf('AdminDeptId=') > 0)
            url = url.replace(/(AdminDeptId=)[^\&]+/, '$1' + adminDeptId);
        else
            url = url + '&AdminDeptId=' + adminDeptId;

        if (url.indexOf('searchtype=') > 0)
            url = that.UpdateQueryStringParameter(url, 'searchtype', searchType);
        else {
            url = url + '&searchtype=' + searchType;
        }

        if (url.indexOf('reqnum=') > 0)
            url = that.UpdateQueryStringParameter(url, 'reqnum', reqNum);
        else {
            url = url + '&reqnum=' + reqNum;
        }

        if (!!selected) {
            url = ViewUtilities.Paging.RemovePagingURL(url);
        }

        if (hashtag && hashtag.length > 0) {
            url += hashtag;
        }

        if (window.location.href === url) {
            window.location.reload();
        }
        else {
            window.location.href = url;
        }
    },
    UpdateQueryStringParameter: function (uri, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        }
        else {
            return uri + separator + key + "=" + value;
        }
    },
    toISOStringTZ: function (dateObject) {
        var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
        var localISOTime = (new Date(dateObject.getTime() - tzoffset)).toISOString().slice(0, -1);
        return localISOTime;
    }
};
