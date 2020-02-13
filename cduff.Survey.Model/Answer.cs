//-----------------------------------------------------------------------
// <copyright file=”Answer.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Answer
    {
        public Answer()
        {
            Responses = new HashSet<Response>();
        }

        [Key]
        public int AnswerId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [StringLength(8000)]
        public string AnswerText { get; set; }

        [Required]
        public int AnswerSort { get; set; }

        public Question Question { get; set; }

        public ICollection<Response> Responses { get; set; }
    }
}
