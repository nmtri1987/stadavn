(function () {
    //stada_shift_detail_link
    var shiftDisplayForm = {};
    shiftDisplayForm.Templates = {};
   shiftDisplayForm.Templates.Fields = {
        "Body": {
            "DisplayForm": Body_DisplayForm_FieldTemplate
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(shiftDisplayForm);
})();

function Body_DisplayForm_FieldTemplate(ctx)
{
    function getUrlParam(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    };
    
    var taskId = getUrlParam("ID");
    if (taskId != undefined && taskId > 0) {
    
        var stringData = ctx.CurrentItem.Body;
        if (stringData != '') {
            stringData = stringData.replace("StadaTaskID", taskId);
        }
        return stringData;
    }
    else
    {
       return RenderItemTemplate(ctx);
    }
}


