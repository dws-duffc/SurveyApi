//-----------------------------------------------------------------------
// <copyright file=”ContactsController.cs” company=”Cody Duff”>
//     Copyright 2020, Cody Duff, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace cduff.Survey.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Business;
    using Model;

    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        readonly IConfiguration config;
        readonly ILogger<ContactsController> logger;
        readonly ContactManager contactManager;

        public ContactsController(ILogger<ContactsController> logger, IConfiguration config,
            ContactManager contactManager)
        {
            this.config = config;
            this.logger = logger;
            this.contactManager = contactManager;
        }

        // GET: api/contacts
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable <Contact> results = contactManager.GetAll();

                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all contacts: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/contacts/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Contact contact = contactManager.Get(id);

                return Ok(contact);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get contact {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // GET api/contacts/search
        [HttpGet("[action]")]
        public IActionResult Search([FromQuery]Contact contact, [FromQuery]Agent agent,
            [FromQuery]string isPrimary, [FromQuery]string isActiveAgent)
        {
            try
            {
                IEnumerable<Contact> contacts = contactManager.Find(x =>
                    x.FirstName == contact.FirstName &&
                    x.MiddleName == contact.MiddleName &&
                    x.LastName == contact.LastName &&
                    x.PhoneNumber == contact.PhoneNumber &&
                    x.RepNotes == contact.RepNotes &&
                    x.AgentId == agent.AgentId &&
                    x.Agent.AgencyCode == agent.AgencyCode &&
                    x.Agent.AgencyName == agent.AgencyName);

                if (!string.IsNullOrWhiteSpace(isPrimary))
                {
                    bool isPrim;
                    bool.TryParse(isPrimary, out isPrim);

                    contacts = contacts.Where(x => x.IsPrimary == isPrim);
                }

                if (!string.IsNullOrWhiteSpace(isActiveAgent))
                {
                    bool isActive;
                    bool.TryParse(isActiveAgent, out isActive);

                    contacts = contacts.Where(x => x.Agent.IsActiveAgent == isActive);
                }

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to find contact(s): {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // POST api/contacts
        [HttpPost]
        public IActionResult Post([FromBody]Contact contact)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Contact newContact = contactManager.Add(contact);

                return Created($"contacts/{newContact.ContactId}", newContact);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to insert contact: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // PUT api/contacts/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Contact contact)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            try
            {
                Contact updatedContact = contactManager.Update(contact);

                return Created($"contacts/{id}", updatedContact);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update contact {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }
        }

        // DELETE api/contacts/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (contactManager.Get(id) == null)
                {
                    return NotFound(id);
                }

                contactManager.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete contact {id}: {ex}");
                return BadRequest(config["Error:Default"]);
            }

            return NoContent();
        }
    }
}