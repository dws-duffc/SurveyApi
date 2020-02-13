//-----------------------------------------------------------------------
// <copyright file=”PeriodsController.cs” company=”Cody Duff”>
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

    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class PeriodsController : Controller
    {
        readonly IConfiguration config;
        readonly ILogger<PeriodsController> logger;
        readonly PeriodManager periodManager;
        readonly AssignmentManager assignmentManager;
        readonly AttemptLogManager attemptLogManager;
        readonly QuestionManager questionManager;

        public PeriodsController(ILogger<PeriodsController> logger, IConfiguration config,
            PeriodManager periodManager, AssignmentManager assignmentManager,
            AttemptLogManager attemptLogManager, QuestionManager questionManager)
        {
            this.config = config;
            this.logger = logger;
            this.periodManager = periodManager;
            this.assignmentManager = assignmentManager;
            this.attemptLogManager = attemptLogManager;
            this.questionManager = questionManager;
        }

        // GET: api/periods
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Period> results = periodManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all periods: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/periods/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Period period = periodManager.Get(id);

                return Ok(period);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get period {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/periods/5/assignments
        [HttpGet("{id}/[action]")]
        public IActionResult Assignments(int id)
        {
            try
            {
                IEnumerable<Assignment> assignments = assignmentManager.Get(null, null, id);

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get assignments for period {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/periods/5/attemptLogs
        [HttpGet("{id}/[action]")]
        public IActionResult AttemptLogs(int id)
        {
            try
            {
                IEnumerable<AttemptLog> attemptLogs = attemptLogManager.Find(x => x.PeriodId == id);

                return Ok(attemptLogs);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get assignments for period {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/periods/5/questions
        [HttpGet("{id}/[action]")]
        public IActionResult Questions(int id)
        {
            try
            {
                IEnumerable<Question> questions = questionManager.Find(x => x.PeriodId == id);

                return Ok(questions);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get answers for question {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/periods/5
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Period period)
        {
            try
            {
                IEnumerable<Period> periods = periodManager.Find(x =>
                    x.PeriodId == period.PeriodId &&
                    x.IsOpen == period.IsOpen &&
                    x.StartDate == period.StartDate &&
                    x.EndDate == period.EndDate);

                return Ok(periods);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find period(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/periods
        [HttpPost]
        public IActionResult Post([FromBody]Period period)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Period newPeriod = periodManager.Add(period);

                return Created($"periods/{newPeriod.PeriodId}", newPeriod);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert period: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/periods/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Period period)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Period updatedPeriod = periodManager.Update(period);

                return Created($"periods/{id}", updatedPeriod);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update period {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/periods/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (periodManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                periodManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete period {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}
