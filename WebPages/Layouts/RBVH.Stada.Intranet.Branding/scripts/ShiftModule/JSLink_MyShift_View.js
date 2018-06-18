
var MyShiftConfig = {
    TimeShift_ListTitle: 'Shift Time',
    ListResourceFileName: "RBVHStadaLists",
    ShiftTimeList: [],
    PageResourceFileName: "RBVHStadaWebpages",
    LeavesByEmployee: window.location.protocol + '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/GetLeavesInRange/{1}/{2}/{3}/{4}/{5}', // {employeeID}/{departmentID}/{locationID}/{fromDate}/{toDate},
    CalendarList: window.location.protocol + '//{0}/_vti_bin/Services/Calendar/CalendarService.svc/GetHolidayInRange/{1}/{2}',
    LeaveArray: [],
    NonWorkingDays: [],
    DaysInMonth: 31,
    ISODateFormat: '{0}-{1}-{2}',
};
(function () {

    var overrideMyShiftCtx = {};
    overrideMyShiftCtx.Templates = {};
    overrideMyShiftCtx.Templates.Item = myShiftCustomItem;
    overrideMyShiftCtx.OnPreRender = function (ctx) {
        GetNonWorkingDays();
        ShiftTimeList();
        $('.ms-menutoolbar').hide();

        // LEAVE popup
        var delay = 1000, setTimeoutConst;
        $(document)
            .on('mouseover', 'td.jsgrid-cell-all-day, td.jsgrid-cell-half-day', function (e) {
                var currentRow = $(this);
                setTimeoutConst = setTimeout(function () {
                    var leaveUrl = currentRow.attr("data-leave-url");
                    $("#leave-link").attr("href", leaveUrl);

                    $("#leave-dialog").dialog();

                    $('#leave-link').off('click').on('click', function () {
                        var itemURL = $(this).attr('href');
                        window.open(itemURL, "_blank");
                        setTimeout(function () {
                            $("#leave-dialog").dialog('close');
                        }, 1000);

                        return false;
                    });

                    return false;
                }, delay);
            })
            .on('mouseout', 'td.jsgrid-cell-all-day, td.jsgrid-cell-half-day', function (e) {
                clearTimeout(setTimeoutConst);
            });
    };
    
    overrideMyShiftCtx.ListTemplateType = 100;
    overrideMyShiftCtx.Templates.Header = renderHeader;
    overrideMyShiftCtx.Templates.Footer = pagingControl;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideMyShiftCtx);
})();

function GetNonWorkingDays()
{
    // Get Non-working days:
    var currentMonth = RBVH.Stada.WebPages.Utilities.GetValueByParam('MyMonth');
    var currentYear = RBVH.Stada.WebPages.Utilities.GetValueByParam('MyYear');

    var startMonth = currentMonth - 1;
    var startYear = currentYear;
    if (currentMonth == 1) {
        startMonth = 12;
        startYear = currentYear - 1;
    }

    MyShiftConfig.DaysInMonth = new Date(currentYear, currentMonth - 1, 0).getDate();

    var startDate = RBVH.Stada.WebPages.Utilities.String.format(MyShiftConfig.ISODateFormat, startMonth, '21', startYear);

    var nextYear = currentYear;
    var nextMonth = currentMonth;// + 1;

    var endDate = RBVH.Stada.WebPages.Utilities.String.format(MyShiftConfig.ISODateFormat, nextMonth, '20', nextYear);

    var nonWorkingDaysURL = RBVH.Stada.WebPages.Utilities.String.format(MyShiftConfig.CalendarList, location.host, startDate, endDate);

    var locationId = 2;
    var locationName = 'NM2';
    var employeeId = '';
    var departmentId = '';

    if (_rbvhContext && _rbvhContext.EmployeeInfo)
    {
        locationId = _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        locationName = _rbvhContext.EmployeeInfo.FactoryLocation.LookupValue;
        employeeId = _rbvhContext.EmployeeInfo.ID;
        departmentId = _rbvhContext.EmployeeInfo.Department.LookupId;
        locationId = _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
    }

    nonWorkingDaysURL = nonWorkingDaysURL + '/' + locationId;

    var leaveByEmployeeURL = RBVH.Stada.WebPages.Utilities.String.format(MyShiftConfig.LeavesByEmployee, location.host, employeeId, departmentId, locationId, startDate, endDate);

    $.ajax({
        type: "GET",
        url: nonWorkingDaysURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            MyShiftConfig.NonWorkingDays = [];
            MyShiftConfig.NonWorkingDays = result.filter(function (item) {
                return item.Location == locationName;
            }).map(function (obj) { return obj.Day; });
        }
    });

    $.ajax({
        type: "GET",
        url: leaveByEmployeeURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            MyShiftConfig.LeaveArray = result;
        }
    });
}

