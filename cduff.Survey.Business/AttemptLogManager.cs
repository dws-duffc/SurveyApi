//-----------------------------------------------------------------------
// <copyright file=”AttemptLogManager.cs” company=”Cody Duff”>
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
    /// Handles all business logic for AttemptLogs
    /// </summary>
    public class AttemptLogManager : IManager<AttemptLog>
    {
        readonly SurveyContext context;
        readonly AttemptLogRepository attemptLogRepo;

        public AttemptLogManager(SurveyContext context)
        {
            this.context = context;
            attemptLogRepo = new AttemptLogRepository(this.context);
        }

        public AttemptLog Add(AttemptLog attemptLog)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                int newAttemptLogId = 0;
                newAttemptLogId = attemptLogRepo.Insert(attemptLog);
                if (newAttemptLogId <= 0)
                {
                    throw new FailedOperationException("Failed to insert AttemptLog.", attemptLog);
                }

                unitOfWork.SaveChanges();
                return attemptLogRepo.GetById(newAttemptLogId);
            }
        }

        public void Delete(int attemptLogId)
        {
            try
            {
                AttemptLog attemptLog = attemptLogRepo.GetById(attemptLogId);
                attemptLogRepo.Delete(attemptLog);
            }
            catch (NotSupportedException ex)
            {
                throw new FailedOperationException("Failed to delete AttemptLog.", ex);
            }
        }

        public IEnumerable<AttemptLog> Find(Expression<Func<AttemptLog, bool>> predicate)
        {
            return attemptLogRepo.Find(predicate);
        }

        public AttemptLog Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "AttemptLogId must be greater than 0.");
            }

            return attemptLogRepo.GetById(id);
        }

        public IEnumerable<AttemptLog> GetAll()
        {
            return attemptLogRepo.GetAll();
        }

        public AttemptLog Update(AttemptLog attemptLog)
        {
            try
            {
                attemptLogRepo.Update(attemptLog);
            }
            catch (NotSupportedException ex)
            {
                throw new FailedOperationException("Failed to update AttemptLog.", ex);
            }

            return null;
        }
    }
}
