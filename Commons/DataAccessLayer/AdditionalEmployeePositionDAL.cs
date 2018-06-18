using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using System.Linq;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class AdditionalEmployeePositionDAL : BaseDAL<AdditionalEmployeePosition>
    {
        public AdditionalEmployeePositionDAL(string siteUrl) : base(siteUrl) { }

        public List<AdditionalEmployeePosition> GetEmployeesByLevel(int level, string module = "")
        {
            return this.GetByQuery(BuildQueryStringGetEmployeeByLevel(level, module));
        }

        public bool GetAdditionalPosition(int employeeId, string module, int additionalEmployeePositionLevelCode)
        {
            var item = GetAdditionalEmployee(employeeId, module, additionalEmployeePositionLevelCode);
            return item != null ? true : false;
        }

        #region Private Methods
        private string BuildQueryStringGetEmployeeByLevel(int level, string module)
        {
            string queryStr = string.Empty;
            if (!string.IsNullOrEmpty(module))
            {
                queryStr = string.Format(@"<Where>
                                              <And>
                                                  <Eq>
                                                     <FieldRef Name='" + StringConstant.AdditionalEmployeePositionList.Fields.Module + @"' />
                                                     <Value Type='Choice'>{0}</Value>
                                                  </Eq>
                                                  <Eq>
                                                     <FieldRef Name='" + StringConstant.AdditionalEmployeePositionList.Fields.EmployeeLevel + @"' />
                                                     <Value Type='Number'>{1}</Value>
                                                  </Eq>
                                              </And>
                                           </Where>", module, level);
            }
            else
            {
                queryStr = string.Format(@"<Where>
                                              <Eq>
                                                 <FieldRef Name='" + StringConstant.AdditionalEmployeePositionList.Fields.EmployeeLevel + @"' />
                                                 <Value Type='Number'>{0}</Value>
                                              </Eq>
                                           </Where>", level);
            }
            return queryStr;
        }

        private AdditionalEmployeePosition GetAdditionalEmployee(int employeeId, string module, int employeeLevel)
        {
            var queryString = $@"<And>
                                    <Eq>
                                        <FieldRef Name='{StringConstant.AdditionalEmployeePositionList.Fields.Employee}' LookupId='TRUE'/>
                                        <Value Type='Lookup'>{employeeId}</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='{StringConstant.AdditionalEmployeePositionList.Fields.EmployeeLevel}' />
                                        <Value Type='Number'>{employeeLevel}</Value>
                                    </Eq>
                                </And>";

            if (!string.IsNullOrEmpty(module))
            {
                queryString = $@"<And><Eq><FieldRef Name='{StringConstant.AdditionalEmployeePositionList.Fields.Module}' /><Value Type='Choice'>{module}</Value></Eq>{queryString}</And>";
            }
            queryString = $@"<Where>{queryString}</Where>";

            var additionalEmployeePositions = GetByQuery(queryString, string.Empty);
            
            return additionalEmployeePositions.FirstOrDefault();
        }
        #endregion


    }
}
