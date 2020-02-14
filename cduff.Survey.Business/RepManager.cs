//-----------------------------------------------------------------------
// <copyright file=”RepManager.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Data;
    using Data.Repositories;
    using Model;

    /// <summary>
    /// Handles all business logic for Reps
    /// </summary>
    public class RepManager : IManager<Rep>
    {
        private readonly SurveyContext context;
        private readonly RepRepository repRepo;

        public RepManager(SurveyContext context)
        {
            this.context = context;
            repRepo = new RepRepository(this.context);
        }

        public Rep Add(Rep rep)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                int newRepId = repRepo.Insert(rep);
                if (newRepId <= 0)
                {
                    throw new FailedOperationException("Failed to insert Rep.", rep);
                }

                unitOfWork.SaveChanges();
                return repRepo.GetById(newRepId);
            }
        }

        public void Delete(int repId)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                Rep rep = repRepo.GetById(repId);

                if (!repRepo.Delete(rep))
                {
                    throw new FailedOperationException("Failed to delete Rep.", rep);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Rep> Find(Expression<Func<Rep, bool>> predicate)
        {
            return repRepo.Find(predicate);
        }

        public Rep Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "RepId must be greater than 0.");
            }

            return repRepo.GetById(id);
        }

        public IEnumerable<Rep> GetAll()
        {
            return repRepo.GetAll();
        }

        public Rep Update(Rep rep)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (!repRepo.Update(rep))
                {
                    throw new FailedOperationException("Failed to update Rep.", rep);
                }

                unitOfWork.SaveChanges();
                return repRepo.GetById(rep.RepId);
            }
        }
    }
}
