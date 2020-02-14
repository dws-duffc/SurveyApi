//-----------------------------------------------------------------------
// <copyright file=”RepRepository.cs” company=”Cody Duff”>
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

    public class RepRepository : Repository<Rep>, IRepository<Rep>
    {
        public RepRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes a Rep given an instance of the Rep to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Rep to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Rep entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Rep_Delete";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = entity.RepId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Reps that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Rep.</returns>
        public override IEnumerable<Rep> Find(Expression<Func<Rep, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Rep>.Decompile(predicate);
            Filter username = filters.SingleOrDefault(x => x.PropertyName == "Username");
            Filter firstName = filters.SingleOrDefault(x => x.PropertyName == "FirstName");
            Filter lastName = filters.SingleOrDefault(x => x.PropertyName == "LastName");
            Filter isActive = filters.SingleOrDefault(x => x.PropertyName == "IsActive");

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Rep_Search";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_Username", SqlDbType.VarChar, 202) { Value = username == null ? DBNull.Value : username.Value });
                command.Parameters.Add(new SqlParameter("@p_FirstName", SqlDbType.VarChar, 12) { Value = firstName == null ? DBNull.Value : firstName.Value });
                command.Parameters.Add(new SqlParameter("@p_LastName", SqlDbType.VarChar, 22) { Value = lastName == null ? DBNull.Value : lastName.Value });
                command.Parameters.Add(new SqlParameter("@p_IsActive", SqlDbType.Bit, 1) { Value = isActive == null ? DBNull.Value : isActive.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Rep), reader, true) as Rep;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Reps.
        /// </summary>
        /// <returns>An IEnumerable of all Reps.</returns>
        public override IEnumerable<Rep> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Rep_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Rep), reader, true) as Rep;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a Rep given a Rep Id.
        /// </summary>
        /// <param name="id">Id of the Rep to be retrieved.</param>
        /// <returns>An instance of a Rep.</returns>
        public Rep GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Rep_Get";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = id.ToDbNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<Rep> reps = new List<Rep>();
                    while (reader.Read())
                    {
                        var rep = MapEntity(typeof(Rep), reader, true) as Rep;
                        reps.Add(rep);
                    }

                    return reps.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new Rep record and returns the Id of the newly created Rep.
        /// </summary>
        /// <param name="entity">An instance of a Rep to be inserted.</param>
        /// <returns>int RepId</returns>
        public int Insert(Rep entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Rep_Insert";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_Username", SqlDbType.VarChar, 200) { Value = entity.Username.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_IsActive", SqlDbType.Bit, 1) { Value = entity.IsActive.ToDbNull() });
                IDbDataParameter repId = new SqlParameter("@p_RepId", SqlDbType.BigInt, 19) { Direction = ParameterDirection.Output };
                command.Parameters.Add(repId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(repId.Value);
            }
        }

        /// <summary>
        /// Updates a Rep given the instance of a Rep.
        /// </summary>
        /// <param name="entity">An instance of a Rep to be updated.</param>
        /// <returns>True if at least one record was updated.</returns>
        public bool Update(Rep entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Rep_Update";
                command.CommandTimeout = TimeOut;

                command.Parameters.Add(new SqlParameter("@p_RepId", SqlDbType.Int, 10) { Value = entity.RepId.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_Username", SqlDbType.VarChar, 200) { Value = entity.Username.ToDbNull() });
                command.Parameters.Add(new SqlParameter("@p_IsActive", SqlDbType.Bit, 1) { Value = entity.IsActive.ToDbNull() });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }
    }
}
