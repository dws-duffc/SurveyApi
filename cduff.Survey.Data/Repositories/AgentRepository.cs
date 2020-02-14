//-----------------------------------------------------------------------
// <copyright file=”AgentRepository.cs” company=”Cody Duff”>
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

    public class AgentRepository : Repository<Agent>, IRepository<Agent>
    {
        public AgentRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes a Agent given an instance of the Agent to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Agent to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Agent entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Agent_Delete";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Agents that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Agent.</returns>
        public override IEnumerable<Agent> Find(Expression<Func<Agent, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Agent>.Decompile(predicate);
            Filter agencyCode = filters.SingleOrDefault(x => x.PropertyName == "AgencyCode");
            Filter agencyName = filters.SingleOrDefault(x => x.PropertyName == "AgencyName");
            Filter activeAgent = filters.SingleOrDefault(x => x.PropertyName == "IsActiveAgent");

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Agent_Search";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AgencyCode", SqlDbType.NVarChar, 10) { Value = agencyCode == null ? DBNull.Value : agencyCode.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyName", SqlDbType.NVarChar, 102) { Value = agencyName == null ? DBNull.Value : agencyName.Value });
                command.Parameters.Add(new SqlParameter("@p_IsActiveAgent", SqlDbType.Bit, 1) { Value = activeAgent == null ? DBNull.Value : activeAgent.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Agent), reader, true) as Agent;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Agents.
        /// </summary>
        /// <returns>An IEnumerable of all Agents.</returns>
        public override IEnumerable<Agent> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Agent_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Agent), reader, true) as Agent;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a Agent given a Agent Id.
        /// </summary>
        /// <param name="id">Id of the Agent to be retrieved.</param>
        /// <returns>An instance of a Agent.</returns>
        public Agent GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Agent_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = id.ToDbNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<Agent> agents = new List<Agent>();
                    while (reader.Read())
                    {
                        var agent = MapEntity(typeof(Agent), reader, true) as Agent;
                        agents.Add(agent);
                    }

                    return agents.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new Agent record and returns the Id of the newly created Agent.
        /// </summary>
        /// <param name="entity">An instance of a Agent to be inserted.</param>
        /// <returns>int AgentId</returns>
        public int Insert(Agent entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Agent_Insert";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AgentCode", SqlDbType.NVarChar, 8) { Value = entity.AgencyCode });
                IDbDataParameter agentId = new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(agentId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(agentId.Value);
            }
        }

        /// <summary>
        /// Not supported. Agents cannot be updated from this application.
        /// </summary>
        public bool Update(Agent entity)
        {
            throw new NotSupportedException("Agents cannot be updated from this application.");
        }
    }
}
