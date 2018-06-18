
(function () {
    var leaveManagementFiledContext = {};
    var pageResourceFileName = "RBVHStadaWebpages";

    leaveManagementFiledContext.Templates = {};
    leaveManagementFiledContext.OnPostRender = leaveManagementOnPostRender;

    leaveManagementFiledContext.Templates.View = leaveRequestNewForm;
    leaveManagementFiledContext.ListTemplateType = 10004;

    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveManagementFiledContext);
    SP.SOD.registerSod(pageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + pageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(pageResourceFileName, "Res", OnPageResourcesCommonReady);

})();

var LeavePageResource =
    {
        Department: "Department",
        ID: "ID",
        LeaveManagement_ErrorOccurre: "An error occurred while loading data from server. Please try again!",
        Dropdown_Select: "Select",
        LeaveManagement_FromDateAlert: "Leave request must be raised before 15 days",
        SaveButton: "Save",
        CancelButton: "Cancel",
        RequireField: "This is a required field."
    }

function OnPageResourcesCommonReady() {
    LeavePageResource.Department = Res.commonDepartment;
    LeavePageResource.ID = Res.commonID;
    LeavePageResource.LeaveManagement_ErrorOccurre = Res.leaveManagement_ErrorOccurre;
    LeavePageResource.Dropdown_Select = Res.dropdown_Select;
    LeavePageResource.LeaveManagement_FromDateAlert = Res.leaveManagement_FromDateAlert;
    LeavePageResource.SaveButton = Res.saveButton;
    LeavePageResource.CancelButton = Res.cancelButton;
    LeavePageResource.RequireField = Res.requiredField;
}

var LeaveRequestPageControl =
    {
        RequesterCustom: "#RequesterCustom select",
        EmployeeIDLabel: "#stada-employee-id",
        DepartmentLabel: "#stada-employee-department",
        TrnfrWorkEmployeeIDLabel: "#trsf-stada-employee-id",
        TranferworkToCustom: "#TranferworkToCustom select",
        TrnfrWorkDepartmentLabel: "#trsf-stada-employee-department",
        FromDateCustom: "#FromDateControl",
        FromDate: "#FromDateCustom :input",
        FromHour: "#FromDateCustom select:first",
        FromMinute: "#FromDateCustom select:last",
        ToDate: "#ToDateCustom :input",
        ToHour: "#ToDateCustom select:first",
        ToMinute: "#ToDateCustom select:last",
        ToDateValidate: "#ToDateValidateCustom",
        LeaveHourCustom: "#LeaveHourCustom :input",
        ApproverOne: "#ApproverOneCustom select:first",
        ApproverTwo: "#ApproverTwoCustom select:first",
        ApproverThree: "#ApproverThreeCustom select",
        AddApproverOnebtn: "#addApproverOnebtn",
        AddApproverTwobtn: "#addApproverTwobtn",
        AlertFromDate: "#AlertFromDate",
        ApproverOneInput: "#ApproverOneCustom :input",
        ApproverTwoInput: "#ApproverTwoCustom :input",
        ApproverThreeInput: "#ApproverThreeCustom :input",
        ApproverTwoErrorLabel: "#ApproverTwoErrorLabel",
        ApproverThreeErrorLabel: "#ApproverThreeErrorLabel",
    }

var CommonVars =
    {
        WorkingHourFromHourAM: 7,
        WorkingHourFromMinuteAM: 15,
        WorkingHourToHourAM: 11,
        WorkingHourToMinuteAM: 15,
        WorkingHourFromHourPM: 13,
        WorkingHourFromMinutePM: 15,
        WorkingHourToHourPM: 16,
        WorkingHourToMinutePM: 30
    }

function leaveManagementOnPostRender() {
    //    $(LeaveRequestPageControl.LeaveHourCustom).val(0);
    //  $(LeaveRequestPageControl.LeaveHourCustom).attr("disabled", true);
    $(LeaveRequestPageControl.ApproverOneOptionalRow).hide();
    $(LeaveRequestPageControl.ApproverTwoOptionalRow).hide();
    load_Requester();
    load_TransferWorkTo();
    load_ToDate();
    load_FromDate();
    //load_Approvers();
    //load_OptionalApprover();
}

