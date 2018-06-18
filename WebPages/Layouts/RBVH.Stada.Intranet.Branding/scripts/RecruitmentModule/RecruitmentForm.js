RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
function ValidateBeforeSaveAndSubmit() {
    var isValidForm = (recruitmentFormInstance.ValidateForm());
    return isValidForm;
}
RBVH.Stada.WebPages.pages.RecruitmentForm = function (settings) {
    this.Settings = {
        Controls:
        {

        },
        Fields: [],
        Data: [],
        IsViewMode: false,
        IsEditMode: false,
        IsFormEditable: false,
        FormModeParam: "new",
        DocumentCount: 0,
        OtherOptionValue: "Khác/Others",
        NoneOptionEnText: "(None)",
        NoneOptionVnText: "(Không có)",
        Grid:
        {
            CustomFields: {
                NoField: null,
                ForeignLanguageField: null,
                LevelField: null,
            },
            ForeignLanguageList: [],
            LevelList: [],
        },
        GridJsonArray: [],
        CurrentLcid: 1033,
        HiddenAttributeName: "current-value",
        Today: {}
    }
    $.extend(true, this.Settings, settings);
    this.Initialize();
};

RBVH.Stada.WebPages.pages.RecruitmentForm.prototype =
    {
        Initialize: function () {

            var that = this;
            $(document).ready(function () {
                ExecuteOrDelayUntilScriptLoaded(function () {
                    that.InitControls();
                    that.PopulateData();
                    that.SetCurrentValueForControls();
                    that.EventsRegister();
                    that.ShowErrorMessage();
                }, "sp.js", "strings.js");
            });
        },
        InitControls: function () {
            var that = this;
            var isEditableValue = $(that.Settings.Controls.hdIsEditableSelector).val();
            that.Settings.IsFormEditable = (isEditableValue == "True");
            var formMode = Functions.getParameterByName("mode");
            that.Settings.FormModeParam = formMode;
            that.CheckToEnableOrDisableInputTemplateName();
            that.DisableButtonControls();
            that.DisableCheckBoxOtherValueControls();
            that.LoadLcid();
            that.LoadHiddenDataToDropdown();
            if ($(that.Settings.Controls.dtAvailableTimeSelector).attr('disabled') == 'disabled') {
                $(that.Settings.Controls.dtAvailableTimeSelector).parent().next().hide();
            }
            if (that.Settings.IsFormEditable == true) {
                var workingExperienceOtherOption = $(that.Settings.Controls.cblWorkingExperience + " input[type='checkbox'][value='" + that.Settings.OtherOptionValue + "']:checked");
                if (workingExperienceOtherOption && workingExperienceOtherOption.length >= 1) {
                    $(that.Settings.Controls.txtOtherWorkingExperience).removeAttr("disabled");
                }
                var txtOtherComputerSkillsOption = $(that.Settings.Controls.cblComputerSkillsSelector + " input[type='checkbox'][value='" + that.Settings.OtherOptionValue + "']:checked");
                if (txtOtherComputerSkillsOption && txtOtherComputerSkillsOption.length >= 1) {
                    $(that.Settings.Controls.txtOtherComputerSkillsSelector).removeAttr("disabled");
                }
            }

            //Set giá trị default của ngày hôm nay là giá trị get từ phía client
            var now = new Date();
            var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            that.Settings.Today = currentDate;
            // but this time is in client side, we need to set current date value from server side
            var todayServerTimeString = $(that.Settings.Controls.hdTodaySelector).val();
            if (todayServerTimeString && todayServerTimeString.length > 0) {
                var serverDateObject = Functions.parseVietNameseDate(todayServerTimeString);
                if (serverDateObject) {
                    var serverDate = new Date(serverDateObject.getFullYear(), serverDateObject.getMonth(), serverDateObject.getDate());
                    that.Settings.Today = serverDate;
                }
            }

            // Show/hide Approval Status
            var approvalStatusValue = $(that.Settings.Controls.ApprovalStatusValueSelector).val();
            if (!!approvalStatusValue) {
                $(that.Settings.Controls.ApprovalStatusTdSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(approvalStatusValue));
                $(that.Settings.Controls.ApprovalStatusTrSelector).show();
            }
        },
        LoadLcid: function () {
            var that = this;
            var lcid = SP.Res.lcid;
            that.Settings.CurrentLcid = lcid;
        },
        DisableButtonControls: function () {
            var that = this;
            $(that.Settings.Controls.btnSaveDraftSelector).prop('disabled', true);
            $(that.Settings.Controls.btnSaveAndSubmitSelector).prop('disabled', true);
        },
        DisableCheckBoxOtherValueControls: function () {
            var $tables = $("table[othervaluecontrolid]");
            for (var i = 0; i < $tables.length; i++) {
                var othervaluecontrolid = $($tables[i]).attr("othervaluecontrolid");
                $("#" + othervaluecontrolid).prop("disabled", "true");
            }
        },
        SetCheckBoxSelectOne: function (control) {
            $(control + " input[type='checkbox']").on('change', function () {
                $(control + ' input[type="checkbox"]').not(this).prop('checked', false);
            });
        },
        SetCheckBoxSelectOneWithOtherValue: function (control, otherValue, otherValueControl) {
            $(control + " input[type='checkbox']").on('change', function () {
                $(control + ' input[type="checkbox"]').not(this).prop('checked', false);
                if ((otherValue.localeCompare($(this).val()) == 0) && ($(this).prop('checked') == true)) {
                    $(otherValueControl).removeAttr('disabled');
                } else {
                    $(otherValueControl).attr('disabled', 'disabled');
                    $(otherValueControl).val('');
                }
            });
        },
        DisbleDeleteButtonOnGrid: function () {
            $(".jsgrid-button.jsgrid-delete-button").prop("disabled", true);
            $(".jsgrid-button.jsgrid-delete-button").css("opacity", 0.1);
        },
        CheckboxSelectOtherValue: function (control) {
            var that = this;
            $(control + " input[type='checkbox']").change(function (element) {
                var curentElement = $(this);
                var parentTable = curentElement.closest("table");
                var otherControl = parentTable.attr("othervaluecontrolid");

                if ($(this).is(':checked')) {
                    if ($(this).val() == that.Settings.OtherOptionValue) {
                        $("#" + otherControl).removeAttr("disabled");
                    }
                    else {
                        $("#" + otherControl).val("");
                        $("#" + otherControl).attr("disabled", "false");
                    }
                }
                else {
                    $("#" + otherControl).val("");
                    $("#" + otherControl).attr("disabled", "false");
                }
            })
        },

        CheckboxSelectOtherValueMulti: function (control) {
            var that = this;
            $(control + " input[type='checkbox']").change(function (element) {
                var curentElement = $(this);
                var parentTable = curentElement.closest("table");
                var otherControl = parentTable.attr("othervaluecontrolid");
                if ($(this).val() == that.Settings.OtherOptionValue) {
                    if ($(this).is(':checked')) {
                        $("#" + otherControl).removeAttr("disabled");
                    }
                    else {
                        $("#" + otherControl).val("");
                        $("#" + otherControl).attr("disabled", "false");
                    }
                }
            })
        },
        EnableButtonControls: function () {
            var that = this;
            $(that.Settings.Controls.btnSaveDraftSelector).removeAttr('disabled');
            $(that.Settings.Controls.btnSaveAndSubmitSelector).removeAttr('disabled');
        },
        LoadHiddenDataToDropdown: function () {
            var that = this;
            var hdForeignLanguagesValue = $(that.Settings.Controls.hdForeignLanguagesSelector).val();
            var noneOptionText = that.Settings.NoneOptionEnText;
            if (that.Settings.CurrentLcid == 1066) {
                noneOptionText = that.Settings.NoneOptionVnText;
            }
            if (hdForeignLanguagesValue != "") {
                var jsonObject = JSON.parse(hdForeignLanguagesValue);
                if (jsonObject) {
                    var foreignLanguageArray = [];
                    // foreignLanguageArray.push({ ID: 0, Name: noneOptionText });
                    for (var i = 0; i < jsonObject.length; i++) {
                        var item = {};
                        item.Name = jsonObject[i].Name;
                        if (that.Settings.CurrentLcid == 1066) {
                            item.Name = jsonObject[i].VietnameseName;
                        }
                        item.ID = jsonObject[i].ID;
                        foreignLanguageArray.push(item);
                    }
                    that.Settings.Grid.ForeignLanguageList = foreignLanguageArray;
                }
            }
            //Load data foreign language level
            var hdForeignLanguageLevelsValue = $(that.Settings.Controls.hdForeignLanguageLevelsSelector).val();
            if (hdForeignLanguageLevelsValue != "") {
                var jsonObject = JSON.parse(hdForeignLanguageLevelsValue);
                if (jsonObject) {
                    var foreignLanguageLevelArray = [];
                    // foreignLanguageLevelArray.push({ ID: 0, Level: noneOptionText });
                    for (var i = 0; i < jsonObject.length; i++) {
                        var item = {};
                        item.Level = jsonObject[i].Level;
                        item.ID = jsonObject[i].ID;
                        foreignLanguageLevelArray.push(item);
                    }
                    that.Settings.Grid.LevelList = foreignLanguageLevelArray;
                }
            }
        },
        PopulateData: function () {
            var that = this;
            that.PopulateLanguageGrid();
        },
        EventsRegister: function () {
            var that = this;
            that.SetCheckBoxSelectOne(that.Settings.Controls.cblAppearanceSelector);
            that.SetCheckBoxSelectOneWithOtherValue(that.Settings.Controls.cblWorkingExperience, that.Settings.OtherOptionValue, that.Settings.Controls.txtOtherWorkingExperience);
            that.CheckboxSelectOtherValue(that.Settings.Controls.cblAppearanceSelector);
            that.CheckboxSelectOtherValueMulti(that.Settings.Controls.cblComputerSkillsSelector);

            $(that.Settings.Controls.dtAvailableTimeSelector).get(0).onvaluesetfrompicker = function (resultfield) {
                $(that.Settings.Controls.dtAvailableTimeSelector_Error).html("");
                var numberDaysAsPolicy = parseInt($(that.Settings.Controls.hdNumberOfStandardizedDaysBeforesubmissionSelector).val());
                var newDate = that.addDays(that.Settings.Today, numberDaysAsPolicy);
                var datePickerValue = Functions.parseVietNameseDate($(that.Settings.Controls.dtAvailableTimeSelector).val());

                if (newDate && datePickerValue) {
                    var datePickerDateOnly = new Date(datePickerValue.getFullYear(), datePickerValue.getMonth(), datePickerValue.getDate());
                    if (newDate > datePickerDateOnly) {
                        var message = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ResourceText.WrongPolicy, numberDaysAsPolicy);
                        var res = confirm(message);
                        if (res != true) {
                            $(that.Settings.Controls.dtAvailableTimeSelector).val("");
                        }
                    }
                }
            };

            // DucVT - ADD. 2017.10.10. TFS#1595
            $(that.Settings.Controls.cbSavingTemplateSelector).on('change', function () {
                that.CheckToEnableOrDisableInputTemplateName();
            });
        },
        addDays: function (theDate, days) {
            return new Date(theDate.getTime() + days * 24 * 60 * 60 * 1000);
        },
        PopulateLanguageGrid: function (control) {
            var that = this;
            that.BindGridColumns();
            $(that.Settings.Controls.divLanguageSkillsSelector).jsGrid({
                width: "100%",
                height: "140px",
                align: "center",
                inserting: that.Settings.IsFormEditable,
                editing: that.Settings.IsFormEditable,
                sorting: false,
                autoload: true,
                noDataContent: '',
                deleteConfirm: that.Settings.ConfirmDeleteMessage,
                data: that.Settings.Data,
                onDataLoaded: function (args) {
                    if (that.Settings.IsFormEditable == false) {
                        that.DisbleDeleteButtonOnGrid();
                    }
                },
                controller: {
                    loadData: function (filter) {
                        var d = $.Deferred();
                        var jsonString = $(that.Settings.Controls.hdRecruitmentLanguageSkillsSelector).val();
                        if (jsonString != "") {
                            var jsonObject = JSON.parse(jsonString);
                            for (var i = 0; i < jsonObject.length; i++) {
                                jsonObject[i].No = (i + 1);
                            }
                            that.Settings.GridJsonArray = jsonObject;
                        }
                        d.resolve(that.Settings.GridJsonArray);

                        return d.promise();
                    },
                    insertItem: function (item) {
                    },
                    updateItem: function (item) {
                    },
                    deleteItem: function (item) {
                    }
                },
                onItemInserting: function (args) {
                    var level = args.item.Level;
                    var foreignLanguage = args.item.ForeignLanguage;
                    if (foreignLanguage == "" || level.trim() == "") {
                        alert(that.Settings.ResourceText.LanguageSkillGrid_PleaseInputData);
                        args.cancel = true;
                    }
                },
                FindItemInArray: function (array, idValue) {
                    var result = $.grep(array, function (e) { return e.ID == idValue; });
                    if (result.length == 0) {
                        return false;
                    } else if (result.length == 1) {
                        return result[0];
                    } else {
                        return false;
                    }
                },
                onItemInserted: function (args) {
                    that.RebindHiddenFields(that.Settings.GridJsonArray);
                },
                onItemEditing: function (args) {
                },
                onItemUpdated: function (args) {
                    that.RebindHiddenFields(that.Settings.GridJsonArray);
                },

                onItemUpdating: function (args) {
                    var level = args.item.Level;
                    var foreignLanguage = args.item.ForeignLanguage;
                    if (foreignLanguage == "" || level.trim() == "") {
                        alert(that.Settings.ResourceText.LanguageSkillGrid_PleaseInputData);
                        args.cancel = true;
                    }
                },
                onItemDeleted: function (args) {

                    that.RebindHiddenFields(that.Settings.GridJsonArray);
                    for (var i = 0; i < that.Settings.GridJsonArray.length; i++) {
                        that.Settings.GridJsonArray[i].No = (i + 1);
                    }
                    if (that.Settings.GridJsonArray.length > 1)
                        $(that.Settings.Controls.divLanguageSkillsSelector).jsGrid("loadData");
                },
                onItemEditCancelling: function (args) {
                },
                fields:
                that.Settings.Grid.Fields,
            });
        },
        BindGridColumns: function () {
            var that = this;
            that.BindOrderNumber();

            jsGrid.fields.custOrderField = that.Settings.Grid.CustomFields.NoField;
            jsGrid.fields.custForeignLanguageField = that.Settings.Grid.CustomFields.ForeignLanguageField;
            jsGrid.fields.custLevelField = that.Settings.Grid.CustomFields.LevelField;

            that.Settings.Grid.Fields = [
                { name: "No", title: that.Settings.ResourceText.LanguageSkillGrid_No, width: 25, type: "custOrderField" },
                { name: "ForeignLanguage", title: that.Settings.ResourceText.LanguageSkillGrid_Language, width: 100, align: "left", type: "select", items: that.Settings.Grid.ForeignLanguageList, valueField: "ID", textField: "Name" },
                { name: "Level", title: that.Settings.ResourceText.LanguageSkillGrid_Level, width: 100, align: "left", align: "left", type: "text" },
                { type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 50, modeSwitchButton: false }
            ];
        },
        BindOrderNumber: function () {
            var that = this;
            that.Settings.Grid.CustomFields.NoField = function (config) {
                jsGrid.Field.call(this, config);
            };
            that.Settings.Grid.CustomFields.NoField.prototype = new jsGrid.Field({
                // itemTemplate: function (value) {
                //     
                //     var $item = value;
                //     var itemCounts = that.Settings.GridJsonArray.length;
                //     return 1;
                // },
                insertTemplate: function (value) {
                },
                editTemplate: function (value) {
                },
                insertValue: function () {
                    var itemCounts = that.Settings.GridJsonArray.length;
                    return itemCounts + 1;
                },
                editValue: function () {
                }
            });
        },
        RebindHiddenFields: function (data) {
            var that = this;
            if (data) {
                $(that.Settings.Controls.hdRecruitmentLanguageSkillsSelector).val(JSON.stringify(data));
            }
        },

        ShowErrorMessage: function () {
            try {
                var that = this;
                var hdErrorMessageId = that.Settings.Controls.hdErrorMessageSelector;
                if ($(hdErrorMessageId).val() != '') {
                    var message = $(hdErrorMessageId).val();
                    $(hdErrorMessageId).val('');
                    alert(message);
                }
            } catch (err) { }
        },

        ValidateForm: function () {
            var that = this;
            var valid = false;
            var valid1 = that.ValidateRequiredField();
            if (valid1 == true) {
                valid = that.IsFormValueChanged();
            }
            return valid;
        },
        ValidateRequiredField: function () {
            $("span[class='ms-formvalidation ms-csrformvalidation']").html("");
            var that = this;
            var emptyFieldCount = 0;

            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.ddlDepartmentSelector).val(), that.Settings.Controls.ddlDepartmentSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.txtPositionSelector).val(), that.Settings.Controls.txtPositionSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            var isQuantityNotEmpty = that.ValidateEmptyField($(that.Settings.Controls.txtQuantitySelector).val(), that.Settings.Controls.txtQuantitySelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += isQuantityNotEmpty;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.txtReasonForRecruitmentSelector).val(), that.Settings.Controls.txtReasonForRecruitmentSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            var isFromAgeNotEmpty = that.ValidateEmptyField($(that.Settings.Controls.txtFromAgeSelector).val(), that.Settings.Controls.txtFromAgeSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += isFromAgeNotEmpty;
            var isToAgeNotEmpty = that.ValidateEmptyField($(that.Settings.Controls.txtToAgeSelector).val(), that.Settings.Controls.txtToAgeSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;;
            emptyFieldCount += isToAgeNotEmpty;
            var isValidDate = that.ValidateEmptyField($(that.Settings.Controls.dtAvailableTimeSelector).val(), that.Settings.Controls.dtAvailableTimeSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += isValidDate;
            //Validate Quantity
            if (isQuantityNotEmpty == 0) {
                emptyFieldCount += that.ValidateNumber($(that.Settings.Controls.txtQuantitySelector).val(), that.Settings.Controls.txtQuantitySelector_Error, that.Settings.ResourceText.WrongNumber) == true ? 0 : 1;
            }
            if (isFromAgeNotEmpty == 0) {
                isFromAgeNotEmpty += that.ValidateNumber($(that.Settings.Controls.txtFromAgeSelector).val(), that.Settings.Controls.txtFromAgeSelector_Error, that.Settings.ResourceText.WrongNumber) == true ? 0 : 1;
            }
            emptyFieldCount += isFromAgeNotEmpty;
            if (isToAgeNotEmpty == 0) {
                isToAgeNotEmpty += that.ValidateNumber($(that.Settings.Controls.txtToAgeSelector).val(), that.Settings.Controls.txtToAgeSelector_Error, that.Settings.ResourceText.WrongNumber) == true ? 0 : 1;
            }
            emptyFieldCount += isToAgeNotEmpty;
            if (isFromAgeNotEmpty == 0 && isToAgeNotEmpty == 0) {
                var fromAge = parseInt($(that.Settings.Controls.txtFromAgeSelector).val());
                var toAge = parseInt($(that.Settings.Controls.txtToAgeSelector).val());
                if (fromAge > toAge) {
                    $(that.Settings.Controls.txtToAgeSelector_Error).html(that.Settings.ResourceText.LanguageSkillGrid_ErrorMessage_FromAge_LessThan_ToAge);
                    $(that.Settings.Controls.txtToAgeSelector_Error).show();
                    emptyFieldCount += 1;
                }
            }
            if (isValidDate == 0) {
                var availableDate = $(that.Settings.Controls.dtAvailableTimeSelector).val();
                var dateObj = Functions.parseVietNameseDate(availableDate);
                if (dateObj < that.Settings.Today) {
                    $(that.Settings.Controls.dtAvailableTimeSelector_Error).html(that.Settings.ResourceText.LanguageSkillGrid_AvailabelDateWrong);
                    $(that.Settings.Controls.dtAvailableTimeSelector_Error).show();
                    emptyFieldCount += 1;
                }
                else {
                    $(that.Settings.Controls.dtAvailableTimeSelector_Error).hide();
                }
            }

            var isValidTemplateName = that.IsValidTemplateName();
            if (isValidTemplateName == false) {
                emptyFieldCount += 1;
            }

            return emptyFieldCount == 0;
        },

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
        SetAttributeValue: function (control, value) {
            var that = this;
            var attributeName = that.Settings.HiddenAttributeName;
            if (control) {
                if (value && value != "") {
                    $(control).attr(attributeName, value);
                }
                else {
                    $(control).attr(attributeName, "");
                }
            }
        },
        SetAttributeValueForCheckBox: function (chkBoxControlId) {
            var that = this;
            var attributeName = that.Settings.HiddenAttributeName;
            if (chkBoxControlId) {
                var dataArray = [];
                var checkedOptions = $(chkBoxControlId + " input[type='checkbox']:checked");

                if (checkedOptions && checkedOptions.length > 0) {
                    for (var i = 0; i < checkedOptions.length; i++) {
                        var optionValue = $(checkedOptions[i]).val();
                        dataArray.push(optionValue);
                    }
                }

                $(chkBoxControlId).attr(attributeName, JSON.stringify(dataArray));
            }
        },
        //return json string
        GetAttributeValueForCheckBox: function (chkBoxControlId) {
            var that = this;
            var attributeName = that.Settings.HiddenAttributeName;
            var dataArray = [];
            if (chkBoxControlId) {
                var checkedOptions = $(chkBoxControlId + " input[type='checkbox']:checked");
                if (checkedOptions && checkedOptions.length > 0) {
                    for (var i = 0; i < checkedOptions.length; i++) {
                        var optionValue = $(checkedOptions[i]).val();
                        dataArray.push(optionValue);
                    }
                }
            }
            return JSON.stringify(dataArray);
        },
        GetPureArray: function () {
            var that = this;
            var gridValueArray = [];
            for (var i = 0; i < that.Settings.GridJsonArray.length; i++) {
                var item = {};
                item.ForeignLanguage = that.Settings.GridJsonArray[i].ForeignLanguage;
                item.Level = that.Settings.GridJsonArray[i].Level;
                gridValueArray.push(item);
            }
            return gridValueArray;
        },
        //xét giá trị có thay đổi khi resubmit
        SetCurrentValueForControls: function () {
            var that = this;
            that.SetAttributeValue(that.Settings.Controls.txtPositionSelector, $(that.Settings.Controls.txtPositionSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtQuantitySelector, $(that.Settings.Controls.txtQuantitySelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtReasonForRecruitmentSelector, $(that.Settings.Controls.txtReasonForRecruitmentSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtFromAgeSelector, $(that.Settings.Controls.txtFromAgeSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtToAgeSelector, $(that.Settings.Controls.txtToAgeSelector).val());
            that.SetAttributeValue(that.Settings.Controls.dtAvailableTimeSelector, $(that.Settings.Controls.dtAvailableTimeSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtSpecialitiesSelector, $(that.Settings.Controls.txtSpecialitiesSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtDescriptionOfBasicWorkSelector, $(that.Settings.Controls.txtDescriptionOfBasicWorkSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtMoralVocationsSelector, $(that.Settings.Controls.txtMoralVocationsSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtWorkingAbilitiesSelector, $(that.Settings.Controls.txtWorkingAbilitiesSelector).val());
            //that.SetAttributeValue(that.Settings.Controls.txtCommentSelector, $(that.Settings.Controls.txtCommentSelector).val());
            that.SetAttributeValue(that.Settings.Controls.hdlanguageSkillsSelector, JSON.stringify(that.GetPureArray()));
            that.SetAttributeValue(that.Settings.Controls.txtTemplateNameSelector, $(that.Settings.Controls.txtTemplateNameSelector).val());

            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblEducationLevelSelector);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblAppearanceSelector);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblWorkingExperience);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblMartialStatusSelector);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblComputerSkillsSelector);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblOtherSkillsSelector);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblGenderSelector);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblWorkingTimeSelector);
            that.SetAttributeValueForCheckBox(that.Settings.Controls.cblOtherRequirementSelector);
        },
        IsFormValueChanged: function () {
            var that = this;
            var attributeName = "current-value";
            var changedCount = 0;
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtPositionSelector).attr(attributeName), $(that.Settings.Controls.txtPositionSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtQuantitySelector).attr(attributeName), $(that.Settings.Controls.txtQuantitySelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtReasonForRecruitmentSelector).attr(attributeName), $(that.Settings.Controls.txtReasonForRecruitmentSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtFromAgeSelector).attr(attributeName), $(that.Settings.Controls.txtFromAgeSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtToAgeSelector).attr(attributeName), $(that.Settings.Controls.txtToAgeSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.dtAvailableTimeSelector).attr(attributeName), $(that.Settings.Controls.dtAvailableTimeSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtSpecialitiesSelector).attr(attributeName), $(that.Settings.Controls.txtSpecialitiesSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtDescriptionOfBasicWorkSelector).attr(attributeName), $(that.Settings.Controls.txtDescriptionOfBasicWorkSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtMoralVocationsSelector).attr(attributeName), $(that.Settings.Controls.txtMoralVocationsSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtWorkingAbilitiesSelector).attr(attributeName), $(that.Settings.Controls.txtWorkingAbilitiesSelector).val());
            //changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtCommentSelector).attr(attributeName), $(that.Settings.Controls.txtCommentSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.hdlanguageSkillsSelector).attr(attributeName), JSON.stringify(that.GetPureArray()));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtTemplateNameSelector).attr(attributeName), $(that.Settings.Controls.txtTemplateNameSelector).val());

            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblEducationLevelSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblEducationLevelSelector));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblAppearanceSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblAppearanceSelector));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblWorkingExperience).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblWorkingExperience));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblMartialStatusSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblMartialStatusSelector));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblComputerSkillsSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblComputerSkillsSelector));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblOtherSkillsSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblOtherSkillsSelector));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblGenderSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblGenderSelector));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblWorkingTimeSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblWorkingTimeSelector));
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.cblOtherRequirementSelector).attr(attributeName), that.GetAttributeValueForCheckBox(that.Settings.Controls.cblOtherRequirementSelector));

            if (changedCount > 0) {
                return true;
            }
            else {
                alert(that.Settings.ResourceText.DataMustBeDiffrent);
                return false;
            }
        },
        //If change : return 1
        CheckIfValueChange: function (oldVal, newVal) {
            if (oldVal != undefined && newVal != undefined && oldVal != newVal) {
                return 1;
            }
            return 0;
        },
        ValidateNumber: function (controlValue, errorControl, message) {
            var that = this;
            if ($.isNumeric(controlValue) == false) {
                $(errorControl).html(message);
                $(errorControl).show();
                return false;
            }
            else {
                if (parseInt(controlValue) <= 0) {
                    $(errorControl).html(message);
                    $(errorControl).show();
                    return false;
                }
                $(errorControl).hide();
                return true;
            }
        },
        ConfirmDeleteTemplate: function () {
            var that = this;
            var res = confirm(that.Settings.ResourceText.DeleteTemplateConfirmationMessage);
            return res;
        },
        CheckToEnableOrDisableInputTemplateName: function () {
            var that = this;

            var noneTemplateValue = $(that.Settings.Controls.hdNoneTemplateValueSelector).val();
            var selectedTemplateId = $(that.Settings.Controls.ddlTemplateSelector).find('option:selected').val();
            var isSavingTemplate = $(that.Settings.Controls.cbSavingTemplateSelector).is(':checked');
            if (isSavingTemplate && (selectedTemplateId == noneTemplateValue)) {
                $(that.Settings.Controls.txtTemplateNameSelector).removeAttr('disabled');
            } else {
                $(that.Settings.Controls.txtTemplateNameSelector).attr("disabled", "disabled");
                $(that.Settings.Controls.txtTemplateNameSelector).val('');
            }
        },
        IsValidTemplateName: function () {
            var that = this;

            // Assumption that it's valid.
            var isValidTemplateName = true;
            var errorMessage = '';

            // Checking validation
            if ($(that.Settings.Controls.cbSavingTemplateSelector).length > 0) {
                var isSavingTemplate = $(that.Settings.Controls.cbSavingTemplateSelector).is(':checked');
                if (isSavingTemplate) {
                    var noneTemplateValue = $(that.Settings.Controls.hdNoneTemplateValueSelector).val();
                    var selectedTemplateId = $(that.Settings.Controls.ddlTemplateSelector).find('option:selected').val();
                    if (noneTemplateValue == selectedTemplateId) {
                        var templateName = $(that.Settings.Controls.txtTemplateNameSelector).val().trim();
                        if (templateName == '') {
                            isValidTemplateName = false;
                            errorMessage = that.Settings.ResourceText.EmptyTemplateNameMessage;
                        } else {
                            var isTemplateNameExisted = false;
                            $.each($(that.Settings.Controls.ddlTemplateSelector).find('option'), function (idx, element) {
                                if (isTemplateNameExisted == false) {
                                    if ($(element).html() == templateName) {
                                        isTemplateNameExisted = true;
                                    }
                                }
                            });

                            if (isTemplateNameExisted == true) {
                                isValidTemplateName = false;
                                errorMessage = that.Settings.ResourceText.ExistedTemplateNameMessage;
                            }
                        }
                    }
                }

                if (isValidTemplateName == false) {
                    $(that.Settings.Controls.txtTemplateNameSelector_Error).html(errorMessage);
                    $(that.Settings.Controls.txtTemplateNameSelector_Error).show();
                } else {
                    $(that.Settings.Controls.txtTemplateNameSelector_Error).html('');
                    $(that.Settings.Controls.txtTemplateNameSelector_Error).hide();
                }
            }

            return isValidTemplateName;
        }
    }