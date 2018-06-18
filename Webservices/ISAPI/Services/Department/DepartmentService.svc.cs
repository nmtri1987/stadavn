using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Webservices.Model;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using System;
using System.Linq;
using RBVH.Core.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using RBVH.Stada.Intranet.Webservices;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Webservices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DepartmentService : IDepartmentService
    {
        readonly DepartmentDAL _departmentDal;

        public DepartmentService()
        {
            _departmentDal = new DepartmentDAL(SPContext.Current.Web.Url);
        }
        /// <summary>
        /// Get Department For Shift
        /// </summary>
        /// <param name="languageCode">vi-VN or other</param>
        /// <returns>list of departments</returns>
        public List<DepartmentInfo> GetDepartmentForShift(string lcid, string locationIds)
        {
            try
            {
                List<DepartmentInfo> departments = new List<DepartmentInfo>();
                DepartmentDAL departmentDAL = new DepartmentDAL(SPContext.Current.Web.Url);
                List<int> locationCollection = locationIds.SplitStringOfLocations().ConvertAll(e => Convert.ToInt32(e));
                var departmentList = departmentDAL.GetByShiftRequestRequired(true, locationCollection);
                if (departmentList.Any())
                {
                    foreach (var item in departmentList)
                    {
                        string departmentName = lcid.Equals("1066") ? item.VietnameseName : item.Name;
                        departments.Add(new DepartmentInfo() { Id = item.ID, DepartmentName = departmentName });
                    }
                }
                return departments;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Service - GetDepartmentForShift",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<DepartmentInfo> GetDepartmentsByLcid(string lcid, string locationIds)
        {
            try
            {
                List<DepartmentInfo> departments = new List<DepartmentInfo>();
                DepartmentDAL departmentDAL = new DepartmentDAL(SPContext.Current.Web.Url);
                List<int> locationCollection = locationIds.SplitStringOfLocations().ConvertAll(e => Convert.ToInt32(e));
                var departmentList = departmentDAL.GetByLocations(locationCollection);
                if (departmentList.Any())
                {
                    foreach (var item in departmentList)
                    {
                        string departmentName = lcid.Equals("1066") ? item.VietnameseName : item.Name;
                        departments.Add(new DepartmentInfo() { Id = item.ID, DepartmentName = departmentName });
                    }
                }
                return departments;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Service - GetDepartmentsByLcid",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// CALL URL : _vti_bin/Services/Department/DepartmentService.svc/GetDepartments/vi-VN
        public List<DepartmentInfo> GetDepartments(string languageCode, string locationIds)
        {
            var listResult = new List<DepartmentInfo>();
            try
            {
                List<Biz.Models.Department> departmentEntities = new List<Biz.Models.Department>();
                List<int> locationCollection = locationIds.SplitStringOfLocations().ConvertAll(e=>Convert.ToInt32(e));
                if (locationCollection != null && !locationCollection.Contains(0))
                {
                    var locationFilter = CommonHelper.BuildFilterCommonMultiLocations(locationCollection);
                    if (!string.IsNullOrEmpty(locationFilter))
                    {
                        departmentEntities = _departmentDal.GetByQuery($"<Where>{locationFilter}</Where>", string.Empty);
                    }
                }
                else
                {
                    departmentEntities = _departmentDal.GetAll();
                }

                foreach (var departmentEntity in departmentEntities)
                {
                    listResult.Add(new DepartmentInfo
                    {
                        Id = departmentEntity.ID,
                        DepartmentName = languageCode == "vi-VN" ? departmentEntity.VietnameseName : departmentEntity.Name
                    });
                }
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Service - GetDepartmentByIdLanguageCode fn",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
            }
            return listResult;
        }

        /// <summary>
        /// Get Department List by language code
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        /// CALL URL : _vti_bin/Services/Department/DepartmentService.svc/GetDepartmentByIdLanguageCode/1/vi-VN
        public DepartmentInfo GetDepartmentByIdLanguageCode(string departmentId, string languageCode)
        {
            var langCode = string.IsNullOrEmpty(languageCode) ? "en-US" : languageCode;
            try
            {
                var codes = langCode.Split('-');
                if (codes.Length > 1)
                {
                    langCode = codes[0].ToLower() + "-" + codes[1].ToUpper();
                }
                else
                {
                    switch (langCode)
                    {
                        case "1066":
                            langCode = "vi-VN";
                            break;
                        case "1033":
                            langCode = "en-US";
                            break;
                    }
                }

                DepartmentInfo departmentInfo = new DepartmentInfo();
                int departmentIdValue;
                if (int.TryParse(departmentId, out departmentIdValue))
                {
                    var department = _departmentDal.GetByID(departmentIdValue);
                    if (department != null)
                    {

                        string departmentName = langCode.Equals("vi-VN") ? department.VietnameseName : department.Name;
                        departmentInfo = new DepartmentInfo() { Id = department.ID, DepartmentName = departmentName };
                    }
                }
                return departmentInfo;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Service - GetDepartmentByIdLanguageCode fn",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Get by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public DepartmentInfo GetByCode(string code, string lcid)
        {
            try
            {
                DepartmentInfo departmentInfo = new DepartmentInfo();
                var department = _departmentDal.GetByCode(code);
                if (department != null)
                {
                    string departmentName = lcid.Equals("1066") ? department.VietnameseName : department.Name;
                    departmentInfo = new DepartmentInfo { Id = department.ID, DepartmentName = departmentName };
                }
                return departmentInfo;
            }
            catch (Exception ex)
            {
                ULSLogging.Log(new SPDiagnosticsCategory("STADA - Department Service - GetByCode fn",
                        TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ex.Message, ex.StackTrace));
                return null;
            }
        }
    }
}