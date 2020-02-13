//-----------------------------------------------------------------------
// <copyright file=”AnswerManager.cs” company=”Cody Duff”>
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
    /// Handles all business logic for Answers
    /// </summary>
    public class AnswerManager : IManager<Answer>
    {
        readonly SurveyContext context;
        readonly AnswerRepository answerRepo;

        public AnswerManager(SurveyContext context)
        {
            this.context = context;
            answerRepo = new AnswerRepository(this.context);
        }

        public Answer Add(Answer answer)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (answer.QuestionId <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(answer.QuestionId), answer.QuestionId , "QuestionId must be greater than 0.");
                }

                if (answerRepo.Find(x => x.AnswerSort == answer.AnswerSort &&
                x.QuestionId == answer.QuestionId).Any())
                {
                    throw new ArgumentException("Answer in this sort position already exists.", nameof(answer.AnswerSort));
                }

                int newAnswerId = 0;
                newAnswerId = answerRepo.Insert(answer);
                if (newAnswerId <= 0)
                {
                    throw new FailedOperationException("Failed to insert Answer.", answer);
                }

                unitOfWork.SaveChanges();
                return answerRepo.GetById(newAnswerId);
            }
        }

        public void Delete(int answerId)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                Answer answer = answerRepo.GetById(answerId);

                if (!answerRepo.Delete(answer))
                {
                    throw new FailedOperationException("Failed to delete Answer.", answer);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Answer> Find(Expression<Func<Answer, bool>> predicate)
        {
            return answerRepo.Find(predicate);
        }

        public Answer Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "AnswerId must be greater than 0.");
            }

            return answerRepo.GetById(id);
        }

        public IEnumerable<Answer> GetAll()
        {
            return answerRepo.GetAll();
        }

        public Answer Update(Answer answer)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (!answerRepo.Update(answer))
                {
                    throw new FailedOperationException("Failed to update Answer.", answer);
                }

                unitOfWork.SaveChanges();
                return answerRepo.GetById(answer.AnswerId);
            }
        }
    }
}
