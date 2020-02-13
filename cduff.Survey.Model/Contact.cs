//-----------------------------------------------------------------------
// <copyright file=”Contact.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Model
{
    using System.ComponentModel.DataAnnotations;

    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required]
        public int AgentId { get; set; }

        [StringLength(200)]
        public string FirstName { get; set; }

        [StringLength(200)]
        public string MiddleName { get; set; }

        [StringLength(200)]
        public string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string PhoneNumber { get; set; }

        [StringLength(2000)]
        public string RepNotes { get; set; }

        public bool IsPrimary { get; set; }

        public Agent Agent { get; set; }
    }
}
