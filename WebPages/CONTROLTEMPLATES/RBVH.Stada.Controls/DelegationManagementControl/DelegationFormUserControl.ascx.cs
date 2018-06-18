using Microsoft.SharePoint;
using Microsoft.SharePoint.Email;
using Microsoft.SharePoint.Utilities;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Helpers.EvalExpression;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.DelegationManagementControl
{
    /// <summary>
    /// DelegationFormUserControl
    /// </summary>
    public partial class DelegationFormUserControl : FormBaseUserControl
    {
        #region Constants

        /// <summary>
        /// DelegatedEmployeePositions
        /// </summary>
        private const string DelegatedEmployeePositionsAttribute = "DelegatedEmployeePositions";

        /// <summary>
        /// DelegationTasks
        /// </summary>
        private const string DelegationTasksSessionKey = "DelegationTasks";

        /// <summary>
        /// DelegationForm_EmailTemplate
        /// </summary>
        private const string DelegationEmailTemplateKey = "DelegationForm_EmailTemplate";

        /// <summary>
        /// DelegationEmailTemplateSubject
        /// </summary>
        private const string DelegationEmailTemplateSubjectKey = "DelegationEmailTemplateSubject";

        /// <summary>
        /// All
        /// </summary>
        private const string AllModulesOptionValue = "All";

        #endregion

        #region Attributes
        private bool isVietnameseLanguage;
        private List<Delegation> selectedDelegationTasks;
        private EmployeeInfo currentEmployeeInfo;
        private EmployeeInfoDAL employeeInfoDAL;
        private EmployeePositionDAL employeePositionDAL;
        private DelegationModulesDAL delegationModulesDAL;
        private DelegationEmployeePositionsDAL delegationEmployeePositionsDAL;
        private DelegationsOfNewTaskDAL delegationsOfNewTaskDAL;
        private DelegationsDAL delegationsDAL;
        private DepartmentDAL departmentDAL;
        #endregion

        #region Properties

        #endregion

        #region Overrides

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                if (!DelegationPermissionManager.DoesCurrentEmployeeHasDelegationPermission())
                {
                    SPUtility.HandleAccessDenied(new Exception(ResourceHelper.GetLocalizedString("AccessDeniedMessage", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID)));
                }

                // Language
                this.isVietnameseLanguage = Page.LCID == PageLanguages.Vietnamese ? true : false;

                // Initialize objects
                InitObjects();

                // Initialize events
                InitEvents();

                // Initialize controls
                InitControls();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void InitEvents()
        {
            this.btnSearch.Click += BtnSearch_Click;
            this.btnDelegate.Click += BtnDelegate_Click;
            this.btnClose.Click += BtnClose_Click;
            this.gridTasks.RowDataBound += GridTasks_RowDataBound;
        }

        private void InitControls()
        {
            if (isVietnameseLanguage)
            {
                this.ddlModule.DataTextField = DelegationModulesList.Fields.VietnameseModuleName;
            }
        }

        public override void Validate()
        {
            IsValid = true;
            hdErrorMessage.Value = "";

            if (this.dtFromDate.IsDateEmpty)
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_FromDate_Empty", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }
            if (!this.dtFromDate.IsValid)
            {
                IsValid = false;
                return;
            }
            if (DateTime.Compare(DateTime.Now.Date, this.dtFromDate.SelectedDate.Date) > 0)
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_FromDate_Invalid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }
            if (this.dtToDate.IsDateEmpty)
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_ToDate_Empty", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }
            if (!this.dtToDate.IsValid)
            {
                IsValid = false;
                return;
            }
            if (DateTime.Compare(this.dtFromDate.SelectedDate.Date, this.dtToDate.SelectedDate.Date) > 0)
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_ToDate_InValid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }

            int selectedFromEmployeeId = 0;
            int.TryParse(this.ddlFromEmployee.SelectedValue, out selectedFromEmployeeId);
            if (selectedFromEmployeeId <= 0)
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_FromEmployee_InValid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }

            JavaScriptSerializer seriallizer = new JavaScriptSerializer();
            var selectedToEmployees = seriallizer.Deserialize<List<int>>(this.hdSelectedToEmployees.Value);
            if ((selectedToEmployees == null) || (selectedToEmployees != null && selectedToEmployees.Count <= 0))
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_ToEmployee_InValid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }

            var selectedModules = seriallizer.Deserialize<List<string>>(this.hdSelectedModules.Value);
            if ((selectedModules == null) || (selectedModules != null && selectedModules.Count <= 0))
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_Module_InValid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }

            //var isDelegationOfNewTaskChecked = this.cbDelegationNewTask.Checked;
            var isDelegationOfNewTaskChecked = true;
            this.selectedDelegationTasks = this.GetSelectedDelegationTasks();
            if ((isDelegationOfNewTaskChecked == false) && (this.selectedDelegationTasks.Count == 0))
            {
                IsValid = false;
                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("DelegationForm_ErrorMessage_DontChooseDelegation", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                return;
            }
        }

        protected override bool SaveForm()
        {
            return base.SaveForm();
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    LoadForm();
                }

                LoadFromEmployeeAndToEmployee();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var delegationTasks = new List<Delegation>();
                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                var selectedModules = seriallizer.Deserialize<List<string>>(this.hdSelectedModules.Value);

                // All modules
                if (selectedModules != null && selectedModules.Count > 0)
                {
                    int fromEmployeeId = 0;
                    int.TryParse(this.ddlFromEmployee.SelectedValue, out fromEmployeeId);
                    if (fromEmployeeId > 0)
                    {
                        var fromEmployeeInfo = this.employeeInfoDAL.GetByID(fromEmployeeId);
                        if (fromEmployeeInfo != null)
                        {
                            foreach (var listUrl in selectedModules)
                            {
                                var tasks = DelegationManager.GetListOfTasks(fromEmployeeInfo, listUrl);
                                if (tasks != null && tasks.Count > 0)
                                {
                                    delegationTasks.AddRange(tasks);
                                }
                            }
                        }
                    }
                }

                this.Page.Session[DelegationTasksSessionKey] = delegationTasks;
                this.gridTasks.DataSource = delegationTasks;
                this.gridTasks.DataBind();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void BtnDelegate_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.IsValid)
                {
                    var savedDelegationsOfNewTasks = false;
                    var savedDelectedDelegationTasks = false;
                    //if (this.cbDelegationNewTask.Checked)
                    var isDelegationOfNewTaskChecked = true;
                    if (isDelegationOfNewTaskChecked)
                    {
                        List<DelegationOfNewTask> delegationOfNewTaskEntities = new List<DelegationOfNewTask>();

                        JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                        var selectedModules = seriallizer.Deserialize<List<string>>(this.hdSelectedModules.Value);
                        // Select all modules
                        if (selectedModules != null && selectedModules.Count > 0)
                        {
                            foreach (var listUrl in selectedModules)
                            {
                                if (!string.IsNullOrEmpty(listUrl))
                                {
                                    DelegationOfNewTask delegationOfNewTask = InitDelegationOfNewTask(listUrl);
                                    var isExisted = DelegationManager.IsDelegationOfNewTaskExisted(delegationOfNewTask, this.SiteUrl);
                                    if (!isExisted)
                                    {
                                        delegationOfNewTaskEntities.Add(delegationOfNewTask);
                                    }
                                }
                            }
                        }

                        if (delegationOfNewTaskEntities.Count > 0)
                        {
                            savedDelegationsOfNewTasks = this.delegationsOfNewTaskDAL.SaveItems(delegationOfNewTaskEntities);
                        }
                        else
                        {
                            savedDelegationsOfNewTasks = true;
                        }
                    }

                    if (this.selectedDelegationTasks != null && this.selectedDelegationTasks.Count > 0)
                    {
                        savedDelectedDelegationTasks = this.delegationsDAL.SaveItems(selectedDelegationTasks);
                    }
                    else
                    {
                        savedDelectedDelegationTasks = true;
                    }

                    if (savedDelegationsOfNewTasks && savedDelectedDelegationTasks)
                    {
                        SendEmail();
                        CloseForm(sender);
                    }
                    else
                    {
                        // TODO: Continue show error message.
                    }
                }
            }
            catch (Exception ex)
            {
                this.hdErrorMessage.Value = ex.Message;
                ULSLogging.LogError(ex);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.CloseForm(sender);
        }

        private void GridTasks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.Delegation;

                    #region Module
                    var litModule = e.Row.FindControl("litModule") as Literal;
                    if (isVietnameseLanguage)
                    {
                        litModule.Text = dataItem.VietnameseModuleName;
                    }
                    else
                    {
                        litModule.Text = dataItem.ModuleName;
                    }
                    #endregion

                    #region Description
                    var litDescription = e.Row.FindControl("litDescription") as Literal;
                    litDescription.Text = dataItem.ListItemDescription;
                    #endregion

                    #region Requester
                    var litRequester = e.Row.FindControl("litRequester") as Literal;
                    litRequester.Text = dataItem.Requester.LookupValue;
                    #endregion

                    #region Department
                    var department = DepartmentListSingleton.GetDepartmentByID(dataItem.Department.LookupId, this.SiteUrl);
                    var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                    if (isVietnameseLanguage)
                    {
                        litDepartment.Text = department.VietnameseName;
                    }
                    else
                    {
                        litDepartment.Text = department.Name;
                    }
                    #endregion

                    #region Created Date
                    var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                    litCreatedDate.Text = dataItem.ListItemCreatedDate.ToString(StringConstant.DateFormatddMMyyyyhhmmssttt);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        #endregion

        #region Methods

        private void InitCurrentEmployeeInfoObject()
        {
            this.currentEmployeeInfo =
                HttpContext.Current.Session[StringConstant.EmployeeLogedin] as EmployeeInfo;

            //User is not common account, we should get from employee list
            if (this.currentEmployeeInfo == null)
            {
                SPUser spUser = SPContext.Current.Web.CurrentUser;
                if (spUser != null)
                {
                    this.currentEmployeeInfo = employeeInfoDAL.GetByADAccount(spUser.ID);
                    //HttpContext.Current.Session[StringConstant.EmployeeLogedin] = this.currentEmployeeInfo;
                }
            }
        }

        private void InitObjects()
        {
            this.employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            this.InitCurrentEmployeeInfoObject();
            this.employeePositionDAL = new EmployeePositionDAL(this.SiteUrl);
            this.delegationModulesDAL = new DelegationModulesDAL(this.SiteUrl);
            this.delegationEmployeePositionsDAL = new DelegationEmployeePositionsDAL(this.SiteUrl);
            this.delegationsOfNewTaskDAL = new DelegationsOfNewTaskDAL(this.SiteUrl);
            this.delegationsDAL = new DelegationsDAL(this.SiteUrl);
            this.departmentDAL = new DepartmentDAL(this.SiteUrl);
        }

        private void LoadForm()
        {
            LoadModules();
            LoadEmptyGridTasks();
        }

        private void LoadModules()
        {
            var modules = this.delegationModulesDAL.GetAll().OrderBy(x => x.ModuleName).ToList();
            // #1906: Ủy quyền phân theo chức năng của phòng, ví dụ Kế toán không có đăng ký ca thì khi ủy quyền ẩn chức năng đi ca
            if (this.currentEmployeeInfo != null && !this.IsPositionBOD(this.currentEmployeeInfo.EmployeePosition.LookupId))
            {
                var departmentId = this.currentEmployeeInfo.Department.LookupId;
                var departmentInfo = departmentDAL.GetById(departmentId);
                if (departmentInfo != null && !departmentInfo.IsShiftRequestRequired)
                    modules = modules.Where(m => (m.ModuleName != "Shift Management" && m.ModuleName != "Change Shift Management")).ToList();
            }
            if (CultureInfo.CurrentUICulture.LCID == 1066)
            {
                modules = modules.OrderBy(x => x.VietnameseModuleName).ToList();
            }
            modules.Insert(0, new DelegationModule { ID = 0, ListUrl = AllModulesOptionValue, ModuleName = "(All)", VietnameseModuleName = "(Tất cả)" });
            this.ddlModule.DataSource = modules;
            this.ddlModule.DataBind();
        }

        /// <summary>
        /// LoadTasks
        /// </summary>
        private void LoadEmptyGridTasks()
        {
            this.gridTasks.DataSource = new List<Delegation>();
            this.gridTasks.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadFromEmployeeAndToEmployee()
        {
            if (!this.Page.IsPostBack)
            {
                #region The first time loading
                this.ddlFromEmployee.Items.Clear();
                this.ddlToEmployee.Items.Clear();

                // If current user is site admin.
                if (this.CurrentUser.IsSiteAdmin)
                {
                    this.ddlFromEmployee.Items.Clear();

                    var delegationEmployeePositions = this.delegationEmployeePositionsDAL.GetAll();
                    if (delegationEmployeePositions != null && delegationEmployeePositions.Count > 0)
                    {
                        JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                        List<ListItem> fromEmployeeListItems = new List<ListItem>();
                        List<ListItem> toEmployeeListItems = new List<ListItem>();

                        foreach (var delegationEmployeePosition in delegationEmployeePositions)
                        {
                            if (delegationEmployeePosition.EmployeePosition != null && delegationEmployeePosition.DelegatedEmployeePositions != null && delegationEmployeePosition.DelegatedEmployeePositions.Count > 0)
                            {
                                List<LookupItem> delegatedEmployeePositions = new List<LookupItem>();
                                delegatedEmployeePositions.AddRange(delegationEmployeePosition.DelegatedEmployeePositions);

                                // build 'toEmployees' list
                                foreach (var delegatedEmployeePosition in delegationEmployeePosition.DelegatedEmployeePositions)
                                {
                                    var toEmployees = this.employeeInfoDAL.GetDelegatedEmployees(delegatedEmployeePosition.LookupId, delegationEmployeePosition.EmployeePosition.LookupId);
                                    if (toEmployees != null && toEmployees.Count > 0)
                                    {
                                        foreach (var toEmployee in toEmployees)
                                        {
                                            ListItem employeeListItem = InitEmployeeListItem(toEmployee);
                                            toEmployeeListItems.Add(employeeListItem);
                                        }
                                    }

                                    IEnumerable<LookupItem> otherEmployeePositions = toEmployees.Where(e => e.DelegatedBy.Where(d => d.LookupId == delegationEmployeePosition.EmployeePosition.LookupId).Any()).Select(ep => ep.EmployeePosition).Distinct();
                                    if (otherEmployeePositions != null && otherEmployeePositions.Any())
                                    {
                                        delegatedEmployeePositions.AddRange(otherEmployeePositions);
                                    }
                                }

                                // build 'fromEmployees' list
                                delegatedEmployeePositions = delegatedEmployeePositions.GroupBy(e => e.LookupId).Select(g => g.First()).ToList();
                                var fromEmployees = this.employeeInfoDAL.GetByPosition(delegationEmployeePosition.EmployeePosition.LookupId);
                                if (fromEmployees != null && fromEmployees.Count > 0)
                                {
                                    foreach (var fromEmployee in fromEmployees)
                                    {
                                        ListItem employeeListItem = InitEmployeeListItem(fromEmployee);
                                        employeeListItem.Attributes.Add(DelegatedEmployeePositionsAttribute, seriallizer.Serialize(delegatedEmployeePositions));
                                        fromEmployeeListItems.Add(employeeListItem);
                                    }
                                }
                            }
                        }

                        fromEmployeeListItems = fromEmployeeListItems.OrderBy(fromEmployeeListItem => fromEmployeeListItem.Text).ToList();
                        this.ddlFromEmployee.Items.AddRange(fromEmployeeListItems.ToArray());

                        toEmployeeListItems = toEmployeeListItems.GroupBy(e => e.Value).Select(g => g.First()).OrderBy(toEmployeeListItem => toEmployeeListItem.Text).ToList();
                        this.ddlToEmployee.Items.AddRange(toEmployeeListItems.ToArray());
                    }
                }
                else    // If current user is BOD or DEH or ...
                {
                    if (this.currentEmployeeInfo != null)
                    {
                        List<LookupItem> delegatedEmployeePositions = new List<LookupItem>();
                        // Initialize ListItem
                        ListItem fromEmployeeListItem = InitEmployeeListItem(this.currentEmployeeInfo);

                        if (this.currentEmployeeInfo.EmployeePosition != null)
                        {
                            var delegationEmployeePositions = this.delegationEmployeePositionsDAL.GetByEmployeePosition(this.currentEmployeeInfo.EmployeePosition.LookupId);
                            if (delegationEmployeePositions != null && delegationEmployeePositions.Count > 0)
                            {
                                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                                List<ListItem> toEmployeeListItems = new List<ListItem>();

                                foreach (var delegationEmployeePosition in delegationEmployeePositions)
                                {
                                    if (delegationEmployeePosition.DelegatedEmployeePositions != null && delegationEmployeePosition.DelegatedEmployeePositions.Count > 0)
                                    {
                                        delegatedEmployeePositions.AddRange(delegationEmployeePosition.DelegatedEmployeePositions);
                                        foreach (var delegatedEmployeePosition in delegationEmployeePosition.DelegatedEmployeePositions)
                                        {
                                            int? departmentId = null;
                                            int locationId = this.currentEmployeeInfo.FactoryLocation.LookupId;
                                            // If BOD
                                            if (!this.IsPositionBOD(delegatedEmployeePosition.LookupId))
                                            {
                                                departmentId = this.currentEmployeeInfo.Department.LookupId;
                                            }

                                            var toEmployees = this.employeeInfoDAL.GetDelegatedEmployeesByDepartment(delegatedEmployeePosition.LookupId, delegationEmployeePosition.EmployeePosition.LookupId, departmentId, locationId);
                                            if (toEmployees != null && toEmployees.Count > 0)
                                            {
                                                foreach (var toEmployee in toEmployees)
                                                {
                                                    ListItem employeeListItem = InitEmployeeListItem(toEmployee);
                                                    toEmployeeListItems.Add(employeeListItem);
                                                }

                                                IEnumerable<LookupItem> otherEmployeePositions = toEmployees.Where(e => e.DelegatedBy.Where(d => d.LookupId == delegationEmployeePosition.EmployeePosition.LookupId).Any()).Select(ep => ep.EmployeePosition).Distinct();
                                                if (otherEmployeePositions != null && otherEmployeePositions.Any())
                                                {
                                                    delegatedEmployeePositions.AddRange(otherEmployeePositions);
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add ListItem into DropDown list.
                                delegatedEmployeePositions = delegatedEmployeePositions.GroupBy(e => e.LookupId).Select(g => g.First()).ToList();
                                fromEmployeeListItem.Attributes.Add(DelegatedEmployeePositionsAttribute, seriallizer.Serialize(delegatedEmployeePositions));
                                this.ddlFromEmployee.Items.Add(fromEmployeeListItem);
                                this.ddlFromEmployee.Enabled = false;

                                toEmployeeListItems = toEmployeeListItems.GroupBy(e => e.Value).Select(g => g.First()).OrderBy(toEmployeeListItem => toEmployeeListItem.Text).ToList();
                                this.ddlToEmployee.Items.AddRange(toEmployeeListItems.ToArray());
                            }
                        }
                    }
                }
                #endregion
            }
            else // Post Back
            {
                #region Post Back

                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                var delegationEmployeePositions = this.delegationEmployeePositionsDAL.GetAll();

                #region From Employee
                foreach (ListItem fromEmployeeListItem in this.ddlFromEmployee.Items)
                {
                    List<LookupItem> delegatedEmployeePositions = new List<LookupItem>();
                    int employeeId = int.Parse(fromEmployeeListItem.Value);
                    var employeeInfo = this.employeeInfoDAL.GetByID(employeeId);
                    SetAttributesForEmployeeListItem(fromEmployeeListItem, employeeInfo);

                    if (employeeInfo.EmployeePosition != null)
                    {
                        var delegationEmployeePositionsOfEmployee = delegationEmployeePositions.Where(delegationEmployeePosition => delegationEmployeePosition.EmployeePosition.LookupId == employeeInfo.EmployeePosition.LookupId).ToList();
                        if (delegationEmployeePositionsOfEmployee != null && delegationEmployeePositionsOfEmployee.Count > 0)
                        {
                            foreach (var delegationEmployeePosition in delegationEmployeePositionsOfEmployee)
                            {
                                delegatedEmployeePositions.AddRange(delegationEmployeePosition.DelegatedEmployeePositions);

                                var toEmployees = this.employeeInfoDAL.GetDelegatedEmployeesByDepartment(delegationEmployeePosition.EmployeePosition.LookupId, employeeInfo.Department.LookupId, employeeInfo.FactoryLocation.LookupId);
                                IEnumerable<LookupItem> otherEmployeePositions = toEmployees.Where(e => e.DelegatedBy.Where(d => d.LookupId == delegationEmployeePosition.EmployeePosition.LookupId).Any()).Select(ep => ep.EmployeePosition).Distinct();
                                if (otherEmployeePositions != null && otherEmployeePositions.Any())
                                {
                                    delegatedEmployeePositions.AddRange(otherEmployeePositions);
                                }
                            }
                        }
                    }
                    delegatedEmployeePositions = delegatedEmployeePositions.GroupBy(e => e.LookupId).Select(g => g.First()).ToList();
                    fromEmployeeListItem.Attributes.Add(DelegatedEmployeePositionsAttribute, seriallizer.Serialize(delegatedEmployeePositions));
                }
                #endregion

                #region To Employee
                // this.hdSelectedToEmployees.Value = this.ddlToEmployee.SelectedValue;
                foreach (ListItem toEmployeeListItem in this.ddlToEmployee.Items)
                {
                    int employeeId = int.Parse(toEmployeeListItem.Value);
                    var employeeInfo = this.employeeInfoDAL.GetByID(employeeId);
                    SetAttributesForEmployeeListItem(toEmployeeListItem, employeeInfo);
                }
                #endregion

                #endregion
            }
        }

        /// <summary>
        /// InitEmployeeListItem
        /// </summary>
        /// <param name="employeeInfo"></param>
        /// <returns></returns>
        private ListItem InitEmployeeListItem(EmployeeInfo employeeInfo)
        {
            ListItem listItem = new ListItem();
            listItem.Value = employeeInfo.ID.ToString();
            listItem.Text = employeeInfo.FullName;

            // Factory
            if (employeeInfo.FactoryLocation != null)
            {
                listItem.Attributes.Add(EmployeeInfoList.FactoryLocationField, employeeInfo.FactoryLocation.LookupId.ToString());
            }
            // Department
            if (employeeInfo.Department != null)
            {
                listItem.Attributes.Add(EmployeeInfoList.DepartmentField, employeeInfo.Department.LookupId.ToString());
            }
            // Position
            if (employeeInfo.EmployeePosition != null)
            {
                listItem.Attributes.Add(EmployeeInfoList.EmployeePositionField, employeeInfo.EmployeePosition.LookupId.ToString());
            }

            return listItem;
        }

        private void SetAttributesForEmployeeListItem(ListItem listItem, EmployeeInfo employeeInfo)
        {
            // Factory
            if (employeeInfo.FactoryLocation != null)
            {
                listItem.Attributes.Add(EmployeeInfoList.FactoryLocationField, employeeInfo.FactoryLocation.LookupId.ToString());
            }
            // Department
            if (employeeInfo.Department != null)
            {
                listItem.Attributes.Add(EmployeeInfoList.DepartmentField, employeeInfo.Department.LookupId.ToString());
            }
            // Position
            if (employeeInfo.EmployeePosition != null)
            {
                listItem.Attributes.Add(EmployeeInfoList.EmployeePositionField, employeeInfo.EmployeePosition.LookupId.ToString());
            }
        }

        /// <summary>
        /// IsPositionBOD
        /// </summary>
        /// <param name="positionId"></param>
        /// <returns></returns>
        private bool IsPositionBOD(int positionId)
        {
            var res = false;

            var employeePosition = this.employeePositionDAL.GetByID(positionId);
            if (employeePosition != null)
            {
                res = (string.Compare(employeePosition.Code, EmployeePositionCode.BOD, true) == 0);
            }

            return res;
        }

        /// <summary>
        /// GetSelectedDelegationTasks
        /// </summary>
        /// <returns></returns>
        private List<Delegation> GetSelectedDelegationTasks()
        {
            var res = new List<Delegation>();

            var delegationTasks = this.Page.Session[DelegationTasksSessionKey] as List<Delegation>;

            if (delegationTasks != null && delegationTasks.Count > 0)
            {
                string title = string.Format("{0} - {1}", this.ddlFromEmployee.SelectedItem.Text, this.ddlToEmployee.SelectedItem.Text);
                DateTime fromDate = this.dtFromDate.SelectedDate.Date;
                DateTime toDate = this.dtToDate.SelectedDate.Date;
                LookupItem fromEmployee = new LookupItem { LookupId = int.Parse(this.ddlFromEmployee.SelectedItem.Value), LookupValue = this.ddlFromEmployee.SelectedItem.Text };
                List<LookupItem> toEmployees = new List<LookupItem>();
                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                var selectedToEmployees = seriallizer.Deserialize<List<int>>(this.hdSelectedToEmployees.Value);
                foreach (var toEmployee in selectedToEmployees)
                {
                    LookupItem lookupItem = new LookupItem();
                    lookupItem.LookupId = toEmployee;
                    toEmployees.Add(lookupItem);
                }

                foreach (GridViewRow gridViewRow in this.gridTasks.Rows)
                {
                    if (gridViewRow.RowType == DataControlRowType.DataRow)
                    {
                        var cbSelect = gridViewRow.FindControl("cbSelect") as CheckBox;
                        if (cbSelect.Checked)
                        {
                            Delegation delegation = delegationTasks[gridViewRow.RowIndex];
                            delegation.Title = title;
                            delegation.FromDate = fromDate;
                            delegation.ToDate = toDate;
                            delegation.FromEmployee = fromEmployee;
                            delegation.ToEmployee = toEmployees;
                            var isExisted = DelegationManager.IsDelegationExisted(delegation, this.SiteUrl);
                            if (!isExisted)
                            {
                                res.Add(delegation);
                            }
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// InitDelegationOfNewTask
        /// </summary>
        /// <param name="listUrl">The listUrl.</param>
        /// <returns></returns>
        private DelegationOfNewTask InitDelegationOfNewTask(string listUrl)
        {
            DelegationOfNewTask delegationOfNewTask = new DelegationOfNewTask();

            delegationOfNewTask.Title = string.Format("{0} - {1}", this.ddlFromEmployee.SelectedItem.Text, this.ddlToEmployee.SelectedItem.Text);
            delegationOfNewTask.FromDate = this.dtFromDate.SelectedDate.Date;
            delegationOfNewTask.ToDate = this.dtToDate.SelectedDate.Date;
            DelegationModule delegationModule = this.delegationModulesDAL.GetByListUrl(listUrl);
            if (delegationModule != null)
            {
                delegationOfNewTask.ModuleName = delegationModule.ModuleName;
                delegationOfNewTask.VietnameseModuleName = delegationModule.VietnameseModuleName;
                delegationOfNewTask.ListUrl = delegationModule.ListUrl;
            }
            delegationOfNewTask.FromEmployee = new LookupItem { LookupId = int.Parse(this.ddlFromEmployee.SelectedItem.Value), LookupValue = this.ddlFromEmployee.SelectedItem.Text };
            List<LookupItem> toEmployees = new List<LookupItem>();
            JavaScriptSerializer seriallizer = new JavaScriptSerializer();
            var selectedToEmployees = seriallizer.Deserialize<List<int>>(this.hdSelectedToEmployees.Value);
            foreach (var toEmployee in selectedToEmployees)
            {
                LookupItem lookupItem = new LookupItem();
                lookupItem.LookupId = toEmployee;
                toEmployees.Add(lookupItem);
            }
            delegationOfNewTask.ToEmployee = toEmployees;

            return delegationOfNewTask;
        }

        #endregion

        #region Email

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <returns></returns>
        private bool SendEmail()
        {
            var res = false;

            List<int> toEmployeeIds = new List<int>();
            foreach (ListItem listItem in this.ddlToEmployee.Items)
            {
                if (listItem.Selected)
                {
                    toEmployeeIds.Add(int.Parse(listItem.Value));
                }
            }

            res = SendEmail(toEmployeeIds);

            return res;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="toEmployeeIds"></param>
        /// <returns></returns>
        private bool SendEmail(List<int> toEmployeeIds)
        {
            var res = false;

            string subjectEmail = ConfigurationDAL.GetValue(this.SiteUrl, DelegationEmailTemplateSubjectKey);
            string delegationBodyEmailTemplate = ConfigurationDAL.GetValue(this.SiteUrl, DelegationEmailTemplateKey);
            DelegationEmailTemplate delegationEmailTemplate = InitDelegationEmailTemplateObject();
            string body = BuildBodyEmail(delegationEmailTemplate, delegationBodyEmailTemplate);
            res = SendEmail(toEmployeeIds, subjectEmail, body);

            return res;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="toEmployeeIds"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private bool SendEmail(List<int> toEmployeeIds, string subject, string body)
        {
            var res = false;

            if (toEmployeeIds != null && toEmployeeIds.Count > 0)
            {
                List<EmployeeInfo> toEmployees = new List<EmployeeInfo>();

                foreach (var employeeId in toEmployeeIds)
                {
                    var employeeInfo = this.employeeInfoDAL.GetByID(employeeId);
                    toEmployees.Add(employeeInfo);
                }

                res = SendEmail(toEmployees, subject, body);
            }

            return res;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="toEmployees"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private bool SendEmail(List<EmployeeInfo> toEmployees, string subject, string body)
        {
            var res = false;

            string[] tos = GetListOfEmails(toEmployees);
            res = SendEmail(tos, subject, body);

            return res;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <param name="tos"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private bool SendEmail(string[] tos, string subject, string body)
        {
            bool res = false;

            try
            {
                string replyTo = "";
                StringDictionary additionalHeaders = new StringDictionary();
                IEnumerable<AttachedEmailResourceDefinition> attachments = new List<AttachedEmailResourceDefinition>(); ;
                IEnumerable<EmbeddedEmailResourceDefinition> embeddedResources = new List<EmbeddedEmailResourceDefinition>();
                if (tos != null && tos.Length > 0)
                {
                    SPUtility.SendEmail(this.CurrentWeb, replyTo, tos, null, null, subject, additionalHeaders, body, attachments, embeddedResources);
                    res = true;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return res;

        }

        /// <summary>
        /// GetListOfEmails
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        private string[] GetListOfEmails(List<EmployeeInfo> employees)
        {
            string[] arrayAddress = null;

            if (employees != null && employees.Count > 0)
            {
                List<string> emails = new List<string>();

                foreach (var employee in employees)
                {
                    if (!string.IsNullOrEmpty(employee.Email) && !string.IsNullOrWhiteSpace(employee.Email))
                    {
                        emails.Add(employee.Email);
                    }
                }

                arrayAddress = emails.ToArray();
            }

            return arrayAddress;
        }

        /// <summary>
        /// InitDelegationEmailTemplateObject
        /// </summary>
        /// <returns></returns>
        private DelegationEmailTemplate InitDelegationEmailTemplateObject()
        {
            DelegationEmailTemplate delegationEmailTemplateObject = new DelegationEmailTemplate();

            StringBuilder receiversBuilder = new StringBuilder();
            foreach (ListItem listItem in this.ddlToEmployee.Items)
            {
                if (listItem.Selected)
                {
                    receiversBuilder.AppendFormat("{0}, ", listItem.Text);
                }
            }
            delegationEmailTemplateObject.Receivers = receiversBuilder.ToString();
            delegationEmailTemplateObject.FromEmployee = ddlFromEmployee.SelectedItem.Text;
            delegationEmailTemplateObject.FromDate = this.dtFromDate.SelectedDate.Date.ToString(DateFormatddMMyyyy2);
            delegationEmailTemplateObject.ToDate = this.dtToDate.SelectedDate.Date.ToString(DateFormatddMMyyyy2);
            delegationEmailTemplateObject.AccessLink = string.Format("{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx?{1}={2}", this.SiteUrl, DelegationListUserControl.TabParamName, DelegationListUserControl.DelegationsApprovalTabId);
            return delegationEmailTemplateObject;
        }

        /// <summary>
        /// BuildBodyEmail
        /// </summary>
        /// <param name="DelegationEmailTemplateObject"></param>
        /// <param name="delegationBodyEmailTemplate"></param>
        /// <returns></returns>
        private string BuildBodyEmail(DelegationEmailTemplate DelegationEmailTemplateObject, string delegationBodyEmailTemplate)
        {
            var body = string.Empty;

            try
            {
                BasicEvaluationContext basicEvaluationContext = new BasicEvaluationContext();
                basicEvaluationContext.Objects["DelegationEmailTemplate"] = DelegationEmailTemplateObject;
                body = basicEvaluationContext.Eval<string>(delegationBodyEmailTemplate);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
            }

            return body;
        }

        #endregion
    }
}
