<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftMenu.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeftMenu" %>

<div id="DeltaPlaceHolderLeftNavBar" class="ms-core-navigation" role="navigation">
    <a id="startNavigation" name="startNavigation" tabindex="-1"></a>
    <div class="ms-core-sideNavBox-removeLeftMargin">
        <div id="ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager" class="stada-leftmenu">
            <div class="noindex ms-core-listMenu-verticalBox">
                <ul runat="server" id="listmenu" class="root ms-core-listMenu-root static">

                    <asp:Repeater ID="RootItem" runat="server">
                        <ItemTemplate>
                            <li class="static">
                                <a class="static menu-item ms-core-listMenu-item ms-displayInline ms-navedit-linkNode menu-item-root" href="/">
                                    <span class="additional-background ms-navedit-flyoutArrow">
                                        <span class="menu-item-text"><%# DataBinder.Eval(Container.DataItem,"Name") %></span>
                                    </span>
                                </a>
                                <ul class="static">
                                    <asp:Repeater runat="server" ID="PermissionGroupsRepeater" EnableViewState="false" DataSource='<%# DataBinder.Eval(Container.DataItem, "PermissionGroups") %>'>
                                        <ItemTemplate>

                                            <li class="static">
                                                <a class="static menu-item ms-core-listMenu-item ms-displayInline ms-navedit-linkNode menu-item-child <%# DataBinder.Eval(Container.DataItem,"VietNameseName")%>" href="<%# DataBinder.Eval(Container.DataItem,"PageName") %>">
                                                    <span class="additional-background ms-navedit-flyoutArrow">
                                                        <span class="menu-item-text"><%# DataBinder.Eval(Container.DataItem,"Name") %></span>
                                                    </span>
                                                </a>
                                            </li>

                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>

                </ul>
            </div>
        </div>
    </div>
</div>

