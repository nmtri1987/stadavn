
var OvertimeRequestPage = {
    SaveButton: "Save",
    CancelButton: "Cancel",
    Employee: "Employee",
    WorkingHour: "Working Hour(s)",
    OvertimeHourFrom: "Overtime Hour From",
    OvertimeHourTo: "Overtime Hour To",
    Task: "Work Content",
    TransportAt: "Company Transport",
    Edit: "Edit",
    Delete: "Delete",
    EmployeeID: "Employee ID",
    OvertimeHour: "Overtime Hour",
    OrderNumber: "No",
    //Message

    Message_HourG024: "Hour must be greater than 0 and less than 24",
    Message_CompareHour: "To Hour must greater than From Hour",
    Message_FieldMustBeNumber: "Only numbers can go here",
    Message_FieldMustBeNotBlank: "You can't leave this blank",
    Message_DeleteRow: "Are you sure you want to delete this row?",
    Message_ErrorOccurred: "An error occurred. Please try again!",
    Message_CannotGetData: "Can not get data from server. Please try again!",
    Message_Modal_UserExist: "This employee already exists. Please select another one!",

    PageResourceFileName: "RBVHStadaWebpages",
    ListResourceFileName: "RBVHStadaLists"
};

(function () {
    SP.SOD.registerSod(OvertimeRequestPage.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + OvertimeRequestPage.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(OvertimeRequestPage.ListResourceFileName, "Res", OnListResourcesOvertimeReady);
    SP.SOD.registerSod(OvertimeRequestPage.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + OvertimeRequestPage.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(OvertimeRequestPage.PageResourceFileName, "Res", OnPageResourcesOvertimeReady);

})();



$(function () {
    OverTimeRequestGrid.Overtime_PrepareDataForGrid();
    OvertimeRequestPage.LoadPage();
    OvertimeRequestPage.RegisterEvent();
});


function OnListResourcesOvertimeReady() {
    
    OvertimeRequestPage.Employee = Res.overtimeDetail_Employee;
    OvertimeRequestPage.WorkingHour = Res.overtimeDetail_WorkingHour_Label;
    OvertimeRequestPage.OvertimeHourFrom = Res.overtimeDetail_OvertimeHourFrom;
    OvertimeRequestPage.OvertimeHourTo = Res.overtimeDetail_OvertimeHourTo;
    OvertimeRequestPage.Task = Res.overtimeDetail_Task;
    OvertimeRequestPage.TransportAt = Res.overtimeDetail_TransportAt;
    OvertimeRequestPage.Edit = Res.overtimeDetail_EditLabel;
    OvertimeRequestPage.Delete = Res.overtimeDetail_DeleleLabel;
    OvertimeRequestPage.EmployeeID = Res.overtimeDetail_EmployeeID;
    OvertimeRequestPage.OvertimeHour = Res.overtimeDetail_OvertimeHour;
    OvertimeRequestPage.OrderNumber = Res.overtimeDetail_OrderNumber;
}

function OnPageResourcesOvertimeReady() {

    OvertimeRequestPage.SaveButton = Res.saveButton;
    OvertimeRequestPage.CancelButton = Res.cancelButton;
    OvertimeRequestPage.Message_HourG024 = Res.overtime_ValidateHour;
    OvertimeRequestPage.Message_FieldMustBeNumber = Res.overtime_FieldMustBeNumber;
    OvertimeRequestPage.Message_FieldMustBeNotBlank = Res.overtime_FieldMustBeNotBlank;
    OvertimeRequestPage.Message_DeleteRow = Res.overtime_DeleteRow;
    OvertimeRequestPage.Message_ErrorOccurred = Res.overtime_ErrorOccurred;
    OvertimeRequestPage.Message_CannotGetData = Res.overtime_CannotGetData;
    OvertimeRequestPage.Message_CompareHour = Res.overtime_ValidateTwoHour;
    OvertimeRequestPage.Message_Modal_UserExist = Res.overtime_Modal_UserExist;

}
//Grid variables init
var OverTime_GridColumnName =
    {
        EmployeeLookupID: "EmployeeLookupID",
        OrderNumber: "No",
        EmployeeName: OvertimeRequestPage.Employee,
        EmployeeID: "Employee ID",
        WorkingHour: OvertimeRequestPage.WorkingHour,
        OvertimeHour: "Overtime Hour",
        OvertimeFrom: OvertimeRequestPage.OvertimeHourFrom,
        OvertimeTo: OvertimeRequestPage.OvertimeHourTo,
        TransportAt: OvertimeRequestPage.TransportAt,
        Task: OvertimeRequestPage.Task,
        Edit: "Edit",
        Delete: "Delete"
    };
var OverTime_GridColumnField =
    {
        EmployeeLookupID: "EmployeeLookupID",
        OrderNumber: "OrderNumber",
        EmployeeName: "EmployeeName",
        EmployeeID: "EmployeeID",
        WorkingHour: "WorkingHour",
        OvertimeHour: "OvertimeHour",
        OvertimeFrom: "OvertimeFrom",
        OvertimeTo: "OvertimeTo",
        TransportAt: "TransportAt",
        Task: "Task",
        Edit: "Edit",
        Delete: "Delete"
    };

var PageGridControl =
    {
        Requester: "#overtimePage_Requester",
        Location: "#overtimePage_Location",
        SumOfEmployee: "#overtimePage_SumOfEmployee",
        SumOfMeal: "#overtimePage_SumOfMeal",
        OtherRequirement: "#overtimePage_OtherRequirement",
        FromDateError: "#overtimePage_FromDate_Error",
        ToDateError: "#overtimePage_ToDate_Error",
        Department: "#overtimePage_Department",
        ButtonSave: "#overtime_SaveButton",
        ButtonCancel: "#overtime_CancelButton",
        DepartmentError: "#overtimePage_Department_Error",
        RequesterError: "#overtimePage_Requester_Error",
        LocationError: "#overtimePage_Location_Error",
        SumOfMealError: "#overtimePage_Meal_Error",
        SumOfEmployeeError: "#overtimePage_SumEmployee_Error"
    }
var ModalControl =
    {
        ModalID: "#modal-content",
        ModalMode: "#overtime_modal_ModalMode",
        EditModeRowIndex: "#overtime_modal_EditModeRowIndex", // Just use when edit 
        Requester: "#overtime_modal_Requester",
        Requester_Error: "#overtime_modal_Requester_Error",
        WorkingHour: "#overtime_modal_WorkingHours",
        WorkingHour_Error: "#overtime_modal_WorkingHours_Error",
        OvertimeFrom: "#overtime_modal_OvertimeFrom",
        OverTimeFrom_Error: "#overtime_modal_OvertimeFrom_Error",
        OvertimeTo: "#overtime_modal_OvertimeTo",
        OverTimeTo_Error: "#overtime_modal_OvertimeTo_Error",
        Task: "#overtime_modal_Task",
        TransportAt: "#overtime_modal_TransportAt",
        ButtonSave: "#overtime_modal_btnSave",
        ButtonClose: "#overtime_modal_btnClose",

        SetDataForModal: function (requesterList, requesterListSelectedValue, workingHour, overTimeFrom, overTimeTo, taskContent, transportList, transportListSelectedValue, editRowIndex) {
            //Set fileds data to empty

            $(ModalControl.WorkingHour).val("");
            $(ModalControl.WorkingHour_Error).html("");
            $(ModalControl.OvertimeFrom).val("");
            $(ModalControl.OverTimeFrom_Error).html("");
            $(ModalControl.OvertimeTo).val("");
            $(ModalControl.OverTimeTo_Error).html("");
            $(ModalControl.Task).val("");
            $(ModalControl.EditModeRowIndex).html();
            $(ModalControl.Requester).empty();
            $(ModalControl.TransportAt).empty();

            var selectedString = "";
            for (var rIndex = 0; rIndex < requesterList.length; rIndex++) {
                selectedString = "";
                if (requesterListSelectedValue > 0) {
                    if (requesterList[rIndex]["LookupId"] === 1 * requesterListSelectedValue) {
                        selectedString = 'selected="selected"';
                    }
                }
                $(ModalControl.Requester).append('<option data-employee-id = "' + requesterList[rIndex]["EmployeeID"] + '" ' + selectedString + ' value="' + requesterList[rIndex]["LookupId"] + '">' + requesterList[rIndex].FullName + "</option>");
            }
            $(ModalControl.WorkingHour).val(workingHour);
            $(ModalControl.OvertimeFrom).val(overTimeFrom);
            $(ModalControl.OvertimeTo).val(overTimeTo);
            $(ModalControl.Task).val(taskContent);

            if (editRowIndex != undefined) {

                $(ModalControl.EditModeRowIndex).html(editRowIndex);
            }

            $(ModalControl.TransportAt).append('<option value="" > --Select-- </option>');
            for (var tIndex = 0; tIndex < transportList.length; tIndex++) {
                selectedString = "";
                if (requesterListSelectedValue > 0) {
                    if (transportList[tIndex]["Value"] === transportListSelectedValue) {
                        selectedString = 'selected="selected"';
                    }
                }
                $(ModalControl.TransportAt).append('<option value="' + transportList[tIndex]["Value"] + '" ' + selectedString + ">" + transportList[tIndex].Text + "</option>");
            }
        },

        AddNewEmployeeModal: function () {
            // ReSharper disable once UseOfImplicitGlobalInFunctionScope
            debugger;
            var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetOvertimeModalData";
            var getModalDataPromise = $.ajax({
                type: "POST",
                url: methodUrl,
                //data: '{"departmentId":' + month + ', "year":' + year + '}',
                data: '{"departmentId": ' + null + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false
            });

            getModalDataPromise.then(function (data) {
                if (data.d != null) {
                    if (data.d.CodeMessageResult.Code === 0) {
                        ModalControl.SetDataForModal(data.d.EmployeeInfoList, 0, data.d.WorkingHour, data.d.OvertimeFrom, data.d.OvertimeTo, data.d.Task, data.d.TransportList, 0, -1);
                        $(ModalControl.ModalID).modal({
                            show: true
                        });
                    }
                    else {
                        alert(data.d.CodeMessageResult.Message);
                    }
                }
                else {
                    alert(OvertimeRequestPage.Message_CannotGetData);
                }
            });
        }
    };
var CommonFunction =
    {
        ValidateControl: function (controlValue, controlErrorLabel) {
            if (controlValue.length > 0) {
                $(controlErrorLabel).html("");
                $(controlErrorLabel).hide();
                if ($.isNumeric(controlValue)) {
                    $(controlErrorLabel).html("");
                    $(controlErrorLabel).hide();
                    if (CommonFunction.ValidateTime(controlValue)) {
                        $(controlErrorLabel).html("");
                        $(controlErrorLabel).hide();
                        return true;
                    }
                    else {
                        $(controlErrorLabel).html(OvertimeRequestPage.Message_HourG024);
                        $(controlErrorLabel).show();
                        return false;
                    }
                }
                else {
                    $(controlErrorLabel).html(OvertimeRequestPage.Message_FieldMustBeNumber);
                    $(controlErrorLabel).show();
                    return false;
                }
            }
            else {
                $(controlErrorLabel).html(OvertimeRequestPage.Message_FieldMustBeNotBlank);
                $(controlErrorLabel).show();
                return false;
            }
        },
        ValidateTime: function (hour) {
            if (hour != undefined) {
                return (hour >= 0 && hour <= 24);
            }
            else {
                return false;
            }
        },

    }
var OverTimeRequestGrid =

    {
        data: [],
        grid: {},
        CheckDuplicateData: function (mode, gData, selectedRequesterId) {

            var returnValue = false;

            if (gData != undefined) {
                if (gData.length == 0) {
                    return false;
                }
                var checkCount = 0;
                for (var index = 0; index < gData.length; index++) {

                    if (gData[index][OverTime_GridColumnField.EmployeeID].length > 0 && gData[index][OverTime_GridColumnField.EmployeeID] === selectedRequesterId) {
                        checkCount++;
                    }
                }
                if (checkCount === 0) {
                    returnValue = false;
                }
                else {
                    if (mode === "Edit") {
                        if (checkCount === 1) {
                            returnValue = false;
                        }
                        else {
                            return true;
                        }
                    }
                    else {
                        returnValue = true;
                    }
                }
            }
            return returnValue;
        },


        GirdEditCell: function (element) {
            $(ModalControl.ModalMode).html("Edit");
            var employeeId = $(element).attr("data-id");
            if (employeeId != "" && employeeId.length > 0) {
                OverTimeRequestGrid.EditGridRow(employeeId);
            }
            else {
                alert(OvertimeRequestPage.Message_ErrorOccurred);
            }
        },
        GridDeleteCell: function (element) {
            var employeeId = $(element).attr("data-id");
            if (employeeId != "" && employeeId.length > 0) {
                //Delete current row
                var mRes = confirm(OvertimeRequestPage.Message_DeleteRow);
                if (mRes == true) {
                    OverTimeRequestGrid.DeleteGridRow(employeeId);
                }
            }
            else {
                alert(OvertimeRequestPage.Message_ErrorOccurred);
            }
        },

        Overtime_PrepareDataForGrid: function () {

            var columns = [];
            for (var i = 0; i < 0; i++) {
                var d = (OverTimeRequestGrid.data[i] = {});
                d[OverTime_GridColumnField.OrderNumber] = i + 1;
                d[OverTime_GridColumnField.EmployeeID] = "";
                d[OverTime_GridColumnField.EmployeeName] = "";
                d[OverTime_GridColumnField.WorkingHour] = "";
                d[OverTime_GridColumnField.OvertimeHour] = "";
                d[OverTime_GridColumnField.OvertimeTo] = "";
                d[OverTime_GridColumnField.OvertimeFrom] = "";
                d[OverTime_GridColumnField.TransportAt] = "";
                d[OverTime_GridColumnField.Task] = "";
            }

            function editActionFormatter(row, cell, value, columnDef, dataContext) {
                var employeeId = dataContext[OverTime_GridColumnField.EmployeeID];
                var html = "<div style='padding-left:45%'  data-id='" + employeeId + "'  onclick='OverTimeRequestGrid.GirdEditCell(this)' class='btn btn-xs'><span class='glyphicon glyphicon-pencil'></span></div>";
                return html;
            }
            function deleteActionFormatter(row, cell, value, columnDef, dataContext) {
                var employeeId = dataContext[OverTime_GridColumnField.EmployeeID];
                var html = "<div style='padding-left:45%' data-id='" + employeeId + "' onclick='OverTimeRequestGrid.GridDeleteCell(this)' class='btn btn-xs'><span class='glyphicon  glyphicon-remove'></span></div>";
                return html;
            }
            columns.push({ id: "EmployeeLookupIDHidden", name: OverTime_GridColumnName.EmployeeLookupID, field: OverTime_GridColumnField.EmployeeLookupID, width: 0, minWidth: 0, maxWidth: 0, cssClass: "reallyHidden", headerCssClass: "reallyHidden" });
            columns.push({ id: "OvertimeFromHidden", name: OverTime_GridColumnName.OvertimeFrom, field: OverTime_GridColumnField.OvertimeFrom, width: 0, minWidth: 0, maxWidth: 0, cssClass: "reallyHidden", headerCssClass: "reallyHidden" });
            columns.push({ id: "OvertimeToHidden", name: OverTime_GridColumnName.OvertimeTo, field: OverTime_GridColumnField.OvertimeTo, width: 0, minWidth: 0, maxWidth: 0, cssClass: "reallyHidden", headerCssClass: "reallyHidden" });
            columns.push({ id: "OrderNumber", name: OvertimeRequestPage.OrderNumber, field: OverTime_GridColumnField.OrderNumber, maxWidth: 50 });
            columns.push({ id: "EmployeeName", name: OvertimeRequestPage.Employee, field: OverTime_GridColumnField.EmployeeName, minWidth: 200, maxWidth: 400 });
            columns.push({ id: "EmployeeID", name: OvertimeRequestPage.EmployeeID, field: OverTime_GridColumnField.EmployeeID, width: 150 });
            columns.push({ id: "WorkingHour", name: OvertimeRequestPage.WorkingHour, field: OverTime_GridColumnField.WorkingHour, maxWidth: 170 });
            columns.push({ id: "OvertimeHour", name: OvertimeRequestPage.OvertimeHour, field: OverTime_GridColumnField.OvertimeHour, maxWidth: 170 });
            columns.push({ id: "Task", name: OvertimeRequestPage.Task, field: OverTime_GridColumnField.Task, minWidth: 300 });
            columns.push({ id: "TransportAt", name: OvertimeRequestPage.TransportAt, field: OverTime_GridColumnField.TransportAt, maxWidth: 200, minwidth: 100 });
            columns.push({ id: "Edit", name: OvertimeRequestPage.Edit, field: OverTime_GridColumnField.Edit, maxWidth: 100, resizable: false, formatter: editActionFormatter });
            columns.push({ id: "Delete", name: OvertimeRequestPage.Delete, field: OverTime_GridColumnField.Delete, maxWidth: 100, resizable: false, formatter: deleteActionFormatter });

            var options = {
                editable: true,
                enableAddRow: true,
                enableCellNavigation: true,
                asyncEditorLoading: true,
                forceFitColumns: true,
                autoEdit: false,
                syncColumnCellResize: true,
                topPanelHeight: 24,
                enableColumnReorder: false,
                autoHeight: true,
                autoWidth: true
            };


            OverTimeRequestGrid.grid = new Slick.Grid("#overTimeRequestGrid", OverTimeRequestGrid.data, columns, options);

            $("#overtime_AddnewEmployeeButton").click(function () {
                $(ModalControl.ModalMode).html("AddNew");
                ModalControl.AddNewEmployeeModal();
            });

            $(ModalControl.ButtonSave).on("click", function (e) {
                if (CommonFunction.ValidateControl($(ModalControl.WorkingHour).val(), ModalControl.WorkingHour_Error) === true &&
                    CommonFunction.ValidateControl($(ModalControl.OvertimeFrom).val(), ModalControl.OverTimeFrom_Error) === true &&
                    CommonFunction.ValidateControl($(ModalControl.OvertimeTo).val(), ModalControl.OverTimeTo_Error) === true) {
                    if (parseInt($(ModalControl.OvertimeFrom).val()) < parseInt($(ModalControl.OvertimeTo).val())) {
                        //hide message
                        $(ModalControl.OverTimeTo_Error).html("");
                        $(ModalControl.OverTimeTo_Error).hide();

                        var selectedRequesterLookupId = $(ModalControl.Requester + " :selected").val();
                        var selectedRequesterEmpId = $(ModalControl.Requester + " :selected").attr("data-employee-id");
                        var selectedRequesterFullName = $(ModalControl.Requester + " :selected").text();
                        var workingHour = $(ModalControl.WorkingHour).val();
                        var overTimeFrom = $(ModalControl.OvertimeFrom).val();
                        var overTimeTo = $(ModalControl.OvertimeTo).val();
                        var taskContent = $(ModalControl.Task).val();
                        var seletectedTransportValue = $(ModalControl.TransportAt + " :selected").val();

                        var modalMode = $(ModalControl.ModalMode).html();
                        overTimeAddModalDataToGrid(modalMode, e, selectedRequesterLookupId, selectedRequesterEmpId, selectedRequesterFullName, workingHour, overTimeFrom, overTimeTo, taskContent, seletectedTransportValue);
                        e.preventDefault();
                    }
                    else {
                        $(ModalControl.OverTimeTo_Error).html(OvertimeRequestPage.Message_CompareHour);
                        $(ModalControl.OverTimeTo_Error).show();
                        e.preventDefault();
                    }
                }
                else {
                    e.preventDefault();
                }
            });

            $(ModalControl.ButtonClose).on('click', function (e) {
                $(ModalControl.WorkingHour_Error).html("");
                $(ModalControl.OverTimeFrom_Error).html("");
                $(ModalControl.OverTimeTo_Error).html("");
                $(ModalControl.ModalID).modal("hide");
                e.preventDefault();
            });


            function overTimeAddModalDataToGrid(mode, event, selectedRequesterLookupId, selectedRequesterEmpId, selectedRequesterFullName, workingHour, overTimeFrom, overTimeTo, taskContent, seletectedTransportValue) {

                var gridCurrentData = OverTimeRequestGrid.grid.getData();
                if (!OverTimeRequestGrid.CheckDuplicateData(mode, gridCurrentData, selectedRequesterEmpId)) {
                    debugger;
                    $(ModalControl.Requester_Error).html(OvertimeRequestPage.Message_Modal_UserExist);
                    $(ModalControl.Requester_Error).hide();
                    if (mode === "AddNew") {
                        //Add new row to end of grid
                        var addData = (gridCurrentData[gridCurrentData.length] = {});
                        addData[OverTime_GridColumnField.EmployeeLookupID] = selectedRequesterLookupId;
                        addData[OverTime_GridColumnField.OrderNumber] = gridCurrentData.length;
                        addData[OverTime_GridColumnField.EmployeeID] = selectedRequesterEmpId;
                        addData[OverTime_GridColumnField.EmployeeName] = selectedRequesterFullName;
                        addData[OverTime_GridColumnField.WorkingHour] = workingHour;
                        addData[OverTime_GridColumnField.OvertimeHour] = (overTimeTo - overTimeFrom) + " (" + overTimeFrom + "h - " + overTimeTo + "h)";
                        addData[OverTime_GridColumnField.OvertimeFrom] = overTimeFrom;
                        addData[OverTime_GridColumnField.OvertimeTo] = overTimeTo;

                        addData[OverTime_GridColumnField.TransportAt] = seletectedTransportValue;
                        addData[OverTime_GridColumnField.Task] = taskContent;
                    }

                    if (mode === "Edit") {
                        debugger;
                        //update current row with new data
                        var editRowIndex = $(ModalControl.EditModeRowIndex).html();
                        if (editRowIndex != undefined && (1 * editRowIndex) > -1) {
                            if (!OverTimeRequestGrid.CheckDuplicateData(mode, gridCurrentData, selectedRequesterEmpId)) {
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.EmployeeLookupID] = selectedRequesterLookupId;
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.EmployeeID] = selectedRequesterEmpId;
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.EmployeeName] = selectedRequesterFullName;
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.WorkingHour] = workingHour;
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.OvertimeFrom] = overTimeFrom;
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.OvertimeTo] = overTimeTo;
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.OvertimeHour] = (overTimeTo - overTimeFrom) + " (" + overTimeFrom + "h - " + overTimeTo + "h)";
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.Task] = taskContent;
                                gridCurrentData[editRowIndex][OverTime_GridColumnField.TransportAt] = seletectedTransportValue;
                            }
                            else {
                                $(ModalControl.Requester_Error).html(OvertimeRequestPage.Message_Modal_UserExist);
                                $(ModalControl.Requester_Error).show();
                                return;
                            }

                        }
                        else {
                            alert(OvertimeRequestPage.Message_ErrorOccurred);
                        }
                    }
                    OverTimeRequestGrid.grid.setData(gridCurrentData);
                    OverTimeRequestGrid.grid.render();
                    //Set sumMeal && sumEmployee
                    $(PageGridControl.SumOfMeal).val(gridCurrentData.length);
                    $(PageGridControl.SumOfEmployee).val(gridCurrentData.length);
                    $(ModalControl.ModalID).modal("hide");
                }
                else {
                    debugger;
                    $(ModalControl.Requester_Error).html(OvertimeRequestPage.Message_Modal_UserExist);
                    $(ModalControl.Requester_Error).show();
                    // e.preventDefault();
                }
                event.preventDefault();
            }
        },
        DeleteGridRow: function (employeeId) {
            var gridData = OverTimeRequestGrid.grid.getData();
            $.each(gridData, function (idx, value) {
                var item = gridData[idx];
                if (item && item[OverTime_GridColumnField.EmployeeID] === employeeId) {
                    var index = gridData.indexOf(item);
                    gridData.splice(index, 1);
                    OverTimeRequestGrid.grid.setData(gridData);
                    OverTimeRequestGrid.grid.render();
                    OverTimeRequestGrid.grid.resizeCanvas();
                }
            });
        },
        EditGridRow: function (employeeId) {
            if (employeeId.trim().length > 0) {
                var gridData = OverTimeRequestGrid.grid.getData();
                var rowData = OverTimeRequestGrid.GetDataRow(gridData, employeeId);
                var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetOvertimeModalData";
                var getModalDataPromise = $.ajax({
                    type: "POST",
                    url: methodUrl,
                    data: '{"departmentId": ' + null + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false
                });

                getModalDataPromise.then(function (data) {
                    if (data.d != null) {
                        if (data.d.CodeMessageResult.Code === 0) {
                            debugger;
                            ModalControl.SetDataForModal(data.d.EmployeeInfoList, rowData.EmployeeLookupID, rowData.WorkingHour, rowData.OvertimeFrom, rowData.OvertimeTo, rowData.Task, data.d.TransportList, rowData.TransportSelectValue, rowData.RowIndex);
                            $(ModalControl.ModalID).modal({
                                show: true
                            });
                        }
                        else {
                            alert(data.d.CodeMessageResult.Message);
                        }
                    }
                    else {
                        alert(OvertimeRequestPage.Message_ErrorOccurred);
                    }
                });
            }
            else {
                alert(OvertimeRequestPage.Message_CannotGetData);
            }
        },
        //Get data of employee Id
        GetDataRow: function (gData, employeeId) {
            var item = {};
            var result = $.grep(gData, function (e) { return e.EmployeeID === employeeId; });
            var index = -1;
            var len;
            for (var i = 0, len = gData.length; i < len; i++) {
                if (gData[i].EmployeeID === employeeId) {
                    index = i;
                    break;
                }
            }

            if (result.length === 1) {
                item.RowIndex = index;
                item.WorkingHour = result[0][OverTime_GridColumnField.WorkingHour];
                item.OvertimeTo = result[0][OverTime_GridColumnField.OvertimeTo];
                item.OvertimeFrom = result[0][OverTime_GridColumnField.OvertimeFrom];
                item.Task = result[0][OverTime_GridColumnField.Task];
                item.TransportSelectValue = result[0][OverTime_GridColumnField.TransportAt];
                item.EmployeeID = result[0][OverTime_GridColumnField.EmployeeID];
                item.EmployeeLookupID = result[0][OverTime_GridColumnField.EmployeeLookupID];
                item.WorkingHour = result[0][OverTime_GridColumnField.OvertimeFrom];
            }
            return item;
        }
    }

