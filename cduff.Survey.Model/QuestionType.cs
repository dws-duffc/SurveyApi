//-----------------------------------------------------------------------
// <copyright file=”QuestionType.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class QuestionType
    {
        public QuestionType()
        {
            Questions = new HashSet<Question>();
        }

        [Key]
        public int QuestionTypeId { get; set; }

        [StringLength(8000)]
        public string Description { get; set; }

        public bool HasAnswers { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
