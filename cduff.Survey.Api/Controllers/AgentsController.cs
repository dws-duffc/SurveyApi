//-----------------------------------------------------------------------
// <copyright file=”AgentsController.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Business;
    using Model;

    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AgentsController : Controller
    {
        readonly IConfiguration config;
        readonly ILogger<AgentsController> logger;
        readonly AgentManager agentManager;
        readonly AssignmentManager assignmentManager;
        readonly AttemptLogManager attemptLogManager;
        readonly ContactManager contactManager;
        readonly ResponseManager responseManager;

        public AgentsController(ILogger<AgentsController> logger, IConfiguration config,
            AgentManager agentManager, AssignmentManager assignmentManager,
            AttemptLogManager attemptLogManager, ContactManager contactManager,
            ResponseManager responseManager)
        {
            this.config = config;
            this.logger = logger;
            this.agentManager = agentManager;
            this.assignmentManager = assignmentManager;
            this.attemptLogManager = attemptLogManager;
            this.contactManager = contactManager;
            this.responseManager = responseManager;
        }

        // GET: api/agents
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Agent> results = agentManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all agents: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/agents/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Agent agent = agentManager.Get(id);

                return Ok(agent);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get agent {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/agents/5/assignments
        [HttpGet("{id}/[action]")]
        public IActionResult Assignments(int id)
        {
            try
            {
                IEnumerable<Assignment> assignments = assignmentManager.Get(id, null, null);

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get assignments for agent {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/agents/5/attemptLogs
        [HttpGet("{id}/[action]")]
        public IActionResult AttemptLogs(int id)
        {
            try
            {
                IEnumerable<AttemptLog> logs = attemptLogManager.Find(x => x.AgentId == id);

                return Ok(logs);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get attempt logs for agent {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/agents/5/contacts
        [HttpGet("{id}/[action]")]
        public IActionResult Contacts(int id)
        {
            try
            {
                IEnumerable<Contact> contacts = contactManager.Find(x => x.AgentId == id);

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get contacts for agent {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/agents/5/responses
        [HttpGet("{id}/[action]")]
        public IActionResult Responses(int id)
        {
            try
            {
                IEnumerable<Response> responses = responseManager.Find(x => x.AgentId == id);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get responses for agent {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/agents/search
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Agent agent, [FromQuery]string isActiveAgent)
        {
            try
            {
                IEnumerable<Agent> agents = agentManager.Find(x =>
                    x.AgencyCode == agent.AgencyCode &&
                    x.AgencyName == agent.AgencyName);

                if (!string.IsNullOrWhiteSpace(isActiveAgent))
                {
                    bool isActive;
                    bool.TryParse(isActiveAgent, out isActive);

                    agents = agents.Where(x => x.IsActiveAgent == isActive);
                }

                return Ok(agents);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find agent(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/agents
        [HttpPost]
        public IActionResult Post([FromBody]Agent agent)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Agent newAgent = agentManager.Add(agent);

                return Created($"agents/{newAgent.AgentId}", newAgent);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert agent: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/agents/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Agent agent)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Agent updatedAgent = agentManager.Update(agent);

                return Created($"agents/{id}", updatedAgent);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update agent {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/agents/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (agentManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                agentManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete agent {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}