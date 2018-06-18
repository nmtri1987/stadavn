var ViewShiftRequestPage =
 {
     EmployeeIdGridColumn: "EmployeeID",
     EmployeeNameGridColumn: "Employee Name",
     BackButton: "Back"
 }
$(document).ready(function () {
    var shiftIdParam = getUrlParameter("shiftId");
    if (shiftIdParam != '' && shiftIdParam != undefined) {
        $('#ViewShiftRequestGrid').show();
        $('#viewShiftGridNodata').hide();
        ViewShiftManagementGrid(shiftIdParam);
    }
    else {
        $('#viewShiftGridNodata').html("(No data)");
        $('#viewShiftGridNodata').show();
        $('#ViewShiftRequestGrid').hide();
    }
})
function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};

function ViewShiftManagementGrid(shiftId) {
    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/LoadViewShiftGrid";
    $.ajax({
        type: "POST",
        url: methodUrl,
        data: '{"shiftId":' + shiftId + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result != null) {
                $('#viewShiftGridNodata').hide();
                PrepareDataForViewShiftGrid(result);
            }
            else {
                $('#ViewShiftRequestGrid').hide();
                $('#viewShiftGridNodata').show();
                $('#viewShiftGridNodata').html("(No data)");
            }
        }
     ,
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 500) {
                console.log('Internal error: ' + jqXHR.responseText);
            } else {
                console.log('Unexpected error.');
            }
        }
    });

    function getNumberOfDaysInMonth(month, year) {
        if (month == 1) {
            month = 12;
            year--;
        }
        else {
            month--;
        }
        return new Date(year, month, 0).getDate();
    }
    function getDaysInMonth(month, year) {

        var days = [];
        var numberOfDays = getNumberOfDaysInMonth(month, year);
        for (var i = 21; i <= numberOfDays; i++) {
            days.push(i);
        }
        for (var j = 1; j <= 20; j++) {
            days.push(j);
        }
        return days;
    }
    function PrepareDataForViewShiftGrid(result) {
        var month = result.d.Month;
        var year = result.d.Year;
        var daysInMonthArray = getDaysInMonth(month, year);
        var serverListData = result.d.ShiftManagementData;

        var grid;
        var data = [];
        var deletedItemsIdArray = [];

        if (serverListData.length == 0) {
            ////Meta data
            for (var i = 0; i < 1; i++) {
                var d = (data[i] = {});
                d["EmployeeID"] = "";
                d["EmployeeName"] = "";
            }
        }
        else {
            var d = (data[0] = {});
            d["EmployeeID"] = serverListData.EmployeeID;
            d["EmployeeName"] = serverListData.EmployeeName;
            for (var j = 1; j <= 31; j++) {

                if (serverListData[j] != undefined) {
                    d[j] = serverListData[j];
                }
                else {
                    d[j] = "";
                }
            }
        }
        var columns = [];
        var checkboxSelector = new Slick.CheckboxSelectColumn({
            // cssClass: "slick-cell-checkboxsel"
        });

        function BoldTextEmployeeID(row, cell, value, columnDef, dataContext) {
            var html = '<div style="text-align:center;"> <b>' + dataContext.EmployeeID + '</b></div>';
            return html;
        }

        function BoldTextEmployeeName(row, cell, value, columnDef, dataContext) {
            var html = '<div style="text-align:center;"> <b>' + dataContext.EmployeeName + '</b></div>';
            return html;
        }

        var employeeIdObject = { id: "EmployeeID", name: ViewShiftRequestPage.EmployeeIdGridColumn, field: "EmployeeID", width: 120, resizable: false, formatter: BoldTextEmployeeID }
        columns.push(employeeIdObject);
        var employeeNameObject = { id: "EmployeeName", name: ViewShiftRequestPage.EmployeeNameGridColumn, field: "EmployeeName", width: 150, formatter: BoldTextEmployeeName };
        columns.push(employeeNameObject);
        for (var index = 0; index < daysInMonthArray.length; index++) {

            var itemOject = { id: daysInMonthArray[index], name: daysInMonthArray[index], field: daysInMonthArray[index], width: 80 };
            columns.push(itemOject);
        }
        var options = {
            editable: false,
            enableAddRow: true,
            enableCellNavigation: true,
            asyncEditorLoading: true,
            forceFitColumns: false,
            autoEdit: false,
            topPanelHeight: 25,
            frozenColumn: 1,
            enableColumnReorder: false,
            autoHeight: true
        };
        grid = new Slick.Grid("#ViewShiftRequestGrid", data, columns, options);
        $('#ViewShiftRequestGrid').on('shown', grid.resizeCanvas);
        $('#btnViewShiftBack').click(function (e) {

            //var taskId = getUrlParameter("Task");
            var id = getUrlParameter("Task");
            var backUrl = getUrlParameter("Source");
            if (backUrl != '' && backUrl != undefined) {
                var redirectUrl = backUrl + "?ID=" + id + "&Source=/SitePages/ShiftManagement.aspx";
                window.location.href = redirectUrl;
            }
            else {
                window.location.href = _spPageContextInfo.webAbsoluteUrl;
            }
            e.preventDefault();
        });
    }
}