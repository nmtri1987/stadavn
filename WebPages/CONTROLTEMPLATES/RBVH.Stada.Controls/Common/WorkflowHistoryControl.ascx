<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowHistoryControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common.WorkflowHistoryControl" %>

<asp:GridView ID="gridViewWorkflowHistory" runat="server" AutoGenerateColumns="false" CssClass="table gridView" Visible="false">
    <Columns>
        <asp:BoundField HeaderText="<%$Resources:RBVHStadaWebpages,WorkflowHistoryControl_ApprovalStatus%>" DataField="Status" Visible="false" ItemStyle-Width="200px" />
        <asp:BoundField HeaderText="<%$Resources:RBVHStadaWebpages,WorkflowHistoryControl_ApprovalStatus%>" DataField="VietnameseStatus" Visible="false" ItemStyle-Width="200px" />
        <asp:BoundField HeaderText="<%$Resources:RBVHStadaWebpages,WorkflowHistoryControl_PostedBy%>" DataField="PostedBy" ItemStyle-Width="250px" />
        <asp:BoundField HeaderText="<%$Resources:RBVHStadaWebpages,WorkflowHistoryControl_Date%>" DataField="CommonDate" DataFormatString="{0:dd/MM/yyy hh:mm:ss ttt}" ItemStyle-Width="200px" />
        <asp:BoundField HeaderText="<%$Resources:RBVHStadaWebpages,WorkflowHistoryControl_Comment%>" DataField="CommonComment" HtmlEncode="false" />
    </Columns>
</asp:GridView>
