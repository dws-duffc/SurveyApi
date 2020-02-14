//-----------------------------------------------------------------------
// <copyright file=”AttemptLogRepository.cs” company=”Cody Duff”>
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

    public class AttemptLogRepository : Repository<AttemptLog>, IRepository<AttemptLog>
    {
        public AttemptLogRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Not supported. AttemptLogs cannot be deleted from this application.
        /// </summary>
        public bool Delete(AttemptLog entity)
        {
            throw new NotSupportedException("AttemptLogs cannot be deleted from this application.");
        }

        /// <summary>
        /// Finds all AttemptLogs that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type AttemptLog.</returns>
        public override IEnumerable<AttemptLog> Find(Expression<Func<AttemptLog, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<AttemptLog>.Decompile(predicate);
            Filter periodId = filters.SingleOrDefault(x => x.PropertyName == "PeriodId");
            Filter periodIsOpen = filters.SingleOrDefault(x => x.PropertyName == "IsOpen");
            Filter agentId = filters.SingleOrDefault(x => x.PropertyName == "AgentId");
            Filter agentCode = filters.SingleOrDefault(x => x.PropertyName == "AgencyCode");
            Filter agentName = filters.SingleOrDefault(x => x.PropertyName == "AgencyName");
            Filter activeAgent = filters.SingleOrDefault(x => x.PropertyName == "IsActiveAgent");
            Filter repId = filters.SingleOrDefault(x => x.PropertyName == "PeriodId");
            Filter repUsername = filters.SingleOrDefault(x => x.PropertyName == "RepId");
            Filter repFirstName = filters.SingleOrDefault(x => x.PropertyName == "FirstName");
            Filter repLastName = filters.SingleOrDefault(x => x.PropertyName == "LastName");
            Filter repIsActive = filters.SingleOrDefault(x => x.PropertyName == "IsActive");
            Filter attemptedDate = filters.SingleOrDefault(x => x.PropertyName == "AttemptedDate");
            Filter attemptedBy = filters.SingleOrDefault(x => x.PropertyName == "AttemptedBy");

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_AttemptLog_Search";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = periodId == null ? DBNull.Value : periodId.Value });
                command.Parameters.Add(new SqlParameter("@p_PeriodIsOpen", SqlDbType.Bit, 1) { Value = periodIsOpen == null ? DBNull.Value : periodIsOpen.Value });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = agentId == null ? DBNull.Value : agentId.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyCode", SqlDbType.VarChar, 10) { Value = agentCode == null ? DBNull.Value : agentCode.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyName", SqlDbType.VarChar, 102) { Value = agentName == null ? DBNull.Value : agentName.Value });
                command.Parameters.Add(new SqlParameter("@p_IsActiveAgent", SqlDbType.Bit, 1) { Value = activeAgent == null ? DBNull.Value : activeAgent.Value });
                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = repId == null ? DBNull.Value : repId.Value });
                command.Parameters.Add(new SqlParameter("@p_RepUsername", SqlDbType.VarChar, 202) { Value = repUsername == null ? DBNull.Value : repUsername.Value });
                command.Parameters.Add(new SqlParameter("@p_RepFirstName", SqlDbType.VarChar, 12) { Value = repFirstName == null ? DBNull.Value : repFirstName.Value });
                command.Parameters.Add(new SqlParameter("@p_RepLastName", SqlDbType.VarChar, 22) { Value = repLastName == null ? DBNull.Value : repLastName.Value });
                command.Parameters.Add(new SqlParameter("@p_RepIsActive", SqlDbType.Bit, 1) { Value = repIsActive == null ? DBNull.Value : repIsActive.Value });
                command.Parameters.Add(new SqlParameter("@p_AttemptedDate", SqlDbType.SmallInt, 10) { Value = attemptedDate == null ? DBNull.Value : attemptedDate.Value });
                command.Parameters.Add(new SqlParameter("@p_AttemptedBy", SqlDbType.SmallInt, 10) { Value = attemptedBy == null ? DBNull.Value : attemptedBy.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(AttemptLog), reader, true) as AttemptLog;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all AttemptLogs.
        /// </summary>
        /// <returns>An IEnumerable of all AttemptLogs.</returns>
        public override IEnumerable<AttemptLog> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_AttemptLog_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AttemptLogId", SqlDbType.Int, 10) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(AttemptLog), reader, true) as AttemptLog;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a AttemptLog given a AttemptLog Id.
        /// </summary>
        /// <param name="id">Id of the AttemptLog to be retrieved.</param>
        /// <returns>An instance of a AttemptLog.</returns>
        public AttemptLog GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_AttemptLog_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AttemptLogId", SqlDbType.BigInt, 19) { Value = id.ToDbNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<AttemptLog> attemptLogs = new List<AttemptLog>();
                    while (reader.Read())
                    {
                        var attemptLog = MapEntity(typeof(AttemptLog), reader, true) as AttemptLog;
                        attemptLogs.Add(attemptLog);
                    }

                    return attemptLogs.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new AttemptLog record and returns the Id of the newly created AttemptLog.
        /// </summary>
        /// <param name="entity">An instance of a AttemptLog to be inserted.</param>
        /// <returns>int AttemptLogId</returns>
        public int Insert(AttemptLog entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_AttemptLog_Insert";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId });
                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = entity.RepId });
                command.Parameters.Add(new SqlParameter("@p_AttemptedDate", SqlDbType.DateTime, 8) { Value = entity.AttemptedDate });
                command.Parameters.Add(new SqlParameter("@p_AttemptedBy", SqlDbType.VarChar, 200) { Value = entity.AttemptedBy });
                IDbDataParameter attemptLogId = new SqlParameter("@p_AttemptLogId", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(attemptLogId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(attemptLogId.Value);
            }
        }

        /// <summary>
        /// Not supported. AttemptLogs cannot be updated from this application.
        /// </summary>
        public bool Update(AttemptLog entity)
        {
            throw new NotSupportedException("AttemptLogs cannot be updated from this application.");
        }
    }
}
