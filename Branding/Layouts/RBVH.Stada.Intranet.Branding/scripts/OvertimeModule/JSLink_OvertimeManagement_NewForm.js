(function () {
    var overtimeManagementContext = {};
    overtimeManagementContext.Templates = {};
    overtimeManagementContext.OnPostRender = overtimeManagementOnPostRender;
    overtimeManagementContext.Templates.View = CustomNewForm;
    // overtimeManagementContext.BaseViewID = 1;
    overtimeManagementContext.ListTemplateType = 10006;
    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overtimeManagementContext);
    //  SPClientForms.ClientFormManager.SubmitClientForm("formID");
    // setValueForOvertimeForm();
    ExecuteOrDelayUntilScriptLoaded(setValueForOvertimeForm, "sp.js");
})();

var OvertimeManagementControl =
    {
        Requester: "#overtime_Requester",
        Department: "#overtime_Department select",
        Location: "#overtime_Location select",
        FromDate: "#overtime_FromDate :input",
        ToDate: "#overtime_ToDate :input",
        SumEmployee: "#overtime_SumEmployee :input",
        SumEmployeeError: "#overtime_SumEmployee_Error",
        SumMealError: "#overtime_SumMeal_Error",
        SumMeal: "#overtime_SumMeal :input",
        OtherRequirement: "#overtime_OtherRequirement :input"
    }

var Validation =
    {
        validateControl: function (controlValue, controlErrorLabel) {
            if (controlValue.length > 0) {
                $(controlErrorLabel).html("");
                $(controlErrorLabel).hide();
                if ($.isNumeric(controlValue)) {
                    $(controlErrorLabel).html("");
                    $(controlErrorLabel).hide();
                    if (Validation.ValidateTime(controlValue)) {
                        $(controlErrorLabel).html("");
                        $(controlErrorLabel).hide();
                        return true;
                    }
                    else {
                        $(controlErrorLabel).html("0 <=  hour <= 24");
                        $(controlErrorLabel).show();
                        return false;
                    }
                }
                else {
                    $(controlErrorLabel).html("Must is a number");
                    $(controlErrorLabel).show();
                    return false;
                }
            }
            else {
                $(controlErrorLabel).html("Must be not empty!");
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
        }
    }

function overtimeManagement_Submitform() {
    var formData = {};
    var requester = $(OvertimeManagementControl.Requester + " :selected").val();
    var department = $(OvertimeManagementControl.Department + " :selected").val();
    var location = $(OvertimeManagementControl.Location + " :selected").val();
    var fromDate = $(OvertimeManagementControl.FromDate).val();
    var toDate = $(OvertimeManagementControl.ToDate).val();
    var sumEmployee = $(OvertimeManagementControl.SumEmployee).val();
    var sumMeal = $(OvertimeManagementControl.SumMeal).val();
    var ortherRequirement = $(OvertimeManagementControl.OtherRequirement).val();

    if (!PreSaveItem()) return false;

    if (Validation.validateControl(sumEmployee, OvertimeManagementControl.SumEmployeeError) === true &&
        Validation.validateControl(sumMeal, OvertimeManagementControl.SumMealError) === true) {
        formData.RequesterId = requester;
        formData.DepartmentId = department;
        formData.LocationId = location;
        formData.FromDate = fromDate;
        formData.ToDate = toDate;
        formData.SumEmployee = sumEmployee;
        formData.SumMeal = sumMeal;
        formData.OtherRequirement = ortherRequirement;
        return formData;
    }
    else {
        return false;
    }
}

function CustomNewForm(ctx) {
    var formTable = Forms.getNewFomHtml();
    formTable = formTable.replace("{{Overtime_Requester_FieldName}}", Functions.getSPFieldTitle(ctx, "Requester"));
    formTable = formTable.replace("{{Overtime_Department_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonDepartment"));
    formTable = formTable.replace("{{Overtime_Location_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonLocation"));
    formTable = formTable.replace("{{Overtime_FromDate_FieldName}}", Functions.getSPFieldTitle(ctx, "CommonFrom"));
    formTable = formTable.replace("{{Overtime_ToDate_FieldName}}", Functions.getSPFieldTitle(ctx, "To"));
    formTable = formTable.replace("{{Overtime_SumEmployee_FieldName}}", Functions.getSPFieldTitle(ctx, "SumOfEmployee"));
    formTable = formTable.replace("{{Overtime_SumMeal_FieldName}}", Functions.getSPFieldTitle(ctx, "SumOfMeal"));
    formTable = formTable.replace("{{Overtime_OtherRequirement_FieldName}}", Functions.getSPFieldTitle(ctx, "OtherRequirements"));

    formTable = formTable.replace("{{Overtime_Requester}}", overtimeRenderRequesterField(ctx));
    formTable = formTable.replace("{{Overtime_Department}}", Functions.getSPFieldRender(ctx, "CommonDepartment"));
    formTable = formTable.replace("{{Overtime_Location}}", Functions.getSPFieldRender(ctx, "CommonLocation"));
    formTable = formTable.replace("{{Overtime_FromDate}}", Functions.getSPFieldRender(ctx, "CommonFrom"));
    formTable = formTable.replace("{{Overtime_ToDate}}", Functions.getSPFieldRender(ctx, "To"));
    formTable = formTable.replace("{{Overtime_SumEmployee}}", Functions.getSPFieldRender(ctx, "SumOfEmployee"));
    formTable = formTable.replace("{{Overtime_SumMeal}}", Functions.getSPFieldRender(ctx, "SumOfMeal"));
    formTable = formTable.replace("{{Overtime_OtherRequirement}}", Functions.getSPFieldRender(ctx, "OtherRequirements"));
    return formTable;
}

