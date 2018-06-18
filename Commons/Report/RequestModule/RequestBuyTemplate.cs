using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.IO;

namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestBuyTemplate
    /// </summary>
    public class RequestBuyTemplate : RequestTemplate
    {
        #region Properties
        protected List<RequestBuyDetailInfo> requestBuyDetailInfoList;
        private string[] tableColumnName = { "STT/ No.", "Nội dung/ Content", "Qui cách/ Form", "ĐVT/ Unit", "Số lượng/ Quant.", "Lý do/ Reason(s)" };
        private string[] tableColumnWidth = { "800", "2712", "2712", "2712", "2712", "2712" };
        #endregion

        #region Constructors
        public RequestBuyTemplate(SPWeb currentWeb, SPListItem requestItem) : base(currentWeb, requestItem)
        {
            requestBuyDetailInfoList = new List<RequestBuyDetailInfo>();

            #region Query
            string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", StringConstant.RequestBuyDetailsList.Fields.Request, requestItem.ID);
            #endregion

            RequestBuyDetailsDAL requestBuyDetailsDAL = new RequestBuyDetailsDAL(this.CurrentWeb.Url);
            List<RequestBuyDetails> requestBuyDetailsItems = requestBuyDetailsDAL.GetByQuery(queryString);
            if (requestBuyDetailsItems != null && requestBuyDetailsItems.Count > 0)
            {
                for (int i = 0; i < requestBuyDetailsItems.Count; i++)
                {
                    RequestBuyDetails requestBuyDetailsItem = requestBuyDetailsItems[i];
                    RequestBuyDetailInfo requestBuyDetailInfo = new RequestBuyDetailInfo(requestBuyDetailsItem);
                    requestBuyDetailInfo.No = (i + 1).ToString();
                    requestBuyDetailInfoList.Add(requestBuyDetailInfo);
                }
            }
        }
        #endregion

        #region Override
        public override void FillDetailsInfo(Stream stream)
        {
            this.FillDataListOfObject(this.requestBuyDetailInfoList, stream, TableName, tableColumnName, tableColumnWidth);
        }
        #endregion
    }
}
