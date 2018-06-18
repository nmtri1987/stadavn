RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
$.fn.showOption = function () {
    this.each(function () {
        if (this.tagName == "OPTION") {
            var opt = this;
            if ($(this).parent().get(0).tagName == "SPAN") {
                var span = $(this).parent().get(0);
                $(span).replaceWith(opt);
                $(span).remove();
            }
            opt.disabled = false;
            $(opt).show();
        }
    });
    return this;
}
$.fn.hideOption = function () {
    this.each(function () {
        if (this.tagName == "OPTION") {
            var opt = this;
            if ($(this).parent().get(0).tagName == "SPAN") {
                var span = $(this).parent().get(0);
                $(span).hide();
            } else {
                $(opt).wrap("span").hide();
            }
            opt.disabled = true;
        }
    });
    return this;
}
//Bug 1876 
function validateBeforeApprove() {
    return requestFormInstance.ValidateBeforeApprove();
}

function ValidateBeforeSaveAndSubmit() {
    var isFormValid = false;
    valid1 = requestFormInstance.ValidateForm();
    if (valid1 == true) {
        valid2 = requestFormInstance.ValidateDataDifferent();
        valid3 = supportingDocumentInstance.ValidateAttachments();
        isFormValid = (valid1 == true && valid2 == true && valid3 == true);
        return isFormValid;
    }
    return false;
}
RBVH.Stada.WebPages.pages.RequestForm = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        Controls:
        {
            FileContainer: "#ctl00_PlaceHolderMain_RequestFormUserControl_SupportingDocumentControl_GridSupportingDocument",
        },
        EmployeeID: "",
        FullName: "",
        LocationId: 0,
        DepartmentId: 0,
        ID: 0,
        EmployeeLevel: 0,
        CurrentRequestType: "RequestBuyDetails",
        CurrentActiveDetailDivId: "details1",
        Grid1:
        {
            CustomFields: {
                NoField: null,
                ContentField: null,
                FormField: null,
                UnitField: null,
                QuantityField: null,
                ReasonField: null,
            },
        },
        Grid2:
        {
            CustomFields: {
                NoField: null,
                ContentField: null,
                FromField: null,
                ToField: null,
                PlaceField: null,
                ReasonField: null,
            },
        },
        Grid3:
        {
            CustomFields: {
                NoField: null,
                ContentField: null,
                FormField: null,
                ToField: null,
                PlaceField: null,
                ReasonField: null,
            },
        },
        Fields: [],
        Data: [],
        Grid1JsonArray: [],
        Grid2JsonArray: [],
        Grid3JsonArray: [],
        OldGrid1JsonArray: [],
        OldGrid2JsonArray: [],
        OldGrid3JsonArray: [],
        OldTitle: '',
        OldFileCount: '',
        OldRequestType: '',
        OldRequestReceivedByDepartmentId: 0,
        OldFinishDate: '',
        OldComment: '',
        OldReferTo: 0,
        IsViewMode: false,
        IsEditMode: false,
        IsFormEditable: false,
        FormModeParam: "new",
        DocumentCount: 0,
    }
    $.extend(true, this.Settings, settings);
    this.Initialize();
};

