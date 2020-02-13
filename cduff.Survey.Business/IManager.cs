//-----------------------------------------------------------------------
// <copyright file=”IManager.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IManager<TEntity> where TEntity : class
    {
        TEntity Add(TEntity entity);

        void Delete(int id);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        TEntity Get(int id);

        IEnumerable<TEntity> GetAll();

        TEntity Update(TEntity entity);
    }
}
