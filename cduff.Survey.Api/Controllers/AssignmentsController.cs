//-----------------------------------------------------------------------
// <copyright file=”AssignmentsController.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Business;
    using Model;

    [Authorize]
    [Route("api/[controller]")]
    public class AssignmentsController : Controller
    {
        readonly IConfiguration config;
        readonly ILogger<AssignmentsController> logger;
        readonly AssignmentManager assignmentManager;
        readonly ContactManager contactManager;
        readonly PeriodManager periodManager;

        public AssignmentsController(ILogger<AssignmentsController> logger, IConfiguration config,
            AssignmentManager assignmentManager, ContactManager contactManager,
            PeriodManager periodManager)
        {
            this.config = config;
            this.logger = logger;
            this.assignmentManager = assignmentManager;
            this.contactManager = contactManager;
            this.periodManager = periodManager;
        }

        // GET: api/assignments
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Assignment> results;
                ClaimsPrincipal user = HttpContext.User;
                if (user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role).Value == "Admin")
                {
                    results = assignmentManager.GetAll();
                }
                else
                {
                    int repId = Convert.ToInt32(user.Claims.FirstOrDefault(x => x.Type == "RepId").Value);
                    int periodId = periodManager.Find(x => x.IsOpen == true).SingleOrDefault().PeriodId;
                    results = assignmentManager.Get(null, repId, periodId);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all assignments: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/assignments/5/4/3
        [HttpGet("{periodId}/{agentId}/{repId}")]
        public IActionResult Get(int periodId, int agentId, int repId)
        {
            try
            {
                ClaimsPrincipal user = HttpContext.User;
                if (user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role).Value != "Admin" &&
                    Convert.ToInt32(user.Claims.FirstOrDefault(x => x.Type == "RepId").Value) != repId)
                {
                    return Unauthorized();
                }

                IEnumerable<Assignment> assignments = assignmentManager.Get(agentId, repId, periodId);

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get assignment(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/assignments/search
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Assignment assignment, [FromQuery]Period period,
            [FromQuery]Agent agent, [FromQuery]Rep rep, [FromQuery]string isActiveAgent,
            [FromQuery]string isActiveRep)
        {
            try
            {
                IEnumerable<Assignment> assignments = assignmentManager.Find(x =>
                    x.PeriodId == assignment.PeriodId &&
                    x.Period.IsOpen == period.IsOpen &&
                    x.AgentId == assignment.AgentId &&
                    x.Agent.AgencyCode == agent.AgencyCode &&
                    x.Agent.AgencyName == agent.AgencyName &&
                    x.RepId == assignment.RepId &&
                    x.Rep.Username == rep.Username &&
                    x.Rep.FirstName == rep.FirstName &&
                    x.Rep.LastName == rep.LastName &&
                    x.Status == assignment.Status &&
                    x.LastAttemptedBy == assignment.LastAttemptedBy &&
                    x.LastAttemptedDate == assignment.LastAttemptedDate &&
                    x.Notes == assignment.Notes);

                if (!string.IsNullOrWhiteSpace(isActiveRep))
                {
                    bool isActive;
                    bool.TryParse(isActiveRep, out isActive);

                    assignments = assignments.Where(x => x.Rep.IsActive == isActive);
                }

                if (!string.IsNullOrWhiteSpace(isActiveAgent))
                {
                    bool isActive;
                    bool.TryParse(isActiveAgent, out isActive);

                    assignments = assignments.Where(x => x.Agent.IsActiveAgent == isActive);
                }

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find assignment(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/assignments
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody]Assignment assignment)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Assignment newAssignment = assignmentManager.Add(assignment);

                return Created($"assignments/{newAssignment.AgentId}/{newAssignment.RepId}/{newAssignment.PeriodId}",
                    newAssignment);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert assignment: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/assignments/5/4/3
        [Authorize(Roles = "Admin")]
        [HttpPut("{agentId}/{repId}/{periodId}")]
        public IActionResult Put([FromBody]Assignment assignment)
        {
            // TODO: might need to change this a bit. Ids in the query but not used here? Not so sure about that.
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Assignment updatedAssignment = assignmentManager.Update(assignment);

                return Created($"assignments/{updatedAssignment.AgentId}/{updatedAssignment.RepId}/{updatedAssignment.PeriodId}",
                    updatedAssignment);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update assignment: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/assignments/5/4/3
        [Authorize(Roles = "Admin")]
        [HttpDelete("{agentId}/{repId}/{periodId}")]
        public IActionResult Delete(int agentId, int repId, int periodId)
        {
            try
            {
                Assignment assignment = assignmentManager.Get(agentId, repId, periodId).SingleOrDefault();

                if (assignment == null)
                {
                    return NotFound(assignment);
                }

                assignmentManager.Delete(assignment);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete assignment: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}