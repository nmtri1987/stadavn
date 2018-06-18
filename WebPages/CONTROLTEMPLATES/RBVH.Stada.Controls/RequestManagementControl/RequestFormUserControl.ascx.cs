using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.Report.RequestModule;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RequestManagementControl
{
    /// <summary>
    /// RequestFormUserControl
    /// </summary>
    public partial class RequestFormUserControl : ApprovalBaseUserControl
    {
        #region Constants
        /// <summary>
        /// Req#
        /// </summary>
        private const string PrefixRequest = "Req {0}";

        /// <summary>
        /// 1
        /// </summary>
        private const string IsShowValue_Yes = "1";

        /// <summary>
        /// 0
        /// </summary>
        private const string IsShowValue_No = "0";
        #endregion

        #region Fields
        private bool isEditable;
        private RequestTypesDAL requestTypeDAL;
        #endregion

        #region Events
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //requestTypeDAL = new RequestTypesDAL(this.SiteUrl);

                //this.OnAfterSaveAsDraft += RequestFormUserControl_OnAfterSaveAsDraft;
                //this.OnAfterSubmitted += RequestFormUserControl_OnAfterSubmitted;
                this.OnAfterApproved += RequestFormUserControl_OnAfterApproved;

                if (!Page.IsPostBack)
                {
                    LoadData();

                    hdDisplayFormUrl.Value = string.Format("{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/RequestManagement/RequestForm.aspx?mode=display&List={1}&ID=", this.CurrentWeb.Url, this.CurrentList.ID);
                }

                // Check Hide Or Show ReceivedBy
                CheckHideOrShowShowReceivedBy();

                // Check to show Complete button.
                //CheckToShowCompleteButton();
                this.FormButtonsControlObject.CompleteButton.Click += CompleteButton_Click;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            try
            {
                // Check to show Complete button.
                CheckToShowCompleteButton();
                #region DucVT-A-2018.01.16. TFS#1898
                // Check to show Print button.
                CheckToShowPrintButton();
                #endregion
                base.Render(output);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
            }
        }

        private void CompleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Update Status
                this.CurrentItem[ApprovalFields.Status] = Status.Completed;
                // Update WFStatus
                this.CurrentItem[ApprovalFields.WFStatus] = ApprovalStatus.Completed;
                // Update Request Item
                this.CurrentItem.Update();

                var receivedBy = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.ReceviedByField]);
                if (receivedBy != null)
                {
                    SPFieldLookupValue locationLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CommonLocation]);
                    if (locationLookupValue != null)
                    {
                        int headOfDepartmentPositionId = this.ApprovalBaseManagerObject.GetDEHPositionId();
                        List<EmployeeInfo> toEmployees = this.ApprovalBaseManagerObject.GetListOfEmployees(locationLookupValue.LookupId, receivedBy.LookupId, headOfDepartmentPositionId);

                        if (toEmployees != null && toEmployees.Count > 0)
                        {
                            // Add Department Head into cc.
                            this.ApprovalBaseManagerObject.ListOfEmployeesEmailTo.AddRange(toEmployees);
                            // Send Email
                            this.ApprovalBaseManagerObject.SendEmail(EWorkflowAction.Complete, this.CommentBoxControlObject.ContentComment);
                        }
                    }
                }

                // Post Comment
                this.ApprovalBaseManagerObject.PostComment(Status.Completed, this.CommentBoxControlObject.ContentComment);
                // Close form
                this.CloseForm(sender);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// Trigger event after BOD approve with Buy request type, send email to Department Head of perform.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestFormUserControl_OnAfterApproved(object sender, EventArgs e)
        {
            try
            {
                SPFieldLookupValue requestTypeRefLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.RequestTypeRefField]);
                int requestTypeRefId = requestTypeRefLookupValue.LookupId;
                string requestTypeName = this.GetRequestTypeName(requestTypeRefId);
                if (this.IsEmployeeBOD() && (string.Compare(requestTypeName, RequestTypeName.RequestBuyDetails, true) == 0))
                {
                    RequestTemplate requestBuyTemplate = new RequestBuyTemplate(this.CurrentWeb, this.CurrentItem);
                    string urlOfFileFormData = requestBuyTemplate.ExportFormData();

                    if (!string.IsNullOrEmpty(urlOfFileFormData))
                    {
                        // this.ApprovalBaseManagerObject.OnBeforeBuildBodyEmail += ApprovalBaseManagerObject_OnBeforeBuildBodyEmail;
                        string linkPrintEN = string.Format("<p>You can click on this <a href=\"{0}\">link<a/> to print form request.<p>", urlOfFileFormData);
                        string linkPrintVN = string.Format("<p>Vui lòng truy vập vào <a href=\"{0}\">liên kết<a/> để in phiếu đề nghị.</p>", urlOfFileFormData);
                        this.ApprovalBaseManagerObject.AdditionalInfoEmailObject[RequestApprovalManagement.PrintLinkEN_Key] = linkPrintEN;
                        this.ApprovalBaseManagerObject.AdditionalInfoEmailObject[RequestApprovalManagement.PrintLinkVN_Key] = linkPrintVN;
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
            }
        }

        #endregion

        #region Overrides

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                isEditable = this.IsEditable();
                this.hdIsEditable.Value = this.isEditable.ToString();
                requestTypeDAL = new RequestTypesDAL(this.SiteUrl);

                InitAllowPostCommentAttribute();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
            }
        }

        /// <summary>
        /// SaveForm
        /// </summary>
        /// <returns></returns>
        protected override bool SaveForm()
        {
            bool res = false;

            try
            {
                if (this.isEditable)
                {
                    // Title
                    this.CurrentItem[RequestsList.TitleField] = txtTitle.Text;
                    // RequestType
                    this.SaveRequestType();
                    // ReceivedBy
                    this.SaveReceivedBy();
                    // FinishDate
                    SaveFinishDate();
                    // ReferTo
                    SaveReferTo();
                    // If requester is Head Of Department
                    if (this.IsEmployeeDEH())
                    {
                        // If request type is Repair or Others
                        string requestTypeName = this.GetRequestTypeNameForCurrentitem();
                        if ((string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0) ||
                            (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0))
                        {
                            // Set -> Required Approval By BOD
                            this.CurrentItem[RequestsList.RequiredApprovalByBODField] = this.chkboxRequireBODdApprove.Checked;
                        }
                    }
                    else //reset Required Approval by BOD in case re-submit
                    {
                        this.CurrentItem[RequestsList.RequiredApprovalByBODField] = null;
                    }

                    res = base.SaveForm();

                    if (res)
                    {
                        res = SaveRequestDetails();
                    }
                }
                else    // Approve - Save Form
                {
                    string requestTypeName = this.GetRequestTypeNameForCurrentitem();
                    if ((string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0) ||
                        (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0))
                    {
                        // Required Approval By BOD
                        SaveRequiredApprovalByBOD();

                        // Due Date
                        SaveDueDate();

                        //TFS: #1755
                        if (this.ddlReceivedBy.Enabled)
                        {
                            //Save RecivedBy
                            SaveReceivedBy();
                        }
                    }
                    else
                    {
                        // Only BOD who has permission to update Received By (Department).
                        if (this.IsEmployeeBOD())
                        {
                            if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                            {
                                int receviedByDepartment = 0;
                                int.TryParse(ddlReceivedBy.SelectedValue, out receviedByDepartment);
                                if (receviedByDepartment > 0)
                                {
                                    this.CurrentItem[StringConstant.RequestsList.ReceviedByField] = receviedByDepartment;
                                    this.CurrentItem.Update();
                                }
                                else
                                {
                                    throw new Exception("Please select received by.");
                                }
                            }
                        }
                    }

                    res = true;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
                res = false;
            }

            //res = true;

            return res;
        }

        public override void Validate()
        {
            try
            {
                base.Validate();

                string title = txtTitle.Text;
                if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title))
                {
                    IsValid = false;
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseInputRequestTitle", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    return;
                }

                int requestTypeRefId;
                int.TryParse(hdRequestType.Value, out requestTypeRefId);
                if (requestTypeRefId <= 0)
                {
                    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseSelectRequestType", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    IsValid = false;
                    return;
                }
                else
                {
                    JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                    string requestTypeName = this.GetRequestTypeName(requestTypeRefId);
                    if (string.Compare(requestTypeName, RequestTypeName.RequestBuyDetails, true) == 0)
                    {
                        List<RequestBuyDetails> requestBuyDetailsItems = seriallizer.Deserialize<List<RequestBuyDetails>>(hdDetailsBuyData.Value);
                        if ((requestBuyDetailsItems == null) || (requestBuyDetailsItems != null && requestBuyDetailsItems.Count < 0))
                        {
                            hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseAddRequestBuyingDetails", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                            IsValid = false;
                            return;
                        }
                    }
                    else if (string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0)
                    {
                        // Received By
                        int receivedByDepartmentId = 0;
                        int.TryParse(ddlReceivedBy.SelectedValue, out receivedByDepartmentId);
                        if (receivedByDepartmentId <= 0)
                        {
                            // If the dropdown is disabled, get the selected value via hidden field
                            if (hdSelectedReceivedBy.Value == "0")
                            {
                                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseSelectReceivedBy", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                                IsValid = false;
                                return;
                            }
                        }

                        // Details
                        List<RequestRepairDetails> requestRepairDetailsItems = seriallizer.Deserialize<List<RequestRepairDetails>>(hdDetailsRepair.Value);
                        if ((requestRepairDetailsItems == null) || (requestRepairDetailsItems != null && requestRepairDetailsItems.Count < 0))
                        {
                            hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseAddRequestRepairDetails", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                            IsValid = false;
                            return;
                        }

                        // Finish Date
                        if (dtFinishDate.IsDateEmpty)
                        {
                            hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseSelectFinishDate", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                            IsValid = false;
                            return;
                        }

                    }
                    else if (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0)
                    {
                        // Received By
                        int receivedByDepartmentId = 0;
                        int.TryParse(ddlReceivedBy.SelectedValue, out receivedByDepartmentId);
                        if (receivedByDepartmentId <= 0)
                        {
                            // If the dropdown is disabled, get the selected value via hidden field
                            if (hdSelectedReceivedBy.Value == "0")
                            {
                                hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseSelectReceivedBy", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                                IsValid = false;
                                return;
                            }
                        }

                        // Details
                        List<RequestOtherDetails> requestOtherDetailsItems = seriallizer.Deserialize<List<RequestOtherDetails>>(hdDetailsOthers.Value);
                        if ((requestOtherDetailsItems == null) || (requestOtherDetailsItems != null && requestOtherDetailsItems.Count < 0))
                        {
                            hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseAddRequestOtherDetails", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                            IsValid = false;
                            return;
                        }

                        // Finish Date
                        if (dtFinishDate.IsDateEmpty)
                        {
                            hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseSelectFinishDate", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                            IsValid = false;
                            return;
                        }
                    }

                    //if (dtDueDate.IsDateEmpty)
                    //{
                    //    hdErrorMessage.Value = ResourceHelper.GetLocalizedString("PleaseSelectDueDate", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
                    //    IsValid = false;
                    //    return;
                    //}
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
                this.ShowClientMessage(ex.Message);
            }
        }

        #endregion

        #region Methods

        private void InitAllowPostCommentAttribute()
        {
            // 2012 https://rbvhsharepointprojects.visualstudio.com/STADA/_workitems/edit/2012
            // Chỉ cho BOD comment ở bước BOD duyệt
            //if (IsCompletedStatus())
            //{
            //    this.allowPostComment = true;
            //}
        }

        #region Load Data
        private void LoadData()
        {
            // Load Title
            if (this.CurrentItem[RequestsList.TitleField] != null)
            {
                this.txtTitle.Text = this.CurrentItem[RequestsList.TitleField].ToString();
            }
            this.txtTitle.Enabled = this.isEditable;

            // Load requester info
            LoadRqueterInfo();

            // Load Request Type
            LoadRequetTypes();

            // Load Department
            LoadReceiviedBy();

            // Bind data list of request details item into hidden field
            LoadRequestDetails();

            // Load Finish Date
            LoadFinishDate();

            // Load Required Approval By BOD
            LoadRequiredApprovalByBOD();

            // Load Refer To info
            LoadListOfReferTos();

            // Load Due Date info
            LoadDueDate();

            // Load Approval Status
            hdApprovalStatus.Value = GetApprovalStatus();
        }

        /// <summary>
        /// LoadRqueterInfo
        /// </summary>
        private void LoadRqueterInfo()
        {
            if (ApprovalBaseManagerObject.Creator != null)
            {
                this.lblRequester.Text = this.ApprovalBaseManagerObject.Creator.FullName;
                this.lblEmployeeId.Text = this.ApprovalBaseManagerObject.Creator.EmployeeID;

                var department = DepartmentListSingleton.GetDepartmentByID(ApprovalBaseManagerObject.Creator.Department.LookupId, this.SiteUrl);
                if (department != null)
                {
                    this.lblDepartment.Text = (CultureInfo.CurrentUICulture.LCID == 1066) ? department.VietnameseName : department.Name;
                }
            }
        }

        /// <summary>
        /// Load list of request types.
        /// </summary>
        private void LoadRequetTypes()
        {
            rblRequetTypes.DataValueField = "ID";
            rblRequetTypes.DataTextField = StringConstant.RequestTypesList.RequestTypeNameField;
            var requestTypeItems = requestTypeDAL.GetAll();
            rblRequetTypes.DataSource = requestTypeItems;
            rblRequetTypes.DataBind();

            if (this.CurrentItem[RequestsList.RequestTypeRefField] != null)
            {
                SPFieldLookupValue requestTypeLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.RequestTypeRefField]);
                rblRequetTypes.SelectedValue = requestTypeLookupValue.LookupId.ToString();
            }
            else
            {
                if (rblRequetTypes.Items.Count > 0)
                {
                    rblRequetTypes.Items[0].Selected = true;
                }
            }

            //if (rblRequetTypes.SelectedItem != null)
            //{
            //    hdRequestType.Value = rblRequetTypes.SelectedValue;
            //}
            hdRequestType.Value = this.rblRequetTypes.SelectedValue;

            this.rblRequetTypes.Enabled = this.isEditable;
        }

        /// <summary>
        /// LoadReceiviedBy
        /// </summary>
        private void LoadReceiviedBy()
        {
            //if (this.IsVietnameseLanguage)
            //{
            //    this.ddlReceivedBy.DataTextField = "VietnameseName";
            //}
            //this.ddlReceivedBy.DataSource = listOfDepartments;
            //this.ddlReceivedBy.DataBind();
            var locationId = 2;
            if (this.ApprovalBaseManagerObject.Creator != null)
            {
                locationId = this.ApprovalBaseManagerObject.Creator.FactoryLocation.LookupId;
            }

            var departments = ApprovalBaseManagerObject.DepartmentDAL.GetDepartmentsByLocation(new List<int> { locationId });
            List<RequestType> requestTypes = requestTypeDAL.GetAll();
            var items = BuildListOfReceivedByDepartment(requestTypes, departments);
            this.ddlReceivedBy.Items.AddRange(items.ToArray());

            SPFieldLookupValue receivedByLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.ReceviedByField]);
            if (receivedByLookupValue != null)
            {
                // this.ddlReceivedBy.SelectedValue = receivedByLookupValue.LookupId.ToString();
                this.hdSelectedReceivedBy.Value = receivedByLookupValue.LookupId.ToString();
            }
            // TFS: #1892
            string requestTypeName = GetRequestTypeNameForCurrentitem();
            // 1984: https://rbvhsharepointprojects.visualstudio.com/STADA/_workitems/edit/1984
            //if (this.IsEmployeeBOD() && string.Compare(requestTypeName, RequestTypeName.RequestBuyDetails, true) == 0)
            //{
            AppendNoneOptionToReceivedByDropdown(requestTypes);
            //}
        }
        /// <summary>
        /// Append None option to received by dropdown
        /// </summary>
        private void AppendNoneOptionToReceivedByDropdown(List<RequestType> requestTypes)
        {
            // TFS: #1892
            string resourceText = ResourceHelper.GetLocalizedString("Dropdown_Select", StringConstant.ResourcesFileWebPages, CultureInfo.CurrentUICulture.LCID);
            var noneOptionListItem = new ListItem() { Value = "0", Text = resourceText };
            var optionString = string.Join(";", requestTypes.Select(x => x.ID).ToArray());
            noneOptionListItem.Attributes.Add("request-type", optionString);
            this.ddlReceivedBy.Items.Insert(0, noneOptionListItem);
        }
        /// <summary>
        /// BuildListOfReceivedByDepartment
        /// </summary>
        /// <param name="requestTypes"></param>
        /// <param name="departments"></param>
        /// <returns></returns>
        private List<ListItem> BuildListOfReceivedByDepartment(List<RequestType> requestTypes, List<Department> departments)
        {
            var items = new List<ListItem>();

            foreach (var department in departments)
            {
                ListItem listItem = BuildListItem(requestTypes, department);
                items.Add(listItem);
            }

            return items;
        }

        /// <summary>
        /// BuildListitem
        /// </summary>
        /// <param name="requestTypes"></param>
        /// <param name="department"></param>
        /// <returns></returns>
        private ListItem BuildListItem(List<RequestType> requestTypes, Department department)
        {
            ListItem listItem = new ListItem();

            listItem.Value = department.ID.ToString();
            if (this.IsVietnameseLanguage)
            {
                listItem.Text = department.VietnameseName;
            }
            else
            {
                listItem.Text = department.Name;
            }

            StringBuilder requestTypeAttribute = new StringBuilder();
            foreach (var requestType in requestTypes)
            {
                var requestTypeDepartment = requestType.Departments.Where(item => item.LookupId == department.ID).FirstOrDefault();
                if (requestTypeDepartment != null)
                {
                    requestTypeAttribute.AppendFormat("{0};", requestType.ID);
                }
            }

            listItem.Attributes.Add("request-type", requestTypeAttribute.ToString());

            return listItem;
        }

        /// <summary>
        /// Bind list of request detail item into hidden field.
        /// </summary>
        private void LoadRequestDetails()
        {
            string requestTypeName = this.GetRequestTypeNameForCurrentitem();

            // Mua hang
            if (string.Compare(requestTypeName, RequestTypeName.RequestBuyDetails, true) == 0)
            {
                #region Query
                string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", StringConstant.RequestBuyDetailsList.Fields.Request, this.CurrentItem.ID);
                #endregion

                RequestBuyDetailsDAL requestBuyDetailsDAL = new RequestBuyDetailsDAL(this.SiteUrl);
                //List<RequestBuyDetails> requestBuyDetailsItems = requestBuyDetailsDAL.GetByQuery(queryString, 
                //    StringConstant.RequestBuyDetailsList.Fields.Form,
                //    StringConstant.RequestBuyDetailsList.Fields.Unit,
                //    StringConstant.RequestBuyDetailsList.Fields.Quantity,
                //    StringConstant.RequestBuyDetailsList.Fields.Reason);
                List<RequestBuyDetails> requestBuyDetailsItems = requestBuyDetailsDAL.GetByQuery(queryString);
                if (requestBuyDetailsItems != null && requestBuyDetailsItems.Count > 0)
                {
                    JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                    hdDetailsBuyData.Value = seriallizer.Serialize(requestBuyDetailsItems);
                }
            }
            else if (string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0)
            {
                #region Query
                string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", StringConstant.RequestRepairDetailsList.Fields.Request, this.CurrentItem.ID);
                #endregion

                RequestRepairDetailsDAL requestRepairDetailsDAL = new RequestRepairDetailsDAL(this.SiteUrl);
                //List<RequestRepairDetails> requestRepairDetailsItems = requestRepairDetailsDAL.GetByQuery(queryString,
                //    StringConstant.RequestRepairDetailsList.Fields.Content,
                //    StringConstant.RequestRepairDetailsList.Fields.Reason,
                //    StringConstant.RequestRepairDetailsList.Fields.Place,
                //    StringConstant.RequestRepairDetailsList.Fields.From,
                //    StringConstant.RequestRepairDetailsList.Fields.To);
                List<RequestRepairDetails> requestRepairDetailsItems = requestRepairDetailsDAL.GetByQuery(queryString);
                if (requestRepairDetailsItems != null && requestRepairDetailsItems.Count > 0)
                {
                    foreach (var item in requestRepairDetailsItems)
                    {
                        //item.From = DateTime.Parse(item.From.ToString(DateFormatddMMyyyy2));
                        //item.To = DateTime.Parse(item.To.ToString(DateFormatddMMyyyy2));
                        item.FromDateString = item.From.HasValue ? item.From.Value.ToString(DateFormatddMMyyyy2) : string.Empty;
                        item.ToDateString = item.To.HasValue ? item.To.Value.ToString(DateFormatddMMyyyy2) : string.Empty;
                    }
                    JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                    hdDetailsRepair.Value = seriallizer.Serialize(requestRepairDetailsItems);
                }
            }
            else if (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0)
            {
                #region Query
                string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", StringConstant.RequestOtherDetailsList.Fields.Request, this.CurrentItem.ID);
                #endregion

                RequestOtherDetailsDAL requestOtherDetailsDAL = new RequestOtherDetailsDAL(this.SiteUrl);
                //List<RequestOtherDetails> requestOtherDetailsItems = requestOtherDetailsDAL.GetByQuery(queryString,
                //    StringConstant.RequestOtherDetailsList.Fields.Content,
                //    StringConstant.RequestOtherDetailsList.Fields.Unit,
                //    StringConstant.RequestOtherDetailsList.Fields.Quantity,
                //    StringConstant.RequestOtherDetailsList.Fields.Reason);
                List<RequestOtherDetails> requestOtherDetailsItems = requestOtherDetailsDAL.GetByQuery(queryString);
                if (requestOtherDetailsItems != null && requestOtherDetailsItems.Count > 0)
                {
                    JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                    hdDetailsOthers.Value = seriallizer.Serialize(requestOtherDetailsItems);
                }
            }
        }

        /// <summary>
        /// LoadFinishDate
        /// </summary>
        private void LoadFinishDate()
        {
            if (this.CurrentItem[RequestsList.FinishDateField] != null)
            {
                DateTime finishDate;
                DateTime.TryParse(this.CurrentItem[RequestsList.FinishDateField].ToString(), out finishDate);
                this.dtFinishDate.SelectedDate = finishDate;

                this.hdIsShowFinishDate.Value = IsShowValue_Yes;
            }

            this.dtFinishDate.Enabled = this.isEditable;
        }

        /// <summary>
        /// LoadRequiredApprovalByBOD
        /// </summary>
        private void LoadRequiredApprovalByBOD()
        {
            string requestTypeName = GetRequestTypeNameForCurrentitem();

            if ((string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0) ||
                (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0))
            {
                // Da qua buoc Department Head
                if (this.CurrentItem[RequestsList.RequiredApprovalByBODField] != null)
                {
                    if (!this.IsRejectedStatus())
                    {
                        trRequireBODdApprove.Visible = true;
                        hdIsShowRequiredApprovalByBOD.Value = IsShowValue_Yes;
                        this.chkboxRequireBODdApprove.Checked = bool.Parse(this.CurrentItem[RequestsList.RequiredApprovalByBODField].ToString());
                        this.chkboxRequireBODdApprove.Enabled = false;
                    }
                }
                else    // Chua qua buoc Department Head
                {
                    if (this.IsSubmittedStatus() && !this.IsCreator())
                    {
                        trRequireBODdApprove.Visible = true;
                        hdIsShowRequiredApprovalByBOD.Value = IsShowValue_Yes;
                        if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                        {
                            this.chkboxRequireBODdApprove.Enabled = true;
                        }
                        else
                        {
                            this.chkboxRequireBODdApprove.Enabled = false;
                        }
                    }
                }

                if (this.IsRejectedStatus() && this.IsCreator() && this.ApprovalBaseManagerObject.IsEmployeeDEH()) //Trưởng phòng resubmit
                {
                    this.hdIsEmployeeDEH.Value = IsShowValue_Yes;
                    this.trRequireBODdApprove.Visible = true;
                    this.trRequireBODdApprove.Attributes.Add("style", "display: none;");
                    this.chkboxRequireBODdApprove.Enabled = true;
                    this.chkboxRequireBODdApprove.Checked = this.CurrentItem[RequestsList.RequiredApprovalByBODField] != null ? bool.Parse(this.CurrentItem[RequestsList.RequiredApprovalByBODField].ToString()) : false;
                    this.hdIsShowRequiredApprovalByBOD.Value = IsShowValue_Yes;
                }
            }
            else    // Buying Request Type
            {
                //trRequireBODdApprove.Visible = false;
                //hdIsShowRequiredApprovalByBOD.Value = IsShowValue_No;
                // Nếu người tạo là Trưởng Phòng thì hiển thị -> "Cần duyệt bởi ban giám đốc"
                if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.New)
                {
                    if (this.IsEmployeeDEH())
                    {
                        this.hdIsEmployeeDEH.Value = IsShowValue_Yes;
                        this.trRequireBODdApprove.Visible = true;
                        this.trRequireBODdApprove.Attributes.Add("style", "display: none;");
                        this.chkboxRequireBODdApprove.Enabled = true;
                        this.hdIsShowRequiredApprovalByBOD.Value = IsShowValue_No;
                    }
                }
                else if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                {
                    if (this.IsEmployeeDEH())
                    {
                        this.hdIsEmployeeDEH.Value = IsShowValue_Yes;
                        this.trRequireBODdApprove.Visible = true;
                        this.trRequireBODdApprove.Attributes.Add("style", "display: none;");
                        this.chkboxRequireBODdApprove.Enabled = true;
                        this.chkboxRequireBODdApprove.Checked = this.CurrentItem[RequestsList.RequiredApprovalByBODField] != null ? bool.Parse(this.CurrentItem[RequestsList.RequiredApprovalByBODField].ToString()) : false;
                        this.hdIsShowRequiredApprovalByBOD.Value = IsShowValue_No;
                    }
                }
            }
        }

        /// <summary>
        /// Load list of refer requests.
        /// </summary>
        private void LoadListOfReferTos()
        {
            // Mode: New or Edit
            if (this.isEditable)
            {
                ddlReferTo.Visible = true;

                ddlReferTo.DataValueField = "ID";
                ddlReferTo.DataTextField = "Title";

                RequestsDAL requestDAL = new RequestsDAL(this.SiteUrl);

                #region Query
                int receivedByDepartmentId = 0;
                if (this.ApprovalBaseManagerObject.CurrentEmployee != null)
                {
                    if (this.ApprovalBaseManagerObject.CurrentEmployee.Department != null)
                    {
                        receivedByDepartmentId = this.ApprovalBaseManagerObject.CurrentEmployee.Department.LookupId;
                    }
                }
                string queryString = string.Format(@"<Where>
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
                                                    </Where>", ApprovalFields.WFStatus, StringConstant.ApprovalStatus.InProcess,
                                                                RequestsList.ReceviedByField, receivedByDepartmentId);
                #endregion

                var requestItems = requestDAL.GetByQuery(queryString);
                if (requestItems == null)
                {
                    requestItems = new List<Biz.Models.Request>();
                }
                requestItems.Insert(0, new Biz.Models.Request { ID = 0, Title = "" });

                ddlReferTo.DataSource = requestItems;
                ddlReferTo.DataBind();

                SPFieldLookupValue referToLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.ReferRequestField]);
                if (referToLookupValue != null)
                {
                    ddlReferTo.SelectedValue = referToLookupValue.LookupId.ToString();
                }

                linkReferTo.Visible = true;
            }
            else
            {
                SPFieldLookupValue referToLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.ReferRequestField]);
                if (referToLookupValue != null)
                {
                    linkReferTo.Visible = true;
                    linkReferTo.InnerText = referToLookupValue.LookupValue;
                    linkReferTo.HRef = "javascript:void(0);";
                    // Bind javascript to open dialog.
                    string url = string.Format("{0}/_layouts/15/RBVH.Stada.Intranet.WebPages/RequestManagement/RequestForm.aspx?mode=display&List={1}&ID={2}", this.CurrentWeb.Url, this.CurrentList.ID, referToLookupValue.LookupId);
                    string linkReferToOnClientClick = string.Format("return openModalDialog('{0}', '{1}', {2}, SP.UI.DialogResult.cancel);", referToLookupValue.LookupValue, url, "null");
                    linkReferTo.Attributes.Add("onclick", linkReferToOnClientClick);
                }
            }
        }

        /// <summary>
        /// LoadDueDate
        /// </summary>
        private void LoadDueDate()
        {
            //if (this.CurrentItem[RequestsList.CommonDueDateField] != null)
            //{
            //    DateTime dueDate;
            //    DateTime.TryParse(this.CurrentItem[RequestsList.CommonDueDateField].ToString(), out dueDate);
            //    this.dtDueDate.SelectedDate = dueDate;
            //}

            //this.dtDueDate.Enabled = this.isEditable;

            string requestTypeName = GetRequestTypeNameForCurrentitem();

            if ((string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0) ||
                (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0))
            {
                // Da qua buoc Truong Phong Thuc Hien aprpoved
                if (this.CurrentItem[RequestsList.CommonDueDateField] != null)
                {
                    if (!this.IsRejectedStatus())
                    {
                        trDueDate.Visible = true;
                        hdIsShowDueDate.Value = IsShowValue_Yes;
                        DateTime dueDate;
                        DateTime.TryParse(this.CurrentItem[RequestsList.CommonDueDateField].ToString(), out dueDate);
                        this.dtDueDate.SelectedDate = dueDate;
                        this.dtDueDate.Enabled = false;
                    }
                }
                else    // Chua qua buoc Truong Phong Thuc Hien aprpoved
                {
                    bool isAdditionalStep = false;
                    if (this.CurrentItem[ApprovalFields.IsAdditionalStep] != null)
                    {
                        bool.TryParse(this.CurrentItem[ApprovalFields.IsAdditionalStep].ToString(), out isAdditionalStep);
                    }

                    if (isAdditionalStep)
                    {
                        trDueDate.Visible = true;
                        hdIsShowDueDate.Value = IsShowValue_Yes;
                        if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                        {
                            this.dtDueDate.Enabled = true;
                        }
                        else
                        {
                            this.dtDueDate.Enabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// CheckHideOrShowShowReceivedBy
        /// </summary>
        private void CheckHideOrShowShowReceivedBy()
        {
            int requestTypeRefId = 0;
            int.TryParse(this.rblRequetTypes.SelectedValue, out requestTypeRefId);
            string requestTypeName = this.GetRequestTypeName(requestTypeRefId);
            // Request Type: Repair or Other
            if ((string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0) ||
                (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0))
            {
                this.hdIsShowReceivedBy.Value = IsShowValue_Yes;
                this.ddlReceivedBy.Enabled = isEditable;

                //TFS: #1755
                if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                {
                    var additionalStep = this.CurrentItem[RequestsList.AdditionalStepField];
                    if (additionalStep == null)
                    {
                        if (this.IsEmployeeDEH())
                        {
                            this.ddlReceivedBy.Enabled = true;
                        }
                        // Fix bug TFS #203 for Case: Group Leader (Pho Phong) is delegated.
                        //else if (this.IsEmployeeGRL() && this.ApprovalBaseManagerObject.IsEmployeeDelegated())
                        else if (this.ApprovalBaseManagerObject.IsEmployeeDelegated())
                        {
                            this.ddlReceivedBy.Enabled = true;
                        }
                    }
                }
            }
            else if ((string.Compare(requestTypeName, RequestTypeName.RequestBuyDetails, true) == 0))   // Request Type: Buy
            {
                SPFieldLookupValue receivedByLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.ReceviedByField]);

                // If ReceivedBy is selected by BOD.
                if (receivedByLookupValue != null)
                {
                    // Neu nguoi tao va trang thai la Reject thi ko show ReceivedBy
                    if (this.IsCreator() && this.IsRejectedStatus())
                    {
                        hdIsShowReceivedBy.Value = IsShowValue_No;
                    }
                    else
                    {
                        hdIsShowReceivedBy.Value = IsShowValue_Yes;
                    }

                    if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                    {
                        #region MOD. 2017.10.04. TFS #1596.
                        //ddlReceivedBy.Enabled = true;
                        if ((this.IsCreator() && this.IsRejectedStatus()) || this.IsEmployeeBOD())
                        {
                            ddlReceivedBy.Enabled = true;
                        }
                        else
                        {
                            ddlReceivedBy.Enabled = false;
                        }
                        #endregion
                    }
                    else
                    {
                        ddlReceivedBy.Enabled = false;
                    }
                }
                else    // Not selected yet.
                {
                    if (this.IsEmployeeBOD())
                    {
                        hdIsShowReceivedBy.Value = IsShowValue_Yes;

                        if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                        {
                            ddlReceivedBy.Enabled = true;
                        }
                        else if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Display)
                        {
                            ddlReceivedBy.Enabled = false;
                        }
                    }
                    else
                    {
                        hdIsShowReceivedBy.Value = IsShowValue_No;
                    }
                }
            }
            else if (string.IsNullOrEmpty(requestTypeName)) // Empty (New: -> have not request type yet. Only load data and no showing trReceivedBy tag.
            {

            }
            //TFS: 1962
            // TFS: #1984: [31.01.2018][Phiếu đề nghị] Hiển thị "None" khi tạo đơn
            //if(this.ddlReceivedBy.Enabled == true && this.ddlReceivedBy.Items.FindByValue("0") != null)
            //{
            //    this.hdSelectedReceivedBy.Value = "0";
            //}
        }

        private void CheckToShowCompleteButton()
        {
            if (this.IsCreator() && this.IsApprovedStatus())
            {
                this.FormButtonsControlObject.TdCompleteWorkflow.Visible = true;
                this.CommentBoxControlObject.Visible = true;
                this.CommentBoxControlObject.TextboxControl.Enabled = true;
                //this.FormButtonsControlObject.CompleteButton.Click += CompleteButton_Click;
            }
        }

        /// <summary>
        /// CheckToShowPrintButton
        /// </summary>
        private void CheckToShowPrintButton()
        {
            try
            {
                var visiblePrintButton = false;
                if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                {
                    SPFieldLookupValue requestTypeRefLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.RequestTypeRefField]);
                    int requestTypeRefId = requestTypeRefLookupValue.LookupId;
                    string requestTypeName = this.GetRequestTypeName(requestTypeRefId);
                    if ((string.Compare(requestTypeName, RequestTypeName.RequestBuyDetails, true)) == 0)
                    {
                        bool isAdditionalStep = ObjectHelper.GetBoolean(this.CurrentItem[RequestsList.IsAdditionalStepField]);
                        if (isAdditionalStep)
                        {
                            if (this.CurrentItem[RequestsList.AdditionalStepField] != null)
                            {
                                if (this.CurrentItem[RequestsList.AdditionalDepartmentField] != null)
                                {
                                    bool isEmployeeProcessing = this.ApprovalBaseManagerObject.IsEmployeeProcessing();
                                    if (isEmployeeProcessing)
                                    {
                                        visiblePrintButton = true;
                                    }
                                    else if (this.ApprovalBaseManagerObject.IsEmployeeDelegated())
                                    {
                                        visiblePrintButton = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (visiblePrintButton)
                {
                    this.FormButtonsControlObject.TdPrint.Visible = true;
                    RequestTemplate requestBuyTemplate = new RequestBuyTemplate(this.CurrentWeb, this.CurrentItem);
                    string urlOfFileFormData = requestBuyTemplate.BuildUrlOfFile();
                    string onPrintClientClick = string.Format("window.open('{0}'); return false;", urlOfFileFormData);
                    this.FormButtonsControlObject.PrintButton.OnClientClick = onPrintClientClick;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
            }
        }

        #endregion

        #region Save Data

        /// <summary>
        /// SaveRequestType
        /// </summary>
        private void SaveRequestType()
        {
            int requestTypeRefId = 0;
            int.TryParse(hdRequestType.Value, out requestTypeRefId);
            if (requestTypeRefId > 0)
            {
                this.CurrentItem[StringConstant.RequestsList.RequestTypeRefField] = requestTypeRefId;
            }
            else
            {
                this.CurrentItem[StringConstant.RequestsList.RequestTypeRefField] = null;
            }
            this.CurrentItem[StringConstant.RequestsList.RequestTypeRefIdField] = requestTypeRefId;
        }

        /// <summary>
        /// SaveReceivedBy
        /// </summary>
        private void SaveReceivedBy()
        {
            string requestTypeName = this.GetRequestTypeNameForCurrentitem();
            // Request Type: Repair or Other
            if ((string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0) ||
                (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0))
            {
                int receivedByDepartmentId = 0;
                int.TryParse(ddlReceivedBy.SelectedValue, out receivedByDepartmentId);
                if (receivedByDepartmentId > 0)
                {
                    this.CurrentItem[StringConstant.RequestsList.ReceviedByField] = receivedByDepartmentId;
                }
            }
            else // Request Type: Buy
            {
                // Clear ReceivedBy
                this.CurrentItem[StringConstant.RequestsList.ReceviedByField] = null;
            }
        }

        /// <summary>
        /// SaveFinishDate
        /// </summary>
        private void SaveFinishDate()
        {
            string requestTypeName = this.GetRequestTypeNameForCurrentitem();
            if ((string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0) ||
                (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0))
            {
                this.CurrentItem[StringConstant.RequestsList.FinishDateField] = this.dtFinishDate.SelectedDate;

                DateTime reqDueDate = this.dtFinishDate.SelectedDate.Date;
                //if (reqDueDate == DateTime.Now.Date)
                //{
                //    reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                //}
                //else
                //{
                //    reqDueDate = reqDueDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                //}
                reqDueDate = reqDueDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                this.CurrentItem[StringConstant.CommonSPListField.CommonReqDueDateField] = reqDueDate;
            }
        }

        /// <summary>
        /// SaveRequiredApprovalByBOD
        /// </summary>
        private void SaveRequiredApprovalByBOD()
        {
            if (this.IsSubmittedStatus() && !this.IsCreator())
            {
                if (this.CurrentFormMode == Microsoft.SharePoint.WebControls.SPControlMode.Edit)
                {
                    this.CurrentItem[RequestsList.RequiredApprovalByBODField] = this.chkboxRequireBODdApprove.Checked;
                    this.CurrentItem.Update();
                }
            }
        }

        /// <summary>
        /// SaveDueDate
        /// </summary>
        private void SaveDueDate()
        {
            bool isAdditionalStep = false;
            if (this.CurrentItem[ApprovalFields.IsAdditionalStep] != null)
            {
                bool.TryParse(this.CurrentItem[ApprovalFields.IsAdditionalStep].ToString(), out isAdditionalStep);
            }
            if (isAdditionalStep)
            {
                if (!this.dtDueDate.IsDateEmpty)
                {
                    this.CurrentItem[StringConstant.RequestsList.CommonDueDateField] = this.dtDueDate.SelectedDate;
                    // TFS: #1913. Cập nhật lại ngày tới hạn là ngày bên thực hiện xét ví dụ QA yêu cầu IT sửa máy tính và đặt ngày mong muốn hoàn thành là 11, khi IT tiếp nhận và đặt ngày dự kiến hoàn thành là 14, chỗ cột ngày đến hạn --> ngày mà bên tiếp nhận dự kiến hoàn thành Vì nếu ng
                    this.CurrentItem[CommonSPListField.CommonReqDueDateField] = this.dtDueDate.SelectedDate;
                    this.CurrentItem.Update();
                }
            }
        }

        /// <summary>
        /// SaveReferTo
        /// </summary>
        private void SaveReferTo()
        {
            int selectedReferTo = 0;
            int.TryParse(ddlReferTo.SelectedValue, out selectedReferTo);
            if (selectedReferTo > 0)
            {
                this.CurrentItem[StringConstant.RequestsList.ReferRequestField] = selectedReferTo;
            }
            else
            {
                this.CurrentItem[StringConstant.RequestsList.ReferRequestField] = null;
            }
        }

        /// <summary>
        /// Save request details.
        /// </summary>
        /// <returns></returns>
        private bool SaveRequestDetails()
        {
            bool res = false;

            string requestTypeName = this.GetRequestTypeNameForCurrentitem();

            if (string.Compare(requestTypeName, RequestTypeName.RequestBuyDetails, true) == 0)
            {
                RequestBuyDetailsDAL requestBuyDetailsDAL = new RequestBuyDetailsDAL(this.SiteUrl);

                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                List<RequestBuyDetails> requestBuyDetailsItems = seriallizer.Deserialize<List<RequestBuyDetails>>(hdDetailsBuyData.Value);
                if (requestBuyDetailsItems != null && requestBuyDetailsItems.Count > 0)
                {
                    #region Delete List Of Detail Items
                    try
                    {
                        string queryString = string.Format(@"<Where>
                                                            <Eq>
                                                                <FieldRef Name='{0}' LookupId='True' />
                                                                <Value Type='Lookup'>{1}</Value>
                                                            </Eq>
                                                       </Where>", StringConstant.RequestBuyDetailsList.Fields.Request, this.CurrentItem.ID);
                        IList<int> ids = null;
                        var items = requestBuyDetailsDAL.GetByQuery(queryString);
                        if (items != null && items.Count > 0)
                        {
                            ids = new List<int>();

                            foreach (var item in items)
                            {
                                var existedItem = requestBuyDetailsItems.Where(buyItem => buyItem.ID == item.ID).FirstOrDefault();
                                // Neu khong ton tai => Xoa
                                if (existedItem == null)
                                {
                                    ids.Add(item.ID);
                                }
                            }
                        }

                        if (ids != null && ids.Count > 0)
                        {
                            requestBuyDetailsDAL.DeleteItems(ids);
                        }
                    }
                    catch (Exception ex)
                    {
                        ULSLogging.LogError(ex);
                        ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
                    }
                    #endregion

                    foreach (var item in requestBuyDetailsItems)
                    {
                        item.Request = new LookupItem { LookupId = this.CurrentItem.ID };
                        //requestBuyDetailsDAL.SaveItem(item);
                    }

                    res = requestBuyDetailsDAL.SaveItems(requestBuyDetailsItems);
                    res = true;
                }
            }
            else if (string.Compare(requestTypeName, RequestTypeName.RequestRepairDetails, true) == 0)
            {
                RequestRepairDetailsDAL requestRepairDetailsDAL = new RequestRepairDetailsDAL(this.SiteUrl);

                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                List<RequestRepairDetails> requestRepairDetailsItems = seriallizer.Deserialize<List<RequestRepairDetails>>(hdDetailsRepair.Value);
                if (requestRepairDetailsItems != null && requestRepairDetailsItems.Count > 0)
                {
                    #region Delete List Of Detail Items
                    try
                    {
                        string queryString = string.Format(@"<Where>
                                                            <Eq>
                                                                <FieldRef Name='{0}' LookupId='True' />
                                                                <Value Type='Lookup'>{1}</Value>
                                                            </Eq>
                                                       </Where>", StringConstant.RequestRepairDetailsList.Fields.Request, this.CurrentItem.ID);
                        IList<int> ids = null;
                        var items = requestRepairDetailsDAL.GetByQuery(queryString);
                        if (items != null && items.Count > 0)
                        {
                            ids = new List<int>();

                            foreach (var item in items)
                            {
                                var existedItem = requestRepairDetailsItems.Where(buyItem => buyItem.ID == item.ID).FirstOrDefault();
                                // Neu khong ton tai => Xoa
                                if (existedItem == null)
                                {
                                    ids.Add(item.ID);
                                }
                            }
                        }

                        if (ids != null && ids.Count > 0)
                        {
                            requestRepairDetailsDAL.DeleteItems(ids);
                        }
                    }
                    catch (Exception ex)
                    {
                        ULSLogging.LogError(ex);
                        ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
                    }
                    #endregion

                    foreach (var item in requestRepairDetailsItems)
                    {
                        item.Request = new LookupItem { LookupId = this.CurrentItem.ID };

                        if (!string.IsNullOrEmpty(item.FromDateString))
                        {
                            DateTime fromDate;
                            DateTime.TryParseExact(item.FromDateString.Trim(), StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                            item.From = fromDate;
                        }
                        else
                        {
                            item.From = null;
                        }

                        if (!string.IsNullOrEmpty(item.ToDateString))
                        {
                            DateTime toDate;
                            DateTime.TryParseExact(item.ToDateString.Trim(), StringConstant.DateFormatddMMyyyy2, CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                            item.To = toDate;
                        }
                        else
                        {
                            item.To = null;
                        }
                        //requestRepairDetailsDAL.SaveItem(item);
                    }
                    res = requestRepairDetailsDAL.SaveItems(requestRepairDetailsItems);
                    res = true;
                }
            }
            else if (string.Compare(requestTypeName, RequestTypeName.RequestOtherDetails, true) == 0)
            {
                RequestOtherDetailsDAL requestOtherDetailsDAL = new RequestOtherDetailsDAL(this.SiteUrl);

                JavaScriptSerializer seriallizer = new JavaScriptSerializer();
                List<RequestOtherDetails> requestOtherDetailsItems = seriallizer.Deserialize<List<RequestOtherDetails>>(hdDetailsOthers.Value);
                if (requestOtherDetailsItems != null && requestOtherDetailsItems.Count > 0)
                {
                    #region Delete List Of Detail Items
                    try
                    {
                        string queryString = string.Format(@"<Where>
                                                            <Eq>
                                                                <FieldRef Name='{0}' LookupId='True' />
                                                                <Value Type='Lookup'>{1}</Value>
                                                            </Eq>
                                                       </Where>", StringConstant.RequestOtherDetailsList.Fields.Request, this.CurrentItem.ID);
                        IList<int> ids = null;
                        var items = requestOtherDetailsDAL.GetByQuery(queryString);
                        if (items != null && items.Count > 0)
                        {
                            ids = new List<int>();

                            foreach (var item in items)
                            {
                                var existedItem = requestOtherDetailsItems.Where(buyItem => buyItem.ID == item.ID).FirstOrDefault();
                                // Neu khong ton tai => Xoa
                                if (existedItem == null)
                                {
                                    ids.Add(item.ID);
                                }
                            }
                        }

                        if (ids != null && ids.Count > 0)
                        {
                            requestOtherDetailsDAL.DeleteItems(ids);
                        }
                    }
                    catch (Exception ex)
                    {
                        ULSLogging.LogError(ex);
                        ULSLogging.LogMessageToFile($"-- Error occurs on RequestFormUserControl: {ex.Message}");
                    }
                    #endregion

                    foreach (var item in requestOtherDetailsItems)
                    {
                        item.Request = new LookupItem { LookupId = this.CurrentItem.ID };
                        //requestOtherDetailsDAL.SaveItem(item);
                    }
                    res = requestOtherDetailsDAL.SaveItems(requestOtherDetailsItems);
                    res = true;
                }
            }

            res = true;

            return res;
        }

        #endregion

        /// <summary>
        /// GetRequestTypeNameForCurrentitem
        /// </summary>
        /// <returns></returns>
        private string GetRequestTypeNameForCurrentitem()
        {
            string requestTypeName = string.Empty;

            SPFieldLookupValue requestTypeRef = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[RequestsList.RequestTypeRefField]);

            if (requestTypeRef != null)
            {
                int requestTypeRefId = requestTypeRef.LookupId;
                requestTypeName = GetRequestTypeName(requestTypeRefId);
            }

            return requestTypeName;
        }

        /// <summary>
        /// GetRequestTypeName
        /// </summary>
        /// <param name="requestTypeRefId"></param>
        /// <returns></returns>
        private string GetRequestTypeName(int requestTypeRefId)
        {
            string requestTypeName = string.Empty;

            if (requestTypeRefId > 0)
            {
                var requestTypeItem = requestTypeDAL.GetByID(requestTypeRefId);
                if (requestTypeItem != null)
                {
                    requestTypeName = requestTypeItem.RequestsType;
                }
            }

            return requestTypeName;
        }

        #endregion

        #region Request Type Constant
        /// <summary>
        /// Request Type Constant
        /// </summary>
        public class RequestTypeName
        {
            /// <summary>
            /// RequestBuyDetails
            /// </summary>
            public const string RequestBuyDetails = "RequestBuyDetails";

            /// <summary>
            /// RequestRepairDetails
            /// </summary>
            public const string RequestRepairDetails = "RequestRepairDetails";

            /// <summary>
            /// RequestOtherDetails
            /// </summary>
            public const string RequestOtherDetails = "RequestOtherDetails";
        }
        #endregion
    }
}
