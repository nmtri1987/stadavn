using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Webservices.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Activation;
using RBVH.Stada.Intranet.Webservices.Helper;
using System.Linq;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Helpers;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using RBVH.Stada.Intranet.WebPages.Utils;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Overtime
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class OvertimeService : IOvertimeService
    {
        private readonly OverTimeManagementDAL _overTimeManagementDAL;
        private readonly OverTimeManagementDetailDAL _overTimeManagementDetailDAL;
        private readonly NotOvertimeManagementDAL _notOvertimeManagementDal;
        private readonly EmployeeInfoDAL _employeeInfoDAL;
        public OvertimeService()
        {
            var webUrl = SPContext.Current.Web.Url;
            _overTimeManagementDAL = new OverTimeManagementDAL(webUrl);
            _overTimeManagementDetailDAL = new OverTimeManagementDetailDAL(webUrl);
            _notOvertimeManagementDal = new NotOvertimeManagementDAL(webUrl);
            _employeeInfoDAL = new EmployeeInfoDAL(webUrl);
        }

        public OverTimeModel GetById(string Id)
        {
            try
            {
                OverTimeModel overTimeModel = new OverTimeModel();
                int itemId = Convert.ToInt32(Id);
                var overtime = _overTimeManagementDAL.GetByID(itemId);
                if (overtime != null && overtime.ID > 0)
                {
                    overTimeModel.ID = overtime.ID;
                    overTimeModel.OtherRequirements = overtime.OtherRequirements;
                    overTimeModel.DHComments = overtime.DHComments;
                    overTimeModel.BODComments = overtime.BODComments;
                    overTimeModel.SecurityComments = overtime.SecurityComments;
                    overTimeModel.Requester = overtime.Requester;
                    overTimeModel.ApprovedBy = overtime.ApprovedBy;

                    overTimeModel.SumOfEmployee = overtime.SumOfEmployee + "";
                    overTimeModel.SumOfMeal = overtime.SumOfMeal + "";

                    overTimeModel.ApprovalStatus = overtime.ApprovalStatus;
                    overTimeModel.CommonDepartment = overtime.CommonDepartment;

                    overTimeModel.Date = overtime.CommonDate.ToString("s");
                    overTimeModel.FirstApprovedDate = overtime.FirstApprovedDate.ToString(StringConstant.DateFormatddMMyyyyHHmmss);
                    overTimeModel.Modified = overtime.Modified.ToString(StringConstant.DateFormatddMMyyyyHHmmss);
                    overTimeModel.FirstApprovedBy = overtime.FirstApprovedBy;
                    overTimeModel.CommonLocation = overtime.CommonLocation;
                    overTimeModel.Place = overtime.Place;
                    overTimeModel.ApprovalStatus = overtime.ApprovalStatus;

                    if (overtime.RequestDueDate != null && overtime.RequestDueDate != default(DateTime))
                    {
                        overTimeModel.RequestDueDate = overtime.RequestDueDate.ToString(StringConstant.DateFormatddMMyyyy2);
                        if (overtime.RequestDueDate.Date < DateTime.Now.Date)
                        {
                            //overTimeModel.RequestExpired = true;
                            overTimeModel.RequestExpired = false;
                        }
                    }

                    overTimeModel.ApprovedLevel = 0;
                    if (overTimeModel.ApprovalStatus == "true")
                    {
                        overTimeModel.ApprovedLevel = 1; // Truong phong Approved
                        // Check Approved By: BOD 
                        var approverInfo = _employeeInfoDAL.GetByADAccount(overtime.ApprovedBy.ID);
                        if (approverInfo.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.BOD) // BOD Approved
                        {
                            overTimeModel.ApprovedLevel = 2;
                        }
                    }
                    else if (overTimeModel.ApprovalStatus == "false")
                    {
                        overTimeModel.ApprovedLevel = 1; // Truong phong Rejected
                        // Check Approved By: BOD 
                        var approverInfo = _employeeInfoDAL.GetByADAccount(overtime.ApprovedBy.ID);
                        if (approverInfo.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.BOD) // BOD Rejected
                        {
                            overTimeModel.ApprovedLevel = 2;
                        }
                    }
                    else // Check Truong phong co duyet chua
                    {
                        // Check Approved By: BOD 
                        var approverInfo = _employeeInfoDAL.GetByADAccount(overtime.ApprovedBy.ID);
                        if (approverInfo.EmployeePosition.LookupId == (int)StringConstant.EmployeePosition.BOD) // BOD Rejected
                        {
                            overTimeModel.ApprovedLevel = 1;
                        }
                    }


                    var OverTimeDetails = _overTimeManagementDetailDAL.GetByOvertimeId(overTimeModel.ID);
                    foreach (var detail in OverTimeDetails)
                    {
                        var overtimeDetailModel = new OvertimeDetailModel();
                        overtimeDetailModel.ID = detail.ID;
                        overtimeDetailModel.OvertimeFrom = detail.OvertimeFrom.ToString();
                        overtimeDetailModel.OvertimeManagementID = detail.OvertimeManagementID;
                        overtimeDetailModel.Employee.LookupId = detail.Employee.LookupId;
                        overtimeDetailModel.Employee.LookupValue = detail.Employee.LookupValue;
                        overtimeDetailModel.OvertimeMgmtApprovalStatus = detail.ApprovalStatus;
                        overtimeDetailModel.SummaryLinks = detail.SummaryLinks;
                        overtimeDetailModel.OvertimeTo = detail.OvertimeTo.ToString();
                        overtimeDetailModel.Task = string.IsNullOrEmpty(detail.SummaryLinks) ? detail.Task : detail.Task + "_" + detail.SummaryLinks;
                        overtimeDetailModel.CompanyTransport = detail.CompanyTransport;
                        overtimeDetailModel.WorkingHours = detail.WorkingHours + "";
                        // Grid
                        overtimeDetailModel.FullName = overtimeDetailModel.Employee.LookupValue + "_" + overtimeDetailModel.Employee.LookupId + "_";
                        overtimeDetailModel.OvertimeHourFrom = detail.OvertimeFrom.Hour + ":" + detail.OvertimeFrom.Minute;
                        overtimeDetailModel.OvertimeHourTo = detail.OvertimeTo.Hour + ":" + detail.OvertimeTo.Minute;
                        overTimeModel.OvertimeDetailModelList.Add(overtimeDetailModel);
                    }

                }
                return overTimeModel;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Overtime Service - GetById fn",
                   TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
               string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public MessageResult UpdateApprove(OverTimeModel overTimeModel)
        {
            try
            {
                _overTimeManagementDAL.UpdateApprovalStatus(overTimeModel.ID, overTimeModel.ApprovalStatus);

                return new MessageResult
                {
                    Code = 0,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - OvertimeService - UpdateApprove fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return new MessageResult
                {
                    Code = 1,
                    Message = ex.Message
                };
            }
        }

        private string GetRequestStatusMessage(OverTimeManagement currentItem)
        {
            string msgResult = "";

            string currentStatus = currentItem.ApprovalStatus.ToLower();
            if (currentStatus == "true")
            {
                msgResult = WebPageResourceHelper.GetResourceString("RequestStatusApproved");
            }
            else if (currentStatus == "false")
            {
                msgResult = WebPageResourceHelper.GetResourceString("RequestStatusRejected");
            }

            return msgResult;
        }

        public MessageResult InsertOvertime(OverTimeModel overTimeModel)
        {
            return InsertOvertime(SPContext.Current.Web, overTimeModel);
        }

        public MessageResult InsertOvertime(SPWeb spWeb, OverTimeModel overTimeModel, bool autoovertime = false)
        {
            var overtimeManagementId = 0;
            bool createNew = overTimeModel.ID == 0;
            bool breakFunc = false;
            var overtimeEntity = overTimeModel.ToEntity();
            try
            {
                if (createNew == false)
                {
                    var currentItem = _overTimeManagementDAL.GetByID(spWeb, overtimeEntity.ID);

                    string actionType = overTimeModel.ApprovalStatus.ToLower();
                    if (actionType == "true" || actionType == "false" || overTimeModel.RequiredBODApprove == true) // action = approve or reject
                    {
                        string retMsg = GetRequestStatusMessage(currentItem);
                        if (!string.IsNullOrEmpty(retMsg))
                        {
                            breakFunc = true;
                            int errorCode = actionType == "false" ? (int)OverTimeErrorCode.CannotReject : (int)OverTimeErrorCode.CannotApprove;
                            return new MessageResult { Code = errorCode, Message = retMsg, ObjectId = 0 };
                        }
                    }
                    else // action = edit
                    {
                        if (currentItem.ApprovalStatus.ToLower().Equals("false") || currentItem.ApprovalStatus.ToLower().Equals("true"))
                        {
                            breakFunc = true;
                            string retMsg = GetRequestStatusMessage(currentItem);
                            return new MessageResult { Code = (int)OverTimeErrorCode.CannotSubmit, Message = retMsg, ObjectId = 0 };
                        }
                        else if (currentItem.FirstApprovedDate != null && currentItem.FirstApprovedDate != DateTime.MinValue && currentItem.ApprovalStatus == "")
                        {
                            breakFunc = true;
                            string retMsg = WebPageResourceHelper.GetResourceString("RequestStatusInProgress");
                            return new MessageResult { Code = (int)OverTimeErrorCode.CannotSubmit, Message = retMsg, ObjectId = 0 };
                        }
                    }

                    string requestExpiredMsg = MessageResultHelper.GetRequestExpiredMessage(currentItem.RequestDueDate);
                    if (!string.IsNullOrEmpty(requestExpiredMsg))
                    {
                        breakFunc = true;
                        return new MessageResult { Code = (int)OverTimeErrorCode.CannotApprove, Message = requestExpiredMsg, ObjectId = 0 };
                    }

                    overtimeEntity.FirstApprovedDate = currentItem.FirstApprovedDate;
                    if (!string.IsNullOrEmpty(currentItem.ApprovalStatus.Trim()))
                    {
                        breakFunc = true;
                        return new MessageResult
                        {
                            Code = 0,
                            Message = "Success",
                            ObjectId = overtimeManagementId
                        };
                    }

                    // Update Approver
                    if (!string.IsNullOrEmpty(overTimeModel.ApprovalStatus))
                    {
                        if (!overTimeModel.RequiredBODApprove) // 1 STEP
                            overtimeEntity.FirstApprovedDate = DateTime.Now;
                        else // 2 STEPs
                        {
                            if (overTimeModel.ApprovalStatus == "true") // BOD Approve
                            {
                                overtimeEntity.DHComments = currentItem.DHComments;
                            }
                            else if (overTimeModel.ApprovalStatus == "false") // DH/BOD Reject
                            {
                                if (currentItem.FirstApprovedBy != null && currentItem.FirstApprovedBy.ID > 0) // BOD Reject
                                {
                                    overtimeEntity.DHComments = currentItem.DHComments;
                                }
                                else // DH Reject
                                {
                                    overtimeEntity.FirstApprovedDate = DateTime.Now;
                                }
                            }
                        }
                    }
                    else // DH Approve, Waiting BOD
                    {
                        overtimeEntity.FirstApprovedDate = DateTime.Now;
                    }
                }
                else
                {
                    overtimeEntity = _overTimeManagementDAL.SetDueDate(overtimeEntity);
                }

                overtimeManagementId = _overTimeManagementDAL.SaveOrUpdate(spWeb, overtimeEntity);
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Overtime Service - UpdateShiftManagement fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));

            }
            finally
            {
                if (overtimeManagementId > 0 && breakFunc == false)
                {
                    var overtimeManagementDetailAddNewList = new List<OverTimeManagementDetail>();
                    foreach (var detail in overTimeModel.OvertimeDetailModelList)
                    {
                        var overTimeManagementDetail = new OverTimeManagementDetail();
                        overTimeManagementDetail.Employee.LookupId = detail.Employee.LookupId;
                        overTimeManagementDetail.OvertimeFrom = Convert.ToDateTime(detail.OvertimeFrom);
                        overTimeManagementDetail.OvertimeManagementID.LookupId = overtimeManagementId;
                        overTimeManagementDetail.ApprovalStatus.LookupId = detail.OvertimeMgmtApprovalStatus.LookupId;
                        overTimeManagementDetail.Task = detail.Task;
                        overTimeManagementDetail.OvertimeTo = Convert.ToDateTime(detail.OvertimeTo);
                        overTimeManagementDetail.CompanyTransport = detail.CompanyTransport;
                        overTimeManagementDetail.WorkingHours = string.IsNullOrEmpty(detail.WorkingHours) ? 0.0 : double.Parse(detail.WorkingHours);
                        overTimeManagementDetail.ID = detail.ID;
                        overTimeManagementDetail.BatchCommand = EntityBase.BatchCmd.Save;

                        overtimeManagementDetailAddNewList.Add(overTimeManagementDetail);
                    }

                    if (overtimeManagementDetailAddNewList.Count > 0)
                    {
                        if (autoovertime == false)
                        {
                            SPQuery spQuery = new SPQuery()
                            {
                                Query = $"<Where><Eq><FieldRef Name='OvertimeManagementID'/><Value Type='Lookup' LookupId='TRUE'>{overtimeManagementId}</Value></Eq></Where>"
                            };
                            List<OverTimeManagementDetail> details = _overTimeManagementDetailDAL.GetByQuery(spQuery, new string[] { "ID" });
                            _overTimeManagementDetailDAL.DeleteBatch(details);
                        }

                        _overTimeManagementDetailDAL.BulkInsert(overtimeManagementDetailAddNewList);
                    }

                    // Create New -> SendEmailToDepartmentHead
                    overtimeEntity = _overTimeManagementDAL.GetByID(spWeb, overtimeManagementId);
                    if (createNew)
                    {
                        _overTimeManagementDAL.SendEmailToApprover(SPContext.Current.Web, overtimeEntity, (int)StringConstant.EmployeePosition.DepartmentHead);
                        _overTimeManagementDAL.SendEmailToDelegatedApprover(SPContext.Current.Web, overtimeEntity);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(overTimeModel.ApprovalStatus) && overTimeModel.ApprovalStatus.Equals("true")) // Approve
                        {
                            _overTimeManagementDAL.Approve(overtimeManagementId, overTimeModel.ApproverFullName);
                        }
                        else if (!string.IsNullOrEmpty(overTimeModel.ApprovalStatus) && overTimeModel.ApprovalStatus.Equals("false")) // Reject
                        {
                            _overTimeManagementDAL.Reject(overtimeManagementId, overTimeModel.ApproverFullName);
                        }
                        else if (string.IsNullOrEmpty(overTimeModel.ApprovalStatus) && overTimeModel.RequiredBODApprove)
                        {
                            _overTimeManagementDAL.SendEmailToApprover(SPContext.Current.Web, overtimeEntity, (int)StringConstant.EmployeePosition.BOD);
                            _overTimeManagementDAL.SendEmailToDelegatedApprover(SPContext.Current.Web, overtimeEntity);
                        }
                    }
                }

            }

            return new MessageResult
            {
                Code = 0,
                Message = "Success",
                ObjectId = overtimeManagementId
            };
        }

        /// <summary>
        /// Get overtime in date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        /// CALL URL:  _vti_bin/Services/Overtime/OvertimeService.svc/GetOvertimeDetailByDate/4/3-24-2017
        public OvertimeEmployeeInDateModel GetOvertimeDetailByDate(string employeeLookupId, string date)
        {
            OvertimeEmployeeInDateModel overtimeData = new OvertimeEmployeeInDateModel { IsHasValue = false };
            try
            {
                int employeeLookupIdValue;
                var fromDateValue = date.ToMMDDYYYYDate(false); // mm-dd-yyyy
                var toDateValue = date.ToMMDDYYYYDate(true);

                if (fromDateValue != DateTime.MinValue && int.TryParse(employeeLookupId, out employeeLookupIdValue))
                {
                    var overtimeDetail = _overTimeManagementDetailDAL.GetForEmployeeByDateRange(employeeLookupIdValue, fromDateValue, toDateValue);
                    var firstOvertime = overtimeDetail.FirstOrDefault(x => (x.ApprovalStatus != null && Convert.ToString(x.ApprovalStatus.LookupValue).ToLower().Equals("true")));
                    if (firstOvertime != null)
                    {

                        overtimeData.FromHour = firstOvertime.OvertimeFrom.Hour;
                        overtimeData.FromMinute = firstOvertime.OvertimeFrom.Minute;

                        overtimeData.ToHour = firstOvertime.OvertimeTo.Hour;
                        overtimeData.ToMinute = firstOvertime.OvertimeTo.Minute;
                        overtimeData.HourPerDay = firstOvertime.WorkingHours;

                        overtimeData.IsHasValue = true;
                    }
                }
                return overtimeData;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Overtime Service - GetOvertimeDetailByDate fn",
                     TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                 string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public bool IsNotOvertimeExist(string employeeId, string date)
        {
            try
            {
                var fromDateValue = date.ToMMDDYYYYDate(false); // mm-dd-yyyy
                //get not overtime item with status null or approved
                var overtimeItem = _notOvertimeManagementDal.GetByDate(Convert.ToInt32(employeeId), fromDateValue);
                if (overtimeItem != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Overtime Service - IsNotOvertimeExist fn",
                      TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                  string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return true;
            }
        }

        public bool IsDelegated(string fromApproverId, string itemId)
        {
            var approverInfo = _employeeInfoDAL.GetByADAccount(Convert.ToInt32(fromApproverId));
            return DelegationPermissionManager.IsDelegation(approverInfo.ID, OverTimeManagementList.ListUrl, Convert.ToInt32(itemId)) != null;
        }
    }
}