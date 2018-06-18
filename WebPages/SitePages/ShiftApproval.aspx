<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
   <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" runat="server" />
   <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
   <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
   <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
   <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/ShiftModule/default.css?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>" />
   <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.js"></script>
   <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
   <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
   <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/toaster/jquery.toaster.js"></script>
   <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/ShiftRequest.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
   <div id="shift-approval-container">
      <asp:Panel runat="server" ID="MainPanel">
          <div id="leave-dialog" style="display: none;" title='<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_LeavePopupTitle%>" />'>
                <br />
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_LeavePopupContent%>" />
            </div>
         <table style="table-layout: fixed; width: 100% !important" class="main-table" id="gridShiftRequest">
            <tr>
               <td style="width: 100%" valign="top">
                  <div class="form-inline">
                    <div class="form-group header-left">
                        <label>
                           <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,AdminPage_Department%>" />
                        </label>
                     </div>
                     <div class="form-group">
                        <select class="form-control" id="cbDepartment" disabled>
                        </select>
                     </div>
                  </div>
               </td>
            </tr>
            <tr>
               <td style="width: 100%" valign="top">
                  <div class="form-inline pt10">
                     <div class="form-group header-left">
                         <label>
                           <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequestForLabel%>" />
                        </label>
                     </div>
                     <div class="form-group">
                        <div class="input-append  date inner-addon right-addon" style="width: 100%" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                           <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                           <input disabled class="span2 form-control shift-calendar" size="16" type="text" readonly id="txtCalendar" />
                        </div>
                     </div>
                  </div>
               </td>
            </tr>
            <tr>
               <td>
                  <br />
                  <!-- BEGIN: JS Grid -->
                  <div id="jsGridEmployee"></div>
                  <!-- END: JS Grid -->
               </td>
            </tr>
            <tr id="error-msg-container" style="display: none;">
               <td id="error-msg">
               </td>
            </tr>
            <tr>
               <td>
                  <br />
                  <div class="row">
                     <div class="col-md-offset-9 col-md-3">
                        <div class="row" style=" float: right;">
                           <button id="btnShiftRequestExport" permission="Administrators" class="btn btn-primary" style="margin-right: 15px;">
                              <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_ExportButton%>" />
                           </button>
                           <button id="btnShiftRequestSave" class="btn btn-success" style="margin-right: 15px;">
                              <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ApproveButton%>" />
                           </button>
                           <button id="btnShiftRequestCancel" class="btn btn-default shift-cancel-btn">
                              <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CancelButton%>" />
                           </button>
                        </div>
                     </div>
                  </div>
               </td>
            </tr>
         </table>
         <div id="dialog-confirm" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,DeleteConfirmMessage%>" />
         </div>
         <div id="dialog-confirm-change" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequestConfirmMessage%>" />
         </div>
         <div id="dialog-inform-dulicate-data" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequestExistedEmployeeMessage%>" />
         </div>
         <div id="cancel-confirm" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_CancelConfirm%>" />
         </div>
         <div id="duplicate-confirm" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_DuplicateMessage%>" />
         </div>
      </asp:Panel>
   </div>
   <style>
      .reallyHidden {
      display: none !important;
      }
   </style>
   <script type="text/javascript">
      $(document).ready(function () {
          var settings = {
              MonthControlSelector: '#dpMonths .span2',
              GridEmployeeControlSelector: '#jsGridEmployee',
              DepartmentControlSelector: '#cbDepartment',
              ExportControlSelector: '#btnShiftRequestExport',
              SaveControlSelector: '#btnShiftRequestSave',
              CancelControlSelector: '#btnShiftRequestCancel',
              LoadingIndicator: '#loading-indicator',
              ShiftManagementListUrl: '//{0}/SitePages/ShiftApprovalList.aspx',
              View: "Approval",
              ErrorMsgContainerSelector: "#error-msg-container",
              ErrorMsgSelector: "#error-msg",
              Grid:
              {
                  Fields: [],
                  Titles:
                  {
                      GridColumn_EmployeeId: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_EmployeeIdGridColumn%>" />',
                      GridColumn_EmployeeName: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_EmployeeNameGridColumn%>" />',
                      GridNoData: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_GridNoData%>" />'
                  }
              },
              SaveCurrentDataConfirmMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_SaveCurrentDataConfirm%>" />',
              SaveDataOK: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_SubmitOK%>" />',
              RequestExpiredMsgFormat: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestExpiredMsgFormat%>' />",
          };
      
          shiftApprovalFormInstance = new RBVH.Stada.WebPages.pages.ShiftRequest(settings);
      });
   </script>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
   <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftApproval_PageTitle%>" />
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
   <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftApproval_PageTitle%>" />
</asp:Content>