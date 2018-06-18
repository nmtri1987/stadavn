$(document).ready(function () {
    $('#dpMonths .span2').datepicker({
        viewMode: "months",
        minViewMode: "months",
        format: "mm/yyyy",
        autoclose: true
    }).on('changeDate', function (ev) {
        var month = ev.date.getMonth() + 1;
        var year = ev.date.getYear() + 1900;
        ShiftRequestGrid(month, year);
    });
    ShiftRequestGrid();
});

var ShiftRequestPage = {
    DeleteConfirmationDialogTitle: "Delele Confirmation",
    CancelConfirmationDialogTitle: "Cancel Confirmation",
    DuplicateConfirmationDialogTitle: "Duplicate Confirmation",
    SaveSuccessfullMessage: "Data was saved successfully!",
    SaveErrorMessage: "An error occurred while saving data. Please try again!",
    InformationDialogTitle: "Information",
    NoItemDelete: "No item to delete",
    NoDataToSave: "No item to save",
    YesButton: "Yes",
    NoButton: "No",
    OkButton: "Ok",
    EmployeeIdGridColumn: "EmployeeID",
    EmployeeNameGridColumn: "Employee Name",
    ChangeConfirmationDialogTitle: "Change Confirmation"
};

function getNumberOfDaysInMonth(month, year) {
    if (month === 1) {
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

function ShiftRequestGrid(m, y) {
    var today = new Date();
    var month = m || today.getMonth() + 1;
    var year = y || today.getYear() + 1900;
    var monthLabel = month;
    var yearLabel = year;
    $('#txtCalendar').val(monthLabel + '/' + yearLabel);
    $('#stadaHiddenMonth').val(month);
    $('#stadaHiddenYear').val(year);

    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/LoadGridData";
    $.ajax({
        type: "POST",
        url: methodUrl,
        data: '{"month":' + month + ', "year":' + year + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (result) {
            prepareDataForGrid(result, null);
        },
        // ReSharper disable once UnusedParameter
        error: function (jqXhr, textStatus) {
            if (jqXhr.status === 500) {
                console.log('Internal error: ' + jqXhr.responseText);
            } else {
                console.log('Unexpected error.');
            }
        }
    });

    function prepareDataForGrid(result, reloadData) {

        var daysInMonthArray = getDaysInMonth(month, year);
        var resultData;
        var serverListData;
        if (result !== undefined && result != null) {
            resultData = result.d;
            serverListData = resultData.ShiftRequestDataList;
        }
        var grid;
        var data = [];
        var deletedItemsIdArray = [];
        var d;
        if (serverListData.length === 0) {
            ////Meta data
            for (var i = 0; i < 1; i++) {
                d = (data[i] = {});
                d["IsChanged"] = false;
                d["EmployeeID"] = "";
                d["EmployeeName"] = "";
                d["ShiftManagementID"] = "";
                d["id"] = "id_" + i;
                d["EmployeeIDHidden"] = "";
            }
        }
        else {
            for (var serverDataIndex = 0; serverDataIndex < serverListData.length; serverDataIndex++) {
                d = (data[serverDataIndex] = {});
                d["IsChanged"] = false;
                d["EmployeeID"] = serverListData[serverDataIndex].EmployeeID;
                d["EmployeeIDHidden"] = serverListData[serverDataIndex].EmployeeID;
                d["EmployeeName"] = serverListData[serverDataIndex].EmployeeName;
                d["ShiftManagementID"] = serverListData[serverDataIndex].ShiftManagementID;
                d["id"] = "id_" + serverDataIndex;
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
        var columns = [];
        var checkboxSelector = new Slick.CheckboxSelectColumn({
            // cssClass: "slick-cell-checkboxsel"
        });
        var isChangedHiddenColumn = { id: "IsChanged", name: "IsChanged", field: "IsChanged", width: 0, minWidth: 0, maxWidth: 0, cssClass: "reallyHidden", headerCssClass: "reallyHidden" };
        columns.push(isChangedHiddenColumn);
        var employeeIdHiddenColumn = { id: "EmployeeIDHidden", name: "EmployeeIDHidden", field: "EmployeeIDHidden", width: 0, minWidth: 0, maxWidth: 0, cssClass: "reallyHidden", headerCssClass: "reallyHidden" };
        columns.push(employeeIdHiddenColumn);
        var shiftManagementHiddenColumn = { id: "ShiftManagementID", name: "ShiftManagementID", field: "ShiftManagementID", width: 0, minWidth: 0, maxWidth: 0, cssClass: "reallyHidden", headerCssClass: "reallyHidden" };
        columns.push(shiftManagementHiddenColumn);
        columns.push(checkboxSelector.getColumnDefinition());
        var employeeIdObject = { id: "EmployeeID", name: ShiftRequestPage.EmployeeIdGridColumn, field: "EmployeeID", options: resultData.EmployeeIdOption, width: 120, resizable: false, editor: selectCellEditor }
        columns.push(employeeIdObject);
        var employeeNameObject = { id: "EmployeeName", name: ShiftRequestPage.EmployeeNameGridColumn, field: "EmployeeName", width: 150 };
        columns.push(employeeNameObject);
        for (var index = 0; index < daysInMonthArray.length; index++) {
            var itemOject = { id: daysInMonthArray[index], name: daysInMonthArray[index], field: daysInMonthArray[index], options: resultData.ShiftTimeOption, width: 80, editor: selectCellEditor };
            columns.push(itemOject);
        }
        var options = {
            editable: true,
            enableAddRow: true,
            enableCellNavigation: true,
            asyncEditorLoading: true,
            forceFitColumns: false,
            autoEdit: false,
            syncColumnCellResize: true,
            topPanelHeight: 25,
            frozenColumn: 5,
            enableColumnReorder: false,
            autoHeight: true,
            autoWidth: true
        };

        function selectCellEditor(args) {
            var $select;
            var defaultValue;
            // var scope = this;

            this.init = function () {
                var optValues;
                if (args.column.options) {
                    optValues = args.column.options.split(',');
                } else {
                    optValues = "".split(',');
                }
                var optionStr = "";
                for (var i in optValues) {
                    if (optValues.hasOwnProperty(i)) {
                        var v = optValues[i];
                        optionStr += "<OPTION value='" + v + "'>" + v + "</OPTION>";
                    }
                }
                $select = $("<SELECT tabIndex='0' style='width:100%;' class='editor-select'>" + optionStr + "</SELECT>");
                $select.appendTo(args.container);
                $select.focus();

            };

            this.destroy = function () {
                $select.remove();
            };

            this.focus = function () {
                $select.focus();
            };

            this.loadValue = function (item) {

                defaultValue = item[args.column.field];
                $select.val(defaultValue);
            };

            this.serializeValue = function () {
                if (args.column.options) {
                    return $select.val();
                } else {
                    return ($select.val() === "");
                }
            };

            this.applyValue = function (item, state) {

                item[args.column.field] = state;
            };
            this.isValueChanged = function () {

                return ($select.val() !== defaultValue);
            };
            this.validate = function () {
                return {
                    valid: true,
                    msg: null
                };
            };
            this.init();
        }

        if (reloadData != null && reloadData != undefined) {
            data = reloadData;
        }
        grid = new Slick.Grid("#shiftRequestGrid", data, columns, options);
        grid.setSelectionModel(new Slick.RowSelectionModel({ selectActiveRow: false }));
        grid.registerPlugin(checkboxSelector);
        var employeeId;
        grid.onCellChange.subscribe(function (e, args) {
            if (args.cell === 4) {
                var currentCell = grid.getDataItem(args.row);
                var employeeIdHidden = currentCell.EmployeeIDHidden;
                employeeId = currentCell.EmployeeID;
                var shiftManagementId = currentCell.ShiftManagementID;

                // ReSharper disable once UseOfImplicitGlobalInFunctionScope
                var webMethodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/CheckIfShiftManagementItemIsExist";
                debugger;
                var checkUserExistPromise = $.ajax({
                    type: "POST",
                    url: webMethodUrl,
                    data: '{"employeeId":' + employeeId + ', "month":' + month + ', "year":' + year + '}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false
                });

                checkUserExistPromise.then(function (data) {
                    debugger;
                    if (data.d === true) {
                        alert($.trim($("#duplicate-confirm").text()));
                        clearAllRowData(args.row);
                    }
                    else {
                        if (employeeId !== "" && employeeIdHidden) {
                            fnConfirmDeleteDialog(args.row, employeeIdHidden, employeeId, shiftManagementId);
                        }
                        else {
                            if (checkUniqueEmployeeValue(currentCell.EmployeeID)) {
                                employeeId = args.item.EmployeeID;
                                updateEmployeeName(args.row, employeeId);
                            }
                            else {
                                fnAlertDuplicateDialog();
                                clearAllRowData(args.row);
                            }
                        }
                    }
                });
            }
            setRowIsChanged(args.row);

            function fnConfirmDeleteDialog(row, oldValue, newValue, shiftManagementId) {
                debugger;
                var txt = $("#dialog-confirm-change").text();
                var d = confirm($.trim(txt));
                if (d === true) {
                    clearAllRowData(row);
                    setNewValueForCell(row, newValue, shiftManagementId);
                } else {
                    fnSetCurrentValue(row, oldValue);
                }

            }
            function fnSetCurrentValue(rowPosition, oldValue) {
                var girdData = grid.getData();
                girdData[rowPosition].EmployeeID = oldValue;
                grid.setData(girdData);
                grid.render();
            }

            function fnAlertDuplicateDialog() {
                alert($.trim($("#dialog-inform-dulicate-data").text()));
            }

            function setNewValueForCell(rowPosition, newValue, shiftManagementId) {
                debugger;
                if (checkUniqueEmployeeValue(employeeId)) {
                    fnAlertDuplicateDialog();
                    deletedItemsIdArray.push(shiftManagementId);
                    clearAllRowData(args.row);
                }
                else {
                    var gridData = grid.getData();
                    gridData[rowPosition].EmployeeID = newValue;
                    gridData[rowPosition].EmployeeIDHidden = "";
                    deletedItemsIdArray.push(shiftManagementId);
                    grid.setData(gridData);
                    grid.render();
                }
            }
        }
		);

        function setRowIsChanged(rowPosision) {
            var gridata = grid.getData();
            gridata[rowPosision].IsChanged = true;
            grid.setData(gridata);
            grid.render();
        }

        grid.onSelectedRowsChanged.subscribe(function (evt, args) {
            if (args.rows.length === 0) {
                $("#btnShiftRequestDelete").hide();
            }
            else {
                $("#btnShiftRequestDelete").show();
            }
        });
        function clearAllRowData(row) {
            debugger;
            var gridData = grid.getData();
            // ReSharper disable once MissingHasOwnPropertyInForeach
            for (var member in gridData[row]) {
                delete gridData[row][member];
            }
            grid.setData(gridData);
            grid.render();
        }

        function checkUniqueEmployeeValue(employeeId) {
            var items = grid.getData();
            var check = 0;
            for (var i = 0; i < items.length; i++) {
                if (items[i].EmployeeID === employeeId) {
                    check++;
                }
            }
            if (check <= 1) {
                return true;
            }
            else {
                return false;
            }
        }

        function updateEmployeeName(rowPosition, emplyeeId) {
            // ReSharper disable once UseOfImplicitGlobalInFunctionScope
            var webServiceUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetEmployeeNameByEmployeeID";
            $.ajax({
                type: "POST",
                url: webServiceUrl,
                data: '{"employeeID":"' + emplyeeId + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false,
                success: function (result) {
                    if (result.d != null) {
                        var data = grid.getData();
                        data[rowPosition].EmployeeName = result.d;
                        grid.setData(data);
                        grid.render();
                    }
                }
			   ,
                error: function (jqXhr) {
                    if (jqXhr.status === 500) {
                        console.log('Internal error: ' + jqXhr.responseText);
                    } else {
                        console.log('Unexpected error.');
                    }
                }
            });
        }

        $('#shiftRequestGrid').on('shown', grid.resizeCanvas);

        $("#btnAddNewEmployee").click(function (e) {
            var data = grid.getData();
            var currRowNum = data.length;
            var newrow = jQuery.extend({}, data[currRowNum]);
            newrow.title = 'newRecord';
            var position = (currRowNum + 1);
            data.splice(position, 0, newrow);
            grid.setData(data);
            grid.render();
            e.preventDefault();
        });

        $('#btnShiftRequestSave').unbind("click").click(function (e) {
            var gridData = grid.getData();
            var shiftRequestList = [];
            for (var index = 0; index < gridData.length; index++) {
                var item = new Object();
                item.EmployeeID = gridData[index]["EmployeeID"];
                if (gridData[index]["ShiftManagementID"] == undefined) {
                    item.ShiftManagementID = "";
                }
                else {
                    item.ShiftManagementID = gridData[index]["ShiftManagementID"];
                }
                if (gridData[index]["ShiftManagementID"] == undefined) {
                    item.IsChanged = true;
                }
                else {
                    item.IsChanged = gridData[index]["IsChanged"];
                }

                item.Day1 = gridData[index]["1"];
                item.Day2 = gridData[index]["2"];
                item.Day3 = gridData[index]["3"];
                item.Day4 = gridData[index]["4"];
                item.Day5 = gridData[index]["5"];
                item.Day6 = gridData[index]["6"];
                item.Day7 = gridData[index]["7"];
                item.Day8 = gridData[index]["8"];
                item.Day9 = gridData[index]["9"];
                item.Day10 = gridData[index]["10"];
                item.Day11 = gridData[index]["11"];
                item.Day12 = gridData[index]["12"];
                item.Day13 = gridData[index]["13"];
                item.Day14 = gridData[index]["14"];
                item.Day15 = gridData[index]["15"];
                item.Day16 = gridData[index]["16"];
                item.Day17 = gridData[index]["17"];
                item.Day18 = gridData[index]["18"];
                item.Day19 = gridData[index]["19"];
                item.Day20 = gridData[index]["20"];
                item.Day21 = gridData[index]["21"];
                item.Day22 = gridData[index]["22"];
                item.Day23 = gridData[index]["23"];
                item.Day24 = gridData[index]["24"];
                item.Day25 = gridData[index]["25"];
                item.Day26 = gridData[index]["26"];
                item.Day27 = gridData[index]["27"];
                item.Day28 = gridData[index]["28"];
                item.Day29 = gridData[index]["29"];
                item.Day30 = gridData[index]["30"];
                item.Day31 = gridData[index]["31"];
                shiftRequestList.push(item);
            }

            if (grid.length === 0) {
                alert(ShiftRequestPage.NoDataToSave);
                return;
            }

            var mm = $('#stadaHiddenMonth').val();
            var yyyy = $('#stadaHiddenYear').val();
            e.preventDefault();
            // ReSharper disable once UseOfImplicitGlobalInFunctionScope
            var webServiceUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/SaveShiftRequest";
            $.ajax({
                type: "POST",
                url: webServiceUrl,
                data: JSON.stringify({ month: mm, year: yyyy, jsonData: shiftRequestList, deletedItems: deletedItemsIdArray }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false,
                success: function (result) {

                    if (result != null) {
                        if (result.d.CodeMessageResult.Code === 0) {
                            ShiftRequestGrid(mm, yyyy);
                            alert(ShiftRequestPage.SaveSuccessfullMessage);
                        }
                        else {
                            console.log(result.d.CodeMessageResult.Message);
                        }
                    }
                    else {
                        alert(ShiftRequestPage.SaveErrorMessage);
                    }
                }
			 ,
                error: function (jqXhr) {
                    if (jqXhr.status === 500) {
                        console.log('Internal error: ' + jqXhr.responseText);
                    } else {
                        console.log('Unexpected error.');
                    }
                }
            });
        });

        $('#btnShiftRequestDelete').click(function () {
            fnOpenDeleteShiftDialog();
        });

        $('#btnShiftRequestCancel').click(function (e) {
            var cancelTextQuestion = $('#cancel-confirm').html();
            var r = confirm($.trim(cancelTextQuestion));
            if (r === true) {
                cancelProcessing();
            }
            e.preventDefault();
        });
        function fnOpenDeleteShiftDialog() {

            var cancelTextQuestion = $('#dialog-confirm').html();
            var r = confirm($.trim(cancelTextQuestion));
            if (r === true) {
                deteleSelectedRows();
            }
        }

        // function fnAlertUserExits() {
        //     $("#duplicate-confirm").dialog({
        //         resizable: false,
        //         modal: true,
        //         title: ShiftRequestPage.DuplicateConfirmationDialogTitle,
        //         height: 250,
        //         width: 400,
        //         buttons: [{
        //             text: ShiftRequestPage.OkButton,
        //             click: function () {
        //                 $(this).dialog('close');
        //             }
        //         }]
        //     });
        // }

        function cancelProcessing() {
            // window.location.href = _spPageContextInfo.webAbsoluteUrl + "/SitePages/ShiftManagement.aspx";
            window.location.href = "/SitePages/ShiftManagement.aspx";
        }

        function deteleSelectedRows() {
            var pureArrayData = grid.getData();
            var gridDataArray = pureArrayData.slice();
            // var selectedItems = [];
            var selectedIndexes = grid.getSelectedRows();

            if (selectedIndexes.length === 0) {
                alert(ShiftRequestPage.NoItemDelete);
            }
            else {
                var gridData = grid.getData();
                $.each(selectedIndexes, function (index, value) {
                    var item = gridDataArray[value];
                    if (item && item.ShiftManagementID) {
                        deletedItemsIdArray.push(item["ShiftManagementID"]);
                    }
                    var indexItem = gridData.indexOf(item);
                    gridData.splice(indexItem, 1);
                });

                grid.setData(gridData);
                grid.render();
                grid.updateRowCount();
                grid.resizeCanvas();
                grid.setSelectedRows([]);
            }
        }
    }
}