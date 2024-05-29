using BankBackend.Database.Models;
using EFGetStarted.Database;
using Microsoft.EntityFrameworkCore;

namespace BankBackend.Services
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetActivitiesAsync();
        Task<Activity?> GetActivityByIdAsync(string id);
        Task<Activity> CreateActivityAsync(Activity activity, string accountID);
        Task<bool> UpdateActivityAsync(string id, Activity activity);
        Task<bool> DeleteActivityAsync(string id);
    }

    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountService _accountService;

        public ActivityService(ApplicationDbContext context, AccountService userService)
        {
            _context = context;
            _accountService = userService;
        }

        public async Task<IEnumerable<Activity>> GetActivitiesAsync()
        {
            return await _context.Activities.ToListAsync();
        }

        public async Task<Activity?> GetActivityByIdAsync(string id)
        {
            return await _context.Activities.FindAsync(id);
        }

        public async Task<Activity> CreateActivityAsync(Activity activity, string accountID)
        {
            activity.Created = new DateTime();
            activity.Account = await _accountService.GetAccountByIdAsync(accountID);
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
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
