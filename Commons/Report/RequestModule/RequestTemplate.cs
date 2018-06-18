using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.FormTemplates;
using System;
using System.IO;

namespace RBVH.Stada.Intranet.Biz.Report.RequestModule
{
    /// <summary>
    /// RequestTemplate
    /// </summary>
    public class RequestTemplate : WordTemplate
    {
        #region Constants
        /// <summary>
        /// Shared Documents
        /// </summary>
        private const string FormTemplates = "Shared Documents";
        private const string TemplateName = "RequestForm.docx";
        private const string Forms = "Lists/Forms";
        private const string Folder = "Requests";
        public const string TableName = "RequestDetailList";
        #endregion

        #region Properties
        protected RequestFormInfo requestFormInfoObject;
        #endregion

        #region Constructors
        public RequestTemplate(SPWeb currentWeb, SPListItem requestItem) : base(currentWeb)
        {
            this.requestFormInfoObject = new RequestFormInfo(requestItem);
        }
        #endregion

        #region Methods

        /// <summary>
        /// ExportFormData
        /// </summary>
        /// <returns></returns>
        public string ExportFormData()
        {
            string urlOfFile = string.Empty;

            // Step 1: Lấy nội dung file template.
            string urlOfFileTemplate = string.Format("{0}/{1}/{2}", this.CurrentWeb.Url, FormTemplates, TemplateName);
            SPFile fileTemplate = this.GetFile(urlOfFileTemplate);

            // Step 2: Copy file template qua url mới. ->File Data
            urlOfFile = BuildUrlOfFile();
            this.CopyTo(fileTemplate, urlOfFile, true);

            // Step 3: Mở file data
            SPFile fileData = this.GetFile(urlOfFile);

            // Step 4: Fill data
            if (fileData != null && fileData.Exists)
            {
                // Get stream
                Stream stream = null;
                byte[] arrBytesSource = this.GetContentFile(fileData);

                try
                {
                    // Fill master info into stream
                    stream = this.FillMasterInfo(arrBytesSource);
                    // File details info into stream
                    this.FillDetailsInfo(stream);

                    // Update file data
                    this.UploadFile(urlOfFile, stream);
                }
                catch (Exception ex)
                {
                    ULSLogging.LogError(ex);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                }
            }

            return urlOfFile;
        }

        /// <summary>
        /// BuildUrlOfFile
        /// </summary>
        /// <returns></returns>
        public string BuildUrlOfFile()
        {
            //string fileName = string.Format("Req {0}.docx", this.requestFormInfoObject.ID);
            //if (!string.IsNullOrEmpty(requestFormInfoObject.Title))
            //{
            //    fileName = string.Format("{0}.docx", this.requestFormInfoObject.Title);
            //}
            // TFS #1995: [29.01.2018][Phiếu đề nghị] In ra không có dữ liệu, em có chụp hình
            string fileName = string.Format("{0}.docx", this.requestFormInfoObject.ID);

            return string.Format("{0}/{1}/{2}/{3}", this.CurrentWeb.Url, Forms, Folder, fileName);
        }

        /// <summary>
        /// FillMasterInfo
        /// </summary>
        /// <param name="arrBytesSource"></param>
        /// <returns></returns>
        private Stream FillMasterInfo(byte[] arrBytesSource)
        {
            return this.FillDataObject(this.requestFormInfoObject, arrBytesSource);
        }

        /// <summary>
        /// FillDetailsInfo
        /// </summary>
        /// <param name="stream"></param>
        public virtual void FillDetailsInfo(Stream stream)
        {
            // Do nothing. It will be peformance by child class.
        }

        #endregion
    }
}
