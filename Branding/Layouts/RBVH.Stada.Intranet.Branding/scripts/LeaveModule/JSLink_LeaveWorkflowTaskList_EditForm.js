(function () {
    var leaveTaskEditFormContext = {};
    leaveTaskEditFormContext.Templates = {};
    leaveTaskEditFormContext.Templates.OnPostRender = TaskEditForm_HiddenFiledOnPreRender;
    leaveTaskEditFormContext.Templates.Fields = {
        "StartDate": {
            "EditForm": TaskEditForm_HiddenFiledTemplate
        },
        "Predecessors":
        {
            "EditForm": TaskEditForm_HiddenFiledTemplate,
        },
        "Body":
        {
            "EditForm": TaskEditForm_HiddenFiledTemplate,
        },
        "Priority":
        {
            "EditForm": TaskEditForm_HiddenFiledTemplate,
        },
        "DueDate":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel
        }
        , "AssignedTo":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel_AssignTo_Dropdown
        }
        , "Title":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel
        }
        , "Requester":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel_Requester_Dropdown
        }
        , "LeaveHours":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel
        }
        ,
        "CommonFrom":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel
        }
        ,
        "To":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel
        },
        "Leaved":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel
        },
        "TransferworkTo":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel_Tranfer_Dropdown
        },
        "Reason":
        {
            "EditForm": LeaveTaskEditForm_ReturnLalel
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveTaskEditFormContext);
})();

function LeaveTaskEditForm_ReturnLalel_Requester_Dropdown(ctx) {
    var value = ctx.CurrentItem.Requester;
    var name = value.toString().split("#")
    return "<div>" + name[1] + "</div>";
}

function LeaveTaskEditForm_ReturnLalel_Tranfer_Dropdown(ctx) {
    var value = ctx.CurrentItem.TransferworkTo;
    var name = value.toString().split("#");
    return "<div>" + name[1] + "</div>";
}

function LeaveTaskEditForm_ReturnDatetimeLabel_From(ctx) {
    return ctx.CurrentItem.CommonFrom;
}
function LeaveTaskEditForm_ReturnDatetimeLabel_To(ctx) {
    return ctx.CurrentItem.To;
}
function TaskEditForm_HiddenFiledTemplate() {
    return "<span class='csrHiddenField'></span>";
}

function TaskEditForm_HiddenFiledOnPreRender(ctx) {
    jQuery(".csrHiddenField").closest("tr").hide();
}

function LeaveTaskEditForm_ReturnLalel_AssignTo_Dropdown(ctx) {

    var text = ctx.CurrentItem.AssignedTo[0].DisplayText;
    return "<div>" + text + "</div>";
}
function LeaveTaskEditForm_ReturnLalel(ctx) {
    var text = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    return "<div>" + text + "</div>";
}