function GenerateTD(ctx, i)
{
    var cellCSS = ' jsgrid-cell jsgrid-align-center ';
    var cellValue = '';

    // LEAVE
    var leaveInfo = null;
    var leaveCSS = '';
    var leaveUrl = '';

    leaveInfo = $.grep(MyShiftConfig.LeaveArray, function (e) { return (e.Day == i && $.inArray(i, MyShiftConfig.NonWorkingDays) < 0); });
    if (leaveInfo.length == 1) {
        leaveCSS = ' jsgrid-cell-half-day ';
        if (leaveInfo[0].AllDay == true) {
            leaveCSS = ' jsgrid-cell-all-day shift-time-valid ';
            cellValue = 'P'; // TODO: MUST load from SHIFT
        }
        leaveUrl = leaveInfo[0].ItemUrl;
    }

    if (ctx.CurrentItem['ShiftTime' + i] == '') {
    }
    else {
        var index = ctx.CurrentItem['ShiftTime' + i][0].lookupId - 1;
        var code = '';
        if (MyShiftConfig.ShiftTimeList[index] != null) {
            code = MyShiftConfig.ShiftTimeList[index].Code;
        }

        cellValue = cellValue != '' ? cellValue : code;
        var isApproved = ctx.CurrentItem['ShiftTime' + i + 'Approval'];
        if (isApproved == "Yes" || isApproved == "Có") {
            cellCSS = cellCSS + ' shift-time-valid '
        }
    }

    return "<td class='" + cellCSS + leaveCSS + "' data-leave-url='" + leaveUrl + "'>" + cellValue + "</td>";
}

function myShiftCustomItem(ctx) {
    var tr = "<tr>";
    var td = '';

    for (i = 21; i <= MyShiftConfig.DaysInMonth; i++) {
        td = GenerateTD(ctx, i);
        tr = tr + td;
    }
    for (i = 1; i <= 20; i++) {
        td = GenerateTD(ctx, i);
        tr = tr + td;
    }
    tr = tr + "</tr>";
    return tr;
}
function renderHeader(ctx) {
    var header = '';

    for (i = 21; i <= MyShiftConfig.DaysInMonth; i++) {
        header = header + '<th class="jsgrid-header-cell jsgrid-align-center">' + i + '</th>';
    }
    for (i = 1; i <= 20; i++) {
        header = header + '<th class="jsgrid-header-cell jsgrid-align-center">' + i + '</th>';
    }
    header = "<div class='col-md-12' style='padding-left: 0px;'><table class='table table-bordered' id='myshift'><thead><tr>" + header + "</tr></thead><tbody>";
    return header;
}

function pagingControl(ctx) {
    return ViewUtilities.Paging.InstanceHtml(ctx);
}
function ShiftTimeList() {
    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('" + String(MyShiftConfig.TimeShift_ListTitle) + "')/items";
    var d = $.Deferred();
    $.ajax({
        url: url,
        method: "GET",
        async: false,
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            // Returning the results
            MyShiftConfig.ShiftTimeList = data.d.results;
            d.resolve(data.d.results);
        },
        error: function (data) {
            status = 'failed';
        }
    });
    return d.promise();

}