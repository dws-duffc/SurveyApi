//-----------------------------------------------------------------------
// <copyright file=”ContactRepository.cs” company=”Cody Duff”>
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
    using System.Data.SqlClient;
    using Model;
    using Utilities;

    public class ContactRepository : Repository<Contact>, IRepository<Contact>
    {
        public ContactRepository(SurveyContext context) : base(context) { }

        /// <summary>
        /// Deletes a Contact given an instance of the Contact to be deleted.
        /// </summary>
        /// <param name="entity">An instance of the Contact to be deleted.</param>
        /// <returns>True if at least one record was deleted.</returns>
        public bool Delete(Contact entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Contact_Delete";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_ContactId", SqlDbType.BigInt, 19) { Value = entity.ContactId });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }

        /// <summary>
        /// Finds all Contacts that satisfy the provided lambda expression.
        /// </summary>
        /// <param name="predicate">A lambda expression providing search criteria.</param>
        /// <returns>IEnumerable of type Contact.</returns>
        public override IEnumerable<Contact> Find(Expression<Func<Contact, bool>> predicate)
        {
            List<Filter> filters = ExpressionDecompiler<Contact>.Decompile(predicate);
            var firstName = filters.SingleOrDefault(x => x.PropertyName == "FirstName");
            var middleName = filters.SingleOrDefault(x => x.PropertyName == "MiddleName");
            var lastName = filters.SingleOrDefault(x => x.PropertyName == "LastName");
            var phoneNumber = filters.SingleOrDefault(x => x.PropertyName == "PhoneNumber");
            var repNotes = filters.SingleOrDefault(x => x.PropertyName == "RepNotes");
            var isPrimary = filters.SingleOrDefault(x => x.PropertyName == "IsPrimary");
            var agentId = filters.SingleOrDefault(x => x.PropertyName == "AgentId");
            var agentCode = filters.SingleOrDefault(x => x.PropertyName == "AgencyCode");
            var agentName = filters.SingleOrDefault(x => x.PropertyName == "AgencyName");
            var activeAgent = filters.SingleOrDefault(x => x.PropertyName == "IsActiveAgent");

            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Contact_Search";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_FirstName", SqlDbType.NVarChar, 202) { Value = firstName == null ? DBNull.Value : firstName.Value });
                command.Parameters.Add(new SqlParameter("@p_MiddleName", SqlDbType.NVarChar, 202) { Value = middleName == null ? DBNull.Value : middleName.Value });
                command.Parameters.Add(new SqlParameter("@p_LastName", SqlDbType.NVarChar, 202) { Value = lastName == null ? DBNull.Value : lastName.Value });
                command.Parameters.Add(new SqlParameter("@p_PhoneNumber", SqlDbType.NVarChar, 202) { Value = phoneNumber == null ? DBNull.Value : phoneNumber.Value });
                command.Parameters.Add(new SqlParameter("@p_RepNotes", SqlDbType.NVarChar, 2002) { Value = repNotes == null ? DBNull.Value : repNotes.Value });
                command.Parameters.Add(new SqlParameter("@p_IsPrimary", SqlDbType.Bit, 1) { Value = isPrimary == null ? DBNull.Value : isPrimary.Value });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = agentId == null ? DBNull.Value : agentId.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyCode", SqlDbType.VarChar, 10) { Value = agentCode == null ? DBNull.Value : agentCode.Value });
                command.Parameters.Add(new SqlParameter("@p_AgencyName", SqlDbType.VarChar, 102) { Value = agentName == null ? DBNull.Value : agentName.Value });
                command.Parameters.Add(new SqlParameter("@p_IsActiveAgent", SqlDbType.Bit, 1) { Value = activeAgent == null ? DBNull.Value : activeAgent.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Contact), reader, true) as Contact;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all Contacts.
        /// </summary>
        /// <returns>An IEnumerable of all Contacts.</returns>
        public override IEnumerable<Contact> GetAll()
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Contact_Get";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_ContactId", SqlDbType.BigInt, 19) { Value = DBNull.Value });

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return MapEntity(typeof(Contact), reader, true) as Contact;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an instance of a Contact given a Contact Id.
        /// </summary>
        /// <param name="id">Id of the Contact to be retrieved.</param>
        /// <returns>An instance of a Contact.</returns>
        public Contact GetById(int id)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Contact_Get";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_ContactId", SqlDbType.BigInt, 19) { Value = id.ToDBNull() });

                using (IDataReader reader = command.ExecuteReader())
                {
                    IList<Contact> contacts = new List<Contact>();
                    while (reader.Read())
                    {
                        Contact contact = MapEntity(typeof(Contact), reader, true) as Contact;
                        contacts.Add(contact);
                    }

                    return contacts.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Inserts a new Contact record and returns the Id of the newly created Contact.
        /// </summary>
        /// <param name="entity">An instance of a Contact to be inserted.</param>
        /// <returns>int ContactId</returns>
        public int Insert(Contact entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Contact_Insert";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId });
                command.Parameters.Add(new SqlParameter("@p_FirstName", SqlDbType.NVarChar, 200) { Value = entity.FirstName.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_MiddleName", SqlDbType.NVarChar, 200) { Value = entity.MiddleName.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_LastName", SqlDbType.NVarChar, 200) { Value = entity.LastName.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_PhoneNumber", SqlDbType.NVarChar, 200) { Value = entity.PhoneNumber.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_RepNotes", SqlDbType.NVarChar, 2000) { Value = entity.RepNotes.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_IsPrimary", SqlDbType.Bit, 1) { Value = entity.IsPrimary.ToDBNull() });
                IDbDataParameter contactId = new SqlParameter("@p_ContactId", SqlDbType.BigInt, 19) { Direction = ParameterDirection.Output };
                command.Parameters.Add(contactId);

                command.ExecuteNonQuery();

                return Convert.ToInt32(contactId.Value);
            }
        }

        /// <summary>
        /// Updates a Contact given the instance of a Contact.
        /// </summary>
        /// <param name="entity">An instance of a Contact to be updated.</param>
        /// <returns>True if at least one record was updated.</returns>
        public bool Update(Contact entity)
        {
            using (IDbCommand command = Context.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"dbo.Survey_Contact_Update";
                command.CommandTimeout = timeOut;

                command.Parameters.Add(new SqlParameter("@p_ContactId", SqlDbType.BigInt, 19) { Value = entity.ContactId });
                command.Parameters.Add(new SqlParameter("@p_AgentId", SqlDbType.Int, 10) { Value = entity.AgentId });
                command.Parameters.Add(new SqlParameter("@p_FirstName", SqlDbType.NVarChar, 200) { Value = entity.FirstName.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_MiddleName", SqlDbType.NVarChar, 200) { Value = entity.MiddleName.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_LastName", SqlDbType.NVarChar, 200) { Value = entity.LastName.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_PhoneNumber", SqlDbType.NVarChar, 200) { Value = entity.PhoneNumber.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_RepNotes", SqlDbType.NVarChar, 2000) { Value = entity.RepNotes.ToDBNull() });
                command.Parameters.Add(new SqlParameter("@p_IsPrimary", SqlDbType.Bit, 1) { Value = entity.IsPrimary.ToDBNull() });
                IDbDataParameter rowCount = new SqlParameter("@p_RowCount", SqlDbType.Int, 10) { Direction = ParameterDirection.Output };
                command.Parameters.Add(rowCount);

                command.ExecuteNonQuery();

                return Convert.ToInt32(rowCount.Value) >= 1;
            }
        }
    }
}
