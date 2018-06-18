(function () {
    var overtimeEmployeeDetailCtx = {};
    overtimeEmployeeDetailCtx.Templates = {};
    overtimeEmployeeDetailCtx.Templates.Fields = {
        "Employee": {
            "NewForm": overtimeGetEmployeeListInDepartment
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overtimeEmployeeDetailCtx);
})();

function overtimeGetEmployeeListInDepartment(ctx) {
    var overtimeDetailControlEmployee = "#overtimeDetailEmployee";
    var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
    formCtx.registerGetValueCallback(formCtx.fieldName, function () {
        return $(overtimeDetailControlEmployee).find(":selected").val();
    });

    var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/GetEmployeeListInCurrentDepartment";
    var getEmployeeListPromise = $.ajax({
        type: "POST",
        url: methodUrl,
        data: '{"departmentId": ' + null + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false
    });
    getEmployeeListPromise.then(
        function (successData) {
            if (successData != null) {

                if (successData.d.CodeMessageResult.Code === 0) {
                    var requesterList = successData.d.EmployeeList;
                    for (var rIndex = 0; rIndex < requesterList.length; rIndex++) {
                        $(overtimeDetailControlEmployee).append('<option data-employee-id = "' + requesterList[rIndex]["EmployeeID"] + '" value="' + requesterList[rIndex]["LookupId"] + '">' + requesterList[rIndex].FullName + "</option>");
                    }
                }
            }
            else {
                console.log(successData.d.CodeMessageResult.Message);
            }
        },
        function (failureData) {
            console.log("Failed to get data");
        }
    );
    return "<select id='overtimeDetailEmployee'> </select>";
}
