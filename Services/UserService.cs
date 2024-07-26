using BankBackend.Database.Models;
using EFGetStarted.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankBackend.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User> CreateUserAsync(User user, string password);
        Task<bool> UpdateUserAsync(string id, User user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> AddAccountToUserAsync(string userId, Account account);
        Task<bool> RemoveAccountFromUserAsync(string userId, string accountId);
        Task<User?> GetUserByIdOrCurrentAsync(string? id = null);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<User> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userManager.Users.Include(u => u.Accounts).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userManager.Users.Include(u => u.Accounts).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        public async Task<bool> UpdateUserAsync(string id, User user)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return false;
            }

            existingUser.FavoriteAnimal = user.FavoriteAnimal;

            var result = await _userManager.UpdateAsync(existingUser);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> AddAccountToUserAsync(string userId, Account account)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Accounts ??= new List<Account>();
            user.Accounts.Add(account);

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveAccountFromUserAsync(string userId, string accountId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var account = user.Accounts?.FirstOrDefault(a => a.Id == accountId);
            if (account == null)
            {
                return false;
            }

            user.Accounts.Remove(account);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User?> GetUserByIdOrCurrentAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return await GetCurrentUserAsync();
            }

            return await GetUserByIdAsync(id);
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user != null)
            {
                await _context.Entry(user)
                    .Collection(u => u.Accounts)
                    .LoadAsync();
            }

            return user;
        }
    }
}
