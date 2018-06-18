RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");

function ValidateBeforeSaveAndSubmit() {
    var isFormValid = requestForDiplomaSupplyFormInstance.ValidateForm();
    var isSuppotingDocumentValid = supportingDocumentInstance.ValidateAttachments();
    return isFormValid == true && isSuppotingDocumentValid == true;
}

RBVH.Stada.WebPages.pages.RequestForDiplomaSupplyForm = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        Controls:
        {
            InputFileContainer: "#ctl00_PlaceHolderMain_RequestForDiplomaSuppliesForm_SupportingDocumentControl_GridSupportingDocument"
        },
        EmployeeID: "",
        FullName: "",
        LocationId: 0,
        DepartmentId: 0,
        ID: 0,
        Grid:
        {
            CustomFields: {
                CurrentDiplomaField: null,
                GraduationYearField: null,
                NewDiplomaField: null,
                FacultyField: null,
                IssuedPlaceField: null,
                TrainingDurationField: null,
                RequestField: null,
            },
        },

        Fields: [],
        Data: [],
        GridJsonArray: [],
        IsFormEditable: false,
    }
    $.extend(true, this.Settings, settings);
    this.Initialize();
};

RBVH.Stada.WebPages.pages.RequestForDiplomaSupplyForm.prototype =
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
            var isEditableValue = $(that.Settings.Controls.hdIsEditableSelector).val();
            that.Settings.IsFormEditable = (isEditableValue == "True");
            that.LoadEmployeeInformation();
            that.PopulateGrid();

            if ($(that.Settings.Controls.dtFromDateSelector) && $(that.Settings.Controls.dtFromDateSelector).attr('disabled') == 'disabled') {
                $(that.Settings.Controls.dtFromDateSelector).parent().next().hide();
            }
            $(that.Settings.Controls.dtFromDateSelector).keydown(function (e) { e.preventDefault(); return false; });
            if (that.Settings.IsFormEditable == true) {
                $(that.Settings.Controls.ddlEmployeeSelector).select2();
            }
            that.SetCurrentValueForControls();

            // Show/hide Approval Status
            var approvalStatusValue = $(that.Settings.Controls.ApprovalStatusValueSelector).val();
            if (!!approvalStatusValue) {
                $(that.Settings.Controls.ApprovalStatusTdSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(approvalStatusValue));
                $(that.Settings.Controls.ApprovalStatusTrSelector).show();
            }
        },
        //Hàm này dùng để set value vào attribute "current-value" của các control cần 
        //xét giá trị có thay đổi khi resubmit
        SetCurrentValueForControls: function () {
            var that = this;
            that.SetAttributeValue(that.Settings.Controls.ddlEmployeeSelector, $(that.Settings.Controls.ddlEmployeeSelector + " option:selected").val());
            that.SetAttributeValue(that.Settings.Controls.txtPositionSelector, $(that.Settings.Controls.txtPositionSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtToTheDailyWorksSelector, $(that.Settings.Controls.txtToTheDailyWorksSelector).val());
            that.SetAttributeValue(that.Settings.Controls.txtNewSuggestionsSelector, $(that.Settings.Controls.txtNewSuggestionsSelector).val());
            that.SetAttributeValue(that.Settings.Controls.CommentSelector, $(that.Settings.Controls.CommentSelector).val());
            that.SetAttributeValue(that.Settings.Controls.InputFileContainer, $(that.Settings.Controls.InputFileContainer + " input[type='file']").length);

            that.SetAttributeValue(that.Settings.Controls.hdGridDetailsValueSelector, JSON.stringify(that.GetPureArray()));
        },
        GetPureArray: function () {
            var that = this;
            var gridValueArray = [];
            for (var i = 0; i < that.Settings.GridJsonArray.length; i++) {
                var item = {};
                item.CurrentDiploma = that.Settings.GridJsonArray[i].CurrentDiploma;
                item.GraduationYear = that.Settings.GridJsonArray[i].GraduationYear;
                item.NewDiploma = that.Settings.GridJsonArray[i].NewDiploma;
                item.Faculty = that.Settings.GridJsonArray[i].Faculty;
                item.IssuedPlace = that.Settings.GridJsonArray[i].IssuedPlace;
                item.IssuedPlace = that.Settings.GridJsonArray[i].IssuedPlace;
                gridValueArray.push(item);
            }
            return gridValueArray;
        },
        SetAttributeValue: function (control, value) {
            var attributeName = "current-value";
            if (control) {
                if (value && value != "") {
                    $(control).attr(attributeName, value);
                }
                else {
                    $(control).attr(attributeName, "");
                }
            }
        },
        EventsRegister: function () {
            var that = this;
            $(that.Settings.Controls.ddlEmployeeSelector).change(function () {
                that.LoadEmployeeInformation();
            });
        },
        LoadEmployeeInformation: function () {
            var that = this;
            var selectedOptionValue = $(that.Settings.Controls.ddlEmployeeSelector + " option:selected").val();
            if (selectedOptionValue) {
                if (selectedOptionValue == "0") {
                    $(that.Settings.Controls.txtEmployeeCodeSelector).val("");
                    $(that.Settings.Controls.lblDateOfEmpSelector).text("");
                }
                else {
                    var $selectedOption = $(that.Settings.Controls.ddlEmployeeSelector + " option[value='" + selectedOptionValue + "']");
                    if ($selectedOption) {
                        $(that.Settings.Controls.txtEmployeeCodeSelector).val($selectedOption.attr("employeecode"));
                        $(that.Settings.Controls.lblDateOfEmpSelector).text($selectedOption.attr("dateofemp"));
                    }
                }
            }
        },
        DisbleDeleteButtonOnGrid: function () {
            $(".jsgrid-button.jsgrid-delete-button").prop("disabled", true);
            $(".jsgrid-button.jsgrid-delete-button").css("opacity", 0.1);
        },
        PopulateGrid: function (control) {
            var that = this;
            that.BindGridColumns();
            $(that.Settings.Controls.GridDetailsSelector).jsGrid({
                width: "100%",
                height: "170px",
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
                        var jsonString = $(that.Settings.Controls.hdRequestDiplomaDetailsSelector).val();
                        if (jsonString != "") {
                            var jsonObject = JSON.parse(jsonString);
                            that.Settings.GridJsonArray = jsonObject;
                        }
                        d.resolve(that.Settings.GridJsonArray);

                        return d.promise();
                    },
                    insertItem: function (item) {
                    },
                    updateItem: function (item) {
                        //that.Settings.EmployeeListEdit.push(item);
                        //that.ToggleSaveButton();
                    },
                    deleteItem: function (item) {
                    }
                },
                onItemInserting: function (args) {
                    if (this.ValiateGridItem(args) == false) {
                        args.cancel = true;
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
                    if (this.ValiateGridItem(args) == false) {
                        args.cancel = true;
                    }
                },
                ValiateGridItem: function (args) {
                    var errorFiledEmptyCount = 0;
                    //TFS: ID- 1614 - Không required bằng cấp hiện tại và năm tốt nghiệp
                    //errorFiledEmptyCount += that.ValiateEmptyField(args.item.CurrentDiploma);
                    //errorFiledEmptyCount += that.ValiateEmptyField(args.item.GraduationYear);
                    errorFiledEmptyCount += that.ValiateEmptyField(args.item.NewDiploma);
                    errorFiledEmptyCount += that.ValiateEmptyField(args.item.Faculty);
                    errorFiledEmptyCount += that.ValiateEmptyField(args.item.IssuedPlace);
                    errorFiledEmptyCount += that.ValiateEmptyField(args.item.TrainingDuration);
                    if (errorFiledEmptyCount > 0) {
                        alert(that.Settings.ResourceText.ErrorMessage_PleaseInputData);
                    }
                    return errorFiledEmptyCount == 0;
                },

                onItemDeleted: function (args) {
                    that.RebindHiddenFields(that.Settings.GridJsonArray);
                },
                onItemEditCancelling: function (args) {
                },
                fields:
                that.Settings.Grid.Fields,
            });
        },

        RebindHiddenFields: function (data) {
            var that = this;
            if (data) {
                $(that.Settings.Controls.hdRequestDiplomaDetailsSelector).val(JSON.stringify(data));
            }
        },
        ValiateEmptyField: function (data) {
            if (!data || data == "") {
                return 1;
            }
            return 0;
        },
        AppendInputFile: function () {
            var that = this;
            var countNumer = that.Settings.DocumentCount++;
            $(that.Settings.Controls.DocumentFilesDivSelector).append("<input type='file' name='supportingDocument" + countNumer + "' style='display:inline'>  <span spanName='supportingDocument" + countNumer + "' class='glyphicon glyphicon-trash span-remove'></span>  <span spanErrorName='supportingDocument" + countNumer + "' class='ms-formvalidation ms-csrformvalidation'></span><br/>")
        },
        // Validation
        ValidateForm: function () {
            var that = this;
            var isValid1 = that.ValidateRequiredField();
            var isValid2 = that.ValiateEmployee();
            var isValid3 = that.ValidateInputDate();
            var isValid4 = this.ValidateDetailGrid();
            var isValid5 = (isValid1 == true && isValid2 == true && isValid3 == true && isValid4 == true);

            var isValid = false;
            if (isValid5 == true) {
                isValid = that.IsFormValueChanged();
            }
            return isValid;
        },
        ValidateRequiredField: function () {
            $("span[class='ms-formvalidation ms-csrformvalidation']").html("");
            var that = this;
            var emptyFieldCount = 0;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.txtPositionSelector).val(), that.Settings.Controls.txtPositionSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.txtToTheDailyWorksSelector).val(), that.Settings.Controls.txtToTheDailyWorksSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.txtNewSuggestionsSelector).val(), that.Settings.Controls.txtNewSuggestionsSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) == true ? 0 : 1;

            return emptyFieldCount == 0;
        },
        ValiateEmployee: function () {
            var that = this;
            var employeeFullNameSelectedValue = $(that.Settings.Controls.ddlEmployeeSelector + " option:selected").val();
            if (!employeeFullNameSelectedValue || employeeFullNameSelectedValue == "0") {
                $(that.Settings.Controls.ddlFullNameSelector_Error).html(that.Settings.ResourceText.PleaseSelectEmp);
                return false;
            }
            else {
                $(that.Settings.Controls.ddlFullNameSelector_Error).html("");
                return true;
            }
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
        ValidateInputDate: function () {
            var that = this;
            var now = new Date();
            var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            var fromDate = $(that.Settings.Controls.dtFromDateSelector).val();
            if (fromDate && fromDate != "") {
                var dateObj = Functions.parseVietNameseDate(fromDate);
                if (dateObj < currentDate) {
                    $(that.Settings.Controls.dtFromDateSelector_Error).html(that.Settings.ResourceText.ErrorMessage_WrongFromDate);
                    $(that.Settings.Controls.dtFromDateSelector_Error).show();
                    return false;
                }
                else {
                    $(that.Settings.Controls.dtFromDateSelector_Error).hide();
                    return true;
                }
            }
            else {
                return true;
            }
        },
        ValidateDetailGrid: function () {
            var that = this;
            if (!that.Settings.GridJsonArray || that.Settings.GridJsonArray.length == 0) {
                alert(that.Settings.ResourceText.ErrorMessage_PleaseInputDetailGrid);
                return false;
            }
            return true;
        },
        //End-Validation
        BindGridColumns: function () {
            var that = this;
            that.Settings.Grid.Fields = [
                { name: "CurrentDiploma", title: that.Settings.ResourceText.DetailGrid_CurrentDiploma, width: 100, type: "text", align: "left" },
                { name: "GraduationYear", title: that.Settings.ResourceText.DetailGrid_GraduationYear, width: 40, align: "left", type: "text" },
                { name: "NewDiploma", title: that.Settings.ResourceText.DetailGrid_NewDiploma, width: 100, align: "left", type: "text" },
                { name: "Faculty", title: that.Settings.ResourceText.DetailGrid_Faculty, width: 80, align: "left", type: "text" },
                { name: "IssuedPlace", title: that.Settings.ResourceText.DetailGrid_IssuedPlace, width: 60, align: "left", type: "text" },
                { name: "TrainingDuration", title: that.Settings.ResourceText.DetailGrid_TrainingDuration, width: 80, align: "left", type: "text" },
                { type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 30, modeSwitchButton: false }
            ];
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
        //Kiểm tra coi giá trị của form có thay đổi khi resubmit => nếu có thì cho submit còn không thay đổi thì không cho submit
        IsFormValueChanged: function () {
            var that = this;
            var attributeName = "current-value";
            var changedCount = 0;
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.ddlEmployeeSelector).attr(attributeName), $(that.Settings.Controls.ddlEmployeeSelector + " option:selected").val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtPositionSelector).attr(attributeName), $(that.Settings.Controls.txtPositionSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtToTheDailyWorksSelector).attr(attributeName), $(that.Settings.Controls.txtToTheDailyWorksSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.txtNewSuggestionsSelector).attr(attributeName), $(that.Settings.Controls.txtNewSuggestionsSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.CommentSelector).attr(attributeName), $(that.Settings.Controls.CommentSelector).val());
            changedCount += that.CheckIfValueChange($(that.Settings.Controls.hdGridDetailsValueSelector).attr(attributeName), JSON.stringify(that.GetPureArray()));
            if ($(that.Settings.Controls.InputFileContainer + " input[type='file']").length != $(that.Settings.Controls.InputFileContainer).attr(attributeName)) {
                changedCount++;
            }
            if (changedCount == 0) {
                alert(that.Settings.ResourceText.DataMustBeDiffrent);
                return false;
            }
            else {
                return true;
            }
        },
        //If change : return 1
        CheckIfValueChange: function (oldVal, newVal) {
            if (oldVal && newVal && oldVal != newVal) {
                return 1;
            }
            return 0;
        }
    }