RBVH.Stada.WebPages.pages.RequestForm.prototype =
    {
        Initialize: function () {
            var that = this;
            $(document).ready(function () {
                ExecuteOrDelayUntilScriptLoaded(function () {
                    that.InitControls();
                    that.PopulateData();
                    that.EventsRegister();
                }, "sp.js");
            });
        },

        InitControls: function () {

            var that = this;

            var isEditableValue = $(that.Settings.Controls.hdIsEditableSelector).val();

            that.Settings.IsFormEditable = (isEditableValue == "True");

            var currentActiveType = $(that.Settings.Controls.hdRequestTypeSelector).val();
            if (currentActiveType != "") {
                switch (currentActiveType) {
                    case "1":
                        that.Settings.CurrentActiveDetailDivId = "details1";
                        that.Settings.CurrentRequestType = "RequestBuyDetails";
                        break;
                    case "2":
                        that.Settings.CurrentActiveDetailDivId = "details2";
                        that.Settings.CurrentRequestType = "RequestRepairDetails";
                        break;
                    case "3":
                        that.Settings.CurrentActiveDetailDivId = "details3";
                        that.Settings.CurrentRequestType = "RequestOtherDetails";
                        break;
                }
            }

            var formMode = Functions.getParameterByName("mode");
            that.Settings.FormModeParam = formMode;
            // that.DisableButtonControls();

            $(that.Settings.Controls.FinishDateSelector).keydown(function (e) { e.preventDefault(); return false; });
            $(that.Settings.Controls.DueDateSelector).keydown(function (e) { e.preventDefault(); return false; });
            that.ShowDetailGrid();

            if ($(that.Settings.Controls.FinishDateSelector).attr('disabled') == 'disabled') {
                $(that.Settings.Controls.FinishDateSelector).parent().next().hide();
            }
            if ($(that.Settings.Controls.DueDateSelector).attr('disabled') == 'disabled') {
                $(that.Settings.Controls.DueDateSelector).parent().next().hide();
            }

            //---- BEGIN Duc.VoTan Added 2017.08.11. Those functions are moved from user control.
            that.CheckToShowReceivedBy();
            that.FilterDepartmentAccordingToRequestType();
            that.CheckToShowFinishDate();
            that.CheckToShowRequiredApprovalByBOD();
            that.SetUrlReferTo();
            that.CheckToShowDueDate();
            that.ShowErrorMessage();
            //---- END Duc.VoTan Added 2017.08.11. Those functions are moved from user control.

            if (that.Settings.IsFormEditable == true) {
                that.Settings.OldFileCount = $(that.Settings.FileContainer + " input[type='file']").length;
                that.Settings.OldTitle = $(that.Settings.Controls.txtTitleSelector).val();
                that.Settings.OldRequestType = that.Settings.CurrentRequestType;
                that.Settings.OldRequestReceivedByDepartmentId = $(that.Settings.Controls.ddlReceivedBySelector).val();
                that.Settings.OldFinishDate = $(that.Settings.Controls.FinishDateSelector).val();
                that.Settings.OldComment = $(that.Settings.Controls.CommentSelector).val();

                that.Settings.OldReferTo = $(that.Settings.Controls.ddlReferToSelector).val();
            }

            // DucVT ADD. 2017.10.03. Fix bug After select [Refer to], user can not scroll vertical scroll bar.
            $(that.Settings.Controls.ddlReferToSelector).select2();

            // Show/hide Approval Status
            var approvalStatusValue = $(that.Settings.Controls.ApprovalStatusValueSelector).val();
            if (!!approvalStatusValue) {
                $(that.Settings.Controls.ApprovalStatusTdSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(approvalStatusValue));
                $(that.Settings.Controls.ApprovalStatusTrSelector).show();
            }

            // Show/hide Approve button:
            // https://rbvhsharepointprojects.visualstudio.com/STADA/_workitems/edit/1968
            if ($("input[id*='btnPrint']").is(':visible')) {
                if ($("input[id*='btnApprove']").is(':visible')) {
                    var printClicked = $(that.Settings.Controls.hdPrintCounter).val() == '1';
                    if (!printClicked)
                        $("input[id*='btnApprove']").hide();
                }
            }
        },
        DisableButtonControls: function () {
            var that = this;
            $(that.Settings.Controls.btnSaveDraftSelector).prop('disabled', true);
            $(that.Settings.Controls.btnSaveAndSubmitSelector).prop('disabled', true);
        },

        EnableButtonControls: function () {
            var that = this;
            $(that.Settings.Controls.btnSaveDraftSelector).removeAttr('disabled');
            $(that.Settings.Controls.btnSaveAndSubmitSelector).removeAttr('disabled');
        },

        PopulateData: function () {
            var that = this;
            // that.PopulateCurrentUserInfo();
            that.PopulateGrid();
        },
        EventsRegister: function () {
            var that = this;

            $("input[type='radio'][name='" + that.Settings.Controls.RadioButtonsNameSelector + "']").click(function () {
                $(that.Settings.Controls.hdRequestTypeSelector).val($(this).val());

                that.FilterDepartmentAccordingToRequestType();
                //---- BEGIN Duc.VoTan Added 2017.08.11. Those functions are moved from user control.

                try {
                    var hdRequestBuyTypeId = that.Settings.Controls.hdRequestBuyTypeIdSelector;
                    var hdRequestRepairTypeId = that.Settings.Controls.hdRequestRepairTypeIdSelector;
                    var hdRequestOtherTypeId = that.Settings.Controls.hdRequestOtherTypeIdSelector;
                    var trReceivedById = that.Settings.Controls.trReceivedBySelector;
                    var trRequireBODdApproveClientId = that.Settings.Controls.trRequireBODdApproveSelector;
                    var trFinishDateClientId = that.Settings.Controls.trFinishDateSelector;
                    var hdRequestTypeClientId = that.Settings.Controls.hdRequestTypeSelector;
                    var requestTypeValue = $(this).val();
                    $(hdRequestTypeClientId).val(requestTypeValue);
                    if (requestTypeValue == $(hdRequestRepairTypeId).val() || requestTypeValue == $(hdRequestOtherTypeId).val()) {
                        $(trReceivedById).show();
                        $(trFinishDateClientId).show();
                        var hdIsEmployeeDEHValue = $(that.Settings.Controls.hdIsEmployeeDEHSelector).val();
                        if (hdIsEmployeeDEHValue == '1') {
                            $(trRequireBODdApproveClientId).show();
                        } else {
                            $(trRequireBODdApproveClientId).hide();
                        }
                    } else {
                        $(trReceivedById).hide();
                        $(trFinishDateClientId).hide();
                        $(trRequireBODdApproveClientId).hide();
                    }
                } catch (err) { }

                //---- END Duc.VoTan Added 2017.08.11. Those functions are moved from user control.

                // show gird details
                var divId = 'details' + $(this).val();
                switch ("#" + divId) {
                    case that.Settings.Controls.GridDetail1ControlSelector:
                        that.Settings.CurrentRequestType = "RequestBuyDetails";
                        break;
                    case that.Settings.Controls.GridDetail2ControlSelector:
                        that.Settings.CurrentRequestType = "RequestRepairDetails";
                        break;
                    case that.Settings.Controls.GridDetail3ControlSelector:
                        that.Settings.CurrentRequestType = "RequestOtherDetails";
                        break;
                }
                that.Settings.CurrentActiveDetailDivId = divId;
                that.ShowDetailGrid();
            });

            //---- BEGIN Duc.VoTan Added 2017.08.11. Those functions are moved from user control.

            $(that.Settings.Controls.ddlReferToSelector).change(function () {
                that.SetUrlReferTo();
            });

            $(that.Settings.Controls.linkReferToSelector).click(function () {
                var url = $(this).attr('url');
                if (url != '') {
                    var title = $(this).attr('title');
                    openModalDialog(title, url, null, SP.UI.DialogResult.cancel);
                }
            });

            // Disable 'ENTER' submit
            //$(window).keydown(function (event) {
            $("input[type=submit]").keydown(function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                    return false;
                }
            });

            // Allow Approve after clicking on 'Print' button
            // https://rbvhsharepointprojects.visualstudio.com/STADA/_workitems/edit/1968
            $("input[id*='btnPrint']").on('click', function () {
                $(that.Settings.Controls.hdPrintCounter).val('1');

                $("input[id*='btnApprove']").show();
            });

        },
        ShowDetailGrid: function () {
            var that = this;
            var currentDetails = $('#' + that.Settings.CurrentActiveDetailDivId);
            $(currentDetails).siblings().hide();
            $(currentDetails).show();
            that.PopulateGrid();
        },
        PopulateGrid: function () {
            var that = this;

            var currentActiveDiv = that.Settings.CurrentActiveDetailDivId;
            switch ("#" + currentActiveDiv) {
                case that.Settings.Controls.GridDetail1ControlSelector:
                    that.PopulateRequestBuyDetailsGrid(currentActiveDiv);
                    break;
                case that.Settings.Controls.GridDetail2ControlSelector:
                    that.PopulateRequestRepairDetailsGrid(currentActiveDiv);
                    break;
                case that.Settings.Controls.GridDetail3ControlSelector:
                    that.PopulateRequestOtherGrid(currentActiveDiv);
                    break;
            }
        },
        //Bug 1876
        ValidateBeforeApprove: function () {
            var that = this;
            var errorCount = 0;
            var now = new Date();
            var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            if ($(that.Settings.Controls.hdIsShowDueDateSelector).val() == '1') {
                var dueDateValueString = $(that.Settings.Controls.DueDateSelector).val();
                if (!dueDateValueString || dueDateValueString == "" || dueDateValueString.trim() == "") {
                    $(that.Settings.Controls.DueDateSelector_Error).html(that.Settings.Messages.CantLeaveTheBlank);
                    errorCount += 1;
                }
                else {
                    var dueDateObj = Functions.parseVietNameseDate(dueDateValueString);
                    if (dueDateObj < currentDate) {
                        $(that.Settings.Controls.DueDateSelector_Error).html(that.Settings.Messages.SelectedDateMustGreaterThanCurrentDate);
                        errorCount += 1;
                    }
                    else {
                        $(that.Settings.Controls.DueDateSelector_Error).html("");
                    }
                }
            }
            //TFS #1892
            var trReceivedById = that.Settings.Controls.trReceivedBySelector;
            if ($(trReceivedById).is(':visible') && $(that.Settings.Controls.ddlReceivedBySelector + " option:selected").val() == "0") {
                errorCount += 1;
                $(that.Settings.Controls.ReceivedBy_Error).html(that.Settings.Messages.PleaseSelectedReceivedDepartment).show();
            }
            else {
                $(that.Settings.Controls.ReceivedBy_Error).html("").hide();
            }
            //END #1892
            return errorCount == 0;
        },
        ValidateDataDifferent: function () {
            var that = this;
            if (that.Settings.IsFormEditable == false) {
                return true;
            }
            else {
                if (that.ValidateDetailGridDiffrent() == false && that.CheckFieldValueChange() == false) {
                    alert(that.Settings.Messages.DataMustBeDifferentAfterEdit);
                    return false;
                }
                else {
                    return true;
                }
            }
        },
        //true: data has changed
        CheckFieldValueChange: function () {
            var that = this;
            var count = 0;
            count += $(that.Settings.Controls.txtTitleSelector).val() != that.Settings.OldTitle ? 1 : 0;
            count += that.Settings.CurrentRequestType != that.Settings.OldRequestType ? 1 : 0;
            count += $(that.Settings.Controls.ddlReceivedBySelector).val() != that.Settings.OldRequestReceivedByDepartmentId ? 1 : 0;
            count += $(that.Settings.Controls.FinishDateSelector).val() != that.Settings.OldFinishDate ? 1 : 0;
            count += $(that.Settings.Controls.CommentSelector).val() != that.Settings.OldComment ? 1 : 0;
            count += $(that.Settings.Controls.FileContainer + " input[type='file']").length != that.Settings.OldFileCount ? 1 : 0;

            count += $(that.Settings.Controls.ddlReferToSelector).val() != that.Settings.OldReferTo ? 1 : 0;
            return count > 0;
        },
        //Check if user has changed  detail grid
        ValidateDetailGridDiffrent: function () {
            var that = this;
            var oldData = [];
            var newData = [];
            if (that.Settings.IsFormEditable) {
                var requestType = that.Settings.CurrentRequestType;
                if (requestType == "RequestBuyDetails") {
                    for (var i = 0; i < that.Settings.Grid1JsonArray.length; i++) {
                        var item = {};
                        item.Content = that.Settings.Grid1JsonArray[i].Content;
                        item.Form = that.Settings.Grid1JsonArray[i].Form;
                        item.Unit = that.Settings.Grid1JsonArray[i].Unit;
                        item.Reason = that.Settings.Grid1JsonArray[i].Reason;
                        newData.push(item);
                    }
                    var oldDataJsonArray = that.Settings.OldGrid1JsonArray;
                    for (var j = 0; j < oldDataJsonArray.length; j++) {
                        var item = {};
                        item.Content = oldDataJsonArray[j].Content;
                        item.Form = oldDataJsonArray[j].Form;
                        item.Unit = oldDataJsonArray[j].Unit;
                        item.Reason = oldDataJsonArray[j].Reason;
                        oldData.push(item);
                    }
                }
                else if (requestType == "RequestRepairDetails") {
                    for (var i = 0; i < that.Settings.Grid2JsonArray.length; i++) {
                        var item = {};
                        item.Content = that.Settings.Grid2JsonArray[i].Content;
                        item.Place = that.Settings.Grid2JsonArray[i].Place;
                        item.FromDateString = that.Settings.Grid2JsonArray[i].FromDateString;
                        item.ToDateString = that.Settings.Grid2JsonArray[i].ToDateString;
                        item.Reason = that.Settings.Grid2JsonArray[i].Reason;
                        newData.push(item);
                    }
                    var oldDataJsonArray = that.Settings.OldGrid2JsonArray;
                    for (var j = 0; j < oldDataJsonArray.length; j++) {
                        var item = {};
                        item.Content = oldDataJsonArray[j].Content;
                        item.Place = oldDataJsonArray[j].Place;
                        item.FromDateString = oldDataJsonArray[j].FromDateString;
                        item.ToDateString = oldDataJsonArray[j].ToDateString;
                        item.Reason = oldDataJsonArray[j].Reason;
                        oldData.push(item);

                    }
                }
                else if (requestType == "RequestOtherDetails") {
                    for (var i = 0; i < that.Settings.Grid3JsonArray.length; i++) {
                        var item = {};
                        item.Content = that.Settings.Grid3JsonArray[i].Content;
                        item.Quantity = that.Settings.Grid3JsonArray[i].Quantity;
                        item.Unit = that.Settings.Grid3JsonArray[i].Unit;
                        item.Reason = that.Settings.Grid3JsonArray[i].Reason;
                        newData.push(item);
                    }
                    var oldDataJsonArray = that.Settings.OldGrid3JsonArray;
                    for (var j = 0; j < oldDataJsonArray.length; j++) {
                        var item = {};
                        item.Content = oldDataJsonArray[j].Content;
                        item.Quantity = oldDataJsonArray[j].Quantity;
                        item.Unit = oldDataJsonArray[j].Unit;
                        item.Reason = oldDataJsonArray[j].Reason;
                        oldData.push(item);
                    }
                }
                var res = (oldData != [] && newData != [] && JSON.stringify(oldData) === JSON.stringify(newData));
                return !res;
            }
        },
        ValidateForm: function () {
            var that = this;
            var errorCount = 0;
            var now = new Date();
            var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            var finishDate = $(that.Settings.Controls.FinishDateSelector).val();
            var requestType = that.Settings.CurrentRequestType;

            var title = $(that.Settings.Controls.txtTitleSelector).val();
            if (title == "" || !title) {
                $(that.Settings.Controls.TitleSelector_Error).html(that.Settings.Messages.CantLeaveTheBlank);
                errorCount += 1;
            }
            else {
                $(that.Settings.Controls.TitleSelector_Error).html("");
            }

            if (finishDate == "" || !finishDate) {
                if (requestType == "RequestRepairDetails" || requestType == "RequestOtherDetails") {
                    $(that.Settings.Controls.FinishDateSelector_Error).html(that.Settings.Messages.CantLeaveTheBlank);
                    errorCount += 1;
                }
            }
            else {
                var finishDateObj = Functions.parseVietNameseDate(finishDate);
                if (finishDateObj < currentDate) {
                    $(that.Settings.Controls.FinishDateSelector_Error).html(that.Settings.Messages.SelectedDateMustGreaterThanCurrentDate);
                    errorCount += 1;
                }
                else {
                    $(that.Settings.Controls.FinishDateSelector_Error).html("");
                }
            }
            switch (requestType) {
                case "RequestBuyDetails":
                    if (that.Settings.Grid1JsonArray.length == 0) {
                        alert(that.Settings.Messages.PleaseInputGridDetail);
                        errorCount += (that.Settings.Grid1JsonArray.length > 0) ? 0 : 1;
                    }
                    break;
                case "RequestRepairDetails":
                    if (that.Settings.Grid2JsonArray.length == 0) {
                        alert(that.Settings.Messages.PleaseInputGridDetail);
                        errorCount += (that.Settings.Grid2JsonArray.length > 0) ? 0 : 1;
                    }
                    break;
                case "RequestOtherDetails":
                    if (that.Settings.Grid3JsonArray.length == 0) {
                        alert(that.Settings.Messages.PleaseInputGridDetail);
                        errorCount += (that.Settings.Grid3JsonArray.length > 0) ? 0 : 1;
                    }
                    break;
            }

            // 1984: https://rbvhsharepointprojects.visualstudio.com/STADA/_workitems/edit/1984
            var ddlReceivedByClientId = that.Settings.Controls.ddlReceivedBySelector;
            var trReceivedById = that.Settings.Controls.trReceivedBySelector;
            //if ($(ddlReceivedByClientId).attr('disabled') != 'disabled') {
            if ($(trReceivedById).is(':visible')) {
                var selectedReceivedByVal = $(that.Settings.Controls.ddlReceivedBySelector + " option:selected").val();
                if (selectedReceivedByVal == 0) {
                    errorCount += 1;
                    $(that.Settings.Controls.ReceivedBy_Error).html(that.Settings.Messages.PleaseSelectedReceivedDepartment).show();
                }
                else {
                    $(that.Settings.Controls.ReceivedBy_Error).html("").hide();
                }
            }

            return (errorCount == 0);
        },

        //---- CUSTOM BIND FIELD -------------------------------------------------------------------------------------
        BindDateTimeControl: function () {
            var that = this;
            that.Settings.Grid2.CustomFields.FromField = function (config) {
                jsGrid.Field.call(this, config);
            };
            that.Settings.Grid2.CustomFields.ToField = function (config) {
                jsGrid.Field.call(this, config);
            };

            that.Settings.Grid2.CustomFields.FromField.prototype = new jsGrid.Field({
                sorter: function (date1, date2) {
                    return new Date(date1) - new Date(date2);
                },

                itemTemplate: function (value) {
                    //return new Date(value).toDateString();
                    return value;
                },

                insertTemplate: function (value) {
                    return this._insertPicker = $("<input>").datepicker({ defaultDate: new Date(), dateFormat: "dd/mm/yy" });
                },

                editTemplate: function (value) {
                    var dateObj = Functions.parseVietNameseDate(value);
                    return this._editPicker = $("<input>").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", dateObj);
                },

                insertValue: function () {
                    var dateObj = this._insertPicker.datepicker("getDate");
                    if (dateObj)
                        return Functions.parseVietnameseDateTimeToDDMMYYYY2(dateObj);
                    else
                        return "";
                },

                editValue: function () {
                    var dateObj = this._editPicker.datepicker("getDate");
                    //return this._editPicker.datepicker("getDate").toISOString();
                    if (dateObj)
                        return Functions.parseVietnameseDateTimeToDDMMYYYY2(dateObj);
                    else
                        return "";
                }
            });

            that.Settings.Grid2.CustomFields.ToField.prototype = new jsGrid.Field({
                sorter: function (date1, date2) {
                    return new Date(date1) - new Date(date2);
                },

                itemTemplate: function (value) {
                    // return new Date(value).toDateString();
                    return value;
                },

                insertTemplate: function (value) {
                    return this._insertPicker = $("<input>").datepicker({ defaultDate: new Date(), dateFormat: "dd/mm/yy" });
                },

                editTemplate: function (value) {
                    var dateObj = Functions.parseVietNameseDate(value);
                    return this._editPicker = $("<input>").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", dateObj);
                },

                insertValue: function () {
                    var dateObj = this._insertPicker.datepicker("getDate");
                    //return this._insertPicker.datepicker("getDate").toISOString();
                    if (dateObj)
                        return Functions.parseVietnameseDateTimeToDDMMYYYY2(dateObj);
                    else
                        return "";
                },

                editValue: function () {
                    var dateObj = this._editPicker.datepicker("getDate");
                    //return this._editPicker.datepicker("getDate").toISOString();
                    if (dateObj)
                        return Functions.parseVietnameseDateTimeToDDMMYYYY2(dateObj);
                    return "";
                }
            });
        },


        BindOrderNumber: function () {

            var that = this;
            that.Settings.Grid1.CustomFields.NoField = function (config) {
                jsGrid.Field.call(this, config);
            };
            that.Settings.Grid2.CustomFields.NoField = function (config) {
                jsGrid.Field.call(this, config);
            };
            that.Settings.Grid3.CustomFields.NoField = function (config) {
                jsGrid.Field.call(this, config);
            };
            that.Settings.Grid1.CustomFields.NoField.prototype = new jsGrid.Field({
                // itemTemplate: function (value) {
                //     
                //     var $item = value;
                //     var itemCounts = that.Settings.Grid1JsonArray.length;
                //     return 1;
                // },
                insertTemplate: function (value) {
                },

                editTemplate: function (value) {
                },
                insertValue: function () {
                    var itemCounts = that.Settings.Grid1JsonArray.length;
                    return itemCounts + 1;
                },
                editValue: function () {
                }
            });

            that.Settings.Grid2.CustomFields.NoField.prototype = new jsGrid.Field({
                // itemTemplate: function (value) {
                // },
                insertTemplate: function (value) {
                },

                editTemplate: function (value) {
                },
                insertValue: function () {

                    var itemCounts = that.Settings.Grid2JsonArray.length;
                    return itemCounts + 1;
                },
                editValue: function () {
                }
            });
            that.Settings.Grid3.CustomFields.NoField.prototype = new jsGrid.Field({
                // itemTemplate: function (value) {
                // },
                insertTemplate: function (value) {
                },
                editTemplate: function (value) {
                },
                insertValue: function () {

                    var itemCounts = that.Settings.Grid3JsonArray.length;
                    return itemCounts + 1;
                },
                editValue: function () {
                }
            });
        },
        DisbleDeleteButtonOnGrid: function () {
            $(".jsgrid-button.jsgrid-delete-button").prop("disabled", true);
            $(".jsgrid-button.jsgrid-delete-button").css("opacity", 0.1);
        },
        //---- GRID-1 -------------------------------------------------------------------------------------
        PopulateRequestBuyDetailsGrid: function (control) {
            var that = this;
            that.BindRequestBuyDetailsGridColumns();
            $(that.Settings.Controls.GridDetail1ControlSelector).jsGrid({
                width: "100%",
                height: "200px",
                align: "center",
                inserting: that.Settings.IsFormEditable,//&& that.Settings.View != "Approval",
                editing: that.Settings.IsFormEditable,
                //   deleting: that.Settings.IsFormEditable,
                sorting: false,
                autoload: true,
                noDataContent: '',//that.Settings.Grid.Titles.GridNoData,
                deleteConfirm: that.Settings.ConfirmDeleteMessage,
                data: that.Settings.Data,
                onDataLoaded: function (args) {
                    if (that.Settings.IsFormEditable == false) {
                        that.DisbleDeleteButtonOnGrid();
                    } else {
                        that.EnableButtonControls();
                    }
                    // if (that.Settings.FormModeParam == "new") {
                    //     that.DisableButtonControls();
                    // }
                },
                controller: {
                    loadData: function (filter) {
                        var d = $.Deferred();
                        var jsonString = $(that.Settings.Controls.hdRequestBuySelector).val();
                        if (jsonString != "") {
                            var jsonObject = JSON.parse(jsonString);

                            that.Settings.OldGrid1JsonArray = JSON.parse(JSON.stringify(jsonObject));
                            for (var i = 0; i < jsonObject.length; i++) {
                                jsonObject[i].No = (i + 1);
                            }
                            that.Settings.Grid1JsonArray = jsonObject;
                        }
                        d.resolve(that.Settings.Grid1JsonArray);

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
                    var quantity = args.item.Quantity;
                    var content = args.item.Content;
                    var reason = args.item.Reason;

                    if (content == "" && !content) {
                        alert(that.Settings.Messages.PleaseInputContentData);
                        args.cancel = true;
                        return;
                    }

                    if (!reason || reason == "") {
                        alert(that.Settings.Messages.PleaseInputReasonData);
                        args.cancel = true;
                        return;
                    }
                    if (quantity != "" && (!$.isNumeric(quantity) || quantity <= 0)) {
                        alert(that.Settings.Messages.QuantityDataError);
                        args.cancel = true;
                        return;
                    }

                },
                onItemInserted: function (args) {
                    that.RebindHiddenBuyDetailsFields(that.Settings.Grid1JsonArray);
                },
                onItemEditing: function (args) {
                },
                onItemUpdated: function (args) {
                    that.RebindHiddenBuyDetailsFields(that.Settings.Grid1JsonArray);
                },

                onItemUpdating: function (args) {
                    var quantity = args.item.Quantity;
                    var content = args.item.Content;
                    var reason = args.item.Reason;

                    if (content == "" && !content) {
                        alert(that.Settings.Messages.PleaseInputContentData);
                        args.cancel = true;
                        return;
                    }

                    if (!reason || reason == "") {
                        alert(that.Settings.Messages.PleaseInputReasonData);
                        args.cancel = true;
                        return;
                    }

                    if (quantity != "" && (!$.isNumeric(quantity) || quantity <= 0)) {
                        alert(that.Settings.Messages.QuantityDataError);
                        args.cancel = true;
                        return;
                    }
                },
                onItemDeleted: function (args) {
                    that.RebindHiddenBuyDetailsFields(that.Settings.Grid1JsonArray);
                    for (var i = 0; i < that.Settings.Grid1JsonArray.length; i++) {
                        that.Settings.Grid1JsonArray[i].No = (i + 1);
                    }
                    if (that.Settings.Grid1JsonArray.length > 1)
                        $(that.Settings.Controls.GridDetail1ControlSelector).jsGrid("loadData");
                },
                onItemEditCancelling: function (args) {
                },
                fields:
                that.Settings.Grid1.Fields,
            });
        },
        RebindHiddenBuyDetailsFields: function (data) {
            var that = this;
            if (data && data.length > 0) {
                $(that.Settings.Controls.hdRequestBuySelector).val(JSON.stringify(data));
                // that.EnableButtonControls();
            }
            else {
                //that.DisableButtonControls();
            }
        },
        BindRequestBuyDetailsGridColumns: function () {
            var that = this;
            that.BindOrderNumber();
            jsGrid.fields.custOrderField = that.Settings.Grid1.CustomFields.NoField;

            that.Settings.Grid1.Fields = [
                { name: "No", title: that.Settings.GridResources.GridOrderNumber, width: 25, type: "custOrderField" },
                { name: "Content", title: that.Settings.GridResources.GridBuyDetail_Content, width: 250, align: "left", type: "text" },
                { name: "Form", title: that.Settings.GridResources.GridBuyDetail_Form, width: 100, align: "left", type: "text" },
                { name: "Unit", title: that.Settings.GridResources.GridBuyDetail_Unit, width: 65, align: "left", type: "text" },
                { name: "Quantity", title: that.Settings.GridResources.GridBuyDetail_Quantity, width: 90, align: "left", type: "text" },
                { name: "Reason", title: that.Settings.GridResources.GridBuyDetail_Reason, width: 350, align: "left", type: "text" },
                { type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 60, modeSwitchButton: false }
            ];
        },

        //------ GRID-2 -----------------------------------------------------------------------------------

        PopulateRequestRepairDetailsGrid: function (control) {
            var that = this;
            that.BindRequestRepairGridColumns();
            $(that.Settings.Controls.GridDetail2ControlSelector).jsGrid({
                width: "100%",
                height: "200px",
                align: "center",
                inserting: that.Settings.IsFormEditable,//!that.Settings.IsViewMode && that.Settings.View != "Approval",
                editing: that.Settings.IsFormEditable,
                sorting: false,
                autoload: true,
                noDataContent: '',//that.Settings.Grid.Titles.GridNoData,
                deleteConfirm: that.Settings.ConfirmDeleteMessage,
                data: that.Settings.Data,
                onDataLoaded: function (args) {
                    if (that.Settings.IsFormEditable == false) {
                        that.DisbleDeleteButtonOnGrid();
                    }
                    else {
                        that.EnableButtonControls();
                    }
                    // if (that.Settings.FormModeParam == "new") {
                    //     that.DisableButtonControls();
                    // }
                },
                controller: {
                    loadData: function (filter) {
                        var d = $.Deferred();
                        var jsonString = $(that.Settings.Controls.hdDetailsRepairSelector).val();
                        if (jsonString != "") {

                            var jsonObject = JSON.parse(jsonString);
                            that.Settings.OldGrid2JsonArray = JSON.parse(JSON.stringify(jsonObject));
                            for (var i = 0; i < jsonObject.length; i++) {
                                if (jsonObject[i].From) {
                                    var fromDateObj = RBVH.Stada.WebPages.Utilities.String.toJSDate(jsonObject[i].From);
                                    if (fromDateObj) {
                                        jsonObject[i].From = Functions.parseVietnameseDateTimeToDDMMYYYY2(fromDateObj);
                                    }
                                }
                                if (jsonObject[i].To) {
                                    var toDateObj = RBVH.Stada.WebPages.Utilities.String.toJSDate(jsonObject[i].To);
                                    if (toDateObj) {
                                        jsonObject[i].To = Functions.parseVietnameseDateTimeToDDMMYYYY2(toDateObj);
                                    }
                                }

                                jsonObject[i].No = (i + 1);
                            }
                            that.Settings.Grid2JsonArray = jsonObject;
                        }

                        d.resolve(that.Settings.Grid2JsonArray);
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
                    var content = args.item.Content;
                    var reason = args.item.Reason;
                    if (content == "" && !content) {
                        alert(that.Settings.Messages.PleaseInputContentData);
                        args.cancel = true;
                        return;
                    }
                    if (!reason || reason == "") {
                        alert(that.Settings.Messages.PleaseInputReasonData);
                        args.cancel = true;
                        return;
                    }
                    this.ValidateSelectedDate(args);
                },
                ValidateSelectedDate: function (args) {
                    var from = args.item.FromDateString;
                    var to = args.item.ToDateString;
                    if (from != "" && to != "") {
                        var fromDateObj = Functions.parseVietNameseDate(from);
                        var toDateObj = Functions.parseVietNameseDate(to);
                        var now = new Date();
                        var currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                        if (fromDateObj && fromDateObj < currentDate) {
                            alert(that.Settings.Messages.SelectedDateMustGreaterThanCurrentDate);
                            args.cancel = true;
                            return;
                        }

                        if (fromDateObj && toDateObj && fromDateObj > toDateObj) {
                            alert(that.Settings.Messages.FromDateToDateError);
                            args.cancel = true;
                            return;
                        }
                    }
                },
                onItemInserted: function (args) {
                    that.RebindHiddenRepairDetailsFields(that.Settings.Grid2JsonArray);
                },
                onItemEditing: function (args) {

                },
                onItemUpdating: function (args) {
                    var content = args.item.Content;
                    var reason = args.item.Reason;

                    if (content == "" && !content) {
                        alert(that.Settings.Messages.PleaseInputContentData);
                        args.cancel = true;
                        return;
                    }

                    if (!reason || reason == "") {
                        alert(that.Settings.Messages.PleaseInputReasonData);
                        args.cancel = true;
                        return;
                    }
                    this.ValidateSelectedDate(args);
                },
                onItemUpdated: function (args) {
                    that.RebindHiddenRepairDetailsFields(that.Settings.Grid2JsonArray);
                },
                onItemDeleted: function (args) {
                    that.RebindHiddenRepairDetailsFields(that.Settings.Grid2JsonArray);
                    for (var i = 0; i < that.Settings.Grid2JsonArray.length; i++) {
                        that.Settings.Grid2JsonArray[i].No = (i + 1);
                    }
                    if (that.Settings.Grid2JsonArray.length > 1)
                        $(that.Settings.Controls.GridDetail2ControlSelector).jsGrid("loadData");
                },
                onItemEditCancelling: function (args) {
                },
                fields:
                that.Settings.Grid2.Fields,
            });
        },
        RebindHiddenRepairDetailsFields: function (data) {
            var that = this;

            if (data && data.length > 0) {
                var now = new Date();
                for (var i = 0; i < data.length; i++) {
                    data[i].From = now;
                    data[i].To = now;
                }
                $(that.Settings.Controls.hdDetailsRepairSelector).val(JSON.stringify(data));
                // that.EnableButtonControls();
            }
            else {
                // that.DisableButtonControls();
            }
        },
        BindRequestRepairGridColumns: function () {
            var that = this;
            that.BindOrderNumber();
            that.BindDateTimeControl();

            jsGrid.fields.custOrderField = that.Settings.Grid2.CustomFields.NoField;
            jsGrid.fields.custFromField = that.Settings.Grid2.CustomFields.FromField;
            jsGrid.fields.custToField = that.Settings.Grid2.CustomFields.ToField;
            that.Settings.Grid2.Fields = [
                // { name: "ID", title: "ID", readOnly: true, headercss: "hide" },
                { name: "No", title: that.Settings.GridResources.GridOrderNumber, width: 25, headercss: "header-center", type: "custOrderField" },
                { name: "Content", title: that.Settings.GridResources.GridRepairDetail_Content, width: 250, align: "left", type: "text" },
                { name: "Reason", title: that.Settings.GridResources.GridRepairDetail_Reason, width: 250, align: "left", type: "text" },
                { name: "Place", title: that.Settings.GridResources.GridRepairDetail_Place, width: 150, align: "left", type: "text" },
                { name: "FromDateString", title: that.Settings.GridResources.GridRepairDetail_FromDate, width: 60, align: "left", type: "custFromField" },
                { name: "ToDateString", title: that.Settings.GridResources.GridRepairDetail_ToDate, width: 60, align: "left", type: "custToField" },
                { type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 60, modeSwitchButton: false }
            ];
        },

        //---- GIRD-3 -------------------------------------------------------------------------------------

        PopulateRequestOtherGrid: function (control) {
            var that = this;
            //grid 3
            that.BindRequestOthersGridColumns();

            $(that.Settings.Controls.GridDetail3ControlSelector).jsGrid({
                width: "100%",
                height: "200px",
                align: "center",
                inserting: that.Settings.IsFormEditable,//!that.Settings.IsViewMode && that.Settings.View != "Approval",
                editing: that.Settings.IsFormEditable,
                sorting: false,
                autoload: true,
                noDataContent: '',//that.Settings.Grid.Titles.GridNoData,
                deleteConfirm: that.Settings.ConfirmDeleteMessage,
                data: that.Settings.Data,
                onDataLoaded: function (args) {
                    if (that.Settings.IsFormEditable == false) {
                        that.DisbleDeleteButtonOnGrid();
                    } else {
                        that.EnableButtonControls();
                    }
                    // if (that.Settings.FormModeParam == "new") {
                    //     that.DisableButtonControls();
                    // }
                },
                controller: {
                    loadData: function (filter) {
                        var d = $.Deferred();

                        var jsonString = $(that.Settings.Controls.hdDetailsOthersSelector).val();
                        if (jsonString != "") {

                            var jsonObject = JSON.parse(jsonString);
                            that.Settings.OldGrid3JsonArray = JSON.parse(JSON.stringify(jsonObject));
                            for (var i = 0; i < jsonObject.length; i++) {
                                jsonObject[i].No = (i + 1);
                            }
                            that.Settings.Grid3JsonArray = jsonObject;
                        }
                        d.resolve(that.Settings.Grid3JsonArray);
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
                    var content = args.item.Content;
                    var reason = args.item.Reason;

                    if (content == "" && !content) {
                        alert(that.Settings.Messages.PleaseInputContentData);
                        args.cancel = true;
                        return;
                    }

                    if (!reason || reason == "") {
                        alert(that.Settings.Messages.PleaseInputReasonData);
                        args.cancel = true;
                        return;
                    }
                    var quantity = args.item.Quantity;
                    if (quantity != "" && (!$.isNumeric(quantity) || quantity <= 0)) {
                        alert(that.Settings.Messages.QuantityDataError);
                        args.cancel = true;
                        return;
                    }
                },

                onItemInserted: function (args) {
                    that.RebindHiddenOthersDetailsFields(that.Settings.Grid3JsonArray);
                },
                onItemEditing: function (args) {
                },
                onItemUpdated: function (args) {
                    that.RebindHiddenOthersDetailsFields(that.Settings.Grid3JsonArray);
                },
                onItemUpdating: function (args) {
                    var content = args.item.Content;
                    var reason = args.item.Reason;

                    if (content == "" && !content) {
                        alert(that.Settings.Messages.PleaseInputContentData);
                        args.cancel = true;
                        return;
                    }

                    if (!reason || reason == "") {
                        alert(that.Settings.Messages.PleaseInputReasonData);
                        args.cancel = true;
                        return;
                    }

                    var quantity = args.item.Quantity;
                    if (quantity != "" && (!$.isNumeric(quantity) || quantity <= 0)) {
                        alert(that.Settings.Messages.QuantityDataError);
                        args.cancel = true;
                        return;
                    }
                },
                onItemDeleted: function (args) {
                    that.RebindHiddenOthersDetailsFields(that.Settings.Grid3JsonArray);
                    for (var i = 0; i < that.Settings.Grid3JsonArray.length; i++) {
                        that.Settings.Grid3JsonArray[i].No = (i + 1);
                    }
                    if (that.Settings.Grid3JsonArray.length > 1)
                        $(that.Settings.Controls.GridDetail3ControlSelector).jsGrid("loadData");
                },
                onItemEditCancelling: function (args) {
                },
                fields:
                that.Settings.Grid3.Fields,
            });
        },

        RebindHiddenOthersDetailsFields: function (data) {
            var that = this;
            if (data && data.length > 0) {
                $(that.Settings.Controls.hdDetailsOthersSelector).val(JSON.stringify(data));
                //that.EnableButtonControls();
            }
            else {
                // that.DisableButtonControls();
            }
        },
        BindRequestOthersGridColumns: function () {

            var that = this;
            that.BindOrderNumber();
            jsGrid.fields.custOrderField = that.Settings.Grid3.CustomFields.NoField;
            that.Settings.Grid3.Fields = [
                { name: "No", title: that.Settings.GridResources.GridOrderNumber, width: 25, headercss: "header-center", type: "custOrderField" },
                { name: "Content", title: that.Settings.GridResources.GridOthersDetail_Content, width: 250, align: "left", type: "text" },
                { name: "Unit", title: that.Settings.GridResources.GridOthersDetail_Unit, width: 65, align: "left", type: "text" },
                { name: "Quantity", title: that.Settings.GridResources.GridOthersDetail_Quantity, width: 90, align: "left", type: "text" },
                { name: "Reason", title: that.Settings.GridResources.GridOthersDetail_Reason, width: 350, align: "left", type: "text" },
                { type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 60, modeSwitchButton: false }
            ];
        },

        //---- BEGIN Duc.VoTan Added 2017.08.11. Those functions are moved from user control.
        CheckToShowReceivedBy: function () {
            try {
                var that = this;
                var hdIsShowReceivedById = that.Settings.Controls.hdIsShowReceivedBySelector;
                var trReceivedById = that.Settings.Controls.trReceivedBySelector;
                if ($(hdIsShowReceivedById).val() == '1') {
                    $(trReceivedById).show();
                } else {
                    $(trReceivedById).hide();
                }
            }
            catch (err) { }
        },
        CheckToShowFinishDate: function () {
            try {
                var that = this;
                var hdIsShowFinishDateClientId = that.Settings.Controls.hdIsShowFinishDateSelector;
                var trFinishDateClientId = that.Settings.Controls.trFinishDateSelector;
                if ($(hdIsShowFinishDateClientId).val() == '1') {
                    $(trFinishDateClientId).show();
                } else {
                    $(trFinishDateClientId).hide();
                }
            } catch (err) { }
        },
        CheckToShowRequiredApprovalByBOD: function () {
            try {
                var that = this;
                var hdIsShowRequiredApprovalByBODClientId = that.Settings.Controls.hdIsShowRequiredApprovalByBODSelector;
                var trRequireBODdApproveClientId = that.Settings.Controls.trRequireBODdApproveSelector;
                if ($(hdIsShowRequiredApprovalByBODClientId).val() == "1") {
                    $(trRequireBODdApproveClientId).show();
                } else {
                    $(trRequireBODdApproveClientId).hide();
                }
            }
            catch (err) { }
        },
        CheckToShowDueDate: function () {
            try {
                var that = this;
                var hdIsShowDueDateClientId = that.Settings.Controls.hdIsShowDueDateSelector;
                var trDueDateClientId = that.Settings.Controls.trDueDateSelector;
                if ($(hdIsShowDueDateClientId).val() == '1') {
                    $(trDueDateClientId).show();
                } else {
                    $(trDueDateClientId).hide();
                }
            } catch (err) { }
        },
        FilterDepartmentAccordingToRequestType: function () {
            try {
                // Filter department according to request type
                var that = this;
                var ddlReceivedByClientId = that.Settings.Controls.ddlReceivedBySelector;
                var hdRequestTypeClientId = that.Settings.Controls.hdRequestTypeSelector;
                var hdSelectedReceivedByClientId = that.Settings.Controls.hdSelectedReceivedBySelector;
                var selectedRequesTypeVal = $(hdRequestTypeClientId).val();
                var isFirst = false;
                var isReceivedByClientDisabled = false;
                if ($(ddlReceivedByClientId).attr('disabled') == 'disabled') {
                    isReceivedByClientDisabled = true;
                }
                var hdSelectedReceivedByVal = $(hdSelectedReceivedByClientId).val();
                var selectedReceivedByVal = parseInt(hdSelectedReceivedByVal);
                $(ddlReceivedByClientId).find('option').each(function (i, obj) {
                    $(obj).removeAttr('selected');
                    var requestTypes = $(obj).attr('request-type');
                    var requestTypeArray = requestTypes.split(';');
                    var flag = false;

                    for (idx = 0; idx < requestTypeArray.length; idx++) {
                        if (requestTypeArray[idx] == selectedRequesTypeVal) {
                            flag = true;
                            break;
                        }
                    }

                    if (flag) {
                        //$(obj).show();
                        $(obj).showOption();
                    } else {
                        //$(obj).hide();
                        $(obj).hideOption();
                    }

                    if (flag == true && isFirst == false && isReceivedByClientDisabled == false && selectedReceivedByVal == 0) {
                        // $(obj).attr('selected', 'selected');
                        selectedReceivedByVal = parseInt($(obj).val());
                        isFirst = true;
                    } /* else {
                        $(obj).removeAttr('selected');
                    } */
                });

                if (selectedReceivedByVal >= 0) {
                    $(ddlReceivedByClientId).find('option[value="' + selectedReceivedByVal.toString() + '"]').attr('selected', 'selected');
                    $(ddlReceivedByClientId).val(selectedReceivedByVal).change();
                } else {
                    $(ddlReceivedByClientId).empty();
                }
            } catch (err) { }
        },
        SetUrlReferTo: function () {
            try {
                var that = this;
                var ddlReferToElement = that.Settings.Controls.ddlReferToSelector;
                // DucVT DEL. 2017.10.03. Fix bug After select [Refer to], user can not scroll vertical scroll bar.
                //$(ddlReferToElement).select2();
                if ($(ddlReferToElement).length > 0) {
                    var referToId = $(ddlReferToElement).val();
                    var linkReferToSelector = that.Settings.Controls.linkReferToSelector;
                    if (referToId != '0' && referToId != null && referToId != 'undefined') {
                        var referToText = $(ddlReferToElement).find('option:selected').text();
                        var disisplayFormUrl = $(that.Settings.Controls.hdDisplayFormUrlSelector).val();
                        disisplayFormUrl += referToId;
                        $(linkReferToSelector).attr('url', disisplayFormUrl);
                        $(linkReferToSelector).attr('title', referToText);
                        $(linkReferToSelector).text(referToText);
                    } else {
                        $(linkReferToSelector).attr('url', '');
                        $(linkReferToSelector).attr('title', '');
                        $(linkReferToSelector).text('');
                    }
                }
            } catch (err) { }
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

        //---- END Duc.VoTan Added 2017.08.11. Those functions are moved from user control.
    }