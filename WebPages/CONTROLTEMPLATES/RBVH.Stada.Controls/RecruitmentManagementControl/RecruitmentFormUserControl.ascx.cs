using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.WebPages.Common;
using RBVH.Stada.Intranet.WebPages.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RecruitmentManagementControl
{
    /// <summary>
    /// RecruitmentFormUserControl class.
    /// </summary>
    public partial class RecruitmentFormUserControl : ApprovalBaseUserControl
    {
        #region Constants

        /// <summary>
        /// Req#
        /// </summary>
        private const string PrefixRequest = "Req {0}";

        /// <summary>
        /// Khác/Others
        /// </summary>
        private const string OtherValueChoiceFieldValue = "Khác/Others";

        /// <summary>
        /// ;#
        /// </summary>
        public const string ChoiceValueSpliter = ";#";

        /// <summary>
        /// 1
        /// </summary>
        private const string IsShowValue_Yes = "1";

        /// <summary>
        /// 0
        /// </summary>
        private const string IsShowValue_No = "0";

        /// <summary>
        /// selecttype
        /// </summary>
        private const string CheckBoxList_SelectType_Attribute = "selecttype";

        /// <summary>
        /// radiobutton
        /// </summary>
        private const string CheckBoxList_SelectType_Value = "radiobutton";

        /// <summary>
        /// othervaluecontrolid
        /// </summary>
        private const string OtherValueControlId_Attribute = "othervaluecontrolid";

        /// <summary>
        /// PrintLinkEN
        /// </summary>
        public const string PrintLinkEN_Key = "PrintLinkEN";

        /// <summary>
        /// PrintLinkVN
        /// </summary>
        public const string PrintLinkVN_Key = "PrintLinkVN";

        /// <summary>
        /// RecruitmentForm_NumberOfStandardizedDaysBeforesubmission
        /// </summary>
        private const string NumberOfStandardizedDaysBeforesubmission_Key = "RecruitmentForm_NumberOfStandardizedDaysBeforesubmission";

        private const string NoneTemplateValue = "0";

        private const string RecruitmentForm_NumberOfDefaultQuantity_Key = "RecruitmentForm_NumberOfDefaultQuantity";
        #endregion

        #region Fields
        private bool isEditable;
        private ForeignLanguageDAL foreignLanguageDAL;
        private ForeignLanguageLevelDAL foreignLanguageLevelDAL;
        private RecruitmentLanguageSkillsDAL recruitmentLanguageSkillsDAL;
        private SPListItem templateItem;
        #endregion

        #region Constructor
        /// <summary>
        /// RecruitmentFormUserControl
        /// </summary>
        public RecruitmentFormUserControl() : base()
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
                foreignLanguageDAL = new ForeignLanguageDAL(this.SiteUrl);
                foreignLanguageLevelDAL = new ForeignLanguageLevelDAL(this.SiteUrl);
                recruitmentLanguageSkillsDAL = new RecruitmentLanguageSkillsDAL(this.SiteUrl);
                isEditable = this.IsEditable();
                this.hdIsEditable.Value = this.isEditable.ToString();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
            }
        }

        public override void Validate()
        {
            try
            {
                base.Validate();

                int departmentId = 0;
                int.TryParse(ddlDepartment.SelectedValue, out departmentId);
                if (departmentId < 1)
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_UnSelectedDepartment", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                if (cbSavingTemplate.Checked)
                {
                    var selectedTemplateId = ddlTemplate.SelectedValue;
                    if (string.Compare(selectedTemplateId, NoneTemplateValue, true) == 0)
                    {
                        string templateName = txtTemplateName.Text;
                        if (string.IsNullOrEmpty(templateName) || string.IsNullOrWhiteSpace(templateName))
                        {
                            IsValid = false;
                            hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_EmptyTemplateName", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                            return;
                        }
                        else
                        {
                            string queryString = string.Format(@"<Where>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{0}' />
                                                                <Value Type='Text'>{1}</Value>
                                                            </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{2}' />
                                                                <Value Type='Boolean'>{3}</Value>
                                                            </Eq>
                                                        </And>
                                                       </Where>",
                                        EmployeeRequirementSheetsList.Fields.Title, templateName,
                                        EmployeeRequirementSheetsList.Fields.IsTemplate, "1");
                            EmployeeRequirementSheetDAL employeeRequirementSheetDAL = new EmployeeRequirementSheetDAL(this.SiteUrl);
                            var templates = employeeRequirementSheetDAL.GetByQuery(queryString);
                            if (templates != null && templates.Count > 0)
                            {
                                IsValid = false;
                                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_ExistedTemplateName", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                                return;
                            }
                        }
                    }
                }

                string position = txtPosition.Text;
                if (string.IsNullOrEmpty(position) || string.IsNullOrWhiteSpace(position))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_EmptyPosition", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                string quantity = txtQuantity.Text;
                if (string.IsNullOrEmpty(quantity) || string.IsNullOrWhiteSpace(quantity))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_Quantity_Empty", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                int numQuantity = 0;
                int.TryParse(quantity, out numQuantity);
                if (numQuantity < 1)
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_Quantity_InValid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                string reasonForRecruitment = txtReasonForRecruitment.Text;
                if (string.IsNullOrEmpty(reasonForRecruitment) || string.IsNullOrWhiteSpace(reasonForRecruitment))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_EmptyReasonForRecruitment", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                string fromAge = txtFromAge.Text;
                if (string.IsNullOrEmpty(fromAge) || string.IsNullOrWhiteSpace(fromAge))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_FromAge_Empty", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                int numFromAge = 0;
                int.TryParse(fromAge, out numFromAge);
                if (numFromAge < 1 || numFromAge > 100)
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_FromAge_InValid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                string toAge = txtToAge.Text;
                if (string.IsNullOrEmpty(toAge) || string.IsNullOrWhiteSpace(toAge))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_ToAge_Empty", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                int numToAge = 0;
                int.TryParse(toAge, out numToAge);
                if (numToAge < 1 || numToAge > 100)
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_ToAge_InValid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                if ((numToAge != 0) && (numFromAge > numToAge))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_FromAge_LessThan_ToAge", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                if (dtAvailableTime.IsDateEmpty)
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_AvailableTime_Empty", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                if (!dtAvailableTime.IsValid)
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("RecruitmentForm_ErrorMessage_AvailableTime_Invalid", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                IsValid = true;
            }
            catch (Exception ex)
            {
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
                ULSLogging.LogError(ex);
            }
        }

        protected override bool SaveForm()
        {
            bool res = false;

            try
            {
                SaveData();

                res = base.SaveForm();

                // Language Skills
                SaveLanguageSkills();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
                res = false;
            }

            return res;
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);

                cblAppearance.Attributes.Add(CheckBoxList_SelectType_Attribute, CheckBoxList_SelectType_Value);
                cblWorkingExperience.Attributes.Add(CheckBoxList_SelectType_Attribute, CheckBoxList_SelectType_Value);
                cblWorkingExperience.Attributes.Add(OtherValueControlId_Attribute, txtOtherWorkingExperience.ClientID);
                cblComputerSkills.Attributes.Add(OtherValueControlId_Attribute, this.txtOtherComputerSkills.ClientID);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
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
                this.OnAfterApproved += RecruitmentFormUserControl_OnAfterApproved;
                this.hdOtherValueChoiceFieldValue.Value = OtherValueChoiceFieldValue;
                this.lbtnDeleteTemplate.Click += lbtnDeleteTemplate_Click;
                this.ddlTemplate.SelectedIndexChanged += ddlTemplate_SelectedIndexChanged;
                if (!Page.IsPostBack)
                {
                    LoadData();
                }

                hdToday.Value = DateTime.Now.ToString(DateFormatddMMyyyy2);
                if (this.isEditable)
                {
                    var numberOfStandardizedDaysBeforesubmission = GetNumberOfStandardizedDaysBeforeSubmission();
                    hdNumberOfStandardizedDaysBeforesubmission.Value = numberOfStandardizedDaysBeforesubmission.ToString();
                }

                #region DucVT ADD. 2017.10.10. TFS#1595.
                if ((CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.New) && this.isEditable)
                {
                    this.trSavingTemplate.Visible = true;
                    this.tdLitTemplate.Visible = true;
                    this.tdDllTemplate.Visible = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
            }
        }

        private void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.Compare(ddlTemplate.SelectedValue, NoneTemplateValue, true) != 0)
                {
                    lbtnDeleteTemplate.Visible = true;

                    // Load template info
                    int templateId = 0;
                    if (int.TryParse(ddlTemplate.SelectedValue, out templateId))
                    {
                        if (templateId > 0)
                        {
                            this.templateItem = this.CurrentList.GetItemById(templateId);
                        }
                    }
                }
                else
                {
                    lbtnDeleteTemplate.Visible = false;
                }

                LoadForm();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
            }
        }

        private void RecruitmentFormUserControl_OnAfterApproved(object sender, EventArgs e)
        {
            try
            {
                #region DEL 2017.09.27. Không cần IN. TFS #1586
                //var currentStep = this.CurrentItem[ApprovalFields.CurrentStep];
                //var additionalStep = this.CurrentItem[ApprovalFields.AdditionalStep];
                //var additionalDepartment = this.CurrentItem[ApprovalFields.AdditionalDepartment];
                //var status = ObjectHelper.GetString(this.CurrentItem[ApprovalFields.WFStatus]);

                //if ((currentStep == null) && (additionalStep != null) && (additionalDepartment != null) && (string.Compare(status, ApprovalStatus.Approved, true) != 0))
                //{
                //    RecruitmentTemplate RecruitmentTemplate = new RecruitmentTemplate(this.CurrentWeb, this.CurrentItem);
                //    string urlOfFileFormData = RecruitmentTemplate.ExportFormData();

                //    if (!string.IsNullOrEmpty(urlOfFileFormData))
                //    {
                //        string linkPrintEN = string.Format("<p>You can click on this <a href=\"{0}\">link<a/> to print form request.<p>", urlOfFileFormData);
                //        string linkPrintVN = string.Format("<p>Vui lòng truy vập vào <a href=\"{0}\">liên kết<a/> để in phiếu yêu cầu tuyển dụng.</p>", urlOfFileFormData);
                //        this.ApprovalBaseManagerObject.AdditionalInfoEmailObject[RecruitmentFormUserControl.PrintLinkEN_Key] = linkPrintEN;
                //        this.ApprovalBaseManagerObject.AdditionalInfoEmailObject[RecruitmentFormUserControl.PrintLinkVN_Key] = linkPrintVN;
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
            }
        }

        private void lbtnDeleteTemplate_Click(object sender, EventArgs e)
        {
            if (DeleteTemplate(ddlTemplate.SelectedValue))
            {
                // Load list of templates
                LoadListOfTemplates();
                // Invisible delete button
                lbtnDeleteTemplate.Visible = false;
                // Load form
                LoadForm();
            }
        }

        #endregion

        #region Methods

        #region Load Data
        private void LoadData()
        {
            LoadHiddenFieldValues();
            LoadDepartments();
            #region ADD. 2017.10.10. TFS#1595. Load list of templates.
            if ((CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.New) && this.isEditable)
            {
                LoadListOfTemplates();
            }
            #endregion
            LoadForm();

            // Load Approval Status
            hdApprovalStatus.Value = GetApprovalStatus();
        }

        private void LoadHiddenFieldValues()
        {
            try
            {
                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                var foreignLanguages = this.foreignLanguageDAL.GetAll();
                if (foreignLanguages != null && foreignLanguages.Count > 0)
                {
                    hdForeignLanguages.Value = seriallizer.Serialize(foreignLanguages);
                }

                var foreignLanguageLevels = this.foreignLanguageLevelDAL.GetAll();
                if (foreignLanguageLevels != null && foreignLanguageLevels.Count > 0)
                {
                    hdForeignLanguageLevels.Value = seriallizer.Serialize(foreignLanguageLevels);
                }

                hdNoneTemplateValue.Value = NoneTemplateValue;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
            }
        }

        private void LoadDepartments()
        {
            try
            {
                this.ddlDepartment.DataValueField = "ID";
                if (this.IsVietnameseLanguage)
                {
                    this.ddlDepartment.DataTextField = "VietnameseName";
                }
                else
                {
                    this.ddlDepartment.DataTextField = "Name";
                }
                var departments = new List<Department>();

                if (this.CurrentItem[EmployeeRequirementSheetsList.Fields.RecruitmentDepartment] != null)
                {
                    SPFieldLookupValue departmentLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[EmployeeRequirementSheetsList.Fields.RecruitmentDepartment]);
                    var department = this.ApprovalBaseManagerObject.DepartmentDAL.GetByID(departmentLookupValue.LookupId);
                    if (department != null)
                    {
                        departments.Add(department);
                    }
                }
                else
                {
                    // Người của phòng ban nào thì làm đơn cho phòng ban của mình thôi. Không làm đơn giúp cho phòng ban khác.
                    var currentEmployee = this.ApprovalBaseManagerObject.CurrentEmployee;
                    if (currentEmployee != null)
                    {
                        if (currentEmployee.Department != null)
                        {
                            var department = this.ApprovalBaseManagerObject.DepartmentDAL.GetByID(currentEmployee.Department.LookupId);
                            if (department != null)
                            {
                                departments.Add(department);
                            }
                        }
                    }
                }
                this.ddlDepartment.DataSource = departments;
                this.ddlDepartment.DataBind();

                #region MOD. 2017.09.29.
                // Tiến 29.09.2017: Chỗ bộ phận anh Disable luôn dùm em. TFS #1587.
                //this.ddlDepartment.Enabled = this.isEditable;
                this.ddlDepartment.Enabled = false;
                #endregion
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
            }
        }

        private void LoadForm()
        {
            SPListItem listItem = GetListItemForLoadForm();

            LoadGeneralRequirements(listItem);
            LoadNecessaryRequirements(listItem);
            LoadSkillRequirements(listItem);
            LoadAnothersRequirements(listItem);
        }

        private void LoadGeneralRequirements(SPListItem listItem)
        {
            LoadPosition(listItem);
            LoadQuanity(listItem);
            LoadReasonForRecruitment(listItem);
            LoadGender(listItem);
            LoadFromAge(listItem);
            LoadToAge(listItem);
            LoadMartialStatus(listItem);
            LoadAvailableTime();
            LoadWorkingTime(listItem);
            LoadEducationLevel(listItem);
            LoadAppearance(listItem);
            LoadWorkingExperience(listItem);
            LoadSpecialities(listItem);
            LoadDesciptionOfBasicWork(listItem);
        }

        private void LoadNecessaryRequirements(SPListItem listItem)
        {
            LoadMoralVocations(listItem);
            LoadWorkingAbilities(listItem);
        }

        private void LoadSkillRequirements(SPListItem listItem)
        {
            LoadLanguageSkills(listItem);
            LoadComputerSkills(listItem);
            LoadAnotherSkills(listItem);
        }

        private void LoadPosition(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.Position] != null)
            {
                this.txtPosition.Text = listItem[EmployeeRequirementSheetsList.Fields.Position].ToString();
            }
            else
            {
                this.txtPosition.Text = string.Empty;
            }

            this.txtPosition.Enabled = this.isEditable;
        }

        private void LoadQuanity(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.Quantity] != null)
            {
                this.txtQuantity.Text = listItem[EmployeeRequirementSheetsList.Fields.Quantity].ToString();
            }
            else
            {
                this.txtQuantity.Text = ConfigurationDAL.GetValue(this.SiteUrl, RecruitmentForm_NumberOfDefaultQuantity_Key);
            }

            this.txtQuantity.Enabled = this.isEditable;
        }

        private void LoadReasonForRecruitment(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.ReasonsForRecruitment] != null)
            {
                this.txtReasonForRecruitment.Text = listItem[EmployeeRequirementSheetsList.Fields.ReasonsForRecruitment].ToString();
            }
            else
            {
                this.txtReasonForRecruitment.Text = string.Empty;
            }

            this.txtReasonForRecruitment.Enabled = this.isEditable;
        }

        private void LoadGender(SPListItem listItem)
        {
            this.cblGender.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.Sex, listItem, ChoiceValueSpliter);
            this.cblGender.Enabled = this.isEditable;
        }

        private void LoadFromAge(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.FromAge] != null)
            {
                this.txtFromAge.Text = listItem[EmployeeRequirementSheetsList.Fields.FromAge].ToString();
            }
            else
            {
                this.txtFromAge.Text = string.Empty;
            }

            this.txtFromAge.Enabled = this.isEditable;
        }

        private void LoadToAge(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.ToAge] != null)
            {
                this.txtToAge.Text = listItem[EmployeeRequirementSheetsList.Fields.ToAge].ToString();
            }
            else
            {
                this.txtToAge.Text = string.Empty;
            }

            this.txtToAge.Enabled = this.isEditable;
        }

        private void LoadMartialStatus(SPListItem listItem)
        {
            this.cblMartialStatus.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.MaritalStatus, listItem, ChoiceValueSpliter);
            this.cblMartialStatus.Enabled = this.isEditable;
        }

        private void LoadAvailableTime()
        {
            if (this.CurrentItem[EmployeeRequirementSheetsList.Fields.AvailableTime] != null)
            {
                DateTime availableTime;
                DateTime.TryParse(this.CurrentItem[EmployeeRequirementSheetsList.Fields.AvailableTime].ToString(), out availableTime);
                this.dtAvailableTime.SelectedDate = availableTime;
            }
            else
            {
                this.dtAvailableTime.ClearSelection();
            }

            this.dtAvailableTime.Enabled = this.isEditable;
        }

        private void LoadWorkingTime(SPListItem listItem)
        {
            this.cblWorkingTime.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.WorkingTime, listItem, ChoiceValueSpliter);
            this.cblWorkingTime.Enabled = this.isEditable;
        }

        private void LoadEducationLevel(SPListItem listItem)
        {
            this.cblEducationLevel.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.EducationLevel, listItem, ChoiceValueSpliter);
            this.cblEducationLevel.Enabled = this.isEditable;

        }

        private void LoadAppearance(SPListItem listItem)
        {
            this.cblAppearance.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.Appearance, listItem, ChoiceValueSpliter);
            this.cblAppearance.Enabled = this.isEditable;
        }

        private void LoadWorkingExperience(SPListItem listItem)
        {
            this.cblWorkingExperience.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.WorkingExperience, listItem, txtOtherWorkingExperience, ChoiceValueSpliter);
            this.cblWorkingExperience.Enabled = this.isEditable;
            this.txtOtherWorkingExperience.Enabled = this.isEditable;
        }

        private void LoadSpecialities(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.Specialities] != null)
            {
                this.txtSpecialities.Text = listItem[EmployeeRequirementSheetsList.Fields.Specialities].ToString();
            }
            else
            {
                this.txtSpecialities.Text = string.Empty;
            }

            this.txtSpecialities.Enabled = this.isEditable;
        }

        private void LoadDesciptionOfBasicWork(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.DescriptionOfBasicWork] != null)
            {
                this.txtDescriptionOfBasicWork.Text = listItem[EmployeeRequirementSheetsList.Fields.DescriptionOfBasicWork].ToString();
            }
            else
            {
                this.txtDescriptionOfBasicWork.Text = string.Empty;
            }

            this.txtDescriptionOfBasicWork.Enabled = this.isEditable;
        }

        private void LoadMoralVocations(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.MoralVocations] != null)
            {
                this.txtMoralVocations.Text = listItem[EmployeeRequirementSheetsList.Fields.MoralVocations].ToString();
            }
            else
            {
                this.txtMoralVocations.Text = string.Empty;
            }

            this.txtMoralVocations.Enabled = this.isEditable;
        }

        private void LoadWorkingAbilities(SPListItem listItem)
        {
            if (listItem[EmployeeRequirementSheetsList.Fields.WorkingAbilities] != null)
            {
                this.txtWorkingAbilities.Text = listItem[EmployeeRequirementSheetsList.Fields.WorkingAbilities].ToString();
            }
            else
            {
                this.txtWorkingAbilities.Text = string.Empty;
            }

            this.txtWorkingAbilities.Enabled = this.isEditable;
        }

        private void LoadLanguageSkills(SPListItem listItem)
        {
            hdRecruitmentLanguageSkills.Value = string.Empty;

            #region Query
            string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", RecruitmentLanguageSkillsList.Fields.Request, listItem.ID);
            #endregion

            List<RecruitmentLanguageSkill> recruitmentLanguageSkillItems = recruitmentLanguageSkillsDAL.GetByQuery(queryString);
            List<RecruitmentLanguageSkillModel> recruitmentLanguageSkillModelItems = null;
            if (recruitmentLanguageSkillItems != null && recruitmentLanguageSkillItems.Count > 0)
            {
                recruitmentLanguageSkillModelItems = new List<RecruitmentLanguageSkillModel>();
                foreach (var recruitmentLanguageSkillItem in recruitmentLanguageSkillItems)
                {
                    RecruitmentLanguageSkillModel recruitmentLanguageSkillModel = new RecruitmentLanguageSkillModel
                    {
                        ForeignLanguage = recruitmentLanguageSkillItem.ForeignLanguage.LookupId,
                        Level = recruitmentLanguageSkillItem.Level
                    };
                    recruitmentLanguageSkillModelItems.Add(recruitmentLanguageSkillModel);
                }
                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                //hdRecruitmentLanguageSkills.Value = seriallizer.Serialize(recruitmentLanguageSkillItems);
                hdRecruitmentLanguageSkills.Value = seriallizer.Serialize(recruitmentLanguageSkillModelItems);
            }
        }

        private void LoadComputerSkills(SPListItem listItem)
        {
            this.cblComputerSkills.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.ComputerSkills, listItem, this.txtOtherComputerSkills, ChoiceValueSpliter, OtherValueChoiceFieldValue);
            this.cblComputerSkills.Enabled = this.isEditable;
            this.txtOtherComputerSkills.Enabled = this.isEditable;
        }

        private void LoadAnotherSkills(SPListItem listItem)
        {
            this.cblOtherSkills.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.OtherSkills, listItem, ChoiceValueSpliter);
            this.cblOtherSkills.Enabled = this.isEditable;
        }

        private void LoadAnothersRequirements(SPListItem listItem)
        {
            this.cblOtherRiquirement.LoadDataChoiceFieldCheckboxes(EmployeeRequirementSheetsList.Fields.OtherRequirement, listItem, ChoiceValueSpliter);
            this.cblOtherRiquirement.Enabled = this.isEditable;
        }

        /// <summary>
        /// LoadListOfTemplates
        /// </summary>
        private void LoadListOfTemplates()
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
                                                                <FieldRef Name='{2}' />
                                                                <Value Type='Boolean'>{3}</Value>
                                                            </Eq>
                                                        </And>
                                                       </Where>
                                                        <OrderBy>
	                                                        <FieldRef Name='Title' Ascending='TRUE'/>
                                                        </OrderBy>",
                                                        EmployeeRequirementSheetsList.Fields.CommonDepartmentField, this.ApprovalBaseManagerObject.CurrentEmployee.Department.LookupId,
                                                        EmployeeRequirementSheetsList.Fields.IsTemplate, "1");
                EmployeeRequirementSheetDAL employeeRequirementSheetDAL = new EmployeeRequirementSheetDAL(this.SiteUrl);
                var templates = employeeRequirementSheetDAL.GetByQuery(queryString);
                this.ddlTemplate.DataSource = templates;
                this.ddlTemplate.DataBind();

                ListItem noneListItem = new ListItem();
                noneListItem.Value = NoneTemplateValue;
                if (this.IsVietnameseLanguage)
                {
                    noneListItem.Text = "(Không có)";
                }
                else
                {
                    noneListItem.Text = "(None)";
                }
                ddlTemplate.Items.Insert(0, noneListItem);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
            }
        }

        #endregion

        #region Save Data

        private void SaveData()
        {
            // Department
            int selectDepartmentId = 0;
            int.TryParse(this.ddlDepartment.SelectedValue, out selectDepartmentId);
            if (selectDepartmentId > 0)
            {
                this.CurrentItem[EmployeeRequirementSheetsList.Fields.RecruitmentDepartment] = selectDepartmentId;
                var department = this.ApprovalBaseManagerObject.DepartmentDAL.GetByID(selectDepartmentId);
                this.CurrentItem[EmployeeRequirementSheetsList.Fields.DepartmentCode] = department.Code;
            }
            // Position
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.Position] = txtPosition.Text;
            // Quantity
            if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                int quantity = 0;
                int.TryParse(txtQuantity.Text, out quantity);
                this.CurrentItem[EmployeeRequirementSheetsList.Fields.Quantity] = quantity;
            }
            // Reason For Recruitment
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.ReasonsForRecruitment] = txtReasonForRecruitment.Text;
            // Sex
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.Sex] = GetChoiceValuesOfCheckBoxList(cblGender);
            // From Age
            if (!string.IsNullOrEmpty(txtFromAge.Text.Trim()))
            {
                int fromAge = 0;
                int.TryParse(txtFromAge.Text, out fromAge);
                this.CurrentItem[EmployeeRequirementSheetsList.Fields.FromAge] = fromAge;
            }
            // To Age
            if (!string.IsNullOrEmpty(txtToAge.Text.Trim()))
            {
                int toAge = 0;
                int.TryParse(txtToAge.Text, out toAge);
                this.CurrentItem[EmployeeRequirementSheetsList.Fields.ToAge] = toAge;
            }
            // Martial Status
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.MaritalStatus] = GetChoiceValuesOfCheckBoxList(cblMartialStatus);
            // Available Time
            if (!dtAvailableTime.IsDateEmpty && dtAvailableTime.IsValid)
            {
                this.CurrentItem[EmployeeRequirementSheetsList.Fields.AvailableTime] = dtAvailableTime.SelectedDate;
                DateTime reqDueDate = dtAvailableTime.SelectedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                this.CurrentItem[CommonSPListField.CommonReqDueDateField] = reqDueDate;
            }
            // Working Time
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.WorkingTime] = GetChoiceValuesOfCheckBoxList(cblWorkingTime);
            // Education Level
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.EducationLevel] = GetChoiceValuesOfCheckBoxList(cblEducationLevel);
            // Appearance
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.Appearance] = GetChoiceValuesOfCheckBoxList(cblAppearance);
            // Working Experience
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.WorkingExperience] = GetChoiceValuesOfCheckBoxList(cblWorkingExperience, txtOtherWorkingExperience);
            // Specialities
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.Specialities] = txtSpecialities.Text;
            // Desciption Of Basic Work
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.DescriptionOfBasicWork] = txtDescriptionOfBasicWork.Text;
            // Moral Vocations
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.MoralVocations] = txtMoralVocations.Text;
            // Working Abilities
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.WorkingAbilities] = txtWorkingAbilities.Text;
            // Computer Skills
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.ComputerSkills] = GetChoiceValuesOfCheckBoxList(cblComputerSkills, txtOtherComputerSkills);
            // Other Skills
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.OtherSkills] = GetChoiceValuesOfCheckBoxList(cblOtherSkills);
            // Other Requirements
            this.CurrentItem[EmployeeRequirementSheetsList.Fields.OtherRequirement] = GetChoiceValuesOfCheckBoxList(cblOtherRiquirement);
            // Is Valid Request
            if (!dtAvailableTime.IsDateEmpty && dtAvailableTime.IsValid)
            {
                // Available Time < Today + 15 => không đúng quy định
                int numberOfStandardizedDaysBeforeSubmission = GetNumberOfStandardizedDaysBeforeSubmission();
                DateTime standardizedAvailableTime = DateTime.Today.AddDays(numberOfStandardizedDaysBeforeSubmission);
                if (DateTime.Compare(dtAvailableTime.SelectedDate.Date, standardizedAvailableTime) < 0)
                {
                    this.CurrentItem[EmployeeRequirementSheetsList.Fields.IsValidRequest] = false;
                }
            }

            if (this.cbSavingTemplate.Checked)
            {
                string templateName = this.CurrentItem.ID.ToString();
                if (string.Compare(ddlTemplate.SelectedValue, NoneTemplateValue, true) == 0)
                {
                    templateName = txtTemplateName.Text;
                }
                else
                {
                    templateName = ddlTemplate.SelectedItem.Text;
                    // Delete Template
                    DeleteTemplate(ddlTemplate.SelectedValue);
                }

                this.CurrentItem[EmployeeRequirementSheetsList.Fields.Title] = templateName;
                this.CurrentItem[EmployeeRequirementSheetsList.Fields.IsTemplate] = true;
            }
        }

        private void SaveLanguageSkills()
        {
            try
            {
                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                List<RecruitmentLanguageSkillModel> recruitmentLanguageSkillItems = seriallizer.Deserialize<List<RecruitmentLanguageSkillModel>>(hdRecruitmentLanguageSkills.Value);
                // List<RecruitmentLanguageSkill> recruitmentLanguageSkillItems = new List<RecruitmentLanguageSkill>();
                if (recruitmentLanguageSkillItems == null)
                {
                    recruitmentLanguageSkillItems = new List<RecruitmentLanguageSkillModel>();
                }

                #region Delete List Of Detail Items
                string queryString = string.Format(@"<Where>
                                                            <Eq>
                                                                <FieldRef Name='{0}' LookupId='True' />
                                                                <Value Type='Lookup'>{1}</Value>
                                                            </Eq>
                                                       </Where>", StringConstant.RecruitmentLanguageSkillsList.Fields.Request, this.CurrentItem.ID);
                IList<int> ids = null;
                var currentItems = recruitmentLanguageSkillsDAL.GetByQuery(queryString);
                if (currentItems != null && currentItems.Count > 0)
                {
                    ids = new List<int>();

                    foreach (var item in currentItems)
                    {
                        var existedItem = recruitmentLanguageSkillItems.Where(recruitmentLanguageSkillItem => recruitmentLanguageSkillItem.ID == item.ID).FirstOrDefault();
                        // Neu khong ton tai => Xoa
                        if (existedItem == null)
                        {
                            ids.Add(item.ID);
                        }
                    }
                }

                if (ids != null && ids.Count > 0)
                {
                    recruitmentLanguageSkillsDAL.DeleteItems(ids);
                }
                #endregion

                foreach (var item in recruitmentLanguageSkillItems)
                {
                    var newItem = new RecruitmentLanguageSkill
                    {
                        ForeignLanguage = new LookupItem { LookupId = item.ForeignLanguage },
                        Level = item.Level,
                        Request = new LookupItem { LookupId = this.CurrentItem.ID }
                    };
                    recruitmentLanguageSkillsDAL.SaveItem(newItem);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// GetNumberOfStandardizedDaysBeforeSubmission
        /// </summary>
        /// <returns></returns>
        private int GetNumberOfStandardizedDaysBeforeSubmission()
        {
            // Set default value.
            int res = 15;
            string numberOfStandardizedDaysBeforeSubmission = ConfigurationDAL.GetValue(this.SiteUrl, NumberOfStandardizedDaysBeforesubmission_Key);
            if (!string.IsNullOrEmpty(numberOfStandardizedDaysBeforeSubmission))
            {
                int.TryParse(numberOfStandardizedDaysBeforeSubmission, out res);
            }

            return res;
        }

        #endregion

        private bool DeleteTemplate(int templateId)
        {
            var res = false;

            try
            {
                if (templateId > 0)
                {
                    SPListItem templateItem = this.CurrentList.GetItemById(templateId);
                    if (templateItem != null)
                    {
                        templateItem[EmployeeRequirementSheetsList.Fields.IsTemplate] = false;
                        templateItem.Update();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentFormUserControl: {ex.Message}");
                res = false;
            }

            return res;
        }

        private bool DeleteTemplate(string templateId)
        {
            int tempId = 0;
            if (int.TryParse(templateId, out tempId))
            {
                return DeleteTemplate(tempId);
            }
            else
            {
                return false;
            }
        }

        private SPListItem GetListItemForLoadForm()
        {
            SPListItem listItem = null;
            if (this.templateItem != null)
            {
                listItem = this.templateItem;
            }
            else
            {
                listItem = this.CurrentItem;
            }

            return listItem;
        }
        #endregion

        #region Utils

        /// <summary>
        /// Get selected choice values.
        /// </summary>
        /// <param name="checkBoxListControl"></param>
        /// <param name="txtOtherValueControl"></param>
        /// <returns></returns>
        private string GetChoiceValuesOfCheckBoxList(CheckBoxList checkBoxListControl, TextBox txtOtherValueControl = null)
        {
            StringBuilder choiceValuesBuilder = new StringBuilder();

            foreach (ListItem item in checkBoxListControl.Items)
            {
                if (item.Selected)
                {
                    choiceValuesBuilder.AppendFormat("{0}{1}", item.Value, ChoiceValueSpliter);
                }
            }

            if (txtOtherValueControl != null)
            {
                if (!string.IsNullOrEmpty(txtOtherValueControl.Text.Trim()))
                {
                    choiceValuesBuilder.AppendFormat("{0}{1}", txtOtherValueControl.Text.Trim(), ChoiceValueSpliter);
                }
            }

            var choiceValues = choiceValuesBuilder.ToString();
            if (!string.IsNullOrEmpty(choiceValues))
            {
                choiceValues = string.Format("{0}{1}", ChoiceValueSpliter, choiceValues);
            }

            return choiceValues;
        }

        #endregion
    }
}
