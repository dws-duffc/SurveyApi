//-----------------------------------------------------------------------
// <copyright file=”AgentManager.cs” company=”Cody Duff”>
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
    /// Handles all business logic for Agents
    /// </summary>
    public class AgentManager : IManager<Agent>
    {
        private readonly SurveyContext context;
        private readonly AgentRepository agentRepo;

        public AgentManager(SurveyContext context)
        {
            this.context = context;
            agentRepo = new AgentRepository(this.context);
        }

        public Agent Add(Agent agent)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                int newAgentId = agentRepo.Insert(agent);
                if (newAgentId <= 0)
                {
                    throw new FailedOperationException("Failed to insert Agent.", agent);
                }

                unitOfWork.SaveChanges();
                return agentRepo.GetById(newAgentId);
            }
        }

        public void Delete(int agentId)
        {
            using (IUnitOfWork unitOfWork = context.CreateUnitOfWork())
            {
                Agent agent = agentRepo.GetById(agentId);

                if (!agentRepo.Delete(agent))
                {
                    throw new FailedOperationException("Failed to delete Agent.", agent);
                }

                unitOfWork.SaveChanges();
            }
        }

        public IEnumerable<Agent> Find(Expression<Func<Agent, bool>> predicate)
        {
            return agentRepo.Find(predicate);
        }

        public Agent Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "AgentId must be greater than 0.");
            }

            return agentRepo.GetById(id);
        }

        public IEnumerable<Agent> GetAll()
        {
            return agentRepo.GetAll();
        }

        public Agent Update(Agent agent)
        {
            try
            {
                agentRepo.Update(agent);
            }
            catch (NotSupportedException ex)
            {
                throw new FailedOperationException("Failed to update Agent.", ex);
            }

            return null;
        }
    }
}
