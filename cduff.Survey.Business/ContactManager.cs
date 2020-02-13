//-----------------------------------------------------------------------
// <copyright file=”ContactManager.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Data;
    using Data.Repositories;
    using Model;

    /// <summary>
    /// Handles all business logic for Contacts
    /// </summary>
    public class ContactManager : IManager<Contact>
    {
        readonly SurveyContext context;
        readonly ContactRepository contactRepo;

        public ContactManager(SurveyContext context)
        {
            this.context = context;
            contactRepo = new ContactRepository(this.context);
        }

        public Contact Add(Contact contact)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (contact.IsPrimary == true)
                {
                    var oldPrimaryContact = contactRepo
                        .Find(x => x.AgentId == contact.AgentId && x.IsPrimary == true)
                        .SingleOrDefault();

                    if (oldPrimaryContact != null)
                    {
                        oldPrimaryContact.IsPrimary = false;
                        if (!contactRepo.Update(oldPrimaryContact))
                        {
                            throw new FailedOperationException("Failed to update Contact.", oldPrimaryContact);
                        }
                    }
                }

                int newContactId = 0;
                newContactId = contactRepo.Insert(contact);
                if (newContactId <= 0)
                {
                    throw new FailedOperationException("Failed to insert Contact.", contact);
                }

                unitOfWork.SaveChanges();
                return contactRepo.GetById(newContactId);
            }
        }

        public void Delete(int contactId)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                Contact contact = contactRepo.GetById(contactId);

                if (!contactRepo.Delete(contact))
                {
                    throw new FailedOperationException("Failed to delete Contact.", contact);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Contact> Find(Expression<Func<Contact, bool>> predicate)
        {
            return contactRepo.Find(predicate);
        }

        public Contact Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "ContactId must be greater than 0.");
            }

            return contactRepo.GetById(id);
        }

        public IEnumerable<Contact> GetAll()
        {
            return contactRepo.GetAll();
        }

        public Contact Update(Contact contact)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (contact.IsPrimary == true)
                {
                    var oldPrimaryContact = contactRepo
                        .Find(x => x.AgentId == contact.AgentId && x.IsPrimary == true)
                        .SingleOrDefault();

                    if (oldPrimaryContact != null)
                    {
                        oldPrimaryContact.IsPrimary = false;
                        if (!contactRepo.Update(oldPrimaryContact))
                        {
                            throw new FailedOperationException("Failed to update Contact.", oldPrimaryContact);
                        }
                    }
                }

                if (!contactRepo.Update(contact))
                {
                    throw new FailedOperationException("Failed to update Contact.", contact);
                }

                unitOfWork.SaveChanges();
                return contactRepo.GetById(contact.ContactId);
            }
        }
    }
}
