using BankBackend.Database.Models;
using EFGetStarted.Database;
using Microsoft.EntityFrameworkCore;

namespace BankBackend.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetActivitiesAsync();
        Task<Activity?> GetActivityByIdAsync(string accountId);

        Task<IEnumerable<Activity>> GetAccountActivities(string id);
        Task<Activity> CreateActivityAsync(CreateActivity activity, string accountID);
        Task<bool> UpdateActivityAsync(string id, Activity activity);
        Task<bool> DeleteActivityAsync(string id);
    }

    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivityService(ApplicationDbContext context, IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Activity>> GetActivitiesAsync()
        {
            return await _context.Activities
                                  .Include(a => a.Account)
                                  .ToListAsync();
        }

        public async Task<Activity?> GetActivityByIdAsync(string id)
        {
            return await _context.Activities.FindAsync(id);
        }

        public async Task<IEnumerable<Activity>> GetAccountActivities(string accountId)
        {
            return await _context.Activities
                .Where(a => a.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<Activity> CreateActivityAsync(CreateActivity activity, string accountID)
        {
            Account account = await _accountService.GetAccountByIdAsync(accountID);
            Activity newActivity = new Activity { Account = account, Name = activity.Name, Created = DateTime.UtcNow, Description = activity.Description };

            _context.Activities.Add(newActivity);
            await _context.SaveChangesAsync();
            return newActivity;
        }

        public async Task<bool> UpdateActivityAsync(string id, Activity activity)
        {
            if (id != activity.Id)
            {
                return false;
            }

            _context.Entry(activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ActivityExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteActivityAsync(string id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return false;
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> ActivityExistsAsync(string id)
        {
            return await _context.Activities.AnyAsync(e => e.Id == id);
        }
    }
}
