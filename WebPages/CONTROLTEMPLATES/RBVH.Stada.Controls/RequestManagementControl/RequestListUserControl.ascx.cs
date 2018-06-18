using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BizConstants = RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using Microsoft.SharePoint.Utilities;
using System.IO;
using RBVH.Stada.Intranet.Biz.Helpers;
using DocumentFormat.OpenXml.Packaging;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.ServiceModel.Web;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.UI;
using System.Globalization;
using System.Web;
using RBVH.Stada.Intranet.WebPages.Utils;
using RBVH.Stada.Intranet.Biz.ApprovalManagement;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RequestManagementControl
{
    /// <summary>
    /// RequestListUserControl
    /// </summary>
    public partial class RequestListUserControl : ListBaseUserUserControl
    {
        /// <summary>
        /// RequestsReceivedByDepartment
        /// </summary>
        protected const string RequestsReceivedByDepartmentSessionKey = "RequestsReceivedByDepartment";

        /// <summary>
        /// RequestsByReceivedDepartmentTab
        /// </summary>
        public const string RequestsByReceivedDepartmentTabId = "RequestsByReceivedDepartmentTab";

        #region Fields

        private RequestsDAL requestsDAL;
        private RequestReceivedDepartmentViewerDAL requestReceivedDepartmentViewerDAL;

        #endregion

        #region overrides

        #region DEL. 2017.10.04. TFS #1596.

        //protected override void InitListOfEmployeePosition_AddNewItem()
        //{
        //    base.InitListOfEmployeePosition_AddNewItem();

        //    // Nhan Vien
        //    listOfEmployeePosition_AddNewItem.Add(BizConstants.EmployeePositionCode.EMP);
        //    // Van Thu
        //    listOfEmployeePosition_AddNewItem.Add(BizConstants.EmployeePositionCode.AMD);
        //}

        //protected override void InitListOfEmployeePosition_ViewMyRequest()
        //{
        //    base.InitListOfEmployeePosition_ViewMyRequest();
        //    // Nhan Vien
        //    listOfEmployeePosition_ViewMyRequest.Add(BizConstants.EmployeePositionCode.EMP);
        //    // Van Thu
        //    listOfEmployeePosition_ViewMyRequest.Add(BizConstants.EmployeePositionCode.AMD);
        //}

        #endregion

        protected override void InitListOfEmployeePosition_ViewRequestToBeApproved()
        {
            base.InitListOfEmployeePosition_ViewRequestToBeApproved();
            // Truong Phong
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.DEH);
            // Giam Doc
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.BOD);
            // Feedback 14.11.2017: Pho Phong
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.GRL);
        }

        protected override string GetFromDateString(BizConstants.TypeOfDateRange typeOfDateRange = BizConstants.TypeOfDateRange.MonthYear)
        {
            return base.GetFromDateString(Biz.Constants.TypeOfDateRange.FromTo);
        }

        protected override string GetToDateString(BizConstants.TypeOfDateRange typeOfDateRange = BizConstants.TypeOfDateRange.MonthYear)
        {
            return base.GetToDateString(Biz.Constants.TypeOfDateRange.FromTo);
        }

        protected override string BuildQueryStringRequestsByDepartmentForBOD()
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

            var dateRange = BuildDateRange();

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
                                                </Where>", ApprovalFields.CommonLocation, this.CurrentEmployeeLocationId,
                                                        ApprovalFields.CommonDepartment, departmentId,
                                                        "Created", dateRange.Item1,
                                                        "Created", dateRange.Item2);
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
                                                </Where>", "Created", dateRange.Item1,
                                                        "Created", dateRange.Item2,
                                                        ApprovalFields.CommonLocation, this.CurrentEmployeeLocationId);
            }

            #endregion

            return stringQuery;
        }

        protected override bool HavePermissionAddNew()
        {
            #region MOD. 2017.10.04. TFS #1596.
            //var res = base.HavePermissionAddNew();

            //if (res)
            //{
            //    // Neu khong phai account AD
            //    if (string.Compare(EmployeeType.ADUser, this.CurrentEmployeeType, true) != 0)
            //    {
            //        res = false;
            //    }
            //}

            //return res;

            #region MOD. 2017.10.13. TFS #1596.
            //return IsEmployeeADUser();
            // Neu la AD User va khong phai la BOD
            return this.IsCurrentEmployeeADUser() && !this.IsCurrentEmployeeBOD();
            #endregion

            #endregion
        }

        #region ADD. 2017.10.04. TFS #1596.

        /// <summary>
        /// IsMyRequestTabActived
        /// </summary>
        /// <returns></returns>
        protected override bool IsMyRequestTabActived()
        {
            #region MOD. 2017.10.13. TFS #1596.
            //return IsCurrentEmployeeADUser();
            return IsCurrentEmployeeADUser() && !this.IsCurrentEmployeeBOD();
            #endregion
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            try
            {
                // Set current list url.
                this.listUrl = Biz.Constants.StringConstant.RequestsList.Url;
                this.requestsDAL = new RequestsDAL(SPContext.Current.Web.Url);
                this.requestReceivedDepartmentViewerDAL = new RequestReceivedDepartmentViewerDAL(SPContext.Current.Web.Url);

                this.RequestByReceivedDepartmentLi.ClientIDMode = ClientIDMode.Static;
                this.RequestsByReceivedDepartmentTab.ClientIDMode = ClientIDMode.Static;

                base.OnInit(e);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);

                var hasCurrentUserViewReceivedDepartmentRequests = this.HasCurrentUserViewReceivedDepartmentRequests();

                if (hasCurrentUserViewReceivedDepartmentRequests)
                {
                    this.ActiveRequestsByReceivedDepartmentTab();
                    if (!Page.IsPostBack)
                    {
                        this.LoadFormSearchRequestsByReceivedDepartment();
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        /// <summary>
        /// DucVT - Added - 15-March-2018. HR Admin don't allow see all reuqest of alll department.
        /// </summary>
        protected override void LoadListOfDepartment()
        {
            try
            {
                if (this.ddlDepartments != null)
                {
                    this.ddlDepartments.DataValueField = "ID";

                    //DepartmentDAL departmentDAL = new DepartmentDAL(SPContext.Current.Web.Url);
                    //var departments = departmentDAL.GetAll();
                    var departments = DepartmentDALObj.GetDepartmentsByLocation(new List<int> { this.CurrentEmployeeInfoObj.FactoryLocation.LookupId });
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
                    if (((string.Compare(this.CurrentEmployeePositionCode, BizConstants.EmployeePositionCode.DEH, true) == 0) && (string.Compare(this.CurrentEmployeeDepartmentCode, BizConstants.DepartmentCode.HR, true) == 0)) ||
                        (string.Compare(this.CurrentEmployeePositionCode, BizConstants.EmployeePositionCode.BOD, true) == 0))
                    {
                        this.ddlDepartments.SelectedValue = this.Page.Request[DepartmentParamName];
                        this.ddlDepartments.Enabled = true;
                    }
                    else
                    {
                        this.ddlDepartments.Enabled = false;
                        if (this.CurrentEmployeeInfoObj != null && this.CurrentEmployeeInfoObj.Department != null)
                        {
                            this.ddlDepartments.SelectedValue = this.CurrentEmployeeInfoObj.Department.LookupId.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

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
                this.lbtnView.Click += BtnView_Click;
                this.lbtnExport.Click += BtnExport_Click;
                this.gridRequestsByReceivedDepartment.RowDataBound += GridRequestsByReceivedDepartment_RowDataBound;
                this.gridRequestsByReceivedDepartment.PageIndexChanging += GridRequestsByReceivedDepartment_PageIndexChanging;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.dtcFromDate.IsDateEmpty && this.dtcFromDate.IsValid)
                {
                    if (!this.dtcToDate.IsDateEmpty && this.dtcToDate.IsValid)
                    {
                        this.LoadRequestsByReceivedDepartment();
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.dtcFromDate.IsDateEmpty && this.dtcFromDate.IsValid)
                {
                    if (!this.dtcToDate.IsDateEmpty && this.dtcToDate.IsValid)
                    {
                        DateTime fromDate = dtcFromDate.SelectedDate.Date;
                        DateTime toDate = dtcToDate.SelectedDate.Date;
                        var requestList = this.Page.Session[RequestsReceivedByDepartmentSessionKey] as List<Request>;
                        if (requestList == null)
                        {
                            requestList = this.GetReceivedDepartmentRequests();
                            this.Page.Session[RequestsReceivedByDepartmentSessionKey] = requestList;
                        }

                        if (requestList != null && requestList.Count > 0)
                        {
                            string templateFileName = "ThongKePhieuDeNghi.xlsx";
                            string destFilePath = "";

                            SPSecurity.RunWithElevatedPrivileges(delegate ()
                            {
                                string tempFolderPath = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\Tmp", 15);
                                Directory.CreateDirectory(tempFolderPath);
                                ExcelHelper.RemoveOldFiles(tempFolderPath, 1);

                                destFilePath = ExcelHelper.DownloadFile(SPContext.Current.Site.RootWeb.Url, "Shared Documents", templateFileName, tempFolderPath, string.Empty);

                                using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Open(destFilePath, true))
                                {
                                    string sheetName = "Phieu-De-Nghi";
                                    string filterStr = string.Format("Từ: {0} - đến: {1}", fromDate.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2), toDate.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2));
                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, "B", 2, filterStr, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    string currentDepatmentName = string.Empty;
                                    var departments = DepartmentDALObj.GetAll();

                                    var currentDepartment = departments.Where(x => x.ID == CurrentEmployeeDepartmentId).FirstOrDefault();
                                    if (currentDepartment != null)
                                    {
                                        currentDepatmentName = string.Format("{0} / {1}", currentDepartment.VietnameseName, currentDepartment.Name);
                                    }
                                    ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, "B", 3, currentDepatmentName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                    if (requestList.Any())
                                    {
                                        var listStatus = ApprovalStatusMapping;

                                        uint startRowIdx = 6;
                                        for (int i = 0; i < requestList.Count; i++)
                                        {
                                            var requestItem = requestList[i];

                                            uint newRowIdx = startRowIdx + (uint)i + 1;
                                            ExcelHelper.DuplicateRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx, newRowIdx);

                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(1), newRowIdx,
                                            (i + 1).ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);

                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(2), newRowIdx,
                                                requestItem.Title, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(3), newRowIdx,
                                              Convert.ToString(requestItem.RequestTypeRef.LookupValue), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                            string departmentName = requestItem.Department.LookupValue;
                                            var department = departments.Where(x => x.ID == requestItem.Department.LookupId).FirstOrDefault();
                                            if (department != null)
                                            {
                                                departmentName = string.Format("{0} / {1}", department.VietnameseName, department.Name);
                                            }
                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(4), newRowIdx,
                                              departmentName, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                            string requester = requestItem.CommonCreator.LookupValue;
                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(5), newRowIdx,
                                                requester, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                            string status = string.Format("{0} / {1}", listStatus[requestItem.WFStatus], requestItem.WFStatus);
                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(6), newRowIdx,
                                                status, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                            string createdDate = requestItem.Created.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2);
                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(7), newRowIdx,
                                                createdDate, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                            string finishDate = requestItem.FinishDate == null || requestItem.FinishDate == default(DateTime) ? string.Empty : requestItem.FinishDate.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2);
                                            ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(8), newRowIdx,
                                                finishDate, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);
                                        }
                                        ExcelHelper.RemoveRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx);
                                    }

                                    ExcelHelper.Save(spreadSheetDoc.WorkbookPart, sheetName);
                                }
                            });

                            if (!string.IsNullOrEmpty(destFilePath) && File.Exists(destFilePath))
                            {
                                string headerInfo = string.Format("attachment; filename={0}", Path.GetFileName(destFilePath));
                                var workbook = File.OpenRead(destFilePath);
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                Response.AddHeader("content-disposition", headerInfo);
                                //Response.ContentType = "application/octet-stream";
                                Response.ContentType = "application/vnd.ms-excel";
                                using (MemoryStream myMemoryStream = new MemoryStream())
                                {
                                    workbook.CopyTo(myMemoryStream);
                                    myMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                                    Response.Close();
                                }
                            }
                        }
                        else
                        {
                            string message = WebPageResourceHelper.GetResourceString("Requests_ExportExcel_Message");
                            string script = string.Format("alert('{0}');", message);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "requetExportExcelMessage", script, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// GridMyRquests_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void GridMyRquests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.Request;
                    if (dataItem != null)
                    {
                        string title = dataItem.Title != null ? dataItem.Title : string.Empty;
                        var litTitle = e.Row.FindControl("litTitle") as Literal;
                        litTitle.Text = title;

                        if (dataItem.RequestTypeRef != null)
                        {
                            var litRequestType = e.Row.FindControl("litRequestType") as Literal;
                            litRequestType.Text = dataItem.RequestTypeRef.LookupValue;
                        }

                        #region DEL 2017.09.25.
                        //string status = dataItem.ApprovalStatus != null ? dataItem.WFStatus : string.Empty;
                        //var lblStatus = e.Row.FindControl("lblStatus") as Label;
                        //lblStatus.Text = status;
                        //SetStatusCssClass(lblStatus, status);

                        //#region Pending At
                        ////if (dataItem.PendingAt != null && dataItem.PendingAt.Count > 0)
                        ////{
                        ////    int count = dataItem.PendingAt.Count;
                        ////    System.Text.StringBuilder employeeNameBuilder = new System.Text.StringBuilder();
                        ////    for (int i = 0; i < count - 1; i++)
                        ////    {
                        ////        var item = dataItem.PendingAt[i];
                        ////        employeeNameBuilder.AppendFormat("{0}, ", item.LookupValue);
                        ////    }
                        ////    employeeNameBuilder.AppendFormat("{0}", dataItem.PendingAt[count-1].LookupValue);

                        ////    var litPendingAt = e.Row.FindControl("litPendingAt") as Literal;
                        ////    litPendingAt.Text = employeeNameBuilder.ToString();
                        ////}
                        //#endregion

                        //if (dataItem.Created != null)
                        //{
                        //    var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                        //    litCreatedDate.Text = dataItem.Created.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);
                        //}

                        //var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
                        //StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
                        //UriBuilder currentUriBuilder = new UriBuilder(this.CurrentUrl);
                        //if (!currentUriBuilder.HasQuery(TabParamName))
                        //{
                        //    sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, MyRequestsTabId);
                        //}
                        //else
                        //{
                        //    sourceParamsUrl = sourceParamsUrl.Replace(RequestsToBeApprovedTabId, MyRequestsTabId);
                        //    sourceParamsUrl = sourceParamsUrl.Replace(RequestsByDepartmentTabId, MyRequestsTabId);
                        //}

                        //bool editable = IsDraftOrRejectedOrSubmittedItem(dataItem);
                        //if (editable)
                        //{
                        //    var linkEdit = e.Row.FindControl("linkEdit") as System.Web.UI.HtmlControls.HtmlControl;
                        //    if (linkEdit != null)
                        //    {
                        //        //string href = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
                        //        //    SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(), this.ListId, dataItem.ID,
                        //        //    sourceParamsUrl);
                        //        string href = BuildEditFormUrl(sourceParamsUrl.ToString(), dataItem);
                        //        linkEdit.Attributes.Add("href", href);
                        //        linkEdit.Visible = true;
                        //    }

                        //    var btnCancelWF = e.Row.FindControl("btnCancelWF") as Button;
                        //    if (btnCancelWF != null)
                        //    {
                        //        btnCancelWF.Enabled = true;
                        //    }
                        //}
                        //else
                        //{
                        //    //var spanEdit = e.Row.FindControl("spanEdit") as System.Web.UI.HtmlControls.HtmlControl;
                        //    //if (spanEdit != null)
                        //    //{
                        //    //    spanEdit.Attributes.Add("style", "visibility: hidden;");
                        //    //}

                        //    // linkView
                        //    var linkView = e.Row.FindControl("linkView") as System.Web.UI.HtmlControls.HtmlControl;
                        //    if (linkView != null)
                        //    {
                        //        //string href = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
                        //        //    SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_DISPLAYFORM).ToString(), this.ListId, dataItem.ID,
                        //        //    sourceParamsUrl);
                        //        string href = BuildDisplayFormUrl(sourceParamsUrl.ToString(), dataItem);
                        //        linkView.Attributes.Add("href", href);
                        //        linkView.Visible = true;
                        //    }
                        //}
                        #endregion

                        #region ADD 2017.09.22
                        base.GridMyRquests_RowDataBound(sender, e);
                        #endregion

                        #region #region Duc.VoTan.ADD.2017.10.12. TFS#1597.
                        if (string.Compare(dataItem.WFStatus, ApprovalStatus.Approved, true) == 0)
                        {
                            // linkView
                            var linkView = e.Row.FindControl("linkView") as HtmlControl;
                            if (linkView != null && linkView.Visible == true && !string.IsNullOrEmpty(linkView.Attributes["href"]))
                            {
                                UriBuilder displayFormUrl = new UriBuilder(linkView.Attributes["href"]);
                                displayFormUrl.SetQuery("PageType", ((int)PAGETYPE.PAGE_EDITFORM).ToString());
                                linkView.Attributes.Add("href", displayFormUrl.ToString());
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// RequestToBeApproved_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void RequestToBeApproved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.Request;
                    if (dataItem != null)
                    {
                        string title = dataItem.Title != null ? dataItem.Title : string.Empty;
                        var litTitle = e.Row.FindControl("litTitle") as Literal;
                        litTitle.Text = title;

                        if (dataItem.RequestTypeRef != null)
                        {
                            var litRequestType = e.Row.FindControl("litRequestType") as Literal;
                            litRequestType.Text = dataItem.RequestTypeRef.LookupValue;
                        }

                        if (dataItem.CommonCreator != null)
                        {
                            var litRequestFrom = e.Row.FindControl("litRequestFrom") as Literal;
                            litRequestFrom.Text = dataItem.CommonCreator.LookupValue;
                        }

                        #region DEL 2017.09.25.
                        //string status = dataItem.ApprovalStatus != null ? dataItem.WFStatus : string.Empty;
                        //var lblStatus = e.Row.FindControl("lblStatus") as Label;
                        //lblStatus.Text = status;
                        //SetStatusCssClass(lblStatus, status);

                        //#region Pending At
                        ////if (dataItem.PendingAt != null && dataItem.PendingAt.Count > 0)
                        ////{
                        ////    int count = dataItem.PendingAt.Count;
                        ////    System.Text.StringBuilder employeeNameBuilder = new System.Text.StringBuilder();
                        ////    for (int i = 0; i < count - 1; i++)
                        ////    {
                        ////        var item = dataItem.PendingAt[i];
                        ////        employeeNameBuilder.AppendFormat("{0}, ", item.LookupValue);
                        ////    }
                        ////    employeeNameBuilder.AppendFormat("{0}", dataItem.PendingAt[count - 1].LookupValue);

                        ////    var litPendingAt = e.Row.FindControl("litPendingAt") as Literal;
                        ////    litPendingAt.Text = employeeNameBuilder.ToString();
                        ////}
                        //#endregion

                        //if (dataItem.Created != null)
                        //{
                        //    var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                        //    litCreatedDate.Text = dataItem.Created.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);
                        //}

                        //var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
                        //StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
                        //UriBuilder currentUriBuilder = new UriBuilder(this.CurrentUrl);
                        //if (!currentUriBuilder.HasQuery(TabParamName))
                        //{
                        //    sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, RequestsToBeApprovedTabId);
                        //}
                        //else
                        //{
                        //    sourceParamsUrl = sourceParamsUrl.Replace(MyRequestsTabId, RequestsToBeApprovedTabId);
                        //    sourceParamsUrl = sourceParamsUrl.Replace(RequestsByDepartmentTabId, RequestsToBeApprovedTabId);
                        //}

                        //// linkView
                        ////var linkView = e.Row.FindControl("linkView") as System.Web.UI.HtmlControls.HtmlControl;
                        ////string viewHref = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
                        ////    SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_DISPLAYFORM).ToString(),
                        ////    this.ListId, dataItem.ID, sourceParamsUrl);
                        ////linkView.Attributes.Add("href", viewHref);

                        //// linkEdit
                        //var linkEdit = e.Row.FindControl("linkEdit") as System.Web.UI.HtmlControls.HtmlControl;
                        ////string editHref = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}", SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(),
                        ////    this.ListId, dataItem.ID, sourceParamsUrl);
                        //string editHref = BuildEditFormUrl(sourceParamsUrl.ToString(), dataItem);
                        //linkEdit.Attributes.Add("href", editHref);
                        #endregion

                        #region ADD 2017.09.25.
                        base.RequestToBeApproved_RowDataBound(sender, e);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// RequestByDepartment_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void RequestByDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.Request;
                    if (dataItem != null)
                    {
                        string title = dataItem.Title != null ? dataItem.Title : string.Empty;
                        var litTitle = e.Row.FindControl("litTitle") as Literal;
                        litTitle.Text = title;

                        if (dataItem.RequestTypeRef != null)
                        {
                            var litRequestType = e.Row.FindControl("litRequestType") as Literal;
                            litRequestType.Text = dataItem.RequestTypeRef.LookupValue;
                        }

                        if (dataItem.CommonCreator != null)
                        {
                            var litRequestFrom = e.Row.FindControl("litRequestFrom") as Literal;
                            litRequestFrom.Text = dataItem.CommonCreator.LookupValue;
                        }

                        #region DEL 2017.09.25.
                        //string status = dataItem.ApprovalStatus != null ? dataItem.WFStatus : string.Empty;
                        //var lblStatus = e.Row.FindControl("lblStatus") as Label;
                        //lblStatus.Text = status;
                        //SetStatusCssClass(lblStatus, status);

                        //#region Pending At
                        ////if (dataItem.PendingAt != null && dataItem.PendingAt.Count > 0)
                        ////{
                        ////    int count = dataItem.PendingAt.Count;
                        ////    System.Text.StringBuilder employeeNameBuilder = new System.Text.StringBuilder();
                        ////    for (int i = 0; i < count - 1; i++)
                        ////    {
                        ////        var item = dataItem.PendingAt[i];
                        ////        employeeNameBuilder.AppendFormat("{0}, ", item.LookupValue);
                        ////    }
                        ////    employeeNameBuilder.AppendFormat("{0}", dataItem.PendingAt[count-1].LookupValue);

                        ////    var litPendingAt = e.Row.FindControl("litPendingAt") as Literal;
                        ////    litPendingAt.Text = employeeNameBuilder.ToString();
                        ////}
                        //#endregion

                        //if (dataItem.Created != null)
                        //{
                        //    var litCreatedDate = e.Row.FindControl("litCreatedDate") as Literal;
                        //    litCreatedDate.Text = dataItem.Created.ToString(Biz.Constants.StringConstant.DateFormatddMMyyyyhhmmssttt);
                        //}

                        //var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
                        //StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
                        //UriBuilder currentUriBuilder = new UriBuilder(this.CurrentUrl);
                        //if (!currentUriBuilder.HasQuery(TabParamName))
                        //{
                        //    sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, RequestsByDepartmentTabId);
                        //}
                        //else
                        //{
                        //    sourceParamsUrl = sourceParamsUrl.Replace(MyRequestsTabId, RequestsByDepartmentTabId);
                        //    sourceParamsUrl = sourceParamsUrl.Replace(RequestsToBeApprovedTabId, RequestsByDepartmentTabId);
                        //}

                        //// linkView
                        //var linkView = e.Row.FindControl("linkView") as System.Web.UI.HtmlControls.HtmlControl;
                        //if (linkView != null)
                        //{
                        //    //string href = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
                        //    //    SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_DISPLAYFORM).ToString(), this.ListId, dataItem.ID,
                        //    //    sourceParamsUrl);
                        //    string href = BuildDisplayFormUrl(sourceParamsUrl.ToString(), dataItem);
                        //    linkView.Attributes.Add("href", href);
                        //    //linkView.Visible = true;
                        //}

                        ////var linkEdit = e.Row.FindControl("linkEdit") as System.Web.UI.HtmlControls.HtmlControl;
                        ////if (linkEdit != null)
                        ////{
                        ////    bool editable = IsDraftOrRejectedOrSubmittedItem(dataItem);
                        ////    if (editable)
                        ////    {
                        ////        string href = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}",
                        ////            SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(), this.ListId, dataItem.ID,
                        ////            sourceParamsUrl);
                        ////        linkEdit.Attributes.Add("href", href);
                        ////        //linkEdit.Visible = true;
                        ////    }
                        ////    else
                        ////    {
                        ////        var spanEdit = e.Row.FindControl("spanEdit") as System.Web.UI.HtmlControls.HtmlControl;
                        ////        if (spanEdit != null)
                        ////        {
                        ////            spanEdit.Attributes.Add("style", "visibility: hidden;");
                        ////        }
                        ////    }
                        ////}
                        #endregion

                        #region ADD 2017.09.25.
                        base.RequestByDepartment_RowDataBound(sender, e);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        private void GridRequestsByReceivedDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Request;
                    var dataItemProperties = dataItem.GetProperties();

                    string title = dataItem.Title;
                    var litTitle = e.Row.FindControl("litTitle") as Literal;
                    litTitle.Text = title;

                    if (dataItem.RequestTypeRef != null)
                    {
                        var litRequestType = e.Row.FindControl("litRequestType") as Literal;
                        litRequestType.Text = dataItem.RequestTypeRef.LookupValue;
                    }

                    if (dataItem.CommonCreator != null)
                    {
                        var litRequestFrom = e.Row.FindControl("litRequestFrom") as Literal;
                        litRequestFrom.Text = dataItem.CommonCreator.LookupValue;
                    }

                    #region Status
                    var wfStatusObject = dataItemProperties[Biz.ApprovalManagement.ApprovalFields.WFStatus];
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
                        litCreatedDate.Text = created.ToString(BizConstants.StringConstant.DateFormatddMMyyyyhhmmssttt);
                    }
                    #endregion

                    var sourceValues = this.BuildSourceValuesParamForCurrentUrl();
                    StringBuilder sourceParamsUrl = this.BuildSourceParamUrl(sourceValues);
                    UriBuilder currentUriBuilder = new UriBuilder(this.CurrentUrl);
                    if (!currentUriBuilder.HasQuery(TabParamName))
                    {
                        sourceParamsUrl.AppendFormat("&{0}={1}={2}", BizConstants.UrlParamName.ReturnUrlParamName, TabParamName, RequestsByReceivedDepartmentTabId);
                    }
                    else
                    {
                        sourceParamsUrl = sourceParamsUrl.Replace(MyRequestsTabId, RequestsByReceivedDepartmentTabId);
                        sourceParamsUrl = sourceParamsUrl.Replace(RequestsToBeApprovedTabId, RequestsByReceivedDepartmentTabId);
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
                    //linkView.Attributes.Add("href", viewHref);
                    linkView.Attributes.Add("onclick", string.Format("openModalDialog('{0}', '{1}', null, SP.UI.DialogResult.cancel);", title, viewHref));

                    // linkEdit
                    var linkEdit = e.Row.FindControl("linkEdit") as System.Web.UI.HtmlControls.HtmlControl;
                    //string editHref = string.Format("{0}/_layouts/15/listform.aspx?PageType={1}&ListId={2}&ID={3}&{4}", SPContext.Current.Web.Url, ((int)PAGETYPE.PAGE_EDITFORM).ToString(),
                    //    this.ListId, dataItem.ID, sourceParamsUrl);

                    string editHref = BuildEditFormUrl(sourceParamsUrl.ToString(), itemID);
                    //linkEdit.Attributes.Add("href", editHref);
                    linkEdit.Attributes.Add("onclick", string.Format("openModalDialog('{0}', '{1}', null, SP.UI.DialogResult.cancel);", title, editHref));
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        private void GridRequestsByReceivedDepartment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.gridRequestsByReceivedDepartment.PageIndex = e.NewPageIndex;
                if (this.Page.Session[RequestsReceivedByDepartmentSessionKey] != null)
                {
                    this.gridRequestsByReceivedDepartment.DataSource = this.Page.Session[RequestsReceivedByDepartmentSessionKey];
                    this.gridRequestsByReceivedDepartment.DataBind();
                }
                else
                {
                    this.LoadRequestsByReceivedDepartment();
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        #endregion

        #region Methods

        public void ExportRequests()
        {
            try
            {
                string monthString = Request.QueryString["Month"];
                string yearString = Request.QueryString["Year"];
                string departmentIdString = Request.QueryString["DepartmentId"];
                if (!string.IsNullOrEmpty(monthString) && !string.IsNullOrEmpty(yearString) && !string.IsNullOrEmpty(departmentIdString))
                {
                    int month = Convert.ToInt32(monthString);
                    int year = Convert.ToInt32(yearString);
                    DateTime fromDate = new DateTime(year, month, 1, 0, 0, 0);
                    DateTime toDate = new DateTime(year, month, 1, 0, 0, 0).AddMonths(1);

                    //if (DateTime.TryParse out fromDate) && DateTime.TryParse(to, out toDate) && fromDate <= toDate)
                    //{
                    string templateFileName = "ThongKePhieuDeNghi.xlsx";
                    string destFilePath = "";

                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        string tempFolderPath = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\Tmp", 15);
                        Directory.CreateDirectory(tempFolderPath);
                        ExcelHelper.RemoveOldFiles(tempFolderPath, 1);

                        destFilePath = ExcelHelper.DownloadFile(SPContext.Current.Site.RootWeb.Url, "Shared Documents", templateFileName, tempFolderPath, string.Empty);
                        string siteUrl = SPContext.Current.Site.Url;

                        using (SpreadsheetDocument spreadSheetDoc = SpreadsheetDocument.Open(destFilePath, true))
                        {
                            string sheetName = "Phieu-De-Nghi";
                            string webUrl = SPContext.Current.Web.Url;
                            var employeeInfoDAL = new EmployeeInfoDAL(webUrl);

                            EmployeeInfo currentUser = employeeInfoDAL.GetByADAccount(SPContext.Current.Web.CurrentUser.ID);
                            if (currentUser != null)
                            {
                                string[] viewFields = new string[] { RequestsList.TitleField, RequestsList.RequestTypeRefField, RequestsList.RequestTypeRefIdField, "ID", RequestsList.FinishDateField, RequestsList.CommonDepartmentField };
                                var requestList = this.requestsDAL.GetRequests(fromDate, toDate, Convert.ToInt32(departmentIdString), viewFields);

                                string filterStr = string.Format("Từ: {0} - đến: {1}", fromDate.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2), toDate.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2));
                                ExcelHelper.InsertSharedText(spreadSheetDoc.WorkbookPart, sheetName, "B", 3, filterStr);

                                if (requestList.Any())
                                {
                                    uint startRowIdx = 6;
                                    for (int i = 0; i < requestList.Count; i++)
                                    {
                                        var requestItem = requestList[i];

                                        uint newRowIdx = startRowIdx + (uint)i + 1;
                                        ExcelHelper.DuplicateRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx, newRowIdx);

                                        ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(1), newRowIdx,
                                        (i + 1).ToString(), DocumentFormat.OpenXml.Spreadsheet.CellValues.Number);

                                        ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(2), newRowIdx,
                                            requestItem.Title, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                        ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(3), newRowIdx,
                                          Convert.ToString(requestItem.RequestTypeRef.LookupValue), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                        ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(4), newRowIdx,
                                          Convert.ToString(requestItem.Department.LookupValue), DocumentFormat.OpenXml.Spreadsheet.CellValues.String);

                                        string finishDate = requestItem.FinishDate == null || requestItem.FinishDate == default(DateTime) ? string.Empty : requestItem.FinishDate.ToString(BizConstants.StringConstant.DateFormatddMMyyyy2);
                                        ExcelHelper.InsertCellValue(spreadSheetDoc.WorkbookPart, sheetName, ExcelHelper.ConvertColumnIndexToLetter(5), newRowIdx,
                                          finishDate, DocumentFormat.OpenXml.Spreadsheet.CellValues.String);
                                    }
                                    ExcelHelper.RemoveRow(spreadSheetDoc.WorkbookPart, sheetName, startRowIdx);
                                }
                            }
                            ExcelHelper.Save(spreadSheetDoc.WorkbookPart, sheetName);
                        }
                    });

                    if (!string.IsNullOrEmpty(destFilePath) && File.Exists(destFilePath))
                    {
                        string headerInfo = string.Format("attachment; filename={0}", Path.GetFileName(destFilePath));
                        var workbook = File.OpenRead(destFilePath);
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("content-disposition", headerInfo);
                        using (MemoryStream myMemoryStream = new MemoryStream())
                        {
                            workbook.CopyTo(myMemoryStream);
                            myMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// LoadMyRequests
        /// </summary>
        protected override void LoadListOfMyRequests()
        {
            try
            {
                StringBuilder queryStringMyRequestBuilder = new StringBuilder();
                queryStringMyRequestBuilder.Append(this.queryStringMyRequest);
                queryStringMyRequestBuilder.Append(this.orderByQueryString);

                List<Biz.Models.Request> myRequests = requestsDAL.GetByQuery(queryStringMyRequestBuilder.ToString());
                this.gridMyRquests.DataSource = myRequests;
                this.gridMyRquests.DataBind();

                this.Page.Session[MyRequestsSessionKey] = myRequests;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// LoadRequestsToBeApproved
        /// </summary>
        protected override void LoadListOfRequestToBeApproved()
        {
            try
            {
                StringBuilder queryStringRequestsToBeApprovedBuilder = new StringBuilder();
                queryStringRequestsToBeApprovedBuilder.Append(this.queryStringReequestsToBeApproved);
                queryStringRequestsToBeApprovedBuilder.Append(this.orderByQueryString);
                List<Biz.Models.Request> requestsToBeApproved = requestsDAL.GetByQuery(queryStringRequestsToBeApprovedBuilder.ToString());
                this.gridRequestToBeApproved.DataSource = requestsToBeApproved;
                this.gridRequestToBeApproved.DataBind();

                this.Page.Session[RequestsToBeApprovedSessionKey] = requestsToBeApproved;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// LoadListOfRequestByDepartment
        /// </summary>
        protected override void LoadListOfRequestByDepartment()
        {
            try
            {
                StringBuilder queryStringRequestsByDepartmentBuilder = new StringBuilder();
                queryStringRequestsByDepartmentBuilder.Append(this.queryStringRequestsByDepartment);
                queryStringRequestsByDepartmentBuilder.Append(this.orderByQueryString);
                List<Biz.Models.Request> requestsByDepartment = requestsDAL.GetByQuery(queryStringRequestsByDepartmentBuilder.ToString());
                this.gridRequestByDepartment.DataSource = requestsByDepartment;
                this.gridRequestByDepartment.DataBind();

                this.Page.Session[RequestsByDepartmentSessionKey] = requestsByDepartment;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// IsDraftOrRejectedOrSubmittedItem
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        protected override bool IsDraftOrRejectedOrSubmittedItem(object dataItem)
        {
            bool res = false;

            Biz.Models.Request request = dataItem as Biz.Models.Request;

            if (request != null)
            {
                string status = request.ApprovalStatus;

                if (string.IsNullOrEmpty(status))
                {
                    res = true;
                }
                else
                {
                    if ((string.Compare(status, BizConstants.Status.Draft, true) == 0) ||
                        (string.Compare(status, BizConstants.Status.Rejected, true) == 0) ||
                        (string.Compare(status, BizConstants.Status.Submitted, true) == 0))
                    {
                        res = true;
                    }
                }
            }

            return res;
        }

        private bool HasCurrentUserViewReceivedDepartmentRequests()
        {
            var res = false;

            try
            {
                var requestReceivedDepartmentViewerItem = this.requestReceivedDepartmentViewerDAL.GetReceivedDepartmentRequestViewerByLocaltionAndDepartmentAndEmployee(this.CurrentEmployeeLocationId, this.CurrentEmployeeDepartmentId, this.CurrentEmployeeID);
                if (requestReceivedDepartmentViewerItem != null)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }

            return res;
        }

        private void ActiveRequestsByReceivedDepartmentTab()
        {
            RequestByReceivedDepartmentLi.Visible = true;
            RequestsByReceivedDepartmentTab.Visible = true;
        }

        private void LoadRequestsByReceivedDepartment()
        {
            try
            {
                int locationId = this.CurrentEmployeeLocationId;
                int departmentId = this.CurrentEmployeeDepartmentId;
                string status = this.ddlStatus.SelectedValue;

                var requests = this.GetReceivedDepartmentRequests();
                this.gridRequestsByReceivedDepartment.DataSource = requests;
                this.gridRequestsByReceivedDepartment.DataBind();

                this.Page.Session[RequestsReceivedByDepartmentSessionKey] = requests;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        private List<Request> GetReceivedDepartmentRequests()
        {
            int locationId = this.CurrentEmployeeLocationId;
            int departmentId = this.CurrentEmployeeDepartmentId;
            DateTime fromDate = this.dtcFromDate.SelectedDate.Date;
            DateTime toDate = this.dtcToDate.SelectedDate.Date;
            string status = this.ddlStatus.SelectedValue;

            var requests = requestsDAL.GetReceivedDepartmentRequests(locationId, departmentId, fromDate, toDate, status);

            return requests;
        }

        private void LoadFormSearchRequestsByReceivedDepartment()
        {
            try
            {
                if (IsVietnameseLanguage)
                {
                    var listStatus = ApprovalStatusMapping;
                    this.ddlStatus.Items.Clear();
                    foreach (var status in listStatus)
                    {
                        ListItem listItem = new ListItem { Value = status.Key, Text = status.Value };
                        this.ddlStatus.Items.Add(listItem);
                    }

                    ListItem emptyListItem = new ListItem { Value = "", Text = "Tât cả" };
                    ddlStatus.Items.Insert(0, emptyListItem);
                }
                else
                {
                    var listStatus = ApprovalStatusMapping;
                    this.ddlStatus.Items.Clear();
                    foreach (var status in listStatus)
                    {
                        ListItem listItem = new ListItem { Value = status.Key, Text = status.Key };
                        this.ddlStatus.Items.Add(listItem);
                    }

                    ListItem emptyListItem = new ListItem { Value = "", Text = "All" };
                    ddlStatus.Items.Insert(0, emptyListItem);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestListUserControl: {ex.Message}");
            }
        }

        protected override Tuple<string, string> BuildDateRange()
        {
            string fromDate = this.Page.Request.Params.Get(FromDateParamName);
            string toDate = this.Page.Request.Params.Get(ToDateParamName);
            string fromDateValue = string.Empty;
            string toDateValue = string.Empty;
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                DateTime dtFromDate;
                bool isValidFromDate = DateTime.TryParseExact(fromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFromDate);
                if (isValidFromDate)
                    fromDateValue = $"{dtFromDate:yyyy-MM-dd}";
                else
                    fromDateValue = $"{DateTime.Now:yyyy-MM-dd}";

                DateTime dtToDate;
                bool isValidToDate = DateTime.TryParseExact(toDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dtToDate);
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
                DateTime endDate = startDate; //new DateTime(year, month, 1);
                //endDate = endDate.AddMonths(1).AddSeconds(-1);

                fromDateValue = $"{startDate:yyyy-MM-dd}";
                toDateValue = $"{endDate:yyyy-MM-dd}";
            }

            return System.Tuple.Create<string, string>(fromDateValue, toDateValue);
        }

        #endregion
    }
}
