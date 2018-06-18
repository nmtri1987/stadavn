var ChangeShiftTaskListQuickApproveResource =
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
    SP.SOD.registerSod(ChangeShiftTaskListQuickApproveResource.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ChangeShiftTaskListQuickApproveResource.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(ChangeShiftTaskListQuickApproveResource.PageResourceFileName, "Res", OnPageChangeShiftTaskListResourcesReady);

    var changeShiftTaskListQuickAppoveCtx = {};
    changeShiftTaskListQuickAppoveCtx.Templates = {};
    changeShiftTaskListQuickAppoveCtx.Templates.Header = ChangeShiftTaskList_RenderHeader;
    changeShiftTaskListQuickAppoveCtx.OnPostRender = ChangeShiftTaskList_OnPostRender;
    changeShiftTaskListQuickAppoveCtx.Templates.Item = ChangeShiftTaskList_ItemOverrideFun;
    changeShiftTaskListQuickAppoveCtx.Templates.Footer = ChangeShiftTaskList_Footer;
    changeShiftTaskListQuickAppoveCtx.BaseViewID = 5;
    changeShiftTaskListQuickAppoveCtx.ListTemplateType = 171;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(changeShiftTaskListQuickAppoveCtx);
})();

function OnPageChangeShiftTaskListResourcesReady() {
    ChangeShiftTaskListQuickApproveResource.ApproveButton = Res.approveButton;
    ChangeShiftTaskListQuickApproveResource.RejectButton = Res.rejectButton;
    ChangeShiftTaskListQuickApproveResource.SelectAtLeastToApprove = Res.selectAtLeastToApprove;
    ChangeShiftTaskListQuickApproveResource.SelectAtLeastToReject = Res.selectAtLeastToReject;
    ChangeShiftTaskListQuickApproveResource.ConfirmQuickApproveMessage = Res.confirmQuickApproveMessage;
    ChangeShiftTaskListQuickApproveResource.ConfirmQuickRejectMessage = Res.confirmQuickRejectMessage;
    ChangeShiftTaskListQuickApproveResource.QuickApproveItemUpdateSuccess = Res.quickApproveItemUpdateSuccess;
    ChangeShiftTaskListQuickApproveResource.QuickApproveItemUpdateFail = Res.quickApproveItemUpdateFail;
    ChangeShiftTaskListQuickApproveResource.QuickApproveItemUpdateExist = Res.quickApproveItemUpdateExist;
}

function ChangeShiftTaskList_RenderHeader(ctx) {
    var imageSliderHtml = "<div class='row'><div class='col-md-12' style='padding-right: 33px;'><div class='pull-right'>" +
                    "<button type='button' id='changeshift_quick_approve' class='ms-ButtonHeightWidth'   style='margin-right:10px'>" + ChangeShiftTaskListQuickApproveResource.ApproveButton + "</button>" +
                    "<button type='button' id='changeshift_quick_reject'  class='ms-ButtonHeightWidth'  >" + ChangeShiftTaskListQuickApproveResource.RejectButton + "</button>" +
                    "</div></div></div>";
    var headerHtml = "<div>" + RenderHeaderTemplate(ctx) + "</div>";
    return headerHtml + imageSliderHtml;
}

function ChangeShiftTaskList_ItemOverrideFun(ctx) {
    return RenderItemTemplate(ctx);
}

function ChangeShiftTaskList_Footer(ctx) {
    return RenderFooterTemplate(ctx);
}

