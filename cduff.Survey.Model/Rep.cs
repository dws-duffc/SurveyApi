//-----------------------------------------------------------------------
// <copyright file=”Rep.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Rep
    {
        public Rep()
        {
            Assignments = new HashSet<Assignment>();
            AttemptLogs = new HashSet<AttemptLog>();
        }

        [Key]
        public int RepId { get; set; }

        [Required]
        [StringLength(200)]
        public string Username { get; set; }

        [StringLength(10)]
        public string FirstName { get; set; }

        [StringLength(20)]
        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Assignment> Assignments { get; set; }

        public ICollection<AttemptLog> AttemptLogs { get; set; }
    }
}
