using BankBackend.Database.Models;
using BankBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BankBackend.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // GET: api/Activity
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetAllActivities()
        {
            var activities = await _activityService.GetActivitiesAsync();
            return Ok(activities);
        }

        // GET: api/Activities/accountId
        [HttpGet("all/{accountId}")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetAllAccountActivities(string accountId)
        {
            var activities = await _activityService.GetAccountActivitiesAsync(accountId);
            return Ok(activities);
        }

        // GET: api/Activity/5
        [HttpGet("{activityId}", Name = "GetActivity")]
        public async Task<ActionResult<Activity>> GetActivity(string activityId)
        {
            var activity = await _activityService.GetActivityByIdAsync(activityId);

            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity);
        }

        // PUT: api/Activity/5
        [HttpPut("{activityId}")]
        public async Task<IActionResult> PutActivity(string activityId, Activity activity)
        {
            if (activityId != activity.Id)
            {
                return BadRequest();
            }

            var result = await _activityService.UpdateActivityAsync(activityId, activity);
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
            return CreatedAtRoute("GetActivity", new { activityId = createdActivity.Id }, createdActivity);
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
