(function () {
    var isFirst = true;
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        var hideField = "<span class='csrHiddenField'></span>";
        var disableField = "<span class='csrDisableField'></span>";
        var overrides = {};
        overrides.Templates = {};
        overrides.Templates.Fields = {
            "Predecessors": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "RelatedTasks": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "StartDate": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "DueDate": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "Priority": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "ListURL": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "ItemId": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "ItemURL": {
                "EditForm": hideField,
                "NewForm": hideField
            },
            "AssignedTo":
            {
                "EditForm": assignCustomDisplay,
            },
            "NextAssign":
            {
                "EditForm": hideField,
                "NewForm": hideField
            }
        };
        overrides.Templates.OnPostRender = function (ctx) {
            if (isFirst) {
                var data = ctx.ListData.Items[0].ItemURL + '&IsDlg=1&TextOnly=true';
                $('.ms-webpart-zone').closest('td').css('vertical-align', 'top');
                $('.ms-webpart-zone').closest('tr').append('<td class="embeddedView" style="margin-top: 8px;">' +
                    '<iframe id="frame"  allowTransparency="true" src="' + data + '" width="700px" height="670px">' +
                    '</iframe>' +
                    '</td>');
                $("#frame").contents().find("#s4-ribbonrow").remove();
            }
            isFirst = false;
            $("#DeltaPlaceHolderMain").addClass('border-container');
            rlfiShowMore();
            disableControls(ctx);
            $(".csrHiddenField").closest("tr").hide();
        };
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrides);
    }, "sp.js", "strings.js");
})();
function makeFieldReadOnly(ctx) {
    ctx.CurrentItem.MakeReadOnly = true;
}

function assignCustomDisplay(ctx) {
    var currentFieldValue = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    var text = "";
    if (currentFieldValue && currentFieldValue != "" && currentFieldValue[0]) {
        text = currentFieldValue[0].DisplayText;

        try {
            if (_rbvhContext.EmployeeInfo && _rbvhContext.EmployeeInfo.ADAccount.ID != currentFieldValue[0].EntityData.SPUserID) {
                $("input[id$='Approved']").hide();
                $("input[id$='Rejected']").hide();
                $("input[id$='SaveItem']").hide();
            }
        } catch (e) { }
    }

    return text;
}

function disableControls(ctx) {
    if (ctx.BaseViewID == "EditForm") {
        $("input[id^='Title_'][id $=Field]").prop("disabled", "disabled");
        $("select[id^='CommonDepartment_'][id $=Field]").prop("disabled", "disabled");
        $("input[id^='PercentComplete_'][id $=Field]").prop("disabled", "disabled");
        $("select[id^='StepModule_']").prop("disabled", "disabled");
        $("select[id^='Status_']").prop("disabled", "disabled");
        $("select[id^='CurrentStepStatus_']").prop("disabled", "disabled");
    }
}
