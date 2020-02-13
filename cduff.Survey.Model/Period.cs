//-----------------------------------------------------------------------
// <copyright file=”Period.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Period
    {
        public Period()
        {
            Assignments = new HashSet<Assignment>();
            AttemptLogs = new HashSet<AttemptLog>();
            Questions = new HashSet<Question>();
        }

        [Key]
        public int PeriodId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsOpen { get; set; }

        public ICollection<Assignment> Assignments { get; set; }

        public ICollection<AttemptLog> AttemptLogs { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