//set value for SumMeal & sumEmployee
function setValueForOvertimeForm() {
    var clientContext = SP.ClientContext.get_current();
    var oList = clientContext.get_web().get_lists().getByTitle('Overtime Employee Details');
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml(
        '<View><Query><Where><IsNull><FieldRef Name=\'OvertimeManagementID\' /></IsNull></Where></Query></View>'
    );
    this.collListItem = oList.getItems(camlQuery);
    clientContext.load(collListItem);
    clientContext.executeQueryAsync(
        Function.createDelegate(this, function (data) {
            var items = [];
            var listItemEnumerator = collListItem.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var oListItem = listItemEnumerator.get_current();
                items.push(oListItem.get_id());
            }
            $(OvertimeManagementControl.SumEmployee).val(items.length);
            $(OvertimeManagementControl.SumMeal).val(items.length);
        }),
        Function.createDelegate(this, function (data) {
        })
    );
}

function overtimeRenderRequesterField(ctx) {
    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetCurrentUser";
    var getCurrentUserDepmtPromise = $.ajax({
        type: "POST",
        url: methodUrl,
        data: null,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false
    });
    getCurrentUserDepmtPromise.then(
        function (result) {

            if (result.d != null) {
                var isdmin = Boolean(result.d["IsAdmin"]);
                var currentEmployee = result.d["CurrentEmployee"];
                var employeeList = result.d["ListEmployees"];
                var department = currentEmployee["Department"].LookupValue;
                var employeeId = currentEmployee.EmployeeID;

                for (var i = 0; i < employeeList.length; i++) {
                    var dept = employeeList[i].Department;
                    var empId = employeeList[i].EmployeeID;
                    var option = '<option department-data="' + department + '" employee-data="' + empId + '" value="' + employeeList[i].ID + '">' + employeeList[i].FullName + '</option>';
                    $(OvertimeManagementControl.Requester).append(option);
                }
                if (currentEmployee["LookupId"] > 0) {
                    $(OvertimeManagementControl.Requester).val(currentEmployee["LookupId"]).change();
                }

                $(OvertimeManagementControl.Requester).prop("disabled", !isdmin);


                if (employeeId != '') {
                    $('#stada-employee-id').html("ID: " + employeeId);
                }
                if (department != '' && department !== undefined) {
                    $('#stada-employee-department').html("Department: " + department);
                }
            }
            $(OvertimeManagementControl.Requester).on('change', function () {
                var dept = $(this).find(':selected').attr('department-data');
                var empID = $(this).find(':selected').attr('employee-data');

                if (empID != '') {
                    //  $('#stada-employee-id').html("ID: " + empID);
                }
                if (dept != '' && dept !== undefined) {
                    //  $('#stada-employee-department').html("Department: " + dept);
                }
            });
        },
         function () {
             alert("Faild to get Data");
         }
    );
    return "<select id='overtime_Requester'></select>";
}


