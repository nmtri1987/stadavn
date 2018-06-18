using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System.ServiceModel.Activation;
using RBVH.Stada.Intranet.Webservices.Model;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.Linq;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.OverviewManagement;
using System.ServiceModel;
using RBVH.Stada.Intranet.Biz.Interfaces;
using System;

namespace RBVH.Stada.Intranet.Webservices.ISAPI.Services.Common
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CommonService : ICommonService
    {
        private readonly DelegationModulesDAL _delegationModulesDAL;
        private readonly DelegationsDAL _delegationDAL;
        private readonly EmployeeInfoDAL _employeeInfoDAL;
        private List<IFilterTaskManager> _listDAL;
        private string _webUrl;
        public CommonService()
        {
            _webUrl = SPContext.Current.Web.Url;
            _delegationModulesDAL = new DelegationModulesDAL(_webUrl);
            _delegationDAL = new DelegationsDAL(_webUrl);
            _employeeInfoDAL = new EmployeeInfoDAL(_webUrl);
            _listDAL = new List<IFilterTaskManager>();
            // Init DAL for Visitor
            _listDAL.Add(new ShiftManagementDAL(_webUrl));
            _listDAL.Add(new ChangeShiftManagementDAL(_webUrl));
            _listDAL.Add(new OverTimeManagementDAL(_webUrl));
            _listDAL.Add(new NotOvertimeManagementDAL(_webUrl));
            _listDAL.Add(new LeaveManagementDAL(_webUrl));
            _listDAL.Add(new VehicleManagementDAL(_webUrl));
            _listDAL.Add(new FreightManagementDAL(_webUrl));
            _listDAL.Add(new BusinessTripManagementDAL(_webUrl));
            _listDAL.Add(new RequestsDAL(_webUrl));
            _listDAL.Add(new EmployeeRequirementSheetDAL(_webUrl));
            _listDAL.Add(new RequestForDiplomaSupplyDAL(_webUrl));
            _listDAL.Add(new RequisitionOfMeetingRoomDAL(_webUrl));
        }

        public IEnumerable<ModuleModel> GetModules(string lcid)
        {
            var modules = this._delegationModulesDAL.GetAll().OrderBy(x => x.ModuleName);
            if (lcid == "1066")
            {
                modules = modules.OrderBy(x => x.VietnameseModuleName);
                return modules.Select(e => new ModuleModel { ID = e.ID, Name = e.VietnameseModuleName });
            }
            else
            {
                modules = modules.OrderBy(x => x.ModuleName);
                return modules.Select(e => new ModuleModel { ID = e.ID, Name = e.ModuleName });
            }
        }

        public IEnumerable<FilterTaskModel> GetTaskByCondition(string condition, string currentUserADId, string currentUserInfoId, string approverFullName)
        {
            int userADId;
            bool currentUserADIdResult = Int32.TryParse(currentUserADId, out userADId);
            if (currentUserADIdResult)
            {
                // Get Delegation list:
                int userInfoId;
                bool currentUserInfoIdResult = Int32.TryParse(currentUserInfoId, out userInfoId);
                List<Delegation> delegationList = _delegationDAL.GetDelegationApprovalList(SPContext.Current.Web, userInfoId);
                switch (condition)
                {
                    case TaskCondition.WaitingApproval:
                        var waitingApprovalVisitor = new WaitingApprovalVisitor(userADId, userInfoId, _webUrl);
                        waitingApprovalVisitor.DelegationList = delegationList;
                        foreach (var dal in _listDAL)
                        {
                            dal.Accept(waitingApprovalVisitor);
                        }

                        return waitingApprovalVisitor.FilterTaskList.OrderBy(i => i.CreatedDate).Select(r => FilterTaskModel.FromDTO(r));

                    case TaskCondition.InProcess:
                        var inProcessVisitor = new InProcessVisitor(userADId, userInfoId, _webUrl);
                        inProcessVisitor.DelegationList = delegationList;
                        foreach (var dal in _listDAL)
                        {
                            dal.Accept(inProcessVisitor);
                        }

                        return inProcessVisitor.FilterTaskList.OrderBy(i => i.CreatedDate).Select(r => FilterTaskModel.FromDTO(r));

                    case TaskCondition.WaitingApprovalToday:
                        var waitingApprovalTodayVisitor = new WaitingApprovalTodayVisitor(userADId, userInfoId, this._webUrl);
                        waitingApprovalTodayVisitor.DelegationList = delegationList;
                        foreach (var dal in _listDAL)
                        {
                            dal.Accept(waitingApprovalTodayVisitor);
                        }

                        return waitingApprovalTodayVisitor.FilterTaskList.Select(r => FilterTaskModel.FromDTO(r));

                    case TaskCondition.ApprovedToday:
                        var approvedTodayVisitor = new ApprovedTodayVisitor(userADId, userInfoId, _webUrl);
                        approvedTodayVisitor.ApproverFullName = approverFullName;
                        // Approved: Dont need to load Delegation
                        foreach (var dal in _listDAL)
                        {
                            dal.Accept(approvedTodayVisitor);
                        }

                        return approvedTodayVisitor.FilterTaskList.OrderBy(i => i.CreatedDate).Select(r => FilterTaskModel.FromDTO(r));

                    default:
                        return Enumerable.Empty<FilterTaskModel>();
                }
            }

            return Enumerable.Empty<FilterTaskModel>();
        }

        public TaskOverviewModel GetTaskOverview(string currentUserADId, string currentUserInfoId, string approverFullName)
        {
            int userADId;
            bool currentUserIdResult = Int32.TryParse(currentUserADId, out userADId);
            int userInfoId;
            bool currentUserInfoIdResult = Int32.TryParse(currentUserInfoId, out userInfoId);
            var totalWaitingApproval = 0;
            var totalWaitingApprovalToday = 0;
            var totalInProcess = 0;
            var totalApprovedToday = 0;
            if (currentUserIdResult)
            {
                // Get Delegation list:

                List<Delegation> delegationList = _delegationDAL.GetDelegationApprovalList(SPContext.Current.Web, userInfoId);
                // Waiting Approval
                var waitingApprovalVisitor = new WaitingApprovalVisitor(userADId, userInfoId, _webUrl);
                waitingApprovalVisitor.CountOnly = true;
                waitingApprovalVisitor.DelegationList = delegationList;
                // Waiting Approval TODAY
                var waitingApprovalTodayVisitor = new WaitingApprovalTodayVisitor(userADId, userInfoId, this._webUrl);
                waitingApprovalTodayVisitor.CountOnly = true;
                waitingApprovalTodayVisitor.DelegationList = delegationList;
                // In-Process
                var inProcessVisitor = new InProcessVisitor(userADId, userInfoId, _webUrl);
                inProcessVisitor.CountOnly = true;
                inProcessVisitor.DelegationList = delegationList;
                // Approved TODAY -> Dont need to load Delegation
                var approvedTodayVisitor = new ApprovedTodayVisitor(userADId, userInfoId, _webUrl);
                approvedTodayVisitor.CountOnly = true;
                approvedTodayVisitor.ApproverFullName = approverFullName;

                foreach (var dal in _listDAL)
                {
                    dal.Accept(waitingApprovalVisitor);
                    dal.Accept(waitingApprovalTodayVisitor);
                    dal.Accept(inProcessVisitor);
                    dal.Accept(approvedTodayVisitor);
                }

                totalWaitingApproval = waitingApprovalVisitor.TotalCount;
                totalWaitingApprovalToday = waitingApprovalTodayVisitor.TotalCount;
                totalInProcess = inProcessVisitor.TotalCount;
                totalApprovedToday = approvedTodayVisitor.TotalCount;
            }

            return new TaskOverviewModel
            {
                CurrentUserADId = userADId,
                CurrentUserId = userInfoId,
                TotalWaitingApproval = totalWaitingApproval,
                TotalWaitingApprovalToday = totalWaitingApprovalToday,
                TotalInProcess = totalInProcess,
                TotalApprovedToday = totalApprovedToday
            };
        }

        public MessageResult GetModifiedDate(string moduleId, string itemId)
        {
            string ret = string.Empty;

            try
            {
                SPWeb currentWeb = SPContext.Current.Web;
                int moduleIdendify = 0;
                int itemIdentify = 0;
                if (int.TryParse(moduleId, out moduleIdendify) && int.TryParse(itemId, out itemIdentify))
                {
                    SPListItemCollection itemCollection = null;
                    SPQuery spQuery = new SPQuery()
                    {
                        Query = $"<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>{itemIdentify}</Value></Eq></Where>",
                        RowLimit = 1
                    };

                    switch (moduleIdendify)
                    {
                        case (int)StepModuleList.ShiftManagement:
                            ShiftManagementDAL _shiftManagementDAL = new ShiftManagementDAL(currentWeb.Url);
                            itemCollection = _shiftManagementDAL.GetByQueryToSPListItemCollection(currentWeb, spQuery, new string[] { StringConstant.DefaultSPListField.ModifiedField });
                            break;
                        case (int)StepModuleList.OvertimeManagement:
                            OverTimeManagementDAL _overTimeManagementDAL = new OverTimeManagementDAL(currentWeb.Url);
                            itemCollection = _overTimeManagementDAL.GetByQueryToSPListItemCollection(currentWeb, spQuery, new string[] { StringConstant.DefaultSPListField.ModifiedField });
                            break;
                        default:
                            break;
                    }

                    if (itemCollection != null && itemCollection.Count > 0)
                    {
                        DateTime modifiedField = (DateTime)itemCollection[0][StringConstant.DefaultSPListField.ModifiedField];
                        ret = modifiedField.ToString(StringConstant.DateFormatddMMyyyyHHmmss);
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageResult { Code = -1, Message = ex.Message };
            }

            return new MessageResult { Code = 0, Message = ret };
        }
    }
}
