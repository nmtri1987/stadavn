var ShiftTaskListQuickApproveResource =
{
    PageResourceFileName: "RBVHStadaWebpages",
    ApproveButton: "Approve",
    RejectButton: "Reject",
    SelectAtLeastToApprove: "Please select at least 1 item to approve!",
    SelectAtLeastToReject: "Please select at least 1 item to reject!",
    ConfirmQuickApproveMessage: "Are you sure you want to update the status of selected items to 'Approved'? ",
    ConfirmQuickRejectMessage: "Are you sure you want to update the status of selected items to 'Rejected'?",
    QuickApproveItemUpdateSuccess: "Item(s) updated success",
    QuickApproveItemUpdateFail: "Item(s) updated fail",
    QuickApproveItemUpdateExist: "Item(s) could not change status"
};

(function () {
    //Register resource
    SP.SOD.registerSod(ShiftTaskListQuickApproveResource.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ShiftTaskListQuickApproveResource.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(ShiftTaskListQuickApproveResource.PageResourceFileName, "Res", OnPageShiftTaskListResourcesReady);

    var shiftTaskListQuickAppoveCtx = {};
    shiftTaskListQuickAppoveCtx.Templates = {};
    shiftTaskListQuickAppoveCtx.Templates.OnPreRender = hideToolbar;
    shiftTaskListQuickAppoveCtx.Templates.Header = ShiftTaskList_RenderHeader;
    shiftTaskListQuickAppoveCtx.OnPostRender = ShiftTaskList_OnPostRender;
    shiftTaskListQuickAppoveCtx.Templates.Item = ShiftTaskList_ItemOverrideFun;
    shiftTaskListQuickAppoveCtx.Templates.Footer = ShiftTaskList_Footer;
    shiftTaskListQuickAppoveCtx.BaseViewID = 5;
    shiftTaskListQuickAppoveCtx.ListTemplateType = 171;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(shiftTaskListQuickAppoveCtx);
})();

function OnPageShiftTaskListResourcesReady() {
    ShiftTaskListQuickApproveResource.ApproveButton = Res.approveButton;
    ShiftTaskListQuickApproveResource.RejectButton = Res.rejectButton;
    ShiftTaskListQuickApproveResource.SelectAtLeastToApprove = Res.selectAtLeastToApprove;
    ShiftTaskListQuickApproveResource.SelectAtLeastToReject = Res.selectAtLeastToReject;
    ShiftTaskListQuickApproveResource.ConfirmQuickApproveMessage = Res.confirmQuickApproveMessage;
    ShiftTaskListQuickApproveResource.ConfirmQuickRejectMessage = Res.confirmQuickRejectMessage;
    ShiftTaskListQuickApproveResource.QuickApproveItemUpdateSuccess = Res.quickApproveItemUpdateSuccess;
    ShiftTaskListQuickApproveResource.QuickApproveItemUpdateFail = Res.quickApproveItemUpdateFail;
    ShiftTaskListQuickApproveResource.QuickApproveItemUpdateExist = Res.quickApproveItemUpdateExist;

}

function hideToolbar(ctx) {
    debugger;
    ctx.DisableHeroButton = true;
}

function ShiftTaskList_RenderHeader() {
    var imageSliderHtml = "<div class='row'><div class='col-md-12' style='padding-right: 33px;'><div class='pull-right'>" +
                    "<button type='button' id='shift_quick_approve' class='ms-ButtonHeightWidth' style='margin-right:10px' >" + ShiftTaskListQuickApproveResource.ApproveButton + "</button>" +
                    "<button type='button' id='shift_quick_reject'  class='ms-ButtonHeightWidth'  >" + ShiftTaskListQuickApproveResource.RejectButton + "</button>" +
                    "</div></div></div>";
    var headerHtml = "<div>" + RenderHeaderTemplate(ctx) + "</div>";
    return headerHtml + imageSliderHtml;
    //  return imageSliderHtml;
}

function ShiftTaskList_ItemOverrideFun(ctx) {
    return RenderItemTemplate(ctx);
}

function ShiftTaskList_Footer(ctx) {
    return RenderFooterTemplate(ctx);
}

function ShiftTaskList_OnPostRender() {
    function getListItemWithId(itemId, listName, siteurl, success, failure) {
        var url = siteurl + "/_api/web/lists/getbytitle('" + listName + "')/items?$filter=Id eq " + itemId;
        $.ajax({
            url: url,
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data.d.results.length === 1) {
                    success(data.d.results[0]);
                }
                else {
                    console.log("Multiple results obtained for the specified Id value");
                }
            },
            error: function (data) {
                failure(data);
            }
        });
    }

    function getItemTypeForListName(name) {
        return "SP.Data." + name.charAt(0).toUpperCase() + name.split(" ").join("").slice(1) + "ListItem";
    }

    function updateListitem(itemId, listName, siteUrl, percentComplete, outcome, status, success, failure) {
        var itemType = getItemTypeForListName(listName);
        var item = {
            "__metadata": { "type": itemType },
            "PercentComplete": percentComplete,
            "TaskOutcome": outcome,
            "Status": status
        };

        getListItemWithId(itemId, listName, siteUrl, function (data) {
            $.ajax({
                url: data.__metadata.uri,
                type: "POST",
                contentType: "application/json;odata=verbose",
                data: JSON.stringify(item),
                headers: {
                    "Accept": "application/json;odata=verbose",
                    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                    "X-HTTP-Method": "MERGE",
                    "If-Match": data.__metadata.etag
                },
                success: function (data) {
                    success(data);
                },
                error: function (data) {
                    failure(data);
                }
            });
        }, function (data) {
            failure(data);
        });
    }
    $('#shift_quick_approve').click(function () {
        var successApproveListItems = [];
        var failureApproveListItems = [];
        var notUpdateApproveListItems = [];
        var countApproveItem = 0;

        var context = SP.ClientContext.get_current();
        var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
        if (selectedItems && selectedItems.length > 0) {
            var cofirmApproveMessageResult = confirm(ShiftTaskListQuickApproveResource.ConfirmQuickApproveMessage);
            if (cofirmApproveMessageResult === true) {

                for (var index = 0; index < selectedItems.length; index++) {
                    var ajaxGetListItemPromise = $.ajax({
                        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Shift Workflow Task List')/items(" + selectedItems[index].id + ")",
                        method: "GET",
                        headers: { "Accept": "application/json; odata=verbose" },
                        success: function (data) {
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                    ajaxGetListItemPromise.then(function (data) {
                        if (data != undefined) {
                            if (data.d.TaskOutcome !== "Approved" && data.d.TaskOutcome !== "Rejected") {
                                updateListitem(data.d.Id, "Shift Workflow Task List", _spPageContextInfo.webAbsoluteUrl, 1, "Approved", "Completed", function () {
                                    countApproveItem++;
                                    successApproveListItems.push(data.d.Id);
                                    if (countApproveItem === selectedItems.length) {
                                        leaveTaskShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                    }
                                }, function () {
                                    console.log("Ooops, an error occured");
                                    countApproveItem++;
                                    failureApproveListItems.push(data.d.Id);
                                    if (countApproveItem === selectedItems.length) {
                                        leaveTaskShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                    }
                                });
                            }
                            else {
                                countApproveItem++;
                                notUpdateApproveListItems.push(data.d.Id);
                                if (countApproveItem === selectedItems.length) {
                                    leaveTaskShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                }
                            }
                        }
                    });
                }
            }
        }
        else {
            alert(ShiftTaskListQuickApproveResource.SelectAtLeastToApprove);
        }
    });

    $('#shift_quick_reject').click(function () {
        var successRejectListItems = [];
        var failureRejectListItems = [];
        var notUpdateRejectListItems = [];

        var countRejectItem = 0;
        var context = SP.ClientContext.get_current();
        var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
        if (selectedItems && selectedItems.length > 0) {
            var cofirmRejectMessageResult = confirm(ShiftTaskListQuickApproveResource.ConfirmQuickRejectMessage);
            if (cofirmRejectMessageResult === true) {
                for (var index = 0; index < selectedItems.length; index++) {
                    var ajaxGetListItemPromise = $.ajax({
                        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Shift Workflow Task List')/items(" + selectedItems[index].id + ")",
                        method: "GET",
                        headers: { "Accept": "application/json; odata=verbose" },
                        success: function (data) {
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                    ajaxGetListItemPromise.then(function (data) {
                        if (data != undefined) {
                            if (data.d.TaskOutcome !== "Approved" && data.d.TaskOutcome !== "Rejected") {
                                updateListitem(data.d.Id, "Shift Workflow Task List", _spPageContextInfo.webAbsoluteUrl, 1, "Rejected", "Completed", function () {
                                    countRejectItem++;
                                    successRejectListItems.push(data.d.Id);
                                    if (countRejectItem === selectedItems.length) {
                                        leaveTaskShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                    }
                                }, function () {
                                    console.log("Ooops, an error occured. Please try again");
                                    countRejectItem++;
                                    failureRejectListItems.push(data.d.Id);
                                    if (countRejectItem === selectedItems.length) {
                                        leaveTaskShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                    }
                                });
                            }
                            else {
                                countRejectItem++;
                                notUpdateRejectListItems.push(data.d.Id);
                                if (countRejectItem === selectedItems.length) {
                                    leaveTaskShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                }
                            }
                        }
                    });
                }
            }
        }
        else {
            alert(ShiftTaskListQuickApproveResource.SelectAtLeastToReject);
        }
    });
    //ToDo : Dislay items in alert
    function leaveTaskShowResultMessage(successApproveListItem, failureApproveListItem, notUpdateApproveListItem) {
        var resultMessage = "";
        if (successApproveListItem != undefined && successApproveListItem.length > 0) {
            resultMessage += ShiftTaskListQuickApproveResource.QuickApproveItemUpdateSuccess + ": " + successApproveListItem.join(", ");
        }
        if (failureApproveListItem != undefined && failureApproveListItem.length > 0) {
            resultMessage += "\n" + ShiftTaskListQuickApproveResource.QuickApproveItemUpdateFail + ": " + failureApproveListItem.join(", ");
        }
        if (notUpdateApproveListItem != undefined && notUpdateApproveListItem.length > 0) {
            resultMessage += "\n" + ShiftTaskListQuickApproveResource.QuickApproveItemUpdateExist + ": " + notUpdateApproveListItem.join(", ");
        }
        var informMessageResult = confirm(resultMessage);
        if (informMessageResult === true) {
            location.reload();
        }
        else {
            location.reload();
        }
    }
}
