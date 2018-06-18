<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReAssignTask.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.ReAssignTask" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
    <script type="text/javascript">
        function buttonOKClick() {
            var returnVal = $('#<%=ddlEmployee.ClientID %>').val();
            if (returnVal != null && returnVal != 'undefined') {
                var dialogResult = SP.UI.DialogResult.OK;
                SP.UI.ModalDialog.commonModalDialogClose(dialogResult, returnVal);
            } else {
                alert('<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ReAssignTask_PleaseSelectEmployee%>" />');
            }
        }

        $(document).ready(function () {
            var ddlEmpoyeesClientId = '#<%=ddlEmployee.ClientID%>';
            //$(ddlEmpoyeesClientId).select2();
        });
    </script>
    <style type="text/css">
        .ddlEmployee {
            width: 300px;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table style="width: 100%; min-height:400px;">
        <tbody>
            <tr>
                <td><asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ReAssignTask_AssignTo%>" /></td>
                <td>
                    <asp:DropDownList ID="ddlEmployee" runat="server" DataValueField="ID" DataTextField="FullName" CssClass="ddlEmployee" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <input type="button" value="OK" name="OK" onclick="buttonOKClick();" />
                    <input type="button" value="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ReAssignTask_Cancel%>' />" name="Cancel" onclick="SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.Cancel);" />
                </td>
            </tr>
        </tbody>
    </table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ReAssignTask_PageTitleArea%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ReAssignTask_PageTitleArea%>" />
</asp:Content>
