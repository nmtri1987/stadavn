using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using BizConstants = RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.DelegationManagementControl
{
    /// <summary>
    /// DelegationListUserControl
    /// </summary>
    public partial class DelegationListUserControl : UserControl
    {
        #region Constants

        /// <summary>
        /// Tab
        /// </summary>
        public const string TabParamName = "Tab";

        /// <summary>
        /// MyDelegationsTab
        /// </summary>
        protected const string MyDelegationsTabId = "MyDelegationsTab";

        /// <summary>
        /// MyDelegationsOfNewTaskTab
        /// </summary>
        protected const string MyDelegationsOfNewTaskTabId = "MyDelegationsOfNewTaskTab";

        /// <summary>
        /// DelegationsApprovalTab
        /// </summary>
        public const string DelegationsApprovalTabId = "DelegationsApprovalTab";

        /// <summary>
        /// active
        /// </summary>
        public const string ActivedTabCssClass = "active";

        /// <summary>
        /// DeleteDelegation
        /// </summary>
        public const string DeleteDelegationCommandName = "DeleteDelegation";

        #endregion

        #region Attributes
        private bool isVietnameseLanguage;
        private string siteUrl;
        protected string listUrl;
        private string listName;
        private string listId;
        private SPList spListObject;
        private SPWeb currentWeb;
        private string currentUrl;
        private EmployeeInfo currentEmployeeInfo;
        private DelegationsDAL delegationsDAL;
        private DelegationsOfNewTaskDAL delegationsOfNewTaskDAL;
        private bool doesCurrentEmployeeHasDelegationPermission;
        private bool doesCurrentEmployeeHasApprovalPermission;
        #endregion

        #region Overrides

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                // Get delegation of permission
                GetDelegationPermission();

                if (!this.doesCurrentEmployeeHasDelegationPermission && !this.doesCurrentEmployeeHasApprovalPermission)
                {
                    SPUtility.HandleAccessDenied(new Exception(ResourceHelper.GetLocalizedString("AccessDeniedMessage", BizConstants.StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID)));
                }

                // Language
                this.isVietnameseLanguage = Page.LCID == BizConstants.PageLanguages.Vietnamese ? true : false;

                // Site Url
                this.siteUrl = SPContext.Current.Web.Url;

                // Initialize objects
                InitObjects();

                // Initialize controls
                InitControls();
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);

                if (!Page.IsPostBack)
                {
                    // Active Tab
                    ActiveTab();

                    // Load Data
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        #endregion

        #region Events

        private void GridDelegationsApproval_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.Delegation;
                    if (dataItem != null)
                    {
                        var linkEdit = e.Row.FindControl("linkEdit") as HtmlControl;
                        var listItemApprovalUrl = BuildEditFormUrl(dataItem.ListItemApprovalUrl, DelegationsApprovalTabId);
                        linkEdit.Attributes.Add("href", listItemApprovalUrl);

                        string fromEmployee = dataItem.FromEmployee != null ? dataItem.FromEmployee.LookupValue : string.Empty;
                        var litFromEmployee = e.Row.FindControl("litFromEmployee") as Literal;
                        litFromEmployee.Text = fromEmployee;

                        var litFromDate = e.Row.FindControl("litFromDate") as Literal;
                        litFromDate.Text = dataItem.FromDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyy2); ;

                        var litToDate = e.Row.FindControl("litToDate") as Literal;
                        litToDate.Text = dataItem.ToDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyy2);

                        var litModule = e.Row.FindControl("litModule") as Literal;
                        litModule.Text = this.isVietnameseLanguage ? dataItem.VietnameseModuleName : dataItem.ModuleName;

                        var litDescription = e.Row.FindControl("litDescription") as Literal;
                        litDescription.Text = dataItem.ListItemDescription;

                        var litRequester = e.Row.FindControl("litRequester") as Literal;
                        litRequester.Text = dataItem.Requester.LookupValue;

                        string departmentName = string.Empty;
                        if (dataItem.Department != null)
                        {
                            var department = BizConstants.DepartmentListSingleton.GetDepartmentByID(dataItem.Department.LookupId, this.siteUrl);
                            if (isVietnameseLanguage)
                            {
                                departmentName = department.VietnameseName;
                            }
                            else
                            {
                                departmentName = department.Name;
                            }
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                        litCreatedDate.Text = dataItem.ListItemCreatedDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);

                        var litDelegatedDate = e.Row.FindControl("litDelegatedDate") as Literal;
                        litDelegatedDate.Text = dataItem.Created.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void GridMyDelegationsOfNewTask_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.DelegationOfNewTask;
                    if (dataItem != null)
                    {
                        var litModule = e.Row.FindControl("litModule") as Literal;
                        litModule.Text = this.isVietnameseLanguage ? dataItem.VietnameseModuleName : dataItem.ModuleName;

                        var litFromDate = e.Row.FindControl("litFromDate") as Literal;
                        litFromDate.Text = dataItem.FromDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyy2);

                        var litToDate = e.Row.FindControl("litToDate") as Literal;
                        litToDate.Text = dataItem.ToDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyy2);

                        var hdSelectDeleteID = e.Row.FindControl("hdSelectDeleteID") as HiddenField;
                        hdSelectDeleteID.Value = Convert.ToString(dataItem.ID);

                        StringBuilder toEmployeesBuilder = new StringBuilder();
                        if (dataItem.ToEmployee != null && dataItem.ToEmployee.Count > 0)
                        {
                            for (int i = 0; i < dataItem.ToEmployee.Count - 1; i++)
                            {
                                toEmployeesBuilder.AppendFormat(@"{0}; ", dataItem.ToEmployee[i].LookupValue);
                            }

                            toEmployeesBuilder.AppendFormat(@"{0}", dataItem.ToEmployee[dataItem.ToEmployee.Count - 1].LookupValue);
                        }
                        var litToEmployee = e.Row.FindControl("litToEmployee") as Literal;
                        litToEmployee.Text = toEmployeesBuilder.ToString();

                        var litDelegatedDate = e.Row.FindControl("litDelegatedDate") as Literal;
                        litDelegatedDate.Text = dataItem.Created.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void GridMyDelegations_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.Delegation;
                    if (dataItem != null)
                    {
                        var linkEdit = e.Row.FindControl("linkEdit") as HtmlControl;
                        var listItemApprovalUrl = BuildEditFormUrl(dataItem.ListItemApprovalUrl, MyDelegationsTabId);
                        linkEdit.Attributes.Add("href", listItemApprovalUrl);

                        StringBuilder toEmployeesBuilder = new StringBuilder();
                        if (dataItem.ToEmployee != null && dataItem.ToEmployee.Count > 0)
                        {
                            for (int i = 0; i < dataItem.ToEmployee.Count - 1; i++)
                            {
                                toEmployeesBuilder.AppendFormat(@"{0}; ", dataItem.ToEmployee[i].LookupValue);
                            }

                            toEmployeesBuilder.AppendFormat(@"{0}", dataItem.ToEmployee[dataItem.ToEmployee.Count - 1].LookupValue);
                        }
                        var litToEmployee = e.Row.FindControl("litToEmployee") as Literal;
                        litToEmployee.Text = toEmployeesBuilder.ToString();

                        var litFromDate = e.Row.FindControl("litFromDate") as Literal;
                        litFromDate.Text = dataItem.FromDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyy2);

                        var litToDate = e.Row.FindControl("litToDate") as Literal;
                        litToDate.Text = dataItem.ToDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyy2);

                        var litModule = e.Row.FindControl("litModule") as Literal;
                        litModule.Text = this.isVietnameseLanguage ? dataItem.VietnameseModuleName : dataItem.ModuleName;

                        string description = dataItem.ListItemDescription != null ? dataItem.ListItemDescription : string.Empty;
                        var litDescription = e.Row.FindControl("litDescription") as Literal;
                        litDescription.Text = description;

                        var litRequester = e.Row.FindControl("litRequester") as Literal;
                        litRequester.Text = dataItem.Requester.LookupValue;

                        var hdSelectDeleteID = e.Row.FindControl("hdSelectDeleteID") as HiddenField;
                        hdSelectDeleteID.Value = Convert.ToString(dataItem.ID);

                        string departmentName = string.Empty;
                        if (dataItem.Department != null)
                        {
                            var department = BizConstants.DepartmentListSingleton.GetDepartmentByID(dataItem.Department.LookupId, this.siteUrl);
                            if (isVietnameseLanguage)
                            {
                                departmentName = department.VietnameseName;
                            }
                            else
                            {
                                departmentName = department.Name;
                            }
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                        litCreatedDate.Text = dataItem.ListItemCreatedDate.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);

                        var litDelegatedDate = e.Row.FindControl("litDelegatedDate") as Literal;
                        litDelegatedDate.Text = dataItem.Created.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void GridMyDelegations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (string.Compare(e.CommandName, DeleteDelegationCommandName, true) == 0)
                {
                    int itemId = Convert.ToInt32(e.CommandArgument);
                    if (itemId > 0)
                    {
                        var deleted = this.delegationsDAL.Delete(itemId);
                        if(deleted)
                        {
                            LoadMyDelegations();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void GridMyDelegationsOfNewTask_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (string.Compare(e.CommandName, DeleteDelegationCommandName, true) == 0)
                {
                    int itemId = Convert.ToInt32(e.CommandArgument);
                    if (itemId > 0)
                    {
                        var deleted = this.delegationsOfNewTaskDAL.Delete(itemId);
                        if (deleted)
                        {
                            LoadMyDelegationsOfNewTask();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void LnkbtnDeleteDelegation_Click(object sender, EventArgs e)
        {
            List<int> ids = new List<int>();

            foreach(GridViewRow row in this.gridMyDelegations.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var cbSelectDelete = row.FindControl("cbSelectDelete") as CheckBox;
                    if (cbSelectDelete.Checked)
                    {
                        var hiddenIdField = row.FindControl("hdSelectDeleteID") as HiddenField;
                        var id = hiddenIdField.Value;
                        if(!string.IsNullOrEmpty(id))
                        {
                            ids.Add(Convert.ToInt32(id));
                        }
                    }
                }
            }

            if (ids.Count > 0)
            {
                delegationsDAL.DeleteItems(ids);
                LoadMyDelegations();
            }
        }

        private void LnkbtnDeleteNewDelegation_Click(object sender, EventArgs e)
        {
            List<int> ids = new List<int>();

            foreach (GridViewRow row in this.gridMyDelegationsOfNewTask.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var cbSelectDelete = row.FindControl("cbSelectDeleteNewTask") as CheckBox;
                    if (cbSelectDelete.Checked)
                    {
                        var hiddenIdField = row.FindControl("hdSelectDeleteID") as HiddenField;
                        var id = hiddenIdField.Value;
                        if (!string.IsNullOrEmpty(id))
                        {
                            ids.Add(Convert.ToInt32(id));
                        }
                    }
                }
            }

            if (ids.Count > 0)
            {
                delegationsOfNewTaskDAL.DeleteItems(ids);
                LoadMyDelegationsOfNewTask();
            }
        }

        private void btnViewMyDelegations_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMyDelegations();
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void BtnViewMyDelegationsNewTask_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMyDelegationsOfNewTask();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// InitObjects
        /// </summary>
        private void InitObjects()
        {
            this.listUrl = DelegationsList.Url;
            this.spListObject = SPContext.Current.Web.GetList(string.Format("{0}{1}", SPContext.Current.Web.Url, this.listUrl));
            if (this.spListObject != null)
            {
                this.listId = spListObject.ID.ToString();
                this.listName = spListObject.TitleResource.GetValueForUICulture(System.Globalization.CultureInfo.GetCultureInfo(BizConstants.PageLanguages.English));
            }
            this.currentWeb = SPContext.Current.Web;

            this.currentUrl = HttpContext.Current.Request.Url.ToString();
            this.delegationsDAL = new DelegationsDAL(SPContext.Current.Web.Url);
            this.delegationsOfNewTaskDAL = new DelegationsOfNewTaskDAL(SPContext.Current.Web.Url);
            this.InitCurrentEmployeeInfoObject();
        }

        /// <summary>
        /// InitControls
        /// </summary>
        private void InitControls()
        {
            #region Add new delegation
            string linkAddNewDelegationUrl = BuildNewFormUrl(MyDelegationsTabId);
            this.linkAddNewDelegation.Attributes.Add("href", linkAddNewDelegationUrl);
            #endregion

            #region Add new delegation of new task
            string linkAddNewDelegationOfNewTaskUrl = BuildNewFormUrl(MyDelegationsOfNewTaskTabId);
            this.linkAddNewDelegationOfNewTask.Attributes.Add("href", linkAddNewDelegationOfNewTaskUrl);
            #endregion

            #region Events
            this.gridMyDelegations.RowDataBound += GridMyDelegations_RowDataBound;
            this.gridMyDelegations.RowCommand += GridMyDelegations_RowCommand;
            this.gridMyDelegationsOfNewTask.RowCommand += GridMyDelegationsOfNewTask_RowCommand;
            this.gridMyDelegationsOfNewTask.RowDataBound += GridMyDelegationsOfNewTask_RowDataBound;
            this.gridDelegationsApproval.RowDataBound += GridDelegationsApproval_RowDataBound;
            this.lnkbtnDeleteDelegation.Click += LnkbtnDeleteDelegation_Click;
            this.lnkbtnDeleteNewDelegation.Click += LnkbtnDeleteNewDelegation_Click;
            this.btnViewMyDelegations.Click += btnViewMyDelegations_Click;
            this.btnViewMyDelegationsNewTask.Click += BtnViewMyDelegationsNewTask_Click;
            #endregion

            this.MyDelegationsLi.ClientIDMode = ClientIDMode.Static;
            this.MyDelegationsOfNewTaskLi.ClientIDMode = ClientIDMode.Static;
            this.DelegationsApprovalLi.ClientIDMode = ClientIDMode.Static;
            this.MyDelegationsTab.ClientIDMode = ClientIDMode.Static;
            this.MyDelegationsOfNewTaskTab.ClientIDMode = ClientIDMode.Static;
            this.DelegationsApprovalTab.ClientIDMode = ClientIDMode.Static;

            string fromDateStr = DateTime.Now.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2);
            string fromToStr = DateTime.Now.AddDays(7).ToString(BizConstants.StringConstant.DateFormatddMMyyyy2);
            this.txtDelegateFromDate.Text = string.IsNullOrEmpty(this.txtDelegateFromDate.Text) == true ? fromDateStr : this.txtDelegateFromDate.Text;
            this.txtDelegateToDate.Text = string.IsNullOrEmpty(this.txtDelegateToDate.Text) == true ? fromToStr : this.txtDelegateToDate.Text;
            this.txtDelegateNewTaskFromDate.Text = string.IsNullOrEmpty(this.txtDelegateNewTaskFromDate.Text) == true ? fromDateStr : this.txtDelegateNewTaskFromDate.Text;
            this.txtDelegateNewTaskToDate.Text = string.IsNullOrEmpty(this.txtDelegateNewTaskToDate.Text) == true ? fromToStr : this.txtDelegateNewTaskToDate.Text;
        }

        private void GetDelegationPermission()
        {
            this.doesCurrentEmployeeHasDelegationPermission = DelegationPermissionManager.DoesCurrentEmployeeHasDelegationPermission();
            this.doesCurrentEmployeeHasApprovalPermission = DelegationPermissionManager.DoesCurrentEmployeeHasApprovalPermission();
        }

        /// <summary>
        /// ActiveTab
        /// </summary>
        private void ActiveTab()
        {
            string activedTab = this.Page.Request[TabParamName];

            if (this.doesCurrentEmployeeHasDelegationPermission)
            {
                if (string.IsNullOrEmpty(activedTab))
                {
                    activedTab = MyDelegationsTabId;
                }

                this.MyDelegationsLi.Visible = true;
                this.MyDelegationsTab.Visible = true;
                this.MyDelegationsOfNewTaskLi.Visible = true;
                this.MyDelegationsOfNewTaskTab.Visible = true;
            }

            if (this.doesCurrentEmployeeHasApprovalPermission)
            {
                if (string.IsNullOrEmpty(activedTab))
                {
                    activedTab = DelegationsApprovalTabId;
                }
                this.DelegationsApprovalLi.Visible = true;
                this.DelegationsApprovalTab.Visible = true;
            }

            if (string.Compare(activedTab, MyDelegationsTabId, true) == 0)
            {
                this.MyDelegationsLi.Attributes.Add("class", ActivedTabCssClass);
                string classAttribute = this.MyDelegationsTab.Attributes["class"];
                this.MyDelegationsTab.Attributes.Add("class", classAttribute + " " + ActivedTabCssClass);
            }
            else if (string.Compare(activedTab, MyDelegationsOfNewTaskTabId, true) == 0)
            {
                this.MyDelegationsOfNewTaskLi.Attributes.Add("class", ActivedTabCssClass);
                string classAttribute = this.MyDelegationsOfNewTaskTab.Attributes["class"];
                this.MyDelegationsOfNewTaskTab.Attributes.Add("class", classAttribute + " " + ActivedTabCssClass);
            }
            else if (string.Compare(activedTab, DelegationsApprovalTabId, true) == 0)
            {
                this.DelegationsApprovalLi.Attributes.Add("class", ActivedTabCssClass);
                string classAttribute = this.DelegationsApprovalTab.Attributes["class"];
                this.DelegationsApprovalTab.Attributes.Add("class", classAttribute + " " + ActivedTabCssClass);
            }

            this.hdActivedTab.Value = activedTab;
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

            UriBuilder uriBuilder = new UriBuilder(this.currentUrl);
            res.Add(uriBuilder.Path);

            var queryKeys = uriBuilder.GetQueryKeys();
            if (queryKeys != null && queryKeys.Count > 0)
            {
                foreach (var queryKey in queryKeys)
                {
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
                    stringBuilder.AppendFormat("{0}={1}", BizConstants.UrlParamName.ReturnUrlParamName, sourceValues[0]);
                }
                else
                {
                    for (int i = 0; i < sourceValues.Count; i++)
                    {
                        if (i == sourceValues.Count - 1)
                        {
                            stringBuilder.AppendFormat("{0}={1}", BizConstants.UrlParamName.ReturnUrlParamName, sourceValues[i]);
                        }
                        else
                        {
                            stringBuilder.AppendFormat("{0}={1}&", BizConstants.UrlParamName.ReturnUrlParamName, sourceValues[i]);
                        }
                    }
                }
            }

            return stringBuilder;
        }

        /// <summary>
        /// BuildNewFormUrl
        /// </summary>
        /// <param name="activatedTabId"></param>
        /// <returns></returns>
        protected string BuildNewFormUrl(string activatedTabId)
        {
            UriBuilder currentUriBuilder = new UriBuilder(this.currentUrl);
            var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
            StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
            if (!currentUriBuilder.HasQuery(TabParamName))
            {
                sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, activatedTabId);
            }
            else
            {
                sourceParamsUrl = sourceParamsUrl.Replace(MyDelegationsTabId, activatedTabId);
                sourceParamsUrl = sourceParamsUrl.Replace(MyDelegationsOfNewTaskTabId, activatedTabId);
                sourceParamsUrl = sourceParamsUrl.Replace(DelegationsApprovalTabId, activatedTabId);
            }

            return string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&RootFolder=&{3}",
                SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_NEWFORM).ToString(), this.listId, sourceParamsUrl);
        }

        protected string BuildEditFormUrl(string listItemApprovalUrl, string activatedTabId)
        {
            UriBuilder currentUriBuilder = new UriBuilder(this.currentUrl);
            var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
            StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
            if (!currentUriBuilder.HasQuery(TabParamName))
            {
                sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, activatedTabId);
            }
            else
            {
                sourceParamsUrl = sourceParamsUrl.Replace(MyDelegationsTabId, activatedTabId);
                sourceParamsUrl = sourceParamsUrl.Replace(MyDelegationsOfNewTaskTabId, activatedTabId);
                sourceParamsUrl = sourceParamsUrl.Replace(DelegationsApprovalTabId, activatedTabId);
            }

            return string.Format("{0}&{1}", listItemApprovalUrl, sourceParamsUrl);
        }

        private void InitCurrentEmployeeInfoObject()
        {
            this.currentEmployeeInfo =
                HttpContext.Current.Session[BizConstants.StringConstant.EmployeeLogedin] as EmployeeInfo;

            //User is not common account, we should get from employee list
            if (this.currentEmployeeInfo == null)
            {
                SPUser spUser = SPContext.Current.Web.CurrentUser;
                if (spUser != null)
                {
                    EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                    this.currentEmployeeInfo = employeeInfoDAL.GetByADAccount(spUser.ID);
                    HttpContext.Current.Session[BizConstants.StringConstant.EmployeeLogedin] = this.currentEmployeeInfo;
                }
            }
        }

        //private void LoadMyDelegations()
        //{
        //    string queryString = $@"<Where>
        //                                <And>
        //                                    <Eq>
        //                                        <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                        <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                    </Eq>
        //                                    <And>
        //                                        <Leq>
        //                                            <FieldRef Name='{DelegationsList.Fields.FromDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Leq>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                    </And>
        //                                </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";
        //    string delegateFromDate = txtDelegateFromDate.Text;
        //    string delegateToDate = txtDelegateToDate.Text;
        //    DateTime fromDate, toDate;
        //    if (!string.IsNullOrEmpty(delegateFromDate) && !string.IsNullOrEmpty(delegateToDate))
        //    {
        //        DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
        //        DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);

        //            queryString = $@"<Where>
        //                            <And>
        //                                <Eq>
        //                                    <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                    <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                </Eq>
        //                                <And>
        //                                    <Leq>
        //                                        <FieldRef Name='{DelegationsList.Fields.FromDate}' />
        //                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                    </Leq>
        //                                    <And>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                        <And>
        //                                            <Geq>
        //                                                <FieldRef Name='{DelegationsList.Fields.Created}' />
        //                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
        //                                            </Geq>
        //                                            <Leq>
        //                                                <FieldRef Name='{DelegationsList.Fields.Created}' />
        //                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
        //                                            </Leq>
        //                                        </And>
        //                                    </And>
        //                                </And>
        //                            </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";

        //    }
        //    else if (!string.IsNullOrEmpty(delegateFromDate))
        //    {
        //        DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
        //        queryString = $@"<Where>
        //                            <And>
        //                                <Eq>
        //                                    <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                    <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                </Eq>
        //                                <And>
        //                                   <Leq>
        //                                        <FieldRef Name='{DelegationsList.Fields.FromDate}' />
        //                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                    </Leq>
        //                                    <And>
        //                                       <Geq>
        //                                            <FieldRef Name='{DelegationsList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsList.Fields.Created}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                    </And>
        //                                </And>
        //                            </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";
        //    }
        //    else if(!string.IsNullOrEmpty(delegateToDate))
        //    {
        //        DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
        //        queryString = $@"<Where>
        //                            <And>
        //                                <Eq>
        //                                    <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                    <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                </Eq>
        //                                <And>
        //                                    <Leq>
        //                                        <FieldRef Name='{DelegationsList.Fields.FromDate}' />
        //                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                    </Leq>
        //                                    <And>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                        <Leq>
        //                                            <FieldRef Name='{DelegationsList.Fields.Created}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Leq>
        //                                    </And>
        //                                </And>
        //                            </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";
        //    }

        //    var delegations = this.delegationsDAL.GetByQuery(queryString, string.Empty);
        //    this.gridMyDelegations.DataSource = delegations;
        //    this.gridMyDelegations.DataBind();
        //}

        private void LoadMyDelegations()
        {
            string queryString = $@"<Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                            </Eq>
                                            <And>
                                                <Leq>
                                                    <FieldRef Name='{DelegationsList.Fields.FromDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
                                                </Leq>
                                                <Geq>
                                                    <FieldRef Name='{DelegationsList.Fields.ToDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
                                                </Geq>
                                            </And>
                                        </And>
                                    </Where>
                                    <OrderBy>
                                        <FieldRef Name='Created' Ascending='False' />
                                    </OrderBy>";
            string delegateFromDate = txtDelegateFromDate.Text;
            string delegateToDate = txtDelegateToDate.Text;
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(delegateFromDate) && !string.IsNullOrEmpty(delegateToDate))
            {
                DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);

                queryString = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                        </Eq>
                                        <And>
                                            <Leq>
                                                <FieldRef Name='{DelegationsList.Fields.FromDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
                                            </Leq>
                                            <Geq>
                                                <FieldRef Name='{DelegationsList.Fields.ToDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
                                            </Geq>
                                        </And>
                                    </And>
                                </Where>
                                <OrderBy>
                                    <FieldRef Name='Created' Ascending='False' />
                                </OrderBy>";

            }
            else if (!string.IsNullOrEmpty(delegateFromDate))
            {
                DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                queryString = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                        </Eq>
                                        <Geq>
                                            <FieldRef Name='{DelegationsList.Fields.ToDate}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
                                        </Geq>
                                    </And>
                                </Where>
                                <OrderBy>
                                    <FieldRef Name='Created' Ascending='False' />
                                </OrderBy>";
            }
            else if (!string.IsNullOrEmpty(delegateToDate))
            {
                DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                queryString = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{DelegationsList.Fields.FromEmployee}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                        </Eq>
                                        <Leq>
                                            <FieldRef Name='{DelegationsList.Fields.FromDate}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
                                        </Leq>
                                    </And>
                                </Where>
                                <OrderBy>
                                    <FieldRef Name='Created' Ascending='False' />
                                </OrderBy>";
            }

            var delegations = this.delegationsDAL.GetByQuery(queryString, string.Empty);
            this.gridMyDelegations.DataSource = delegations;
            this.gridMyDelegations.DataBind();
        }

        //private void LoadMyDelegationsOfNewTask()
        //{
        //    string queryString = $@"<Where>
        //                                <And>
        //                                    <Eq>
        //                                        <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                        <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                    </Eq>
        //                                    <And>
        //                                        <Leq>
        //                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Leq>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                    </And>
        //                                </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";
        //    string delegateFromDate = txtDelegateNewTaskFromDate.Text;
        //    string delegateToDate = txtDelegateNewTaskToDate.Text;
        //    DateTime fromDate, toDate;
        //    if (!string.IsNullOrEmpty(delegateFromDate) && !string.IsNullOrEmpty(delegateToDate))
        //    {
        //        DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
        //        DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);

        //        queryString = $@"<Where>
        //                            <And>
        //                                <Eq>
        //                                    <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                    <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                </Eq>
        //                                <And>
        //                                    <Leq>
        //                                        <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromDate}' />
        //                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                    </Leq>
        //                                    <And>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                        <And>
        //                                            <Geq>
        //                                                <FieldRef Name='{DelegationsOfNewTaskList.Fields.Created}' />
        //                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
        //                                            </Geq>
        //                                            <Leq>
        //                                                <FieldRef Name='{DelegationsOfNewTaskList.Fields.Created}' />
        //                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
        //                                            </Leq>
        //                                        </And>
        //                                    </And>
        //                                </And>
        //                            </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";

        //    }
        //    else if (!string.IsNullOrEmpty(delegateFromDate))
        //    {
        //        DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
        //        queryString = $@"<Where>
        //                            <And>
        //                                <Eq>
        //                                    <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                    <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                </Eq>
        //                                <And>
        //                                   <Leq>
        //                                        <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromDate}' />
        //                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                    </Leq>
        //                                    <And>
        //                                       <Geq>
        //                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.Created}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                    </And>
        //                                </And>
        //                            </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";
        //    }
        //    else if (!string.IsNullOrEmpty(delegateToDate))
        //    {
        //        DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
        //        queryString = $@"<Where>
        //                            <And>
        //                                <Eq>
        //                                    <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
        //                                    <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
        //                                </Eq>
        //                                <And>
        //                                    <Leq>
        //                                        <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromDate}' />
        //                                        <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                    </Leq>
        //                                    <And>
        //                                        <Geq>
        //                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.ToDate}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Geq>
        //                                        <Leq>
        //                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.Created}' />
        //                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
        //                                        </Leq>
        //                                    </And>
        //                                </And>
        //                            </And>
        //                            </Where>
        //                            <OrderBy>
        //                                <FieldRef Name='Created' Ascending='False' />
        //                            </OrderBy>";
        //    }

        //    var delegationsOfNewTask = this.delegationsOfNewTaskDAL.GetByQuery(queryString);
        //    this.gridMyDelegationsOfNewTask.DataSource = delegationsOfNewTask;
        //    this.gridMyDelegationsOfNewTask.DataBind();
        //}

        private void LoadMyDelegationsOfNewTask()
        {
            string queryString = $@"<Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                            </Eq>
                                            <And>
                                                <Leq>
                                                    <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
                                                </Leq>
                                                <Geq>
                                                    <FieldRef Name='{DelegationsOfNewTaskList.Fields.ToDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatTZForCAML)}</Value>
                                                </Geq>
                                            </And>
                                        </And>
                                    </Where>
                                    <OrderBy>
                                        <FieldRef Name='Created' Ascending='False' />
                                    </OrderBy>";
            string delegateFromDate = txtDelegateNewTaskFromDate.Text;
            string delegateToDate = txtDelegateNewTaskToDate.Text;
            DateTime fromDate, toDate;
            if (!string.IsNullOrEmpty(delegateFromDate) && !string.IsNullOrEmpty(delegateToDate))
            {
                DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);

                queryString = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                        </Eq>
                                        <And>
                                            <Leq>
                                                <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
                                            </Leq>
                                            <Geq>
                                                <FieldRef Name='{DelegationsOfNewTaskList.Fields.ToDate}' />
                                                <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
                                            </Geq>
                                        </And>
                                    </And>
                                </Where>
                                <OrderBy>
                                    <FieldRef Name='Created' Ascending='False' />
                                </OrderBy>";

            }
            else if (!string.IsNullOrEmpty(delegateFromDate))
            {
                DateTime.TryParseExact(delegateFromDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                queryString = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                        </Eq>
                                        <Geq>
                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.ToDate}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{fromDate.ToString(DateFormatTZForCAML)}</Value>
                                        </Geq>
                                    </And>
                                </Where>
                                <OrderBy>
                                    <FieldRef Name='Created' Ascending='False' />
                                </OrderBy>";
            }
            else if (!string.IsNullOrEmpty(delegateToDate))
            {
                DateTime.TryParseExact(delegateToDate, BizConstants.StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                queryString = $@"<Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromEmployee}' LookupId='TRUE'/>
                                            <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                        </Eq>
                                        <Leq>
                                            <FieldRef Name='{DelegationsOfNewTaskList.Fields.FromDate}' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>{toDate.ToString(DateFormatTZForCAML)}</Value>
                                        </Leq>
                                    </And>
                                </Where>
                                <OrderBy>
                                    <FieldRef Name='Created' Ascending='False' />
                                </OrderBy>";
            }

            var delegationsOfNewTask = this.delegationsOfNewTaskDAL.GetByQuery(queryString);
            this.gridMyDelegationsOfNewTask.DataSource = delegationsOfNewTask;
            this.gridMyDelegationsOfNewTask.DataBind();
        }

        private void LoadDelegationApproval()
        {
            string queryString = $@"<Where>
                                        <And>    
                                            <Eq>
                                                <FieldRef Name='{DelegationsList.Fields.ToEmployee}' LookupId='TRUE'/>
                                                <Value Type='Lookup'>{this.currentEmployeeInfo.ID}</Value>
                                            </Eq>
                                            <And>
                                                <Leq>
                                                    <FieldRef Name='{DelegationsList.Fields.FromDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatForCAML)}</Value>
                                                </Leq>
                                                <Geq>
                                                    <FieldRef Name='{DelegationsList.Fields.ToDate}' />
                                                    <Value IncludeTimeValue='FALSE' Type='DateTime'>{DateTime.Today.Date.ToString(DateFormatForCAML)}</Value>
                                                </Geq>
                                            </And>
                                        </And>
                                    </Where>
                                    <OrderBy>
                                        <FieldRef Name='Created' Ascending='False' />
                                    </OrderBy>";
            var delegations = this.delegationsDAL.GetByQuery(queryString);
            // Filter delegations which is not approved yet.
            var listOfDelegationsAreNotApprovedYet = new List<Delegation>();
            if (delegations != null && delegations.Count > 0)
            {
                foreach(var delegation in delegations)
                {
                    if (this.IsDelegationNotApprovedYet(delegation))
                    {
                        listOfDelegationsAreNotApprovedYet.Add(delegation);
                    }
                }
            }
            this.gridDelegationsApproval.DataSource = listOfDelegationsAreNotApprovedYet;
            this.gridDelegationsApproval.DataBind();
        }

        private void LoadData()
        {
            if (this.doesCurrentEmployeeHasDelegationPermission)
            {
                LoadMyDelegations();
                LoadMyDelegationsOfNewTask();
            }

            if (this.doesCurrentEmployeeHasApprovalPermission)
            {
                LoadDelegationApproval();
            }
        }

        /// <summary>
        /// To check delegation which is approved?
        /// </summary>
        /// <param name="delegation">The Delegation object.</param>
        /// <returns>If the delegation is not approved yet, return true. Otherwise return false.</returns>
        private bool IsDelegationNotApprovedYet(Delegation delegation)
        {
            var isDelegationNotApprovedYet = false;

            string listUrl = delegation.ListUrl;
            int itemId = delegation.ListItemID;
            var queryString = $"<Where><Eq><FieldRef Name='ID'></FieldRef><Value Type='Integer'>{itemId.ToString(CultureInfo.InvariantCulture)}</Value></Eq></Where>";
            SPList list = this.currentWeb.GetList(string.Format("{0}{1}", this.siteUrl, listUrl));
            var query = new SPQuery { Query = queryString };
            var results = list.GetItems(query);
            if (results != null && results.Count > 0)
            {
                int fromEmployeeId = 0;
                var delegatedEmployee = DelegationManager.GetCurrentEmployeeProcessing(listUrl, results[0], this.currentWeb);
                if (delegatedEmployee != null)
                {
                    fromEmployeeId = delegatedEmployee.LookupId;
                }
                if (fromEmployeeId > 0)
                {
                    var isDelegation = DelegationPermissionManager.IsDelegation(fromEmployeeId, listUrl, itemId);
                    if (isDelegation != null)
                    {
                        isDelegationNotApprovedYet = true;
                    }
                }
                return isDelegationNotApprovedYet;
            }

            return false;
        }

        #endregion
    }
}
