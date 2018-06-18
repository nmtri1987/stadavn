<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormButtonsControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common.FormButtonsControl" %>

<table>
    <tr>
        <%--Save as draft--%>
        <td id="tdSaveAsDraft" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnSaveDraft" runat="server" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_SaveDraft%>" CssClass="ms-ButtonHeightWidth" CausesValidation="false" OnClientClick="return buttonSaveAsDraftClick();" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Save & Submit--%>
        <td id="tdSaveAndSubmit" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnSaveAndSubmit" OnClientClick="return onSubmitClick();" runat="server" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_SaveAndSubmit%>" CssClass="ms-ButtonHeightWidth" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Print--%>
        <td id="tdPrint" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnPrint" runat="server" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_Print%>" CssClass="ms-ButtonHeightWidth btn btn-info" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Reject--%>
        <td id="tdReject" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnReject" runat="server" OnClientClick="return onBtnRejectClick();" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_Reject%>" CssClass="ms-ButtonHeightWidth btn-primary" CausesValidation="false" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Approve--%>
        <td id="tdApprove" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnApprove" runat="server" OnClientClick="return onBtnApproveClick();" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_Approve%>" CssClass="ms-ButtonHeightWidth btn-success" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Re-Assign--%>
        <td id="tdReAssign" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnReAssign" runat="server" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_ReAssign%>" CssClass="ms-ButtonHeightWidth" CausesValidation="false" OnClientClick="return openReAssignTaskDialog();" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Cancel workflow--%>
        <td id="tdCancelWorkflow" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnCancelWorkflow" runat="server" OnClientClick="return showWaitingDialog();" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_TerminateProcess%>" CssClass="ms-ButtonHeightWidth" CausesValidation="false" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Complete Workflow--%>
        <td id="tdCompleteWorkflow" runat="server" visible="false">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="right">
                                        <asp:Button ID="btnCompleteWorkflow" runat="server" OnClientClick="return showWaitingDialog();" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_CompleteWorkflow%>" CssClass="ms-ButtonHeightWidth btn-success" CausesValidation="false" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
        <%--Cancel--%>
        <td id="tdCancel" runat="server" visible="true">
            <table>
                <tr>
                    <td class="ms-separator">&nbsp;
                    </td>
                    <td nowrap="nowrap" class="ms-toolbar">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100%" nowrap="nowrap" align="left">
                                        <asp:Button ID="btnCancel" runat="server" OnClientClick="return showWaitingDialog();" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_Close%>" CssClass="ms-ButtonHeightWidth" OnClick="btnCancel_Click" CausesValidation="false" />
                                        <input type="button" id="btnClose" runat="server" value="<%$Resources:RBVHStadaWebpages,FormButtonsControl_Close%>" visible="false" onclick="return closeFormDialog();" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdAssignedToEmployeeId" runat="server" Value="" />

<script type="text/javascript">
    function openReAssignTaskDialog() {
        var title = '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ReAssignTask_AssignTask%>" />';
        var url = '/_layouts/15/RBVH.Stada.Intranet.WebPages/ReAssignTask.aspx';
        var args = null;
        openModalDialog(title, url, closeReAssignTaskDialogCallbackFunction, args);
        return false;
    }

    function closeReAssignTaskDialogCallbackFunction(result, target) {
        if (result == SP.UI.DialogResult.OK) {
            var btnReAssignName = $('#').attr('name');
            $('#<%=hdAssignedToEmployeeId.ClientID%>').val(target);
            showWaitingDialog();
            __doPostBack('<%=btnReAssign.UniqueID%>', '');
        }
    }

    function closeFormDialog() {
        SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, '');
    }

    function showWaitingDialog() {
        $(".se-pre-con").fadeIn(0);
        return true;
    }
    //Bug 1876 - add 2 functions
    function onBtnApproveClick() {
        var isShowWaitingDialog = false;
        if (typeof validateBeforeApprove == 'function') {
            if (validateBeforeApprove() == true) {
                isShowWaitingDialog = true;
            }
        }
        else {
            isShowWaitingDialog = true;
        }
        if (isShowWaitingDialog) {
            return showWaitingDialog();
        } else {
            return false;
        }
    }
    //Bug 1876 
    function onBtnRejectClick() {
        return showWaitingDialog();
    }

    function onSubmitClick() {
        var res = ValidateBeforeSaveAndSubmit();
        if (res) {
            showWaitingDialog();
        }
        return res;
    }
</script>
