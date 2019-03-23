using System;
using System.Threading.Tasks;
using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CollAction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventTrackingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserEventTrackingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> IngestEvent([FromBody] JObject eventData)
        {
            var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            if (consentFeature.CanTrack)
            {
                var userEvent = new UserEvent()
                {
                    Timestamp = DateTime.UtcNow,
                    EventData = eventData.ToString()
                };
                _context.UserEvents.Add(userEvent);
                await _context.SaveChangesAsync();
                return Ok(userEvent.Id);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}