using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using Microsoft.SharePoint.Utilities;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Core.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;

namespace RBVH.Stada.Intranet.Branding.Features.FeatureWeb
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("d21f13b6-6b6b-455d-a697-9fc82fd7255c")]
    public class FeatureWebEventReceiver : SPFeatureReceiver
    {
        private const string TimeSheet = "TimeSheet_LeftMenu";
        /*Children of TIME_SHEET */
        private const string LeaveRequest = "LeaveRequest_LeftMenu";
        private const string LeaveApprovalList = "LeaveApprovalList_LeftMenu";
        private const string MyOverTime = "MyOverTime_LeftMenu";
        private const string OvertimeRequest = "OvertimeRequest_LeftMenu";
        private const string OvertimeApprovallist = "OvertimeApprovalList_LeftMenu";
        private const string MyShiftRequest = "ShiftManagement_MShiftRequestTitle";
        private const string ShiftRequest = "ShiftRequest_LeftMenu";
        private const string ShiftApprovallist = "ShiftApprovalList_LeftMenu";
        private const string ChangeShiftRequest = "ChangeShiftRequest_LeftMenu";
        private const string ChangeShiftRequestTaskList = "ChangeShiftRequestTaskList_LeftMenu";
        private const string RequestToNotOvertime = "RequesttoNotOvertime_LeftMenu";
        private const string RequestToNotOvertimeTaskList = "RequesttoNotOvertimeTaskList_LeftMenu";

        private const string MeetingRoomBooking = "MeetingRoomBooking_LeftMenu";
        private const string LicenseToTransport = "LicenseToTransport_LeftMenu";
        private const string BusinessTrip = "BusinessTrip_LeftMenu";
        private const string RecruitmentRequest = "RecruitmentRequest_LeftMenu";
        private const string DiplomaSupplementRequest = "DiplomaSupplementRequest_LeftMenu ";
        private const string RequisitionOfDocuments = "RequisitionOfDocuments_LeftMenu";
        private const string InformationUpdateRequest = "InformationUpdateRequest_LeftMenu";
        private const string GuessWelcomingRequest = "GuessWelcomingRequest_LeftMenu";
        private const string RequestForm = "RequestForm_LeftMenu";
        private const string RegisterForTransportation = "RegisterForTransportation_LeftMenu";
        private const string StationaryRequest = "StationaryRequest_LeftMenu";
        private const string SupplierMeetingRequest = "SupplierMeetingRequest_LeftMenu";
        private const string EmployeeShiftTime = "EmployeeShiftTime_LeftMenu";
        private const string TransportationRequest = "TransportRequest_LeftMenu";
        private const string TransportTaskList= "TransportTaskList_LeftMenu";
        private const string RAndD = "RnD_LeftMenu";
        /*Children of  R_AND_D*/
        private const string DissoierForExportingRequest = "DissoierForExportingRequest_LeftMenu";
        private const string DissoierForDomesticRequest = "DissoierForDomesticRequest_LeftMenu";
        private const string NotconformingRequest = "NotConfirmingRequest_LeftMenu";
        private const string DesignRequest = "DesignRequest_LeftMenu";

        private const string ResourceFilename = "RBVHStadaMasterPage";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWeb web = (SPWeb)properties.Feature.Parent;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite site = new SPSite(web.Site.ID))
                    {
                        using (SPWeb currentWeb = site.OpenWeb())
                        {
                            if (web == null) return;
                            GenerateLeftMenu(currentWeb);

                            CreateDepartmentSite(web);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void CreateDepartmentSite(SPWeb web)
        {
            SPList departmentList = web.GetList($"{web.Url}/Lists/Departments");
            if (departmentList == null) return;
            var departmentRepo = new DepartmentDAL(web.Url);
            departmentRepo.CreateDepartmentsSite(web.Site.ID, web.ID, departmentList.Items);
        }

        private void GenerateLeftMenu(SPWeb web)
        {
            var nodeCollection = web.Navigation.QuickLaunch;

            // Delete all existing items in Quick Launch
            var counter = 0;
            while (nodeCollection.Count != counter && nodeCollection.Count > 0)
            {
                counter++;
                var item = nodeCollection[0];
                item.Delete();
            }

            //nodeCollection = web.Navigation.QuickLaunch;
            var timeSheetHeading = new SPNavigationNode("Timesheet", web.Url);
            nodeCollection.AddAsLast(timeSheetHeading);
            foreach (var culture in web.SupportedUICultures)
            {
                string timeSheetResourceKey = $"$Resources:{ResourceFilename},{TimeSheet}";
                var locStr = SPUtility.GetLocalizedString(timeSheetResourceKey, ResourceFilename,
                    (uint)culture.LCID);
                timeSheetHeading.TitleResource.SetValueForUICulture(culture, locStr);
            }
            timeSheetHeading.Update();

            //add child
            var leaveRequest = new SPNavigationNode(
                $"$Resources:{ResourceFilename},{LeaveRequest}",
                $"{web.Url}/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement.aspx");
            timeSheetHeading.Children.AddAsLast(leaveRequest);

            var leaveApprovalTaskList = new SPNavigationNode(
              $"$Resources:{ResourceFilename},{LeaveApprovalList}",
              $"{web.Url}/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagementApprovalTask.aspx");
            timeSheetHeading.Children.AddAsLast(leaveApprovalTaskList);


            var myOvertimeRequest = new SPNavigationNode(
                $"$Resources:{ResourceFilename},{MyOverTime}", $"{web.Url}/_layouts/15/RBVH.Stada.Intranet.WebPages/MyOvertime.aspx");
            timeSheetHeading.Children.AddAsLast(myOvertimeRequest);

            var overtimeRequest = new SPNavigationNode(
                $"$Resources:{ResourceFilename},{OvertimeRequest}", $"{web.Url}/SitePages/OvertimeManagement.aspx");
            timeSheetHeading.Children.AddAsLast(overtimeRequest);

            var overtimeApprovalList = new SPNavigationNode(
                $"$Resources:{ResourceFilename},{OvertimeApprovallist}", $"{web.Url}/SitePages/OvertimeApprovalList.aspx");
            timeSheetHeading.Children.AddAsLast(overtimeApprovalList);

            var myshiftRequest =
              new SPNavigationNode($"$Resources:{ResourceFilename},{MyShiftRequest}",
                  $"{web.Url}/_layouts/15/RBVH.Stada.Intranet.WebPages/MyShiftTime.aspx");
            timeSheetHeading.Children.AddAsLast(myshiftRequest);

            var shiftRequest =
                new SPNavigationNode($"$Resources:{ResourceFilename},{ShiftRequest}",
                    $"{web.Url}/SitePages/ShiftManagement.aspx");
            timeSheetHeading.Children.AddAsLast(shiftRequest);

            var shiftApprovalList = new SPNavigationNode(
                $"$Resources:{ResourceFilename},{ShiftApprovallist}", $"{web.Url}/SitePages/ShiftApprovalList.aspx");
            timeSheetHeading.Children.AddAsLast(shiftApprovalList);

            var changeShiftRequest =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{ChangeShiftRequest}", $"{web.Url}/SitePages/ChangeShiftManagement.aspx");
            timeSheetHeading.Children.AddAsLast(changeShiftRequest);
            var changeShiftRequestTaskList =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{ChangeShiftRequestTaskList}", $"{web.Url}/SitePages/ChangeShiftManagementTaskList.aspx");
            timeSheetHeading.Children.AddAsLast(changeShiftRequestTaskList);

            var requestToNotOvertime =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{RequestToNotOvertime}", $"{web.Url}/SitePages/LeaveOfAbsenceManagement.aspx");
            timeSheetHeading.Children.AddAsLast(requestToNotOvertime);
            var requestToNotOvertimeTaskList =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{RequestToNotOvertimeTaskList}", $"{web.Url}/SitePages/LeaveOfAbsenceManagementTaskList.aspx");
            timeSheetHeading.Children.AddAsLast(requestToNotOvertimeTaskList);

            var transportationRequest =
            new SPNavigationNode(
             $"$Resources:{ResourceFilename},{TransportationRequest}", $"{web.Url}/SitePages/TransportationManagement.aspx");
            timeSheetHeading.Children.AddAsLast(transportationRequest);

            var transportationTaskList =
            new SPNavigationNode(
             $"$Resources:{ResourceFilename},{TransportTaskList}", $"{web.Url}/SitePages/TransportationManagementTaskList.aspx");
            timeSheetHeading.Children.AddAsLast(transportationTaskList);

            timeSheetHeading.Update();

            var supplierMeeting =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{SupplierMeetingRequest}",
                    web.Url);
            nodeCollection.AddAsLast(supplierMeeting);

            var stationaryRequestHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{StationaryRequest}", web.Url);
            nodeCollection.AddAsLast(stationaryRequestHeading);

            var registerForTransportationHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{RegisterForTransportation}",
                    web.Url);
            nodeCollection.AddAsLast(registerForTransportationHeading);

            var requestFormHeading =
                new SPNavigationNode($"$Resources:{ResourceFilename},{RequestForm}",
                    web.Url);
            nodeCollection.AddAsLast(requestFormHeading);

            var guessWelcomingRequestHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{GuessWelcomingRequest}", web.Url);
            nodeCollection.AddAsLast(guessWelcomingRequestHeading);

            var informationUpdateRequestHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{InformationUpdateRequest}",
                    web.Url);
            nodeCollection.AddAsLast(informationUpdateRequestHeading);

            var requisitionOfDocumentsHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{RequisitionOfDocuments}",
                    web.Url);
            nodeCollection.AddAsLast(requisitionOfDocumentsHeading);

            var diplomaSupplementRequestHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{DiplomaSupplementRequest}",
                    web.Url);
            nodeCollection.AddAsLast(diplomaSupplementRequestHeading);

            var recruitmentRequestHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{RecruitmentRequest}", web.Url);
            nodeCollection.AddAsLast(recruitmentRequestHeading);

            var businessTripHeading =
                new SPNavigationNode($"$Resources:{ResourceFilename},{BusinessTrip}",
                    web.Url);
            nodeCollection.AddAsLast(businessTripHeading);

            var licenseToTransportHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{LicenseToTransport}", web.Url);
            nodeCollection.AddAsLast(licenseToTransportHeading);

            var meetingRoomHeading =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{MeetingRoomBooking}", web.Url);
            nodeCollection.AddAsLast(meetingRoomHeading);

            var employeeShiftTime =
               new SPNavigationNode("Employee Shift Time", web.Url);
            nodeCollection.AddAsLast(employeeShiftTime);
            // Update title resource
            foreach (var culture in web.SupportedUICultures)
            {
                var timeSheetResourceKey = $"$Resources:{ResourceFilename},{EmployeeShiftTime}";
                var locStr = SPUtility.GetLocalizedString(timeSheetResourceKey, ResourceFilename,
                    (uint)culture.LCID);
                employeeShiftTime.TitleResource.SetValueForUICulture(culture, locStr);
            }
            employeeShiftTime.Update();

            var rAndDHeading = new SPNavigationNode("R&D", web.Url);
            nodeCollection.AddAsLast(rAndDHeading);
            // Update title resource
            foreach (var culture in web.SupportedUICultures)
            {
                var timeSheetResourceKey = $"$Resources:{ResourceFilename},{RAndD}";
                var locStr = SPUtility.GetLocalizedString(timeSheetResourceKey, ResourceFilename,
                    (uint)culture.LCID);
                rAndDHeading.TitleResource.SetValueForUICulture(culture, locStr);
            }
            rAndDHeading.Update();

            //Add child
            var dissoierForExportingRequest =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{DissoierForExportingRequest}",
                    web.Url);
            rAndDHeading.Children.AddAsLast(dissoierForExportingRequest);
            var dissoierForDomesticRequest =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{DissoierForDomesticRequest}",
                    web.Url);
            rAndDHeading.Children.AddAsLast(dissoierForDomesticRequest);
            var notConfirmingRequest =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{NotconformingRequest}", web.Url);
            rAndDHeading.Children.AddAsLast(notConfirmingRequest);
            var designRequest =
                new SPNavigationNode(
                    $"$Resources:{ResourceFilename},{DesignRequest}", web.Url);
            rAndDHeading.Children.AddAsLast(designRequest);
            rAndDHeading.Update();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //Deletes the links when feature is deactivated 
            SPWeb web = null;
            try
            {
                web = properties.Feature.Parent as SPWeb;
                if (web == null) return;
                web.AllowUnsafeUpdates = true;
                var nodeColl = web.Navigation.QuickLaunch;
                for (var nodeIndex = nodeColl.Count - 1; nodeIndex >= 0; nodeIndex--)
                {
                    if (nodeColl[nodeIndex].Children.Count > 0)
                    {
                        var childrenCollection = nodeColl[nodeIndex].Children;
                        for (var nodeChildIndex = childrenCollection.Count - 1; nodeChildIndex >= 0; nodeChildIndex--)
                            childrenCollection[nodeChildIndex].Delete();
                    }
                    nodeColl[nodeIndex].Delete();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA-Branding - FeatureDeactivating", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                if (web != null)
                    web.AllowUnsafeUpdates = false;
            }
        }

        //}
        //{

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)

        // Uncomment the method below to handle the event raised when a feature is upgrading.
        //}
        //{

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)


        // Uncomment the method below to handle the event raised before a feature is uninstalled.
        //}
        //{

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)


        // Uncomment the method below to handle the event raised after a feature has been installed.
    }
}
