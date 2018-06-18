using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace RBVH.Stada.Intranet.WebPages.Common
{
    /// <summary>
    /// FormBaseUserControl class.
    /// </summary>
    public class FormBaseUserControl : UserControl, IValidator
    {
        #region Constants
        /// <summary>
        /// Access Denied
        /// </summary>
        public const string AccessDenied = "Access Denied";
        #endregion

        #region Attributes
        private SPContext currentContext;
        private SPWeb currentWeb;
        private SPList currentList;
        private SPListItem currentItem;
        private SPUser currentUser;
        private int itemID;
        private string listName;
        private string siteUrl;
        #endregion

        #region Properties
        /// <summary>
        /// Get current context object.
        /// </summary>
        public SPContext CurrentContext
        {
            get
            {
                return currentContext;
            }
        }

        /// <summary>
        /// Get current web object.
        /// </summary>
        public SPWeb CurrentWeb
        {
            get
            {
                return currentWeb;
            }
        }

        /// <summary>
        /// Get current list object.
        /// </summary>
        public SPList CurrentList
        {
            get
            {
                return currentList;
            }
        }

        /// <summary>
        /// Get current item object.
        /// </summary>
        public SPListItem CurrentItem
        {
            get
            {
                return currentItem;
            }
        }

        /// <summary>
        /// Get current user object.
        /// </summary>
        public SPUser CurrentUser
        {
            get
            {
                return currentUser;
            }
        }

        /// <summary>
        /// Gets or sets the control mode of the form.
        /// </summary>
        public SPControlMode CurrentFormMode
        {
            get
            {
                string mode = Page.Request[UrlParamName.FormModeParamName];
                if (string.Compare(mode, FormMode.NewMode, true) == 0)
                {
                    return SPControlMode.New;
                }
                else if (string.Compare(mode, FormMode.EditMode, true) == 0)
                {
                    return SPControlMode.Edit;
                }
                else if (string.Compare(mode, FormMode.DisplayMode, true) == 0)
                {
                    return SPControlMode.Display;
                }
                else
                {
                    return SPControlMode.Invalid;
                }
            }
        }

        /// <summary>
        /// ID of current item.
        /// </summary>
        public int ItemID
        {
            get
            {
                return this.itemID;
            }
        }

        /// <summary>
        /// Get in English Name of current list. Always return in English Name. Its independence on current page language.
        /// </summary>
        public string ListName
        {
            get
            {
                return listName;
            }
        }

        public string SiteUrl
        {
            get
            {
                return siteUrl;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public FormBaseUserControl()
        {
        }
        #endregion

        #region Overrides
        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                // Inits
                Inits();

                this.IsValid = true;
                if (CurrentFormMode == SPControlMode.New || CurrentFormMode == SPControlMode.Edit)
                {
                    CurrentContext.FormContext.OnSaveHandler += SaveFormHandler;
                    this.Page.Validators.Add(this);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);

                // Used for submitting forms that contain files, non-ASCII data, and binary data.
                this.Page.Form.Enctype = "multipart/form-data";

                if (CurrentFormMode == SPControlMode.Display)
                {
                    if (!HavePermissionToView())
                    {
                        SPUtility.HandleAccessDenied(new Exception(FormBaseUserControl.AccessDenied));
                    }
                }
                else if (CurrentFormMode == SPControlMode.New)
                {
                    if (!HavePermissionToAddNew())
                    {
                        SPUtility.HandleAccessDenied(new Exception(FormBaseUserControl.AccessDenied));
                    }
                }
                else if (CurrentFormMode == SPControlMode.Edit)
                {
                    if (!HavePermissionToEdit())
                    {
                        SPUtility.HandleAccessDenied(new Exception(FormBaseUserControl.AccessDenied));
                    }
                }

                DisplayControls();
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// OnPreRender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                throw ex;
            }
        }
        #endregion

        #region Validator
        public string ErrorMessage
        {
            get;
            set;
        }

        public bool IsValid
        {
            get;
            set;
        }

        public virtual void Validate()
        {
            if (!IsValid)
            {
                CloseWaitingDialog();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// HavePermissionToView
        /// </summary>
        public virtual bool HavePermissionToView()
        {
            return true;
        }

        /// <summary>
        /// HavePermissionToAddNew
        /// </summary>
        public virtual bool HavePermissionToAddNew()
        {
            return true;
        }

        /// <summary>
        /// HavePermissionToEdit
        /// </summary>
        public virtual bool HavePermissionToEdit()
        {
            return true;
        }

        /// <summary>
        /// SaveForm
        /// </summary>
        /// <returns></returns>
        protected virtual bool SaveForm()
        {
            currentItem.Update();
            return true;
        }

        /// <summary>
        /// Inits some attributes.
        /// </summary>
        private void Inits()
        {
            currentContext = SPContext.Current;
            currentWeb = SPContext.Current.Web;
            currentList = SPContext.Current.List;
            currentUser = SPContext.Current.Web.CurrentUser;
            string itemIDRequest = Page.Request[UrlParamName.IDParamName];
            int.TryParse(itemIDRequest, out itemID);
            #region Fix bug get title is wrong when switch language.
            //listName = this.currentList.Title;
            // Always get static name of list (In English Name)
            listName = this.currentList.TitleResource.GetValueForUICulture(System.Globalization.CultureInfo.GetCultureInfo(PageLanguages.English));
            #endregion
            siteUrl = currentWeb.Site.Url;

            if (CurrentFormMode == SPControlMode.New)
            {
                currentItem = currentList.AddItem();
            }
            else if (CurrentFormMode == SPControlMode.Edit || CurrentFormMode == SPControlMode.Display)
            {
                if (currentContext.ListItem != null)
                {
                    this.currentItem = SPContext.Current.ListItem;
                }
                else if (itemID > 0)
                {
                    this.currentItem = this.currentList.GetItemById(itemID);
                }
            }
        }

        /// <summary>
        /// DisplayControls
        /// </summary>
        protected virtual void DisplayControls()
        {

        }

        /// <summary>
        /// SaveFormHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveFormHandler(object sender, EventArgs e)
        {
            if (SaveForm())
            {
                CloseForm(sender);
            }
        }

        /// <summary>
        /// CloseForm
        /// </summary>
        /// <param name="sender">The asp button object.</param>
        protected void CloseForm(object sender)
        {
            //Button button = sender as Button;

            #region Close form
            // Open dialog
            if (string.Compare(Request.QueryString["IsDlg"], "1") == 0)
            {
                string script = @"if (window.parent.waitDialog != null) {
                                        window.parent.waitDialog.close();
                                    }
                                    SP.UI.ModalDialog.commonModalDialogClose(1, 0);
                                ";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "PopupScript", script, true);
            }
            else // Open new tab
            {
                //string returlUrl = Page.Request[UrlParamName.ReturnUrlParamName];
                //if (!string.IsNullOrEmpty(returlUrl))
                //{
                //    SPUtility.Redirect(returlUrl, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
                //}
                //else
                //{
                //    SPUtility.Redirect(CurrentList.DefaultViewUrl, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
                //}

                string returnlUrl = string.Empty;
                StringBuilder returnUrlBuilder = new StringBuilder();
                UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url);
                var sourceValues = uriBuilder.GetQueryValues(UrlParamName.ReturnUrlParamName);
                if (sourceValues != null && sourceValues.Count > 0)
                {
                    var sourceValueArray = sourceValues.ElementAt(0).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (sourceValueArray != null && sourceValueArray.Length > 0)
                    {
                        if (sourceValueArray.Length == 1)
                        {
                            returnUrlBuilder.Append(sourceValueArray[0]);
                        }
                        else
                        {
                            for (int i = 0; i < sourceValueArray.Length; i++)
                            {
                                if (i == 0)
                                {
                                    returnUrlBuilder.AppendFormat("{0}?", (sourceValueArray[i]));
                                }
                                else if (i == sourceValueArray.Length - 1)
                                {
                                    returnUrlBuilder.AppendFormat("{0}", (sourceValueArray[i]));
                                }
                                else
                                {
                                    returnUrlBuilder.AppendFormat("{0}&", (sourceValueArray[i]));
                                }
                            }
                        }
                    }
                }
                returnlUrl = returnUrlBuilder.ToString();
                if (!string.IsNullOrEmpty(returnlUrl))
                {
                    SPUtility.Redirect(returnlUrl, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
                }
                else
                {

                    SPUtility.Redirect(SPContext.Current.ListItem.ParentList.DefaultViewUrl, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
                }
            }
            #endregion
        }

        /// <summary>
        /// ShowClientMessage
        /// </summary>
        /// <param name="message">The content message.</param>
        protected void ShowClientMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), this.ClientID, string.Format("alert('{0}');", message), true);
        }

        /// <summary>
        /// CloseWaitingDialog
        /// </summary>
        protected void CloseWaitingDialog(string additionalScripts = "")
        {
            string script = @"
                            if (window.parent.waitDialog != null) {
                                window.parent.waitDialog.close();"
                + additionalScripts +
                        @"
                            }
                        ";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "closeWaitingDialog", script, true);
        }
        #endregion
    }
}
