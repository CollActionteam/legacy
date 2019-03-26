using System;
using System.Threading.Tasks;
using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CollAction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventTrackingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserEventTrackingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> IngestEvent([FromBody] JObject eventData)
        {
            var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            if (consentFeature.CanTrack)
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                var userEvent = new UserEvent()
                {
                    EventLoggedAt = DateTime.UtcNow,
                    EventData = eventData.ToString(),
                    UserId = user?.Id
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