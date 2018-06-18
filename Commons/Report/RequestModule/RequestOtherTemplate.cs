using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.IO;

namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestOtherTemplate
    /// </summary>
    public class RequestOtherTemplate : RequestTemplate
    {
        #region Properties
        protected List<RequestOtherDetailInfo> requestOtherDetailInfoList;
        private string[] tableColumnName = { "STT/ No.", "Nội dung/ Content", "ĐVT/ Unit", "Số lượng/ Quant.", "Lý do/ Reason(s)" };
        #endregion

        #region Constructors
        /// <summary>
        /// RequestOtherTemplate
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="requestItem"></param>
        public RequestOtherTemplate(SPWeb currentWeb, SPListItem requestItem) : base(currentWeb, requestItem)
        {
            requestOtherDetailInfoList = new List<RequestOtherDetailInfo>();

            #region Query
            string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", StringConstant.RequestOtherDetailsList.Fields.Request, requestItem.ID);
            #endregion

            RequestOtherDetailsDAL requestOtherDetailsDAL = new RequestOtherDetailsDAL(this.CurrentWeb.Url);

            List<RequestOtherDetails> requestOtherDetailsItems = requestOtherDetailsDAL.GetByQuery(queryString);
            if (requestOtherDetailsItems != null && requestOtherDetailsItems.Count > 0)
            {
                for (int i = 0; i < requestOtherDetailsItems.Count; i++)
                {
                    RequestOtherDetails requestOtherDetailsItem = requestOtherDetailsItems[i];
                    RequestOtherDetailInfo requestOtherDetailInfo = new RequestOtherDetailInfo(requestOtherDetailsItem);
                    requestOtherDetailInfo.No = (i + 1).ToString();
                    requestOtherDetailInfoList.Add(requestOtherDetailInfo);
                }
            }
        }
        #endregion

        #region Override
        public override void FillDetailsInfo(Stream stream)
        {
            this.FillDataListOfObject(this.requestOtherDetailInfoList, stream, TableName, tableColumnName);
        }
        #endregion
    }
}
