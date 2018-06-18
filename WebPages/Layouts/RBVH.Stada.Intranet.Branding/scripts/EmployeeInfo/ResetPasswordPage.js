$(document).ready(function () {
    $("[id$=SearchEmployeeTextBox]").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _spPageContextInfo.webAbsoluteUrl + '/_layouts/15/RBVH.Stada.Intranet.WebPages/ResetPassword.aspx/GetCommonAccounts',
                data: "{ 'employeeNameOrId': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data.d, function (item) {
                        return {
                            label: item.DisplayName,
                            val: item.EmployeeId
                        }
                    }))
                },
                error: function (response) {
                    //console.log("ERROR - ResetPasswordPage.js file : " + response.responseText);
                },
                failure: function (response) {
                    //console.log("ERROR - ResetPasswordPage.js file : " + response.responseText);
                }
            });
        },
        select: function (e, i) {
            $("[id$=SelectedEmployeeId]").val(i.item.val);
        },
        minLength: 1
    });
});
