//-----------------------------------------------------------------------
// <copyright file=”ResponsesController.cs” company=”Cody Duff”>
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

    [Authorize]
    [Route("api/[controller]")]
    public class ResponsesController : Controller
    {
        readonly IConfiguration config;
        readonly ILogger<ResponsesController> logger;
        readonly ResponseManager responseManager;

        public ResponsesController(ILogger<ResponsesController> logger, IConfiguration config,
            ResponseManager responseManager)
        {
            this.config = config;
            this.logger = logger;
            this.responseManager = responseManager;
        }

        // GET: api/responses
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Response> results = responseManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all responses: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/responses/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Response response = responseManager.Get(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get response {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/responses/search?param=value
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Response response, [FromQuery]Answer answer,
            [FromQuery]Question question, [FromQuery]Period period, QuestionType questionType,
            [FromQuery]Agent agent, [FromQuery]string isActiveAgent, [FromQuery]string isOpen,
            [FromQuery]string hasAnswers)
        {
            try
            {
                IEnumerable<Response> responses = responseManager.Find(x =>
                    x.ResponseId == response.ResponseId &&
                    x.ResponseText == response.ResponseText &&
                    x.Answer.AnswerId == answer.AnswerId &&
                    x.Answer.AnswerText == answer.AnswerText &&
                    x.Answer.AnswerSort == answer.AnswerSort &&
                    x.Answer.Question.QuestionTypeId == question.QuestionTypeId &&
                    x.Answer.Question.PeriodId == question.PeriodId &&
                    x.Answer.Question.Period.StartDate == period.StartDate &&
                    x.Answer.Question.Period.EndDate == period.EndDate &&
                    x.Answer.Question.QuestionText == question.QuestionText &&
                    x.Answer.Question.QuestionSort == question.QuestionSort &&
                    x.Answer.Question.QuestionType.Description == questionType.Description &&
                    x.Agent.AgencyCode == agent.AgencyCode &&
                    x.Agent.AgencyName == agent.AgencyName);

                if (!string.IsNullOrWhiteSpace(isOpen))
                {
                    bool isOp;
                    bool.TryParse(isOpen, out isOp);

                    responses = responses.Where(x => x.Answer.Question.Period.IsOpen == isOp);
                }

                if (!string.IsNullOrWhiteSpace(hasAnswers))
                {
                    bool hasAns;
                    bool.TryParse(hasAnswers, out hasAns);

                    responses = responses.Where(x => x.Answer.Question.QuestionType.HasAnswers == hasAns);
                }

                if (!string.IsNullOrWhiteSpace(isActiveAgent))
                {
                    bool isActive;
                    bool.TryParse(isActiveAgent, out isActive);

                    responses = responses.Where(x => x.Agent.IsActiveAgent == isActive);
                }

                return Ok(responses);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find response(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/responses
        [HttpPost]
        public IActionResult Post([FromBody]Response response)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Response newResponse = responseManager.Add(response);

                return Created($"responses/{newResponse.ResponseId}", newResponse);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert response: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/responses/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Response response)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Response updatedResponse = responseManager.Update(response);

                return Created($"responses/{id}", updatedResponse);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update response {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/responses/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (responseManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                responseManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete response {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}