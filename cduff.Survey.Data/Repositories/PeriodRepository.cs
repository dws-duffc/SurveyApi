//-----------------------------------------------------------------------
// <copyright file=”PeriodRepository.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using Model;
    using Utilities;

    public class PeriodRepository : Repository<Period>, IRepository<Period>
    {
        public PeriodRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes a Period given an instance of the Period to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Period to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Period entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Period_Delete";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Periods that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Period.</returns>
        public override IEnumerable<Period> Find(Expression<Func<Period, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Period>.Decompile(predicate);
            Filter periodId = filters.SingleOrDefault(x => x.PropertyName == "PeriodId");
            Filter periodDate = filters.SingleOrDefault(x => x.PropertyName == "StartDate");
            if (periodDate == null) periodDate = filters.SingleOrDefault(x => x.PropertyName == "EndDate");
            Filter periodIsOpen = filters.SingleOrDefault(x => x.PropertyName == "IsOpen");

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Period_Search";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 10) { Value = periodId == null ? DBNull.Value : periodId.Value });
                command.Parameters.Add(new SqlParameter("@p_PeriodDate", SqlDbType.Date, 10) { Value = periodDate == null ? DBNull.Value : periodDate.Value });
                command.Parameters.Add(new SqlParameter("@p_IsOpen", SqlDbType.Bit, 1) { Value = periodIsOpen == null ? DBNull.Value : periodIsOpen.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Period), reader, true) as Period;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Periods.
        /// </summary>
        /// <returns>An IEnumerable of all Periods.</returns>
        public override IEnumerable<Period> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Period_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Period), reader, true) as Period;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a Period given a Period Id.
        /// </summary>
        /// <param name="id">Id of the Period to be retrieved.</param>
        /// <returns>An instance of a Period.</returns>
        public Period GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Period_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = id.ToDbNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<Period> periods = new List<Period>();
                    while (reader.Read())
                    {
                        var period = MapEntity(typeof(Period), reader, true) as Period;
                        periods.Add(period);
                    }

                    return periods.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new Period record and returns the Id of the newly created Period.
        /// </summary>
        /// <param name="entity">An instance of a Period to be inserted.</param>
        /// <returns>int PeriodId</returns>
        public int Insert(Period entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Period_Insert";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_StartDate", SqlDbType.Date, 10) { Value = entity.StartDate.ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_EndDate", SqlDbType.Date, 10) { Value = entity.EndDate.ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_IsOpen", SqlDbType.Bit, 1) { Value = entity.IsOpen });
                IDbDataParameter periodId = new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Direction = ParameterDirection.Output };
                command.Parameters.Add(periodId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(periodId.Value);
            }
        }

        /// <summary>
        /// Updates a Period given the instance of a Period.
        /// </summary>
        /// <param name="entity">An instance of a Period to be updated.</param>
        /// <returns>True if at least one record was updated.</returns>
        public bool Update(Period entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Period_Update";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_StartDate", SqlDbType.Date, 10) { Value = entity.StartDate.ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_EndDate", SqlDbType.Date, 10) { Value = entity.EndDate.ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_IsOpen", SqlDbType.Bit, 1) { Value = entity.IsOpen });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }
    }
}
