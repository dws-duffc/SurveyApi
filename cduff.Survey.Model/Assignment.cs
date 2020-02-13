//-----------------------------------------------------------------------
// <copyright file=”Assignment.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Assignment
    {
        public Assignment()
        {
            Agent = new Agent();
            Rep = new Rep();
            Period = new Period();
            AssignmentStatus = new AssignmentStatus();
        }

        [Key]
        [Column(Order = 0)]
        [Range(1, int.MaxValue)]
        public int PeriodId { get; set; }

        [Key]
        [Column(Order = 1)]
        [Range(1, int.MaxValue)]
        public int AgentId { get; set; }

        [Key]
        [Column(Order = 2)]
        [Range(1, int.MaxValue)]
        public int RepId { get; set; }

        [StringLength(200)]
        public string Status { get; set; }

        [StringLength(8000)]
        public string Notes { get; set; }

        [StringLength(200)]
        public string LastAttemptedBy { get; set; }

        public DateTime LastAttemptedDate { get; set; }

        public int AttemptCount { get; set; }

        public Agent Agent { get; set; }

        public Rep Rep { get; set; }

        public Period Period { get; set; }

        public AssignmentStatus AssignmentStatus { get; set; }
    }
}
