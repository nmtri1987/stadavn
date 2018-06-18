//-----------------------------------------------------------------------
// <copyright file="DepartmentEventReceiver.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the EmployeeInfoEventReceiver class.</summary>
//-----------------------------------------------------------------------


namespace RBVH.Stada.Intranet.ListEventReceiver.EmployeeInfoEventReceiver
{
    using Microsoft.SharePoint;
    using Biz.DataAccessLayer;
    /// <summary>
    /// List Item Events
    /// </summary>
    public class EmployeeInfoEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);

            SPListItem curItem = properties.ListItem;

            var employeeInfoRepo = new EmployeeInfoDAL(properties.Site.Url);
            employeeInfoRepo.AddUserToDepartmentGroupPermission(properties.Site.ID, properties.Web.ID, curItem);
        }
        /// <summary>
        /// An item is Deleting.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);

            SPListItem curItem = properties.ListItem;

            var employeeInfoRepo = new EmployeeInfoDAL(properties.Site.Url);
            employeeInfoRepo.RemoveUserFromGroupPermission(properties.Site.ID, properties.Web.ID, curItem);
        }
        /// <summary>
        /// An item is Updating.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);

            SPListItem curItem = properties.ListItem;

            var employeeInfoRepo = new EmployeeInfoDAL(properties.Site.Url);
            employeeInfoRepo.RemoveUserFromGroupPermission(properties.Site.ID, properties.Web.ID, curItem);
        }

        /// <summary>
        /// An item was Updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);

            SPListItem curItem = properties.ListItem;
            var employeeInfoRepo = new EmployeeInfoDAL(properties.Site.Url);
            employeeInfoRepo.UpdateUserGroupPermission(properties.Site.ID, properties.Web.ID, curItem);
        }
    }
}