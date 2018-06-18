var Task = Task || {};
Task.Config = {};
Task.Config.Url = "/Lists/TaskList/EditForm.aspx?ID={0}";
Task.ListResourceFileName = "RBVHStadaLists";
Task.PageResourceFileName = "RBVHStadaWebpages";
(function () {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(Task.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + Task.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(Task.ListResourceFileName, "Res", OnPageResourcesReady);
    }, "strings.js");

    var overrideCtx = {};
    overrideCtx.Templates = {};
    overrideCtx.OnPreRender = function (ctx) {

    };
    overrideCtx.OnPostRender = function (ctx) {
        $('.ms-csrlistview-controldiv, .ms-list-addnew ,.ms-menutoolbar').hide();

        $('.ms-vh2 , .ms-vh').each(function () {
            var text = $(this).text();
            $(this).html(text);
        });
    };
    overrideCtx.BaseViewID = 10;
    overrideCtx.ListTemplateType = 171;
    overrideCtx.Templates.Fields = {
        "Title": { "View": titleTemplate }
        //  "Status": { "View": statusTemplate }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);

})();

function OnPageResourcesReady() {
    $('.label-success').text(Res.approvalStatus_Approved);
    //$('.label-default').text(Res.approvalStatus_InProgress);
    $('.label-warning').text(Res.approvalStatus_Cancelled);
    $('.label-danger').text(Res.approvalStatus_Rejected);
}

function titleTemplate(ctx) {
    var id = ctx.CurrentItem['ID'];
    var title = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    var currentTab = "#tab2";
    var url = String.format(Task.Config.Url, id);
    var source = "&Source=" + window.location.pathname + window.location.search + currentTab;
    var html = String.format('<a href="{0}" >{1}</a>', url + source, title).toString();
    return html;
}

function statusTemplate(ctx) {
    var status = ctx.CurrentItem['Status'];

    if (status == 'Approved') {
        status = '<span class="label label-success">Approved</span>';
    }
    else if (status == "Cancelled") {
        status = '<span class="label label-warning">Cancelled</span>';
    }
    else if (status == "Rejected") {
        status = '<span class="label label-danger">Rejected</span>';
    }
    else {
        status = '<span class="label label-default">In-Progress</span>';
    }

    return status;
}