function validateApprover() {
    var isValid = false;
    var approverOne = $(LeaveRequestPageControl.ApproverOneInput).val();
    var approverTwo = $(LeaveRequestPageControl.ApproverTwoInput).val();
    var approverThree = $(LeaveRequestPageControl.ApproverThreeInput).val();

    if (approverOne !== "") {
        if (approverOne === approverTwo) {
            isValid = false;
            $(LeaveRequestPageControl.ApproverTwoErrorLabel).text("Approver 2 must be different with approver 1");
        }
        else {
            isValid = true;
            $(LeaveRequestPageControl.ApproverTwoErrorLabel).text("");
        }
    }

    if (approverTwo !== "" && approverThree !== "" && approverTwo !== approverThree) {
        $(LeaveRequestPageControl.ApproverThreeErrorLabel).text("");
        isValid = true;
        if (approverOne !== "" && approverThree !== "" && approverOne !== approverThree) {
            $(LeaveRequestPageControl.ApproverThreeErrorLabel).text("");
            isValid = true;
        }
        else {
            $(LeaveRequestPageControl.ApproverThreeErrorLabel).text("Approver 3 must be different with approver 1");
            isValid = false;
        }
    }
    else {
        $(LeaveRequestPageControl.ApproverThreeErrorLabel).text("Approver 3 must be different with approver 2");
        isValid = false;
    }


    return isValid;
}

