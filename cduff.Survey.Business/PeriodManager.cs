//-----------------------------------------------------------------------
// <copyright file=”PeriodManager.cs” company=”Cody Duff”>
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
    /// Handles all business logic for Periods
    /// </summary>
    public class PeriodManager : IManager<Period>
    {
        readonly SurveyContext context;
        readonly AssignmentRepository assignmentRepo;
        readonly PeriodRepository periodRepo;

        public PeriodManager(SurveyContext context)
        {
            this.context = context;
            assignmentRepo = new AssignmentRepository(this.context);
            periodRepo = new PeriodRepository(this.context);
        }

        public Period Add(Period period)
        {
            if (period.StartDate.Date <= DateTime.Now.Date)
            {
                throw new ArgumentOutOfRangeException(
                  nameof(period.StartDate),
                  period.StartDate.Date,
                  "Cannot start a period that is less than or equal to the current date.");
            }

            if (period.EndDate.Date <= period.StartDate.Date)
            {
                throw new ArgumentOutOfRangeException(
                  nameof(period.EndDate),
                  period.StartDate.Date,
                  "Period cannot have an end date that is less than or equal to the start date.");
            }

            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                int newPeriodId = 0;
                newPeriodId = periodRepo.Insert(period);
                if (newPeriodId <= 0)
                {
                    throw new FailedOperationException("Failed to insert Period.", period);
                }

                if (period.IsOpen == true)
                {
                    var oldPeriod = periodRepo
                        .Find(x => x.IsOpen == true)
                        .FirstOrDefault(y => y.PeriodId != newPeriodId);

                    if (oldPeriod != null)
                    {
                        oldPeriod.IsOpen = false;
                        if (!periodRepo.Update(oldPeriod))
                        {
                            throw new FailedOperationException("Failed to update Period.", oldPeriod);
                        }
                    }

                    IEnumerable<Assignment> assignments = assignmentRepo.Get(null, null, oldPeriod.PeriodId);
                    foreach (Assignment assignment in assignments)
                    {
                        assignment.PeriodId = newPeriodId;
                        assignment.AttemptCount = 0;
                        assignment.LastAttemptedBy = null;
                        assignment.LastAttemptedDate = DateTime.MinValue;
                        assignment.Status = "Not Called";
                        assignment.Notes = null;

                        if (assignmentRepo.Insert(assignment) <= 0)
                        {
                            throw new FailedOperationException("Failed to insert Assignment.", assignment);
                        }
                    }
                }

                unitOfWork.SaveChanges();
                return periodRepo.GetById(newPeriodId);
            }
        }

        public void Delete(int periodId)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                IEnumerable<Assignment> assignments = assignmentRepo.Get(null, null, periodId);

                foreach (Assignment assignment in assignments)
                {
                    if (!assignmentRepo.Delete(assignment))
                    {
                        throw new FailedOperationException("Failed to delete Assignment.", assignment);
                    }
                }

                Period period = periodRepo.GetById(periodId);

                if (!periodRepo.Delete(period))
                {
                    throw new FailedOperationException("Failed to delete Period.", period);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Period> Find(Expression<Func<Period, bool>> predicate)
        {
            return periodRepo.Find(predicate);
        }

        public Period Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "PeriodId must be greater than 0.");
            }

            return periodRepo.GetById(id);
        }

        public IEnumerable<Period> GetAll()
        {
            return periodRepo.GetAll();
        }

        public Period Update(Period period)
        {
            if (period.StartDate.Date <= DateTime.Now.Date)
            {
                throw new ArgumentOutOfRangeException(
                  nameof(period.StartDate),
                  period.StartDate.Date,
                  "Cannot start a period that is less than or equal to the current date.");
            }

            if (period.EndDate.Date <= period.StartDate.Date)
            {
                throw new ArgumentOutOfRangeException(
                  nameof(period.EndDate),
                  period.StartDate.Date,
                  "Period cannot have an end date that is less than or equal to the start date.");
            }

            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (!periodRepo.Update(period))
                {
                    throw new FailedOperationException("Failed to update Period.", period);
                }

                if (period.IsOpen == true)
                {
                    var oldPeriod = periodRepo
                        .Find(x => x.IsOpen == true)
                        .FirstOrDefault(y => y.PeriodId != period.PeriodId);

                    if (oldPeriod != null)
                    {
                        oldPeriod.IsOpen = false;
                        if (!periodRepo.Update(oldPeriod))
                        {
                            throw new FailedOperationException("Failed to update Period.", oldPeriod);
                        }
                    }
                }

                unitOfWork.SaveChanges();
                return periodRepo.GetById(period.PeriodId);
            }
        }
    }
}
