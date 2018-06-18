using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.DiplomaManagementControl
{
    public partial class RequestForDiplomaSuppliesFormUserControl : ApprovalBaseUserControl
    {
        #region Constants
        public const string EmployeeListItem_EmployeeCode_Attribute = "employeecode";
        public const string EmployeeListItem_DateOfEmp_Attribute = "dateofemp";
        #endregion

        #region Fields
        private bool isEditable;
        private EmployeePositionDAL employeePositionDAL;
        private EmployeeInfoDAL employeeInfoDAL;
        private RequestDiplomaDetailDAL requestDiplomaDetailDAL;
        private SPFieldLookupValue currentStepLookupValue;
        #endregion

        #region Constructor
        public RequestForDiplomaSuppliesFormUserControl() : base()
        {
            this.workflowHistoryStyle = EWorkflowHistoryStyle.Simple;
        }
        #endregion

        #region Overrides

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                this.isEditable = this.IsEditable();
                this.hdIsEditable.Value = this.isEditable.ToString();
                this.employeePositionDAL = new EmployeePositionDAL(this.SiteUrl);
                this.employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
                this.requestDiplomaDetailDAL = new RequestDiplomaDetailDAL(this.SiteUrl);
                this.currentStepLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CurrentStep]);
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
                this.ShowClientMessage(ex.Message);
            }
        }

        protected override bool SaveForm()
        {
            bool res = false;

            try
            {
                if (this.isEditable)
                {
                    SaveData();

                    res = base.SaveForm();

                    if (res)
                    {
                        // Văn bằng đạt được/Obtained diploma
                        SaveRequestDiplomaDetails();
                    }
                }
                else // Approve - Save Form
                {
                    // Only BOD who has permission to update Received By (Department).
                    if (this.IsEmployeeBOD())
                    {
                        if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                        {
                            this.CurrentItem[RequestForDiplomaSuppliesList.Fields.DiplomaRevision] = this.chkboxDiplomaRevision.Checked;
                            this.CurrentItem[RequestForDiplomaSuppliesList.Fields.SalaryRevision] = this.chkboxSalaryRevision.Checked;
                            if (!this.dtFromDate.IsDateEmpty && this.dtFromDate.IsValid)
                            {
                                this.CurrentItem[RequestForDiplomaSuppliesList.Fields.CommonFrom] = this.dtFromDate.SelectedDate;
                            }
                            this.CurrentItem.Update();

                            res = true;
                        }
                    }
                    else // Only Apporval. No updating.
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
                res = false;
            }

            return res;
        }

        public override void Validate()
        {
            try
            {
                base.Validate();

                int employeeId = 0;
                int.TryParse(ddlEmployee.SelectedValue, out employeeId);
                if (employeeId < 1)
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RequestDiplomaForm_ErrorMessage_UnSelectedEmployee", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                string position = txtPosition.Text;
                if (string.IsNullOrEmpty(position) || string.IsNullOrWhiteSpace(position))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RequestDiplomaForm_ErrorMessage_EmptyPosition", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                string toTheDailyWorks = txtToTheDailyWorks.Text;
                if (string.IsNullOrEmpty(toTheDailyWorks) || string.IsNullOrWhiteSpace(toTheDailyWorks))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RequestDiplomaForm_ErrorMessage_EmptyToTheDailyWorks", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                string newSuggestions = txtNewSuggestions.Text;
                if (string.IsNullOrEmpty(newSuggestions) || string.IsNullOrWhiteSpace(newSuggestions))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RequestDiplomaForm_ErrorMessage_EmptyNewSuggestions", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                if (this.IsEmployeeBOD())
                {
                    if (!this.dtFromDate.IsValid)
                    {
                        IsValid = false;
                        hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RequestDiplomaForm_ErrorMessage_InvalidFromDate", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                        return;
                    }
                }

                IsValid = true;
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        public override bool IsRejectionAllowableAtAdditionalStep()
        {
            return false;
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        #endregion

        #region Methods

        #region Load Data

        private void LoadData()
        {
            // Load Requester Info
            LoadRqueterInfo();

            // Load List Of Employees
            LoadListOfEmployees();

            // Load Position
            LoadPosition();

            // Load List Of Request Diploma Details
            LoadListOfRequestDiplomaDetails();

            // Load To the daily works
            LoadToTheDailyWorks();

            // Load New suggestions/ Contributions/Achivements or Innovations to the Department
            LoadNewSuggestions();

            // Load Diploma Revision
            LoadDiplomaRevision();

            // Load Salary Revision
            LoadSalaryRevision();

            // Load FromDate
            LoadFromDate();

            // Load Approval Status
            hdApprovalStatus.Value = GetApprovalStatus();
        }

        private void LoadRqueterInfo()
        {
            try
            {
                if (ApprovalBaseManagerObject.Creator != null)
                {
                    this.lblRequester.Text = this.ApprovalBaseManagerObject.Creator.FullName;

                    if (ApprovalBaseManagerObject.Creator.Department != null)
                    {
                        var department = DepartmentListSingleton.GetDepartmentByID(ApprovalBaseManagerObject.Creator.Department.LookupId, this.SiteUrl);
                        if (department != null)
                        {

                            this.lblDepartment.Text = (CultureInfo.CurrentUICulture.LCID == 1066) ? department.VietnameseName : department.Name;
                        }
                    }

                    if (ApprovalBaseManagerObject.Creator.EmployeePosition != null)
                    {
                        var employeePosition = this.employeePositionDAL.GetByID(ApprovalBaseManagerObject.Creator.EmployeePosition.LookupId);
                        if (employeePosition != null)
                        {

                            this.lblPosition.Text = (this.IsVietnameseLanguage) ? employeePosition.VietnameseName : employeePosition.Name;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void LoadListOfEmployees()
        {
            try
            {
                string queryString = string.Format(@"<Where>
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
                                                            
                                                    </Where>", EmployeeInfoList.DepartmentField, ApprovalBaseManagerObject.Creator.Department.LookupId,
                                                       EmployeeInfoList.FactoryLocationField, ApprovalBaseManagerObject.Creator.FactoryLocation.LookupId);
                var employees = employeeInfoDAL.GetByQuery(queryString);
                if (employees != null)
                {
                    employees = employees.OrderBy(emp => emp.FullName).ToList();

                    foreach (var employeeInfo in employees)
                    {
                        var listItem = NewEmployeeListItem(employeeInfo);
                        this.ddlEmployee.Items.Add(listItem);
                    }

                    if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.Employee] != null)
                    {
                        SPFieldLookupValue employeeLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestForDiplomaSuppliesList.Fields.Employee]);
                        this.ddlEmployee.SelectedValue = employeeLookupValue.LookupId.ToString();
                    }
                }
                var emptyListItem = NewEmptyEmployeeListItem();
                this.ddlEmployee.Items.Insert(0, emptyListItem);
                this.ddlEmployee.Enabled = this.isEditable;
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        private void LoadPosition()
        {
            try
            {
                if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.Position] != null)
                {
                    this.txtPosition.Text = this.CurrentItem[RequestForDiplomaSuppliesList.Fields.Position].ToString();
                }

                this.txtPosition.Enabled = this.isEditable;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        private void LoadListOfRequestDiplomaDetails()
        {
            try
            {
                #region Query
                string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", RequestBuyDetailsList.Fields.Request, this.CurrentItem.ID);
                #endregion

                var requestDiplomaDetailItems = this.requestDiplomaDetailDAL.GetByQuery(queryString);
                if (requestDiplomaDetailItems != null)
                {
                    JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                    this.hdRequestDiplomaDetails.Value = seriallizer.Serialize(requestDiplomaDetailItems);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        private void LoadToTheDailyWorks()
        {
            try
            {
                if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.ToTheDailyWorks] != null)
                {
                    this.txtToTheDailyWorks.Text = this.CurrentItem[RequestForDiplomaSuppliesList.Fields.ToTheDailyWorks].ToString();
                }

                this.txtToTheDailyWorks.Enabled = this.isEditable;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        private void LoadNewSuggestions()
        {
            try
            {
                if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.NewSuggestions] != null)
                {
                    this.txtNewSuggestions.Text = this.CurrentItem[RequestForDiplomaSuppliesList.Fields.NewSuggestions].ToString();
                }

                this.txtNewSuggestions.Enabled = this.isEditable;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        private void LoadDiplomaRevision()
        {
            try
            {
                #region DEL. 2017.09.29.
                //// Da qua buoc BOD
                //if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.DiplomaRevision] != null)
                //{
                //    if (!this.IsRejectedStatus())
                //    {
                //        this.trDiplomaRevision.Visible = true;
                //        this.chkboxDiplomaRevision.Checked = bool.Parse(this.CurrentItem[RequestForDiplomaSuppliesList.Fields.DiplomaRevision].ToString());
                //        this.chkboxDiplomaRevision.Enabled = false;
                //    }
                //}
                //else    // Chua qua buoc BOD
                //{
                //    if (this.IsSubmittedStatus() && !this.IsCreator())
                //    {
                //        this.trDiplomaRevision.Visible = true;
                //        if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                //        {
                //            this.chkboxDiplomaRevision.Enabled = true;
                //        }
                //        else
                //        {
                //            this.chkboxDiplomaRevision.Enabled = false;
                //        }
                //    }
                //}
                #endregion

                #region ADD. 2017.09.29. Copy and modify from [LoadFromDate] method.
                if ((this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Display) ||
                    (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit))
                {
                    // BOD đã approve hoặc reject
                    if (this.currentStepLookupValue == null)
                    {
                        if (!this.IsRejectedStatus() && !this.IsCancelWorkflowStatus())
                        {
                            this.trDiplomaRevision.Visible = true;
                            this.chkboxDiplomaRevision.Enabled = false;
                        }
                    }
                    else    // Chưa qua step BOD
                    {
                        if (this.IsEmployeeBOD())
                        {
                            this.trDiplomaRevision.Visible = true;
                            if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                            {
                                this.chkboxDiplomaRevision.Enabled = true;
                            }
                            else
                            {
                                this.chkboxDiplomaRevision.Enabled = false;
                            }
                        }
                    }

                    if (this.trDiplomaRevision.Visible == true)
                    {
                        if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.DiplomaRevision] != null)
                        {
                            this.chkboxDiplomaRevision.Checked = bool.Parse(this.CurrentItem[RequestForDiplomaSuppliesList.Fields.DiplomaRevision].ToString());
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        private void LoadSalaryRevision()
        {
            try
            {
                #region DEL. 2017.09.29.
                //// Da qua buoc BOD
                //if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.SalaryRevision] != null)
                //{
                //    if (!this.IsRejectedStatus())
                //    {
                //        this.trSalaryRevision.Visible = true;
                //        this.chkboxSalaryRevision.Checked = bool.Parse(this.CurrentItem[RequestForDiplomaSuppliesList.Fields.SalaryRevision].ToString());
                //        this.chkboxSalaryRevision.Enabled = false;
                //    }
                //}
                //else    // Chua qua buoc BOD
                //{
                //    if (this.IsSubmittedStatus() && !this.IsCreator())
                //    {
                //        this.trSalaryRevision.Visible = true;
                //        if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                //        {
                //            this.chkboxSalaryRevision.Enabled = true;
                //        }
                //        else
                //        {
                //            this.chkboxSalaryRevision.Enabled = false;
                //        }
                //    }
                //}
                #endregion

                #region ADD. 2017.09.29. Copy and modify from [LoadFromDate] method.
                if ((this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Display) ||
                    (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit))
                {
                    // BOD đã approve hoặc reject
                    if (this.currentStepLookupValue == null)
                    {
                        if (!this.IsRejectedStatus() && !this.IsCancelWorkflowStatus())
                        {
                            this.trSalaryRevision.Visible = true;
                            this.chkboxSalaryRevision.Enabled = false;
                        }
                    }
                    else    // Chưa qua step BOD
                    {
                        if (this.IsEmployeeBOD())
                        {
                            this.trSalaryRevision.Visible = true;
                            if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                            {
                                this.chkboxSalaryRevision.Enabled = true;
                            }
                            else
                            {
                                this.chkboxSalaryRevision.Enabled = false;
                            }
                        }
                    }

                    if (this.trSalaryRevision.Visible == true)
                    {
                        if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.SalaryRevision] != null)
                        {
                            this.chkboxSalaryRevision.Checked = bool.Parse(this.CurrentItem[RequestForDiplomaSuppliesList.Fields.SalaryRevision].ToString());
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        private void LoadFromDate()
        {
            try
            {
                #region DEL. 2017.09.29.
                //// Da qua buoc BOD
                //if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.CommonFrom] != null)
                //{
                //    if (!this.IsRejectedStatus())
                //    {
                //        this.trFromDate.Visible = true;
                //        DateTime fromDate;
                //        DateTime.TryParse(this.CurrentItem[RequestForDiplomaSuppliesList.Fields.CommonFrom].ToString(), out fromDate);
                //        this.dtFromDate.SelectedDate = fromDate;
                //        this.dtFromDate.Enabled = false;
                //    }
                //}
                //else    // Chua qua buoc BOD
                //{
                //    if (this.IsSubmittedStatus() && !this.IsCreator())
                //    {
                //        this.trFromDate.Visible = true;
                //        if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                //        {
                //            this.dtFromDate.Enabled = true;
                //        }
                //        else
                //        {
                //            this.dtFromDate.Enabled = false;
                //        }
                //    }
                //}
                #endregion

                #region ADD. 2017.09.29.
                /*
                    - Nếu CurrentStep == NULL (BOD đã approve hoặc reject)
	                    + Nếu Status != Rejected
		                    ++ Show := TRUE
                            ++ Don't allow edition [From Date]
	                    + Nếu Status == Rejected
		                    ++ Show := FALSE
                    - Nếu CurrentStep != NULL (Chưa qua step BOD)
	                    + Nếu User là BOD
		                    ++ Show := TRUE
		                    ++ Nếu FormMode == EDIT
			                    +++ Allow edition [From Date]
		                    ++ Nếu FormMode != EDIT
			                    +++ Don't allow edition [From Date]
	                    + Nếu User không phải là BOD
		                    ++ Show := FALSE
                 */
                if ((this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Display) ||
                    (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit))
                {
                    // BOD đã approve hoặc reject
                    if (this.currentStepLookupValue == null)
                    {
                        if (!this.IsRejectedStatus() && !this.IsCancelWorkflowStatus())
                        {
                            this.trFromDate.Visible = true;
                            this.dtFromDate.Enabled = false;
                        }
                    }
                    else    // Chưa qua step BOD
                    {
                        if (this.IsEmployeeBOD())
                        {
                            this.trFromDate.Visible = true;
                            if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                            {
                                this.dtFromDate.Enabled = true;
                            }
                            else
                            {
                                this.dtFromDate.Enabled = false;
                            }
                        }
                    }

                    if (this.trFromDate.Visible == true)
                    {
                        if (this.CurrentItem[RequestForDiplomaSuppliesList.Fields.CommonFrom] != null)
                        {
                            DateTime fromDate;
                            DateTime.TryParse(this.CurrentItem[RequestForDiplomaSuppliesList.Fields.CommonFrom].ToString(), out fromDate);
                            this.dtFromDate.SelectedDate = fromDate;
                        }
                    }
                }
                
                #endregion
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }
        }

        private ListItem NewEmployeeListItem(EmployeeInfo employeeInfo)
        {
            ListItem listItem = new ListItem();
            listItem.Value = employeeInfo.ID.ToString();
            listItem.Text = employeeInfo.FullName;
            listItem.Attributes.Add(EmployeeListItem_EmployeeCode_Attribute, employeeInfo.EmployeeID);
            if (employeeInfo.JoinedDate != null)
            {
                listItem.Attributes.Add(EmployeeListItem_DateOfEmp_Attribute, employeeInfo.JoinedDate.ToString(DateFormatddMMyyyy2));
            }
            else
            {
                listItem.Attributes.Add(EmployeeListItem_DateOfEmp_Attribute, "");
            }
            return listItem;
        }

        private ListItem NewEmptyEmployeeListItem()
        {
            ListItem listItem = new ListItem();
            listItem.Value = "0";
            listItem.Text = "Select/Chọn";
            listItem.Attributes.Add(EmployeeListItem_EmployeeCode_Attribute, "");
            listItem.Attributes.Add(EmployeeListItem_DateOfEmp_Attribute, "");
            
            return listItem;
        }

        #endregion

        #region Save Data

        private void SaveData()
        {
            // Employee
            int selectEmployeeId = 0;
            int.TryParse(this.ddlEmployee.SelectedValue, out selectEmployeeId);
            if (selectEmployeeId > 0)
            {
                this.CurrentItem[RequestForDiplomaSuppliesList.Fields.Employee] = selectEmployeeId;
                ListItem selectedListItem = this.ddlEmployee.SelectedItem;
                var employeeInfo = this.employeeInfoDAL.GetByID(selectEmployeeId);
                if (employeeInfo != null)
                {
                    this.CurrentItem[RequestForDiplomaSuppliesList.Fields.EmployeeName] = employeeInfo.FullName;
                    this.CurrentItem[RequestForDiplomaSuppliesList.Fields.EmployeeCode] = employeeInfo.EmployeeID;
                    this.CurrentItem[RequestForDiplomaSuppliesList.Fields.DateOfEmp] = employeeInfo.JoinedDate;
                }
            }
            // Position
            this.CurrentItem[RequestForDiplomaSuppliesList.Fields.Position] = txtPosition.Text;
            // To the daily works
            this.CurrentItem[RequestForDiplomaSuppliesList.Fields.ToTheDailyWorks] = txtToTheDailyWorks.Text;
            // New suggestions/ Contributions/Achivements or Innovations to the Department
            this.CurrentItem[RequestForDiplomaSuppliesList.Fields.NewSuggestions] = txtNewSuggestions.Text;
        }

        private bool SaveRequestDiplomaDetails()
        {
            var res = true;

            try
            {
                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                List<RequestDiplomaDetail> requestDiplomaDetailItems = seriallizer.Deserialize<List<RequestDiplomaDetail>>(hdRequestDiplomaDetails.Value);

                #region Delete List Of Detail Items
                string queryString = string.Format(@"<Where>
                                                            <Eq>
                                                                <FieldRef Name='{0}' LookupId='True' />
                                                                <Value Type='Lookup'>{1}</Value>
                                                            </Eq>
                                                       </Where>", StringConstant.RequestDiplomaDetailsList.Fields.Request, this.CurrentItem.ID);
                IList<int> ids = null;
                var currentItems = this.requestDiplomaDetailDAL.GetByQuery(queryString);
                if (currentItems != null && currentItems.Count > 0)
                {
                    ids = new List<int>();

                    foreach (var item in currentItems)
                    {
                        var existedItem = requestDiplomaDetailItems.Where(requestDiplomaDetailItem => requestDiplomaDetailItem.ID == item.ID).FirstOrDefault();
                        // Neu khong ton tai => Xoa
                        if (existedItem == null)
                        {
                            ids.Add(item.ID);
                        }
                    }
                }

                if (ids != null && ids.Count > 0)
                {
                    this.requestDiplomaDetailDAL.DeleteItems(ids);
                }
                #endregion

                foreach (var item in requestDiplomaDetailItems)
                {
                    item.Request = new LookupItem { LookupId = this.CurrentItem.ID };
                    this.requestDiplomaDetailDAL.SaveItem(item);
                }
            }
            catch (Exception ex)
            {
                res = false;
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesFormUserControl: {ex.Message}");
            }

            return res;
        }

        #endregion

        #endregion
    }
}
