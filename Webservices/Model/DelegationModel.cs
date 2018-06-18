using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class DelegationModel
    {
        public string ModuleName { get; set; }
        public string VietnameseModuleName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public User FromEmployee { get; set; }
        public List<LookupItem> ToEmployee { get; set; }
        public LookupItem Requester { get; set; }
        public LookupItem Department { get; set; }

        public DelegationModel()
        {
            FromEmployee = new User();
            ToEmployee = new List<LookupItem>();
            Requester = new LookupItem();
            Department = new LookupItem();
        }

        public DelegationModel(Delegation delegation) : this()
        {
            if (delegation != null)
            {
                ModuleName = delegation.ModuleName;
                VietnameseModuleName = delegation.VietnameseModuleName;
                FromDate = delegation.FromDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                ToDate = delegation.ToDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                ToEmployee = delegation.ToEmployee;
                Requester = delegation.Requester;
                Department = delegation.Department;

                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(SPContext.Current.Site.Url);
                FromEmployee = _employeeInfoDAL.GetByID( delegation.FromEmployee.LookupId).ADAccount;
            }
        }
    }
}
