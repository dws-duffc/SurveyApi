//-----------------------------------------------------------------------
// <copyright file=”IRepository.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data
{
    using System.Collections.Generic;

    public interface IRepository<TEntity> where TEntity : class
    {
        bool Delete(TEntity entity);
        IEnumerable<TEntity> GetAll();
        TEntity GetById(int id);
        int Insert(TEntity entity);
        bool Update(TEntity entity);
    }
}
