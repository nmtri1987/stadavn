using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using BizConstants = RBVH.Stada.Intranet.Biz.Constants;


namespace RBVH.Stada.Intranet.WebPages.Common
{
    /// <summary>
    /// ListBaseUserUserControl
    /// </summary>
    public class ListBaseUserUserControl : UserControl
    {
        #region Constants

        /// <summary>
        /// active
        /// </summary>
        protected const string ActivedTabClass = "active";

        /// <summary>
        /// Tab
        /// </summary>
        public const string TabParamName = "Tab";

        /// <summary>
        /// DepartmentId
        /// </summary>
        public const string DepartmentParamName = "DepartmentId";

        /// <summary>
        /// Month
        /// </summary>
        public const string MonthParamname = "Month";

        /// <summary>
        /// Year
        /// </summary>
        public const string YearParamName = "Year";

        /// <summary>
        /// From Date
        /// </summary>
        public const string FromDateParamName = "FromDate";

        /// <summary>
        /// To Date
        /// </summary>
        public const string ToDateParamName = "ToDate";

        public const int Department_All = 0;

        public const string CancelWFCommandName = "CancelWF";

        /// <summary>
        /// ApprovalStatus
        /// </summary>
        protected const string EntityApprovalStatusPropertyName = "ApprovalStatus";

        #region Session Key
        /// <summary>
        /// MyRequests
        /// </summary>
        protected const string MyRequestsSessionKey = "MyRequests";

        /// <summary>
        /// RequestsToBeApproved
        /// </summary>
        protected const string RequestsToBeApprovedSessionKey = "RequestsToBeApproved";

        /// <summary>
        /// RequestsByDepartment
        /// </summary>
        protected const string RequestsByDepartmentSessionKey = "RequestsByDepartment";
        #endregion

        #region Control Id

        /// <summary>
        /// MyRequestLi
        /// </summary>
        protected const string MyRequestLiId = "MyRequestLi";

        /// <summary>
        /// RequestToBeApprovedLi
        /// </summary>
        protected const string RequestToBeApprovedLiId = "RequestToBeApprovedLi";

        /// <summary>
        /// RequestByDepartmentLi
        /// </summary>
        protected const string RequestByDepartmentLiId = "RequestByDepartmentLi";

        /// <summary>
        /// MyRequestsTab
        /// </summary>
        protected const string MyRequestsTabId = "MyRequestsTab";

        /// <summary>
        /// RequestsToBeApprovedTab
        /// </summary>
        protected const string RequestsToBeApprovedTabId = "RequestsToBeApprovedTab";

        /// <summary>
        /// RequestsByDepartmentTab
        /// </summary>
        public const string RequestsByDepartmentTabId = "RequestsByDepartmentTab";

        /// <summary>
        /// linkAddNewItem
        /// </summary>
        protected const string linkAddNewItemId = "linkAddNewItem";

        /// <summary>
        /// upMyRequests
        /// </summary>
        protected const string UpdatePanelMyRequestsId = "upMyRequests";

        /// <summary>
        /// upRequestToBeApproved
        /// </summary>
        protected const string UpdatePanelRequestToBeApprovedId = "upRequestToBeApproved";

        /// <summary>
        /// upRequestByDepartment
        /// </summary>
        protected const string UpdatePanelRequestByDepartmentId = "upRequestByDepartment";

        /// <summary>
        /// gridMyRquests
        /// </summary>
        protected const string gridMyRquestsId = "gridMyRquests";

        /// <summary>
        /// gridRequestToBeApproved
        /// </summary>
        protected const string gridRequestToBeApprovedId = "gridRequestToBeApproved";

        /// <summary>
        /// gridRequestByDepartment
        /// </summary>
        protected const string gridRequestByDepartmentId = "gridRequestByDepartment";

        /// <summary>
        /// hdActivedTab
        /// </summary>
        protected const string hdActivedTabId = "hdActivedTab";

        /// <summary>
        /// ddlDepartments
        /// </summary>
        protected const string ddlDepartmentsId = "ddlDepartments";

        /// <summary>
        /// txtMonth
        /// </summary>
        protected const string txtMonthId = "txtMonth";

        /// <summary>
        /// txtFromDateId
        /// </summary>
        protected const string txtFromDateId = "txtFromDate";

        /// <summary>
        /// txtToDate
        /// </summary>
        protected const string txtToDateId = "txtToDate";

        #endregion

        #endregion

        #region Attributes

        protected string listUrl;
        private string listName;
        private string listId;
        private SPList spListObject;
        private string currentUrl;
        private EmployeeInfo currentEmployeeInfoObj;
        private int currentEmployeeID;
        private string currentEmployeePositionCode;
        private string currentEmployeeType;
        private int currentEmployeeLocationId;
        private int currentEmployeeDepartmentId;
        private string currentEmployeeDepartmentCode;
        private EmployeePositionDAL employeePositionDAL;
        private static DepartmentDAL departmentDALObj;
        private static bool isVietnameseLanguage;

        protected List<string> listOfEmployeePosition_ViewMyRequest;
        protected List<string> listOfEmployeePosition_AddNewItem;
        protected List<string> listOfEmployeePosition_RequestToBeApproved;

        #region Controls

        private HtmlGenericControl myRequestLi;
        private HtmlGenericControl requestToBeApprovedLi;
        private HtmlGenericControl requestByDepartmentLi;
        private HtmlGenericControl myRequestTab;
        private HtmlGenericControl requestToBeApprovedTab;
        private HtmlGenericControl requestByDepartmentTab;
        private GridView gridMyRquestsObj;
        private GridView gridRequestToBeApprovedObj;
        private GridView gridRequestByDepartmentObj;
        private HtmlAnchor linkAddNewItem;
        private HiddenField hdActivedTab;
        private DropDownList ddlDepartments;
        private TextBox txtMonth;
        private TextBox txtFromDate;
        private TextBox txtToDate;

        #endregion

        #region Query String

        protected string queryStringMyRequest;
        protected string queryStringReequestsToBeApproved;
        protected string queryStringRequestsByDepartment;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Default: 
        /// ** Status => acending (In-Progress, Approved, Rejected, Cancelled)
        /// ** Created => descending
        /// </summary>
        protected string orderByQueryString;

        //public bool IsMyRequestTabShow { get; set; }

        //public bool IsRequestToBeApprovedShow { get; set; }

        //public bool IsRequestsByDepartmentShow { get; set; }

        public string ListUrl
        {
            get
            {
                return this.listUrl;
            }
        }

        public string ListId
        {
            get
            {
                return this.listId;
            }
        }

        public SPList SPListObject
        {
            get
            {
                return this.spListObject;
            }
        }

        public string CurrentUrl
        {
            get
            {
                return this.currentUrl;
            }
        }

        public EmployeeInfo CurrentEmployeeInfoObj
        {
            get
            {
                return this.currentEmployeeInfoObj;
            }
        }

        public int CurrentEmployeeID
        {
            get
            {
                return this.currentEmployeeID;
            }
        }

        public string CurrentEmployeePositionCode
        {
            get
            {
                return this.currentEmployeePositionCode;
            }
        }

        public string CurrentEmployeeType
        {
            get
            {
                return this.currentEmployeeType;
            }
        }

        public int CurrentEmployeeLocationId
        {
            get
            {
                return this.currentEmployeeLocationId;
            }
        }

        public int CurrentEmployeeDepartmentId
        {
            get
            {
                return this.currentEmployeeDepartmentId;
            }
        }

        public string CurrentEmployeeDepartmentCode
        {
            get
            {
                return this.currentEmployeeDepartmentCode;
            }
        }

        public EmployeePositionDAL EmployeePositionDALObj
        {
            get
            {
                return this.employeePositionDAL;
            }
        }

        public static DepartmentDAL DepartmentDALObj
        {
            get
            {
                return departmentDALObj;
            }
        }

        public string[] ListOfEmployeePosition_AddNewItem
        {
            get
            {
                return this.listOfEmployeePosition_AddNewItem.ToArray();
            }
        }

        public string[] ListOfEmployeePosition_ViewMyRequests
        {
            get
            {
                return listOfEmployeePosition_ViewMyRequest.ToArray();
            }
        }

        public string[] ListOfEmployeePosition_ViewRequestToBeApproved
        {
            get
            {
                return this.listOfEmployeePosition_RequestToBeApproved.ToArray();
            }
        }

        #endregion

        #region Constructors

        public ListBaseUserUserControl() : base()
        {
        }

        #endregion

        #region Overrides

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                Inits();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);

                isVietnameseLanguage = Page.LCID == PageLanguages.Vietnamese ? true : false;