function ChangeShiftTaskList_OnPostRender(ctx) {
    function getListItemWithId(itemId, listName, siteurl, success, failure) {
        var url = siteurl + "/_api/web/lists/getbytitle('" + listName + "')/items?$filter=Id eq " + itemId;
        $.ajax({
            url: url,
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data.d.results.length == 1) {
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

    function GetItemTypeForListName(name) {
        return "SP.Data." + name.charAt(0).toUpperCase() + name.split(" ").join("").slice(1) + "ListItem";
    }

    function UpdateListitem(itemId, listName, siteUrl, percentComplete, outcome, status, success, failure) {
        var itemType = GetItemTypeForListName(listName);
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
    $('#changeshift_quick_approve').click(function () {
        var successApproveListItems = [];
        var failureApproveListItems = [];
        var notUpdateApproveListItems = [];
        var countApproveItem = 0;

        var context = SP.ClientContext.get_current();
        var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
        if (selectedItems && selectedItems != undefined && selectedItems.length > 0) {
            var cofirmApproveMessageResult = confirm(ChangeShiftTaskListQuickApproveResource.ConfirmQuickApproveMessage);
            if (cofirmApproveMessageResult == true) {

                for (var index = 0; index < selectedItems.length; index++) {
                    var ajaxGetListItemPromise = $.ajax({
                        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Change Shift Workflow Task List')/items(" + selectedItems[index].id + ")",
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
                            if (data.d.TaskOutcome != "Approved" && data.d.TaskOutcome != "Rejected") {
                                UpdateListitem(data.d.Id, "Change Shift Workflow Task List", _spPageContextInfo.webAbsoluteUrl, 1, "Approved", "Completed", function () {
                                    countApproveItem++;
                                    successApproveListItems.push(data.d.Id);
                                    if (countApproveItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                    }
                                }, function () {
                                    console.log("Ooops, an error occured");
                                    countApproveItem++;
                                    failureApproveListItems.push(data.d.Id);
                                    if (countApproveItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                    }
                                });
                            }
                            else {
                                countApproveItem++;
                                notUpdateApproveListItems.push(data.d.Id);
                                if (countApproveItem === selectedItems.length) {
                                    LeaveTask_ShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                }
                            }
                        }
                    });
                }

            }
            else {
                //DO nothing
            }
        }
        else {
            alert(ChangeShiftTaskListQuickApproveResource.SelectAtLeastToApprove);
        }
    });

    $('#changeshift_quick_reject').click(function () {
        var successRejectListItems = [];
        var failureRejectListItems = [];
        var notUpdateRejectListItems = [];

        var countRejectItem = 0;
        var context = SP.ClientContext.get_current();
        var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
        if (selectedItems && selectedItems != undefined && selectedItems.length > 0) {
            var cofirmRejectMessageResult = confirm(ChangeShiftTaskListQuickApproveResource.ConfirmQuickRejectMessage);
            if (cofirmRejectMessageResult == true) {
                for (var index = 0; index < selectedItems.length; index++) {
                    var ajaxGetListItemPromise = $.ajax({
                        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Change Shift Workflow Task List')/items(" + selectedItems[index].id + ")",
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
                            if (data.d.TaskOutcome != "Approved" && data.d.TaskOutcome != "Rejected") {
                                UpdateListitem(data.d.Id, "Change Shift Workflow Task List", _spPageContextInfo.webAbsoluteUrl, 1, "Rejected", "Completed", function () {
                                    countRejectItem++;
                                    successRejectListItems.push(data.d.Id);
                                    if (countRejectItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                    }
                                }, function () {
                                    console.log("Ooops, an error occured. Please try again");
                                    countRejectItem++;
                                    failureRejectListItems.push(data.d.Id);
                                    if (countRejectItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                    }
                                });
                            }
                            else {
                                countRejectItem++;
                                notUpdateRejectListItems.push(data.d.Id);
                                if (countRejectItem === selectedItems.length) {
                                    LeaveTask_ShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                }
                            }
                        }
                    });
                }
            }
            else {
                //DO nothing
            }
        }
        else {
            alert(ChangeShiftTaskListQuickApproveResource.SelectAtLeastToReject);
        }
    });

    function LeaveTask_ShowResultMessage(successApproveListItem, failureApproveListItem, notUpdateApproveListItem) {
        var resultMessage = '';
        if (typeof successApproveListItem != undefined && successApproveListItem.length > 0) {
            resultMessage += ChangeShiftTaskListQuickApproveResource.QuickApproveItemUpdateSuccess + ": " + successApproveListItem.join(", ");
        }
        if (typeof failureApproveListItem != undefined && failureApproveListItem.length > 0) {
            resultMessage += "\n" + ChangeShiftTaskListQuickApproveResource.QuickApproveItemUpdateFail + ": " + failureApproveListItem.join(", ");
        }
        if (typeof notUpdateApproveListItem != undefined && notUpdateApproveListItem.length > 0) {
            resultMessage += "\n" + ChangeShiftTaskListQuickApproveResource.QuickApproveItemUpdateExist + ": " + notUpdateApproveListItem.join(", ");
        }
        var informMessageResult = confirm(resultMessage);
        if (informMessageResult == true) {
            location.reload();
        }
        else {
            location.reload();
        }
    }
}
