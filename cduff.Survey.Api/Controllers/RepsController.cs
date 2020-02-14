//-----------------------------------------------------------------------
// <copyright file=”RepsController.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Business;
    using Model;

    [Route("api/[controller]")]
    public class RepsController : Controller
    {
        private readonly IConfiguration config;
        private readonly ILogger<RepsController> logger;
        private readonly RepManager repManager;
        private readonly AssignmentManager assignmentManager;
        private readonly AttemptLogManager attemptLogManager;
        private readonly PeriodManager periodManager;

        public RepsController(ILogger<RepsController> logger, IConfiguration config,
            RepManager repManager, AssignmentManager assignmentManager,
            PeriodManager periodManager, AttemptLogManager attemptLogManager)
        {
            this.config = config;
            this.logger = logger;
            this.repManager = repManager;
            this.assignmentManager = assignmentManager;
            this.attemptLogManager = attemptLogManager;
            this.periodManager = periodManager;
        }

        // GET: api/reps
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Rep> results = repManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all reps: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/reps/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Rep rep = repManager.Get(id);

                return Ok(rep);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get rep {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/reps/5/assignments
        [HttpGet("{id}/[action]")]
        public IActionResult Assignments(int id)
        {
            try
            {
                Period openPeriod = periodManager.Find(x => x.IsOpen).SingleOrDefault();
                if (openPeriod == null)
                {
                    return BadRequest(config["Error:Default"]);
                }

                IEnumerable<Assignment> assignments = assignmentManager.Get(null, id, openPeriod.PeriodId);

                return Ok(assignments);

            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get assignments for rep {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/reps/5/attemptLogs
        [HttpGet("{id}/[action]")]
        public IActionResult AttemptLogs(int id)
        {
            try
            {
                IEnumerable<AttemptLog> logs = attemptLogManager.Find(x => x.RepId == id);

                return Ok(logs);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get attempt logs for rep {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/reps/search?param=value
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Rep rep, [FromQuery]string isActive)
        {
            try
            {
                IEnumerable<Rep> reps = repManager.Find(x =>
                    x.Username == rep.Username &&
                    x.FirstName == rep.FirstName &&
                    x.LastName == rep.LastName);

                if (!string.IsNullOrWhiteSpace(isActive))
                {
                    bool isAct;
                    bool.TryParse(isActive, out isAct);

                    reps = reps.Where(x => x.IsActive == isAct);
                }

                return Ok(reps);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find rep(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/reps
        [HttpPost]
        public IActionResult Post([FromBody]Rep rep)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Rep newRep = repManager.Add(rep);

                return Created($"reps/{newRep.RepId}", newRep);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert rep: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/reps/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Rep rep)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Rep updatedRep = repManager.Update(rep);

                return Created($"reps/{id}", updatedRep);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update rep {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/reps/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (repManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                repManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete rep {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}