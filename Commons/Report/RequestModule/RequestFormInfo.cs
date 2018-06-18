using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestFormInfo
    /// </summary>
    public class RequestFormInfo
    {
        #region Properties
        public string ID { get; set; }

        public string Title { get; set; }

        public string RequestedBy { get; set; }

        public string RequestType { get; set; }

        public string ReceivedBy { get; set; }
        #endregion

        #region Constructors
        public RequestFormInfo()
        {
        }

        /// <summary>
        /// RequestFormInfo
        /// </summary>
        /// <param name="requestItem">The Request SPListItem object.</param>
        public RequestFormInfo(SPListItem requestItem)
        {
            if (requestItem != null)
            {
                this.ID = requestItem.ID.ToString();
                this.Title = requestItem.Title;
                
                #region TFS: 1943
                SPFieldLookupValue department = ObjectHelper.GetSPFieldLookupValue(requestItem[StringConstant.RequestsList.CommonDepartmentField]);
                if (department != null)
                {
                    var departmentObj = DepartmentListSingleton.GetDepartmentByID(department.LookupId, requestItem.ParentList.ParentWebUrl);
                    this.RequestedBy = string.Format("{0} / {1}", departmentObj.VietnameseName, departmentObj.Name);
                }
                #endregion TFS: 1943

                SPFieldLookupValue requestType = ObjectHelper.GetSPFieldLookupValue(requestItem[StringConstant.RequestsList.RequestTypeRefField]);
                if (requestType != null)
                {
                    this.RequestType = requestType.LookupValue;
                }

                SPFieldLookupValue receivedBy = ObjectHelper.GetSPFieldLookupValue(requestItem[StringConstant.RequestsList.ReceviedByField]);
                if (receivedBy != null)
                {
                    var receivedDepartmentObj = DepartmentListSingleton.GetDepartmentByID(receivedBy.LookupId, requestItem.ParentList.ParentWebUrl);
                    this.ReceivedBy = string.Format("{0} / {1}", receivedDepartmentObj.VietnameseName, receivedDepartmentObj.Name);
                }
            }
        }
        #endregion
    }
}
