//-----------------------------------------------------------------------
// <copyright file=”AssignmentRepository.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Model;
    using Utilities;
    using System.Linq.Expressions;

    public class AssignmentRepository : Repository<Assignment>, IRepository<Assignment>
    {
        public AssignmentRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes an Assignment given an instance of the Assignment to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Assignment to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Assignment entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Assignment_Delete";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId });
                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = entity.RepId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Assignments that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Assignment.</returns>
        public override IEnumerable<Assignment> Find(Expression<Func<Assignment, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Assignment>.Decompile(predicate);
            var periodId = filters.SingleOrDefault(x => x.PropertyName == "PeriodId");
            var periodStartDate = filters.SingleOrDefault(x => x.PropertyName == "StartDate");
            var periodEndDate = filters.SingleOrDefault(x => x.PropertyName == "EndDate");
            var periodIsOpen = filters.SingleOrDefault(x => x.PropertyName == "IsOpen");
            var agentId = filters.SingleOrDefault(x => x.PropertyName == "AgentId");
            var agentCode = filters.SingleOrDefault(x => x.PropertyName == "AgencyCode");
            var agentName = filters.SingleOrDefault(x => x.PropertyName == "AgencyName");
            var activeAgent = filters.SingleOrDefault(x => x.PropertyName == "IsActiveAgent");
            var repId = filters.SingleOrDefault(x => x.PropertyName == "RepId");
            var repUsername = filters.SingleOrDefault(x => x.PropertyName == "Username");
            var repFirstName = filters.SingleOrDefault(x => x.PropertyName == "FirstName");
            var repLastName = filters.SingleOrDefault(x => x.PropertyName == "LastName");
            var repIsActive = filters.SingleOrDefault(x => x.PropertyName == "IsActive");
            var status = filters.SingleOrDefault(x => x.PropertyName == "Status");
            var attemptedBy = filters.SingleOrDefault(x => x.PropertyName == "LastAttemptedBy");
            var attemptDate = filters.SingleOrDefault(x => x.PropertyName == "LastAttemptedDate");
            var notes = filters.SingleOrDefault(x => x.PropertyName == "Notes");

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Assignment_Search";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = periodId == null ? DBNull.Value : periodId.Value });
                command.Parameters.Add(new SqlParameter("@p_PeriodStartDate", SqlDbType.DateTime) { Value = periodStartDate == null ? DBNull.Value : Convert.ToDateTime(periodStartDate.Value).ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_PeriodEndDate", SqlDbType.DateTime) { Value = periodEndDate == null ? DBNull.Value : Convert.ToDateTime(periodEndDate.Value).ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_PeriodIsOpen", SqlDbType.Bit, 1) { Value = periodIsOpen == null ? DBNull.Value : periodIsOpen.Value });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = periodId == null ? DBNull.Value : agentId.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyCode", SqlDbType.VarChar, 10) { Value = agentCode == null ? DBNull.Value : agentCode.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyName", SqlDbType.VarChar, 102) { Value = agentName == null ? DBNull.Value : agentName.Value });
                command.Parameters.Add(new SqlParameter("@p_IsActiveAgent", SqlDbType.Bit, 1) { Value = activeAgent == null ? DBNull.Value : activeAgent.Value });
                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = periodId == null ? DBNull.Value : repId.Value });
                command.Parameters.Add(new SqlParameter("@p_RepUsername", SqlDbType.VarChar, 202) { Value = repUsername == null ? DBNull.Value : repUsername.Value });
                command.Parameters.Add(new SqlParameter("@p_RepFirstName", SqlDbType.VarChar, 12) { Value = repFirstName == null ? DBNull.Value : repFirstName.Value });
                command.Parameters.Add(new SqlParameter("@p_RepLastName", SqlDbType.VarChar, 22) { Value = repLastName == null ? DBNull.Value : repLastName.Value });
                command.Parameters.Add(new SqlParameter("@p_RepIsActive", SqlDbType.Bit, 1) { Value = repIsActive == null ? DBNull.Value : repIsActive.Value });
                command.Parameters.Add(new SqlParameter("@p_AssignmentStatus", SqlDbType.VarChar, 202) { Value = status == null ? DBNull.Value : status.Value });
                command.Parameters.Add(new SqlParameter("@p_LastAttemptedBy", SqlDbType.VarChar, 202) { Value = attemptedBy == null ? DBNull.Value : attemptedBy.Value });
                command.Parameters.Add(new SqlParameter("@p_LastAttemptedDate", SqlDbType.DateTime) { Value = attemptDate == null ? DBNull.Value : Convert.ToDateTime(attemptDate.Value).ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_Notes", SqlDbType.VarChar, 8000) { Value = notes == null ? DBNull.Value : notes.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Assignment), reader, true) as Assignment;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Assignments with the given AgentId, RepId, and/or PeriodId.
        /// </summary>
        /// <param name="agentId">The Id that belongs to the Agent of the Assignment.</param>
        /// <param name="repId">The Id that belongs to the Rep of the Assignment.</param>
        /// <param name="periodId">The Id that belongs to the Period the Assignment is in.</param>
        /// <returns></returns>
        public IEnumerable<Assignment> Get(int? agentId, int? repId, int? periodId)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Assignment_Get";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = agentId.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = repId.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = periodId.ToDBNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Assignment), reader, true) as Assignment;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Assignments.
        /// </summary>
        /// <returns>An IEnumerable of all Assignments.</returns>
        public override IEnumerable<Assignment> GetAll()
        {
            return Get(null, null, null);
        }

        /// <summary>
        /// Not supported. Use Get() because Assignment class uses a composite key of AgentId, RepId, and PeriodId
        /// </summary>
        public Assignment GetById(int id)
        {
            throw new NotSupportedException("Use Get() instead of GetById() for Assignments. "
                + "Assignments use a composite key of AgentId, RepId, and PeriodId.");
        }

        /// <summary>
        /// Inserts a new Assignment record and returns the number of rows inserted.
        /// </summary>
        /// <param name="entity">An instance of a Assignment to be inserted.</param>
        /// <returns>int rowCount</returns>
        public int Insert(Assignment entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Assignment_Insert";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId });
                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = entity.RepId });
                command.Parameters.Add(new SqlParameter("@p_Status", SqlDbType.VarChar, 200) { Value = entity.Status.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_Notes", SqlDbType.VarChar, 8000) { Value = entity.Notes.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_LastAttemptedBy", SqlDbType.VarChar, 200) { Value = entity.LastAttemptedBy.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_LastAttemptedDate", SqlDbType.DateTime) { Value = entity.LastAttemptedDate.ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_AttemptCount", SqlDbType.TinyInt, 3) { Value = entity.AttemptCount });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value);
            }
        }

        /// <summary>
        /// Updates an Assignment given the instance of an Assignment.
        /// </summary>
        /// <param name="entity">An instance of a Assignment to be updated.</param>
        /// <returns>True if at least one record was updated.</returns>
        public bool Update(Assignment entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Assignment_Update";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = entity.RepId.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_Status", SqlDbType.VarChar, 200) { Value = entity.Status.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_Notes", SqlDbType.VarChar, 8000) { Value = entity.Notes.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_LastAttemptedBy", SqlDbType.VarChar, 200) { Value = entity.LastAttemptedBy.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_LastAttemptedDate", SqlDbType.DateTime) { Value = entity.LastAttemptedDate.ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_AttemptCount", SqlDbType.TinyInt, 3) { Value = entity.AttemptCount.ToDBNull() });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }
    }
}