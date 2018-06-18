(function () {
    var overtimeEmployeeFormContext = {};
    overtimeEmployeeFormContext.Templates = {};
    overtimeEmployeeFormContext.OnPostRender = shiftTimeFormOnPostRender;
    overtimeEmployeeFormContext.Templates.Fields = {
        "OvertimeFrom": {
            "DisplayForm": hideDatePicker
        },
        "OvertimeTo": {
            "DisplayForm": hideDatePicker
        },
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overtimeEmployeeFormContext);
})();

function shiftTimeFormOnPostRender(ctx) {
    hideShiftTimeDatePicker(ctx);
}

function hideDatePicker(ctx) {
    //common function in CommForm.js
    return Functions.hideDatePickerDisplayForm(ctx);
}

function hideShiftTimeDatePicker(ctx) {
    var datePickers = ["OvertimeFrom", "OvertimeTo"];
    //common function in CommForm.js
    Functions.hideDatePartInDatetimePicker(datePickers);
}
