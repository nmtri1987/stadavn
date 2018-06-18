using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Extension
{
   public static class ListExtension
    {
        public static SPList TryGetSPList(this SPWeb web, string listUrl)
        {
            try
            {
                SPList splist = web.GetList(listUrl);
                if(splist != null)
                {
                    return splist;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;   
            }
        }
    }
}
