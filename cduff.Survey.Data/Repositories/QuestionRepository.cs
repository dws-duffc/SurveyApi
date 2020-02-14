//-----------------------------------------------------------------------
// <copyright file=”QuestionRepository.cs” company=”Cody Duff”>
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

    public class QuestionRepository : Repository<Question>, IRepository<Question>
    {
        public QuestionRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes a Question given an instance of the Question to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Question to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Question entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Question_Delete";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_QuestionId", SqlDbType.Int, 10) { Value = entity.QuestionId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Questions that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Question.</returns>
        public override IEnumerable<Question> Find(Expression<Func<Question, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Question>.Decompile(predicate);
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
                command.CommandText = @"dbo.Survey_Question_Search";
                command.CommandTimeout = TimeOut;

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
                        yield return MapEntity(typeof(Question), reader, true) as Question;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Questions.
        /// </summary>
        /// <returns>An IEnumerable of all Questions.</returns>
        public override IEnumerable<Question> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Question_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_QuestionId", SqlDbType.Int, 10) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Question), reader, true) as Question;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a Question given a Question Id.
        /// </summary>
        /// <param name="id">Id of the Question to be retrieved.</param>
        /// <returns>An instance of a Question.</returns>
        public Question GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Question_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_QuestionId", SqlDbType.Int, 10) { Value = id.ToDbNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<Question> questions = new List<Question>();
                    while (reader.Read())
                    {
                        var question = MapEntity(typeof(Question), reader, true) as Question;
                        questions.Add(question);
                    }

                    return questions.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new Question record and returns the Id of the newly created Question.
        /// </summary>
        /// <param name="entity">An instance of a Question to be inserted.</param>
        /// <returns>int QuestionId</returns>
        public int Insert(Question entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Question_Insert";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_QuestionTypeId", SqlDbType.TinyInt, 3) { Value = entity.QuestionTypeId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_Text", SqlDbType.VarChar, 8000) { Value = entity.QuestionText.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_SortOrder", SqlDbType.Int, 1) { Value = entity.QuestionSort.ToDbNull() });
                IDbDataParameter questionId = new SqlParameter("@p_QuestionId", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(questionId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(questionId.Value);
            }
        }

        /// <summary>
        /// Updates a Question given the instance of a Question.
        /// </summary>
        /// <param name="entity">An instance of a Question to be updated.</param>
        /// <returns>True if at least one record was updated.</returns>
        public bool Update(Question entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Question_Update";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_QuestionId", SqlDbType.Int, 10) { Value = entity.QuestionId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_QuestionTypeId", SqlDbType.TinyInt, 3) { Value = entity.QuestionTypeId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_PeriodId", SqlDbType.SmallInt, 5) { Value = entity.PeriodId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_Text", SqlDbType.VarChar, 8000) { Value = entity.QuestionText.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_SortOrder", SqlDbType.Int, 10) { Value = entity.QuestionSort.ToDbNull() });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }
    }
}
