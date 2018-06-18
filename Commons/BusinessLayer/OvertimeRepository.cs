using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RBVH.Stada.Intranet.Biz.BusinessLayer
{
    public class OvertimeRepository
    {
        private OverTimeManagementDAL _overtimeDAL;
        private OverTimeManagementDetailDAL _overtimeDetailDAL;

        public OvertimeRepository(string siteURL)
        {
            _overtimeDAL = new OverTimeManagementDAL(siteURL);
            _overtimeDetailDAL = new OverTimeManagementDetailDAL(siteURL);
        }

        //public void CreateFromShift(ShiftManagement shift, List<OverTimeManagementDetail> sourceOvertimeDetails)
        //{
        //    var overtimeList = _overtimeDAL.GetByShiftMonth(shift.Month, shift.Year, shift.Department.LookupId);
        //    List<DateTime> overtimeDateList = sourceOvertimeDetails.Select(x => x.OvertimeFrom).Distinct().ToList();
        //    // have overtime
        //    if (overtimeList != null && overtimeList.Count > 0)
        //    {
        //        List<int> overtimeIds = overtimeList.Select(x => x.ID).ToList();
        //        var overtimeDetailList = _overtimeDetailDAL.GetByMasterIds(overtimeIds);
        //        // already existed
        //        foreach (var date in overtimeDateList)
        //        {
        //            // check if overtime has been appoved, do nothing => remove them from detail
        //            var existedOvertimes = overtimeDetailList.Where(x => x.OvertimeFrom.Date == date.Date && x.ApprovalStatus.LookupValue.ToLower().Equals("true"));
        //            if (existedOvertimes != null && existedOvertimes.Count() > 0)
        //            {
        //                foreach (var item in existedOvertimes)
        //                {
        //                    sourceOvertimeDetails.RemoveAll(x => x.EmployeeID.LookupId == item.EmployeeID.LookupId && x.OvertimeFrom.Date == item.OvertimeFrom.Date);
        //                    overtimeList.RemoveAll(x => x.ID == item.OvertimeManagementID.LookupId);
        //                }
        //            }
        //            // all aproved, do nothing
        //            if (sourceOvertimeDetails == null || sourceOvertimeDetails.Count == 0)
        //            {
        //                return;
        //            }
        //            // update in-progress
        //            var canUpdateOvertimes = overtimeDetailList.Where(x => x.OvertimeFrom.Date == date.Date && x.ApprovalStatus.LookupValue.ToLower().Equals(""));
        //            // status in progress, can update
        //            if (canUpdateOvertimes != null && canUpdateOvertimes.Count() > 0)
        //            {
        //                foreach (var item in canUpdateOvertimes)
        //                {
        //                    var canUpdateDetail = sourceOvertimeDetails.FirstOrDefault(x => x.OvertimeFrom.Date == item.OvertimeFrom.Date);
        //                    if (canUpdateDetail != null)
        //                    {
        //                        canUpdateDetail.OvertimeManagementID = item.OvertimeManagementID;
        //                        if (item.Employee.LookupId == canUpdateDetail.Employee.LookupId)
        //                        {
        //                            canUpdateDetail.ID = item.ID;
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        // update overtimes existed
        //        var existedDetailList = sourceOvertimeDetails.Where(x => x.OvertimeManagementID != null && x.OvertimeManagementID.LookupId > 0).ToList();
        //        if (existedDetailList != null && existedDetailList.Count() > 0)
        //        {
        //            _overtimeDetailDAL.SaveItems(existedDetailList);
        //            // update number employees and service
        //            foreach (var overtime in overtimeList)
        //            {
        //                int countNewEmployess = existedDetailList.Count(x => x.OvertimeManagementID.LookupId == overtime.ID && x.ID <= 0);
        //                overtime.SumOfEmployee += countNewEmployess;
        //                overtime.SumOfMeal += countNewEmployess;
        //            }
        //            _overtimeDAL.SaveItems(overtimeList);
        //        }
        //        // create new overtimes
        //        var newDetailList = sourceOvertimeDetails.Where(x => x.OvertimeManagementID == null || x.OvertimeManagementID.LookupId <= 0).ToList();
        //        if (newDetailList != null && newDetailList.Count() > 0)
        //        {
        //            CreateNewOvertime(shift, newDetailList, overtimeDateList);
        //        }
        //    }
        //    // donot have any overtime, create the new one
        //    else
        //    {
        //        CreateNewOvertime(shift, sourceOvertimeDetails, overtimeDateList);
        //    }
        //}

        //private void CreateNewOvertime(ShiftManagement shift, List<OverTimeManagementDetail> overtimeDetails, List<DateTime> overtimeDateList)
        //{
        //    foreach (var date in overtimeDateList)
        //    {
        //        var detailsToSave = overtimeDetails.Where(x => x.OvertimeFrom == date && (x.OvertimeManagementID == null || x.OvertimeManagementID.LookupId <= 0)).ToList();
        //        // create overtime master
        //        var overtime = new OverTimeManagement
        //        {
        //            CommonDate = date,
        //            Requester = shift.Requester,
        //            CommonLocation = shift.Location,
        //            CommonDepartment = shift.Department,
        //            SumOfEmployee = detailsToSave.Count,
        //            SumOfMeal = detailsToSave.Count
        //        };
        //        var newId = _overtimeDAL.SaveItem(overtime);
        //        // create detail
        //        foreach (var item in detailsToSave)
        //        {
        //            item.OvertimeManagementID = new LookupItem { LookupId = newId, LookupValue = newId.ToString() };
        //        }
        //        _overtimeDetailDAL.SaveItems(detailsToSave);
        //    }
        //}
    }
}
