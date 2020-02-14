//-----------------------------------------------------------------------
// <copyright file=”ResponseRepository.cs” company=”Cody Duff”>
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

    public class ResponseRepository : Repository<Response>, IRepository<Response>
    {
        public ResponseRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes a Response given an instance of the Response to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Response to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Response entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Response_Delete";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_ResponseId", SqlDbType.BigInt, 19) { Value = entity.ResponseId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Responses that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Response.</returns>
        public override IEnumerable<Response> Find(Expression<Func<Response, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Response>.Decompile(predicate);
            Filter responseId = filters.SingleOrDefault(x => x.PropertyName == "ResponseId");
            Filter responseText = filters.SingleOrDefault(x => x.PropertyName == "ResponseText");
            Filter answerId = filters.SingleOrDefault(x => x.PropertyName == "AnswerId");
            Filter answerText = filters.SingleOrDefault(x => x.PropertyName == "AnswerText");
            Filter answerSort = filters.SingleOrDefault(x => x.PropertyName == "AnswerSort");
            Filter questionId = filters.SingleOrDefault(x => x.PropertyName == "QuestionId");
            Filter questionType = filters.SingleOrDefault(x => x.PropertyName == "QuestionTypeId");
            Filter periodId = filters.SingleOrDefault(x => x.PropertyName == "PeriodId");
            Filter periodStartDate = filters.SingleOrDefault(x => x.PropertyName == "StartDate");
            Filter periodEndDate = filters.SingleOrDefault(x => x.PropertyName == "EndDate");
            Filter periodIsOpen = filters.SingleOrDefault(x => x.PropertyName == "IsOpen");
            Filter questionText = filters.SingleOrDefault(x => x.PropertyName == "QuestionText");
            Filter questionSort = filters.SingleOrDefault(x => x.PropertyName == "QuestionSort");
            Filter questionDesc = filters.SingleOrDefault(x => x.PropertyName == "Description");
            Filter hasAnswers = filters.SingleOrDefault(x => x.PropertyName == "HasAnswers");
            Filter agentId = filters.SingleOrDefault(x => x.PropertyName == "AgentId");
            Filter agencyCode = filters.SingleOrDefault(x => x.PropertyName == "AgencyCode");
            Filter agencyName = filters.SingleOrDefault(x => x.PropertyName == "AgencyName");
            Filter activeAgent = filters.SingleOrDefault(x => x.PropertyName == "IsActiveAgent");

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Response_Search";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_ResponseId", SqlDbType.BigInt, 19) { Value = responseId == null ? DBNull.Value : responseId.Value });
                command.Parameters.Add(new SqlParameter("@p_ResponseText", SqlDbType.VarChar, 8000) { Value = responseText == null ? DBNull.Value : responseText.Value });
                command.Parameters.Add(new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Value = answerId == null ? DBNull.Value : answerId.Value });
                command.Parameters.Add(new SqlParameter("@p_AnswerText", SqlDbType.VarChar, 8000) { Value = answerText == null ? DBNull.Value : answerText.Value });
                command.Parameters.Add(new SqlParameter("@p_AnswerSort", SqlDbType.BigInt, 19) { Value = answerSort == null ? DBNull.Value : answerSort.Value });
                command.Parameters.Add(new SqlParameter("@p_QuestionId", SqlDbType.Int, 10) { Value = questionId == null ? DBNull.Value : questionId.Value });
                command.Parameters.Add(new SqlParameter("@p_QuestionType", SqlDbType.TinyInt, 3) { Value = questionType == null ? DBNull.Value : questionType.Value });
                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = periodId == null ? DBNull.Value : periodId.Value });
                command.Parameters.Add(new SqlParameter("@p_PeriodStartDate", SqlDbType.DateTime) { Value = periodStartDate == null ? DBNull.Value : Convert.ToDateTime(periodStartDate.Value).ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_PeriodEndDate", SqlDbType.DateTime) { Value = periodEndDate == null ? DBNull.Value : Convert.ToDateTime(periodEndDate.Value).ToValidRange() });
                command.Parameters.Add(new SqlParameter("@p_PeriodIsOpen", SqlDbType.Bit, 1) { Value = periodIsOpen == null ? DBNull.Value : periodIsOpen.Value });
                command.Parameters.Add(new SqlParameter("@p_QuestionText", SqlDbType.VarChar, 8000) { Value = questionText == null ? DBNull.Value : questionText.Value });
                command.Parameters.Add(new SqlParameter("@p_QuestionSort", SqlDbType.Int, 10) { Value = questionSort == null ? DBNull.Value : questionSort.Value });
                command.Parameters.Add(new SqlParameter("@p_QuestionDesc", SqlDbType.VarChar, 8000) { Value = questionDesc == null ? DBNull.Value : questionDesc.Value });
                command.Parameters.Add(new SqlParameter("@p_HasAnswers", SqlDbType.Bit, 1) { Value = hasAnswers == null ? DBNull.Value : hasAnswers.Value });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = agentId == null ? DBNull.Value : agentId.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyCode", SqlDbType.NVarChar, 10) { Value = agencyCode == null ? DBNull.Value : agencyCode.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyName", SqlDbType.NVarChar, 102) { Value = agencyName == null ? DBNull.Value : agencyName.Value });
                command.Parameters.Add(new SqlParameter("@p_IsActiveAgent", SqlDbType.Bit, 1) { Value = activeAgent == null ? DBNull.Value : activeAgent.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Response), reader, true) as Response;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Responses.
        /// </summary>
        /// <returns>An IEnumerable of all Responses.</returns>
        public override IEnumerable<Response> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Response_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_ResponseId", SqlDbType.BigInt, 19) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Response), reader, true) as Response;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a Response given a Response Id.
        /// </summary>
        /// <param name="id">Id of the Response to be retrieved.</param>
        /// <returns>An instance of a Response.</returns>
        public Response GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Response_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_ResponseId", SqlDbType.BigInt, 19) { Value = id.ToDbNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<Response> responses = new List<Response>();
                    while (reader.Read())
                    {
                        var response = MapEntity(typeof(Response), reader, true) as Response;
                        responses.Add(response);
                    }

                    return responses.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new Response record and returns the Id of the newly created Response.
        /// </summary>
        /// <param name="entity">An instance of a Response to be inserted.</param>
        /// <returns>int ResponseId</returns>
        public int Insert(Response entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Response_Insert";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Value = entity.AnswerId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_Text", SqlDbType.VarChar, 8000) { Value = entity.ResponseText.ToDbNull() });
                IDbDataParameter responseId = new SqlParameter("@p_ResponseId", SqlDbType.BigInt, 19) { Direction = ParameterDirection.Output };
                command.Parameters.Add(responseId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(responseId.Value);
            }
        }

        /// <summary>
        /// Updates a Response given the instance of a Response.
        /// </summary>
        /// <param name="entity">An instance of a Response to be updated.</param>
        /// <returns>True if at least one record was updated.</returns>
        public bool Update(Response entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Response_Update";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_ResponseId", SqlDbType.BigInt, 19) { Value = entity.ResponseId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Value = entity.AnswerId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_Text", SqlDbType.VarChar, 8000) { Value = entity.ResponseText.ToDbNull() });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }
    }
}
