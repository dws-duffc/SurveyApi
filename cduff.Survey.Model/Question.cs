//-----------------------------------------------------------------------
// <copyright file=”Question.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int QuestionTypeId { get; set; }

        [Required]
        public int PeriodId { get; set; }

        [StringLength(8000)]
        public string QuestionText { get; set; }

        public int QuestionSort { get; set; }

        public Period Period { get; set; }

        public QuestionType QuestionType { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
