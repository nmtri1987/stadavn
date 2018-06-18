using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.IO;

namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestRepairTemplate
    /// </summary>
    public class RequestRepairTemplate : RequestTemplate
    {
        #region Properties
        protected List<RequestRepairDetailInfo> requestRepairDetailInfoList;
        private string[] tableColumnName = { "STT/ No.", "Nội dung/ Content", "Lý do sửa chữa/ Reason(s)", "Địa điểm/ Place", "Từ ngày…../ From…..", "Đến ngày…../To…." };
        #endregion

        #region Constructors
        public RequestRepairTemplate(SPWeb currentWeb, SPListItem requestItem) : base(currentWeb, requestItem)
        {
            requestRepairDetailInfoList = new List<RequestRepairDetailInfo>();

            #region Query
            string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", StringConstant.RequestRepairDetailsList.Fields.Request, requestItem.ID);
            #endregion

            RequestRepairDetailsDAL requestRepairDetailsDAL = new RequestRepairDetailsDAL(this.CurrentWeb.Url);
            List<RequestRepairDetails> requestRepairDetailsItems = requestRepairDetailsDAL.GetByQuery(queryString);
            if (requestRepairDetailsItems != null && requestRepairDetailsItems.Count > 0)
            {
                for (int i = 0; i < requestRepairDetailsItems.Count; i++)
                {
                    RequestRepairDetails requestRepairDetailsItem = requestRepairDetailsItems[i];
                    RequestRepairDetailInfo requestRepairDetailInfo = new RequestRepairDetailInfo(requestRepairDetailsItem);
                    requestRepairDetailInfo.No = (i + 1).ToString();
                    requestRepairDetailInfoList.Add(requestRepairDetailInfo);
                }
            }
        }
        #endregion

        #region Override
        public override void FillDetailsInfo(Stream stream)
        {
            this.FillDataListOfObject(this.requestRepairDetailInfoList, stream, TableName, tableColumnName);
        }
        #endregion
    }
}
