<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SupportingDocumentControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common.SupportingDocumentControl" %>
<script src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/Common/SupportingDocument.js?v=6498381C-A713-4720-A0F2-53D4893EE044"></script>

<script type="text/javascript">
    $(document).ready(function () {
        var supportingDocumentsettings = {
            Controls: {
                GridSupportingDocumentSelector: '#<%=GridSupportingDocument.ClientID%>',
                AddMoreFileSelector: '#<%=AddMoreFile.ClientID%>',
                hdIsEditableSelector: '#<%=hdIsEditable.ClientID%>',
            },
            Message:
            {
                InvalidFileName: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,SuppotingDoc_InvalidFileName%>' />",
                FileEmpty: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,SupportingDoc_EmptyFile%>' />",
                CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
            }
        };
        supportingDocumentInstance = new RBVH.Stada.WebPages.pages.SupportingDocument(supportingDocumentsettings);
    });
</script>
<asp:HiddenField ID="hdIsEditable" runat="server" Value="True" />
<div id="GridSupportingDocument" runat="server" >

</div>
<span class="glyphicon glyphicon-plus addmorefile" aria-hidden="true" id="AddMoreFile" runat="server" visible="false" ></span>
