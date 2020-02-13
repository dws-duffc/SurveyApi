//-----------------------------------------------------------------------
// <copyright file=”Repository.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Utilities;

    public abstract class Repository<TEntity> where TEntity : class
    {
        protected const int timeOut = 30, extendedTimeout = 150;
        SurveyContext context;

        protected Repository(SurveyContext context)
        {
            this.context = context;
        }

        public SurveyContext Context => context;

        public abstract IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        public abstract IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Returns a mapped object of the given type using a given record.
        /// </summary>
        /// <param name="type">The type to map the DataRecord to.</param>
        /// <param name="record">The record containing data to be mapped to object of given type.</param>
        /// <param name="mapRecursive">Used to determine whether to map nested objects recursively.</param>
        /// <returns>Populated object of given type.</returns>
        protected static object MapEntity(Type type, IDataRecord record, bool mapRecursive = false)
        {
            object obj = Activator.CreateInstance(type);
            foreach (var prop in type.GetRuntimeProperties())
            {
                object value;
                Type propType = prop.PropertyType;
                if (record.HasColumn(prop.Name))
                {
                    value = record[prop.Name];

                    //set property accordingly
                    if (canChangeType(value, propType))
                    {
                        prop.SetValue(obj, Convert.ChangeType(value, propType), null);
                    }
                }
                else if (propType.Namespace != "System" && propType.GetTypeInfo().GetInterface("IEnumerable") == null && mapRecursive)
                {
                    value = MapEntity(propType, record, mapRecursive);
                    prop.SetValue(obj, Convert.ChangeType(value, propType), null);
                }
            }

            return obj;
        }

        protected IEnumerable<TEntity> MapList(IDbCommand command)
        {
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return (TEntity)MapEntity(typeof(TEntity), reader, false);
                }
            }
        }

        static bool canChangeType(object value, Type convertType)
        {
            if (convertType == null)
            {
                return false;
            }

            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            return value is IConvertible;
        }
    }
}
