using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class CalendarDAL : DataAccessLayer.BaseDAL<Calendar>
    {
        public CalendarDAL(string siteUrl) : base(siteUrl)
        {
            ListUrl = "/Lists/CompanyCalendar";
        }

        public override Calendar ParseToEntity(SPListItem listItem)
        {
            var location = listItem.ToString(StringConstant.CalendarList.LocationField);
            location = string.IsNullOrEmpty(location) ? StringConstant.FactoryLocation1 : location;
            var calendar = new Calendar
            {
                ID = listItem.ID,
                Title = listItem.ToString(StringConstant.CalendarList.TitleField),
                StartDate = Convert.ToDateTime(listItem[StringConstant.CalendarList.StartDateField]),
                EndDate = Convert.ToDateTime(listItem[StringConstant.CalendarList.EndDateField]),
                Description = listItem.ToString(StringConstant.CalendarList.DescriptionField),
                Location = location,
                Category = listItem.ToString(StringConstant.CalendarList.CategoryField)
            };
            return calendar;
        }

        /// <summary>
        /// Get calendar list by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<Calendar> GetByCategory(string category)
        {
            List<Calendar> calendars = new List<Calendar>();

            string queryStr = $@" <Where><Eq><FieldRef Name='Category' /><Value Type='Text'>{category}</Value></Eq></Where>";
            var calendarItems = this.GetByQueryToSPListItemCollection(SPContext.Current.Web, queryStr);
            if (calendarItems != null && calendarItems.Count > 0)
            {
                foreach (SPListItem calendarItem in calendarItems)
                {
                    var calendar = ParseToEntity(calendarItem);
                    calendars.Add(calendar);
                }
            }

            return calendars;
        }
        /// <summary>
        /// Get list of calendar by categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<Calendar> GetByCategories(List<string> categories)
        {
            string categoriesString = string.Empty;
            foreach (string category in categories)
            {
                categoriesString += string.Format("<Value Type='Text'>{0}</Value>", category);
            }

            List<Calendar> calendars = new List<Calendar>();

            string queryStr = $@" <Where><In><FieldRef Name='Category' /><Values>{categoriesString}</Values></In></Where>";
            var calendarItems = this.GetByQueryToSPListItemCollection(SPContext.Current.Web, queryStr);
            if (calendarItems != null && calendarItems.Count > 0)
            {
                foreach (SPListItem calendarItem in calendarItems)
                {
                    var calendar = ParseToEntity(calendarItem);
                    calendars.Add(calendar);
                }
            }

            return calendars;
        }

        /// <summary>
        /// Get by by range and categories
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<Calendar> GetByDateAndCategories(DateTime fromDate, DateTime toDate, List<string> categories)
        {
            string fromDateString = fromDate.ToString(StringConstant.DateFormatTZForCAML);
            string toDateString = toDate.ToString(StringConstant.DateFormatTZForCAML);

            string categoriesString = string.Empty;

            foreach (string category in categories)
            {
                categoriesString += string.Format("<Value Type='Text'>{0}</Value>", category);
            }


            List<Calendar> calendars = new List<Calendar>();

            string queryStr = $@"<Where><And><In><FieldRef Name='Category'/><Values>{categoriesString}</Values></In><And><Geq><FieldRef Name='EventDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>{fromDateString}</Value></Geq><Leq><FieldRef Name='EndDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>{toDateString}</Value></Leq></And></And></Where>";
            var calendarItems = this.GetByQueryToSPListItemCollection(SPContext.Current.Web, queryStr);
            if (calendarItems != null && calendarItems.Count > 0)
            {
                foreach (SPListItem calendarItem in calendarItems)
                {
                    var calendar = ParseToEntity(calendarItem);
                    calendars.Add(calendar);
                }
            }

            return calendars;
        }

        public Calendar GetByDateAndCategories(DateTime date, List<string> categories)
        {
            string dateStr = date.ToString(StringConstant.DateFormatTZForCAML);
            string categoriesString = string.Empty;

            foreach (string category in categories)
            {
                categoriesString += string.Format("<Value Type='Text'>{0}</Value>", category);
            }

            Calendar calendar = null;
            
            string queryStr = $@"<Where><And><In><FieldRef Name='Category'/><Values>{categoriesString}</Values></In><And><Leq><FieldRef Name='EventDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>{dateStr}</Value></Leq><Geq><FieldRef Name='EndDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>{dateStr}</Value></Geq></And></And></Where>";
            var calendarItems = this.GetByQueryToSPListItemCollection(SPContext.Current.Web, queryStr);
            if (calendarItems != null && calendarItems.Count > 0)
            {
                calendar = new Calendar();
                calendar = ParseToEntity(calendarItems[0]);
            }

            return calendar;
        }
    }
}
