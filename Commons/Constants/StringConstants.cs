using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Constants
{
    public enum TaskOutcome { Approved, Rejected };

    public static class TaskStatusList
    {
        public static string NotStarted { get { return "Not Started"; } }
        public static string InProgress { get { return "In Progress"; } }
        public static string Completed { get { return "Completed"; } }
        public static string Deferred { get { return "Deferred"; } }
        public static string Waiting { get { return "Waiting on someone else"; } }
    }

    public static class StepStatusList
    {
        public static string SLDApproval { get { return "Waiting SLD Approval/Chờ trưởng ca duyệt"; } }
        public static string TLEApproval { get { return "Waiting TLE Approval/Chờ tổ trưởng duyệt"; } }
        public static string DHApproval { get { return "Waiting DH Approval/Chờ trưởng phòng duyệt"; } }
        public static string BODApproval { get { return "Waiting BOD Approval/Chờ BGD duyệt"; } }
        public static string AdminAproval { get { return "Waiting Admin Aproval/Chờ phòng HC duyệt"; } }
        public static string AdminDirectorAproval { get { return "Waiting Admin Director Aproval/Chờ GĐ HC duyệt"; } }
        public static string DirectManagerAproval { get { return "Waiting Direct Manager Aproval/Chờ tổ trưởng duyệt"; } }
        public static string SecurityAproval { get { return "Waiting Security Aproval/Chờ bảo vệ duyệt"; } }
        public static string Approved { get { return "Approved/Đã được duyệt"; } }
        public static string Rejected { get { return "Rejected/Đã từ chối"; } }
    }

    public enum StepModuleList { LeaveManagement, VehicleManagement, ShiftManagement, OvertimeManagement, ChangeShiftManagement, NotOvertimeManagement, FreightManagement, BusinessTripManagement };

    public enum TypeOfDateRange { MonthYear = 0, FromTo };

    public class StringConstant
    {
        #region Batch operation definition
        public const string BatchNewKeyWord = "New";
        public const string BatchFormat = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                         "<ows:Batch OnError=\"Return\">{0}</ows:Batch>";
        public const string UrnPrefix = "urn:schemas-microsoft-com:office:office#";
        #endregion

        public const string DateFormatForCAML = "yyyy-MM-dd HH:mm:ss";

        public const string DateFormatTZForCAML = "yyyy-MM-ddTHH:mm:ssZ";

        public const string DateFormatddMMyyyy = "dd-MM-yyyy";
        public const string DateFormatddMMyyyy2 = "dd/MM/yyyy";

        public const string DateFormatMMddyyyy = "MM-dd-yyyy";

        public const string DateFormatddMMyyyyHHmm = "dd/MM/yyyy HH:mm";

        public const string DateFormatddMMyyyyHHmmss = "dd/MM/yyyy HH:mm:ss";

        public const string DateFormatMMddyyyySlash = "dd/MM/yyyy";

        public const string DateFormatddMMyyyyhhmmssttt = "dd/MM/yyy HH:mm:ss";

        public const string DateFormatyyyyMMddHHmmssfff = "yyyy-MM-ddTHH:mm:ss.fff";

        public static string SeparatorFactoryLocation = ";";

        public class EmployeeInfoList
        {

            /// <summary>
            /// List Employee url
            /// </summary>
            public const string ListUrl = "/Lists/EmployeeInfo";    // -> List

            /// <summary>
            /// Employee info Field Employee ID
            /// </summary>
            public const string EmployeeIDField = "EmployeeID";

            /// <summary>
            /// Employee info Field Employee Info FullName
            /// </summary>
            public const string FullNameField = "EmployeeDisplayName";

            /// <summary>
            /// Employee info Field Department Permission
            /// </summary>
            public const string DepartmentPermissionField = "DepartmentPermission";

            /// <summary>
            /// Employee info Field Employee AD Account
            /// </summary>
            public const string ADAccountField = "ADAccount";

            /// <summary>
            /// Employee info Field Employee Department
            /// </summary>
            public const string DepartmentField = "EmployeeInfoDepartment";

            /// <summary>
            /// Employee info Field Employee Password
            /// </summary>
            public const string PasswordField = "Password";
            /// <summary>
            /// Employee info Field Factory Location
            /// </summary>
            public const string FactoryLocationField = "CommonLocation";

            /// <summary>
            /// Employee info Field Employee Type
            /// </summary>
            public const string EmployeeTypeField = "EmployeeType";

            /// <summary>
            /// Employee level
            /// </summary>
            public const string EmployeeLevelField = "EmployeeLevel";

            /// <summary>
            /// Employee info Field IsActive
            /// </summary>
            public const string IsActiveField = "IsActive";

            /// <summary>
            /// Employee info EmployeePosition Field
            /// </summary>
            public const string EmployeePositionField = "Position";

            public const string EmployeeInfoManagerField = "EmployeeInfoManager";
            public const string EmailField = "Email";
            public const string PublishingPageImageField = "PublishingPageImage";
            public const string JoinedDateField = "JoinedDate";
            public const string DelegatedByField = "DelegatedBy";
        }

        public class SessionString
        {
            public const string EmployeeLogedin = "EmployeeLogin";
        }

        public class EmailTemplateList
        {
            public const string MailBodyField = "MailBody";
            public const string MailKeyField = "MailKey";
            public const string MailSubjectField = "MailSubject";
            public const string Title = "Title";
        }

        public class DepartmentList
        {
            public const string ListUrl = "/Lists/Departments";

            public const string NameField = "CommonName";
            public const string DepartmentNoField = "DepartmentNo";
            public const string VietnameseNameField = "CommonName1066";
            public const string SortOrderField = "SortOrder";
            public const string CodeField = "Code";
            public const string IsShiftRequestRequiredField = "IsShiftRequestRequired";
            public const string AdministratorField = "Administrator";
            public const string BODField = "BODApprover";
            public const string IsBODApprovalRequiredField = "IsBODApprovalRequired";
            public const string LocationsField = "CommonMultiLocations";
            public const string AutoCreateOverTime = "AutoCreateOverTime";
        }

        public class DefaultSPListField
        {
            public const string CreatedByField = "Author";
            public const string ModifiedByField = "Editor";
            public const string CreatedField = "Created";
            public const string ModifiedField = "Modified";
        }

        public class CommonSPListField
        {
            public const string RequesterField = "Requester";

            public const string CommonDepartmentField = "CommonDepartment";
            public const string DepartmentName1066Field = "DepartmentName1066";
            public const string CommonLocationField = "CommonLocation";

            public const string ApprovalStatusField = "ApprovalStatus";
            public const string CommonCommentField = "CommonComment";

            public const string ColForSortField = "ColForSort";

            public const string CommonReqDueDateField = "CommonReqDueDate";
            public const string CommonLastRemindField = "CommonLastRemind";
            public const string CommonNextRemindField = "CommonNextRemind";
            public const string CommonRemindedAttemptsField = "CommonRemindedAttempts";
        }

        public class OverTimeManagementList
        {
            public const string ListUrl = "/Lists/OvertimeManagement";
            public const string PlaceField = "Place";
            public const string SumOfEmployeeField = "SumOfEmployee";
            public const string SumOfMealField = "SumOfMeal";
            public const string OtherRequirementsField = "OtherRequirements";
            public const string DHCommentsField = "DHComments";
            public const string BODCommentsField = "BODComments";
            public const string SecurityCommentsField = "SecurityComments";
            public const string CommonDateField = "CommonDate";
            public const string ApprovedByField = "CommonApprover1";
            public const string ApprovalStatusField = "ApprovalStatus";
            public const string FirstApprovedByField = "CommonApprover2";
            public const string FirstApprovedDateField = "FirstApprovedDate";
        }

        public class OverTimeManagementDetailList
        {
            public const string Employee = "Employee";
            public const string EmployeeID = "EmployeeID";
            public const string OvertimeFrom = "OvertimeFrom";
            public const string OvertimeTo = "OvertimeTo";
            public const string OvertimeManagementID = "OvertimeManagementID";
            public const string Task = "Task";
            public const string HM = "HM";
            public const string KD = "KD";
            public const string CompanyTransport = "CompanyTransport";
            public const string WorkingHours = "WorkingHours";
            public const string SummaryLinks = "SummaryLinks";
        }

        public class OverTimeManagementReport
        {
            public const string ReportFileName = "Overtime Management Report";
        }
        /// <summary>
        /// Field names of Shift Management List
        /// </summary>
        public class ShiftManagementList
        {
            public const string ListUrl = "/Lists/ShiftManagement";
            public const string MonthField = "CommonMonth";
            public const string YearField = "CommonYear";
            public const string LocationField = "CommonLocation";
            public const string DepartmentField = "CommonDepartment";
            public const string ApprovedByField = "CommonApprover1";
            public const string CommonAddApprover1Field = "CommonAddApprover1";
        }

        public class RequestTypesList
        {
            public const string ListUrl = "/Lists/RequestTypes";
            public const string RequestTypeNameField = "RequestTypeName";
            public const string RequestsTypeField = "RequestType";
            public const string DepartmentsField = "Departments";
        }

        public class ShiftManagementDetailList
        {
            public const string ListUrl = "/Lists/ShiftManagementDetail";
            public const string ShiftManagementIDField = "ShiftManagementID";
            public const string MonthField = "CommonMonth";
            public const string YearField = "CommonYear";
            public const string EmployeeField = "Employee";
            public const string EmployeeIDField = "Employee_x003a_Employee_x0020_ID";
            public const string ShiftTime1Field = "ShiftTime1";
            public const string ShiftTime2Field = "ShiftTime2";
            public const string ShiftTime3Field = "ShiftTime3";
            public const string ShiftTime4Field = "ShiftTime4";
            public const string ShiftTime5Field = "ShiftTime5";
            public const string ShiftTime6Field = "ShiftTime6";
            public const string ShiftTime7Field = "ShiftTime7";
            public const string ShiftTime8Field = "ShiftTime8";
            public const string ShiftTime9Field = "ShiftTime9";
            public const string ShiftTime10Field = "ShiftTime10";
            public const string ShiftTime11Field = "ShiftTime11";
            public const string ShiftTime12Field = "ShiftTime12";
            public const string ShiftTime13Field = "ShiftTime13";
            public const string ShiftTime14Field = "ShiftTime14";
            public const string ShiftTime15Field = "ShiftTime15";
            public const string ShiftTime16Field = "ShiftTime16";
            public const string ShiftTime17Field = "ShiftTime17";
            public const string ShiftTime18Field = "ShiftTime18";
            public const string ShiftTime19Field = "ShiftTime19";
            public const string ShiftTime20Field = "ShiftTime20";
            public const string ShiftTime21Field = "ShiftTime21";
            public const string ShiftTime22Field = "ShiftTime22";
            public const string ShiftTime23Field = "ShiftTime23";
            public const string ShiftTime24Field = "ShiftTime24";
            public const string ShiftTime25Field = "ShiftTime25";
            public const string ShiftTime26Field = "ShiftTime26";
            public const string ShiftTime27Field = "ShiftTime27";
            public const string ShiftTime28Field = "ShiftTime28";
            public const string ShiftTime29Field = "ShiftTime29";
            public const string ShiftTime30Field = "ShiftTime30";
            public const string ShiftTime31Field = "ShiftTime31";
            public const string ShiftTime1ApprovalField = "ShiftTime1Approval";
            public const string ShiftTime2ApprovalField = "ShiftTime2Approval";
            public const string ShiftTime3ApprovalField = "ShiftTime3Approval";
            public const string ShiftTime4ApprovalField = "ShiftTime4Approval";
            public const string ShiftTime5ApprovalField = "ShiftTime5Approval";
            public const string ShiftTime6ApprovalField = "ShiftTime6Approval";
            public const string ShiftTime7ApprovalField = "ShiftTime7Approval";
            public const string ShiftTime8ApprovalField = "ShiftTime8Approval";
            public const string ShiftTime9ApprovalField = "ShiftTime9Approval";
            public const string ShiftTime10ApprovalField = "ShiftTime10Approval";
            public const string ShiftTime11ApprovalField = "ShiftTime11Approval";
            public const string ShiftTime12ApprovalField = "ShiftTime12Approval";
            public const string ShiftTime13ApprovalField = "ShiftTime13Approval";
            public const string ShiftTime14ApprovalField = "ShiftTime14Approval";
            public const string ShiftTime15ApprovalField = "ShiftTime15Approval";
            public const string ShiftTime16ApprovalField = "ShiftTime16Approval";
            public const string ShiftTime17ApprovalField = "ShiftTime17Approval";
            public const string ShiftTime18ApprovalField = "ShiftTime18Approval";
            public const string ShiftTime19ApprovalField = "ShiftTime19Approval";
            public const string ShiftTime20ApprovalField = "ShiftTime20Approval";
            public const string ShiftTime21ApprovalField = "ShiftTime21Approval";
            public const string ShiftTime22ApprovalField = "ShiftTime22Approval";
            public const string ShiftTime23ApprovalField = "ShiftTime23Approval";
            public const string ShiftTime24ApprovalField = "ShiftTime24Approval";
            public const string ShiftTime25ApprovalField = "ShiftTime25Approval";
            public const string ShiftTime26ApprovalField = "ShiftTime26Approval";
            public const string ShiftTime27ApprovalField = "ShiftTime27Approval";
            public const string ShiftTime28ApprovalField = "ShiftTime28Approval";
            public const string ShiftTime29ApprovalField = "ShiftTime29Approval";
            public const string ShiftTime30ApprovalField = "ShiftTime30Approval";
            public const string ShiftTime31ApprovalField = "ShiftTime31Approval";

        }
        public class VehicleManagementList
        {
            public const string ListUrl = "/Lists/VehicleManagement";
            public const string CompanyPickup = "CompanyPickup";
            public const string CommonFrom = "CommonFrom";
            public const string To = "To";
            public const string Reason = "Reason";
            public const string VehicleType = "VehicleType";
            public const string DH = "CommonApprover1";
            public const string BOD = "CommonApprover2";
        }
        public class LeaveManagementList
        {
            public const string ListUrl = "/Lists/LeaveManagement";
            public const string RequestForField = "RequestFor";
            public const string FromField = "CommonFrom";
            public const string ToField = "To";
            public const string LeaveHoursField = "LeaveHours";
            public const string TotalDaysField = "TotalDays";
            public const string ReasonField = "Reason";
            public const string TransferworkToField = "TransferworkTo";
            public const string LeftAtField = "LeftAt";
            public const string LeftField = "Left";
            public const string UnexpectedLeaveField = "UnexpectedLeave";
            public const string IsValidRequestField = "IsValidRequest";
            public const string TLEField = "CommonApprover1";
            public const string DHField = "CommonApprover2";
            public const string BODField = "CommonApprover3";
            public const string AdditionalApproverField = "CommonAddApprover1";
        }

        public class FreightManagementList
        {
            public const string ListUrl = "/Lists/FreightManagement";
            public const string RequestNoField = "RequestNo";
            public const string BringerField = "Bringer";
            public const string BringerDepartmentField = "BringerDepartment";
            public const string BringerLocationField = "BringerLocation";
            public const string BringerNameField = "BringerName";
            public const string CompanyNameField = "CompanyName";
            public const string CompanyVehicleField = "CompanyVehicle";
            public const string ReasonField = "Reason";
            public const string ReceiverField = "Receiver";
            public const string TransportTimeField = "TransportTime";
            public const string ReceiverDepartmentLookupField = "ReceiverDepartmentLookup";
            public const string ReceiverDepartmentVNField = "ReceiverDepartmentVN";
            public const string ReceiverDepartmentTextField = "ReceiverDepartmentText";
            public const string ReceiverPhoneField = "ReceiverPhone";
            public const string FreightTypeField = "FreightType";
            public const string ReturnedGoodsField = "ReturnedGoods";
            public const string HighPriorityField = "HighPriority";
            public const string OtherReasonField = "OtherReason";
            public const string IsValidRequestField = "IsValidRequest";
            public const string IsFinishedField = "IsFinished";
            public const string VehicleLookupField = "VehicleLookup";
            public const string VehicleVNField = "VehicleVN";
            public const string SecurityNotesField = "SecurityNotes";
            public const string DHField = "CommonApprover1";
            public const string BODField = "CommonApprover2";
            public const string AdminDeptField = "CommonApprover3";
        }

        public class FreightDetailsList
        {
            public const string ListUrl = "/Lists/FreightDetails";
            public const string FreightManagementIDField = "FreightManagementID";
            public const string GoodsNameField = "GoodsName";
            public const string UnitField = "Unit";
            public const string QuantityField = "Quantity";
            public const string RemarksField = "Remarks";
            public const string ShippingInField = "ShippingIn";
            public const string ShippingOutField = "ShippingOut";
            public const string CheckInByField = "CheckInBy";
            public const string CheckOutByField = "CheckOutBy";
        }

        public class FreightReceiverDepartmentList
        {
            public const string ListUrl = "/Lists/FreightReceiverDepartment";
            public const string ReceiverDepartmentField = "ReceiverDepartment";
            public const string ReceiverDepartmentVNField = "ReceiverDepartmentVN";
        }

        public class FreightVehicleList
        {
            public const string ListUrl = "/Lists/FreightVehicle";
            public class Fields
            {
                public const string Vehicle = "Vehicle";
                public const string VehicleVN = "VehicleVN";
            }
        }

        public class EmployeeShiftTimeList
        {
            public const string ListUrl = "/Lists/EmployeeShiftTime";

            public const string ShiftManagementIdField = "ShiftManagementID";
            public const string DateField = "StadaDate";
            public const string ShiftField = "Shift";
            public const string IsValidField = "IsValid";
        }
        public class StepManagementList
        {
            public const string ListUrl = "/Lists/StepList";
            public const string StepModule = "StepModule";
            public const string StepNumber = "StepNumber";
            public const string StepStatus = "CurrentStepStatus";
        }
        public class TaskManagementList
        {
            public const string ListUrl = "/Lists/TaskList";
            public const string PercentComplete = "PercentComplete";
            public const string AssignedTo = "AssignedTo";
            public const string Description = "Body";
            public const string DueDate = "DueDate";
            public const string ItemId = "ItemId";
            public const string ItemURL = "ItemURL";
            public const string ListURL = "ListURL";
            public const string StartDate = "StartDate";
            public const string StepStatus = "CurrentStepStatus";
            public const string TaskName = "Title";
            public const string TaskOutcome = "TaskOutcome";
            public const string TaskStatus = "Status";
            public const string NextAssign = "NextAssign";
            public const string StepModule = "StepModule";
            public const string RelatedTasks = "RelatedTasks";
        }

        public class ShiftTime
        {
            public const string ListUrl = "/Lists/ShiftTime";
            public const string NameField = "CommonName";
            public const string CodeField = "Code";
            public const string ShiftTimeWorkingHourFromField = "ShiftTimeWorkingHourFrom";
            public const string ShiftTimeWorkingHourToField = "ShiftTimeWorkingHourTo";
            public const string ShiftTimeWorkingHourMidField = "ShiftTimeWorkingHourMid";
            public const string ShiftTimeBreakingHourFromField = "ShiftTimeBreakingHourFrom";
            public const string ShiftTimeBreakingHourToField = "ShiftTimeBreakingHourTo";
            public const string ShiftTimeWorkingHourField = "ShiftTimeWorkingHour";
            public const string ShiftTimeBreakingHourField = "ShiftTimeBreakingHour";
            public const string DescriptionField = "StadaDescription";
            public const string UnexpectedLeaveFirstApprovalRoleField = "UnexpectedLeaveFirstApprovalRole";
            public const string ShiftRequiredField = "ShiftRequired";
        }

        public class ApprovalStatus
        {
            public const string Approved = "Approved";
            public const string Rejected = "Rejected";
            public const string InProgress = "In-Progress";
            public const string Cancelled = "Cancelled";
            public const string Completed = "Completed";
            public const string InProcess = "In-Process";
        }

        /// <summary>
        /// VietnameseApprovalStatus
        /// </summary>
        public class VietnameseApprovalStatus
        {
            /// <summary>
            /// Đã duyệt
            /// </summary>
            public const string Approved = "Đã duyệt";
            /// <summary>
            /// Từ chối 
            /// </summary>
            public const string Rejected = "Từ chối";
            /// <summary>
            /// Đang chờ duyệt
            /// </summary>
            public const string InProgress = "Đang chờ duyệt";
            /// <summary>
            /// Đã hủy
            /// </summary>
            public const string Cancelled = "Đã hủy";
            /// <summary>
            /// Hoàn thành
            /// </summary>
            public const string Completed = "Hoàn thành";
            /// <summary>
            /// Đang thực hiện
            /// </summary>
            public const string InProcess = "Đang thực hiện";
        }

        public static Dictionary<string, string> ApprovalStatusMapping = new Dictionary<string, string>
        {
            { ApprovalStatus.Approved, VietnameseApprovalStatus.Approved },
            { ApprovalStatus.Rejected, VietnameseApprovalStatus.Rejected },
            { ApprovalStatus.InProgress, VietnameseApprovalStatus.InProgress },
            { ApprovalStatus.InProcess, VietnameseApprovalStatus.InProcess },
            { ApprovalStatus.Cancelled, VietnameseApprovalStatus.Cancelled },
            { ApprovalStatus.Completed, VietnameseApprovalStatus.Completed }
        };


        public enum EnumApprovalStatus
        {
            Approved = 1,
            Rejected = 2,
            InProgress = 3,
            Cancelled = 4
        }

        public class EmployeeType
        {
            public const string CommonUser = "Common User";
            public const string ADUser = "AD User";
        }

        public class Group
        {
            public const string Administrators = "Administrators";
            public const string Contributors = "Contributors";
            public const string Members = "Members";
            public const string CommonAccountGroupName = "Common Accounts";
            public const string BODGroupName = "BOD";
        }

        public class CalendarList
        {
            public const string TitleField = "Title";
            public const string LocationField = "Location";
            public const string StartDateField = "EventDate";
            public const string EndDateField = "EndDate";
            public const string DescriptionField = "Description";
            public const string CategoryField = "Category";
        }

        public class CaledarCategory
        {
            public const string Holiday = "Holiday";
            public const string Weekend = "Weekend";
            public const string CompensationDayOff = "Compensation Day-Off";
        }

        public class GroupList
        {
            public const string NameField = "CommonName";
        }

        public class NotOvertimeList
        {
            public const string ListUrl = "/Lists/NotOverTimeManagement";
            public const string HourPerDayField = "HoursPerDay";
            public const string DateField = "CommonDate";
            public const string FromDateField = "CommonFrom";
            public const string ToDateField = "To";
            public const string ReasonField = "Reason";
            public const string AprovalStatusField = "ApprovalStatus";
            public const string DHField = "CommonApprover1";
            public const string BODField = "CommonApprover2";
        }

        public class ChangeShiftList
        {
            public const string ListUrl = "/Lists/ChangeShiftManagement";
            public const string FromDateField = "CommonFrom";
            public const string FromShiftField = "FromShift";
            public const string ToDateField = "To";
            public const string ToShiftField = "ToShift";
            public const string ReasonField = "Reason";
            public const string AprovalStatusField = "ApprovalStatus";
            public const string DHField = "CommonApprover1";
            public const string BODField = "CommonApprover2";
            public const string ChangeShiftRelatedTaskIdField = "ChangeShiftRelatedTaskId";
        }

        public class ModuleCategoryList
        {
            public const string NameField = "CommonName";
            public const string VietNameseNameField = "CommonName1066";
        }
        public class PermissionGroupList
        {
            public const string NameField = "CommonName";
            public const string VietNameseNameField = "CommonName1066";
            public const string IsOnLeftMenuField = "IsOnLeftMenu";
            public const string PermissionModuleCategoryField = "PermissionModuleCategory";
            public const string PermissionModuleCategoryVNField = "PermissionModuleCategoryVN";
            public const string PageNameFiled = "PageName";
            public const string LeftMenuOrderFiled = "LeftMenuOrder";
        }

        public enum EmployeePosition
        {
            /// <summary>
            /// Ban giam doc
            /// </summary>
            BOD = 1,
            /// <summary>
            /// Truong phong
            /// </summary>
            DepartmentHead = 2,
            /// <summary>
            /// Pho phong
            /// </summary>
            GroupLeader = 3,
            /// <summary>
            /// Van thu
            /// </summary>
            Administrator = 4,
            /// <summary>
            /// Nhan vien
            /// </summary>
            Employee = 5,
            /// <summary>
            /// Tap vu
            /// </summary>
            Helper = 6,
            /// <summary>
            /// Bao ve
            /// </summary>
            SecurityGuard = 7,
            /// <summary>
            /// Nhan vien cay xanh
            /// </summary>
            Gardener = 8,
            /// <summary>
            /// Quan ly truc tiep
            /// </summary>
            DirectManagement = 9,
            /// <summary>
            /// TeamLeader
            /// </summary>
            TeamLeader = 10,
            /// <summary>
            /// Associate Team Leader
            /// </summary>
            AssociateTeamLeader = 11,
            /// <summary>
            /// Shift Leader
            /// </summary>
            ShiftLeader = 12
        }

        public enum AdditionalEmployeePosition
        {
            Driver = 201,
            Accountant = 202
        }

        public enum EmployeeRole
        {
            /// <summary>
            /// Truong phong
            /// </summary>
            DepartmentHead = 1,

            /// <summary>
            /// Truong phong hanh chanh
            /// </summary>
            DepartmentHeadOfHR = 1,

            /// <summary>
            /// Van thu phong hanh chinh
            /// </summary>
            AdminOfHR = 2,

            /// <summary>
            /// BOD
            /// </summary>
            BOD = 3,

            /// <summary>
            /// Staff
            /// </summary>
            Staff = 4
        }

        public class EmployeePositionName
        {
            public const string BoardOfDirector = "Board of Director";

            public const string SecurityGuard = "Security Guard";
            public const string TeamLeader = "Team Leader";
            public const string ShiftLeader = "Shift Leader";
            public const string DepartmentHead = "Department Head";
        }

        public enum EmployeeLevel
        {
            /// <summary>
            /// Ban giam doc
            /// </summary>
            BOD = 7,
            /// <summary>
            /// Truong phong
            /// </summary>
            DepartmentHead = 5,
            /// <summary>
            /// Pho phong
            /// </summary>
            GroupLeader = 4,
            /// <summary>
            /// Van thu
            /// </summary>
            Administrator = 3,
            /// <summary>
            /// Nhan vien
            /// </summary>
            Employee = 2,
            /// <summary>
            /// Tap vu
            /// </summary>
            Helper = 1,
            /// <summary>
            /// Bao ve
            /// </summary>
            SecurityGuard = 1,
            /// <summary>
            /// Nhan vien cay xanh
            /// </summary>
            Gardener = 1,
            /// <summary>
            /// Quan ly truc tiep
            /// </summary>
            DirectManagement = 6,
        }

        public enum ChangeShiftErrorCode
        {
            RequestInProgress = 1,
            CannotApprove = 2,
            CannotReject = 3,
            Unexpected = 999
        }

        public enum NotOverTimeErrorCode
        {
            RequestInProgress = 1,
            CannotApprove = 2,
            CannotReject = 3,
            Unexpected = 999
        }

        public enum OverTimeErrorCode
        {
            RequestInProgress = 1,
            CannotApprove = 2,
            CannotReject = 3,
            CannotSubmit = 4,
            Unexpected = 999
        }

        public enum LeaveErrorCode
        {
            FromDateRelateToDate = 1,
            FromDateIsNoneWorkingDay = 2,
            ToDateIsNoneWorkingDay = 3,
            Policy1 = 4,
            Policy2 = 5,
            Policy3 = 6,
            Overlap = 7,
            FromDateInvalid = 8, //From date less than current date
            SequenceLeave = 9,
            RequestInProgress = 10,
            CannotApprove = 11,
            CannotReject = 12,
            InvalidData = 100,
            Unexpected = 101
        }

        public enum FreightErrorCode
        {
            RequestInProgress = 1,
            CannotApprove = 2,
            CannotReject = 3,
            CannotUpdate = 4,
            InvalidSubmitTime = 5,
            CannotCancel = 6,
            Unexpected = 999
        }

        public enum VehicleErrorCode
        {
            RequestInProgress = 1,
            CannotApprove = 2,
            CannotReject = 3,
            Unexpected = 999
        }

        public enum BusinessTripErrorCode
        {
            RequestInProgress = 1,
            CannotApprove = 2,
            CannotReject = 3,
            CannotUpdate = 4,
            CannotCancel = 5,
            Unexpected = 999
        }

        public enum VehiclePerson { Requester, BOD, DH }
        public enum VehicleTypeOfEmail { Request, Approve, Reject }

        #region Groups

        /// <summary>
        /// Group name Common Accounts
        /// </summary>
        public const string CommonAccounts = "Common Accounts";

        /// <summary>
        /// Group name Security
        /// </summary>
        public const string Security = "Security";

        /// <summary>
        /// Group name Administration Department
        /// </summary>
        public const string AdministrationDepartment = "Administration Department";

        /// <summary>
        /// Group name IT Members
        /// </summary>
        public const string ITMembers = "IT Members";

        /// <summary>
        /// Group name IT Contributors
        /// </summary>
        public const string ITContributors = "IT Contributors";

        /// <summary>
        /// Group name System Admin
        /// </summary>
        public const string SystemAdmin = "System Admin";

        /// <summary>
        /// Group name BOD
        /// </summary>
        public const string BOD = "BOD";

        public const string SecurityGuard = "Security Guard";
        #endregion

        #region Employee Info

        /// <summary>
        /// Log Category name
        /// </summary>
        public const string EmployeeInfoLogCategory = "EmployeeInfo";

        /// <summary>
        /// Employee Login page
        /// </summary>
        public const string EmployeeLogedin = "EmployeeLogin";

        /// <summary>
        /// Page Login Url
        /// </summary>
        public const string PageLoginURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/Login.aspx";

        /// <summary>
        /// Page Login Url
        /// </summary>
        public const string PageHomeURL = "/SitePages/Home.aspx";

        /// <summary>
        /// Page Change Password Url
        /// </summary>
        public const string PageChangePasswordURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangePassword.aspx";

        /// <summary>
        /// Page Reset Password Url
        /// </summary>
        public const string PageResetPasswordURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/ResetPassword.aspx";

        /// <summary>
        /// Page logout Url
        /// </summary>
        public const string PageLogoutURL = "/_layouts/15/RBVH.Stada.Intranet.WebPages/Logout.aspx";

        /// <summary>
        /// List Employee url
        /// </summary>
        public const string EmployeeInfoURL = "/Lists/EmployeeInfo";    // -> List

        /// <summary>
        /// Employee info Field Employee ID
        /// </summary>
        public const string EmployeeInfoFieldEmployeeID = "EmployeeID";

        /// <summary>
        /// Employee info Field Employee Info FullName
        /// </summary>
        public const string EmployeeInfoFieldFullName = "EmployeeInfoFullName";

        /// <summary>
        /// Employee info Field Employee AD Account
        /// </summary>
        public const string EmployeeInfoFieldADAccount = "ADAccount";

        /// <summary>
        /// Employee info Field Employee Department
        /// </summary>
        public const string EmployeeInfoFieldEmployeeInfoDepartment = "EmployeeInfoDepartment";

        /// <summary>
        /// Employee info Field Employee Password
        /// </summary>
        public const string EmployeeInfoFieldPassword = "Password";

        /// <summary>
        /// Overview url
        /// </summary>
        public const string PageOverviewURL = "/SitePages/Overview.aspx";

        #endregion

        #region Leave management
        public const string LeaveManagement_NewForm = "/Lists/LeaveManagement/NewForm.aspx";

        /// <summary>
        /// Leave management List url
        /// </summary>
        public const string LeaveManagementURL = "/Lists/LeaveManagement";

        /// <summary>
        /// Leave management New form url
        /// </summary>
        public const string LeaveManagementNewForm = "/Lists/LeaveManagement/NewForm.aspx";

        /// <summary>
        /// Leave management Site page url
        /// </summary>
        public const string SitePageLeaveManagementURL = "/SitePages/LeaveManagement.aspx";

        /// <summary>
        /// Security Leave management Site page url
        /// </summary>
        public const string SitePageSecurityLeaveManagementURL = "/SitePages/SecurityLeaveManagement.aspx";

        #endregion

        #region Resources

        /// <summary>
        /// Resource Webpages filename
        /// </summary>
        public const string ResourcesFileWebPages = "RBVHStadaWebpages";

        public const string ResourcesFileLists = "RBVHStadaLists";

        /// <summary>
        /// Resource key General Message Error
        /// </summary>
        public const string ResourcekeyGeneralMessageError = "General_Message_Error";

        public const string ResourcekeyGeneralTitleError = "General_Prefix_Error";

        public const string ResourcekeyGeneralTitleSuccess = "General_Prefix_Success";

        /// <summary>
        /// Resource key Current Password is invalid
        /// </summary>
        public const string ResourcesKeyChangePasswordCurrentPasswordIsInValid = "ChangePassword_CurrentPassword_IsInvalid";

        /// <summary>
        /// Resource key Message success
        /// </summary>
        public const string ResourcesKeyChangePasswordMessageSuccess = "ChangePassword_Message_Success";

        /// <summary>
        /// Resource key Message fail
        /// </summary>
        public const string ResourcesKeyChangePasswordMessageUnsuccess = "ChangePassword_Message_Unsuccess";

        /// <summary>
        /// Resource key Login fail
        /// </summary>
        public const string ResourcesKeyLoginMessageFail = "Login_Message_Fail";

        /// <summary>
        /// Resource key Reset password employee is invalid
        /// </summary>
        public const string ResourcesKeyResetPasswordEmployeeIsInvalid = "ResetPassword_Employee_IsInvalid";

        /// <summary>
        /// Resource key Reset password success
        /// </summary>
        public const string ResourcesKeyResetPasswordMessageSuccess = "ResetPassword_Message_Success";

        /// <summary>
        /// Resource key Reset password fail
        /// </summary>
        public const string ResourcesKeyResetPasswordMessageUnsuccess = "ResetPassword_Message_Unsuccess";


        #endregion

        #region Shift Management

        /// <summary>
        /// Shift management workflow name
        /// </summary>
        public const string ShiftManagementWorklowName = "ShiftManagementWorkflow";

        /// <summary>
        /// My Shift paging number
        /// </summary>
        public const int MyShiftPagingNumber = 10;

        /// <summary>
        /// My shift number of Next month
        /// </summary>
        public const int MyShiftNumberOfNextMonth = 1;

        /// <summary>
        /// Start Day Number
        /// </summary>
        public const int StartDayNumber = 21;

        /// <summary>
        /// End Day Number
        /// </summary>
        public const int EndDayNumber = 20;

        /// <summary>
        /// Date format for My Shift
        /// </summary>
        public const string DateFormatForMyShift = "MM/yyyy";

        /// <summary>
        /// list url
        /// </summary>
        public const string ShiftTimeURL = "/Lists/ShiftTime";

        /// <summary>
        /// Employee list url
        /// </summary>
        public const string EmployeeShiftTimeURL = "/Lists/EmployeeShiftTime";

        /// <summary>
        /// Shift management list url
        /// </summary>
        public const string ShiftManagementURL = "/Lists/ShiftManagement";

        /// <summary>
        /// Shift request SitePage url
        /// </summary>
        public const string SitePageShiftRequestURL = "/SitePages/ShiftRequest.aspx";

        #endregion

        #region Department
        public const string DepartmentURL = "/Lists/Departments";
        #endregion Department

        #region Factories
        public const string FactoriesURL = "/Lists/Factories";
        public const string FactoryLocation1 = "NM1";
        public const string FactoryLocation2 = "NM2";
        #endregion

        #region Overtime

        /// <summary>
        /// Overtime Employee Details List Url
        /// </summary>
        public const string OvertimeEmployeeDetailsURL = "/Lists/OvertimeEmployeeDetails";

        /// <summary>
        /// Overtime Management List Url
        /// </summary>
        public const string OvertimeManagementURL = "/Lists/OvertimeManagement";

        public const string SitePageOvertimeRequestURL = "/SitePages/OvertimeRequest.aspx";
        public const string SitePageOvertimeManagementURL = "/SitePages/OvertimeManagement.aspx";

        #endregion

        #region Change Shift Management

        /// <summary>
        /// Change Shift Management New form url
        /// </summary>
        public const string ChangeShiftManagementNewForm = "/Lists/ChangeShiftManagement/NewForm.aspx";

        /// <summary>
        /// Change Shift management site page url
        /// </summary>
        public const string SitePageChangeShiftManagementURL = "/SitePages/ChangeShiftManagement.aspx";
        public const string ChangeShiftManagement_NewForm = "/Lists/ChangeShiftManagement/NewForm.aspx";

        #endregion


        /// <summary>
        /// Session IsAdmin
        /// </summary>
        public const string IsAdmin = "IsAdmin";

        public const string IsAdminPartment = "IsAdminDepartment";

        public const int AdminDepartmentId = 2;

        public class AdditionalEmployeePositionLevelCode
        {
            public const int Driver = 201;
            public const int Cashier = 202;
            public const int VehicleOperator = 203;
            public const int ExtAdmin = 204;
            public const int SecurityGuard = 205;
        }

        public class AdditionalEmployeePositionModule
        {
            public const string BusinessTripManagement = "BusinessTripManagement";
            public const string FreightManagement = "FreightManagement";
            public const string LeaveManagement = "LeaveManagement";
        }

        public class WebPageLinks
        {
            public const string ChangeShiftMember = "_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementMember.aspx";
            public const string ChangeShiftAdmin = "_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementAdmin.aspx";
            public const string ChangeShiftManager = "_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementManager.aspx";
            public const string ChangeShiftBOD = "_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementBOD.aspx";

            public const string LeaveOfAbsenceManager = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementManager.aspx";
            public const string LeaveOfAbsenceMember = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementMember.aspx";
            public const string LeaveOfAbsenceAdmin = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementAdmin.aspx";
            public const string LeaveOfAbsenceBOD = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementBOD.aspx";

            public const string OvertimeMember = "_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementMember.aspx";
            public const string OvertimeAdmin = "_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementAdmin.aspx";
            public const string OvertimeManager = "_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementManager.aspx";
            public const string OvertimeBOD = "_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementBOD.aspx";

            public const string LeaveManagementMember = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementMember.aspx";
            public const string LeaveManagementAdmin = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementAdmin.aspx";
            public const string LeaveManagementManager = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementManager.aspx";
            public const string LeaveManagementBOD = "_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementBOD.aspx";

            public const string FreightManagementMember = "_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementMember.aspx";
            public const string FreightManagementAdmin = "_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementAdmin.aspx";
            public const string FreightManagementManager = "_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementManager.aspx";
            public const string FreightManagementBOD = "_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementBOD.aspx";

            public const string BusinessTripManagementMember = "_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementMember.aspx";
            public const string BusinessTripManagementAdmin = "_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementAdmin.aspx";
            public const string BusinessTripManagementManager = "_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementManager.aspx";
            public const string BusinessTripManagementBOD = "_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementBOD.aspx";

            public const string LoginPage = "_layouts/15/RBVH.Stada.Intranet.WebPages/Login.aspx";
        }

        #region Approval Lists

        #region WorkflowStepsList
        /// <summary>
        /// /Lists/WorkflowSteps
        /// </summary>
        public class WorkflowStepsList
        {
            /// <summary>
            /// /Lists/WorkflowSteps
            /// </summary>
            public const string Url = "/Lists/WorkflowSteps";

            public class Fields
            {
                /// <summary>
                /// ListName
                /// </summary>
                public const string ListName = "ListName";

                /// <summary>
                /// CurrentStep
                /// </summary>
                public const string CurrentStep = "CurrentStep";

                /// <summary>
                /// NextStep
                /// </summary>
                public const string NextStep = "NextStep";

                ///// <summary>
                ///// RejectToStep
                ///// </summary>
                //public const string RejectToStep = "RejectToStep";

                /// <summary>
                /// NotificationEmailToRoles
                /// </summary>
                public const string NotificationEmailToRoles = "NotificationEmailToRoles";

                /// <summary>
                /// NotificationEmailToEmployees
                /// </summary>
                public const string NotificationEmailToEmployees = "NotificationEmailToEmployees";

                /// <summary>
                /// NotificationEmailCcRoles
                /// </summary>
                public const string NotificationEmailCcRoles = "NotificationEmailCcRoles";

                /// <summary>
                /// NotificationEmailCcEmployees
                /// </summary>
                public const string NotificationEmailCcEmployees = "NotificationEmailCcEmployees";

                /// <summary>
                /// AllowReject
                /// </summary>
                public const string AllowReject = "AllowReject";

                ///// <summary>
                ///// ApprovalEmailTemplate
                ///// </summary>
                //public const string ApprovalEmailTemplate = "ApprovalEmailTemplate";

                ///// <summary>
                ///// RejectEmailTemplate
                ///// </summary>
                //public const string RejectEmailTemplate = "RejectEmailTemplate";

                /// <summary>
                /// ConditionalExpression
                /// </summary>
                public const string ConditionalExpression = "ConditionalExpression";

                /// <summary>
                /// OrderStep
                /// </summary>
                public const string OrderStep = "OrderStep";
            }
        }
        #endregion

        #region WorkflowHistoryList
        /// <summary>
        /// /Lists/WorkflowHistory
        /// </summary>
        public class WorkflowHistoryList
        {
            public const string Url = "/Lists/WorkflowHistory";

            public class Fields
            {
                /// <summary>
                /// Status
                /// </summary>
                public const string Status = "Status";

                /// <summary>
                /// VietnameseStatus
                /// </summary>
                public const string VietnameseStatus = "VietnameseStatus";

                /// <summary>
                /// PostedBy
                /// </summary>
                public const string PostedBy = "PostedBy";

                /// <summary>
                /// CommonDate
                /// </summary>
                public const string CommonDate = "CommonDate";

                /// <summary>
                /// CommonComment
                /// </summary>
                public const string CommonComment = "CommonComment";

                /// <summary>
                /// ListName
                /// </summary>
                public const string ListName = "ListName";

                /// <summary>
                /// CommonItemID
                /// </summary>
                public const string CommonItemID = "CommonItemID";
            }
        }
        #endregion

        #region EmailTemplatesList
        /// <summary>
        /// /Lists/WorkflowEmailTemplates
        /// </summary>
        public class WorkflowEmailTemplateList
        {
            public const string Url = "/Lists/WorkflowEmailTemplates";

            public class Fields
            {
                /// <summary>
                /// Key
                /// </summary>
                public const string Key = "Key";

                /// <summary>
                /// Subject
                /// </summary>
                public const string Subject = "Subject";

                /// <summary>
                /// Body
                /// </summary>
                public const string Body = "Body";

                /// <summary>
                /// Description
                /// </summary>
                public const string Description = "Description";

                /// <summary>
                /// ListName
                /// </summary>
                public const string ListName = "ListName";

                /// <summary>
                /// Action
                /// </summary>
                public const string Action = "Action";
            }
        }
        #endregion

        #endregion

        #region RequestsList
        /// <summary>
        /// /Lists/Requests
        /// </summary>
        public class RequestsList
        {
            /// <summary>
            /// /Lists/Requests
            /// </summary>
            public const string Url = "/Lists/Requests";

            /// <summary>
            /// Requests
            /// </summary>
            public const string ListName = "Requests";

            public const string TitleField = "Title";
            public const string RequestTypeRefField = "RequestTypeRef";
            public const string RequestTypeRefIdField = "RequestTypeRefId";
            public const string ReceviedByField = "ReceviedBy";
            public const string FinishDateField = "FinishDate";
            public const string RequiredApprovalByBODField = "RequiredApprovalByBOD";
            public const string ReferRequestField = "ReferRequest";

            public const string CommonCreatorField = "Creator";
            public const string ApprovalStatusField = "ApprovalStatus";
            public const string PendingAtField = "PendingAt";
            public const string CurrentStepField = "CurrentStep";
            public const string NextStepField = "NextStep";
            public const string CommonLocationField = "CommonLocation";
            public const string CommonDepartmentField = "CommonDepartment";
            public const string IsAdditionalStepField = "IsAdditionalStep";
            public const string AdditionalPrevisousStepField = "AdditionalPrevisousStep";
            public const string AdditionalStepField = "AdditionalStep";
            public const string AdditionalNextStepField = "AdditionalNextStep";
            public const string AdditionalDepartmentField = "AdditionalDepartment";
            public const string AssignFromField = "AssignFrom";
            public const string AssignToField = "AssignTo";
            public const string CommonDueDateField = "CommonDueDate";
        }
        #endregion

        #region RequestBuyDetailsList
        /// <summary>
        /// /Lists/BuyDetails
        /// </summary>
        public class RequestBuyDetailsList
        {
            /// <summary>
            /// /Lists/RequestBuyDetails
            /// </summary>
            public const string Url = "/Lists/RequestBuyDetails";

            public const string ListName = "RequestBuyDetails";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";

                /// <summary>
                /// Content
                /// </summary>
                public const string Content = "RequestContent";

                /// <summary>
                /// Form
                /// </summary>
                public const string Form = "Form";

                /// <summary>
                /// Unit
                /// </summary>
                public const string Unit = "Unit";

                /// <summary>
                /// Quantity
                /// </summary>
                public const string Quantity = "Quantity";

                /// <summary>
                /// Title
                /// </summary>
                public const string Reason = "Reason";

                /// <summary>
                /// Request
                /// </summary>
                public const string Request = "Request";
            }
        }
        #endregion

        #region RequestRepairDetailsList
        /// <summary>
        /// /Lists/RequestRepairDetails
        /// </summary>
        public class RequestRepairDetailsList
        {
            /// <summary>
            /// /Lists/BuyDetails
            /// </summary>
            public const string Url = "/Lists/RequestRepairDetails";

            public const string ListName = "RequestRepairDetails";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";

                /// <summary>
                /// Content
                /// </summary>
                public const string Content = "RequestContent";

                /// <summary>
                /// Reason
                /// </summary>
                public const string Reason = "Reason";

                /// <summary>
                /// Place
                /// </summary>
                public const string Place = "Place";

                /// <summary>
                /// From
                /// </summary>
                public const string From = "CommonFrom";

                /// <summary>
                /// To
                /// </summary>
                public const string To = "To";

                /// <summary>
                /// Request
                /// </summary>
                public const string Request = "Request";
            }
        }
        #endregion

        #region RequestOtherDetailsList
        /// <summary>
        /// /Lists/RequestOtherDetails
        /// </summary>
        public class RequestOtherDetailsList
        {
            /// <summary>
            /// /Lists/RequestOtherDetails
            /// </summary>
            public const string Url = "/Lists/RequestOtherDetails";

            public const string ListName = "RequestOtherDetails";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";

                /// <summary>
                /// Content
                /// </summary>
                public const string Content = "RequestContent";

                /// <summary>
                /// Unit
                /// </summary>
                public const string Unit = "Unit";

                /// <summary>
                /// Quantity
                /// </summary>
                public const string Quantity = "Quantity";

                /// <summary>
                /// Reason
                /// </summary>
                public const string Reason = "Reason";

                /// <summary>
                /// Request
                /// </summary>
                public const string Request = "Request";
            }
        }
        #endregion

        #region SupportingDocuments
        /// <summary>
        /// SupportingDocumentsList
        /// </summary>
        public class SupportingDocumentsList
        {
            /// <summary>
            /// /SupportingDocuments
            /// </summary>
            public const string Url = "/Lists/SupportingDocuments";

            /// <summary>
            /// SupportingDocuments
            /// </summary>
            public const string ListName = "SupportingDocuments";

            public class Fields
            {
                /// <summary>
                /// ListItemID
                /// </summary>
                public const string ListItemID = "ListItemID";
            }
        }
        #endregion

        #region EmployeeRequirementSheetsList
        /// <summary>
        /// EmployeeRequirementSheetsList
        /// </summary>
        public class EmployeeRequirementSheetsList
        {
            /// <summary>
            /// /Lists/EmployeeRequirementSheets
            /// </summary>
            public const string Url = "/Lists/EmployeeRequirementSheets";

            /// <summary>
            /// EmployeeRequirementSheets
            /// </summary>
            public const string ListName = "Employee requirement sheets";

            /// <summary>
            /// Fields
            /// </summary>
            public class Fields
            {
                public const string Title = "Title";
                public const string RecruitmentDepartment = "RecruitmentDepartment";
                public const string DepartmentCode = "DepartmentCode";
                public const string Position = "Position";
                public const string Quantity = "Quantity";
                public const string ReasonsForRecruitment = "ReasonsForRecruitment";
                public const string Sex = "Sex";
                //public const string Age = "Age";
                public const string FromAge = "FromAge";
                public const string ToAge = "ToAge";
                public const string AvailableTime = "AvailableTime";
                public const string MaritalStatus = "MaritalStatus";
                public const string WorkingTime = "WorkingTime";
                public const string EducationLevel = "EducationLevel";
                public const string Appearance = "Appearance";
                public const string WorkingExperience = "WorkingExperience";
                public const string Specialities = "Specialities";
                public const string DescriptionOfBasicWork = "DescriptionOfBasicWork";
                public const string MoralVocations = "MoralVocations";
                public const string WorkingAbilities = "WorkingAbilities";
                //public const string EnglishSkills = "EnglishSkills";
                public const string ComputerSkills = "ComputerSkills";
                public const string OtherSkills = "OtherSkills";
                public const string OtherRequirement = "OtherRequirement";
                public const string IsValidRequest = "IsValidRequest";
                public const string IsTemplate = "IsTemplate";

                public const string CommonCreatorField = "Creator";
                public const string ApprovalStatusField = "ApprovalStatus";
                public const string PendingAtField = "PendingAt";
                public const string CurrentStepField = "CurrentStep";
                public const string NextStepField = "NextStep";
                public const string CommonLocationField = "CommonLocation";
                public const string CommonDepartmentField = "CommonDepartment";
                public const string IsAdditionalStepField = "IsAdditionalStep";
                public const string AdditionalPrevisousStepField = "AdditionalPrevisousStep";
                public const string AdditionalStepField = "AdditionalStep";
                public const string AdditionalNextStepField = "AdditionalNextStep";
                public const string AdditionalDepartmentField = "AdditionalDepartment";
                public const string AssignFromField = "AssignFrom";
                public const string AssignToField = "AssignTo";
                public const string CommonDueDateField = "CommonDueDate";
            }
        }

        /// <summary>
        /// ForeignLanguagesList
        /// </summary>
        public class ForeignLanguagesList
        {
            /// <summary>
            /// /Lists/ForeignLanguages
            /// </summary>
            public const string Url = "/Lists/ForeignLanguages";

            /// <summary>
            /// ForeignLanguages
            /// </summary>
            public const string ListName = "ForeignLanguages";

            public class Fields
            {
                /// <summary>
                /// Name
                /// </summary>
                public const string Name = "CommonName";

                /// <summary>
                /// VietnameseName
                /// </summary>
                public const string VietnameseName = "CommonName1066";
            }
        }

        /// <summary>
        /// ForeignLanguageLevelsList
        /// </summary>
        public class ForeignLanguageLevelsList
        {
            /// <summary>
            /// /Lists/ForeignLanguageLevels
            /// </summary>
            public const string Url = "/Lists/ForeignLanguageLevels";

            /// <summary>
            /// ForeignLanguageLevels
            /// </summary>
            public const string ListName = "ForeignLanguageLevels";

            public class Fields
            {
                ///// <summary>
                ///// Level
                ///// </summary>
                //public const string Level = "Level";

                /// <summary>
                /// Level
                /// </summary>
                public const string Title = "Title";
            }
        }

        /// <summary>
        /// RecruitmentLanguageSkillsList
        /// </summary>
        public class RecruitmentLanguageSkillsList
        {
            /// <summary>
            /// /Lists/RecruitmentLanguageSkills
            /// </summary>
            public const string Url = "/Lists/RecruitmentLanguageSkills";

            /// <summary>
            /// RecruitmentLanguageSkills
            /// </summary>
            public const string ListName = "RecruitmentLanguageSkills";

            public class Fields
            {
                /// <summary>
                /// ForeignLanguage
                /// </summary>
                public const string ForeignLanguage = "ForeignLanguage";

                /// <summary>
                /// Level
                /// </summary>
                public const string Level = "Level";

                /// <summary>
                /// Request
                /// </summary>
                public const string Request = "Request";
            }
        }

        /// <summary>
        /// RecruitmentTeamList
        /// </summary>
        public class RecruitmentTeamList
        {
            /// <summary>
            /// /Lists/RecruitmentTeam
            /// </summary>
            public const string Url = "/Lists/RecruitmentTeam";

            /// <summary>
            /// Recruitment Team
            /// </summary>
            public const string ListName = "Recruitment Team";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";

                /// <summary>
                /// Employees
                /// </summary>
                public const string Employees = "Employees";
            }
        }
        #endregion

        #region RequestForDiplomaSuppliesList
        /// <summary>
        /// RequestForDiplomaSuppliesList
        /// </summary>
        public class RequestForDiplomaSuppliesList
        {
            /// <summary>
            /// /Lists/RequestForDiplomaSupplies
            /// </summary>
            public const string Url = "/Lists/RequestForDiplomaSupplies";

            /// <summary>
            /// Request for diploma supplies
            /// </summary>
            public const string ListName = "Request for diploma supplies";

            /// <summary>
            /// Fields
            /// </summary>
            public class Fields
            {
                public const string Title = "Title";
                public const string Employee = "Employee";
                public const string EmployeeName = "EmployeeName";
                public const string EmployeeCode = "EmployeeCode";
                public const string Position = "Position";
                public const string DateOfEmp = "DateOfEmp";
                public const string ToTheDailyWorks = "ToTheDailyWorks";
                public const string NewSuggestions = "NewSuggestions";
                public const string DiplomaRevision = "DiplomaRevision";
                public const string SalaryRevision = "SalaryRevision";
                public const string CommonFrom = "CommonFrom";

                public const string CommonCreatorField = "Creator";
                public const string ApprovalStatusField = "ApprovalStatus";
                public const string PendingAtField = "PendingAt";
                public const string CurrentStepField = "CurrentStep";
                public const string NextStepField = "NextStep";
                public const string CommonLocationField = "CommonLocation";
                public const string CommonDepartmentField = "CommonDepartment";
                public const string IsAdditionalStepField = "IsAdditionalStep";
                public const string AdditionalPrevisousStepField = "AdditionalPrevisousStep";
                public const string AdditionalStepField = "AdditionalStep";
                public const string AdditionalNextStepField = "AdditionalNextStep";
                public const string AdditionalDepartmentField = "AdditionalDepartment";
                public const string AssignFromField = "AssignFrom";
                public const string AssignToField = "AssignTo";
                public const string CommonDueDateField = "CommonDueDate";
            }
        }

        /// <summary>
        /// RequestDiplomaDetailsList
        /// </summary>
        public class RequestDiplomaDetailsList
        {
            /// <summary>
            /// /Lists/RequestDiplomaDetails
            /// </summary>
            public const string Url = "/Lists/RequestDiplomaDetails";

            /// <summary>
            /// Request diploma details
            /// </summary>
            public const string ListName = "Request diploma details";

            public class Fields
            {
                public const string Title = "Title";
                public const string CurrentDiploma = "CurrentDiploma";
                public const string GraduationYear = "GraduationYear";
                public const string NewDiploma = "NewDiploma";
                public const string Faculty = "Faculty";
                public const string IssuedPlace = "IssuedPlace";
                public const string TrainingDuration = "TrainingDuration";
                public const string Request = "Request";
            }
        }
        #endregion

        #region BusinessTripEmployeeDetailsList
        public class BusinessTripEmployeeDetailsList
        {
            public const string Url = "/Lists/BusinessTripEmployeeDetails";
            public class Fields
            {
                public const string BusinessTripManagementID = "BusinessTripManagementID";
                public const string ApprovalStatus = "ApprovalStatus";
                public const string Employee = "Employee";
                public const string EmployeeID = "EmployeeID";
            }
        }
        #endregion

        #region BusinessTripManagementList
        public class BusinessTripManagementList
        {
            public const string Url = "/Lists/BusinessTripManagement";
            public class Fields
            {
                public const string Domestic = "Domestic";
                public const string BusinessTripPurpose = "BusinessTripPurpose";
                public const string HotelBooking = "HotelBooking";
                public const string TripHighPriority = "TripHighPriority";
                public const string PaidBy = "PaidBy";
                public const string OtherService = "OtherService";
                public const string TransportationType = "TransportationType";
                public const string OtherTransportationDetail = "OtherTransportationDetail";
                public const string HasVisa = "HasVisa";
                public const string CashRequestDetail = "CashRequestDetail";
                public const string OtherRequestDetail = "OtherRequestDetail";
                public const string Driver = "Driver";
                public const string Cashier = "Cashier";
                public const string DH = "CommonApprover1";
                public const string DirectBOD = "CommonApprover2";
                public const string BOD = "CommonApprover3";
                public const string AdminDept = "CommonApprover4";
            }
        }
        #endregion

        #region BusinessTripScheduleList
        public class BusinessTripScheduleList
        {
            public const string Url = "/Lists/BusinessTripSchedule";
            public class Fields
            {
                public const string BusinessTripManagementID = "BusinessTripManagementID";
                public const string DepartDate = "DepartDate";
                public const string FlightName = "FlightName";
                public const string City = "City";
                public const string Country = "Country";
                public const string ContactCompany = "ContactCompany";
                public const string ContactPhone = "ContactPhone";
                public const string OtherSchedule = "OtherSchedule";
            }
        }
        #endregion

        #region ConfigurationList
        public class ConfigurationList
        {
            /// <summary>
            /// /Lists/ForeignLanguageLevels
            /// </summary>
            public const string Url = "/Lists/Configurations";

            /// <summary>
            /// ForeignLanguageLevels
            /// </summary>
            public const string ListName = "Configurations";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";

                /// <summary>
                /// Key
                /// </summary>
                public const string Key = "Key";

                /// <summary>
                /// Value
                /// </summary>
                public const string Value = "Value";

                /// <summary>
                /// Description
                /// </summary>
                public const string Description = "Description";
            }
        }
        #endregion

        #region AdditionalEmployeePosition
        public class AdditionalEmployeePositionList
        {
            public const string Url = "/Lists/AdditionalEmployeePosition";
            public class Fields
            {
                public const string Module = "Module";
                public const string Employee = "Employee";
                public const string EmployeeID = "EmployeeID";
                public const string EmployeeLevel = "EmployeeLevel";
            }
        }
        #endregion

        #region Delegation

        /// <summary>
        /// DelegationsList
        /// </summary>
        public class DelegationsList
        {
            /// <summary>
            /// /Lists/Delegations
            /// </summary>
            public const string Url = "/Lists/Delegations";
            /// <summary>
            /// Delegations
            /// </summary>
            public const string ListName = "Delegations";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";
                /// <summary>
                /// ModuleName
                /// </summary>
                public const string ModuleName = "ModuleName";
                /// <summary>
                /// VietnameseModuleName
                /// </summary>
                public const string VietnameseModuleName = "VietnameseModuleName";
                /// <summary>
                /// FromDate
                /// </summary>
                public const string FromDate = "FromDate";
                /// <summary>
                /// ToDate
                /// </summary>
                public const string ToDate = "ToDate";
                /// <summary>
                /// FromEmployee
                /// </summary>
                public const string FromEmployee = "FromEmployee";
                /// <summary>
                /// ToEmployee
                /// </summary>
                public const string ToEmployee = "ToEmployee";
                /// <summary>
                /// Requester
                /// </summary>
                public const string Requester = "Requester";
                /// <summary>
                /// Department
                /// </summary>
                public const string Department = "Department";
                /// <summary>
                /// ListUrl
                /// </summary>
                public const string ListUrl = "ListURL";
                /// <summary>
                /// ListItemID
                /// </summary>
                public const string ListItemID = "ListItemID";
                /// <summary>
                /// ListItemDescription
                /// </summary>
                public const string ListItemDescription = "ListItemDescription";
                /// <summary>
                /// ListItemCreatedDate
                /// </summary>
                public const string ListItemCreatedDate = "ListItemCreatedDate";
                /// <summary>
                /// ListItemApprovalUrl
                /// </summary>
                public const string ListItemApprovalUrl = "ListItemApprovalUrl";
            }
        }

        /// <summary>
        /// DelegationsOfNewTaskList
        /// </summary>
        public class DelegationsOfNewTaskList
        {
            /// <summary>
            /// /Lists/DelegationsOfNewTask
            /// </summary>
            public const string Url = "/Lists/DelegationsOfNewTask";
            /// <summary>
            /// DelegationsOfNewTask
            /// </summary>
            public const string ListName = "DelegationsOfNewTask";

            public class Fields
            {
                /// <summary>
                /// ModuleName
                /// </summary>
                public const string ModuleName = "ModuleName";
                /// <summary>
                /// VietnameseModuleName
                /// </summary>
                public const string VietnameseModuleName = "VietnameseModuleName";
                /// <summary>
                /// FromDate
                /// </summary>
                public const string FromDate = "FromDate";
                /// <summary>
                /// ToDate
                /// </summary>
                public const string ToDate = "ToDate";
                /// <summary>
                /// FromEmployee
                /// </summary>
                public const string FromEmployee = "FromEmployee";
                /// <summary>
                /// ToEmployee
                /// </summary>
                public const string ToEmployee = "ToEmployee";
                /// <summary>
                /// ListUrl
                /// </summary>
                public const string ListUrl = "ListURL";
            }
        }

        /// <summary>
        /// DelegationModulesList
        /// </summary>
        public class DelegationModulesList
        {
            /// <summary>
            /// /Lists/DelegationModules
            /// </summary>
            public const string Url = "/Lists/DelegationModules";
            /// <summary>
            /// DelegationModules
            /// </summary>
            public const string ListName = "DelegationModules";

            public class Fields
            {
                /// <summary>
                /// ModuleName
                /// </summary>
                public const string ModuleName = "ModuleName";
                /// <summary>
                /// VietnameseModuleName
                /// </summary>
                public const string VietnameseModuleName = "VietnameseModuleName";
                /// <summary>
                /// ListUrl
                /// </summary>
                public const string ListUrl = "ListURL";
            }
        }

        /// <summary>
        /// DelegationEmployeePositionsList
        /// </summary>
        public class DelegationEmployeePositionsList
        {
            /// <summary>
            /// /Lists/DelegationEmployeePositions
            /// </summary>
            public const string Url = "/Lists/DelegationEmployeePositions";
            /// <summary>
            /// DelegationEmployeePositions
            /// </summary>
            public const string ListName = "DelegationEmployeePositions";

            public class Fields
            {
                /// <summary>
                /// EmployeePosition
                /// </summary>
                public const string EmployeePosition = "EmployeePosition";
                /// <summary>
                /// DelegatedEmployeePositions
                /// </summary>
                public const string DelegatedEmployeePositions = "DelegatedEmployeePositions";
            }
        }

        #endregion

        #region ReceivedDepartmentRequestViewer
        /// <summary>
        /// ReceivedDepartmentRequestViewersList
        /// </summary>
        public class RequestReceivedDepartmentViewersList
        {
            /// <summary>
            /// /Lists/RequestReceivedDepartmentViewers
            /// </summary>
            public const string Url = "/Lists/RequestReceivedDepartmentViewers";
            /// <summary>
            /// RequestReceivedDepartmentViewers
            /// </summary>
            public const string ListName = "RequestReceivedDepartmentViewers";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";
                /// <summary>
                /// Location
                /// </summary>
                public const string Location = "CommonLocation";
                /// <summary>
                /// Department
                /// </summary>
                public const string Department = "CommonDepartment";
                /// <summary>
                /// Employees
                /// </summary>
                public const string Employees = "Employees";
            }
        }
        #endregion

        #region RequisitionOfMeetingRoomList
        /// <summary>
        /// RequisitionOfMeetingRoomList
        /// </summary>
        public class RequisitionOfMeetingRoomList
        {
            /// <summary>
            /// /Lists/RequisitionOfMeetingRoom
            /// </summary>
            public const string Url = "/Lists/RequisitionOfMeetingRoom";
            /// <summary>
            /// Requisition of meeting room
            /// </summary>
            public const string ListName = "Requisition of meeting room";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";
                /// <summary>
                /// StartDate
                /// </summary>
                public const string StartDate = "StartDate";
                /// <summary>
                /// EndDate
                /// </summary>
                public const string EndDate = "EndDate";
                /// <summary>
                /// DiscussionMeeting
                /// </summary>
                public const string DiscussionMeeting = "DiscussionMeeting";
                /// <summary>
                /// Participation
                /// </summary>
                public const string Participation = "Participation";
                /// <summary>
                /// MeetingRoomLocation
                /// </summary>
                public const string MeetingRoomLocation = "MeetingRoomLocation";
                /// <summary>
                /// Equipment
                /// </summary>
                public const string Equipment = "Equipment";
                /// <summary>
                /// EquipmentVN
                /// </summary>
                public const string EquipmentVN = "EquipmentVN";
                /// <summary>
                /// Seats
                /// </summary>
                public const string Seats = "Seats";
                /// <summary>
                /// Others
                /// </summary>
                public const string Others = "Others";

                /// <summary>
                /// PendingAt
                /// </summary>
                public const string PendingAtField = "PendingAt";
            }
        }

        #endregion

        #region MeetingRoomList

        /// <summary>
        /// MeetingRoomsList
        /// </summary>
        public class MeetingRoomsList
        {
            /// <summary>
            /// /Lists/MeetingRooms
            /// </summary>
            public const string Url = "/Lists/MeetingRooms";
            /// <summary>
            /// MeetingRooms
            /// </summary>
            public const string ListName = "MeetingRooms";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";
            }
        }

        #endregion

        #region EquipmentList
        public class EquipmentList
        {
            /// <summary>
            /// /Lists/Equipments
            /// </summary>
            public const string Url = "/Lists/Equipments";
            /// <summary>
            /// Equipments
            /// </summary>
            public const string ListName = "Equipments";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";

                /// <summary>
                /// CommonName
                /// </summary>
                public const string CommonName = "CommonName";

                /// <summary>
                /// CommonName1066
                /// </summary>
                public const string CommonName1066 = "CommonName1066";
            }
        }
        #endregion

        #region GuestReceptionManagementList
        /// <summary>
        /// GuestReceptionManagementList
        /// </summary>
        public class GuestReceptionManagementList
        {
            /// <summary>
            /// /Lists/GuestReceptionManagement
            /// </summary>
            public const string Url = "/Lists/GuestReceptionManagement";
            /// <summary>
            /// Guest reception management
            /// </summary>
            public const string ListName = "Guest reception management";

            public class Fields
            {
                /// <summary>
                /// Title
                /// </summary>
                public const string Title = "Title";
                
                
                /// <summary>
                /// PendingAt
                /// </summary>
                public const string PendingAtField = "PendingAt";
            }
        }
        #endregion
    }
}