//Requester
function load_Requester() {

    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetCurrentUser";
    $.ajax({
        type: "POST",
        url: methodUrl,
        data: null,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.d !== null) {
                var isdmin = Boolean(result.d["IsAdmin"]);
                var currentEmployee = result.d["CurrentEmployee"];
                var employeeList = result.d["ListEmployees"];
                var department = currentEmployee["Department"].LookupValue;
                var employeeId = currentEmployee.EmployeeID;

                $(LeaveRequestPageControl.RequesterCustom).empty();
                //append new option
                for (var i = 0; i < employeeList.length; i++) {
                    var empId = employeeList[i].EmployeeID;
                    var option = '<option department-data="' + department + '" employee-data="' + empId + '" value="' + employeeList[i].ID + '">' + employeeList[i].FullName + '</option>';
                    $(LeaveRequestPageControl.RequesterCustom).append(option);
                }

                if (currentEmployee.ID > 0) {
                    $(LeaveRequestPageControl.RequesterCustom).val(currentEmployee.ID).change();
                }
                $(LeaveRequestPageControl.RequesterCustom).prop("disabled", !isdmin);

                if (employeeId !== "") {
                    $(LeaveRequestPageControl.EmployeeIDLabel).html(LeavePageResource.ID + ": " + employeeId);
                }
                if (department !== "" && department !== undefined) {
                    $(LeaveRequestPageControl.DepartmentLabel).html(LeavePageResource.Department + ": " + department);
                }
            }
            $(LeaveRequestPageControl.RequesterCustom).on('change', function () {
                var dept = $(this).find(':selected').attr('department-data');
                var empId = $(this).find(':selected').attr('employee-data');

                if (empId !== '') {
                    $(LeaveRequestPageControl.EmployeeIDLabel).html(LeavePageResource.ID + ": " + empId);
                }
                if (dept !== '' && dept !== undefined) {
                    $(LeaveRequestPageControl.DepartmentLabel).html(LeavePageResource.Department + ": " + dept);
                }
            });
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

//Transfer Work To
function load_TransferWorkTo() {
    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetCommonAccountList";
    $.ajax({
        type: "POST",
        url: methodUrl,
        data: null,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (result) {
            var data = result.d;
            var firstDepartment, firstEmployeeId;
            if (data !== null && data.length > 0) {
                firstDepartment = data[0]["Department"].LookupValue;
                firstEmployeeId = data[0].EmployeeID;
                $(LeaveRequestPageControl.TranferworkToCustom).empty();
                for (var i = 0; i < data.length; i++) {
                    var option = '<option department-data="' +
                        data[i]["Department"].LookupValue +
                        '" employee-data="' +
                        data[i].EmployeeID +
                        '" value="' +
                        data[i].ID +
                        '">' +
                        data[i].FullName +
                        '</option>';
                    $(LeaveRequestPageControl.TranferworkToCustom).append(option);
                }
            }
            //Set value for label when page load
            if (firstEmployeeId !== '') {
                $(LeaveRequestPageControl.TrnfrWorkEmployeeIDLabel).html(LeavePageResource.ID + ": " + firstEmployeeId);
            }
            if (firstDepartment !== '' && firstDepartment !== undefined) {
                $(LeaveRequestPageControl.TrnfrWorkDepartmentLabel)
                    .html(LeavePageResource.Department + ": " + firstDepartment);
            }

            $(LeaveRequestPageControl.TranferworkToCustom).on('change',
                function () {
                    var dept = $(this).find(':selected').attr('department-data');
                    var empId = $(this).find(':selected').attr('employee-data');

                    if (empId !== '') {
                        $(LeaveRequestPageControl.TrnfrWorkEmployeeIDLabel).html(LeavePageResource.ID + ": " + empId);
                    }
                    if (dept !== '' && dept !== undefined) {
                        $(LeaveRequestPageControl.TrnfrWorkDepartmentLabel)
                            .html(LeavePageResource.Department + ": " + dept);
                    }
                });
        },
        error: function (jqXhr) {
            if (jqXhr.status === 500) {
                console.log('Internal error: ' + jqXhr.responseText);
            } else {
                console.log('Unexpected error.');
            }
        }
    });
}

function load_FromDate() {
    $(LeaveRequestPageControl.FromDate).on("blur", function () {
        var fromDate = $(LeaveRequestPageControl.FromDate).val();
        var fromDateObject = new Date(fromDate);
        var fromDateCompare = new Date();
        fromDateCompare.setDate(fromDateObject.getDate() - 15);
        var now = new Date();
        if (fromDateCompare < now) {
            $(LeaveRequestPageControl.AlertFromDate).show();
        }
        else {
            $(LeaveRequestPageControl.AlertFromDate).hide();
        }
    });

}

function load_ToDate() {
    $(LeaveRequestPageControl.ToDate).on("blur", function () {

        var fromDate = $(LeaveRequestPageControl.FromDate).val();
        // var fromDateObject = new Date(fromDate);
        // var fromDay = fromDateObject.getDate();
        // var fromMonth = fromDateObject.getMonth() + 1;
        // var fromYear = fromDateObject.getYear() + 1900;

        var fromHour = $(LeaveRequestPageControl.FromHour + " :selected").val();
        var fromMinute = $(LeaveRequestPageControl.FromMinute + " :selected").val();

        var toDate = $(LeaveRequestPageControl.ToDate).val();
        //  var toDateObject = new Date(toDate);
        // var toDay = toDateObject.getDate();
        // var toMonth = toDateObject.getMonth() + 1;
        // var toYear = toDateObject.getYear() + 1900;

        var toHour = $(LeaveRequestPageControl.ToHour + " :selected").val();
        var toMinute = $(LeaveRequestPageControl.ToMinute + " :selected").val();

        //Compare 2 Date
        var fromDateString = fromDate + " " + fromHour + ":" + fromMinute + ":00".replace(/(\d{2})\/(\d{2})\/(\d{4})/, "$2/$1/$3");
        var toDateString = toDate + " " + toHour + ":" + toMinute + ":00".replace(/(\d{2})\/(\d{2})\/(\d{4})/, "$2/$1/$3");

        // now you can compare them using:
        var fromDateFull = new Date(fromDateString);
        var toDateFull = new Date(toDateString);
        if (fromDateFull >= toDateFull) {
            //$(LeaveRequestPageControl.ToDateValidate).html("Ngay sau phai lon hon ngay truoc");
            $(LeaveRequestPageControl.LeaveHourCustom).val(0);
            return;
        }
        // else {
        //     $(LeaveRequestPageControl.ToDateValidate).html("");

        // }
        // calculateWorkingHour(fromDateFull, toDateFull);
    });
}


function load_Approvers() {
    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/LoadLeaveApprovers";
    $.ajax({
        type: "POST",
        url: methodUrl,
        data: null,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result !== null && result !== undefined) {
                if (result.d.CodeMessageResult.Code === 0) {
                    populateDataToDropdown(LeaveRequestPageControl.ApproverOne, result.d.ApproverOneList, false);
                    populateDataToDropdown(LeaveRequestPageControl.ApproverTwo, result.d.ApproverTwoList, true);
                    populateDataToDropdown(LeaveRequestPageControl.ApproverThree, result.d.ApproverThreeList, true);
                    populateDataToDropdown(LeaveRequestPageControl.ApproverOneOptional, result.d.OptionalApproverOneList, false);
                    populateDataToDropdown(LeaveRequestPageControl.ApproverTwoOptional, result.d.OptionalApproverTwoList, false);
                }
                else {
                    alert(LeavePageResource.LeaveManagement_ErrorOccurre);
                    console.log("ERROR: " + result.CodeMessageResult.Message);
                }
            }
            else {
                alert(LeavePageResource.LeaveManagement_ErrorOccurre);
            }
        }
        ,
        error: function (jqXhr) {
            if (jqXhr.status === 500) {
                console.log('Internal error: ' + jqXhr.responseText);
            } else {
                console.log("Unexpected error.");
            }
        }
    });
}

