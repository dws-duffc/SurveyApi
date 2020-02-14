//-----------------------------------------------------------------------
// <copyright file=”AssignmentManager.cs” company=”Cody Duff”>
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
    /// Handles all business logic for Assignments
    /// </summary>
    public class AssignmentManager
    {
        private readonly SurveyContext context;
        private readonly AssignmentRepository assignmentRepo;

        public AssignmentManager(SurveyContext context)
        {
            this.context = context;
            assignmentRepo = new AssignmentRepository(this.context);
        }

        public Assignment Add(Assignment assignment)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (assignmentRepo.Insert(assignment) <= 0)
                {
                    throw new FailedOperationException("Failed to insert Assignment.", assignment);
                }

                unitOfWork.SaveChanges();
                return assignment;
            }
        }

        public void Delete(Assignment assignment)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (!assignmentRepo.Delete(assignment))
                {
                    throw new FailedOperationException("Failed to insert Assignment.", assignment);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Assignment> Find(Expression<Func<Assignment, bool>> predicate)
        {
            return assignmentRepo.Find(predicate);
        }

        public IEnumerable<Assignment> GetAll()
        {
            return assignmentRepo.GetAll();
        }

        public IEnumerable<Assignment> Get(int? agentId, int? repId, int? periodId)
        {
            return assignmentRepo.Get(agentId, repId, periodId);
        }

        public Assignment Update(Assignment assignment)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (!assignmentRepo.Update(assignment))
                {
                    throw new FailedOperationException("Failed to update Assignment.", assignment);
                }

                unitOfWork.SaveChanges();
                return assignment;
            }
        }

        public Assignment Update(Assignment originalAssignment, Assignment newAssignment)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (originalAssignment.PeriodId.Equals(newAssignment.PeriodId) &&
                originalAssignment.AgentId.Equals(newAssignment.AgentId) &&
                originalAssignment.RepId.Equals(newAssignment.RepId))
                {
                    if (!assignmentRepo.Update(newAssignment))
                    {
                        throw new FailedOperationException("Failed to update Assignment.", newAssignment);
                    }
                }

                if (!assignmentRepo.Delete(originalAssignment))
                {
                    throw new FailedOperationException("Failed to delete Assignment.", originalAssignment);
                }

                if (assignmentRepo.Insert(newAssignment) <= 0)
                {
                    throw new FailedOperationException("Failed to insert Assignment.", newAssignment);
                }

                unitOfWork.SaveChanges();
                return newAssignment;
            }
        }
    }
}