function overtimeManagementOnPostRender(ctx) {

}


var Forms =
    {
        getNewFomHtml: function () {
            var html =
                "<table class='ms-formtable' style='margin-top: 8px;' border='0' cellpadding='0' cellspacing='0' width='100%'>" +
                "<tbody>" +
                "<tr>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
                "<h3 class='ms-standardheader'>" +
                "<nobr>{{Overtime_Requester_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
                "</h3>" +
                " </td>" +
                "<td valign='top' width='350px' class='ms-formbody'>" +
                "<div title='Requester'>{{Overtime_Requester}}</div>" +
                "</td>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'><h3 class='ms-standardheader'>" +
                "<nobr>{{Overtime_Department_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
                "</h3>" +
                "</td>" +
                "<td valign='top' width='350px' class='ms-formbody'>" +
                "<div id='overtime_Department'>{{Overtime_Department}}</div>" +
                "</td>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
                "<h3 class='ms-standardheader'>" +
                "<nobr>{{Overtime_Location_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
                "</h3>" +
                "</td>" +
                "<td valign='top' width='350px' class='ms-formbody'>" +
                "<div id='overtime_Location'>{{Overtime_Location}}</div>" +
                "</td>" +
                "</tr>" +
                "<tr>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
                " <h3 class='ms-standardheader'>" +
                " <nobr>{{Overtime_FromDate_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
                " </h3>" +
                "  </td>" +
                "<td valign='top' width='350px' class='ms-formbody'>" +
                " <div id='overtime_FromDate'>{{Overtime_FromDate}}</div>" +
                " </td>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
                "<h3 class='ms-standardheader'>" +
                " <nobr>{{Overtime_ToDate_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
                " </h3>" +
                "</td>" +
                " <td valign='top' width='350px' class='ms-formbody'>" +
                "  <div id='overtime_ToDate'>{{Overtime_ToDate}}</div>" +
                "</td>" +
                "</tr>" +
                "<tr>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
                "  <h3 class='ms-standardheader'>" +
                "   <nobr>{{Overtime_SumEmployee_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
                " </h3>" +
                "</td>" +
                "<td valign='top' width='350px' class='ms-formbody'>" +
                " <div id='overtime_SumEmployee'>  {{Overtime_SumEmployee}}<span role='alert' id='overtime_SumEmployee_Error'><br></span></div>" +
                "</td>" +
                "<td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
                "<h3 class='ms-standardheader'>" +
                " <nobr>{{Overtime_SumMeal_FieldName}}<span class='ms-accentText' title='This is a required field.'> *</span></nobr>" +
                " </h3>" +
                " </td>" +
                "<td valign='top' width='350px' class='ms-formbody'>" +
                "<div id='overtime_SumMeal'> {{Overtime_SumMeal}}<span role='alert' id='overtime_SumMeal_Error'><br></span></div>" +
                " </td>" +
                " </tr>" +
                "<tr>" +
                " <td nowrap='true' valign='top' width='113px' class='ms-formlabel'>" +
                " <h3 class='ms-standardheader'>" +
                "  <nobr>{{Overtime_OtherRequirement_FieldName}}</nobr>" +
                " </h3>" +
                "</td>" +
                " <td valign='top' width='350px' class='ms-formbody'>" +
                "  <div id='overtime_OtherRequirement'>{{Overtime_OtherRequirement}}</div>" +
                " </td>" +
                "</tr>" +
                "</tbody>" +
                "</table>";
            return html;
        }
    }

//  <JSLink xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">~siteCollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/OvertimeModule/JSLink_OvertimeManagement_NewForm.js</JSLink>

