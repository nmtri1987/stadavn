
(function () {
    //Register resource
    var overtimeDetailCtx = {};
    overtimeDetailCtx.Templates = {};
    overtimeDetailCtx.Templates.Header = overtime_RenderHeader;
    overtimeDetailCtx.OnPostRender = overtime_OnPostRender;
    overtimeDetailCtx.Templates.Item = overtime_ItemOverrideFun;
    overtimeDetailCtx.Templates.Footer = overtime_Footer;
    // overtimeDetailCtx.BaseViewID = 5;
    // overtimeDetailCtx.ListTemplateType = 171;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overtimeDetailCtx);
    isRunOnce = false;
})();
var isRunOnce = false;
function overtime_RenderHeader(ctx) {
    return RenderHeaderTemplate(ctx);
}

function overtime_ItemOverrideFun(ctx) {
    return RenderItemTemplate(ctx);
}

function overtime_Footer(ctx) {
    return RenderFooterTemplate(ctx);
}

function overtime_OnPostRender(ctx) {
    //  var items = ctx.ListData.Items;

    var overtimeSaveButton = "#overtimeSaveButton";
    var overtimeCancelButton = "#overtimeCancelButton";
    if (isRunOnce === false) {
        isRunOnce = true;
        $(overtimeSaveButton).on('click', function () {
            var overtimeManagementFormData = overtimeManagement_Submitform();
            if (overtimeManagementFormData !== undefined && overtimeManagementFormData !== true) {
                var methodUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/RBVH.Stada.Intranet.WebPages/WebServices.aspx/SaveOvertimeManagementForm";
                var savePageDataPromise = $.ajax({
                    type: "POST",
                    url: methodUrl,
                    data: JSON.stringify({ overtimeManagementData: overtimeManagementFormData }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false
                });
                savePageDataPromise.then(
                    function (data) {

                        if (data.d !== null) {
                            if (data.d.CodeMessageResult.Code === 0) {
                                //alert(data.d.OvertimeId);
                                updateOvertimeDetailList(data.d.OvertimeId);
                            }
                            else {
                                console.log(data.d.CodeMessageResult.Message);
                            }
                        }
                        else {
                            alert("Error occurred");
                        }
                    },
                    function (errorData) {
                        console.log(errorData);
                    });
            }
            else {
                alert("Error occurred");
            }
            var context = SP.ClientContext.get_current();
            // var selectedItems = SP.ListOperation.Selection.selectListItem(context);
        });
        $(overtimeCancelButton).on("click", function () {

            deleteEmployeeDetail();
        });

    }

    function updateOvertimeDetailList(overtimeManagementId) {
        var clientContext = SP.ClientContext.get_current();
        var oList = clientContext.get_web().get_lists().getByTitle('Overtime Employee Details');
        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml(
            '<View><Query><Where><IsNull><FieldRef Name=\'OvertimeManagementID\' /></IsNull></Where></Query></View>'
        );
        this.collListItem = oList.getItems(camlQuery);
        clientContext.load(collListItem);
        clientContext.executeQueryAsync(
            Function.createDelegate(this, function (data) {
                var ovetimeEmployeeDetailItems = [];
                var listItemEnumerator = collListItem.getEnumerator();
                while (listItemEnumerator.moveNext()) {
                    var oListItem = listItemEnumerator.get_current();
                    ovetimeEmployeeDetailItems.push(oListItem.get_id());
                }
                proccessSaveEmployeeDetail(ovetimeEmployeeDetailItems, overtimeManagementId);
            }),
            Function.createDelegate(this, function (data) {
            })
        );

        function proccessSaveEmployeeDetail(items, overtimeManagementUpdateId) {
            if (items.length > 0) {
                itemSaveCount = 0;
                for (var i = 0; i < items.length; i++) {
                    var clientContext = SP.ClientContext.get_current();
                    var oList = clientContext.get_web().get_lists().getByTitle('Overtime Employee Details');
                    this.oListItem = oList.getItemById(items[i]);
                    oListItem.set_item('OvertimeManagementID', overtimeManagementUpdateId);
                    oListItem.update();

                    clientContext.executeQueryAsync(Function.createDelegate(this, function () {
                        itemSaveCount++;
                        if (itemSaveCount === items.length) {
                            location.href = "/SitePages/OvertimeManagement.aspx";
                        }
                    }),
                     Function.createDelegate(this, function () {
                         itemSaveCount++;
                         if (itemSaveCount === items.length) {
                             location.href = "/SitePages/OvertimeManagement.aspx";
                         }
                     }));
                }
            }
            else {
                alert("No data");
            }
        }
    }

    function deleteEmployeeDetail() {

        var clientContext = SP.ClientContext.get_current();
        var oList = clientContext.get_web().get_lists().getByTitle('Overtime Employee Details');
        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml(
            '<View><Query><Where><IsNull><FieldRef Name=\'OvertimeManagementID\' /></IsNull></Where></Query></View>'
        );
        this.collListItem = oList.getItems(camlQuery);
        clientContext.load(collListItem);
        clientContext.executeQueryAsync(
            Function.createDelegate(this, function (data) {
                var ovetimeEmployeeDetailItems = [];
                var listItemEnumerator = collListItem.getEnumerator();
                while (listItemEnumerator.moveNext()) {
                    var oListItem = listItemEnumerator.get_current();
                    ovetimeEmployeeDetailItems.push(oListItem.get_id());
                }

                processDeleteEmployeeDetail(ovetimeEmployeeDetailItems);
            }),
            Function.createDelegate(this, function (data) {
            })
        );
    }
    function processDeleteEmployeeDetail(items) {
        if (items.length > 0) {
            itemDeleteCount = 0;
            for (var index = 0; index < items.length; index++) {
                var clientContext = SP.ClientContext.get_current();
                var oList = clientContext.get_web().get_lists().getByTitle('Overtime Employee Details');
                this.oListItem = oList.getItemById(items[index]);
                oListItem.deleteObject();
                //count item
                clientContext.executeQueryAsync(Function.createDelegate(this, function (data) {
                    itemDeleteCount++;
                    if (itemDeleteCount === items.length) {
                        location.href = "/SitePages/OvertimeManagement.aspx";
                    }
                }),
                    Function.createDelegate(this, function (data) {
                        itemDeleteCount++;
                        if (itemDeleteCount === items.length) {
                            location.href = "/SitePages/OvertimeManagement.aspx";
                        }
                    }));
            }
        }
    }

    wireUpEvents();
    function wireUpEvents() {
        // Attach the event keypress to exclude the F5 refresh
        $(document).bind('keypress', function (e) {
            if (e.keyCode === 116) {
                deleteEmployeeDetail();
            }
        });

        window.onbeforeunload = function () {
            deleteEmployeeDetail();
        };

        if (window.location.hash === 'redirected') {
            deleteEmployeeDetail();
        }
    }

}



