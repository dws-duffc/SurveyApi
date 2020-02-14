//-----------------------------------------------------------------------
// <copyright file=”AnswerRepository.cs” company=”Cody Duff”>
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

    public class AnswerRepository : Repository<Answer>, IRepository<Answer>
    {
        public AnswerRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes a Answer given an instance of the Answer to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Answer to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Answer entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Answer_Delete";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Value = entity.AnswerId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Answers that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Answer.</returns>
        public override IEnumerable<Answer> Find(Expression<Func<Answer, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Answer>.Decompile(predicate);
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

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Answer_Search";
                command.CommandTimeout = TimeOut;

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

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Answer), reader, true) as Answer;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Answers.
        /// </summary>
        /// <returns>An IEnumerable of all Answers.</returns>
        public override IEnumerable<Answer> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Answer_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Answer), reader, true) as Answer;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a Answer given a Answer Id.
        /// </summary>
        /// <param name="id">Id of the Answer to be retrieved.</param>
        /// <returns>An instance of a Answer.</returns>
        public Answer GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Answer_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Value = id.ToDbNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<Answer> answers = new List<Answer>();
                    while (reader.Read())
                    {
                        var answer = MapEntity(typeof(Answer), reader, true) as Answer;
                        answers.Add(answer);
                    }

                    return answers.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new Answer record and returns the Id of the newly created Answer.
        /// </summary>
        /// <param name="entity">An instance of a Answer to be inserted.</param>
        /// <returns>int AnswerId</returns>
        public int Insert(Answer entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Answer_Insert";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_QuestionId", SqlDbType.Int, 3) { Value = entity.QuestionId });
                command.Parameters.Add(new SqlParameter("@p_Text", SqlDbType.VarChar, 8000) { Value = entity.AnswerText.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_SortOrder", SqlDbType.BigInt, 19) { Value = entity.AnswerSort.ToDbNull() });
                IDbDataParameter answerId = new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Direction = ParameterDirection.Output };
                command.Parameters.Add(answerId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(answerId.Value);
            }
        }

        /// <summary>
        /// Updates a Answer given the instance of a Answer.
        /// </summary>
        /// <param name="entity">An instance of a Answer to be updated.</param>
        /// <returns>True if at least one record was updated.</returns>
        public bool Update(Answer entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Answer_Update";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_AnswerId", SqlDbType.BigInt, 19) { Value = entity.AnswerId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_QuestionId", SqlDbType.Int, 10) { Value = entity.QuestionId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_Text", SqlDbType.VarChar, 8000) { Value = entity.AnswerText.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_SortOrder", SqlDbType.BigInt, 19) { Value = entity.AnswerSort.ToDbNull() });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }
    }
}
