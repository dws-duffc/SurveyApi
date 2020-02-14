//-----------------------------------------------------------------------
// <copyright file=”QuestionsController.cs” company=”Cody Duff”>
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
    public class QuestionsController : Controller
    {
        private readonly IConfiguration config;
        private readonly ILogger<QuestionsController> logger;
        private readonly QuestionManager questionManager;
        private readonly AnswerManager answerManager;

        public QuestionsController(ILogger<QuestionsController> logger, IConfiguration config,
            QuestionManager questionManager, AnswerManager answerManager)
        {
            this.config = config;
            this.logger = logger;
            this.questionManager = questionManager;
            this.answerManager = answerManager;
        }

        // GET: api/questions
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Question> results = questionManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all questions: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/questions/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Question question = questionManager.Get(id);

                return Ok(question);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get question {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/questions/5/answers
        [HttpGet("{id}/[action]")]
        public IActionResult Answers(int id)
        {
            try
            {
                IEnumerable<Answer> answers = answerManager.Find(x => x.QuestionId == id);

                return Ok(answers);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get answers for question {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/questions/search?param=value
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Question question, [FromQuery]Period period,
            QuestionType questionType, [FromQuery]string isOpen, [FromQuery]string hasAnswers)
        {
            try
            {
                IEnumerable<Question> questions = questionManager.Find(x =>
                    x.QuestionTypeId == question.QuestionTypeId &&
                    x.PeriodId == question.PeriodId &&
                    x.Period.StartDate == period.StartDate &&
                    x.Period.EndDate == period.EndDate &&
                    x.QuestionText == question.QuestionText &&
                    x.QuestionSort == question.QuestionSort &&
                    x.QuestionType.Description == questionType.Description);

                if (!string.IsNullOrWhiteSpace(isOpen))
                {
                    bool isOp;
                    bool.TryParse(isOpen, out isOp);

                    questions = questions.Where(x => x.Period.IsOpen == isOp);
                }

                if (!string.IsNullOrWhiteSpace(hasAnswers))
                {
                    bool hasAns;
                    bool.TryParse(hasAnswers, out hasAns);

                    questions = questions.Where(x => x.QuestionType.HasAnswers == hasAns);
                }

                return Ok(questions);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find question(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/questions
        [HttpPost]
        public IActionResult Post([FromBody]Question question)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Question newQuestion = questionManager.Add(question);

                return Created($"questions/{newQuestion.QuestionId}", newQuestion);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert question: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/questions/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Question question)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Question updatedQuestion = questionManager.Update(question);

                return Created($"questions/{id}", updatedQuestion);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update question {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/questions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (questionManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                questionManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete question {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}