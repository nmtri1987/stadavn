var ShiftManagementPage = {
    DataLoadFailError: "Failed to load data from server. Please try again!",
    NoDataMessage: "(No data)",
    LoadMyShiftSlickgrid: function(pageNumber) {
        var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/LoadMyShiftGridData";
        $.ajax({
            type: "POST",
            url: methodUrl,
            data: '{"pageNumber":' + pageNumber + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (result) {
                if (result != null) {
                    if (result.d.MyShiftListData != null && result.d.MyShiftListData.length == 0) {
                        $('#myShiftGrid').hide();
                        $('#lblMyShiftNodata').html(ShiftManagementPage.NoDataMessage);
                        $('#lblMyShiftNodata').show();
                    }
                    else {
                        $('#myShiftGrid').show();
                        prepareDataForMyShiftGrid(result);
                    }
                }
                else {
                    alert(ShiftManagementPage.DataLoadFailError);
                }
            }
         ,
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status == 500) {
                    //console.log('Internal error: ' + jqXHR.responseText);
                } else {
                    //console.log('Unexpected error.');
                }
            }
        });

        function prepareDataForMyShiftGrid(result) {
            var startDayofMonth = result.d.StartDay || 21;
            var endDayofMonth = result.d.EndDay || 20;
            var daysInMonthArray = getDaysInMonth(startDayofMonth, endDayofMonth);
            var serverListData = result.d.MyShiftListData;
            var grid;
            var data = [];
            var dataView;
            var rowSelected = [];

            if (serverListData.length == 0) {
                //Meta data
                for (var i = 0; i < 1; i++) {
                    var d = (data[i] = {});
                    d["id"] = "id_" + i;
                    d["MonthYear"] = "";
                }
            }
            else {
                for (var serverDataIndex = 0; serverDataIndex < serverListData.length; serverDataIndex++) {
                    var d = (data[serverDataIndex] = {});
                    d["id"] = "id_" + serverDataIndex;
                    d["MonthYear"] = serverListData[serverDataIndex].MonthYear;


                    for (var j = 1; j <= 31; j++) {
                        if (serverListData[serverDataIndex][j] != undefined) {
                            d[j] = serverListData[serverDataIndex][j];
                        }
                        else {
                            d[j] = "";
                        }
                    }
                }
            }

            function BoldText(row, cell, value, columnDef, dataContext) {
                var html = '<div style="text-align:center;"> <b>' + dataContext.MonthYear + '</b></div>';
                return html;
            }
            var columns = [];
            var checkboxSelector = new Slick.CheckboxSelectColumn({
                cssClass: "slick-cell-checkboxsel"
            });

            var employeeNameObject = { id: "MonthYear", name: "Month/Year", width: 150, resizeable: false, formatter: BoldText };
            columns.push(employeeNameObject);
            for (var index = 0; index < daysInMonthArray.length; index++) {
                var itemOject = { id: daysInMonthArray[index], resizeable: false, name: daysInMonthArray[index], field: daysInMonthArray[index], width: 80 };
                columns.push(itemOject);
            }
            var options = {
                editable: false,
                enableAddRow: true,
                enableCellNavigation: true,
                asyncEditorLoading: true,
                forceFitColumns: false,
                autoEdit: false,
                topPanelHeight: 24,
                frozenColumn: 0,
                enableColumnReorder: false,
                autoHeight: true
            };
            dataView = new Slick.Data.DataView();
            grid = new Slick.Grid("#myShiftGrid", data, columns, options);
            $('#myShiftGrid').on('shown', grid.resizeCanvas);
        }
    }
};

$(document).ready(function () {
    $('#lblMyShiftNodata').hide();
    ShiftManagementPage.LoadMyShiftSlickgrid(1); //Note: 1 is first page, In case paging, change that number
});

function getNumberOfDaysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
}

function getDaysInMonth(startDay, endDay) {
    var days = [];
    for (var i = startDay; i <= 31; i++) {
        days.push(i);
    }
    for (var j = 1; j <= endDay; j++) {
        days.push(j);
    }
    return days;
}