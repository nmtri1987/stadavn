using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Core.SharePoint;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using System.Linq;
using System.Collections.Generic;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages
{
    public partial class ReAssignTask : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadListOfEmployees();
        }

        /// <summary>
        /// LoadListOfEmployees
        /// </summary>
        private void LoadListOfEmployees()
        {
            try
            {
                EmployeeInfoDAL employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Web.Url);
                //string queryString = string.Format(@"<OrderBy>
                //                                        <FieldRef Name='{0}' Ascending='TRUE'></FieldRef>
                //               </OrderBy>", EmployeeInfoList.FullNameField);
                //this.ddlEmployee.DataSource = employeeInfoDAL.GetByQuery(queryString, "ID", EmployeeInfoList.FullNameField);
               List<EmployeeInfo> emmployees = null;
                var employeeList = employeeInfoDAL.GetAll();
                if (employeeList.Any())
                {
                    emmployees = employeeList.OrderBy(e => e.FullName).ToList();
                }
                this.ddlEmployee.DataSource = emmployees;

                this.ddlEmployee.DataBind();
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
    }
}
