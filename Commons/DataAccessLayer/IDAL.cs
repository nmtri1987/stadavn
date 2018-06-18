//-----------------------------------------------------------------------
// <copyright file="IDAL.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the IDAL class.</summary>
//-----------------------------------------------------------------------
namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    using Microsoft.SharePoint;
    using Models;
    using System.Collections.Generic;

    public interface IDAL<T> where T : EntityBase, new()
    {
        string ListUrl { get; }

        string SiteUrl { get; set; }

        T ParseToEntity(SPListItem listItem);
        
        T GetByID(int id);

        List<T> GetAll();

        int SaveItem(T entity);

        bool SaveItems(List<T> entities);

        bool SaveItems(List<T> entities, bool deleteDataFirst);

        bool Delete(int id);

        void DeleteItems(IList<int> ids);

        List<T> GetByQuery(string queryString, params string[] viewFieldsQuery);
    }
}