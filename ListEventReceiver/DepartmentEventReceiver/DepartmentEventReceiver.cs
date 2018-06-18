//-----------------------------------------------------------------------
// <copyright file="DepartmentEventReceiver.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the DepartmentEventReceiver class.</summary>
//-----------------------------------------------------------------------

namespace RBVH.Stada.Intranet.ListEventReceiver.DepartmentEventReceiver
{
    using Microsoft.SharePoint;
    using Biz.DataAccessLayer;
    using System;
    using Microsoft.SharePoint.Administration;
    using Core.SharePoint;
    using System.Globalization;
    using Biz.Constants;

    /// <summary>
    /// List Item Events
    /// </summary>
    public class DepartmentEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item is being added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            try
            {
                SPListItem curItem = properties.ListItem;

                var departmentRepo = new DepartmentDAL(properties.Site.Url);
                departmentRepo.CreateDepartmentSite(properties.SiteId, properties.Web.ID, curItem);

                // Add bod approver to BOD group
                var userBodString = Convert.ToString(properties.ListItem[StringConstant.DepartmentList.BODField]);
                if (!string.IsNullOrEmpty(userBodString))
                {
                    var employeeInfoRepo = new EmployeeInfoDAL(properties.Site.Url);
                    employeeInfoRepo.AddUserToGroup(properties.Site.ID, properties.Web.ID, curItem, StringConstant.Group.BODGroupName, userBodString);
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Event Receiver - ItemAdded fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }


        /// <summary>
        /// An item is updating.
        /// </summary>
        /// <param name="properties"></param>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);
            try
            {
                string currentNameValue = Convert.ToString(properties.ListItem[StringConstant.DepartmentList.NameField]);
                string newName = Convert.ToString(properties.AfterProperties[StringConstant.DepartmentList.NameField]);

                string currentVnNameValue = Convert.ToString(properties.ListItem[StringConstant.DepartmentList.VietnameseNameField]);
                string newVnName = Convert.ToString(properties.AfterProperties[StringConstant.DepartmentList.VietnameseNameField]);

                //Update sub site title if changed
                if (!currentNameValue.Equals(newName) || !currentVnNameValue.Equals(newVnName))
                {
                    var departmentRepo = new DepartmentDAL(properties.Site.Url);
                    SPListItem curItem = properties.ListItem;
                    departmentRepo.UpdateSubSiteTitle(properties.SiteId, properties.Web.ID, curItem, newName, newVnName);
                }
                // Update bod approver to BOD group
                var bodUserString = Convert.ToString(properties.AfterProperties[StringConstant.DepartmentList.BODField]);

                if (!string.IsNullOrEmpty(bodUserString))
                {
                    SPListItem curItem = properties.ListItem;

                    var employeeInfoRepo = new EmployeeInfoDAL(properties.Site.Url);
                    employeeInfoRepo.AddUserToGroup(properties.Site.ID, properties.Web.ID, curItem, StringConstant.Group.BODGroupName, bodUserString);
                }
                DepartmentListSingleton.ResetDepartmentListInstance();
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Event Receiver - ItemUpdating fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// An item is deleting.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            try
            {
                SPListItem curItem = properties.ListItem;
                var departmentRepo = new DepartmentDAL(properties.Site.Url);
                var department = departmentRepo.ParseToEntity(curItem);
                //delete Site
                departmentRepo.DeleteSite(properties.SiteId, properties.Web.ID, department);
                DepartmentListSingleton.ResetDepartmentListInstance();
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Event Receiver - ItemDeleting fn",
                       TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                   string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }


            base.ItemDeleted(properties);
        }


    }
}