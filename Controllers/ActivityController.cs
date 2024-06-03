using BankBackend.Database.Models;
using BankBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BankBackend.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // GET: api/Activity
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
        {
            var activities = await _activityService.GetActivitiesAsync();
            return Ok(activities);
        }

        // GET: api/Activity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(string id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);

            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity);
        }

        // PUT: api/Activity/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity(string id, Activity activity)
        {
            if (id != activity.Id)
            {
                return BadRequest();
            }

            var result = await _activityService.UpdateActivityAsync(id, activity);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Activity
        [HttpPost]
        public async Task<ActionResult<Activity>> PostActivity(CreateActivity activity)
        {
            var createdActivity = await _activityService.CreateActivityAsync(activity, activity.AccountID);
            return CreatedAtAction(nameof(GetActivity), new { id = createdActivity.Id }, createdActivity);
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(string id)
        {
            var result = await _activityService.DeleteActivityAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
