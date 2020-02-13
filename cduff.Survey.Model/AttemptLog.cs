//-----------------------------------------------------------------------
// <copyright file=”AttemptLog.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AttemptLog
    {
        [Key]
        public int AttemptLogId { get; set; }

        [Required]
        public int PeriodId { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        public int RepId { get; set; }

        [Required]
        public DateTime AttemptedDate { get; set; }

        [Required]
        [StringLength(200)]
        public string AttemptedBy { get; set; }

        public Rep Rep { get; set; }

        public Agent Agent { get; set; }

        public Period Period { get; set; }
    }
}
