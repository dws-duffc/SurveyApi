//-----------------------------------------------------------------------
// <copyright file=”AttemptLogsController.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Business;
    using Model;

    [Authorize]
    [Route("api/[controller]")]
    public class AttemptLogsController : Controller
    {
        readonly IConfiguration config;
        readonly ILogger<AttemptLogsController> logger;
        readonly AttemptLogManager attemptLogManager;
        readonly AssignmentManager assignmentManager;
        readonly ContactManager contactManager;

        public AttemptLogsController(ILogger<AttemptLogsController> logger, IConfiguration config,
            AttemptLogManager attemptLogManager, AssignmentManager assignmentManager,
            ContactManager contactManager)
        {
            this.config = config;
            this.logger = logger;
            this.attemptLogManager = attemptLogManager;
            this.assignmentManager = assignmentManager;
            this.contactManager = contactManager;
        }

        // GET: api/attemptLogs
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<AttemptLog> results = attemptLogManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all attempt logs: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/attemptLogs/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                AttemptLog attemptLog = attemptLogManager.Get(id);

                return Ok(attemptLog);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get attempt log {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/attemptLogs/5
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]AttemptLog attemptLog)
        {
            try
            {
                // TODO: add other fields. Currently always returns empty logs
                // (check default values passed to search)
                IEnumerable<AttemptLog> attemptLogs = attemptLogManager.Find(x =>
                    x.PeriodId == attemptLog.PeriodId &&
                    x.AgentId == attemptLog.AgentId &&
                    x.RepId == attemptLog.RepId &&
                    x.AttemptedDate == attemptLog.AttemptedDate &&
                    x.AttemptedBy == attemptLog.AttemptedBy);

                return Ok(attemptLogs);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find attempt log(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/attemptLogs
        [HttpPost]
        public IActionResult Post([FromBody]AttemptLog attemptLog)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                AttemptLog newAttemptLog = attemptLogManager.Add(attemptLog);

                return Created($"attemptLogs/{newAttemptLog.AttemptLogId}", newAttemptLog);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert attempt log: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/attemptLogs/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]AttemptLog attemptLog)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                AttemptLog updatedAttemptLog = attemptLogManager.Update(attemptLog);

                return Created($"attemptLogs/{id}", updatedAttemptLog);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update attempt log {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/attemptLogs/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (attemptLogManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                attemptLogManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete attempt log {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}