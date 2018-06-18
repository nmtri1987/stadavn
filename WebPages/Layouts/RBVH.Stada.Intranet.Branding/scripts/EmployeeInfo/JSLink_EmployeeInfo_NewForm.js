var FullNameOrder = 0;// 0: Ho + ten ; 1: Ten + Ho
(function () {
    var propArray = [
        { PropertyName: 'FirstName', FieldName: "FirstName" },
        { PropertyName: 'LastName', FieldName: "LastName" },
        { PropertyName: 'WorkEmail', FieldName: "Email" },
        { PropertyName: 'Title', FieldName: "Position" }
    ];
    function renderPeoplePickerTemplate(renderCtx) {
        var fieldCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(renderCtx);
        //1. Add event handler for People Picker control 
        fieldCtx.fieldSchema.OnUserResolvedClientScript = function (topLevelElementId, usersInfo) {
            populateFields(renderCtx, usersInfo);
        };
        var renderTemplate = SPClientPeoplePickerCSRTemplate(renderCtx);
        return renderTemplate;
    }

    function populateFields(renderCtx, usersInfo) {
        if (usersInfo.length === 0)
            return;

        var userKey = usersInfo[0].Description;
        var url = _spPageContextInfo.webAbsoluteUrl + "/_api/SP.UserProfiles.PeopleManager/getpropertiesfor(@v)?@v='" + userKey + "'";// + "'$select=FirstName,LastName,WorkEmail,Title";
        try {
            $.ajax({
                url: url,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                cache: false,
                success: function (data) {
                    //2. Retrieving an additional data like user profile properties goes here 
                    if (data.d.UserProfileProperties != undefined && data.d.UserProfileProperties.results.length > 0) {
                        var properties = data.d.UserProfileProperties.results;
                        for (var i = 0; i < properties.length; i++) {
                            for (var j = 0; j < propArray.length; j++) {
                                //3. Set fields values  
                                if (properties[i].Key === propArray[j].PropertyName) {
                                    EmployeeForm.setFieldControlValue(propArray[j].FieldName, properties[i].Value);
                                    EmployeeForm.setFullName();
                                }
                            }
                        }
                    }
                    else {
                        EmployeeForm.clearTextBoxs();
                    }
                },
                error: function (x, y, z) {
                }
            });
        } catch (e) {
        }
    }
    function registerFormRenderer() {
        var formContext = {};
        formContext.Templates = {};
        formContext.Templates.OnPostRender = onPostRender;
        formContext.Templates.Fields = {
            "ADAccount": {
                "NewForm": renderPeoplePickerTemplate,
                "EditForm": renderPeoplePickerTemplate
            },
        };
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(formContext);
    }
    ExecuteOrDelayUntilScriptLoaded(registerFormRenderer, 'clienttemplates.js');
})();

var runOnce = false;
function onPostRender(ctx) {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {

        SP.SOD.registerSod(EmployeeFormResource.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + EmployeeFormResource.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(EmployeeFormResource.ListResourceFileName, "Res", OnListResourcesReadyEmployeeForm);
        if (!runOnce) {
            EmployeeForm.onTypeEmployeeName();
            EmployeeForm.onDepartmentChange();
            runOnce = true;
        }
    }, "strings.js", "sp.js");
    function OnListResourcesReadyEmployeeForm() {
        EmployeeFormResource.None = Res.none;
        if (ctx.BaseViewID == "NewForm") {
            //Just load when new form
            EmployeeForm.loadManager("");
        }
    }
}
var EmployeeFormResource = {
    ListResourceFileName: "RBVHStadaLists",
    None: "(None)"
}

var EmployeeForm =
    {
        MIN_EMPLOYEE_LEVEL: 3.1, // Tổ phó
        LoadManagerUrl: "/_vti_bin/Services/Employee/EmployeeService.svc/GetManagerByDepartment/",
        setFieldControlValue: function (fieldName, fieldValue) {
            var fieldControl = $('[id ^=' + fieldName + '][id $=Field]');
            fieldControl.val(fieldValue);
        },
        onTypeEmployeeName: function () {
            $(document).on("input", this.getControlInput("FirstName"), function () {
                EmployeeForm.setFullName();
            });

            $(document).on("input", this.getControlInput("LastName"), function () {
                EmployeeForm.setFullName();
            });

            $(document).on("input", this.getControlInput("ADAccount"), function () {
                EmployeeForm.clearTextBoxs();
            });
        },
        getControlInput: function (controlName) {
            return "input[id^='" + controlName + "_'][id $=Field]";
        },
        getControlSelect: function (controlName) {
            return "select[id^='" + controlName + "_'][id $=Field]";
        },
        setFullName: function () {
            var firstName = $(this.getControlInput("FirstName")).val();
            var lastName = $(this.getControlInput("LastName")).val();
            if (FullNameOrder == 0) {
                $(this.getControlInput("EmployeeDisplayName")).val(lastName + " " + firstName);
            }
            else {
                $(this.getControlInput("EmployeeDisplayName")).val(firstName + " " + lastName);
            }
        },
        clearTextBoxs: function () {
            this.setFieldControlValue("FirstName", "");
            this.setFieldControlValue("LastName", "");
            this.setFieldControlValue("EmployeeDisplayName", "");
            this.setFieldControlValue("Email", "");
        },
        onDepartmentChange: function () {
            $(document).on("change", EmployeeForm.getControlSelect("EmployeeInfoDepartment"), function () {
                var item = $(this);
                var selectedDepartmentId = item.val();
                EmployeeForm.loadManager(selectedDepartmentId);
            });
        },
        loadManager: function (departmentId) {
            if (departmentId == 0 || departmentId == "") {
                departmentId = "0";
            }
            $.ajax({
                type: "GET",
                url: _spPageContextInfo.webAbsoluteUrl + String(EmployeeForm.LoadManagerUrl) + departmentId + "/" + EmployeeForm.MIN_EMPLOYEE_LEVEL,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    EmployeeForm.appendDepartmentToSelect(data);
                },
                error: function () {
                    //console.log("Error");
                }
            });
        },
        appendDepartmentToSelect: function (data) {
            var managerSelectcontrol = $(EmployeeForm.getControlSelect("EmployeeInfoManager"));
            var departmentSelectControl = $(EmployeeForm.getControlSelect("EmployeeInfoDepartment"));
            managerSelectcontrol.empty();
            managerSelectcontrol.append('<option value="0">' + EmployeeFormResource.None + '</option>');
            if (data && data.length > 0) {

                data.forEach(function (element) {
                    var option = '<option value="' + element.ID + '">' + element.FullName + '</option>';
                    managerSelectcontrol.append(option);
                }, this);
            }
        }
    }