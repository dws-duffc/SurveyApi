//-----------------------------------------------------------------------
// <copyright file=”Agent.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Agent
    {
        public Agent()
        {
            Assignments = new HashSet<Assignment>();
            AttemptLogs = new HashSet<AttemptLog>();
            Contacts = new HashSet<Contact>();
            Responses = new HashSet<Response>();
        }
        // TODO: add validation attributes to all models
        [Key]
        public int AgentId { get; set; }

        [StringLength(8)]
        public string AgencyCode { get; set; }

        [StringLength(100)]
        public string AgencyName { get; set; }

        public bool IsActiveAgent { get; set; }

        public ICollection<Assignment> Assignments { get; set; }

        public ICollection<AttemptLog> AttemptLogs { get; set; }

        public ICollection<Contact> Contacts { get; set; }

        public ICollection<Response> Responses { get; set; }
    }
}
