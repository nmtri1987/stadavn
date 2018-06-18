using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RBVH.Stada.Intranet.WebPages.Utils;

namespace RBVH.Stada.Intranet.WebPages.Layouts
{
    public partial class MyShiftTime : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
        }
        private void InitialData(bool isPostBack)
        {
            //var day = DateTime.Today.Day;
            //var month = DateTime.Today.Month;
            //if (day > 21)
            //    month++;
            //if (!isPostBack)
            //{
            //    HiddenMonth.Value = month.ToString();
            //    HiddenYear.Value = DateTime.Today.Year.ToString();
            //    DateTextbox.Text = HiddenMonth.Value + "/" + HiddenYear.Value;
            //}
            //else
            //{
            //    string monthString = Request["__EVENTARGUMENT"];
            //    var monthValues = monthString.Split('/');
            //    if (monthValues.Length == 2)
            //    {
            //        HiddenMonth.Value = monthValues[0];
            //        HiddenYear.Value = monthValues[1];
            //        DateTextbox.Text = monthString;
            //    }
            //}
            
            UserHelper userHelper = new UserHelper();
            var currentEmployee = userHelper.GetCurrentLoginUser();
            if (currentEmployee == null || currentEmployee.ID <= 0)
            {
                ParamRequesterLookupIDHidden.Value = "";
            }
            else
            {
                ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
            }
        }
        //protected void ShiftDateControl_DateChanged(object sender, EventArgs e)
        //{
        //    var selectedDate = ShiftDateControl.SelectedDate;
        //    HiddenMonth.Value = ShiftDateControl.SelectedDate.Month.ToString();
        //    HiddenYear.Value = ShiftDateControl.SelectedDate.Year.ToString();
       
        //}
    

        protected void DateTextbox_TextChanged(object sender, EventArgs e)
        {
            var selectedDate = this.DateTextbox.Text;
        }
    }
}
