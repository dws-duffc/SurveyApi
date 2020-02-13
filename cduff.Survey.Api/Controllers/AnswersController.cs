//-----------------------------------------------------------------------
// <copyright file=”AnswersController.cs” company=”Cody Duff”>
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
    public class AnswersController : Controller
    {
        readonly IConfiguration config;
        readonly ILogger<AnswersController> logger;
        readonly AnswerManager answerManager;
        readonly ResponseManager responseManager;

        public AnswersController(ILogger<AnswersController> logger, IConfiguration config,
            AnswerManager answerManager, ResponseManager responseManager)
        {
            this.config = config;
            this.logger = logger;
            this.answerManager = answerManager;
            this.responseManager = responseManager;
        }

        // GET: api/answers
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Answer> results = answerManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all answers: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/answers/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Answer answer = answerManager.Get(id);

                return Ok(answer);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get answer {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/answers/5/responses
        [HttpGet("{id}/[action]")]
        public IActionResult Responses(int id)
        {
            try
            {
                IEnumerable<Response> responses = responseManager.Find(x => x.AnswerId == id);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get responses for answer {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/answers/search?param=value
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Answer answer, [FromQuery]Question question,
            [FromQuery]Period period, QuestionType questionType, [FromQuery]string isOpen,
            [FromQuery]string hasAnswers)
        {
            try
            {
                IEnumerable<Answer> answers = answerManager.Find(x =>
                    x.AnswerId == answer.AnswerId &&
                    x.AnswerText == answer.AnswerText &&
                    x.AnswerSort == answer.AnswerSort &&
                    x.Question.QuestionTypeId == question.QuestionTypeId &&
                    x.Question.PeriodId == question.PeriodId &&
                    x.Question.Period.StartDate == period.StartDate &&
                    x.Question.Period.EndDate == period.EndDate &&
                    x.Question.QuestionText == question.QuestionText &&
                    x.Question.QuestionSort == question.QuestionSort &&
                    x.Question.QuestionType.Description == questionType.Description);

                if (!string.IsNullOrWhiteSpace(isOpen))
                {
                    bool isOp;
                    bool.TryParse(isOpen, out isOp);

                    answers = answers.Where(x => x.Question.Period.IsOpen == isOp);
                }

                if (!string.IsNullOrWhiteSpace(hasAnswers))
                {
                    bool hasAns;
                    bool.TryParse(hasAnswers, out hasAns);

                    answers = answers.Where(x => x.Question.QuestionType.HasAnswers == hasAns);
                }

                return Ok(answers);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find answer(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/answers
        [HttpPost]
        public IActionResult Post([FromBody]Answer answer)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Answer newAnswer = answerManager.Add(answer);

                return Created($"answers/{newAnswer.AnswerId}", newAnswer);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert answer: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/answers/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Answer answer)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Answer updatedAnswer = answerManager.Update(answer);

                return Created($"answers/{id}", updatedAnswer);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update answer {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/answers/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (answerManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                answerManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete answer {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}