var OvertimeRequestPage =
    {
        LoadPage: function () {
            var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/LoadOvertimePage";
            var getPageDataPromise = $.ajax({
                type: "POST",
                url: methodUrl,
                data: null,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false
            });
            getPageDataPromise.then(
                function (data) {
                    if (data != null) {
                        if (data.d.CodeMessageResult.Code === 0) //No error
                        {
                            OvertimeRequestPage.SetDataForPageControl(data.d.RequesterInfo, data.d.FactoryList, data.d.DepartmentList);
                        }
                        else // show error
                        {
                            alert(OvertimeRequestPage.Message_ErrorOccurred);
                        }
                    }
                    else {
                        alert(OvertimeRequestPage.Message_CannotGetData);
                    }
                },
                function (errorData) {
                });
        }
        ,
        SetDataForPageControl: function (requesterList, factoryList, departmentList) {
            //  var currDate = new Date($.now());
            $(PageGridControl.Department).empty();
            $(PageGridControl.Location).empty();
            $(PageGridControl.Requester).empty();
            // $(PageGridControl.FromDate).val(currDate);
            // $(PageGridControl.ToDate).val(currDate);
            $(PageGridControl.OtherRequirement).val("");
            $(PageGridControl.SumOfEmployee).val(0);
            $(PageGridControl.SumOfMeal).val(0);

            if (requesterList != null) {
                var isdmin = Boolean(requesterList["IsAdmin"]);
                var currentEmployee = requesterList["CurrentEmployee"];
                var employeeList = requesterList["ListEmployees"];
                //populate date to department and factory list
                for (var fIndex = 0; fIndex < factoryList.length; fIndex++) {
                    var facOption = "<option value='" + factoryList[fIndex].FactoryId + "' >" + factoryList[fIndex].FactoryName + "</option>";
                    $(PageGridControl.Location).append(facOption);
                }

                for (var dIndex = 0; dIndex < departmentList.length; dIndex++) {
                    var depOption = "<option value='" + departmentList[dIndex].DepartmentId + "' >" + departmentList[dIndex].DepartmentName + "</option>";
                    $(PageGridControl.Department).append(depOption);
                }


                for (var i = 0; i < employeeList.length; i++) {
                    var deptId = employeeList[i].DepartmentId;
                    var locId = employeeList[i].FactoryId;
                    var option = "<option department-id='" + deptId + "' location-id='" + locId + "'  value='" + employeeList[i].LookupId + "'>" + employeeList[i].FullName + "</option>";
                    $(PageGridControl.Requester).append(option);
                }
                if (currentEmployee["LookupId"] > 0) {
                    $(PageGridControl.Requester).val(currentEmployee["LookupId"]).change();
                }
                if (currentEmployee["DepartmentId"] > 0) {
                    $(PageGridControl.Department).val(currentEmployee["DepartmentId"]).change();
                }
                if (currentEmployee["FactoryId"] > 0) {
                    $(PageGridControl.Location).val(currentEmployee["FactoryId"]).change();
                }
                //set disable if user is not admin    
                $(PageGridControl.Requester).prop("disabled", !isdmin);

            }
            $(PageGridControl.Requester).on("change", function () {
                var departmentId = $(this).find(":selected").attr("department-id");
                var locationId = $(this).find(":selected").attr("location-id");
                if (departmentId !== undefined && departmentId > 0) {
                    $(PageGridControl.Department).val(departmentId).change();
                }
                if (locationId !== undefined && locationId > 0) {
                    $(PageGridControl.Location).val(locationId).change();
                }
            });
        },

        RegisterEvent: function () {
            $(PageGridControl.ButtonSave).on("click", function (e) {
                OvertimeRequestPage.SaveData(e);
                e.preventDefault();
            });
        },

        //SaveData : function(requesterId, locationId, fromDate, toDate, locationId, sumEmployee, sumMeal, otherRequirement)
        SaveData: function (e) {
            debugger;
            var overTimeEmployeeData = OverTimeRequestGrid.grid.getData();
            if (overTimeEmployeeData.length > 0) {
                var currentDate = new Date($.now());
                var overtimeManagementItem = {};
                var fromDate = getFromDateControlValue();
                var toDate = getToDateControlValue();

                var requesterId = $(PageGridControl.Requester + " :selected").val();
                var departmentId = $(PageGridControl.Department + " :selected").val();
                var factoryId = $(PageGridControl.Location + " :selected").val();
                var sumMeal = $(PageGridControl.SumOfMeal).val();
                var sumEmployee = $(PageGridControl.SumOfEmployee).val();
                var otherRequirement = $(PageGridControl.OtherRequirement).val();

                if (!OvertimeRequestPage.ValidateDropdownIsValid(PageGridControl.RequesterError, requesterId)) { return };
                if (!OvertimeRequestPage.ValidateDropdownIsValid(PageGridControl.DepartmentError, departmentId)) { return };

                if (OvertimeRequestPage.ValidateDatetime(fromDate.Date) === false) {
                    $(PageGridControl.FromDateError).html("Ngay nhap vao khong dung");
                    $(PageGridControl.FromDateError).show();
                    return;
                }
                else {
                    $(PageGridControl.FromDateError).html("");
                    $(PageGridControl.FromDateError).hide();
                }

                if (OvertimeRequestPage.ValidateDatetime(toDate.Date) === false) {
                    $(PageGridControl.ToDateError).html("Ngay nhap vao khong dung");
                    $(PageGridControl.ToDateError).show();
                    return;
                }
                else {
                    $(PageGridControl.ToDateError).html("");
                    $(PageGridControl.ToDateError).hide();
                }

                if ((new Date(fromDate.Date).getTime() > new Date(toDate.Date).getTime())) {
                    $(PageGridControl.ToDateError).html("Ngay sau khong nho hon ngay truoc");
                    $(PageGridControl.ToDateError).show();
                    return;
                }
                else {
                    $(PageGridControl.ToDateError).html("");
                    $(PageGridControl.ToDateError).hide();
                }

                if (!OvertimeRequestPage.ValidateDropdownIsValid(PageGridControl.LocationError, factoryId)) { return };

                //get date to submit

                if (!OvertimeRequestPage.ValidateNumberIsValid(PageGridControl.SumOfEmployeeError, sumEmployee)) { return };
                if (!OvertimeRequestPage.ValidateNumberIsValid(PageGridControl.SumOfMealError, sumMeal)) { return };
                //overtimeManagementItem.RequesterInfo = null;
                overtimeManagementItem.SelectedRequesterId = requesterId;
                overtimeManagementItem.SelectedDepartmentId = departmentId;
                // overtimeManagementItem.FactoryList = null;
                overtimeManagementItem.SelectedFactoryId = factoryId;
                overtimeManagementItem.FromDate = fromDate.Date;
                overtimeManagementItem.ToDate = toDate.Date;
                overtimeManagementItem.SumOfEmployee = sumEmployee;
                overtimeManagementItem.SumOfMeal = sumMeal;
                overtimeManagementItem.OtherRequirement = otherRequirement;
                //overtimeManagementItem.ResultCodeNumber = 0;

                var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/SaveOvertimeData";
                var savePageDataPromise = $.ajax({
                    type: "POST",
                    url: methodUrl,
                    data: JSON.stringify({ overtimeManagementItem: overtimeManagementItem, overTimeEmployeeData: overTimeEmployeeData }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false
                });
                savePageDataPromise.then(
                    function (data) {
                        if (data != null) {
                            if (data.d.CodeMessageResult.Code === 0) {
                                if (data.d.FailureList.length === 0) {
                                    alert("Du lieu duoc luu thanh cong");
                                }
                                else if (data.d.SuccessList.length === 0) {
                                    alert("Du lieu duoc luu khong thanh cong");
                                }
                                else {
                                    alert("mot vai nguoi duoc luu khong thnah cong");
                                }
                            }
                            else {
                                alert(data.d.CodeMessageResult.Message);
                            }
                        }
                    },
                    function (errorData) {
                        alert("Co loi xay tra khi luu du lieu");
                    });
            }
            else {
                alert("Vui long nhap du lieu cho user");
            }
            e.preventDefault();
        },
        ValidateDatetime: function (input) {
            var status = false;
            if (!input || input.length <= 0) {
                status = false;
            } else {
                var result = new Date(input);
                if (result == 'Invalid Date') {
                    status = false;
                } else {
                    status = true;
                }
            }
            return status;
        },

        ValidateDropdownIsValid: function (dropdownNameError, selectedvalue) {

            var result = false;
            if (selectedvalue === undefined || selectedvalue === "") {
                return false;
            }
            if (selectedvalue < 1) {
                result = false;
                $(dropdownNameError).html("Vui long chon du lieu");
                $(dropdownNameError).show();
            }
            else {
                result = true;
                $(dropdownNameError).html("");
                $(dropdownNameError).hide();
            }
            return result;
        }
        ,
        ValidateNumberIsValid: function (errorControl, value) {
            var result = false;
            if (value === undefined || value === "") {
                return false;
            }

            if (value < 0) {
                $(errorControl).html("Vui nhap so > 0");
                $(errorControl).show();
                result = false;
            }
            else {
                $(errorControl).html("");
                $(errorControl).show();
                result = true;
            }
            return result;
        }
    }




