//-----------------------------------------------------------------------
// <copyright file=”Response.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.ComponentModel.DataAnnotations;

    public class Response
    {
        [Key]
        public int ResponseId { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        public int AnswerId { get; set; }

        [StringLength(8000)]
        public string ResponseText { get; set; }

        public Answer Answer { get; set; }

        public Agent Agent { get; set; }
    }
}