                if (!Page.IsPostBack)
                {
                    string activedTab = Page.Request[TabParamName];

                    if (IsMyRequestTabActived())
                    {
                        if (myRequestLi != null)
                        {
                            myRequestLi.Visible = true;
                        }
                        if (myRequestTab != null)
                        {
                            myRequestTab.Visible = true;
                        }

                        LoadListOfMyRequests();

                        if (string.IsNullOrEmpty(activedTab))
                        {
                            activedTab = MyRequestsTabId;
                        }
                    }

                    if (IsRequestToBeApprovedTabActived())
                    {
                        if (requestToBeApprovedLi != null)
                        {
                            requestToBeApprovedLi.Visible = true;
                        }
                        if (requestToBeApprovedTab != null)
                        {
                            requestToBeApprovedTab.Visible = true;
                        }

                        LoadListOfRequestToBeApproved();

                        if (string.IsNullOrEmpty(activedTab))
                        {
                            activedTab = RequestsToBeApprovedTabId;
                        }
                    }

                    if (IsRequestByDepartmentActived())
                    {
                        if (requestByDepartmentLi != null)
                        {
                            requestByDepartmentLi.Visible = true;
                        }
                        if (requestByDepartmentTab != null)
                        {
                            requestByDepartmentTab.Visible = true;
                        }

                        LoadListOfRequestByDepartment();

                        if (string.IsNullOrEmpty(activedTab))
                        {
                            activedTab = RequestsByDepartmentTabId;
                        }
                    }

                    if (this.HavePermissionAddNew())
                    {
                        ShowAddNewItemLink();
                    }

                    this.hdActivedTab = this.FindControl(hdActivedTabId) as HiddenField;
                    if (this.hdActivedTab != null)
                    {
                        this.hdActivedTab.Value = activedTab;
                    }

                    // Active Tab: Add css class: active
                    ActiveTabDiv();

                    LoadListOfDepartment();

                    LoadMonthCalendar();
                    LoadDateCalendars();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        #endregion

        #region Inits

        private void Inits()
        {
            try
            {
                InitObjects();

                InitCurrentEmployeeInfo();

                InitCurrentEmployeePositionCode();

                InitCurrentEmployeeType();

                InitOderByQueryString();

                InitListOfEmployeePosition();

                InitControls();

                InitQueryString();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void InitObjects()
        {
            this.spListObject = SPContext.Current.Web.GetList(string.Format("{0}{1}", SPContext.Current.Web.Url, this.listUrl));
            if (this.spListObject != null)
            {
                this.listId = spListObject.ID.ToString();
                this.listName = spListObject.TitleResource.GetValueForUICulture(System.Globalization.CultureInfo.GetCultureInfo(PageLanguages.English));
            }

            this.currentUrl = HttpContext.Current.Request.Url.ToString();

            employeePositionDAL = new EmployeePositionDAL(SPContext.Current.Web.Url);
            departmentDALObj = new DepartmentDAL(SPContext.Current.Web.Url);
        }

        private void InitControls()
        {
            myRequestLi = this.FindControl(MyRequestLiId) as HtmlGenericControl;
            if (myRequestLi != null)
            {
                myRequestLi.ClientIDMode = ClientIDMode.Static;
            }
            requestToBeApprovedLi = this.FindControl(RequestToBeApprovedLiId) as HtmlGenericControl;
            if (requestToBeApprovedLi != null)
            {
                requestToBeApprovedLi.ClientIDMode = ClientIDMode.Static;
            }
            requestByDepartmentLi = this.FindControl(RequestByDepartmentLiId) as HtmlGenericControl;
            if (requestByDepartmentLi != null)
            {
                requestByDepartmentLi.ClientIDMode = ClientIDMode.Static;
            }
            myRequestTab = this.FindControl(MyRequestsTabId) as HtmlGenericControl;
            if (myRequestTab != null)
            {
                myRequestTab.ClientIDMode = ClientIDMode.Static;
            }
            requestToBeApprovedTab = this.FindControl(RequestsToBeApprovedTabId) as HtmlGenericControl;
            if (requestToBeApprovedTab != null)
            {
                requestToBeApprovedTab.ClientIDMode = ClientIDMode.Static;
            }
            requestByDepartmentTab = this.FindControl(RequestsByDepartmentTabId) as HtmlGenericControl;
            if (requestByDepartmentTab != null)
            {
                requestByDepartmentTab.ClientIDMode = ClientIDMode.Static;
            }

            linkAddNewItem = this.FindControl(linkAddNewItemId) as HtmlAnchor;
            var upMyRequests = this.FindControl(UpdatePanelMyRequestsId) as UpdatePanel;
            if (upMyRequests != null)
            {
                this.gridMyRquestsObj = upMyRequests.FindControl(gridMyRquestsId) as GridView;
                if (this.gridMyRquestsObj != null)
                {
                    this.gridMyRquestsObj.RowDataBound += GridMyRquests_RowDataBound;
                    this.gridMyRquestsObj.RowCommand += GridMyRquests_OnRowCommand;
                    this.gridMyRquestsObj.AllowPaging = BizConstants.GridViewSettings.AllowPaging;
                    this.gridMyRquestsObj.PageSize = BizConstants.GridViewSettings.PageSize;
                    if (this.gridMyRquestsObj.AllowPaging)
                    {
                        this.gridMyRquestsObj.PageIndexChanging += GridMyRquests_PageIndexChanging;
                    }
                }
            }

            var upRequestToBeApproved = this.FindControl(UpdatePanelRequestToBeApprovedId) as UpdatePanel;
            if (upRequestToBeApproved != null)
            {
                this.gridRequestToBeApprovedObj = upRequestToBeApproved.FindControl(gridRequestToBeApprovedId) as GridView;
                if (this.gridRequestToBeApprovedObj != null)
                {
                    this.gridRequestToBeApprovedObj.RowDataBound += RequestToBeApproved_RowDataBound;
                    this.gridRequestToBeApprovedObj.AllowPaging = BizConstants.GridViewSettings.AllowPaging;
                    this.gridRequestToBeApprovedObj.PageSize = BizConstants.GridViewSettings.PageSize;
                    if (this.gridRequestToBeApprovedObj.AllowPaging)
                    {
                        this.gridRequestToBeApprovedObj.PageIndexChanging += GridRequestToBeApproved_PageIndexChanging;
                    }
                }
            }

            var upRequestByDepartment = this.FindControl(UpdatePanelRequestByDepartmentId) as UpdatePanel;
            if (upRequestByDepartment != null)
            {
                this.gridRequestByDepartmentObj = upRequestToBeApproved.FindControl(gridRequestByDepartmentId) as GridView;
                if (this.gridRequestByDepartmentObj != null)
                {
                    this.gridRequestByDepartmentObj.RowDataBound += RequestByDepartment_RowDataBound;
                    this.gridRequestByDepartmentObj.AllowPaging = BizConstants.GridViewSettings.AllowPaging;
                    this.gridRequestByDepartmentObj.PageSize = BizConstants.GridViewSettings.PageSize;
                    if (this.gridRequestByDepartmentObj.AllowPaging)
                    {
                        this.gridRequestByDepartmentObj.PageIndexChanging += GridRequestByDepartment_PageIndexChanging;
                    }
                }
            }

            this.ddlDepartments = this.FindControl(ddlDepartmentsId) as DropDownList;
            this.txtMonth = this.FindControl(txtMonthId) as TextBox;
            this.txtFromDate = this.FindControl(txtFromDateId) as TextBox;
            this.txtToDate = this.FindControl(txtToDateId) as TextBox;
        }

        protected virtual void InitOderByQueryString()
        {
            this.orderByQueryString = string.Format(@"<OrderBy>
	                                                    <FieldRef Name='{0}' Ascending='TRUE'/>
                                                        <FieldRef Name='{1}' Ascending='FALSE'/>
                                                    </OrderBy>", ApprovalFields.StatusOrder, "Created");
        }

        private void InitListOfEmployeePosition()
        {
            InitListOfEmployeePosition_AddNewItem();
            InitListOfEmployeePosition_ViewMyRequest();
            InitListOfEmployeePosition_ViewRequestToBeApproved();
        }

        private void InitCurrentEmployeePositionCode()
        {
            try
            {
                this.currentEmployeePositionCode = string.Empty;

                if (this.currentEmployeeInfoObj != null)
                {
                    this.currentEmployeeID = this.currentEmployeeInfoObj.ID;

                    if (this.currentEmployeeInfoObj.EmployeePosition != null)
                    {
                        Biz.Models.EmployeePosition employeePosition = employeePositionDAL.GetByID(this.currentEmployeeInfoObj.EmployeePosition.LookupId);
                        if (employeePosition != null)
                        {
                            this.currentEmployeePositionCode = employeePosition.Code;
                        }
                    }

                    if (this.currentEmployeeInfoObj.FactoryLocation != null)
                    {
                        this.currentEmployeeLocationId = this.currentEmployeeInfoObj.FactoryLocation.LookupId;
                    }

                    if (this.currentEmployeeInfoObj.Department != null)
                    {
                        this.currentEmployeeDepartmentId = this.currentEmployeeInfoObj.Department.LookupId;
                        var department = departmentDALObj.GetByID(this.currentEmployeeDepartmentId);
                        if (department != null)
                        {
                            this.currentEmployeeDepartmentCode = department.Code;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void InitCurrentEmployeeType()
        {
            this.currentEmployeeType = string.Empty;

            if (this.currentEmployeeInfoObj != null)
            {
                this.currentEmployeeType = this.currentEmployeeInfoObj.EmployeeType;
            }
        }

        protected virtual void InitListOfEmployeePosition_AddNewItem()
        {
            this.listOfEmployeePosition_AddNewItem = new List<string>();
        }

        protected virtual void InitListOfEmployeePosition_ViewMyRequest()
        {
            listOfEmployeePosition_ViewMyRequest = new List<string>();
        }

        protected virtual void InitListOfEmployeePosition_ViewRequestToBeApproved()
        {
            this.listOfEmployeePosition_RequestToBeApproved = new List<string>();
        }

        private void InitQueryString()
        {
            InitQueryStringMyRequest();

            InitQueryStringRequestsToBeApproved();

            InitQueryStringRequestsByDepartment();
        }

        protected virtual void InitQueryStringMyRequest()
        {
            queryStringMyRequest = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                        </Eq>
                                                    </Where>", Biz.ApprovalManagement.ApprovalFields.Creator, this.CurrentEmployeeInfoObj.ID);
        }

        protected virtual void InitQueryStringRequestsToBeApproved()
        {
            queryStringReequestsToBeApproved = string.Format(@"<Where>
                                                                <And>
                                                                    <Or>
                                                                        <Eq>
                                                                            <FieldRef Name='{0}' />
                                                                            <Value Type='Text'>{1}</Value>
                                                                        </Eq>
                                                                        <Eq>
                                                                            <FieldRef Name='{0}' />
                                                                            <Value Type='Text'>{2}</Value>
                                                                        </Eq>
                                                                    </Or>
                                                                    <Or>
                                                                        <Eq>
                                                                            <FieldRef Name='{3}' LookupId='True' />
                                                                            <Value Type='Lookup'>{4}</Value>
                                                                        </Eq>
                                                                        <Eq>
                                                                            <FieldRef Name='{5}' LookupId='True' />
                                                                            <Value Type='Lookup'>{6}</Value>
                                                                        </Eq>
                                                                    </Or>
                                                                </And>   
                                                                   </Where>", ApprovalFields.WFStatus, ApprovalStatus.InProgress, ApprovalStatus.InProcess,
                                                                   ApprovalFields.PendingAt, this.CurrentEmployeeInfoObj.ID,
                                                           ApprovalFields.AssignTo, this.CurrentEmployeeInfoObj.ID);
        }

        protected virtual void InitQueryStringRequestsByDepartment()
        {
            #region Del 2017.09.14. TFS #1523.
            //int month = DateTime.Now.Month;
            //int year = DateTime.Now.Year;
            //int departmentId = 0;

            //if (this.Page.Request[MonthParamname] != null)
            //{
            //    int.TryParse(this.Page.Request[MonthParamname], out month);
            //}
            //if (this.Page.Request[YearParamName] != null)
            //{
            //    int.TryParse(this.Page.Request[YearParamName], out year);
            //}
            //DateTime startDate = new DateTime(year, month, 1);
            //DateTime endDate = new DateTime(year, month, 1);
            //endDate = endDate.AddMonths(1).AddSeconds(-1);

            //if (this.Page.Request[DepartmentParamName] != null)
            //{
            //    int.TryParse(this.Page.Request[DepartmentParamName], out departmentId);
            //}
            #endregion

            this.queryStringRequestsByDepartment = string.Format(@"<Where>
                                                                        <Eq>
                                                                            <FieldRef Name='{0}' />
                                                                            <Value Type='Number'>{1}</Value>
                                                                        </Eq>
                                                                    </Where>", "ID", "0");

            // - BOD: Xem được tât cả các đơn (tất cả các trạng) của tất cả các phòng ban.
            if (string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.BOD, true) == 0)
            {

                #region BOD

                #region Del 2017.09.14. TFS #1523.
                //// Chon Department
                //if (departmentId > 0)
                //{
                //    // Filter by: Department
                //    this.queryStringRequestsByDepartment = string.Format(@"<Where>
                //                                                                <And>
                //                                                                    <And>
                //                                                                        <And>
                //                                                                            <Eq>
                //                                                                                <FieldRef Name='{0}' LookupId='True' />
                //                                                                                <Value Type='Lookup'>{1}</Value>
                //                                                                            </Eq>
                //                                                                            <Eq>
                //                                                                                <FieldRef Name='{2}' LookupId='True' />
                //                                                                                <Value Type='Lookup'>{3}</Value>
                //                                                                            </Eq>
                //                                                                        </And>
                //                                                                        <Geq>
                //                                                                            <FieldRef Name='{4}' />
                //                                                                            <Value Type='DateTime' IncludeTimeValue='True'>{5}</Value>
                //                                                                        </Geq>
                //                                                                    </And>
                //                                                                    <Leq>
                //                                                                        <FieldRef Name='{6}' />
                //                                                                        <Value Type='DateTime' IncludeTimeValue='True'>{7}</Value>
                //                                                                    </Leq>
                //                                                                </And>
                //                                                            </Where>", ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                //                                                                        ApprovalFields.CommonDepartment, departmentId,
                //                                                                        "Created", startDate.ToString(StringConstant.DateFormatTZForCAML),
                //                                                                        "Created", endDate.ToString(StringConstant.DateFormatTZForCAML));
                //}
                //// Khong chon Department
                //else
                //{
                //    this.queryStringRequestsByDepartment = string.Format(@"<Where>
                //                                                                <And>
                //                                                                    <And>
                //                                                                        <Geq>
                //                                                                            <FieldRef Name='{0}' />
                //                                                                            <Value Type='DateTime' IncludeTimeValue='True'>{1}</Value>
                //                                                                        </Geq>
                //                                                                        <Leq>
                //                                                                            <FieldRef Name='{2}' />
                //                                                                            <Value Type='DateTime' IncludeTimeValue='True'>{3}</Value>
                //                                                                        </Leq>
                //                                                                    </And>
                //                                                                    <Eq>
                //                                                                        <FieldRef Name='{4}' LookupId='True' />
                //                                                                        <Value Type='Lookup'>{5}</Value>
                //                                                                    </Eq>
                //                                                                </And>
                //                                                            </Where>", "Created", startDate.ToString(StringConstant.DateFormatTZForCAML),
                //                                                                        "Created", endDate.ToString(StringConstant.DateFormatTZForCAML),
                //                                                                        ApprovalFields.CommonLocation, this.currentEmployeeLocationId);
                //}
                #endregion

                #region Add 2017.09.14. TFS #1523.
                this.queryStringRequestsByDepartment = BuildQueryStringRequestsByDepartmentForBOD();
                #endregion

                #endregion
            }
            // - Văn Thư Hành Chánh: Xem được đơn đã duyệt (Approved) của tất cả các phòng ban.
            else if ((string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.AMD, true) == 0) &&
                (string.Compare(this.currentEmployeeDepartmentCode, DepartmentCode.HR, true) == 0))
            {

                #region Văn Thư - Hành Chánh

                #region Del 2017.09.14.TFS #1523.
                //if (departmentId > 0)
                //{
                //    // Filter by: Status, Location
                //    this.queryStringRequestsByDepartment = string.Format(@"<Where>
                //                                                                <And>
                //                                                                    <And>
                //                                                                        <And>
                //                                                                            <And>
                //                                                                                <Eq>
                //                                                                                    <FieldRef Name='{0}' />
                //                                                                                    <Value Type='Text'>{1}</Value>
                //                                                                                </Eq>
                //                                                                                <Eq>
                //                                                                                    <FieldRef Name='{2}' LookupId='True' />
                //                                                                                    <Value Type='Lookup'>{3}</Value>
                //                                                                                </Eq>
                //                                                                            </And>
                //                                                                            <Geq>
                //                                                                                <FieldRef Name='{4}' />
                //                                                                                <Value Type='DateTime' IncludeTimeValue='True'>{5}</Value>
                //                                                                            </Geq>
                //                                                                        </And>
                //                                                                        <Leq>
                //                                                                            <FieldRef Name='{6}' />
                //                                                                            <Value Type='DateTime' IncludeTimeValue='True'>{7}</Value>
                //                                                                        </Leq>
                //                                                                    </And>
                //                                                                    <Eq>
                //                                                                        <FieldRef Name='{8}' LookupId='True' />
                //                                                                        <Value Type='Lookup'>{9}</Value>
                //                                                                    </Eq>
                //                                                                </And>
                //                                                        </Where>", ApprovalFields.WFStatus, StringConstant.ApprovalStatus.Approved,
                //                                                                        ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                //                                                                        "Created", startDate.ToString(StringConstant.DateFormatTZForCAML),
                //                                                                        "Created", endDate.ToString(StringConstant.DateFormatTZForCAML),
                //                                                                        ApprovalFields.CommonDepartment, departmentId);
                //}
                //else // Khong chon Department
                //{
                //    // Filter by: Status, Location
                //    this.queryStringRequestsByDepartment = string.Format(@"<Where>
                //                                                            <And>
                //                                                                <And>
                //                                                                    <And>
                //                                                                        <Eq>
                //                                                                            <FieldRef Name='{0}' />
                //                                                                            <Value Type='Text'>{1}</Value>
                //                                                                        </Eq>
                //                                                                        <Eq>
                //                                                                            <FieldRef Name='{2}' LookupId='True' />
                //                                                                            <Value Type='Lookup'>{3}</Value>
                //                                                                        </Eq>
                //                                                                    </And>
                //                                                                    <Geq>
                //                                                                        <FieldRef Name='{4}' />
                //                                                                        <Value Type='DateTime' IncludeTimeValue='True'>{5}</Value>
                //                                                                    </Geq>
                //                                                                </And>
                //                                                                <Leq>
                //                                                                    <FieldRef Name='{6}' />
                //                                                                    <Value Type='DateTime' IncludeTimeValue='True'>{7}</Value>
                //                                                                </Leq>
                //                                                            </And>    
                //                                                        </Where>", ApprovalFields.WFStatus, StringConstant.ApprovalStatus.Approved,
                //                                                                        ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                //                                                                        "Created", startDate.ToString(StringConstant.DateFormatTZForCAML),
                //                                                                        "Created", endDate.ToString(StringConstant.DateFormatTZForCAML));
                //}

                #endregion

                #region Add 2017.09.14.TFS #1523.
                this.queryStringRequestsByDepartment = BuildQueryStringRequestsByDepartmentForAMDHR();
                #endregion

                #endregion
            }
            // - Trưởng Phòng: Xem tất cả các đơn của phòng ban mình (Không ban biệt trạng thái).
            else if (string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.DEH, true) == 0)
            {
                #region Trưởng Phòng - Khác

                #region Del 2017.09.14. TFS #1523.
                //// Filter by: Status, Location and Department
                //this.queryStringRequestsByDepartment = string.Format(@"<Where>
                //                                                            <And>
                //                                                                <And>
                //                                                                    <And>
                //                                                                        <Eq>
                //                                                                            <FieldRef Name='{0}' LookupId='True' />
                //                                                                            <Value Type='Lookup'>{1}</Value>
                //                                                                        </Eq>
                //                                                                        <Eq>
                //                                                                            <FieldRef Name='{2}' LookupId='True' />
                //                                                                            <Value Type='Lookup'>{3}</Value>
                //                                                                        </Eq>
                //                                                                    </And>
                //                                                                    <Geq>
                //                                                                        <FieldRef Name='{4}' />
                //                                                                        <Value Type='DateTime' IncludeTimeValue='True'>{5}</Value>
                //                                                                    </Geq>
                //                                                                </And>
                //                                                                <Leq>
                //                                                                    <FieldRef Name='{6}' />
                //                                                                    <Value Type='DateTime' IncludeTimeValue='True'>{7}</Value>
                //                                                                </Leq>
                //                                                            </And>
                //                                                        </Where>", ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                //                                                                    ApprovalFields.CommonDepartment, this.currentEmployeeDepartmentId,
                //                                                                        "Created", startDate.ToString(StringConstant.DateFormatTZForCAML),
                //                                                                        "Created", endDate.ToString(StringConstant.DateFormatTZForCAML));
                #endregion

                #region Add 2017.09.14. TFS #1523.
                //this.queryStringRequestsByDepartment = BuildQueryStringRequestsByDepartmentForDEH();
                #endregion

                // Trưởng Phòng - Hành Chánh
                if ((string.Compare(this.currentEmployeeDepartmentCode, DepartmentCode.HR, true) == 0))
                {
                    #region Trưởng Phòng - Hánh Chánh
                    this.queryStringRequestsByDepartment = BuildQueryStringRequestsByDepartmentForDEHHR();
                    #endregion
                }
                else    // Trưởng Phòng - Khác
                {
                    this.queryStringRequestsByDepartment = BuildQueryStringRequestsByDepartmentForDEH();
                }

                #endregion
            }
        }

        /// <summary>
        /// BuildQueryStringRequestsByDepartmentForBOD: Ban Giám Đốc
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildQueryStringRequestsByDepartmentForBOD()
        {
            var stringQuery = string.Format(@"<Where>
                                                <Eq>
                                                    <FieldRef Name='{0}' />
                                                    <Value Type='Number'>{1}</Value>
                                                </Eq>
                                            </Where>", "ID", "0");

            int departmentId = 0;
            
            if (this.Page.Request[DepartmentParamName] != null)
            {
                int.TryParse(this.Page.Request[DepartmentParamName], out departmentId);
            }

            #region BOD

            // Chon Department
            if (departmentId > 0)
            {
                // Filter by: Department
                stringQuery = string.Format(@"<Where>
                                                    <And>
                                                        <And>
                                                            <And>
                                                                <Eq>
                                                                    <FieldRef Name='{0}' LookupId='True' />
                                                                    <Value Type='Lookup'>{1}</Value>
                                                                </Eq>
                                                                <Eq>
                                                                    <FieldRef Name='{2}' LookupId='True' />
                                                                    <Value Type='Lookup'>{3}</Value>
                                                                </Eq>
                                                            </And>
                                                            <Geq>
                                                                <FieldRef Name='{4}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{5}</Value>
                                                            </Geq>
                                                        </And>
                                                        <Leq>
                                                            <FieldRef Name='{6}' />
                                                            <Value Type='DateTime' IncludeTimeValue='False'>{7}</Value>
                                                        </Leq>
                                                    </And>
                                                </Where>", ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                                                        ApprovalFields.CommonDepartment, departmentId,
                                                        "Created", this.GetFromDateString(),
                                                        "Created", this.GetToDateString());
            }
            // Khong chon Department
            else
            {
                stringQuery = string.Format(@"<Where>
                                                    <And>
                                                        <And>
                                                            <Geq>
                                                                <FieldRef Name='{0}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{1}</Value>
                                                            </Geq>
                                                            <Leq>
                                                                <FieldRef Name='{2}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{3}</Value>
                                                            </Leq>
                                                        </And>
                                                        <Eq>
                                                            <FieldRef Name='{4}' LookupId='True' />
                                                            <Value Type='Lookup'>{5}</Value>
                                                        </Eq>
                                                    </And>
                                                </Where>", "Created", this.GetFromDateString(),
                                                        "Created", this.GetToDateString(),
                                                        ApprovalFields.CommonLocation, this.currentEmployeeLocationId);
            }

            #endregion

            return stringQuery;
        }

        /// <summary>
        /// BuildQueryStringRequestsByDepartmentForAMDHR: Văn Thư - Hành Chánh
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildQueryStringRequestsByDepartmentForAMDHR()
        {
            var stringQuery = string.Format(@"<Where>
                                                <Eq>
                                                    <FieldRef Name='{0}' />
                                                    <Value Type='Number'>{1}</Value>
                                                </Eq>
                                            </Where>", "ID", "0");
            
            int departmentId = 0;
            
            if (this.Page.Request[DepartmentParamName] != null)
            {
                int.TryParse(this.Page.Request[DepartmentParamName], out departmentId);
            }

            #region Văn Thư - Hành Chánh
            if (departmentId > 0)
            {
                // Filter by: Status, Location
                stringQuery = string.Format(@"<Where>
                                                    <And>
                                                        <And>
                                                            <And>
                                                                <And>
                                                                    <Or>
                                                                        <Eq>
                                                                            <FieldRef Name='{0}' />
                                                                            <Value Type='Text'>{1}</Value>
                                                                        </Eq>
                                                                        <Eq>
                                                                            <FieldRef Name='{0}' />
                                                                            <Value Type='Text'>{2}</Value>
                                                                        </Eq>
                                                                    </Or>
                                                                    <Eq>
                                                                        <FieldRef Name='{3}' LookupId='True' />
                                                                        <Value Type='Lookup'>{4}</Value>
                                                                    </Eq>
                                                                </And>
                                                                <Geq>
                                                                    <FieldRef Name='{5}' />
                                                                    <Value Type='DateTime' IncludeTimeValue='False'>{6}</Value>
                                                                </Geq>
                                                            </And>
                                                            <Leq>
                                                                <FieldRef Name='{7}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{8}</Value>
                                                            </Leq>
                                                        </And>
                                                        <Eq>
                                                            <FieldRef Name='{9}' LookupId='True' />
                                                            <Value Type='Lookup'>{10}</Value>
                                                        </Eq>
                                                    </And>
                                            </Where>", ApprovalFields.WFStatus, StringConstant.ApprovalStatus.Approved, ApprovalStatus.Completed,
                                                        ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                                                        "Created", this.GetFromDateString(),
                                                        "Created", this.GetToDateString(),
                                                        ApprovalFields.CommonDepartment, departmentId);
            }
            else // Khong chon Department
            {
                // Filter by: Status, Location
                stringQuery = string.Format(@"<Where>
                                                    <And>
                                                        <And>
                                                            <And>
                                                                <Eq>
                                                                    <FieldRef Name='{0}' />
                                                                    <Value Type='Text'>{1}</Value>
                                                                </Eq>
                                                                <Eq>
                                                                    <FieldRef Name='{2}' LookupId='True' />
                                                                    <Value Type='Lookup'>{3}</Value>
                                                                </Eq>
                                                            </And>
                                                            <Geq>
                                                                <FieldRef Name='{4}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{5}</Value>
                                                            </Geq>
                                                        </And>
                                                        <Leq>
                                                            <FieldRef Name='{6}' />
                                                            <Value Type='DateTime' IncludeTimeValue='False'>{7}</Value>
                                                        </Leq>
                                                    </And>    
                                                </Where>", ApprovalFields.WFStatus, StringConstant.ApprovalStatus.Approved,
                                                            ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                                                            "Created", this.GetFromDateString(),
                                                            "Created", this.GetToDateString());
            }

            #endregion

            return stringQuery;
        }

        /// <summary>
        /// BuildQueryStringRequestsByDepartmentForDEH: Trưởng Phòng
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildQueryStringRequestsByDepartmentForDEH()
        {
            var queryString = string.Format(@"<Where>
                                                <Eq>
                                                    <FieldRef Name='{0}' />
                                                    <Value Type='Number'>{1}</Value>
                                                </Eq>
                                            </Where>", "ID", "0");
            
            #region Trưởng Phòng

            // Filter by: Status, Location and Department
            queryString = string.Format(@"<Where>
                                                <And>
                                                    <And>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{0}' LookupId='True' />
                                                                <Value Type='Lookup'>{1}</Value>
                                                            </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{2}' LookupId='True' />
                                                                <Value Type='Lookup'>{3}</Value>
                                                            </Eq>
                                                        </And>
                                                        <Geq>
                                                            <FieldRef Name='{4}' />
                                                            <Value Type='DateTime' IncludeTimeValue='False'>{5}</Value>
                                                        </Geq>
                                                    </And>
                                                    <Leq>
                                                        <FieldRef Name='{6}' />
                                                        <Value Type='DateTime' IncludeTimeValue='False'>{7}</Value>
                                                    </Leq>
                                                </And>
                                            </Where>", ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                                                    ApprovalFields.CommonDepartment, this.currentEmployeeDepartmentId,
                                                        "Created", this.GetFromDateString(),
                                                        "Created", this.GetToDateString());

            #endregion

            return queryString;
        }

        /// <summary>
        /// BuildQueryStringRequestsByDepartmentForDEHHR: Trưởng Phòng - Hánh Chánh
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildQueryStringRequestsByDepartmentForDEHHR()
        {
            var stringQuery = string.Format(@"<Where>
                                                <Eq>
                                                    <FieldRef Name='{0}' />
                                                    <Value Type='Number'>{1}</Value>
                                                </Eq>
                                            </Where>", "ID", "0");
            
            int departmentId = 0;

            if (this.Page.Request[DepartmentParamName] != null)
            {
                int.TryParse(this.Page.Request[DepartmentParamName], out departmentId);
            }

            #region Trưởng Phòng - Hánh Chánh
            // Chon Department
            if (departmentId > 0)
            {
                // Filter by: Department
                stringQuery = string.Format(@"<Where>
                                                    <And>
                                                        <And>
                                                            <And>
                                                                <Eq>
                                                                    <FieldRef Name='{0}' LookupId='True' />
                                                                    <Value Type='Lookup'>{1}</Value>
                                                                </Eq>
                                                                <Eq>
                                                                    <FieldRef Name='{2}' LookupId='True' />
                                                                    <Value Type='Lookup'>{3}</Value>
                                                                </Eq>
                                                            </And>
                                                            <Geq>
                                                                <FieldRef Name='{4}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{5}</Value>
                                                            </Geq>
                                                        </And>
                                                        <Leq>
                                                            <FieldRef Name='{6}' />
                                                            <Value Type='DateTime' IncludeTimeValue='False'>{7}</Value>
                                                        </Leq>
                                                    </And>
                                                </Where>", ApprovalFields.CommonLocation, this.currentEmployeeLocationId,
                                                        ApprovalFields.CommonDepartment, departmentId,
                                                        "Created", this.GetFromDateString(),
                                                        "Created", this.GetToDateString());
            }
            // Khong chon Department
            else
            {
                stringQuery = string.Format(@"<Where>
                                                    <And>
                                                        <And>
                                                            <Geq>
                                                                <FieldRef Name='{0}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{1}</Value>
                                                            </Geq>
                                                            <Leq>
                                                                <FieldRef Name='{2}' />
                                                                <Value Type='DateTime' IncludeTimeValue='False'>{3}</Value>
                                                            </Leq>
                                                        </And>
                                                        <Eq>
                                                            <FieldRef Name='{4}' LookupId='True' />
                                                            <Value Type='Lookup'>{5}</Value>
                                                        </Eq>
                                                    </And>
                                                </Where>", "Created", this.GetFromDateString(),
                                                        "Created", this.GetToDateString(),
                                                        ApprovalFields.CommonLocation, this.currentEmployeeLocationId);
            }
            #endregion

            return stringQuery;
        }

        protected virtual Tuple<string, string> BuildDateRange()
        {
            string fromDate = this.Page.Request.Params.Get(FromDateParamName);
            string toDate = this.Page.Request.Params.Get(ToDateParamName);
            string fromDateValue = string.Empty;
            string toDateValue = string.Empty;
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                DateTime dtFromDate;
                bool isValidFromDate = DateTime.TryParseExact(fromDate, StringConstant.DateFormatddMMyyyy2, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFromDate);
                if (isValidFromDate)
                    fromDateValue = $"{dtFromDate:yyyy-MM-dd}";
                else
                    fromDateValue = $"{DateTime.Now:yyyy-MM-dd}";

                DateTime dtToDate;
                bool isValidToDate = DateTime.TryParseExact(toDate, StringConstant.DateFormatddMMyyyy2, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtToDate);
                if (isValidToDate)
                    toDateValue = $"{dtToDate:yyyy-MM-dd}";
                else
                    toDateValue = $"{DateTime.Now:yyyy-MM-dd}";
            }
            else
            {
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                int day = DateTime.Now.Day;

                if (this.Page.Request[MonthParamname] != null)
                {
                    int.TryParse(this.Page.Request[MonthParamname], out month);
                }
                if (this.Page.Request[YearParamName] != null)
                {
                    int.TryParse(this.Page.Request[YearParamName], out year);
                }

                DateTime startDate = new DateTime(year, month, day);
                DateTime endDate = startDate;

                fromDateValue = $"{startDate:yyyy-MM-dd}";
                toDateValue = $"{endDate:yyyy-MM-dd}";
            }

            return System.Tuple.Create<string, string>(fromDateValue, toDateValue);
        }

        #endregion

        #region Methods

        protected virtual bool IsDraftOrRejectedOrSubmittedItem(object dataItem)
        {
            var res = false;

            string status = Status.Invalid;
            if (dataItem != null)
            {
                var statusObject = dataItem.GetProperties(EntityApprovalStatusPropertyName);
                if (statusObject != null)
                {
                    status = statusObject.ToString();
                }

                if (string.IsNullOrEmpty(status) ||
                       (string.Compare(status, Status.Draft, true) == 0) ||
                       (string.Compare(status, Status.Rejected, true) == 0) ||
                       (string.Compare(status, Status.Submitted, true) == 0))
                {
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// Build source values from current url.
        /// For example. http://tronghieusp/_layouts/15/RBVH.Stada.Intranet.WebPages/RequestManagement/RequestList.aspx?lang=en-US&tab=Tab1 
        /// => res[0]: "/_layouts/15/RBVH.Stada.Intranet.WebPages/RequestManagement/RequestForm.aspx"
        /// => res[1]: ?lang=en-US&tab=Tab1
        /// </summary>
        /// <returns></returns>
        public List<string> BuildSourceValuesParamForCurrentUrl()
        {
            List<string> res = new List<string>();

            UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url);
            res.Add(uriBuilder.Path);

            var queryKeys = uriBuilder.GetQueryKeys();
            if (queryKeys != null && queryKeys.Count > 0)
            {
                foreach (var queryKey in queryKeys)
                {
                    if (queryKey.ToLower().Equals("lang"))
                    {
                        continue;
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    var queryValues = uriBuilder.GetQueryValues(queryKey);
                    foreach (var queryValue in queryValues)
                    {
                        stringBuilder.Append(queryValue);
                    }

                    res.Add(string.Format("{0}={1}", queryKey, stringBuilder.ToString()));
                }
            }

            return res;
        }

        /// <summary>
        /// BuildDisplayFormUrl
        /// </summary>
        /// <param name="sourceParamsUrl"></param>
        /// <param name="entityBase"></param>
        /// <returns></returns>
        protected string BuildDisplayFormUrl(string sourceParamsUrl, EntityBase entityBase)
        {
            string url = BuildDisplayFormUrl(sourceParamsUrl, entityBase.ID.ToString());

            return url;
        }

        protected string BuildDisplayFormUrl(string sourceParamsUrl, string itemID)
        {
            string url = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
                                SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_DISPLAYFORM).ToString(), this.ListId, itemID,
                                sourceParamsUrl);

            return url;
        }

        /// <summary>
        /// BuildEditFormUrl
        /// </summary>
        /// <param name="sourceParamsUrl"></param>
        /// <param name="entityBase"></param>
        /// <returns></returns>
        protected string BuildEditFormUrl(string sourceParamsUrl, EntityBase entityBase)
        {
            string url = BuildEditFormUrl(sourceParamsUrl, entityBase.ID.ToString());

            return url;
        }

        protected string BuildEditFormUrl(string sourceParamsUrl, string itemID)
        {
            string url = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
                                   SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(), this.ListId, itemID,
                                   sourceParamsUrl);

            return url;
        }

        /// <summary>
        /// BuildSourceParamUrl
        /// </summary>
        /// <param name="sourceValues"></param>
        /// <returns></returns>
        public StringBuilder BuildSourceParamUrl(List<string> sourceValues)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (sourceValues != null && sourceValues.Count > 0)
            {
                if (sourceValues.Count == 1)
                {
                    stringBuilder.AppendFormat("{0}={1}", UrlParamName.ReturnUrlParamName, sourceValues[0]);
                }
                else
                {
                    for (int i = 0; i < sourceValues.Count; i++)
                    {
                        if (i == sourceValues.Count - 1)
                        {
                            stringBuilder.AppendFormat("{0}={1}", UrlParamName.ReturnUrlParamName, sourceValues[i]);
                        }
                        else
                        {
                            stringBuilder.AppendFormat("{0}={1}&", UrlParamName.ReturnUrlParamName, sourceValues[i]);
                        }
                    }
                }
            }

            return stringBuilder;
        }

        protected virtual bool IsMyRequestTabActived()
        {
            var res = false;

            if (ListOfEmployeePosition_ViewMyRequests != null && ListOfEmployeePosition_ViewMyRequests.Length > 0)
            {
                foreach (var employeePositionCode in this.ListOfEmployeePosition_ViewMyRequests)
                {
                    if (string.Compare(employeePositionCode, currentEmployeePositionCode, true) == 0)
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        protected virtual bool IsRequestToBeApprovedTabActived()
        {
            var res = false;

            if (ListOfEmployeePosition_ViewRequestToBeApproved != null && ListOfEmployeePosition_ViewRequestToBeApproved.Length > 0)
            {
                foreach (var employeePositionCode in this.ListOfEmployeePosition_ViewRequestToBeApproved)
                {
                    if (string.Compare(employeePositionCode, currentEmployeePositionCode, true) == 0)
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Check current employee who has permission to view "Requests By Department" tab.
        /// Hard Code. We should have configuration for this. :(
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsRequestByDepartmentActived()
        {
            var res = false;

            // Neu Van Thu
            if (string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.AMD, true) == 0)
            {
                // Neu Phong Hanh Chanh
                if (string.Compare(this.currentEmployeeDepartmentCode, DepartmentCode.HR, true) == 0)
                {
                    res = true;
                }
            }
            else if (string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.DEH, true) == 0)     // Truong Phong
            {
                res = true;
            }
            else if (string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.GRL, true) == 0)     // Pho Phong
            {
                res = true;
            }
            else if (string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.BOD, true) == 0)     // Ban Giam Doc
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// InitCurrentEmployeeInfo
        /// </summary>
        private void InitCurrentEmployeeInfo()
        {
            if (HttpContext.Current.Session != null)
            {
                this.currentEmployeeInfoObj = HttpContext.Current.Session[Biz.Constants.StringConstant.EmployeeLogedin] as EmployeeInfo;

                //User is not common account, we should get from employee list
                if (this.currentEmployeeInfoObj == null)
                {
                    SPUser spUser = SPContext.Current.Web.CurrentUser;
                    if (spUser != null)
                    {
                        var employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                        this.currentEmployeeInfoObj = employeeInfoDAL.GetByADAccount(spUser.ID);
                        HttpContext.Current.Session[Biz.Constants.StringConstant.EmployeeLogedin] = this.currentEmployeeInfoObj;
                    }
                }
            }
        }

        protected virtual void LoadListOfMyRequests()
        {
        }

        protected virtual void LoadListOfRequestToBeApproved()
        {
        }

        protected virtual void LoadListOfRequestByDepartment()
        {
        }

        protected string BuildNewFormUrl()
        {
            UriBuilder currentUrlBuilder = new UriBuilder(this.currentUrl);
            currentUrlBuilder.RemoveQuery(TabParamName);

            currentUrlBuilder.AddQuery(TabParamName, MyRequestsTabId);

            //return string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&RootFolder=&{3}={4}",
            //                SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_NEWFORM).ToString(), this.listId, BizConstants.UrlParamName.ReturnUrlParamName, this.currentUrl);
            return string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&RootFolder=&{3}={4}",
                SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_NEWFORM).ToString(), this.listId, BizConstants.UrlParamName.ReturnUrlParamName, currentUrlBuilder.ToString());
        }

        protected void ShowAddNewItemLink()
        {
            try
            {
                if (this.linkAddNewItem != null)
                {
                    this.linkAddNewItem.Visible = true;
                    string newFormUrl = BuildNewFormUrl();
                    this.linkAddNewItem.Attributes.Add("href", newFormUrl);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected virtual bool HavePermissionAddNew()
        {
            var res = false;

            if (this.ListOfEmployeePosition_AddNewItem != null && this.ListOfEmployeePosition_AddNewItem.Length > 0)
            {
                foreach (var employeePositionCode in this.ListOfEmployeePosition_AddNewItem)
                {
                    if (string.Compare(employeePositionCode, this.currentEmployeePositionCode, true) == 0)
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        private void ActiveTabDiv()
        {
            if (this.hdActivedTab != null)
            {
                string activedTab = hdActivedTab.Value;

                if (string.Compare(activedTab, MyRequestsTabId, true) == 0)
                {
                    if (this.myRequestTab != null)
                    {
                        string classAttribute = this.myRequestTab.Attributes["class"];
                        this.myRequestTab.Attributes.Add("class", classAttribute + " " + ActivedTabClass);
                    }

                    this.myRequestLi.Attributes.Add("class", ActivedTabClass);
                }
                else if (string.Compare(activedTab, RequestsToBeApprovedTabId, true) == 0)
                {
                    if (this.requestToBeApprovedTab != null)
                    {
                        string classAttribute = this.requestToBeApprovedTab.Attributes["class"];
                        this.requestToBeApprovedTab.Attributes.Add("class", classAttribute + " " + ActivedTabClass);
                    }

                    this.requestToBeApprovedLi.Attributes.Add("class", ActivedTabClass);
                }
                else if (string.Compare(activedTab, RequestsByDepartmentTabId, true) == 0)
                {
                    if (this.requestByDepartmentTab != null)
                    {
                        string classAttribute = this.requestByDepartmentTab.Attributes["class"];
                        this.requestByDepartmentTab.Attributes.Add("class", classAttribute + " " + ActivedTabClass);
                    }

                    this.requestByDepartmentLi.Attributes.Add("class", ActivedTabClass);
                }
            }
        }

        protected void SetStatusCssClass(Label lblStatus, string status)
        {
            if (string.Compare(StringConstant.ApprovalStatus.InProgress, status, true) == 0)
            {
                lblStatus.Attributes.Add("class", StatusCssClass.In_Progress);
                if (isVietnameseLanguage)
                {
                    lblStatus.Text = VietnameseApprovalStatus.InProgress;
                }
            }
            else if (string.Compare(StringConstant.ApprovalStatus.Approved, status, true) == 0)
            {
                lblStatus.Attributes.Add("class", StatusCssClass.Approved);
                if (isVietnameseLanguage)
                {
                    lblStatus.Text = VietnameseApprovalStatus.Approved;
                }
            }
            else if (string.Compare(StringConstant.ApprovalStatus.Rejected, status, true) == 0)
            {
                lblStatus.Attributes.Add("class", StatusCssClass.Rejected);
                if (isVietnameseLanguage)
                {
                    lblStatus.Text = VietnameseApprovalStatus.Rejected;
                }
            }
            else if (string.Compare(StringConstant.ApprovalStatus.Cancelled, status, true) == 0)
            {
                lblStatus.Attributes.Add("class", StatusCssClass.Cancelled);
                if (isVietnameseLanguage)
                {
                    lblStatus.Text = VietnameseApprovalStatus.Cancelled;
                }
            }
            else if (string.Compare(StringConstant.ApprovalStatus.Completed, status, true) == 0)
            {
                lblStatus.Attributes.Add("class", StatusCssClass.Completed);
                if (isVietnameseLanguage)
                {
                    lblStatus.Text = VietnameseApprovalStatus.Completed;
                }
            }
            else if (string.Compare(StringConstant.ApprovalStatus.InProcess, status, true) == 0)
            {
                lblStatus.Attributes.Add("class", StatusCssClass.In_Process);
                if (isVietnameseLanguage)
                {
                    lblStatus.Text = VietnameseApprovalStatus.InProcess;
                }
            }
        }

        protected virtual void LoadListOfDepartment()
        {
            try
            {
                if (this.ddlDepartments != null)
                {
                    this.ddlDepartments.DataValueField = "ID";

                    //DepartmentDAL departmentDAL = new DepartmentDAL(SPContext.Current.Web.Url);
                    //var departments = departmentDAL.GetAll();
                    var departments = departmentDALObj.GetDepartmentsByLocation(new List<int> { this.currentEmployeeInfoObj.FactoryLocation.LookupId });
                    if (departments == null)
                    {
                        departments = new List<Department>();
                    }
                    if (IsVietnameseLanguage)
                    {
                        this.ddlDepartments.DataTextField = "VietnameseName";
                        departments = departments.OrderBy(dept => dept.VietnameseName).ToList<Department>();
                    }
                    else
                    {
                        this.ddlDepartments.DataTextField = "Name";
                        departments = departments.OrderBy(dept => dept.Name).ToList<Department>();
                    }
                    departments.Insert(0, new Department { ID = Department_All, Name = "All/Tất cả", VietnameseName = "All/Tất cả" });

                    this.ddlDepartments.DataSource = departments;
                    this.ddlDepartments.DataBind();

                    // Văn Thư - Hành Chánh; Trưởng Phòng - Hành Chánh; BOD
                    if (((string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.AMD, true) == 0) && (string.Compare(this.currentEmployeeDepartmentCode, DepartmentCode.HR, true) == 0)) ||
                        ((string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.DEH, true) == 0) && (string.Compare(this.currentEmployeeDepartmentCode, DepartmentCode.HR, true) == 0)) ||
                        (string.Compare(this.currentEmployeePositionCode, EmployeePositionCode.BOD, true) == 0))
                    {
                        this.ddlDepartments.SelectedValue = this.Page.Request[DepartmentParamName];
                        this.ddlDepartments.Enabled = true;
                    }
                    else
                    {
                        this.ddlDepartments.Enabled = false;
                        if (this.currentEmployeeInfoObj != null && this.currentEmployeeInfoObj.Department != null)
                        {
                            this.ddlDepartments.SelectedValue = this.currentEmployeeInfoObj.Department.LookupId.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected virtual void LoadMonthCalendar()
        {
            if (this.txtMonth != null)
            {
                int month = 0;
                int year = 0;

                int.TryParse(this.Request[MonthParamname], out month);
                int.TryParse(this.Request[YearParamName], out year);

                DateTime selectedDate;
                if (month != 0 && year != 0)
                {
                    selectedDate = new DateTime(year, month, 1);
                }
                else
                {
                    selectedDate = DateTime.Now;
                }

                txtMonth.Text = selectedDate.ToString("MM/yyyy");
            }
        }

        protected virtual void LoadDateCalendars()
        {
            if (this.txtFromDate != null)
            {
                string fromDate = this.Page.Request.Params.Get(FromDateParamName);
                if (!string.IsNullOrEmpty(fromDate))
                {
                    DateTime dtFromDate;
                    bool isValidFromDate = DateTime.TryParseExact(fromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFromDate);
                    if (isValidFromDate)
                        txtFromDate.Text = $"{dtFromDate:dd/MM/yyyy}";
                    else
                        txtFromDate.Text = $"{DateTime.Now:dd/MM/yyyy}";
                }
                else
                    txtFromDate.Text = $"{DateTime.Now:dd/MM/yyyy}";
            }
            if (this.txtToDate != null)
            {
                string toDate = this.Page.Request.Params.Get(ToDateParamName);
                if (!string.IsNullOrEmpty(toDate))
                {
                    DateTime dtToDate;
                    bool isValidToDate = DateTime.TryParseExact(toDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtToDate);
                    if (isValidToDate)
                        txtToDate.Text = $"{dtToDate:dd/MM/yyyy}";
                    else
                        txtToDate.Text = $"{DateTime.Now:dd/MM/yyyy}";
                }
                else
                    txtToDate.Text = $"{DateTime.Now:dd/MM/yyyy}";
            }
        }

        public static bool IsVietnameseLanguage
        {
            get
            {
                return isVietnameseLanguage;
            }
        }

        protected static string GetDepartmentName(int departmentId)
        {
            string departmentName = string.Empty;

            var department = DepartmentDALObj.GetByID(departmentId);
            if (department != null)
            {
                if (IsVietnameseLanguage)
                {
                    departmentName = department.VietnameseName;
                }
                else
                {
                    departmentName = department.Name;
                }
            }

            return departmentName;
        }

        /// <summary>
        /// Check current employee who is AD user?
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentEmployeeADUser()
        {
            var res = false;

            if (string.Compare(EmployeeType.ADUser, this.currentEmployeeType, true) == 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Check current employee who is BOD user?
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentEmployeeBOD()
        {
            var isCurrentEmployeeBOD = false;

            if (this.currentEmployeeInfoObj.EmployeePosition != null)
            {
                int employeePositionId = this.currentEmployeeInfoObj.EmployeePosition.LookupId;
                if (employeePositionId > 0)
                {
                    Biz.Models.EmployeePosition employeePosition = this.employeePositionDAL.GetByID(employeePositionId);
                    if (string.Compare(EmployeePositionCode.BOD, employeePosition.Code, true) == 0)
                    {
                        isCurrentEmployeeBOD = true;
                    }
                }
            }

            return isCurrentEmployeeBOD;
        }

        protected virtual string GetFromDateString(TypeOfDateRange typeOfDateRange = TypeOfDateRange.MonthYear)
        {
            var res = "";

            DateTime fromDate;

            string monthStr = this.Page.Request[MonthParamname];
            string yearStr = this.Page.Request[YearParamName];
            string fromDateStr = this.Page.Request[FromDateParamName];
            if (!string.IsNullOrEmpty(monthStr) && !string.IsNullOrEmpty(yearStr))
            {
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;

                int.TryParse(monthStr, out month);
                int.TryParse(yearStr, out year);

                fromDate = new DateTime(year, month, 1);
            }
            else if (!string.IsNullOrEmpty(fromDateStr))
            {
                bool isValidFromDate = DateTime.TryParseExact(fromDateStr, StringConstant.DateFormatddMMyyyy2, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                if (!isValidFromDate)
                {
                    fromDate = DateTime.Now;
                }
            }
            else
            {
                if (typeOfDateRange == TypeOfDateRange.FromTo)
                {
                    fromDate = DateTime.Now;
                }
                else
                {
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;

                    fromDate = new DateTime(year, month, 1);
                }
            }

            res = fromDate.ToString(StringConstant.DateFormatTZForCAML);

            return res;
        }

        protected virtual string GetToDateString(TypeOfDateRange typeOfDateRange = TypeOfDateRange.MonthYear)
        {
            var res = "";

            DateTime toDate;

            string monthStr = this.Page.Request[MonthParamname];
            string yearStr = this.Page.Request[YearParamName];
            string toDateStr = this.Page.Request[ToDateParamName];
            if (!string.IsNullOrEmpty(monthStr) && !string.IsNullOrEmpty(yearStr))
            {
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;

                int.TryParse(monthStr, out month);
                int.TryParse(yearStr, out year);

                toDate = new DateTime(year, month, 1).AddMonths(1).AddSeconds(-1);
            }
            else if (!string.IsNullOrEmpty(toDateStr))
            {
                bool isValidToDate = DateTime.TryParseExact(toDateStr, StringConstant.DateFormatddMMyyyy2, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                if (!isValidToDate)
                {
                    toDate = DateTime.Now;
                }
            }
            else
            {
                if (typeOfDateRange == TypeOfDateRange.FromTo)
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;

                    toDate = new DateTime(year, month, 1).AddMonths(1).AddSeconds(-1);
                }
            }

            res = toDate.ToString(StringConstant.DateFormatTZForCAML);

            return res;
        }

        #endregion

        #region Events

        protected virtual void GridMyRquests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gridMyRquestsObj.PageIndex = e.NewPageIndex;
                if (this.Page.Session[MyRequestsSessionKey] != null)
                {
                    this.gridMyRquestsObj.DataSource = this.Page.Session[MyRequestsSessionKey];
                    this.gridMyRquestsObj.DataBind();
                }
                else
                {
                    this.LoadListOfMyRequests();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected virtual void GridRequestToBeApproved_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gridRequestToBeApprovedObj.PageIndex = e.NewPageIndex;
                if (this.Page.Session[RequestsToBeApprovedSessionKey] != null)
                {
                    this.gridRequestToBeApprovedObj.DataSource = this.Page.Session[RequestsToBeApprovedSessionKey];
                    this.gridRequestToBeApprovedObj.DataBind();
                }
                else
                {
                    this.LoadListOfRequestToBeApproved();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected virtual void GridRequestByDepartment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gridRequestByDepartmentObj.PageIndex = e.NewPageIndex;
                if (this.Page.Session[RequestsByDepartmentSessionKey] != null)
                {
                    this.gridRequestByDepartmentObj.DataSource = this.Page.Session[RequestsByDepartmentSessionKey];
                    this.gridRequestByDepartmentObj.DataBind();
                }
                else
                {
                    this.LoadListOfRequestByDepartment();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected virtual void GridMyRquests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var dataItem = e.Row.DataItem;
            var dataItemProperties = dataItem.GetProperties();

            #region Status
            var wfStatusObject = dataItemProperties[ApprovalFields.WFStatus];
            string status = wfStatusObject != null ? wfStatusObject.ToString() : string.Empty;
            var lblStatus = e.Row.FindControl("lblStatus") as Label;
            lblStatus.Text = status;
            SetStatusCssClass(lblStatus, status);
            #endregion

            #region Created
            var createdObject = dataItemProperties["Created"];
            if (createdObject != null)
            {
                var created = Convert.ToDateTime(createdObject);
                var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                litCreatedDate.Text = created.ToString(StringConstant.DateFormatddMMyyyyhhmmssttt);
            }
            #endregion

            var itemIdObject = dataItemProperties["ID"];
            string itemID = string.Empty;
            if (itemIdObject != null)
            {
                itemID = itemIdObject.ToString();
            }
            var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
            StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
            UriBuilder currentUriBuilder = new UriBuilder(this.CurrentUrl);
            if (!currentUriBuilder.HasQuery(TabParamName))
            {
                sourceParamsUrl.AppendFormat("&{0}={1}={2}", UrlParamName.ReturnUrlParamName, TabParamName, MyRequestsTabId);
            }
            else
            {
                sourceParamsUrl = sourceParamsUrl.Replace(RequestsToBeApprovedTabId, MyRequestsTabId);
                sourceParamsUrl = sourceParamsUrl.Replace(RequestsByDepartmentTabId, MyRequestsTabId);
            }

            bool editable = IsDraftOrRejectedOrSubmittedItem(dataItem);
            if (editable)
            {
                var linkEdit = e.Row.FindControl("linkEdit") as HtmlControl;
                if (linkEdit != null)
                {
                    string href = BuildEditFormUrl(sourceParamsUrl.ToString(), itemID);
                    linkEdit.Attributes.Add("href", href);
                    linkEdit.Visible = true;
                }

                var btnCancelWF = e.Row.FindControl("btnCancelWF") as Button;
                if (btnCancelWF != null)
                {
                    btnCancelWF.Enabled = true;
                }
            }
            else
            {
                // linkView
                var linkView = e.Row.FindControl("linkView") as HtmlControl;
                if (linkView != null)
                {
                    string href = BuildDisplayFormUrl(sourceParamsUrl.ToString(), itemID);
                    linkView.Attributes.Add("href", href);
                    linkView.Visible = true;
                }
            }
        }

        protected virtual void GridMyRquests_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (string.Compare(e.CommandName, CancelWFCommandName, true) == 0)
                {
                    int itemId = Convert.ToInt32(e.CommandArgument);
                    if (itemId > 0)
                    {
                        SPListItem currentItem = this.spListObject.GetItemById(itemId);
                        if (currentItem != null)
                        {
                            string status = ObjectHelper.GetString(currentItem[ApprovalFields.Status]);
                            // Only allow cancel when status is Submitted or Rejected. Other status values don't allow cancellation.
                            if ((string.Compare(status, Status.Submitted, true) == 0) || (string.Compare(status, Status.Rejected, true) == 0))
                            {
                                var canceled = ApprovalBaseManager.CancelWF(currentItem);
                                if (canceled)
                                {
                                    Button btnCancelWF = e.CommandSource as Button;
                                    if (btnCancelWF != null)
                                    {
                                        btnCancelWF.Enabled = false;

                                        //Get the row that contains this button
                                        GridViewRow gridViewRow = btnCancelWF.NamingContainer as GridViewRow;
                                        if (gridViewRow != null)
                                        {
                                            var lblStatus = gridViewRow.FindControl("lblStatus") as Label;
                                            if (lblStatus != null)
                                            {
                                                lblStatus.Text = ApprovalStatus.Cancelled;
                                                SetStatusCssClass(lblStatus, ApprovalStatus.Cancelled);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Button btnCancelWF = e.CommandSource as Button;
                                if (btnCancelWF != null)
                                {
                                    btnCancelWF.Enabled = false;

                                    //Get the row that contains this button
                                    GridViewRow gridViewRow = btnCancelWF.NamingContainer as GridViewRow;
                                    if (gridViewRow != null)
                                    {
                                        var lblStatus = gridViewRow.FindControl("lblStatus") as Label;
                                        if (lblStatus != null)
                                        {
                                            string approvalStatus = ObjectHelper.GetString(currentItem[ApprovalFields.WFStatus]);
                                            lblStatus.Text = approvalStatus;
                                            SetStatusCssClass(lblStatus, approvalStatus);
                                        }
                                    }
                                }

                                string message = ResourceHelper.GetLocalizedString("CannotCancelApprovedRequest", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                                string script = string.Format(@"alert('{0}');", message);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alertCancelWF", script, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected virtual void RequestToBeApproved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var dataItem = e.Row.DataItem;
            var dataItemProperties = dataItem.GetProperties();

            #region Status
            var wfStatusObject = dataItemProperties[ApprovalFields.WFStatus];
            string status = wfStatusObject != null ? wfStatusObject.ToString() : string.Empty;
            var lblStatus = e.Row.FindControl("lblStatus") as Label;
            lblStatus.Text = status;
            SetStatusCssClass(lblStatus, status);
            #endregion

            #region Created
            var createdObject = dataItemProperties["Created"];
            if (createdObject != null)
            {
                var created = Convert.ToDateTime(createdObject);
                var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                litCreatedDate.Text = created.ToString(StringConstant.DateFormatddMMyyyyhhmmssttt);
            }
            #endregion

            var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
            StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
            UriBuilder currentUriBuilder = new UriBuilder(this.CurrentUrl);
            if (!currentUriBuilder.HasQuery(TabParamName))
            {
                sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, RequestsToBeApprovedTabId);
            }
            else
            {
                sourceParamsUrl = sourceParamsUrl.Replace(MyRequestsTabId, RequestsToBeApprovedTabId);
                sourceParamsUrl = sourceParamsUrl.Replace(RequestsByDepartmentTabId, RequestsToBeApprovedTabId);
            }

            // linkView
            //var linkView = e.Row.FindControl("linkView") as System.Web.UI.HtmlControls.HtmlControl;
            //string viewHref = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
            //    SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_DISPLAYFORM).ToString(),
            //    this.ListId, dataItem.ID, sourceParamsUrl);
            //linkView.Attributes.Add("href", viewHref);

            // linkEdit
            var linkEdit = e.Row.FindControl("linkEdit") as System.Web.UI.HtmlControls.HtmlControl;
            //string editHref = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}", SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(),
            //    this.ListId, dataItem.ID, sourceParamsUrl);
            var itemIdObject = dataItemProperties["ID"];
            string itemID = string.Empty;
            if (itemIdObject != null)
            {
                itemID = itemIdObject.ToString();
            }
            string editHref = BuildEditFormUrl(sourceParamsUrl.ToString(), itemID);
            linkEdit.Attributes.Add("href", editHref);
        }

        protected virtual void RequestByDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var dataItem = e.Row.DataItem;
            var dataItemProperties = dataItem.GetProperties();

            #region Status
            var wfStatusObject = dataItemProperties[ApprovalFields.WFStatus];
            string status = wfStatusObject != null ? wfStatusObject.ToString() : string.Empty;
            var lblStatus = e.Row.FindControl("lblStatus") as Label;
            lblStatus.Text = status;
            SetStatusCssClass(lblStatus, status);
            #endregion

            #region Created
            var createdObject = dataItemProperties["Created"];
            if (createdObject != null)
            {
                var created = Convert.ToDateTime(createdObject);
                var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                litCreatedDate.Text = created.ToString(StringConstant.DateFormatddMMyyyyhhmmssttt);
            }
            #endregion

            var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
            StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
            UriBuilder currentUriBuilder = new UriBuilder(this.CurrentUrl);
            if (!currentUriBuilder.HasQuery(TabParamName))
            {
                sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, RequestsByDepartmentTabId);
            }
            else
            {
                sourceParamsUrl = sourceParamsUrl.Replace(MyRequestsTabId, RequestsByDepartmentTabId);
                sourceParamsUrl = sourceParamsUrl.Replace(RequestsToBeApprovedTabId, RequestsByDepartmentTabId);
            }

            var itemIdObject = dataItemProperties["ID"];
            string itemID = string.Empty;
            if (itemIdObject != null)
            {
                itemID = itemIdObject.ToString();
            }

            // linkView
            var linkView = e.Row.FindControl("linkView") as System.Web.UI.HtmlControls.HtmlControl;
            string viewHref = BuildDisplayFormUrl(sourceParamsUrl.ToString(), itemID);
            linkView.Attributes.Add("href", viewHref);

            // linkEdit
            var linkEdit = e.Row.FindControl("linkEdit") as System.Web.UI.HtmlControls.HtmlControl;
            //string editHref = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}", SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(),
            //    this.ListId, dataItem.ID, sourceParamsUrl);

            string editHref = BuildEditFormUrl(sourceParamsUrl.ToString(), itemID);
            linkEdit.Attributes.Add("href", editHref);
        }

        #endregion
    }

    /// <summary>
    /// StatusCssClass
    /// </summary>
    public class StatusCssClass
    {
        /// <summary>
        /// label label-default
        /// </summary>
        public const string In_Progress = "label label-default";
        /// <summary>
        /// label label-success
        /// </summary>
        public const string Approved = "label label-success";
        /// <summary>
        /// label label-danger
        /// </summary>
        public const string Rejected = "label label-danger";
        /// <summary>
        /// label label-warning
        /// </summary>
        public const string Cancelled = "label label-warning";
        /// <summary>
        /// label label-success
        /// </summary>
        public const string Completed = "label label-success";
        /// <summary>
        /// label label-primary
        /// </summary>
        public const string In_Process = "label label-primary";
    }
}
