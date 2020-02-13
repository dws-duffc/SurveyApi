//-----------------------------------------------------------------------
// <copyright file=”QuestionManager.cs” company=”Cody Duff”>
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
    /// Handles all business logic for Questions
    /// </summary>
    public class QuestionManager : IManager<Question>
    {
        readonly SurveyContext context;
        readonly QuestionRepository questionRepo;

        public QuestionManager(SurveyContext context)
        {
            this.context = context;
            questionRepo = new QuestionRepository(this.context);
        }

        public Question Add(Question question)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (questionRepo.Find(x => x.QuestionSort == question.QuestionSort &&
                x.PeriodId == question.PeriodId).Any())
                {
                    throw new ArgumentException("Question in this sort position already exists.", nameof(question.QuestionSort));
                }

                int newQuestionId = 0;
                newQuestionId = questionRepo.Insert(question);
                if (newQuestionId <= 0)
                {
                    throw new FailedOperationException("Failed to insert Question.", question);
                }

                unitOfWork.SaveChanges();
                return questionRepo.GetById(newQuestionId);
            }
        }

        public void Delete(int questionId)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                Question question = questionRepo.GetById(questionId);

                if (!questionRepo.Delete(question))
                {
                    throw new FailedOperationException("Failed to delete Question.", question);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Question> Find(Expression<Func<Question, bool>> predicate)
        {
            return questionRepo.Find(predicate);
        }

        public Question Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "QuestionId must be greater than 0.");
            }

            return questionRepo.GetById(id);
        }

        public IEnumerable<Question> GetAll()
        {
            return questionRepo.GetAll();
        }

        public Question Update(Question question)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (!questionRepo.Update(question))
                {
                    throw new FailedOperationException("Failed to update Question.", question);
                }

                unitOfWork.SaveChanges();
                return questionRepo.GetById(question.QuestionId);
            }
        }
    }
}
