using BankBackend.Database.Models;
using EFGetStarted.Database;
using Microsoft.EntityFrameworkCore;

namespace BankBackend.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task<Account?> GetAccountByIdAsync(string id);
        Task<Account> CreateAccountAsync(string userId, Account account);
        Task<bool> UpdateAccountAsync(string id, Account account);
        Task<bool> DeleteAccountAsync(string id);
    }
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(string id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<Account> CreateAccountAsync(string userId, Account account)
        {

            account.Id = null;
            // Find the user by their ID
            var user = await _context.Users.Include(u => u.Accounts).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Add the account to the user's list of accounts
            if (user.Accounts == null)
            {
                user.Accounts = new List<Account>();
            }
            user.Accounts.Add(account);

            // Add the account to the Accounts DbSet
            _context.Accounts.Add(account);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<bool> UpdateAccountAsync(string id, Account account)
        {
            if (id != account.Id)
            {
                return false;
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AccountExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return false;
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> AccountExistsAsync(string id)
        {
            return await _context.Accounts.AnyAsync(e => e.Id == id);
        }
    }
}
