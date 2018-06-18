$(document).ready(function () {
    ExecuteOrDelayUntilScriptLoaded(function () {
        loadDepartmentName();
    }, "sp.js");
});

function loadDepartmentName() {
    var context = new SP.ClientContext;
    var departmentUrl = context.get_url();
    if (departmentUrl && departmentUrl.length > 0) {
        departmentCode = departmentUrl.replace("/", "");
        var lcid = SP.Res.lcid;
        var url = "/_vti_bin/Services/Department/DepartmentService.svc/GetByCode/" + departmentCode + "/" + lcid;
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                if (result) {
                    $("#pageTitle").text(result.DepartmentName);
                }
            }
        });
    }
}