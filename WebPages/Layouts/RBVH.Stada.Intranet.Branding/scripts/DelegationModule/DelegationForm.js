RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
function ValidateBeforeDelegate() {
    var isValid = delegationFormInstance.IsValidForm();
    if (isValid) {
        $(".se-pre-con").fadeIn(0);
    }
    return isValid;
}
function validateBeforeSearch() {
    var isValid = delegationFormInstance.IsValidSearchingInformation();
    if (isValid == false) {
        $(".se-pre-con").fadeOut(0);
    }
    else {
        $(".se-pre-con").fadeIn(0);
    }
    return isValid;
}
RBVH.Stada.WebPages.pages.DelegationForm = function (settings) {
    this.Settings = {
        Controls:
        {

        },
        Today: {},
        Modules: []
    }
    $.extend(true, this.Settings, settings);
    this.Initialize();
};

RBVH.Stada.WebPages.pages.DelegationForm.prototype =
    {
        Initialize: function () {
            var that = this;
            $(document).ready(function () {
                ExecuteOrDelayUntilScriptLoaded(function () {
                    that.InitControls();
                    that.EventsRegister();
                    that.ShowErrorMessage();
                }, "sp.js", "strings.js");
            });
        },

        InitControls: function () {
            var that = this;
            that.Settings.Modules = [];
            var moduleOptions = $(that.Settings.Controls.ddlModuleSelector + " option");
            if (moduleOptions) {
                for (var i = 0; i < moduleOptions.length; i++) {
                    var optionValue = $(moduleOptions[i]).val();
                    if (optionValue && optionValue != "All") {
                        that.Settings.Modules.push(optionValue);
                    }
                }
            }

            that.PopulateData();

            $(that.Settings.Controls.ddlModuleSelector).select2({
                placeholder: that.Settings.Resources.DelegationModuleSelectText,
                width: "80%"
            })

            $(that.Settings.Controls.toEmployeeSelector).select2({
                placeholder: that.Settings.Resources.DelegationModuleSelectText,
                width: "80%"
            })

            $(that.Settings.Controls.fromDateSelector).keydown(function (e) { e.preventDefault(); return false; });
            $(that.Settings.Controls.toDateSelector).keydown(function (e) { e.preventDefault(); return false; });
        },
        FilterEmployee: function (isFirstLoad) {
            var that = this;
            $(that.Settings.Controls.toEmployeeSelector).val([]).trigger('change.select2');
            var selectedFromDateOption = $(that.Settings.Controls.fromEmployeeSelector + " option:selected");
            if (selectedFromDateOption) {
                var delegatedPositions = [];
                var fromDateDelegatedPositionsString = $(selectedFromDateOption).attr("delegatedemployeepositions");
                if (fromDateDelegatedPositionsString && fromDateDelegatedPositionsString != "") {
                    delegatedPositions = JSON.parse(fromDateDelegatedPositionsString);
                }
                var employeeDepartmentId = parseInt($(selectedFromDateOption).attr("employeeinfodepartment"));
                var employeeId = parseInt($(selectedFromDateOption).attr("value"));
                var toEmpOptions = $(that.Settings.Controls.toEmployeeSelector + " option");
                if (toEmpOptions) {
                    // hide all options
                    for (var i = 0; i < toEmpOptions.length; i++) {
                        $(toEmpOptions[i]).hideOption();
                    }
                    //enable options that depend on role & department
                    for (var j = 0; j < toEmpOptions.length; j++) {
                        var $item = $(toEmpOptions[j]);
                        var departmentId = parseInt($item.attr("employeeinfodepartment"));
                        var positionId = parseInt($item.attr("position"));
                        var itemId = parseInt($item.attr("value"))

                        if (that.FindObjectByValue(delegatedPositions, positionId) != null && departmentId === employeeDepartmentId && employeeId !== itemId) {
                            $(toEmpOptions[j]).showOption();
                        }
                    }
                }
            }

            var $toEmployeeOptions = $(that.Settings.Controls.toEmployeeSelector + " option:not([disabled])");

            if ($toEmployeeOptions && $toEmployeeOptions.length > 0) {
                $($toEmployeeOptions[0]).removeAttr("disabled");
                // var id = $($toEmployeeOptions[0]).val();
                // if (id) {
                //     $(that.Settings.Controls.toEmployeeSelector).val([id]).trigger('change.select2');
                //     $(that.Settings.Controls.hdSelectedToEmployeeSelector).val(JSON.stringify([id]));
                // }
                var hdToemployeeDdlValues = $(that.Settings.Controls.hdSelectedToEmployeeSelector).val();
                if (hdToemployeeDdlValues && hdToemployeeDdlValues != "" && isFirstLoad == true) {
                    $(that.Settings.Controls.toEmployeeSelector).val(JSON.parse(hdToemployeeDdlValues)).trigger('change.select2');
                }
            }

            // var hdModuleValues = $(that.Settings.Controls.hdSelectedModulesSelector).val();
            var hdModuleValues = $(that.Settings.Controls.hdSelectedModulesSelector).val();
            if (!hdModuleValues || hdModuleValues == "") {
                $(that.Settings.Controls.ddlModuleSelector).val(["All"]).trigger('change.select2');
                $(that.Settings.Controls.hdSelectedModulesSelector).val(JSON.stringify(that.Settings.Modules));
            }
            else if (hdModuleValues && hdModuleValues != "" && isFirstLoad == true) {
                if (that.ArraysEqual(that.Settings.Modules, JSON.parse(hdModuleValues))) {
                    $(that.Settings.Controls.ddlModuleSelector).val(["All"]).trigger('change.select2');
                    $(that.Settings.Controls.hdSelectedModulesSelector).val(JSON.stringify(that.Settings.Modules));
                }
                else {
                    $(that.Settings.Controls.ddlModuleSelector).val(JSON.parse(hdModuleValues)).trigger('change.select2');
                }
            }
        },
        FindObjectByValue: function (objectArray, propValue) {
            for (var i = 0, len = objectArray.length; i < len; i++) {
                if (objectArray[i].LookupId === propValue)
                    return objectArray[i]; // Return as soon as the object is found
            }
            return null; // The object was not found
        },
        PopulateData: function () {
            var that = this;
            //Set default value
            var now = new Date();
            var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            that.Settings.Today = currentDate;

            that.FilterEmployee(true);

        },
        EventsRegister: function () {
            var that = this;
            $(that.Settings.Controls.fromEmployeeSelector).change(function () {
                that.FilterEmployee(false);
                //Trường hợp này xảy ra khi user là System Account. 
                //Phải clear hết các checkbox và disable các checkbox ở bảng kết quả tìm kiếm
                that.DisableCheckboxes();
            });

            that.SetCheckAllCheckBox();

            //$(that.Settings.Controls.btnSearchSelector).on('click', function () {
            //    $(".se-pre-con").fadeIn(0);
            //});
            $(that.Settings.Controls.btnCloseSelector).on('click', function () {
                $(".se-pre-con").fadeIn(0);
            });

            $(that.Settings.Controls.ddlModuleSelector).on('select2:select', function (e) {
                var allDataOption = "All";
                var modules = $(that.Settings.Controls.ddlModuleSelector).val();
                var data = e.params.data;
                if (data.id === allDataOption) {
                    modules = [allDataOption];
                    $(that.Settings.Controls.hdSelectedModulesSelector).val(JSON.stringify(that.Settings.Modules));
                }
                else {
                    if ($.inArray(allDataOption, modules) > -1) {
                        //   modules = modules.filter(val => val !== allDataOption);
                        var moduleArray = [];
                        for (var i = 0; i < modules.length; i++) {
                            if (modules[i] != allDataOption) {
                                moduleArray.push(modules[i]);
                            }
                        }
                        modules = moduleArray;
                    }
                    $(that.Settings.Controls.hdSelectedModulesSelector).val(JSON.stringify(modules));
                }
                $(that.Settings.Controls.ddlModuleSelector).val(modules).trigger('change.select2');
            });
            $(that.Settings.Controls.ddlModuleSelector).on('select2:unselect', function (e) {
                var modules = $(that.Settings.Controls.ddlModuleSelector).val();
                $(that.Settings.Controls.hdSelectedModulesSelector).val(JSON.stringify(modules));
            });
            // $(that.Settings.Controls.ddlModuleSelector).change(function (e, a, b, s) {
            //     var modules = $(that.Settings.Controls.ddlModuleSelector).val();
            //     if ($.inArray("All", modules) > -1) {
            //         modules = ["All"];
            //     }
            //     $(that.Settings.Controls.ddlModuleSelector).val(modules).trigger('change.select2');
            // })
            $(that.Settings.Controls.toEmployeeSelector).on('select2:select', function (e) {
                var modules = $(that.Settings.Controls.toEmployeeSelector).val();
                $(that.Settings.Controls.hdSelectedToEmployeeSelector).val(JSON.stringify(modules));
            })
            $(that.Settings.Controls.toEmployeeSelector).on('select2:unselect', function (e) {
                var modules = $(that.Settings.Controls.toEmployeeSelector).val();
                $(that.Settings.Controls.hdSelectedToEmployeeSelector).val(JSON.stringify(modules));
            })
        },
        SetCheckAllCheckBox: function () {
            //Set item checkboxes is checked or uncheck when clink on check all checkbox
            var $checkAllCheckbox = $("span[class='select-all-task'] input[type='checkbox']");
            var $itemCheckBoxes = $("span[class='select-task'] input[type='checkbox']");
            $checkAllCheckbox.change(function () {
                if ($checkAllCheckbox.is(":checked") == true) {
                    $itemCheckBoxes.prop("checked", true);
                }
                else {
                    $itemCheckBoxes.removeAttr("checked");
                }
            })
            $itemCheckBoxes.change(function () {
                $checkAllCheckbox.removeAttr("checked");
                var checkedCount = 0;
                var uncheckedCount = 0;
                var itemlength = $itemCheckBoxes.length;
                if ($itemCheckBoxes) {
                    for (var i = 0; i < itemlength; i++) {
                        if ($($itemCheckBoxes[i]).is(":checked") == true) {
                            checkedCount++;
                        }
                        else {
                            uncheckedCount++;
                        }
                    }
                }
                if (checkedCount > 0 && checkedCount === itemlength) {
                    $checkAllCheckbox.prop("checked", true);
                }
                if (uncheckedCount > 0 && uncheckedCount === itemlength) {
                    $checkAllCheckbox.removeAttr("checked");
                }
            });
        },
        DisableCheckboxes: function () {
            var that = this;
            $("span[class='select-task'] input[type='checkbox']").removeAttr("checked");
            $("span[class='select-task'] input[type='checkbox']").attr("disabled", "true");
            $("span[class='select-all-task'] input[type='checkbox']").removeAttr("checked");
            $("span[class='select-all-task'] input[type='checkbox']").attr("disabled", "true");
        },
        ShowErrorMessage: function () {
            try {
                var that = this;
                var hdErrorMessageId = that.Settings.Controls.hdErrorMessageSelector;
                if ($(hdErrorMessageId).val() != '') {
                    alert($(hdErrorMessageId).val());
                }
            } catch (err) { }
        },
        //Validation
        IsValidForm: function () {
            var that = this;
            var isValid = false;
            var isValid1 = that.ValidateRequiredField();
            if (isValid1 === true) {
                var valid2 = that.IsFromDateValid();
                var valid3 = that.IsToDateValid();
                //var valid4 = that.IsTaskCheckBoxValid();
                if (valid2 === true && valid3 === true) {
                    isValid = true;
                }
            }
            return isValid;
        },
        IsFromDateValid: function () {
            var that = this;
            var errorCount = 0;
            var fromDateVal = $(that.Settings.Controls.fromDateSelector).val();
            var fromDateObject = Functions.parseVietNameseDate(fromDateVal);
            if (fromDateObject) {
                fromDateOnly = new Date(fromDateObject.getFullYear(), fromDateObject.getMonth(), fromDateObject.getDate());
                if (fromDateOnly < that.Settings.Today) {
                    $(that.Settings.Controls.dtFromDateSelector_Error).html(that.Settings.Resources.FromDateErrorMessage).show();
                    errorCount++;
                }
                else {
                    $(that.Settings.Controls.dtFromDateSelector_Error).html("").hide();
                }
            }
            else {
                errorCount++;
            }

            return errorCount == 0;
        },
        IsToDateValid: function () {
            var that = this;
            var errorCount = 0;
            var fromDateVal = $(that.Settings.Controls.fromDateSelector).val();
            var toDateVal = $(that.Settings.Controls.toDateSelector).val();
            var fromDateObject = Functions.parseVietNameseDate(fromDateVal);
            var toDateObject = Functions.parseVietNameseDate(toDateVal);
            if (fromDateObject && toDateObject) {
                fromDateOnly = new Date(fromDateObject.getFullYear(), fromDateObject.getMonth(), fromDateObject.getDate());
                toDateOnly = new Date(toDateObject.getFullYear(), toDateObject.getMonth(), toDateObject.getDate());
                if (toDateOnly < that.Settings.Today || fromDateOnly < that.Settings.Today || fromDateOnly > toDateOnly) {
                    $(that.Settings.Controls.dtToDateSelector_Error).html(that.Settings.Resources.ToDateErrorMessage).show();
                    errorCount++;
                }
                else {
                    $(that.Settings.Controls.dtToDateSelector_Error).html("").hide();
                }
            }
            else {
                errorCount++;
            }

            return errorCount == 0;
        },
        IsFromEmployeeEmpty: function () {
            var that = this;
            var errorCount = 0;
            errorCount += that.ValidateEmptyField($(that.Settings.Controls.fromEmployeeSelector + " option:selected").val(), that.Settings.Controls.ddlFromEmployeeSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            return !(errorCount == 0);
        },
        IsToEmployeeEmpty: function () {
            var that = this;
            var errorCount = 0;
            errorCount += that.ValidateEmptyField($(that.Settings.Controls.toEmployeeSelector + " option:selected").val(), that.Settings.Controls.ddlToEmployeeSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            return !(errorCount == 0);
        },

        ValidateRequiredField: function () {
            $("span[class='ms-formvalidation ms-csrformvalidation']").html("");
            var that = this;
            var emptyFieldCount = 0;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.fromDateSelector).val(), that.Settings.Controls.dtFromDateSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.toDateSelector).val(), that.Settings.Controls.dtToDateSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.ddlModuleSelector).val(), that.Settings.Controls.ddlModuleSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.fromEmployeeSelector + " option:selected").val(), that.Settings.Controls.ddlFromEmployeeSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.toEmployeeSelector + " option:selected:not([disabled])").val(), that.Settings.Controls.ddlToEmployeeSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            return emptyFieldCount == 0;
        },
        //IsTaskCheckBoxValid: function () {
        //    var that = this;
        //    var count = 0;
        //    var $itemCheckBoxes = $("span[class='select-task'] input[type='checkbox']");
        //    if ($itemCheckBoxes) {
        //        for (var i = 0; i < $itemCheckBoxes.length; i++) {
        //            if ($($itemCheckBoxes[i]).is(":checked") == true) {
        //                count++;
        //            }
        //        }
        //    }
        //    if (count > 0 || $(that.Settings.Controls.cbDelegationNewTaskSelector).is(":checked") == true) {
        //        $(that.Settings.Controls.cbDelegationNewTaskSelector_Error).html("").hide();
        //        return true;
        //    }
        //    else {
        //        $(that.Settings.Controls.cbDelegationNewTaskSelector_Error).html(that.Settings.Resources.DelegateTaskCheckBoxErrorMessage).show();
        //        return false;
        //    }
        //},
        ValidateEmptyField: function (controlValue, errorControl, message) {
            if (!controlValue || controlValue == "") {
                $(errorControl).html(message);
                $(errorControl).show();
                return false;
            }
            else {
                $(errorControl).hide();
                return true;
            }
        },
        IsValidSearchingInformation: function () {
            var that = this;
            $(that.Settings.Controls.ddlModuleSelector_Error).html("").hide();
            var emptyFieldCount = 0;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.ddlModuleSelector).val(), that.Settings.Controls.ddlModuleSelector_Error, that.Settings.Resources.CantLeaveTheBlank) == true ? 0 : 1;
            return emptyFieldCount === 0;
        },
        //End-Validation
        ArraysEqual: function (arr1, arr2) {
            if (arr1.length !== arr2.length)
                return false;
            for (var i = arr1.length; i--;) {
                if (arr1[i] !== arr2[i])
                    return false;
            }
            return true;
        },

    }