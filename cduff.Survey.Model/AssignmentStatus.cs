//-----------------------------------------------------------------------
// <copyright file=”AssignmentStatus.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AssignmentStatus
    {
        public AssignmentStatus()
        {
            Assignments = new HashSet<Assignment>();
        }

        [Key]
        [StringLength(200)]
        public string Status { get; set; }

        public int SortOrder { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
    }
}
