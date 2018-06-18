using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.HotFixes.CalendarList;
using RBVH.Stada.Intranet.HotFixes.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.HotFixes
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteUrl = "";
            Console.Write("Site Url: ");
            siteUrl = Console.ReadLine();

            Console.WriteLine("");
            Console.Write("Waiting...");

            MoveData.MoveDateUserToMultiUser(siteUrl, "Business Trip Management", "CommonApprover1", "CommonApprover1Multi");
            MoveData.MoveDateUserToMultiUser(siteUrl, "Business Trip Management", "CommonApprover2", "CommonApprover2Multi");
            MoveData.MoveDateUserToMultiUser(siteUrl, "Business Trip Management", "CommonApprover3", "CommonApprover3Multi");
            MoveData.MoveDateUserToMultiUser(siteUrl, "Business Trip Management", "CommonApprover3", "CommonApprover4Multi");

            Console.WriteLine("");
            Console.Write("Done!");
            Console.Read();

            //siteUrl = Console.ReadLine();

            //AddNewColToList.AddDelegatedByColToList(siteUrl);

            //AddNewColToList.AddCommonCommentToVehicleManagement(siteUrl);
            //AddNewColToList.AddShiftRequiredColToList(siteUrl);
            //AddNewColToList.AddColForSortToList(siteUrl);
            //AddNewColToList.UpdateColForSortData(siteUrl);
            //CalendarHelper.ChangeFillInChoices(siteUrl);
            //TaskListHelper.UpdateTypeOfDateTime(siteUrl);


            //AddNewColToList.AddCommonLocationToList(siteUrl);
            //AddNewColToList.AddCommonMultiLocationToList(siteUrl);

            //ServiceHelper.UpdateCustomWCF();
        }
    }
}
