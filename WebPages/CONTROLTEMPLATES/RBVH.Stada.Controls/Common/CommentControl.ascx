<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common.CommentControl" %>

<div ID="workflowHistory" runat="server" class="aspNetDisabled ms-inputBox workflowHistory" visible="false"></div>
<asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Width="100%" Rows="6" Columns="20" style="display: block;"/>
<asp:Label ID="lblErrorMessage" runat="server" Text="<%$Resources:RBVHStadaWebpages,CommentControl_EnterComment%>" Visible="false" CssClass="error-message ms-formvalidation ms-csrformvalidation" />
<asp:Label ID="lblEmptyContentErrorMessage" runat="server" Text="<%$Resources:RBVHStadaWebpages,CommentControl_EnterComment%>" CssClass="error-message ms-formvalidation ms-csrformvalidation" style="display: none;" />
<asp:Button ID="btnPostComment" runat="server" Text="<%$Resources:RBVHStadaWebpages,CommentControl_PostCommentButton%>" Visible="false" OnClick="BtnPostComment_Click" OnClientClick="return btnPostComment_OnClientClick();" style="margin-left: 0px !important; display:block; margin-top: 10px;" />

<style type="text/css">
    .workflowHistory {
        width: 100%;
        display: block;
        margin-bottom: 10px;
        min-height: 110px;
    }
    .workflowHistory:hover {
        border-top-color: rgb(171, 171, 171);
        border-top-style: solid;
        border-top-width: 1px;
        border-right-color: rgb(171, 171, 171);
        border-right-style: solid;
        border-right-width: 1px;
        border-bottom-color: rgb(171, 171, 171);
        border-bottom-style: solid;
        border-bottom-width: 1px;
        border-left-color: rgb(171, 171, 171);
        border-left-style: solid;
        border-left-width: 1px;
        border-image-source: initial;
        border-image-slice: initial;
        border-image-width: initial;
        border-image-outset: initial;
        border-image-repeat: initial;
    }
</style>

<script type="text/javascript">
    function btnPostComment_OnClientClick() {
        var res = true;

        var txtCommentClientId = '#<%=txtComment.ClientID%>';
        if ($(txtCommentClientId).val().trim() == '') {
            var lblEmptyContentErrorMessageId = '#<%=lblEmptyContentErrorMessage.ClientID%>';
            $(lblEmptyContentErrorMessageId).show();
            res = false;
        }

        return res;
    }
</script>