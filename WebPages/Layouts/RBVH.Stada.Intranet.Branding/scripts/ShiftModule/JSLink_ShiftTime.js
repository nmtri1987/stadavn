(function () {
    var shiftTimeFormContext = {};
    shiftTimeFormContext.Templates = {};
    shiftTimeFormContext.OnPostRender = shiftTimeFormOnPostRender;
    shiftTimeFormContext.Templates.Fields = {
        "ShiftTimeWorkingHourFrom": {
            "DisplayForm": hideDatePicker
        },
        "ShiftTimeWorkingHourTo": {
            "DisplayForm": hideDatePicker
        },
        "ShiftTimeWorkingHourMid": {
            "DisplayForm": hideDatePicker
        },
        "ShiftTimeBreakingHourFrom": {
            "DisplayForm": hideDatePicker
        },
        "ShiftTimeBreakingHourTo": {
            "DisplayForm": hideDatePicker
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(shiftTimeFormContext);
})();

function shiftTimeFormOnPostRender(ctx) {
    hideShiftTimeDatePicker(ctx);
}

function hideDatePicker(ctx) {
    //common function in CommForm.js
    return Functions.hideDatePickerDisplayForm(ctx);
}

function hideShiftTimeDatePicker(ctx) {
    var datePickers = ["ShiftTimeWorkingHourFrom", "ShiftTimeWorkingHourTo", "ShiftTimeWorkingHourMid", "ShiftTimeBreakingHourFrom", "ShiftTimeBreakingHourTo"];
    //common function in CommForm.js
    Functions.hideDatePartInDatetimePicker(datePickers);
}