function populateDataToDropdown(dropdown, dataList, isRequire) {
    $(dropdown).empty();
    if (!isRequire) {
        var defaultOption = '<option value="0">--' + LeavePageResource.Dropdown_Select + "--</option>";
        $(dropdown).append(defaultOption);
    }
    if (dataList !== undefined) {
        for (var index = 0; index < dataList.length; index++) {
            var option = '<option value="' + dataList[index].ID + '">' + dataList[index].FullName + "</option>";
            $(dropdown).append(option);
        }
    }
}

function calculateWorkingHour(fromDate, toDate) {
    //ToDo: Need to calculate working hour
    $(LeaveRequestPageControl.LeaveHourCustom).val(40);
}

function leaveRequestNewForm(ctx) {
    var formTable = LeaveRequestForm.getHtml();
    formTable = formTable.replace("{{Requester_Title}}", Functions.getSPFieldTitle(ctx, "Requester"));
    formTable = formTable.replace("{{From_Title}}", Functions.getSPFieldTitle(ctx, "CommonFrom"));
    formTable = formTable.replace("{{To_Title}}", Functions.getSPFieldTitle(ctx, "To"));
    formTable = formTable.replace("{{LeaveHour_Title}}", Functions.getSPFieldTitle(ctx, "LeaveHours"));
    formTable = formTable.replace("{{Reason_Title}}", Functions.getSPFieldTitle(ctx, "Reason"));
    formTable = formTable.replace("{{Left_Title}}", Functions.getSPFieldTitle(ctx, "Leaved"));
    formTable = formTable.replace("{{TransferworkTo_Title}}", Functions.getSPFieldTitle(ctx, "TransferworkTo"));
    formTable = formTable.replace("{{ApproverOne_Title}}", Functions.getSPFieldTitle(ctx, "LeaveApproverOne"));
    formTable = formTable.replace("{{ApproverTwo_Title}}", Functions.getSPFieldTitle(ctx, "LeaveApproverTwo"));
    formTable = formTable.replace("{{ApproverThree_Title}}", Functions.getSPFieldTitle(ctx, "LeaveApproverThree"));

    formTable = formTable.replace("{{Requester}}", Functions.getSPFieldRender(ctx, "Requester"));
    formTable = formTable.replace("{{From}}", Functions.getSPFieldRender(ctx, "CommonFrom"));
    formTable = formTable.replace("{{To}}", Functions.getSPFieldRender(ctx, "To"));
    formTable = formTable.replace("{{LeaveHour}}", Functions.getSPFieldRender(ctx, "LeaveHours"));
    formTable = formTable.replace("{{Reason}}", Functions.getSPFieldRender(ctx, "Reason"));
    formTable = formTable.replace("{{Left}}", Functions.getSPFieldRender(ctx, "Leaved"));
    formTable = formTable.replace("{{TransferworkTo}}", Functions.getSPFieldRender(ctx, "TransferworkTo"));
    formTable = formTable.replace("{{ApproverOne}}", Functions.getSPFieldRender(ctx, "LeaveApproverOne"));
    formTable = formTable.replace("{{ApproverTwo}}", Functions.getSPFieldRender(ctx, "LeaveApproverTwo"));
    formTable = formTable.replace("{{ApproverThree}}", Functions.getSPFieldRender(ctx, "LeaveApproverThree"));

    formTable = formTable.replace("{{AlertFromDate}}", LeavePageResource.LeaveManagement_FromDateAlert);
    formTable = formTable.replace("{{Save_Value}}", LeavePageResource.SaveButton);
    formTable = formTable.replace("{{Cancel_Value}}", LeavePageResource.CancelButton);

    formTable = formTable.replace(/{{RequiredFieldText}}/gi, LeavePageResource.RequireField);

    formTable = formTable.replace("{{FormId}}", ctx.FormUniqueId);
    return formTable;
}


