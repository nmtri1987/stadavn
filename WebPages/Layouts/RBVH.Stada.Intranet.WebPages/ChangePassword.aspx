﻿<%@ Assembly Name="RBVH.Stada.Intranet.WebPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=2c1266c12d78d768" %>

<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.ChangePassword" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangePassword_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangePassword_PageTitleArea%>" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="container">

        <div id="Div_Form" class="form-horizontal loginpage-container" runat="server">

            <%--Form Controls--%>

            <%--Hidden Fields--%>
            <asp:HiddenField ID="HiddenField_EmployeeID" runat="server" />
            <div class="form-group">
                <label class="control-label col-sm-4">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangePassword_Employee%>" /></label>
                <div class="col-sm-4">
                    <asp:TextBox ID="TextBox_Employee" CssClass="form-control" ReadOnly="true" runat="server" />
                </div>
            </div>

            <div id="Div_CurrentPassword" class="form-group required">
                <label class="control-label col-sm-4">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangePassword_CurrentPassword%>" /></label>
                <div class="col-sm-4">
                    <asp:TextBox ID="TextBox_CurrentPassword" CssClass="form-control" TextMode="Password" placeholder="<%$Resources:RBVHStadaWebpages,ChangePassword_CurrentPassword_Enter%>" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_CurrentPassword" runat="server" ControlToValidate="TextBox_CurrentPassword" ErrorMessage="<%$Resources:RBVHStadaWebpages,ChangePassword_CurrentPassword_Required%>" CssClass="text text-danger" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                  

                </div>
            </div>

            <div id="Div_NewPassword" class="form-group required">
                <label class="control-label col-sm-4">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangePassword_NewPassword%>" /></label>
                <div class="col-sm-4">
                    <asp:TextBox ID="TextBox_NewPassword" CssClass="form-control" TextMode="Password" placeholder="<%$Resources:RBVHStadaWebpages,ChangePassword_NewPassword_Enter%>" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_NewPassword" runat="server" ControlToValidate="TextBox_NewPassword" ErrorMessage="<%$Resources:RBVHStadaWebpages,ChangePassword_NewPassword_Required%>" CssClass="text text-danger" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="TextBox_NewPassword" ID="RegularExpressionValidator1" ValidationExpression="^[\s\S]{6,50}$" runat="server" CssClass="text text-danger"  ErrorMessage="<%$Resources:RBVHStadaWebpages,PasswordLengthValidate%>"></asp:RegularExpressionValidator>
                </div>
            </div>

            <div id="Div_ConfirmPassword" class="form-group required">
                <label class="control-label col-sm-4">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangePassword_ConfirmPassword%>" /></label>
                <div class="col-sm-4">
                    <asp:TextBox ID="TextBox_ConfirmPassword" CssClass="form-control" TextMode="Password" placeholder="<%$Resources:RBVHStadaWebpages,ChangePassword_ConfirmPassword_Enter%>" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_ConfirmPassword" runat="server" ControlToValidate="TextBox_ConfirmPassword" ErrorMessage="<%$Resources:RBVHStadaWebpages,ChangePassword_ConfirmPassword_Required%>" CssClass="text text-danger" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="CompareValidator_ConfirmPassword" ControlToValidate="TextBox_ConfirmPassword" ControlToCompare="TextBox_NewPassword" ErrorMessage="<%$Resources:RBVHStadaWebpages,ChangePassword_ConfirmPassword_IsTheSame%>" CssClass="text text-danger" Display="Dynamic" runat="server">
                    </asp:CompareValidator>
                    <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="TextBox_ConfirmPassword" ID="RegularExpressionValidatorCurrentPassword" ValidationExpression="^[\s\S]{6,50}$" runat="server"  CssClass="text text-danger"  ErrorMessage="<%$Resources:RBVHStadaWebpages,PasswordLengthValidate%>"></asp:RegularExpressionValidator>
                </div>
            </div>

            <%--Actions--%>
            <div class="form-group">
                <div class="col-sm-offset-4 col-sm-8">
                    <asp:Button ID="Button_OK" Text="Submit" OnClick="Button_OK_Click" CssClass="btn btn-default hidden" runat="server" />
                    <asp:Button ID="Button_Cancel" Text="Cancel" OnClick="Button_Cancel_Click" CssClass="btn btn-default hidden" CausesValidation="false" runat="server" />

                    <button type="button" id="Button_JS_OK" class="btn btn-primary" onclick="OnOK();">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,OkButton%>" /></button>
                    <button type="button" id="Button_JS_Cancel" class="btn btn-default" onclick="OnCancel();">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CancelButton%>" /></button>
                </div>
            </div>

        </div>
    </div>

    <script type="text/javascript">

        $(document).ready(function () {
            RegisterEvents();
        });

        function RegisterEvents() {
            //New password change
            $('#<%=TextBox_CurrentPassword.ClientID %>').change(function () {
                ValidateAll();
            });
            $('#<%=TextBox_NewPassword.ClientID %>').change(function () {
                ValidateAll();
            });
            $('#<%=TextBox_ConfirmPassword.ClientID %>').change(function () {
                ValidateAll();
            });
        }
        function ValidateAll() {
            ValidateCurrentPassword();
            ValidateNewPassword();
            ValidateConfirmPassword();
        }
        function ValidateCurrentPassword() {
            if ($('#<%=RequiredFieldValidator_CurrentPassword.ClientID %>').is(':visible')) {
                $('#Div_CurrentPassword').addClass('has-error');
            } else {
                $('#Div_CurrentPassword').removeClass('has-error');
            }
        }
        function ValidateNewPassword() {
            if ($('#<%=RequiredFieldValidator_NewPassword.ClientID %>').is(':visible')) {
                $('#Div_NewPassword').addClass('has-error');
            } else {
                $('#Div_NewPassword').removeClass('has-error');
            }
        }
        function ValidateConfirmPassword() {
            //Require | Match new password
            if ($('#<%=RequiredFieldValidator_ConfirmPassword.ClientID %>').is(':visible')
                || $('#<%=CompareValidator_ConfirmPassword.ClientID %>').is(':visible')) {
                $('#Div_ConfirmPassword').addClass('has-error');
            } else {
                $('#Div_ConfirmPassword').removeClass('has-error');
            }
        }

        function OnOK() {
            //Call button server click
            $('#<%=Button_OK.ClientID%>').click();
            ValidateAll();
        }
        function OnCancel() {
            //Call button server click
            $('#<%=Button_Cancel.ClientID%>').click();
        }

    </script>

</asp:Content>
