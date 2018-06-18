<%@ Assembly Name="RBVH.Stada.Intranet.WebPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=2c1266c12d78d768" %>

<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.Login" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Login_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Login_PageTitleArea%>" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="container">
        <div id="Div_Form" class="form-horizontal loginpage-container" runat="server">

            <%--Form Controls--%>
            <div id="Div_EmployeeID" class="form-group required">
                <label class="control-label col-sm-4">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Login_EmployeeID%>" /></label>
                <div class="col-sm-4">
                    <asp:TextBox ID="TextBox_EmployeeID" CssClass="form-control" placeholder="<%$Resources:RBVHStadaWebpages,Login_EmployeeID_Enter%>" onchange="ValidateAll();" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_EmployeeID" runat="server" ControlToValidate="TextBox_EmployeeID" ErrorMessage="<%$Resources:RBVHStadaWebpages,Login_EmployeeID_Required%>" CssClass="text text-danger" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
            </div>

            <div id="Div_AuthorizationCode" class="form-group required">
                <label class="control-label col-sm-4">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Login_AuthorizationCode%>" /></label>
                <div class="col-sm-4">
                    <asp:TextBox ID="TextBox_AuthorizationCode" CssClass="form-control" TextMode="Password" placeholder="<%$Resources:RBVHStadaWebpages,Login_AuthorizationCode_Enter%>" onchange="ValidateAll();" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_AuthorizationCode" runat="server" ControlToValidate="TextBox_AuthorizationCode" ErrorMessage="<%$Resources:RBVHStadaWebpages,Login_AuthorizationCode_Required%>" CssClass="text text-danger" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
            </div>

            <%--Actions--%>
            <div class="form-group">
                <div class="col-sm-offset-4 col-sm-8">
                    <asp:Button ID="Button_Verify" Text="Submit" OnClick="Button_Verify_Click" CssClass="btn btn-default hidden" runat="server" />
                    <button type="button" id="Button_JS_OK" class="btn btn-primary" onclick="OnVerify();">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Login_Button_Verify%>" /></button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">

        function ValidateAll() {
            ValidateEmployeeID();
            ValidateAuthorizationCode();
        }
        function ValidateEmployeeID() {
            if ($('#<%=RequiredFieldValidator_EmployeeID.ClientID %>').is(':visible')) {
                $('#Div_EmployeeID').addClass('has-error');
            } else {
                $('#Div_EmployeeID').removeClass('has-error');
            }
        }
        function ValidateAuthorizationCode() {
            if ($('#<%=RequiredFieldValidator_AuthorizationCode.ClientID %>').is(':visible')) {
                $('#Div_AuthorizationCode').addClass('has-error');
            } else {
                $('#Div_AuthorizationCode').removeClass('has-error');
            }
        }

        function OnVerify() {
            //Call button server click
            $('#<%=Button_Verify.ClientID%>').click();
            ValidateAll();
        }

    </script>

</asp:Content>