function LeaveFormSubmit(formId) {
    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    debugger;
    var isApproverValidationValid = validateApprover();
    if (isApproverValidationValid === true) {
        SPClientForms.ClientFormManager.SubmitClientForm(formId);
    }
}

function LeaveFormCancel() {
    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    window.location.href = _spPageContextInfo.webAbsoluteUrl + "/SitePages/LeaveManagement.aspx";
}



var LeaveRequestForm =
    {
        getHtml: function () {
            var html = '<table class="ms-formtable" style="margin-top: 8px;" border="0" cellpadding="0" cellspacing="0" width="100%">'
                + '<tbody>'
                + '    <tr>'
                + '       <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '		<nobr>{{Requester_Title}}<span class="ms-accentText"   title="{{RequiredFieldText}}"> *</span></nobr>'
                + '          </span>'
                + '      </td>'
                + '       <td valign="top" width="350px" class="ms-formbody">'
                + '           <span dir="none"><div id="RequesterCustom">{{Requester}}</div><br/><span id="stada-employee-id"></span>'
                + '<span style="margin-left:10px;" id="stada-employee-department"></span></span>'
                + '</td>'
                + '</tr>'
                + '<tr>'
                + '  <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '		<nobr>  {{From_Title}}<span class="ms-accentText"   title="{{RequiredFieldText}}"> *</span></nobr>'
                + '    </span>'
                + '   </td>'
                + '   <td valign="top" width="350px" class="ms-formbody">'
                + '       <span dir="none"><div id="FromDateCustom">{{From}}</div ></span><div id="AlertFromDate"><span  role="alert" class="ms-formvalidation ms-csrformvalidation" style="color:orange;">{{AlertFromDate}}</span></div>'
                + '  </td>'
                + '</tr>'
                + '<tr>'
                + '  <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '     <nobr>{{To_Title}}<span class="ms-accentText"   title="{{RequiredFieldText}}"> *</span></nobr>'
                + '   </span>'
                + ' </td>'
                + ' <td valign="top" width="350px" class="ms-formbody">'
                + '     <span dir="none"><div id="ToDateCustom">{{To}}</div><span role="alert" class="ms-formvalidation ms-csrformvalidation" id="ToDateValidateCustom"></span></span>'
                + ' </td>'
                + '</tr>'
                + '<tr>'
                + '   <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '      <nobr>{{LeaveHour_Title}}<span class="ms-accentText"   title="{{RequiredFieldText}}"> *</span></nobr>'
                + '     </span>'
                + ' </td>'
                + ' <td valign="top" width="350px" class="ms-formbody">'
                + '     <span dir="none"><div id="LeaveHourCustom">{{LeaveHour}}</div></span>'
                + '   </td>'
                + '</tr>'
                + '<tr>'
                + ' <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '	<nobr>{{Reason_Title}}</nobr>'
                + '</span></td>'
                + '  <td valign="top" width="350px" class="ms-formbody">'
                + '     <span dir="none">{{Reason}}<br></span>'
                + '  </td>'
                + '</tr>'
                + '<tr>'
                + ' <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '     <nobr>{{TransferworkTo_Title}}<span class="ms-accentText"   title="{{RequiredFieldText}}"> *</span></nobr>'
                + '     </span>'
                + '  </td>'
                + '  <td valign="top" width="350px" class="ms-formbody">'
                + '     <span dir="none"><div id="TranferworkToCustom">{{TransferworkTo}}</div><br/></span><span id="trsf-stada-employee-id"></span>'
                + '<span style="margin-left:10px;" id="trsf-stada-employee-department"></span>'
                + '</td>'
                + '</tr>'
                + '<tr>'
                + '  <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '		<nobr>{{Left_Title}}</nobr>'
                + '	</span></td>'
                + '   <td valign="top" width="350px" class="ms-formbody">'
                + '        <span dir="none">{{Left}}<br></span>'
                + ' </td>'
                + '</tr>'
                + '<tr>'
                + '  <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '    <nobr>{{ApproverOne_Title}}</nobr>'
                + '    </span>'
                + ' </td>'
                + ' <td valign="top" width="350px" class="ms-formbody">'
                + '     <span dir="none"><div id="ApproverOneCustom">{{ApproverOne}}</div><br></span>'
                + '  </td>'
                + '</tr>'
                + '<tr>'
                + '   <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '		<nobr>{{ApproverTwo_Title}}<span class="ms-accentText"   title="{{RequiredFieldText}}"> *</span></nobr>'
                + '      </span>'
                + '  </td>'
                + '   <td valign="top" width="350px" class="ms-formbody">'
                + '       <span dir="none"><div id="ApproverTwoCustom">{{ApproverTwo}}</div><span class="ms-formvalidation sp-peoplepicker-errorMsg"><span role="alert" id="ApproverTwoErrorLabel"><br></span></span<br></span>'
                + '  </td>'
                + '</tr>'
                + '<tr>'
                + ' <td nowrap="true" valign="top" width="113px" class="ms-formlabel"><span class="ms-h3 ms-standardheader">'
                + '    <nobr>{{ApproverThree_Title}}<span class="ms-accentText"> *</span></nobr>'
                + '     </span>'
                + '  </td>'
                + ' <td valign="top" width="350px" class="ms-formbody">'
                + '      <span dir="none"><div id="ApproverThreeCustom">{{ApproverThree}}</div><span class="ms-formvalidation sp-peoplepicker-errorMsg"><span role="alert" id="ApproverThreeErrorLabel"><br></span></span><br></span>'
                + '  </td>'
                + '</tr>'
                + '</tbody>'
                + '</table>'
                + '<table class="ms-formtoolbar" cellpadding="2" cellspacing="0" border="0" width="100%" role="presentation">'
                + ' <tbody><tr>'
                + '		<td class="ms-toolbar" nowrap="nowrap" width="85%">'
                + '	<table cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td align="right" width="100%" nowrap="nowrap">'
                + '			<input type="button"  value="{{Save_Value}}" class="ms-ButtonHeightWidth"  onclick="LeaveFormSubmit(\'{{FormId}}\')">'
                + '		</td> </tr> </tbody></table>'
                + '		</td>'
                + '	<td class="ms-separator">&nbsp;</td>'
                + '		<td class="ms-toolbar" nowrap="nowrap">'
                + '		<table cellpadding="0" cellspacing="0" width="100%"><tbody><tr><td align="right" width="100%" nowrap="nowrap">'
                + '		<input type="button" value="{{Cancel_Value}}" class="ms-ButtonHeightWidth" target="_self" onclick="LeaveFormCancel()">'
                + '		</td> </tr> </tbody></table>'
                + '		</td>'
                + ' </tr>'
                + '</tbody></table>'
                + '</td></tr></tbody></table>';
            return html;
        }
    }

