using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jihc_event_api.Models;

namespace jihc_event_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventRegistrationsController : ControllerBase
    {
        private readonly JihcEventsContext _context;

        public EventRegistrationsController(JihcEventsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterForEvent([FromBody] EventRegistration registration)
        {
            // Fetch the event
            var eventItem = await _context.Events.FindAsync(registration.EventId);
            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            // Fetch the user
            var user = await _context.Users.FindAsync(registration.UserId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Validate event audience
            if (eventItem.Audience == "Teacher" && user.UserType != "Teacher")
                return BadRequest(new { message = "This event is only for teachers." });

            if (eventItem.Audience == "Student" && user.UserType != "Student")
                return BadRequest(new { message = "This event is only for students." });

            // Validate event capacity
            if (eventItem.Attendees >= eventItem.MaxAttendees)
                return BadRequest(new { message = "Event is full. Registration is closed." });

            // Check if the user is already registered for the event
            var existingRegistration = _context.EventRegistrations
                .FirstOrDefault(r => r.EventId == registration.EventId && r.UserId == registration.UserId);
            if (existingRegistration != null)
                return BadRequest(new { message = "You have already registered for this event." });

            // Register the user
            eventItem.Attendees += 1;
            _context.EventRegistrations.Add(new EventRegistration
            {
                EventId = registration.EventId,
                UserId = registration.UserId
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "You have successfully registered for the event." });
        }



        [HttpGet("{eventId}")]
        public async Task<ActionResult> GetEventRegistrations(int eventId)
        {
            // Fetch the event and include the title
            var eventItem = await _context.Events
                .Where(e => e.Id == eventId)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    Registrations = e.EventRegistrations.Select(r => new
                    {
                        r.UserId,
                        r.User.Name,
                        r.User.Email,
                        r.User.UserType
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (eventItem == null) return NotFound(new { message = "Event not found." });

            // Return the formatted response
            return Ok(new
            {
                eventId = eventItem.Id,
                title = eventItem.Title,
                registrations = eventItem.Registrations
            });
        }

    }
}
