using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.FormTemplates;
using System;
using System.IO;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.Report.RecruitmentModule
{
    /// <summary>
    /// RecruitmentTemplate
    /// </summary>
    public class RecruitmentTemplate : WordTemplate
    {
        #region Constants
        /// <summary>
        /// ;#
        /// </summary>
        public const string ChoiceValueSpliter = ";#";

        /// <summary>
        /// Shared Documents
        /// </summary>
        private const string FormTemplates = "Shared Documents";

        /// <summary>
        /// RecruitmentForm.docx
        /// </summary>
        private const string TemplateName = "RecruitmentForm.docx";

        /// <summary>
        /// Lists/Forms
        /// </summary>
        private const string Forms = "Lists/Forms";

        /// <summary>
        /// Recruitments
        /// </summary>
        private const string Folder = "Recruitments";

        /// <summary>
        /// LanguageSkills
        /// </summary>
        public const string TableName = "LanguageSkills";
        #endregion

        #region Attributes
        private RecruitmentFormInfo recruitmentFormInfoObject;
        private string[] tableColumnName = { "Ngoại ngữ/ Foreign language", "Trình độ/ Level", "Trình độ khác/ Other level" };
        private System.Collections.Generic.List<RecruitmentLanguageSkillInfo> recruitmentLanguageSkillInfoList;
        #endregion

        #region Constructor
        public RecruitmentTemplate(SPWeb currentWeb, SPListItem recruitmentItem) : base(currentWeb)
        {
            recruitmentFormInfoObject = new RecruitmentFormInfo(recruitmentItem);

            RecruitmentLanguageSkillsDAL recruitmentLanguageSkillsDAL = new RecruitmentLanguageSkillsDAL(currentWeb.Url);
            string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                         </Eq>
                                                        </Where>", RecruitmentLanguageSkillsList.Fields.Request, recruitmentItem.ID);
            var recruitmentLanguageSkillItems = recruitmentLanguageSkillsDAL.GetByQuery(queryString);
            if (recruitmentLanguageSkillItems != null && recruitmentLanguageSkillItems.Count > 0)
            {
                ForeignLanguageDAL foreignLanguageDAL = new ForeignLanguageDAL(currentWeb.Url);
                recruitmentLanguageSkillInfoList = new System.Collections.Generic.List<RecruitmentLanguageSkillInfo>();
                foreach(var recruitmentLanguageSkillItem in recruitmentLanguageSkillItems)
                {
                    RecruitmentLanguageSkillInfo recruitmentLanguageSkillInfo = new RecruitmentLanguageSkillInfo(recruitmentLanguageSkillItem, foreignLanguageDAL);
                    recruitmentLanguageSkillInfoList.Add(recruitmentLanguageSkillInfo);
                }
            }
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
        private string BuildUrlOfFile()
        {
            string fileName = string.Format("Req {0}.docx", this.recruitmentFormInfoObject.ID);
            if (!string.IsNullOrEmpty(recruitmentFormInfoObject.Title))
            {
                fileName = string.Format("{0}.docx", this.recruitmentFormInfoObject.Title);
            }

            return string.Format("{0}/{1}/{2}/{3}", this.CurrentWeb.Url, Forms, Folder, fileName);
        }

        /// <summary>
        /// FillMasterInfo
        /// </summary>
        /// <param name="arrBytesSource"></param>
        /// <returns></returns>
        private Stream FillMasterInfo(byte[] arrBytesSource)
        {
            return this.FillDataObject(this.recruitmentFormInfoObject, arrBytesSource);
        }

        private void FillDetailsInfo(Stream stream)
        {
            if (this.recruitmentLanguageSkillInfoList != null && this.recruitmentLanguageSkillInfoList.Count > 0)
            {
                this.FillDataListOfObject(recruitmentLanguageSkillInfoList, stream, TableName, tableColumnName);
            }
        }

        #endregion
    }
}
