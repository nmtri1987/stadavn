RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.DelegationList = function (settings) {
    this.Settings = {
        Controls:
            {
            },
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.DelegationList.prototype = {
    Initialize: function () {
        var that = this;

        $(document).ready(function () {
            ExecuteOrDelayUntilScriptLoaded(function () {
                that.InitControls();
                that.RegisterEvents();
            }, "sp.js");
        });
    },

    InitControls: function () {
        var that = this;

        var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
        var currentTab = $(hdActivedTabId).val();
        that.ActivaTab(currentTab);
        var $checkedOptions = $("span[class='select-task-delete'] input[type='checkbox']:checked");
        if ($checkedOptions && $checkedOptions.length > 0) {
            $(that.Settings.Controls.linkDeleteDelegationSeletor).removeAttr("disabled");
        }

        var $checkedNewTaskOptions = $("span[class='select-task-delete'] input[type='checkbox']:checked");
        if ($checkedNewTaskOptions && $checkedNewTaskOptions.length > 0) {
            $(that.Settings.Controls.linkDeleteNewDelegationSelector).removeAttr("disabled");
        }
        $(that.Settings.Controls.txtDelegateFromDateSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
        $(that.Settings.Controls.txtDelegateToDateSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
        $(that.Settings.Controls.txtDelegateNewTaskFromDateSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
        $(that.Settings.Controls.txtDelegateNewTaskToDateSelector).datepicker({
            viewMode: "days",
            minViewMode: "days",
            format: "dd/mm/yyyy",
            autoclose: true
        });
    },

    RegisterEvents: function () {
        var that = this;

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var targetTab = $(e.target).attr("href") // activated tab

            var hdActivedTabId = that.Settings.Controls.ActivedTabSelector;
            $(hdActivedTabId).val($(this).attr('aria-controls'));
        });

        $(that.Settings.Controls.linkAddNewDelegationSelector).on('click', function () {
            $(".se-pre-con").fadeIn(0);
        });

        $(that.Settings.Controls.linkAddNewDelegationOfNewTaskSelector).on('click', function () {
            $(".se-pre-con").fadeIn(0);
        });

        $('.linkEdit').on('click', function () {
            $(".se-pre-con").fadeIn(0);
        });
        that.DeleteCheckboxItemEvent("select-all-task-delete", "select-task-delete", that.Settings.Controls.linkDeleteDelegationSeletor);
        that.DeleteCheckboxItemEvent("select-all-newtask-delete", "select-newtask-delete", that.Settings.Controls.linkDeleteNewDelegationSelector);
    },
    ActivaTab: function (tab) {
        $('.nav-tabs a[href="#' + tab + '"]').tab('show');
    },
    ConfirmDeleteDelegation: function (button) {
        var that = this;
        var message = $(button).attr('confirmation-message');
        var res = confirm(message);
        if (res) {
            $(".se-pre-con").fadeIn(0);
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function (s, e) {
                $(".se-pre-con").fadeOut(0);
                document.title = that.Settings.Resources.PageTitle;
            });
        }
        return res;
    },
    DeleteCheckboxItemEvent: function (selectAllCheckBoxSelector, selectItemCheckboxSelector, deleteButtonSelector) {
        var that = this;
        //Set item checkboxes is checked or uncheck when clink on check all checkbox
        var $checkAllCheckbox = $("span[class='" + selectAllCheckBoxSelector + "'] input[type='checkbox']");
        var $itemCheckBoxes = $("span[class='" + selectItemCheckboxSelector + "'] input[type='checkbox']");
        $checkAllCheckbox.change(function () {
            if ($checkAllCheckbox.is(":checked") == true) {
                if ($itemCheckBoxes.length > 0) {
                    $itemCheckBoxes.prop("checked", true);
                    $(deleteButtonSelector).removeAttr("disabled");
                }
            }
            else {
                $itemCheckBoxes.removeAttr("checked");
                $(deleteButtonSelector).attr("disabled", "true");
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
            if (checkedCount > 0) {
                $(deleteButtonSelector).removeAttr("disabled");
            }
            else {
                $(deleteButtonSelector).attr("disabled", "true");
            }
        });
    },
    ConfirmDeleteDelegationItems: function () {
        var that = this;
        var result = false;
        var disable = $(that.Settings.Controls.linkDeleteDelegationSeletor).attr("disabled");
        if (disable == "" || disable == "disabled" || disable == "true") {
            result = false;
        }
        else {
            var $checkedOptions = $("span[class='select-task-delete'] input[type='checkbox']:checked");
            if (!$checkedOptions || $checkedOptions.length == 0) {
                alert(that.Settings.Resources.SelectAtLeastOneItem);
            }
            else {
                result = confirm(that.Settings.Resources.ConfirmDeleteItems);
                if (result == true) {
                    $(".se-pre-con").fadeIn(0);
                }
            }
        }
        return result;
    },
    ConfirmDeleteNewDelegationItems: function () {
        var that = this;
        var result = false;
        var disable = $(that.Settings.Controls.linkDeleteNewDelegationSelector).attr("disabled");
        if (disable == "" || disable == "disabled" || disable == "true") {
            result = false;
        }
        else {
            var $checkedOptions = $("span[class='select-newtask-delete'] input[type='checkbox']:checked");
            if (!$checkedOptions || $checkedOptions.length == 0) {
                alert(that.Settings.Resources.SelectAtLeastOneItem);
            }
            else {
                result = confirm(that.Settings.Resources.ConfirmDeleteItems);
                if (result == true) {
                    $(".se-pre-con").fadeIn(0);
                }
            }
        }
        return result;
    },

    ValidateFilter: function(fromDateControlSelector, toDateControlSelector, errorMessageControlSelector, message)
    {
        $(errorMessageControlSelector).html("");
        var isValid = true;
        var that = this;
        var fromDateString = $(fromDateControlSelector).val();
        var toDateString = $(toDateControlSelector).val();
   
        if (fromDateString && fromDateString != "" && toDateString && toDateString != "")
        {
            var fromDateObject = Functions.parseVietNameseDate(fromDateString);
            var toDateObject = Functions.parseVietNameseDate(toDateString);
            if (fromDateObject && toDateObject) {
                var fromDateOnly = new Date(fromDateObject.getFullYear(), fromDateObject.getMonth(), fromDateObject.getDate());
                var toDateOnly = new Date(toDateObject.getFullYear(), toDateObject.getMonth(), toDateObject.getDate());
                if (fromDateOnly <= toDateOnly) {
                    isValid = true;
                    $(errorMessageControlSelector).html("").hide();
                }
                else {
                    isValid = false;
                    $(errorMessageControlSelector).html(message).show();
                }
            }
        }
        return isValid;
    },
     ValidateFilterMyDelegation: function()
     {
         var that = this;
         var isValid = that.ValidateFilter(that.Settings.Controls.txtDelegateFromDateSelector, that.Settings.Controls.txtDelegateToDateSelector, that.Settings.Controls.txtDelegateToDateSelector_Error, that.Settings.Resources.ToDateGreaterThanFromDate);
         if(isValid == true)
         {
             $(".se-pre-con").fadeIn(0);
         }
         return isValid;
     },

    ValidateFilterNewTaskDelegation: function()
    {
        var that = this;
        var isValid = that.ValidateFilter(that.Settings.Controls.txtDelegateNewTaskFromDateSelector, that.Settings.Controls.txtDelegateNewTaskToDateSelector, that.Settings.Controls.txtDelegateNewTaskFromDateSelector_Error, that.Settings.Resources.ToDateGreaterThanFromDate);
        if(isValid == true)
        {
            $(".se-pre-con").fadeIn(0);
        }
        return isValid;
    }
};