//-----------------------------------------------------------------------
// <copyright file=”ResponseManager.cs” company=”Cody Duff”>
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
    /// Handles all business logic for Responses
    /// </summary>
    public class ResponseManager : IManager<Response>
    {
        private readonly SurveyContext context;
        private readonly ResponseRepository responseRepo;

        public ResponseManager(SurveyContext context)
        {
            this.context = context;
            responseRepo = new ResponseRepository(this.context);
        }

        public Response Add(Response response)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (response.ResponseId <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(response.ResponseId), response.ResponseId, "QuestionId must be greater than 0.");
                }

                int newResponseId = responseRepo.Insert(response);
                if (newResponseId <= 0)
                {
                    throw new FailedOperationException("Failed to insert Response.", response);
                }

                unitOfWork.SaveChanges();
                return responseRepo.GetById(newResponseId);
            }
        }

        public void Delete(int responseId)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                Response response = responseRepo.GetById(responseId);

                if (!responseRepo.Delete(response))
                {
                    throw new FailedOperationException("Failed to delete Response.", response);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Response> Find(Expression<Func<Response, bool>> predicate)
        {
            return responseRepo.Find(predicate);
        }

        public Response Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "ResponseId must be greater than 0.");
            }

            return responseRepo.GetById(id);
        }

        public IEnumerable<Response> GetAll()
        {
            return responseRepo.GetAll();
        }

        public Response Update(Response response)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                if (!responseRepo.Update(response))
                {
                    throw new FailedOperationException("Failed to update Response.", response);
                }

                unitOfWork.SaveChanges();
                return responseRepo.GetById(response.ResponseId);
            }
        }
    }
}
