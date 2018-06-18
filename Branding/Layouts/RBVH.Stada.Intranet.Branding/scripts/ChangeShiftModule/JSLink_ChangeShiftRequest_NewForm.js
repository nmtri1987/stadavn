var ChangeShiftRequestNewForm = {
    SaveButton: "Save",
    CancelButton: "Cancel",
    EmployeeIDFieldName: "ID",
    DepartmentFieldName: "Department",
    FromDateInvalid: "User doesn't have any shift in selected day. Please pick another day!",
    PageResourceFileName: "RBVHStadaWebpages",
    ListResourceFileName: "RBVHStadaLists"
};

(function () {
    // Register resource
    SP.SOD.registerSod(ChangeShiftRequestNewForm.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ChangeShiftRequestNewForm.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(ChangeShiftRequestNewForm.PageResourceFileName, "Res", OnPageResourcesReady);
    SP.SOD.registerSod(ChangeShiftRequestNewForm.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ChangeShiftRequestNewForm.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(ChangeShiftRequestNewForm.ListResourceFileName, "Res", OnListResourcesReady);


    var changeShiftRequestNewFormContext = {};
    changeShiftRequestNewFormContext.Templates = {};
    changeShiftRequestNewFormContext.OnPostRender = changeShiftRequestOnPostRender;
    changeShiftRequestNewFormContext.Templates.View = CustomChangeShiftRequestView;

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(changeShiftRequestNewFormContext);

})();


function OnPageResourcesReady() {
    ChangeShiftRequestNewForm.SaveButton = Res.saveButton;
    ChangeShiftRequestNewForm.CancelButton = Res.cancelButton;
}


function OnListResourcesReady() {
    ChangeShiftRequestNewForm.EmployeeIDFieldName = Res.changeShiftList_EmployeeID;
    ChangeShiftRequestNewForm.DepartmentFieldName = Res.changeShiftList_Department;
    ChangeShiftRequestNewForm.FromDateInvalid = Res.changeShiftList_CommonFromInvalidError;
}

var changeShift_StartDayNumer;
var changeShift_EndDayNumer;

function CustomChangeShiftRequestView(ctx) {
    currentChangeShiftItem = { typeForm: ctx.BaseViewID, id: ctx.FormContext.itemAttributes.Id };
    var formTable = "".concat("<table class='ms-formtable' style='margin-top: 8px;' border='0' cellpadding='0' cellspacing='0' width='100%'>",
   "<tbody>",
       "<tr>",
           "<td height='30' width='25%' class='ms-formlabel'><h3 class='ms-standardheader'><nobr>{{ChangeShiftRequest_Requester_FieldName}}</nobr></h3></td>",
           "<td class='ms-formbody'><div id='ChangeShiftRequest_Requester'>{{ChangeShiftRequest_Requester_Ctrl}} </div> <div hidden='hidden' id='ChangeShiftRequest_Requester_HiddenCtrl'>{{ChangeShiftRequest_Requester_HiddenCtrl}}</div></td>",
			"<td colspan='2'></td>",
        "</tr>",
		"<tr>",
		 "<td><div style='display:none' id='changeShift_Requester_ID_Title'>" + ChangeShiftRequestNewForm.EmployeeIDFieldName + ": </div></td>",
            "<td><div style='display:none' id='changeShift_Requester_ID_Value'></div></td>",
            "<td><div style='display:none' id='changeShift_Requester_Department_Title'>" + ChangeShiftRequestNewForm.DepartmentFieldName + " </div></td>",
			"<td><div style='display:none' id='changeShift_Requester_Department_Value'></div></td>",
        "<tr>",
             "<td height='30' class='ms-formbody'><div class='ms-formlabel'>{{ChangeShiftRequest_FromDate_FieldName}}</div></td>",
            "<td><div id='ChangeShiftRequest_FromDate'>{{ChangeShiftRequest_FromDate_Ctrl}}</div>",
			"<span id='Error_CommonFrom_Invalid' style='display:none' class='ms-formvalidation ms-csrformvalidation'><span role='alert'>" + ChangeShiftRequestNewForm.FromDateInvalid + "<br></span></span>",
			"</td>",
            "<td>{{ChangeShiftRequest_FromShift_FieldName}}</td>",
            "<td><div id='ChangeShiftRequest_FromShift'>{{ChangeShiftRequest_FromShift_Ctrl}}</div></td>",
        "</tr>",
         "<tr>",
            "<td height='30' class='ms-formbody'><div class='ms-formlabel'>{{ChangeShiftRequest_ToDate_FieldName}}</div></td>",
            "<td><div id='ChangeShiftRequest_ToDate'>{{ChangeShiftRequest_ToDate_Ctrl}}</div></td>",
            "<td>{{ChangeShiftRequest_ToShift_FieldName}}</td>",
            "<td><div id='ChangeShiftRequest_ToShift'>{{ChangeShiftRequest_ToShift_Ctrl}}</div></td>",
        "</tr>",
         "<tr>",
            "<td height='30' class='ms-formbody'><div class='ms-formlabel'>{{ChangeShiftRequest_Reason_FieldName}}</div></td>",
            "<td colspan='3'><div id='ChangeShiftRequest_Reason'>{{ChangeShiftRequest_Reason_Ctrl}}</div></td>",
        "</tr>",
         "<tr>",
            "<td height='30' class='ms-formbody'>&nbsp;</td>",
            "<td colspan='3'>&nbsp;</td>",
        "</tr>",
        "<tr>&nbsp;</tr>",
          "<tr>",
          "<td colspan='4'><div style='float: right;'><input type='button' value='{{ChangeShiftRequest_SaveButton}}' onclick=\"Submit('{{FormId}}')\" style='margin-left:0' > <input type='button' value='{{ChangeShiftRequest_CancelButton}}' onclick=\"Cancel()\"> </div></td>",
          "</tr>",
    "</tbody>",
    "</table>");
    //class='ms-formlabel'
    //class='ms-formbody'

    // Display
    formTable = formTable.replace("{{ChangeShiftRequest_Requester_FieldName}}", getSPFieldTitle(ctx, "Requester"));
    formTable = formTable.replace("{{ChangeShiftRequest_FromDate_FieldName}}", getSPFieldTitle(ctx, "CommonFrom"));
    formTable = formTable.replace("{{ChangeShiftRequest_FromShift_FieldName}}", getSPFieldTitle(ctx, "FromShift"));
    formTable = formTable.replace("{{ChangeShiftRequest_ToDate_FieldName}}", getSPFieldTitle(ctx, "To"));
    formTable = formTable.replace("{{ChangeShiftRequest_ToShift_FieldName}}", getSPFieldTitle(ctx, "ToShift"));
    formTable = formTable.replace("{{ChangeShiftRequest_Reason_FieldName}}", getSPFieldTitle(ctx, "Reason"));

    // Control
    formTable = formTable.replace("{{ChangeShiftRequest_Requester_Ctrl}}", setCurrrentEmployeeLogedIn(ctx, "Requester"));
    formTable = formTable.replace("{{ChangeShiftRequest_Requester_HiddenCtrl}}", getSPFieldRender(ctx, "Requester"));
    formTable = formTable.replace("{{ChangeShiftRequest_FromDate_Ctrl}}", getSPFieldRender(ctx, "CommonFrom"));
    formTable = formTable.replace("{{ChangeShiftRequest_FromShift_Ctrl}}", getSPFieldRender(ctx, "FromShift"));
    formTable = formTable.replace("{{ChangeShiftRequest_ToDate_Ctrl}}", getSPFieldRender(ctx, "To"));
    formTable = formTable.replace("{{ChangeShiftRequest_ToShift_Ctrl}}", getSPFieldRender(ctx, "ToShift"));
    formTable = formTable.replace("{{ChangeShiftRequest_Reason_Ctrl}}", getSPFieldRender(ctx, "Reason"));
    formTable = formTable.replace("{{FormId}}", ctx.FormUniqueId);
    formTable = formTable.replace("{{ChangeShiftRequest_SaveButton}}", ChangeShiftRequestNewForm.SaveButton);
    formTable = formTable.replace("{{ChangeShiftRequest_CancelButton}}", ChangeShiftRequestNewForm.CancelButton);
    return formTable;
}


function setCurrrentEmployeeLogedIn(ctx, fieldName) {

    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetCurrentUser";
    var changeRequestFieldId = "ChangeShift_Requester";
    var GetCurrentUserAjax =
        $.ajax({
            type: "POST",
            url: methodUrl,
            data: null,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (result) {
            }
        });

    GetCurrentUserAjax.then(function (result, ctx) {
        var ChangeShiftRequest_Requester_Dropdown = $('#' + changeRequestFieldId);
        var changeShift_Requester_ID_Label_Title = $('#changeShift_Requester_ID_Title');
        var changeShift_Requester_ID_Label_Value = $('#changeShift_Requester_ID_Value');
        var changeShift_Requester_Department_Label_Title = $('#changeShift_Requester_Department_Title');
        var changeShift_Requester_Department_Label_Value = $('#changeShift_Requester_Department_Value');
        var changeShift_Requester_Hidden_Control = $('#ChangeShiftRequest_Requester_HiddenCtrl select');


        if (result.d != null) {
            var isAdmin = Boolean(result.d["IsAdmin"]);
            var currentEmployee = result.d["CurrentEmployee"];
            var employeeList = result.d["ListEmployees"];
            var department = currentEmployee["Department"].LookupValue;
            var employeeId = currentEmployee.EmployeeID;

            for (var i = 0; i < employeeList.length; i++) {
                var empId = employeeList[i].EmployeeID;
                //set selected for current user 
                var option = '<option department-data="' + department + '" employee-data="' + empId + '" value="' + employeeList[i].ID + '">' + employeeList[i].FullName + '</option>';
                ChangeShiftRequest_Requester_Dropdown.append(option);
            }

            changeShift_Requester_Hidden_Control.val(currentEmployee.ID).change();
            ChangeShiftRequest_Requester_Dropdown.val(currentEmployee.ID).change();
            ChangeShiftRequest_Requester_Dropdown.prop("disabled", !isAdmin);

            if (employeeId != '') {
                changeShift_Requester_ID_Label_Title.show();
                changeShift_Requester_ID_Label_Value.html(employeeId);
                changeShift_Requester_ID_Label_Value.show();
            }
            if (department != '' && department !== undefined) {
                changeShift_Requester_Department_Label_Title.show();
                changeShift_Requester_Department_Label_Value.html(department);
                changeShift_Requester_Department_Label_Value.show();
            }
        }

        ChangeShiftRequest_Requester_Dropdown.on('change', function () {
            var dept = $(this).find(':selected').attr('department-data');
            var empID = $(this).find(':selected').attr('employee-data');
            var selectedValue = $(this).find(':selected').attr('value');
            //set selected value for hidden field
            if (selectedValue > 0) {

                changeShift_Requester_Hidden_Control.val(selectedValue).change();
            }

            if (empID != '') {
                changeShift_Requester_ID_Label_Title.show();
                changeShift_Requester_ID_Label_Value.html(empID);
                changeShift_Requester_ID_Label_Value.show();
            }
            if (dept != '' && dept !== undefined) {
                changeShift_Requester_Department_Label_Title.show();
                changeShift_Requester_Department_Label_Value.html(dept);
                changeShift_Requester_Department_Label_Value.show();
            }
        });
    });
    //this return before
    return '<span dir="none"><select id="' + changeRequestFieldId + '" title="Requester"></select><br/></span>';
}


function getSPFieldRender(ctx, fieldName) {
    var fieldContext = ctx;
    //Get the filed Schema 
    var result = ctx.ListSchema.Field.filter(function (obj) {
        return obj.Name == fieldName;
    });
    //Set the field Schema  & default value 
    fieldContext.CurrentFieldSchema = result[0];
    fieldContext.CurrentFieldValue = ctx.ListData.Items[0][fieldName];
    //Call  OOTB field render function  
    return ctx.Templates.Fields[fieldName](fieldContext);
}

function getSPFieldTitle(ctx, fieldName) {
    //Get the filed Schema 
    var result = ctx.ListSchema.Field.filter(function (obj) {
        return obj.Name == fieldName;
    });
    //Set the field Schema  & default value 
    return result[0].Title;
}

function Submit(formId) {
    SPClientForms.ClientFormManager.SubmitClientForm(formId);
}

function Cancel() {
    window.location.href = _spPageContextInfo.listUrl;
}

function getDatetime(idText) {

    var date = $("#" + idText + " :input").val()
    var hours = $("#" + idText + " option:selected").val()
    var minute = $("#" + idText + " option:selected")[1].text;
    return new Date(date + " " + hours + ":" + minute)
}
function DisableDateControl(name, isDisable) {
    var inputControl = $("#" + name + ' input');
    if (inputControl) {
        inputControl.prop("disabled", isDisable);
    }
    var pickerImage = $("#" + name + ' img');
    if (pickerImage) {
        pickerImage.prop("hidden", isDisable);
    }
}

function changeShiftRequestOnPostRender(ctx) {

    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetEmployeeShiftTime";

    var fromShiftControl = $('#ChangeShiftRequest_FromShift select');
    var toShiftControl = $('#ChangeShiftRequest_ToShift select');
    var toDateControl = $('#ChangeShiftRequest_ToDate');

    //set disable as default 
    fromShiftControl.prop("disabled", true);
    toShiftControl.prop("disabled", true);
    fromShiftControl.prepend(new Option("-- No Shift --", 0, true, true));
    toShiftControl.prepend(new Option("-- No Shift --", 0, true, true));

    //for IE browser
    fromShiftControl.val(0).change();
    toShiftControl.val(0).change();

    DisableDateControl('ChangeShiftRequest_ToDate', true);

    $('#ChangeShiftRequest_FromDate :input').blur(function () {
        var date = $('#ChangeShiftRequest_FromDate :input').val();
        var shiftDate = new Date(date);
        var day = shiftDate.getDate();
        var month = shiftDate.getMonth() + 1;
        var year = shiftDate.getYear() + 1900;

        var currentEmployeeId = $('#ChangeShiftRequest_Requester').find(':selected').attr('employee-data');

        var GetfromShiftFromDatePromise = $.ajax({
            type: "POST",
            url: methodUrl,
            data: '{"employeeId": ' + currentEmployeeId + ',"shiftDay":' + day + ' , "shiftMonth": ' + month + ' , "shiftYear": ' + year + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (result) {
            }
,
            error: function (jqXHR, textStatus, errorThrown) {
                alert("An error occurred while getting data from server. Please try again");
                if (jqXHR.status == 500) {
                    console.log('Internal error: ' + jqXHR.responseText);
                } else {
                    console.log('Unexpected error.');
                }
            }
        });

        GetfromShiftFromDatePromise.then(function (result) {
            if (result != null) {
                var serverData = result.d;
                changeShift_StartDayNumer = serverData.StartDay;
                changeShift_EndDayNumer = serverData.EndDay;
                if (serverData.IsHasData) {
                    //fromShiftErrorLabel.hide()
                    fromShiftControl.val(serverData.employeeShiftTime.ShiftLookupId).change();
                    DisableDateControl('ChangeShiftRequest_ToDate', false);
                    fromShiftControl.prop("disabled", true).css('color', 'black');
                    $("#Error_CommonFrom_Invalid").hide();
                }
                else {
                    //Don't have time 
                    // fromShiftControl.prepend(new Option("-- No Shift --", 0, true, true));
                    // toShiftControl.prepend(new Option("-- No Shift --", 0, true, true));

                    //for IE browser
                    fromShiftControl.val(0).change();
                    toShiftControl.val(0).change();
                    fromShiftControl.prop("disabled", true);
                    toShiftControl.prop("disabled", true);

                    $("#Error_CommonFrom_Invalid").show();
                    //alert("User: '" + $('#ChangeShiftRequest_Requester').find(':selected').text() + "' doesn't have any shift in selected day. Please pick another day!");
                }
            }
        });
    });

    $('#ChangeShiftRequest_ToDate :input').blur(function () {
        var date = $('#ChangeShiftRequest_ToDate :input').val();
        var shiftDate = new Date(date);
        var day = shiftDate.getDate();
        var month = shiftDate.getMonth() + 1;
        var year = shiftDate.getYear() + 1900;
        var currentEmployeeId = $('#ChangeShiftRequest_Requester').find(':selected').attr('employee-data');

        var GetfromShiftToDatePromise = $.ajax({
            type: "POST",
            url: methodUrl,
            data: '{"employeeId": ' + currentEmployeeId + ', "shiftDay":' + day + ' , "shiftMonth": ' + month + ' , "shiftYear": ' + year + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (result) {
            }
           ,
            error: function (jqXHR, textStatus, errorThrown) {
                alert("An error occurred while getting data from server. Please try again");
                if (jqXHR.status == 500) {
                    console.log('Internal error: ' + jqXHR.responseText);
                } else {
                    console.log('Unexpected error.');
                }
            }
        });

        GetfromShiftToDatePromise.then(function (result) {
            if (result != null) {
                var serverData = result.d;
                changeShift_StartDayNumer = serverData.StartDay;
                changeShift_EndDayNumer = serverData.EndDay;
                if (serverData.IsHasData === false) {
                    // $("#ChangeShiftRequest_ToShift option[value='0']").remove();
                    toShiftControl.prop("disabled", false);
                }
                else {
                    toShiftControl.val(serverData.employeeShiftTime.ShiftLookupId).change();
                    toShiftControl.find(":selected").text();
                    alert("User: " + $('#ChangeShiftRequest_Requester').find(':selected').text() + " already had shift " + toShiftControl.find(":selected").text() + " for day: " + $('#ChangeShiftRequest_ToDate :input').val());
                    toShiftControl.prop("disabled", false);
                }
            }
        });

    });
}

